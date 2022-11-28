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
    public class AddUserToSession : SessionServiceTest
    {
        public AddUserToSession(ITestOutputHelper output) : base(output)
        {
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
    }
}

