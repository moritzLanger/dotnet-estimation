using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using ErrorOr;
using Moq;
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
            Assert.True(invalidSession.Value);
        }

        [Fact]
        public async void InvalidateSession_WithExpiredSession_ReturnsError()
        {
            //Arrange 

            repositoryStub.Setup(repo => repo.GetFirstOrDefault
            (It.IsAny<LiteDB.BsonExpression>())
            )
                .Returns<Session>(null);



            var service = new SessionService(repositoryStub.Object);

            //Act
            var ErrorOrEstimation = await service.InvalidateSession(77);

            var errorDescription = "no session with the sessionId: 77";
            //Act and Assert
            Assert.IsType<ErrorOr<bool>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);

        }
        [Fact]
        public async void InvalidateSession_WiThExpiredSession_ReturnsEstimation()
        {
            //Arrange 
            var InitialSession = CreateExpiredSession(17);


            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);


            var service = new SessionService(repositoryStub.Object);
            var errorDescription = "Session with the SessionId: 17 is no longer valid";


            //Act
            var ErrorOrEstimation = await service.InvalidateSession(17);

            //Assert

            Assert.IsType<ErrorOr<bool>>(ErrorOrEstimation);
            Assert.Equal(errorDescription, ErrorOrEstimation.FirstError.Description);

        }
    }
}
