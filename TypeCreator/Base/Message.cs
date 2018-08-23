using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TypeCreator.Base
{
    public abstract class Message
    {
        //protected Message()
        //{
        //    Values = new Dictionary<String, List<Object>>();
        //    Index++;
        //}

        //~Message()
        //{
        //    foreach (var value in Values.Values)
        //        value.RemoveAt(Index);
        //}

        public abstract void SetProperties(IList<Byte[]> valuesList);
        public abstract void SetProperties(Object[] valuesArray);
        public abstract IList<Byte[]> GetPropertiesBytes();
        public abstract Object[] GetPropertiesObjects();

        public abstract String[] PropertiesNames { get; }

        public override String ToString()
        {
            if (PropertiesNumber == 0) return String.Empty;

            var names = PropertiesNames;
            var obj = GetPropertiesObjects();
            StringBuilder res = new StringBuilder();
            res.AppendFormat("{0}: {1}", names[0], obj[0].ToString());

            for (Int32 i = 1; i < PropertiesNumber; i++)
                res.AppendFormat("{2}{0}: {1}", names[i], obj[i].ToString(), Environment.NewLine);

            return res.ToString();
        }

        //public Object this[String key] => Values[key][Index];
        
        public abstract Int32 PropertiesNumber { get; }

        //protected abstract Dictionary<String, List<Object>> Values { get; set; }
        //protected abstract Int32 Index { get; set; }
    }
}
