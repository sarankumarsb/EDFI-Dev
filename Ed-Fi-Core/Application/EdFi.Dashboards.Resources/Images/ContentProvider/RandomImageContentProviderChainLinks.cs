// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Common;

namespace EdFi.Dashboards.Resources.Images.ContentProvider
{
    public class RandomImageContentProviderChainLinks
    {
    }

    public class StudentRandomImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileBasedImageContentProvider provider;
        private const string personImageFilePathFormat = "~/Core_Content/Images/Students/{0}{1}{2}";
        private const string defaultPersonImageFilePathFormat = "~/Core_Content/Images/Students/NoImage{0}.jpg";

        public StudentRandomImageContentProvider(IFileBasedImageContentProvider provider, IImageContentProvider next) 
            : base(next)
        {
            this.provider = provider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StudentSchoolImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var studentRequest = request as StudentSchoolImageRequest;

            var imageName = GetImageName(studentRequest.StudentUSI, studentRequest.Gender);

            var fileName = Path.GetFileNameWithoutExtension(imageName);
            var extension = Path.GetExtension(imageName);

            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var filePath = string.Format(personImageFilePathFormat, fileName, displayType, extension);
            var defaultFilePath = string.Format(defaultPersonImageFilePathFormat, displayType);

            var res = provider.GetImageContent(filePath) ?? provider.GetImageContent(defaultFilePath);

            return res;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            //Delagate to the base...
            return ProcessRequest(request);
        }

		private static string GetImageName(long studentUSI, string gender)
        {
            //string imgPath = "~/Core_Content/Images/Students/"; 
            string resImg = "NoImage.jpg";

            //Else we choose a random one.
            var female = new[] { 
                        "01FemaleAmericanIndian.png",
                        "02FemaleWhite.png",
                        "04FemaleBlack.png",
                        "05FemaleAmericanIndian.png",
                        "07FemaleWhite.png",
                        "09FemaleWhite.png",
                        "11FemaleBlack.png",
                        "17FemaleAfricanAmerican.png",
                        "19FemaleAfricanAmerican.png",
                        "20FemaleWhite.png",
                        "ES-00LatinFemale.png",
                        "ES-01AsianFemale.png",
                        "ES-01IndianFemale.png",
                        "ES-01LatinFemale.png",
                        "ES-01WhiteFemale.png",
                        "ES-02AsianFemale.png",
                        "ES-02WhiteFemale.png",
                        "ES-03LatinFemale.png",
                        "ES-05WhiteFemale.png",
                        "ES-06BlackFemale.png",
                        "ES-07BlackFemale.png",
                        "ES-10WhiteFemale.png",
                        "ES-11WhiteFemale.png",
                        "ES-12BlackFemale.png",
                        "ES-12WhiteFemale.png",
                        "femaleAfrican1.png",
                        "femaleAfrican2.png",
                        "femaleAfrican3.png",
                        "femaleWhite1.png",
                        "femaleWhite7.png",
                        "femaleWhite8.png",
                        "femaleWhite9.png" };
            var male = new[] { 
						"03MaleWhite.png",
                        "06MaleWhite.png",
                        "08MaleWhite.png",
                        "10MaleWhite.png",
                        "12MaleAmericanIndian.png",
                        "13MaleWhite.png",
                        "14MaleWhite.png",
                        "15MaleWhite.png",
                        "16MaleAfricanAmerican.png",
                        "18MaleWhite.png",
                        "1Tyson.jpg",
                        "21MaleWhite.png",
                        "2Maria.jpg",
                        "3Anthony.jpg",
                        "ES-00LatinMale.png",
                        "ES-01BlackMale.png",
                        "ES-01WhiteMale.png",
                        "ES-02LatMale.png",
                        "ES-02WhiteMale.png",
                        "ES-03LatinMale.png",
                        "ES-03WhiteMale.png",
                        "ES-04BlackMale.png",
                        "ES-04WhiteMale.png",
                        "ES-05WhiteMale.png",
                        "ES-07LatinMale.png",
                        "ES-11BlackMale.png",
                        "ES-11LatMale.png",
                        "maleAfrican1.png",
                        "maleNative2.png",
                        "maleWhite1.png",
                        "maleWhite12.png",
                        "maleWhite13.png",
                        "maleWhite16.png",
                        "maleWhite20.png",
                        "maleWhite21.png",
                        "maleWhite23.png",
                        "maleWhite3.png",
                        "maleWhite4.png" };
            var seed = (int)(studentUSI % int.MaxValue);
            var ranFemale = new Random(seed);
            var ranMale = new Random(seed);

            //If we have a gender then:
            if (!string.IsNullOrEmpty(gender))
            {
                if (gender == "Male")
                {
                    int pIndex = ranMale.Next(male.Length - 1);
                    //resImg = "~/Core_Content/Images/Students/" + male[pIndex];
                    resImg = male[pIndex];
                }
                else //must be female you never know...
                {
                    int pIndex = ranFemale.Next(female.Length - 1);
                    //resImg = "~/Core_Content/Images/Students/" + female[pIndex];
                    resImg = female[pIndex];
                }
            }

            //Specific images for student IDS.
            var imgFile = (from i in GetHardCodedImages()
                           where i.StudentUSI == studentUSI
                           select i).SingleOrDefault();

            if (imgFile != null)
                resImg = imgFile.ImagePath;

            return resImg;
        }

