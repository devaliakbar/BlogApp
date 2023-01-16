using BlogApp.DTOs;
using BlogApp.Entities;

namespace BlogApp.Interfaces
{
    public interface IBlogRepository
    {
        Task<BlogDto> CreateBlog(User owner, CreateBlogDto createBlogDto);
        Task<BlogDto> DeleteBlog(string userId, string blogId);
        Task<BlogDto> UpdateBlog(string userId, string blogId, CreateBlogDto createBlogDto);
        Task<IEnumerable<BlogDto>> GetBlogs(string userId);
        Task<BlogDto> GetBlog(string userId, string blogId);
    }
}