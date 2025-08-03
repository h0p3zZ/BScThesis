# Project Overview: Constraint Language for Scheduling

## 1. Objective

Design a **simple constraint language** to represent user-defined scheduling instructions (e.g., "open from 9–5", "go there ASAP but not later than X", "low priority", etc.). This language should:

- Be **minimal** (e.g., use booleans or other primitive data types).
- Have a clear **correlation** between natural language user input and formalized constraints.

## 2. Language Definition

- Use **JSON** as the container format (acts as the syntax for a custom dialect).
- Define a minimal yet expressive schema for encoding constraints.
- Express constraints with **mathematical clarity**:
  - Specify **precisely** when a constraint file is considered valid.
  - Enable **parsing** and **semantic interpretation**.

## 3. Semantics & Validation

- A **schedule** (assignment of tasks to timeslots) must be checkable against the defined constraints.
- The system must:
  - Verify whether a given schedule **satisfies** the constraints.
  - Allow **comparison** of constraint sets or schedules using a **score** or **cost** metric.
  - Encode constraints in a way that is computationally meaningful and unambiguous.

## 4. Explicit Non-Goals

The project **does not** aim to:

- Handle timekeeping or time zone logic.
- Account for daylight saving time (summer/winter time).
- Add new constraints or increase expressiveness unnecessarily.
- Solve the scheduling problem (e.g., no SAT solving or optimization).
- Provide a working implementation (focus is on definition, not execution).

## 5. Extra Points (Optional)

- **Explain why** the system or encoding works (or fails).
- **Reduce** the constraint-checking system to a **Max-SAT** formulation as a bonus exercise.
