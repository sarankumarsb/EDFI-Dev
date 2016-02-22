// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Navigation
{
    public interface ICommonLinks 
    {
        //string Default();
        //string Default(string returnUrl);
        //string Image(string image, string imageType);
        //string Image(string image, string display, string imageType);
        //string ApplicationImage(string image);
        string ThemeImage(string image);
        //string ThemeResource(string fileName);
        //string Script(string fileName);
        string Content(string fileName);
        string Feedback();
        string EdFiGridWebService_SetSessionObject(int localEducationAgencyId);
    }
}