using BlogApp.Controllers;
using BlogApp.DTOs;
using BlogApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogAppTest.Controllers
{
    public class UserControllerTest
    {
        private readonly UserController UserController;
        private readonly Mock<IUserRepository> UserRepository = new Mock<IUserRepository>();
        private readonly Mock<ITokenService> TokenService = new Mock<ITokenService>();
        private readonly List<UserDTO> testUsers = new List<UserDTO>() {
                new UserDTO(){
                    Id = "testId",
                    UserName = "userEmail"
                },

            };
        public UserControllerTest()
        {
            UserController = new UserController(UserRepository.Object, TokenService.Object);
        }

        [OneTimeSetUp]
        public void Setup()
        {
            UserRepository.Setup(p => p.GetUsers()).ReturnsAsync(testUsers);
        }

        [Test]
        public async Task TestGetUsersAPI()
        {

            ActionResult<IEnumerable<UserDTO>> result = await UserController.GetUsers();
            if (result.Result is OkObjectResult okResult)
            {
                if (okResult.Value is IEnumerable<UserDTO> users)
                {
                    Assert.That((users.ToList())[0].Id, Is.EqualTo(testUsers[0].Id));
                    return;
                }
            }

            Assert.Fail("Unexpected response");
        }
    }
}