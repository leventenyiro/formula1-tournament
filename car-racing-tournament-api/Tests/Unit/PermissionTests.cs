﻿using car_racing_tournament_api.Data;
using car_racing_tournament_api.Models;
using car_racing_tournament_api.Services;
using car_racing_tournament_api.DTO;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace car_racing_tournament_api.Tests.Unit
{
    public class PermissionTests
    {
        private CarRacingTournamentDbContext? _context;
        private PermissionService? _permissionService;
        private Models.User? _user1;
        private Models.User? _user2;
        private Season? _season1;
        private Season? _season2;
        private Permission? _permissionAdmin;
        private Permission? _permissionModerator;
        private IConfiguration? _configuration;

        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<CarRacingTournamentDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CarRacingTournamentDbContext(options);

            _season1 = new Season
            {
                Id = Guid.NewGuid(),
                Name = "Test Season",
                IsArchived = false
            };
            _context.Seasons.Add(_season1);

            _user1 = new Models.User
            {
                Id = Guid.NewGuid(),
                Username = "FirstUser",
                Email = "first@user.com",
                Password = "test"
            };
            _context.Users.Add(_user1);

            _season2 = new Season
            {
                Id = Guid.NewGuid(),
                Name = "Test Season 2",
                IsArchived = false
            };
            _context.Seasons.Add(_season2);

            _user2 = new Models.User
            {
                Id = Guid.NewGuid(),
                Username = "SecondUser",
                Email = "second@user.com",
                Password = "test"
            };
            _context.Users.Add(_user2);

            _permissionAdmin = new Permission
            {
                Id = Guid.NewGuid(),
                Season = _season1,
                User = _user1,
                Type = PermissionType.Admin
            };
            _context.Permissions.Add(_permissionAdmin);

            _permissionModerator = new Permission
            {
                Id = Guid.NewGuid(),
                Season = _season1,
                User = _user2,
                Type = PermissionType.Moderator
            };
            _context.Permissions.Add(_permissionModerator);

            _context.SaveChanges();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _permissionService = new PermissionService(_context);
        }

        [Test]
        public async Task GetPermissionsBySeasonSuccess()
        {
            var result = await _permissionService!.GetPermissionsBySeason(_context!.Seasons.First());
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsNotNull(result.Permissions);
            Assert.AreEqual(result.Permissions!.Count, 2);
        }

        [Test]
        public async Task GetPermissionByIdSuccess() {
            var result = await _permissionService!.GetPermissionById(_permissionAdmin!.Id);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(result.Permission!.Type, PermissionType.Admin);
        }

        [Test]
        public async Task GetPermissionByIdNotFound() {
            var result = await _permissionService!.GetPermissionById(Guid.NewGuid());
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PermissionNotFound"]);
        }

        [Test]
        public async Task IsAdminSuccess()
        {
            var result = await _permissionService!.IsAdmin(_user1!.Id, _season1!.Id);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsAdminFailed()
        {
            var result = await _permissionService!.IsAdmin(_user2!.Id, _season1!.Id);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsAdminModeratorSuccess()
        {
            var result = await _permissionService!.IsAdminModerator(_user1!.Id, _season1!.Id);
            Assert.IsTrue(result);

            result = await _permissionService!.IsAdminModerator(_user2!.Id, _season1!.Id);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsAdminModeratorFailed()
        {
            var result = await _permissionService!.IsAdmin(_user1!.Id, _season2!.Id);
            Assert.IsFalse(result);

            result = await _permissionService!.IsAdmin(_user2!.Id, _season2!.Id);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddAdminSuccess()
        {
            var result = await _permissionService!.AddPermission(_user1!, _season2!, PermissionType.Admin);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Permissions.Count(), 3);
        }

        [Test]
        public async Task AddPermissionExists() {
            var result = await _permissionService!.AddPermission(_user1!, _season1!, PermissionType.Admin);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:PermissionExists"]);
        }

        [Test]
        public async Task UpdatePermissionTypeSuccess()
        {
            var season = new Season
            {
                Id = Guid.NewGuid(),
                Name = "New Season",
                IsArchived = false
            };
            _context!.Seasons.Add(season);

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                User = _user1!,
                Season = season,
                Type = PermissionType.Moderator
            };
            _context.Permissions.Add(permission);
            _context.SaveChanges();

            var result = await _permissionService!.UpdatePermissionType(permission, PermissionType.Admin);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(season.Permissions.Count(), 1);
            Assert.AreEqual(permission.Type, PermissionType.Admin);
        }

        [Test]
        public async Task UpdatePermissionTypeAdminExists()
        {
            var season = new Season
            {
                Id = Guid.NewGuid(),
                Name = "New Season",
                IsArchived = false,
                Permissions = new List<Permission>()
                {
                    new Permission
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        SeasonId = Guid.NewGuid(),
                        Type = PermissionType.Admin
                    }
                }
            };
            _context!.Seasons.Add(season);

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                User = _user1!,
                Season = season,
                Type = PermissionType.Moderator
            };
            _context.Permissions.Add(permission);
            _context.SaveChanges();

            var result = await _permissionService!.UpdatePermissionType(permission, PermissionType.Admin);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, _configuration!["ErrorMessages:SeasonHasAdmin"]);
            Assert.AreEqual(season.Permissions.Count(), 2);
            Assert.AreEqual(permission.Type, PermissionType.Moderator);
        }

        [Test]
        public async Task RemovePermission() {
            var result = await _permissionService!.RemovePermission(_permissionAdmin!);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(_context!.Permissions.Count(), 1);
        }
    }
}
