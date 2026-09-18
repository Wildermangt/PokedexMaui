using PokedexMaui.Models;
using PokedexMaui.Services;

namespace PokedexMaui;

// Parte 13. Misma separacion de responsabilidades que la Pokedex: la pagina
// solo muestra estados y delega la consulta HTTP en VehicleApiService.
public partial class VehiclesPage : ContentPage
{
    private readonly VehicleApiService _vehicleApiService;
    private List<VehicleMake> _allMakes = [];
    private CancellationTokenSource? _loadCts;

    public VehiclesPage(VehicleApiService vehicleApiService)
    {
        InitializeComponent();
        _vehicleApiService = vehicleApiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_allMakes.Count > 0)
            return;

        await LoadMakesAsync();
    }

    // Regresa a la Pokedex. Como VehiclesPage se abrio con PushAsync sobre la
    // NavigationPage, basta con sacarla de la pila: MainPage conserva su estado.
    private async void OnPokedexClicked(object? sender, EventArgs e)
        => await Navigation.PopAsync();

    private async Task LoadMakesAsync()
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();

        SetLoading(true);

        try
        {
            _allMakes = await _vehicleApiService.GetAllMakesAsync(_loadCts.Token);

            if (_allMakes.Count == 0)
            {
                ShowMessage("El servicio no devolvió fabricantes.");
                return;
            }

            MessageLabel.IsVisible = false;
            ApplyFilter(FilterEntry.Text);
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

    // El filtro es local: se aplica sobre la lista ya descargada y no genera
    // una nueva peticion HTTP por cada tecla.
    private void OnFilterTextChanged(object? sender, TextChangedEventArgs e)
        => ApplyFilter(e.NewTextValue);

    private void ApplyFilter(string? text)
    {
        string filter = (text ?? string.Empty).Trim();

        List<VehicleMake> visible = string.IsNullOrEmpty(filter)
            ? _allMakes
            : _allMakes
                .Where(make => make.MakeName.Contains(
                    filter, StringComparison.OrdinalIgnoreCase))
                .ToList();

        MakesCollection.ItemsSource = visible;
        SummaryLabel.Text = string.IsNullOrEmpty(filter)
            ? $"{_allMakes.Count} fabricantes descargados desde la API vPIC."
            : $"{visible.Count} de {_allMakes.Count} fabricantes contienen \"{filter}\".";
    }

    private void ShowMessage(string message)
    {
        MakesCollection.ItemsSource = null;
        SummaryLabel.Text = "API vPIC de la NHTSA — GetAllMakes";
        MessageLabel.Text = message;
        MessageLabel.IsVisible = true;
    }

    private void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        FilterEntry.IsEnabled = !isLoading;
    }
}
