using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

namespace Framework.NodeEditor
{
    [Serializable]
    public class NodeData
    {
        public string ClassType { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
        public Vector2 Position { get; set; }

        public static NodeData Convert(Node node)
        {
            return new NodeData()
            {
                ClassType = node.GetType().ToString(),
                Name = node.Name,
                ID = node.ID,
                Position = node.Position,
            };
        }
    }

    [Serializable]
    public class NodePinData
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
        public int Index { get; set; }
        public string ConnectedPin { get; set; }
    }

    [Serializable]
    public class NodeGraphConnection
    {
        public string SourceNodeID { get; set; }
        public int SourcePinIndex { get; set; }
        public string TargetNodeID { get; set; }
        public int TargetPinIndex { get; set; }

        public NodeGraphConnection(NodePin sourcePin, NodePin targetPin)
        {
            SourceNodeID = sourcePin.Node.ID;
            SourcePinIndex = sourcePin.Index;
            TargetNodeID = targetPin.Node.ID;
            TargetPinIndex = targetPin.Index;
        }
    }

    [Serializable]
    public class NodeGraphData
    {
        public string ID { get; set; }
        public List<NodeData> Nodes { get; set; }
        public List<NodePinData> Pins { get; set; }

        public NodeGraphData()
        {
            ID = "N/A";
            Nodes = new List<NodeData>();
            Pins = new List<NodePinData>();
        }
    }

    public class NodeGraphHelper
    {
        public static void RestoreGraph(NodeGraphData data, NodeGraph graph)
        {
            DebugEx.Log<NodeGraphHelper>("Reading from graph...");

            Assert.IsNotNull(graph);

            data.Nodes.ForEach(nodeData => graph.AddNode(nodeData));
            // TODO: Pins.
        }

        public static NodeGraphData SaveGraph(NodeGraph graph)
        {
            DebugEx.Log<NodeGraphHelper>("Saving from graph...");

            var data = new NodeGraphData();
            graph.Nodes.ForEach(node => data.Nodes.Add(NodeData.Convert(node)));
            // TODO: Pins.

            return data;
        }
    }
}
