using System;
using System.Collections.Generic;
using System.Linq;
using SystemSignalsLogic.Enums;

namespace SystemSignalsLogic
{
    internal static class NodeSignalsCodec
    {
        private const Int32 SignalIndex = 0;

        public static (INSignal Signal, IList<Byte[]> Data) DecodeSignal(IList<Byte[]> message)
        {
            INSignal signal = (INSignal)message[SignalIndex][0];

            return (signal, message.Skip(1).ToArray());
        }

        public static IList<Byte[]> EncodeSignal(OUTSignal signal, IList<Byte[]> data)
        {
            if (data is null || data.Count == 0) return new Byte[][] { new Byte[] { (Byte)signal } };

            data.Insert(0, new[] { (Byte)signal });
            return data;
        }
    }
}
