using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    public class HttpContextItemsProvider : IHttpContextItemsProvider 
    {
        public void Add(object key, object value)
        {
            HttpContext.Current.Items.Add(key, value);
        }

        public bool Contains(object key)
        {
            return HttpContext.Current.Items.Contains(key);
        }

        public void Clear()
        {
            HttpContext.Current.Items.Clear();
        }

        public void Remove(object key)
        {
            HttpContext.Current.Items.Remove(key);
        }

        public object this[object key]
        {
            get { return GetValue(key); }
            set { HttpContext.Current.Items[key] = value; }
        }

        public object GetValue(object key)
        {
            if (!HttpContext.Current.Items.Contains(key))
                return null;
            return HttpContext.Current.Items[key];
        }
    }
}
