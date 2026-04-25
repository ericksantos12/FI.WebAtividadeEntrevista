# Repository Guidelines

## Project Structure & Module Organization

This repository contains a .NET Framework 4.8 ASP.NET MVC solution.

- `FI.WebAtividadeEntrevista/`: web application with MVC controllers, views, models, JavaScript, CSS, bundles, routes, and `Web.config`.
- `FI.AtividadeEntrevista/`: class library with domain models, business logic, and data access.
- `FI.WebAtividadeEntrevista/App_Data/`: LocalDB `.mdf` database files used by the web app.
- `FI.WebAtividadeEntrevista/Scripts/Clientes/`: page-specific client scripts for customer listing, creation, and editing.
- `FI.AtividadeEntrevista/DAL/Clientes/Procedures/`: SQL stored procedure scripts.
- `specs/`: implementation specifications, organized by incremental folders.
- `packages/`: NuGet packages restored in classic `packages.config` style.

There is currently no dedicated test project in the solution.

## Build, Test, and Development Commands

Build the web project:

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" FI.WebAtividadeEntrevista\FI.WebAtividadeEntrevista.csproj /p:Configuration=Debug /p:Platform=AnyCPU
```

Build the full solution from Visual Studio using `Build > Rebuild Solution`.

Run locally through Visual Studio or IIS Express. The web project is configured for IIS Express at `https://localhost:44333/`.

If runtime compilation fails with a missing `bin\roslyn\csc.exe`, rebuild the web project so the Roslyn compiler files are copied to `FI.WebAtividadeEntrevista/bin/roslyn`.

## Coding Style & Naming Conventions

Use C# conventions already present in the codebase: PascalCase for classes, methods, and properties; camelCase for local variables. Keep indentation at four spaces.

Follow the existing layered naming style:

- Controllers: `ClienteController`
- MVC models: `ClienteModel`
- Business classes: `BoCliente`
- Data access classes: `DaoCliente`
- Domain entities: `Cliente`

Prefer explicit, small methods over broad refactors. Keep MVC validation in view models and persistence logic in DAL classes.

## Specification Files

Before implementing planned work, create or update a specification under `specs/`. Use one incremental folder per implementation, named with a three-digit code and short slug, for example `specs/001-cpf-cliente/`. Each folder must contain `SPEC.md` for scope, affected files, database impact, and acceptance criteria, plus `TASKS.md` with implementation tasks formatted as Markdown checklists. Do not leave implementation specs in the repository root.

## Testing Guidelines

No automated test framework is currently configured. For changes, perform manual validation through the MVC screens:

- `/Cliente/Index` for listing and paging.
- `/Cliente/Incluir` for customer creation.
- `/Cliente/Alterar/{id}` for editing.

When adding tests, create a separate test project and name tests by behavior, for example `Incluir_DeveRetornarErro_QuandoModelInvalido`.

## Commit & Pull Request Guidelines

This workspace does not include Git history, so no existing commit convention can be inferred. Use concise imperative commit messages, for example:

```text
Add cliente CPF validation
Fix Roslyn compiler copy on build
```

Pull requests should include a short summary, affected layers, manual test steps, database or stored procedure changes, and screenshots for visible UI changes.

## Security & Configuration Tips

Do not commit machine-specific connection strings or credentials. The current LocalDB connection is defined in `FI.WebAtividadeEntrevista/Web.config` and points to `App_Data/BancoDeDados.mdf`. Keep SQL access parameterized through `SqlParameter`; do not concatenate user input into SQL commands.
