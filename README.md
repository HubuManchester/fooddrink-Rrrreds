# 食味记 (TasteDiary)

A .NET MAUI cross-platform mobile app for recording and exploring Chinese cuisine — nutrition tracking, food photography, and meal context.

## Coursework

6G6Z0014 Mobile Computing — Final Assignment. Food & Drink theme.

## Features

- Browse and search Chinese food and drink records
- View nutrition details (calories, protein, carbs, fat)
- Add new food/drink records with validation
- Camera capture for food photos
- Device location and geocoding for meal places
- Text-to-speech for nutrition summaries
- Vibration and haptic feedback
- Light/dark theme and large-text accessibility mode

## Project structure

```
TasteDiary/
  Models/FoodItem.cs       — Data model
  Services/                 — FoodCatalogService, SpeechService, AccessibilityService
  Pages (XAML + code-behind) — MainPage, AddItemPage, FoodDetailPage, HardwarePage, SettingsPage
```

## Build

```powershell
# Windows
dotnet build .\TasteDiary\TasteDiary.csproj -f net9.0-windows10.0.19041.0

# Android
dotnet build .\TasteDiary\TasteDiary.csproj -f net9.0-android
```
