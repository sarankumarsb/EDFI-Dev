using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Infrastructure
{
    public interface IHttpContextItemsProvider
    {
        void Add(object key, object value);
        bool Contains(object key);
        void Clear();
        void Remove(object key);
        object this[object key] { get; set; }
        object GetValue(object key);
    }
}
