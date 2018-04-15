namespace Forum.App.Commands
{
    using Contracts;

    public class LogOutCommand : ICommand
    {
        private ISession session;
        private IMenuFactory menuFactory;

        public LogOutCommand(ISession session, IMenuFactory menuFactory)
        {
            this.session = session;
            this.menuFactory = menuFactory;
        }

        public IMenu Execute(params string[] args)
        {
            this.session.Reset();
            IMenu menu = this.menuFactory.CreateMenu("MainMenu");
            return menu;
        }
    }
}
