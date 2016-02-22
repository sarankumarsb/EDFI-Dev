// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.StudentSchool;

namespace EdFi.Dashboards.Resources.Navigation.RouteValueProviders
{
    public class StudentRouteValueProvider : IRouteValueProvider
    {
        private readonly IIdNameService idNameService;
        private readonly ICacheProvider cacheProvider;
        private const string cacheKey = "StudentRouteValueProvider.ProvideRouteValue";

        public StudentRouteValueProvider(IIdNameService idNameService, ICacheProvider cacheProvider)
        {
            this.idNameService = idNameService;
            this.cacheProvider = cacheProvider;
        }

        public bool CanProvideRouteValue(string key, Func<string, object> getValue)
        {
            if (key.Equals("student", StringComparison.OrdinalIgnoreCase))
            {
                if (GetStudentUSI(getValue) != 0 && GetSchoolId(getValue) != 0)
                    return true;
            }

            return false;
        }

        public void ProvideRouteValue(string key, Func<string, object> getValue, Action<string, object> setValue)
        {
            var schoolId = GetSchoolId(getValue);
            var studentUSI = GetStudentUSI(getValue);
            string idName = null;
            object cacheValue;

            if (!cacheProvider.TryGetCachedObject(cacheKey, out cacheValue))
            {
                cacheValue = new ConcurrentDictionary<int, ConcurrentDictionary<int, string>>();
                cacheProvider.SetCachedObject(cacheKey, cacheValue);
            }

            var cachedStudentNameLookup = cacheValue as ConcurrentDictionary<int, ConcurrentDictionary<int, string>>;
            
            var studentUSINameList = cachedStudentNameLookup.GetOrAdd(schoolId, 
                id => new ConcurrentDictionary<int, string>());

            if (!studentUSINameList.TryGetValue(studentUSI, out idName))
            {
                var idNameModel = idNameService.Get(IdNameRequest.Create(schoolId, studentUSI));
                idName = Regex.Replace(idNameModel.FullName, @"[^\w]", "-");
                studentUSINameList.TryAdd(studentUSI, idName);
            }

            setValue("student", idName);
        }

		private static int GetStudentUSI(Func<string, object> getValue)
        {
            return Convert.ToInt32(getValue("studentUSI"));
        }

		private static int GetSchoolId(Func<string, object> getValue)
        {
            return Convert.ToInt32(getValue("schoolId"));
        }
    }
}
