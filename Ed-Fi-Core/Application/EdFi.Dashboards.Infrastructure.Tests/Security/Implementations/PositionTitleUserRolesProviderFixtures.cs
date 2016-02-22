// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
// TODO: Deferred - GKM - Removed fixture for Dave to review due to design changes.
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using EdFi.Dashboards.Resources.Security;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Web.Providers;
using NUnit.Framework;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Testing;
using System.IO;
using Rhino.Mocks;

namespace EdFi.Dashboards.Infrastructure.Tests.Security.Implementations
{

    public class PositionTitleUserRolesProviderFixtures : TestFixtureBase
    {
        public const string defaultPersistedRepositoryDirectory = @"c:\Projects\EDFIData";
        public const string defaultLocalEducationAgencyName = @"LubbockISD";

        public const int EducationOrganizationId = 123456789;
        public const string OrganizationCategory = "School";

        protected Exception exceptionThrown;
        protected Exception contextExceptionThrown;
        protected Dictionary<string, Role> myMapping;

        protected IConfigValueProvider myConfigValueProvider;
        protected ICacheProvider myCacheProvider;
        protected IAuthorizationInformationProvider authorizationInformationProvider;
        protected IHttpServerProvider httpServerProvider;
    	protected IFile file;

        protected string[] myRawData;
        protected PositionTitleUserRolesProvider MyRolesMappingProvider;
        protected Dictionary<string, ICollection<Role>> myParsedData;

        protected void ExpectGetCachedDataObject(int count)
        {
            Expect.Call(myCacheProvider.GetCachedObject(PositionTitleUserRolesProvider.RoleMappingKey))
                .Return(myParsedData)
                .Repeat.Times(count);
        }

        protected string CreateEmailAddress(string name)
        {
            return name.Replace(" ", "") + "@edfi.org";
        }

        protected void ExpectGetConfigValueLocalEducationAgencyName()
        {
            Expect.Call(myConfigValueProvider.GetValue(PositionTitleUserRolesProvider.RoleMappingLocalEducationAgencyName)).Return(defaultLocalEducationAgencyName);
        }

        protected override void EstablishContext(MockRepository mocks)
        {
            try
            {
                myConfigValueProvider = mocks.StrictMock<IConfigValueProvider>();
                myCacheProvider = mocks.StrictMock<ICacheProvider>();
                authorizationInformationProvider = mocks.StrictMock<IAuthorizationInformationProvider>();
                httpServerProvider = mocks.StrictMock<IHttpServerProvider>();
				SetDataSource();

            	file = mocks.StrictMock<IFile>();
            	SetupResult.For(file.ReadAllLines(null))
            		.IgnoreArguments()
            		.Return(myRawData);

				//myParsedData = PositionTitleUserRolesProvider.ParseMappings(myRawData);
            }
            catch (Exception ex)
            {
                contextExceptionThrown = ex;
            }
        }

