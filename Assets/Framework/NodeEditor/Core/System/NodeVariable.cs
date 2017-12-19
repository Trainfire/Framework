using NodeSystem.Editor;

namespace NodeSystem
{
    public class NodeVariable : Node
    {
        public NodeGraphVariable Variable { get; private set; }
        public NodeGraphVariableAccessorType AccessorType { get; private set; }

        public void Set(NodeGraphVariable variable, NodeGraphVariableAccessorType accessorType)
        {
            Variable = variable;
            AccessorType = accessorType;

            NodeEditor.Assertions.IsNotNull(Variable);

            switch (AccessorType)
            {
                case NodeGraphVariableAccessorType.Get: AddPin("Get", Variable.ActualType, true); break;
                case NodeGraphVariableAccessorType.GetSet:
                    AddPin("Get", Variable.ActualType, true);
                    AddPin("Set", Variable.ActualType, false);
                    break;
                case NodeGraphVariableAccessorType.Set: AddPin("Set", Variable.ActualType, false); break;
            }
        }
    }
}
