using System;
using System.Collections.Generic;

namespace NodeSystem
{
    [Serializable]
    public class NodeData
    {
        public string ClassType;
        public string Name;
        public string ID;
        public NodeVec2 Position;

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
        public NodeConstantType ConstantType;
        public string Value;

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
                case NodeConstantType.None: constantData.Value = string.Empty; break;
                case NodeConstantType.Float: constantData.Value = constant.GetFloat().ToString(); break;
                case NodeConstantType.Int: constantData.Value = constant.GetInt().ToString(); break;
                case NodeConstantType.Bool: constantData.Value = constant.GetBool().ToString(); break;
                case NodeConstantType.String: constantData.Value = constant.GetString(); break;
            };

            return constantData;
        }
    }

    [Serializable]
    public class NodePinData
    {
        public string Type;
        public string Name;
        public string ID;
        public int Index;
        public string ConnectedPin;
    }

    [Serializable]
    public class NodeConnectionData
    {
        public string SourceNodeId;
        public int SourcePinId;
        public string TargetNodeId;
        public int TargetPinId;

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
        public string ID;
        public List<NodeData> Nodes;
        public List<NodeConnectionData> Connections;
        public List<NodeConstantData> Constants;

        public NodeGraphData()
        {
            ID = "N/A";
            Nodes = new List<NodeData>();
            Connections = new List<NodeConnectionData>();
            Constants = new List<NodeConstantData>();
        }

        public NodeGraphData(NodeGraphData original)
        {
            ID = original.ID;
            Nodes = original.Nodes;
            Connections = original.Connections;
            Constants = original.Constants;
        }
    }
}
