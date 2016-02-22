using System;

namespace EdFi.Dashboards.SecurityTokenService.Web
{
    public partial class NewDialog : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            // using <%= %> blocks inside the <head></head> tag in the master page causes ASP.NET to throw an exception
            // using <%# %> (binding) fixes the issue but requires the following code
            base.OnLoad(e);
            Page.Header.DataBind();
        }

        public string DistrictName()
        {
            return Request["leaName"] ?? String.Empty;
        }
    }
}