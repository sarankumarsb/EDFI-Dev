using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.LocalEducationAgency;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;
using Moq;
using NUnit.Framework;

namespace EdFi.Dashboards.Application.Resources.Tests.LocalEducationAgency
{
    [TestFixture]
    public abstract class MetricThresholdServiceFixtureBase : TestFixtureBase
    {
        protected int suppliedLocalEducationAgencyId = 1;
        protected int suppliedMetricId = 1000;
        protected decimal suppliedMetricThreshold = 0.1m;
        protected bool suppliedIsInclusive = true;

        protected Mock<IPersistingRepository<LocalEducationAgencyMetricThreshold>> LocalEducationAgencyMetricThresholdRepository = new Mock<IPersistingRepository<LocalEducationAgencyMetricThreshold>>();

        protected override void EstablishContext()
        {
            LocalEducationAgencyMetricThresholdRepository.Setup(x => x.GetAll()).Returns(GetSuppliedLocalEducationAgencyMetricThresholdInformation());
        }


        protected IQueryable<LocalEducationAgencyMetricThreshold>
            GetSuppliedLocalEducationAgencyMetricThresholdInformation()
        {
            return (new List<LocalEducationAgencyMetricThreshold>
                        {
                            new LocalEducationAgencyMetricThreshold
                                {
                                    LocalEducationAgencyMetricThresholdId = 1,
                                    LocalEducationAgencyId = suppliedLocalEducationAgencyId,
                                    MetricId = suppliedMetricId,
                                    Threshold = suppliedMetricThreshold,
                                    IsInclusive = suppliedIsInclusive
                                },
                            new LocalEducationAgencyMetricThreshold
                                {
                                    LocalEducationAgencyMetricThresholdId = 2,
                                    LocalEducationAgencyId = suppliedLocalEducationAgencyId,
                                    MetricId = 1234,
                                    Threshold = 0.24m,
                                    IsInclusive = true
                                },
                            new LocalEducationAgencyMetricThreshold
                                {
                                    LocalEducationAgencyMetricThresholdId = 3,
                                    LocalEducationAgencyId = suppliedLocalEducationAgencyId,
                                    MetricId = 1235,
                                    Threshold = suppliedMetricThreshold,
                                    IsInclusive = true
                                },
                            new LocalEducationAgencyMetricThreshold
                                {
                                    LocalEducationAgencyMetricThresholdId = 4,
                                    LocalEducationAgencyId = 9,
                                    MetricId = 1236,
                                    Threshold = suppliedMetricThreshold,
                                    IsInclusive = true
                                },
                            new LocalEducationAgencyMetricThreshold
                                {
                                    LocalEducationAgencyMetricThresholdId = 5,
                                    LocalEducationAgencyId = 99,
                                    MetricId = 1237,
                                    Threshold = 0.25m,
                                    IsInclusive = true
                                }

                        }).AsQueryable();
        }

    }

    public class When_calling_metric_threshold_service_with_only_the_localEducationAgencyId : MetricThresholdServiceFixtureBase
    {
        protected IEnumerable<MetricThresholdGetResponse> GetResponse;

        protected override void ExecuteTest()
        {
            var service = new MetricThresholdService(LocalEducationAgencyMetricThresholdRepository.Object);
            GetResponse = service.Get(new MetricThresholdGetRequest { LocalEducationAgencyId = suppliedLocalEducationAgencyId });
        }

        [Test]
        public virtual void Should_create_model_correctly()
        {
            Assert.That(GetResponse, Is.Not.Null);
        }

        [Test]
        public void Should_have_all_values_set()
        {
            GetResponse.EnsureNoDefaultValues();
        }

        [Test]
        public virtual void Should_return_data()
        {
            Assert.That(GetResponse.Count(), Is.GreaterThan(0));
        }
    }

    public class When_calling_metric_threshold_service_get_with_LocalEducationAgencyId_and_metricId : MetricThresholdServiceFixtureBase
    {
        protected IEnumerable<MetricThresholdGetResponse> GetResponse;

        protected override void ExecuteTest()
        {
            var service = new MetricThresholdService(LocalEducationAgencyMetricThresholdRepository.Object);
            GetResponse = service.Get(new MetricThresholdGetRequest { LocalEducationAgencyId = suppliedLocalEducationAgencyId, MetricId = suppliedMetricId });
        }

        [Test]
        public virtual void Should_create_model_correctly()
        {
            Assert.That(GetResponse, Is.Not.Null);
        }

        [Test]
        public void Should_have_all_values_set()
        {
            GetResponse.EnsureNoDefaultValues();
        }

        [Test]
        public virtual void Should_return_one_record_if_metric_id_is_given()
        {
            Assert.That(GetResponse.Count(), Is.EqualTo(1));
        }

        [Test]
        public virtual void Should_return_correct_data_in_single_item()
        {
            Assert.That(GetResponse.First().MetricId, Is.EqualTo(suppliedMetricId));
            Assert.That(GetResponse.First().Threshold, Is.EqualTo(suppliedMetricThreshold));
            Assert.That(GetResponse.First().IsInclusive, Is.EqualTo(true));
        }

    }

