using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Admin;
using EdFi.Dashboards.Resources.Application;
using EdFi.Dashboards.Resources.Models.Admin;
using EdFi.Dashboards.Resources.Photo;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Admin
{
    public abstract class PhotoManagementServiceFixture : TestFixtureBase
    {
        protected const int LocalEducationAgencyId = 1;
        protected PhotoManagementService photoManagementService;
        protected PhotoManagementModel photoManagementModel;
        protected IPhotoProcessor photoProcessor;
        protected IErrorLoggingService errorLoggingService;

        protected override void EstablishContext()
        {
            var schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();

            photoProcessor = mocks.StrictMock<IPhotoProcessor>();
            errorLoggingService = mocks.StrictMock<IErrorLoggingService>();

            Expect.Call(schoolInformationRepository.GetAll()).Return(
                new List<SchoolInformation>
                    {
                        new SchoolInformation {SchoolId = 222, LocalEducationAgencyId = LocalEducationAgencyId},
                        new SchoolInformation {SchoolId = 333, LocalEducationAgencyId = LocalEducationAgencyId},
                        new SchoolInformation {SchoolId = 444, LocalEducationAgencyId = LocalEducationAgencyId + 900}
                    }.
                    AsQueryable());

            photoManagementService = new PhotoManagementService(schoolInformationRepository, photoProcessor, errorLoggingService);
        }
    }

    public class When_executing_get : PhotoManagementServiceFixture
    {
        protected override void ExecuteTest()
        {
            var request = new PhotoManagementRequest
            {
                LocalEducationAgencyId = LocalEducationAgencyId
            };

            photoManagementModel = photoManagementService.Get(request);
        }

        [Test]
        public void Should_return_only_schools_for_current_LEA()
        {
            Assert.AreEqual(2, photoManagementModel.Schools.Count);
            Assert.IsTrue(photoManagementModel.Schools.Any(s => s.SchoolId == 222));
            Assert.IsTrue(photoManagementModel.Schools.Any(s => s.SchoolId == 333));
        }
    }

    public class Security_when_executing_post : PhotoManagementServiceFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(photoProcessor.Process(null)).IgnoreArguments().Repeat.Never();
        }

        protected override void ExecuteTest()
        {
            var request = new PhotoManagementPostRequest
            {
                SchoolIdToLoadImagesTo = 123, // Incorrect school Id.
                LocalEducationAgencyId = LocalEducationAgencyId
            };

            photoManagementModel = photoManagementService.Post(request);
        }

        [Test]
        public void Should_not_allow_uploads_to_schools_not_in_current_LEA()
        {
            // Just make sure process isn't called. Notice that the Process()
            // method has been set to Repeat.Never() in EstablishContext().
        }
    }

    public class When_executing_post : PhotoManagementServiceFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            var photoProcessorResponse = new PhotoProcessorResponse { SuccessfullyProcessedPhotos = 20, TotalRecords = 30 };

            photoProcessorResponse.ErrorMessages.Add("Error 1");
            photoProcessorResponse.ErrorMessages.Add("Error 2");

            Expect.Call(photoProcessor.Process(null)).IgnoreArguments().Return(photoProcessorResponse).Repeat.Once();
        }

        protected override void ExecuteTest()
        {
            var request = new PhotoManagementPostRequest
            {
                SchoolIdToLoadImagesTo = 222,
                LocalEducationAgencyId = LocalEducationAgencyId
            };

            photoManagementModel = photoManagementService.Post(request);
        }

        [Test]
        public void Should_return_status_from_photo_processor()
        {
            Assert.AreEqual(20, photoManagementModel.SuccessfullyProcessedPhotos);
            Assert.AreEqual(30, photoManagementModel.TotalRecords);
            Assert.AreEqual(2, photoManagementModel.ErrorMessages.Count);
            Assert.That(photoManagementModel.ErrorMessages.Contains("Error 1"), Is.True);
            Assert.That(photoManagementModel.ErrorMessages.Contains("Error 2"), Is.True);
        }
    }
}
