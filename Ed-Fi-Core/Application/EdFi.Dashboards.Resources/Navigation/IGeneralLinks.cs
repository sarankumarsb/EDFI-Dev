namespace EdFi.Dashboards.Resources.Navigation
{
    public interface IGeneralLinks
    {
        string Logout();
        string MetricsBasedWatchList(string resourceName, int? id = null, object additionalValues = null);
		string Resource(string resourceName, object additionalValues = null);
    }
}