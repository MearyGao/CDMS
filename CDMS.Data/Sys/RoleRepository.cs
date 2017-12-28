using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Data
{
    public interface IRoleRepository : IRepository<Role>
    {
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 获得角色授权菜单列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        IEnumerable<RoleMenu> GetRoleMenus(int roleId);

        /// <summary>
        /// 添加角色授权
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="ids">菜单ID 数组</param>
        /// <returns></returns>
        bool AddRoleMenus(int roleId, int[] ids);

        /// <summary>
        /// 根据角色ID获得授权用户
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        IEnumerable<RoleUser> GetRoleUsers(int roleId);

        /// <summary>
        /// 添加授权用户
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool AddRoleUsers(int roleId, int[] ids);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        bool Delete(int[] roleIds);
    }

    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            sql.SelectAll();

            sql.And(m => m.ENABLED == true);
            //string key = p.json;
            //if (!key.IsEmpty())
            //{
            //    sql.And().Begin();
            //    sql.Or(m => m.NAME.Contains(key));
            //    sql.Or(m => m.REMARK.Contains(key));
            //    sql.End();
            //}
            Dictionary<string, object> dic;
            string condition = ToSql(p.json, out dic);
            sql.And(condition);
            sql.AddParameters(dic);
            sql.OrderBy(m => m.ID);
            var list = base.GetPageList(p);
            return new LayuiPaginationOut(p, list);
        }

        public bool Delete(int[] roleIds)
        {
            return UseTran(() =>
            {
                sql.In(m => m.ID, roleIds);
                sql.Update(new { ENABLED = false }, m => m.ENABLED);
                base.Execute(sql);

                var roleUserSql = base.GetSqlLam<RoleUser>();
                roleUserSql.In(m => m.ROLEID, roleIds).Delete();
                base.Execute(roleUserSql);

                var roleMenuSql = base.GetSqlLam<RoleMenu>();
                roleMenuSql.In(m => m.ROLEID, roleIds).Delete();
                base.Execute(roleMenuSql);
            });
        }

        public IEnumerable<RoleMenu> GetRoleMenus(int roleId)
        {
            var roleMenuSql = base.GetSqlLam<RoleMenu>();
            roleMenuSql.SelectAll();

            roleMenuSql.Where(m => m.ROLEID == roleId);

            return base.GetList<RoleMenu>(roleMenuSql.GetSql(), roleMenuSql.GetParameters());
        }

        public bool AddRoleMenus(int roleId, int[] ids)
        {
            return base.UseTran(() =>
            {
                var menuSql = base.GetSqlLam<Menu>("b");
                menuSql.Select(m => new { MENUID = m.ID }).Select(string.Format("{0} AS ROLEID", roleId));
                menuSql.Where(m => m.ENABLED == true).In(m => m.ID, ids);

                var roleMenuSql = base.GetSqlLam<RoleMenu>();
                roleMenuSql.Delete(m => m.ROLEID == roleId);
                base.Execute(roleMenuSql);

                roleMenuSql.InsertWithQuery(m => new { m.MENUID, m.ROLEID }, menuSql);
                base.Execute(roleMenuSql);
            });

        }

        public IEnumerable<RoleUser> GetRoleUsers(int roleId)
        {
            var roleUserSql = base.GetSqlLam<RoleUser>();
            roleUserSql.SelectAll();

            roleUserSql.Where(m => m.ROLEID == roleId);

            return base.GetList<RoleUser>(roleUserSql.GetSql(), roleUserSql.GetParameters());
        }

        public bool AddRoleUsers(int roleId, int[] ids)
        {
            return base.UseTran(() =>
            {

                var roleUserSql = base.GetSqlLam<RoleUser>();
                roleUserSql.Delete(m => m.ROLEID == roleId);
                base.Execute(roleUserSql);

                if (ids != null && ids.Length > 0)
                {
                    var userSql = base.GetSqlLam<User>("b");
                    userSql.Select(m => new { USERID = m.ID }).Select(string.Format("{0} AS ROLEID", roleId));
                    userSql.Where(m => m.STATUS == 1).In(m => m.ID, ids);

                    roleUserSql.InsertWithQuery(m => new { m.USERID, m.ROLEID }, userSql);
                    base.Execute(roleUserSql);
                }
            });
        }
    }
}
