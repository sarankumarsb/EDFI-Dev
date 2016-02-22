// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    /// <summary>
    /// Represents a request to the <see cref="IIdCodeService"/>.
    /// </summary>
    public class IdCodeRequest
    {
        /// <summary>
        /// The identifier of the local education agency.
        /// </summary>
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// The code of the local education agency.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdCodeRequest"/> class with the specified local education agency identifier.
        /// </summary>
        /// <param name="id">The identifier of the local education agency.</param>
        /// <param name="code">The code of the local education agency.</param>
        /// <returns>The initialized request.</returns>
        public static IdCodeRequest Create(int id = 0, string code = null)
        {
            return new IdCodeRequest
                       {
                           LocalEducationAgencyId = id,
                           Code = code,
                       };
        }
    }

    /// <summary>
    /// Gets the identifier and name for a local education agency.
    /// </summary>
    public interface IIdCodeService : IService<IdCodeRequest, IdCodeModel> { }

    /// <summary>
    /// Gets the identifier and name for a local education agency.
    /// </summary>
    public class IdCodeService : IIdCodeService
    {
        private readonly IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdCodeService"/> class.
        /// </summary>
        /// <param name="repository">Provides access to the <see cref="EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency"/> data.</param>
        public IdCodeService(IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets the identifier and name for the local education agency.
        /// </summary>
        /// <param name="request">The request object containing the parameters for the service call.</param>
        /// <returns>The initialized <see cref="IdCodeModel"/> instance, or <b>null</b> if none was found.</returns>
        [AuthenticationIgnore("Local Education Agency Name is a public record.")] // DJWhite: 26 Jan 2012: This service is used by claim validation. Attempting to authorize this service will cause a stack overflow. 
        public IdCodeModel Get(IdCodeRequest request)
        {
            var model =
                (from lea in repository.GetAll()
                 where lea.LocalEducationAgencyId == request.LocalEducationAgencyId || lea.Code == request.Code
                 select new IdCodeModel
                            {
                                LocalEducationAgencyId = lea.LocalEducationAgencyId,
                                Code = lea.Code,
                                Name = lea.Name,
                            })
                    .SingleOrDefault();

            return model;
        }
    }
}
