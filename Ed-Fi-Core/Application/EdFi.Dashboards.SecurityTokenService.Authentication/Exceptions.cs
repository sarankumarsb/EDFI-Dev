// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Runtime.Serialization;
using System.Text;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
    [Serializable]
    public abstract class DashboardsException : Exception, IErrorLogOutput
    {
        public DashboardsException() { }

        public DashboardsException(string message) : base(message) { }

        public DashboardsException(string message, Exception exception) : base(message, exception) { }

        protected DashboardsException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }

        public string Name { get; set; }
        public long StaffUSI { get; set; }
        public bool IsImpersonating { get; set; }

        public virtual string ToErrorLogText()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("User '{0}' (Staff USI: {1}) logged into the dashboard, yet was not issued claims to access the dashboard. Should this user be allowed to access dashboards for their associated organizations?", Name, StaffUSI);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(Message);

            return stringBuilder.ToString();
        }
    }

    [Serializable]
    public class DashboardsAuthenticationException : DashboardsException
    {
        public DashboardsAuthenticationException() { }

        public DashboardsAuthenticationException(string message) : base(message) { }

        public DashboardsAuthenticationException(string message, Exception exception) : base(message, exception) { }

        protected DashboardsAuthenticationException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }

        public override string ToErrorLogText()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("User '{0}' (Staff USI: {1}, Email: ?) logged into the dashboard, yet was not issued claims to access the dashboard. Should this user be allowed to access dashboards for their associated organizations?", Name, StaffUSI);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(Message);

            return stringBuilder.ToString();
        }
    }

    [Serializable]
    public class DistrictProtocolAuthenticationException : DashboardsException
    {
        public DistrictProtocolAuthenticationException() { }

        public DistrictProtocolAuthenticationException(string message) : base(message) { }

        public DistrictProtocolAuthenticationException(string message, Exception exception) : base(message, exception) { }

        protected DistrictProtocolAuthenticationException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
        public override string ToErrorLogText()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("User was not successfully identified with these credentials against the LDAP.  User authentication not accurate.  User '{0}' (Staff USI: {1}, Email: ?)", Name, StaffUSI);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(Message);

            return stringBuilder.ToString();
        }
    }

    [Serializable]
    public class DashboardsMissingStaffAuthenticationException : DashboardsException
    {
        public DashboardsMissingStaffAuthenticationException() { }

        public DashboardsMissingStaffAuthenticationException(string message) : base(message) { }

        public DashboardsMissingStaffAuthenticationException(string message, Exception exception) : base(message, exception) { }

        protected DashboardsMissingStaffAuthenticationException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
        public override string ToErrorLogText()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("Staff member not currently in StudentGPS system.  Staff not in system (verify in HR system) or could be a new employee (verify in 24 hours).  User '{0}' (Staff USI: {1}, Email: ?)", Name, StaffUSI);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(Message);

            return stringBuilder.ToString();
        }
    }

    [Serializable]
    public class DashboardsMultipleStaffAuthenticationException : DashboardsException
    {
        public DashboardsMultipleStaffAuthenticationException() { }

        public DashboardsMultipleStaffAuthenticationException(string message) : base(message) { }

        public DashboardsMultipleStaffAuthenticationException(string message, Exception exception) : base(message, exception) { }

        protected DashboardsMultipleStaffAuthenticationException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
        public override string ToErrorLogText()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("Multiple staff members currently in StudentGPS system.  User '{0}' (Staff USI: {1}, Email: ?)", Name, StaffUSI);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(Message);

            return stringBuilder.ToString();
        }
    }

    [Serializable]
    public class StaffSchoolClassAssociationException : DashboardsException
    {
        public StaffSchoolClassAssociationException() { }

        public StaffSchoolClassAssociationException(string message) : base(message) { }

        public StaffSchoolClassAssociationException(string message, Exception exception) : base(message, exception) { }

        protected StaffSchoolClassAssociationException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
        public override string ToErrorLogText()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("Staff does not have an association to either a school or class section.  User '{0}' (Staff USI: {1}, Email: ?)", Name, StaffUSI);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(Message);

            return stringBuilder.ToString();
        }
    }

    [Serializable]
    public class StaffPositionTitleAssociationException<TUserSecurityDetails> : DashboardsException where TUserSecurityDetails : IErrorLogOutput
    {
        public StaffPositionTitleAssociationException() { }

        public StaffPositionTitleAssociationException(string message) : base(message) { }

        public StaffPositionTitleAssociationException(string message, Exception exception) : base(message, exception) { }

        protected StaffPositionTitleAssociationException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }

        public DashboardUserClaimsInformation<TUserSecurityDetails> ClaimsInformation { get; set; }

        public override string ToErrorLogText()
        {
            var stringBuilder = new StringBuilder(base.ToErrorLogText());
            stringBuilder.AppendFormat("Position title is not associated with a user access role or dashboard claim set.  User '{0}' (Staff USI: {1}, Email: {2})", Name, StaffUSI, ClaimsInformation.Email);
            stringBuilder.AppendLine();
            if (ClaimsInformation != null)
            {
                stringBuilder.AppendLine();

                foreach (var org in ClaimsInformation.AssociatedOrganizations)
                {
                    //Position title is not associated with a user access role or dashboard claim set.  User '{0}' (Staff USI: {1}, Email: {2})
                    stringBuilder.AppendFormat("Local Education Agency: '{0}'  School: '{1}'  {2}", org.Ids.LocalEducationAgencyId, org.Ids.SchoolId, org.SecurityDetails.ToErrorLogText());
                    stringBuilder.AppendLine();
                }
            }

            return stringBuilder.ToString();
        }
    }
}
