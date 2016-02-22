using System.Linq;
using EdFi.Dashboards.Data.Repository;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    /// <summary>
    /// Gets the identifier and name for a local education agency.
    /// </summary>
    public interface ICodeIdProvider
    {
        int Get(string code);
    }

    /// <summary>
    /// Gets the identifier and name for a local education agency.
    /// </summary>
    public class CodeIdProvider : ICodeIdProvider
    {
        private readonly IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeIdProvider"/> class.
        /// </summary>
        /// <param name="repository">Provides access to the <see cref="EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency"/> data.</param>
        public CodeIdProvider(IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets the identifier and name for the local education agency.
        /// </summary>
        /// <param name="code">The local education agency code to resolve.</param>
        /// <returns>The local education agency id, or <b>0</b> if none was found.</returns>
        public int Get(string code)
        {
            var model = (from lea in repository.GetAll()
                         where lea.Code == code
                         select lea.LocalEducationAgencyId)
                            .SingleOrDefault();

            return model;
        }
    }
}
