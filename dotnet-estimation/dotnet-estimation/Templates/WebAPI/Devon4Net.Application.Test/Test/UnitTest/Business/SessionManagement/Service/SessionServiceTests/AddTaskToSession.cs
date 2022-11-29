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
            var (modifed, addedTask) = await sessionService.AddTaskToSession(17, TaskToAdd);

            //Assert
            Assert.True(modifed);
            Assert.IsType<TaskDto>(addedTask);
        }

    }
}