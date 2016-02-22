using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Models.Staff;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface IClassroomViewProvider
    {
        string GetDefaultClassroomView(int leaId);
    }

    public class ClassroomViewProvider : IClassroomViewProvider
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IRepository<LocalEducationAgencyAdministration> _administrationRepository;
        private readonly IConfigValueProvider _configValueProvider;

        public const string CacheKeyGetDefault = "ClassroomViewProvider.GetDefaultClassroomView.{0}";

        public ClassroomViewProvider(ICacheProvider cacheProvider, IRepository<LocalEducationAgencyAdministration> administrationRepository, IConfigValueProvider configValueProvider)
        {
            _cacheProvider = cacheProvider;
            _administrationRepository = administrationRepository;
            _configValueProvider = configValueProvider;
        }

        public string GetDefaultClassroomView(int leaId)
        {
            object result;

            if (_cacheProvider.TryGetCachedObject(string.Format(CacheKeyGetDefault, leaId), out result))
                return (string)result;

            return GetClassroomViewFromRepo(leaId);
        }

        #region Helper Methods
        private string GetClassroomViewFromRepo(int leaId)
        {
            var adminConfig = from leaAdmin in _administrationRepository.GetAll()
                              where leaAdmin.LocalEducationAgencyId == leaId
                              select leaAdmin;

            int cacheExpiryMinutes = Convert.ToInt32(_configValueProvider.GetValue("CacheInterceptor.SlidingExpiration"));

            var tempClassroomView = StaffModel.ViewType.GeneralOverview.ToString();

            if (adminConfig != null && adminConfig.Count() > 0)
            {
                tempClassroomView = adminConfig.First().DefaultClassroomView;
            }

            _cacheProvider.Insert(string.Format(CacheKeyGetDefault, leaId), tempClassroomView, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(cacheExpiryMinutes));

            return tempClassroomView;
        }
        #endregion
    }
}
