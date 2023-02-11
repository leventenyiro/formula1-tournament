﻿using car_racing_tournament_api.Data;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Services
{
    public class UserSeasonService : IUserSeason
    {
        private readonly CarRacingTournamentDbContext _carRacingTournamentDbContext;
        private readonly IConfiguration _configuration;

        public UserSeasonService(CarRacingTournamentDbContext carRacingTournamentDbContext, IConfiguration configuration)
        {
            _carRacingTournamentDbContext = carRacingTournamentDbContext;
            _configuration = configuration;
        }

        public async Task<bool> IsAdmin(Guid userId, Guid seasonId)
        {
            var userSeason = await _carRacingTournamentDbContext.UserSeasons.Where(x => x.UserId == userId && x.SeasonId == seasonId).FirstOrDefaultAsync();
            if (userSeason == null)
                return false;

            return userSeason.Permission == UserSeasonPermission.Admin;
        }

        public async Task<bool> IsAdminModerator(Guid userId, Guid seasonId)
        {
            var userSeason = await _carRacingTournamentDbContext.UserSeasons.Where(x => x.UserId == userId && x.SeasonId == seasonId).FirstOrDefaultAsync();
            if (userSeason == null)
                return false;

            return userSeason.Permission == UserSeasonPermission.Moderator || userSeason.Permission == UserSeasonPermission.Admin;
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddAdmin(Guid userId, Guid seasonId)
        {
            var season = await _carRacingTournamentDbContext.Seasons.Where(e => e.Id == seasonId).FirstOrDefaultAsync();
            if (season == null)
                return (false, _configuration["ErrorMessages:SeasonNotFound"]);

            await _carRacingTournamentDbContext.UserSeasons.AddAsync(new UserSeason { Id = new Guid(), UserId = userId, SeasonId = seasonId, Permission = UserSeasonPermission.Admin });
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> AddModerator(Guid adminId, Guid moderatorId, Guid seasonId)
        {
            var season = await _carRacingTournamentDbContext.Seasons.Where(e => e.Id == seasonId).FirstOrDefaultAsync();
            if (season == null)
                return (false, _configuration["ErrorMessages:SeasonNotFound"]);

            await _carRacingTournamentDbContext.UserSeasons.AddAsync(new UserSeason { Id = new Guid(), UserId = moderatorId, SeasonId = seasonId, Permission = UserSeasonPermission.Moderator });
            _carRacingTournamentDbContext.SaveChanges();

            return (true, null);
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> RemovePermission(Guid adminId, Guid moderatorId, Guid seasonId)
        {
            UserSeason moderatorObj = await _carRacingTournamentDbContext.UserSeasons.Where(x => x.UserId == moderatorId && x.SeasonId == seasonId).FirstAsync();
            if (moderatorObj == null)
                return (false, _configuration["ErrorMessages:ModeratorNotFound"]);

            _carRacingTournamentDbContext.UserSeasons.Remove(moderatorObj);
            _carRacingTournamentDbContext.SaveChanges();
            
            return (true, null);
        }
    }
}
