using System;
using System.Collections.Generic;
using System.Reflection;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests
{
    [TestFixture]
    public class When_function_is_marked_with_authentication_ignore : TestFixtureBase
    {
        private readonly List<string> _whiteList = new List<string>(20);
        private Type[] _assemblyTypes;

        protected override void EstablishContext()
        {
            _whiteList.Add("EdFi.Dashboards.Resources.Admin.ResourcesService Get Everyone should be able to get list of resources");
            _whiteList.Add("EdFi.Dashboards.Resources.Application.ErrorLoggingService Post Anyone can log an exception");
            _whiteList.Add("EdFi.Dashboards.Resources.Application.HomeService Get Local Education Agency Name is a public record.");
            _whiteList.Add("EdFi.Dashboards.Resources.Application.FeedbackService Post Anyone can submit a feedback request");
            _whiteList.Add("EdFi.Dashboards.Resources.Common.MetricsBasedWatchListService Post Watchlists themselves do not contain data.");
            _whiteList.Add("EdFi.Dashboards.Resources.LocalEducationAgency.ResourcesService Get Everyone should be able to get list of resources");
            _whiteList.Add("EdFi.Dashboards.Resources.LocalEducationAgency.HomeService Get Local Education Agency Name is a public record.");
            _whiteList.Add("EdFi.Dashboards.Resources.LocalEducationAgency.IdCodeService Get Local Education Agency Name is a public record.");
            _whiteList.Add("EdFi.Dashboards.Resources.LocalEducationAgency.ImageService Get This service returns the LEA image.");
            _whiteList.Add("EdFi.Dashboards.Resources.LocalEducationAgency.RouteValueResolutionService Get This service merely translates route values containing a representation of the LEA Name to the internal Local Education Agency Ids.");
            _whiteList.Add("EdFi.Dashboards.Resources.EntryService Get This service uses the user's claims to determine a landing page URL within the application based on theLEA/School identifiers passed in.  No information that the current user cannot see will be returned.");
            _whiteList.Add("EdFi.Dashboards.Resources.School.ResourcesService Get Everyone should be able to get list of resources");
            _whiteList.Add("EdFi.Dashboards.Resources.School.ImageService Get This service returns the school image.");
            _whiteList.Add("EdFi.Dashboards.Resources.School.RouteValueResolutionService Get Service method returns identifiers for schools based on school names, neither of which is sensitive data.");
            _whiteList.Add("EdFi.Dashboards.Resources.School.IdNameService Get School Name is a public record.");
            _whiteList.Add("EdFi.Dashboards.Resources.Staff.MetadataListColumnService Get Everyone should be able to get metadata");
            _whiteList.Add("EdFi.Dashboards.Resources.Staff.ImageService Get This service returns the staff image.");
            _whiteList.Add("EdFi.Dashboards.Resources.Staff.MetadataListService Get Everyone should be able to get metadata");
            _whiteList.Add("EdFi.Dashboards.Resources.StudentSchool.ResourcesService Get Everyone should be able to get list of resources");
            _whiteList.Add("EdFi.Dashboards.Resources.StudentSchool.ImageService Get This service returns the student image.");
        }

        protected override void ExecuteTest()
        {
            _assemblyTypes = typeof(Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();
        }

        [Test]
        public void Must_be_in_the_white_list()
        {
            foreach (Type assemblyType in _assemblyTypes)
            {
                var methods = assemblyType.GetMethods();

                foreach (MethodInfo method in methods)
                {
                    var attributes = method.GetCustomAttributes(true);
                    foreach (Attribute attr in attributes)
                    {
                        var attrName = attr.GetType().Name;
                        if (!attrName.Equals("AuthenticationIgnoreAttribute")) continue;

                        var authIgnoreAttr = (AuthenticationIgnoreAttribute)attr;
                        var authIgnoreInfo = string.Format("{0} {1} {2}", assemblyType.FullName, method.Name, authIgnoreAttr.Justification);
                        Assert.Contains(authIgnoreInfo, _whiteList);
                    }
                }
            }
        }
    }
}
