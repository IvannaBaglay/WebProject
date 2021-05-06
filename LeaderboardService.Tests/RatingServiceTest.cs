using System;
using Xunit;
using LeaderboardService.Services;
using LeaderboardService.Models;
using Moq;
using System.Collections.Generic;
using Castle.Core.Internal;
using LeaderboardService.Services.RatingService;
using LeaderboardService.Services.RatingData;
using System.Threading.Tasks;

namespace LeaderboardService.Tests
{
    public class RatingServiceTest
    {
        private RatingService Service;
        private Mock<IRatingData> Data;

        private List<NameScore> VALID_DATA;
        private List<UserRating> VALID_RATINGS;

        public RatingServiceTest()
        {
            Data = new Mock<IRatingData>();
            Service = new RatingService(Data.Object);

            VALID_DATA = new List<NameScore>
            {
                new NameScore{ Name="Alice",    Score=1 },
                new NameScore{ Name="Bob",      Score=1 },
                new NameScore{ Name="Tracy",    Score=1 }
            };

            VALID_RATINGS = new List<UserRating>
            {
                new UserRating{ Name="Alice",   Matches=1 },
                new UserRating{ Name="Bob",     Matches=1 },
                new UserRating{ Name="Tracy",   Matches=1 }
            };
        }

        [Fact]
        public async void Update_ShouldReturnFalse_IfDataIsNull()
        {
            Assert.False(await Service.Update(null));
        }

        [Fact]
        public async void Update_ShouldReturnFalse_IfDataIsEmpty()
        {
            Assert.False(await Service.Update(new List<NameScore>()));
        }

        [Fact]
        public async void Update_ShouldReturnFalse_IfDataSizeIsLessThanTwo()
        {
            Assert.False(await Service.Update(new List<NameScore> { new NameScore() }));
        }

        [Fact]
        public async void Update_ShouldCallCreateRatingsIfNotExist_IfDataIsValid()
        {
            await Service.Update(VALID_DATA);
            Data.Verify(
                data => data.CreateRatingsIfNotExist(
                    It.IsAny<List<string>>(),
                    It.IsAny<float>()),
                Times.Once
            );
        }

        [Fact]
        public async void Update_ShouldCallGetUserRatings_IfDataIsValid()
        {
            await Service.Update(VALID_DATA);
            Data.Verify(
                data => data.GetUserRatings(It.IsAny<List<string>>()),
                Times.Once
            );
        }

        [Fact]
        public async void Update_ShouldReturnFalse_IfUnableToGetUserRatings()
        {
            Data
                .Setup(data => data.GetUserRatings(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(null as List<UserRating>));
            Assert.False(await Service.Update(VALID_DATA));
        }

        [Fact]
        public async void Update_ShouldReturnFalse_IfGotInvalidUserRatings()
        {
            Data
                .Setup(data => data.GetUserRatings(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(new List<UserRating> { }));
            Assert.False(await Service.Update(VALID_DATA));
        }

        [Fact]
        public async void Update_ShouldReturnFalse_IfUnableToUpdateUserRatings()
        {
            Data
                .Setup(data => data.GetUserRatings(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(VALID_RATINGS));

            Data
                .Setup(data => data.UpdateUserRatings(It.IsAny<List<UserRating>>()))
                .Returns(Task.FromResult(false));

            Assert.False(await Service.Update(VALID_DATA));
        }

        [Fact]
        public async void Update_ShouldReturnTrue_IfUpdateUserRatingsReturnsTrue()
        {
            Data
                .Setup(data => data.GetUserRatings(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(VALID_RATINGS));

            Data
                .Setup(data => data.UpdateUserRatings(It.IsAny<List<UserRating>>()))
                .Returns(Task.FromResult(true));

            Assert.True(await Service.Update(VALID_DATA));
        }
    }
}
