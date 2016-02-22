// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Web;
using System.Web.SessionState;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
	public class AspNetSessionStateProvider : ISessionStateProvider
	{
		public object GetValue(string name)
		{
		    HttpSessionState session = GetSessionState();
            if (session != null)
                return session[name];
            return null;
		}

		public void SetValue(string name, object value)
        {
            HttpSessionState session = GetSessionState();
            if (session != null)
			    session[name] = value;
		}

        public void RemoveValue(string name)
        {
            HttpSessionState session = GetSessionState();
            if (session != null)
                session.Remove(name);
        }

		public object this[string name]
		{
			get { return GetValue(name); }
            set { SetValue(name, value); }
		}
        
        public void Clear()
        {
            HttpSessionState session = GetSessionState();
            if (session != null)
            {
                session.Abandon();
                session.Clear();
            }
        }

        public int Count
        {
            get
            {
                HttpSessionState session = GetSessionState();
                if (session != null)
                    return session.Count;
                return 0;
            }
        }

	    public string SessionId
	    {
	        get
            {
                HttpSessionState session = GetSessionState();
                if (session != null)
                    return session.SessionID;
                return string.Empty;
	        }
	    }

        private static HttpSessionState GetSessionState()
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Session;
            return null;
        }
	}
}
