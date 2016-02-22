<%@ Page Language="C#" MasterPageFile="~/NewDialog.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" 
         Inherits="EdFi.Dashboards.SecurityTokenService.Web.Error" Theme=""  %>
<%@ Import Namespace="EdFi.Dashboards.SecurityTokenService.Authentication" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DialogContent" runat="server">

    <script type='text/javascript'>
        $(function () {
            var $exDialog = $('#exception-dialog')
                    .dialog({
                        autoOpen: false,
                        height: 600,
                        width: 800,
                        title: 'Exception Details',
                        modal: true,
                        buttons: {
                            'Close': function () {
                                $(this).dialog('close');
                            }
                        }
                    });

            $('#ExceptionDetailsLink').show();

            $('#ExceptionDetailsLink').click(function () {
                $exDialog.dialog('open');
                // prevent the default action, e.g., following a link
                return false;
            });
        })
    </script>

    <div class="l-right">
            <p>There was a problem while trying to log you in.</p>
            <a href="#" id="buttonErrorBack" class="buttonImage uppercase" onclick="history.back()">Back</a>
            <a href="#" id="buttonErrorFeedback" class="buttonImage feedback uppercase">Support</a>
                <div class="tinyLabel" style="padding-top: 10px;"><a id="ExceptionDetailsLink" style="color:#255b80;" href="#">More Details</a></div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FooterMessage" runat="server">
    <asp:Label ID="ErrorMessageLabel" runat="server" class="error"  />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalContent" runat="server">
    <div id="exception-dialog" style="display: none; ">
        <fieldset>
            <label for="name">Contents:</label>
            <div style="font-family: Consolas, Courier New; font-size: 8pt; overflow: auto;">
                <asp:Literal ID="ExceptionDetailsLiteral" runat="server" />
            </div>
        </fieldset>
    </div>
</asp:Content>

    