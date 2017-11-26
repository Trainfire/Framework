using UnityEngine;
using UnityEditor;
using System;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorMenuView : BaseView
    {
        public event Action Save;
        public event Action Revert;
        public event Action Run;

        private bool _graphLoaded;
        public bool GraphLoaded { set { _graphLoaded = value; } }

        private bool _graphDirty;
        public bool GraphDirty { set { _graphDirty = value; } }

        protected override void OnDraw()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true));

            if (!_graphLoaded)
                GUILayout.Label("No graph selected.", EditorStyles.miniLabel);

            if (_graphDirty)
            {
                if (DrawButton("Save"))
                    Save.InvokeSafe();

                if (DrawButton("Revert"))
                    Revert.InvokeSafe();
            }

            if (_graphLoaded)
            {
                if (DrawButton("Run"))
                    Run.InvokeSafe();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        bool DrawButton(string label)
        {
            return GUILayout.Button(label, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false), GUILayout.MinWidth(100f));
        }
    }
}
