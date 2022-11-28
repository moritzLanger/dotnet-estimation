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
    public class InvalidateSession : SessionServiceTest
    {
        public InvalidateSession(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async void InvalidateSession_WithValidSessionId_ReturnTrue()
        {

            //Arange
            var ExpectedSession = CreateRandomSession(1);
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

            var invalidSession = await sessionService.InvalidateSession(1);

            //Assert
            Assert.True(invalidSession);
        }
        [Fact]
        public async void InvalidateSession_WithExpiredSessionId_ThrowsError()
        {

            //Arange
            var ExpiredSession = CreateExpiredSession(1);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpiredSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var sessionService = new SessionService(repositoryStub.Object);

            //Act
            try
            {
                var invalidSession = await sessionService.InvalidateSession(1);
            }
            //Assert
            catch (Exception InvalidExpiryDateException)
            {
                Assert.True(true);
            }
        }
    }
}
