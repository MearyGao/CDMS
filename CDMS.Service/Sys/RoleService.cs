using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Data;
using CDMS.Utility;

namespace CDMS.Service
{
    public interface IRoleService : IDependency
    {
        /// <summary>
        /// 查询角色分页列表
        /// </summary>
        /// <param name="p">分页对象</param>
        /// <returns></returns>
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 保存角色信息
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        AjaxResult Save(Role old, Role role);

        /// <summary>
        ///批量删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        AjaxResult Delete(int[] roleIds);

        /// <summary>
        /// 获得角色信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Role Get(int roleId);

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
        AjaxResult AddRoleMenus(int roleId, int[] ids);

        /// <summary>
        /// 根据角色ID获得授权用户
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        IEnumerable<RoleUser> GetRoleUsers(int roleId);

        /// <summary>
        /// 添加授权用户
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="ids">用户ID 数组</param>
        /// <returns></returns>
        AjaxResult AddRoleUsers(int roleId, int[] ids, int[] deleteIds);
    }

    internal class RoleService : IRoleService
    {
        readonly IRoleRepository roleRep;
        readonly IUserService us;
        readonly ILogService ls;
        public RoleService(IRoleRepository irr, IUserService ius, ILogService ils)
        {
            roleRep = irr;
            us = ius;
            ls = ils;
            ls.Type = TableType.SYS_ROLE;
            ls.Title = "角色";
        }

        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            return roleRep.GetList(p);
        }

        public AjaxResult Save(Role old, Role role)
        {
            bool addFlag = role.ID < 1;

            var user = us.GetCurrent();
            role.CREATEBY = user.ACCOUNT;
            role.CREATEDATE = DateTime.Now;
            role.UPDATEBY = user.ACCOUNT;
            role.UPDATEDATE = DateTime.Now;
            role.ENABLED = true;
            if (addFlag)
            {
                bool flag = roleRep.Add(role);
                ActionType type = ActionType.SYS_ADD;
                string msg = WebConst.GetActionMsg(type, flag);
                ls.Append(msg).AddSystem(type);
                return new AjaxResult(flag, msg);
            }
            else
            {
                ActionType type = ActionType.SYS_UPDATE;
                bool flag = roleRep.Update(role, m => new { m.NAME, m.REMARK, m.SORTID, m.UPDATEBY, m.UPDATEDATE }, m => m.ID == role.ID);
                string msg = WebConst.GetActionMsg(type, flag);
                ls.AppendUpdate("角色名称", old.NAME, role.NAME);
                ls.AppendUpdate("角色描述", old.REMARK, role.REMARK);
                ls.AddSystem(type, role.ID);
                return new AjaxResult(flag, msg);
            }
        }

        public AjaxResult Delete(int[] roleIds)
        {
            bool flag = roleRep.Delete(roleIds);
            ActionType type = ActionType.SYS_DELETE;
            string msg = WebConst.GetActionMsg(type, flag);
            ls.AppendDelete(msg, "角色编码", roleIds).AddSystem(type, roleIds);
            return new AjaxResult(flag, msg);
        }

        public Role Get(int roleId)
        {
            return roleRep.GetEntity(m => m.ID == roleId);
        }

        public IEnumerable<RoleMenu> GetRoleMenus(int roleId)
        {
            return roleRep.GetRoleMenus(roleId);
        }

        public AjaxResult AddRoleMenus(int roleId, int[] ids)
        {
            if (roleId < 1) return new AjaxResult(false, "参数错误[ROLEID]");
            bool flag = roleRep.AddRoleMenus(roleId, ids);
            if (flag)
            {
                RemoveCache();
            }
            return new AjaxResult(flag, flag ? "授权成功" : "授权失败");
        }

        public IEnumerable<RoleUser> GetRoleUsers(int roleId)
        {
            return roleRep.GetRoleUsers(roleId);
        }

        public AjaxResult AddRoleUsers(int roleId, int[] ids, int[] deleteIds)
        {
            if (roleId < 1) return new AjaxResult(false, "参数错误[ROLEID]");
            bool flag = roleRep.AddRoleUsers(roleId, ids, deleteIds);
            if (flag)
            {
                string key = ServiceConst.UserAuthListCache.Split('_')[0];

                CacheHelper.RemoveByPrefix(key);
            }
            return new AjaxResult(flag, flag ? "授权用户成功" : "授权用户失败");
        }

        private void RemoveCache()
        {
            string key = ServiceConst.UserAuthListCache.Split('_')[0];
            CacheHelper.RemoveByPrefix(key);
        }
    }
}
