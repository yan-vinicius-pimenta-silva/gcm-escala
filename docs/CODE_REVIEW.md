# Code Review - Escala GCM App

## Backend

### `Program.cs`

| Line | Severity | Issue |
|------|----------|-------|
| 20 | High | **Hardcoded JWT fallback key.** If `appsettings.json` is misconfigured in production, the weak secret `"EscalaGcmSuperSecretKeyForDevelopment2024!"` silently applies. Throw an exception if `Jwt:Key` is missing in non-Development environments. |
| 70-76 | Medium | **Hardcoded CORS origin.** `localhost:5173` is hardcoded. Use `appsettings.json` to configure per environment. |
| 83-86 | Medium | **Auto-migrate on every startup.** `db.Database.Migrate()` runs on boot. Risky in production -- migrations should be a deliberate deployment step. |

### `AuthService.cs`

| Line | Severity | Issue |
|------|----------|-------|
| 40 | High | **Duplicated JWT key string.** Same fallback key appears in `Program.cs` and here. If one changes, token validation breaks silently. Extract to a shared constant or inject the resolved key. |
| 54 | Low | **Hardcoded token lifetime.** 8-hour expiration is hardcoded. Make configurable via `appsettings.json`. |

### `SeedData.cs`

| Line | Severity | Issue |
|------|----------|-------|
| 17-38 | Medium | **Weak seed passwords.** `admin123`, `operador123`, `consulta123` are trivially guessable. Acceptable for dev, but the only production guard is `if (await context.Usuarios.AnyAsync())` -- add an environment check. |

### `ConflictValidationService.cs`

| Line | Severity | Issue |
|------|----------|-------|
| 20-37 | High | **N+1 query problem.** Each allocation triggers individual DB calls (`FindAsync` per guard, query per team). Step 3 (lines 48-73) re-queries the same teams. Batch-load all guards and team members upfront in a single query. |

### `EscalaService.cs`

| Line | Severity | Issue |
|------|----------|-------|
| 71, 119 | Medium | **`DateOnly.Parse` without error handling.** Throws `FormatException` on malformed input, resulting in a 500. Use `TryParse` and return a validation error. |
| 87-98 | Medium | **Two `SaveChangesAsync` calls without transaction.** Item is saved first, then allocations. If the second save fails, an orphaned `EscalaItem` remains. Use a single save or wrap in a transaction. |
| 179-190 | Low | **Manual cascade delete.** Allocations and items are removed manually in code. Configure `OnDelete(DeleteBehavior.Cascade)` in EF model config so the DB handles this atomically. |

### `EscalasController.cs`

| Line | Severity | Issue |
|------|----------|-------|
| 10 | High | **No role-based authorization.** `[Authorize]` without role checks means a `Consulta` user can create, delete, and publish escalas. Add `[Authorize(Roles = "Admin,Operador")]` on mutation endpoints. |
| 54, 62, 69 | Low | **Cryptic variable names.** `(s, e)` for destructured results hurts readability. Use `(success, error)`. |

### `Repository.cs`

| Line | Severity | Issue |
|------|----------|-------|
| 9 | Low | **`SaveChangesAsync` exposed per-repository.** Any service can flush the entire `DbContext`, breaking unit-of-work boundaries. Consider a separate `IUnitOfWork` interface. |

### `AppDbContext.cs`

| Line | Severity | Issue |
|------|----------|-------|
| 36-46 | Low | **`CreatedBy`/`UpdatedBy` never populated.** The `SaveChangesAsync` override only sets timestamps. Inject `IHttpContextAccessor` and set them from JWT claims, or remove the fields from `AuditableEntity`. |

---

## Frontend

### `client.ts`

| Line | Severity | Issue |
|------|----------|-------|
| 18-21 | Medium | **Hard redirect destroys React state.** `window.location.href = '/login'` forces a full page reload. Use the router's `navigate` or clear auth context and let the route guard redirect. |

### `AuthContext.tsx`

| Line | Severity | Issue |
|------|----------|-------|
| 17-19 | Low | **Three separate `useState` calls.** Causes 3 re-renders on login with potentially inconsistent intermediate states. Combine into a single state object. |
| 8 | Low | **`perfil` typed as `string \| null`.** Loses enum type safety. Define a union type `'Admin' \| 'Operador' \| 'Consulta'`. |

### `MainLayout.tsx`

| Line | Severity | Issue |
|------|----------|-------|
| 127-133 | Medium | **Hidden pages stay mounted in DOM.** Pages rendered with `display: none` keep all queries, event listeners, and timers active. Trades memory and network overhead for preserved tab state. Consider persisting only the relevant data instead. |

### `useEscalas.ts`

| Line | Severity | Issue |
|------|----------|-------|
| 9-11 | Low | **`enabled: false` with manual `refetch`.** Bypasses TanStack Query's reactive paradigm. Let the query be enabled based on whether filters are set. |

### `EscalasPage.tsx`

| Line | Severity | Issue |
|------|----------|-------|
| 36 | Low | **Unsafe type cast.** `setor?.tipo as TipoSetor` trusts the API string matches the enum at runtime. Use a type guard or validation. |
| 54, 69, 88, 99 | Low | **`err: any` throughout.** All catch blocks use untyped errors. Define a typed `AxiosError` shape for type-safe error access. |

---

## Summary

| Category | Count | Highlights |
|----------|-------|------------|
| **High** | 3 | Missing role-based auth, hardcoded JWT secret, N+1 queries |
| **Medium** | 5 | No transaction on multi-save, auto-migrate, hidden pages mounted, hard redirect, weak seed passwords |
| **Low** | 7 | Hardcoded token lifetime, cryptic names, untyped errors, unused audit fields |
