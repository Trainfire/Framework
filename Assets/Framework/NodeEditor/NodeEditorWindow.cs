using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    public class NodeEditorWindow : EditorWindow
    {
        private const string WindowName = "Node Editor";

        /// <summary>
        /// Lazy initialize. Can't use Awake because Unity is a strange beast.
        /// </summary>
        private NodeEditorGraphView _graphView;
        private NodeEditorGraphView GraphView
        {
            get
            {
                return _graphView == null ? _graphView = new NodeEditorGraphView() : _graphView;
            }
        }

        [MenuItem("Window/Node Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<NodeEditorWindow>(WindowName);
        }

        void OnGUI()
        {
            BeginWindows();
            GraphView.Draw();
            EndWindows();
        }
    }
}
