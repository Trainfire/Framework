using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeSystem
{
    public class NodePinType
    {
        public string Name { get; private set; }
        public Type WrappedType { get; private set; }
        public bool IsConstant { get; private set; }
        public List<Type> CompatibleTypes { get; private set; } 

        public NodePinType(string name, Type type, bool isConstant, params Type[] compatibleTypes)
        {
            Name = name;
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

    public class NodePinTypeNone { }
    public class NodePinTypeExecute { }
    public class NodePinTypeAny { }

    public static class NodePinTypeRegistry
    {
        private static Dictionary<Type, NodePinType> _registry;

        static NodePinTypeRegistry()
        {
            _registry = new Dictionary<Type, NodePinType>();

            RegisterConstant<float>("Float", typeof(int));
            RegisterConstant<int>("Int");
            RegisterConstant<bool>("Bool");
            RegisterConstant<string>("String");

            Register<NodePinTypeNone>("None");
            Register<NodePinTypeExecute>("Execute");
            Register<NodePinTypeAny>("Any");
        }

        static void Register<T>(string name)
        {
            _registry.Add(typeof(T), new NodePinType(name, typeof(T), false));
        }

        static void RegisterConstant<T>(string name, params Type[] compatibleTypes)
        {
            _registry.Add(typeof(T), new NodePinType(name, typeof(T), true, compatibleTypes));
        }

        public static NodePinType Get<T>()
        {
            return _registry[typeof(T)]; 
        }

        public static List<NodePinType> GetConstants()
        {
            return _registry.Values.Where(x => x.IsConstant).ToList();
        }
    }
}
