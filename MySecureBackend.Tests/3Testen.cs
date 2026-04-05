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

            // Zorg dat de mock altijd dezelfde ingelogde gebruiker teruggeeft.
            authServiceMock.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(CurrentUserId);

            highscoresController = new HighscoresController(highscoreRepoMock.Object, authServiceMock.Object);
            userAvatarsController = new UserAvatarsController(userAvatarRepoMock.Object, authServiceMock.Object);
        }

        [TestMethod]
        // GET /highscores moet een empty terug geven, als er geen scores zijn.
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
        // POST /highscores uittesten.
        public async Task HighscoreController_AddHighscore_ReturnsCreatedHighscore()
        {
            var newHighscore = new Highscore
            {
                Score = "100"
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
        // DELETE /highscores verwijdert score
        public async Task HighscoreController_DeleteHighscore_ReturnsOk()
        {
            highscoreRepoMock.Setup(r => r.DeleteAsync(CurrentUserId))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            var result = await highscoresController.Delete();
            var okResult = result as OkResult;

            Assert.IsNotNull(okResult);
            highscoreRepoMock.Verify();
        }
    }
}