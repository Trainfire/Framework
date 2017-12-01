using System;
using Framework.NodeSystem;
using Framework.NodeEditor.Views;

namespace Framework.NodeEditor
{
    public class NodeEditorPinConnector
    {
        private enum ValidationResult
        {
            Valid,
            Invalid,
            ConnectingToSelf,
            IncompatiblePins,
        }

        private NodeGraph _graph;
        private NodeEditorPinConnectorView _view;
        private INodeEditorUserEventsListener _input;

        private NodeConnection _modifyingConnection;
        private NodePin _sourcePin;
        private NodePin _targetPin;

        public NodeEditorPinConnector(NodeGraph graph, NodeEditorPinConnectorView view, INodeEditorUserEventsListener input)
        {
            _graph = graph;

            _view = view;
            _input = input;
            _input.MouseDownOverPin += Input_SelectPin;
            _input.MouseUpOverPin += Input_MouseUpOverPin;
            _input.MouseUp += Input_MouseUp;
            _input.MouseHoverEnterPin += Input_MouseHoverEnterPin;
            _input.MouseHoverLeavePin += Input_MouseHoverLeavePin;
        }

        void Input_MouseHoverLeavePin()
        {
            _view.Tooltip = string.Empty;
        }

        void Input_MouseHoverEnterPin(NodePin nodePin)
        {
            var validationResult = ValidateConnection(nodePin);

            if (validationResult == ValidationResult.Valid)
            {
                _view.Tooltip = "Connect";
            }
            else
            {
                _view.Tooltip = GetErrorMessage(validationResult);
            }
            
            _view.SetEndPin(nodePin);
        }

        void Input_SelectPin(NodePin nodePin)
        {
            if (nodePin.IsInput())
            {
                DebugEx.Log<NodeEditorPinConnector>("Modifying a connection...");

                _modifyingConnection = _graph.Helper.GetConnection(nodePin);

                if (_modifyingConnection != null)
                {
                    _modifyingConnection.Hide();

                    // Executes flow left to right so get the correct starting pin.
                    var startPin = nodePin.WrappedType == typeof(NodePinTypeExecute) ? _modifyingConnection.StartPin : _modifyingConnection.EndPin;
                    _view.EnterDrawState(startPin);
                }
            }
            else
            {
                DebugEx.Log<NodeEditorPinConnector>("Creating a new connection...");
                _view.EnterDrawState(nodePin);
            }

            _sourcePin = nodePin;
        }

        void Input_MouseUp()
        {
            if (_modifyingConnection != null)
            {
                DebugEx.Log<NodeEditorPinConnector>("Removing a modified connection.");
                _graph.Disconnect(_modifyingConnection);
            }

            _view.EndDrawState();
            _modifyingConnection = null;
            _sourcePin = null;
        }

        void Input_MouseUpOverPin(NodePin targetPin)
        {
            _targetPin = targetPin;

            if (ValidateConnection(targetPin) != ValidationResult.Valid)
            {
                DebugEx.Log<NodeEditorPinConnector>(GetErrorMessage(ValidateConnection(targetPin)));
                return;
            }

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
            if (_sourcePin.WrappedType == typeof(NodePinTypeExecute))
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

            _view.EndDrawState();

            _modifyingConnection = null;
            _sourcePin = null;
        }

        bool IsModifyingConnection()
        {
            return _modifyingConnection != null;
        }

        ValidationResult ValidateConnection(NodePin targetPin)
        {
            if (targetPin == null || _sourcePin == null)
                return ValidationResult.Invalid;

            if (!IsModifyingConnection() && targetPin.Node == _sourcePin.Node)
            {
                DebugEx.LogWarning<NodeEditorPinConnector>("Attempted to connect a pin to itself!");
                return ValidationResult.ConnectingToSelf;
            }

            if (!targetPin.ArePinsCompatible(_sourcePin))
                return ValidationResult.IncompatiblePins;

            return ValidationResult.Valid;
        }

        string GetErrorMessage(ValidationResult connectionError)
        {
            switch (connectionError)
            {
                case ValidationResult.Invalid: return "Invalid";
                case ValidationResult.ConnectingToSelf: return "Cannot connect to self";
                case ValidationResult.IncompatiblePins: return "Incompatible pins";
                default: return string.Empty;
            }
        }
    }
}
