using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Framework.NodeEditor
{
    public class NodeEditorGraphView
    {
        private List<NodeView> nodeViews = new List<NodeView>();

        public NodeEditorGraphView()
        {
            // TEMP: Just generate some nodes for now.
            const int nodeCount = 2;

            for (int i = 0; i < nodeCount; i++)
            {
                var node = new Node();
                node.SetID(i);

                var view = new NodeView(node);
                nodeViews.Add(view);
            }
        }

        public void Draw()
        {
            nodeViews.ForEach(x => x.Draw());
        }
    }
}
