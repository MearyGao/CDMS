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
    public class ArticleController : Controller
    {
        readonly IArticleService article;

        public ArticleController(IArticleService ias)
        {
            article = ias;
        }
        // GET: Sys/Article
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form(long? id)
        {
            long articleId = id.HasValue ? id.Value : 0;
            if (articleId > 0)
            {
                var model = article.Get(articleId);
                ViewBag.Json = JsonHelper.ToJson(model);
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetList(LayuiPaginationIn p)
        {
            var result = article.GetList(p);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Save(Article model)
        {
            var result = article.Save(model);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(long[] ids)
        {
            var result = article.Delete(ids);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Update(int type, long id, bool status)
        {
            var result = article.Update(type, id, status);
            return Json(result);
        }
    }
}