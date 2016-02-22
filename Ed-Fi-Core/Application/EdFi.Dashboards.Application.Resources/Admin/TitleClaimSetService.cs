using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Models;
using EdFi.Dashboards.Application.Resources.Models.Admin;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using LumenWorks.Framework.IO.Csv;

namespace EdFi.Dashboards.Application.Resources.Admin
{
    public class TitleClaimSetRequest
    {
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleClaimSetRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="TitleClaimSetRequest"/> instance.</returns>
        public static TitleClaimSetRequest Create(int localEducationAgencyId)
        {
            return new TitleClaimSetRequest { LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public interface ITitleClaimSetService : IService<TitleClaimSetRequest, TitleClaimSetModel>
    {
        void Post(TitleClaimSetModel titleClaimSetModel);
        PostResultsModel PostBatch(TitleClaimSetModel titleClaimSetModel);
    }

    public class TitleClaimSetService : TitleClaimSetServiceBase, ITitleClaimSetService
    {
        private readonly IAdminAreaLinks _adminAreaLinks;

        public TitleClaimSetService(IAdminAreaLinks adminAreaLinks)
        {
            _adminAreaLinks = adminAreaLinks;
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public TitleClaimSetModel Get(TitleClaimSetRequest request)
        {
            //Get all the different position titles
            var staffEdOrgPositionTitles = GetUniquePositionTitles();

            if (staffEdOrgPositionTitles.Count == 0)
                return new TitleClaimSetModel()
                {
                    LocalEducationAgencyId = request.LocalEducationAgencyId,
                    EdOrgPositionTitles = new Dictionary<string, string>(),
                    ClaimSetMaps = new Dictionary<string, string>(),
                    PossibleClaimSets = new Dictionary<string, string>(),
                    Url = _adminAreaLinks.TitleClaimSet(request.LocalEducationAgencyId)
                };

            //Get a dictionary of current title ClaimSet mappings
            var claimSetMaps = GetClaimSetMappings(request.LocalEducationAgencyId).ToDictionary(ClaimSetMap => ClaimSetMap.PositionTitle.Trim().ToUpper(), ClaimSetMap => ClaimSetMap.ClaimSet.Trim().Replace(" ", ""));

            if (claimSetMaps.Count == 0)
                return new TitleClaimSetModel()
                {
                    LocalEducationAgencyId = request.LocalEducationAgencyId,
                    EdOrgPositionTitles = new Dictionary<string, string>(),
                    ClaimSetMaps = new Dictionary<string, string>(),
                    PossibleClaimSets = new Dictionary<string, string>(),
                    Url = _adminAreaLinks.TitleClaimSet(request.LocalEducationAgencyId)
                };

            //Get a list of possible ClaimSets
            var possibleClaimSets = GetPossibleClaimSets();

            return new TitleClaimSetModel
            {
                LocalEducationAgencyId = request.LocalEducationAgencyId,
                EdOrgPositionTitles = staffEdOrgPositionTitles.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value),
                ClaimSetMaps = claimSetMaps,
                PossibleClaimSets = possibleClaimSets.ToDictionary(x => x, x => x),
                Url = _adminAreaLinks.TitleClaimSet(request.LocalEducationAgencyId)
            };
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public void Post(TitleClaimSetModel titleClaimSetModel)
        {
            if (titleClaimSetModel == null)
                throw new ArgumentNullException("titleClaimSetModel");

            if (string.IsNullOrEmpty(titleClaimSetModel.PositionTitle))
                throw new ArgumentException("titleClaimSetModel: PositionTitle cannot be empty.");

            if (string.IsNullOrEmpty(titleClaimSetModel.ClaimSet))
                throw new ArgumentException("titleClaimSetModel: ClaimSet cannot be empty.");

            var existingTitleClaimSets = GetClaimSetMappings(titleClaimSetModel.LocalEducationAgencyId);

            if (existingTitleClaimSets.Count > 0)
            {
                SaveClaimSetMapping(existingTitleClaimSets, titleClaimSetModel.PositionTitle, titleClaimSetModel.ClaimSet, titleClaimSetModel.LocalEducationAgencyId);
            }
        }

        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.AdministerDashboard)]
        public PostResultsModel PostBatch(TitleClaimSetModel titleClaimSetModel)
        {
            //validate fileName
            if (!titleClaimSetModel.FileName.EndsWith(".csv"))
                return new PostResultsModel
                {
                    IsPost = true,
                    IsSuccess = false,
                    Messages = new List<string> { string.Format("{0} is not a CSV file.", titleClaimSetModel.FileName) }
                };


            var recordsProcessed = 0;
            var totalRecords = 0;
            var errorMessages = new List<string>();

            using (var csvReader = new CsvReader(new StreamReader(titleClaimSetModel.FileInputStream), true))
            {
                //validate number of columns
                if (csvReader.FieldCount != 2)
                    return new PostResultsModel
                    {
                        IsPost = true,
                        IsSuccess = false,
                        Messages = new List<string> { "Only 2 columns should exist.  Position Title and ClaimSet." }
                    };

                //Get all the unique Position Titles
                var staffEdOrgPositionTitles = GetUniquePositionTitles();

                //Get all the ClaimSets
                var possibleClaimSets = GetPossibleClaimSets();

                //get all the existing values out of the database
                var existingTitleClaimSets = GetClaimSetMappings(titleClaimSetModel.LocalEducationAgencyId);

                while (csvReader.ReadNextRecord())
                {
                    totalRecords++;

                    var positionTitle = csvReader[0];
                    var claimSet = csvReader[1];
                    var inputClaimSet = claimSet;
                    //putting a try catch here because we still want to process all records even if one fails.
                    try
                    {

                        //check for null values
                        if (string.IsNullOrWhiteSpace(positionTitle))
                        {
                            errorMessages.Add(string.Format("Row {0} is not allowed to have an empty position title value.", csvReader.CurrentRecordIndex + 1));
                            continue;
                        }

                        //for empty ClaimSet values we just want to skip
                        if (string.IsNullOrEmpty(csvReader[1]))
                        {
                            //since we are skipping we don't want to count this towards total records
                            totalRecords--;
                            continue;
                        }

                        positionTitle = positionTitle.Trim().ToUpper();
                        claimSet = claimSet.Trim().Replace(" ", "");

                        //check that the position title really exists as something we already have
                        if (!staffEdOrgPositionTitles.ContainsKey(positionTitle))
                        {
                            errorMessages.Add(string.Format("Position Title {0} does not exist.", positionTitle));
                            continue;
                        }

                        //check that the ClaimSet is something that we have defined
                        var matchedClaimSet = possibleClaimSets.FirstOrDefault(x => string.Compare(x, claimSet, true) == 0);
                        if (matchedClaimSet == null)
                        {
                            errorMessages.Add(string.Format("ClaimSet Name {0} does not exist.", inputClaimSet));
                            continue;
                        }

                        SaveClaimSetMapping(existingTitleClaimSets, positionTitle, matchedClaimSet, titleClaimSetModel.LocalEducationAgencyId);

                        recordsProcessed++;
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add(string.Format("An unhandled error occurred while processing Position Title {0} on row {1}. {2}", positionTitle, csvReader.CurrentRecordIndex + 1, ex));
                    }
                }
            }

            if (totalRecords == recordsProcessed)
            {
                return new PostResultsModel
                {
                    IsPost = true,
                    IsSuccess = true,
                    Messages = new List<string> { string.Format("{0} row(s) were successfully processed.", recordsProcessed) }
                };
            }

            var model = new PostResultsModel
            {
                IsPost = true,
                IsSuccess = false,
                Messages = new List<string>(errorMessages)
            };

            model.Messages.Insert(0, string.Format("{0} of {1} records were successfully processed.", recordsProcessed, totalRecords));
            return model;
        }

        #region Private Methods
        private void SaveClaimSetMapping(IEnumerable<ClaimSetMapping> existingTitleClaimSets, string positionTitle, string ClaimSet, int localEducationAgencyId)
        {
            var existingTitleClaimSet = existingTitleClaimSets.SingleOrDefault(x => string.Compare(x.PositionTitle, positionTitle, true) == 0);

            if (existingTitleClaimSet == null)
            {
                existingTitleClaimSet = new ClaimSetMapping
                {
                    LocalEducationAgencyId = localEducationAgencyId,
                    ClaimSet = ClaimSet,
                    PositionTitle = positionTitle
                };
            }
            else
            {
                existingTitleClaimSet.ClaimSet = ClaimSet;
            }

            ClaimSetMappingRepo.Save(existingTitleClaimSet);
        }

        private static IList<string> GetPossibleClaimSets()
        {
            var claimsSets = Enum.GetValues(typeof(ClaimsSet));

            var result = new List<string>();
            result.Add("None");
            foreach (var claimSet in claimsSets)
            {
                if ((ClaimsSet)claimSet != ClaimsSet.Impersonation)
                    result.Add(claimSet.ToString());
            }

            return result;
        }
        #endregion
    }
}
