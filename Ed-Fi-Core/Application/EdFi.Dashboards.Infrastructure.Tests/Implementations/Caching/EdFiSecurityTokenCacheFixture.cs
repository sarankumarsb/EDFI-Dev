using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Infrastructure.Tests.Implementations.Caching;
using EdFi.Dashboards.Testing;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace EdFi.Dashboards.Infrastructure.Tests.Implementations.Caching
{
    public abstract class When_using_session_for_security_token_cache : TestFixtureBase
    {
        protected HashtableSessionStateProvider sessionStateProvider;
        protected EdFiSecurityTokenCache securityTokenCache;
        protected SecurityToken suppliedSecurityToken;
        protected SecurityTokenCacheKey suppliedSecurityTokenCacheKey;
        protected Exception actualException;
        protected string suppliedSecurityTokenCacheKeyString;
        protected bool actualReturnBooleanValue;
        protected string alternateSecurityTokenCacheKey;
        protected Dictionary<string, object> sessionCollection;

        protected string GetTokenCacheKeyString(SecurityTokenCacheKey key)
        {
            return string.Format("{0}; {1}; {2}", key.ContextId, key.KeyGeneration, key.EndpointId);
        }

        protected override void EstablishContext()
        {
            alternateSecurityTokenCacheKey = "not a SecurityTokenCacheKey";
            sessionCollection = new Dictionary<string, object>();
            suppliedSecurityTokenCacheKey = new SecurityTokenCacheKey("endpointId", new System.Xml.UniqueId(), new System.Xml.UniqueId(), true);
            suppliedSecurityTokenCacheKeyString = GetTokenCacheKeyString(suppliedSecurityTokenCacheKey);
            
            
            sessionStateProvider = new HashtableSessionStateProvider();
            suppliedSecurityToken = mocks.StrictMock<SecurityToken>();
            securityTokenCache = new EdFiSecurityTokenCache(sessionStateProvider);
        }
    }

    [TestFixture]
    public class When_trying_to_add_an_entry_with_a_null_key : When_using_session_for_security_token_cache
    {
        protected override void ExecuteTest()
        {
            try
            {
                securityTokenCache.TryAddEntry(null, suppliedSecurityToken);

            }
            catch (Exception exception)
            {

                actualException = exception;
            }
        }

        [Test]
        public void Should_throw_an_argument_null_exception()
        {
            Assert.That(actualException, Is.TypeOf<ArgumentNullException>());
        }
    }

    [TestFixture]
    public class When_trying_to_add_an_entry_with_a_key_that_is_not_a_security_token_cache_key : When_using_session_for_security_token_cache
    {
        protected override void ExecuteTest()
        {
            try
            {
                securityTokenCache.TryAddEntry(alternateSecurityTokenCacheKey, suppliedSecurityToken);

            }
            catch (Exception exception)
            {

                actualException = exception;
            }
        }

        [Test]
        public void Should_throw_an_argument_exception()
        {
            Assert.That(actualException, Is.TypeOf<ArgumentException>());
        }
    }
}

[TestFixture]
public class When_adding_an_entry_with_a_valid_key : When_using_session_for_security_token_cache
{
    protected override void ExecuteTest()
    {
        try
        {
            actualReturnBooleanValue = securityTokenCache.TryAddEntry(suppliedSecurityTokenCacheKey, suppliedSecurityToken);

        }
        catch (Exception exception)
        {

            actualException = exception;
        }
    }

    [Test]
    public void Should_store_an_entry_in_the_session_provider_with_the_correct_key()
    {
        Assert.That(sessionStateProvider[suppliedSecurityTokenCacheKeyString], Is.EqualTo(suppliedSecurityToken));
    }

    [Test]
    public void Should_indicate_success()
    {
        Assert.That(actualReturnBooleanValue, Is.EqualTo(true));
    }
}

[TestFixture]
public class When_trying_to_retrieve_an_entry_with_a_null_key : When_using_session_for_security_token_cache
{
    protected override void ExecuteTest()
    {
        try
        {
            actualReturnBooleanValue = securityTokenCache.TryGetEntry(null, out suppliedSecurityToken);

        }
        catch (Exception exception)
        {

            actualException = exception;
        }
    }

    [Test]
    public void Should_throw_an_argument_null_exception()
    {
        Assert.That(actualException, Is.TypeOf<ArgumentNullException>());
    }
}

[TestFixture]
public class When_trying_to_retrieve_an_entry_with_a_key_that_is_not_a_security_token_cache_key : When_using_session_for_security_token_cache
{
    protected override void ExecuteTest()
    {
        try
        {
            actualReturnBooleanValue = securityTokenCache.TryGetEntry(alternateSecurityTokenCacheKey, out suppliedSecurityToken);

        }
        catch (Exception exception)
        {

            actualException = exception;
        }
    }

    [Test]
    public void Should_throw_an_argument_exception()
    {
        Assert.That(actualException, Is.TypeOf<ArgumentException>());
    }
}

[TestFixture]
public class When_trying_to_retrieve_an_entry_that_does_not_exist : When_using_session_for_security_token_cache
{
    protected override void ExecuteTest()
    {
        try
        {
            actualReturnBooleanValue = securityTokenCache.TryGetEntry(alternateSecurityTokenCacheKey, out suppliedSecurityToken);

        }
        catch (Exception exception)
        {

            actualException = exception;
        }
    }

    [Test]
    public void Should_return_an_outbound_null_value()
    {
        Assert.That(suppliedSecurityToken, Is.Null);
    }

    [Test]
    public void should_indicate_failure()
    {
        Assert.That(actualReturnBooleanValue, Is.EqualTo(false));
    }
}

[TestFixture]
public class When_trying_to_retrieve_an_entry_that_exists : When_using_session_for_security_token_cache
{
    protected override void ExecuteTest()
    {
        actualReturnBooleanValue = securityTokenCache.TryAddEntry(suppliedSecurityTokenCacheKey, suppliedSecurityToken);
        actualReturnBooleanValue = securityTokenCache.TryGetEntry(suppliedSecurityTokenCacheKey, out suppliedSecurityToken);
    }

    [Test]
    public void Should_return_an_outbound_value_obtained_from_session_provider()
    {
        Assert.That(suppliedSecurityToken, Is.EqualTo(sessionStateProvider[suppliedSecurityTokenCacheKeyString]));
    }

    [Test]
    public void Should_indicate_success()
    {
        Assert.That(actualReturnBooleanValue, Is.EqualTo(true));
    }
}
