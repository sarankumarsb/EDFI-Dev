// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

namespace EdFi.Dashboards.Resources.Navigation.UserEntryProviders
{
    public interface IUserEntryProvider
    {
        string GetUserEntry(EntryRequest request);
    }
}