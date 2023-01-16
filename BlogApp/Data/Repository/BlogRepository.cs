
using AutoMapper;
using BlogApp.DTOs;
using BlogApp.Entities;
using BlogApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace BlogApp.Data.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public BlogRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BlogDto> CreateBlog(User owner, CreateBlogDto createBlogDto)
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

            return _mapper.Map<BlogDto>(blog);
        }

        public async Task<BlogDto> UpdateBlog(string userId, string blogId, CreateBlogDto createBlogDto)
        {
            Blog blog = await _context.Blogs.Include(blog => blog.Owner).
            FirstOrDefaultAsync(x => x.Id == blogId && x.Owner.Id == userId);
            if (blog != null)
            {
                blog.BlogTitle = createBlogDto.BlogTitle;
                blog.BlogContent = createBlogDto.BlogContent;
                blog.IsPrivate = createBlogDto.IsPrivate;
                await _context.SaveChangesAsync();

                return _mapper.Map<BlogDto>(blog);
            }

            return null;
        }

        public async Task<BlogDto> DeleteBlog(string userId, string blogId)
        {
            Blog blog = await _context.Blogs.Include(blog => blog.Owner).
            FirstOrDefaultAsync(x => x.Id == blogId && x.Owner.Id == userId);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();

                return _mapper.Map<BlogDto>(blog);
            }

            return null;
        }

        public async Task<BlogDto> GetBlog(string userId, string blogId)
        {
            return await _context.Blogs.Include(blog => blog.Owner)
             .ProjectTo<BlogDto>(_mapper.ConfigurationProvider)
               .FirstOrDefaultAsync(x => x.Id == blogId && (x.Owner.Id == userId || x.IsPrivate == false));
        }

        public async Task<IEnumerable<BlogDto>> GetBlogs(string userId)
        {
            return await _context.Blogs.Include(blog => blog.Owner).
             Where(blog => blog.Owner.Id == userId || blog.IsPrivate == false)
             .ProjectTo<BlogDto>(_mapper.ConfigurationProvider)
                        .ToListAsync();
        }
    }
}