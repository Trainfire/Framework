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
            Variable.PreTypeChanged += Variable_PreTypeChanged;
            Variable.PostTypeChanged += Variable_PostTypeChanged;
            AccessorType = accessorType;

            NodeEditor.Assertions.IsNotNull(Variable);

            SpawnPins();
        }

        void SpawnPins()
        {
            switch (AccessorType)
            {
                case NodeGraphVariableAccessorType.Get: AddPin("Get", Variable.WrappedType, true); break;
                case NodeGraphVariableAccessorType.GetSet:
                    AddPin("Get", Variable.WrappedType, true);
                    AddPin("Set", Variable.WrappedType, false);
                    break;
                case NodeGraphVariableAccessorType.Set: AddPin("Set", Variable.WrappedType, false); break;
            }
        }

        void Variable_PreTypeChanged(NodeGraphVariableTypeChangeEvent variableTypeChangeEvent)
        {
            RemoveAllPins();
        }

        void Variable_PostTypeChanged(NodeGraphVariableTypeChangeEvent variableTypeChangeEvent)
        {
            // The type changed so update pins to reflect new type.
            SpawnPins();
        }
    }
}
