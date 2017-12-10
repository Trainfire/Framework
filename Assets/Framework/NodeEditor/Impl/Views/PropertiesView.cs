using UnityEngine;
using UnityEditor;
using NodeSystem;
using System;
using System.Collections.Generic;

namespace Framework.NodeEditorViews
{
    public class NodeEditorPropertiesView : BaseView
    {
        enum DrawState
        {
            ContextualProperties,
            GraphProperties,
        }

        DrawState _drawState;
        Dictionary<DrawState, NodeEditorPropertiesPanel> _stateViews;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _stateViews = new Dictionary<DrawState, NodeEditorPropertiesPanel>();
            _stateViews.Add(DrawState.ContextualProperties, new ContextualPropertiesView(GraphHelper));
            _stateViews.Add(DrawState.GraphProperties, new GraphPropertiesView(GraphHelper));
        }

        protected override void OnDraw()
        {
            var style = new GUIStyle();
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f, 1f));
            tex.Apply();
            style.normal.background = tex;

            GUILayout.BeginVertical(style, GUILayout.ExpandHeight(true));

            GUILayout.BeginHorizontal();

            var str = new string[2] { "Contextual Properties", "Graph Variables" };
            _drawState = (DrawState)GUILayout.Toolbar((int)_drawState, str);

            GUILayout.EndHorizontal();

            _stateViews[_drawState].Draw();
        }
    }

    public class NodeEditorPropertiesPanel
    {
        public virtual string Name { get { return "N/A"; } }
        protected NodeGraphHelper GraphHelper { get; private set; }

        public NodeEditorPropertiesPanel(NodeGraphHelper graphHelper)
        {
            GraphHelper = graphHelper;
        }

        public virtual void Draw() { }
    }
}