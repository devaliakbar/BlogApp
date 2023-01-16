
using AutoMapper;
using BlogApp.DTOs;
using BlogApp.Entities;
using BlogApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public BlogRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<Blog> CreateBlog(User owner, CreateBlogDto createBlogDto)
        {
            Blog blog = new Blog()
            {
                Id = Guid.NewGuid().ToString(),
                Owner = owner,
                BlogTitle = createBlogDto.BlogTitle,
                BlogContent = createBlogDto.BlogContent,
                IsPrivate = createBlogDto.IsPrivate
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return blog;
        }

        public async Task<Blog> UpdateBlog(string userId, string blogId, CreateBlogDto createBlogDto)
        {
            Blog blog = await _context.Blogs.Include(blog => blog.Owner).
            FirstOrDefaultAsync(x => x.Id == blogId && x.Owner.Id == userId);
            if (blog != null)
            {
                blog.BlogTitle = createBlogDto.BlogTitle;
                blog.BlogContent = createBlogDto.BlogContent;
                blog.IsPrivate = createBlogDto.IsPrivate;
                await _context.SaveChangesAsync();

                return blog;
            }

            return null;
        }

        public async Task<Blog> DeleteBlog(string userId, string blogId)
        {
            Blog blog = await _context.Blogs.Include(blog => blog.Owner).
            FirstOrDefaultAsync(x => x.Id == blogId && x.Owner.Id == userId);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();

                return blog;
            }

            return null;
        }

        public async Task<Blog> GetBlog(string userId, string blogId)
        {
            return await _context.Blogs.Include(blog => blog.Owner).
               FirstOrDefaultAsync(x => x.Id == blogId && (x.Owner.Id == userId || x.IsPrivate == false));
        }

        public async Task<List<Blog>> GetBlogs(string userId)
        {
            return await _context.Blogs.Include(blog => blog.Owner).
             Where(blog => blog.Owner.Id == userId || blog.IsPrivate == false).ToListAsync();
        }
    }
}