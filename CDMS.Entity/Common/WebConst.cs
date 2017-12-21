using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDMS.Entity
{
    public class WebConst
    {
        public static readonly string UserLoginSessionKey = "CDMSSKEY_{0}";

        public static readonly string UserLoginCookieKey = "CDMS_UID";

        public static readonly string UserLoginUICookieKey = "CDMS_UI";

        public static readonly int UserLoginExpiredMinutes = 600;

        public static string DateTimeFmt1 = "yy/MM/dd";

        public static string MENU_BUTTON_CODE_CONDITION = "btn-query";
        public static string MENU_BUTTON_CODE_REPORT = "btn-download";

        public static string GetActionMsg(ActionType type, bool flag)
        {
            string v = "";
            switch (type)
            {
                case ActionType.SYS_ADD:
                    v = "添加";
                    break;
                case ActionType.SYS_UPDATE:
                    v = "修改";
                    break;
                case ActionType.SYS_DELETE:
                    v = "删除";
                    break;
            }
            v += flag ? "成功" : "失败";
            return v;
        }
    }
}
