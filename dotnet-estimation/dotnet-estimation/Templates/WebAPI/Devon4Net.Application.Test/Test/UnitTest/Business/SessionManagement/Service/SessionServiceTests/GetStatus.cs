using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using ErrorOr;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
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
            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);

            var sessionService = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            //Act


            var ErrorOrStatus = await sessionService.GetStatus(7);

            //Assert
            Assert.True(ErrorOrStatus.Value.Item1);
            ErrorOrStatus.Value.Item3.Should().BeEquivalentTo(ExpectedSession.Tasks.ToList(), options => options.ComparingByMembers<TaskStatusChangeDto>());
        }

        [Fact]
        public async void GetStatus_WithunPoluatedTasks_ReturnsTrueAndTasklist()
        {

            //Arange
            var ExpectedSession = CreateExpiredSession(7);
            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);

            var sessionService = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            //Act

            var ErrorOrStatus = await sessionService.GetStatus(7);

            //Assert
            Assert.True(ErrorOrStatus.IsError);
        }
        [Fact]
        public async void GetStatus_WithExpiredSession_ReturnsError()
        {
            //Arrange 

            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault
            (It.IsAny<LiteDB.BsonExpression>())
            )
                .Returns<Session>(null);



            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            //Act
            var ErrorOrEstimation = await service.GetStatus(77);

            var errorDescription = "no session with the sessionId: 77";
            //Act and Assert
            Assert.IsType<ErrorOr<(bool, string?, List<Task>, List<User>)>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);

        }
        [Fact]
        public async void GetStatus_WiThExpiredSession_ReturnsEstimation()
        {
            //Arrange 
            var InitialSession = CreateExpiredSession(17);


            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);


            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);
            var errorDescription = "Session with the SessionId: 17 is no longer valid";


            //Act
            var ErrorOrEstimation = await service.GetStatus(17);

            //Assert

            Assert.IsType<ErrorOr<(bool, string?, List<Task>, List<User>)>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);

        }
    }
}
