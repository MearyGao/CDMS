using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;
using Roc.Data;

namespace CDMS.Data
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// 获得用户信息
        /// </summary>
        /// <param name="wwid"></param>
        /// <returns></returns>
        User GetUserByWWID(string wwid);

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        bool Logout(string eid, string loginKey);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        bool Login(UserLogin login);

        /// <summary>
        /// 是否登录
        /// </summary>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        User IsLogin(string loginKey);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Delete(int[] ids);
    }

    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository()
        {

        }

        public User GetUserByWWID(string wwid)
        {
            sql.Where(m => m.ACCOUNT == wwid);
            return base.GetEntity();
        }

        public bool Login(UserLogin login)
        {
            var sqlLogin = this.GetSqlLam<UserLogin>();
            sqlLogin.Insert(login);
            int count = this.Execute(sqlLogin);
            return count > 0 ? true : false;
        }

        public bool Logout(string eid, string loginKey)
        {
            var sqlLogin = base.GetSqlLam<UserLogin>();

            var model = new UserLogin()
            {
                STATUS = false,
                EMPLOYEEID = eid,
                LOGOUTTIME = DateTime.Now
            };
            sqlLogin.Update<object>(model, m => new { m.STATUS, m.LOGOUTTIME });
            sqlLogin.Where(m => m.EMPLOYEEID == model.EMPLOYEEID).And(m => m.LOGINKEY == loginKey);

            int count = base.Execute<UserLogin>(sqlLogin);

            return count > 0 ? true : false;
        }

        public User IsLogin(string loginKey)
        {
            var sqlLogin = base.GetSqlLam<UserLogin>();
            var sqlUser = sqlLogin.Join<User>((ul, u) => ul.EMPLOYEEID == u.ACCOUNT, aliasName: "b");
            sqlUser.SelectAll();

            sqlLogin.Where(m => m.LOGINKEY == loginKey && m.STATUS).And(m => m.EXPIREDTIME > DateTime.Now);
            sqlLogin.OrderByDescending(m => m.ID);

            var model = this.GetEntity(sqlLogin.GetSql(), sqlLogin.GetParameters());
            return model;
        }

        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            sql.SelectAll();

            sql.And(m => m.STATUS == 1);
            string key = p.json;
            if (!key.IsEmpty())
            {
                sql.And().Begin();
                sql.Or(m => m.CNNAME.Contains(key));
                sql.Or(m => m.ACCOUNT.Contains(key));
                sql.Or(m => m.ENNAME.Contains(key));
                sql.End();
            }
            sql.OrderBy(m => m.ID);
            var list = base.GetPageList(p);
            return new LayuiPaginationOut(p, list);
        }

        public bool Delete(int[] ids)
        {
            sql.In(m => m.ID, ids);

            sql.Update(new User() { STATUS = 0 }, m => m.STATUS);

            int count = base.Execute(sql.GetSql(), sql.GetParameters());
            return count > 0;
        }
    }
}
