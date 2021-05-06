using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LeaderboardService.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LeaderboardService.Models;

namespace LeaderboardService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        ILeaderboardService Service;

        public ScoreController(ILeaderboardService service)
        {
            Service = service;
        }

        // GET: api/Score
        [Authorize]
        [HttpGet("Board/{rank}")]
        public async Task<IEnumerable<UserScoreView>> Get(int rank)
        {
            return await Service.GetTopUsers(rank);
        }

        // GET: api/Score/Vasya
        [Authorize]
        [HttpGet("{login}")]
        public async Task<ActionResult<UserScoreView>> Get(string login)
        {
            var (isSucceed, userScore) = await Service.GetScore(login);

            if (isSucceed)
            {
                return Ok(userScore);
            }

            return BadRequest("User does not exist");
        }

        // POST: api/Score
        [Authorize]
        [HttpPost]
        public async Task Post([FromBody] int value)
        {
            string login = User.Claims.Where(claim => claim.Type == ClaimTypes.Name).FirstOrDefault()?.Value;

            if (login != null)
            {
                await Service.AddScore(login, value);
            }
        }
    }
}
