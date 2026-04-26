# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Common commands

### Build
Build the web project:

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" FI.WebAtividadeEntrevista\FI.WebAtividadeEntrevista.csproj /p:Configuration=Debug /p:Platform=AnyCPU
```

Build the full solution:

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" FI.WebAtividadeEntrevista.sln /p:Configuration=Debug /p:Platform="Any CPU"
```

### Run locally
Run through Visual Studio with IIS Express. The web project is configured for:

```text
https://localhost:44333/
```

If runtime compilation fails because `bin\roslyn\csc.exe` is missing, rebuild `FI.WebAtividadeEntrevista` so Roslyn compiler files are copied into `FI.WebAtividadeEntrevista/bin/roslyn`.

### Tests and validation
There is no automated test project or lint configuration in this repository today.

Manual validation happens through the MVC screens:

- `/Cliente/Index` for listing and paging
- `/Cliente/Incluir` for customer creation
- `/Cliente/Alterar/{id}` for editing

There is currently no single-test command because no test project is configured.

## High-level architecture

This is a .NET Framework 4.8 ASP.NET MVC solution with two projects:

- `FI.WebAtividadeEntrevista`: MVC web app with controllers, Razor views, client scripts, bundles, routes, and `Web.config`
- `FI.AtividadeEntrevista`: class library containing domain models (`DML`), business logic (`BLL`), validation, and data access (`DAL`)

The application uses a LocalDB database attached from `FI.WebAtividadeEntrevista/App_Data/BancoDeDados.mdf`. SQL changes are represented as stored procedure or migration-style scripts under `FI.AtividadeEntrevista/DAL/Clientes/Procedures/`.

## Request and data flow

The main vertical slice is the cliente workflow:

1. Razor views under `Views/Cliente/` render the screens.
2. Page-specific jQuery scripts under `Scripts/Clientes/` submit AJAX requests and configure the jTable grid.
3. `ClienteController` serves both HTML views and JSON endpoints for create, update, and paged listing.
4. MVC model binding targets `ClienteModel`, which carries DataAnnotations validation.
5. `BoCliente` handles business-layer normalization and delegates to the DAL.
6. `DaoCliente` calls stored procedures through `AcessoDados` using `SqlParameter` lists.
7. Stored procedures read and write the LocalDB `CLIENTES` table.

When changing cliente fields, keep these layers in sync: Razor form, page script payloads, MVC model, domain entity, business normalization, DAL parameter mapping, and stored procedures.

## Important implementation patterns

- Routing is conventional MVC routing from `App_Start/RouteConfig.cs`: `/{controller}/{action}/{id}`.
- Client-side behavior is bundle-based in `App_Start/BundleConfig.cs`:
  - `~/bundles/clientes` for create form submission
  - `~/bundles/altClientes` for edit form prefill/submission
  - `~/bundles/listClientes` for the jTable listing screen
- `Views/Cliente/Forms.cshtml` is the shared form partial used by both `Incluir` and `Alterar`.
- The list endpoint is shaped for jTable and returns `{ Result, Records, TotalRecordCount }`; preserve that contract if the grid behavior changes.
- CPF handling is intentionally enforced in multiple layers:
  - input masking in `Views/Cliente/Forms.cshtml`
  - model validation via `Models/Validacao/CpfValidationAttribute.cs`
  - normalization in `FI.AtividadeEntrevista/Validacao/Cpf.cs` and `BoCliente`
  - duplicate checks in `FI_SP_VerificaCliente.sql`
  - database uniqueness via `FI_ALTER_CLIENTES_ADD_CPF.sql`
- DAL access is stored-procedure based and parameterized. Follow the existing `SqlParameter` pattern in `DaoCliente` and avoid introducing inline SQL.
- The web project has mixed namespaces (`FI.WebAtividadeEntrevista` and `WebAtividadeEntrevista`). Follow the surrounding file’s existing namespace instead of trying to normalize namespaces as part of unrelated work.

## Repository-specific workflow

Before planned implementation work, create or update a spec folder under `specs/NNN-short-slug/` containing:

- `SPEC.md` for scope, affected files, database impact, and acceptance criteria
- `TASKS.md` as a Markdown checklist of implementation tasks

Do not place implementation specs in the repository root.

If you add automated tests later, create a separate test project and name tests by behavior.