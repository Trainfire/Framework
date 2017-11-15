using System;
using Framework.NodeSystem;
using Framework.NodeEditor.Views;

namespace Framework.NodeEditor
{
    public class NodeEditorPinConnector
    {
        private NodeGraph _graph;
        private NodeEditorView _view;

        private NodeConnection _modifyingConnection;
        private NodePin _sourcePin;

        public NodeEditorPinConnector(NodeGraph graph, NodeEditorView view)
        {
            _graph = graph;

            _view = view;
            _view.GraphView.MouseLeftClickedPin += GraphView_MouseLeftClickedPin;
            _view.GraphView.MouseLeftReleasedOverPin += GraphView_MouseLeftReleasedOverPin;
            _view.GraphView.MouseReleased += GraphView_MouseReleased;
        }

        void GraphView_MouseLeftClickedPin(NodePin nodePin)
        {
            if (nodePin.IsInput())
            {
                DebugEx.Log<NodeEditorPinConnector>("Modifying a connection...");

                _modifyingConnection = _graph.Helper.GetConnection(nodePin);

                if (_modifyingConnection != null)
                {
                    _modifyingConnection.Hide();

                    // Executes flow left to right so get the correct starting pin.
                    var startPin = nodePin.Type == NodePinType.Execute ? _modifyingConnection.StartPin : _modifyingConnection.EndPin;
                    _view.ConnectorView.EnterDrawState(startPin);
                }
            }
            else
            {
                DebugEx.Log<NodeEditorPinConnector>("Creating a new connection...");
                _view.ConnectorView.EnterDrawState(nodePin);
            }

            _sourcePin = nodePin;
        }

        void GraphView_MouseReleased()
        {
            DebugEx.Log<NodeEditorPinConnector>("MouseReleased");

            if (_modifyingConnection != null)
            {
                DebugEx.Log<NodeEditorPinConnector>("Removing a modified connection.");
                _graph.Disconnect(_modifyingConnection);
            }
            else
            {
                DebugEx.Log<NodeEditorPinConnector>("Cancelling a new connection.");
            }

            _view.ConnectorView.EndDrawState();
            _modifyingConnection = null;
            _sourcePin = null;
        }

        void GraphView_MouseLeftReleasedOverPin(NodePin targetPin)
        {
            DebugEx.Log<NodeEditorPinConnector>("MouseLeftReleased");

            if (targetPin == null)
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Cannot make a new connection as target pin is null.");
                return;
            }

            if (_sourcePin == null)
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Cannot make a new connection as source pin is null.");
                return;
            }

            if (!IsModifyingConnection() && targetPin.Node == _sourcePin.Node)
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Attempted to connect a pin to itself!");
                return;
            }

            if (targetPin.ArePinsCompatible(_sourcePin))
            {
                DebugEx.Log<NodeEditorPinConnector>("Connecting pin '{0}' in '{1}' to '{2}' in '{3}'",
                    _sourcePin.Name,
                    _sourcePin.Node.Name,
                    targetPin.Name,
                    targetPin.Node.Name);

                NodeConnection connection = null;
                NodePin startPin = null;
                NodePin endPin = null;

                // Execution pins flow left right.
                // Value pins flow right to left.
                if (_sourcePin.GetType() == typeof(NodeExecutePin))
                {
                    DebugEx.Log<NodeEditorPinConnector>("Connected execution pins.");

                    if (IsModifyingConnection())
                    {
                        startPin = _modifyingConnection.StartPin;
                        endPin = _sourcePin;
                    }
                    else
                    {
                        startPin = _sourcePin;
                        endPin = targetPin;
                    }
                }
                else
                {
                    DebugEx.Log<NodeEditorPinConnector>("Connected value pins.");

                    if (IsModifyingConnection())
                    {
                        startPin = targetPin;
                        endPin = _modifyingConnection.EndPin;
                    }
                    else
                    {
                        startPin = targetPin;
                        endPin = _sourcePin;
                    }
                }

                connection = new NodeConnection(startPin, endPin);

                if (IsModifyingConnection())
                {
                    _graph.Replace(_modifyingConnection, connection);
                }
                else
                {
                    _graph.Connect(connection);
                }

                _view.ConnectorView.EndDrawState();

                _modifyingConnection = null;
                _sourcePin = null;
            }
        }

        bool IsModifyingConnection()
        {
            return _modifyingConnection != null;
        }
    }
}
