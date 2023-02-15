﻿using AutoMapper;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.DTO;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class RaceService : IRace
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public RaceService(CarRacingTournamentDbContext carRacingTournamentDbContext, IMapper mapper, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, Race? Race, string? ErrorMessage)> GetRaceById(Guid id)
        {
            var race = await _carRacingTournamentDbContext.Races
                .Where(e => e.Id == id)
                .Include(x => x.Results!).ThenInclude(x => x.Driver)
                .Include(x => x.Results!).ThenInclude(x => x.Team)
                .Select(x => new Race
                {
                    Id = x.Id,
                    Name = x.Name,
                    DateTime = x.DateTime,
                    Results = x.Results!.Select(x => new Result
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
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (race == null)
                return (false, null, _configuration["ErrorMessages:RaceNotFound"]);

            return (true, race, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateRace(Race race, RaceDto raceDto)
        {
            if (raceDto == null)
                return (false, _configuration["ErrorMessages:MissingRace"]);

            raceDto.Name = raceDto.Name.Trim();
            if (string.IsNullOrEmpty(raceDto.Name))
                return (false, _configuration["ErrorMessages:RaceNameCannotBeEmpty"]);

            race.Name = raceDto.Name;
            race.DateTime = raceDto.DateTime;
            _carRacingTournamentDbContext.Races.Update(race);
            await _carRacingTournamentDbContext.SaveChangesAsync();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> DeleteRace(Race race)
        {
            _carRacingTournamentDbContext.Races.Remove(race);
            await _carRacingTournamentDbContext.SaveChangesAsync();
            
            return (true, null);
        }

        public async Task<(bool IsSuccess, List<Result>? Results, string? ErrorMessage)> GetResultsByRaceId(Race race)
        {
            var results = await _carRacingTournamentDbContext.Results
            .Where(x => x.RaceId == race.Id)
            .Include(x => x.Driver)
            .Include(x => x.Team)
            .Select(x => new Result {
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
            }).ToListAsync();

            if (results == null)
                return (false, null, _configuration["ErrorMessages:ResultNotFound"]);

            return (true, results, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddResult(Race race, ResultDto resultDto, Driver driver, Team team)
        {
            if (resultDto == null)
                return (false, _configuration["ErrorMessages:MissingResult"]);

            if (race.SeasonId != team.SeasonId)
                return (false, _configuration["ErrorMessages:RaceTeamNotSameSeason"]);

            if (race.SeasonId != driver.SeasonId)
                return (false, _configuration["ErrorMessages:RaceDriverNotSameSeason"]);

            if (resultDto.Position <= 0)
                return (false, _configuration["ErrorMessages:ResultPosition"]);

            if (resultDto.Points < 0)
                return (false, _configuration["ErrorMessages:ResultPoints"]);

            var result = _mapper.Map<Result>(resultDto);
            result.Id = Guid.NewGuid();
            result.Race = race;
            await _carRacingTournamentDbContext.AddAsync(result);
            await _carRacingTournamentDbContext.SaveChangesAsync();
            
            return (true, null);
        }
    }
}
