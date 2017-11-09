using System.Collections.Generic;

namespace Framework.NodeSystem
{
    public class NodeExecuteParameters
    {

    }

    public class NodeExecuteOutput
    {
        
    }

    public abstract class NodeExecute : Node
    {
        protected NodeExecutePin Out { get; private set; }

        protected override void OnInitialize()
        {
            AddExecuteInPin();
            Out = AddExecuteOutPin();
        }

        public abstract void Execute(NodeExecuteParameters parameters);
    }

    public abstract class NodeExecute1In<T> : NodeExecute
    {
        protected NodeValuePin<T> In { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            In = AddInputPin<T>("Log");
        }

        public override abstract void Execute(NodeExecuteParameters parameters);
    }
}