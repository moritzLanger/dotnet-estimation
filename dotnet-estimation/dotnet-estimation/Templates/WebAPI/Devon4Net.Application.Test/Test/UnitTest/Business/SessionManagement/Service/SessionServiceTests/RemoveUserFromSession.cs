using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using ErrorOr;
using Moq;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Devon4Net.Test.Test.UnitTest.Business.SessionManagement.Service.SessionServiceTests
{
    public class RemoveUserFromSession : SessionServiceTest
    {
        public RemoveUserFromSession(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async void RemoveUserFromSession_WithValidSessionAndInvalidUser_ReturnsFalse()
        {
            //Arrange
            var InitialSession = CreateRandomSession(3);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            var session = new SessionService(repositoryStub.Object);

            //Act
            var removeUser = await session.RemoveUserFromSession(3, Guid.NewGuid().ToString());

            //Assert
            Assert.False(removeUser.Value);
        }

       

        [Fact]
        public async void RemoveUserFromSession_WithValidSessionAndUser_ReturnsTrue()
        {
            //Arrange
            var InitialSession = CreateRandomSession(3);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            var session = new SessionService(repositoryStub.Object);
            var userToDelete = InitialSession.Users[1];

            //Act
            var removeUser = await session.RemoveUserFromSession(3, userToDelete.Id);

            //Assert
            Assert.True(removeUser.Value);
        }
        [Fact]
        public async void RemoveUserFromSession_WithExpiredSession_ReturnsError()
        {
            //Arrange 

            repositoryStub.Setup(repo => repo.GetFirstOrDefault
            (It.IsAny<LiteDB.BsonExpression>())
            )
                .Returns<Session>(null);



            var service = new SessionService(repositoryStub.Object);

            //Act
            var ErrorOrEstimation = await service.RemoveUserFromSession(77, "RandomUserID");

            var errorDescription = "no session with the sessionId: 77";
            //Act and Assert
            Assert.IsType<ErrorOr<bool>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);

        }

        [Fact]
        public async void RemoveUserFromSession_WiThExpiredSession_ReturnsEstimation()
        {
            //Arrange 
            var InitialSession = CreateExpiredSession(17);


            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);


            var service = new SessionService(repositoryStub.Object);
            var errorDescription = "Session with the SessionId: 17 is no longer valid";


            //Act
            var ErrorOrEstimation = await service.RemoveUserFromSession(17, "RandomUserId");

            //Assert

            Assert.IsType<ErrorOr<bool>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);

        }


    }
}
