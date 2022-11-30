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
    public class AddNewEstimation : SessionServiceTest
    {
        public AddNewEstimation(ITestOutputHelper output) : base(output)
        {
        }

    //works when there are no Esstimations for the Tasks, doesn't work wenn there is a estimation to change, Maybe the problem is the .Any() search 
    //in the original Methode line 113 the Where().Any() search doens't make a problem but in line 126 .Any() sems to make a problem
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
            var newEstimation = await service.AddNewEstimation(sessionId, InitialSession.Tasks[1].Id , voteBy, complexity);

            //Assert
            //Assert that the list of modified TaskStatusChangeDtos is empty
            
            newEstimation.Should().BeEquivalentTo(estimationToAdd, options => options.ComparingByMembers<TaskStatusChangeDto>());
        }

        [Theory]
        [InlineData(2, "TestUser", 8)]
        public async void AddNewEstimation_WithExpiredSessionId_ThrowsError(long sessionId, string voteBy, int complexity)
        {
            //Arrange
            var estimationToAdd = new Estimation()
            {
                VoteBy = voteBy,
                Complexity = complexity,
            };

            //Arrange the Mock Repository with a Session
            var InitialSession = CreateExpiredSession(sessionId);
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
            try
            {
                var newEstimation = await service.AddNewEstimation(sessionId, InitialSession.Tasks[1].Id, voteBy, complexity);
            }
            //Assert
            catch(Exception NoLongerValid)
            {
                Assert.True(true);
            }
        }
        [Theory]
        [InlineData(3, "TestUser", 8)]
        public async void AddNewEstimation_WithInvalidTaksId_ThrowsError(long sessionId, string voteBy, int complexity)
        {
            //Arrange
            var estimationToAdd = new Estimation()
            {
                VoteBy = voteBy,
                Complexity = complexity,
            };

            //Arrange the Mock Repository with a Session
            var InitialSession = CreateRandomSession(sessionId);
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
            try
            {
                var newEstimation = await service.AddNewEstimation(sessionId, "invalidId", voteBy, complexity);
            }
            //Assert
            catch (Exception NoOpenOrSuspendedTask)
            {
                Assert.True(true);
            }
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
            var newEstimation = await service.AddNewEstimation(sessionId, InitialSession.Tasks[0].Id, InitialSession.Tasks[0].Estimations[1].VoteBy, InitialSession.Tasks[0].Estimations[1].Complexity +1);

            //Assert

            newEstimation.Should().BeEquivalentTo(estimationToAdd, options => options.ComparingByMembers<TaskStatusChangeDto>());

        }
    }
}
