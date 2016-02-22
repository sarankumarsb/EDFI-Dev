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

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    [TestFixture]
    public class When_invoking_Get_for_exporting_all_student_and_metric_data_in_a_Cohort_for_a_StaffUSI : When_invoking_Get_for_exporting_all_student_and_metric_data
    {
        //The Injected Dependencies.
        private IRepository<StaffCohort> staffCohortRepository;
        private IRepository<StaffStudentCohort> staffStudentCohortRepository;

        //The supplied Data models.
        private int suppliedStaffCohortId = 3;
        private IQueryable<StaffCohort> suppliedStaffCohortData;
        private IQueryable<StaffStudentCohort> suppliedStaffStudentCohortData;

        protected override void EstablishContext()
        {
            //Execute the Base EstablishContext()
            base.EstablishContext();

            //Prepare supplied data collections
            suppliedStaffCohortData = GetSuppliedStaffCohort();
            suppliedStaffStudentCohortData = GetSuppliedStaffStudentCohort();

            //Set up the mocks
            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();

            //Set expectations
            Expect.Call(staffCohortRepository.GetAll()).Return(suppliedStaffCohortData);
            Expect.Call(staffStudentCohortRepository.GetAll()).Return(suppliedStaffStudentCohortData);
        }

        #region Prepare Supplied Data
        protected IQueryable<StaffCohort> GetSuppliedStaffCohort()
        {
            return (new List<StaffCohort>
                        {
                            new StaffCohort {StaffUSI = suppliedStaffUSI, StaffCohortId = suppliedStaffCohortId, EducationOrganizationId = suppliedSchoolId},
                            //These below will be filtered out...
                            new StaffCohort {StaffUSI = suppliedStaffUSI, StaffCohortId = suppliedStaffCohortId, EducationOrganizationId = 9999},
                            new StaffCohort {StaffUSI = suppliedStaffUSI, StaffCohortId = 9999, EducationOrganizationId = suppliedSchoolId},
                            new StaffCohort {StaffUSI = 9999, StaffCohortId = suppliedStaffCohortId, EducationOrganizationId = suppliedSchoolId},
                            new StaffCohort {StaffUSI = 9999, StaffCohortId = 9999, EducationOrganizationId = suppliedSchoolId},
                            new StaffCohort {StaffUSI = 9999, StaffCohortId = 9999, EducationOrganizationId = 9999},
                            new StaffCohort {StaffUSI = suppliedStaffUSI, StaffCohortId = 9999, EducationOrganizationId = 9999},

                        }).AsQueryable();
        }

        protected IQueryable<StaffStudentCohort> GetSuppliedStaffStudentCohort()
        {
            return (new List<StaffStudentCohort>
                        {
                            new StaffStudentCohort {StaffCohortId = suppliedStaffCohortId, StudentUSI = 1},
                            new StaffStudentCohort {StaffCohortId = suppliedStaffCohortId, StudentUSI = 2},
                            new StaffStudentCohort {StaffCohortId = suppliedStaffCohortId, StudentUSI = 3},
                            //These below will be filtered out...
                            new StaffStudentCohort {StaffCohortId = 9999, StudentUSI = 4},
                            new StaffStudentCohort {StaffCohortId = suppliedStaffCohortId, StudentUSI = 5},

                        }).AsQueryable();
        }
        #endregion

        protected override void ExecuteTest()
        {
            var service = new ExportAllStudentMetricsForStaffCohortService(staffCohortRepository, staffStudentCohortRepository, StudentSchoolMetricInstanceSetRepository, metricInstanceRepository, studentInformationRepository, rootMetricNodeResolver);
            actualModel = service.Get(new ExportAllStudentMetricsForStaffCohortRequest()
                                          {
                                              SchoolId = suppliedSchoolId,
                                              StaffUSI = suppliedStaffUSI,
                                              StaffCohortId = suppliedStaffCohortId
                                          });
        }

        #region query projections for  reusable supplied info
        /// <summary>
        /// Query that returns Students that should be included in the model.
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<SuppliedStudentProjection> GetSuppliedStudentQuery()
        {
            return (from ssc in suppliedStaffStudentCohortData
                    join sc in suppliedStaffCohortData on ssc.StaffCohortId equals sc.StaffCohortId
                    join si in suppliedStudentInformationData on ssc.StudentUSI equals si.StudentUSI
                    where sc.EducationOrganizationId == suppliedSchoolId
                    && sc.StaffUSI == suppliedStaffUSI
                    && sc.StaffCohortId == suppliedStaffCohortId
                    select new SuppliedStudentProjection
                    {
                        StudentUSI = ssc.StudentUSI,
                        Name = Utilities.FormatPersonNameByLastName(si.FirstName, si.MiddleName, si.LastSurname),
                    }).OrderBy(x => x.Name);
        }

        protected override IQueryable<SuppliedStudentAndMetricsProjection> SuppliedStudentAndMetricsQuery()
        {
            var query = (from tss in suppliedStaffStudentCohortData
                         join ts in suppliedStaffCohortData on tss.StaffCohortId equals ts.StaffCohortId
                         join si in suppliedStudentInformationData on tss.StudentUSI equals si.StudentUSI
                         join se in suppliedStudentSchoolMetricInstanceSetData on tss.StudentUSI equals se.StudentUSI
                         join m in suppliedMetricInstanceData on se.MetricInstanceSetKey equals m.MetricInstanceSetKey
                         where ts.EducationOrganizationId == suppliedSchoolId
                         && ts.StaffUSI == suppliedStaffUSI
                         && ts.StaffCohortId == suppliedStaffCohortId
                         && se.SchoolId == suppliedSchoolId
                         select new
                         {
                             tss.StudentUSI,
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
                                       }).OrderBy(x => x.StudentName);

            return groupedStudents.AsQueryable();
        }
        #endregion
    }
}
