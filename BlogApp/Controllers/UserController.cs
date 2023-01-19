using BlogApp.DTOs;
using BlogApp.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using BlogApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using BlogApp.Middleware;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        private readonly ILogger<UserController> _logger;
        public UserController(IUserRepository userRepository, ITokenService tokenService, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<UserDTO>> Signup(SignUpDTO signUpDTO)
        {
            _logger.LogInformation("Hitted");
            if (await _userRepository.GetUser(signUpDTO.UserName) != null)
            {
                return BadRequest("User already taken");
            }

            User user = await _userRepository.CreateUser(signUpDTO);

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
            User user = await _userRepository.GetUser(loginDto.UserName);

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
            IEnumerable<UserDTO> result = await _userRepository.GetUsers();
            return Ok(result);
        }
    }
}