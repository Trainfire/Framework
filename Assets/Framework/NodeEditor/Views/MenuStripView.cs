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
            if (!_graphLoaded)
                return;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Changes"))
                Save.InvokeSafe();

            if (_graphDirty)
            {
                if (GUILayout.Button("Revert Changes"))
                    Revert.InvokeSafe();
            }

            GUILayout.EndHorizontal();
        }
    }
}
