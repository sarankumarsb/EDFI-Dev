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
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    public abstract class When_invoking_Get_for_exporting_all_student_and_metric_data : TestFixtureBase
    {
        //The Injected Dependencies.
        protected IRepository<StudentInformation> studentInformationRepository;
        protected IRepository<StudentSchoolMetricInstanceSet> StudentSchoolMetricInstanceSetRepository;
        protected IRepository<MetricInstance> metricInstanceRepository;
        protected IRootMetricNodeResolver rootMetricNodeResolver;

        //The Actual Model.
        protected StudentExportAllModel actualModel;

        //The supplied Data models.
        protected int suppliedSchoolId = 1;
        protected int suppliedStaffUSI = 2;
        protected IQueryable<StudentInformation> suppliedStudentInformationData;
        protected IQueryable<StudentSchoolMetricInstanceSet> suppliedStudentSchoolMetricInstanceSetData;
        protected IQueryable<MetricInstance> suppliedMetricInstanceData;
        protected MetricMetadataTree suppliedMetricHierarchy;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedStudentInformationData = GetSuppliedStudentInformation();
            suppliedStudentSchoolMetricInstanceSetData = GetSuppliedStudentSchoolMetricInstanceSet();
            suppliedMetricInstanceData = GetSuppliedMetricInstance();
            suppliedMetricHierarchy = GetSuppliedMetadataHierarchy();

            //Set up the mocks
            studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
            StudentSchoolMetricInstanceSetRepository = mocks.StrictMock<IRepository<StudentSchoolMetricInstanceSet>>();
            metricInstanceRepository = mocks.StrictMock<IRepository<MetricInstance>>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();

            //Set expectations
            Expect.Call(studentInformationRepository.GetAll()).Return(suppliedStudentInformationData);
            Expect.Call(StudentSchoolMetricInstanceSetRepository.GetAll()).Return(suppliedStudentSchoolMetricInstanceSetData);
            Expect.Call(metricInstanceRepository.GetAll()).Return(suppliedMetricInstanceData);
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId)).Return(suppliedMetricHierarchy.Children.First());
        }

        #region Prepare Supplied Data
        protected IQueryable<StudentInformation> GetSuppliedStudentInformation()
        {
            return (new List<StudentInformation>
                        {
                            new StudentInformation { StudentUSI = 1, FirstName = "CF", MiddleName = "CM", LastSurname = "CL"},
                            new StudentInformation { StudentUSI = 2, FirstName = "BF", MiddleName = "BM", LastSurname = "BL"},
                            new StudentInformation { StudentUSI = 3, FirstName = "AF", MiddleName = "AM", LastSurname = "AL"},
                            new StudentInformation { StudentUSI = 4, FirstName = "DF", MiddleName = "DM", LastSurname = "DL"},
                        }).AsQueryable();
        }

        protected IQueryable<StudentSchoolMetricInstanceSet> GetSuppliedStudentSchoolMetricInstanceSet()
        {
            return (new List<StudentSchoolMetricInstanceSet>
                        {
                            new StudentSchoolMetricInstanceSet{SchoolId = suppliedSchoolId, StudentUSI = 1, MetricInstanceSetKey =new Guid("11111111-1111-1111-1111-111111111111")},
                            new StudentSchoolMetricInstanceSet{SchoolId = suppliedSchoolId, StudentUSI = 2, MetricInstanceSetKey =new Guid("22222222-2222-2222-2222-222222222222")},
                            new StudentSchoolMetricInstanceSet{SchoolId = suppliedSchoolId, StudentUSI = 3, MetricInstanceSetKey =new Guid("33333333-3333-3333-3333-333333333333")},
                            new StudentSchoolMetricInstanceSet{SchoolId = 9999, StudentUSI = 4, MetricInstanceSetKey =new Guid("44444444-4444-4444-4444-444444444444")},//Will be filtered out.
                        }).AsQueryable();
        }

        protected IQueryable<MetricInstance> GetSuppliedMetricInstance()
        {
            return (new List<MetricInstance>
                        {
                            new MetricInstance{MetricInstanceSetKey =new Guid("11111111-1111-1111-1111-111111111111"), Value = ".89", MetricId = 3},
                            new MetricInstance{MetricInstanceSetKey =new Guid("11111111-1111-1111-1111-111111111111"), Value = "string", MetricId = 9},
                            new MetricInstance{MetricInstanceSetKey =new Guid("11111111-1111-1111-1111-111111111111"), Value = "123", MetricId = 5},

                            new MetricInstance{MetricInstanceSetKey =new Guid("22222222-2222-2222-2222-222222222222"), Value = "string", MetricId = 8},
                            new MetricInstance{MetricInstanceSetKey =new Guid("22222222-2222-2222-2222-222222222222"), Value = ".56", MetricId = 7},

                            new MetricInstance{MetricInstanceSetKey =new Guid("33333333-3333-3333-3333-333333333333"), Value = "123", MetricId = 4},

                            new MetricInstance{MetricInstanceSetKey =new Guid("44444444-4444-4444-4444-444444444444"), Value = ".88", MetricId = 3},//Will be filtered out.
                        }).AsQueryable();
        }

        protected MetricMetadataTree GetSuppliedMetadataHierarchy()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
            {
                new MetricMetadataNode(tree)
                {
                    MetricId = 1,
                    MetricNodeId = 10,
                    DisplayName = "Root",
                    MetricType = Dashboards.Metric.Resources.Models.MetricType.ContainerMetric,
                    Children = new List<MetricMetadataNode>
                                          {
                                              new MetricMetadataNode(tree)
                                                  {
                                                        MetricId = 2,
                                                        MetricNodeId = 20,
                                                        DisplayName = "Container 2",
                                                        MetricType = Dashboards.Metric.Resources.Models.MetricType.ContainerMetric,
                                                        Children = new List<MetricMetadataNode>
                                                                       {
                                                                            new MetricMetadataNode(tree)
                                                                            {
                                                                                MetricId = 3,
                                                                                MetricNodeId = 30,
                                                                                DisplayName = "Granular 3",
                                                                                MetricType = Dashboards.Metric.Resources.Models.MetricType.GranularMetric,
                                                                            },
                                                                            new MetricMetadataNode(tree)
                                                                            {
                                                                                MetricId = 4,
                                                                                MetricNodeId = 40,
                                                                                DisplayName = "Granular 4",
                                                                                MetricType = Dashboards.Metric.Resources.Models.MetricType.GranularMetric,
                                                                            },
                                                                            new MetricMetadataNode(tree)
                                                                            {
                                                                                MetricId = 5,
                                                                                MetricNodeId = 50,
                                                                                DisplayName = "Granular 5",
                                                                                MetricType = Dashboards.Metric.Resources.Models.MetricType.GranularMetric,
                                                                            },
                                                                       }

                                                  },
                                              new MetricMetadataNode(tree)
                                                  {
                                                        MetricId = 6,
                                                        MetricNodeId = 60,
                                                        DisplayName = "Container 6",
                                                        MetricType = Dashboards.Metric.Resources.Models.MetricType.ContainerMetric,
                                                        Children = new List<MetricMetadataNode>
                                                                       {
                                                                            new MetricMetadataNode(tree)
                                                                            {
                                                                                MetricId = 7,
                                                                                MetricNodeId = 70,
                                                                                DisplayName = "Granular 7",
                                                                                MetricType = Dashboards.Metric.Resources.Models.MetricType.GranularMetric,
                                                                            },
                                                                       }

                                                  },
                                            new MetricMetadataNode(tree)
                                            {
                                                MetricId = 8,
                                                MetricNodeId = 80,
                                                DisplayName = "Granular 8",
                                                MetricType = Dashboards.Metric.Resources.Models.MetricType.GranularMetric,
                                            },
                                            new MetricMetadataNode(tree)
                                            {
                                                MetricId = 9,
                                                MetricNodeId = 90,
                                                DisplayName = "Granular 9",
                                                MetricType = Dashboards.Metric.Resources.Models.MetricType.GranularMetric,
                                            },

                    },
                }
            };

            foreach (var childMetric in tree.Children)
                setParents(childMetric, null);

            return tree;
        }

        //Method used to tag the parents to the hierarchy
        protected void setParents(MetricMetadataNode metric, MetricMetadataNode parentMetric)
        {
            metric.Parent = parentMetric;

            foreach (var childMetric in metric.Children)
                setParents(childMetric, metric);
        }
        #endregion

        #region query projections for  reusable supplied info

        //Local Class to hold a reusable projection.
        protected class SuppliedStudentProjection
        {
            public long StudentUSI { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// Query that returns Students that should be included in the model.
        /// </summary>
        /// <returns></returns>
        protected abstract IQueryable<SuppliedStudentProjection> GetSuppliedStudentQuery();

        //Local Class to hold a reusable projection.
        protected class SuppliedStudentAndMetricsProjection
        {
            public SuppliedStudentAndMetricsProjection()
            {
                Metrics = new List<Metric>();
            }

            public long StudentUSI { get; set; }
            public string StudentName { get; set; }
            public List<Metric> Metrics { get; set; }

            public class Metric
            {
                public int MetricId { get; set; }
                public string Value { get; set; }
            }
        }

        protected abstract IQueryable<SuppliedStudentAndMetricsProjection> SuppliedStudentAndMetricsQuery();

        #endregion

        [Test]
        public void Should_return_model_with_correct_number_of_rows()
        {
            Assert.That(actualModel.Rows.Count(), Is.EqualTo(GetSuppliedStudentQuery().Count()));
        }

        [Test]
        public void Should_return_model_with_studentUSI_on_each_row()
        {
            var i = 0;
            foreach (var suppliedStudent in GetSuppliedStudentQuery())
            {
                var actualRow = actualModel.Rows.ElementAt(i);
                Assert.That(actualRow.StudentUSI, Is.EqualTo(suppliedStudent.StudentUSI));
                i++;
            }

        }

        [Test]
        public void Should_return_model_with_first_column_entitled_Student_Name()
        {
            var i = 0;
            foreach (var suppliedStudent in GetSuppliedStudentQuery())
            {
                var actualColumnTitle = actualModel.Rows.ElementAt(i).Cells.ElementAt(0).Key;
                Assert.That(actualColumnTitle, Is.EqualTo("Student Name"));
                i++;
            }

        }

        [Test]
        public void Should_return_model_with_first_column_value_containing_the_student_name()
        {
            var i = 0;
            foreach (var suppliedStudent in GetSuppliedStudentQuery())
            {
                var actualStudentName = actualModel.Rows.ElementAt(i).Cells.ElementAt(0).Value;
                Assert.That(actualStudentName, Is.EqualTo(suppliedStudent.Name));
                i++;
            }

        }

        [Test]
        public void Should_return_model_with_column_titles_in_depth_first_order_of_metric_tree()
        {
            var granualrMetricsInTreeOrder = suppliedMetricHierarchy.Descendants.Where(x => x.MetricType == Dashboards.Metric.Resources.Models.MetricType.GranularMetric);

            var i = 0;
            foreach (var suppliedStudent in SuppliedStudentAndMetricsQuery())
            {
                var j = 1;
                foreach (var suppliedGranularMetric in granualrMetricsInTreeOrder)
                {
                    var actualMetricName = actualModel.Rows.ElementAt(i).Cells.ElementAt(j).Key;

                    Assert.That(actualMetricName, Is.EqualTo(Utilities.Metrics.GetMetricName(suppliedGranularMetric)));
                    j++;
                }

                i++;
            }

        }

        [Test]
        public void Should_return_model_with_column_data_in_depth_first_order_of_metric_tree()
        {
            var granualrMetricsInTreeOrder = suppliedMetricHierarchy.Descendants.Where(x => x.MetricType == Dashboards.Metric.Resources.Models.MetricType.GranularMetric);

            var i = 0;
            foreach (var suppliedStudent in SuppliedStudentAndMetricsQuery())
            {
                var j = 1;
                foreach (var suppliedGranularMetric in granualrMetricsInTreeOrder)
                {
                    var suppliedMetric = suppliedStudent.Metrics.SingleOrDefault(x => x.MetricId == suppliedGranularMetric.MetricId);
                    var suppliedMetricValue = (suppliedMetric == null) ? string.Empty : suppliedMetric.Value;

                    var actualMetric = actualModel.Rows.ElementAt(i).Cells.ElementAt(j);
                    var actualMetricValue = actualMetric.Value ?? String.Empty;

                    Assert.That(actualMetricValue, Is.EqualTo(suppliedMetricValue));
                    j++;
                }

                i++;
            }
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
