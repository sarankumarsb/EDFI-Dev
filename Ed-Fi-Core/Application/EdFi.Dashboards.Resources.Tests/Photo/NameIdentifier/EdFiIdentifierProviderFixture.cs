using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Photo;
using EdFi.Dashboards.Resources.Photo.Implementations.NameIdentifier;
using EdFi.Dashboards.Resources.Photo.Models;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Photo.NameIdentifier
{
    public class When_using_the_identifier_provider_to_find_UniqueOriginalPhoto_instances : TestFixtureBase
    {
        protected const int schoolId = 1;
        protected EdFiIdentifierProvider identifierProvider;
        protected List<OriginalPhoto> originalPhotos;
        protected List<Identifier> identifiers;

        protected override void EstablishContext()
        {
            var staffEducationOrgInformation = new List<StaffEducationOrgInformation>
                                                   {
                                                       new StaffEducationOrgInformation {EducationOrganizationId = schoolId, StaffUSI = 101},
                                                       new StaffEducationOrgInformation {EducationOrganizationId = schoolId + 1, StaffUSI = 102},
                                                       new StaffEducationOrgInformation {EducationOrganizationId = schoolId, StaffUSI = 103}
                                                   };

            var studentSchoolInformation = new List<StudentSchoolInformation>
                                               {
                                                   new StudentSchoolInformation {StudentUSI = 201, SchoolId = schoolId},
                                                   new StudentSchoolInformation {StudentUSI = 202, SchoolId = schoolId + 1},
                                                   new StudentSchoolInformation {StudentUSI = 203, SchoolId = schoolId}
                                               };

            originalPhotos = new List<OriginalPhoto>
                                 {
                                     new UniqueOriginalPhoto {Id = 101, ImageType = ImageType.Staff},
                                     new UniqueOriginalPhoto {Id = 102, ImageType = ImageType.Staff},
                                     new UniqueOriginalPhoto {Id = 103, ImageType = ImageType.Staff},
                                     new UniqueOriginalPhoto {Id = 201, ImageType = ImageType.Student},
                                     new UniqueOriginalPhoto {Id = 202, ImageType = ImageType.Student},
                                     new UniqueOriginalPhoto {Id = 203, ImageType = ImageType.Student}
                                 };

            var staffEducationOrgInformationRepository = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            var studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();

            Expect.Call(studentSchoolInformationRepository.GetAll()).Return(studentSchoolInformation.AsQueryable()).Repeat.Once();
            Expect.Call(staffEducationOrgInformationRepository.GetAll()).Return(staffEducationOrgInformation.AsQueryable()).Repeat.Once();

            identifiers = new List<Identifier>();
            identifierProvider = new EdFiIdentifierProvider(studentSchoolInformationRepository,
                                                            staffEducationOrgInformationRepository,
                                                            null);
        }

        protected override void ExecuteTest()
        {
            foreach (var originalPhoto in originalPhotos)
            {
                var identifier = identifierProvider.Get(new IdentifierRequest { SchoolId = schoolId, OriginalPhoto = originalPhoto });

                if (identifier != null)
                    identifiers.Add(identifier);
            }
        }

        [Test]
        public void Should_have_valid_identifiers()
        {
            Assert.That(identifiers.Single(i => i.Id == 101 && i.ImageType == ImageType.Staff), Is.Not.Null);
            Assert.That(identifiers.Single(i => i.Id == 103 && i.ImageType == ImageType.Staff), Is.Not.Null);
            Assert.That(identifiers.Single(i => i.Id == 201 && i.ImageType == ImageType.Student), Is.Not.Null);
            Assert.That(identifiers.Single(i => i.Id == 203 && i.ImageType == ImageType.Student), Is.Not.Null);
        }

        [Test]
        public void Should_ignore_staff_that_do_not_map_to_the_school()
        {
            Assert.That(identifiers.Any(i => i.Id == 102), Is.False);
        }

        [Test]
        public void Should_ignore_students_that_do_not_map_to_the_school()
        {
            Assert.That(identifiers.Any(i => i.Id == 202), Is.False);
        }
    }
}
