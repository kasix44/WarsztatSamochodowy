# System Zarządzania Warsztatem Samochodowym

## Opis Projektu
System Zarządzania Warsztatem Samochodowym to kompleksowe rozwiązanie informatyczne przeznaczone do zarządzania warsztatem samochodowym. Aplikacja umożliwia efektywne zarządzanie klientami, pojazdami, zleceniami serwisowymi, częściami zamiennymi oraz pracownikami warsztatu.

## Funkcjonalności
- Zarządzanie bazą klientów
- Zarządzanie pojazdami klientów
- Obsługa zleceń serwisowych
- Zarządzanie częściami zamiennymi
- Śledzenie aktywności mechaników
- Generowanie raportów
- System komentarzy do zleceń
- Zarządzanie użytkownikami i rolami

## Technologie
- **Backend**: ASP.NET Core 8.0
- **Frontend**: Razor Pages
- **Baza danych**: Microsoft SQL Server
- **Autentykacja**: ASP.NET Core Identity
- **Logowanie**: NLog
- **Generowanie PDF**: QuestPDF
- **Testy wydajnościowe**: NBomber
- **API Documentation**: Swagger/OpenAPI
- **Mapowanie obiektów**: Riok.Mapperly

## Wymagania Systemowe
- .NET 8.0 SDK
- Microsoft SQL Server
- Przeglądarka internetowa z obsługą JavaScript

## Instalacja i Konfiguracja

1. Sklonuj repozytorium
2. Skonfiguruj połączenie z bazą danych w pliku `appsettings.json`
3. Wykonaj migracje bazy danych:
   ```bash
   dotnet ef database update
   ```
4. Uruchom aplikację:
   ```bash
   dotnet run
   ```

## Role Użytkowników

### Administrator
- Pełny dostęp do wszystkich funkcji systemu
- Zarządzanie użytkownikami i rolami
- Konfiguracja systemu
- Generowanie raportów
- Domyślne dane logowania:
  - Email: admin@demo.com
  - Hasło: Admin123!

### Mechanik
- Przeglądanie i aktualizacja zleceń serwisowych
- Dodawanie aktywności do zleceń
- Przeglądanie historii pojazdów
- Domyślne dane logowania:
  - Email: mech@demo.com
  - Hasło: Mech123!

### Recepcjonista
- Dodawanie i edycja klientów
- Rejestracja nowych zleceń serwisowych
- Przeglądanie statusu zleceń
- Domyślne dane logowania:
  - Email: recep@demo.com
  - Hasło: Recep123!

## Struktura Projektu
- **Data/** - Kontekst bazy danych i migracje
- **Models/** - Modele danych
- **Services/** - Logika biznesowa
- **Controllers/** - Kontrolery aplikacji
- **Views/** - Widoki Razor Pages
- **wwwroot/** - Statyczne pliki (CSS, JavaScript, obrazy)

## Bezpieczeństwo
- Uwierzytelnianie oparte na ASP.NET Core Identity
- Autoryzacja oparta na rolach
- Szyfrowane hasła
- Zabezpieczone endpointy API
- Walidacja danych wejściowych

## Logowanie
System wykorzystuje NLog do logowania zdarzeń. Logi są zapisywane w następujących lokalizacjach:
- Logi aplikacji: `logs/all.log`
- Logi błędów: `logs/error.log`
- Logi web: `logs/web.log`

## API
Dokumentacja API jest dostępna pod adresem `/swagger` w trybie deweloperskim. API zawiera endpointy do:
- Zarządzania klientami
- Zarządzania pojazdami
- Obsługi zleceń serwisowych
- Zarządzania częściami
- Zarządzania aktywnościami

## Licencja
Ten projekt jest udostępniany na licencji MIT. 