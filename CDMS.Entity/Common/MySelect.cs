using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDMS.Entity
{
    public class MySelect
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public KeyValue Default { get; set; }
        public IEnumerable<KeyValue> List { get; set; }

        /// <summary>
        /// 验证字符串
        /// </summary>
        public string LayVerify { get; set; }

        /// <summary>
        /// 过滤器
        /// </summary>
        public string LayFilter { get; set; }

        /// <summary>
        /// 选择的value 是否是 Text
        /// </summary>
        public bool ValueIsText { get; set; }

        public MySelect()
        {
            InitDefaultValue();
        }

        public MySelect(string name, IEnumerable<KeyValue> list, KeyValue dv = null, string value = "", string verify = "", string filter = "", bool isText = false)
        {
            this.Id = this.Name = name;
            this.List = list;
            this.Value = value;
            this.LayVerify = verify;
            this.LayFilter = filter;
            ValueIsText = isText;
            if (dv == null)
            {
                InitDefaultValue();
            }
        }
        private void InitDefaultValue()
        {
            this.Default = new KeyValue("请选择", "");
        }
    }
}
