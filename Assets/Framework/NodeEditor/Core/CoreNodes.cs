using System;

namespace Framework.NodeSystem
{
    public class CoreStart : NodeExecute
    {
        public override void Execute()
        {
            DebugEx.Log<CoreStart>("Executing...");
        }
    }

    public class CoreDebugLog : NodeExecute1In1Out<string, string>
    {
        public override void Execute()
        {
            if (In.Value != null)
            {
                DebugEx.Log<CoreDebugLog>(In.Value);
            }
            else
            {
                DebugEx.Log<CoreDebugLog>("Value is null.");
            }

            Out.Value = In.Value;
        }
    }

    public class DynamicNode : Node
    {
        NodePin In;
        NodePin Out;
        Action OnCalculate;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            In = AddInputPin<NodePinTypeAny>("In");
            Out = AddOutputPin<NodePinTypeAny>("Out");
        }

        protected override void OnPinConnected(NodePinConnectEvent pinConnectEvent)
        {
            if (In.WrappedType != typeof(NodePinTypeAny))
                return;

            if (pinConnectEvent.OtherPin.WrappedType == typeof(float))
            {
                In = ChangePinType<float>(In);
                Out = ChangePinType<float>(Out);
            }
            else if (pinConnectEvent.OtherPin.WrappedType == typeof(int))
            {
                In = ChangePinType<int>(In);
                Out = ChangePinType<int>(Out);
            }
        }

        public override void Calculate()
        {
            Out.SetValueFromPin(In);
        }
    }

    public abstract class MathBase : Node
    {
        NodePin In1;
        NodePin In2;
        NodePin Out;
        Action OnCalculate;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            In1 = AddInputPin<NodePinTypeAny>("In 1");
            In2 = AddInputPin<NodePinTypeAny>("In 2");
            Out = AddOutputPin<NodePinTypeAny>("Out");
        }

        protected override void OnPinConnected(NodePinConnectEvent pinConnectEvent)
        {
            if (In1.WrappedType != typeof(NodePinTypeAny) || In2.WrappedType != typeof(NodePinTypeAny))
                return;

            if (pinConnectEvent.OtherPin.WrappedType == typeof(float))
            {
                In1 = ChangePinType<float>(In1);
                In2 = ChangePinType<float>(In2);
                Out = ChangePinType<float>(Out);
                OnCalculate = CalculateFloat;
            }
            else if (pinConnectEvent.OtherPin.WrappedType == typeof(int))
            {
                In1 = ChangePinType<int>(In1);
                In2 = ChangePinType<int>(In2);
                Out = ChangePinType<int>(Out);
                OnCalculate = CalculateInt;
            }
        }

        public override void Calculate() { OnCalculate(); }

        protected void CalculateFloat() { Write(Out, GetFloat(Read<float>(In1), Read<float>(In2))); }
        protected void CalculateInt() { Write(Out, GetInt(Read<int>(In1), Read<int>(In2))); }

        protected abstract float GetFloat(float a, float b);
        protected abstract int GetInt(int a, int b);
    }

    public class MathAdd : MathBase
    {
        protected override float GetFloat(float a, float b) { return a + b; }
        protected override int GetInt(int a, int b) { return a + b; }
    }

    public class MathSubtract : MathBase
    {
        protected override float GetFloat(float a, float b) { return a - b; }
        protected override int GetInt(int a, int b) { return a - b; }
    }

    public class MathMultiply : MathBase
    {
        protected override float GetFloat(float a, float b) { return a * b; }
        protected override int GetInt(int a, int b) { return a * b; }
    }

    public class MathDivide : MathBase
    {
        protected override float GetFloat(float a, float b) { return a / b; }
        protected override int GetInt(int a, int b) { return a / b; }
    }

    public class ConversionToString<TIn> : Node1In1Out<TIn, string>
    {
        public override void Calculate() { Out.Value = In.ToString(); }
    }

    public class LogicSelect : Node3In1Out<bool, string, string, string>
    {
        public override void Calculate()
        {
            bool condition = In1.Value;
            Out.Value = condition ? In2.Value : In3.Value;
        }
    }

    public class LogicEquals : Node2In1Out<int, int, bool>
    {
        public override void Calculate() { Out.Value = In1.Value == In2.Value; }
    }

    public class LogicNot : Node1In1Out<bool, bool>
    {
        public override void Calculate() { Out.Value = !In.Value; }
    }

    public class LogicAnd : Node2In1Out<bool, bool, bool>
    {
        public override void Calculate() { Out.Value = In1.Value && In2.Value; }
    }

    public class LogicOr : Node2In1Out<bool, bool, bool>
    {
        public override void Calculate() { Out.Value = In1.Value || In2.Value; }
    }
}