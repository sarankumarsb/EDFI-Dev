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
    public class When_exporting_all_student_metrics_for_a_staffs_sections : When_invoking_Get_for_exporting_all_student_and_metric_data
    {
        //The Injected Dependencies.
        private IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        private IRepository<TeacherSection> teacherSectionRepository;

        //The Actual Model.
        //Is in the Base.

        //The supplied Data models.
        private const int suppliedTeacherSectionId = 3;
        private IQueryable<TeacherStudentSection> suppliedTeacherStudentSectionData;
        private IQueryable<TeacherSection> suppliedTeacherSectionData;
        

        protected override void EstablishContext()
        {
            base.EstablishContext();

            //Prepare supplied data collections
            suppliedTeacherSectionData = GetSuppliedTeacherSection();
            suppliedTeacherStudentSectionData = GetSuppliedTeacherStudentSection();

            //Set up the mocks
            teacherSectionRepository = mocks.StrictMock<IRepository<TeacherSection>>();
            teacherStudentSectionRepository = mocks.StrictMock<IRepository<TeacherStudentSection>>();
            

            //Set expectations
            Expect.Call(teacherSectionRepository.GetAll()).Return(suppliedTeacherSectionData);
            Expect.Call(teacherStudentSectionRepository.GetAll()).Return(suppliedTeacherStudentSectionData);
        }

        #region Prepare Supplied Data
        
        protected IQueryable<TeacherSection> GetSuppliedTeacherSection()
        {
            return (new List<TeacherSection>
                        {
                            new TeacherSection {StaffUSI = suppliedStaffUSI, TeacherSectionId = suppliedTeacherSectionId, SchoolId = suppliedSchoolId},
                            //These below will be filtered out...
                            new TeacherSection {StaffUSI = suppliedStaffUSI, TeacherSectionId = suppliedTeacherSectionId, SchoolId = 9999},
                            new TeacherSection {StaffUSI = suppliedStaffUSI, TeacherSectionId = 9999, SchoolId = suppliedSchoolId},
                            new TeacherSection {StaffUSI = 9999, TeacherSectionId = suppliedTeacherSectionId, SchoolId = suppliedSchoolId},
                            new TeacherSection {StaffUSI = 9999, TeacherSectionId = 9999, SchoolId = suppliedSchoolId},
                            new TeacherSection {StaffUSI = 9999, TeacherSectionId = 9999, SchoolId = 9999},
                            new TeacherSection {StaffUSI = suppliedStaffUSI, TeacherSectionId = 9999, SchoolId = 9999},

                        }).AsQueryable();
        }

        protected IQueryable<TeacherStudentSection> GetSuppliedTeacherStudentSection()
        {
            return (new List<TeacherStudentSection>
                        {
                            new TeacherStudentSection {TeacherSectionId = suppliedTeacherSectionId, StudentUSI = 1},
                            new TeacherStudentSection {TeacherSectionId = suppliedTeacherSectionId, StudentUSI = 2},
                            new TeacherStudentSection {TeacherSectionId = suppliedTeacherSectionId, StudentUSI = 3},
                            //These below will be filtered out...
                            new TeacherStudentSection {TeacherSectionId = 9999, StudentUSI = 4},
                            new TeacherStudentSection {TeacherSectionId = suppliedTeacherSectionId, StudentUSI = 5},

                        }).AsQueryable();
        }
        
        #endregion

        protected override void ExecuteTest()
        {
            var service = new ExportAllStudentMetricsForStaffSectionService(teacherStudentSectionRepository, teacherSectionRepository, StudentSchoolMetricInstanceSetRepository, metricInstanceRepository, studentInformationRepository, rootMetricNodeResolver);
            actualModel = service.Get(ExportAllStudentMetricsForStaffSectionRequest.Create(suppliedStaffUSI, suppliedSchoolId, suppliedTeacherSectionId));
        }

        #region query projections for  reusable supplied info
        
        /// <summary>
        /// Query that returns Students that should be included in the model.
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<SuppliedStudentProjection> GetSuppliedStudentQuery()
        {
            return (from tss in suppliedTeacherStudentSectionData
                    join ts in suppliedTeacherSectionData on tss.TeacherSectionId equals ts.TeacherSectionId
                    join si in suppliedStudentInformationData on tss.StudentUSI equals si.StudentUSI
                    where ts.SchoolId == suppliedSchoolId 
                    && ts.StaffUSI == suppliedStaffUSI
                    && ts.TeacherSectionId == suppliedTeacherSectionId
                    select new SuppliedStudentProjection
                    {
                        StudentUSI = tss.StudentUSI,
                        Name = Utilities.FormatPersonNameByLastName(si.FirstName, si.MiddleName, si.LastSurname),
                    }).OrderBy(x => x.Name);
        }

        protected override IQueryable<SuppliedStudentAndMetricsProjection> SuppliedStudentAndMetricsQuery()
        {
            var query = (from tss in suppliedTeacherStudentSectionData
                         join ts in suppliedTeacherSectionData on tss.TeacherSectionId equals ts.TeacherSectionId
                         join si in suppliedStudentInformationData on tss.StudentUSI equals si.StudentUSI
                         join se in suppliedStudentSchoolMetricInstanceSetData on new { ts.SchoolId, tss.StudentUSI } equals new { se.SchoolId, se.StudentUSI }
                         join m in suppliedMetricInstanceData on se.MetricInstanceSetKey equals m.MetricInstanceSetKey
                         where ts.SchoolId == suppliedSchoolId 
                         && ts.StaffUSI == suppliedStaffUSI
                         && ts.TeacherSectionId == suppliedTeacherSectionId
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
