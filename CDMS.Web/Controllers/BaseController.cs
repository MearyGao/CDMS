using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CDMS.Entity;
using CDMS.Service;
using CDMS.Utility;

namespace CDMS.Web
{
    [Login(Order = 1)]
    [Auth(Order = 2)]
    public class BaseController : Controller
    {
        private User user;
        public BaseController()
        {

        }

        public IUserService UserService { get; set; }

        public new User User
        {
            get
            {
                if (user == null)
                {
                    user = UserService.GetCurrent();
                }
                return user;
            }
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return this.Json(data, contentType, contentEncoding, JsonRequestBehavior.DenyGet);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            MyJsonResult json = new MyJsonResult(false);
            json.Data = data;
            json.ContentEncoding = contentEncoding;
            json.ContentType = contentType;
            json.JsonRequestBehavior = behavior;
            return json;
        }
    }
}