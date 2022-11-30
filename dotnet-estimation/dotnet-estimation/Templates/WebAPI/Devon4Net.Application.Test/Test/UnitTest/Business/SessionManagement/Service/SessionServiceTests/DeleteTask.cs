using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
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
            Assert.True(TaskDeleted);
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

            //Act and Assert
            await Assert.ThrowsAsync<TaskNotFoundException>(()=>sessionService.DeleteTask(2, "invalidId")).ConfigureAwait(false);

        } 
    }
}
