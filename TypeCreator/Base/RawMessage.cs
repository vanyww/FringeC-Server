using System;
using System.Collections.Generic;

namespace TypeCreator.Base
{
    public sealed class RawMessage : Message
    {
        private const Int32 RawMessagePropertiesNumber = 1;
        private static readonly String[] NamesOfProperties = { nameof(RawData) };

        public override String[] PropertiesNames => NamesOfProperties;

        public override Int32 PropertiesNumber => RawMessagePropertiesNumber;

        public override IList<Byte[]> GetPropertiesBytes() =>
            RawData;

        public override Object[] GetPropertiesObjects() =>
            throw new NotImplementedException();

        public override void SetProperties(IList<Byte[]> values) =>
            RawData = values;

        public override void SetProperties(Object[] values) =>
            throw new NotImplementedException();

        public IList<Byte[]> RawData { get; private set; }
    }
}
