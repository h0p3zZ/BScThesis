# 05.08.2025

This is a good start for the IR language.

Some notes:
- Priorities can take several forms: urgency != importance
- After/Before might be better expressed as either a boolean array for allowed slots or a sequence of time intervals.
- Let the front-end deal with dates and holidays
- You need to find a good way to make this work with reocurrences.
- Currently you can only express order dependencies (i.e. X happens before Y), but sometimes you also want time dependencies (X happens at most 1 week after and at least 2 days after Y).
- Instead of location, you could consider clustering constraints where some tasks happen preferably in clusters with each other.
- Duration can have many different variations (variable duration, task splitting, preferred duration). You should think about which of these you can already express within your language, and which ones would need new features.

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