namespace Framework.NodeEditor
{
    public class NodeGraphInfo
    {
        public int NodeCount { get { return _graph == null ? 0 : _graph.Nodes.Count; } }
        public int ConnectionsCount { get { return _graph == null ? 0 : _graph.Connections.Count; } }

        private NodeGraph _graph;

        public NodeGraphInfo(NodeGraph graph)
        {
            if (graph == null)
                return;

            _graph = graph;
        }
    }
}
