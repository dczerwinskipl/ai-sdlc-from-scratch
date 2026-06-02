# Reference App — Booking System

The booking system used across all lessons in the [From Vibe Coding to AI Workflow](https://dczerwinski.pl) series.

## What this is

A small .NET 10 modular monolith with two modules: `RoomManagement` and `Reservations`. Intentionally minimal — in-memory infrastructure, no database, no authentication.

## Intentional flaws

The app contains deliberate design problems. This is not low quality for its own sake — it is the material the lessons work on.

The flaws are there to make a specific point visible: without structured AI instructions and a defined workflow, a model will make the simplest, fastest decision at every step. Each individual choice looks reasonable. The accumulated result is a codebase that started as a modular monolith and drifted toward a tightly-coupled ball of mud — god entities, high coupling (temporal and logical), low cohesion, blurred module boundaries.

In a real project this happens feature by feature, quietly. The app is small enough that the drift is obvious and the before/after fits in a single lesson.

## How it evolves

The app state at the start of each lesson is the baseline for that lesson. Each lesson introduces a new practice and shows what changes when the model operates with and without it. The `app/` folder here is the current working state — specific lesson snapshots live inside their respective lesson folders.

## Structure

```
booking-system/
  src/BookingSystem/         — ASP.NET Core Minimal API, two modules
  tests/BookingSystem.Tests/ — Unit and integration tests
```

Open `booking-system/` as a separate project in your editor:

```
code app/booking-system/
```
