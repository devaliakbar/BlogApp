using BlogApp.Data;
using BlogApp.DTOs;
using BlogApp.Entities;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using BlogApp.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        public UserController(DataContext context, IMapper mapper, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<UserDTO>> CreateUser(SignUpDTO signUpDTO)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == signUpDTO.UserName.ToLower()))
            {
                return BadRequest("User already taken");
            }

            using var hmac = new HMACSHA512();

            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = signUpDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signUpDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return StatusCode(((int)HttpStatusCode.Created), new SignInResponseDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDto loginDto)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null) return Unauthorized("Username or password is incorrect");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            byte[] passedPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < passedPasswordHash.Length; i++)
                if (passedPasswordHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Username or password is incorrect");
                }

            return Ok(new SignInResponseDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            });
        }

        [HttpGet("getAllUsers")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var result = await _context.Users
                        .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                        .ToListAsync();
            return Ok(result);
        }
    }
}