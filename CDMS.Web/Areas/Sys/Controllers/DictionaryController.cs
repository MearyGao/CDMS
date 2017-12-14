using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMS.Service;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Web.Areas.Sys.Controllers
{
    public class DictionaryController : BaseController
    {
        // GET: Sys/Dictionary
        readonly IDictionaryService dicService;
        public DictionaryController(IDictionaryService ibs)//依赖注入
        {
            dicService = ibs;
        }

        public ActionResult Index()
        {
            var values = dicService.GetRootDicList();

            return View(values);
        }

        public ActionResult Form(int? id)
        {
            int key = id.HasValue ? id.Value : 0;
            if (key > 0)
            {
                ViewBag.Json = JsonHelper.ToJson(dicService.Get(key));
            }
            var values = dicService.GetRootDicList();
            return View(values);
        }

        [HttpPost]
        public ActionResult GetList(LayuiPaginationIn p)
        {
            var getlist = dicService.GetList(p);
            return Json(getlist);
        }

        [HttpPost]
        public ActionResult Delete(string[] ids)
        {
            var result = dicService.Delete(ids);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Save(Dictionary old, Dictionary model)
        {
            var result = dicService.Save(old, model);
            return Json(result);
        }
    }
}