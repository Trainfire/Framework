using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;

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
    public class NodeConstantData : NodeData
    {
        public NodePinType ConstantType { get; set; }
        public string Value { get; set; }

        public static NodeConstantData Convert(NodeConstant constant)
        {
            // Convert base node data.
            var nodeData = NodeData.Convert(constant);

            var constantData = new NodeConstantData()
            {
                ClassType = nodeData.ClassType,
                Name = nodeData.Name,
                ID = nodeData.ID,
                Position = nodeData.Position,
                ConstantType = constant.PinType
            };

            switch (constant.PinType)
            {
                case NodePinType.None: constantData.Value = string.Empty; break;
                case NodePinType.Float: constantData.Value = constant.GetFloat().ToString(); break;
                case NodePinType.Int: constantData.Value = constant.GetInt().ToString(); break;
                case NodePinType.Bool: constantData.Value = constant.GetBool().ToString(); break;
                case NodePinType.String: constantData.Value = constant.GetString(); break;
            };

            return constantData;
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
    public class NodeConnectionData
    {
        public string SourceNodeId { get; private set; }
        public int SourcePinId { get; private set; }
        public string TargetNodeId { get; private set; }
        public int TargetPinId { get; private set; }

        public NodeConnectionData(string sourceNodeId, int sourcePinId, string targetNodeId, int targetPinId)
        {
            SourceNodeId = sourceNodeId;
            SourcePinId = sourcePinId;
            TargetNodeId = targetNodeId;
            TargetPinId = targetPinId;
        }

        public NodeConnectionData(NodePin startPin, NodePin endPin) : this(startPin.Node.ID, startPin.Index, endPin.Node.ID, endPin.Index)
        {

        }
    }

    [Serializable]
    public class NodeGraphData
    {
        public string ID { get; set; }
        public List<NodeData> Nodes { get; set; }
        public List<NodeConnectionData> Connections { get; set; }
        public List<NodeConstantData> Constants { get; set; }

        public NodeGraphData()
        {
            ID = "N/A";
            Nodes = new List<NodeData>();
            Connections = new List<NodeConnectionData>();
            Constants = new List<NodeConstantData>();
        }
    }
}
