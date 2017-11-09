using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeSystem
{
    public class NodeFactory
    {
        private Dictionary<string, Action<NodeGraph>> _nodeRegistry;

        public List<string> Registry { get { return _nodeRegistry.Keys.ToList(); } }

        public NodeFactory()
        {
            _nodeRegistry = new Dictionary<string, Action<NodeGraph>>();
            _nodeRegistry.Add("Core/Start", (graph) => graph.AddNode<CoreStart>("Start"));
            _nodeRegistry.Add("Core/Debug Log", (graph) => graph.AddNode<CoreDebugLog>("Debug Log"));
            _nodeRegistry.Add("Misc/Constant", (graph) => graph.AddNode<NodeConstant>("Constant"));
            _nodeRegistry.Add("Conversion/Float to String", (graph) => graph.AddNode<ConversionFloatToString>("Float to String"));
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
