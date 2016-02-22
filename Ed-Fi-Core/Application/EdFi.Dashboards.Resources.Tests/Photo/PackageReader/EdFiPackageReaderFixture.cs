using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Photo;
using EdFi.Dashboards.Resources.Photo.Implementations.PackageReader;
using EdFi.Dashboards.Resources.Photo.Models;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.Photo.PackageReader
{
    public class When_reading_a_package_of_photos : TestFixtureBase
    {
        protected IEnumerable<OriginalPhoto> originalPhotos;
        protected IPackageReader packageReader;

        protected override void EstablishContext()
        {
            packageReader = new EdFiPackageReader(null);
        }

        protected override void ExecuteTest()
        {
            var files = new Dictionary<string, byte[]>();

            files["/students/123.jpg"] = new byte[] { 1, 2, 3 };
            files["/staff/345.jpg"] = new byte[] { 1, 2, 3, 4 };

            originalPhotos = packageReader.GetPhotos(files);
        }

        [Test]
        public void Should_have_staff_photo()
        {
            var uniqueOriginalPhoto = (UniqueOriginalPhoto)originalPhotos.Single(p => ((UniqueOriginalPhoto)p).Id == 345);

            Assert.AreEqual(4, uniqueOriginalPhoto.Photo.Count());
            Assert.AreEqual(ImageType.Staff, uniqueOriginalPhoto.ImageType);
        }

        [Test]
        public void Should_have_student_photo()
        {
            var uniqueOriginalPhoto = (UniqueOriginalPhoto)originalPhotos.Single(p => ((UniqueOriginalPhoto)p).Id == 123);

            Assert.AreEqual(3, uniqueOriginalPhoto.Photo.Count());
            Assert.AreEqual(ImageType.Student, uniqueOriginalPhoto.ImageType);
        }

        [Test]
        public void Should_have_required_attributes()
        {
            foreach (var originalPhoto in originalPhotos)
            {
                // Ensure that the name and level is specified.
                var uniqueOriginalPhoto = (UniqueOriginalPhoto)originalPhoto;

                Assert.IsTrue(uniqueOriginalPhoto.Id > 0);
                Assert.IsTrue(uniqueOriginalPhoto.ImageType == ImageType.Student || uniqueOriginalPhoto.ImageType == ImageType.Staff);
                Assert.IsNotNull(uniqueOriginalPhoto.Photo);
            }
        }
    }
}
