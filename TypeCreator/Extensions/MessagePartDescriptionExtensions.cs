using System;
using System.Collections.Generic;
using System.Text;
using TypeCreator.Base;
using TypeCreator.Enums;

namespace TypeCreator.Extensions
{
    public static class MessagePartDescriptionExtensions
    {
        public static String TypeToString(this IEnumerable<MessagePartDescription> me, MessageType msgType)
        {
            if (me is null)
                return "None";

            var result = new StringBuilder();

            result.AppendFormat("[{0}] : {{", msgType);
            foreach (var type in me)
            {
                result.Append(Environment.NewLine);
                result.AppendFormat("   {0}  <{1}>", type.Name, type.Type.ToString());
            }
            result.Append(Environment.NewLine);
            result.Append('}');

            return result.ToString();
        }

        public static String TypeToString(this IEnumerable<MessagePartDescription> me)
        {
            if (me is null)
                return Environment.NewLine + "   None";

            var result = new StringBuilder();
            
            foreach (var type in me)
            {
                result.Append(Environment.NewLine);
                result.AppendFormat("   {0}  <{1}>", type.Name, type.Type.ToString());
            }

            return result.ToString();
        }

        public static String TypesToString(this IEnumerable<MessagePartDescription>[] me)
        {
            var result = new StringBuilder();

            result.AppendFormat("[{0}] : {{", (MessageType)0);
            foreach (var type in me[0])
            {
                result.Append(Environment.NewLine);
                result.AppendFormat("   {0}  <{1}>", type.Name, type.Type.ToString());
            }
            result.Append(Environment.NewLine);
            result.Append('}');

            for (Int32 i = 1; i < me.Length; i++)
            {
                result.Append(Environment.NewLine);
                result.AppendFormat("[{0}] : {{", (MessageType)i);
                foreach (var type in me[i])
                {
                    result.Append(Environment.NewLine);
                    result.AppendFormat(" {0} <{1}>", type.Name, type.Type.ToString());
                }
                result.Append(Environment.NewLine);
                result.Append('}');
            }

            return result.ToString();
        }
    }
}