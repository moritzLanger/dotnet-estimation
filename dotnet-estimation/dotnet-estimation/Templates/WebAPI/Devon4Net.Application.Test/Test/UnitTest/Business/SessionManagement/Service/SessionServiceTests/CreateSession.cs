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
    public class CreateSession : SessionServiceTest
    {
        public CreateSession(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public async void CreateSession_withValidDto_ReturnsCreatedSession()
        {
            //Arrange
            repositoryStub.Setup(repo => repo.Create(
                It.IsAny<Session>()
             ))
                .Returns(new LiteDB.BsonValue());

            var session = new SessionDto()
            {
                ExpiresAt = DateTime.Now.AddMinutes(30),
            };

            var sessionService = new SessionService(repositoryStub.Object);

            //Act
            var createdSession = await sessionService.CreateSession(session);

            //Assert

            Assert.IsType<LiteDB.BsonValue>(createdSession);
        }


    }
}
