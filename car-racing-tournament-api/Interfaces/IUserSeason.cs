﻿using car_racing_tournament_api.Models;

namespace car_racing_tournament_api.Interfaces
{
    public interface IUserSeason
    {
        public Task<bool> HasPermissionAsync(Guid userId, Guid seasonId);
        public Task<bool> IsAdmin(Guid userId, Guid seasonId);
        public Task<bool> IsModeratorAsync(Guid userId, Guid seasonId);
        public Task<(bool IsSuccess, string? ErrorMessage)> AddAdmin(Guid userId, Guid seasonId);
        public Task<(bool IsSuccess, string? ErrorMessage)> AddModerator(Guid adminId, Guid moderatorId, Guid seasonId);
        public Task<(bool IsSuccess, string? ErrorMessage)> RemovePermission(Guid adminId, Guid moderatorId, Guid seasonId);
        public Task<(bool IsSuccess, List<UserSeason>? UserSeasons, string? ErrorMessage)> GetSeasonsByUserId(Guid userId);
    }
}