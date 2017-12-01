using System;
using UnityEngine.Assertions;
using NodeSystem.Editor;

namespace NodeSystem
{
    public enum NodeConstantType
    {
        None,
        Float,
        Int,
        Bool,
        String
    }

    public class NodeConstant : Node
    {
        private NodeConstantType _pinType;
        public NodeConstantType PinType
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

                NodeEditor.Logger.Log<NodeConstant>("Set pin type to '{0}'", _pinType.ToString());

                UpdatePin();
            }
        }

        private object _cachedValue;

        protected override void OnInitialize()
        {
            UpdatePin();
        }

        void UpdatePin()
        {
            NodeEditor.Logger.Log<NodeConstant>("Updating pin...");

            if (HasPin(0))
                RemoveOutputPin(0);

            const string pinName = "Out";

            switch (_pinType)
            {
                case NodeConstantType.Float:
                    AddOutputPin<float>(pinName);
                    break;
                case NodeConstantType.Int:
                    AddOutputPin<int>(pinName);
                    break;
                case NodeConstantType.Bool:
                    AddOutputPin<bool>(pinName);
                    break;
                case NodeConstantType.String:
                    AddOutputPin<string>(pinName);
                    break;
            }

            TriggerChange();
        }

        public void Set(NodeConstantData data)
        {
            PinType = data.ConstantType;

            switch (PinType)
            {
                case NodeConstantType.Float: SetFloat(float.Parse(data.Value)); break;
                case NodeConstantType.Int: SetInt(int.Parse(data.Value)); break;
                case NodeConstantType.Bool: SetBool(bool.Parse(data.Value)); break;
                case NodeConstantType.String: SetString(data.Value); break;
            }
        }

        public int GetInt() { return GetValue<int>(NodeConstantType.Int); }
        public void SetInt(int value) { SetValue(NodeConstantType.Int, value); }

        public float GetFloat() { return GetValue<float>(NodeConstantType.Float); }
        public void SetFloat(float value) { SetValue(NodeConstantType.Float, value); }

        public string GetString() { return GetValue<string>(NodeConstantType.String, () => string.Empty); }
        public void SetString(string value) { SetValue(NodeConstantType.String, value); }

        public bool GetBool() { return GetValue<bool>(NodeConstantType.Bool); }
        public void SetBool(bool value) { SetValue(NodeConstantType.Bool, value); }

        T GetValue<T>(NodeConstantType pinTypeQualifier, Func<T> getDefault = null)
        {
            //var defaultValue = getDefault != null ? getDefault() : default(T);
            return _pinType == pinTypeQualifier ? (OutputPins[0] as NodePin<T>).Value : default(T);
        }

        void SetValue<T>(NodeConstantType pinTypeQualifier, T value)
        {
            Assert.IsTrue(_pinType == pinTypeQualifier);

            if (_pinType == pinTypeQualifier)
            {
                var outValue = (OutputPins[0] as NodePin<T>).Value;
                if (outValue == null || !outValue.Equals(value))
                {
                    (OutputPins[0] as NodePin<T>).Value = value;

                    if (_cachedValue != null && !value.Equals(_cachedValue))
                        TriggerChange();
                }
            }

            _cachedValue = value;
        }
    }
}
