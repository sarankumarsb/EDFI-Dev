// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Cassette.Views;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Presentation.Architecture.HttpModules
{
    public class WoopraAnalyticsFilter : Stream
    {
        private const string analyticsScriptFormat = "<script src='{3}' type='text/javascript'></script><script type='text/javascript' language='javascript'>TrackWoopraUsage('{0}', {1}, {2});</script>";
        private const string visitorPropertyFormat = "[['name', '{0}'], ['email', '{1}'], ['StaffUSI', '{2}'], ['LEACode', '{3}']]";
        private readonly Stream responseStream;
        private bool headStarted;

        public WoopraAnalyticsFilter(Stream inputStream)
        {
            responseStream = inputStream;
        }

        public string Domain { get; set; }
        public string UserFullName { get; set; }
        public string LocalEducationAgencyCode { get; set; }
        public string Email { get; set; }
        public long UserUSI { get; set; }

        public int? LocalEducationAgencyId { get; set; }
        public int? SchoolId { get; set; }
        public long? StudentUSI { get; set; }
        public long? StaffUSI { get; set; }
        public long? SectionOrCohortId { get; set; }
        public string StudentListType { get; set; }

        public string TrackingUrl { get; set; }

        public override void Write(byte[] buffer, int offset, int count)
        {    
            string strBuffer = Encoding.UTF8.GetString(buffer, offset, count);

            // ---------------------------------
            // Wait for the opening <head> tag
            // ---------------------------------
            if (!headStarted)
            {
                var startHead = new Regex("<head", RegexOptions.IgnoreCase);
                if (!startHead.IsMatch(strBuffer))
                {
                    responseStream.Write(buffer, 0, count);
                    return;
                }
            }

            headStarted = true;

            var endHead = new Regex("</head>", RegexOptions.IgnoreCase);
            if (!endHead.IsMatch(strBuffer))
            {
                responseStream.Write(buffer, 0, count);
                return;
            }

            var re = new Regex("(</head>)", RegexOptions.IgnoreCase);
            var modifiedBuffer = re.Replace(strBuffer, HeaderEndMatch);

            byte[] data = Encoding.UTF8.GetBytes(modifiedBuffer);
            responseStream.Write(data, 0, data.Length);
            headStarted = false;
        }

        private string FormatAnalyticsScript()
        {
            var visitorPropertyArray = String.Format(visitorPropertyFormat, UserFullName, Email, UserUSI, LocalEducationAgencyCode);

            var pageViewPropertyObject = new StringBuilder();
            var usageTracking = new StringBuilder();
            pageViewPropertyObject.AppendFormat("{{ url: '{0}', UserUSI: '{1}', UserName: '{1}-{2}'", TrackingUrl, UserUSI, UserFullName);
            usageTracking.AppendFormat("{0}|", UserUSI);

            if (LocalEducationAgencyId.HasValue)
            {
                pageViewPropertyObject.AppendFormat(", LocalEducationAgencyId: '{0}'", LocalEducationAgencyId);
                usageTracking.AppendFormat("{0}|", LocalEducationAgencyId);
            }
            else
            {
                usageTracking.Append("null|");
            }

            if (SchoolId.HasValue)
            {
                pageViewPropertyObject.AppendFormat(", SchoolId: '{0}'", SchoolId);
                usageTracking.AppendFormat("{0}|", SchoolId);
            }
            else
            {
                usageTracking.Append("null|");
            }

            if (StudentUSI.HasValue)
            {
                pageViewPropertyObject.AppendFormat(", StudentUSI: '{0}'", StudentUSI);
                usageTracking.AppendFormat("{0}|", StudentUSI);
            }
            else
            {
                usageTracking.Append("null|");
            }

            if (StaffUSI.HasValue)
            {
                pageViewPropertyObject.AppendFormat(", StaffUSI: '{0}'", StaffUSI);
                usageTracking.AppendFormat("{0}|", StaffUSI);
            }
            else
            {
                usageTracking.Append("null|");
            }

            if (SectionOrCohortId.HasValue)
            {
                pageViewPropertyObject.AppendFormat(", StudentListId: '{0}'", SectionOrCohortId);
                usageTracking.AppendFormat("{0}|", SectionOrCohortId);
            }
            else
            {
                usageTracking.Append("null|");
            }

            if (!String.IsNullOrWhiteSpace(StudentListType))
            {
                pageViewPropertyObject.AppendFormat(", StudentListType: '{0}'", StudentListType);
                usageTracking.AppendFormat("{0}|", StudentListType);
            }
            else
            {
                usageTracking.Append("null|");
            }
            pageViewPropertyObject.AppendFormat(", UsageStudy: '{0}' }}", usageTracking);

            return String.Format(analyticsScriptFormat, Domain, visitorPropertyArray, pageViewPropertyObject, Bundles.Url("Core_Content/Scripts/WoopraAnalytics/WoopraAnalytics.js"));
        }

        private string HeaderEndMatch(Match m)
        {
            return string.Concat(FormatAnalyticsScript(), m.Groups[1].Value);
        }

        #region Stream overrides

        public override void Flush()
        {
            responseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return responseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            responseStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return responseStream.Read(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position { get; set; }

        #endregion
    }
}