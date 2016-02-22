// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Web.WebPages;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Extensions
{
    public static class RepeaterHelper
    {
        public static HelperResult Repeater<T>(this IEnumerable<T> items, 
                                               Func<T, HelperResult> itemTemplate,
                                               Func<T, HelperResult> alternateTemplate,
                                               Func<object, HelperResult> footerTemplate)
        {
            return new HelperResult(writer =>
            {
                int index = 0;

                foreach (var item in items)
                {
                    index++;
                    HelperResult result;
                    if (index % 2 == 1)
                        result = itemTemplate(item);
                    else
                        result = alternateTemplate(item);
                    result.WriteTo(writer);
                }

                var footer = footerTemplate(null);
                footer.WriteTo(writer);
            });

        }


        public class IndexedItem<TModel>
        {
            public IndexedItem(int index, TModel item)
            {
                Index = index;
                Item = item;
            }

            public int Index { get; private set; }
            public TModel Item { get; private set; }
        }
    }
}