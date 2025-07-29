# 29.07.2025

The project will be focused on the intermediate representation.

## Goals

Minimum goals:
- Data format for the intermediate representation.
- Precise description of valid files, possibly starting off some preexistent format.
- Mathematical semantics for each file. This includes:
    * The set of possible solutions
    * The feasible solutions for a given IR file.
    * The fitness of a feasible solution for a given IR file.

Bonus goals:
- Implement a file validator.
- Solution format and checker.
- Expressing IR constraints as constraints in some solving paradigm (SAT/SMT/ILP/MIP)

Non-goals:
- Timekeeping, timezones.
- User level input: you can assume that the frontend does a bunch of menial work.
- Implementation in a solver.

## To do

Come up with a first attempt at the data format.