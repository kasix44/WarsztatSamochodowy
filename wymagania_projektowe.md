
# 🧑‍🔧 Projekt zaliczeniowy – ASP.NET Core
## **System zarządzania warsztatem samochodowym 2.0**

---

## 🧠 **Cel projektu**

Zaprojektuj i zaimplementuj aplikację webową do obsługi warsztatu samochodowego. Aplikacja powinna umożliwiać:

- zarządzanie klientami i pojazdami,
- zlecenia serwisowe z czynnościami i częściami,
- przypisywanie zadań mechanikom,
- komentowanie zleceń,
- generowanie raportów PDF,
- dodawanie zdjęć pojazdów,
- filtrowanie i raportowanie napraw.

Projekt powinien mieć przejrzystą strukturę, modularność, oraz używać nowoczesnych narzędzi: **EF Core**, **Dependency Injection**, **Mapperly**, **Identity**, **Swagger**, **Razor Pages, MVC (lub frontend SPA)**.

---

## 🧑‍🤝‍🧑 Zespół
- Zespół: **2 osoby**
- Praca nad repozytorium GitHub / GitLab (wymagana historia commitów)

---

## ✅ **Wymagania funkcjonalne**

| Moduł                 | Wymagania                                                                 |
|-----------------------|---------------------------------------------------------------------------|
| 🔐 Uwierzytelnianie   | Rejestracja, logowanie (ASP.NET Identity), role: `Admin`, `Mechanik`, `Recepcjonista` |
| 👤 Klienci            | CRUD klientów, wyszukiwanie, lista pojazdów klienta                      |
| 🚘 Pojazdy            | CRUD, zdjęcie pojazdu (upload), VIN, rejestracja                         |
| 🧾 Zlecenia           | Tworzenie zleceń, statusy, przypisywanie mechaników                      |
| 🔧 Czynności          | Lista czynności: opis + koszt robocizny                                  |
| ⚙️ Części             | Wybór części z katalogu, ilość, koszt                                     |
| 💬 Komentarze         | Komentarze wewnętrzne do zleceń (historia przebiegu)                     |
| 📦 Katalog części     | CRUD części, tylko dla `Admin` / `Recepcjonista`                         |
| 📈 Raporty            | Koszt napraw danego klienta / pojazdu / miesiąca + eksport do PDF        |

---

## ✅ **Pozostałe wymagania**

### 🧩 **1. Indeksy – optymalizacja zapytań**

#### 📌 Zadanie:
- Zidentyfikuj **co najmniej dwa zapytania SELECT**, które są często wykonywane i mają **WHERE** lub **JOIN** po kolumnie niekluczowej.
- Dodaj **indeksy nieklastrowane (non-clustered)** do wybranych kolumn.
- Zrób analizę wydajności:
    - **Zrzut planu zapytania (Query Plan)** przed i po dodaniu indeksu.
    - Krótkie porównanie (np. liczba odczytów, operacje przeszukiwania vs seek).
    - Umieść to w **raporcie PDF** z opisem + screenshotami.

#### 📎 Plik: `raport-indeksy.pdf`

---

### 📡 **2. SQL Profiler – nasłuch endpointu**

#### 📌 Zadanie:
- Uruchom **SQL Server Profiler (lub EF Core Logging)**.
- Wybierz konkretny **endpoint API**.
- Uruchom aplikację → wywołaj endpoint → zrób screenshot z Profilerem pokazującym zapytanie.
- Dodaj screenshoty + opis działania zapytania + krótki komentarz.

#### 📎 Plik: `raport-sql-profiler.pdf`

---

### ⚙️ **3. GitHub Actions – CI/CD**

#### 📌 Zadanie:
- Skonfiguruj workflow z następującymi krokami:
    - build (`dotnet build`)
    - test (`dotnet test`)
    - opcjonalnie: build obrazu Docker
    - opcjonalnie: push do DockerHub (wymaga tokenu)

#### 📎 Plik: `README.md` → opis działania CI/CD
#### 📎 Plik: `dotnet-ci.yml` w repozytorium

---

### 📝 **4. Logowanie błędów – NLog**

#### 📌 Zadanie:
- Skonfiguruj **NLog** do logowania wyjątków i zdarzeń:
    - logi zapisywane do pliku (np. `/logs/errors.log`)
    - logowanie błędów kontrolerów i serwisów
    - obsługa logowania przez DI (`ILogger<T>`)

---

### 📤 **5. BackgroundService – raport e-mail**

#### 📌 Zadanie:
- Zaimplementuj usługę w tle (`BackgroundService`), która:
    - raz dziennie (lub co 1–2 minuty dla testów) generuje raport z aktualnych zleceń
    - zapisuje go jako PDF (np. `open_orders.pdf`)
    - wysyła jako załącznik na e-mail admina (np. za pomocą SMTP)

#### 📎 Plik: `raport-otwarte-naprawy.pdf`
#### 📎 Klasa: `OpenOrderReportBackgroundService.cs`

---

### 🚀 **6. NBomber – testy wydajności**

#### 📌 Zadanie:
- Skonfiguruj **NBomber** do przetestowania wybranego endpointu, np. `GET /api/orders/active`
- Uruchom test z 50 równoległymi użytkownikami, 100 żądaniami
- Zapisz **raport PDF z wynikami testu**

#### 📎 Plik: `nbomber-report.pdf`
#### 📎 Kod testu: np. `PerformanceTests/OrdersLoadTest.cs`

---

## 🧱 **Modele danych (przykładowe)**

```csharp
class Customer { ... }
class Vehicle { string ImageUrl; ... }
class ServiceOrder { Status, AssignedMechanic, List<ServiceTask>, List<Comment> }
class ServiceTask { Description, LaborCost, List<UsedPart> }
class Part { Name, UnitPrice }
class UsedPart { Part, Quantity }
class Comment { Author, Content, Timestamp }
```

---

## 🛠️ **Wymagania techniczne**

| Obszar                  | Szczegóły                                                                 |
|-------------------------|---------------------------------------------------------------------------|
| **ASP.NET Core**        | Wersja 7 lub 8                                                            |
| **EF Core**             | Code First + migracje - SQL Server                             |
| **Identity**            | Logowanie, role, autoryzacja                                              |
| **Mapperly**            | Mapowanie DTO ↔️ encje np. Mapperly                                 |
| **DI**                  | Serwisy biznesowe (`ICustomerService`, `IOrderService`, ...)              |
| **Swagger**             | Dokumentacja API                                                          |
| **Upload plików**       | Zdjęcie pojazdu (np. do `/wwwroot/uploads`)                              |
| **PDF**    | Generowanie raportów jako PDF                 |
| **Frontend**            | Razor Pages + Bootstrap (opcjonalnie SPA: React/Blazor/Angular)           |
| **Testy**  | testy jednostkowe (xUnit/NUnit)                                |

---

## 🗂️ **Struktura projektu**

```
/WorkshopManager
├── Controllers/
├── DTOs/
├── Models/
├── Services/
├── Mappers/             // Mapperly mappery
├── Views/
├── wwwroot/
│   └── uploads/         // zdjęcia pojazdów
├── Data/
├── Program.cs
```



## ✅ Co należy oddać?

- Repozytorium GitHub z historią commitów
- Działająca aplikacja ASP.NET Core
- Migracje + seed danych (lub dump bazy)
- `README.md` z opisem projektu, logowania, rolami


---

## 📌 Wskazówki

- Wszystkie dane domenowe mapuj za pomocą **Mapperly**
- Używaj **DataAnnotations** do walidacji
- Dbaj o **separację warstw**: logika w serwisach, nie w kontrolerach
