using BlogApp.DTOs;
using BlogApp.Entities;

namespace BlogApp.Interfaces
{
    public interface IBlogRepository
    {
        Task<Blog> CreateBlog(User owner, CreateBlogDto createBlogDto);
        Task<Blog> DeleteBlog(string userId, string blogId);
        Task<Blog> UpdateBlog(string userId, string blogId, CreateBlogDto createBlogDto);
        Task<List<Blog>> GetBlogs(string userId);
        Task<Blog> GetBlog(string userId, string blogId);
    }
}