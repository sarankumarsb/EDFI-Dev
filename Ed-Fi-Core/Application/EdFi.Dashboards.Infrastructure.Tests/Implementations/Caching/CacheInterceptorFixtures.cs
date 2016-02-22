using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Castle.DynamicProxy;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Infrastructure.Tests.Implementations.Caching
{
    public class FakeService
    {
        public void VoidMethod() {}

        [NoCache]
        public int NoCacheMethod() { return 0; }

        public int IntMethod() { return 0; }

        public object ObjectMethod() { return default(object); }

        [CacheInitializer(typeof(FakeInitializer))]
        public object ObjectMethodWithCacheInitializer() { return default(object); }

        [CacheInitializer(typeof(StringBuilder))]
        public object ObjectMethodWithInvalidCacheInitializer() { return default(object); }

        [CacheBehavior(copyOnSet: false, copyOnGet: false, absoluteExpirationInSecondsPastMidnight: 60 * 60 * 2)] 
        public object ObjectMethodWithAbsoluteExpiry() { return default(object); }

        [CacheBehavior(copyOnSet: false, copyOnGet: false)]
        public object ObjectMethodWithNoCopy() { return default(object); }
    }

    public class FakeInvocation : IInvocation
    {
        public FakeInvocation(MethodInfo methodInvocationTarget, MethodInfo method, object[] arguments)
        {
            this.methodInvocationTarget = methodInvocationTarget;
            this.method = method;
            this.arguments = arguments;
        }

        public void SetArgumentValue(int index, object value)
        {
            throw new NotImplementedException();
        }

        public object GetArgumentValue(int index)
        {
            throw new NotImplementedException();
        }

        public MethodInfo GetConcreteMethod()
        {
            throw new NotImplementedException();
        }

        public MethodInfo GetConcreteMethodInvocationTarget()
        {
            throw new NotImplementedException();
        }

        public void Proceed()
        {
            throw new NotImplementedException();
        }

        public object Proxy
        {
            get { throw new NotImplementedException(); }
        }

        public object InvocationTarget
        {
            get { throw new NotImplementedException(); }
        }

        public Type TargetType
        {
            get { throw new NotImplementedException(); }
        }

        private object[] arguments;
        public object[] Arguments
        {
            get { return arguments; }
        }

        public Type[] GenericArguments
        {
            get { throw new NotImplementedException(); }
        }

        private MethodInfo method;
        public MethodInfo Method
        {
            get { return method; }
        }

        private MethodInfo methodInvocationTarget;
        public MethodInfo MethodInvocationTarget
        {
            get { return methodInvocationTarget; }
        }

        public object ReturnValue { get; set; }
    }

    public class FakeInitializer : ICacheInitializer
    {
        public ICacheProvider CacheProvider;
        public MethodInfo MethodInvocationTarget;
        public object[] Arguments;

        public void InitializeCacheValues(ICacheProvider cacheProvider, MethodInfo methodInvocationTarget, object[] arguments)
        {
            this.CacheProvider = cacheProvider;
            this.MethodInvocationTarget = methodInvocationTarget;
            this.Arguments = arguments;
        }
    }

    public abstract class When_intercepting_a_method : TestFixtureBase 
    {
        protected NameValueCollectionConfigValueProvider configValueProvider;
        protected IInvocation invocation;
        protected ICacheProvider cacheProvider;
        protected ICacheKeyGenerator cacheKeyGenerator;
        protected StageResult actualStageResult;
        protected string suppliedCacheKey;
        protected object actualReturnValue = new object(); // Non-default
        protected IServiceLocator serviceLocator;

        protected void MockInvocation(string methodName)
        {
            MockInvocation(methodName, null);
        }

        protected void MockInvocation(string methodName, object[] arguments)
        {
            var targetMethod = typeof(FakeService).GetMethod(methodName);

            invocation = mocks.StrictMock<IInvocation>();

            SetupResult.For(invocation.MethodInvocationTarget)
                .Return(targetMethod);

            SetupResult.For(invocation.Method)
                .Return(targetMethod);

            // Convert null to empty array
            if (arguments == null)
                arguments = new object[0];

            SetupResult.For(invocation.Arguments)
                .Return(arguments);

            invocation.ReturnValue = null;
            LastCall.Repeat.Any();
            LastCall.Constraints(
                new ActionConstraint<object>(x => actualReturnValue = x));
        }

        protected virtual void MockConfigValueProvider()
        {
            configValueProvider = new NameValueCollectionConfigValueProvider();
            configValueProvider.Values["CacheInterceptor.Enabled"] = true.ToString();
        }

        protected void MockCacheKeyGenerator(string cacheKey)
        {
            suppliedCacheKey = cacheKey;

            cacheKeyGenerator = mocks.StrictMock<ICacheKeyGenerator>();
            Expect.Call(cacheKeyGenerator.GenerateCacheKey(null, null))
                .IgnoreArguments()
                .Return(suppliedCacheKey);
        }

        protected virtual void MockCacheProvider(string cacheKey, object cacheEntry, bool tryGetCachedObjectResult)
        {
            if (cacheProvider == null)
                cacheProvider = mocks.StrictMock<ICacheProvider>();

            object cacheEntryVariable;
            Expect.Call(cacheProvider.TryGetCachedObject(cacheKey, out cacheEntryVariable))
                .OutRef(new[] {cacheEntry})
                .Return(tryGetCachedObjectResult);
        }

        protected CacheCallStackContext GetCacheCallStackContext()
        {
            return CallContext.GetData(CacheInterceptor.CallStackContextKey) as CacheCallStackContext;
        }
    }

    public abstract class When_intercepting_a_method_before_execution 
        : When_intercepting_a_method
    {
        protected ISerializer binaryFormatterSerializer = new BinaryFormatterSerializer();

        protected override void BeforeExecuteTest()
        {
            CallContext.FreeNamedDataSlot(CacheInterceptor.CallStackContextKey);
        }

        protected override void ExecuteTest()
        {
            var interceptor = new CacheInterceptor(new[] { cacheProvider }, cacheKeyGenerator, configValueProvider, serviceLocator, binaryFormatterSerializer);
            actualStageResult = interceptor.BeforeExecution(invocation, true);
        }
    }

    public abstract class When_intercepting_a_method_after_execution 
        : When_intercepting_a_method
    {
        protected ISerializer serializer = new BinaryFormatterSerializer();

        protected override void BeforeExecuteTest()
        {
            var cacheCallStackContext = new CacheCallStackContext();

            // Reinitialize the context
            CallContext.FreeNamedDataSlot(CacheInterceptor.CallStackContextKey);
            CallContext.SetData(CacheInterceptor.CallStackContextKey, cacheCallStackContext);

            // Simulate invocation already entered the cache interceptor
            cacheCallStackContext.Enter(invocation);

            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.GreaterThan(0));
        }

        protected override void ExecuteTest()
        {
            var interceptor = new CacheInterceptor(new[] {cacheProvider}, cacheKeyGenerator, configValueProvider, serviceLocator, serializer);
            interceptor.AfterExecution(invocation, true, GetSuppliedStageResult());
        }

        protected virtual StageResult GetSuppliedStageResult()
        {
            return new StageResult();
        }
    }

    
    public class When_intercepting_a_method_before_execution_returning_void 
        : When_intercepting_a_method_before_execution
    {
        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("VoidMethod");

            cacheProvider = mocks.StrictMock<ICacheProvider>();
            cacheKeyGenerator = mocks.StrictMock<ICacheKeyGenerator>();
        }

        [Test]
        public void Should_allow_call_to_proceed()
        {
            Assert.That(actualStageResult.Proceed);
        }

        [Test]
        public void Should_increase_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(1));
        }
    }

    
    public class When_intercepting_a_method_after_execution_returning_void
        : When_intercepting_a_method_after_execution
    {
        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("VoidMethod");

            cacheProvider = mocks.StrictMock<ICacheProvider>();
            cacheKeyGenerator = mocks.StrictMock<ICacheKeyGenerator>();
        }

        [Test]
        public void Should_decrease_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(0));
        }
    }
    
    public class When_intercepting_a_method_before_execution_marked_with_NoCache_attribute 
        : When_intercepting_a_method_before_execution
    {
        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("NoCacheMethod");

            cacheProvider = mocks.StrictMock<ICacheProvider>();
            cacheKeyGenerator = mocks.StrictMock<ICacheKeyGenerator>();
        }

        [Test]
        public void Should_allow_call_to_proceed()
        {
            Assert.That(actualStageResult.Proceed);
        }

        [Test]
        public void Should_increase_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(1));
        }
    }

    
    public class When_intercepting_a_method_after_execution_marked_with_NoCache_attribute
        : When_intercepting_a_method_after_execution
    {
        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("NoCacheMethod");

            cacheProvider = mocks.StrictMock<ICacheProvider>();
            cacheKeyGenerator = mocks.StrictMock<ICacheKeyGenerator>();
        }

        [Test]
        public void Should_decrease_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(0));
        }
    }
    
    public abstract class When_intercepting_a_method_before_execution_for_which_a_cache_key_cannot_be_generated 
        : When_intercepting_a_method_before_execution
    {
        protected abstract string GetSuppliedCacheKey();

        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("IntMethod");
            MockCacheKeyGenerator(GetSuppliedCacheKey());

            cacheProvider = mocks.StrictMock<ICacheProvider>();
        }

        [Test]
        public void Should_allow_call_to_proceed()
        {
            Assert.That(actualStageResult.Proceed);
        }

        [Test]
        public void Should_increase_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(1));
        }

        [Test]
        public void Should_save_cache_key_in_StageResult()
        {
            Assert.That(actualStageResult.State, Is.EqualTo(suppliedCacheKey));
        }
    }
    
    public class When_intercepting_a_method_before_execution_for_which_a_cache_key_is_null 
        : When_intercepting_a_method_before_execution_for_which_a_cache_key_cannot_be_generated
    {
        protected override string GetSuppliedCacheKey()
        {
            return null;
        }
    }
  
    public class When_intercepting_a_method_before_execution_for_which_a_cache_key_is_empty
        : When_intercepting_a_method_before_execution_for_which_a_cache_key_cannot_be_generated
    {
        protected override string GetSuppliedCacheKey()
        {
            return string.Empty;
        }
    }
 
    public class When_intercepting_a_method_before_execution_which_is_not_yet_cached 
        : When_intercepting_a_method_before_execution
    {
        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("IntMethod");
            MockCacheKeyGenerator("abc");

            MockCacheProvider("abc", null, false);
        }

        [Test]
        public void Should_allow_call_to_proceed()
        {
            Assert.That(actualStageResult.Proceed);
        }

        [Test]
        public void Should_increase_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(1));
        }
    }

    public abstract class When_intercepting_a_method_before_execution_which_is_not_cached_and_has_a_CacheInitializer 
        : When_intercepting_a_method_before_execution
    {
        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation(GetServiceMethodName());
            MockCacheKeyGenerator("abc");

            MockCacheProvider("abc", null, false);

            InitializeIoC();
        }

        protected abstract string GetServiceMethodName();

        protected abstract void InitializeIoC();
    }

    public class When_intercepting_a_method_before_execution_which_is_not_cached_and_has_a_valid_CacheInitializer
        : When_intercepting_a_method_before_execution_which_is_not_cached_and_has_a_CacheInitializer
    {
        private FakeInitializer fakeInitializer = new FakeInitializer();

        protected override string GetServiceMethodName()
        {
            return "ObjectMethodWithCacheInitializer";
        }

        protected override void InitializeIoC()
        {
            serviceLocator = mocks.StrictMock<IServiceLocator>();

            SetupResult.For(serviceLocator.Resolve(typeof(FakeInitializer)))
                .Return(fakeInitializer);
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            // Add another mocked invocation of the cache provider since an attempt 
            // should be made to fetch from cache after initializer is done.
            MockCacheProvider("abc", new object(), true);
        }

        [Test]
        public void Should_invoke_cache_initializer_to_initialize_cache_values()
        {
            // Value will be captured if the initializer is invoked
            Assert.That(fakeInitializer.MethodInvocationTarget, Is.Not.Null);
        }

        [Test]
        public void Should_not_allow_call_to_proceed()
        {
            Assert.That(actualStageResult.Proceed, Is.False);
        }

        [Test]
        public void Should_not_increase_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(0));
        }
    }

    public class When_intercepting_a_method_before_execution_which_is_not_cached_and_has_a_CacheInitializer_that_does_not_implement_ICacheInitializer
        : When_intercepting_a_method_before_execution_which_is_not_cached_and_has_a_CacheInitializer
    {
        private StringBuilder fakeInvalidInitializer = new StringBuilder();
        private Exception actualException;

        protected override string GetServiceMethodName()
        {
            return "ObjectMethodWithInvalidCacheInitializer";
        }

        protected override void InitializeIoC()
        {
            serviceLocator = mocks.StrictMock<IServiceLocator>();

            SetupResult.For(serviceLocator.Resolve(typeof(StringBuilder)))
                .Return(fakeInvalidInitializer);
        }

        protected override void ExecuteTest()
        {
            try
            {
                base.ExecuteTest();
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
        }

        [Test]
        public void Should_throw_an_exception_due_to_invalid_initializer_type()
        {
            Assert.That(actualException, Is.TypeOf<ArgumentException>());
        }
    }
 
    public class When_intercepting_a_method_before_execution_which_has_a_null_value_cached 
        : When_intercepting_a_method_before_execution
    {
        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("ObjectMethod");
            MockCacheKeyGenerator("abc");

            MockCacheProvider("abc", null, true);
        }

        [Test]
        public void Should_prevent_call_from_proceeding()
        {
            Assert.That(actualStageResult.Proceed, Is.False);
        }

        [Test]
        public void Should_set_return_value_to_null()
        {
            Assert.That(actualReturnValue, Is.Null);
        }

        [Test]
        public void Should_not_increase_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(0));
        }
    }

    public class When_intercepting_a_method_before_execution_which_has_a_value_cached_without_serialization 
        : When_intercepting_a_method_before_execution
    {
        private object suppliedCacheValue;

        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("ObjectMethod");
            MockCacheKeyGenerator("abc");

            suppliedCacheValue = new object();
            MockCacheProvider("abc", suppliedCacheValue, true);
        }

        [Test]
        public void Should_prevent_call_from_proceeding()
        {
            Assert.That(actualStageResult.Proceed, Is.False);
        }

        [Test]
        public void Should_set_return_value_to_cached_value()
        {
            Assert.That(actualReturnValue, Is.EqualTo(suppliedCacheValue));
        }

        [Test]
        public void Should_not_increase_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(0));
        }
    }

    public class When_intercepting_a_method_before_execution_which_has_a_value_cached_with_serialization 
        : When_intercepting_a_method_before_execution
    {
        private FakeModel suppliedCacheValue;

        [Serializable]
        private class FakeModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("ObjectMethod");
            MockCacheKeyGenerator("abc");

            suppliedCacheValue = new FakeModel { Id = 1, Name = "One" };

            MockCacheProvider("abc", binaryFormatterSerializer.Serialize(suppliedCacheValue), true);
        }

        [Test]
        public void Should_prevent_call_from_proceeding()
        {
            Assert.That(actualStageResult.Proceed, Is.False);
        }

        [Test]
        public void Should_set_return_value_to_cached_value()
        {
            // Perform a deep property by property comparison of the two objects
            actualReturnValue.Should_have_same_values_as(suppliedCacheValue);
        }

        [Test]
        public void Should_not_increase_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(0));
        }
    }

    public class When_intercepting_a_method_after_execution_with_caching_disabled 
        : When_intercepting_a_method_after_execution
    {
        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("VoidMethod");

            cacheProvider = mocks.StrictMock<ICacheProvider>();
            cacheKeyGenerator = mocks.StrictMock<ICacheKeyGenerator>();
        }
        
        protected override void MockConfigValueProvider()
        {
            base.MockConfigValueProvider();

            // Disable caching through a config setting
            configValueProvider.Values["CacheInterceptor.Enabled"] = false.ToString();
        }

        [Test]
        public void Should_decrease_call_stack_depth()
        {
            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.EqualTo(0));
        }
    }

    public abstract class When_intercepting_a_method_with_multiple_cache_providers : TestFixtureBase
    {
        protected NameValueCollectionConfigValueProvider configValueProvider;
        protected IInvocation invocation;
        protected ICacheProvider[] cacheProviders;
        protected ICacheKeyGenerator cacheKeyGenerator;
        protected StageResult actualStageResult;
        protected string suppliedCacheKey;
        protected object actualReturnValue = new object(); // Non-default
        protected IServiceLocator serviceLocator;

        protected void MockInvocation(string methodName)
        {
            MockInvocation(methodName, null);
        }

        protected void MockInvocation(string methodName, object[] arguments)
        {
            var targetMethod = typeof(FakeService).GetMethod(methodName);

            // Convert null to empty array
            if (arguments == null)
                arguments = new object[0];

            invocation = new FakeInvocation(targetMethod, targetMethod, arguments);
        }

        protected virtual void MockConfigValueProvider()
        {
            configValueProvider = new NameValueCollectionConfigValueProvider();
            configValueProvider.Values["CacheInterceptor.Enabled"] = true.ToString();
        }

        protected void MockCacheKeyGenerator(string cacheKey)
        {
            suppliedCacheKey = cacheKey;

            cacheKeyGenerator = mocks.StrictMock<ICacheKeyGenerator>();
            Expect.Call(cacheKeyGenerator.GenerateCacheKey(null, null))
                .IgnoreArguments()
                .Return(suppliedCacheKey);
        }

        protected virtual ICacheProvider MockCacheProvider()
        {
            var cacheProvider = mocks.StrictMock<ICacheProvider>();
            return cacheProvider;
        }

        protected virtual ICacheProvider MockCacheProvider(string cacheKey, object cacheEntry, bool tryGetCachedObjectResult)
        {
            var cacheProvider = MockCacheProvider();

            object cacheEntryVariable;
            Expect.Call(cacheProvider.TryGetCachedObject(cacheKey, out cacheEntryVariable))
                .OutRef(new[] { cacheEntry })
                .Return(tryGetCachedObjectResult);
            return cacheProvider;
        }

        protected CacheCallStackContext GetCacheCallStackContext()
        {
            return CallContext.GetData(CacheInterceptor.CallStackContextKey) as CacheCallStackContext;
        }
    }

    public abstract class When_intercepting_a_method_before_execution_with_multiple_cache_providers
        : When_intercepting_a_method_with_multiple_cache_providers
    {
        protected ISerializer serializer = new BinaryFormatterSerializer();

        protected override void BeforeExecuteTest()
        {
            CallContext.FreeNamedDataSlot(CacheInterceptor.CallStackContextKey);
        }

        protected override void ExecuteTest()
        {
            var interceptor = new CacheInterceptor(cacheProviders, cacheKeyGenerator, configValueProvider, serviceLocator, serializer);
            actualStageResult = interceptor.BeforeExecution(invocation, true);
        }
    }

    public abstract class When_intercepting_a_method_after_execution_with_multiple_cache_providers
    : When_intercepting_a_method_with_multiple_cache_providers
    {
        protected ISerializer serializer = new BinaryFormatterSerializer();

        protected override void BeforeExecuteTest()
        {
            var cacheCallStackContext = new CacheCallStackContext();

            // Reinitialize the context
            CallContext.FreeNamedDataSlot(CacheInterceptor.CallStackContextKey);
            CallContext.SetData(CacheInterceptor.CallStackContextKey, cacheCallStackContext);

            // Simulate invocation already entered the cache interceptor
            cacheCallStackContext.Enter(invocation);

            Assert.That(GetCacheCallStackContext().CurrentDepth, Is.GreaterThan(0));
        }

        protected override void ExecuteTest()
        {
            var interceptor = new CacheInterceptor(cacheProviders, cacheKeyGenerator, configValueProvider, serviceLocator, serializer);
            interceptor.AfterExecution(invocation, true, GetSuppliedStageResult());
        }

        protected virtual StageResult GetSuppliedStageResult()
        {
            var stageResult = new StageResult();
            stageResult.Proceed = true;
            return stageResult;
        }
    }

    public class When_intercepting_a_method_before_execution_which_is_not_yet_cached_by_any_cache_provider
        : When_intercepting_a_method_before_execution_with_multiple_cache_providers
    {
        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("IntMethod");
            MockCacheKeyGenerator("abc");

            cacheProviders = new[] {MockCacheProvider("abc", null, false), MockCacheProvider("abc", null, false)};
        }

        [Test]
        public void Should_allow_call_to_proceed()
        {
            Assert.That(actualStageResult.Proceed);
        }
    }

    public class When_intercepting_a_method_before_execution_which_is_cached_by_a_secondary_cache_provider
        : When_intercepting_a_method_before_execution_with_multiple_cache_providers
    {
        private object suppliedCacheValue;
        private ICacheProvider defaultCacheProvider;
        private dynamic constraintBag;

        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("ObjectMethod");
            MockCacheKeyGenerator("abc");
            suppliedCacheValue = new object();

            //Create Default cache provider without the cache value.
            defaultCacheProvider = MockCacheProvider("abc", null, false);
            //Populate constraintBag with with values passed to insert.
            defaultCacheProvider.Expect(x => x.Insert(null, null, DateTime.Now, TimeSpan.Parse("1:00")));
            constraintBag = new ExpandoObject();
            LastCall.Constraints(
                new ActionConstraint<string>(x => constraintBag.key = x),
                new ActionConstraint<object>(x => constraintBag.value = x),
                new ActionConstraint<DateTime>(x => constraintBag.absoluteExpiration = x),
                new ActionConstraint<TimeSpan>(x => constraintBag.slidingExpiration = x)
                );
            //defaultCacheProvider.Expect(x => x.Insert(Arg<string>.Matches(arg => arg == "abc"), Arg<object>.Matches(arg => arg.Equals(suppliedCacheValue)), Arg<DateTime>.Is.Anything,Arg<TimeSpan>.Is.Anything));
            cacheProviders = new[] {defaultCacheProvider, MockCacheProvider("abc", suppliedCacheValue, true)};
        }

        [Test]
        public void Should_prevent_call_from_proceeding()
        {
            Assert.That(actualStageResult.Proceed, Is.False);
        }

        [Test]
        public void Should_set_return_value_to_supplied_value()
        {
            Assert.That(invocation.ReturnValue, Is.EqualTo(suppliedCacheValue));
        }

        [Test]
        public void Should_set_default_cache_provider_value_to_supplied_value_for_the_key()
        {
            Assert.That(constraintBag.key, Is.EqualTo("abc"));
            Assert.That(constraintBag.value, Is.EqualTo(suppliedCacheValue));
        }

    }

    public class When_intercepting_a_method_after_execution_which_is_not_yet_cached_by_any_cache_provider_and_has_absolute_expiry
        : When_intercepting_a_method_after_execution_with_multiple_cache_providers
    {
        private object suppliedCacheValue;
        private ICacheProvider defaultCacheProvider;
        private dynamic constraintBag;

        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("ObjectMethodWithAbsoluteExpiry");
            MockCacheKeyGenerator("abc");

            suppliedCacheValue = new object();
            invocation.ReturnValue = suppliedCacheValue;

            //Create Default cache provider without the cache value.
            defaultCacheProvider = MockCacheProvider();
            //Populate constraintBag with with values passed to insert.
            defaultCacheProvider.Expect(x => x.Insert(null, null, DateTime.Now, TimeSpan.Parse("1:00")));
            constraintBag = new ExpandoObject();
            LastCall.Constraints(
                new ActionConstraint<string>(x => constraintBag.key = x),
                new ActionConstraint<object>(x => constraintBag.value = x),
                new ActionConstraint<DateTime>(x => constraintBag.absoluteExpiration = x),
                new ActionConstraint<TimeSpan>(x => constraintBag.slidingExpiration = x)
                );
            
            cacheProviders = new[] { defaultCacheProvider, MockCacheProvider() };
        }

        [Test]
        public void Should_set_default_cache_provider_value_to_supplied_value_for_the_key()
        {
            Assert.That(constraintBag.key, Is.EqualTo("abc"));
            Assert.That(constraintBag.value, Is.EqualTo(suppliedCacheValue));
        }
    }

    public class When_intercepting_a_method_after_execution_with_no_copy_which_is_not_yet_cached_by_any_cache_provider
        : When_intercepting_a_method_after_execution_with_multiple_cache_providers
    {
        private object suppliedCacheValue;
        private ICacheProvider defaultCacheProvider;
        private dynamic constraintBag;
        private ICacheProvider secondaryCacheProvider;
        private dynamic secondaryConstraintBag;

        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("ObjectMethodWithNoCopy");
            MockCacheKeyGenerator("abc");

            suppliedCacheValue = new object();
            invocation.ReturnValue = suppliedCacheValue;

            //Create Default cache provider without the cache value.
            defaultCacheProvider = MockCacheProvider();
            //Populate constraintBag with with values passed to insert.
            defaultCacheProvider.Expect(x => x.Insert(null, null, DateTime.Now, TimeSpan.Parse("1:00")));
            constraintBag = new ExpandoObject();
            LastCall.Constraints(
                new ActionConstraint<string>(x => constraintBag.key = x),
                new ActionConstraint<object>(x => constraintBag.value = x),
                new ActionConstraint<DateTime>(x => constraintBag.absoluteExpiration = x),
                new ActionConstraint<TimeSpan>(x => constraintBag.slidingExpiration = x)
                );

            secondaryCacheProvider = MockCacheProvider();

            //Populate constraintBag with with values passed to insert.
            secondaryCacheProvider.Expect(x => x.Insert(null, null, DateTime.Now, TimeSpan.Parse("1:00")));
            secondaryConstraintBag = new ExpandoObject();
            LastCall.Constraints(
                new ActionConstraint<string>(x => secondaryConstraintBag.key = x),
                new ActionConstraint<object>(x => secondaryConstraintBag.value = x),
                new ActionConstraint<DateTime>(x => secondaryConstraintBag.absoluteExpiration = x),
                new ActionConstraint<TimeSpan>(x => secondaryConstraintBag.slidingExpiration = x)
                );

            cacheProviders = new[] { defaultCacheProvider, secondaryCacheProvider };
        }

        [Test]
        public void Should_set_default_cache_provider_value_to_supplied_value_for_the_key()
        {
            Assert.That(constraintBag.key, Is.EqualTo("abc"));
            Assert.That(constraintBag.value, Is.EqualTo(suppliedCacheValue));
        }

        [Test]
        public void Should_set_secondary_cache_provider_value_to_supplied_value_for_the_key()
        {
            Assert.That(secondaryConstraintBag.key, Is.EqualTo("abc"));
            Assert.That(secondaryConstraintBag.value, Is.EqualTo(suppliedCacheValue));
        }

        [Test]
        public void Should_set_cache_providers_to_have_equal_values_and_keys()
        {
            Assert.That(secondaryConstraintBag.key, Is.EqualTo(constraintBag.key));
            Assert.That(secondaryConstraintBag.value, Is.EqualTo(constraintBag.value));
        }
    }

    public class When_intercepting_a_method_after_execution_which_is_not_yet_cached_by_any_cache_provider
     : When_intercepting_a_method_after_execution_with_multiple_cache_providers
    {
        private ICacheProvider defaultCacheProvider;
        private dynamic constraintBag;
        private ICacheProvider secondaryCacheProvider;
        private dynamic secondaryConstraintBag;

        private FakeModel suppliedCacheValue;

        [Serializable]
        private class FakeModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


        protected override void EstablishContext()
        {
            MockConfigValueProvider();
            MockInvocation("ObjectMethod");
            MockCacheKeyGenerator("abc");
            suppliedCacheValue = new FakeModel { Id = 1, Name = "One" };

            invocation.ReturnValue = suppliedCacheValue;

            //Create Default cache provider without the cache value.
            defaultCacheProvider = MockCacheProvider();
            //Populate constraintBag with with values passed to insert.
            defaultCacheProvider.Expect(x => x.Insert(null, null, DateTime.Now, TimeSpan.Parse("1:00")));
            constraintBag = new ExpandoObject();
            LastCall.Constraints(
                new ActionConstraint<string>(x => constraintBag.key = x),
                new ActionConstraint<object>(x => constraintBag.value = x),
                new ActionConstraint<DateTime>(x => constraintBag.absoluteExpiration = x),
                new ActionConstraint<TimeSpan>(x => constraintBag.slidingExpiration = x)
                );

            secondaryCacheProvider = MockCacheProvider();

            //Populate constraintBag with with values passed to insert.
            secondaryCacheProvider.Expect(x => x.Insert(null, null, DateTime.Now, TimeSpan.Parse("1:00")));
            secondaryConstraintBag = new ExpandoObject();
            LastCall.Constraints(
                new ActionConstraint<string>(x => secondaryConstraintBag.key = x),
                new ActionConstraint<object>(x => secondaryConstraintBag.value = x),
                new ActionConstraint<DateTime>(x => secondaryConstraintBag.absoluteExpiration = x),
                new ActionConstraint<TimeSpan>(x => secondaryConstraintBag.slidingExpiration = x)
                );

            cacheProviders = new[] { defaultCacheProvider, secondaryCacheProvider };
        }

        [Test]
        public void Should_set_default_cache_provider_value_to_supplied_value_for_the_key()
        {
            Assert.That(constraintBag.key, Is.EqualTo("abc"));
            Assert.That(constraintBag.value, Is.EqualTo(serializer.Serialize(suppliedCacheValue)));
        }

        [Test]
        public void Should_set_secondary_cache_provider_value_to_supplied_value_for_the_key()
        {
            Assert.That(secondaryConstraintBag.key, Is.EqualTo("abc"));
            Assert.That(secondaryConstraintBag.value, Is.EqualTo(serializer.Serialize(suppliedCacheValue)));
        }

        [Test]
        public void Should_set_cache_providers_to_have_equal_values_and_keys()
        {
            Assert.That(secondaryConstraintBag.key, Is.EqualTo(constraintBag.key));
            Assert.That(secondaryConstraintBag.value, Is.EqualTo(constraintBag.value));
        }
    }
}
