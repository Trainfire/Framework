using System;
using NodeSystem.Editor;

namespace NodeSystem
{
    public abstract class NodeVariable : Node, INodeExecuteOutput
    {
        public NodeGraphVariable Variable { get; protected set; }
        public NodeGraphVariableAccessorType AccessorType { get; private set; }

        protected NodePin In { get; private set; }
        protected NodePin Out { get; private set; }

        public NodePin<NodePinTypeExecute> ExecuteOut { get; private set; }

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
            if (AccessorType == NodeGraphVariableAccessorType.GetSet || AccessorType == NodeGraphVariableAccessorType.Set)
            {
                AddExecuteInPin();
                ExecuteOut = AddExecuteOutPin();
            }

            if (AccessorType == NodeGraphVariableAccessorType.GetSet || AccessorType == NodeGraphVariableAccessorType.Get)
                AddGetPin();

            if (AccessorType == NodeGraphVariableAccessorType.GetSet || AccessorType == NodeGraphVariableAccessorType.Set)
                AddSetPin();
        }

        void AddGetPin()
        {
            Out = AddPin("Get", Variable.WrappedType, true);
        }

        void AddSetPin()
        {
            In = AddPin("Set", Variable.WrappedType, false);
        }

        void Variable_PreTypeChanged(NodeGraphVariableTypeChangeEvent variableTypeChangeEvent)
        {
            ExecuteOut = null;
            RemoveAllPins();
        }

        void Variable_PostTypeChanged(NodeGraphVariableTypeChangeEvent variableTypeChangeEvent)
        {
            // The type changed so update pins to reflect new type.
            SpawnPins();
        }
    }

    public class NodeVariable<T> : NodeVariable
    {
        public override void Calculate()
        {
            if (In != null)
                (Variable.WrappedValue as NodeValueWrapper<T>).Value = (In as NodePin<T>).Value;

            if (Out != null)
                (Out as NodePin<T>).Value = (Variable.WrappedValue as NodeValueWrapper<T>).Value;
        }
    }
}
