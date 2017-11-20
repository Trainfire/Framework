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
            // TODO: Would be nice to automate this using reflection...
            _nodeRegistry = new Dictionary<string, Action<NodeGraph>>();

            _nodeRegistry.Add("Conversion/Float to String", (graph) => graph.AddNode<ConversionToString<float>>("Float to String"));
            _nodeRegistry.Add("Conversion/Bool to String", (graph) => graph.AddNode<ConversionToString<bool>>("Bool to String"));
            _nodeRegistry.Add("Conversion/Int to String", (graph) => graph.AddNode<ConversionToString<int>>("Int to String"));

            _nodeRegistry.Add("Core/Start", (graph) => graph.AddNode<CoreStart>("Start"));
            _nodeRegistry.Add("Core/Debug Log", (graph) => graph.AddNode<CoreDebugLog>("Debug Log"));

            _nodeRegistry.Add("Logic/Equals", (graph) => graph.AddNode<LogicEquals>("Equals"));
            _nodeRegistry.Add("Logic/Not", (graph) => graph.AddNode<LogicNot>("Not"));
            _nodeRegistry.Add("Logic/And", (graph) => graph.AddNode<LogicAnd>("And"));
            _nodeRegistry.Add("Logic/Or", (graph) => graph.AddNode<LogicOr>("Or"));

            _nodeRegistry.Add("Math/Add", (graph) => graph.AddNode<MathAdd>("Add"));
            _nodeRegistry.Add("Math/Subtract", (graph) => graph.AddNode<MathSubtract>("Subtract"));
            _nodeRegistry.Add("Math/Multiply", (graph) => graph.AddNode<MathMultiply>("Multiply"));
            _nodeRegistry.Add("Math/Divide", (graph) => graph.AddNode<MathDivide>("Divide"));

            _nodeRegistry.Add("Misc/Constant", (graph) => graph.AddNode<NodeConstant>("Constant"));
            _nodeRegistry.Add("Misc/Dynamic Node (Test)", (graph) => graph.AddNode<DynamicNode>("Dynamic Node"));
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
