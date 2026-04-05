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

            authServiceMock.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(CurrentUserId);

            highscoresController = new HighscoresController(highscoreRepoMock.Object, authServiceMock.Object);
            userAvatarsController = new UserAvatarsController(userAvatarRepoMock.Object, authServiceMock.Object);
        }

        [TestMethod]
        // controleren dat de API een 404 Not Found teruggeeft als er geen highscore bestaat voor de gebruiker.
        public async Task HighscoreController_GetAll_ReturnsEmpty_WhenNoHighscores()
        {
            highscoreRepoMock.Setup(r => r.SelectAsync())
                             .ReturnsAsync(new List<Highscore>());

            var result = await highscoresController.GetAll();
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var returnedHighscores = okResult.Value as IEnumerable<Highscore>;
            Assert.IsNotNull(returnedHighscores);
            Assert.AreEqual(0, ((List<Highscore>)returnedHighscores).Count);
        }

        [TestMethod]
        // controleren dat het toevoegen van een highscore werkt en dat de controller een OkObjectResult teruggeeft
        public async Task HighscoreController_AddHighscore_ReturnsCreatedHighscore()
        {
            var newHighscore = new Highscore
            {
                Score = 100
            };

            highscoreRepoMock.Setup(r => r.InsertAsync(It.IsAny<Highscore>()))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            var result = await highscoresController.Add(newHighscore);
            var createdResult = result.Result as CreatedResult;

            Assert.IsNotNull(createdResult);

            var returnedHighscore = createdResult.Value as Highscore;
            Assert.IsNotNull(returnedHighscore);
            Assert.AreEqual(CurrentUserId, returnedHighscore.UserId);
            Assert.AreEqual(newHighscore.Score, returnedHighscore.Score);

            highscoreRepoMock.Verify();
        }

        [TestMethod]
        // controleren dat het updaten van een highscore werkt
        public async Task HighscoreController_UpdateHighscore_ReturnsOk()
        {
            var existingHighscore = new Highscore
            {
                Score = 50
            };

            highscoreRepoMock.Setup(r => r.SelectAsync(CurrentUserId))
                             .ReturnsAsync(existingHighscore);

            highscoreRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Highscore>()))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            var updateHighscore = new Highscore
            {
                Score = 150
            };

            var result = await highscoresController.Update(updateHighscore);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);

            var returnedHighscore = okResult.Value as Highscore;
            Assert.IsNotNull(returnedHighscore);
            Assert.AreEqual(CurrentUserId, returnedHighscore.UserId);
            Assert.AreEqual(150, returnedHighscore.Score);

            highscoreRepoMock.Verify();
        }
    }
}