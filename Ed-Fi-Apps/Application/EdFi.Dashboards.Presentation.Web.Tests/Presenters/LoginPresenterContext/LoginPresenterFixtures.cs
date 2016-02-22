// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
// TODO: Deferred - A lot has changed since these tests were run.  Needs review, and probably a move to a SecurityTokenService.Tests project, and update tests

/*
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Presentation.Web.Presenters;
using EdFi.Dashboards.Presentation.Web.Utilities;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Security;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Testing;
using Is = NUnit.Framework.Is;
using EdFi.Dashboards.Infrastructure.Implementations;

namespace EdFi.Dashboards.Presentation.Web.Tests.Presenters.LoginPresenterContext
{
    [TestFixture]
    public abstract class When_authenticating : TestFixtureBase
    {
        protected IAuthenticationService authenticationService;
        protected IAuthorizationInformationProvider authorizationInformationProvider;
        protected IFormsAuthenticationService formsAuthenticationService;
        protected HashtableSessionStateProvider sessionStateProvider;
        protected IUserRolesProvider userRolesProvider;
        protected ISiteAvailableService siteAvailableService;
        protected ICurrentUserProvider currentUserProvider;
        protected ILoginView view;
        protected CapturingConstraint validateUserArgs;
        protected CapturingConstraint showLoginMessageArgs;
        protected CapturingConstraint getRedirectArgs;
        protected CapturingConstraint getEmailAddressArgs;
        protected CapturingConstraint getUserInformationArgs;
        protected CapturingConstraint getUserRolesArgs;
        protected CapturingConstraint getPerformBrowserLoginArgs;
        protected CapturingConstraint getSessionStateProviderArgs;

        protected UserInformation suppliedUserInformationNoOrg = new UserInformation { StaffUSI = 1 };
        protected UserInformation suppliedUserInformationSchool = new UserInformation { StaffUSI = 1, AssociatedOrganizations = new List<UserInformation.EducationOrganization> { new UserInformation.EducationOrganization { Category = EducationOrganizationCategory.School, EducationOrganizationId = SCHOOL_0 } } };
        protected UserInformation suppliedUserInformation = new UserInformation { StaffUSI = 1, AssociatedOrganizations = new List<UserInformation.EducationOrganization> { new UserInformation.EducationOrganization { Category = EducationOrganizationCategory.LocalEducationAgency, EducationOrganizationId = LOCALEDUCATIONAGENCY_0 } } };

        protected const string USERNAME = "username";
        protected const string PASSWORD = "password";

        protected const int USER_0 = 100;
        protected const int USER_1 = USER_0 + 1;
        protected const int USER_2 = USER_0 + 2;

        protected const int LOCALEDUCATIONAGENCY_0 = 15290;

        protected const int SCHOOL_0 = 15290020;
        protected const int SCHOOL_1 = SCHOOL_0 + 1;
        protected const int SCHOOL_2 = SCHOOL_0 + 2;

        protected const int TEACHER_0 = 2000;
        protected const int TEACHER_1 = TEACHER_0 + 1;
        protected const int TEACHER_2 = TEACHER_0 + 2;

        protected const int STUDENT_0 = 7000;
        protected const int STUDENT_1 = STUDENT_0 + 1;
        protected const int STUDENT_2 = STUDENT_0 + 2;

        protected const int PRINCIPAL_0 = 3000;
        protected const int PRINCIPAL_1 = PRINCIPAL_0 + 1;
        protected const int PRINCIPAL_2 = PRINCIPAL_0 + 2;

        protected const int SECTION_0 = 4000;
        protected const int SECTION_1 = SECTION_0 + 1;
        protected const int SECTION_2 = SECTION_0 + 2;

        protected const int SPECIALIST_0 = 5000;
        protected const int SPECIALIST_1 = SPECIALIST_0 + 1;
        protected const int SPECIALIST_2 = SPECIALIST_0 + 2;

        protected Exception exceptionThrown;

        protected UserInformation CreateUserInformationWithTwoOrganizations(long staffUSI, int? educationOrganizationId1, int? educationOrganizationId2)
        {
            var userInfo = new UserInformation { StaffUSI = staffUSI };

            if (educationOrganizationId1.HasValue)
            {
                var org = new UserInformation.EducationOrganization { EducationOrganizationId = educationOrganizationId1.Value, Category = EducationOrganizationCategory.School };
                userInfo.AssociatedOrganizations.Add(org);
            }

            if (educationOrganizationId2.HasValue)
            {
                var org = new UserInformation.EducationOrganization { EducationOrganizationId = educationOrganizationId2.Value, Category = EducationOrganizationCategory.School };
                userInfo.AssociatedOrganizations.Add(org);
            }

            return( userInfo);
        }        

        protected void CreateMocks(MockRepository mocks)
        {
            view = mocks.DynamicMock<ILoginView>();
            authenticationService = mocks.DynamicMock<IAuthenticationService>();
            authorizationInformationProvider = mocks.DynamicMock<IAuthorizationInformationProvider>();
            userRolesProvider = mocks.DynamicMock<IUserRolesProvider>();
            formsAuthenticationService = mocks.DynamicMock<IFormsAuthenticationService>();
            siteAvailableService = mockRepository.DynamicMock<ISiteAvailableService>();
            currentUserProvider = mockRepository.DynamicMock<ICurrentUserProvider>();
            sessionStateProvider = new HashtableSessionStateProvider();
        }

        protected override void EstablishContext(MockRepository mocks)
        {
            CreateMocks(mocks);
            view.HideLoginMessage();

            EstablishContextForScenario(mocks);
        }

        protected abstract void EstablishContextForScenario(MockRepository mocks);

        protected override void ExecuteTest()
        {
            var presenter = new LoginPresenter(authenticationService, authorizationInformationProvider, userRolesProvider, sessionStateProvider, formsAuthenticationService, siteAvailableService, currentUserProvider);

            try
            {
                view = presenter.View = view;
                presenter.AuthenticateAndRedirect(USERNAME, PASSWORD, GetSuppliedRedirectUrl());
            }
            catch (Exception ex)
            {
                exceptionThrown = ex;
            }
        }

        // Default behavior is to pass supplied redirect as empty
        protected virtual string GetSuppliedRedirectUrl() { return null;}

        protected void Mock_AuthenticationService_ValidateUser(bool result)
        {
            validateUserArgs = authenticationService.CaptureArgumentsFor(x => x.ValidateUser(null, null));
            LastCall.Return(result);
        }

        protected void Mock_View_Redirect()
        {
            getRedirectArgs = view.CaptureArgumentsFor(x => x.Redirect(null));
        }

        protected void Mock_View_ShowLoginMessage()
        {
            showLoginMessageArgs = view.CaptureArgumentsFor(x => x.ShowLoginMessage(null));
        }

        protected void Mock_AuthenticationService_GetEmailAddress(string result)
        {
            getEmailAddressArgs = authenticationService.CaptureArgumentsFor(x => x.GetEmailAddress(null));
            LastCall.Return(result);
        }

        protected void Mock_UserAuthorization_GetUserInformation(UserInformation result)
        {
            getUserInformationArgs = authorizationInformationProvider.CaptureArgumentsFor(x => x.GetUserInformation(null));
            LastCall.Return(result);
        }

        protected void Mock_UserRolesProvider_GetUserRoles(Role[] result)
        {
            getUserRolesArgs = userRolesProvider.CaptureArgumentsFor(x => x.GetUserRoles(null));
            LastCall.Return(result);
        }

        protected void Mock_FormsAuthenticationService_PerformBrowserLogin()
        {
            getPerformBrowserLoginArgs = formsAuthenticationService.CaptureArgumentsFor(x => x.PerformBrowserLogin(null, null, 0));
        }

        protected virtual Role[] GetUserRolesResult()
        {
            return null;
        }

        protected virtual UserInformation GetUserInformationResult()
        {
            return null;
        }

        protected UserInformation CreateUserInformation(long staffUSI, EducationOrganizationCategory educationOrganizationCategory, int educationOrganizationId)
        {
            return new UserInformation
                    {
                        StaffUSI = staffUSI,
                        AssociatedOrganizations =
                            new List<UserInformation.EducationOrganization>
                                {
                                    new UserInformation.EducationOrganization
                                        {
                                            Category = educationOrganizationCategory,
                                            EducationOrganizationId = educationOrganizationId
                                        }
                                }
                    };
        }
    }

    [TestFixture]
    public class When_authenticating_with_incorrect_username_and_password
        : When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            // User fails authentication
            Mock_AuthenticationService_ValidateUser(false);
        }

        protected override string GetSuppliedRedirectUrl()
        {
            return null;
        }

        [Test]
        public void Should_attempt_to_authenticate_with_username_and_password()
        {
            Assert.That(validateUserArgs.MethodInvoked, "Authentication attempt was not made.");
            Assert.That(validateUserArgs.First<string>(), Is.EqualTo(USERNAME));
            Assert.That(validateUserArgs.Second<string>(), Is.EqualTo(PASSWORD));
        }
    }

    [TestFixture]
    public class When_authenticating_and_login_fails
        : When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(false);
            Mock_View_ShowLoginMessage();
        }

        [Test]
        public void Should_display_message_indicating_the_login_failed()
        {
            Assert.That(showLoginMessageArgs.MethodInvoked, "Login message not displayed.");
            Assert.That(showLoginMessageArgs.First<string>(), Is.EqualTo(LoginPresenter.UserNameOrPasswordIncorrectMessage));
        }
    }

    [TestFixture]
    public class When_login_succeeds_and_no_email_address_is_available 
        : When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress(null);
            Mock_View_ShowLoginMessage();
        }

        [Test]
        public void Should_display_no_user_information_available_error_message()
        {
            Assert.That(showLoginMessageArgs.MethodInvoked, "User information error message not displayed.");
            Assert.That(showLoginMessageArgs.First<string>(), Is.EqualTo(Authentication.NoUserInformationErrorMessage));
        }
    }

    [TestFixture]
    public class When_login_succeeds_and_email_address_is_available
        : When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");
        }

        [Test]
        public void Should_retrieve_the_email_address()
        {
            Assert.That(getEmailAddressArgs.MethodInvoked, "Email address was not requested.");
            Assert.That(getEmailAddressArgs.First<string>(), Is.EqualTo("username"));
        }
    }

    [TestFixture]
    public class When_login_succeeds_and_user_information_is_available
        : When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");
            Mock_UserAuthorization_GetUserInformation(suppliedUserInformation);
        }

        [Test]
        public void Should_get_the_user_information()
        {
            Assert.That(getUserInformationArgs.MethodInvoked, "User information error message not displayed.");
            Assert.That(getUserInformationArgs.First<string>(), Is.EqualTo("username@domain.org"));
        }
    }

    [TestFixture]
    public class When_login_succeeds_and_user_information_is_not_available 
        : When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");
            Mock_UserAuthorization_GetUserInformation(null);
            Mock_View_ShowLoginMessage();
        }

        [Test]
        public void Should_display_no_user_information_available_error_message()
        {
            Assert.That(showLoginMessageArgs.MethodInvoked, "User information error message not displayed.");
            Assert.That(showLoginMessageArgs.First<string>(), Is.EqualTo(Authentication.NoUserInformationErrorMessage));
        }
    }

    public class When_login_succeeds_and_roles_is_null :
        When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");
            Mock_UserAuthorization_GetUserInformation(suppliedUserInformation);
            Mock_UserRolesProvider_GetUserRoles(GetUserRolesResult()); 
            Mock_View_ShowLoginMessage();
        }

        protected override Role[] GetUserRolesResult()
        {
            return null;
        }

        [Test]
        public void Should_display_no_roles_assigned_error_message()
        {
            Assert.That(showLoginMessageArgs.MethodInvoked, "No roles error message was not displayed.");
            Assert.That(showLoginMessageArgs.First<string>(), Is.EqualTo(Authentication.NoRolesErrorMessage));
        }
    }

    public class When_login_succeeds_and_roles_is_empty :
        When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");
            Mock_UserAuthorization_GetUserInformation(suppliedUserInformation);
            Mock_UserRolesProvider_GetUserRoles(GetUserRolesResult());
            Mock_View_ShowLoginMessage();
        }

        protected override Role[] GetUserRolesResult()
        {
            return new Role[0];
        }

        [Test]
        public void Should_display_no_roles_assigned_error_message()
        {
            Assert.That(showLoginMessageArgs.MethodInvoked, "No roles error message was not displayed.");
            Assert.That(showLoginMessageArgs.First<string>(), Is.EqualTo(Authentication.NoRolesErrorMessage));
        }
    }

    public class When_login_succeeds_and_there_are_multiple_roles :
        When_authenticating
    {
        private Role[] suppliedRoles = new []
                                        {
                                            Role.SchoolSpecialist, 
                                            Role.Principal, 
                                            Role.Teacher,
                                        };

        private string[] expectedRoles = new[]
                                            {
                                                Role.SchoolSpecialist.ToString(), 
                                                Role.Principal.ToString(), 
                                                Role.Teacher.ToString()
                                            };

        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");
            Mock_UserAuthorization_GetUserInformation(suppliedUserInformation);
            Mock_UserRolesProvider_GetUserRoles(GetUserRolesResult());
            Mock_FormsAuthenticationService_PerformBrowserLogin();
        }

        protected override Role[] GetUserRolesResult()
        {
            return suppliedRoles;
        }

        [Test]
        public void Should_convert_roles_to_strings()
        {
            Assert.That(getPerformBrowserLoginArgs.MethodInvoked, "Browser login not performed.");
            Assert.That(getPerformBrowserLoginArgs.First<string>(), Is.EqualTo("username"));
            Assert.That(getPerformBrowserLoginArgs.Second<string[]>(), Is.EqualTo(expectedRoles));
        }
    }

    public class When_login_succeeds_for_a_user_in_a_school_level_role_but_user_is_not_associated_with_any_school :
        When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            // Principal only associated with a Local Education Agency in the data (shouldn't happen, really)
            suppliedUserInformation = CreateUserInformation(1, EducationOrganizationCategory.LocalEducationAgency, LOCALEDUCATIONAGENCY_0);

            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");
            Mock_UserAuthorization_GetUserInformation(suppliedUserInformationNoOrg);
            Mock_UserRolesProvider_GetUserRoles(GetUserRolesResult());
            Mock_FormsAuthenticationService_PerformBrowserLogin();

            Mock_View_ShowLoginMessage();
        }

        protected override Role[] GetUserRolesResult()
        {
            return new[] { Role.Principal };
        }

        [Test]
        public void Should_display_a_login_error_indicating_user_is_not_associated_with_any_school()
        {
            Assert.That(showLoginMessageArgs.MethodInvoked, "ShowLoginMessage was not invoked.");
            Assert.That(showLoginMessageArgs.First<string>(), Is.EqualTo(Authentication.AssociatedSchoolCouldNotBeIdentified));
        }
    }

    public class When_login_succeeds_for_a_user_in_a_local_education_agency_level_role_but_user_is_not_associated_with_any_local_education_agency :
    When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            // Principal only associated with a Local Education Agency in the data (shouldn't happen, really)
            suppliedUserInformation = suppliedUserInformationSchool;

            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");
            Mock_UserAuthorization_GetUserInformation(suppliedUserInformationSchool);
            Mock_UserRolesProvider_GetUserRoles(GetUserRolesResult());
            Mock_FormsAuthenticationService_PerformBrowserLogin();
            Mock_View_Redirect();
        }

        protected override Role[] GetUserRolesResult()
        {
            return new[] { Role.Superintendent };
        }

        [Test]
        public void Should_redirect_browser_to_users_default_page()
        {
            Assert.That(getRedirectArgs.First<string>(),
                Is.EqualTo(EdFiWebFormsDashboards.Site.LocalEducationAgency.LocalEducationAgencyInformation(LOCALEDUCATIONAGENCY_0)));
        }

        [Test]
        public void Should_save_user_information_to_session_state()
        {
            Assert.That(sessionStateProvider[EdFiApp.Session.UserInformation], Is.SameAs(suppliedUserInformationSchool));
        }
    }


    public class When_login_succeeds_and_explicit_redirect_is_present :
        When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");
            Mock_UserAuthorization_GetUserInformation(GetUserInformationResult());
            Mock_UserRolesProvider_GetUserRoles(GetUserRolesResult());
            Mock_FormsAuthenticationService_PerformBrowserLogin();
            Mock_View_Redirect();
        }

        protected override UserInformation GetUserInformationResult()
        {
            // Save user info for session state verification
            suppliedUserInformation = CreateUserInformation(TEACHER_1, EducationOrganizationCategory.School, SCHOOL_1);
            
            return suppliedUserInformation;
        }

        protected override string GetSuppliedRedirectUrl()
        {
            return "http://redirect.com";
        }

        protected override Role[] GetUserRolesResult()
        {
            return  new [] { Role.Teacher, };
        }

        [Test]
        public void Should_redirect_browser_to_supplied_redirect()
        {
            Assert.That(getRedirectArgs.MethodInvoked, "View Redirect not invoked.");
            Assert.That(getRedirectArgs.First<string>(), Is.EqualTo(GetSuppliedRedirectUrl()));
        }
    }

    public abstract class When_login_succeeds_and_explicit_redirect_is_present_but_is_not_an_acceptable_redirection_target :
        When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");
            Mock_UserAuthorization_GetUserInformation(GetUserInformationResult());
            Mock_UserRolesProvider_GetUserRoles(GetUserRolesResult());
            Mock_FormsAuthenticationService_PerformBrowserLogin();
            Mock_View_Redirect();
        }

        protected override UserInformation GetUserInformationResult()
        {
            // Save user info for session state verification
            suppliedUserInformation = CreateUserInformation(TEACHER_1, EducationOrganizationCategory.School, SCHOOL_1);
            
            return suppliedUserInformation;
        }

        protected override Role[] GetUserRolesResult()
        {
            return  new [] { Role.Teacher, };
        }

        [Test]
        public void Should_redirect_browser_to_users_default_page_instead_of_the_login_page()
        {
            Assert.That(getRedirectArgs.MethodInvoked, "View Redirect not invoked.");
            Assert.That(getRedirectArgs.First<string>(), 
                Is.EqualTo(EdFiWebFormsDashboards.Site.Staff.GeneralOverview(SCHOOL_1, TEACHER_1)));
        }
    }

    public class When_login_succeeds_and_explicit_redirect_is_present_but_is_the_login_page
        : When_login_succeeds_and_explicit_redirect_is_present_but_is_not_an_acceptable_redirection_target
    {
        protected override string GetSuppliedRedirectUrl()
        {
            return "http://redirect.com/whatever/folders/Login.aspx";
        }
    }

    public class When_login_succeeds_and_explicit_redirect_is_present_but_is_the_logout_page
        : When_login_succeeds_and_explicit_redirect_is_present_but_is_not_an_acceptable_redirection_target
    {
        protected override string GetSuppliedRedirectUrl()
        {
            return "http://redirect.com/whatever/folders/Logout";
        }
    }

    public abstract class When_redirect_succeeds :
        When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");

            suppliedUserInformation = GetUserInformationResult();
            Mock_UserAuthorization_GetUserInformation(suppliedUserInformation);
            
            Mock_UserRolesProvider_GetUserRoles(GetUserRolesResult());
            Mock_FormsAuthenticationService_PerformBrowserLogin();
            Mock_View_Redirect();
        }

        [Test]
        public void Should_save_user_information_to_session_state()
        {
            Assert.That(sessionStateProvider[EdFiApp.Session.UserInformation], Is.SameAs(suppliedUserInformation));
        }
    }

    public abstract class When_redirect_fails :  // GKM: Not sure what this means... When_redirect_fails. Sounds like you're testing a failure to redirect, which shouldn't happen.
        When_authenticating
    {
        protected override void EstablishContextForScenario(MockRepository mocks)
        {
            Mock_AuthenticationService_ValidateUser(true);
            Mock_AuthenticationService_GetEmailAddress("username@domain.org");

            suppliedUserInformation = GetUserInformationResult();
            Mock_UserAuthorization_GetUserInformation(suppliedUserInformation);
            Mock_UserRolesProvider_GetUserRoles(GetUserRolesResult());
            Mock_FormsAuthenticationService_PerformBrowserLogin();
            Mock_View_ShowLoginMessage();
        }

        [Test]
        public void Should_save_user_information_to_session_state()
        {
            Assert.That(sessionStateProvider[EdFiApp.Session.UserInformation], Is.SameAs(suppliedUserInformation));
        }
    }

    public class When_role_is_teacher :
        When_redirect_succeeds
    {
        protected override UserInformation GetUserInformationResult()
        {
            // Note that the school used for initial redirect is the first one passed (i.e. SCHOOL_2 here).
            return CreateUserInformationWithTwoOrganizations(TEACHER_1, SCHOOL_2, SCHOOL_1);
        }

        protected override Role[] GetUserRolesResult()
        {
            return new Role[] { Role.Teacher,};
        }

        [Test]
        public void Should_redirect_to_teacher_landing_page()
        {
            Assert.That(getRedirectArgs.MethodInvoked, "View Redirect not invoked.");
            Assert.That(getRedirectArgs.First<string>(), 
                Is.EqualTo(EdFiWebFormsDashboards.Site.Staff.GeneralOverview(SCHOOL_2, TEACHER_1)));
        }
    }

    public class When_role_is_teacher_and_school_not_found :
        When_redirect_fails
    {
        protected override UserInformation GetUserInformationResult()
        {
            return CreateUserInformation(TEACHER_1, EducationOrganizationCategory.LocalEducationAgency, LOCALEDUCATIONAGENCY_0);
        }

        protected override Role[] GetUserRolesResult()
        {
            return new [] { Role.Teacher,};
        }

        [Test]
        public void Should_display_message_that_school_was_not_found()
        {
            Assert.That(showLoginMessageArgs.MethodInvoked, "Login message not displayed.");
            Assert.That(showLoginMessageArgs.First<string>(), Is.EqualTo(Authentication.AssociatedSchoolCouldNotBeIdentified));
        }
    }

    public class When_role_is_principal :
        When_redirect_succeeds
    {
        protected override UserInformation GetUserInformationResult()
        {
            return CreateUserInformationWithTwoOrganizations(PRINCIPAL_1, SCHOOL_2, SCHOOL_1);
        }

        protected override Role[] GetUserRolesResult()
        {
            return new [] { Role.Principal, };
        }

        [Test]
        public void Should_redirect_to_principal_landing_page()
        {
            Assert.That(getRedirectArgs.MethodInvoked, "View Redirect not invoked.");
            Assert.That(getRedirectArgs.First<string>(),
                    Is.EqualTo(EdFiWebFormsDashboards.Site.School.SchoolInformation(SCHOOL_2)));
        }
    }

    public class When_role_is_principal_and_school_not_found :
        When_redirect_fails
    {
        protected override UserInformation GetUserInformationResult()
        {
            UserInformation userInfo = CreateUserInformation(PRINCIPAL_1, EducationOrganizationCategory.LocalEducationAgency, LOCALEDUCATIONAGENCY_0);
            return userInfo;
        }

        protected override Role[] GetUserRolesResult()
        {
            var roles = new [] { Role.Principal, };
            return roles;
        }

        [Test]
        public void Should_display_message_that_school_was_not_found()
        {
            Assert.That(showLoginMessageArgs.MethodInvoked, "Login message not displayed.");
            Assert.That(showLoginMessageArgs.First<string>(), Is.EqualTo(Authentication.AssociatedSchoolCouldNotBeIdentified));
        }
    }

    public class When_role_is_specialist :
        When_redirect_succeeds
    {
        protected override UserInformation GetUserInformationResult()
        {
            return CreateUserInformationWithTwoOrganizations(SPECIALIST_1, SCHOOL_2, SCHOOL_1);
        }

        protected override Role[] GetUserRolesResult()
        {
            return new Role[] { Role.SchoolSpecialist, };
        }

        [Test]
        public void Should_redirect_to_specialist_landing_page_for_first_school_association()
        {
            Assert.That(getRedirectArgs.MethodInvoked, "View Redirect not invoked.");

            Assert.That(getRedirectArgs.First<string>(), 
                Is.EqualTo(EdFiWebFormsDashboards.Site.Staff.GeneralOverview(SCHOOL_2, SPECIALIST_1)));
        }
    }

    public class When_role_is_specialist_and_school_not_found :
        When_redirect_fails
    {
        protected override UserInformation GetUserInformationResult()
        {
            return CreateUserInformation(SPECIALIST_1, EducationOrganizationCategory.LocalEducationAgency, LOCALEDUCATIONAGENCY_0);
        }

        protected override Role[] GetUserRolesResult()
        {
            return new Role[] { Role.SchoolSpecialist, };
        }

        [Test]
        public void Should_display_message_that_school_was_not_found()
        {
            Assert.That(showLoginMessageArgs.MethodInvoked, "Login message not displayed.");
            Assert.That(showLoginMessageArgs.First<string>(), Is.EqualTo(Authentication.AssociatedSchoolCouldNotBeIdentified));
        }
    }
}
*/