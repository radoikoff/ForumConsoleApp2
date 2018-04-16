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
            throw new NotImplementedException();
        }

        public void AddReplyToPost(int postId, string replyContents, int userId)
        {
            throw new NotImplementedException();
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
    }
}
