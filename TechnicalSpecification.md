# Seismic Event Analytics & Instrument Integrity Platform

## Technical Specification Document

## 1. Purpose

Build a seismic analytics platform that ingests raw, multi-channel seismic CSV exports and computes:

- Event validity probability score
- Estimated blast distance via acoustic lag
- Instrument integrity score

The system provides probabilistic scoring and statistical diagnostics. It is **assistive, not authoritative**.

---

## 2. Data Input Specification

### 2.1 Event Window

Each event file contains:

- Fixed duration (target: ~10 seconds)
- Sample-rate metadata (`256`, `512`, or `1024` Hz)
- Pre-trigger buffer
- 4 synchronized channels:
  - Radial (`R`)
  - Transverse (`T`)
  - Vertical (`V`)
  - Air (`A`)

### 2.2 CSV Columns

Required columns:

1. Timestamp **or** sample index
2. `R` value
3. `T` value
4. `V` value
5. `A` value

Optional metadata:

- Logged distance
- PPV per axis
- Frequency
- R²
- Z-curve
- Trigger thresholds
- Event timestamp

### 2.3 Ingestion Validation Rules

1. File must include all four channels (`R/T/V/A`).
2. All channels must have equal sample count.
3. If timestamp is provided, sampling interval must be consistent.
4. Sample rate must be one of the supported rates unless custom override is enabled.
5. Missing optional metadata does **not** block processing; event is marked `metadata_partial`.

---

## 3. Core Module 1: Onset Detection Engine

### 3.1 Purpose

Determine precise onset times for seismic and acoustic signals.

### 3.2 Processing Steps

1. Identify pre-trigger region from metadata (or configured pre-trigger duration).
2. Compute baseline mean and standard deviation for each relevant channel.
3. Define dynamic threshold:

```text
Threshold = BaselineMean + k * BaselineStdDev
```

4. Detect first sustained crossing:
   - Seismic onset from seismic composite signal (`R/T/V`)
   - Acoustic onset from `A`
5. Compute lag:

```text
Δt = AcousticOnset - SeismicOnset
```

### 3.3 Recommended Defaults (Configurable)

- `k = 4.0` for onset thresholding
- Sustained crossing window: `10 ms`
- Refractory window after onset pick: `20 ms`
- Seismic composite mode: `max(|R|, |T|, |V|)`

### 3.4 Outputs

- Seismic onset time/index
- Acoustic onset time/index
- `Δt` lag (samples and seconds)
- Onset quality metrics (SNR, slope-at-onset, threshold margin)

---

## 4. Core Module 2: Distance Estimation Engine

### 4.1 Formula

```text
EstimatedDistance_m = Δt_seconds * SpeedOfSound_mps
```

Default speed of sound: `343 m/s` (temperature-adjustable).

### 4.2 Processing Steps

1. Convert `Δt` to seconds.
2. Compute estimated distance.
3. If logged distance exists, compute percentage deviation:

```text
DeviationPct = ((EstimatedDistance - LoggedDistance) / LoggedDistance) * 100
```

4. Compute confidence band from onset uncertainty and speed-of-sound uncertainty.

### 4.3 Outputs

- Estimated distance (m)
- Distance deviation (%) when logged distance is available
- Confidence band (lower/upper meters)

---

## 5. Core Module 3: Event Classification Engine

### 5.1 Purpose

Classify event as:

- Valid Blast
- False Trigger

### 5.2 Features

- `Δt` lag
- Peak amplitude per axis
- RMS energy
- Frequency-domain features
- Axis coherence
- Cross-channel correlation

### 5.3 Model Options

- Logistic Regression (baseline)
- Random Forest
- Gradient Boosting (XGBoost / LightGBM)

### 5.4 Inference Output

- Probability score (`0–100%`)
- Confidence category (`High`, `Medium`, `Low`)
- Optional top-feature contribution summary

### 5.5 Confidence Category Policy (Default)

- `High`: probability ≥ 80%
- `Medium`: probability 60–79.99%
- `Low`: probability < 60%

---

