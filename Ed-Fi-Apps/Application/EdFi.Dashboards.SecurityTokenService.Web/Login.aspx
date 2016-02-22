<%@ Page Language="C#" MasterPageFile="~/NewDialog.Master" AutoEventWireup="true" ViewStateEncryptionMode="Always" 
         CodeBehind="Login.aspx.cs" Inherits="EdFi.Dashboards.SecurityTokenService.Web.Login" Theme="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $("#userName").focus();

        $("#login-table").keyup(function (event) {
            if (event.keyCode == '13') {
                event.preventDefault();
                $("#aspnetForm").submit();
            }
        });
    });

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DialogContent" runat="server">
	<form id="Form1" class="form-horizontal" runat="server">
        <p id="login-form-wrap custom-loginstyle">
      <%--      <h3>Log In</h3>--%>
	        <div class="control-group">
		        <label class="control-label Customlabel-align loginlabel" for="inputUsername">Username:</label>
		        <div class="controls CustomCntrl-align">
		            <input type="text" id="inputUsername" name="inputUsername" placeholder="Username" class="logininput" value="<%= Username %>"/>
		        </div>
	        </div>
	        <div class="control-group">
		        <label class="control-label Customlabel-align loginlabel" for="inputPassword">Password:</label>
		        <div class="controls CustomCntrl-align">
		            <input type="password" id="inputPassword" name="inputPassword" class="logininput" placeholder="Password"/>
		        </div>
	        </div>
	        <div class="control-group">
		        <div class="controls alignleft">
			        <label class="checkbox  loginlabel-small">
			            <input type="checkbox" runat="server" id="checkRememberMe" class="logincheck" /> Remember Me
			        </label>
		        </div>

                <div class="controls alignright">
			        <button type="submit" class="btn btn-primary Custom-bgcolor">Log in</button>
		        </div>

	        </div>
<%--	        <div class="control-group">
		        <div class="controls">
			        <button type="submit" class="btn btn-primary">Sign in</button>
		        </div>
	        </div>--%>
        <%--    <div class="tinyLabel">
                Having trouble? <a id="feedbackLink" style="color: rgb(37, 91, 128);" href="#">Please contact support.</a>
            </div>--%>
        </p>
    </form>
</asp:Content>
<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="FooterMessage">
    <asp:Label ID="ErrorMessageLabel" runat="server" class="error" Visible="false">Invalid username or password.</asp:Label>
</asp:Content>

<%--
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
--%>
