﻿using formula1_tournament_api.Interfaces;
using formula1_tournament_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace formula1_tournament_api.Controllers
{
    [Route("api/season")]
    [ApiController]
    public class SeasonController : Controller
    {
        private ISeason _seasonService;
        private IUserSeason _userSeasonService;

        public SeasonController(ISeason seasonService, IUserSeason userSeasonService)
        {
            _seasonService = seasonService;
            _userSeasonService = userSeasonService;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var result = await _seasonService.GetAllSeasons();
            if (result.IsSuccess)
            {
                return Ok(result.Seasons);
            }
            return NotFound(result.ErrorMessage);
        }

        // not ready yet
        /*[HttpGet("user"), Authorize]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            List<UserSeason>
            var result = await _seasonService.GetAllSeasons();
            if (result.IsSuccess)
            {
                return Ok(result.Seasons);
            }
            return NotFound(result.ErrorMessage);
        }*/

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _seasonService.GetSeasonById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Season);
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Season season)
        {
            var result = await _seasonService.AddSeason(season);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Season season)
        {
            var result = await _seasonService.UpdateSeason(id, season);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _seasonService.DeleteSeason(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
