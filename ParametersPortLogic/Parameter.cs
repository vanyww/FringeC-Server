using ROSPortLogic.Enums;
using System;
using System.Diagnostics.Contracts;
using TypeCreator.Enums;

namespace ParametersWorker
{
    public sealed class Parameter
    {
        public ParamFlags Flags { get; }
        public MessageValueType Type { get; }
        public String Name { get; }
        public String Description { get; }
        public Int32 Id { get; }
        public Object Value
        {
            get => m_value;
            set
            {
                if (Flags.HasFlag(ParamFlags.ReadOnly))
                    throw new InvalidOperationException("Parameter is readonly.");
                Contract.EndContractBlock();

                m_value = value;
            }
        }

        internal void UpdateValue(Object value) => 
            m_value = value;

        private Object m_value;
    }
}
