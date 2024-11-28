using System.Text.Json.Serialization;

namespace ABC123_HSZF_2024251.Model
{
    public class TaxiCarDto
    {
        public string LicensePlate { get; set; }
        public string Driver { get; set; }

        [JsonPropertyName("Fares")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<FareDto> Fares { get; set; }

        [JsonPropertyName("Services")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<FareDto> Services { get; set; }

        public List<FareDto> GetAllFares() =>
            (Fares ?? Enumerable.Empty<FareDto>())
            .Concat(Services ?? Enumerable.Empty<FareDto>())
            .ToList();
    }
}