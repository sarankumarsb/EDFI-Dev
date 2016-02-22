// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using MetricType = EdFi.Dashboards.Metric.Data.Entities.MetricType;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    [TestFixture]
    public class When_exporting_all_students_for_staffs_sections_and_cohorts : When_invoking_Get_for_exporting_all_student_and_metric_data
    {
        //The Injected Dependencies.
        private IRepository<StaffStudentAssociation> staffStudentAssociationRepository;

        //The Actual Model.
        //In Base.

        //The supplied Data models.
        private IQueryable<StaffStudentAssociation> suppliedStaffStudentAssociationData;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedStaffStudentAssociationData = GetSuppliedStaffStudentAssociation();
            suppliedStudentInformationData = GetSuppliedStudentInformation();
            suppliedStudentSchoolMetricInstanceSetData = GetSuppliedStudentSchoolMetricInstanceSet();
            suppliedMetricInstanceData = GetSuppliedMetricInstance();
            suppliedMetricHierarchy = GetSuppliedMetadataHierarchy();

            //Set up the mocks
            staffStudentAssociationRepository = mocks.StrictMock<IRepository<StaffStudentAssociation>>();
            studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
            StudentSchoolMetricInstanceSetRepository = mocks.StrictMock<IRepository<StudentSchoolMetricInstanceSet>>();
            metricInstanceRepository = mocks.StrictMock<IRepository<MetricInstance>>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();

            //Set expectations
            Expect.Call(staffStudentAssociationRepository.GetAll()).Return(suppliedStaffStudentAssociationData);
            Expect.Call(studentInformationRepository.GetAll()).Return(suppliedStudentInformationData);
            Expect.Call(StudentSchoolMetricInstanceSetRepository.GetAll()).Return(suppliedStudentSchoolMetricInstanceSetData);
            Expect.Call(metricInstanceRepository.GetAll()).Return(suppliedMetricInstanceData);
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId)).Return(suppliedMetricHierarchy.Children.First());
        }

        #region Prepare Supplied Data
        protected IQueryable<StaffStudentAssociation> GetSuppliedStaffStudentAssociation()
        {
            return (new List<StaffStudentAssociation>
                        {
                            new StaffStudentAssociation{SchoolId = suppliedSchoolId, StaffUSI = suppliedStaffUSI, StudentUSI = 1},
                            new StaffStudentAssociation{SchoolId = suppliedSchoolId, StaffUSI = suppliedStaffUSI, StudentUSI = 2},
                            new StaffStudentAssociation{SchoolId = suppliedSchoolId, StaffUSI = suppliedStaffUSI, StudentUSI = 3},
                            new StaffStudentAssociation{SchoolId = 9999, StaffUSI = 9999, StudentUSI = 9999}, //Should be filtered out.
                        }).AsQueryable();
        }
        #endregion

        protected override void ExecuteTest()
        {
            var service = new ExportAllStudentsForStaffsSectionsAndCohortsService(staffStudentAssociationRepository, StudentSchoolMetricInstanceSetRepository, metricInstanceRepository, studentInformationRepository, rootMetricNodeResolver);
            actualModel = service.Get(new ExportAllStudentsForStaffsSectionsAndCohortsRequest()
                                          {
                                              SchoolId = suppliedSchoolId,
                                              StaffUSI = suppliedStaffUSI
                                          });
        }

        #region query projections for  reusable supplied info

        /// <summary>
        /// Query that returns Students that should be included in the model.
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<SuppliedStudentProjection> GetSuppliedStudentQuery()
        {
            return (from s in suppliedStaffStudentAssociationData
                   join si in suppliedStudentInformationData on s.StudentUSI equals si.StudentUSI
                   where s.SchoolId == suppliedSchoolId && s.StaffUSI == suppliedStaffUSI
                   select new SuppliedStudentProjection
                              {
                                  StudentUSI = s.StudentUSI,
                                  Name = Utilities.FormatPersonNameByLastName(si.FirstName, si.MiddleName, si.LastSurname),
                              }).OrderBy(x=>x.Name);
        }

        protected override IQueryable<SuppliedStudentAndMetricsProjection> SuppliedStudentAndMetricsQuery()
        {
            var query = (from s in suppliedStaffStudentAssociationData
                         join si in suppliedStudentInformationData on s.StudentUSI equals si.StudentUSI
                         join se in suppliedStudentSchoolMetricInstanceSetData on new { s.SchoolId, s.StudentUSI } equals new { se.SchoolId, se.StudentUSI }
                         join m in suppliedMetricInstanceData on se.MetricInstanceSetKey equals m.MetricInstanceSetKey
                         where s.SchoolId == suppliedSchoolId && s.StaffUSI == suppliedStaffUSI
                         select new
                         {
                             s.StudentUSI,
                             Name = Utilities.FormatPersonNameByLastName(si.FirstName, si.MiddleName, si.LastSurname),
                             m.MetricId,
                             m.Value
                         }
                        ).ToList();

            var groupedStudents = (from s in query
                                  group s by s.StudentUSI
                                      into g
                                      select new SuppliedStudentAndMetricsProjection
                                      {
                                          StudentUSI = g.Key,
                                          StudentName = g.First().Name,
                                          Metrics = g.Select(x => new SuppliedStudentAndMetricsProjection.Metric { MetricId = x.MetricId, Value = x.Value }).ToList()
                                      }).OrderBy(x=>x.StudentName);

            return groupedStudents.AsQueryable();
        }
        #endregion
    }


}
