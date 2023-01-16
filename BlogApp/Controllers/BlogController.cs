using System.Net;
using System.Security.Claims;
using AutoMapper;
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
        private readonly IMapper _mapper;
        public BlogController(IUserRepository userRepository, IBlogRepository blogRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<BlogDto>> CreateBlog(CreateBlogDto createBlogDto)
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            User currentUser = await _userRepository.GetUserFromId(claim.Value);
            if (currentUser != null)
            {
                return StatusCode(((int)HttpStatusCode.Created), await _blogRepository.CreateBlog(currentUser, createBlogDto));
            }

            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BlogDto>> DeleteBlog(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            BlogDto blog = await _blogRepository.DeleteBlog(userId, id);
            if (blog != null)
            {
                return Ok(blog);
            }

            return BadRequest("Can't find the blog");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BlogDto>> UpdateBlog(string id, CreateBlogDto createBlogDto)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            BlogDto blog = await _blogRepository.UpdateBlog(userId, id, createBlogDto);
            if (blog != null)
            {
                return Ok(blog);
            }

            return BadRequest("Can't find the blog");
        }

        [HttpGet("blogs")]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetBlogs()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Ok(await _blogRepository.GetBlogs(userId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetBlog(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            BlogDto blog = await _blogRepository.GetBlog(userId, id);
            if (blog != null)
            {
                return Ok(blog);
            }

            return BadRequest("Can't find the blog");
        }
    }
}