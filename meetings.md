# Reviewing commit `dfb39b1ce14f26e1b641145f9a5f39b99c7a0707`

Nice work! I think you got the point of the assignment. I really like that you wrote down your rationale for each design decision; this makes it very easy to follow what you did. I'm leaving some notes below. Regarding semantics, I believe they are alright, but they will become a bit clearer once the issues with time representation below are tackled.

## On importance vs urgency

If I understand correctly your current version, priority has been replaced by importance and urgency. Neither feature has an impact on when a plan is a feasible solution; rather, they just tell us when a solution is preferable to another solution. Just so we don't get mired in the details, I suggest that for now we don't try to give hard definitions about what effect importance and priority will have. However, you should keep them in mind that they *exist* while desigining the rest of the IR language.

## Time representation

Much of your writeup revolves around how to make timeslots fit with dates (and window preferences). This is probably something we should talk about on Tuesday in more detail; I'm afraid I don't fully understand what's going on in there. But let me throw some ideas here for now:

* Sometimes tasks have *weird* schedules where they can or cannot be done. Just as an example, our secretary at SAI works on Tuesday *mornings* and Wednesday *afternoons*. Your representation should be able to handle that schedule. I'm not entirely sure it currently is.

* You mention at some point that if we just write a huge Boolean array for timeslots this would be a problem because the array is sparse. I think at the scales we expect to handle this is not a problem: memory is not *that* limited a resource, bandwidth is irrelevant here (since if this is done on a server-client protocol, the request would contain the user-level representation rather than the IR), and if you end up generating a MaxSAT query you're going to need to go down to a Boolean array anyway. However, there are some reasons against windows as Boolean arrays related to how different solvers behave performance-wise. That discussion is a bit long, so let's leave it for Tuesday.

* There might be a third option other than huge Boolean arrays or dates + timeslots. Say we have a horizon of 30 days and our timeslot length is 15 minutes (so, 2880 timeslots in total). Any schedule you could think (assuming that schedule boundaries align neatly with the 15-minute boundaries) can be expressed as a union of intervals:
`[ [a1:b1], [a2:b2], [a3:b3], ... ]`. This would be less resource-intensive than the Boolean array without sacrificing expressivity. To some extent, this is what you compute as `W_shifted`.

* You allow `b` to be infinite, and then you use it to compute `b - timeslots.shift + 24k`, do you see the problem there? `:)`

## Location as dependencies

You currently propose to handle location as a dependency and letting the frontend figure those out from location tags and other external sources. The idea to let the frontend do the dirty work is good, but I think dependencies don't quite cover the idea of location, and conversely, dependencies might introduce unwanted behavior in locations.

Say, you need to go shopping groceries, and you also need to buy ink for your printer; it just so happens the supermarket is next to the stationery shop.

For the first problem, let's assume both shops have the same schedule. You need to go shopping twice within the horizon (so, tasks `A1` and `A2`), but you only need to buy ink once (say, task `B`). To handle this via dependencies, you would need to set `B` to depend on `A1` with `minAfter = maxAfter = 0`. Problem is, you might be forced (by other constraints) to schedule `A1` on a particularly busy day; in that situation, you'd expect the engine to schedule `B` after `A2` instead. But that won't happen, because `B` is explicitly linked to `A1`, not `A2`.

For the second problem, let's assume the supermarket (task `A`) closes at 20:00 but the stationery shop (task `B`) closes at 18:00. Let's also assume, for the sake of the argument, that you have something else to do (task `C`) exactly until 17:45. As before, we model locations via dependencies, so `B` depends on `A` with `minAfter = maxAfter = 0`. Now, there is an obvious schedule here: `C` is scheduled until 17:45, then task `B` is scheduled at 17:45-18:00, then task `A` is scheduled at 18:00-18:15. However, this won't happen, because `B` depends on `A`, not the other way around. So, the engine would instead schedule `A` on some other day at an earlier time so you can go afterwards to the stationery shop. I think we can call this a bug.


# 05.08.2025

This is a good start for the IR language.

Some notes:
- Priorities can take several forms: urgency != importance
- After/Before might be better expressed as either a boolean array for allowed slots or a sequence of time intervals.
- Let the front-end deal with dates and holidays
- You need to find a good way to make this work with reoccurrences.
- Currently, you can only express order dependencies (i.e. X happens before Y), but sometimes you also want time dependencies (X happens at most 1 week after and at least 2 days after Y).
- Instead of location, you could consider clustering constraints where some tasks happen preferably in clusters with each other.
- Duration can have many variations (variable duration, task splitting, preferred duration). You should think about which of these you can already express within your language, and which ones would need new features.

## To do

Reiterate the IR language design.

# 29.07.2025

The project will be focused on the intermediate representation.

## Goals

Minimum goals:
- Data format for the intermediate representation.
- Precise description of valid files, possibly starting off some preexisting format.
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