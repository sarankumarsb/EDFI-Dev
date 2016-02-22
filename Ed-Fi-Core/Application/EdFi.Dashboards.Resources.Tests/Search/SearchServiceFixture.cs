// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Resources.Models.Search;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Queries;
using EdFi.Dashboards.Resources.Search;

namespace EdFi.Dashboards.Resources.Tests.Search
{
    public class BaseSearchServiceFixture : TestFixtureBase
    {
        protected const int suppliedLocalEducationAgencyId = 2000;
        protected const int suppliedSchoolId = 1000;
        protected const int suppliedStaffUSI = 3000;
        protected const int suppliedStudentUSI = 4000;
        protected const int suppliedMetricNodeId = 7;
	    protected const int suppliedIdentificationCode = 900;

        protected IRepository<SchoolInformation> schoolInformationRepository;
        protected IRepository<StudentInformation> studentInformationRepository;
        protected IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        protected IRepository<StaffInformation> staffInformationRepository;
        protected IRepository<StaffEducationOrgInformation> staffEducationOrganizationRepository;
        protected IRepository<StaffSectionCohortAssociation> staffSectionCohortAssociationRepository;
        protected IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        protected IRepository<StaffIdentificationCode> staffIdentificationCodeRepository;
        protected IRepository<StudentIdentificationCode> studentIdentificationCodeRepository; 
        protected StaffInformationAndAssociatedOrganizationsByUSIQuery staffInfoAndOrgQuery;
        protected IStaffInformationLookupKeyProvider StaffInformationLookupKeyProvider;
        protected ILocalEducationAgencySearchProvider searchProvider;

        protected Dictionary<SearchFilter, HashSet<Tuple<int, Type>>> userSearchFilters;
        protected HashSet<Tuple<int, Type>> leaHashKeyTuple = new HashSet<Tuple<int, Type>>
                                                {
                                                    Tuple.Create(suppliedLocalEducationAgencyId,
                                                                 typeof (UserInformation.LocalEducationAgency))
                                                };
        protected HashSet<Tuple<int, Type>> schoolHashKeyTuple = new HashSet<Tuple<int, Type>>
                                                {
                                                    Tuple.Create(suppliedSchoolId,
                                                                 typeof (UserInformation.School)),
                                                    Tuple.Create(suppliedSchoolId + 1,
                                                                 typeof (UserInformation.School)),
                                                };
        protected string suppliedTextToFind;
        protected int suppliedRowCountToReturn;
        protected bool suppliedMatchContains;
        protected SearchFilter suppliedFilter;

        protected SearchModel actualModel;
        protected IStaffAreaLinks staffAreaLinksFake = new StaffAreaLinksFake();
        protected ISchoolAreaLinks schoolAreaLinksFake = new SchoolAreaLinksFake();
        protected IAdminAreaLinks adminAreaLinksFake = new AdminAreaLinksFake();

        protected override void EstablishContext()
        {
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
            studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
            studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
            staffInformationRepository = mocks.StrictMock<IRepository<StaffInformation>>();
            staffEducationOrganizationRepository = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            staffSectionCohortAssociationRepository = mocks.StrictMock<IRepository<StaffSectionCohortAssociation>>();
            localEducationAgencyInformationRepository = mocks.StrictMock<IRepository<LocalEducationAgencyInformation>>();
            staffIdentificationCodeRepository = mocks.StrictMock<IRepository<StaffIdentificationCode>>();
            studentIdentificationCodeRepository = mocks.StrictMock<IRepository<StudentIdentificationCode>>();
            StaffInformationLookupKeyProvider = mocks.StrictMock<IStaffInformationLookupKeyProvider>();
            searchProvider = mocks.StrictMock<ILocalEducationAgencySearchProvider>();

            staffInfoAndOrgQuery = new StaffInformationAndAssociatedOrganizationsByUSIQuery(staffInformationRepository, staffEducationOrganizationRepository, localEducationAgencyInformationRepository, schoolInformationRepository, staffIdentificationCodeRepository);
            
            CreateEdFiDashboardContext(suppliedLocalEducationAgencyId);

            Expect.Call(schoolInformationRepository.GetAll()).Repeat.Any().Return(GetSuppliedSchoolInformationData());
            Expect.Call(studentInformationRepository.GetAll()).Repeat.Any().Return(GetSuppliedStudentInformationData());
            Expect.Call(studentSchoolInformationRepository.GetAll()).Repeat.Any().Return(GetSuppliedStudentSchoolInformationData());
            Expect.Call(staffInformationRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffInformationData());
            Expect.Call(staffEducationOrganizationRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffEducationOrganizationData());
            Expect.Call(staffSectionCohortAssociationRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffSectionCohortAssociationData());
            Expect.Call(localEducationAgencyInformationRepository.GetAll()).Repeat.Any().Return(GetSuppliedLocalEducationAgencyInformationData());
            Expect.Call(staffIdentificationCodeRepository.GetAll()).Repeat.Any().Return(GetSuppliedStaffIdentificationCodeData());
            Expect.Call(studentIdentificationCodeRepository.GetAll()).Repeat.Any().Return(GetSuppliedStudentIdentificationCodeData());
            
            //This is only needed for tests that search teachers.  So make the calls to it optional.
            Expect.Call(StaffInformationLookupKeyProvider.GetStaffInformationLookupKey()).Return("EmailAddress").Repeat.Any();

            // This is only for Student searches
            Expect.Call(searchProvider.GetEnabledStudentIdentificationSystems(suppliedLocalEducationAgencyId))
                .Return(GetSuppliedStudentIdentificationSystems())
                .Repeat.Any();

            // This is only for Staff searches
            Expect.Call(searchProvider.GetEnabledStaffIdentificationSystems(suppliedLocalEducationAgencyId))
                .Return(GetSuppliedStaffIdentificationSystems())
                .Repeat.Any();

            userSearchFilters = new Dictionary<SearchFilter, HashSet<Tuple<int, Type>>>
                                    {
                                        {SearchFilter.Schools,leaHashKeyTuple},
                                        {SearchFilter.Staff,leaHashKeyTuple},
                                        {SearchFilter.Teachers,leaHashKeyTuple},
                                        {SearchFilter.Students,leaHashKeyTuple}
                                    };
            
            base.EstablishContext();
            
        }

