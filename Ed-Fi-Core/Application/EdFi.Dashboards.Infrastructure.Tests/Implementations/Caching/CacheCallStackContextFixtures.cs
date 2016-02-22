using System.Collections.Generic;
using Castle.DynamicProxy;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Infrastructure.Tests.Implementations.Caching
{
    public class FakeService1
    {
        public void Method() {}
    }

    public class FakeService2
    {
        public void Method() { }
    }

    public class FakeService3
    {
        public void Method() { }
    }

    public class FakeService4
    {
        public void Method() { }
    }

    public class FakeInterceptor : FilteringInterceptorBase
    {
        public void MarkAsProcessed()
        {
            this.MarkCurrentResultProcessedForModification();
        }
    }

    public abstract class When_services_are_invoked : TestFixtureBase 
    {
        protected object[] fakeServices = 
            new object[]
                {
                    new FakeService1(), 
                    new FakeService2(),
                    new FakeService3(),
                    new FakeService4(),
                };

        protected FakeInterceptor fakeInterceptor = new FakeInterceptor();

        protected List<IInvocation> invocations = new List<IInvocation>();
        protected CacheCallStackContext callContext;

        protected void MockInvocations(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var mock = mocks.StrictMock<IInvocation>();
                
                // Get the corresponding fake service
                SetupResult.For(mock.InvocationTarget)
                    .Return(fakeServices[i]);
                
                // Get the MethodInfo for the fake service method
                SetupResult.For(mock.Method)
                    .Return(fakeServices[i].GetType().GetMethod("Method"));

                invocations.Add(mock);
            }
        }

        protected virtual LazyCacheAttributes GetCacheAttributes()
        {
            return new LazyCacheAttributes(new object[0]);
        }
    }

    [TestFixture]
    public class When_one_service_method_is_invoked 
        : When_services_are_invoked
    {
        protected override void EstablishContext()
        {
            MockInvocations(1);
        }

        protected override void ExecuteTest()
        {
            callContext = new CacheCallStackContext();
            callContext.Enter(invocations[0]);
        }

        [Test]
        public void Should_be_able_to_cache_the_result()
        {
            Assert.That(callContext.IsResultCacheable(invocations[0], GetCacheAttributes()));
        }
    }

    [TestFixture]
    public class When_a_service_is_invoked_that_invokes_a_second_service_and_the_result_IS_NOT_processed_for_modification 
        : When_services_are_invoked
    {
        protected override void EstablishContext()
        {
            MockInvocations(2);
        }

        protected override void ExecuteTest()
        {
            callContext = new CacheCallStackContext();
            callContext.Enter(invocations[0]);
            callContext.Enter(invocations[1]);
            callContext.Exit();
        }

        [Test]
        public void Should_still_be_able_to_cache_the_outer_service_method_result()
        {
            Assert.That(callContext.IsResultCacheable(invocations[0], GetCacheAttributes()));
        }
    }

    public abstract class When_a_service_is_invoked_that_invokes_a_second_service_and_the_result_IS_processed_for_modification 
        : When_services_are_invoked
    {
        protected override void EstablishContext()
        {
            MockInvocations(2);
        }

        protected override void ExecuteTest()
        {
            callContext = new CacheCallStackContext();
            callContext.Enter(invocations[0]); // Outer service ENTER
            callContext.Enter(invocations[1]); // Inner service ENTER
            callContext.Exit();                // Inner service EXIT
            fakeInterceptor.MarkAsProcessed(); // Result modified by upstream interceptor
        }

        [Test]
        public void Should_be_able_to_cache_the_inner_service_method_result()
        {
            Assert.That(callContext.IsResultCacheable(invocations[1], GetCacheAttributes()));
        }
    }

    [TestFixture]
    public class When_a_service_NOT_marked_as_always_safe_to_be_cached_is_invoked_that_invokes_a_second_service_and_the_result_IS_processed_for_modification 
        : When_a_service_is_invoked_that_invokes_a_second_service_and_the_result_IS_processed_for_modification
    {
        [Test]
        public void Should_not_be_able_to_cache_the_outer_service_method_result()
        {
            Assert.That(callContext.IsResultCacheable(invocations[0], GetCacheAttributes()), Is.False);
        }
    }

    [TestFixture]
    public class When_a_service_marked_as_always_safe_to_be_cached_is_invoked_that_invokes_a_second_service_and_the_result_IS_processed_for_modification 
        : When_a_service_is_invoked_that_invokes_a_second_service_and_the_result_IS_processed_for_modification
    {
        protected override LazyCacheAttributes GetCacheAttributes()
        {
            return new LazyCacheAttributes(
                new object[]
                    {
                        new AlwaysSafeToCacheAttribute(),
                    });
        }

        [Test]
        public void Should_be_able_to_cache_the_outer_service_method_result()
        {
            Assert.That(callContext.IsResultCacheable(invocations[0], GetCacheAttributes()));
        }
    }

    [TestFixture]
    public class When_a_service_call_stack_completes_where_a_result_was_processed_for_modification 
        : When_services_are_invoked
    {
        protected override void EstablishContext()
        {
            MockInvocations(4);
        }

        protected override void ExecuteTest()
        {
            callContext = new CacheCallStackContext();

            // First service call context
            callContext.Enter(invocations[0]); // Outer service ENTER
            callContext.Enter(invocations[1]); // Inner service ENTER
            callContext.Exit();                // Inner service EXIT
            fakeInterceptor.MarkAsProcessed(); // Result modified by upstream interceptor
            callContext.Exit();                // Outer service EXIT

            // Starting a second service call
            callContext.Enter(invocations[2]); // Outer service ENTER
            callContext.Enter(invocations[3]); // Inner service ENTER
            callContext.Exit();                // Inner service EXIT
        }

        [Test]
        public void Should_still_be_able_to_cache_the_results_of_the_second_outer_services_method()
        {
            Assert.That(callContext.IsResultCacheable(invocations[2], GetCacheAttributes()));
        }
    }
}
