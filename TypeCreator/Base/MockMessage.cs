using System;
using System.Collections.Generic;

namespace TypeCreator.Base
{
    public sealed class MockMessage : Message
    {
        static MockMessage()
        {
            m_emptyObjectArray = Array.Empty<Object>();
            m_emptyByteArray = Array.Empty<Byte[]>();
            m_emptyStringArray = Array.Empty<String>();
        }

        public override String[] PropertiesNames => m_emptyStringArray;
        public override Int32 PropertiesNumber => 0;

        public override IList<Byte[]> GetPropertiesBytes() => m_emptyByteArray;
        public override Object[] GetPropertiesObjects() => m_emptyObjectArray;

        public override void SetProperties(IList<Byte[]> values) { }
        public override void SetProperties(Object[] values) { }

        private static Object[] m_emptyObjectArray;
        private static IList<Byte[]> m_emptyByteArray;
        private static String[] m_emptyStringArray;
    }
}
