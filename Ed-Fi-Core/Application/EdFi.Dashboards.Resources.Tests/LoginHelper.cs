// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Tests
{
    public class LoginHelper
    {
        public static long staffUSILocalEducationAgencyAdministrator = 1000;
        public static long staffUSILocalEducationAgencySystemAdministrator = 2000;
        public static long staffUSIPrincipalOneSchool = 2501;
        public static long staffUSIPrincipalTwoSchool = 2502;
        public static long staffUSISchoolAdministrator = 3000;
        public static long staffUSISchoolSpecialistOne = 3501;
        public static long staffUSISchoolSpecialistTwo = 3502;
        public static long staffUSISuperintendent = 4000;
        public static long staffUSITeacherOne = 5001;
        public static long staffUSITeacherTwo = 5002;
        

        public static long staffUSIStateLevelClaim = 6000;
        public static long staffUSILocalEducationAgency = 7000;
        public static long staffUSISchool = 8000;


        public static int stateOneId = 9000;
        

        public static int localEducationAgencyOneId = 10000;
        public static int localEducationAgencyTwoId = 20000;

        public static int schoolOneId = 20001;
        public static int schoolTwoId = 20002;

        public static List<string> claimTypesLocalEducationAgencyAdministrator =
            new List<String>
                {
                    EdFiClaimTypes.AccessOrganization,
                    EdFiClaimTypes.ViewMyMetrics,
                    EdFiClaimTypes.ManageGoals,
                };

        public static List<string> claimTypesLocalEducationAgencySystemAdministrator =
            new List<String>
                {
                    EdFiClaimTypes.AccessOrganization,
                    EdFiClaimTypes.ViewAllMetrics,
                    EdFiClaimTypes.ViewAllStudents,
                    EdFiClaimTypes.ViewAllTeachers,
                    EdFiClaimTypes.AdministerDashboard,
                    EdFiClaimTypes.ViewOperationalDashboard,
                    EdFiClaimTypes.ManageGoals,
                };

        public static List<string> claimTypesPrincipalSchool =
            new List<String>
                {
                    EdFiClaimTypes.AccessOrganization,
                    EdFiClaimTypes.ViewAllStudents,
                    EdFiClaimTypes.ViewAllMetrics,
                    EdFiClaimTypes.ViewOperationalDashboard,
                    EdFiClaimTypes.ViewAllTeachers,
                };
        public static List<string> claimTypesPrincipalLea =
    new List<String>
                {
                    EdFiClaimTypes.ViewMyMetrics
                };

        public static List<string> claimTypesSchoolAdministrator =
            new List<String>
                {
                    EdFiClaimTypes.AccessOrganization,
                    EdFiClaimTypes.ViewMyMetrics,
                };

        public static List<string> claimTypesSchoolSpecialist =
            new List<String>
                {
                    EdFiClaimTypes.AccessOrganization,
                    EdFiClaimTypes.ViewMyMetrics,
                    EdFiClaimTypes.ViewMyStudents,
                };

        public static List<string> claimTypesSuperintendent =
            new List<String>
                {
                    EdFiClaimTypes.AccessOrganization,
                    EdFiClaimTypes.ViewAllStudents,
                    EdFiClaimTypes.ViewAllMetrics,
                    EdFiClaimTypes.ViewOperationalDashboard,
                    EdFiClaimTypes.ViewAllTeachers
                };

        public static List<string> claimTypesTeacher =
            new List<String>
                {
                    EdFiClaimTypes.AccessOrganization,
                    EdFiClaimTypes.ViewMyMetrics,
                    EdFiClaimTypes.ViewMyStudents,
                };

        public static List<string> claimTypesTeacherImpersonation =
            new List<String>
                {
                    EdFiClaimTypes.AccessOrganization,
                    EdFiClaimTypes.ViewMyMetrics,
                    EdFiClaimTypes.ViewMyStudents,
                    EdFiClaimTypes.Impersonating
                };

        public static UserInformation userInfoWithStateLevelClaim =
            new UserInformation
            {
                StaffUSI = staffUSIStateLevelClaim,
                FullName = "User With State Level Claim",
                AssociatedOrganizations = new List<UserInformation.StateAgency>
                                                  {
                                                      new UserInformation.StateAgency
                                                          (stateOneId)
                                                          {
                                                              Name = "State",
                                                              ClaimTypes = new List<String> { EdFiClaimTypes.ViewAllStudents }
                                                          }
                                                  }
            };

        public static UserInformation userInfoWithLocalEducationAgencyClaim =
           new UserInformation
           {
               StaffUSI = staffUSILocalEducationAgency,
               FullName = "User With LEA Level Claim",
               AssociatedOrganizations = new List<UserInformation.LocalEducationAgency>
                                                  {
                                                      new UserInformation.LocalEducationAgency
                                                          (localEducationAgencyOneId)
                                                          {
                                                              Name = "Local Education Agency",
                                                              ClaimTypes = new List<String> { EdFiClaimTypes.ViewAllStudents }
                                                          }
                                                  }
           };

        public static UserInformation userInfoWithSchoolViewAllStudentsClaim =
           new UserInformation
           {
               StaffUSI = staffUSISchool,
               FullName = "User With School Claim",
               AssociatedOrganizations = new List<UserInformation.School>
                                                  {
                                                      new UserInformation.School
                                                          (localEducationAgencyOneId, schoolOneId)
                                                          {
                                                              Name = "School",
                                                              ClaimTypes = new List<String> { EdFiClaimTypes.ViewAllStudents }
                                                          }
                                                  }
           };

        public static UserInformation userInfoWithSchoolViewMyStudentsClaim =
           new UserInformation
           {
               StaffUSI = staffUSISchool,
               FullName = "User With School Claim",
               AssociatedOrganizations = new List<UserInformation.School>
                                                  {
                                                      new UserInformation.School
                                                          (localEducationAgencyTwoId, schoolTwoId)
                                                          {
                                                              Name = "School",
                                                              ClaimTypes = new List<String> { EdFiClaimTypes.ViewMyStudents }
                                                          }
                                                  }
           };

        public static UserInformation userInfoLocalEducationAgencyAdministrator =
            new UserInformation
            {
                StaffUSI = staffUSILocalEducationAgencyAdministrator,
                FullName = "LocalEducationAgencyAdministrator",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.LocalEducationAgency
                                                          (localEducationAgencyOneId)
                                                          {
                                                              Name = "LEA",
                                                              ClaimTypes = claimTypesLocalEducationAgencyAdministrator
                                                          }
                                                  }
            };

        public static UserInformation userInfoLocalEducationAgencySystemAdministratorOne =
            new UserInformation
            {
                StaffUSI = staffUSILocalEducationAgencySystemAdministrator,
                FullName = "SystemAdministrator",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.LocalEducationAgency
                                                          (localEducationAgencyOneId)
                                                          {
                                                              Name = "LEA",
                                                              ClaimTypes = claimTypesLocalEducationAgencySystemAdministrator
                                                          }
                                                  }
            };

        public static UserInformation userInfoLocalEducationAgencySystemAdministratorTwo =
            new UserInformation
            {
                StaffUSI = staffUSILocalEducationAgencySystemAdministrator,
                FullName = "SystemAdministrator",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.LocalEducationAgency
                                                          (localEducationAgencyTwoId)
                                                          {
                                                              Name = "LEA",
                                                              ClaimTypes = claimTypesLocalEducationAgencySystemAdministrator
                                                          }
                                                  }
            };

        public static UserInformation userInfoPrincipalOneSchool =
            new UserInformation
            {
                StaffUSI = staffUSIPrincipalOneSchool,
                FullName = "Principal One School",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.School(localEducationAgencyOneId, schoolOneId)
                                                          {
                                                              Name = "School One",
                                                              ClaimTypes = claimTypesPrincipalSchool
                                                          },
                                                      new UserInformation.LocalEducationAgency(localEducationAgencyOneId)
                                                          {
                                                              Name = "LEA",
                                                              ClaimTypes = claimTypesPrincipalLea
                                                          }
                                                  }

            };

        public static UserInformation userInfoPrincipalTwoSchool =
            new UserInformation
            {
                StaffUSI = staffUSIPrincipalTwoSchool,
                FullName = "Principal Two School",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.School(localEducationAgencyOneId, schoolOneId)
                                                          {
                                                              Name = "School One",
                                                              ClaimTypes = claimTypesPrincipalSchool
                                                          },
                                                      new UserInformation.School(localEducationAgencyOneId, schoolTwoId)
                                                          {
                                                              Name = "School Two",
                                                              ClaimTypes = claimTypesPrincipalSchool
                                                          },
                                                      new UserInformation.LocalEducationAgency(localEducationAgencyOneId)
                                                          {
                                                              Name = "LEA",
                                                              ClaimTypes = claimTypesPrincipalLea
                                                          }
                                                  }
            };

        public static UserInformation userInfoSchoolAdministrator =
            new UserInformation
            {
                StaffUSI = staffUSISchoolAdministrator,
                FullName = "SchoolAdministrator",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.School(localEducationAgencyOneId, schoolOneId)
                                                          {
                                                              Name = "School One",
                                                              ClaimTypes = claimTypesSchoolAdministrator
                                                          }
                                                  }
            };

        public static UserInformation userInfoSchoolSpecialistOne =
            new UserInformation
            {
                StaffUSI = staffUSISchoolSpecialistOne,
                FullName = "School Specialist One",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.School(localEducationAgencyOneId, schoolOneId)
                                                          {
                                                              Name = "School One",
                                                              ClaimTypes = claimTypesSchoolSpecialist
                                                          }
                                                  }
            };

        public static UserInformation userInfoSchoolSpecialistTwo =
            new UserInformation
            {
                StaffUSI = staffUSISchoolSpecialistTwo,
                FullName = "School Specialist Two",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.School(localEducationAgencyOneId, schoolOneId)
                                                          {
                                                              Name = "School One",
                                                              ClaimTypes = claimTypesSchoolSpecialist
                                                          },
                                                      new UserInformation.School(localEducationAgencyOneId, schoolTwoId)
                                                          {
                                                              Name = "School Two",
                                                              ClaimTypes = claimTypesSchoolSpecialist
                                                          }
                                                  }
            };



        public static UserInformation userInfoSuperintendent =
            new UserInformation
            {
                StaffUSI = staffUSISuperintendent,
                FullName = "superintendent",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.LocalEducationAgency
                                                          (localEducationAgencyOneId)
                                                          {
                                                              Name = "LEA",
                                                              ClaimTypes = claimTypesSuperintendent
                                                          }
                                                  }
            };

        public static UserInformation userInfoTeacherOne =
            new UserInformation
            {
                StaffUSI = staffUSITeacherOne,
                FullName = "Teacher One",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.School(localEducationAgencyOneId, schoolOneId)
                                                          {
                                                              Name = "School One",
                                                              ClaimTypes = claimTypesTeacher
                                                          }
                                                  }
            };

        public static UserInformation userInfoTeacherTwo =
            new UserInformation
            {
                StaffUSI = staffUSITeacherTwo,
                FullName = "Teacher Two",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.School(localEducationAgencyOneId, schoolOneId)
                                                          {
                                                              Name = "School One",
                                                              ClaimTypes = claimTypesTeacher
                                                          },
                                                      new UserInformation.School(localEducationAgencyOneId, schoolTwoId)
                                                          {
                                                              Name = "School Two",
                                                              ClaimTypes = claimTypesTeacher
                                                          }
                                                  }
            };

        public static UserInformation userInfoTeacherThree =
            new UserInformation
            {
                StaffUSI = staffUSITeacherTwo,
                FullName = "Teacher Three",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.School(localEducationAgencyTwoId, schoolOneId)
                                                          {
                                                              Name = "School One",
                                                              ClaimTypes = claimTypesTeacher
                                                          },
                                                      new UserInformation.School(localEducationAgencyTwoId, schoolTwoId)
                                                          {
                                                              Name = "School Two",
                                                              ClaimTypes = claimTypesTeacher
                                                          }
                                                  }
            };

        public static UserInformation userInfoTeacherOnePrincipalTwo =
    new UserInformation
    {
        StaffUSI = staffUSITeacherTwo,
        FullName = "Teacher Three",
        AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.School(localEducationAgencyTwoId, schoolOneId)
                                                          {
                                                              Name = "School One",
                                                              ClaimTypes = claimTypesTeacher
                                                          },
                                                      new UserInformation.School(localEducationAgencyTwoId, schoolTwoId)
                                                          {
                                                              Name = "School Two",
                                                              ClaimTypes = claimTypesPrincipalSchool
                                                          },
                                                      new UserInformation.LocalEducationAgency(localEducationAgencyTwoId)
                                                          {
                                                              Name = "LEA",
                                                              ClaimTypes = claimTypesPrincipalLea
                                                          }
                                                  }
    };


        public static UserInformation impersonateTeacherOne =
            new UserInformation
            {
                StaffUSI = staffUSITeacherOne,
                FullName = "Teacher One",
                AssociatedOrganizations = new List<UserInformation.EducationOrganization>
                                                  {
                                                      new UserInformation.School(localEducationAgencyOneId, schoolOneId)
                                                          {
                                                              Name = "School One",
                                                              ClaimTypes = claimTypesTeacherImpersonation
                                                          }
                                                  }
            };

        public static void LoginUser(UserInformation userInformation)
        {
            Thread.CurrentPrincipal = userInformation.ToClaimsPrincipal();
        }

        public static void LoginUserTeacherOnePrincipalTwo()
        {
            LoginUser(userInfoTeacherOnePrincipalTwo);
        }

        public static void LoginUserWithStateLevelClaim()
        {
            LoginUser(userInfoWithStateLevelClaim);
        }

        public static void LoginUserWithLocalEducationAgencyClaim()
        {
            LoginUser(userInfoWithLocalEducationAgencyClaim);
        }

        public static void LoginUserWithSchoolViewAllStudentsClaim()
        {
            LoginUser(userInfoWithSchoolViewAllStudentsClaim);
        }

        public static void LoginUserWithSchoolViewMyStudentsClaim()
        {
            LoginUser(userInfoWithSchoolViewMyStudentsClaim);
        }

        public static void LoginLocalEducationAgencyAdministrator()
        {
            LoginUser(userInfoLocalEducationAgencyAdministrator);
        }

        public static void LoginLocalEducationAgencySystemAdministratorOne()
        {
            LoginUser(userInfoLocalEducationAgencySystemAdministratorOne);
        }
        
        public static void LoginLocalEducationAgencySystemAdministratorTwo()
        {
            LoginUser(userInfoLocalEducationAgencySystemAdministratorTwo);
        }

        public static void LoginPrincipalOneSchool()
        {
            LoginUser(userInfoPrincipalOneSchool);
        }

        public static void LoginPrincipalTwoSchool()
        {
            LoginUser(userInfoPrincipalTwoSchool);
        }
        
        public static void LoginSchoolAdministrator()
        {
            LoginUser(userInfoSchoolAdministrator);
        }

        public static void LoginSchoolSpecialistOne()
        {
            LoginUser(userInfoSchoolSpecialistOne);
        }

        public static void LoginSchoolSpecialistTwo()
        {
            LoginUser(userInfoSchoolSpecialistTwo);
        }

        public static void LoginSuperintendent()
        {
            LoginUser(userInfoSuperintendent);
        }

        public static void LoginTeacherOne()
        {
            LoginUser(userInfoTeacherOne);
        }

        public static void LoginTeacherTwo()
        {
            LoginUser(userInfoTeacherTwo);
        }
       
        public static void LoginTeacherThree()
        {
            LoginUser(userInfoTeacherThree);
        }

        public static void ImpersonateTeacherOne()
        {
            LoginUser(impersonateTeacherOne);
        }
    }

}
