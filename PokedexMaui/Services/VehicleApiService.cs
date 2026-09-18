using System.Net.Http.Json;
using PokedexMaui.Models;

namespace PokedexMaui.Services;

// Parte 13. Reutiliza la misma arquitectura de PokeApiService: un HttpClient
// inyectado, una llamada asincrona con CancellationToken y el control de los
// codigos de estado HTTP en un unico lugar.
public class VehicleApiService
{
    private readonly HttpClient _httpClient;

    public VehicleApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<VehicleMake>> GetAllMakesAsync(
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(
            "GetAllMakes?format=json",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        VehicleMakeResponse? payload =
            await response.Content.ReadFromJsonAsync<VehicleMakeResponse>(
                cancellationToken: cancellationToken);

        return payload?.Results ?? [];
    }
}
