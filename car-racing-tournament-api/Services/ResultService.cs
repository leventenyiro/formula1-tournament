﻿using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class ResultService : Interfaces.IResult
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IConfiguration _configuration;

        public ResultService(CarRacingTournamentDbContext carRacingTournamentDbContext, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, Result? Result, string? ErrorMessage)> GetResultById(Guid id)
        {
            var result = await _carRacingTournamentDbContext.Results
                .Where(e => e.Id == id)
                .Include(x => x.Driver)
                .Include(x => x.Team)
                .Select(x => new Result
                {
                    Id = x.Id,
                    Position = x.Position,
                    Points = x.Points,
                    Driver = new Driver
                    {
                        Id = x.Driver.Id,
                        Name = x.Driver.Name,
                        RealName = x.Driver.RealName,

                    },
                    Team = new Team
                    {
                        Id = x.Team.Id,
                        Name = x.Team.Name,
                        Color = x.Team.Color
                    }
                }).FirstOrDefaultAsync();
            if (result == null)
                return (false, null, _configuration["ErrorMessages:ResultNotFound"]);
            
            return (true, result, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateResult(Result result, ResultDto resultDto, Race race, Driver driver, Team team)
        {
            if (race.SeasonId != team.SeasonId)
                return (false, _configuration["ErrorMessages:RaceTeamNotSameSeason"]);

            if (race.SeasonId != driver.SeasonId)
                return (false, _configuration["ErrorMessages:RaceDriverNotSameSeason"]);

            if (resultDto.Position <= 0)
                return (false, _configuration["ErrorMessages:ResultPosition"]);

            if (resultDto.Points < 0)
                return (false, _configuration["ErrorMessages:ResultPoints"]);

            result.Position = resultDto.Position;
            result.Points = resultDto.Points;
            result.DriverId = resultDto.DriverId;
            result.TeamId = resultDto.TeamId;
            _carRacingTournamentDbContext.Results.Update(result);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteResult(Result result)
        {
            _carRacingTournamentDbContext.Results.Remove(result);
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }
    }
}
