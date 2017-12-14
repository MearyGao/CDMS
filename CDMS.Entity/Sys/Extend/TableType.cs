using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDMS.Entity
{
    /// <summary>
    /// 100以内为系统表 100以上 为业务表
    /// </summary>
    public enum TableType
    {
        NONE = 0,
        SYS_BUTTON = 1,
        SYS_LOG = 2,
        SYS_MENU = 3,
        SYS_ORGANIZATION = 4,
        SYS_ORGANIZATIONUSER = 5,
        SYS_ROLE = 6,
        SYS_ROLEMENU = 7,
        SYS_ROLEUSER = 8,
        SYS_USER = 9,
        SYS_USERLOGIN = 10,
        SYS_CATEGORY = 11,
        SYS_ARTICLE = 12,
        SYS_IMAGE = 13,
        SYS_IMAGEDETAIL = 14,
        SYS_MENUTABLE = 15,
        SYS_MENUCOLUMN = 16,
        SYS_DICTIONARY = 17
    }
}
