using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDMS.Entity
{
    public class KeyValue
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public KeyValue()
        { }

        public KeyValue(string key, string value)
        {
            this.Text = key;
            this.Value = value;
        }

        public KeyValue(string key)
        {
            this.Text = key;
            this.Value = key;
        }
    }
}
