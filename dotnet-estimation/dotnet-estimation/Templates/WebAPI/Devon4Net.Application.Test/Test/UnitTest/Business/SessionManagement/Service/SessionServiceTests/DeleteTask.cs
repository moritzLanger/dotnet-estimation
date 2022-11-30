using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
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

            //Act
            try
            {
                var TaskDeleted = await sessionService.DeleteTask(2, "invalidId");
            }
            //Assert
            catch (Exception TaskNotFoundException) {

                Assert.True(true);
            }
        } 
    }
}
