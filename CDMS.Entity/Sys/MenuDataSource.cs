using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDMS.Entity
{
    [Table("SYS_MENUDATASOURCE")]
    public class MenuDataSource
    {
        /// <summary>
        ///主键ID 自增
        /// </summary>
        [Column(true)]
        public int ID { get; set; }
        /// <summary>
        ///列ID
        /// </summary>
        public int COLUMNID { get; set; }
        /// <summary>
        ///1=远程 2=本地 数据源类型
        /// </summary>
        public int TYPE { get; set; }
        /// <summary>
        ///数据（可以是URL或数组）
        /// </summary>
        public string DATA { get; set; }
        /// <summary>
        ///参数配置
        /// </summary>
        public string PARAMETERDATA { get; set; }
        /// <summary>
        ///整体选项数据配置
        /// </summary>
        public string OPTOINDATA { get; set; }
        /// <summary>
        ///DEFAULTTEXT
        /// </summary>
        public string DEFAULTTEXT { get; set; }
        /// <summary>
        ///DEFAULTVALUE
        /// </summary>
        public string DEFAULTVALUE { get; set; }
        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime CREATEDATE { get; set; }
        /// <summary>
        ///创建人
        /// </summary>
        public string CREATEBY { get; set; }
        /// <summary>
        ///修改时间
        /// </summary>
        public DateTime UPDATEDATE { get; set; }
        /// <summary>
        ///修改人
        /// </summary>
        public string UPDATEBY { get; set; }
        /// <summary>
        ///是否可用 1=可用 0=删除
        /// </summary>
        public bool ENABLED { get; set; }
    }
}
