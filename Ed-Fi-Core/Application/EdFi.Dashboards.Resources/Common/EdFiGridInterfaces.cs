
namespace EdFi.Dashboards.Resources.Common
{
    /// <summary>
    /// Used to allow a request to be converted to an EdFiGridMetaRequest.
    /// </summary>
    public interface IEdFiGridMetaRequestConvertable
    {
        /// <summary>
        /// Converts the current request to a meta request using the data in
        /// the current object.
        /// </summary>
        /// <returns>An <see cref="EdFiGridMetaRequest"/> object.</returns>
        EdFiGridMetaRequest ConvertToGridMetaRequest();
    }
}
