using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Admin;
using EdFi.Dashboards.Application.Resources.Models;
using EdFi.Dashboards.Application.Resources.Models.Admin;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Is = NUnit.Framework.Is;

namespace EdFi.Dashboards.Application.Resources.Tests.Admin
{
    public abstract class TitleClaimSetServiceFixture : TestFixtureBase
    {
        protected TitleClaimSetModel resultModel;
        protected PostResultsModel postResultModel;
        protected IAdminAreaLinks adminAreaLinks;
        protected ITitleClaimSetService titleClaimSetService;
        protected IRepository<StaffEducationOrgInformation> staffEdOrgInfoRepo;
        protected IPersistingRepository<ClaimSetMapping> claimSetMappingRepo;

        protected string suppliedPositionTitle1 = "Position Title 1";
        protected string suppliedPositionTitle2 = "Position Title 2";
        protected string suppliedPositionTitle3 = "Position Title 3";
        protected string suppliedPositionTitle4 = "Position Title 4";

        protected string suppliedClaimSet1 = "Claim Set 1";
        protected string suppliedClaimSet2 = "Claim Set 2";
        protected string suppliedClaimSet3 = "Claim Set 3";
        protected string suppliedClaimSet4 = "None";
        protected string suppliedClaimSet5 = "Administration";

        protected int suppliedLea1 = 1;
        protected int suppliedLea2 = 2;
        protected int suppliedLea3 = 3;

        #region TestFixture Override Methods
        protected override void EstablishContext()
        {                       
            titleClaimSetService = new TitleClaimSetService(adminAreaLinks)
                                       {
                                           ClaimSetMappingRepo = claimSetMappingRepo,
                                           StaffEdOrgInfoRepo = staffEdOrgInfoRepo
                                       };

            base.EstablishContext();
        }
        #endregion

        protected IQueryable<StaffEducationOrgInformation> GetPositionTitles()
        {
            return new List<StaffEducationOrgInformation>
                       {
                           new StaffEducationOrgInformation() { PositionTitle = suppliedPositionTitle1 },
                           new StaffEducationOrgInformation() { PositionTitle = suppliedPositionTitle2 },
                           new StaffEducationOrgInformation() { PositionTitle = suppliedPositionTitle1 },
                           new StaffEducationOrgInformation() { PositionTitle = suppliedPositionTitle3 }
                       }.AsQueryable();
        }

        protected IQueryable<ClaimSetMapping> GetClaimSetMappingsSingleLea()
        {
            return new List<ClaimSetMapping>
                       {
                           new ClaimSetMapping{ LocalEducationAgencyId = suppliedLea2, PositionTitle = suppliedPositionTitle1, ClaimSet = suppliedClaimSet1 },
                           new ClaimSetMapping{ LocalEducationAgencyId = suppliedLea2, PositionTitle = suppliedPositionTitle2, ClaimSet = suppliedClaimSet2 }
                       }.AsQueryable();
        }

        protected IQueryable<ClaimSetMapping> GetClaimSetMappings()
        {
            return new List<ClaimSetMapping>
                       {
                           new ClaimSetMapping{ LocalEducationAgencyId = suppliedLea1, PositionTitle = suppliedPositionTitle1, ClaimSet = suppliedClaimSet1 },
                           new ClaimSetMapping{ LocalEducationAgencyId = suppliedLea1, PositionTitle = suppliedPositionTitle2, ClaimSet = suppliedClaimSet2 },
                           new ClaimSetMapping{ LocalEducationAgencyId = suppliedLea2, PositionTitle = suppliedPositionTitle3, ClaimSet = suppliedClaimSet3 },

                           new ClaimSetMapping{ LocalEducationAgencyId = suppliedLea3, PositionTitle = suppliedPositionTitle2, ClaimSet = suppliedClaimSet1 },
                           new ClaimSetMapping{ LocalEducationAgencyId = suppliedLea3, PositionTitle = suppliedPositionTitle1, ClaimSet = suppliedClaimSet4 },
                           new ClaimSetMapping{ LocalEducationAgencyId = suppliedLea3, PositionTitle = suppliedPositionTitle3, ClaimSet = suppliedClaimSet5 }
                       }.AsQueryable();
        }

