using QuickFix;

namespace IGX
{
    internal class SocketAcceptor
    {
        private IApplication server;
        private FileStoreFactory storeFactory;
        private SessionSettings settings;
        private ScreenLogFactory logFactory;
        private IMessageFactory messageFactory;

        public SocketAcceptor(IApplication server, FileStoreFactory storeFactory, SessionSettings settings, ScreenLogFactory logFactory, IMessageFactory messageFactory)
        {
            this.server = server;
            this.storeFactory = storeFactory;
            this.settings = settings;
            this.logFactory = logFactory;
            this.messageFactory = messageFactory;
        }
    }
}