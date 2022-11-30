using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Devon4Net.Test.Test.UnitTest.Business.SessionManagement.Service.SessionServiceTests
{
    public class GetSession : SessionServiceTest
    {
        public GetSession(ITestOutputHelper output) : base(output)
        {
        }


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
    }
}
