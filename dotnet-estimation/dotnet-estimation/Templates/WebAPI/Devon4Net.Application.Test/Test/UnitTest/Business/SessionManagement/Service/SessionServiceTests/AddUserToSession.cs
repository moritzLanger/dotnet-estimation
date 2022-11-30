using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Devon4Net.Test.Test.UnitTest.Business.SessionManagement.Service.SessionServiceTests
{
    public class AddUserToSession : SessionServiceTest
    {
        public AddUserToSession(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async void AddUserToSession_WithPopulatedRepo_ReturnsTrue()
        {
            //Arrange
            var newUser = new User { Id = "Vlad", Role = 0 };
            var InitialSession = CreateRandomSession(1);

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var Controller = new SessionService(repositoryStub.Object);
            var InitialUsers = InitialSession.Users;

            //Act
            var UserAdded = await Controller.AddUserToSession(1, newUser.Id, newUser.Role);

            //Assert
            Assert.True(UserAdded);
        }
        

        [Fact]
        public async void AddUserToSession_WithExistingUser_ReturnsFalse()
        {
            //Arrange
            var InitialSession = CreateRandomSession(1);

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var Controller = new SessionService(repositoryStub.Object);
            var InitialUsers = InitialSession.Users;

            //Act
            var UserAdded = await Controller.AddUserToSession(1, InitialSession.Users[1].Id, InitialSession.Users[1].Role);

            //Assert
            Assert.False(UserAdded);
       }
    }
}

