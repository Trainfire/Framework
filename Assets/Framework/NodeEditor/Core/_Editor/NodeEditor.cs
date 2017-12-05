using System;

namespace NodeSystem
{
    public static class NodeEditorEx
    {
        public static void InvokeSafe(this Action action)
        {
            if (action != null)
                action.Invoke();
        }

        public static void InvokeSafe<T>(this Action<T> action, T arg)
        {
            if (action != null)
                action.Invoke(arg);
        }

        public static void InvokeSafe<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action != null)
                action.Invoke(arg1, arg2);
        }
    }
}

namespace NodeSystem.Editor
{
    public class NodeEditor
    {
        private static INodeEditorLogger _logger;
        public static INodeEditorLogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = new NullNodeEditorLogger();
                return _logger;
            }
            set
            {
                if (value != null)
                    _logger = value;
            }
        }

        private static INodeEditorAssertions _assertions;
        public static INodeEditorAssertions Assertions
        {
            get
            {
                if (_assertions == null)
                    _assertions = new NullNodeEditorAssertions();
                return _assertions;
            }
            set
            {
                if (value != null)
                    _assertions = value;
            }
        }
    }
}