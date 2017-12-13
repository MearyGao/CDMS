using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMS.Entity;
using CDMS.Utility;
using CDMS.Service;

namespace CDMS.Web.Areas.Sys.Controllers
{
    public class MenuTableController : BaseController
    {
        readonly IMenuTableService table;

        public MenuTableController(IMenuTableService imts)
        {
            table = imts;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form(int? id)
        {
            int tableId = id.HasValue ? id.Value : 0;
            if (tableId > 0)
            {
                var model = table.Get(tableId);

                ViewBag.Json = JsonHelper.ToJson(model);
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetList(LayuiPaginationIn p)
        {
            var result = table.GetList(p);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Save(MenuTable old, MenuTable model)
        {
            var result = table.Save(old, model);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(int[] ids)
        {
            var result = table.Delete(ids);
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetTableList(string dbKey)
        {
            var result = table.GetTableList(dbKey);
            return Json(result);
        }
    }
}