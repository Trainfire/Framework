using System;
using Framework.NodeSystem;

namespace Framework.NodeEditor
{
    class NodeEditorPinConnector
    {
        public event Action<NodeConnectionData> ConnectionMade;

        private NodePin _sourcePin;

        public void StartConnection(NodePin sourcePin)
        {
            if (sourcePin == null)
                return;

            _sourcePin = sourcePin;

            if (sourcePin.Node.IsInputPin(_sourcePin))
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Attempted to start a connection from an Input pin!");
                _sourcePin = null;
            }
            else
            {
                DebugEx.Log<NodeEditorPinConnector>("Start connection from pin '{0}'", sourcePin.Name);
            }
        }

        public void CancelConnection()
        {
            if (_sourcePin == null)
                return;

            DebugEx.Log<NodeEditorPinConnector>("Cancelling connection.");
            _sourcePin = null;
        }

        public void AttemptMakeConnection(NodePin targetPin)
        {
            if (targetPin == null)
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Cannot make connection as target pin is null.");
                return;
            }

            if (_sourcePin == null)
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Cannot make connection as source pin is null.");
                return;
            }

            if (targetPin.Node == _sourcePin.Node)
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Attempt to connect a pin to itself!");
                return;
            }

            if (_sourcePin.WillPinConnectionCreateCircularDependency(targetPin))
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Pin connection would create a circular dependency!");
                return;
            }

            if (targetPin.ArePinsCompatible(_sourcePin))
            {
                DebugEx.Log<NodeEditorPinConnector>("Connecting pin '{0}' in '{1}' to '{2}' in '{3}'",
                    _sourcePin.Name,
                    _sourcePin.Node.ID,
                    targetPin.Name,
                    targetPin.Node.ID);

                // Execution pins flow left right.
                // Value pins flow right to left.
                if (_sourcePin.GetType() == typeof(NodeExecutePin))
                {
                    DebugEx.Log<NodeEditorPinConnector>("Connected execution pins.");
                    var nodeConnection = new NodeConnectionData(_sourcePin.Node.ID, _sourcePin.Index, targetPin.Node.ID, targetPin.Index);
                    ConnectionMade.InvokeSafe(nodeConnection);
                }
                else
                {
                    DebugEx.Log<NodeEditorPinConnector>("Connected value pins.");
                    var nodeConnection = new NodeConnectionData(targetPin.Node.ID, targetPin.Index, _sourcePin.Node.ID, _sourcePin.Index);
                    ConnectionMade.InvokeSafe(nodeConnection);
                }

                _sourcePin = null;
            }
        }
    }
}