        protected StringBuilder GetCSVFileAndHeaders()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Position Title, ClaimSet");

            return sb;
        }
    }

    #region Get Title ClaimSet Model
    public class When_getting_title_claim_set_model_with_no_position_titles : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            adminAreaLinks = mocks.StrictMock<IAdminAreaLinks>();
            staffEdOrgInfoRepo = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();

            var tempList = new List<StaffEducationOrgInformation>();

            Expect.Call(adminAreaLinks.TitleClaimSet(1)).Return("Hello World");
            Expect.Call(staffEdOrgInfoRepo.GetAll()).Return(tempList.AsQueryable());

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {            
            resultModel = titleClaimSetService.Get(new TitleClaimSetRequest { LocalEducationAgencyId = 1 });
        }

        [Test]
        public void Should_return_empty_model()
        {
            Assert.That(resultModel.LocalEducationAgencyId, Is.EqualTo(1));
            Assert.That(resultModel.EdOrgPositionTitles, Is.EqualTo(new Dictionary<string, string>()));
            Assert.That(resultModel.ClaimSetMaps, Is.EqualTo(new Dictionary<string, string>()));
            Assert.That(resultModel.PossibleClaimSets, Is.EqualTo(new Dictionary<string, string>()));
            Assert.That(resultModel.Url, Is.EqualTo("Hello World"));
        }
    }

    public class When_getting_title_claim_set_model_with_no_claim_set_mappings : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            adminAreaLinks = mocks.StrictMock<IAdminAreaLinks>();
            staffEdOrgInfoRepo = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(adminAreaLinks.TitleClaimSet(1)).Return("Hello World");
            Expect.Call(staffEdOrgInfoRepo.GetAll()).Return(GetPositionTitles());
            Expect.Call(claimSetMappingRepo.GetAll()).Return((new List<ClaimSetMapping>()).AsQueryable());

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            resultModel = titleClaimSetService.Get(new TitleClaimSetRequest { LocalEducationAgencyId = 1 });
        }

        [Test]
        public void Should_return_empty_model()
        {
            Assert.That(resultModel.LocalEducationAgencyId, Is.EqualTo(1));
            Assert.That(resultModel.EdOrgPositionTitles, Is.EqualTo(new Dictionary<string, string>()));
            Assert.That(resultModel.ClaimSetMaps, Is.EqualTo(new Dictionary<string, string>()));
            Assert.That(resultModel.PossibleClaimSets, Is.EqualTo(new Dictionary<string, string>()));
            Assert.That(resultModel.Url, Is.EqualTo("Hello World"));
        }
    }

    public class When_getting_title_claim_set_model_with_claim_set_mappings_of_single_lea : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            adminAreaLinks = mocks.StrictMock<IAdminAreaLinks>();
            staffEdOrgInfoRepo = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(adminAreaLinks.TitleClaimSet(1)).Return("Hello World");
            Expect.Call(staffEdOrgInfoRepo.GetAll()).Return(GetPositionTitles());
            Expect.Call(claimSetMappingRepo.GetAll()).Return(GetClaimSetMappingsSingleLea());

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            resultModel = titleClaimSetService.Get(new TitleClaimSetRequest { LocalEducationAgencyId = 1 });
        }

        [Test]
        public void Should_return_empty_model()
        {
            Assert.That(resultModel.LocalEducationAgencyId, Is.EqualTo(1));
            Assert.That(resultModel.EdOrgPositionTitles, Is.EqualTo(new Dictionary<string, string>()));
            Assert.That(resultModel.ClaimSetMaps, Is.EqualTo(new Dictionary<string, string>()));
            Assert.That(resultModel.PossibleClaimSets, Is.EqualTo(new Dictionary<string, string>()));
            Assert.That(resultModel.Url, Is.EqualTo("Hello World"));
        }
    }

    public class When_getting_title_claim_set_model : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            adminAreaLinks = mocks.StrictMock<IAdminAreaLinks>();
            staffEdOrgInfoRepo = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(adminAreaLinks.TitleClaimSet(1)).Return("Hello World");
            Expect.Call(staffEdOrgInfoRepo.GetAll()).Return(GetPositionTitles());
            Expect.Call(claimSetMappingRepo.GetAll()).Return(GetClaimSetMappings());

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            resultModel = titleClaimSetService.Get(new TitleClaimSetRequest { LocalEducationAgencyId = 1 });
        }

        [Test]
        public void Should_return_model_with_valid_position_titles()
        {
            Assert.That(resultModel.LocalEducationAgencyId, Is.EqualTo(1));

            Assert.That(resultModel.EdOrgPositionTitles.Count, Is.EqualTo(3));
            Assert.That(resultModel.EdOrgPositionTitles.ContainsKey(suppliedPositionTitle1.ToUpper()), Is.True);
            Assert.That(resultModel.EdOrgPositionTitles.ContainsKey(suppliedPositionTitle2.ToUpper()), Is.True);
            Assert.That(resultModel.EdOrgPositionTitles.ContainsKey(suppliedPositionTitle3.ToUpper()), Is.True);
            Assert.That(resultModel.EdOrgPositionTitles.ContainsKey(suppliedPositionTitle4.ToUpper()), Is.False);
        }

        [Test]
        public void Should_return_model_with_valid_claim_set_maps()
        {
            Assert.That(resultModel.LocalEducationAgencyId, Is.EqualTo(1));

            Assert.That(resultModel.ClaimSetMaps.Count, Is.EqualTo(2));
            Assert.That(resultModel.ClaimSetMaps.ContainsKey(suppliedPositionTitle1.ToUpper()), Is.True);
            Assert.That(resultModel.ClaimSetMaps.ContainsKey(suppliedPositionTitle2.ToUpper()), Is.True);
            Assert.That(resultModel.ClaimSetMaps.ContainsKey(suppliedPositionTitle3.ToUpper()), Is.False);
            Assert.That(resultModel.ClaimSetMaps[suppliedPositionTitle1.ToUpper()], Is.EqualTo(suppliedClaimSet1.Replace(" ", "")));
            Assert.That(resultModel.ClaimSetMaps[suppliedPositionTitle2.ToUpper()], Is.EqualTo(suppliedClaimSet2.Replace(" ", "")));
        }

    }
    #endregion

    #region Post a single claim set
    public class When_posting_title_claim_set_model_with_no_model : TitleClaimSetServiceFixture
    {
        private string message = string.Empty;

        protected override void EstablishContext()
        {
            adminAreaLinks = mocks.StrictMock<IAdminAreaLinks>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            try
            {
                titleClaimSetService.Post(null);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }            
        }

        [Test]
        public void Verify_exception_message()
        {
            Assert.That(message, Is.EqualTo("Value cannot be null.\r\nParameter name: titleClaimSetModel"));
        }
    }

    public class When_posting_title_claim_set_model_with_no_position_title : TitleClaimSetServiceFixture
    {
        private string message = string.Empty;

        protected override void EstablishContext()
        {
            adminAreaLinks = mocks.StrictMock<IAdminAreaLinks>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            try
            {
                titleClaimSetService.Post(new TitleClaimSetModel { PositionTitle = string.Empty });
            }
            catch (Exception ex)
            {
                message = ex.Message;
            } 
        }

        [Test]
        public void Verify_exception_message()
        {
            Assert.That(message, Is.EqualTo("titleClaimSetModel: PositionTitle cannot be empty."));
        }
    }

    public class When_posting_title_claim_set_model_with_no_ClaimSet : TitleClaimSetServiceFixture
    {
        private string message = string.Empty;

        protected override void EstablishContext()
        {
            adminAreaLinks = mocks.StrictMock<IAdminAreaLinks>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            try
            {
                titleClaimSetService.Post(new TitleClaimSetModel { PositionTitle = suppliedPositionTitle1, ClaimSet = string.Empty });
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        [Test]
        public void Verify_exception_message()
        {
            Assert.That(message, Is.EqualTo("titleClaimSetModel: ClaimSet cannot be empty."));
        }
    }

    public class When_posting_title_claim_set_model_with_new_claim_set_mapping : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            adminAreaLinks = mocks.StrictMock<IAdminAreaLinks>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(claimSetMappingRepo.GetAll()).Return(GetClaimSetMappings());
            Expect.Call(() => claimSetMappingRepo.Save(new ClaimSetMapping())).Constraints(Property.Value("LocalEducationAgencyId", suppliedLea1) && Property.Value("PositionTitle", suppliedPositionTitle3) && Property.Value("ClaimSet", suppliedClaimSet1));

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            titleClaimSetService.Post(new TitleClaimSetModel { LocalEducationAgencyId = suppliedLea1, PositionTitle = suppliedPositionTitle3, ClaimSet = suppliedClaimSet1 });
        }

        [Test]
        public void Dummy_Test()
        {
            
        }
    }

    public class When_posting_title_claim_set_model_with_an_existing_claim_set_mapping : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            adminAreaLinks = mocks.StrictMock<IAdminAreaLinks>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(claimSetMappingRepo.GetAll()).Return(GetClaimSetMappings());
            Expect.Call(() => claimSetMappingRepo.Save(new ClaimSetMapping())).Constraints(Property.Value("LocalEducationAgencyId", suppliedLea1) && Property.Value("PositionTitle", suppliedPositionTitle2) && Property.Value("ClaimSet", suppliedClaimSet1));

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            titleClaimSetService.Post(new TitleClaimSetModel { LocalEducationAgencyId = suppliedLea1, PositionTitle = suppliedPositionTitle2, ClaimSet = suppliedClaimSet1 });
        }

        [Test]
        public void Dummy_Test()
        {

        }
    }
    #endregion

    #region Post a batch file input stream to be processed
    public class When_posting_batch_title_claim_set_model_with_a_non_csv_file : TitleClaimSetServiceFixture
    {
        protected override void ExecuteTest()
        {
            postResultModel = titleClaimSetService.PostBatch(new TitleClaimSetModel { FileName = "Test.txt" });
        }

        [Test]
        public void Verify_Post_Results_Model()
        {
            Assert.That(postResultModel.IsPost, Is.True);
            Assert.That(postResultModel.IsSuccess, Is.False);
            Assert.That(postResultModel.Messages.Count, Is.EqualTo(1));
            Assert.That(postResultModel.Messages[0], Is.EqualTo("Test.txt is not a CSV file."));
        }
    }

    public class When_posting_batch_title_claim_set_model_with_a_field_count_less_than_2 : TitleClaimSetServiceFixture
    {
        protected override void ExecuteTest()
        {
            var csvFile = new StringBuilder("Position Title");
            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(csvFile.ToString()));

            postResultModel = titleClaimSetService.PostBatch(new TitleClaimSetModel { FileName = "Test.csv", FileInputStream = fileStream });
        }

        [Test]
        public void Verify_Post_Results_Model()
        {
            Assert.That(postResultModel.IsPost, Is.True);
            Assert.That(postResultModel.IsSuccess, Is.False);
            Assert.That(postResultModel.Messages.Count, Is.EqualTo(1));
            Assert.That(postResultModel.Messages[0], Is.EqualTo("Only 2 columns should exist.  Position Title and ClaimSet."));
        }
    }

    public class When_posting_batch_title_claim_set_model_with_a_field_count_greater_than_2 : TitleClaimSetServiceFixture
    {
        protected override void ExecuteTest()
        {
            var csvFile = new StringBuilder("Position Title, ClaimSet, Another Column");
            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(csvFile.ToString()));

            postResultModel = titleClaimSetService.PostBatch(new TitleClaimSetModel { FileName = "Test.csv", FileInputStream = fileStream });
        }

        [Test]
        public void Verify_Post_Results_Model()
        {
            Assert.That(postResultModel.IsPost, Is.True);
            Assert.That(postResultModel.IsSuccess, Is.False);
            Assert.That(postResultModel.Messages.Count, Is.EqualTo(1));
            Assert.That(postResultModel.Messages[0], Is.EqualTo("Only 2 columns should exist.  Position Title and ClaimSet."));
        }
    }

    public class When_posting_batch_title_claim_set_model_with_position_title_being_an_empty_value : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            staffEdOrgInfoRepo = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(staffEdOrgInfoRepo.GetAll()).Return(GetPositionTitles());
            Expect.Call(claimSetMappingRepo.GetAll()).Return(GetClaimSetMappings());

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var csvFile = GetCSVFileAndHeaders();
            csvFile.AppendLine(",None");

            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(csvFile.ToString()));

            postResultModel = titleClaimSetService.PostBatch(new TitleClaimSetModel { LocalEducationAgencyId = suppliedLea3, FileName = "Test.csv", FileInputStream = fileStream });
        }

        [Test]
        public void Verify_Post_Results_Model()
        {
            Assert.That(postResultModel.IsPost, Is.True);
            Assert.That(postResultModel.IsSuccess, Is.False);
            Assert.That(postResultModel.Messages.Count, Is.EqualTo(2));
            Assert.That(postResultModel.Messages[0], Is.EqualTo("0 of 1 records were successfully processed."));
            Assert.That(postResultModel.Messages[1], Is.EqualTo("Row 1 is not allowed to have an empty position title value."));
        }
    }

    public class When_posting_batch_title_claim_set_model_with_claim_set_being_an_empty_value : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            staffEdOrgInfoRepo = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(staffEdOrgInfoRepo.GetAll()).Return(GetPositionTitles());
            Expect.Call(claimSetMappingRepo.GetAll()).Return(GetClaimSetMappings());

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var csvFile = GetCSVFileAndHeaders();
            csvFile.AppendLine("Test,");

            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(csvFile.ToString()));

            postResultModel = titleClaimSetService.PostBatch(new TitleClaimSetModel { LocalEducationAgencyId = suppliedLea3, FileName = "Test.csv", FileInputStream = fileStream });
        }

        [Test]
        public void Verify_Post_Results_Model()
        {
            Assert.That(postResultModel.IsPost, Is.True);
            Assert.That(postResultModel.IsSuccess, Is.True);
            Assert.That(postResultModel.Messages.Count, Is.EqualTo(1));
            Assert.That(postResultModel.Messages[0], Is.EqualTo("0 row(s) were successfully processed."));
        }
    }

    public class When_posting_batch_title_claim_set_model_with_position_title_not_existing_in_list : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            staffEdOrgInfoRepo = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(staffEdOrgInfoRepo.GetAll()).Return(GetPositionTitles());
            Expect.Call(claimSetMappingRepo.GetAll()).Return(GetClaimSetMappings());

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var csvFile = GetCSVFileAndHeaders();
            csvFile.AppendLine(string.Format("Test, {0}", suppliedClaimSet4));

            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(csvFile.ToString()));

            postResultModel = titleClaimSetService.PostBatch(new TitleClaimSetModel { LocalEducationAgencyId = suppliedLea3, FileName = "Test.csv", FileInputStream = fileStream });
        }

        [Test]
        public void Verify_Post_Results_Model()
        {
            Assert.That(postResultModel.IsPost, Is.True);
            Assert.That(postResultModel.IsSuccess, Is.False);
            Assert.That(postResultModel.Messages.Count, Is.EqualTo(2));
            Assert.That(postResultModel.Messages[0], Is.EqualTo("0 of 1 records were successfully processed."));
            Assert.That(postResultModel.Messages[1], Is.EqualTo("Position Title TEST does not exist."));
        }
    }

    public class When_posting_batch_title_claim_set_model_with_claim_set_not_existing_in_list : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            staffEdOrgInfoRepo = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(staffEdOrgInfoRepo.GetAll()).Return(GetPositionTitles());
            Expect.Call(claimSetMappingRepo.GetAll()).Return(GetClaimSetMappings());

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var csvFile = GetCSVFileAndHeaders();
            csvFile.AppendLine(string.Format("{0}, {1}", suppliedPositionTitle2, suppliedClaimSet1));

            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(csvFile.ToString()));

            postResultModel = titleClaimSetService.PostBatch(new TitleClaimSetModel { LocalEducationAgencyId = suppliedLea3, FileName = "Test.csv", FileInputStream = fileStream });
        }

        [Test]
        public void Verify_Post_Results_Model()
        {
            Assert.That(postResultModel.IsPost, Is.True);
            Assert.That(postResultModel.IsSuccess, Is.False);
            Assert.That(postResultModel.Messages.Count, Is.EqualTo(2));
            Assert.That(postResultModel.Messages[0], Is.EqualTo("0 of 1 records were successfully processed."));
            Assert.That(postResultModel.Messages[1], Is.EqualTo(string.Format("ClaimSet Name {0} does not exist.", suppliedClaimSet1)));
        }
    }

    public class When_posting_batch_title_claim_set_model_with_total_records_not_equal_processed_records : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            staffEdOrgInfoRepo = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(staffEdOrgInfoRepo.GetAll()).Return(GetPositionTitles());
            Expect.Call(claimSetMappingRepo.GetAll()).Return(GetClaimSetMappings());
            Expect.Call(() => claimSetMappingRepo.Save(null)).IgnoreArguments();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var csvFile = GetCSVFileAndHeaders();
            csvFile.AppendLine(string.Format("{0}, {1}", suppliedPositionTitle2, suppliedClaimSet1));
            csvFile.AppendLine(string.Format("{0}, {1}", suppliedPositionTitle1, suppliedClaimSet4));

            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(csvFile.ToString()));

            postResultModel = titleClaimSetService.PostBatch(new TitleClaimSetModel { LocalEducationAgencyId = suppliedLea3, FileName = "Test.csv", FileInputStream = fileStream });
        }

        [Test]
        public void Verify_Post_Results_Model()
        {
            Assert.That(postResultModel.IsPost, Is.True);
            Assert.That(postResultModel.IsSuccess, Is.False);
            Assert.That(postResultModel.Messages.Count, Is.EqualTo(2));
            Assert.That(postResultModel.Messages[0], Is.EqualTo("1 of 2 records were successfully processed."));
            Assert.That(postResultModel.Messages[1], Is.EqualTo(string.Format("ClaimSet Name {0} does not exist.", suppliedClaimSet1)));
        }
    }

    public class When_posting_batch_title_claim_set_model_with_total_records_equal_processed_records : TitleClaimSetServiceFixture
    {
        protected override void EstablishContext()
        {
            staffEdOrgInfoRepo = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            claimSetMappingRepo = mocks.StrictMock<IPersistingRepository<ClaimSetMapping>>();

            Expect.Call(staffEdOrgInfoRepo.GetAll()).Return(GetPositionTitles());
            Expect.Call(claimSetMappingRepo.GetAll()).Return(GetClaimSetMappings());
            Expect.Call(() => claimSetMappingRepo.Save(null)).IgnoreArguments();
            Expect.Call(() => claimSetMappingRepo.Save(null)).IgnoreArguments();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var csvFile = GetCSVFileAndHeaders();
            csvFile.AppendLine(string.Format("{0}, {1}", suppliedPositionTitle3, suppliedClaimSet5));
            csvFile.AppendLine(string.Format("{0}, {1}", suppliedPositionTitle1, suppliedClaimSet4));

            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(csvFile.ToString()));

            postResultModel = titleClaimSetService.PostBatch(new TitleClaimSetModel { LocalEducationAgencyId = suppliedLea3, FileName = "Test.csv", FileInputStream = fileStream });
        }

        [Test]
        public void Verify_Post_Results_Model()
        {
            Assert.That(postResultModel.IsPost, Is.True);
            Assert.That(postResultModel.IsSuccess, Is.True);
            Assert.That(postResultModel.Messages.Count, Is.EqualTo(1));
            Assert.That(postResultModel.Messages[0], Is.EqualTo("2 row(s) were successfully processed."));
        }
    }
    #endregion
}
