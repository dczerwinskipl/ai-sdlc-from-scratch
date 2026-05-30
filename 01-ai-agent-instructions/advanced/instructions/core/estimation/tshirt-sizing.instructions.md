<!-- Type: reasoning -->

# T-Shirt Sizing

Use t-shirt sizing to represent relative implementation complexity across solution models.

Size represents relative implementation complexity, architectural impact, uncertainty, and regression scope. It is not a time estimate.

## Size definitions

| Size | Meaning |
|---|---|
| XS | Trivial local change. No domain decision, persistence change, or contract change. |
| S | Small local change inside one module. Known pattern, low uncertainty. |
| M | Moderate feature or rule change. One main module, some persistence, contract, or test impact. |
| L | Significant domain change. Affects lifecycle, aggregate boundaries, or multiple behaviors. |
| XL | Architectural change. Multiple modules, ownership changes, new contracts, migration, broad regression scope. |
| XXL | Too large for one implementation slice. Must be split before implementation. |

## Usage rules

- Assign a size to each solution model in option analysis.
- If a model is XL or XXL, propose smaller implementation slices.
- For small-scope changes (XS/S), simplify the evaluation — focus on AC coverage, benefits, risks, and complexity. Skip fields that would be identical or empty for all models.
- Do not use size as a tie-breaker between models that have different domain risk profiles. A smaller model with higher domain risk is not automatically better.
