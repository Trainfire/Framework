using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeEditor
{
    public class NodeGraphStateManager
    {
        public static void RestoreGraph(NodeGraphData data, NodeGraph graph)
        {
            DebugEx.Log<NodeGraphStateManager>("Reading from graph...");

            Assert.IsNotNull(graph);

            // TODO: Find a nicer way to do this...
            var allNodes = data.Nodes.Concat(data.Constants.Cast<NodeData>()).ToList();
            allNodes.ForEach(nodeData =>
            {
                if (nodeData.GetType() == typeof(NodeConstantData))
                {
                    graph.AddNode(nodeData as NodeConstantData);
                }
                else
                {
                    graph.AddNode(nodeData);
                }
            });

            data.Connections.ForEach(connectionData => graph.Connect(connectionData));
        }

        public static NodeGraphData SaveGraph(NodeGraph graph)
        {
            DebugEx.Log<NodeGraphStateManager>("Saving from graph...");

            var data = new NodeGraphData();

            // TODO: Find a nicer way to do this...
            graph.Nodes.ForEach(node =>
            {
                if (node.GetType() == typeof(NodeConstant))
                {
                    data.Constants.Add(NodeConstantData.Convert(node as NodeConstant));
                }
                else
                {
                    data.Nodes.Add(NodeData.Convert(node));
                }
            });

            graph.Connections.ForEach(connection =>
            {
                data.Connections.Add(new NodeConnectionData(connection.StartPin, connection.EndPin));
            });

            return data;
        }
    }
}