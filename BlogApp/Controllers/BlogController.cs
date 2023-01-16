using System.Net;
using System.Security.Claims;
using BlogApp.DTOs;
using BlogApp.Entities;
using BlogApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/blog")]
    public class BlogController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IBlogRepository _blogRepository;
        public BlogController(IUserRepository userRepository, IBlogRepository blogRepository)
        {
            _userRepository = userRepository;
            _blogRepository = blogRepository;
        }

        [HttpPost]
        public async Task<ActionResult<BlogDto>> CreateBlog(CreateBlogDto createBlogDto)
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            User currentUser = await _userRepository.GetUserFromId(claim.Value);
            if (currentUser != null)
            {
                Blog blog = await _blogRepository.CreateBlog(currentUser, createBlogDto);

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
                                 IsPrivate = blog.IsPrivate
                             });
            }

            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BlogDto>> DeleteBlog(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Blog blog = await _blogRepository.DeleteBlog(userId, id);
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
                    IsPrivate = blog.IsPrivate
                });
            }

            return BadRequest("Can't find the blog");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BlogDto>> UpdateBlog(string id, CreateBlogDto createBlogDto)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Blog blog = await _blogRepository.UpdateBlog(userId, id, createBlogDto);
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
                    IsPrivate = blog.IsPrivate
                });
            }

            return BadRequest("Can't find the blog");
        }

        [HttpGet("blogs")]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetBlogs()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Blog> queryResult = await _blogRepository.GetBlogs(userId);

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
                    IsPrivate = blog.IsPrivate
                });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetBlog(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Blog blog = await _blogRepository.GetBlog(userId, id);
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
                    IsPrivate = blog.IsPrivate
                });
            }

            return BadRequest("Can't find the blog");
        }
    }
}