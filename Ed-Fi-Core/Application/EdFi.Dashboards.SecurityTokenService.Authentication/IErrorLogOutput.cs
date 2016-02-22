namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
    /// <summary>
    /// Defines a method to output a string for error logging purposes.
    /// </summary>
    public interface IErrorLogOutput
    {
        /// <summary>
        /// Returns a string representation of the object for error logging.
        /// </summary>
        /// <returns>A formatted a string that represents details of the object being logged.</returns>
        string ToErrorLogText();
    }
}
