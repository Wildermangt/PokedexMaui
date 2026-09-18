using System.Globalization;
using PokedexMaui.Models;
using PokedexMaui.Services;

namespace PokedexMaui;

public partial class MainPage : ContentPage
{
    private readonly PokeApiService _pokeApiService;
    private CancellationTokenSource? _searchCts;

    public MainPage(PokeApiService pokeApiService)
    {
        InitializeComponent();
        _pokeApiService = pokeApiService;
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        string query = SearchEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(query))
        {
            ShowMessage("Escriba un nombre o número.");
            return;
        }

        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();

        SetLoading(true);

        try
        {
            PokemonResponse? pokemon = await _pokeApiService.GetPokemonAsync(
                query,
                _searchCts.Token);

            if (pokemon is null)
            {
                ShowMessage("No se encontró el Pokémon solicitado.");
                return;
            }

            ShowPokemon(pokemon);
        }
        catch (TaskCanceledException)
        {
            ShowMessage("La consulta fue cancelada o tardó demasiado.");
        }
        catch (HttpRequestException)
        {
            ShowMessage("No fue posible conectarse con el servicio.");
        }
        catch (Exception)
        {
            ShowMessage("Ocurrió un error inesperado.");
        }
        finally
        {
            SetLoading(false);
        }
    }

    private void ShowPokemon(PokemonResponse pokemon)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        string displayName = textInfo.ToTitleCase(pokemon.Name);
        string types = string.Join(", ",
            pokemon.Types.Select(item => item.Type.Name));

        NameLabel.Text = displayName;
        IdLabel.Text = $"Número #{pokemon.Id}";
        TypeLabel.Text = $"Tipos: {types}";
        HeightLabel.Text = $"Altura: {pokemon.Height / 10.0:F1} m";
        WeightLabel.Text = $"Peso: {pokemon.Weight / 10.0:F1} kg";
        ExperienceLabel.Text = $"Experiencia: {pokemon.BaseExperience}";
        PokemonImage.Source = pokemon.Sprites.FrontDefault;

        MessageLabel.IsVisible = false;
        ResultCard.IsVisible = true;
    }

    private void ShowMessage(string message)
    {
        ResultCard.IsVisible = false;
        MessageLabel.Text = message;
        MessageLabel.IsVisible = true;
    }

    private void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        SearchEntry.IsEnabled = !isLoading;
    }
}
