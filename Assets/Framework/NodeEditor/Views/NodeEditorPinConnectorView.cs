using Framework.NodeSystem;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorPinConnectorView : BaseView
    {
        private bool _isDrawing;
        private NodePin _startPin;

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

        protected override void OnDraw()
        {
            if (_isDrawing)
            {
                if (_startPin == null)
                {
                    _isDrawing = false;
                    return;
                }

                NodeEditorHelper.DrawConnection(_startPin, InputListener.MousePosition);
            }
        }
    }
}
