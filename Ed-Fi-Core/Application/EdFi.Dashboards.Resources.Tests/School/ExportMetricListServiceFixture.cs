// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Tests.Staff;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    [TestFixture]
    public class When_invoking_Get_for_exporting_all_student_and_metric_data_in_a_school_metric_drilldown : When_invoking_Get_for_exporting_all_student_and_metric_data
    {
        //The Injected Dependencies.
        private IRepository<SchoolMetricStudentList> schoolMetricStudentListRepository;
        protected IMetricNodeResolver metricNodeResolver;

        //The Actual Model.
        //In Base.

        //The supplied Data models.
        private const int suppliedMetricId = 100;
        private const int suppliedMetricVariantId = 1000;
        private IQueryable<SchoolMetricStudentList> suppliedSchoolMetricStudentListData;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedSchoolMetricStudentListData = GetSuppliedSchoolMetricStudentList();

            //Set up the mocks
            schoolMetricStudentListRepository = mocks.StrictMock<IRepository<SchoolMetricStudentList>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            
            //Set expectations
            Expect.Call(metricNodeResolver.ResolveMetricId(suppliedMetricVariantId)).Return(suppliedMetricId);
            Expect.Call(schoolMetricStudentListRepository.GetAll()).Return(suppliedSchoolMetricStudentListData);
        
            base.EstablishContext();
        }

        protected IQueryable<SchoolMetricStudentList> GetSuppliedSchoolMetricStudentList()
        {
            return (new List<SchoolMetricStudentList>
                        {
                            new SchoolMetricStudentList{MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, StudentUSI = 1},//This student will have all available supplied metrics.
                            new SchoolMetricStudentList{MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, StudentUSI = 2},//This student will have only a few
                            new SchoolMetricStudentList{MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, StudentUSI = 3},//This student will have only two
                            new SchoolMetricStudentList{MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, StudentUSI = 4},//This student will have only one
                            new SchoolMetricStudentList{MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, StudentUSI = 5},//This student will not have a matching metricInstance value.
                            //These below should be filtered out in the join.
                            new SchoolMetricStudentList{MetricId = 9999, SchoolId = suppliedSchoolId, StudentUSI = 6},
                            new SchoolMetricStudentList{MetricId = suppliedMetricId, SchoolId = 9999, StudentUSI = 7},
                            new SchoolMetricStudentList{MetricId = 9999, SchoolId = 9999, StudentUSI = 8},
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new ExportMetricListService(rootMetricNodeResolver, studentInformationRepository, schoolMetricStudentListRepository, StudentSchoolMetricInstanceSetRepository, metricInstanceRepository, metricNodeResolver);
            actualModel = service.Get(new ExportMetricListRequest()
                                          {
                                              SchoolId = suppliedSchoolId,
                                              MetricVariantId = suppliedMetricVariantId
                                          });
        }

        #region query projections for  reusable supplied info
        /// <summary>
        /// Query that returns Students that should be included in the model.
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<SuppliedStudentProjection> GetSuppliedStudentQuery()
        {
            var q = (from smsl in suppliedSchoolMetricStudentListData
                    join si in suppliedStudentInformationData on smsl.StudentUSI equals si.StudentUSI
                    join se in suppliedStudentSchoolMetricInstanceSetData on smsl.StudentUSI equals se.StudentUSI
                    join m in suppliedMetricInstanceData on se.MetricInstanceSetKey equals m.MetricInstanceSetKey
                    where smsl.SchoolId == suppliedSchoolId
                    && smsl.MetricId == suppliedMetricId
                    && se.SchoolId == suppliedSchoolId
                    select new SuppliedStudentProjection
                    {
                        StudentUSI = smsl.StudentUSI,
                        Name = Utilities.FormatPersonNameByLastName(si.FirstName, si.MiddleName, si.LastSurname),
                    }).OrderBy(x => x.Name);

            return (from s in q
                    group s by s.StudentUSI
                    into g
                    select new SuppliedStudentProjection
                               {
                                   StudentUSI = g.Key,
                                   Name = g.First().Name
                               });
        }

        protected override IQueryable<SuppliedStudentAndMetricsProjection> SuppliedStudentAndMetricsQuery()
        {
            var query = (from smsl in suppliedSchoolMetricStudentListData
                         join si in suppliedStudentInformationData on smsl.StudentUSI equals si.StudentUSI
                         join se in suppliedStudentSchoolMetricInstanceSetData on smsl.StudentUSI equals se.StudentUSI
                         join m in suppliedMetricInstanceData on se.MetricInstanceSetKey equals m.MetricInstanceSetKey
                         where smsl.SchoolId == suppliedSchoolId
                         && smsl.MetricId == suppliedMetricId
                         && se.SchoolId == suppliedSchoolId
                         select new
                         {
                             smsl.StudentUSI,
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
