using System.Net;
using System.Security.Claims;
using AutoMapper;
using BlogApp.Data;
using BlogApp.DTOs;
using BlogApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/blog")]
    public class BlogController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public BlogController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<BlogDto>> CreateBlog(CreateBlogDto createBlogDto)
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            User currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == claim.Value);
            if (currentUser != null)
            {
                Blog blog = new Blog()
                {
                    Id = Guid.NewGuid().ToString(),
                    Owner = currentUser,
                    BlogTitle = createBlogDto.BlogTitle,
                    BlogContent = createBlogDto.BlogContent,
                    IsPublished = createBlogDto.IsPublished
                };

                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();

                return StatusCode(((int)HttpStatusCode.Created),
                             new BlogDto()
                             {
                                 Id = blog.Id,
                                 Owner = new UserDTO()
                                 {
                                     Id = currentUser.Id,
                                     UserName = currentUser.UserName
                                 },
                                 BlogTitle = blog.BlogTitle,
                                 BlogContent = blog.BlogContent,
                                 IsPublished = blog.IsPublished
                             });
            }

            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BlogDto>> DeleteBlog(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Blog blog = await _context.Blogs.Include(blog => blog.Owner).
            FirstOrDefaultAsync(x => x.Id == id && x.Owner.Id == userId);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();

                return Ok(blog);
            }

            return BadRequest("Can't find the blog");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BlogDto>> UpdateBlog(string id, CreateBlogDto createBlogDto)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Blog blog = await _context.Blogs.Include(blog => blog.Owner).
            FirstOrDefaultAsync(x => x.Id == id && x.Owner.Id == userId);
            if (blog != null)
            {
                blog.BlogTitle = createBlogDto.BlogTitle;
                blog.BlogContent = createBlogDto.BlogContent;
                blog.IsPublished = createBlogDto.IsPublished;
                await _context.SaveChangesAsync();

                return Ok(blog);
            }

            return BadRequest("Can't find the blog");
        }

        [HttpGet("privateBlogs")]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetPrivateBlogs()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Blog> queryResult = await _context.Blogs.Include(blog => blog.Owner).
            Where(blog => blog.Owner.Id == userId).ToListAsync();

            List<BlogDto> result = new List<BlogDto>();
            foreach (Blog blog in queryResult)
                result.Add(new BlogDto()
                {
                    Id = blog.Id,
                    Owner = new UserDTO()
                    {
                        Id = blog.Owner.Id,
                        UserName = blog.Owner.UserName
                    },
                    BlogTitle = blog.BlogTitle,
                    BlogContent = blog.BlogContent,
                    IsPublished = blog.IsPublished
                });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetBlog(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Blog blog = await _context.Blogs.Include(blog => blog.Owner).
            FirstOrDefaultAsync(x => x.Id == id && x.Owner.Id == userId);
            if (blog != null)
            {
                return Ok(new BlogDto()
                {
                    Id = blog.Id,
                    Owner = new UserDTO()
                    {
                        Id = blog.Owner.Id,
                        UserName = blog.Owner.UserName
                    },
                    BlogTitle = blog.BlogTitle,
                    BlogContent = blog.BlogContent,
                    IsPublished = blog.IsPublished
                });
            }

            return BadRequest("Can't find the blog");
        }
    }
}