## 6. Core Module 4: Instrument Integrity & Regression Diagnostics

### 6.1 Detect

- Geophone decoupling
- Axis inconsistency
- Frequency distortion
- Baseline deviation

### 6.2 Diagnostic Features

- Axis-to-axis regression slopes
- R² stability metrics
- Energy symmetry ratios
- Rolling baseline comparison against instrument history

### 6.3 Scoring

Produce Instrument Health Score (`0–100`) from weighted sub-metrics:

- Baseline stability
- Axis consistency
- Frequency consistency
- Historical drift

Weights must be configurable and versioned.

### 6.4 Outputs

- Instrument Health Score (`0–100`)
- Anomaly flags
- Regression diagnostics summary

---

## 7. System Architecture Overview

### 7.1 Backend

- .NET Core API (signal processing + ML inference)
- .NET Core API (orchestration + reporting)
- Microsoft SQL Server (event and analytics persistence)

### 7.2 Processing Flow

1. CSV ingestion
2. Feature extraction
3. Onset detection
4. Distance estimation
5. Classification and health scoring
6. Result persistence
7. Report generation (`JSON` / `PDF`)

### 7.3 Deployment

- Cloud-hosted API
- Optional on-premise processing

---

## 8. Data Persistence (Minimum Schema)

### 8.1 `Events`

- `EventId` (PK)
- `IngestedAtUtc`
- `SampleRateHz`
- `DurationSeconds`
- `HasMetadataPartial` (bit)

### 8.2 `EventResults`

- `EventId` (FK)
- `SeismicOnset`
- `AcousticOnset`
- `DeltaTSeconds`
- `EstimatedDistanceM`
- `DeviationPct` (nullable)
- `ValidityProbability`
- `ValidityConfidence`
- `InstrumentHealthScore`

### 8.3 `EventFlags`

- `EventId` (FK)
- `FlagCode`
- `Severity`
- `DetailsJson`

### 8.4 `ModelRegistry`

- `ModelId` (PK)
- `ModelType`
- `ModelVersion`
- `FeatureSchemaVersion`
- `TrainedAtUtc`

---

## 9. API Contract (Reference)

### 9.1 Endpoints

- `POST /api/events/ingest`
- `POST /api/events/{eventId}/analyze`
- `GET /api/events/{eventId}/report?format=json|pdf`
- `GET /api/instruments/{instrumentId}/health`

### 9.2 Example Analysis Response

```json
{
  "eventId": "evt-2026-0001",
  "onset": {
    "seismicOnset": 1.842,
    "acousticOnset": 2.164,
    "deltaTSeconds": 0.322,
    "quality": {
      "snr": 13.5,
      "thresholdMargin": 2.1
    }
  },
  "distance": {
    "estimatedMeters": 110.45,
    "loggedMeters": 116.0,
    "deviationPct": -4.78,
    "confidenceBandMeters": [105.1, 115.8]
  },
  "classification": {
    "probabilityValidBlast": 0.86,
    "confidenceCategory": "High"
  },
  "instrumentIntegrity": {
    "healthScore": 91,
    "flags": ["NONE"],
    "regressionDiagnostics": {
      "r2": 0.95,
      "slopeRT": 0.99,
      "slopeRV": 1.03
    }
  }
}
```

---

## 10. Non-Functional Requirements

- Deterministic and auditable outputs
- Full traceability of model version and feature schema
- Robust malformed-file handling
- Configurable batch and near-real-time operation
- Secure storage and controlled report access

---

## 11. Acceptance Criteria (MVP)

1. Ingest valid multi-channel CSV and reject invalid schema with explicit diagnostics.
2. Produce seismic onset, acoustic onset, and `Δt` for each processed event.
3. Produce estimated distance and deviation if logged distance exists.
4. Produce event validity probability and confidence category.
5. Produce instrument health score and anomaly flags.
6. Persist event outputs and generate JSON report (PDF optional for MVP).

---

## 12. Disclaimer

This platform provides statistical decision support and does not replace engineering judgment, regulatory obligations, or site-specific blast verification procedures.
