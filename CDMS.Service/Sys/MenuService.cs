using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Data;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Service
{
    public interface IMenuService : IDependency
    {
        /// <summary>
        /// 获得树列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<Menu> GetTreeList();
        /// <summary>
        /// 获得菜单列表 for select
        /// </summary>
        /// <returns></returns>
        IEnumerable<dynamic> GetTreeSelectList();
        /// <summary>
        /// 保存菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        AjaxResult Save(Menu menu);

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AjaxResult Delete(int[] ids);

        /// <summary>
        /// 获得菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Menu Get(int id);

        /// <summary>
        /// 根据用户ID获得授权列表 （包括菜单 和 按钮）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<Menu> GetAuthList();

        /// <summary>
        /// 根据用户ID获得授权菜单列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<MenuTree> GetAuthMenuList();

        /// <summary>
        /// 根据URL获得用户授权菜单列表
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        AuthMenu GetAuthMenuList(string url, MenuType type = MenuType.Menu, MenuColumnType columnType = MenuColumnType.CONDITION);
    }

    public class MenuService : IMenuService
    {
        readonly IMenuRepository menuRep;
        readonly ILogService log;
        public MenuService(IMenuRepository imr, ILogService ils)
        {
            menuRep = imr;
            log = ils;
        }

        public IEnumerable<Menu> GetTreeList()
        {
            var list = menuRep.GetTreeList();
            return GetMenuTree(0, list);
        }

        public IEnumerable<dynamic> GetTreeSelectList()
        {
            var list = GetTreeList();

            var treeList = from item in list
                           where item.TYPE < 3
                           select new
                           {
                               id = item.ID,
                               pid = item.PARENTID,
                               text = item.NAME,
                               value = item.ID
                           };
            return treeList;
        }

        public AjaxResult Save(Menu menu)
        {
            bool addFlag = menu.ID < 1;
            var user = log.User;

            menu.CREATEBY = user.ACCOUNT;
            menu.CREATEDATE = DateTime.Now;
            menu.UPDATEBY = user.ACCOUNT;
            menu.UPDATEDATE = DateTime.Now;
            menu.ENABLED = true;

            if (addFlag)
            {
                bool flag = menuRep.Add(menu);
                if (flag) RemoveAuthListCache();
                return new AjaxResult(flag, flag ? "菜单添加成功" : "菜单添加失败");
            }
            else
            {
                bool flag = menuRep.Update(menu, m => new
                {
                    m.NAME,
                    m.CLASSNAME,
                    m.CODE,
                    m.IMG,
                    m.TARGET,
                    m.TITLE,
                    m.TYPE,
                    m.URL,
                    m.REMARK,
                    m.SORTID,
                    m.UPDATEBY,
                    m.UPDATEDATE,
                    m.PARENTID,
                    m.DISPLAY
                }, m => m.ID == menu.ID);
                if (flag) RemoveAuthListCache();
                return new AjaxResult(flag, flag ? "菜单修改成功" : "菜单修改失败");
            }
        }

        public AjaxResult Delete(int[] roleIds)
        {
            bool flag = menuRep.Delete(roleIds);
            if (flag) RemoveAuthListCache();
            return new AjaxResult(flag, flag ? "菜单删除成功" : "菜单删除失败");
        }

        public Menu Get(int id)
        {
            return menuRep.GetEntity(m => m.ID == id);
        }

        #region 授权

        public IEnumerable<Menu> GetAuthList()
        {
            var user = log.User;
            int userId = user.ID;

            string key = string.Format(ServiceConst.UserAuthListCache, userId);
            var list = CacheHelper.Get<IEnumerable<Menu>>(key);
            if (list != null && list.Count() > 0) return list;
            list = menuRep.GetAuthList(userId);
            CacheHelper.Add(key, list);
            return list;
        }

        public IEnumerable<MenuTree> GetAuthMenuList()
        {
            var list = this.GetAuthList();
            if (list != null && list.Count() > 0)
            {
                int type = (int)MenuType.Button;
                list = list.Where(m => m.TYPE < type);
                if (list != null)
                {
                    var menu = list.FirstOrDefault(m => m.PARENTID == 0);
                    if (menu != null)
                    {
                        var rs = GetMenuTree2(menu.ID, list);
                        var first = rs.FirstOrDefault();
                        if (first != null) first.spread = true;
                        return rs;
                    }
                }
            }
            return null;
        }

        public AuthMenu GetAuthMenuList(string url, MenuType type = MenuType.Menu, MenuColumnType columnType = MenuColumnType.CONDITION)
        {
            if (string.IsNullOrEmpty(url)) return new AuthMenu();
            url = url.ToLower();
            var list = this.GetAuthList();
            if (list != null && list.Count() > 0)
            {
                var tempList = list.Where(m =>
                {
                    if (string.IsNullOrEmpty(m.URL)) return false;
                    string[] urls = m.URL.Split('|');
                    bool flag = false;
                    foreach (var item in urls)
                    {
                        flag = string.Equals(item, url, StringComparison.InvariantCultureIgnoreCase);
                        if (flag) break;
                    }
                    return flag;
                });
                if (type == MenuType.Menu)
                {
                    return new AuthMenu(tempList);
                }
                else if (type == MenuType.Button)
                {
                    AuthMenu am = new AuthMenu();
                    int menuId = 0;
                    if (tempList != null && tempList.Count() > 0)
                    {
                        int menuType = (int)type;
                        var menu = tempList.FirstOrDefault();
                        am.Menus = list.Where(m => m.PARENTID == menu.ID && m.TYPE == menuType);
                        //查找 按钮 为 查询 或 下载的 菜单
                        if (am.Menus != null)
                        {
                            string btn_code = columnType == MenuColumnType.CONDITION ? WebConst.MENU_BUTTON_CODE_CONDITION : WebConst.MENU_BUTTON_CODE_REPORT;
                            var obj = am.Menus.FirstOrDefault(m => string.Equals(m.CODE, btn_code, StringComparison.InvariantCultureIgnoreCase));
                            if (obj != null) menuId = obj.ID;
                        }
                    }
                    int uid = log.User.ID;
                    if (menuId > 0) am.Columns = menuRep.GetAuthColumnList(menuId, uid, columnType);
                    return am;
                }
            }
            return new AuthMenu();
        }

        #endregion

        private void RemoveAuthListCache()
        {
            var user = log.User;
            int userId = user.ID;

            string key = string.Format(ServiceConst.UserAuthListCache, userId);
            CacheHelper.Remove(key);
        }

        private IEnumerable<Menu> GetMenuTree(int parentId, IEnumerable<Menu> list)
        {
            var children = list.Where(m => m.PARENTID == parentId);
            if (children != null)
            {
                List<Menu> menus = new List<Menu>();
                children = children.OrderBy(m => m.SORTID).ToList();
                foreach (var m in children)
                {
                    menus.Add(m);
                    var ms = GetMenuTree(m.ID, list);
                    if (ms != null && ms.Count() > 0) menus.AddRange(ms);
                }
                return menus;
            }
            else return null;
        }

        private IEnumerable<MenuTree> GetMenuTree2(int parentId, IEnumerable<Menu> list)
        {
            var children = list.Where(m => m.PARENTID == parentId);
            if (children != null)
            {
                List<MenuTree> menus = new List<MenuTree>();
                children = children.OrderBy(m => m.SORTID).ToList();
                foreach (var m in children)
                {
                    if (!m.DISPLAY) continue;
                    var tree = new MenuTree(m);
                    var ms = GetMenuTree2(m.ID, list);
                    if (ms != null && ms.Count() > 0) tree.children = ms;
                    menus.Add(tree);
                }
                return menus;
            }
            else return null;
        }
    }
}
