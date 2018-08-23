using System;
using System.Reflection.Emit;

namespace TypeCreator
{
    public sealed class InstanceFactoryCreator
    {
        private InstanceFactoryCreator()
        { }

        public Func<T> CreateFactory<T>()
            where T : new()
        {
            DynamicMethod factory = new DynamicMethod("create_type", typeof(T), null);
            var ilGen = factory.GetILGenerator();
            var ctor = typeof(T).GetConstructor(Type.EmptyTypes);

            ilGen.Emit(OpCodes.Newobj, ctor);
            ilGen.Emit(OpCodes.Ret);

            return (Func<T>)factory.CreateDelegate(typeof(Func<T>));
        }

        public Func<T> CreateFactory<T>(Type type)
        {
            var factory = new DynamicMethod("create_type", type, null);
            var ilGen = factory.GetILGenerator();
            var ctor = type.GetConstructor(Type.EmptyTypes);

            ilGen.Emit(OpCodes.Newobj, ctor);
            ilGen.Emit(OpCodes.Ret);

            return (Func<T>)factory.CreateDelegate(typeof(Func<T>));
        }

        public static InstanceFactoryCreator Instance =>
            m_instance ?? (m_instance = new InstanceFactoryCreator());

        private static InstanceFactoryCreator m_instance;
    }
}
