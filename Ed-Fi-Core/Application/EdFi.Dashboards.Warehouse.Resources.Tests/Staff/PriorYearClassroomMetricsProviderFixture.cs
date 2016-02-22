using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resources.Metric.AdditionalPriorYearMetricProviders;
using EdFi.Dashboards.Warehouse.Resources.Staff;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Warehouse.Resources.Tests.Staff
{
    [TestFixture]
    public abstract class When_calling_the_prior_year_classroom_metrics_provider_base : TestFixtureBase
    {
        protected IWindsorContainer windsorContainer;

        //The Injected Dependencies.
        protected IListMetadataProvider listMetadataProvider;
        protected IStudentListUtilitiesProvider studentListUtilitiesProvider;
        protected ITrendRenderingDispositionProvider trendRenderingDispositionProvider;
        protected IMetricStateProvider metricStateProvider;
        protected IAdditionalPriorYearMetricProvider additionalPriorYearMetricProvider;
        protected IRepository<MetricVariant> metricVariantRepository;

        //The Actual Model.
        protected List<StudentWithMetrics.Metric> actualModel;

        //The supplied Data models.
        protected List<MetadataColumnGroup> suppliedListMetadata;
        protected IEnumerable<StudentSchoolMetricInstance> suppliedPriorYearStudentRow;
        protected IEnumerable<StudentMetric> suppliedStudentRow;
        protected IEnumerable<MetricVariant> suppliedMetricVariants; 
        protected int suppliedStudentUSI = 12345;
        protected int suppliedAbsencesMetricVariantId = 200024;
        protected int suppliedAttendanceMetricVariantId = 2000442;
        protected const int suppliedSchoolId = 1000;
        protected const int suppliedSchoolId2 = 2000;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedListMetadata = GetSuppliedListMetadata();
            suppliedStudentRow = GetSuppliedStudentRow();
            suppliedPriorYearStudentRow = GetSuppliedPriorYearStudentList();
            suppliedMetricVariants = GetSuppliedMetricVariants();

            //Set up the mocks
            listMetadataProvider = mocks.StrictMock<IListMetadataProvider>();
            studentListUtilitiesProvider = mocks.StrictMock<IStudentListUtilitiesProvider>();
            trendRenderingDispositionProvider = mocks.StrictMock<ITrendRenderingDispositionProvider>();
            metricStateProvider = mocks.StrictMock<IMetricStateProvider>();
            metricVariantRepository = mocks.StrictMock<IRepository<MetricVariant>>();

            windsorContainer = new WindsorContainer();
            windsorContainer.Kernel.Resolver.AddSubResolver(new ArrayResolver(windsorContainer.Kernel));
            RegisterProviders(windsorContainer);
            IoC.Initialize(windsorContainer);

            additionalPriorYearMetricProvider = IoC.Resolve<IAdditionalPriorYearMetricProvider>();

            Expect.Call(metricVariantRepository.GetAll())
                .Return(suppliedMetricVariants.AsQueryable()).Repeat.AtLeastOnce();
        }

        private void RegisterProviders(IWindsorContainer container)
        {
            container.Register(Component
                .For<IListMetadataProvider>()
                .Instance(listMetadataProvider));

            container.Register(Component
                .For<IStudentListUtilitiesProvider>()
                .Instance(studentListUtilitiesProvider));

            container.Register(Component
                .For<ITrendRenderingDispositionProvider>()
                .Instance(trendRenderingDispositionProvider));

            container.Register(Component
                .For<IMetricStateProvider>()
                .Instance(metricStateProvider));

            var assemblyTypes = typeof(Marker_EdFi_Dashboards_Warehouse_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                              where typeof(IAdditionalPriorYearMetricProvider).IsAssignableFrom(t) && t != typeof(IAdditionalPriorYearMetricProvider)
                              select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IAdditionalPriorYearMetricProvider, NullAdditionalPriorYearMetricProvider>(chainTypes.ToArray());
        }

        private IEnumerable<MetricVariant> GetSuppliedMetricVariants()
        {
            return new List<MetricVariant>
                       {
                           new MetricVariant{MetricId = suppliedAbsencesMetricVariantId - 20000, MetricVariantId = suppliedAbsencesMetricVariantId},
                           new MetricVariant{MetricId = 3, MetricVariantId = 3},
                           new MetricVariant{MetricId = 4, MetricVariantId = 4},
                           new MetricVariant{MetricId = 5, MetricVariantId = 5},
                           new MetricVariant{MetricId = suppliedAttendanceMetricVariantId - 20000, MetricVariantId = suppliedAttendanceMetricVariantId},
                       };
        }

        protected virtual List<MetadataColumnGroup> GetSuppliedListMetadata()
        {
            return new List<MetadataColumnGroup>
                       {
                           new MetadataColumnGroup
                                        {
                                            //this group will be ignored because we are looking for metrics...
                                            GroupType = GroupType.EntityInformation,
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "ATTENDANCE / DISCIPLINE",
                                            IsVisibleByDefault = true,
                                            Columns = new List<MetadataColumn>
                                                           {
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 400,
                                                                       ColumnPrefix = "PriorYearAbsences",
                                                                       MetricVariantId = suppliedAbsencesMetricVariantId,
                                                                       ColumnName = "Prior Year Absences",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.TrendMetric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 100,
                                                                       ColumnPrefix = "AttendanceCurrent",
                                                                       MetricVariantId = 3,
                                                                       ColumnName = "Current Attendance",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.TrendMetric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 200,
                                                                       ColumnPrefix = "AttendancePrevious",
                                                                       MetricVariantId = 4,
                                                                       ColumnName = "Previous Attendance",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.Metric,
                                                                   },
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 500,
                                                                       ColumnPrefix = "PriorYearAttendance",
                                                                       MetricVariantId = suppliedAttendanceMetricVariantId,
                                                                       ColumnName = "Prior Year Attendance",
                                                                       IsVisibleByDefault = false,
                                                                       MetricListCellType = MetricListCellType.TrendMetric,
                                                                   },
                                                           },
                                        },
                                    new MetadataColumnGroup
                                        {
                                            GroupType = GroupType.MetricData,
                                            Title = "ASSESSMENTS",
                                            IsVisibleByDefault = true,
                                            Columns = new List<MetadataColumn>
                                                           {
                                                               new MetadataColumn
                                                                   {
                                                                       UniqueIdentifier = 300,
                                                                       ColumnPrefix = "BenchmarkReading",
                                                                       ColumnName = "Benchmark ELA/Reading",
                                                                       IsVisibleByDefault = true,
                                                                       MetricListCellType = MetricListCellType.StateValueMetric,
                                                                   },                                                           
                                                           },
                                        }
                       };
        }

        protected virtual IEnumerable<StudentSchoolMetricInstance> GetSuppliedPriorYearStudentList()
        {
            return new List<StudentSchoolMetricInstance>
            {
                new StudentSchoolMetricInstance
                       {
                           StudentUSI = suppliedStudentUSI,
                           SchoolId = suppliedSchoolId,

                           MetricId = 5,
                           Value = "4",
                           MetricStateTypeId = 1,
                           TrendDirection = -1,
                           ValueTypeName = "System.Int32",
                       },
                new StudentSchoolMetricInstance
                       {
                           StudentUSI = suppliedStudentUSI,
                           SchoolId = suppliedSchoolId,

                           MetricId = 6,
                           Value = ".98",
                           MetricStateTypeId = 3,
                           TrendDirection = 1,
                           ValueTypeName = "System.Double"
                       },


            };
        }

        protected virtual IEnumerable<StudentMetric> GetSuppliedStudentRow()
        {
            return new List<StudentMetric>
            {
                new StudentMetric
                {
                    StudentUSI = suppliedStudentUSI,
                    SchoolId = suppliedSchoolId2,

                    MetricId = 3,
                    MetricVariantId = 399,
                    Value = ".111",
                    MetricStateTypeId = 1,
                    ValueTypeName = "System.Double",
                    Format = "{0:P1}",
                    TrendInterpretation = 1,
                    TrendDirection = 1,

                },
                new StudentMetric
                {
                    StudentUSI = suppliedStudentUSI,
                    SchoolId = suppliedSchoolId2,

                    MetricId = 4,
                    MetricVariantId = 499,
                    Value = ".222",
                    MetricStateTypeId = null,
                    ValueTypeName = "System.Double",
                    Format = "{0:P1}",
                    TrendInterpretation = 1,
                    TrendDirection = -1,

                },
                new StudentMetric
                {
                    StudentUSI = suppliedStudentUSI,
                    SchoolId = suppliedSchoolId2,

                    MetricId = 74,
                    MetricVariantId = 7499,
                    Value = ".333",
                    MetricStateTypeId = null,
                    ValueTypeName = "System.Double",
                    Format = "{0:P1}",
                    TrendInterpretation = 1,
                    TrendDirection = -1,

                }
            };
        }

        protected override void ExecuteTest()
        {
            var provider = new PriorYearClassroomMetricsProvider(additionalPriorYearMetricProvider, metricVariantRepository);

            actualModel = provider.GetAdditionalMetrics(suppliedPriorYearStudentRow, suppliedStudentRow, suppliedListMetadata);
        }

        [Test]
        public void Should_call_methods_with_right_parameters()
        {
            //The sole purpose of this test fixture is to test that the right things get called so this test relies solely on the Expect.Call and not on any actual return.
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }

    }

    public class When_calling_the_prior_year_classroom_metrics_provider :
        When_calling_the_prior_year_classroom_metrics_provider_base
    {
    }
}
