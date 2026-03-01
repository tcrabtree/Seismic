# Seismic Analytics POC – Codex Agent Rules

## ROLE

You are operating as a deterministic source-code generator for a Seismic Event Analytics Proof-of-Concept.

This is an engineering validation tool.
It is NOT a production SaaS system.

You generate static source code only.

---

## EXECUTION POLICY (CRITICAL)

Execution is NOT permitted.

You must NOT:

- Run dotnet commands
- Check SDK versions
- Execute builds
- Launch browsers
- Run Playwright
- Take screenshots
- Inspect environment
- Perform runtime validation

Assume:
- .NET 9 SDK is installed
- Project builds successfully
- Browser rendering works
- Dependencies are available

Your job is to generate code only.

Do not attempt verification.

---

## STACK

- .NET 9
- ASP.NET Core MVC (Razor Views)
- Bootstrap 5 (CDN)
- Chart.js
- SQL Server
- CsvHelper for CSV parsing
- Desktop-first UI

No authentication.
No background workers.
No cloud services.

---

## UI DESIGN PRINCIPLES

- Engineering aesthetic (not SaaS dashboard style)
- Dense but readable
- Thin borders
- Minimal whitespace
- No animations
- No marketing language
- Print-friendly when applicable

Use:
- table-bordered table-sm for report tables
- Shadowless layout
- Neutral color palette

Status colors:
- Green = OK / High confidence
- Yellow = Warning
- Red = Risk
- Blue = Informational

---

## CODE STYLE RULES

- Keep controllers thin
- Use ViewModels (never bind EF entities directly to views)
- Separate parsing/services into dedicated classes
- Avoid overengineering
- Avoid unnecessary abstractions
- Keep files concise and readable

No speculative architecture.
No microservices.
No event buses.

---

## CSV EVENT RULES

Event CSV format:

Required columns:
Time,R,T,V,A

Parsing rules:
- Use CsvHelper
- Validate headers strictly
- Reject malformed rows
- Compute:
  - Max amplitude per channel
  - Duration
  - Approximate onset time (threshold-based)

Store raw CSV to:
App_Data/Events/{guid}.csv

Each file = one Event.

---

## WAVEFORM DISPLAY RULES

When rendering waveform:

- Use one Chart.js canvas
- Vertically separated channel bands
- Order top-to-bottom:
  R
  T
  V
  A
- No overlapping channel lines
- No smoothing (tension = 0)
- Thin lines
- Engineering appearance

---

## REPORT STYLE RULES

Event Detail page should resemble a traditional seismic monitoring printout:

- Structured header block
- Dense bordered tables
- Unified seismograph record
- No modern card UI
- No rounded corners
- No shadows

Must remain print-friendly.

---

## RESPONSE BEHAVIOR

When generating code:

- Output only necessary files
- Do not restate instructions
- Do not explain obvious steps
- Do not attempt to validate runtime
- Do not add extra features not requested

Be precise.
Be minimal.
Be deterministic.

---

## OUT OF SCOPE

Do not:

- Add authentication
- Add user roles
- Add cloud integrations
- Add background processing
- Add CI/CD
- Add logging frameworks unless requested

---

## PRIMARY OBJECTIVE

Support rapid engineering validation of:

- Δt lag detection
- Distance estimation
- Event classification
- Instrument health diagnostics

The UI exists to accelerate technical validation — not product polish.