        private static IEnumerable<StudentImage> GetHardCodedImages()
        {
            return new List<StudentImage>
                       { 
				new StudentImage{ StudentUSI=100061162, ImagePath="01FemaleAmericanIndian.png"},
				new StudentImage{ StudentUSI=100058148, ImagePath="02FemaleWhite.png"},
				new StudentImage{ StudentUSI=100080868, ImagePath="03MaleWhite.png"},
				new StudentImage{ StudentUSI=100041240, ImagePath="04FemaleBlack.png"},
				new StudentImage{ StudentUSI=100057336, ImagePath="05FemaleAmericanIndian.png"},
				new StudentImage{ StudentUSI=100105733, ImagePath="06MaleWhite.png"},
				new StudentImage{ StudentUSI=100061957, ImagePath="07FemaleWhite.png"},
				new StudentImage{ StudentUSI=100077550, ImagePath="08MaleWhite.png"},
				new StudentImage{ StudentUSI=100063979, ImagePath="09FemaleWhite.png"},
				new StudentImage{ StudentUSI=100057334, ImagePath="10MaleWhite.png"},
				new StudentImage{ StudentUSI=100058063, ImagePath="11FemaleBlack.png"},
				new StudentImage{ StudentUSI=100062970, ImagePath="12MaleAmericanIndian.png"},
				new StudentImage{ StudentUSI=100057281, ImagePath="13MaleWhite.png"},
				new StudentImage{ StudentUSI=100090419, ImagePath="14MaleWhite.png"},
				new StudentImage{ StudentUSI=100057910, ImagePath="15MaleWhite.png"},
				new StudentImage{ StudentUSI=100086938, ImagePath="16MaleAfricanAmerican.png"},
				new StudentImage{ StudentUSI=100064729, ImagePath="17FemaleAfricanAmerican.png"},
				new StudentImage{ StudentUSI=100096724, ImagePath="18MaleWhite.png"},
				new StudentImage{ StudentUSI=100057387, ImagePath="19FemaleAfricanAmerican.png"},
				new StudentImage{ StudentUSI=100058720, ImagePath="20FemaleWhite.png"},
				new StudentImage{ StudentUSI=100121726, ImagePath="21MaleWhite.png"},			
			};
        }

