// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Infrastructure
{
    public interface ISiteAvailableProvider
    {
        bool IsKillSwitchActivatedForCurrentUser(int localEducationAgencyId);
    }
}
