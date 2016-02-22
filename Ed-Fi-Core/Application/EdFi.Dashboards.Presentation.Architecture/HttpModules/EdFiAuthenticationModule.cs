// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Web.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;

namespace EdFi.Dashboards.Presentation.Architecture.HttpModules
{
    public class EdFiAuthenticationModule : SessionAuthenticationModule
    {
        private static TimeSpan sessionTimeout = TimeSpan.Zero;

         public EdFiAuthenticationModule()
         {
             SessionSecurityTokenReceived += SessionAuthenticationModule_SessionSecurityTokenReceived;
             SignedOut += SessionAuthenticationModule_SignedOut;
         }

         protected void SessionAuthenticationModule_SessionSecurityTokenReceived(object sender, SessionSecurityTokenReceivedEventArgs e)
         {
             // Initialize session timeout value from web.config
             InitializeSessionTimeout();

             var securityTokenHandler = ServiceConfiguration.SecurityTokenHandlers[typeof(SessionSecurityToken)] as SessionSecurityTokenHandler;
             var maxClockSkew = securityTokenHandler.Configuration.MaxClockSkew;

             DateTime now = DateTime.UtcNow;
             DateTime validFrom = e.SessionToken.ValidFrom;
             DateTime validTo = e.SessionToken.ValidTo;

             double halfSpan = (validTo - validFrom).TotalMinutes / 2;

             // Reissue sliding expiration token only if we're in the second half of the active session period.
             // WIF book says this is the same strategy used by out-of-the-box ASP.NET Forms Authentication
             if (validFrom.AddMinutes(halfSpan) < now && now < validTo.Add(maxClockSkew))
             {
                 var sam = sender as SessionAuthenticationModule;
                 e.SessionToken = sam.CreateSessionSecurityToken(
                     e.SessionToken.ClaimsPrincipal,
                     e.SessionToken.Context,
                     now, now.AddMinutes(sessionTimeout.TotalMinutes), e.SessionToken.IsPersistent);
                 e.ReissueCookie = true;
             }
         }

         protected void SessionAuthenticationModule_SignedOut(object sender, EventArgs e)
         {
             var sam = sender as SessionAuthenticationModule;
             sam.DeleteSessionTokenCookie();
         }

         private static void InitializeSessionTimeout()
         {
             if (sessionTimeout == TimeSpan.Zero)
             {
                 var section = WebConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;
                 sessionTimeout = section != null ? section.Timeout : TimeSpan.FromMinutes(20);
             }
         }
    }
}