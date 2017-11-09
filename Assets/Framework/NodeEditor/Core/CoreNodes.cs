namespace Framework.NodeSystem
{
    public class CoreStart : NodeExecute
    {
        public override void Execute(NodeExecuteParameters parameters)
        {
            DebugEx.Log<CoreStart>("Executing...");
        }
    }

    public class CoreDebugLog : NodeExecute1In<string>
    {
        public override void Execute(NodeExecuteParameters parameters)
        {
            if (In.Value != null)
            {
                DebugEx.Log<CoreDebugLog>(In.Value);
            }
            else
            {
                DebugEx.Log<CoreDebugLog>("Nowt connected.");
            }
        }
    }

    public class MathAdd : Node
    {
        protected override void OnInitialize()
        {
            AddInputPin<float>("In 1");
            AddInputPin<float>("In 2");
            AddOutputPin<float>("Result");
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

            _in.Value = 64f;
            _out.OnGet += NodeConversionFloatToString_OnGet;
        }

        void NodeConversionFloatToString_OnGet()
        {
            _out.Value = _in.Value.ToString();
        }
    }
}