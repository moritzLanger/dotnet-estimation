using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
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
    public class DeleteTask : SessionServiceTest
    {
        public DeleteTask(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public async void DeleteTask_WithValidSessionIdAndTaskID_ReturnsTrue()
        {
            //Arrange
            var ExpectedSession = CreateRandomSession(2);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);
            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var sessionService = new SessionService(repositoryStub.Object);

            //Act
            var TaskDeleted = await sessionService.DeleteTask(2, ExpectedSession.Tasks[0].Id);

            //Assert
            Assert.True(TaskDeleted.Value);
        }

        [Fact]
        public async void DeleteTask_WithInValidTaskId_ThrowsError()
        {
            //Arrange
            var ExpectedSession = CreateRandomSession(2);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);
            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var sessionService = new SessionService(repositoryStub.Object);
            var errorDescription = "Session doesn't contain Task with TaskId : invalidId";

            //Act
            var errorOrResult = await sessionService.DeleteTask(2, "invalidId");

            //Assert
            Assert.IsType<ErrorOr<bool>>(errorOrResult);
            Assert.Equal(errorDescription, errorOrResult.FirstError.Description);

        }
        [Fact]
        public async void DeleteTask_WithExpiredSession_ReturnsError()
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
            var ErrorOrEstimation = await service.DeleteTask(77, "randomTaskID");

            var errorDescription = "no session with the sessionId: 77";

            //Act and Assert
            Assert.IsType<ErrorOr<bool>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);

        }
        [Fact]
        public async void ChangeTaskStatus_WiThExpiredSession_ReturnsEstimation()
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
            var ErrorOrEstimation = await service.DeleteTask(17, "RandomTaskId");

            //Assert

            Assert.IsType<ErrorOr<bool>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);

        }
    }
}
