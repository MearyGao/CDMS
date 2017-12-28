using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Data
{
    public interface IMenuRepository : IRepository<Menu>
    {
        /// <summary>
        /// 获得树列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<Menu> GetTreeList();

        /// <summary>
        /// 根据用户ID获得授权列表 （包括菜单 和 按钮）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<Menu> GetAuthList(int userId);

        /// <summary>
        /// 根据菜单ID查询获得授权列列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetAuthColumnList(int menuId, int uid, MenuColumnType type);

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Delete(int[] ids);
    }

    public class MenuRepository : RepositoryBase<Menu>, IMenuRepository
    {
        public IEnumerable<Menu> GetTreeList()
        {
            sql.SelectAll();
            sql.Where(m => m.ENABLED == true);
            sql.OrderBy(m => m.PARENTID, m => m.SORTID);
            return GetList();
        }

        public bool Delete(int[] ids)
        {
            sql.In(m => m.ID, ids);

            sql.Update(new Menu() { ENABLED = false }, m => m.ENABLED);

            int count = base.Execute(sql.GetSql(), sql.GetParameters());
            return count > 0;
        }

        public IEnumerable<Menu> GetAuthList(int userId)
        {
            var roleMenuSql = base.GetSqlLam<RoleMenu>();

            var roleUserSql = base.GetSqlLam<RoleUser>();
            roleUserSql.Where(m => m.USERID == userId).Select(m => m.ROLEID);

            roleMenuSql.In(m => m.ROLEID, roleUserSql);
            roleMenuSql.SelectDistinct(m => m.MENUID);

            sql.Where(m => m.ENABLED == true).In(m => m.ID, roleMenuSql);
            sql.OrderBy(m => m.PARENTID, m => m.SORTID);

            return GetList();
        }

        public IEnumerable<dynamic> GetAuthColumnList(int menuId, int uid, MenuColumnType type)
        {
            //            string sqlText = @";WITH 
            //TREE AS( 
            //SELECT ID,PARENTID,OBJECTID  FROM dbo.SYS_MENU 
            //WHERE ID =@mid AND ENABLED=1
            //UNION ALL 
            //SELECT a.ID,a.PARENTID,a.OBJECTID FROM dbo.SYS_MENU AS a ,TREE AS b
            //WHERE b.ID=a.PARENTID AND a.ENABLED=1) 
            //SELECT b.* FROM TREE AS a LEFT JOIN dbo.SYS_MENUCOLUMN AS b ON a.OBJECTID=b.ID LEFT JOIN dbo.SYS_ROLEMENU AS c ON c.MENUID=a.ID AND ROLEID IN(SELECT ROLEID FROM dbo.SYS_ROLEUSER WHERE USERID=@uid)
            //WHERE a.OBJECTID IS NOT NULL AND b.ENABLED=1 AND c.ID IS NOT NULL AND b.TYPE=@type
            //ORDER BY b.SORTID";
            string sqlText = @"
SELECT a.*,b.TYPE AS STYPE,b.DATA,b.PARAMETERDATA,b.OPTOINDATA,b.DEFAULTTEXT,b.DEFAULTVALUE FROM dbo.SYS_MENUCOLUMN AS a LEFT JOIN dbo.SYS_MENUDATASOURCE AS b ON a.ID=b.COLUMNID AND b.ENABLED=1 WHERE MENUID IN(
SELECT OBJECTID FROM (
SELECT OBJECTID,ID FROM dbo.SYS_MENU WHERE ID=@mid AND OBJECTID IS NOT NULL
UNION 
SELECT OBJECTID,ID FROM dbo.SYS_MENU WHERE PARENTID=@mid AND OBJECTID IS NOT NULL
) AS a LEFT JOIN dbo.SYS_ROLEMENU AS b ON a.ID=b.MENUID 
WHERE b.ROLEID IN(SELECT ROLEID FROM dbo.SYS_ROLEUSER AS a WHERE USERID=@uid)
) AND a.ENABLED=1 AND a.TYPE=@type ORDER BY A.SORTID,A.ID ";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@mid", menuId);
            dic.Add("@uid", uid);
            dic.Add("@type", (int)type);

            return base.GetDynamicList(sqlText, dic);
        }
    }
}
