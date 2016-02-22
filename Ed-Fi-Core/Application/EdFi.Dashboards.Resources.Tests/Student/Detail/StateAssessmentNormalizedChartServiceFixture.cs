using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Models.Charting;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Student.Detail
{
    public class When_loading_normalized_state_assessment_chart : TestFixtureBase
    {
        private const int suppliedStudentUSI = 100;
        private const int suppliedSchoolId = 200;
        private const int suppliedMetricId = 300;
        private const int suppliedMetricVariantId = 30099;
        private const int suppliedRootNodeId = 400;
        private const short suppliedAdministrationYear = 2005;
        private const string suppliedContext4 = "context 4";
        private const string suppliedContext3 = "context 3";
        private const string suppliedContext2 = "context 2";
        private const string suppliedContext1 = "context 1";
        private readonly int? suppliedNonNormalized14 = 1000;
        private readonly int? suppliedNonNormalized13 = 1001;
        private readonly int? suppliedNonNormalized12 = 1002;
        private readonly int? suppliedNonNormalized11 = 1003;
        private readonly int? suppliedNormalized14 = 20;
        private readonly int? suppliedNormalized13 = 59;
        private readonly int? suppliedNormalized12 = 100;
        private readonly int? suppliedNormalized11 = 20;
        private readonly int? suppliedNonNormalized24 = 1004;
        private readonly int? suppliedNonNormalized23 = 1005;
        private readonly int? suppliedNonNormalized21 = 1007;
        private readonly int? suppliedNormalized24 = 59;
        private readonly int? suppliedNormalized23 = 94;
        private readonly int? suppliedNormalized21 = 92;
        private readonly int? suppliedNonNormalized34 = 1008;
        private readonly int? suppliedNonNormalized32 = 1009;
        private readonly int? suppliedNonNormalized31 = 1010;
        private readonly int? suppliedNormalized34 = 60;
        private readonly int? suppliedNormalized32 = 90;
        private readonly int? suppliedNormalized31 = 89;
        private readonly int? suppliedNonNormalized64 = 1012;
        private readonly int? suppliedNonNormalized54 = 1011;
        private readonly int? suppliedNonNormalized44 = 1000;
        private readonly int? suppliedNormalized64 = 61;
        private readonly int? suppliedNormalized54 = 85;
        private readonly int? suppliedNormalized44 = 100;

        private IRepository<StudentMetricStateAssessmentHistorical> repository;
        private IMetricNodeResolver metricNodeResolver;

        private ChartData actualModel;

        protected override void EstablishContext()
        {
            repository = mocks.StrictMock<IRepository<StudentMetricStateAssessmentHistorical>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            Expect.Call(repository.GetAll()).Return(GetStudentMetricStateAssessmentHistorical());

            var metricMetadataTree = GetMetricMetadata();
            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Return(GetMetricMetadata());

            base.EstablishContext();
        }

        private MetricMetadataNode GetRootMetricNode(MetricMetadataTree tree)
        {
            MetricMetadataNode value;
            tree.AllNodesByMetricNodeId.TryGetValue(suppliedRootNodeId, out value);
            return value;
        }

        private MetricMetadataNode GetMetricMetadata()
        {
            var tree = new TestMetricMetadataTree();

            return new MetricMetadataNode(tree)
            {
                MetricId = suppliedMetricId,
                MetricVariantId = suppliedMetricVariantId,
                MetricVariantType = MetricVariantType.CurrentYear,
                RootNodeId = suppliedRootNodeId,
                Children = new List<MetricMetadataNode>
                                                {
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 1,
                                                            MetricVariantId = suppliedMetricId + 10001,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "ELA / Reading",
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 1,
                                                            MetricVariantId = suppliedMetricId + 20001,
                                                            MetricVariantType = MetricVariantType.PriorYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Prior Year ELA / Reading",
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 2,
                                                            MetricVariantId = suppliedMetricId + 10002,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Mathematics",
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 3,
                                                            MetricVariantId = suppliedMetricId + 10003,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Writing",
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 4,
                                                            MetricVariantId = suppliedMetricId + 10004,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Science"
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 5,
                                                            MetricVariantId = suppliedMetricId + 10005,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Social Studies"
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 6,
                                                            MetricVariantId = suppliedMetricId + 10006,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Unknown Metric Title"
                                                        },
                                                }
            };
        }

        private IQueryable<StudentMetricStateAssessmentHistorical> GetStudentMetricStateAssessmentHistorical()
        {
            var data = new List<StudentMetricStateAssessmentHistorical>
                                {
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1, AdministrationYear = suppliedAdministrationYear, Context = "wrong data"},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId + 1, AdministrationYear = suppliedAdministrationYear, Context = "wrong data"},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 11, AdministrationYear = suppliedAdministrationYear, Context = "wrong data"},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 10, AdministrationYear = suppliedAdministrationYear, Context = "wrong data"},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1, AdministrationYear = suppliedAdministrationYear + 4, Context = suppliedContext4, NonNormalized = suppliedNonNormalized14, Normalized = suppliedNormalized14, MetricStateTypeId = 3},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1, AdministrationYear = suppliedAdministrationYear + 3, Context = suppliedContext3, NonNormalized = suppliedNonNormalized13, Normalized = suppliedNormalized13, MetricStateTypeId = 1},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 3, AdministrationYear = suppliedAdministrationYear + 2, Context = suppliedContext2, NonNormalized = suppliedNonNormalized32, Normalized = suppliedNormalized32, MetricStateTypeId = 1},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 3, AdministrationYear = suppliedAdministrationYear + 1, Context = suppliedContext1, NonNormalized = suppliedNonNormalized31, Normalized = suppliedNormalized31, MetricStateTypeId = 1},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 2, AdministrationYear = suppliedAdministrationYear + 4, Context = suppliedContext4, NonNormalized = suppliedNonNormalized24, Normalized = suppliedNormalized24, MetricStateTypeId = 1},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 2, AdministrationYear = suppliedAdministrationYear + 3, Context = suppliedContext3, NonNormalized = suppliedNonNormalized23, Normalized = suppliedNormalized23, MetricStateTypeId = 3},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 2, AdministrationYear = suppliedAdministrationYear + 1, Context = suppliedContext1, NonNormalized = suppliedNonNormalized21, Normalized = suppliedNormalized21, MetricStateTypeId = 3},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1, AdministrationYear = suppliedAdministrationYear + 2, Context = suppliedContext2, NonNormalized = suppliedNonNormalized12, Normalized = suppliedNormalized12, MetricStateTypeId = 3},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1, AdministrationYear = suppliedAdministrationYear + 1, Context = suppliedContext1, NonNormalized = suppliedNonNormalized11, Normalized = suppliedNormalized11, MetricStateTypeId = 3},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 3, AdministrationYear = suppliedAdministrationYear + 4, Context = suppliedContext4, NonNormalized = suppliedNonNormalized34, Normalized = suppliedNormalized34, MetricStateTypeId = 3},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 3, AdministrationYear = suppliedAdministrationYear + 3, Context = suppliedContext3, NonNormalized = null, Normalized = null, MetricStateTypeId = 3},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 6, AdministrationYear = suppliedAdministrationYear + 4, Context = suppliedContext4, NonNormalized = suppliedNonNormalized64, Normalized = suppliedNormalized64, MetricStateTypeId = 1},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 5, AdministrationYear = suppliedAdministrationYear + 4, Context = suppliedContext4, NonNormalized = suppliedNonNormalized54, Normalized = suppliedNormalized54, MetricStateTypeId = 3},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 4, AdministrationYear = suppliedAdministrationYear + 4, Context = suppliedContext4, NonNormalized = suppliedNonNormalized44, Normalized = suppliedNormalized44, MetricStateTypeId = 3},
                               };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new StateAssessmentNormalizedChartService(repository, metricNodeResolver);
            actualModel = service.Get(StateAssessmentNormalizedChartRequest.Create(suppliedStudentUSI, suppliedSchoolId, suppliedMetricVariantId));
        }

        [Test]
        public void Should_load_strip_lines_correctly()
        {
            Assert.That(actualModel.StripLines.Count, Is.EqualTo(2));

            Assert.That(actualModel.StripLines[0].Value, Is.EqualTo(60));
            Assert.That(actualModel.StripLines[0].Tooltip, Is.EqualTo("Met Standard"));
            Assert.That(actualModel.StripLines[0].Label, Is.EqualTo("Met Standard"));
            Assert.That(actualModel.StripLines[0].Color, Is.EqualTo(ChartColors.BlueLimit));

            Assert.That(actualModel.StripLines[1].Value, Is.EqualTo(90));
            Assert.That(actualModel.StripLines[1].Tooltip, Is.EqualTo("Commended"));
            Assert.That(actualModel.StripLines[1].Label, Is.EqualTo("Commended"));
            Assert.That(actualModel.StripLines[1].Color, Is.EqualTo(ChartColors.Green));
        }

        [Test]
        public void Should_load_correct_number_of_series()
        {
            Assert.That(actualModel.SeriesCollection.Count, Is.EqualTo(4));
            Assert.That(actualModel.SeriesCollection[0].Name, Is.EqualTo(suppliedContext1));
			Assert.That(actualModel.SeriesCollection[0].Style.BackgroundColor, Is.EqualTo(ChartColors.Gray));
            Assert.That(actualModel.SeriesCollection[1].Name, Is.EqualTo(suppliedContext2));
			Assert.That(actualModel.SeriesCollection[1].Style.BackgroundColor, Is.EqualTo(ChartColors.Gray));
            Assert.That(actualModel.SeriesCollection[2].Name, Is.EqualTo(suppliedContext3));
			Assert.That(actualModel.SeriesCollection[2].Style.BackgroundColor, Is.EqualTo(ChartColors.Gray));
            Assert.That(actualModel.SeriesCollection[3].Name, Is.EqualTo(suppliedContext4));
            Assert.That(actualModel.SeriesCollection[3].Style.BackgroundColor, Is.EqualTo(ChartColors.Gray));
        }

        [Test]
        public void Should_load_data_correctly()
        {
            Assert.That(actualModel.SeriesCollection[0].Points.Count, Is.EqualTo(3));
            Assert.That(actualModel.SeriesCollection[1].Points.Count, Is.EqualTo(2));
            Assert.That(actualModel.SeriesCollection[2].Points.Count, Is.EqualTo(2));
            Assert.That(actualModel.SeriesCollection[3].Points.Count, Is.EqualTo(6));

            Assert.That(actualModel.SeriesCollection[0].Points[0].Value, Is.EqualTo(suppliedNormalized11));
            Assert.That(actualModel.SeriesCollection[0].Points[1].Value, Is.EqualTo(suppliedNormalized21));
            Assert.That(actualModel.SeriesCollection[0].Points[2].Value, Is.EqualTo(suppliedNormalized31));

            Assert.That(actualModel.SeriesCollection[0].Points[0].Tooltip, Is.EqualTo(suppliedNonNormalized11.ToString()));
            Assert.That(actualModel.SeriesCollection[0].Points[1].Tooltip, Is.EqualTo(suppliedNonNormalized21.ToString()));
            Assert.That(actualModel.SeriesCollection[0].Points[2].Tooltip, Is.EqualTo(suppliedNonNormalized31.ToString()));

            Assert.That(actualModel.SeriesCollection[0].Points[0].Label, Is.EqualTo(suppliedNonNormalized11.ToString()));
            Assert.That(actualModel.SeriesCollection[0].Points[1].Label, Is.EqualTo(suppliedNonNormalized21.ToString()));
            Assert.That(actualModel.SeriesCollection[0].Points[2].Label, Is.EqualTo(suppliedNonNormalized31.ToString()));

            Assert.That(actualModel.SeriesCollection[0].Points[0].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.SeriesCollection[0].Points[1].State, Is.EqualTo(MetricStateType.None));
            Assert.That(actualModel.SeriesCollection[0].Points[2].State, Is.EqualTo(MetricStateType.None));
        }

        [Test]
        public void Should_set_state_of_most_recent_data()
        {
            Assert.That(actualModel.SeriesCollection[3].Points[0].State, Is.EqualTo(MetricStateType.Low));
            Assert.That(actualModel.SeriesCollection[3].Points[1].State, Is.EqualTo(MetricStateType.Low));
            Assert.That(actualModel.SeriesCollection[3].Points[2].State, Is.EqualTo(MetricStateType.Good));
            Assert.That(actualModel.SeriesCollection[3].Points[3].State, Is.EqualTo(MetricStateType.Good));
            Assert.That(actualModel.SeriesCollection[3].Points[4].State, Is.EqualTo(MetricStateType.Good));
            Assert.That(actualModel.SeriesCollection[3].Points[5].State, Is.EqualTo(MetricStateType.Good));
        }

        [Test]
        public void Should_shorten_metric_name()
        {
            var points = actualModel.SeriesCollection[3].Points;
            Assert.That(points[0].AxisLabel, Is.EqualTo("ELA"));
            Assert.That(points[1].AxisLabel, Is.EqualTo("M"));
            Assert.That(points[2].AxisLabel, Is.EqualTo("W"));
            Assert.That(points[3].AxisLabel, Is.EqualTo("Sc"));
            Assert.That(points[4].AxisLabel, Is.EqualTo("SS"));
            Assert.That(points[5].AxisLabel, Is.EqualTo("Unknown Metric Title"));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }

    public class When_loading_normalized_state_assessment_chart_with_no_data : TestFixtureBase
    {
        private const int suppliedStudentUSI = 100;
        private const int suppliedSchoolId = 200;
        private const int suppliedMetricId = 300;
        private const int suppliedMetricVariantId = 30099;
        private const int suppliedRootNodeId = 400;
        private const short suppliedAdministrationYear = 2005;

        private IRepository<StudentMetricStateAssessmentHistorical> repository;
        private IMetricNodeResolver metricNodeResolver;

        private ChartData actualModel;

        protected override void EstablishContext()
        {
            repository = mocks.StrictMock<IRepository<StudentMetricStateAssessmentHistorical>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            Expect.Call(repository.GetAll()).Return(GetStudentMetricStateAssessmentHistorical());

            Expect.Call(metricNodeResolver.GetMetricNodeForStudentFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Return(GetMetricMetadata());

            base.EstablishContext();
        }

        private MetricMetadataNode GetRootMetricNode(MetricMetadataTree tree)
        {
            MetricMetadataNode value;
            tree.AllNodesByMetricNodeId.TryGetValue(suppliedRootNodeId, out value);
            return value;
        }

        private MetricMetadataNode GetMetricMetadata()
        {
            var tree = new TestMetricMetadataTree();

            return new MetricMetadataNode(tree)
            {
                MetricId = suppliedMetricId,
                MetricVariantId = suppliedMetricVariantId,
                MetricVariantType = MetricVariantType.CurrentYear,
                RootNodeId = suppliedRootNodeId,
                Children = new List<MetricMetadataNode>
                                                {
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 1,
                                                            MetricVariantId = suppliedMetricId + 10001,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "ELA / Reading",
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 1,
                                                            MetricVariantId = suppliedMetricId + 20001,
                                                            MetricVariantType = MetricVariantType.PriorYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Prior Year ELA / Reading",
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 2,
                                                            MetricVariantId = suppliedMetricId + 10002,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Mathematics",
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 3,
                                                            MetricVariantId = suppliedMetricId + 10003,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Writing",
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 4,
                                                            MetricVariantId = suppliedMetricId + 10004,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Science"
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 5,
                                                            MetricVariantId = suppliedMetricId + 10005,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Social Studies"
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId = suppliedMetricId + 6,
                                                            MetricVariantId = suppliedMetricId + 10006,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            RootNodeId = suppliedRootNodeId,
                                                            DisplayName = "Unknown Metric Title"
                                                        },
                                                }
            };
        }

        private IQueryable<StudentMetricStateAssessmentHistorical> GetStudentMetricStateAssessmentHistorical()
        {
            var data = new List<StudentMetricStateAssessmentHistorical>
                                {
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 1, AdministrationYear = suppliedAdministrationYear, Context = "wrong data"},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId + 1, MetricId = suppliedMetricId + 1, AdministrationYear = suppliedAdministrationYear, Context = "wrong data"},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 11, AdministrationYear = suppliedAdministrationYear, Context = "wrong data"},
                                    new StudentMetricStateAssessmentHistorical { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId, MetricId = suppliedMetricId + 10, AdministrationYear = suppliedAdministrationYear, Context = "wrong data"},
                                };
            return data.AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new StateAssessmentNormalizedChartService(repository, metricNodeResolver);
            actualModel = service.Get(StateAssessmentNormalizedChartRequest.Create(suppliedStudentUSI, suppliedSchoolId, suppliedMetricVariantId));
        }

        [Test]
        public void Should_return_null_object()
        {
            Assert.That(actualModel, Is.Null);
        }
    }
}
