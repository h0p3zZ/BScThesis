# 13.10.2025

- Either `dep` should be a partial function at the `T x T` mapping,
or `dep` should be a total function `T x T --> Z --> N U {inf}`.
- `loc` has some weird behavior because it's not symmetric. We can either go with this and see if it creates an issue in the future, or alternatively include in the mathematical definition a finite set of locations `L`, and then `loc` is a function `T --> L x (N U {inf})`.
- Probably you're going to need another function in the math description `dur` for duration `T --> N`.
- Assignments need to account for unaccomplished tasks, e.g. by having `T --> N U {nope}`.
- The feasibility condition on overlapping tasks should take into account the duration of the task. Same for the horizon condition.
- The dependency condition will change depending on your new formalization.
- The location condition is not taking into account when do two tasks happen one after another.
- Cost: redo.

# 03.10.2025

I see you wrote two sections on data description. Just to be clear, what you need at this stage are three things.

## Data format description

This should be a detailed description of the fragment of JSON you would be targetting as a valid input file.
This is roughly what you did in the first Data Description section.
You can assume the input is valid *as a JSON file*; your goal is to describe unambiguously which valid JSON files
are valid input files.

What you wrote here is essentially correct; I wouldn't touch it anymore for the time being and come back to it fix what I write below.
The data you described is expressed as math. This is appropriate for the next two sections, but at this point you are still dealing with a computer file.
So, what I would expect to see is something along the lines of:

```
<file>: a JSON object containing exactly the following fields:
    "horizon": a JSON integer
    "duration" a JSON integer
    "importance": either a JSON integer or the string "inf"
    "timeSlots": a list of <timeslot>
    ...

<timeslot>: a JSON object containing exactly the following fields:
    "interval": a list of JSON integers of length 2
    "cost": a JSON integer
...
```

## Mathematical problem definition

I'm assuming this is what the second Data Description section does. This is quite good for a first approach! Some comments on this:
- You can simply say that "tasks" is a finite set. A set of what, you may ask? Of whatever.
In your formalization you only need to be able to tell when two tasks are the same or not, which is always assumed to be possible in math.
- If I understand correctly, in "dependencies", you are trying to define the following idea:
for *some* pairs of tasks a, b, there is a cost associated to scheduling b after a depending on how long the delay between the two tasks is.
I think this is close to a good definition, but maybe it's better if you define it as a *partial* function `dep: T \times T \times \N_0 -> \N_0 U {\infty}`.
Think about it and let's discuss it in our next meeting.
- I don't think I understand what's going on with locations, but let's discuss that in the meeting.
- I don't agree that the time slot function captures importance, duration and urgency, but I might be missing something.

## Search space

Here you wrote a class definition, but what we need is a mathematical description of "something we can start talking about whether it is a solution or not".
Either way, don't worry too much, let's talk about it.

# 17.09.2025

- If you're defining some feature e.g. duration, for each task t, it makes sense to add t as a subscript.
- Timeslots are currently defined close to their JSON representation, but it doesn't need to be like that. It might save you work if you define it in terms of a mathematical function (and similarly in other places).
- Defining a search space is analogous to defining a data structure that will contain the type of your potential solutions.

# 02.09.2025

- Currently your dependencies can only handle a hard delay interval (minAfter, maxAfter). You could pull off the same trick as with the timeslots and have multiple allowed delays with different costs.
- Currently locations are missing costs. This is not too critical for now, just leaving it here as a reminder.
- The constraint duration < horizon is not really needed, the solver can take care of that.
- Beyond the list of tasks, there are some properties that are global for a problem (e.g. the horizon). These are currently not accommodated in your file format.
- Specifying the semantics will become a lot easier if you separate your levels of abstraction:
    1. What data defines a scheduling problem?
    2. What data is a potential solution for a fixed scheduling problem? (i.e. the search space)
    3. When is a potential solution an actual solution of a scheduling problem? (i.e. feasible solutions)
    4. When is an solution preferable to another solution (i.e. optimal solutions)
- Remember when writing the specifications above that math is not constrained by computation: you have access to constructions that do not exist as data structures, and it's ok to define constructions that you don't know how to compute (as long as they're unambiguously defined).

## To do

- Improve dependency delays in the file format.
- Give a full file format description, including global features e.g. horizon.
- Separate semantics in the levels described above.

# 26.08.2025

- We talked about the points I raised in the last review.
- Duration: it doesn't seem to make much sense to "split" tasks but rather to have independent tasks that are in some way connected by constraints (dependencies, or locations).

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