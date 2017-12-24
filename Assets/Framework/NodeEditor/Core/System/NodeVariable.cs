using System;
using NodeSystem.Editor;

namespace NodeSystem
{
    public class NodeVariable : Node, INodeExecuteOutput
    {
        public NodeGraphVariable Variable { get; protected set; }
        public NodeGraphVariableAccessorType AccessorType { get; private set; }

        protected NodePin In { get; private set; }
        protected NodePin Out { get; private set; }

        NodePinValueWrapper _wrappedIn;
        NodePinValueWrapper _wrappedOut;

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
            _wrappedOut = NodePinValueWrapper.Instantiate(Out, Variable.WrappedValue);
        }

        void AddSetPin()
        {
            In = AddPin("Set", Variable.WrappedType, false);
            _wrappedIn = NodePinValueWrapper.Instantiate(In, Variable.WrappedValue);
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

        public override void Calculate()
        {
            // Calculate value for Set
            if (In != null)
            {
                NodeEditor.Assertions.IsNotNull(_wrappedIn, "Missing wrapper for Set pin");
                if (_wrappedIn != null)
                    _wrappedIn.Calculate();
            }

            // Calculcate value for Get
            if (Out != null)
            {
                NodeEditor.Assertions.IsNotNull(_wrappedOut, "Missing wrapped for Get pin");
                if (_wrappedOut != null)
                    _wrappedOut.Calculate();
            }
        }
    }
}
