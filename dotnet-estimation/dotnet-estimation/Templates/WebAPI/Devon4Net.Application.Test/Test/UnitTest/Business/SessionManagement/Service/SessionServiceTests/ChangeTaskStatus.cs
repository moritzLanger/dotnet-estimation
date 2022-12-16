using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using ErrorOr;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Devon4Net.Test.Test.UnitTest.Business.SessionManagement.Service.SessionServiceTests
{
    public class ChangeTaskStatus : SessionServiceTest
    {
        public ChangeTaskStatus(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(1, "Task1", Status.Open)]
        [InlineData(1, "Task1", Status.Evaluated)]
        [InlineData(1, "Task1", Status.Suspended)]
        [InlineData(1, "Task1", Status.Ended)]
        public async void ChangeTaskStatus_WithSameStatus_ReturnsNoStatusChanges(long sessionId, string statusId, Status status)
        {
            //Arrange
            var expectedTaskStatusChangeDtoResult = new List<TaskStatusChangeDto>() { };
            var destinationStatus = new TaskStatusChangeDto()
            {
                Id = statusId,
                Status = status
            };

            //Arrange the Mock Repository with a Session
            var repoStub = new Mock<ILiteDbRepository<Session>>();
            var InitialSession = CreateRandomSession(1);
            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            //Act
            var errorOrResult = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.False(errorOrResult.Value.Item1);
            //Assert that the list of modified TaskStatusChangeDtos is empty
            errorOrResult.Value.Item2.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
        }


        [Theory]
        [InlineData(1, "Task1", Status.Suspended)]
        public async void ChangeTaskStatus_OpenToSuspended_ReturnsTheChangedTaskStatus(long sessionId, string statusId, Status status)
        {
            //Arrange
            //Arrange the expected Result
            var expectedTaskStatusChangeDtoResult = new List<TaskStatusChangeDto>() { new TaskStatusChangeDto() { Id = statusId, Status = status } };
            var destinationStatus = new TaskStatusChangeDto()
            {
                Id = statusId,
                Status = status
            };

            //Arrange the Mock Repository with a Session
            var repoStub = new Mock<ILiteDbRepository<Session>>(MockBehavior.Strict);
            var InitialSession = CreateRandomSession(1);
            var firstTask = InitialSession.Tasks[0];
            firstTask.Id = statusId;
            firstTask.Status = Status.Open;

            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            sessionRepositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            //Act
            var errorOrResult = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert 
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.True(errorOrResult.Value.Item1);
            //Assert that the list of modified TaskStatusChangeDtos delivers a suspended Status
            errorOrResult.Value.Item2.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
        }

        [Theory]
        [InlineData(1, "Task1", Status.Ended)]
        public async void ChangeTaskStatus_EvaluatedToEnded_ReturnsTheChangedTaskStatus(long sessionId, string statusId, Status status)
        {
            //Arrange
            //Arrange the expected Result
            var expectedTaskStatusChangeDtoResult = new List<TaskStatusChangeDto>() { new TaskStatusChangeDto() { Id = statusId, Status = status } };
            var destinationStatus = new TaskStatusChangeDto()
            {
                Id = statusId,
                Status = status
            };

            //Arrange the Mock Repository with a Session
            var repoStub = new Mock<ILiteDbRepository<Session>>(MockBehavior.Strict);
            var InitialSession = CreateRandomSession(1);
            var firstTask = InitialSession.Tasks[0];
            firstTask.Id = statusId;
            firstTask.Status = Status.Evaluated;

            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            sessionRepositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            //Act
            var errorOrResult = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert 
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.True(errorOrResult.Value.Item1);
            //Assert that the list of modified TaskStatusChangeDtos delivers a suspended Status
            errorOrResult.Value.Item2.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
        }

        [Theory]
        [InlineData(1, "Task1", Status.Ended)]
        public async void ChangeTaskStatus_SuspendedToEnded_ReturnsFalse(long sessionId, string statusId, Status status)
        {
            //Arrange
            //Arrange the expected Result
            var expectedTaskStatusChangeDtoResult = new List<TaskStatusChangeDto>() { new TaskStatusChangeDto() { Id = statusId, Status = status } };
            var destinationStatus = new TaskStatusChangeDto()
            {
                Id = statusId,
                Status = status
            };

            //Arrange the Mock Repository with a Session
            var repoStub = new Mock<ILiteDbRepository<Session>>(MockBehavior.Strict);
            var InitialSession = CreateRandomSession(1);
            var firstTask = InitialSession.Tasks[0];
            firstTask.Id = statusId;
            firstTask.Status = Status.Suspended;

            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            sessionRepositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            //Act
            var errorOrResult = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert 
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.False(errorOrResult.Value.Item1);
        }
        [Fact]
        public async void ChangeTaskStatus_WithExpiredSession_ReturnsError()
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

            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            //Act
            var ErrorOrEstimation = await service.ChangeTaskStatus(77, new TaskStatusChangeDto());

            var errorDescription = "no session with the sessionId: 77";
            //Act and Assert
            Assert.IsType<ErrorOr<(bool, List<TaskStatusChangeDto>)>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);

        }
        [Fact]
        public async void ChangeTaskStatus_WiThExpiredSession_ReturnsEstimation()
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

            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);
            var errorDescription = "Session with the SessionId: 17 is no longer valid";


            //Act
            var ErrorOrEstimation = await service.ChangeTaskStatus(17, new TaskStatusChangeDto());

            //Assert

            Assert.IsType<ErrorOr<(bool, List<TaskStatusChangeDto>)>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);

        }
    }
}
