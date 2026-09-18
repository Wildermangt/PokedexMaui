using System.Net;
using System.Net.Http.Json;
using PokedexMaui.Models;

namespace PokedexMaui.Services;

public class PokeApiService
{
    private readonly HttpClient _httpClient;

    public PokeApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PokemonResponse?> GetPokemonAsync(
        string nameOrId,
        CancellationToken cancellationToken = default)
    {
        string value = nameOrId.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Escriba un nombre o número.");

        using HttpResponseMessage response = await _httpClient.GetAsync(
            $"pokemon/{Uri.EscapeDataString(value)}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PokemonResponse>(
            cancellationToken: cancellationToken);
    }
}
