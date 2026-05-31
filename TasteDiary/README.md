# TasteDiary &mdash; Food & Drink Nutrition Tracker

A cross-platform mobile app for browsing, searching, and recording food &amp; drink items with detailed nutritional information, built with **.NET MAUI** (.NET 9.0) for the 6G6Z0014 Mobile Computing coursework at Manchester Metropolitan University.

TasteDiary demonstrates six mobile hardware APIs (camera, GPS, geocoding, text-to-speech, vibration, haptic feedback) alongside accessibility features aligned with WCAG principles.

---

## Table of Contents

- [Features](#features)
- [Built-in Food Items](#built-in-food-items)
- [Project Structure](#project-structure)
- [Build &amp; Run](#build--run)
- [Marking Criteria Coverage](#marking-criteria-coverage)
- [Mobile Hardware APIs](#mobile-hardware-apis)
- [Accessibility](#accessibility)
- [Screencast Checklist](#screencast-checklist)

---

## Features

| Feature | Description |
|---------|-------------|
| 🔍 **Food Search** | Real-time filtering by name, category, description, and tags |
| 📋 **Food List** | Scrollable cards with thumbnail, calorie badge, macro summary, and category |
| 🔄 **Pull-to-Refresh** | RefreshView bound to the data source with screen-reader announcement |
| ➕ **Add Record** | Validated form with name, category, description, calories, protein, carbs, fat, and allergen notes |
| 📊 **Nutrition Detail** | Full nutrition card with image, macros, allergens, TTS read-aloud, and vibration |
| 📸 **Camera** | Capture food photos using `MediaPicker` |
| 📍 **GPS &amp; Geocoding** | Get coordinates and reverse-geocode to a human-readable address |
| 🔊 **Text-to-Speech** | Read nutrition summaries and help text aloud (English locale) |
| 📳 **Vibration &amp; Haptics** | Haptic feedback on buttons, validation errors, and reminders |
| 🎨 **Theme Switching** | System default / Light / Dark mode applied instantly |
| 🔤 **Large Text Mode** | 1.22&times; font scaling across all pages (idempotent, survives navigation) |
| ♿ **Screen Reader** | `SemanticScreenReader.Announce()` on every user action and state change |
| ☁️ **Mock API Ready** | Optional mockapi.io integration with automatic local fallback when offline |

---

## Built-in Food Items

| # | Name | Category | kcal | P | C | F |
|---|------|----------|------|---|---|---|
| 1 | Braised Beef Noodle Soup | Lunch | 580 | 32 | 68 | 18 |
| 2 | Bubble Milk Tea | Drink | 320 | 4 | 56 | 8 |
| 3 | Tomato Egg Rice Bowl | Lunch | 450 | 18 | 62 | 14 |
| 4 | Jianbing Pancake | Breakfast | 380 | 14 | 44 | 16 |
| 5 | Mala Stir-Fry Pot | Dinner | 680 | 42 | 28 | 38 |
| 6 | Xiaolongbao (Soup Dumplings) | Lunch | 420 | 24 | 38 | 20 |
| 7 | Soy Milk &amp; Youtiao | Breakfast | 350 | 12 | 42 | 16 |
| 8 | Mango Pomelo Sago | Drink | 280 | 3 | 48 | 10 |

*P = Protein, C = Carbs, F = Fat (all in grams)*

---

## Project Structure

```
TasteDiary/
├── App.xaml(.cs)                # Application entry point
├── AppShell.xaml(.cs)           # Shell: 3-tab bar + 2 push routes
├── MauiProgram.cs               # MAUI builder (fonts, debug logging)
├── GlobalXmlns.cs               # Global XAML namespace mapping
│
├── Models/
│   └── FoodItem.cs              # Data model with JSON serialisation
│
├── Services/
│   ├── FoodCatalogService.cs    # Data layer (mockapi.io + 8 local items)
│   ├── SpeechService.cs         # TTS wrapper (English locale, cancellation)
│   ├── AccessibilityService.cs  # Font scaling (1.22×, ConditionalWeakTable)
│   └── MockApiConfig.cs         # mockapi.io endpoint config
│
├── *.xaml(.cs)                  # 5 pages (see below)
│
├── Resources/
│   ├── AppIcon/                 # SVG app icon (green #2F6B45)
│   ├── Splash/                  # SVG splash screen
│   ├── Fonts/                   # OpenSans Regular & Semibold
│   ├── Images/                  # MAUI image resources
│   └── Raw/FoodImages/          # 8 embedded food photos
│
└── Platforms/
    ├── Android/                 # MainActivity, MainApplication
    ├── iOS/                     # AppDelegate, Program
    ├── MacCatalyst/             # AppDelegate, Program
    └── Windows/                 # App.xaml(.cs)
```

### Pages

| Page | Route | Purpose |
|------|-------|---------|
| `MainPage` | Tab &ldquo;Foods&rdquo; | Food list, search, pull-to-refresh, navigate to Add/Detail |
| `HardwarePage` | Tab &ldquo;Hardware&rdquo; | Camera, GPS, TTS, vibration, haptic feedback demo |
| `SettingsPage` | Tab &ldquo;Settings&rdquo; | Theme picker (System/Light/Dark), large-text toggle |
| `AddItemPage` | Push route | Validated form to add a new food record |
| `FoodDetailPage` | Push route `?id=...` | Nutrition detail with TTS read-aloud and vibration |

---

## Build &amp; Run

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- MAUI workload: `dotnet workload install maui`
- (Android) Android SDK via Visual Studio or standalone
- (Windows) Windows 10 19041+ or Windows 11

### Windows

```powershell
cd TasteDiary
dotnet run --framework net9.0-windows10.0.19041.0
```

### Android

```powershell
dotnet build -c Release -f net9.0-android
# Deploy the APK from bin\Release\net9.0-android\
```

Or open `TasteDiary.sln` in Visual Studio 2022, select an Android emulator, and press **F5**.

### Enable mockapi.io (Optional)

1. Create a resource at [mockapi.io](https://mockapi.io)
2. Set `EndpointUrl` in `Services/MockApiConfig.cs`
3. The app will use the remote API; if offline it falls back to local data automatically

---

## Marking Criteria Coverage

| Criterion (Weight) | Implementation |
|--------------------|----------------|
| **UI/UX &amp; Accessibility** (30%) | 5 XAML pages, warm earth-tone theme, dark mode, large-text scaling (1.22&times;), screen-reader announcements, semantic properties, WCAG-aligned colour contrast |
| **Mobile Hardware** (20%) | 6 hardware APIs: Camera, GPS, Geocoding, TTS, Vibration, Haptic Feedback |
| **Functionality** (20%) | Search, detail navigation, add record, pull-to-refresh, theme switching, empty states |
| **Validation &amp; Error Handling** (10%) | Form validation with user-friendly messages, try/catch on all hardware calls, permission-denied handling, graceful API fallbacks |
| **Code Quality** (10%) | Full XML documentation (`///`) on all public members, consistent naming, reusable helpers (`SetStatus`, `ApplyFontScale`, `Announce`), `sealed` classes, nullable reference types enabled |
| **Deployment** (5%) | Cross-platform targets: Android + Windows (builds for iOS and Mac Catalyst also configured) |
| **GitHub Usage** (5%) | Regular commits showing incremental development over multiple days |

---

## Mobile Hardware APIs

| # | API | Method | Page(s) |
|---|-----|--------|---------|
| 1 | **Camera** | `MediaPicker.Default.CapturePhotoAsync()` | Hardware |
| 2 | **GPS** | `Geolocation.Default.GetLocationAsync()` | Hardware |
| 3 | **Geocoding** | `Geocoding.Default.GetPlacemarksAsync()` | Hardware |
| 4 | **Text-to-Speech** | `TextToSpeech.Default.SpeakAsync()` | Hardware, Detail |
| 5 | **Vibration** | `Vibration.Default.Vibrate()` | Hardware, Detail, Add |
| 6 | **Haptic Feedback** | `HapticFeedback.Default.Perform()` | Hardware, Detail, Add |

Vibration and haptic feedback, which cannot be captured in emulator recordings, are verified via an on-screen counter that increments each time they are triggered.

---

## Accessibility

Accessibility features are aligned with the four WCAG principles:

| Principle | Implementation |
|-----------|---------------|
| **Perceivable** | Large-text mode (1.22&times;), high-contrast earth-tone palette, `SemanticScreenReader.Announce()` on all state changes |
| **Operable** | All controls keyboard/touch accessible, semantic heading levels on labels, clear `SemanticProperties.Hint` on buttons |
| **Understandable** | User-friendly validation errors (&ldquo;Please enter a food or drink name&rdquo;), confirmation alerts, descriptive status labels |
| **Robust** | `SemanticProperties.Description` on images, `ConditionalWeakTable` for idempotent font scaling, screen-reader-compatible navigation |



