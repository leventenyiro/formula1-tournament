﻿using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class ResultService : Interfaces.IResult
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ResultService(CarRacingTournamentDbContext carRacingTournamentDbContext, IMapper mapper)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _mapper = mapper;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
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
                    Type = x.Type,
                    Position = x.Position,
                    Point = x.Point,
                    RaceId = x.RaceId,
                    DriverId = x.DriverId,
                    TeamId = x.TeamId,
                    Driver = new Driver
                    {
                        Id = x.Driver.Id,
                        Name = x.Driver.Name,
                        RealName = x.Driver.RealName,
                        Number = x.Driver.Number
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

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddResult(Race race, ResultDto resultDto, Driver driver, Team team)
        {
            if (race.SeasonId != team.SeasonId)
                return (false, _configuration["ErrorMessages:RaceTeamNotSameSeason"]);

            if (race.SeasonId != driver.SeasonId)
                return (false, _configuration["ErrorMessages:RaceDriverNotSameSeason"]);

            if (resultDto.Type == ResultType.Finished && (resultDto.Position <= 0 || resultDto.Position == null))
                return (false, _configuration["ErrorMessages:ResultPosition"]);

            if (resultDto.Point < 0)
                return (false, _configuration["ErrorMessages:ResultPoint"]);

            if (resultDto.Point % 0.5 != 0)
                return (false, _configuration["ErrorMessages:ResultPointNotHalf"]);

            if (await _carRacingTournamentDbContext.Results.CountAsync(
                x => x.DriverId == resultDto.DriverId && x.RaceId == race.Id) != 0)
                return (false, _configuration["ErrorMessages:ResultExists"]);

            var result = _mapper.Map<Result>(resultDto);
            result.Id = Guid.NewGuid();
            result.Position = resultDto.Type == ResultType.Finished ? resultDto.Position : null;
            await _carRacingTournamentDbContext.Results.AddAsync(result);
            _carRacingTournamentDbContext.Entry(driver).State = EntityState.Modified;
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateResult(Result result, ResultDto resultDto, Race race, Driver driver, Team team)
        {
            if (race.SeasonId != team.SeasonId)
                return (false, _configuration["ErrorMessages:RaceTeamNotSameSeason"]);

            if (race.SeasonId != driver.SeasonId)
                return (false, _configuration["ErrorMessages:RaceDriverNotSameSeason"]);

            if (resultDto.Type == ResultType.Finished && (resultDto.Position <= 0 || resultDto.Position == null))
                return (false, _configuration["ErrorMessages:ResultPosition"]);

            if (resultDto.Point < 0)
                return (false, _configuration["ErrorMessages:ResultPoint"]);

            if (resultDto.Point % 0.5 != 0)
                return (false, _configuration["ErrorMessages:ResultPointNotHalf"]);

            if ((result.DriverId != resultDto.DriverId || result.RaceId != resultDto.RaceId) && await _carRacingTournamentDbContext.Results.CountAsync(
                x => x.DriverId == resultDto.DriverId && x.RaceId == resultDto.RaceId) != 0)
                return (false, _configuration["ErrorMessages:ResultExists"]);

            result.Type = resultDto.Type;
            result.Position = resultDto.Type == ResultType.Finished ? resultDto.Position : null;
            result.Point = resultDto.Point;
            result.DriverId = resultDto.DriverId;
            result.TeamId = resultDto.TeamId;
            result.RaceId = resultDto.RaceId;
            _carRacingTournamentDbContext.Entry(driver).State = EntityState.Modified;
            _carRacingTournamentDbContext.Entry(result).State = EntityState.Modified;
            await _carRacingTournamentDbContext.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteResult(Result result)
        {
            var existingResult = await _carRacingTournamentDbContext.Results.FindAsync(result.Id);
            if (existingResult != null)
            {
                _carRacingTournamentDbContext.Results.Remove(existingResult);
                await _carRacingTournamentDbContext.SaveChangesAsync();
            }
            else
            {
                return (false, _configuration["ErrorMessages:ResultNotFound"]);
            }
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteResultsByTeamId(Guid teamId)
        {
            var results = await _carRacingTournamentDbContext.Results
                .Where(x => x.TeamId == teamId)
                .ToListAsync();
            _carRacingTournamentDbContext.Results.RemoveRange(results);
            await _carRacingTournamentDbContext.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteResultsByRaceId(Guid raceId)
        {
            var results = await _carRacingTournamentDbContext.Results
                .Where(x => x.RaceId == raceId)
                .ToListAsync();
            _carRacingTournamentDbContext.Results.RemoveRange(results);
            await _carRacingTournamentDbContext.SaveChangesAsync();
            return (true, null);
        }
    }
}
