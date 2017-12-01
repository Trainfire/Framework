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

            const string conversion = "Conversion";
            Register<ConversionToString<float>>("Float to String", conversion);
            Register<ConversionToString<bool>>("Bool to String", conversion);
            Register<ConversionToString<int>>("Int to String", conversion);

            const string core = "Core";
            Register<CoreStart>("Start", core);
            Register<CoreDebugLog>("Debug Log", core);

            const string execute = "Execute";
            Register<ExecuteBranch>("Branch", execute);

            const string logic = "Logic";
            Register<LogicEquals>("Equals", logic);
            Register<LogicNot>("Not", logic);
            Register<LogicAnd>("And", logic);
            Register<LogicOr>("Or", logic);

            const string math = "Math";
            Register<MathAdd>("Add", math);
            Register<MathSubtract>("Subtract", math);
            Register<MathMultiply>("Multiply", math);
            Register<MathDivide>("Divide", math);

            const string misc = "Misc";
            Register<NodeConstant>("Constant", misc);
        }

        void Register<T>(string name, string folder = "") where T : Node
        {
            var key = folder != string.Empty ? string.Format("{0}/{1}", folder, name) : name;
            _nodeRegistry.Add(key, (graph) => graph.AddNode<T>(name));
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
