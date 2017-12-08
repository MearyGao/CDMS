using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMS.Entity;
using CDMS.Service;

namespace CDMS.Web
{
    [Login]
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
    }
}