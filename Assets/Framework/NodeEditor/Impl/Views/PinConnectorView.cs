using UnityEngine;
using NodeSystem;
using NodeSystem.Editor;

namespace Framework.NodeEditorViews
{
    public class NodeEditorPinConnectorView : BaseView, INodeEditorPinConnectorView
    {
        public string Tooltip { get; set; }
        public NodePin EndPin { set { _endPin = value; } }

        private bool _isDrawing;
        private NodePin _startPin;
        private NodePin _endPin;

        public void EnterDrawState(NodePin startPin)
        {
            _isDrawing = true;
            _startPin = startPin;
        }

        public void EndDrawState()
        {
            _isDrawing = false;
            _startPin = null;
        }

        public void SetEndPin(NodePin pin)
        {
            _endPin = pin;
        }

        protected override void OnDraw()
        {
            if (_isDrawing)
            {
                if (_startPin == null)
                {
                    _isDrawing = false;
                    return;
                }

                //NodeEditorConnectionDrawer.Draw(_startPin, InputListener.MousePosition);
                NodeEditorConnectionDrawer.Draw(Vector2.zero, InputListener.MousePosition, Color.white);

                if (_endPin != null)
                {
                    var offset = new Vector2(0, -25f);
                    var rect = new Rect(InputListener.MousePosition + offset, new Vector2(200f, 20f));

                    if (Tooltip != string.Empty)
                    {
                        GUILayout.BeginArea(rect);
                        GUILayout.Box(Tooltip);
                        GUILayout.EndArea();
                    }
                }
            }
        }

        protected override void OnDispose()
        {
            _startPin = null;
            _endPin = null;
        }
    }
}
