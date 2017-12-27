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
    public class MenuColumnController : BaseController
    {
        readonly IMenuColumnService column;

        public MenuColumnController(IMenuColumnService imcs)
        {
            column = imcs;
        }
        // GET: Sys/MenuColumn
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form(int? id)
        {
            int columnId = id.HasValue ? id.Value : 0;
            if (columnId > 0)
            {
                var model = column.GetColumn(columnId);
                ViewBag.Json = JsonHelper.ToJson(model);
            }
            return View();
        }

        public ActionResult FormColumn()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetList(LayuiPaginationIn p)
        {
            var result = column.GetList(p);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Save(MenuColumn old, MenuColumn model)
        {
            var result = column.Save(old, model);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(int[] ids)
        {
            var result = column.Delete(ids);
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetColumnList(int tableId)
        {
            var result = column.GetColumnList(tableId);
            return Json(result);
        }

        [IgnoreAuth]
        [HttpPost]
        public ActionResult GetTableList()
        {
            var result = column.GetTableList();
            return Json(result);
        }

        [HttpPost]
        public ActionResult AddColumns(int pid, int[] cids)
        {
            var result = column.AddColumns(pid, cids);
            return Json(result);
        }
    }
}