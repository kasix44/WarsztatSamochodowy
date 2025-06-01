# WorkshopManager

System zarządzania warsztatem samochodowym.

## CI/CD (GitHub Actions)

Automatyczna integracja i wdrażanie (CI/CD) dla tego projektu jest skonfigurowane przy użyciu GitHub Actions. Workflow zdefiniowany jest w pliku `.github/workflows/dotnet-ci.yml`.

### Główne kroki workflow:

1.  **Checkout code (`actions/checkout@v4`):**
    *   Pobiera najnowszą wersję kodu z repozytorium.

2.  **Setup .NET (`actions/setup-dotnet@v4`):**
    *   Konfiguruje środowisko .NET w wersji 8.0.x na maszynie budującej.

3.  **Restore dependencies (`dotnet restore`):**
    *   Przywraca wszystkie zależności NuGet zdefiniowane w pliku `.csproj`.

4.  **Build (`dotnet build --configuration Release --no-restore`):**
    *   Kompiluje projekt w konfiguracji `Release`.
    *   Opcja `--no-restore` jest używana, ponieważ zależności zostały już przywrócone w poprzednim kroku.

5.  **Test (`dotnet test --configuration Release --no-build --verbosity normal`):**
    *   Uruchamia wszystkie testy jednostkowe i integracyjne w projekcie.
    *   Opcja `--no-build` zapobiega ponownej kompilacji, ponieważ projekt został już zbudowany.
    *   `--verbosity normal` ustawia poziom szczegółowości logów z testów.

### Wyzwalacze (Triggers):

Workflow jest automatycznie uruchamiany w następujących przypadkach:

*   **Push do gałęzi `main`**: Każde wypchnięcie zmian do głównej gałęzi `main`.
*   **Pull request do gałęzi `main`**: Każde utworzenie lub aktualizacja pull requesta skierowanego do gałęzi `main`.

Ten proces zapewnia, że każda zmiana w kodzie jest automatycznie budowana i testowana, co pomaga w utrzymaniu jakości kodu i wczesnym wykrywaniu błędów. 