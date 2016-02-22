// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
	public class HashtableSessionStateProvider : ISessionStateProvider
	{
        public HashtableSessionStateProvider()
        {
            var r = new Random();
            sessionId = r.Next().ToString();
        }
	    private readonly string sessionId;

		private readonly Hashtable values = new Hashtable();
		public Hashtable Values
		{
			get { return values; }
		}

		public object GetValue(string name)
		{
			return Values[name];
		}

		public void SetValue(string name, object value)
		{
			Values[name] = value;
		}

        public void RemoveValue(string name)
        {
            if (Values.ContainsKey(name))
                Values.Remove(name);
        }

		public object this[string name]
		{
			get { return Values[name]; }
			set { Values[name] = value; }
		}

        public void Clear()
        {
            Values.Clear();
        }

        public int Count
        {
            get { return Values.Count; }
        }

	    public string SessionId
	    {
	        get
	        {
                return sessionId;
	        }
	    }
	}
}
