using CommandPortLogic;
using System;
using System.Diagnostics.Contracts;

namespace ParametersWorker
{
    public sealed class CommandedParameterViewModel
    {
        public Parameter Parameter { get; set; }
        public OuterCommand GetInfoCommand
        {
            get => m_getInfoCmd;
            set
            {
                if (Parameter is null) throw new InvalidOperationException("Command is already set");
                Contract.EndContractBlock();
                m_getInfoCmd = value;
                IsFull = GetValueCommand != null && SetValueCommand != null;
            }
        }
        public OuterCommand GetValueCommand
        {
            get => m_getInfoCmd;
            set
            {
                if (Parameter is null) throw new InvalidOperationException("Command is already set");
                Contract.EndContractBlock();
                m_getValueCmd = value;
                IsFull = GetInfoCommand != null && SetValueCommand != null;
            }
        }
        public OuterCommand SetValueCommand
        {
            get => m_getInfoCmd;
            set
            {
                if (Parameter is null) throw new InvalidOperationException("Command is already set");
                Contract.EndContractBlock();
                m_setValueCmd = value;
                IsFull = GetValueCommand != null && GetInfoCommand != null;
            }
        }

        public Boolean IsFull;
        public OuterCommand m_getInfoCmd,
                            m_getValueCmd,
                            m_setValueCmd;
    }
}
