using UnityEngine.Assertions;

namespace Framework.NodeEditor
{
    public enum NodePinType
    {
        None,
        Float,
        Int,
        Bool,
        String
    }

    public class NodeConstant : Node
    {
        private NodePinType _pinType;
        public NodePinType PinType
        {
            get
            {
                return _pinType;
            }
            set
            {
                // Only trigger pin change when the type is different from the current type.
                if (_pinType == value)
                    return;

                _pinType = value;

                DebugEx.Log<NodeConstant>("Set pin type to '{0}'", _pinType.ToString());

                UpdatePin();
            }
        }

        protected override void OnInitialize()
        {
            UpdatePin();
        }

        void UpdatePin()
        {
            DebugEx.Log<NodeConstant>("Updating pin...");
            RemoveOutputPin(0);

            const string pinName = "Out";

            switch (_pinType)
            {
                case NodePinType.Float:
                    AddOutputPin<float>(pinName);
                    break;
                case NodePinType.Int:
                    AddOutputPin<int>(pinName);
                    break;
                case NodePinType.Bool:
                    AddOutputPin<bool>(pinName);
                    break;
                case NodePinType.String:
                    AddOutputPin<string>(pinName);
                    break;
            }
        }

        public void Set(NodeConstantData data)
        {
            PinType = data.ConstantType;

            switch (PinType)
            {
                case NodePinType.Float: SetFloat(float.Parse(data.Value)); break;
                case NodePinType.Int: SetInt(int.Parse(data.Value)); break;
                case NodePinType.Bool: SetBool(bool.Parse(data.Value)); break;
                case NodePinType.String: SetString(data.Value); break;
            }
        }

        public int GetInt() { return GetValue<int>(NodePinType.Int); }
        public void SetInt(int value) { SetValue(NodePinType.Int, value); }

        public float GetFloat() { return GetValue<float>(NodePinType.Float); }
        public void SetFloat(float value) { SetValue(NodePinType.Float, value); }

        public string GetString() { return GetValue<string>(NodePinType.String); }
        public void SetString(string value) { SetValue(NodePinType.String, value); }

        public bool GetBool() { return GetValue<bool>(NodePinType.Bool); }
        public void SetBool(bool value) { SetValue(NodePinType.Bool, value); }

        T GetValue<T>(NodePinType pinTypeQualifier)
        {
            return _pinType == pinTypeQualifier ? (OutputPins[0] as NodeValuePin<T>).Value : default(T);
        }

        void SetValue<T>(NodePinType pinTypeQualifier, T value)
        {
            Assert.IsTrue(_pinType == pinTypeQualifier);

            if (_pinType == pinTypeQualifier)
                (OutputPins[0] as NodeValuePin<T>).Value = value;
        }
    }
}
