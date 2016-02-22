// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Web.Application.ViewModels;

namespace EdFi.Dashboards.SecurityTokenService.Web
{
    public partial class Error : System.Web.UI.Page
    {
        private ISessionStateProvider session;

        protected UserInformation UserInformation { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            session = IoC.Resolve<ISessionStateProvider>();

            this.UserInformation = UserInformation.Current;

            // Only generate this in Test environments...
            if (ShowExceptionDetails)
                RegisterFullExceptionScripts(); 

            SetErrorMessage();
        }

        private bool? _showExceptionDetails;

        protected bool ShowExceptionDetails
        {
            get
            {
                if (_showExceptionDetails == null)
                {
                    var configValueProvider = IoC.Resolve<IConfigValueProvider>();

                    try
                    {
                        _showExceptionDetails = Convert.ToBoolean(configValueProvider.GetValue("showExceptionDetails"));
                    }
                    catch
                    {
                        // Fail to not showing the exception details
                        _showExceptionDetails = false;
                    }
                }

                return _showExceptionDetails.Value;
            }
        }

        private void SetErrorMessage()
        {
            var ex = session[EdFiApp.Session.LastException] as ExceptionModel;

            if (ex != null)
            {
                if (typeof(ApplicationException).IsAssignableFrom(ex.Type))
                {
                    ErrorMessageLabel.Visible = true;
                    ErrorMessageLabel.Text = ex.Message;
                }
            }

            ExceptionDetailsLiteral.Text = 
                ex == null ?
                "No exception details available." 
                : ex.StackTrace.Replace("\n", "<br/>");
        }

        private void RegisterFullExceptionScripts()
        {
            if (Page.ClientScript.IsStartupScriptRegistered("HeadEXCEPTION")) return;
            const string exceptionScript =
                @"
    <script type='text/javascript'>
        $(function() {
            var $exDialog = $('#exception-dialog')
                    .dialog({
                        autoOpen: false,
                        height: 600,
                        width: 800,
                        title: 'Exception Details',
                        modal: true,
                        buttons: {
                            'Close': function() {
                                $(this).dialog('close');
                            }
                        }
                    });

            $('#ExceptionDetailsLink').show();

            $('#ExceptionDetailsLink').click(function() {
                $exDialog.dialog('open');
                // prevent the default action, e.g., following a link
                return false;
            });
        })
    </script>";
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "HeadEXCEPTION", exceptionScript);
        }
    }
}
