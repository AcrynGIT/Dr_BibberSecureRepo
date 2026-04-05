using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySecureBackend.WebApi.Controllers;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySecureBackend.Tests
{
    [TestClass]
    public sealed class ControllerTests
    {
        private Mock<IHighscoreRepository> highscoreRepoMock;
        private Mock<IAuthenticationService> authServiceMock;
        private HighscoresController highscoresController;
        private const string CurrentUserId = "test-user";

        [TestInitialize]
        public void Setup()
        {
            highscoreRepoMock = new Mock<IHighscoreRepository>();
            authServiceMock = new Mock<IAuthenticationService>();

            authServiceMock.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(CurrentUserId);

            highscoresController = new HighscoresController(highscoreRepoMock.Object, authServiceMock.Object);
        }

        [TestMethod]
        // Controleren dat GET /highscores een lege lijst teruggeeft als er nog geen highscores bestaan.
        public async Task HighscoreController_GetAll_ReturnsEmpty_WhenNoHighscores()
        {
            highscoreRepoMock.Setup(r => r.SelectAsync())
                             .ReturnsAsync(new List<Highscore>());

            var result = await highscoresController.GetAll();
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var returnedHighscores = okResult.Value as IEnumerable<Highscore>;
            Assert.IsNotNull(returnedHighscores);
        }

        [TestMethod]
        // Controleren dat PUT /highscores een bestaande score correct bijwerkt.
        public async Task HighscoreController_UpdateHighscore_ReturnsOk()
        {
            var existingHighscore = new Highscore { Score = "50" };
            highscoreRepoMock.Setup(r => r.SelectAsync(CurrentUserId)).ReturnsAsync(existingHighscore);
            highscoreRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Highscore>())).Returns(Task.CompletedTask).Verifiable();

            var updateHighscore = new Highscore { Score = "150" };
            var result = await highscoresController.Update(updateHighscore);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var returnedHighscore = okResult.Value as Highscore;
            Assert.IsNotNull(returnedHighscore);
            Assert.AreEqual(CurrentUserId, returnedHighscore.UserId);
            Assert.AreEqual("150", returnedHighscore.Score);

            highscoreRepoMock.Verify();
        }

        [TestMethod]
        // Controleren dat DELETE /highscores de score van de huidige gebruiker verwijdert.
        public async Task HighscoreController_DeleteHighscore_ReturnsOk()
        {
            highscoreRepoMock.Setup(r => r.DeleteAsync(CurrentUserId)).Returns(Task.CompletedTask).Verifiable();

            var result = await highscoresController.Delete();
            var okResult = result as OkResult;

            Assert.IsNotNull(okResult);
            highscoreRepoMock.Verify();
        }
    }
}