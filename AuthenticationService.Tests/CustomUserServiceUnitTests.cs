using System;
using Xunit;
using AuthenticationService.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using AuthenticationService.Models;
using AuthenticationService.Services;
using System.Security.Claims;
using AuthenticationService.Services.UserService;
using System.Threading.Tasks;
using Xunit.Abstractions;
using System.Text;

namespace AuthenticationService.Tests
{
    public class CustomUserServiceUnitTests
    {
        private const string ValidAccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZmVyMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6InVzZXIiLCJuYmYiOjE1OTA0MTUwNDksImV4cCI6MTU5MDQxNTY0OSwiaXNzIjoiSUFicm9zaW1vdlRhbmtzQXV0aGVudGljYXRpb25TZXJ2aWNlIiwiYXVkIjoiSUFicm9zaW1vdlRhbmtzQ2xpZW50In0.jOrhSzEZs4NVX8p5VagYHRabnJK0Luv5g9b2J7LF62c";

        private readonly ITestOutputHelper output;

        /*** Test data ***/
        public static object[][] ValidClaims = new object[][]
        {
            new object[] {
                new List<Claim> {
                    new Claim(ClaimTypes.Name, "fer1"),
                    new Claim(ClaimTypes.Role, "user"),
                    new Claim("access_token", ValidAccessToken)
                }
            },
            new object[] {
                new List<Claim> {
                    new Claim(ClaimTypes.Name, "fer2"),
                    new Claim(ClaimTypes.Role, "user"),
                    new Claim("access_token", ValidAccessToken)
                }
            },
            new object[] {
                new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim("access_token", ValidAccessToken)
                }
            }
        };

        public static object[][] InvalidClaims = new object[][]
        {
            new object[] {
                new List<Claim> { new Claim(ClaimTypes.Name, "fer1_invalid") }
            },
            new object[] {
                new List<Claim> { new Claim(ClaimTypes.Role, "user") }
            },
            new object[] {
                new List<Claim> {}
            }
        };

