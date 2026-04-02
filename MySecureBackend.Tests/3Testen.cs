using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySecureBackend.WebApi.Controllers;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySecureBackend.Tests
{
    [TestClass]
    public sealed class ControllerTests
    {
        private Mock<IHighscoreRepository> highscoreRepoMock;
        private Mock<IUserAvatarRepository> userAvatarRepoMock;
        private Mock<IAuthenticationService> authServiceMock;
        private HighscoresController highscoresController;
        private UserAvatarsController userAvatarsController;
        private const string CurrentUserId = "test-user";

        [TestInitialize]
        public void Setup()
        {
            highscoreRepoMock = new Mock<IHighscoreRepository>();
            userAvatarRepoMock = new Mock<IUserAvatarRepository>();
            authServiceMock = new Mock<IAuthenticationService>();

            // Zorg dat de mock altijd dezelfde ingelogde gebruiker teruggeeft
            authServiceMock.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(CurrentUserId);

            highscoresController = new HighscoresController(highscoreRepoMock.Object, authServiceMock.Object);
            userAvatarsController = new UserAvatarsController(userAvatarRepoMock.Object, authServiceMock.Object);
        }

        [TestMethod]
        public async Task HighscoreController_GetByGame_ReturnsNotFound_WhenHighscoreDoesNotExist()
        {
            string gameName = "game123";

            highscoreRepoMock.Setup(r => r.SelectAsync(CurrentUserId, gameName))
                             .ReturnsAsync((Highscore?)null);

            var result = await highscoresController.GetByGameAsync(gameName);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task UserAvatarsController_GetAsync_ReturnsNotFound_WhenAvatarDoesNotExist()
        {
            userAvatarRepoMock.Setup(r => r.SelectAsync(CurrentUserId))
                              .ReturnsAsync((UserAvatar?)null);

            var result = await userAvatarsController.GetAsync();

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task HighscoreController_AddHighscore_ReturnsCreatedHighscore()
        {
            var newHighscore = new Highscore
            {
                GameName = "game789",
                Score = 100
                // UserId wordt automatisch gezet door controller
            };

            highscoreRepoMock.Setup(r => r.InsertAsync(It.IsAny<Highscore>()))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            var result = await highscoresController.AddAsync(newHighscore);
            var createdResult = result.Result as CreatedAtRouteResult;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("GetHighscoreByGame", createdResult.RouteName);

            // Check dat de UserId correct werd gezet
            var returnedHighscore = createdResult.Value as Highscore;
            Assert.IsNotNull(returnedHighscore);
            Assert.AreEqual(CurrentUserId, returnedHighscore.UserId);
            Assert.AreEqual(newHighscore.GameName, returnedHighscore.GameName);
            Assert.AreEqual(newHighscore.Score, returnedHighscore.Score);

            highscoreRepoMock.Verify();
        }
    }
}