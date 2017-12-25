using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMS.Utility;
using CDMS.Service;
using CDMS.Entity;

namespace CDMS.Web.Areas.Sys.Controllers
{
    public class CacheController : BaseController
    {
        readonly ICaCheService cache;
        public CacheController(ICaCheService ics)
        {
            cache = ics;
        }
        // GET: Sys/Cache
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetList(string key)
        {
            var result = cache.GetCacheKeys(key);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Remove(string key)
        {
            var result = cache.Remove(key);
            return Json(result);
        }

        [HttpPost]
        public ActionResult RemoveList(string[] keys)
        {
            var result = cache.RemoveList(keys);
            return Json(result);
        }

        [HttpPost]
        public ActionResult RemoveAll()
        {
            var result = cache.RemoveAll();
            return Json(result);
        }
    }
}