using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Data
{
    public interface IMenuColumnRepository : IRepository<MenuColumn>
    {
        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 获得列列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetColumnList(int menuId);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetColumnList(int menuId, string key);

        /// <summary>
        /// 查询列信息
        /// </summary>
        /// <param name="columnId"></param>
        /// <returns></returns>
        dynamic GetColumn(int columnId);

        /// <summary>
        /// 添加菜单列
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="cids"></param>
        /// <returns></returns>
        bool AddColumns(int pid, string uid, int[] cids);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Delete(int[] ids);
    }

    public class MenuColumnRepository : RepositoryBase<MenuColumn>, IMenuColumnRepository
    {
        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            sql.SelectAll();

            var tableSql = sql.Join<MenuTable>((c, t) => c.TABLEID == t.ID && c.MENUID == t.MENUID, aliasName: "b");
            tableSql.Select(m => m.TABLENAME);

            var menuSql = sql.Join<Menu>((c, m) => c.MENUID == m.ID, aliasName: "c");
            menuSql.Select(m => new { MENUNAME = m.NAME });

            sql.Where(m => m.ENABLED == true);

            //var model = p.json.ToObject<MenuColumn>();
            //if (model != null)
            //{
            //    if (model.MENUID > 0) sql.And(m => m.MENUID == model.MENUID);
            //    if (model.TABLEID > 0) sql.And(m => m.TABLEID == m.TABLEID);
            //    if (!model.NAME.IsEmpty()) sql.And(m => m.NAME.Contains(model.NAME));
            //}
            Dictionary<string, object> dic;
            string condition = ToSql(p.json, out dic);
            sql.And(condition);
            sql.AddParameters(dic);

            sql.OrderBy(m => m.MENUID, m => m.SORTID);

            var list = base.GetDynamicPageList(p, sql);

            return new LayuiPaginationOut(p, list);
        }

        public bool Delete(int[] ids)
        {
            sql.In(m => m.ID, ids);

            sql.Update(new { ENABLED = false }, m => m.ENABLED);

            int count = Execute();

            return count > 0;
        }

        public IEnumerable<dynamic> GetColumnList(int menuId)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@MENUID", menuId);
            return base.GetDynamicList("[P_GETTABLECOLUMN]", dic, System.Data.CommandType.StoredProcedure);
        }

        public IEnumerable<dynamic> GetColumnList(int menuId, string key)
        {
            string sqlText = @";WITH 
TREE AS( 
SELECT ID,PARENTID,NAME AS MENUNAME FROM dbo.SYS_MENU 
WHERE ID =@mid
UNION ALL 
SELECT a.ID,a.PARENTID,a.NAME AS MENUNAME FROM dbo.SYS_MENU AS a ,TREE AS b
WHERE b.PARENTID=a.ID
) 
SELECT b.*,a.MENUNAME,c.DBNAME,c.SCHEMANAME,c.TABLENAME FROM TREE AS a RIGHT JOIN dbo.SYS_MENUCOLUMN AS b ON a.ID=b.MENUID
LEFT JOIN dbo.SYS_MENUTABLE AS c ON b.MENUID=c.MENUID AND b.TABLEID=c.ID
WHERE b.ENABLED=1 AND a.ID IS NOT NULL ";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@mid", menuId);
            if (!key.IsEmpty())
            {
                sqlText += " AND B.NAME LIKE @key ";
                dic.Add("@key", key);
            }
            return base.GetDynamicList(sqlText, dic);
        }

        public bool AddColumns(int pid, string uid, int[] cids)
        {
            return base.UseTran(() =>
            {
                //string sqlText = @"DELETE FROM dbo.SYS_MENU WHERE PARENTID=@pid";
                //Dictionary<string, object> dic = new Dictionary<string, object>();
                //dic.Add("pid", pid);
                //base.Execute(sqlText, dic);

                string sqlText = string.Format(@"INSERT INTO dbo.SYS_MENU
        ( NAME ,
          TYPE ,
          PARENTID ,
          DISPLAY ,
          REMARK ,
          SORTID ,
          CREATEBY ,
          CREATEDATE ,
          UPDATEBY ,
          UPDATEDATE ,
          ENABLED
        )
SELECT FIELDTEXT,4,{0},1,ID,SORTID,'{1}',GETDATE(),'{1}',GETDATE(),1 FROM dbo.SYS_MENUCOLUMN WHERE ID IN({2})", pid, uid, string.Join(",", cids));
                base.Execute(sqlText, null);
            });
        }

        public dynamic GetColumn(int columnId)
        {
            sql.SelectAll();
            sql.Where(m => m.ENABLED == true && m.ID == columnId);
            var tableSql = sql.Join<MenuTable>((c, t) => c.TABLEID == t.ID && c.MENUID == t.MENUID, aliasName: "b");
            tableSql.Select(m => m.TABLENAME);
            return GetDynamicList(sql.GetSql(), sql.GetParameters()).FirstOrDefault();
        }
    }
}
