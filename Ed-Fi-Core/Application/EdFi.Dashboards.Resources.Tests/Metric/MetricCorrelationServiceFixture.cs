// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Metric;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Tests.Metric
{
    [TestFixture]
    public class When_correlating_local_education_agency_metric_to_school_metric : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRootMetricNodeResolver rootMetricNodeResolver;

        //The Actual Model.
        private MetricCorrelationProvider.MetricRenderingContext actualModel;

        //The supplied Data models.
        private const int suppliedLocalEducationAgencyMetricVariantId = 1111;
        private const int suppliedSchoolId = 100;
        private readonly MetricCorrelationProvider.MetricRenderingContext expectedModel = new MetricCorrelationProvider.MetricRenderingContext
                                                                                                {
                                                                                                    ContextMetricVariantId = 1021,
                                                                                                    MetricVariantId = 1211
                                                                                                };

        protected override void EstablishContext()
        {
            //Prepare supplied data collections

            //Set up the mocks
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();

            //Set expectations
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForLocalEducationAgency()).Return(GetLEARootOverviewNode());
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForSchool(suppliedSchoolId)).Return(GetSchoolRootOverviewNode());
        }

        private MetricMetadataNode GetLEARootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    { 
                        MetricId = 1, 
                        MetricVariantId = 1001,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "LEA Overview", 
                        ChildDomainEntityMetricId = 2,
                        Children = new List<MetricMetadataNode>
                        {
                            new MetricMetadataNode(tree) {
                                MetricId=11, 
                                MetricVariantId = 1011,
                                MetricVariantType = MetricVariantType.CurrentYear,
                                ChildDomainEntityMetricId = 21, 
                                Name = "LEA's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                            {
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=111, 
                                                        MetricVariantId = 1111,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        ChildDomainEntityMetricId = 211, 
                                                        Name = "Attendance"
                                                    },
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=112, 
                                                        MetricVariantId = 1112,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        Name = "Discipline"
                                                    } 
                                            }
                            }
                        }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> {root};

            return root.Children.ElementAt(0);
        }

        private MetricMetadataNode GetSchoolRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    {
                        MetricId = 2, 
                        MetricVariantId = 1002,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "School Overview", 
                        MetricNodeId = 7, 
                        Parent = null,
                        Children = new List<MetricMetadataNode>
                                    {
                                        new MetricMetadataNode(tree)
                                        {
                                            MetricId=21, 
                                            MetricVariantId = 1021,
                                            MetricVariantType = MetricVariantType.CurrentYear,
                                            MetricNodeId = 71, 
                                            Name = "School's Attendance and Discipline",
                                            Children = new List<MetricMetadataNode>
                                                        {
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=211, 
                                                                    MetricVariantId = 1211,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    MetricNodeId = 711, 
                                                                    Name = "Attendance"
                                                                },
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=212, 
                                                                    MetricVariantId = 1212,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    Name = "Discipline"
                                                                } 
                                                        }
                                        },
                                        new MetricMetadataNode(tree)
                                            {
                                                MetricId=22, 
                                                MetricVariantId = 1022,
                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                MetricNodeId = 72, 
                                                Name = "School's Other Metric"
                                            },
                                    }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricCorrelationProvider(rootMetricNodeResolver);
            actualModel = service.GetRenderingParentMetricVariantIdForSchool(suppliedLocalEducationAgencyMetricVariantId, suppliedSchoolId);
        }

        [Test]
        public void Should_calculate_the_right_correlation_ids()
        {
            Assert.That(actualModel.ContextMetricVariantId, Is.EqualTo(expectedModel.ContextMetricVariantId));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(expectedModel.MetricVariantId));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

    }
    [TestFixture]
    public class When_correlating_local_education_agency_metric_to_school_metric_for_prior_year : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRootMetricNodeResolver rootMetricNodeResolver;

        //The Actual Model.
        private MetricCorrelationProvider.MetricRenderingContext actualModel;

        //The supplied Data models.
        private const int suppliedLocalEducationAgencyMetricVariantId = 20111;
        private const int suppliedSchoolId = 100;
        private readonly MetricCorrelationProvider.MetricRenderingContext expectedModel = new MetricCorrelationProvider.MetricRenderingContext
        {
            ContextMetricVariantId = 1021,
            MetricVariantId = 20211
        };

        protected override void EstablishContext()
        {
            //Prepare supplied data collections

            //Set up the mocks
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();

            //Set expectations
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForLocalEducationAgency()).Return(GetLEARootOverviewNode());
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForSchool(suppliedSchoolId)).Return(GetSchoolRootOverviewNode());
        }

        private MetricMetadataNode GetLEARootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    { 
                        MetricId = 1, 
                        MetricVariantId = 1001,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "LEA Overview", 
                        ChildDomainEntityMetricId = 2,
                        Children = new List<MetricMetadataNode>
                        {
                            new MetricMetadataNode(tree) {
                                MetricId=11, 
                                MetricVariantId = 1011,
                                MetricVariantType = MetricVariantType.CurrentYear,
                                ChildDomainEntityMetricId = 21, 
                                Name = "LEA's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                            {
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=111, 
                                                        MetricVariantId = 1111,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        ChildDomainEntityMetricId = 211, 
                                                        Name = "Attendance"
                                                    },
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=111, 
                                                        MetricVariantId = 20111,
                                                        MetricVariantType = MetricVariantType.PriorYear,
                                                        ChildDomainEntityMetricId = 211, 
                                                        Name = "Attendance"
                                                    },
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=112, 
                                                        MetricVariantId = 1112,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        Name = "Discipline"
                                                    } 
                                            }
                            }
                        }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        private MetricMetadataNode GetSchoolRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    {
                        MetricId = 2, 
                        MetricVariantId = 1002,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "School Overview", 
                        MetricNodeId = 7, 
                        Parent = null,
                        Children = new List<MetricMetadataNode>
                                    {
                                        new MetricMetadataNode(tree)
                                        {
                                            MetricId=21, 
                                            MetricVariantId = 1021,
                                            MetricVariantType = MetricVariantType.CurrentYear,
                                            MetricNodeId = 71, 
                                            Name = "School's Attendance and Discipline",
                                            Children = new List<MetricMetadataNode>
                                                        {
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=211, 
                                                                    MetricVariantId = 1211,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    MetricNodeId = 711, 
                                                                    Name = "Attendance"
                                                                },
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=211, 
                                                                    MetricVariantId = 20211,
                                                                    MetricVariantType = MetricVariantType.PriorYear,
                                                                    MetricNodeId = 712, 
                                                                    Name = "Attendance"
                                                                },
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=212, 
                                                                    MetricVariantId = 1212,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    Name = "Discipline"
                                                                } 
                                                        }
                                        },
                                        new MetricMetadataNode(tree)
                                            {
                                                MetricId=22, 
                                                MetricVariantId = 1022,
                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                MetricNodeId = 72, 
                                                Name = "School's Other Metric"
                                            },
                                    }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricCorrelationProvider(rootMetricNodeResolver);
            actualModel = service.GetRenderingParentMetricVariantIdForSchool(suppliedLocalEducationAgencyMetricVariantId, suppliedSchoolId);
        }

        [Test]
        public void Should_calculate_the_right_correlation_ids()
        {
            Assert.That(actualModel.ContextMetricVariantId, Is.EqualTo(expectedModel.ContextMetricVariantId));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(expectedModel.MetricVariantId));
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

    }

    [TestFixture]
    public class When_correlating_local_education_agency_metric_to_school_metric_with_no_match : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRootMetricNodeResolver rootMetricNodeResolver;

        //The Actual Model.
        private MetricCorrelationProvider.MetricRenderingContext actualModel;

        //The supplied Data models.
        private const int suppliedLocalEducationAgencyMetricVariantId = 1112;
        private const int suppliedSchoolId = 100;
        private MetricCorrelationProvider.MetricRenderingContext expectedModel = new MetricCorrelationProvider.MetricRenderingContext
                                                                                        {
                                                                                            ContextMetricVariantId = 1021,
                                                                                            MetricVariantId = null
                                                                                        };

        protected override void EstablishContext()
        {
            //Prepare supplied data collections

            //Set up the mocks
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();

            //Set expectations
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForLocalEducationAgency()).Return(GetLEARootOverviewNode());
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForSchool(suppliedSchoolId)).Return(GetSchoolRootOverviewNode());
        }

        private MetricMetadataNode GetLEARootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    { 
                        MetricId = 1, 
                        MetricVariantId = 1001,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "LEA Overview", 
                        ChildDomainEntityMetricId = 2,
                        Children = new List<MetricMetadataNode>
                        {
                            new MetricMetadataNode(tree) {
                                MetricId=11, 
                                MetricVariantId = 1011,
                                MetricVariantType = MetricVariantType.CurrentYear,
                                ChildDomainEntityMetricId = 21, 
                                Name = "LEA's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                            {
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=111, 
                                                        MetricVariantId = 1111,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        ChildDomainEntityMetricId = 211, 
                                                        Name = "Attendance"
                                                    },
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=112, 
                                                        MetricVariantId = 1112,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        Name = "Discipline"
                                                    } 
                                            }
                            }
                        }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        private MetricMetadataNode GetSchoolRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    {
                        MetricId = 2, 
                        MetricVariantId = 1002,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "School Overview", 
                        MetricNodeId = 7, 
                        Parent = null,
                        Children = new List<MetricMetadataNode>
                                    {
                                        new MetricMetadataNode(tree)
                                        {
                                            MetricId=21, 
                                            MetricVariantId = 1021,
                                            MetricVariantType = MetricVariantType.CurrentYear,
                                            MetricNodeId = 71, 
                                            Name = "School's Attendance and Discipline",
                                            Children = new List<MetricMetadataNode>
                                                        {
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=211, 
                                                                    MetricVariantId = 1211,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    MetricNodeId = 711, 
                                                                    Name = "Attendance"
                                                                },
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=212, 
                                                                    MetricVariantId = 1212,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    Name = "Discipline"
                                                                } 
                                                        }
                                        },
                                        new MetricMetadataNode(tree)
                                            {
                                                MetricId=22, 
                                                MetricVariantId = 1022,
                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                MetricNodeId = 72, 
                                                Name = "School's Other Metric"
                                            },
                                    }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricCorrelationProvider(rootMetricNodeResolver);
            actualModel = service.GetRenderingParentMetricVariantIdForSchool(suppliedLocalEducationAgencyMetricVariantId, suppliedSchoolId);
        }

        [Test]
        public void Should_calculate_the_right_correlation_ids()
        {
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(expectedModel.MetricVariantId));
            Assert.That(actualModel.ContextMetricVariantId, Is.EqualTo(expectedModel.ContextMetricVariantId));
        }

    }

    [TestFixture]
    public class When_correlating_local_education_agency_metric_to_school_metric_with_no_matching_correlated_metric_in_both : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRootMetricNodeResolver rootMetricNodeResolver;

        //The Actual Model.
        private MetricCorrelationProvider.MetricRenderingContext actualModel;

        //The supplied Data models.
        private const int suppliedLocalEducationAgencyMetricVariantId = 1112;
        private const int suppliedSchoolId = 100;
        private MetricCorrelationProvider.MetricRenderingContext expectedModel = new MetricCorrelationProvider.MetricRenderingContext
                                                                                                {
                                                                                                    ContextMetricVariantId = null,
                                                                                                    MetricVariantId = null
                                                                                                };

        protected override void EstablishContext()
        {
            //Prepare supplied data collections

            //Set up the mocks
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();

            //Set expectations
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForLocalEducationAgency()).Return(GetLEARootOverviewNode());
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForSchool(suppliedSchoolId)).Return(GetSchoolRootOverviewNode());
        }

        private MetricMetadataNode GetLEARootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    { 
                        MetricId = 1, 
                        MetricVariantId = 1001,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "LEA Overview", 
                        Children = new List<MetricMetadataNode>
                        {
                            new MetricMetadataNode(tree) {
                                MetricId=11, 
                                MetricVariantId = 1011,
                                MetricVariantType = MetricVariantType.CurrentYear,
                                Name = "LEA's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                            {
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=111, 
                                                        MetricVariantId = 1111,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        Name = "Attendance"
                                                    },
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=112, 
                                                        MetricVariantId = 1112,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        Name = "Discipline"
                                                    } 
                                            }
                            }
                        }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        private MetricMetadataNode GetSchoolRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    {
                        MetricId = 2, 
                        MetricVariantId = 1002,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "School Overview", 
                        MetricNodeId = 7, 
                        Parent = null,
                        Children = new List<MetricMetadataNode>
                                    {
                                        new MetricMetadataNode(tree)
                                        {
                                            MetricId=21, 
                                            MetricVariantId = 1021,
                                            MetricVariantType = MetricVariantType.CurrentYear,
                                            MetricNodeId = 71, 
                                            Name = "School's Attendance and Discipline",
                                            Children = new List<MetricMetadataNode>
                                                        {
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=211, 
                                                                    MetricVariantId = 1211,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    MetricNodeId = 711, 
                                                                    Name = "Attendance"
                                                                },
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=212, 
                                                                    MetricVariantId = 1212,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    Name = "Discipline"
                                                                } 
                                                        }
                                        },
                                        new MetricMetadataNode(tree)
                                            {
                                                MetricId=22, 
                                                MetricVariantId = 1022,
                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                MetricNodeId = 72, 
                                                Name = "School's Other Metric"
                                            },
                                    }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricCorrelationProvider(rootMetricNodeResolver);
            actualModel = service.GetRenderingParentMetricVariantIdForSchool(suppliedLocalEducationAgencyMetricVariantId, suppliedSchoolId);
        }

        [Test]
        public void Should_calculate_the_right_correlation_ids()
        {
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(expectedModel.MetricVariantId));
            Assert.That(actualModel.ContextMetricVariantId, Is.EqualTo(expectedModel.ContextMetricVariantId));
        }

    }

    [TestFixture]
    public class When_correlating_school_metric_to_student_metric : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRootMetricNodeResolver rootMetricNodeResolver;

        //The Actual Model.
        private MetricCorrelationProvider.MetricRenderingContext actualModel;

        //The supplied Data models.
        private const int suppliedSchoolMetricVariantId = 1111;
        private const int suppliedSchoolId = 100;
        private MetricCorrelationProvider.MetricRenderingContext expectedModel = new MetricCorrelationProvider.MetricRenderingContext
                                                                                        {
                                                                                            ContextMetricVariantId = 1021,
                                                                                            MetricVariantId = 1211
                                                                                        };

        protected override void EstablishContext()
        {
            //Prepare supplied data collections

            //Set up the mocks
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();

            //Set expectations
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForSchool(suppliedSchoolId)).Return(GetSchoolRootOverviewNode());
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId)).Return(GetStudentRootOverviewNode());
        }

        private MetricMetadataNode GetSchoolRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    { 
                        MetricId = 1, 
                        MetricVariantId = 1001,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "LEA Overview", 
                        ChildDomainEntityMetricId = 2,
                        Children = new List<MetricMetadataNode>
                        {
                            new MetricMetadataNode(tree) {
                                MetricId=11, 
                                MetricVariantId = 1011,
                                MetricVariantType = MetricVariantType.CurrentYear,
                                ChildDomainEntityMetricId = 21, 
                                Name = "LEA's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                            {
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=111, 
                                                        MetricVariantId = 1111,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        ChildDomainEntityMetricId = 211, 
                                                        Name = "Attendance"
                                                    },
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=112, 
                                                        MetricVariantId = 1112,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        Name = "Discipline"
                                                    } 
                                            }
                            }
                        }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        private MetricMetadataNode GetStudentRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    {
                        MetricId = 2, 
                        MetricVariantId = 1002,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "School Overview", 
                        MetricNodeId = 7, 
                        Parent = null,
                        Children = new List<MetricMetadataNode>
                                    {
                                        new MetricMetadataNode(tree)
                                        {
                                            MetricId=21, 
                                            MetricVariantId = 1021,
                                            MetricVariantType = MetricVariantType.CurrentYear,
                                            MetricNodeId = 71, 
                                            Name = "School's Attendance and Discipline",
                                            Children = new List<MetricMetadataNode>
                                                        {
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=211, 
                                                                    MetricVariantId = 1211,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    MetricNodeId = 711, 
                                                                    Name = "Attendance"
                                                                },
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=212, 
                                                                    MetricVariantId = 1212,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    Name = "Discipline"
                                                                } 
                                                        }
                                        },
                                        new MetricMetadataNode(tree)
                                            {
                                                MetricId=22, 
                                                MetricVariantId = 1022,
                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                MetricNodeId = 72, 
                                                Name = "School's Other Metric"
                                            },
                                    }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricCorrelationProvider(rootMetricNodeResolver);
            actualModel = service.GetRenderingParentMetricVariantIdForStudent(suppliedSchoolMetricVariantId, suppliedSchoolId);
        }

        [Test]
        public void Should_calculate_the_right_correlation_ids()
        {
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(expectedModel.MetricVariantId));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(expectedModel.MetricVariantId));
            Assert.That(actualModel.ContextMetricVariantId, Is.EqualTo(expectedModel.ContextMetricVariantId));
        }

    }

    [TestFixture]
    public class When_correlating_school_metric_to_student_metric_for_prior_year : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRootMetricNodeResolver rootMetricNodeResolver;

        //The Actual Model.
        private MetricCorrelationProvider.MetricRenderingContext actualModel;

        //The supplied Data models.
        private const int suppliedSchoolMetricVariantId = 2111;
        private const int suppliedSchoolId = 100;
        private MetricCorrelationProvider.MetricRenderingContext expectedModel = new MetricCorrelationProvider.MetricRenderingContext
                                                                                    {
                                                                                        ContextMetricVariantId = 1021,
                                                                                        MetricVariantId = 2211
                                                                                    };

        protected override void EstablishContext()
        {
            //Prepare supplied data collections

            //Set up the mocks
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();

            //Set expectations
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForSchool(suppliedSchoolId)).Return(GetSchoolRootOverviewNode());
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId)).Return(GetStudentRootOverviewNode());
        }

        private MetricMetadataNode GetSchoolRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    { 
                        MetricId = 1, 
                        MetricVariantId = 1001,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "LEA Overview", 
                        ChildDomainEntityMetricId = 2,
                        Children = new List<MetricMetadataNode>
                        {
                            new MetricMetadataNode(tree) {
                                MetricId=11, 
                                MetricVariantId = 1011,
                                MetricVariantType = MetricVariantType.CurrentYear,
                                ChildDomainEntityMetricId = 21, 
                                Name = "LEA's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                            {
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=111, 
                                                        MetricVariantId = 1111,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        ChildDomainEntityMetricId = 211, 
                                                        Name = "Attendance"
                                                    },
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=111, 
                                                        MetricVariantId = 2111,
                                                        MetricVariantType = MetricVariantType.PriorYear,
                                                        ChildDomainEntityMetricId = 211, 
                                                        Name = "Attendance"
                                                    },
                                                new MetricMetadataNode(tree)
                                                    {
                                                        MetricId=112, 
                                                        MetricVariantId = 1112,
                                                        MetricVariantType = MetricVariantType.CurrentYear,
                                                        Name = "Discipline"
                                                    } 
                                            }
                            }
                        }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        private MetricMetadataNode GetStudentRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 0,
                MetricVariantId = 1000,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Root",
                Children = new List<MetricMetadataNode>
                {
                    new MetricMetadataNode(tree)
                    {
                        MetricId = 2, 
                        MetricVariantId = 1002,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        Name = "School Overview", 
                        MetricNodeId = 7, 
                        Parent = null,
                        Children = new List<MetricMetadataNode>
                                    {
                                        new MetricMetadataNode(tree)
                                        {
                                            MetricId=21, 
                                            MetricVariantId = 1021,
                                            MetricVariantType = MetricVariantType.CurrentYear,
                                            MetricNodeId = 71, 
                                            Name = "School's Attendance and Discipline",
                                            Children = new List<MetricMetadataNode>
                                                        {
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=211, 
                                                                    MetricVariantId = 1211,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    MetricNodeId = 711, 
                                                                    Name = "Attendance"
                                                                },
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=211, 
                                                                    MetricVariantId = 2211,
                                                                    MetricVariantType = MetricVariantType.PriorYear,
                                                                    MetricNodeId = 811, 
                                                                    Name = "Attendance"
                                                                },
                                                            new MetricMetadataNode(tree)
                                                                {
                                                                    MetricId=212, 
                                                                    MetricVariantId = 1212,
                                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                                    Name = "Discipline"
                                                                } 
                                                        }
                                        },
                                        new MetricMetadataNode(tree)
                                            {
                                                MetricId=22, 
                                                MetricVariantId = 1022,
                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                MetricNodeId = 72, 
                                                Name = "School's Other Metric"
                                            },
                                    }
                    }
                }
            };

            tree.Children = new List<MetricMetadataNode> { root };

            return root.Children.ElementAt(0);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricCorrelationProvider(rootMetricNodeResolver);
            actualModel = service.GetRenderingParentMetricVariantIdForStudent(suppliedSchoolMetricVariantId, suppliedSchoolId);
        }

        [Test]
        public void Should_calculate_the_right_correlation_ids()
        {
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(expectedModel.MetricVariantId));
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(expectedModel.MetricVariantId));
            Assert.That(actualModel.ContextMetricVariantId, Is.EqualTo(expectedModel.ContextMetricVariantId));
        }

    }

    [TestFixture]
    public class When_correlating_school_metric_to_student_metric_with_no_match : TestFixtureBase
    {
        //The Injected Dependencies.
        private IRootMetricNodeResolver rootMetricNodeResolver;

        //The Actual Model.
        private MetricCorrelationProvider.MetricRenderingContext actualModel;

        //The supplied Data models.
        private const int suppliedSchoolMetricVariantId = 1111;
        private const int suppliedSchoolId = 100;
        private MetricCorrelationProvider.MetricRenderingContext expectedModel = new MetricCorrelationProvider.MetricRenderingContext
                                                                                    {
                                                                                        ContextMetricVariantId = 1021,
                                                                                        MetricVariantId = null
                                                                                    };

        protected override void EstablishContext()
        {
            //Prepare supplied data collections

            //Set up the mocks
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();

            //Set expectations
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForSchool(suppliedSchoolId)).Return(GetSchoolRootOverviewNode());
            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId)).Return(GetStudentRootOverviewNode());
        }

        private MetricMetadataNode GetSchoolRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 1,
                MetricVariantId = 1001,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "School Overview",
                ChildDomainEntityMetricId = 2,
                Children = new List<MetricMetadataNode>
                    {
                        new MetricMetadataNode(tree)
                            {
                                MetricId=11, 
                                MetricVariantId = 1011,
                                MetricVariantType = MetricVariantType.CurrentYear,
                                ChildDomainEntityMetricId = 21, 
                                Name = "School's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                        {
                                            new MetricMetadataNode(tree)
                                                {
                                                    MetricId=111, 
                                                    MetricVariantId = 1111,
                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                    ChildDomainEntityMetricId = 1000, 
                                                    Name = "Attendance"
                                                },
                                            new MetricMetadataNode(tree)
                                                {
                                                    MetricId=112, 
                                                    MetricVariantId = 1112,
                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                    Name = "Discipline"
                                                } 
                                        }
                        }
                    }
            };

            tree.Children = new List<MetricMetadataNode> { root };
            return root;
        }

        private MetricMetadataNode GetStudentRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            var root = new MetricMetadataNode(tree)
            {
                MetricId = 2,
                MetricVariantId = 1002,
                MetricVariantType = MetricVariantType.CurrentYear,
                Name = "Student's Overview",
                MetricNodeId = 7,
                Children = new List<MetricMetadataNode>
                            {
                                new MetricMetadataNode(tree)
                                    {
                                        MetricId=21, 
                                        MetricVariantId = 1021,
                                        MetricVariantType = MetricVariantType.CurrentYear,
                                        MetricNodeId = 71, 
                                        Name = "Student's Attendance and Discipline",
                                        Children = new List<MetricMetadataNode>
                                                {
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId=211, 
                                                            MetricVariantId = 1211,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            MetricNodeId = 711, 
                                                            Name = "Attendance"
                                                        },
                                                    new MetricMetadataNode(tree)
                                                        {
                                                            MetricId=212, 
                                                            MetricVariantId = 1212,
                                                            MetricVariantType = MetricVariantType.CurrentYear,
                                                            Name = "Discipline"
                                                        } 
                                                }
                                },
                                new MetricMetadataNode(tree)
                                    {
                                        MetricId=22, 
                                        MetricVariantId = 1022,
                                        MetricVariantType = MetricVariantType.CurrentYear,
                                        MetricNodeId = 72, 
                                        Name = "School's Other Metric"
                                    },
                            }
            };

            tree.Children = new List<MetricMetadataNode> { root };
            return root;
        }

        protected override void ExecuteTest()
        {
            var service = new MetricCorrelationProvider(rootMetricNodeResolver);
            actualModel = service.GetRenderingParentMetricVariantIdForStudent(suppliedSchoolMetricVariantId, suppliedSchoolId);
        }

        [Test]
        public void Should_calculate_the_right_correlation_ids()
        {
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(expectedModel.MetricVariantId));
            Assert.That(actualModel.ContextMetricVariantId, Is.EqualTo(expectedModel.ContextMetricVariantId));
        }

    }
}
