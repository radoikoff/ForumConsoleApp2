namespace Forum.App.Menus
{
    using Models;
    using Contracts;
    using System.Collections.Generic;

    class EditPostMenu : Menu, ITextAreaMenu, IIdHoldingMenu
    {
        private ILabelFactory labelFactory;
        private ITextAreaFactory textAreaFactory;
        private IForumReader forumReader;
        private ICommandFactory commandFactory;
        private IPostService postService;

        private int postId;
        private IPostViewModel post;

        private bool error;

        public EditPostMenu(ILabelFactory labelFactory, ITextAreaFactory textAreaFactory, IForumReader forumReader, ICommandFactory commandFactory, IPostService postService)
        {
            this.labelFactory = labelFactory;
            this.textAreaFactory = textAreaFactory;
            this.forumReader = forumReader;
            this.commandFactory = commandFactory;
            this.postService = postService;
        }

        public override void Open()
        {
            this.LoadPost();
            this.InitializeTextArea();
            base.Open();
        }

        //private string TitleInput => this.Buttons[0].Text.TrimStart();

        //private string CategoryInput => this.Buttons[1].Text.TrimStart();

        public ITextInputArea TextArea { get; private set; }

        protected override void InitializeStaticLabels(Position consoleCenter)
        {
            string[] labelContents = new string[]
            {
                "All fields must be filled!",
                $"Title: {this.post.Title}",
                $"Old Content:",
            };
            Position[] labelPositions = new Position[]
            {
                new Position(consoleCenter.Left - 18, consoleCenter.Top - 14), // Error: 
                new Position(consoleCenter.Left - 18, consoleCenter.Top - 12), // Title: 
                new Position(consoleCenter.Left - 18, consoleCenter.Top - 10) // Category:
            };

            var labels = new List<ILabel>();
            labels.Add(this.labelFactory.CreateLabel(labelContents[0], labelPositions[0], !error));
            labels.Add(this.labelFactory.CreateLabel(labelContents[1], labelPositions[1]));
            labels.Add(this.labelFactory.CreateLabel(labelContents[2], labelPositions[2]));

            for (int i = 0; i < this.post.Content.Length; i++)
            {
                labels.Add(this.labelFactory.CreateLabel(this.post.Content[i], new Position(consoleCenter.Left - 18, consoleCenter.Top - 10 + (i + 1))));
            }

            this.Labels = labels.ToArray();
        }

        protected override void InitializeButtons(Position consoleCenter)
        {
            string[] buttonContents = new string[]
            {
                "Write",
                "UpdatePost"
            };

            Position[] buttonPositions = new Position[]
            {
                new Position(consoleCenter.Left + 14, consoleCenter.Top - 0),  // Write
                new Position(consoleCenter.Left + 14, consoleCenter.Top + 12)  // UpdatePost
            };

            this.Buttons = new IButton[buttonPositions.Length];

            for (int i = 0; i < buttonPositions.Length; i++)
            {
                this.Buttons[i] = this.labelFactory.CreateButton(buttonContents[i], buttonPositions[i]);
            }

            this.TextArea.Render();
        }

        private void InitializeTextArea()
        {
            Position consoleCenter = Position.ConsoleCenter();
            this.TextArea = this.textAreaFactory.CreateTextArea(this.forumReader, consoleCenter.Left - 18, consoleCenter.Top + 1); //-7
        }

        public override IMenu ExecuteCommand()
        {
            if (this.CurrentOption.IsField)
            {
                string fieldInput = " " + this.forumReader.ReadLine(this.CurrentOption.Position.Left + 1, this.CurrentOption.Position.Top);
                this.Buttons[this.currentIndex] = this.labelFactory.CreateButton(fieldInput, this.CurrentOption.Position, this.CurrentOption.IsHidden, this.CurrentOption.IsField);
                return this;
            }
            else
            {
                try
                {
                    string commandName = string.Join("", this.CurrentOption.Text.Split());
                    ICommand command = this.commandFactory.CreateCommand(commandName);
                    IMenu view = command.Execute(this.postId.ToString(), this.TextArea.Text);
                    return view;
                }
                catch
                {
                    this.error = true;
                    this.InitializeStaticLabels(Position.ConsoleCenter());
                    return this;
                }
            }
        }

        public void SetId(int id)
        {
            this.postId = id;
            this.Open();
        }

        private void LoadPost()
        {
            this.post = this.postService.GetPostViewModel(this.postId);
        }
    }
}
