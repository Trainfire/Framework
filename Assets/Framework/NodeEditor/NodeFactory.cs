using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeEditor
{
    public class NodeFactory
    {
        private Dictionary<string, Action<NodeGraph>> _nodeRegistry;

        public List<string> Registry { get { return _nodeRegistry.Keys.ToList(); } }

        public NodeFactory()
        {
            _nodeRegistry = new Dictionary<string, Action<NodeGraph>>();
            _nodeRegistry.Add("Node/Math/Add", (graph) => graph.AddNode<MathNodeAdd>("Add"));
        }

        public void Instantiate(string id, NodeGraph graph)
        {
            var containsID = _nodeRegistry.ContainsKey(id);

            if (containsID)
            {
                DebugEx.Log<NodeFactory>("Spawn node '" + id + "'");
                _nodeRegistry[id](graph);
            }
        }
    }
}
