using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using CDMS.Service;
using CDMS.Entity;

namespace CDMS.Web
{
    /// <summary>
    /// 授权
    /// </summary>
    public class AuthAttribute : ActionFilterAttribute
    {
        public IMenuService MenuService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var controllerIgnores = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(IgnoreAuthAttribute), true);
            if (controllerIgnores != null && controllerIgnores.Length > 0) return;

            var actionIgnores = filterContext.ActionDescriptor.GetCustomAttributes(typeof(IgnoreAuthAttribute), true);
            if (actionIgnores != null && actionIgnores.Length > 0) return;

            //string url = filterContext.HttpContext.Request.Path.ToLower();
            string url = GetRequestURL(filterContext.RouteData);

            var model = MenuService.GetAuthMenuList(url);

            if (model == null || !model.HaveMenuRight())
            {
                bool isAjax = filterContext.HttpContext.Request.IsAjaxRequest();
                if (isAjax)
                {
                    filterContext.Result = new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new AjaxResult(false, "对不起,您的访问未经授权")
                    };
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/common/error");
                }
            }
        }

        private string GetRequestURL(RouteData route)
        {
            string url = "";
            if (route.DataTokens.ContainsKey("area"))
            {
                url = string.Format("/{0}", route.DataTokens["area"].ToString());
            }
            string controller = route.Values["controller"].ToString();
            string aciton = route.Values["action"].ToString();
            url += string.Format("/{0}", controller);
            if (!string.Equals(aciton, "index", StringComparison.InvariantCultureIgnoreCase))
            {
                url += string.Format("/{0}", aciton);
            }
            return url;
        }
    }
}