using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeEditor
{
    class NodeEditorPinConnector
    {
        private NodePin _sourcePin;

        public void StartConnection(NodePin sourcePin)
        {
            if (sourcePin == null)
                return;

            if (sourcePin.PinType == NodePinType.Input)
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Attempted to start a connection from an Input pin!");
            }
            else
            {
                DebugEx.Log<NodeEditorPinConnector>("Start connection from pin '{0}'", sourcePin.Name);
                _sourcePin = sourcePin;
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

            if (targetPin.PinType == NodePinType.Output)
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Attempted to make a connection to an Input pin!");
            }
            else if (targetPin.Node == _sourcePin.Node)
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Attempt to connect a pin to itself!");
            }
            else
            {
                DebugEx.Log<NodeEditorPinConnector>("Connecting pin '{0}' in '{1}' to '{2}' in '{3}'",
                    _sourcePin.Name,
                    _sourcePin.Node.ID,
                    targetPin.Name,
                    targetPin.Node.ID);

                // TODO: Connect the pins here.

                _sourcePin = null;
            }
        }
    }
}
