using System.Text.Json.Serialization;

namespace PokedexMaui.Models;

public class PokemonResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("base_experience")]
    public int BaseExperience { get; set; }

    [JsonPropertyName("sprites")]
    public PokemonSprites Sprites { get; set; } = new();

    [JsonPropertyName("types")]
    public List<PokemonTypeSlot> Types { get; set; } = [];
}

public class PokemonSprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; set; }
}

public class PokemonTypeSlot
{
    [JsonPropertyName("type")]
    public NamedResource Type { get; set; } = new();
}

public class NamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
