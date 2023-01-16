
using BlogApp.DTOs;
using BlogApp.Entities;

namespace BlogApp.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserFromId(string id);
        Task<User> GetUser(string username);
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<User> CreateUser(SignUpDTO signUpDTO);
    }
}