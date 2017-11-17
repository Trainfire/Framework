using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeSystem
{
    public class NodePinType
    {
        public Type WrappedType { get; private set; }
        public bool IsConstant { get; private set; }
        public List<Type> CompatibleTypes { get; private set; } 

        public NodePinType(Type type, bool isConstant, params Type[] compatibleTypes)
        {
            WrappedType = type;
            IsConstant = isConstant;

            CompatibleTypes = new List<Type>();
            CompatibleTypes.Add(type);
            foreach (var compatibleType in compatibleTypes)
            {
                CompatibleTypes.Add(compatibleType);
            }
        }

        public bool AreTypesCompatible<T>()
        {
            return CompatibleTypes.Any(x => x == typeof(T));
        }
    }

    class NodePinTypeNone { }
    class NodePinTypeExecute { }

    static class NodePinTypeRegistry
    {
        private static Dictionary<Type, NodePinType> _registry;

        static NodePinTypeRegistry()
        {
            _registry = new Dictionary<Type, NodePinType>();

            RegisterConstant<float>(typeof(int));
            RegisterConstant<int>();
            RegisterConstant<bool>();
            RegisterConstant<string>();

            Register<NodePinTypeNone>();
            Register<NodePinTypeExecute>();
        }

        static void Register<T>()
        {
            _registry.Add(typeof(T), new NodePinType(typeof(T), false));
        }

        static void RegisterConstant<T>(params Type[] compatibleTypes)
        {
            _registry.Add(typeof(T), new NodePinType(typeof(T), true, compatibleTypes));
        }

        public static NodePinType Get<T>()
        {
            return _registry[typeof(T)]; 
        }
    }
}
