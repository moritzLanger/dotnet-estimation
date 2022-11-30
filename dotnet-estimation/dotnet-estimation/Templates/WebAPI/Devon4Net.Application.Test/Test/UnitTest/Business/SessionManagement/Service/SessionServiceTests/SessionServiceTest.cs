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
    public class SessionServiceTest : UnitTest
    {
        //private readonly Mock<ISessionRepository> repositoryStub = new();
        protected readonly Mock<ILiteDbRepository<Session>> repositoryStub = new Mock<ILiteDbRepository<Session>>();
        protected readonly Random rnd = new();
        protected readonly ITestOutputHelper output;

        public SessionServiceTest(ITestOutputHelper output)
        {
            this.output = output;
        }
        protected Session CreateRadomSessionWithOpenTaskAndEstimations(long sessionId)
        {
            IList<User> Users = new List<User>();
            Users.Add(CreateRandomUser());
            Users.Add(CreateRandomUser());
            Users.Add(CreateRandomUser());
            IList<Task> Tasks = new List<Task>();
            Tasks.Add(CreateOpenTask());
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
        protected Session CreateRandomSession(long sessionId)
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

        protected Session CreateExpiredSession(long sessionId)
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
    
        private Task CreateOpenTask()
        {
            
            return new()
            {
                Id = "132",
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Url = Guid.NewGuid().ToString(),
                Status = Status.Open,
                CreatedAt = DateTime.Now,
                Estimations = new List<Estimation>(),
                Result = new Result { AmountOfVotes = 0, ComplexityAverage = 0 },
            };

        }
    }

}