using System.Text.Json.Serialization;

namespace GameApi.Entities;

public class Game
{
    [JsonIgnore]
    public int Id { get; set; }
    public required string Name { get; set; }

    public required string Genre { get; set; }

    public decimal Price { get; set; }

    public DateTime ReleaseDate { get; set; }

    public required string ImageUri { get; set; }

}
