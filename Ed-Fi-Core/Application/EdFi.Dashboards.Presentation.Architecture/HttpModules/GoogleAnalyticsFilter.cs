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
    public class GoogleAnalyticsFilter : Stream
    {
        private const string analyticsScriptFormat = "<script src='{4}' type='text/javascript'></script><script type='text/javascript' language='javascript'>GoogleAnalyticsTrackUsage('{0}', '{1}', '{2}', '{3}');</script>";
        private readonly Stream responseStream;
        private bool headStarted;

        public GoogleAnalyticsFilter(Stream inputStream)
        {
            responseStream = inputStream;
        }

        public string AnalyticsId { get; set; }
        public string UserTracking { get; set; }
        public string LocalEducationAgency { get; set; }
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
            return String.Format(analyticsScriptFormat, AnalyticsId, UserTracking, LocalEducationAgency, TrackingUrl, Bundles.Url("Core_Content/Scripts/GoogleAnalytics/GoogleAnalytics.js"));
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