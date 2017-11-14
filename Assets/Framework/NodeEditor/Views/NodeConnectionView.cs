﻿using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor.Views
{
    class NodeConnectionView : BaseView
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        protected override void OnDraw()
        {
            Handles.BeginGUI();
            Handles.DrawAAPolyLine(5f, Start, End);
            Handles.EndGUI();

            DrawConnectionEnd(Start);
            DrawConnectionEnd(End);
        }

        void DrawConnectionEnd(Vector2 position)
        {
            const float size = 10f;
            //position = position;
            var rect = new Rect(position, new Vector2(size, size));
            GUI.Box(rect, "");
        }
    }
}
