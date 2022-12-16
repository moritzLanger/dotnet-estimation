using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using ErrorOr;
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
        public async void validateSession_WithValidSession_ThrowsInvalidSessionException()
        {
            //Arrange
            var ExpectedSession = CreateRandomSession(17);
            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);

            var sessionService = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            //Act
            var isValid = sessionService.validateSession(ExpectedSession, 17);
            
            //Assert
            Assert.True(isValid.Value);
        }
        [Fact]
        public async void validateSession_WithExpiredSession_ThrowsInvalidSessionException()
        {
            //Arrange
            var ExpectedSession = CreateExpiredSession(17);
            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                 It.IsAny<LiteDB.BsonExpression>()
             ))
                 .Returns(ExpectedSession);

            var sessionService = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            var isValid = sessionService.validateSession(ExpectedSession, 17);

            //Act
            var errorDescription = "Session with the SessionId: 17 is no longer valid";
            //Assert
            Assert.IsType<ErrorOr<bool>>(isValid);
            Assert.Equal(errorDescription, isValid.FirstError.Description);
            
        }
        [Fact]
        public async void validateSession_WithNullSession_ReturnsError()
        {
            //Arrange
            var ExpectedSession = CreateExpiredSession(17);
            sessionRepositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);

            var sessionService = new SessionService(sessionRepositoryStub.Object, userRepositoryStub.Object, jwtHandler.Object);

            var errorDescription = "no session with the sessionId: 77";

            //Act 

            var isValid = sessionService.validateSession(null, 77);

            //Assert
            Assert.IsType<ErrorOr<bool>>(isValid);
            Assert.Equal(errorDescription, isValid.FirstError.Description);
        }
    }
}
