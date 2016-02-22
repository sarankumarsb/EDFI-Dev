namespace EdFi.Dashboards.Resources.Security
{
    public interface IStaffInformationFromEmailProvider
    {
        long ResolveStaffUSI(string email);
    }
}
