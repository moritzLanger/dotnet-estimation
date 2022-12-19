using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using ErrorOr;
using Moq;
using System;
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
            var expectedSession = CreateRandomSession(17);
            sessionRepositoryStub.Setup(repo => repo.Create(
                It.IsAny<Session>()
             ))
                .Returns(new LiteDB.BsonValue((long) 17));

            userRepositoryStub.Setup(repo => repo.Create(
                It.IsAny<User>()
              ))
                .Returns(new LiteDB.BsonValue());

            jwtHandler.Setup(jwtHandler => jwtHandler.CreateJwtToken(
                It.IsAny<System.Collections.Generic.List<System.Security.Claims.Claim>>()
                )).Returns("inviteToken");

            var session = new SessionDto()
            {
                ExpiresAt = DateTime.Now.AddMinutes(30),
                Username = "exampleUSername"
            };

            var sessionService = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);


            //Act
            var createdSession = await sessionService.CreateSession(session);

            //Assert
            Assert.IsType<ErrorOr<ResultCreateSessionDto>>(createdSession);
        }

        [Fact]
        public async void CreateSession_withExpiredDtoSession_ThrowsInvalidExpiryDateException()
        {
            //Arrange
            sessionRepositoryStub.Setup(repo => repo.Create(
                It.IsAny<Session>()
             ))
                .Returns(new LiteDB.BsonValue());

            var session = new SessionDto()
            {
                ExpiresAt = DateTime.Now,
            };

            var sessionService = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);
            var errorDescription = "Session is no longer valid or never existed";

            //Act
            var errorOrResult = await sessionService.CreateSession(session);

            //Assert
            Assert.IsType<ErrorOr<ResultCreateSessionDto>>(errorOrResult);

            Assert.Equal(errorDescription, errorOrResult.FirstError.Description);

            Assert.True(errorOrResult.IsError);
        }
    }
}