        private string[] GetSuppliedStudentIdentificationSystems()
        {
            return new string[]{"District"};
        }

        private string[] GetSuppliedStaffIdentificationSystems()
        {
            return new string[] {"District"};
        }

        protected MetricMetadataNode GetSuppliedRootNode()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree()) { MetricNodeId = 1 };
        }

        protected MetricMetadataNode GetStudentRootOverviewNode()
        {
            var tree = new TestMetricMetadataTree();

            return new MetricMetadataNode(tree)
            {
                MetricId = 2,
                Name = "Student's Overview",
                MetricNodeId = suppliedMetricNodeId,
                Children = new List<MetricMetadataNode>
                            {
                                new MetricMetadataNode(tree){MetricId=21, MetricNodeId = 71, Name = "Student's Attendance and Discipline",
                                Children = new List<MetricMetadataNode>
                                                {
                                                    new MetricMetadataNode(tree){MetricId=211, MetricNodeId = 711, Name = "Attendance"},
                                                    new MetricMetadataNode(tree){MetricId=212, Name = "Discipline"} 
                                                }
                                },
                                new MetricMetadataNode(tree){MetricId=22, MetricNodeId = 72, Name = "School's Other Metric"},
                            }
            };
        }
        private IQueryable<LocalEducationAgencyInformation> GetSuppliedLocalEducationAgencyInformationData()
        {
            var data = new List<LocalEducationAgencyInformation>
                           {
                               new LocalEducationAgencyInformation { LocalEducationAgencyId = 9999, Name = "wrong data" },
                               new LocalEducationAgencyInformation { LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "elephant"}
                           };
            return data.AsQueryable();
        }

        private IQueryable<StaffIdentificationCode> GetSuppliedStaffIdentificationCodeData()
        {
            var data = new List<StaffIdentificationCode>
            {
                                new StaffIdentificationCode { StaffUSI = 999, IdentificationCode = "999", AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                //new StaffIdentificationCode { StaffUSI = 998, FullName = searchText, FirstName = "Apple", MiddleName = "banana", LastSurname = "carrot", EmailAddress = "email2@test.com" },
                                //new StaffIdentificationCode { StaffUSI = 997, FullName = "wrong data", FirstName = splitText[0], MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1), EmailAddress = "email01@test.com" },
                                //new StaffIdentificationCode { StaffUSI = 996, FullName = "wrong data", FirstName = splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 2), EmailAddress = "email01@test.com" },

                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI, IdentificationCode = suppliedIdentificationCode.ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 1,  IdentificationCode = (suppliedIdentificationCode + 1).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 2,  IdentificationCode = (suppliedIdentificationCode + 2).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 3,  IdentificationCode = (suppliedIdentificationCode + 3).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 4,  IdentificationCode = (suppliedIdentificationCode + 4).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 6, IdentificationCode = (suppliedIdentificationCode + 5).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 7, IdentificationCode = (suppliedIdentificationCode + 6).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 8, IdentificationCode = (suppliedIdentificationCode + 7).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 9, IdentificationCode = (suppliedIdentificationCode + 8).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},

                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 10, IdentificationCode = (suppliedIdentificationCode + 9).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 11, IdentificationCode = (suppliedIdentificationCode + 10).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 12, IdentificationCode = (suppliedIdentificationCode + 11).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 13, IdentificationCode = (suppliedIdentificationCode + 12).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 14, IdentificationCode = (suppliedIdentificationCode + 13).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 15, IdentificationCode = (suppliedIdentificationCode + 14).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 16, IdentificationCode = (suppliedIdentificationCode + 15).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 17, IdentificationCode = (suppliedIdentificationCode + 16).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 18, IdentificationCode = (suppliedIdentificationCode + 17).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 19, IdentificationCode = (suppliedIdentificationCode + 18).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 20, IdentificationCode = (suppliedIdentificationCode + 19).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 21, IdentificationCode = (suppliedIdentificationCode + 20).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 22, IdentificationCode = (suppliedIdentificationCode + 21).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 23, IdentificationCode = (suppliedIdentificationCode + 22).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 24, IdentificationCode = (suppliedIdentificationCode + 23).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 25, IdentificationCode = (suppliedIdentificationCode + 24).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 26, IdentificationCode = (suppliedIdentificationCode + 25).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 27, IdentificationCode = (suppliedIdentificationCode + 26).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 28, IdentificationCode = (suppliedIdentificationCode + 27).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 29, IdentificationCode = (suppliedIdentificationCode + 28).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 30, IdentificationCode = (suppliedIdentificationCode + 29).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 31, IdentificationCode = (suppliedIdentificationCode + 30).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 32, IdentificationCode = (suppliedIdentificationCode + 31).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                                new StaffIdentificationCode { StaffUSI = suppliedStaffUSI + 33, IdentificationCode = (suppliedIdentificationCode + 32).ToString(), AssigningOrganizationCode="x", StaffIdentificationSystemType = "District"},
                               
            };
            return data.AsQueryable();
        }

        private IQueryable<StudentIdentificationCode> GetSuppliedStudentIdentificationCodeData()
        {
            var data = new List<StudentIdentificationCode>
            {
                                new StudentIdentificationCode { StudentUSI = 999, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },

                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI,IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },

                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 1, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },

                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 2, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },

                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 3, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },

                                
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 6, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },

                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 7, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },

                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 8, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },

                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 9, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },

                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 10, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },

                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 11, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 12, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 13,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 14,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 15,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 16,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 17,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 18,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 19,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 20,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 21,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 22, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 23,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 24,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 25, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 26,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 27,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 28, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 29,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 30,  IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
                                new StudentIdentificationCode { StudentUSI = suppliedStudentUSI + 31, IdentificationCode = "999",AssigningOrganizationCode = "x", StudentIdentificationSystemType = "District" },
            };
            return data.AsQueryable();
        }

        protected virtual IQueryable<StaffSectionCohortAssociation> GetSuppliedStaffSectionCohortAssociationData()
        {
            var data = new List<StaffSectionCohortAssociation>
                            {
                                new StaffSectionCohortAssociation { StaffUSI = 999, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = 998, EducationOrganizationId = 999},
                                new StaffSectionCohortAssociation { StaffUSI = 997, EducationOrganizationId = 999},
                                new StaffSectionCohortAssociation { StaffUSI = 996, EducationOrganizationId = 999},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId, SectionIdOrCohortId = 1},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId, SectionIdOrCohortId = 2},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 1, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 2, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 3, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 5, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 6, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 7, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 8, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 9, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 10, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 11, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 12, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 13, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 14, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 15, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 16, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 17, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 18, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 19, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 20, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 21, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 22, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 23, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 24, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 25, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 26, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 27, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 28, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 29, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 30, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 31, EducationOrganizationId = suppliedSchoolId},
                            };
            return data.AsQueryable();
        }

        private IQueryable<StaffEducationOrgInformation> GetSuppliedStaffEducationOrganizationData()
        {
            var data = new List<StaffEducationOrgInformation>
                            {
                                new StaffEducationOrgInformation { StaffUSI = 999, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = 998, EducationOrganizationId = 999},
                                new StaffEducationOrgInformation { StaffUSI = 997, EducationOrganizationId = 999},
                                new StaffEducationOrgInformation { StaffUSI = 996, EducationOrganizationId = 999},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId + 1},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 1, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 1, EducationOrganizationId = suppliedSchoolId + 1},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 2, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 2, EducationOrganizationId = suppliedSchoolId + 2},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 3, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 4, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 5, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 6, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 7, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 8, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 9, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 10, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 11, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 12, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 13, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 14, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 15, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 16, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 17, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 18, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 19, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 20, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 21, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 22, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 23, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 24, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 25, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 26, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 27, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 28, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 29, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 30, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 31, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 32, EducationOrganizationId = suppliedSchoolId},
                                new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 33, EducationOrganizationId = suppliedSchoolId},
                                //new StaffEducationOrgInformation { StaffUSI = suppliedStaffUSI + 314, EducationOrganizationId = suppliedSchoolId + 314},
                            };
            return data.AsQueryable();
        }

        protected virtual IQueryable<StaffInformation> GetSuppliedStaffInformationData()
        {
            var splitText = suppliedTextToFind.Split(' ');
            var searchText = suppliedMatchContains && splitText.Length > 3 ? suppliedTextToFind.Replace(' ', '%') : suppliedTextToFind;
            var data = new List<StaffInformation>
                            {
                                new StaffInformation { StaffUSI = 999, FullName = "wrong data", FirstName = "Apple", MiddleName = "banana", LastSurname = "carrot", EmailAddress = "email1@test.com" },
                                //new StaffInformation { StaffUSI = 998, FullName = searchText, FirstName = "Apple", MiddleName = "banana", LastSurname = "carrot", EmailAddress = "email2@test.com" },
                                //new StaffInformation { StaffUSI = 997, FullName = "wrong data", FirstName = splitText[0], MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1), EmailAddress = "email01@test.com" },
                                //new StaffInformation { StaffUSI = 996, FullName = "wrong data", FirstName = splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 2), EmailAddress = "email01@test.com" },

                                new StaffInformation { StaffUSI = suppliedStaffUSI, FullName = searchText, FirstName = "Apple", MiddleName = "banana", LastSurname = "elephant", EmailAddress = "email3@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 1, FullName = "begin text " + searchText, FirstName = "Arrow", MiddleName = "banana", LastSurname = "cat", EmailAddress = "email4@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 2, FullName = searchText + " end text", FirstName = "Apple", MiddleName = "banana", LastSurname = "cat", EmailAddress = "email5@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 3, FullName = "begin text " + searchText + " end text", FirstName = "Apple", MiddleName = "banana", LastSurname = "corn", EmailAddress = "email6@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 4, FullName = searchText + " end text 2", FirstName = "Apple", MiddleName = "banana", LastSurname = "ape", EmailAddress = "email7@test.com" },
                                
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 6, FullName = "not the search text", FirstName = splitText[0], MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1), EmailAddress = "email9@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 7, FullName = "not the search text", FirstName = splitText[0] + " end text", MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1) + " end text", EmailAddress = "email10@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 8, FullName = "not the search text", FirstName = "begin text " + splitText[0], MiddleName = "banana", LastSurname = "begin text" + GetSplitStringFromPosition(splitText, 1), EmailAddress = "email11@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 9, FullName = "not the search text", FirstName = "begin text " + splitText[0] + " end text", MiddleName = "banana", LastSurname = "begin text " + GetSplitStringFromPosition(splitText, 1) + " end text", EmailAddress = "email12@test.com" },

                                new StaffInformation { StaffUSI = suppliedStaffUSI + 10, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 1), MiddleName = "banana", LastSurname = splitText[0], EmailAddress = "email13@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 11, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 1) + " end text", MiddleName = "banana", LastSurname = splitText[0] + " end text", EmailAddress = "email14@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 12, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 1), MiddleName = "banana", LastSurname = "begin text" + splitText[0], EmailAddress = "email15@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 13, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 1) + " end text", MiddleName = "banana", LastSurname = "begin text " + splitText[0] + " end text", EmailAddress = "email16@test.com" },
                                
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 14, FullName = "not the search text", FirstName = splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 2), EmailAddress = "email17@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 15, FullName = "not the search text", FirstName = splitText[0] + " end text", MiddleName = GetSplitStringFromPosition(splitText, 1) + " end text", LastSurname = GetSplitStringFromPosition(splitText, 2) + " end text", EmailAddress = "email18@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 16, FullName = "not the search text", FirstName = "begin text " + splitText[0], MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 1), LastSurname = "begin text" + GetSplitStringFromPosition(splitText, 2), EmailAddress = "email19@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 17, FullName = "not the search text", FirstName = "begin text " + splitText[0] + " end text", MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 1) + " end text", LastSurname = "begin text " + GetSplitStringFromPosition(splitText, 2) + " end text", EmailAddress = "email20@test.com" },
                                
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 18, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 1), MiddleName = GetSplitStringFromPosition(splitText, 2), LastSurname = splitText[0], EmailAddress = "email21@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 19, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 1) + " end text", MiddleName = GetSplitStringFromPosition(splitText, 2) + " end text", LastSurname = splitText[0] + " end text", EmailAddress = "email22@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 20, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 1), MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 2), LastSurname = "begin text" + splitText[0], EmailAddress = "email23@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 21, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 1) + " end text", MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 2) + " end text", LastSurname = "begin text " + splitText[0] + " end text", EmailAddress = "email24@test.com" },
                                
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 22, FullName = "not the search text", FirstName = splitText[0], MiddleName = "banana", LastSurname = "begin text " + GetSplitStringFromPosition(splitText, 1), EmailAddress = "email25@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 23, FullName = "not the search text", FirstName = "begin text " + splitText[0], MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1), EmailAddress = "email26@test.com" },
                                
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 24, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 1), MiddleName = "banana", LastSurname = "begin text " + splitText[0], EmailAddress = "email27@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 25, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 1), MiddleName = "banana", LastSurname = splitText[0], EmailAddress = "email28@test.com" },
                                
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 26, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 2), MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = "begin text " + splitText[0], EmailAddress = "email29@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 27, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 2), MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = splitText[0], EmailAddress = "email30@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 28, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 2), MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 1), LastSurname = splitText[0], EmailAddress = "email31@test.com" },
                                
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 29, FullName = "not the search text", FirstName = splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = "begin text " + GetSplitStringFromPosition(splitText, 2), EmailAddress = "email32@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 30, FullName = "not the search text", FirstName = "begin text " + splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 2), EmailAddress = "email33@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 31, FullName = "not the search text", FirstName = splitText[0], MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 2), EmailAddress = "email34@test.com" },
                                
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 32, FullName = "not the search text", FirstName = splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 2), EmailAddress = "email35@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 33, FullName = "not the search text", FirstName = splitText[0], MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1), EmailAddress = "email36@test.com" },
                                //new StaffInformation { StaffUSI = suppliedStaffUSI + 314, FullName = searchText, FirstName = splitText[0], MiddleName = "Jack", LastSurname = "Sparrow", EmailAddress = "email314@test.com"},
                            };  
            return data.AsQueryable();
        }

        private IQueryable<StudentSchoolInformation> GetSuppliedStudentSchoolInformationData()
        {
            var data = new List<StudentSchoolInformation>
                            {
                                new StudentSchoolInformation { StudentUSI = 999, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = 998, SchoolId = 999},
                                new StudentSchoolInformation { StudentUSI = 997, SchoolId = 999},
                                new StudentSchoolInformation { StudentUSI = 996, SchoolId = 999},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 1, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 2, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 3, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 4, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 5, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 6, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 7, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 8, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 9, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 10, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 11, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 12, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 13, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 14, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 15, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 16, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 17, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 18, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 19, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 20, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 21, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 22, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 23, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 24, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 25, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 26, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 27, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 28, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 29, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 30, SchoolId = suppliedSchoolId},
                                new StudentSchoolInformation { StudentUSI = suppliedStudentUSI + 31, SchoolId = suppliedSchoolId},
                            };
            return data.AsQueryable();
        }

        private IQueryable<StudentInformation> GetSuppliedStudentInformationData()
        {
            var splitText = suppliedTextToFind.Split(' ');
            var searchText = suppliedMatchContains && splitText.Length > 3 ? suppliedTextToFind.Replace(' ', '%') : suppliedTextToFind;
            var data = new List<StudentInformation>
                            {
                                new StudentInformation { StudentUSI = 999, FullName = "wrong data", FirstName = "Apple", MiddleName = "banana", LastSurname = "carrot" },
                                //new StudentInformation { StudentUSI = 998, FullName = searchText, FirstName = "Apple", MiddleName = "banana", LastSurname = "carrot" },
                                //new StudentInformation { StudentUSI = 997, FullName = "wrong data", FirstName = splitText[0], MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1), EmailAddress = "email01@test.com" },
                                //new StudentInformation { StudentUSI = 996, FullName = "wrong data", FirstName = splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 2), EmailAddress = "email01@test.com" },

                                new StudentInformation { StudentUSI = suppliedStudentUSI, FullName = searchText, FirstName = "Apple", MiddleName = "banana", LastSurname = "elephant" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 1, FullName = "begin text " + searchText, FirstName = "Arrow", MiddleName = "banana", LastSurname = "cat" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 2, FullName = searchText + " end text", FirstName = "Apple", MiddleName = "banana", LastSurname = "cat" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 3, FullName = "begin text " + searchText + " end text", FirstName = "Apple", MiddleName = "banana", LastSurname = "ape" },
                                
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 6, FullName = "not the search text", FirstName = splitText[0], MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1), EmailAddress = "email9@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 7, FullName = "not the search text", FirstName = splitText[0] + " end text", MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1) + " end text", EmailAddress = "email10@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 8, FullName = "not the search text", FirstName = "begin text " + splitText[0], MiddleName = "banana", LastSurname = "begin text" + GetSplitStringFromPosition(splitText, 1), EmailAddress = "email11@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 9, FullName = "not the search text", FirstName = "begin text " + splitText[0] + " end text", MiddleName = "banana", LastSurname = "begin text " + GetSplitStringFromPosition(splitText, 1) + " end text", EmailAddress = "email12@test.com" },
                                
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 10, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 1), MiddleName = "banana", LastSurname = splitText[0], EmailAddress = "email13@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 11, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 1) + " end text", MiddleName = "banana", LastSurname = splitText[0] + " end text", EmailAddress = "email14@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 12, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 1), MiddleName = "banana", LastSurname = "begin text" + splitText[0], EmailAddress = "email15@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 13, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 1) + " end text", MiddleName = "banana", LastSurname = "begin text " + splitText[0] + " end text", EmailAddress = "email16@test.com" },
                                
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 14, FullName = "not the search text", FirstName = splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 2), EmailAddress = "email17@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 15, FullName = "not the search text", FirstName = splitText[0] + " end text", MiddleName = GetSplitStringFromPosition(splitText, 1) + " end text", LastSurname = GetSplitStringFromPosition(splitText, 2) + " end text", EmailAddress = "email18@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 16, FullName = "not the search text", FirstName = "begin text " + splitText[0], MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 1), LastSurname = "begin text" + GetSplitStringFromPosition(splitText, 2), EmailAddress = "email19@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 17, FullName = "not the search text", FirstName = "begin text " + splitText[0] + " end text", MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 1) + " end text", LastSurname = "begin text " + GetSplitStringFromPosition(splitText, 2) + " end text", EmailAddress = "email20@test.com" },
                                
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 18, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 2), MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = splitText[0], EmailAddress = "email21@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 19, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 2) + " end text", MiddleName = GetSplitStringFromPosition(splitText, 1) + " end text", LastSurname = splitText[0] + " end text", EmailAddress = "email22@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 20, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 2), MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 1), LastSurname = "begin text" + splitText[0], EmailAddress = "email23@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 21, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 2) + " end text", MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 1) + " end text", LastSurname = "begin text " + splitText[0] + " end text", EmailAddress = "email24@test.com" },
                                
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 22, FullName = "not the search text", FirstName = splitText[0], MiddleName = "banana", LastSurname = "begin text " + GetSplitStringFromPosition(splitText, 1), EmailAddress = "email25@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 23, FullName = "not the search text", FirstName = "begin text " + splitText[0], MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1), EmailAddress = "email26@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 24, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 1), MiddleName = "banana", LastSurname = "begin text " + splitText[0], EmailAddress = "email27@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 25, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 1), MiddleName = "banana", LastSurname = splitText[0], EmailAddress = "email28@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 26, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 2), MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = "begin text " + splitText[0], EmailAddress = "email29@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 27, FullName = "not the search text", FirstName = "begin text " + GetSplitStringFromPosition(splitText, 2), MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = splitText[0], EmailAddress = "email30@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 28, FullName = "not the search text", FirstName = GetSplitStringFromPosition(splitText, 2), MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 1), LastSurname = splitText[0], EmailAddress = "email31@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 29, FullName = "not the search text", FirstName = splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = "begin text " + GetSplitStringFromPosition(splitText, 1), EmailAddress = "email32@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 30, FullName = "not the search text", FirstName = "begin text " + splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 1), EmailAddress = "email33@test.com" },
                                new StudentInformation { StudentUSI = suppliedStudentUSI + 31, FullName = "not the search text", FirstName = splitText[0], MiddleName = "begin text " + GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 2), EmailAddress = "email34@test.com" },
                                
                            };
            return data.AsQueryable();
        }

        protected IQueryable<SchoolInformation> GetSuppliedSchoolInformationData()
        {
            var searchText = suppliedMatchContains ? suppliedTextToFind.Replace(' ', '%') : suppliedTextToFind;
            var data = new List<SchoolInformation>
                           {
                               //new SchoolInformation { SchoolId = suppliedSchoolId + 314, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "PI School", City = searchText},
                               new SchoolInformation { SchoolId = 999, LocalEducationAgencyId = 9999, Name = "wrong data", City = searchText},
                               //new SchoolInformation { SchoolId = 998, LocalEducationAgencyId = 9999, Name = searchText},
                               new SchoolInformation { SchoolId = 997, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "wrong data", City = searchText},
                               new SchoolInformation { SchoolId = suppliedSchoolId, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = searchText},
                               new SchoolInformation { SchoolId = suppliedSchoolId + 1, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = searchText + " end text"},
                               new SchoolInformation { SchoolId = suppliedSchoolId + 2, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "beginning text " + searchText},
                               new SchoolInformation { SchoolId = suppliedSchoolId + 3, LocalEducationAgencyId = suppliedLocalEducationAgencyId, Name = "beginning text " + searchText + " end text"}
                           };

            return data.AsQueryable();
        }

        private string GetSplitStringFromPosition(string[] splitText, int position)
        {
            return splitText.Length > position ? splitText[position] : "orange";
        }

        protected void CreateEdFiDashboardContext(int localEducationAgencyId)
        {
            var dashboardContext = new EdFiDashboardContext
            {
                LocalEducationAgencyId = localEducationAgencyId
            };

            CallContext.SetData(EdFiDashboardContext.CallContextKey, dashboardContext);
        }

        protected override void ExecuteTest()
        {
            var service = new SearchService(schoolInformationRepository, studentInformationRepository, studentSchoolInformationRepository, staffInfoAndOrgQuery, staffInformationRepository, staffEducationOrganizationRepository, staffSectionCohortAssociationRepository, staffIdentificationCodeRepository, studentIdentificationCodeRepository, staffAreaLinksFake, schoolAreaLinksFake, adminAreaLinksFake, StaffInformationLookupKeyProvider, searchProvider );
            actualModel = service.Get(SearchRequest.Create(suppliedTextToFind, suppliedRowCountToReturn, suppliedMatchContains, suppliedFilter, userSearchFilters));
        }
    }

    public class When_searching_without_match_contains : BaseSearchServiceFixture
    {
        protected override void EstablishContext()
        {
            suppliedTextToFind = "Baboon Face";
            suppliedRowCountToReturn = 10;
            suppliedMatchContains = false;

            userSearchFilters = new Dictionary<SearchFilter, HashSet<Tuple<int, Type>>>
                                    {
                                        {SearchFilter.Schools,leaHashKeyTuple},
                                        {SearchFilter.Staff,leaHashKeyTuple},
                                        {SearchFilter.Teachers,leaHashKeyTuple},
                                        {SearchFilter.Students,leaHashKeyTuple}
                                    };

            base.EstablishContext();
        }
        
        [Test]
        public void Should_return_correct_school_results()
        {
            Assert.That(actualModel.Schools.Count, Is.EqualTo(2));
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 1), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_teacher_results()
        {
            Assert.That(actualModel.Teachers.Count, Is.EqualTo(2));
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 2), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_student_results()
        {
            Assert.That(actualModel.StudentQuery.Count(), Is.EqualTo(2));
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 2), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_staff_results()
        {
            Assert.That(actualModel.Staff.Count, Is.EqualTo(3));
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 2), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 4), Is.Not.Null);
        }
    }

    public class When_searching_with_match_contains : BaseSearchServiceFixture
    {
        protected override void EstablishContext()
        {
            suppliedTextToFind = "Big Stinky Baboon Face";
            suppliedRowCountToReturn = 10;
            suppliedMatchContains = true;
            userSearchFilters = new Dictionary<SearchFilter, HashSet<Tuple<int, Type>>>
                                    {
                                        {SearchFilter.Schools,leaHashKeyTuple},
                                        {SearchFilter.Staff,leaHashKeyTuple},
                                        {SearchFilter.Teachers,leaHashKeyTuple},
                                        {SearchFilter.Students,leaHashKeyTuple}
                                    };

            base.EstablishContext();

        }

        [Test]
        public void Should_return_correct_school_results()
        {
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 1), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 2), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 3), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_teacher_results()
        {
            Assert.That(actualModel.Teachers.Count, Is.EqualTo(4));
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 1), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 2), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 3), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_student_results()
        {
            Assert.That(actualModel.StudentQuery.Count(), Is.EqualTo(4));
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 1), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 2), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 3), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_staff_results()
        {
            Assert.That(actualModel.Staff.Count, Is.EqualTo(5));
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 1), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 2), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 3), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 4), Is.Not.Null);
        }
    
        [Test]
        public void Should_sort_schools_correctly()
        {
            var previousSchoolName = String.Empty;
            foreach (var school in actualModel.Schools)
            {
                Assert.That(school.Text, Is.GreaterThan(previousSchoolName));
                previousSchoolName = school.Text;
            }
        }

        [Test]
        public void Should_sort_teachers_correctly()
        {
            var previousLastName = String.Empty;
            foreach (var teacher in actualModel.Teachers)
            {
                Assert.That(teacher.Text, Is.GreaterThan(previousLastName));
                previousLastName = teacher.Text;
            }
        }

        [Test]
        public void Should_sort_students_correctly()
        {
            var previousLastName = String.Empty;
            foreach (var student in actualModel.Students)
            {
                Assert.That(student.Text, Is.GreaterThan(previousLastName));
                previousLastName = student.Text;
            }
        }

        [Test]
        public void Should_sort_staff_correctly()
        {
            var previousLastName = String.Empty;
            foreach (var staff in actualModel.Staff)
            {
                Assert.That(staff.Text, Is.GreaterThan(previousLastName));
                previousLastName = staff.Text;
            }
        }
    }

    public class When_searching_for_school_specific_teachers : BaseSearchServiceFixture
    {
        protected override IQueryable<StaffSectionCohortAssociation> GetSuppliedStaffSectionCohortAssociationData()
        {
            var data = new List<StaffSectionCohortAssociation>
                            {
                                new StaffSectionCohortAssociation { StaffUSI = 999, EducationOrganizationId = suppliedSchoolId},
                                new StaffSectionCohortAssociation { StaffUSI = 998, EducationOrganizationId = 999},
                                new StaffSectionCohortAssociation { StaffUSI = 997, EducationOrganizationId = 999},
                                new StaffSectionCohortAssociation { StaffUSI = 996, EducationOrganizationId = 999},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId, SectionIdOrCohortId = 1},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId, SectionIdOrCohortId = 2},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI, EducationOrganizationId = suppliedSchoolId + 1, SectionIdOrCohortId = 3},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 1, EducationOrganizationId = suppliedSchoolId + 1},
                                new StaffSectionCohortAssociation { StaffUSI = suppliedStaffUSI + 2, EducationOrganizationId = suppliedSchoolId + 2}
                            };
            return data.AsQueryable();
        }
        
        protected override IQueryable<StaffInformation> GetSuppliedStaffInformationData()
        {
            var splitText = suppliedTextToFind.Split(' ');
            var searchText = suppliedMatchContains && splitText.Length > 3 ? suppliedTextToFind.Replace(' ', '%') : suppliedTextToFind;
            var data = new List<StaffInformation>
                            {
                                new StaffInformation { StaffUSI = 999, FullName = "wrong data", FirstName = "Apple", MiddleName = "banana", LastSurname = "carrot", EmailAddress = "email1@test.com" },
                                //new StaffInformation { StaffUSI = 998, FullName = searchText, FirstName = "Apple", MiddleName = "banana", LastSurname = "carrot", EmailAddress = "email2@test.com" },
                                //new StaffInformation { StaffUSI = 997, FullName = "wrong data", FirstName = splitText[0], MiddleName = "banana", LastSurname = GetSplitStringFromPosition(splitText, 1), EmailAddress = "email01@test.com" },
                                //new StaffInformation { StaffUSI = 996, FullName = "wrong data", FirstName = splitText[0], MiddleName = GetSplitStringFromPosition(splitText, 1), LastSurname = GetSplitStringFromPosition(splitText, 2), EmailAddress = "email01@test.com" },

                                new StaffInformation { StaffUSI = suppliedStaffUSI, FullName = searchText, FirstName = "Apple", MiddleName = "banana", LastSurname = "elephant", EmailAddress = "email3@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 1, FullName = searchText, FirstName = "Arrow", MiddleName = "banana", LastSurname = "cat", EmailAddress = "email4@test.com" },
                                new StaffInformation { StaffUSI = suppliedStaffUSI + 2, FullName = searchText, FirstName = "Apple", MiddleName = "banana", LastSurname = "cat", EmailAddress = "email5@test.com" },
                                
                            };
            return data.AsQueryable();
        }

        protected override void EstablishContext()
        {
            suppliedTextToFind = "Baboon Face";
            suppliedRowCountToReturn = 10;
            suppliedMatchContains = false;
            suppliedFilter = SearchFilter.Teachers;

            base.EstablishContext();
            userSearchFilters = new Dictionary<SearchFilter, HashSet<Tuple<int, Type>>>
                                    {
                                        //{SearchFilter.Schools,leaHashKeyTuple},
                                        //{SearchFilter.Staff,leaHashKeyTuple},
                                        {SearchFilter.Teachers,schoolHashKeyTuple},
                                        //{SearchFilter.Students,leaHashKeyTuple}
                                    };
        }

        protected override void ExecuteTest()
        {
            var service = new SearchService(schoolInformationRepository, studentInformationRepository, studentSchoolInformationRepository, staffInfoAndOrgQuery, staffInformationRepository, staffEducationOrganizationRepository, staffSectionCohortAssociationRepository,staffIdentificationCodeRepository,studentIdentificationCodeRepository, staffAreaLinksFake, schoolAreaLinksFake, adminAreaLinksFake, StaffInformationLookupKeyProvider, searchProvider);
            actualModel = service.Get(new SearchRequest
                                          {
                                              TextToFind = suppliedTextToFind,
                                              RowCountToReturn = suppliedRowCountToReturn,
                                              MatchContains = suppliedMatchContains,
                                              PageFilter = suppliedFilter,
                                              SearchFilters = userSearchFilters
                                          });
        }

        [Test]
        public void Should_return_correct_teacher_results()
        {
            Assert.That(actualModel.Teachers.Count, Is.EqualTo(3));
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI && x.SchoolId == suppliedSchoolId), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI && x.SchoolId == suppliedSchoolId + 1), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 1 && x.SchoolId == suppliedSchoolId + 1), Is.Not.Null);
        }
    }

    public class When_searching_for_two_names : BaseSearchServiceFixture
    {
        protected override void EstablishContext()
        {
            suppliedTextToFind = "Baboon Face";
            suppliedRowCountToReturn = 10;
            suppliedMatchContains = true;
            suppliedFilter = SearchFilter.Schools | SearchFilter.Staff | SearchFilter.Teachers | SearchFilter.Students;

            base.EstablishContext();
        }

        [Test]
        public void Should_return_correct_school_results()
        {
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 1), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 2), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 3), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_teacher_results()
        {
            Assert.That(actualModel.Teachers.Count, Is.EqualTo(6));
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 6), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 7), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 10), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 11), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 18), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 19), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_student_results()
        {
            Assert.That(actualModel.StudentQuery.Count(), Is.EqualTo(4));
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 6), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 7), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 10), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 11), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_staff_results()
        {
            Assert.That(actualModel.Staff.Count, Is.EqualTo(7));
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 6), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 7), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 10), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 11), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 18), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 19), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 33), Is.Not.Null);
        }
        
    }

    public class When_searching_for_three_names : BaseSearchServiceFixture
    {
        protected override void EstablishContext()
        {
            suppliedTextToFind = "Big Baboon Face";
            suppliedRowCountToReturn = 10;
            suppliedMatchContains = true;
            suppliedFilter = SearchFilter.Schools | SearchFilter.Staff | SearchFilter.Teachers | SearchFilter.Students;

            base.EstablishContext();
        }

        [Test]
        public void Should_return_correct_school_results()
        {
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 1), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 2), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 3), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_teacher_results()
        {
            Assert.That(actualModel.Teachers.Count, Is.EqualTo(4));
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 14), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 15), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 18), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 19), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_student_results()
        {
            Assert.That(actualModel.StudentQuery.Count(), Is.EqualTo(4));
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 14), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 15), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 18), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 19), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_staff_results()
        {
            Assert.That(actualModel.Staff.Count, Is.EqualTo(5));
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 14), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 15), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 18), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 19), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 32), Is.Not.Null);
        }

    }
    
    public class When_searching_for_schools : BaseSearchServiceFixture
    {
        protected override void EstablishContext()
        {
            suppliedTextToFind = "Big Stinky Baboon Face";
            suppliedRowCountToReturn = 10;
            suppliedMatchContains = true;
            suppliedFilter = SearchFilter.Schools;

            base.EstablishContext();
        }

        [Test]
        public void Should_return_school_results()
        {
            Assert.That(actualModel.Schools.Count, Is.EqualTo(4));
        }

        [Test]
        public void Should_provide_correct_url_for_each_school()
        {
            foreach (var school in actualModel.Schools)
            {
                string schoolName = GetSuppliedSchoolInformationData().Where(s => s.SchoolId == school.SchoolId).Select(s => s.Name).SingleOrDefault();
                string expectedLink = schoolAreaLinksFake.Overview(school.SchoolId, schoolName);

                Assert.That(school.Link.Href, Is.EqualTo(expectedLink));
            }
        }

        [Test]
        public void Should_not_return_teacher_results()
        {
            Assert.That(actualModel.Teachers.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_return_student_results()
        {
            Assert.That(actualModel.Students.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_return_staff_results()
        {
            Assert.That(actualModel.Staff.Count, Is.EqualTo(0));
        }
    }

    public class When_searching_for_teachers : BaseSearchServiceFixture
    {
        protected override void EstablishContext()
        {
            suppliedTextToFind = "Big Stinky Baboon Face";
            suppliedRowCountToReturn = 10;
            suppliedMatchContains = true;
            suppliedFilter = SearchFilter.Teachers;

            base.EstablishContext();

        }

        [Test]
        public void Should_not_return_school_results()
        {
            Assert.That(actualModel.Schools.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_return_teacher_results()
        {
            Assert.That(actualModel.Teachers.Count, Is.EqualTo(4));
        }

        [Test]
        public void Should_provide_correct_url_for_each_teacher()
        {
            foreach (var teacher in actualModel.Teachers)
            {
                string fullName = GetSuppliedStaffInformationData().Where(s => s.StaffUSI == teacher.StaffUSI).Select(s => s.FullName).SingleOrDefault();
                string expectedLink = staffAreaLinksFake.Default(teacher.SchoolId, teacher.StaffUSI, fullName);

                Assert.That(teacher.Link.Href, Is.EqualTo(expectedLink));
            }
        }

        [Test]
        public void Should_not_return_student_results()
        {
            Assert.That(actualModel.Students.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_return_staff_results()
        {
            Assert.That(actualModel.Staff.Count, Is.EqualTo(0));
        }
    }

    public class When_searching_for_students : BaseSearchServiceFixture
    {
        protected override void EstablishContext()
        {
            suppliedTextToFind = "Big Stinky Baboon Face";
            suppliedRowCountToReturn = 10;
            suppliedMatchContains = true;
            suppliedFilter = SearchFilter.Students;

            base.EstablishContext();
        }

        [Test]
        public void Should_not_return_school_results()
        {
            Assert.That(actualModel.Schools.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_return_teacher_results()
        {
            Assert.That(actualModel.Teachers.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_return_student_results()
        {
            Assert.That(actualModel.StudentQuery.Count(), Is.EqualTo(4));
        }

        [Test]
        public void Should_not_return_staff_results()
        {
            Assert.That(actualModel.Staff.Count, Is.EqualTo(0));
        }
    }

    public class When_searching_for_staff : BaseSearchServiceFixture
    {
        protected override void EstablishContext()
        {
            suppliedTextToFind = "Big Stinky Baboon Face";
            suppliedRowCountToReturn = 10;
            suppliedMatchContains = true;
            suppliedFilter = SearchFilter.Staff;
            base.EstablishContext();
            userSearchFilters = new Dictionary<SearchFilter, HashSet<Tuple<int, Type>>>
                                    {
                                        //{SearchFilter.Schools,leaHashKeyTuple},
                                        {SearchFilter.Staff,leaHashKeyTuple},
                                        //{SearchFilter.Teachers,schoolHashKeyTuple},
                                        //{SearchFilter.Students,leaHashKeyTuple}
                                    };
        }
        
        [Test]
        public void Should_not_return_school_results()
        {
            Assert.That(actualModel.Schools.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_return_teacher_results()
        {
            Assert.That(actualModel.Teachers.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_return_student_results()
        {
            Assert.That(actualModel.Students.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_return_staff_results()
        {
            Assert.That(actualModel.Staff.Count, Is.EqualTo(5));
        }

        [Test]
        public void Should_not_return_results_for_education_organizations_that_do_not_have_claim_in_context()
        {
            Assert.That(!actualModel.Staff.Any(s => s.SchoolId.Equals((suppliedSchoolId + 314))));
        }
    }
    public class When_searching_with_limited_rows : BaseSearchServiceFixture
    {
        protected override void EstablishContext()
        {
            suppliedTextToFind = "Big Stinky Baboon Face";
            suppliedRowCountToReturn = 2;
            suppliedMatchContains = true;
            suppliedFilter = SearchFilter.Schools | SearchFilter.Staff | SearchFilter.Teachers | SearchFilter.Students;
            userSearchFilters = new Dictionary<SearchFilter, HashSet<Tuple<int, Type>>>
                                    {
                                        {SearchFilter.Schools,leaHashKeyTuple},
                                        {SearchFilter.Staff,leaHashKeyTuple},
                                        {SearchFilter.Teachers,leaHashKeyTuple},
                                        {SearchFilter.Students,leaHashKeyTuple}
                                    };
            base.EstablishContext();
        }

        [Test]
        public void Should_return_correct_school_results()
        {
            Assert.That(actualModel.Schools.Count, Is.EqualTo(2));
            Assert.That(actualModel.AbsoluteSchoolsCount, Is.EqualTo(4));
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 2), Is.Not.Null);
            Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId + 3), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_teacher_results()
        {
            Assert.That(actualModel.Teachers.Count, Is.EqualTo(2));
            Assert.That(actualModel.AbsoluteTeachersCount, Is.EqualTo(4));
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 2), Is.Not.Null);
            Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 1), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_student_results()
        {
            Assert.That(actualModel.StudentQuery.Count(), Is.EqualTo(4));
            Assert.That(actualModel.AbsoluteStudentsCount, Is.EqualTo(0));
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 3), Is.Not.Null);
            Assert.That(actualModel.StudentQuery.SingleOrDefault(x => x.StudentUSI == suppliedStudentUSI + 2), Is.Not.Null);
        }

        [Test]
        public void Should_return_correct_staff_results()
        {
            Assert.That(actualModel.Staff.Count, Is.EqualTo(2));
            Assert.That(actualModel.AbsoluteStaffCount, Is.EqualTo(5));
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 4), Is.Not.Null);
            Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI + 2), Is.Not.Null);
        }
    }

	public class When_searching_for_teachers_with_identification_code : BaseSearchServiceFixture
	{
		protected override void EstablishContext()
		{
			suppliedTextToFind = suppliedIdentificationCode.ToString();
			suppliedRowCountToReturn = 1;
			suppliedMatchContains = true;
			suppliedFilter = SearchFilter.Schools | SearchFilter.Staff | SearchFilter.Teachers | SearchFilter.Students;
			userSearchFilters = new Dictionary<SearchFilter, HashSet<Tuple<int, Type>>>
                                    {
                                        {SearchFilter.Schools,leaHashKeyTuple},
                                        {SearchFilter.Staff,leaHashKeyTuple},
                                        {SearchFilter.Teachers,leaHashKeyTuple},
                                        {SearchFilter.Students,leaHashKeyTuple}
                                    };
			base.EstablishContext();
		}

		[Test]
		public void Should_return_correct_school_results()
		{
			Assert.That(actualModel.Schools.Count, Is.EqualTo(1));
			Assert.That(actualModel.AbsoluteSchoolsCount, Is.EqualTo(4));
			Assert.That(actualModel.Schools.SingleOrDefault(x => x.SchoolId == suppliedSchoolId), Is.Not.Null);
		}

		[Test]
		public void Should_one_teacher()
		{
			Assert.That(actualModel.Teachers.Count, Is.EqualTo(1));
			Assert.That(actualModel.AbsoluteTeachersCount, Is.EqualTo(1));
			Assert.That(actualModel.Teachers.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI), Is.Not.Null);
		}

		[Test]
		public void Should_return_no_students()
		{
			Assert.That(actualModel.StudentQuery.Count(), Is.EqualTo(0));
			Assert.That(actualModel.AbsoluteStudentsCount, Is.EqualTo(0));
		}

		[Test]
		public void Should_return_one_staff()
		{
			Assert.That(actualModel.Staff.Count, Is.EqualTo(1));
			Assert.That(actualModel.AbsoluteStaffCount, Is.EqualTo(1));
			Assert.That(actualModel.Staff.SingleOrDefault(x => x.StaffUSI == suppliedStaffUSI), Is.Not.Null);
		}
	}
}
