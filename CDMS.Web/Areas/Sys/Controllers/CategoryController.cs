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
    public class CategoryController : BaseController
    {
        readonly ICategoryService cate;
        public CategoryController(ICategoryService ics)
        {
            cate = ics;
        }
        // GET: Sys/Category
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form(int? id)
        {
            int categoryId = id.HasValue ? id.Value : 0;
            if (categoryId > 0)
            {
                var model = cate.Get(categoryId);
                ViewBag.Json = JsonHelper.ToJson(model);
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetTreeList()
        {
            var list = cate.GetTreeList();
            var result = new LayuiPaginationOut(list);
            return Json(result);
        }

        [IgnoreAuth]
        [HttpPost]
        public ActionResult GetTreeSelectList()
        {
            var list = cate.GetTreeSelectList();
            return Json(list);
        }

        [HttpPost]
        public ActionResult Save(Category model)
        {
            var result = cate.Save(model);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(int[] ids)
        {
            var result = cate.Delete(ids);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Update(int id, int type, bool status)
        {
            var result = cate.Update(id, type, status);
            return Json(result);
        }
    }
}