        private class StudentImage
        {
            public long StudentUSI { get; set; }
            public string ImagePath { get; set; }
        }
    }

    public class StaffRandomImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileBasedImageContentProvider provider;
        private const string personImageFilePathFormat = "~/Core_Content/Images/Students/{0}{1}{2}";
        private const string defaultPersonImageFilePathFormat = "~/Core_Content/Images/Students/NoImage{0}.jpg";

        public StaffRandomImageContentProvider(IFileBasedImageContentProvider provider, IImageContentProvider next)
            : base(next)
        {
            this.provider = provider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StaffSchoolImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var staffRequest = request as StaffSchoolImageRequest;

            var imageName = GetStaffImageName(staffRequest.StaffUSI, staffRequest.Gender);

            var fileName = Path.GetFileNameWithoutExtension(imageName);
            var extension = Path.GetExtension(imageName);

            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var filePath = string.Format(personImageFilePathFormat, fileName, displayType, extension);
            var defaultFilePath = string.Format(defaultPersonImageFilePathFormat, displayType);

            var res = provider.GetImageContent(filePath) ?? provider.GetImageContent(defaultFilePath);

            return res;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            return ProcessRequest(request);
        }

        private static string GetStaffImageName(long staffUSI, string gender)
        {
            //string imgPath = "~/Core_Content/Images/Students/"; 
            string resImg = "NoImage.jpg";

            //Else we choose a random one.
            var female = new[] { 
                        "01FemaleTeacher.png",
                        "02FemaleTeacher.png",
                        "03FemaleTeacher.png",
                        "04FemaleTeacher.png",
                        "05FemaleTeacher.png",
                        "06FemaleTeacher.png",
                        "07FemaleTeacher.png",
                        "08FemaleTeacher.png",
                        "09FemaleTeacher.png",
                        "10FemaleTeacher.png",
                        "06FemaleTeacher.png"};
            var male = new[] { 
						"01MaleTeacher.png",
                        "02MaleTeacher.png",
                        "03MaleTeacher.png",
                        "04MaleTeacher.png",
                        "05MaleTeacher.png" };
            var seed = (int)(staffUSI % int.MaxValue);
            var ranFemale = new Random(seed);
            var ranMale = new Random(seed);

            //If we have a gender then:
            if (!string.IsNullOrEmpty(gender))
            {
                if (gender == "Male")
                {
                    int pIndex = ranMale.Next(male.Length - 1);
                    //resImg = "~/Core_Content/Images/Students/" + male[pIndex];
                    resImg = male[pIndex];
                }
                else //must be female you never know...
                {
                    int pIndex = ranFemale.Next(female.Length - 1);
                    //resImg = "~/Core_Content/Images/Students/" + female[pIndex];
                    resImg = female[pIndex];
                }
            }

            return resImg;
        }
    }

    public class SchoolRandomImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileBasedImageContentProvider provider;
        private const string schoolImageFilePathFormat = "~/Core_Content/Images/EducationOrganization/{0}{1}{2}";
        private const string defaultSchoolImageFilePathFormat = "~/Core_Content/Images/EducationOrganization/NoEducationOrganizationImage{0}.jpg";

        public SchoolRandomImageContentProvider(IFileBasedImageContentProvider provider, IImageContentProvider next)
            : base(next)
        {
            this.provider = provider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as SchoolImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var schoolRequest = request as SchoolImageRequest;

            var imageName = GetEducationOrganizationImageName(schoolRequest.SchoolId);

            var fileName = Path.GetFileNameWithoutExtension(imageName);
            var extension = Path.GetExtension(imageName);

            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var filePath = string.Format(schoolImageFilePathFormat, fileName, displayType, extension);
            var defaultFilePath = string.Format(defaultSchoolImageFilePathFormat, displayType);

            var res = provider.GetImageContent(filePath) ?? provider.GetImageContent(defaultFilePath);

            return res;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            return ProcessRequest(request);
        }

        public string GetEducationOrganizationImageName(int educationOrganizationId)
        {
            const string resImg = "Bradock.jpg";
            return resImg;
        }
    }

    public class LocalEducationAgencyRandomImageContentProvider : ChainOfResponsibilityBase<IImageContentProvider, ImageRequestBase, ImageModel>, IImageContentProvider
    {
        private readonly IFileBasedImageContentProvider provider;
        private const string leaImageFilePathFormat = "~/Core_Content/Images/EducationOrganization/{0}{1}{2}";
        private const string defaultLeaImageFilePathFormat = "~/Core_Content/Images/EducationOrganization/NoEducationOrganizationImage{0}.jpg";

        public LocalEducationAgencyRandomImageContentProvider(IFileBasedImageContentProvider provider, IImageContentProvider next)
            : base(next)
        {
            this.provider = provider;
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as LocalEducationAgencyImageRequest != null);
        }

        protected override ImageModel HandleRequest(ImageRequestBase request)
        {
            var leaRequest = request as LocalEducationAgencyImageRequest;

            var imageName = GetEducationOrganizationImageName(leaRequest.LocalEducationAgencyId);

            var fileName = Path.GetFileNameWithoutExtension(imageName);
            var extension = Path.GetExtension(imageName);

            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var filePath = string.Format(leaImageFilePathFormat, fileName, displayType, extension);
            var defaultFilePath = string.Format(defaultLeaImageFilePathFormat, displayType);

            var res = provider.GetImageContent(filePath) ?? provider.GetImageContent(defaultFilePath);

            return res;
        }

        public ImageModel GetImage(ImageRequestBase request)
        {
            return ProcessRequest(request);
        }

        public string GetEducationOrganizationImageName(int educationOrganizationId)
        {
            const string resImg = "Bradock.jpg";
            return resImg;
        }
    }
}
