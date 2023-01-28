﻿using car_racing_tournament_api.Models;
using Microsoft.EntityFrameworkCore;

namespace car_racing_tournament_api.Data
{
    public class CarRacingTournamentDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; } = default!;
        public virtual DbSet<Season> Seasons { get; set; } = default!;
        public virtual DbSet<Team> Teams { get; set; } = default!;
        public virtual DbSet<Driver> Drivers { get; set; } = default!;
        public virtual DbSet<Race> Races { get; set; } = default!;
        public virtual DbSet<Result> Results { get; set; } = default!;
        public virtual DbSet<UserSeason> UserSeasons { get; set; } = default!;

        public CarRacingTournamentDbContext() { }

        public CarRacingTournamentDbContext(DbContextOptions<CarRacingTournamentDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Username)
                    .IsUnique();

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.Password)
                    .IsRequired();
            });

            modelBuilder.Entity<UserSeason>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Permission)
                    .IsRequired();

                entity.HasIndex(e => new { e.SeasonId, e.UserId })
                    .IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserSeasons)
                    .HasForeignKey(e => e.UserId)
                    .IsRequired();

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.UserSeasons)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Season>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.Description);
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.Color)
                    .IsRequired();

                entity.HasIndex(e => new { e.SeasonId, e.Name, e.Color })
                    .IsUnique();

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Teams)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.RealName);

                entity.Property(e => e.Number)
                    .IsRequired();

                entity.HasIndex(e => new { e.SeasonId, e.Name, e.Number })
                    .IsUnique();

                entity.HasOne(e => e.ActualTeam)
                    .WithMany(e => e.Drivers)
                    .HasForeignKey(e => e.ActualTeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Drivers)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Race>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.DateTime);

                entity.HasIndex(e => new { e.SeasonId, e.Name })
                    .IsUnique();

                entity.HasOne(e => e.Season)
                    .WithMany(e => e.Races)
                    .HasForeignKey(e => e.SeasonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Position)
                    .IsRequired();

                entity.Property(e => e.Points)
                    .IsRequired();

                entity.HasOne(e => e.Driver)
                    .WithMany(e => e.Results)
                    .HasForeignKey(e => e.DriverId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(e => e.Team)
                    .WithMany(e => e.Results)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasOne(e => e.Race)
                    .WithMany(e => e.Results)
                    .HasForeignKey(e => e.RaceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
        }
    }
}