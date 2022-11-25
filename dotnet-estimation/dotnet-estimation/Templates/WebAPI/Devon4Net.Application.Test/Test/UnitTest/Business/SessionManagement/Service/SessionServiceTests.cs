using System;
using Xunit;
using Moq;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Task = Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Task;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Infrastructure.Test;
using FluentAssertions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers
{
    public class SessionServiceTests : UnitTest
    {
        //private readonly Mock<ISessionRepository> repositoryStub = new();
        private readonly Mock<ILiteDbRepository<Session>> repositoryStub = new Mock<ILiteDbRepository<Session>>();
        private readonly Random rnd = new();
        private readonly ITestOutputHelper output;

        public SessionServiceTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        
/*        [Fact]
        public async void CreateSession_withValidDto_ReturnsCreatedSession()
        {
            //Arrange

            repositoryStub.Setup(repo => repo.Create(
                CreateRandomSession(1)
            ));
            var session = new SessionDto()
            {
                ExpiresAt = DateTime.Now.AddMinutes(30),
            };

            var sessionService = new SessionService(repositoryStub.Object);

            //Act
            var createdSession = await sessionService.CreateSession(session);

            //Assert

            Assert.Null(createdSession);
        }
*/
        


        [Fact]
        public async void GetSession_WithPopulatedRepo_ReturnsTheSearchedSession()
        {
            //Arrange
            var ExpectedSession = CreateRandomSession(2);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);

            var sessionService = new SessionService(repositoryStub.Object);

            //Act
            var actualSession2 = await sessionService.GetSession(2);

            //Assert
            actualSession2.Should().BeEquivalentTo(ExpectedSession, options => options.ComparingByMembers<Session>());
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
            var expectedTaskStatusChangeDtoResult = new List<TaskStatusChangeDto>() { new TaskStatusChangeDto () { Id = statusId, Status = status } };
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

        [Fact]
        public async void AddUserToSession_WithPopulatedRepo_ReturnsTrue()
        {
            //Arrange
            var newUser = new User { Id = "Vlad", Role = 0 };
            var InitialSession = CreateRandomSession(1);

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var Controller = new SessionService(repositoryStub.Object);
            var InitialUsers = InitialSession.Users;

            //Act
            var UserAdded = await Controller.AddUserToSession(1, newUser.Id, newUser.Role);
            var actualSession = await Controller.GetSession(1);
            var resultUsers = actualSession.Users;
            var LastUser = resultUsers.Last<User>();

            //Assert
            Assert.True(UserAdded);
            resultUsers.Last<User>().Should().BeEquivalentTo(newUser);
        }

        [Fact]
        public async void RemoveUserFromSession_WithValidSessionAndUser_ReturnsTrue()
        {
            //Arrange
            var InitialSession = CreateRandomSession(3);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            var session = new SessionService(repositoryStub.Object);
            var userToDelete = InitialSession.Users[1];

            //Act
            var removeUser = await session.RemoveUserFromSession(3, userToDelete.Id);

            //Assert
            Assert.True(removeUser);  
        }

        [Fact]
        public async void RemoveUserFromSession_WithValidSessionAndInvalidUser_ReturnsFalse()
        {
            //Arrange
            var InitialSession = CreateRandomSession(3);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            var session = new SessionService(repositoryStub.Object);

            //Act
            var removeUser = await session.RemoveUserFromSession(3, Guid.NewGuid().ToString());

            //Assert
            Assert.False(removeUser);
        }

        [Fact]
        public async void RemoveUserFromSession_WithInvalidSession_ReturnsFalse()
        {
            //Arrange
            var InitialSession = CreateExpiredSession(3);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            var session = new SessionService(repositoryStub.Object);

            //Act
            var removeUser = await session.RemoveUserFromSession(3, Guid.NewGuid().ToString());

            //Assert
            Assert.False(removeUser);
        }

        private Session CreateRandomSession(long sessionId)
        {
            IList<User> Users = new List<User>();
            Users.Add(CreateRandomUser());
            Users.Add(CreateRandomUser());
            Users.Add(CreateRandomUser());
            IList<Task> Tasks = new List<Task>();
            Tasks.Add(CreateRandomTask());
            Tasks.Add(CreateRandomTask());
            Tasks.Add(CreateRandomTask());


            //Mockup estimations by each of the 3 random created users
            Estimation estimation1 = new Estimation { VoteBy = Users[0].Id, Complexity = rnd.Next(13) };
            Estimation estimation2 = new Estimation { VoteBy = Users[1].Id, Complexity = rnd.Next(13) };
            Estimation estimation3 = new Estimation { VoteBy = Users[2].Id, Complexity = rnd.Next(13) };

            Tasks[0].Estimations.Add(estimation1);
            Tasks[0].Estimations.Add(estimation2);
            Tasks[0].Estimations.Add(estimation3);


            //TODO Add to Task.Estimations, one or two Voteby strings of already created Users + random int Complexity vote
            return new()
            {
                Id = sessionId,
                InviteToken = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.Now.AddDays(1),
                Tasks = Tasks,
                Users = Users,
            };
        }

        private Session CreateExpiredSession(long sessionId)
        {
            IList<User> Users = new List<User>();
            Users.Add(CreateRandomUser());
            Users.Add(CreateRandomUser());
            Users.Add(CreateRandomUser());
            IList<Task> Tasks = new List<Task>();
            Tasks.Add(CreateRandomTask());
            Tasks.Add(CreateRandomTask());
            Tasks.Add(CreateRandomTask());


            //Mockup estimations by each of the 3 random created users
            Estimation estimation1 = new Estimation { VoteBy = Users[0].Id, Complexity = rnd.Next(13) };
            Estimation estimation2 = new Estimation { VoteBy = Users[1].Id, Complexity = rnd.Next(13) };
            Estimation estimation3 = new Estimation { VoteBy = Users[2].Id, Complexity = rnd.Next(13) };

            Tasks[0].Estimations.Add(estimation1);
            Tasks[0].Estimations.Add(estimation2);
            Tasks[0].Estimations.Add(estimation3);


            //TODO Add to Task.Estimations, one or two Voteby strings of already created Users + random int Complexity vote
            return new()
            {
                Id = sessionId,
                InviteToken = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.Now,
                Tasks = Tasks,
                Users = Users,
            };
        }

        private Task CreateRandomTask()
        {
            var StatusValues = Enum.GetValues(typeof(Status));
            var Status = (Status)StatusValues.GetValue(rnd.Next(StatusValues.Length));
            return new()
            {
                Id = rnd.Next().ToString(),
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Url = Guid.NewGuid().ToString(),
                Status = Status,
                CreatedAt = DateTime.Now,
                Estimations = new List<Estimation>(),
                Result = new Result { AmountOfVotes = 0, ComplexityAverage = 0 },
            };
        }

        private User CreateRandomUser()
        {
            var RoleValues = Enum.GetValues(typeof(Role));
            var role = (Role)RoleValues.GetValue(rnd.Next(RoleValues.Length));

            return new()
            {
                Id = Guid.NewGuid().ToString(),
                Role = role,
            };
        }
    }
}