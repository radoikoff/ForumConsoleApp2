namespace Forum.App.Commands
{
    using Contracts;

    class UpdatePostCommand : ICommand
    {
        private ISession session;
        private IPostService postService;
        private ICommandFactory commandFactory;

        public UpdatePostCommand(ISession session, IPostService postService, ICommandFactory commandFactory)
        {
            this.session = session;
            this.postService = postService;
            this.commandFactory = commandFactory;
        }

        public IMenu Execute(params string[] args)
        {
            int userId = this.session.UserId;
            int postId = int.Parse(args[0]);
            string postContent = args[1];

            this.postService.UpdatePost(postId, userId, postContent);

            this.session.Back();
            this.session.Back();

            ICommand command = this.commandFactory.CreateCommand("ViewPostMenu");
            IMenu view = command.Execute(postId.ToString());

            return view;
        }
    }
}
