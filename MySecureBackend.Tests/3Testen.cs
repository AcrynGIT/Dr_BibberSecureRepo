using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MySecureBackend.WebApi.Controllers;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
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
        private HighscoresController highscoresController;
        private UserAvatarsController userAvatarsController;

        [TestInitialize]
        public void Setup()
        {
            highscoreRepoMock = new Mock<IHighscoreRepository>();
            userAvatarRepoMock = new Mock<IUserAvatarRepository>();

            highscoresController = new HighscoresController(highscoreRepoMock.Object);
            userAvatarsController = new UserAvatarsController(userAvatarRepoMock.Object);
        }

        [TestMethod]
        // Controleert dat de API 404 terugstuurt als een highscore niet bestaat.
        public async Task HighscoreController_GetById_ReturnsNotFound_WhenHighscoreDoesNotExist()
        {
            string userId = "user123";
            string gameName = "game123";

            highscoreRepoMock.Setup(r => r.SelectAsync(userId, gameName))
                             .ReturnsAsync((Highscore?)null);
            
            var result = await highscoresController.GetByIdAsync(userId, gameName);
          
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        //
        public async Task UserAvatarsController_GetById_ReturnsNotFound_WhenAvatarDoesNotExist()
        {
            string userId = "user456";

            userAvatarRepoMock.Setup(r => r.SelectAsync(userId))
                              .ReturnsAsync((UserAvatar?)null);
            
            var result = await userAvatarsController.GetByIdAsync(userId);
            
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task HighscoreController_AddHighscore_ReturnsCreatedHighscore()
        {
            var newHighscore = new Highscore
            {
                UserId = "user789",
                GameName = "game789",
                Score = 100
            };

            highscoreRepoMock.Setup(r => r.InsertAsync(newHighscore))
                             .Returns(Task.CompletedTask);

            var result = await highscoresController.AddAsync(newHighscore);
            var createdResult = result.Result as CreatedAtRouteResult;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("GetHighscoreById", createdResult.RouteName);
            Assert.AreEqual(newHighscore, createdResult.Value);
        }
    }
}