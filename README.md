# Seismic Repository

This repository includes documentation and implementation assets for the **Seismic Event Analytics & Instrument Integrity Platform**.

## Repository Contents

- [`TechnicalSpecification.md`](./TechnicalSpecification.md) — Full technical specification.
- [`src/Seismic.Analytics`](./src/Seismic.Analytics) — C# analytics pipeline scaffolding:
  - Onset detection
  - Distance estimation
  - Event classification baseline
  - Instrument integrity scoring

## Notes

The code in `src/Seismic.Analytics` is starter scaffolding intended to accelerate development against the specification. It should be production-hardened with tests, calibration, and API integration.
