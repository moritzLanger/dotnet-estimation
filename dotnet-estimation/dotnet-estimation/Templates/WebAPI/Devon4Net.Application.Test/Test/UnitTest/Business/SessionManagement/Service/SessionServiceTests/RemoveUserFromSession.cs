using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
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
            Assert.False(removeUser);
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
            Assert.True(removeUser);
        }


    }
}
