using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using ErrorOr;
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
            Assert.True(UserAdded.Value);
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
            Assert.False(UserAdded.Value);
        }

        [Fact]
        public async void AddUserToSession_WithExpiredSession_ReturnsError()
        {
            //Arrange 

            repositoryStub.Setup(repo => repo.GetFirstOrDefault
            (It.IsAny<LiteDB.BsonExpression>())
            )
                .Returns<Session>(null);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(repositoryStub.Object);

            //Act
            var ErrorOrEstimation = await service.AddUserToSession(77, "RandomId", Role.Voter);

            var errorDescription = "no session with the sessionId: 77";
            //Act and Assert
            Assert.IsType<ErrorOr<bool>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);
        }

        [Fact]
        public async void AddTaskToSession_WiThExpiredSession_ReturnsEstimation()
        {
            //Arrange 
            var InitialSession = CreateExpiredSession(17);

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(repositoryStub.Object);
            var errorDescription = "Session with the SessionId: 17 is no longer valid";


            //Act
            var ErrorOrEstimation = await service.AddUserToSession(17, "RandomId", Role.Voter);

            //Assert

            Assert.IsType<ErrorOr<bool>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);
        }

    }
}

