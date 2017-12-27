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
    public class LogController : BaseController
    {
        readonly IDevelopLogService devService;
        readonly ILogService logService;
        public LogController(IDevelopLogService idl,ILogService ils)
        {
            devService = idl;
            logService = ils;
        }
        // GET: Sys/Log
        public ActionResult System()
        {
            return View();
        }

        public ActionResult Cdms()
        {
            return View();
        }

        public ActionResult Develop()
        {
            return View();
        }

        public ActionResult DevelopShow(string path)
        {
            ViewBag.Content = devService.GetFileContent(path);
            return View();
        }

        [HttpPost]
        public ActionResult GetDevelopList(LayuiPaginationIn p)
        {
            var result = devService.GetList(p);
            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteFiles(string[] fileNames)
        {
            var result = devService.DeleteFiles(fileNames);
            return Json(result);
        }
        [HttpPost]
        public ActionResult GetList(LayuiPaginationIn p)
        {
            var result = logService.GetList(p);
            return Json(result);
        }
    }
}