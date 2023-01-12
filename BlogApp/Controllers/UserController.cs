using BlogApp.Data;
using BlogApp.DTOs;
using BlogApp.Entities;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly DataContext _context;
        public IMapper _mapper;

        public UserController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(CreateUserDTO createUserDTO)
        {
            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = createUserDTO.UserName
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var result = await _context.Users
                        .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                        .ToListAsync();
            return Ok(result);
        }
    }
}