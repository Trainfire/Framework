namespace Framework.NodeSystem
{
    public class CoreStart : NodeExecute
    {
        public override void Execute(NodeExecuteParameters parameters)
        {
            DebugEx.Log<CoreStart>("Executing...");
        }
    }

    public class CoreDebugLog : NodeExecute1In1Out<string>
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

    public class MathAdd : Node
    {
        private NodeValuePin<float> _in1;
        private NodeValuePin<float> _in2;
        private NodeValuePin<float> _out;

        protected override void OnInitialize()
        {
            _in1 = AddInputPin<float>("In 1");
            _in2 = AddInputPin<float>("In 2");
            _out = AddOutputPin<float>("Result");
        }

        public override void Calculate()
        {
            _out.Value = _in1.Value + _in2.Value;
        }
    }

    public class ConversionFloatToString : Node
    {
        private NodeValuePin<float> _in;
        private NodeValuePin<string> _out;

        protected override void OnInitialize()
        {
            _in = AddInputPin<float>("In");
            _out = AddOutputPin<string>("Out");
        }

        public override void Calculate()
        {
            _out.Value = _in.Value.ToString();
        }
    }
}