        public static object[][] ValidClaimsWithNewUser = new object[][]
        {
            new object[] {
                new List<Claim> {
                    new Claim(ClaimTypes.Name, "fer1"),
                    new Claim(ClaimTypes.Role, "user"),
                    new Claim("access_token", ValidAccessToken)
                },
                new User{login = "new1", password = Encoding.ASCII.GetBytes("pass"), role = "user"}
            },
            new object[] {
                new List<Claim> {
                    new Claim(ClaimTypes.Name, "fer2"),
                    new Claim(ClaimTypes.Role, "user"),
                    new Claim("access_token", ValidAccessToken)
                },
                new User{login = "new2", password = Encoding.ASCII.GetBytes("pass"), role = "admin"}
            },
            new object[] {
                new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim("access_token", ValidAccessToken)
                },
                new User{login = "new3", password = Encoding.ASCII.GetBytes("pass"), role = "user"}
            }
        };

        public static object[][] InvalidClaimsWithNewUser = new object[][]
        {
            new object[] {
                new List<Claim> { new Claim(ClaimTypes.Name, "invalid_login") },
                new User{login = "fer1", password = Encoding.ASCII.GetBytes("pass"), role = "user"}
            },
            new object[] {
                new List<Claim> { new Claim(ClaimTypes.Role, "user") },
                new User{login = "fer1", password = Encoding.ASCII.GetBytes("pass"), role = "user"}
            },
            new object[] {
                new List<Claim> {},
                new User{login = "fer1", password = Encoding.ASCII.GetBytes("pass"), role = "user"}
            },
            new object[] {
                new List<Claim> {
                    new Claim(ClaimTypes.Name, "fer2"),
                    new Claim(ClaimTypes.Role, "user"),
                    new Claim("access_token", ValidAccessToken)
                },
                new User{login = "fer1", password = Encoding.ASCII.GetBytes("pass"), role = "admin"}
            },
            new object[] {
                new List<Claim> { new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim("access_token", ValidAccessToken)
                },
                new User{login = "fer2", password = Encoding.ASCII.GetBytes("pass"), role = "user"}
            }
        };

        Mock<IUserData> IUserDataMock;
        CustomUserService UserService;

        public CustomUserServiceUnitTests(ITestOutputHelper helper)
        {
            output = helper;

            IUserDataMock = new Mock<IUserData>();
            UserService = new CustomUserService(IUserDataMock.Object);
        }


        /*** Login ***/
        [Theory]
        [InlineData("fer1", "few")]
        [InlineData("fer2", "few")]
        [InlineData("fer3", "few")]
        public async void TestLogin_Expect_True(string login, string pass)
        {
            IUserDataMock.Setup(context => context.IsUserExist(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            IUserDataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(new User { login = login, password = Encoding.ASCII.GetBytes(pass), role = "user" });

            var (isSucceed, token) = await UserService.Login(login, pass);

            Assert.True(isSucceed);
        }

        [Theory]
        [InlineData("fer143", "few")]
        [InlineData("fer2", "fe")]
        [InlineData("", "")]
        public async void TestLogin_Expect_False(string login, string pass)
        {
            IUserDataMock.Setup(context => context.IsUserExist(It.IsAny<string>()))
                .ReturnsAsync(false);
            IUserDataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(null as User);

            var (isSucceed, token) = await UserService.Login(login, pass);

            Assert.False(isSucceed);
        }


        /*** Refresh ***/
        [Theory]
        [MemberData(nameof(ValidClaims))]
        public async void TestRefresh_Expect_True(IEnumerable<Claim> claims)
        {
            IUserService service = new CustomUserService(new Mock<IUserData>().Object);

            var (isSucceed, token) = await service.Refresh(claims);

            Assert.True(isSucceed);
        }

        [Theory]
        [MemberData(nameof(InvalidClaims))]
        public async void TestRefresh_Expect_False(IEnumerable<Claim> claims)
        {
            IUserService service = new CustomUserService(new Mock<IUserData>().Object);

            var (isSucceed, token) = await service.Refresh(claims);

            Assert.False(isSucceed);
        }


        /*** CloseSession ***/
        [Theory]
        [MemberData(nameof(ValidClaims))]
        public async void TestCloseSession_Expect_True(IEnumerable<Claim> claims)
        {
            IUserService service = new CustomUserService(new Mock<IUserData>().Object);

            bool isSucceed = await service.CloseSession(claims);

            Assert.True(isSucceed);
        }


        /*** Register ***/
        [Theory]
        [InlineData("new_fer1", "few")]
        [InlineData("new_fer2", "few")]
        [InlineData("new_fer3", "few")]
        public async void TestRegister_Expect_True(string login, string pass)
        {
            IUserDataMock.Setup(context => context.IsUserExist(It.IsAny<string>()))
                .ReturnsAsync(false);
            IUserDataMock.Setup(context => context.AddUser(It.IsAny<User>()))
                .ReturnsAsync(true);

            var (isSucceed, token) = await UserService.Register(login, pass);

            Assert.True(isSucceed);
        }

        [Theory]
        [InlineData("fer1", "few")]
        [InlineData("fer2", "few")]
        [InlineData("fer3", "few")]
        public async void TestRegister_Expect_False(string login, string pass)
        {
            IUserDataMock.Setup(context => context.IsUserExist(It.IsAny<string>()))
                .ReturnsAsync(true);
            IUserDataMock.Setup(context => context.AddUser(It.IsAny<User>()))
                .ReturnsAsync(false);

            var (isSucceed, token) = await UserService.Register(login, pass);

            Assert.False(isSucceed);
        }


        /*** Change ***/
        [Theory]
        [MemberData(nameof(ValidClaimsWithNewUser))]
        public async void TestChange_Expect_True(IEnumerable<Claim> claims, User user)
        {
            IUserDataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(new User{ login = "login"});
            IUserDataMock.Setup(context => context.IsUserExist(It.IsAny<string>()))
                .ReturnsAsync(false);
            IUserDataMock.Setup(context => context.ChangeUser(It.IsAny<User>(), It.IsAny<User>()))
                .ReturnsAsync(user);

            var (isSucceed, token) = await UserService.ChangeUser(claims, user);

            Assert.True(isSucceed);
        }

        [Theory]
        [MemberData(nameof(InvalidClaimsWithNewUser))]
        public async void TestChange_Expect_False(IEnumerable<Claim> claims, User user)
        {
            IUserDataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(null as User);
            IUserDataMock.Setup(context => context.IsUserExist(It.IsAny<string>()))
                .ReturnsAsync(true);
            IUserDataMock.Setup(context => context.ChangeUser(It.IsAny<User>(), It.IsAny<User>()))
                .ReturnsAsync(null as User);

            var (isSucceed, token) = await UserService.ChangeUser(claims, user);

            Assert.False(isSucceed);
        }


        /*** Delete ***/
        [Theory]
        [MemberData(nameof(ValidClaims))]
        public async void TestDelete_Expect_True(IEnumerable<Claim> claims)
        {
            IUserDataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(new User {login = "login"});

            IUserDataMock.Setup(context => context.DeleteUser(It.IsAny<User>()))
                .ReturnsAsync(new User {login = "login"});

            bool isSucceed = await UserService.DeleteUser(claims);

            Assert.True(isSucceed);
        }

        [Theory]
        [MemberData(nameof(InvalidClaims))]
        public async void TestDelete_Expect_False(IEnumerable<Claim> claims)
        {
            IUserDataMock.Setup(context => context.GetUser(It.IsAny<string>()))
                .ReturnsAsync(null as User);

            IUserDataMock.Setup(context => context.DeleteUser(It.IsAny<User>()))
                .ReturnsAsync(null as User);

            bool isSucceed = await UserService.DeleteUser(claims);

            Assert.False(isSucceed);
        }
    }
}
