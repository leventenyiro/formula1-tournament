using System.Text.Json.Serialization;

namespace car_racing_tournament_api.Models
{
    public class Statistics {
        public string? Name { get; set; }
        public int? NumberOfRace { get; set; }
        public int? NumberOfWin { get; set; }
        public int? NumberOfPodium { get; set; }
        public int? NumberOfChampion { get; set; }
        public double? SumPoint { get; set; }
        public List<SeasonStatistics>? SeasonStatistics { get; set; }
        public List<PositionStatistics>? PositionStatistics { get; set; }
    }

    public class SeasonStatistics {
        public string? Name { get; set; }
        public string? TeamName { get; set; }
        public string? TeamColor { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? Position { get; set; }
    }

    public class PositionStatistics {
        public string? Position { get; set; }
        public int? Number { get; set; }
    }
}