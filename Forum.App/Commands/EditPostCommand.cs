namespace Forum.App.Commands
{
    using Contracts;

    public class EditPostCommand : ICommand
    {
        private IMenuFactory menuFactory;

        public EditPostCommand(IMenuFactory menuFactory)
        {
            this.menuFactory = menuFactory;
        }

        public IMenu Execute(params string[] args)
        {
            int postId = int.Parse(args[0]);

            string commandName = this.GetType().Name;
            string menuName = commandName.Substring(0, commandName.Length - "Command".Length) + "Menu";
            IIdHoldingMenu menu = (IIdHoldingMenu)this.menuFactory.CreateMenu(menuName);

            menu.SetId(postId);

            return menu;
        }
    }
}
