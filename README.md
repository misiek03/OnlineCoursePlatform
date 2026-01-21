# Platforma do Zarządzania Kursami Online (OnlineCoursePlatform)

Kompleksowy system webowy oparty na technologii ASP.NET Core MVC, służący do obsługi sprzedaży i dystrybucji kursów wideo. Aplikacja umożliwia zarządzanie treściami edukacyjnymi przez administratorów oraz przeglądanie i zakup kursów przez użytkowników.

## 📋 Spis treści
1. [Technologie i Architektura](#technologie-i-architektura)
2. [Instalacja i Uruchomienie](#instalacja-i-uruchomienie)
3. [Konfiguracja Bazy Danych](#konfiguracja-bazy-danych)
4. [Role i Uprawnienia (Dostęp Testowy)](#role-i-uprawnienia-dostęp-testowy)
5. [Opis Funkcjonalności](#opis-funkcjonalności)
6. [Struktura Bazy Danych](#struktura-bazy-danych)
7. [API](#api)

---

## 🛠 Technologie i Architektura

Projekt został zrealizowany zgodnie ze wzorcem projektowym **MVC (Model-View-Controller)**, co zapewnia separację logiki biznesowej od warstwy prezentacji.

* **Platforma:** .NET 10 (ASP.NET Core)
* **Język:** C#
* **Baza danych:** SQLite (plikowa baza danych, niewymagająca zewnętrznego serwera)
* **ORM:** Entity Framework Core (podejście Code-First)
* **System Autoryzacji:** ASP.NET Core Identity (Individual Accounts)
* **Frontend:** Razor Views (.cshtml), Bootstrap 5, HTML5/CSS3
* **IDE:** JetBrains Rider / Visual Studio

---

## 🚀 Instalacja i Uruchomienie

Aplikacja jest gotowa do uruchomienia na systemach Windows, macOS oraz Linux.

### Wymagania wstępne
* Zainstalowane SDK .NET 10.0 (lub nowsze/kompatybilne).
* Narzędzie git (do sklonowania repozytorium).

### Instrukcja krok po kroku

1. **Klonowanie repozytorium:**

    git clone https://github.com/TwojLogin/OnlineCoursePlatform.git
    cd OnlineCoursePlatform

2. **Przywracanie pakietów i budowanie:**

    dotnet restore
    dotnet build

3. **Inicjalizacja Bazy Danych:**
    Projekt korzysta z lokalnej bazy SQLite. Przy pierwszym uruchomieniu mechanizm migracji powinien automatycznie utworzyć plik bazy, ale można to wymusić ręcznie:

    dotnet ef database update

4. **Uruchomienie:**

    dotnet run

    Aplikacja domyślnie uruchomi się pod adresem `https://localhost:7146` (lub podobnym, zależnie od konfiguracji `launchSettings.json`).

---

## ⚙️ Konfiguracja Bazy Danych

System wykorzystuje lekką bazę danych SQLite, co eliminuje konieczność instalacji serwerów SQL (jak MS SQL Server). Plik bazy danych tworzony jest automatycznie w głównym katalogu projektu.

**Łańcuch połączenia (Connection String)** w pliku `appsettings.json`:

    "ConnectionStrings": {
      "DefaultConnection": "Data Source=kursy.db"
    }

Plik `kursy.db` zawiera kompletną strukturę tabel oraz dane użytkowników.

---

## 🔐 Role i Uprawnienia (Dostęp Testowy)

W projekcie zaimplementowano mechanizm **Seedowania Danych** (`DbInitializer`). Przy pierwszym uruchomieniu aplikacja automatycznie tworzy konto Administratora i domyślne role.

### Konta testowe:

| Rola | Login (Email) | Hasło | Opis uprawnień |
| :--- | :--- | :--- | :--- |
| **Administrator** | `admin@admin.com` | `Admin123!` | Pełny dostęp: CRUD kursów, kategorii, podgląd zapisów. |
| **Użytkownik** | *(Rejestracja własna)* | *(Dowolne)* | Przeglądanie, zapisywanie się na kursy, dostęp do materiałów wideo. |

> **Uwaga:** Aby przetestować funkcjonalność zwykłego użytkownika, należy zarejestrować nowe konto poprzez formularz "Register" dostępny na stronie głównej.

---

## 📱 Opis Funkcjonalności

### 1. Panel Administratora
Zalogowany Administrator ma dostęp do rozszerzonych funkcji zarządzania platformą:
* **Zarządzanie Kursami:** Dodawanie nowych kursów, edycja istniejących oraz ich usuwanie.
* **Obsługa Wideo:** Możliwość dodawania linków z YouTube (np. `youtube.com/watch?v=...`), które są automatycznie konwertowane na format osadzony (embed).
* **Zarządzanie Kategoriami:** Definiowanie kategorii tematycznych (np. Programowanie, Grafika).
* **Podgląd Statystyk:** Widoczność liczby osób zapisanych na każdy kurs.

### 2. Panel Użytkownika (Studenta)
Użytkownik po zalogowaniu zyskuje dostęp do interaktywnych funkcji:
* **Wyszukiwarka:** Zaawansowane filtrowanie kursów po nazwie lub opisie (ignorujące wielkość liter).
* **System Zapisów:**
    * Przycisk **"Zapisz się"**: Dodaje użytkownika do listy uczestników.
    * Przycisk **"Wypisz się"**: Usuwa użytkownika z kursu.
* **Moje Kursy:** Dedykowany widok `/Courses/MyCourses` wyświetlający tylko zakupione kursy.
* **Odtwarzacz Wideo:** Dostęp do szczegółów kursu (wideo iframe) jest zablokowany dla osób niezapisanych. Dopiero po kliknięciu "Zapisz się", użytkownik może wejść w szczegóły i obejrzeć materiał.

---

## 🗂 Struktura Bazy Danych

Baza danych została zaprojektowana przy użyciu podejścia Code-First i zawiera następujące kluczowe encje:

1. **Course (Kurs):**
    * Główna encja zawierająca: Tytuł, Opis, Cenę, URL do YouTube.
    * Relacja `N:1` z Kategorią.
    * Relacja `1:N` z Zapisami (Enrollments).
2. **Category (Kategoria):**
    * Słownik kategorii (np. IT, Marketing).
3. **Enrollment (Zapis):**
    * Tabela łącząca (Junction Table) realizująca relację `N:M` między Użytkownikiem a Kursem.
    * Przechowuje datę zapisu.
4. **AspNetUsers (Identity):**
    * Standardowa tabela frameworka Identity przechowywująca dane logowania i role.

---

## 🌐 API

Aplikacja udostępnia interfejs REST API, umożliwiający pobieranie danych o kursach przez zewnętrzne systemy.

* **Endpoint:** `/api/CoursesApi`
* **Format:** JSON
* **Metody:**
    * `GET /api/CoursesApi` - Lista wszystkich kursów.
    * `GET /api/CoursesApi/{id}` - Szczegóły konkretnego kursu.
    * `POST`, `PUT`, `DELETE` - Operacje modyfikacji (zabezpieczone).

W konfiguracji API zastosowano `ReferenceHandler.IgnoreCycles`, aby zapobiec błędom serializacji przy relacjach dwukierunkowych (Kurs <-> Kategoria).

---
*Autor: Michał Grygiel*
