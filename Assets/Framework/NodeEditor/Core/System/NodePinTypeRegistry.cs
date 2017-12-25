using System;
using System.Collections.Generic;
using System.Linq;
using NodeSystem.Editor;

namespace NodeSystem
{
    public enum NodePinTypeCategory
    {
        Uncategorized,
        Special,
        Constant,
    }

    public class NodePinType
    {
        public string Name { get; private set; }
        public Type WrappedType { get; private set; }
        public NodePinTypeCategory Category { get; private set; }
        public List<Type> CompatibleTypes { get; private set; } 

        public NodePinType(string name, Type type, NodePinTypeCategory category, params Type[] compatibleTypes)
        {
            Name = name;
            WrappedType = type;
            Category = category;

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

        public static List<NodePinType> AllPinTypes { get { return _registry.Values.ToList(); } }
        public static List<string> AllPinTypeNames { get { return AllPinTypes.Select(x => x.Name).ToList(); } }

        public static List<NodePinType> AllConstantTypes { get { return _registry.Values.Where(x => x.Category == NodePinTypeCategory.Constant).ToList(); } }
        public static List<string> AllConstantTypeNames { get { return AllConstantTypes.Select(x => x.Name).ToList(); } }

        static NodePinTypeRegistry()
        {
            _registry = new Dictionary<Type, NodePinType>();

            Add<NodePinTypeNone>("None", NodePinTypeCategory.Constant);
            Add<NodePinTypeExecute>("Execute", NodePinTypeCategory.Special);
            Add<NodePinTypeAny>("Any", NodePinTypeCategory.Special);

            AddConstant<float>("Float", typeof(int));
            AddConstant<int>("Int");
            AddConstant<bool>("Bool");
            AddConstant<string>("String");
        }

        static void Add<T>(string name, NodePinTypeCategory category = NodePinTypeCategory.Uncategorized)
        {
            Register(new NodePinType(name, typeof(T), category));
        }

        static void AddConstant<T>(string name, params Type[] compatibleTypes)
        {
            Register(new NodePinType(name, typeof(T), NodePinTypeCategory.Constant, compatibleTypes));
        }

        static void Register(NodePinType pinType)
        {
            _registry.Add(pinType.WrappedType, pinType);
        }

        public static NodePinType Get<T>()
        {
            return Get(typeof(T));
        }

        public static NodePinType Get(Type type)
        {
            NodeEditor.Assertions.IsTrue(_registry.ContainsKey(type), "Registry does not contain type");
            return _registry[type];
        }

        public static List<NodePinType> Get(params NodePinTypeCategory[] categories)
        {
            var list = new List<NodePinType>();

            foreach (var category in categories)
            {
                list.AddRange(_registry.Values.Where(x => x.Category == category));
            }

            return list;
        }
    }
}
