// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4netContext = log4net.ThreadContext;

namespace EdFi.Dashboards.Infrastructure.log4net
{
	public class SmtpCustomAppender : SmtpAppender
	{
		protected override void SendBuffer(LoggingEvent[] events)
		{
			PrepareSubject(events); // customize subject before call base. 

			base.SendBuffer(events);
		}

		protected virtual void PrepareSubject(ICollection<LoggingEvent> events)
		{
		    var subject = log4netContext.Properties["EmailSubject"];
            if (subject != null)
            {
                //Lets get the subject form the variable set in the log4net thread context.
                Subject = subject.ToString().Replace('\r', ' ').Replace('\n', ' ');
            }
		}

	}
}