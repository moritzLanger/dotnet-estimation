using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using ErrorOr;
using FluentAssertions;
using Moq;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Devon4Net.Test.Test.UnitTest.Business.SessionManagement.Service.SessionServiceTests
{
    public class AddNewEstimation : SessionServiceTest
    {
        public AddNewEstimation(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(1, "TestUser", 8)]

        public async void AddNewEstimation_WithValidInputs_ReturnsEstimation(long sessionId, string voteBy, int complexity)
        {
            //Arrange
            var estimationToAdd = new Estimation()
            {
                VoteBy = voteBy,
                Complexity = complexity,
            };

            //Arrange the Mock Repository with a Session
            var InitialSession = CreateRadomSessionWithOpenTaskAndEstimations(sessionId);
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
            var newEstimation = await service.AddNewEstimation(sessionId, InitialSession.Tasks[1].Id , voteBy, complexity);

            //Assert
            //Assert that the list of modified TaskStatusChangeDtos is empty
            
            newEstimation.Value.Should().BeEquivalentTo(estimationToAdd, options => options.ComparingByMembers<TaskStatusChangeDto>());
        }

        [Theory]
        [InlineData(3, "TestUser", 8)]
        public async void AddNewEstimation_WithInvalidTaksId_ReturnsError(long sessionId, string voteBy, int complexity)
        {
            //Arrange
            var estimationToAdd = new Estimation()
            {
                VoteBy = voteBy,
                Complexity = complexity,
            };

            //Arrange the Mock Repository with a Session
            var InitialSession = CreateRandomSession(sessionId);
            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            sessionRepositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);
            var errorDescription = "No open or suspended tasks";


            //Act
            var errorOrResult = await service.AddNewEstimation(sessionId, "invalidId", voteBy, complexity);

            //Assert
            Assert.IsType<ErrorOr<Estimation>>(errorOrResult);
            Assert.Equal(errorDescription, errorOrResult.FirstError.Description);


        }

        [Theory]
        [InlineData(4)]
        public async void AddNewEstimation_WithExistingEstimation_ReturnsEstimation(long sessionId)
        {
            //Arrange 
            var InitialSession = CreateRadomSessionWithOpenTaskAndEstimations(sessionId);
            var estimationToAdd = new Estimation()
            {
                VoteBy = InitialSession.Tasks[0].Estimations[1].VoteBy,
                Complexity = InitialSession.Tasks[0].Estimations[1].Complexity + 1,
            };

            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            sessionRepositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(sessionRepositoryStub.Object,userRepositoryStub.Object , jwtHandler.Object);

            //Act
            var newEstimation = await service.AddNewEstimation(sessionId, InitialSession.Tasks[0].Id, InitialSession.Tasks[0].Estimations[1].VoteBy, InitialSession.Tasks[0].Estimations[1].Complexity +1);

            //Assert

            newEstimation.Value.Should().BeEquivalentTo(estimationToAdd, options => options.ComparingByMembers<TaskStatusChangeDto>());

        }
        
        [Fact]
        public async void AddNewEstimation_WithExpiredSession_ReturnsError()
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
            var ErrorOrEstimation = await service.AddNewEstimation(77, "randomID", "RandomName", 9);

            var errorDescription = "no session with the sessionId: 77";
            //Act and Assert
            Assert.IsType<ErrorOr<Estimation>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);
        }
        [Fact]
        public async void AddNewEstimation_WiThExpiredSession_ReturnsEstimation()
        {
            //Arrange 
            var InitialSession = CreateExpiredSession(17);
            var estimationToAdd = new Estimation()
            {
                VoteBy = InitialSession.Tasks[0].Estimations[1].VoteBy,
                Complexity = InitialSession.Tasks[0].Estimations[1].Complexity + 1,
            };

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
            var newEstimation = await service.AddNewEstimation(17, InitialSession.Tasks[0].Id, InitialSession.Tasks[0].Estimations[1].VoteBy, InitialSession.Tasks[0].Estimations[1].Complexity + 1);

            //Assert

            Assert.IsType<ErrorOr<Estimation>>(newEstimation);
            Assert.Equal(errorDescription, newEstimation.FirstError.Description);
        }
    }
}
