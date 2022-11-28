using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
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
    public class GetStatus : SessionServiceTest
    {
        public GetStatus(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async void GetStatus_WithPoluatedTasks_ReturnsTrueAndTasklist()
        {

            //Arange
            var ExpectedSession = CreateRandomSession(7);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);

            var sessionService = new SessionService(repositoryStub.Object);

            //Act

            var (status, tasks) = await sessionService.GetStatus(7);

            //Assert
            //Assert that the delivered indicator whether the Session is valid or not
            Assert.True(status);
            //Assert that the list of Tasks is the correct one
            tasks.Should().BeEquivalentTo(ExpectedSession.Tasks.ToList(), options => options.ComparingByMembers<TaskStatusChangeDto>());
        }

        [Fact]
        public async void GetStatus_WithExpiredSession_ReturnsFalseAndNull()
        {

            //Arange
            var ExpectedSession = CreateExpiredSession(8);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
            It.IsAny<LiteDB.BsonExpression>()
        ))
            .Returns(ExpectedSession);

            var sessionService = new SessionService(repositoryStub.Object);

            //Act

            var (status, tasks) = await sessionService.GetStatus(8);

            //Assert
            //Assert that the delivered indicator whether the Session is valid or not
            Assert.False(status);
            //Assert that the list of Tasks is Null
            Assert.Null(tasks);
        }
        [Fact]
        public async void GetStatus_WithNoSession_ThrowError()
        {


            var sessionService = new SessionService(repositoryStub.Object);

            //Act
            try
            {
                var (status, tasks) = await sessionService.GetStatus(10);
            }
            //Assert
            catch (Exception NotFoundException)
            {
                Assert.True(true);
            }
        }

    }
}
