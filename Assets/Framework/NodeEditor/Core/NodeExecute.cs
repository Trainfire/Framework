using System.Collections.Generic;

namespace Framework.NodeSystem
{
    public class NodeExecuteParameters
    {

    }

    public abstract class NodeExecute : Node
    {
        protected NodeExecutePin ExecuteOut { get; private set; }

        protected override void OnInitialize()
        {
            AddExecuteInPin();
            ExecuteOut = AddExecuteOutPin();
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

    public abstract class NodeExecute1In1Out<T> : NodeExecute
    {
        protected NodeValuePin<T> In { get; private set; }
        protected NodeValuePin<T> Out { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            In = AddInputPin<T>("Log");
            Out = AddOutputPin<T>("Log Out");
        }

        public override abstract void Execute(NodeExecuteParameters parameters);
    }
}