using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
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
    public class AddTaskToSession : SessionServiceTest
    {
        public AddTaskToSession(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public async void AddTaskToSession_WithVaildSessionIdAndTask_ReturnsTrueAndTaskDto()
        {
            //Arrange
            var TaskToAdd = new TaskDto
            {
                Id = Guid.NewGuid().ToString(),
                Title = "testTitle",
                Status = Status.Open
            };

            var ExpectedSession = CreateRandomSession(17);
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
            var errorOrResult = await sessionService.AddTaskToSession(17, TaskToAdd);

            //Assert
            Assert.True(errorOrResult.Value.Item1);
            Assert.IsType<TaskDto>(errorOrResult.Value.Item2);
        }
        [Fact]
        public async void AddTaskToSession_WithExpiredSession_ReturnsError()
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
            var ErrorOrEstimation = await service.AddTaskToSession(77, new TaskDto());

            var errorDescription = "no session with the sessionId: 77";
            //Act and Assert
            Assert.IsType<ErrorOr<(bool,TaskDto)>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);
        }
        [Fact]
        public async void AddTaskToSession_WiThExpiredSession_ReturnsEstimation()
        {
            //Arrange 
            var InitialSession = CreateExpiredSession(17);
            var estimationToAdd = new Estimation()
            {
                VoteBy = InitialSession.Tasks[0].Estimations[1].VoteBy,
                Complexity = InitialSession.Tasks[0].Estimations[1].Complexity + 1,
            };

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
            var ErrorOrEstimation = await service.AddTaskToSession(17, new TaskDto());

            //Assert

            Assert.IsType<ErrorOr<(bool, TaskDto)>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);
        }
    }
}