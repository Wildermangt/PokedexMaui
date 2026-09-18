# PokedexMaui

Aplicación móvil multiplataforma construida con **.NET MAUI** que consulta un Pokémon
por nombre o por número contra la API REST pública [PokeAPI](https://pokeapi.co/),
transforma la respuesta JSON en objetos C# y presenta sus datos con estados de carga
y manejo de errores.

Proyecto académico — Programación Avanzada y Aplicada, VI semestre, Tecnología en
Desarrollo de Software.

---

## Qué hace

- Consulta por **nombre** (`pikachu`) o por **número** (`25`) contra el mismo endpoint.
- Muestra imagen, nombre, número, tipos, altura, peso y experiencia base.
- Convierte las unidades que entrega la API: decímetros a metros y hectogramos a kilogramos.
- Distingue **cuatro estados** en la interfaz: carga, éxito, recurso no encontrado y
  error de conexión.
- Incluye una segunda pantalla que consume la API [vPIC de la NHTSA](https://vpic.nhtsa.dot.gov/api/)
  y lista 12.363 fabricantes de vehículos con filtro local por nombre.

---

## Arquitectura

El proyecto separa responsabilidades en capas, de modo que cada archivo tenga una sola
razón para cambiar.

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Presentación | `MainPage.xaml` / `.xaml.cs` | Recibe la búsqueda, muestra estados y presenta datos |
| Servicio | `Services/PokeApiService.cs` | Construye la solicitud, consume la API y controla los códigos HTTP |
| Modelo | `Models/PokemonResponse.cs` | Representa los campos del JSON que usa la aplicación |
| Configuración | `MauiProgram.cs` / `App.xaml.cs` | Registra dependencias y crea la ventana principal |

```
PokedexMaui/
├─ NuGet.config
├─ PokedexMaui.sln
└─ PokedexMaui/
   ├─ Models/
   │  ├─ PokemonResponse.cs        PokeAPI
   │  └─ VehicleMakeResponse.cs    API vPIC
   ├─ Services/
   │  ├─ PokeApiService.cs
   │  └─ VehicleApiService.cs
   ├─ MainPage.xaml / .xaml.cs     Pokédex
   ├─ VehiclesPage.xaml / .xaml.cs Fabricantes de vehículos
   └─ MauiProgram.cs
```

### El recorrido de los datos

1. El usuario escribe un valor y pulsa **Buscar** (o Intro) → se activa `OnSearchClicked`.
2. La página valida la entrada, enciende el indicador de carga y llama a `GetPokemonAsync`.
3. El servicio codifica el texto con `Uri.EscapeDataString` y envía `GET pokemon/{valor}`.
4. PokeAPI responde con un código HTTP y, si el recurso existe, con un documento JSON.
5. `ReadFromJsonAsync` construye un `PokemonResponse` guiándose por `JsonPropertyName`.
6. La página convierte unidades, une los tipos y actualiza los controles XAML.
7. La interfaz refleja el desenlace: éxito, no encontrado o error de conexión.

### Una decisión de diseño

El 404 **no** se trata como una excepción. El servicio lo convierte en `null`, porque
«el recurso no existe» es una respuesta válida del servidor, no un fallo:

```csharp
if (response.StatusCode == HttpStatusCode.NotFound)
    return null;

response.EnsureSuccessStatusCode();
```

Un fallo de red, en cambio, lanza `HttpRequestException` y se captura en su propio
bloque. Son dos mensajes distintos porque son dos problemas distintos, y el usuario
necesita saber cuál de los dos le ocurrió.

---

## Requisitos

- .NET SDK 10.0 o superior
- Carga de trabajo de .NET MAUI
  (Visual Studio Installer → *Desarrollo de la interfaz de usuario de aplicaciones
  multiplataforma de .NET*)

## Ejecutar

```bash
git clone https://github.com/Wildermangt/PokedexMaui.git
cd PokedexMaui
dotnet restore
dotnet build PokedexMaui/PokedexMaui.csproj -f net10.0-windows10.0.19041.0
dotnet run --project PokedexMaui/PokedexMaui.csproj -f net10.0-windows10.0.19041.0
```

O abrir `PokedexMaui.sln` en Visual Studio, seleccionar **Windows Machine** y pulsar F5.
También compila para Android; el permiso de Internet ya está declarado en
`AndroidManifest.xml`.

> El `NuGet.config` incluido registra nuget.org de forma explícita. Sin él, en un equipo
> donde ese origen no esté configurado, la restauración falla con `NU1101`.

---

## Casos de prueba

| # | Caso | Entrada | Resultado |
|---|---|---|---|
| 1 | Búsqueda por nombre | `pikachu` | Pikachu · #25 · electric · 0,4 m · 6,0 kg · 112 |
| 2 | Búsqueda por número | `1` | Bulbasaur · #1 · grass, poison · 0,7 m · 6,9 kg · 64 |
| 3 | Recurso inexistente | `pokemon-inexistente` | «No se encontró el Pokémon solicitado» |
| 4 | Entrada vacía | *(vacío)* | «Escriba un nombre o número», sin llamar a la red |
| 5 | Sin conexión | `pikachu` | «No fue posible conectarse con el servicio» |

Los cinco casos se ejecutaron sobre la aplicación en Windows Machine. **5 de 5 correctos.**

---

## Tecnologías

`.NET MAUI` · `C#` · `XAML` · `HttpClient` · `System.Text.Json` · Inyección de dependencias

## APIs consumidas

- [PokeAPI v2](https://pokeapi.co/docs/v2) — datos de Pokémon
- [vPIC — NHTSA Vehicle API](https://vpic.nhtsa.dot.gov/api/) — fabricantes de vehículos

## Autor

**Jeferson Wilderman González Tenjo**
Tecnología en Desarrollo de Software — VI semestre
