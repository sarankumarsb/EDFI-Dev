using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    public abstract class ClassroomViewProviderFixture : TestFixtureBase
    {
        protected IRepository<LocalEducationAgencyAdministration> leaAdminRepo;
        protected ICacheProvider cacheProvider;
        protected IConfigValueProvider configValueProvider;
        protected ClassroomViewProvider provider;
        protected string classroomView = string.Empty;

        #region TestFixture Override Methods
        protected override void EstablishContext()
        {
            

            provider = new ClassroomViewProvider(cacheProvider, leaAdminRepo, configValueProvider);
        }
        #endregion

        public virtual void MockCacheProvider(string cacheKey, object cacheEntry, bool tryGetCachedObjectResult)
        {
            if (cacheProvider == null)
                cacheProvider = mocks.StrictMock<ICacheProvider>();

            object cacheEntryVariable;
            Expect.Call(cacheProvider.TryGetCachedObject(cacheKey, out cacheEntryVariable))
                .OutRef(new[] { cacheEntry })
                .Return(tryGetCachedObjectResult);
        }
    }

    public class When_calling_classroom_view_provider_for_lea_in_cache : ClassroomViewProviderFixture
    {
        protected override void EstablishContext()
        {
            MockCacheProvider("ClassroomViewProvider.GetDefaultClassroomView.1", "PriorYear", true);

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {          
            classroomView = provider.GetDefaultClassroomView(1);
        }

        [Test]
        public void Should_return_classroom_view()
        {
            Assert.That(classroomView, Is.EqualTo(StaffModel.ViewType.PriorYear.ToString()));
        }
    }

    public class When_calling_classroom_view_provider_for_lea_not_in_cache : ClassroomViewProviderFixture
    {
        protected override void EstablishContext()
        {
            configValueProvider = mocks.StrictMock<IConfigValueProvider>();
            leaAdminRepo = mocks.StrictMock<IRepository<LocalEducationAgencyAdministration>>();

            MockCacheProvider("ClassroomViewProvider.GetDefaultClassroomView.2", "PriorYear", false);

            Expect.Call(leaAdminRepo.GetAll()).Return(new List<LocalEducationAgencyAdministration>
                                                          {
                                                              new LocalEducationAgencyAdministration()
                                                                  {
                                                                      LocalEducationAgencyId = 1,
                                                                      DefaultClassroomView = "PriorYear"
                                                                  }
                                                          }.AsQueryable());

            Expect.Call(() => cacheProvider.Insert(string.Empty, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration)).IgnoreArguments();
            Expect.Call(configValueProvider.GetValue("CacheInterceptor.SlidingExpiration")).IgnoreArguments().Return("1");

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            classroomView = provider.GetDefaultClassroomView(2);
        }

        [Test]
        public void Should_return_classroom_view()
        {
            Assert.That(classroomView, Is.EqualTo(StaffModel.ViewType.GeneralOverview.ToString()));
        }

        
    } 
}
