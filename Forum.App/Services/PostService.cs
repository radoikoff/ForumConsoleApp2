namespace Forum.App.Services
{
    using Contracts;
    using ViewModels;
    using Forum.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Forum.DataModels;

    public class PostService : IPostService
    {
        private ForumData forumData;
        private IUserService userService;

        public PostService(ForumData forumData, IUserService userService)
        {
            this.forumData = forumData;
            this.userService = userService;
        }


        public int AddPost(int userId, string postTitle, string postCategory, string postContent)
        {
            bool emptyCategory = string.IsNullOrWhiteSpace(postCategory);
            bool emptyTitle = string.IsNullOrWhiteSpace(postTitle);
            bool emptyContent = string.IsNullOrWhiteSpace(postContent);

            if (emptyCategory || emptyTitle || emptyContent)
            {
                throw new ArgumentException("All fields must be filled!");
            }

            Category category = this.EnsureCategory(postCategory);

            int postId = forumData.Posts.LastOrDefault()?.Id + 1 ?? 1;

            User author = this.userService.GetUserById(userId);

            Post post = new Post(postId, postTitle, postContent, category.Id, userId, new List<int>());

            this.forumData.Posts.Add(post);
            author.Posts.Add(post.Id);
            category.Posts.Add(post.Id);
            this.forumData.SaveChanges();

            return post.Id;
        }

        private Category EnsureCategory(string categoryName)
        {
            Category category = this.forumData.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
            {
                int categoryId = forumData.Categories.LastOrDefault()?.Id + 1 ?? 1;

                category = new Category(categoryId, categoryName, new List<int>());

                this.forumData.Categories.Add(category);
                this.forumData.SaveChanges();
            }
            return category;
        }

        public void AddReplyToPost(int postId, string replyContents, int userId)
        {
            bool emptyContent = string.IsNullOrWhiteSpace(replyContents);

            if (emptyContent)
            {
                throw new ArgumentException("Reply content cannot be empty!");
            }

            int replyId = forumData.Replies.LastOrDefault()?.Id + 1 ?? 1;

            Post post = this.forumData.Posts.FirstOrDefault(p => p.Id == postId);

            Reply reply = new Reply(replyId, replyContents, userId, postId);

            this.forumData.Replies.Add(reply);
            post.Replies.Add(replyId);
            this.forumData.SaveChanges();

        }

        public IEnumerable<ICategoryInfoViewModel> GetAllCategories()
        {
            IEnumerable<ICategoryInfoViewModel> categories = this.forumData.Categories.Select(c => new CategoryInfoViewModel(c.Id, c.Name, c.Posts.Count));
            return categories;
        }

        public string GetCategoryName(int categoryId)
        {
            string categoryName = this.forumData.Categories.FirstOrDefault(c => c.Id == categoryId)?.Name;
            if (categoryName == null)
            {
                throw new ArgumentException($"Category with {categoryId} not found!");
            }
            return categoryName;
        }

        public IEnumerable<IPostInfoViewModel> GetCategoryPostsInfo(int categoryId)
        {
            IEnumerable<IPostInfoViewModel> categoryPostInfo = this.forumData.Posts.Where(p => p.CategoryId == categoryId).Select(p => new PostInfoViewModel(p.Id, p.Title, p.Replies.Count));
            return categoryPostInfo;
        }

        public IPostViewModel GetPostViewModel(int postId)
        {
            Post post = this.forumData.Posts.FirstOrDefault(p => p.Id == postId);
            IPostViewModel postView = new PostViewModel(post.Title, this.userService.GetUserName(post.AuthorId), post.Content, this.GetPostReplies(postId));
            return postView;
        }

        public IEnumerable<IReplyViewModel> GetPostReplies(int postId)
        {
            IEnumerable<IReplyViewModel> replies = this.forumData.Replies
                .Where(r => r.PostId == postId)
                .Select(r => new ReplyViewModel(this.userService.GetUserName(r.AuthorId), r.Content));

            return replies;
        }

        public void UpdatePost(int postId, int authorId, string postContent)
        {
            bool emptyContent = string.IsNullOrWhiteSpace(postContent);

            if (emptyContent)
            {
                throw new ArgumentException("Post content cannot be empty!");
            }

            Post post = this.forumData.Posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                throw new ArgumentException($"Post with Id: {postId} does not exisit!");
            }

            if (post.AuthorId != authorId)
            {
                throw new ArgumentException($"Post Id: {postId} cannot be editied by autor Id {authorId}!");
            }

            post.Content = postContent;

            this.forumData.SaveChanges();
        }
    }
}
