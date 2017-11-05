using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeEditor
{
    /// <summary>
    /// Wrapper to expose graph info safely to UI.
    /// </summary>
    public class NodeGraphInfo
    {
        public int NodeCount { get { return _graph == null ? 0 : _graph.Nodes.Count; } }
        public List<NodeConnection> Connections { get { return _graph != null ? _graph.Connections.ToList() : new List<NodeConnection>(); } }

        private NodeGraph _graph;

        public NodeGraphInfo(NodeGraph graph)
        {
            if (graph == null)
                return;

            _graph = graph;
        }
    }
}
