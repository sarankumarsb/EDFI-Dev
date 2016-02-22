// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Presentation.Core.Providers.Metric;
using EdFi.Dashboards.Presentation.Web.Providers.Metric;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Tests;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Presentation.Web.Tests.Providers.Metric
{
    [TestFixture]
    public abstract class MetricNodeResolverTestFixtureBase : TestFixtureBase
    {
        //The Injected Dependencies.
        protected IEdFiDashboardContextProvider dashboardContextProvider;
        protected IMetricMetadataTreeService metricMetadataTreeService;
        protected IRootMetricNodeResolver rootMetricNodeResolver;

        //The supplied Data models.
        protected int suppliedStudentMetricId = 1;
        protected int suppliedSchoolMetricId = 2;
        protected int suppliedLocalEducationAgencyMetricId = 3;
        protected int suppliedStudentMetricVariantId = 11;
        protected int suppliedSchoolMetricVariantId = 21;
        protected int suppliedLocalEducationAgencyMetricVariantId = 31;
        protected int suppliedSchoolId = 10;
        protected int suppliedLocalEducationAgencyId = 20;
        protected MetricMetadataTree suppliedMetadataTree;
        protected EdFiDashboardContext suppliedContext;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedMetadataTree = GetSuppliedMetricMetadataTree();

            //Set up the mocks
            dashboardContextProvider = mocks.StrictMock<IEdFiDashboardContextProvider>();
            metricMetadataTreeService = mocks.StrictMock<IMetricMetadataTreeService>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();

            //Set expectations
            Expect.Call(metricMetadataTreeService.Get(null)).IgnoreArguments().Return(suppliedMetadataTree);
        }

        protected EdFiDashboardContext GetStudentSchoolContext()
        {
            return new EdFiDashboardContext
                       {
                           MetricInstanceSetType = MetricInstanceSetType.StudentSchool,
                           SchoolId = suppliedSchoolId
                       };
        }

        protected EdFiDashboardContext GetSchoolContext()
        {
            return new EdFiDashboardContext
                        {
                            MetricInstanceSetType = MetricInstanceSetType.School,
                            SchoolId = suppliedSchoolId
                        };
        }

        protected EdFiDashboardContext GetStaffContext()
        {
            return new EdFiDashboardContext
            {
                MetricInstanceSetType = MetricInstanceSetType.Staff,
                SchoolId = suppliedSchoolId
            };
        }

        protected EdFiDashboardContext GetLocalEducationAgencyContext()
        {
            return new EdFiDashboardContext
                        {
                            MetricInstanceSetType = MetricInstanceSetType.LocalEducationAgency,
                            LocalEducationAgencyId = suppliedLocalEducationAgencyId
                        };
        }

        protected MetricMetadataTree GetSuppliedMetricMetadataTree()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
            {
                new MetricMetadataNode(tree)
                    {
                        Name = "High School Root",
                        MetricId = 10,
                        MetricVariantId = 1000,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        RootNodeId = 265,
                        Children = new List<MetricMetadataNode>
                                        {
                                            new MetricMetadataNode(tree)
                                                {
                                                    Name = "School Overview",
                                                    MetricId = 101,
                                                    MetricVariantId = 10100,
                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                    RootNodeId = 265,
                                                    Children = new List<MetricMetadataNode>
                                                                    {
                                                                        new MetricMetadataNode(tree)
                                                                            {
                                                                                Name = "Metric to Find",
                                                                                RootNodeId = 265,
                                                                                MetricId = suppliedSchoolMetricId,
                                                                                MetricVariantId = suppliedSchoolMetricVariantId,
                                                                                MetricVariantType = MetricVariantType.PriorYear,
                                                                                MetricNodeId = 1005,
                                                                            },
                                                                        new MetricMetadataNode(tree)
                                                                            {
                                                                                Name = "Metric to Find",
                                                                                RootNodeId = 265,
                                                                                MetricId = suppliedSchoolMetricId,
                                                                                MetricVariantId = suppliedSchoolMetricVariantId + 2000,
                                                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                                                MetricNodeId = 1006,
                                                                            }
                                                                    }
                                                },
                                            new MetricMetadataNode(tree)
                                                {
                                                    Name = "Student Overview",
                                                    MetricId = 102,
                                                    MetricVariantId = 10200,
                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                    RootNodeId = 265,
                                                    Children = new List<MetricMetadataNode>
                                                                    {
                                                                        new MetricMetadataNode(tree)
                                                                            {
                                                                                Name = "Metric to Find",
                                                                                RootNodeId = 265,
                                                                                MetricId = suppliedStudentMetricId,
                                                                                MetricVariantId = suppliedStudentMetricVariantId,
                                                                                MetricVariantType = MetricVariantType.PriorYear,
                                                                                MetricNodeId = 1000,
                                                                            },
                                                                        new MetricMetadataNode(tree)
                                                                            {
                                                                                Name = "Metric to Find",
                                                                                RootNodeId = 265,
                                                                                MetricId = suppliedStudentMetricId,
                                                                                MetricVariantId = suppliedStudentMetricVariantId + 2000,
                                                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                                                MetricNodeId = 1004,
                                                                            }
                                                                    }
                                                }
                                        }
                    },
                new MetricMetadataNode(tree)
                    {
                        Name = "Elementary Root",
                        MetricId = 20,
                        MetricVariantId = 2000,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        RootNodeId = 300,
                        Children = new List<MetricMetadataNode>
                                        {
                                            new MetricMetadataNode(tree)
                                                {
                                                    Name = "School Overview",
                                                    MetricId = 201,
                                                    MetricVariantId = 20100,
                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                    RootNodeId = 300,
                                                    Children = new List<MetricMetadataNode>
                                                                    {
                                                                        new MetricMetadataNode(tree)
                                                                            {
                                                                                Name = "Metric to Ignore",
                                                                                RootNodeId = 300,
                                                                                MetricId = suppliedSchoolMetricId,
                                                                                MetricVariantId = suppliedSchoolMetricVariantId,
                                                                                MetricVariantType = MetricVariantType.PriorYear,
                                                                                MetricNodeId = 2005,
                                                                            },
                                                                        new MetricMetadataNode(tree)
                                                                            {
                                                                                Name = "Metric to Ignore",
                                                                                RootNodeId = 300,
                                                                                MetricId = suppliedSchoolMetricId,
                                                                                MetricVariantId = suppliedSchoolMetricVariantId + 2000,
                                                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                                                MetricNodeId = 2006,
                                                                            }
                                                                    }
                                                },
                                            new MetricMetadataNode(tree)
                                                {
                                                    Name = "Student Overview",
                                                    MetricId = 202,
                                                    MetricVariantId = 20200,
                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                    RootNodeId = 300,
                                                    Children = new List<MetricMetadataNode>
                                                                    {
                                                                        new MetricMetadataNode(tree)
                                                                            {
                                                                                Name = "Metric to Ignore",
                                                                                RootNodeId = 300,
                                                                                MetricId = suppliedStudentMetricId,
                                                                                MetricVariantId = suppliedStudentMetricVariantId,
                                                                                MetricVariantType = MetricVariantType.PriorYear,
                                                                                MetricNodeId = 2001,
                                                                            },
                                                                        new MetricMetadataNode(tree)
                                                                            {
                                                                                Name = "Metric to Ignore2",
                                                                                RootNodeId = 300,
                                                                                MetricId = suppliedStudentMetricId,
                                                                                MetricVariantId = suppliedStudentMetricVariantId + 2000,
                                                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                                                MetricNodeId = 2002,
                                                                            }
                                                                    }
                                                }
                                        }
                    },
                    new MetricMetadataNode(tree)
                    {
                        Name = "Local Education Agency Root",
                        MetricId = 10,
                        MetricVariantId = 1000,
                        MetricVariantType = MetricVariantType.CurrentYear,
                        RootNodeId = 1662,
                        Children = new List<MetricMetadataNode>
                                        {
                                            new MetricMetadataNode(tree)
                                                {
                                                    Name = "Local Education Agency Overview",
                                                    MetricId = 101,
                                                    MetricVariantId = 10100,
                                                    MetricVariantType = MetricVariantType.CurrentYear,
                                                    RootNodeId = 1662,
                                                    Children = new List<MetricMetadataNode>
                                                                    {
                                                                        new MetricMetadataNode(tree)
                                                                            {
                                                                                Name = "Metric to Find",
                                                                                RootNodeId = 1662,
                                                                                MetricId = suppliedLocalEducationAgencyMetricId,
                                                                                MetricVariantId = suppliedLocalEducationAgencyMetricVariantId,
                                                                                MetricVariantType = MetricVariantType.PriorYear,
                                                                                MetricNodeId = 3005,
                                                                            },
                                                                        new MetricMetadataNode(tree)
                                                                            {
                                                                                Name = "Metric to Find",
                                                                                RootNodeId = 1662,
                                                                                MetricId = suppliedLocalEducationAgencyMetricId,
                                                                                MetricVariantId = suppliedLocalEducationAgencyMetricVariantId + 2000,
                                                                                MetricVariantType = MetricVariantType.CurrentYear,
                                                                                MetricNodeId = 3006,
                                                                            }
                                                                    }
                                                },
                                        }
                    }
                };

            return tree;
        }
    }

    public class When_resolving_a_metric_id_for_student_school_context : MetricNodeResolverTestFixtureBase
    {
        private IEnumerable<MetricMetadataNode> actualModel;

        protected override void EstablishContext()
        {
            suppliedContext = GetStudentSchoolContext();

            base.EstablishContext();
            Expect.Call(rootMetricNodeResolver.GetMetricHierarchyRoot(suppliedSchoolId)).Return(MetricHierarchyRoot.HighSchool);
            Expect.Call(dashboardContextProvider.GetEdFiDashboardContext()).Return(suppliedContext);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricNodeResolver(dashboardContextProvider, metricMetadataTreeService, rootMetricNodeResolver);
            actualModel = service.ResolveFromMetricId(suppliedStudentMetricId);
        }

        [Test]
        public void Should_return_metric_nodes_correctly()
        {
            Assert.That(actualModel.Count(), Is.EqualTo(2));
            Assert.That(actualModel.Count(x => x.MetricId == suppliedStudentMetricId), Is.EqualTo(2));
            Assert.That(actualModel.Count(x => x.RootNodeId == 265), Is.EqualTo(2));
        }
    }

    public class When_resolving_a_metric_id_for_school_context : MetricNodeResolverTestFixtureBase
    {
        private IEnumerable<MetricMetadataNode> actualModel;

        protected override void EstablishContext()
        {
            suppliedContext = GetSchoolContext();

            base.EstablishContext();
            Expect.Call(rootMetricNodeResolver.GetMetricHierarchyRoot(suppliedSchoolId)).Return(MetricHierarchyRoot.HighSchool);
            Expect.Call(dashboardContextProvider.GetEdFiDashboardContext()).Return(suppliedContext);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricNodeResolver(dashboardContextProvider, metricMetadataTreeService, rootMetricNodeResolver);
            actualModel = service.ResolveFromMetricId(suppliedSchoolMetricId);
        }

        [Test]
        public void Should_return_metric_nodes_correctly()
        {
            Assert.That(actualModel.Count(), Is.EqualTo(2));
            Assert.That(actualModel.Count(x => x.MetricId == suppliedSchoolMetricId), Is.EqualTo(2));
            Assert.That(actualModel.Count(x => x.RootNodeId == 265), Is.EqualTo(2));
        }
    }

    public class When_resolving_a_metric_id_for_staff_context : MetricNodeResolverTestFixtureBase
    {
        private IEnumerable<MetricMetadataNode> actualModel;

        protected override void EstablishContext()
        {
            suppliedContext = GetStaffContext();

            base.EstablishContext();
            Expect.Call(rootMetricNodeResolver.GetMetricHierarchyRoot(suppliedSchoolId)).Return(MetricHierarchyRoot.HighSchool);
            Expect.Call(dashboardContextProvider.GetEdFiDashboardContext()).Return(suppliedContext);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricNodeResolver(dashboardContextProvider, metricMetadataTreeService, rootMetricNodeResolver);
            actualModel = service.ResolveFromMetricId(suppliedSchoolMetricId);
        }

        [Test]
        public void Should_return_metric_nodes_correctly()
        {
            Assert.That(actualModel.Count(), Is.EqualTo(2));
            Assert.That(actualModel.Count(x => x.MetricId == suppliedSchoolMetricId), Is.EqualTo(2));
            Assert.That(actualModel.Count(x => x.RootNodeId == 265), Is.EqualTo(2));
        }
    }

    public class When_resolving_a_metric_id_for_local_education_agency_context : MetricNodeResolverTestFixtureBase
    {
        private IEnumerable<MetricMetadataNode> actualModel;

        protected override void EstablishContext()
        {
            suppliedContext = GetLocalEducationAgencyContext();

            base.EstablishContext();
            Expect.Call(dashboardContextProvider.GetEdFiDashboardContext()).Return(suppliedContext);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricNodeResolver(dashboardContextProvider, metricMetadataTreeService, rootMetricNodeResolver);
            actualModel = service.ResolveFromMetricId(suppliedLocalEducationAgencyMetricId);
        }

        [Test]
        public void Should_return_metric_nodes_correctly()
        {
            Assert.That(actualModel.Count(), Is.EqualTo(2));
            Assert.That(actualModel.Count(x => x.MetricId == suppliedLocalEducationAgencyMetricId), Is.EqualTo(2));
            Assert.That(actualModel.Count(x => x.RootNodeId == 1662), Is.EqualTo(2));
        }
    }
    public class When_resolving_a_metric_variant_id_for_student_school_context : MetricNodeResolverTestFixtureBase
    {
        private MetricMetadataNode actualModel;

        protected override void EstablishContext()
        {
            suppliedContext = GetStudentSchoolContext();

            base.EstablishContext();
            Expect.Call(rootMetricNodeResolver.GetMetricHierarchyRoot(suppliedSchoolId)).Return(MetricHierarchyRoot.HighSchool);
            Expect.Call(dashboardContextProvider.GetEdFiDashboardContext()).Return(suppliedContext);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricNodeResolver(dashboardContextProvider, metricMetadataTreeService, rootMetricNodeResolver);
            actualModel = service.ResolveFromMetricVariantId(suppliedStudentMetricVariantId);
        }

        [Test]
        public void Should_return_metric_nodes_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedStudentMetricVariantId));
            Assert.That(actualModel.RootNodeId, Is.EqualTo(265));
        }
    }

    public class When_resolving_a_metric_variant_id_for_school_context : MetricNodeResolverTestFixtureBase
    {
        private MetricMetadataNode actualModel;

        protected override void EstablishContext()
        {
            suppliedContext = GetSchoolContext();

            base.EstablishContext();
            Expect.Call(rootMetricNodeResolver.GetMetricHierarchyRoot(suppliedSchoolId)).Return(MetricHierarchyRoot.HighSchool);
            Expect.Call(dashboardContextProvider.GetEdFiDashboardContext()).Return(suppliedContext);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricNodeResolver(dashboardContextProvider, metricMetadataTreeService, rootMetricNodeResolver);
            actualModel = service.ResolveFromMetricVariantId(suppliedSchoolMetricVariantId);
        }

        [Test]
        public void Should_return_metric_nodes_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedSchoolMetricVariantId));
            Assert.That(actualModel.RootNodeId, Is.EqualTo(265));
        }
    }

    public class When_resolving_a_metric_variant_id_for_staff_context : MetricNodeResolverTestFixtureBase
    {
        private MetricMetadataNode actualModel;

        protected override void EstablishContext()
        {
            suppliedContext = GetStaffContext();

            base.EstablishContext();
            Expect.Call(rootMetricNodeResolver.GetMetricHierarchyRoot(suppliedSchoolId)).Return(MetricHierarchyRoot.HighSchool);
            Expect.Call(dashboardContextProvider.GetEdFiDashboardContext()).Return(suppliedContext);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricNodeResolver(dashboardContextProvider, metricMetadataTreeService, rootMetricNodeResolver);
            actualModel = service.ResolveFromMetricVariantId(suppliedSchoolMetricVariantId);
        }

        [Test]
        public void Should_return_metric_nodes_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedSchoolMetricVariantId));
            Assert.That(actualModel.RootNodeId, Is.EqualTo(265));
        }
    }

    public class When_resolving_a_metric_variant_id_for_local_education_agency_context : MetricNodeResolverTestFixtureBase
    {
        private MetricMetadataNode actualModel;

        protected override void EstablishContext()
        {
            suppliedContext = GetLocalEducationAgencyContext();

            base.EstablishContext();
            Expect.Call(dashboardContextProvider.GetEdFiDashboardContext()).Return(suppliedContext);
        }

        protected override void ExecuteTest()
        {
            var service = new MetricNodeResolver(dashboardContextProvider, metricMetadataTreeService, rootMetricNodeResolver);
            actualModel = service.ResolveFromMetricVariantId(suppliedLocalEducationAgencyMetricVariantId);
        }

        [Test]
        public void Should_return_metric_nodes_correctly()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.MetricVariantId, Is.EqualTo(suppliedLocalEducationAgencyMetricVariantId));
            Assert.That(actualModel.RootNodeId, Is.EqualTo(1662));
        }
    }

    public class When_resolving_metric_variant_id_to_metric_id : MetricNodeResolverTestFixtureBase
    {
        private int actualModel;

        protected override void ExecuteTest()
        {
            var service = new MetricNodeResolver(dashboardContextProvider, metricMetadataTreeService, rootMetricNodeResolver);
            actualModel = service.ResolveMetricId(suppliedSchoolMetricVariantId);
        }

        [Test]
        public void Should_return_metric_id_correctly()
        {
            Assert.That(actualModel, Is.EqualTo(suppliedSchoolMetricId));
        }
    }

    public class When_resolving_metric_variant_id_to_metric_variant_type : MetricNodeResolverTestFixtureBase
    {
        private MetricVariantType actualModel;

        protected override void ExecuteTest()
        {
            var service = new MetricNodeResolver(dashboardContextProvider, metricMetadataTreeService, rootMetricNodeResolver);
            actualModel = service.ResolveMetricVariantType(suppliedSchoolMetricVariantId);
        }

        [Test]
        public void Should_return_metric_id_correctly()
        {
            Assert.That(actualModel, Is.EqualTo(MetricVariantType.PriorYear));
        }
    }
}
