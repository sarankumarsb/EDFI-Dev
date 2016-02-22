<%@ Page Language="C#" MasterPageFile="~/NewDialog.Master" AutoEventWireup="true" CodeBehind="UserAccessDenied.aspx.cs" 
         Inherits="EdFi.Dashboards.SecurityTokenService.Web.UserAccessDenied" Theme=""  %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DialogContent" runat="server">
    <div class="l-right">
            <p>There was a problem while trying to log you in. You do not have access to the system.</p>
            <a href="#" id="buttonErrorBack" class="buttonImage uppercase" <%= HomeCustomTags %>>Home</a>
            <a href="#" id="buttonErrorFeedback" class="buttonImage feedback uppercase">Support</a>

            <div class="tinyLabel">Having trouble? <a id="feedbackLink" style="color:#255b80;" href="#">Please contact support.</a></div>
    </div>
</asp:Content>
