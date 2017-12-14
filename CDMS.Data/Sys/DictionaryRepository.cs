using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;
using Roc.Data;
namespace CDMS.Data
{
    public interface IDictionaryRepository : IRepository<Dictionary>
    {
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        LayuiPaginationOut GetList(LayuiPaginationIn p);

        /// <summary>
        /// 删除字典信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Delete(string[] ids);
    }
    public class DictionaryRepository : RepositoryBase<Dictionary>, IDictionaryRepository
    {
        public bool Delete(string[] ids)
        {
            sql.In(m => m.ID, ids);

            sql.Update(new Dictionary() { ENABLED = false }, m => m.ENABLED);

            int count = base.Execute(sql.GetSql(), sql.GetParameters());
            return count > 0;
        }

        public LayuiPaginationOut GetList(LayuiPaginationIn p)
        {
            SqlTextEntity entity = new SqlTextEntity();
            entity.PageNumber = p.page;
            entity.PageSize = p.limit;
            entity.Selection = "a.*,b.[TEXT] AS PTEXT";
            entity.From = "[SYS_DICTIONARY] AS a LEFT JOIN [SYS_DICTIONARY] AS b ON a.[TYPE] = b.[VALUE]";
            entity.OrderBy = "ORDER BY a.[SORTID] ASC";
            entity.Conditions = " WHERE a.[ENABLED] = 1 AND b.[ENABLED] = 1 ";

            Dictionary<string, object> dic = new Dictionary<string, object>();
            Dictionary model = p.json.ToObject<Dictionary>();

            if (!model.CODE.IsEmpty())
            {
                entity.Conditions += " AND a.CODE LIKE '%'+@CODE+'%' ";
                dic.Add("@CODE", model.CODE);
            }

            if (!model.TYPE.IsEmpty())
            {
                entity.Conditions += " AND a.TYPE=@TYPE ";
                dic.Add("@TYPE", model.TYPE);
            }

            if (!model.VALUE.IsEmpty())
            {
                entity.Conditions += " AND (a.VALUE LIKE '%'+@VALUE+'%' OR a.TEXT LIKE '%'+@VALUE+'%')";
                dic.Add("@VALUE", model.VALUE);
            }

            IEnumerable<dynamic> list = null;
            var reader = base.GetReader(sql.GetSql(SqlTextType.QueryPage, entity), dic);
            if (reader != null)
            {
                list = reader.Read();
                p.total = reader.Read<int>().FirstOrDefault();
            }
            return new LayuiPaginationOut(p, list);
        }
    }
}
