using UnityEngine;
using UnityEditor;
using System;

namespace Framework.NodeSystem
{
    public class NodeGraphState
    {
        public event Action<NodeGraphState> Changed;

        public bool IsDirty { get; private set; }
        public bool GraphLoaded { get; private set; }

        private NodeGraph _graph;

        public NodeGraphState(NodeGraph graph)
        {
            _graph = graph;

            _graph.PostLoad += Graph_PostLoad;
            _graph.PreUnload += Graph_PreUnload;
            _graph.Saved += Graph_Saved;
            _graph.Edited += Graph_Edited;
        }

        public void Destroy()
        {
            _graph.PostLoad -= Graph_PostLoad;
            _graph.PreUnload -= Graph_PreUnload;
            _graph.Saved -= Graph_Saved;
            _graph.Edited -= Graph_Edited;
        }

        void Graph_PostLoad(NodeGraph graph)
        {
            GraphLoaded = true;
            DebugEx.Log<NodeGraphState>("Graph is now loaded.");
            Changed.InvokeSafe(this);
        }

        void Graph_PreUnload(NodeGraph graph)
        {
            IsDirty = false;
            GraphLoaded = false;
            DebugEx.Log<NodeGraphState>("Graph is unloading...");
        }

        void Graph_Saved(NodeGraph graph)
        {
            DebugEx.Log<NodeGraphState>("Graph was saved.");
            IsDirty = false;
            Changed.InvokeSafe(this);
        }

        void Graph_Edited(NodeGraph graph)
        {
            if (GraphLoaded)
            {
                DebugEx.Log<NodeGraphState>("Graph state changed");
                IsDirty = true;
                Changed.InvokeSafe(this);

                // TODO: Record undo changes here?
            }
        }
    }
}