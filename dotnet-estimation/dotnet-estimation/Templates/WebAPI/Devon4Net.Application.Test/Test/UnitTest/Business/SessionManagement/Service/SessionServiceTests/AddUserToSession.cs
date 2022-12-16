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
            var newUser = new User { Id = "Vlad", Username = "testUser" };
            var InitialSession = CreateRandomSession(1);

            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            sessionRepositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            userRepositoryStub.Setup(repo => repo.Create(
                It.IsAny<User>()
              ))
                .Returns(new LiteDB.BsonValue());

            jwtHandler.Setup(jwtHandler => jwtHandler.CreateJwtToken(
                It.IsAny<System.Collections.Generic.List<System.Security.Claims.Claim>>()
                )).Returns("inviteToken");


            var Controller = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);
            var InitialUsers = InitialSession.Users;

            //Act
            var UserAdded = await Controller.AddUserToSession(InitialSession.InviteToken,  newUser.Username, Role.Voter);

            //Assert
            Assert.IsType<ErrorOr<(bool, Application.WebAPI.Implementation.Business.SessionManagement.Dtos.JoinSessionResultDto?)>>(UserAdded);
            Assert.False(UserAdded.IsError);
            Assert.True(UserAdded.Value.Item1);
        }
        
/*
        [Fact]
        public async void AddUserToSession_WithExistingUser_ReturnsFalse()
        {
            //Arrange
            var InitialSession = CreateRandomSession(1);

            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            sessionRepositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);
            var InitialUsers = InitialSession.Users;

            //Act
            var UserAdded = await service.AddUserToSession(InitialSession.InviteToken, InitialSession.Users[1].Username, Role.Voter);

            //Assert

            Assert.False(UserAdded.Value.Item1);
        }*/

        [Fact]
        public async void AddUserToSession_WithNullSession_ReturnsError()
        {
            //Arrange 

            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault
            (It.IsAny<LiteDB.BsonExpression>())
            )
                .Returns<Session>(null);

            sessionRepositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            userRepositoryStub.Setup(repo => repo.Create(
                It.IsAny<User>()
              ))
                .Returns(new LiteDB.BsonValue());

            jwtHandler.Setup(jwtHandler => jwtHandler.CreateJwtToken(
                It.IsAny<System.Collections.Generic.List<System.Security.Claims.Claim>>()
                )).Returns("inviteToken");

            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            //Act
            var ErrorOrEstimation = await service.AddUserToSession("invalidInviteToken", "RandomId", Role.Voter);

            var errorDescription = "no session with the sessionId: 77";
            //Act and Assert
            Assert.IsType<ErrorOr<(bool, Application.WebAPI.Implementation.Business.SessionManagement.Dtos.JoinSessionResultDto?)>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);
        }

        [Fact]
        public async void AddTaskToSession_WiThExpiredSession_ReturnsEstimation()
        {
            //Arrange 
            var InitialSession = CreateExpiredSession(17);

            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            sessionRepositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            userRepositoryStub.Setup(repo => repo.Create(
                It.IsAny<User>()
              ))
                .Returns(new LiteDB.BsonValue());

            jwtHandler.Setup(jwtHandler => jwtHandler.CreateJwtToken(
                It.IsAny<System.Collections.Generic.List<System.Security.Claims.Claim>>()
                )).Returns("inviteToken");

            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);
            var errorDescription = "Session with the SessionId: 17 is no longer valid";


            //Act
            var ErrorOrEstimation = await service.AddUserToSession("invalidInviteToken", "RandomId", Role.Voter);

            //Assert
            Assert.IsType<ErrorOr<(bool, Application.WebAPI.Implementation.Business.SessionManagement.Dtos.JoinSessionResultDto?)>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);
        }
    }
}

