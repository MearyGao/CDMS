using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Data;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Service
{
    public interface IMenuColumnService : IDependency
    {
        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 获得列对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MenuColumn Get(int id);

        /// <summary>
        /// 获得列列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        AjaxResult GetColumnList(int menuId);

        /// <summary>
        /// 获得列表(菜单下拉框)
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        LayuiPaginationOut GetColumnList(int menuId, string key);

        /// <summary>
        /// 保存列信息
        /// </summary>
        /// <param name="old"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        AjaxResult Save(MenuColumn old, MenuColumn model);

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="uid"></param>
        /// <param name="cids"></param>
        /// <returns></returns>
        AjaxResult AddColumns(int pid, int[] cids);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        AjaxResult Delete(int[] ids);

        /// <summary>
        /// 查询列信息
        /// </summary>
        /// <param name="columnId"></param>
        /// <returns></returns>
        dynamic GetColumn(int columnId);

        /// <summary>
        /// 获得表列表
        /// </summary>
        /// <returns></returns>
        AjaxResult GetTableList();
    }

    internal class MenuColumnService : IMenuColumnService
    {
        readonly IMenuColumnRepository columnRep;
        readonly ILogService log;
        public MenuColumnService(IMenuColumnRepository imcr, ILogService ils)
        {
            columnRep = imcr;
            log = ils;
            log.Type = TableType.SYS_MENUCOLUMN;
            log.Title = "菜单列";
        }

        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            return columnRep.GetList(p);
        }

        public LayuiPaginationOut GetColumnList(int menuId, string key)
        {
            var list = columnRep.GetColumnList(menuId, key);

            return new LayuiPaginationOut(list);
        }

        public MenuColumn Get(int id)
        {
            return columnRep.GetEntity(m => m.ID == id && m.ENABLED == true);
        }

        public dynamic GetColumn(int columnId)
        {
            return columnRep.GetColumn(columnId);
        }

        public AjaxResult GetColumnList(int menuId)
        {
            var list = columnRep.GetColumnList(menuId);
            if (list != null)
            {
                var tempList = from item in list
                               select new
                               {
                                   text = string.Format("[{0}].[{1}]", item.TABLENAME, item.COLUMNNAME),
                                   value = string.Format("{0}|{1}|{2}|{3}", item.TABLEID, item.COLUMNTYPE, item.COLUMNTEXT, item.COLUMNNAME)
                               };
                return new AjaxResult(true, data: tempList);
            }
            return new AjaxResult(false, "");
        }

        public AjaxResult GetTableList()
        {
            var list = columnRep.GetTableList();
            if (list != null)
            {
                var tempList = from item in list
                               select new
                               {
                                   text = string.Format("[{0}].[{1}]", item.DBNAME, item.TABLENAME),
                                   value = item.ID
                               };
                return new AjaxResult(true, data: tempList);
            }
            return new AjaxResult(false, "");
        }

        public AjaxResult Save(MenuColumn old, MenuColumn model)
        {
            bool addFlag = model.ID < 1;
            var user = log.User;
            model.CREATEBY = user.ACCOUNT;
            model.CREATEDATE = DateTime.Now;
            model.UPDATEBY = model.CREATEBY;
            model.UPDATEDATE = model.CREATEDATE;
            model.ENABLED = true;
            if (addFlag)
            {
                model.MENUID = Guid.NewGuid().ToString();
                //bool existFlag = columnRep.Exist(m => m.TYPE == model.TYPE && m.MENUID == model.MENUID && m.NAME == model.NAME && m.ENABLED == true);
                //if (existFlag) return new AjaxResult(false, "已经存在该菜单列");
                int columnId = columnRep.Add<int>(model);
                bool flag = columnId > 0;
                if (flag) RemoveCache();
                ActionType type = ActionType.SYS_ADD;
                string msg = WebConst.GetActionMsg(type, flag);
                log.AppendAdd("表ID", model.TABLEID.ToString()).AppendAdd("列名", model.NAME).AppendAdd("类型", model.TYPE.ToString()).AppendAdd("条件类型", model.CONDITIONTYPE.ToString()).AppendAdd("控件类型", model.INPUTTYPE.ToString()).AppendAdd("控件文本", model.FIELDTEXT).AddSystem(type, columnId);
                return new AjaxResult(flag, msg);
            }
            else
            {
                //bool existFlag = columnRep.Exist(m => m.TYPE == model.TYPE && m.MENUID == model.MENUID && m.NAME == model.NAME && m.ENABLED == true && m.ID != model.ID);
                //if (existFlag) return new AjaxResult(false, "已经存在该菜单列");
                bool flag = columnRep.Update(model, m => new
                {
                    m.NAME,
                    m.CONDITIONTYPE,
                    m.INPUTTYPE,
                    m.TABLEID,
                    m.UPDATEBY,
                    m.TYPE,
                    m.FIELDTEXT,
                    m.FIELDTYPE,
                    m.UPDATEDATE,
                    m.SORTID,
                    m.FIELDVALUE
                }, m => m.ID == model.ID);
                if (flag) RemoveCache();
                ActionType type = ActionType.SYS_UPDATE;
                string msg = WebConst.GetActionMsg(type, flag);
                log.AppendUpdate("表ID", old.TABLEID.ToString(), model.TABLEID.ToString()).AppendUpdate("列名", old.NAME, model.NAME).AppendUpdate("类型", old.TYPE.ToString(), model.TYPE.ToString()).AppendUpdate("条件类型", old.CONDITIONTYPE.ToString(), model.CONDITIONTYPE.ToString()).AppendUpdate("控件类型", old.INPUTTYPE.ToString(), model.INPUTTYPE.ToString()).AppendUpdate("控件文本", old.FIELDTEXT, model.FIELDTEXT).AddSystem(type, model.ID);
                return new AjaxResult(flag, msg);
            }
        }

        public AjaxResult AddColumns(int pid, int[] cids)
        {
            var user = log.User;
            bool flag = columnRep.AddColumns(pid, user.ACCOUNT, cids);
            string msg = flag ? "分配成功" : "分配失败";

            return new AjaxResult(flag, msg);
        }

        public AjaxResult Delete(int[] ids)
        {
            ActionType type = ActionType.SYS_DELETE;
            bool flag = columnRep.Delete(ids);
            if (flag) RemoveCache();
            string msg = WebConst.GetActionMsg(type, flag);
            log.AppendDelete(msg, "菜单列ID", ids).AddSystem(type, ids);
            return new AjaxResult(flag, msg);
        }

        private void RemoveCache()
        {
            string key = ServiceConst.MenuColumnListCache;

            CacheHelper.Remove(key);
        }
    }
}
