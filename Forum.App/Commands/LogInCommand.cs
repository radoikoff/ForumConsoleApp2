namespace Forum.App.Commands
{
    using Contracts;
    using System;

    public class LogInCommand : ICommand
    {
        private IUserService userSevice;
        private IMenuFactory menuFactory;

        public LogInCommand(IUserService userSevice, IMenuFactory menuFactory)
        {
            this.userSevice = userSevice;
            this.menuFactory = menuFactory;
        }

        public IMenu Execute(params string[] args)
        {
            string username = args[0];
            string password = args[1];

            bool success = this.userSevice.TryLogInUser(username, password);

            if (success == false)
            {
                throw new InvalidOperationException("Invalid login!");
            }

            return this.menuFactory.CreateMenu("MainMenu");
        }
    }
}
