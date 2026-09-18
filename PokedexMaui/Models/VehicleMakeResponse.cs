using System.Text.Json.Serialization;

namespace PokedexMaui.Models;

// Parte 13. La API vPIC envuelve los datos en un sobre con Count, Message,
// SearchCriteria y Results. PokeAPI, en cambio, devuelve el recurso en la raiz.
public class VehicleMakeResponse
{
    [JsonPropertyName("Count")]
    public int Count { get; set; }

    [JsonPropertyName("Message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("Results")]
    public List<VehicleMake> Results { get; set; } = [];
}

public class VehicleMake
{
    [JsonPropertyName("Make_ID")]
    public int MakeId { get; set; }

    [JsonPropertyName("Make_Name")]
    public string MakeName { get; set; } = string.Empty;
}