        protected override void ExecuteTest()
        {
            if (null != contextExceptionThrown) return;

            try
            {
                MyRolesMappingProvider = new PositionTitleUserRolesProvider(file, myCacheProvider, myConfigValueProvider, authorizationInformationProvider, httpServerProvider, null);
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        protected virtual void SetDataSource()
        {
            myRawData = null;
        }

        public void ExpectException(Type type)
        {
            Assert.That(exceptionThrown, Is.InstanceOf(type));
        }

        public void ExpectException(Type type, string message)
        {
            var exception = contextExceptionThrown ?? exceptionThrown;

            Assert.That(exception, Is.InstanceOf(type));
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        public void ExpectMappingFail(UserRoleDetails userRoleDetails, Type type)
        {
            ExpectMappingFail(userRoleDetails, type, null);
        }

        public void ExpectMappingFail(UserRoleDetails userRoleDetails, Type type, string message)
        {
            try
            {
                GetUserRoles(userRoleDetails);
            }
            catch (Exception exception)
            {
                Assert.That(exception, Is.InstanceOf(type));

                if (message != null) Assert.That(exception.Message, Is.EqualTo(message));
            }
        }

		// TODO: Deferred - GKM - This is an assertion, not an expectation.  Rename?
        public void ExpectMappingPass(UserRoleDetails userRoleDetails, Role applicationRole)
        {
            var myRoles = GetUserRoles(userRoleDetails);
            
			Assert.True(myRoles.Contains(applicationRole));
        }

        public IEnumerable<Role> GetUserRoles(UserRoleDetails userRoleDetails)
        {
            // n.b. Directly calling DoGetUserRoles instead of GetUserRoles.
            // This avoids invoking CanGetUserRoles and attempting to parse the input file.
            //
            var myRoles = MyRolesMappingProvider.GetUserRoles(userRoleDetails);
            
			return myRoles;
        }
    }

    public class WhenMappingIsNull:
        PositionTitleUserRolesProviderFixtures
    {
        [Test]
        public void InitializationShouldFail()
        {
            ExpectMappingFail(new UserRoleDetails { PositionTitle = "pirate" }, typeof(NullReferenceException));
        }
    }

    public class WhenMappingIsEmpty :
        PositionTitleUserRolesProviderFixtures
    {
        protected override void SetDataSource()
        {
            myRawData = new string[0];
        }

        protected override void EstablishContext(MockRepository mocks)
        {
            base.EstablishContext(mocks);
            ExpectGetCachedDataObject(1);
        }

        [Test]
        public void InitializationShouldFail()
        {
            ExpectMappingFail(CreateEmailAddress("Teacher"), typeof(ArgumentException), string.Format(PositionTitleUserRolesProvider.CouldNotMapFormat, "Teacher"));
        }
    }

    public class WhenTextLineHasLessThanTwoFields :
        PositionTitleUserRolesProviderFixtures
    {
        protected string fields = "Teacher";
        protected override void SetDataSource()
        {
            myRawData = new[] { "Teacher" };
        }

        [Test]
        public void InitializationShouldFail()
        {
            ExpectException(typeof(ArgumentException), string.Format(PositionTitleUserRolesProvider.InvalidMappingFormat, fields));
        }
    }

    public class WhenTextContainsABlankLine :
        PositionTitleUserRolesProviderFixtures
    {
        protected override void SetDataSource()
        {
            myRawData = new[] { "",
                                 "superintendent,Superintendent"};
        }

        protected override void EstablishContext(MockRepository mocks)
        {
            base.EstablishContext(mocks);
            ExpectGetCachedDataObject(1);
            ExpectGetUserInfo("superintendent", "superintendent");
        }

        [Test]
        public void MappingShouldPass()
        {
            ExpectMappingPass(CreateEmailAddress("superintendent"), Role.Superintendent);
        }
    }

    public class WhenTextContainsAComment :
        PositionTitleUserRolesProviderFixtures
    {
        protected override void SetDataSource()
        {
            myRawData = new[] 
            { 
                "# This is a comment",
                "superintendent,Superintendent"
            };
        }

        protected override void EstablishContext(MockRepository mocks)
        {
            base.EstablishContext(mocks);
            ExpectGetCachedDataObject(1);
            ExpectGetUserInfo("superintendent", "superintendent");
        }

        [Test]
        public void MappingShouldPass()
        {
            ExpectMappingPass(CreateEmailAddress("superintendent"), Role.Superintendent);
        }
    }

    public class WhenTextLineHasMoreThanTwoFields :
        PositionTitleUserRolesProviderFixtures
    {
        protected override void SetDataSource()
        {
            myRawData = new[] 
            { 
                "Teacher,Teacher,Teacher"
            };
        }

        [Test]
        public void InitializationShouldFail()
        {
            ExpectException(typeof(ArgumentException), string.Format(PositionTitleUserRolesProvider.InvalidMappingFormat, myRawData[0]));
        }
    }

    public class WhenTextLineHasInvalidSecurityRole :
        PositionTitleUserRolesProviderFixtures
    {
        protected override void SetDataSource()
        {
            myRawData = new[] 
            { 
                "superintendent,Mayor"
            };
        }

        [Test]
        public void InitializationShouldFail()
        {
            ExpectException(typeof(ArgumentException), string.Format(PositionTitleUserRolesProvider.InvalidSystemRoleFormat, "Mayor"));
        }
    }

    public class WhenUserHasMoreThanOneRole :
        PositionTitleUserRolesProviderFixtures
    {
        protected override void SetDataSource()
        {
            myRawData = new[] { "",
                "Superintendent,Superintendent",
                "Teacher,Teacher"};
        }

        protected override void EstablishContext(MockRepository mocks)
        {
            base.EstablishContext(mocks);
            ExpectGetCachedDataObject(2);
            ExpectGetUserInfo("Superintendent", "Superintendent,Teacher");
            ExpectGetUserInfo("Superintendent", "Superintendent,Teacher");
        }

        [Test]
        public void MappingShouldPass()
        {
            ExpectMappingPass(CreateEmailAddress("Superintendent,Teacher"), Role.Superintendent);
            ExpectMappingPass(CreateEmailAddress("Superintendent,Teacher"), Role.Teacher);
        }
    }

    public class WhenPositionTitleContainsManyLocalRolesToOneApplicationRole :
        PositionTitleUserRolesProviderFixtures
    {
        protected override void SetDataSource()
        {
            myRawData = new[] {
                                "Super,					        Superintendent",
                                "Superintendent,		        Superintendent",
                                "LocalEducationAgencyAdmin,			        LocalEducationAgencyAdministrator",
                                "LocalEducationAgencyAdministrator,		    LocalEducationAgencyAdministrator",
                                "LocalEducationAgencySysAdmin,		        LocalEducationAgencySystemAdministrator",
                                "LocalEducationAgencySystemAdministrator,   LocalEducationAgencySystemAdministrator",
                                "Boss,				            Principal",
                                "Principal,				        Principal",
                                "SchoolAdministrator,		    SchoolAdministrator",
                                "SchoolAdmin,			        SchoolAdministrator",
                                "SchoolSpecialist,			    SchoolSpecialist",
                                "SchoolSpec,			        SchoolSpecialist",
                                "Teacher,					    Teacher",
                                "Teach,					        Teacher"
                            };
        }

        protected override void EstablishContext(MockRepository mocks)
        {
            base.EstablishContext(mocks);
            ExpectGetCachedDataObject(14);
            ExpectGetUserInfo("Superintendent", "Super");
            ExpectGetUserInfo("Superintendent", "Superintendent");
            ExpectGetUserInfo("LocalEducationAgencyAdministrator", "LocalEducationAgencyAdmin");
            ExpectGetUserInfo("LocalEducationAgencyAdministrator", "LocalEducationAgencyAdministrator");
            ExpectGetUserInfo("LocalEducationAgencySystemAdministrator", "LocalEducationAgencySysAdmin");
            ExpectGetUserInfo("LocalEducationAgencySystemAdministrator", "LocalEducationAgencySystemAdministrator");
            ExpectGetUserInfo("Principal", "Boss");
            ExpectGetUserInfo("Principal", "Principal");
            ExpectGetUserInfo("SchoolAdministrator", "SchoolAdministrator");
            ExpectGetUserInfo("SchoolAdministrator", "SchoolAdmin");
            ExpectGetUserInfo("SchoolSpecialist", "SchoolSpecialist");
            ExpectGetUserInfo("SchoolSpecialist", "SchoolSpec");
            ExpectGetUserInfo("Teacher", "Teacher");
            ExpectGetUserInfo("Teacher", "Teach");
        }

        [Test]
        public void AllMappingsShouldPass()
        {
            ExpectMappingPass(CreateEmailAddress("Super"), Role.Superintendent);
            ExpectMappingPass(CreateEmailAddress("Superintendent"), Role.Superintendent);
            ExpectMappingPass(CreateEmailAddress("LocalEducationAgencyAdmin"), Role.LocalEducationAgencyAdministrator);
            ExpectMappingPass(CreateEmailAddress("LocalEducationAgencyAdministrator"), Role.LocalEducationAgencyAdministrator);
            ExpectMappingPass(CreateEmailAddress("LocalEducationAgencySysAdmin"), Role.LocalEducationAgencySystemAdministrator);
            ExpectMappingPass(CreateEmailAddress("LocalEducationAgencySystemAdministrator"), Role.LocalEducationAgencySystemAdministrator);
            ExpectMappingPass(CreateEmailAddress("Principal"), Role.Principal);
            ExpectMappingPass(CreateEmailAddress("Boss"), Role.Principal);
            ExpectMappingPass(CreateEmailAddress("SchoolAdministrator"), Role.SchoolAdministrator);
            ExpectMappingPass(CreateEmailAddress("SchoolAdmin"), Role.SchoolAdministrator);
            ExpectMappingPass(CreateEmailAddress("SchoolSpecialist"), Role.SchoolSpecialist);
            ExpectMappingPass(CreateEmailAddress("SchoolSpec"), Role.SchoolSpecialist);
            ExpectMappingPass(CreateEmailAddress("Teacher"), Role.Teacher);
            ExpectMappingPass(CreateEmailAddress("Teach"), Role.Teacher);
        }
    }

    public class WhenDataSourceSystemRolesContainInternalSpaces :
        PositionTitleUserRolesProviderFixtures
    {
        protected override void SetDataSource()
        {
            myRawData = new[]  {
                                "Local Education Agency Administrator,		    Local Education Agency Administrator",
                                "Local Education Agency System Administrator,     Local Education Agency System Administrator",
                                "School Administrator,		        School Administrator",
                                "School Specialist,			        School Specialist"
                            };
        }

        protected override void EstablishContext(MockRepository mocks)
        {
            base.EstablishContext(mocks);
            ExpectGetCachedDataObject(4);
            ExpectGetUserInfo("LocalEducationAgencyAdministrator", "Local Education Agency Administrator");
            ExpectGetUserInfo("LocalEducationAgencySystemAdministrator", "Local Education Agency System Administrator");
            ExpectGetUserInfo("SchoolAdministrator", "School Administrator");
            ExpectGetUserInfo("SchoolSpecialist", "School Specialist");
        }

        [Test]
        public void TheMappingShouldPass()
        {
            ExpectMappingPass(CreateEmailAddress("Local Education Agency Administrator"), Role.LocalEducationAgencyAdministrator);
            ExpectMappingPass(CreateEmailAddress("Local Education Agency System Administrator"), Role.LocalEducationAgencySystemAdministrator);
            ExpectMappingPass(CreateEmailAddress("School Administrator"), Role.SchoolAdministrator);
            ExpectMappingPass(CreateEmailAddress("School Specialist"), Role.SchoolSpecialist);
        }
    }

    public class WhenPositionTitleHasOneLocalRoleMappedToManySystemRoles :
        PositionTitleUserRolesProviderFixtures
    {

        protected override void SetDataSource()
        {
            myRawData = new[]
                            {
                                "Bozo The Clone,School Administrator",
                                "Bozo The Clone,Local Education Agency Administrator"
                            };
        }

        [Test]
        public void InitializationShouldPass()
        {
            Assert.IsNull(exceptionThrown);
            Assert.IsNull( contextExceptionThrown);
            var roles = myParsedData[("Bozo The Clone").ToUpper()];
            Assert.IsTrue(roles.ElementAt(0).Equals(Role.SchoolAdministrator));
            Assert.IsTrue(roles.ElementAt(1).Equals(Role.LocalEducationAgencyAdministrator));
        }
    }

    public class WhenLocalRoleIsNotMapped :
        PositionTitleUserRolesProviderFixtures
    {

        protected override void SetDataSource()
        {
            myRawData = new[]
                            {
                                "Bozo The Clone,None"
                            };
        }

        protected override void EstablishContext(MockRepository mocks)
        {
            base.EstablishContext(mocks);
            ExpectGetCachedDataObject(1);
            ExpectGetUserInfo("LocalEducationAgencyAdministrator", "Bozo The Clone");
        }

        [Test]
        public void MappingShouldFail()
        {
            ExpectMappingFail(CreateEmailAddress( "Bozo The Clone"), typeof(ArgumentException), string.Format(PositionTitleUserRolesProvider.CouldNotMapFormat, "Bozo The Clone"));
        }
    }

    public class WhenLoadingPositionTitle : TestFixtureBase
    {
        protected IConfigValueProvider myConfigValueProvider;
        protected ICacheProvider myCacheProvider;
        protected IAuthorizationInformationProvider authorizationInformationProvider;
        protected IHttpServerProvider httpServerProvider;

        protected string[] myRawData;
        protected PositionTitleUserRolesProvider MyRolesMappingProvider;
        protected Dictionary<string, ICollection<Role>> myParsedData;

        private const string localEducationAgencyName = "LubbockISD";
        private string providedFileName;

        protected void ExpectGetCachedDataObject(int count)
        {
            Expect.Call(myCacheProvider.GetCachedObject(PositionTitleUserRolesProvider.RoleMappingKey))
                .Return(myParsedData)
                .Repeat.Times(count);
        }

        protected override void EstablishContext(MockRepository mocks)
        {
            myConfigValueProvider = mocks.StrictMock<IConfigValueProvider>();
            myCacheProvider = mocks.StrictMock<ICacheProvider>();
            authorizationInformationProvider = mocks.StrictMock<IAuthorizationInformationProvider>();
            httpServerProvider = mockRepository.StrictMock<IHttpServerProvider>();

            providedFileName = Path.GetFullPath(localEducationAgencyName.Replace(" ", "") + ".csv");
            Expect.Call(myConfigValueProvider.GetValue("LocalEducationAgencyCode")).Return(localEducationAgencyName);
            Expect.Call(httpServerProvider.MapPath(String.Format("~/Files/RoleMapping/{0}.csv", localEducationAgencyName.Replace(" ", "")))).Return(providedFileName);

            ExpectGetCachedDataObject(1);

            myCacheProvider.Insert(PositionTitleUserRolesProvider.RoleMappingKey, myParsedData, new CacheDependency(providedFileName), DateTime.Now, TimeSpan.Zero);
            LastCall.IgnoreArguments();
        }


        protected override void ExecuteTest()
        {
            MyRolesMappingProvider = new PositionTitleUserRolesProvider(myCacheProvider, myConfigValueProvider, authorizationInformationProvider, httpServerProvider, null);
            myParsedData = MyRolesMappingProvider.RolesByTitle;
        }

        [Test]
        public void RowCountShouldMatchMapCount()
        {
            Console.WriteLine("Absolute Path: " + providedFileName);

            // N.B. This depends upon the CSV file containing no comments and no blank lines.
            //
            var lines = File.ReadAllLines(providedFileName);
            var rowCount = lines.Where(line => !line.Contains("None")).Count();

            var mapCount = myParsedData.Count;
            Assert.That(mapCount, Is.EqualTo(rowCount - 1));//remove one line b/c School leader is in the list twice

            myCacheProvider.AssertWasCalled(
                x => x.Insert(
                    Arg<string>.Is.Equal(PositionTitleUserRolesProvider.RoleMappingKey),
                    Arg<Dictionary<string, Role>>.Is.Equal(myParsedData),
                    Arg<CacheDependency>.Is.Anything,
                    Arg<DateTime>.Is.Anything,
                    Arg<TimeSpan>.Is.Anything));
        }
    }

    // Test fail over to StaffCategoryUserRolesProvider

    public class WhenPositionTitlesFailOverToStaffCategory :
        PositionTitleUserRolesProviderFixtures
    {
        protected override void EstablishContext(MockRepository mocks)
        {
            const string NO_SUCH_FILE = "NO_SUCH_FILE.XYZ";

            base.EstablishContext(mocks); 
            
            var staffCategoryUserRolesProvider = new StaffCategoryUserRolesProvider(authorizationInformationProvider,
                                                                                        null);
            MyRolesMappingProvider = new PositionTitleUserRolesProvider(myCacheProvider, myConfigValueProvider,
                                                                       authorizationInformationProvider,
                                                                       httpServerProvider, staffCategoryUserRolesProvider);
            ExpectGetCachedDataObject(1);

            // If the Local Education Agency name is an absolute pathname it will be used as the position title file location.
            //
            var roleName = Role.Superintendent.ToString();
            var absolutePath = Path.GetFullPath(NO_SUCH_FILE);
            Expect.Call(myConfigValueProvider.GetValue("LocalEducationAgencyCode")).Return(absolutePath);

            ExpectGetUserInfo(roleName, roleName);
        }

        [Test]
        public void GetUserRolesShouldPass()
        {
            var emailAddress = CreateEmailAddress(Role.Superintendent.ToString());
            var myRoles = MyRolesMappingProvider.GetUserRoles(emailAddress);

            Assert.True(myRoles.Contains(Role.Superintendent));
        }
    }
}
*/