﻿using Framework.NodeSystem;
using UnityEngine;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorPinConnectorView : BaseView
    {
        public string Tooltip { get; set; }

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

                NodeEditorHelper.DrawConnection(_startPin, InputListener.MousePosition);

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
    }
}
