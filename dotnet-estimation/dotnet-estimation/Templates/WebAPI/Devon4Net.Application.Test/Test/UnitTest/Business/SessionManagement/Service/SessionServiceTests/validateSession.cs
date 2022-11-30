using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Devon4Net.Test.Test.UnitTest.Business.SessionManagement.Service.SessionServiceTests
{
    public class validateSession : SessionServiceTest
    {
        public validateSession(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public async void validateSession_withValidSession()
        {
            //Arrange
            var ExpectedSession = CreateRandomSession(2);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);

            var sessionService = new SessionService(repositoryStub.Object);

            //Act and Assert
            try
            {
                sessionService.validateSession(ExpectedSession, 2);
            }
            //Assert
            catch
            {
                Assert.True(false);
            }
        }
        [Fact]
        public async void validateSession_WithExpiredSession_ThrowsInvalidSessionException()
        {
            //Arrange
            var ExpectedSession = CreateExpiredSession(17);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);

            var sessionService = new SessionService(repositoryStub.Object);

            //Act and Assert
            Assert.Throws<InvalidSessionException>(() => sessionService.validateSession(ExpectedSession, 17));

        }
        [Fact]
        public async void validateSession_WithNull_Throws()
        {
            //Arrange
            var ExpectedSession = CreateExpiredSession(17);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);

            var sessionService = new SessionService(repositoryStub.Object);

            //Act and Assert
            Assert.Throws<NotFoundException>(() => sessionService.validateSession(null, 17));

        }

    }
}
