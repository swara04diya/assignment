# assignment


@'
# Minimal Workflow Engine Backend (ASP.NET 8)

A tiny yet fully‑functional backend service that lets you **define** state‑machine workflows, **start** workflow instances, **execute** transitions with validation, and **inspect** definitions & instances. It’s purposely lightweight—an in‑memory store by default—with clean boundaries so you can swap in a database or message bus later.

---

## Table of Contents
- [Features](#-features)
- [Architecture Overview](#-architecture-overview)
- [Folder Structure](#️-folder-structure)
- [Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [Clone](#clone)
  - [Run Locally (SDK)](#run-locally-sdk)
  - [Run in Docker (no-sdk)](#run-in-docker-no-sdk)
  - [Quick Windows PowerShell Bootstrap](#quick-windows-powershell-bootstrap)
- [API Reference](#-api-reference)
  - [Health](#health)
  - [Workflow Definitions](#workflow-definitions)
  - [Workflow Instances](#workflow-instances)
  - [Error Format](#error-format)
- [Sample Workflow Walkthrough](#-sample-workflow-walkthrough)
- [curl Examples](#curl-examples)
- [Build & Test](#-build--test)
- [Design Notes & Tradeoffs](#-design-notes--tradeoffs)
- [Extending the Project](#-extending-the-project)
- [Interview / Discussion Prompts](#-interview--discussion-prompts)
- [Contributing](#-contributing)
- [License](#-license)

---

## ✨ Features

| Capability | Details |
|------------|---------|
| **Workflow definition** | Upload a JSON payload describing **states** & **actions** (transitions). Exactly one state must be `isInitial=true`. |
| **Runtime execution**   | Start an instance from a chosen definition; execute an action only if it’s enabled, valid from the current state, and the target state exists. |
| **History & inspection** | Each transition is recorded with timestamp; you can fetch instance + history or list all running instances. |
| **Validation** | Clear error codes & HTTP 400 *ProblemDetails* for any invalid definition or action. |
| **Persistence** | 🔌 Default is in‑memory (thread‑safe). Swap one line to use JSON‑file snapshot persistence, or implement `IWorkflowRepository` for a DB. |
| **Extensible design** | Domain layer is decoupled from API & persistence; add auth, metrics, or alternate transports (gRPC, SignalR) without touching core rules. |

---

## 🧠 Architecture Overview

This project follows a **clean-ish, layered** mini‑architecture:
      +-----------------------------+
      |         HTTP API            |
      | (Minimal APIs in Program.cs)|
      +-------------+---------------+
                    |
                    v
           +------------------+
           |   Services       |  <-- business orchestration / use cases
           +------------------+
                    |
      +-------------+-------------+
      |                           |
      v                           v
      +---------------+ +--------------------+
| Domain | | Persistence |
| (State, etc.) | | IWorkflowRepository|
+---------------+ +--------------------+

- **Domain**: Pure models, very little behavior (state, action, definition, instance, history).
- **Services**: Enforce rules; separate **definition** operations from **runtime** ops.
- **Persistence**: Swappable storage (in‑memory, JSON file sample).
- **HTTP API**: Thin mapping layer (DTOs in /ApiModels) + minimal endpoints.

---

## 🗂️ Folder Structure
WorkflowEngine/ ← project root (.csproj here)
│
├── ApiModels/ ← DTOs used by the HTTP API
├── Domain/ ← Pure domain entities (State, Action, WorkflowDefinition, WorkflowInstance)
├── Persistence/ ← Storage abstractions + in‑memory & JSON implementations
├── Services/ ← Application services (definition + runtime) + mapping helpers
├── Validation/ ← ValidationError, ValidationException, DefinitionValidator
├── Program.cs ← Minimal API host & endpoint mappings
├── appsettings*.json ← config stubs (unused for now)
└── README.md ← this file


> **Why this layout?**  
> *Domain* is infrastructure‑free (testable, portable). *Services* capture use‑case logic. The API layer is a thin adapter. Persistence is behind an interface so future storage swaps don’t ripple through your code.

---

## 🚀 Getting Started

### Prerequisites
Pick one of the following:
- **.NET 8 SDK** (recommended for local dev) – verify: `dotnet --info`.
- **Docker** – if you don’t want to install the SDK.

> Windows users: If `dotnet` isn’t found in VS Code terminal, add `C:\Program Files\dotnet` to PATH or reinstall the SDK.

---

### Clone

```bash
git clone https://github.com/<your-user>/workflow-engine.git
cd workflow-engine/src/WorkflowEngine

