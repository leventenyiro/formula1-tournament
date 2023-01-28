﻿namespace car_racing_tournament_api.Models
{
    public class Result
    {
        public Guid Id { get; set; }
        public int Position { get; set; }
        public int Points { get; set; }
        public Guid DriverId { get; set; }
        public Guid TeamId { get; set; }
        public Guid RaceId { get; set; }
    }
}
