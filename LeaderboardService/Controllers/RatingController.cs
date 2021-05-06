using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderboardService.Models;
using LeaderboardService.Services.RatingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaderboardService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : Controller
    {
        IRatingService Service;

        public RatingController(IRatingService service)
        {
            Service = service;
        }

        [Authorize]
        [HttpGet("Board/{rank}")]
        public async Task<IEnumerable<UserRatingView>> Get(int rank)
        {
            return await Service.GetTopUsers(rank);
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UserMatchData data)
        {
            if (data.MatchResults == null || data.MatchResults.Count < 2)
            {
                return BadRequest("Invalid data");
            }

            bool isUpdated = await Service.Update(data.MatchResults);
            if (!isUpdated)
            {
                return BadRequest("Unable to update rating");
            }

            return Ok();
        }

        [Authorize]
        [HttpGet("one")]
        public async Task<IActionResult> GetRating([FromBody] UserNameModel model)
        {
            if (model.Name.Length == 0)
            {
                return BadRequest("Invalid data");
            }

            UserRatingView rating = await Service.GetRating(model.Name);
            if (rating == null)
            {
                return BadRequest("No data found");
            }
            return Ok(rating);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetRating()
        {
            UserRatingView rating = await Service.GetRating(User.Identity.Name);
            if (rating == null)
            {
                return BadRequest("No data found");
            }
            return Ok(rating);
        }

        [Authorize]
        [HttpGet("many")]
        public async Task<IActionResult> GetRating([FromBody] UserNamesModel model)
        {
            if (model.Names.Count == 0)
            {
                return BadRequest("Invalid data");
            }

            List<UserRating> ratings = await Service.GetRating(model.Names);
            if (ratings == null)
            {
                return BadRequest("No data found");
            }
            return Ok(ratings);
        }

        [Authorize]
        [HttpGet("view")]
        public async Task<IActionResult> View()
        {
            return Ok(await Service.View());
        }
    }
}
