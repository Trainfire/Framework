namespace Framework.NodeSystem
{
    public class CoreStart : NodeExecute
    {
        public override void Execute(NodeExecuteParameters parameters)
        {
            DebugEx.Log<CoreStart>("Executing...");
        }
    }

    public class CoreDebugLog : NodeExecute1In1Out<string, string>
    {
        public override void Execute(NodeExecuteParameters parameters)
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

    public class MathAdd : Node2In1Out<float, float, float>
    {
        public override void Calculate() { Out.Value = In1.Value + In2.Value; }
    }

    public class MathSubtract : Node2In1Out<float, float, float>
    {
        public override void Calculate() { Out.Value = In1.Value - In2.Value; }
    }

    public class MathMultiply : Node2In1Out<float, float, float>
    {
        public override void Calculate() { Out.Value = In1.Value * In2.Value; }
    }

    public class MathDivide : Node2In1Out<float, float, float>
    {
        public override void Calculate() { Out.Value = In1.Value / In2.Value; }
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