    // TODO: GKM commented out due to limitation of using callbacks with Moq that contain "out" or "ref" parameters on the original method.  This fixture needs to use RhinoMocks?
    //public class When_calling_metric_threshold_service_put_method : MetricThresholdServiceFixtureBase
    //{
    //    private readonly List<LocalEducationAgencyMetricThreshold> putMetricThreshold = new List<LocalEducationAgencyMetricThreshold>();
    //    protected MetricThresholdPutResponse PutResponse;

    //    private readonly decimal updatedThresholdValue = .099m;
    //    private bool suppliedCreated;

    //    protected override void EstablishContext()
    //    {
    //        base.EstablishContext();

    //        suppliedCreated = false;
    //        LocalEducationAgencyMetricThresholdRepository
    //            .Setup(x => x.Save(It.IsAny<LocalEducationAgencyMetricThreshold>(), out suppliedCreated))
    //            .Callback((LocalEducationAgencyMetricThreshold entity) => putMetricThreshold.Add(entity));
    //    }

    //    protected override void ExecuteTest()
    //    {
    //        bool created;
    //        var service = new MetricThresholdService(LocalEducationAgencyMetricThresholdRepository.Object);
    //        PutResponse = service.Put(MetricThresholdPutRequest.Create(suppliedLocalEducationAgencyId, suppliedMetricId, suppliedMetricThreshold, suppliedIsInclusive), out created);
    //    }

    //    [Test]
    //    public virtual void Should_create_model_correctly()
    //    {
    //        Assert.That(PutResponse, Is.Not.Null);
    //    }

    //    [Test]
    //    public void Should_have_all_values_set()
    //    {
    //        PutResponse.EnsureNoDefaultValues();
    //    }

    //    [Test]
    //    public void Should_add_a_new_record_if_it_does_not_exist()
    //    {
    //        Assert.That(putMetricThreshold.Count(), Is.EqualTo(1));
    //        Assert.That(putMetricThreshold[0].LocalEducationAgencyId, Is.EqualTo(suppliedLocalEducationAgencyId));
    //        Assert.That(putMetricThreshold[0].MetricId, Is.EqualTo(suppliedMetricId));
    //        Assert.That(putMetricThreshold[0].Threshold, Is.EqualTo(suppliedMetricThreshold));
    //        Assert.That(putMetricThreshold[0].IsInclusive, Is.EqualTo(suppliedIsInclusive));
    //    }

    //    [Test]
    //    public void Should_update_if_the_record_does_not_exist()
    //    {
    //        bool created;   // TODO: GKM - Test should be reviewed
    //        var service = new MetricThresholdService(LocalEducationAgencyMetricThresholdRepository.Object);
    //        // TODO: GKM - Don't change the state of the scenario outside of the ExecuteTest method.  The tests should just be asserting.  Consider refactoring to abstract base class, with derived classes.
    //        PutResponse = service.Put(MetricThresholdPutRequest.Create(suppliedLocalEducationAgencyId, suppliedMetricId, updatedThresholdValue, suppliedIsInclusive), out created);
    //        Assert.That(putMetricThreshold.Count(), Is.EqualTo(2));
    //        Assert.That(putMetricThreshold[1].LocalEducationAgencyMetricThresholdId, Is.EqualTo(1));
    //        Assert.That(putMetricThreshold[1].LocalEducationAgencyId, Is.EqualTo(suppliedLocalEducationAgencyId));
    //        Assert.That(putMetricThreshold[1].MetricId, Is.EqualTo(suppliedMetricId));
    //        Assert.That(putMetricThreshold[1].Threshold, Is.EqualTo(updatedThresholdValue));
    //        Assert.That(putMetricThreshold[1].IsInclusive, Is.EqualTo(suppliedIsInclusive));
    //    }
    //}
     public class When_calling_metric_threshold_service_delete_method : MetricThresholdServiceFixtureBase
     {
         private readonly List<Expression<Func<LocalEducationAgencyMetricThreshold, bool>>> deletedMetricThreshold = new List<Expression<Func<LocalEducationAgencyMetricThreshold, bool>>>();

         protected override void EstablishContext()
         {
             base.EstablishContext();
             LocalEducationAgencyMetricThresholdRepository.Setup(x => x.Delete(It.IsAny<Expression<Func<LocalEducationAgencyMetricThreshold, bool>>>())).Callback<Expression<Func<LocalEducationAgencyMetricThreshold, bool>>>(x => deletedMetricThreshold.Add(x));

         }
         protected override void ExecuteTest()
         {
             var service = new MetricThresholdService(LocalEducationAgencyMetricThresholdRepository.Object);
             service.Delete(MetricThresholdDeleteRequest.Create(suppliedLocalEducationAgencyId, suppliedMetricId));
         }

         //[Test]
         //public virtual void Should_create_model_correctly()
         //{
         //    Assert.That(DeleteResponse, Is.Not.Null);
         //}

         //[Test]
         //public void Should_have_all_values_set()
         //{
         //    DeleteResponse.EnsureNoDefaultValues();
         //}

         [Test]
         public void Should_delete_correct_metric_threshold()
         {
             Assert.That(deletedMetricThreshold.Count(), Is.EqualTo(1));
         }
     }
}


