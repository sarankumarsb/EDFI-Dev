using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Web.Models.Cache;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
#if DEBUG

    public class CacheController : Controller
    {
        //
        // GET: /Cache/

        public ActionResult Get()
        {
            var model = GetCacheEntries();

            return View(model);
        }

        private CacheModel GetCacheEntries()
        {
            List<string> cachePrefixes = new List<string>();

            Cache cache = HttpContext.Cache;

            var enumerator = cache.GetEnumerator();

            while (enumerator.MoveNext())
                cachePrefixes.Add(enumerator.Key.ToString().Split('$')[0]);

            HttpApplicationStateBase app = this.ControllerContext.RequestContext.HttpContext.Application;
            var disabledPrefixes = app["DisabledCachePrefixes"] as List<string>;

            disabledPrefixes = disabledPrefixes ?? new List<string>();

            var model = new CacheModel
            {
                CachePrefixes = cachePrefixes
                    .Concat(disabledPrefixes)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList(),
                DisabledPrefixes = disabledPrefixes
            };

            return model;
        }

        [HttpPost]
        public ActionResult Get(string[] remove, string[] disable)
        {
            remove = remove ?? new string[0];
            disable = disable ?? new string[0];

            RemoveCacheEntries(remove.Concat(disable).ToArray());

            DisableCacheEntries(disable);

            return RedirectToAction("Get");
        }

        private void DisableCacheEntries(string[] disable)
        {
            HttpApplicationStateBase app = this.ControllerContext.RequestContext.HttpContext.Application;

            var disabledCachePrefixes = new List<string>(disable);
            app["DisabledCachePrefixes"] = disabledCachePrefixes;
        }

        private void RemoveCacheEntries(string[] cachePrefixes)
        {
            var cacheEntriesToRemove = new List<string>();

            Cache cache = HttpContext.Cache;

            var enumerator = cache.GetEnumerator();

            while (enumerator.MoveNext())
            {
                foreach (string cachePrefix in cachePrefixes)
                {
                    if (enumerator.Key.ToString().StartsWith(cachePrefix + "$"))
                        cacheEntriesToRemove.Add(enumerator.Key.ToString());
                }
            }

            foreach (string cacheKey in cacheEntriesToRemove)
                cache.Remove(cacheKey);
        }
    }
#endif
}
