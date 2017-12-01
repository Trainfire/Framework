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
    }
}