using BlogApp.Entities;

namespace BlogApp.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}