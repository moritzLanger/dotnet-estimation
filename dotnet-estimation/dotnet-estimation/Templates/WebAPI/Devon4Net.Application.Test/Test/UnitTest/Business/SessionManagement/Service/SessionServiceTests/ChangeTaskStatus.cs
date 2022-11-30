using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            var service = new SessionService(repositoryStub.Object);

            //Act
            var (modified, modifiedTasks) = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.False(modified);
            //Assert that the list of modified TaskStatusChangeDtos is empty
            modifiedTasks.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
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

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(repositoryStub.Object);

            //Act
            var (modified, modifiedTasks) = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert 
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.True(modified);
            //Assert that the list of modified TaskStatusChangeDtos delivers a suspended Status
            modifiedTasks.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
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

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(repositoryStub.Object);

            //Act
            var (modified, modifiedTasks) = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert 
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.True(modified);
            //Assert that the list of modified TaskStatusChangeDtos delivers a suspended Status
            modifiedTasks.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
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

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(repositoryStub.Object);

            //Act
            var (modified, modifiedTasks) = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert 
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.False(modified);
            //Assert that the list of modified TaskStatusChangeDtos delivers a suspended Status
            //modifiedTasks.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
        }
    }
}
