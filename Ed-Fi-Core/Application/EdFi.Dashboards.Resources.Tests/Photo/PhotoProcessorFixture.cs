using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Photo;
using EdFi.Dashboards.Resources.Photo.Implementations;
using EdFi.Dashboards.Resources.Photo.Models;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Photo
{
    public class When_processing_the_PhotoProcessorResponse : TestFixtureBase
    {
        protected const int LocalEducationAgencyId = 1;
        protected PhotoProcessor photoProcessor;
        protected PhotoProcessorResponse photoProcessorResponse;

        protected override void EstablishContext()
        {
            var archiveParser = mocks.StrictMock<IArchiveParser>();
            var packageReader = mocks.StrictMock<IPackageReader>();
            var identifierProvider = mocks.StrictMock<IIdentifierProvider>();
            var photoResizer = mocks.StrictMock<IPhotoResizer>();
            var photoStorage = mocks.StrictMock<IPhotoStorage>();
            var originalPhotos = new List<OriginalPhoto>
                                     {
                                         new UniqueOriginalPhoto
                                             {
                                                 Id = 111,
                                                 Photo = new byte[] {1, 2}
                                             },
                                         new UniqueOriginalPhoto
                                             {
                                                 Photo = new byte[] {1, 2}
                                             },
                                         new UniqueOriginalPhoto // Doesn't have an associated photo.
                                             {
                                                 Id = 1,
                                             },
                                         new UniqueOriginalPhoto
                                             {
                                                 Id = 333,
                                                 Photo = new byte[] {1, 2}
                                             }
                                     };

            Expect.Call(archiveParser.Parse(null)).IgnoreArguments().Return(null);
            Expect.Call(packageReader.GetPhotos(null)).IgnoreArguments().Return(originalPhotos.AsQueryable()).Repeat.Once();
            Expect.Call(identifierProvider.Get(null)).IgnoreArguments().Repeat.Once().Return(new Identifier { Id = 111, ImageType = ImageType.Student });
            Expect.Call(identifierProvider.Get(null)).IgnoreArguments().Repeat.Once().Return(null);
            Expect.Call(identifierProvider.Get(null)).IgnoreArguments().Repeat.Once().Return(new Identifier { Id = 333, ImageType = ImageType.Staff });
            Expect.Call(() => photoResizer.Resize(null)).IgnoreArguments().Repeat.Times(2);
            Expect.Call(() => photoStorage.Save(null, null)).IgnoreArguments().Repeat.Times(2);

            photoProcessor = new PhotoProcessor(archiveParser, packageReader, identifierProvider, photoResizer, photoStorage);
        }

        protected override void ExecuteTest()
        {
            var photoProcessorRequest = new PhotoProcessorRequest { FileBytes = new byte[] { 1, 2, 3 } };

            photoProcessorResponse = photoProcessor.Process(photoProcessorRequest);
        }

        [Test]
        public void Should_return_error_messages()
        {
            Assert.AreEqual(2, photoProcessorResponse.ErrorMessages.Count);
        }

        [Test]
        public void Should_return_proper_counts()
        {
            Assert.AreEqual(2, photoProcessorResponse.SuccessfullyProcessedPhotos);
            Assert.AreEqual(4, photoProcessorResponse.TotalRecords);
        }
    }
}
