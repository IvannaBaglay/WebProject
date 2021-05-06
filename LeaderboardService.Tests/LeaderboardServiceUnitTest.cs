using System;
using Xunit;
using LeaderboardService.Services;
using LeaderboardService.Models;
using Moq;
using System.Collections.Generic;
using Castle.Core.Internal;

namespace LeaderboardService.Tests
{
    public class LeaderboardServiceUnitTest
    {
        public static object[][] Leaderboard = new object[][]
        {
            new object[] {
                new List<UserScore> {
                    new UserScore { Rank = 1 },
                    new UserScore { Rank = 2 },
                    new UserScore { Rank = 3 },
                },
                1
            },
            new object[] {
                new List<UserScore> {
                    new UserScore { Rank = 11 },
                    new UserScore { Rank = 12 },
                    new UserScore { Rank = 13 },
                },
                2
            },
            new object[] {
                new List<UserScore> {
                    new UserScore { Rank = 21 },
                    new UserScore { Rank = 22 },
                    new UserScore { Rank = 23 },
                },
                3
            }
        };

        Mock<ILeaderboardData> DataMock;
        ILeaderboardService Service;

        public LeaderboardServiceUnitTest()
        {
            DataMock = new Mock<ILeaderboardData>();

            Service = new Leaderboard(DataMock.Object);
        }

        [Theory]
        [InlineData("test_login1", 0)]
        [InlineData("test_login2", 11)]
        [InlineData("test_login3", 999)]
        public async void TestAddScore_Expect_CallDataAdd(string login, int score)
        {
            DataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(null as UserScore);

            await Service.AddScore(login, score);

            DataMock.Verify(data => data.AddUserScore(login, score), Times.Once);
        }

        [Theory]
        [InlineData("test_login1", 0)]
        [InlineData("test_login2", 11)]
        [InlineData("test_login3", 999)]
        public async void TestAddScore_Expect_CallDataChange(string login, int score)
        {
            var prevUserScore = new UserScore { Score = score - 1 };
            DataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(prevUserScore);

            await Service.AddScore(login, score);

            DataMock.Verify(data => data.ChangeUserScore(prevUserScore, score), Times.Once);
        }

        [Theory]
        [InlineData("test_login1", 0)]
        [InlineData("test_login2", 11)]
        [InlineData("test_login3", 999)]
        public async void TestAddScore_Expect_NeverCallData(string login, int score)
        {
            var prevUserScore = new UserScore { Score = score + 1};

            DataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(prevUserScore);

            await Service.AddScore(login, score);

            DataMock.Verify(data => data.ChangeUserScore(prevUserScore, score), Times.Never);
            DataMock.Verify(data => data.AddUserScore(login, score), Times.Never);
        }

        [Theory]
        [InlineData("test_login1", 0, 1)]
        [InlineData("test_login2", 11, 2)]
        [InlineData("test_login3", 999, 3)]
        public async void TestGetScore_Expect_True(string login, int score, int rank)
        {
            DataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(new UserScore { Login = login, Score = score, Rank = rank});

            var (isSucceed, scoreView) = await Service.GetScore(login);

            Assert.True(isSucceed);
            Assert.True(scoreView.Score == score);
            Assert.True(scoreView.Rank == rank);
        }

        [Fact]
        public async void TestGetScore_Expect_False()
        {
            DataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(null as UserScore);

            var (isSucceed, scoreView) = await Service.GetScore("test_login");

            Assert.False(isSucceed);
            Assert.True(scoreView == null);
        }

        [Theory]
        [MemberData(nameof(Leaderboard))]
        public async void TestGetTopUsers_Expect_True(IEnumerable<UserScore> userScores, int rank)
        {
            DataMock.Setup(context => context.GetTopUsers(It.IsAny<int>()))
                .ReturnsAsync(userScores);

            var topUsers = await Service.GetTopUsers(rank);

            Assert.False(topUsers.IsNullOrEmpty());
            foreach (UserScoreView userScore in topUsers)
            {
                Assert.True(userScore.Rank / 10 == rank - 1);

            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void TestGetTopUsers_Expect_Empty(int rank)
        {
            DataMock.Setup(context => context.GetTopUsers(It.IsAny<int>()))
                .ReturnsAsync(new List<UserScore>());

            var topUsers = await Service.GetTopUsers(rank);

            Assert.True(topUsers.IsNullOrEmpty());
        }
    }
}
