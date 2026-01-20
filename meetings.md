# 20.01.2026

I'm collecting here some general issues with the current writeup. Particular issues are addressed in the notes inside the thesis. The first two are major issues, but I think they won't take a lot of time to fix (it's mostly moving things around and writing a few additional paragraphs). The third is also a major issue and requires a bit of writing. The rest are rather minor issues, but they happen in several places or it was just easier to write them here.

### Preliminaries

I think the current structure in the thesis is a bit mixed up. Broadly, you should have 5 chapters (although you might split some of these into several; they don't need to have the titles I'm writing here):

1. Introduction: what problem you're solving, and why (without assuming the reader knows the context)
2. Preliminaries: what you need to know to understand the contribution
3. Methodology: how you're going to solve the problem
4. Implementation: the actual solution you're providing
5. Conclusions: essentially the same as the introduction, but now assuming the reader has read the thesis

As a general rule, "Preliminaries" should contain whatever you didn't do yourself (it's fine to include forward pointers to how this or that is used in your solution though). This is important for "setting the stage" and, more importantly, to separate what is your contribution from what is the state of the art. Bear in mind that failing to clearly separate this (and a preliminaries chapter is the easiest way to achieve that) counts as plagiarism, even if involuntary; but no worries, I know that was not your intention.

* The content currently in Chapter 5 should go into "Preliminaries" (the parts about how the solvers work) and into "Implementation" (the parts about how the scheduling constraints correspond to solver constraints). I think it would be good to briefly mention metaheuristic optimization approaches (e.g. genetic algorithms, local search, GRASP (as in "greedy randomized adaptive search procedure"; note that the first CDCL SAT solver was, unfortunately, also named GRASP)).

* The content currently in Chapter 2.1 that is specific to the solution you provide should go into "Methodology".

* Chapter 2 should contain a discussion of the issues with time representation and modeling. Specifically, you currently imply that in order to model time it is *necessary* to discretize it. This is not quite true: if all your constraints are continuously defined, then it is sometimes *more* efficient to model time as a real-valued variable (counterintuitively, linear programming over the reals is *easier* than linear programming over the integers; we can discuss this if you're interested). There is no need to dive into that in your thesis, and discretizing time is a popular enough choice that you don't need to justify that. But you should not imply that this is the only way to do it.

* Chapter 2 should contain a discussion of constrained optimization (i.e. the problem of finding solutions within a feasibility space defined by constraints that optimize a given objective function); there you can also explain how hard and soft constraints are a specific case of constrained optimization where the objective function value depends on what constraints are violated.

In summary, Preliminaries in your case should contain:
* Time representations and models.
* Constrained optimization as a general approach, and soft vs hard constraints as a particular case.
* Scheduling problems as a case of constrained optimization.
* Solving methods for constrained optimization.
* Existing scheduling languages, and their shortcomings for the problem you're solving.
* Data interchange formats, e.g. JSON, XML

### Methodology

The methodology chapter should address three main questions (and probably it's a good idea to separate these into four sections):
1. What are the concepts that define your intermediate representation, i.e. horizon, tasks etc?
2. What kinds of computation you're deferring to the frontend? (e.g. converting dates with timezones to slots)
3. How are these concepts represented in your language (i.e. the JSON representation of those concepts)?
4. How can these concepts be combined to express new useful concepts (see section below).

Currently, the methodology chapter combines these four concerns. I think the reader will have a much easier time if you separate these, since then the layers of abstraction become clearer (e.g. one could design the same fundamental concepts for the IR and have a completely different language). So, for example, you could say "a task is defined by the following features: (an itemized list of duration, time slots etc follows)" in (1), and then in (3) you describe how this is represented in your JSON format.

The language spec is currently a code fragment. I suggest you change this into a normal paragraph. You can use `$\langle\mathrm{\textit{SyntaxItem}}\rangle$` to typeset syntax items, and the `\begin{itemize}...\end{itemize}` environment to list fields for objects.

### Expressivity of the defined constraints

There's one design discussion that ran throughout our discussions but does not appear in your thesis, namely that the constraints you have defined are *way more expressive* than they look. For example, your framework can express tasks with variable duration (say, I want to go to the gym for at least 45 min but preferably up to 1h) by splitting the task into several subtasks and imposing dependency and importance constraints (say, t1 is 45 min long, and t2 is 15min long but conditional on t1, with a cost of 0 at distance 0, an infinite cost if at any other distance, and a cost of 30 if unscheduled).

Another thing you might want to discuss here are "quirks" of your semantics or things you can't encode in this formalism (e.g. costs of "moving to a different location" are only paid if the first task is allocated immediately before the second). You don't need to provide a fix, but you should acknowledge the limitations of your approach.

I think your thesis is incomplete without some amount of discussion about these details.
I know you want to be done soon; it's alright if you don't want to go into full detail (your thesis is otherwise excellent, and the other comments I've written are actually quite minor, even if annoying to fix). But I think it is important to include the discussion.

### Minor issues

## Scheduling as a problem

At several points in your thesis you refer to scheduling problems in general and there's some implication that the problem you're solving neatly fits there. This is not quite the case (otherwise we would just pick any of the many many solvers that exist for this problem). Unless otherwise specified, the archetypical scheduling problem is the job-shop scheduling problem (https://en.wikipedia.org/wiki/Job-shop_scheduling). I think it would be good if you qualify what you're doing as "personal scheduling". It also won't hurt if you discuss JSP in the preliminaries and explain how it is similar or different to your problem.

## Italization of defined concepts

When writing technical documentation, we often use some terms with a very specific, formally defined meaning that have a broader, everyday meaning (e.g., "task"). To indicate that we're going to be using a word with a technical meaning (usually introduced later in the text), it is customary to use the `\emph` command to italicize the word (you might have seen this in papers or in textbooks). Generally you do this at the point of definition, but sometimes you also do it in the introduction or in the introductory paragraphs of a chapter to indicate to the reader that you plan to define this. It's not really a hard rule, more a matter of making the reader's life a bit easier :)

## Presenting the larger context

Your thesis is solving a small piece of a much larger project. This is not entirely obvious from the introduction. I think a good strategy here is to follow this general schema:
1. Define the large-scale problem the project tries to solve.
2. Explain the rough parts on which the project is divided, and how they connect to each other. A diagram here would help a lot. (This is currently missing; it will be very helpful to explain this here because "yeah no that's handled by the frontend" is a big part of your thesis later on.)
3. Explain exactly what part in that project you're going to be working on.
4. Explain what the issues to solve there are (this is basically your second itemized list).
5. Explain, briefly and without getting into many details, what your solution is (this is currently missing).
6. An outline of the thesis structure.

You mostly already have these parts (I'd say only 2 and 5 are missing), but they're not clearly separated, so a first-time reader who knows nothing about your thesis would finish reading the introduction and still not know how what you did relates to the larger project. I don't think it is necessary to separate these parts with sections. Rather, you can guide the reader with the text e.g.:

Current scheduling applications have this and that problem. This would be desirable because of blah.

This thesis provides a component for a project that solves this problem. The broad goal of the project
is to build a system that takes X as input and returns Y. To do this, the system does this and this and this with the input, and this results in a schedule. This approach presents challenges in (blah) and (blah).

In this thesis, we focus on this part of the aforementioned pipeline. This is a critical part because this and that reason. To design this, we need to overcome the following challenges: (blah).
This leads us to the following research questions: (blah).

This thesis proposes a solution to these challenges consisting of (briefly describe the language).
Associated formal semantics are defined for this language, based on (briefly describe the semantics).
This IR is expressive enough to capture (briefly describe the things you can do by combining the features of your language).

The remainder of this thesis is structured as follows: (blah)

## Examples

It is sometimes useful to use the `example` LaTeX environment to typeset examples. Just like sections, you can assign labels to refer to them later in the text. In particular you can "chain" examples by first writing "Example 1: here is a scheduling problem in natural language: (blah)", then "Example 2: Recall the problem in Example 1. The intermediate representation language can describe this problem as follows: (blah)".

## Don't claim what you don't know

At some points I've noticed claims that, although plausible, you haven't actually run experiments, or found justifying literature, to know if they're true or not. For example, in Section 2.1.2 you claim "Ideally, a solution with a cost of zero represents the best achievable outcome; however, such solutions are rarely attainable in practice".
This claim is false in at least some not so rare contexts e.g. you give a very long horizon, or you don't have a lot of tasks to allocate (wouldn't it be nice...). What is true is that, even when those zero-cost solutions exist, it might be hard for a solver to find them.

The good news is that avoiding these kinds of problems is very easy: just don't claim it. Most of the time it is not needed (e.g. in the example above you could just say "if a solution with cost zero exists, which is not guaranteed, then it is optimal"). As a general rule, these statements tend to be about "the real world" as opposed to "pure math", although it's not a perfect rule (e.g. "gravity makes things go down" is a practically correct statement; "P is not NP" refers to mathematics but we have no clue if it's true or not).

## On the reasons for time slot representation

You mention that the representation for time slots was chosen because it is more human-readable. This is a incorrect because, first, it is not (good luck figuring out what time of the week slot 3532 is), and second, because the whole point of the IR is that it does not need to be understood by humans.

The reasons are more subtle and have to do with the format being designed to be solver-agnostic. Choosing a bit array is inconvenient because, if we use e.g. ILP for encoding, the natural encoding is an array of intervals. On the other hand, if we use MaxSAT, then converting intervals to a big disjunction is quite easy. Since you're moving the solver discussion in the preliminaries,
now you can explain this at this point in the thesis.

## Overlapping interval costs

Perhaps it is worth clarifying already in Section 3 what happens if you have two overlapping intervals.
According to Section 4.1 the cost of a slot `s` in that case is defined by picking one of the intervals that contain `s` and taking the cost of that interval. This is a problem because this operation is non-deterministic (which interval is picked?).

There are several ways to fix this. One is to combine the costs of different overlapping intervals in some way; generally, you'd expect that:
1. the combination cost is greater than the cost of each independent interval,
2. the combination cost for zero overlapping intervals is 0, and
3. the combination cost for one overlapping intervals is exactly the cost of that interval.
Two combinations that fit this bill are:
A) the maximum of the interval costs, assuming max of the empty set is 0, and
B) the sum of the interval costs, assuming the sum of zero terms is 0, and assuming that infinity plus anything is infinity; both of these assumptions are commonplace in the literature.
I would say (B) is the best approximation for the intuition of what "cost of a given interval" tries to express. Technically you could also choose to say that the combined cost is the interval cost of the first occurring applicable interval. I think that's ugly but not terrible. Alternatively, if you just want to be done with the thesis, you can just say that overlapping intervals are forbidden - I'm not going to give a worse grade for that, since technically this is something the frontend could handle.

Remember to change the examples and validator accordingly!
Note also that the same problem appears with dependencies (but also read my comments below about explaining dependencies).

## Is location a mandatory field?

I think the text is contradictory on whether `location` is mandatory or not in the file format.
There are several ways to handle this. One is to say that it is mandatory but the frontend is supposed
to create a fresh, unique location if one is not provided. Another way is to make it mandatory but allowed to be `null`. Another way is to say it is optional (then please change the "exactly the following fields" part where applicable).

Note that in Section 4.1 the `loc` mapping is defined to be a total function, so items with `null` location would technically be clustered together. Using that definition, items with `null` location should then be excluded from the sum in equation (4.3). In my opinion, the easiest, minimum-change solution is the first one above (unique fresh location created on-demand by the frontend).

## Barbeque example

I think this is a great place to use the `example` LaTeX environment I mentioned above. For example, you could already at the start of Chapter 3 say that you will use this as a running example, and introduce the problem in natural language (possibly replacing specific dates and times by numbers, so "between slot 13 and slot 46" rather than "between 13:00 and 16:00 on December 32nd").
Then in Section 3.2 you can use it as an example and explain how each part translates to your format.

## Explaining dependencies

I think one detail where the text is not quite clear is the notion of "dependency", especially by the time we arrive at Section 4.1.
I suggest you introduce this notion in abstraction stages in Section 3.
First you can explain that there are two different situations you want to model:
1. A task B is optional but only makes sense to schedule it if another task A has also been scheduled.
2. A task B must be scheduled after task A has been executed.
Then you can point out to the reader that these two situations are covered by a more general abstract notion, which you call "dependency". This is a cost (potentially zero or infinite) that is only paid whenever task A is scheduled, and that depends on whether B is scheduled, and on the temporal distance between A and B if scheduled.
Then the explanation in Section 4.1 should be pretty straightforward.

## On comparing solving paradigms

Towards the end of Section 5 you present some comparison of different solvers,
and an encoding into ILP.

Please note that designing a good encoding of your problem into ILP is a whole MSc thesis on its own. If you insist on including the encoding, please clarify that this is a naive encoding (not an insult, this is how such a "straightforward" encoding is usually called) only intended as an example.

However, I think you should consider leaving the encoding and the solver comparison out. For one, the encoding you give is not quite an encoding: the disjointness constraint uses disjunction, and ILP only considers conjunctions of linear inequalities. Although disjunction can be encoded using extra variables, explaining that would take a while and wouldn't add much to your thesis. For another, figuring out what solving paradigm is best suited for a problem is a *huge* problem, and one that's very much open, so I don't think it is reasonable to dispatch it by how easy it is to encode the problem.

It is actually quite nice that you took the interest to try to see how you'd go about encoding the problem into ILP, but that's a can of worms - if you are curious, we can take a look into that once you're done with the thesis.

Regarding section 5.1.5, I think in this case it is fine to just claim that your problem can be encoded into MaxSAT, MaxSMT or ILP, and say that finding efficient encodings and determining which solving procedure would work best is out of the scope of your thesis.
It is fine to just say "that's someone else's job" :)

## Symbols and typography

Feel free to skip the following if you don't feel like fixing them.

* You currently use the symbol `T_s` to denote the tasks actually scheduled by a solution `a`.
I think the symbol should reflect that `T_s` depends on `a` (it is not that important that `T` is in the symbol, since `a` is only defined for some specific set of tasks). I would suggest `a_s` or `T_a` as alternatives.
* Similarly, it is customary to use `\operatorname` for "predefined" functions, so it makes more sense to not use `\operatorname` for `a`.
* If you write the command `\urlstyle{tt}` right before `\begin{document}`, your URLs will be typeset in a monospaced font.
* Many people use it with the same meaning you use ("see also"), but technically "cf. X" means "compare with X". So, it rather means something like "note the difference with X" (this is quite frequent in philosophy and law). But as I said, no one seems to care :)
* Is the `\clearpage` at the end of Section 4 really needed?
* Boolean is written in uppercase, since it refers to a person (namely, George Boole).

# 17.11.2025

I took a look into your thesis, here's some of my thoughts.

## Structure

While there's no predefined structure for a thesis, I recommend to start from the rough schema I lay out below, and then adapt it to each particular thesis as needed.

1. Introduction. Here you present your thesis assuming the reader *doesn't know* what you did. This should roughly contain:
    - What is the general problem your thesis works on. Not necessarily the problem you're solving, but the long-term problem your solution is a part of.
    - What specifically is the sub-problem you worked on. This includes: why is this even a problem? Why is it a *relevant* problem? What is not so straightforward about it?
    - What are your contributions i.e. what did you do to solve it. Here you summarize very briefly and without going into details what you did.
    - What are your results. Explain what things you achieved with your solution.
    - Outline the rest of your thesis.

2. Preliminaries. Here you introduce any information the reader might need to understand what comes next that was *not your own contribution*.
    So, here you can talk a bit about data formats like JSON, about solvers, etc.

3. Methodology. Here you explain in much more detail what you did to solve the problem, but only what you need before going into details.
    Here's where you'd talk about your choice of what is a task, what are the timeslots, what are dependencies etc.
    Crucially, here's where you should talk about how your very limited IR language suffices to achieve great expressivity in different situations.
    You also talk here about limitations.

4. Implementation. Here you go into the tiny details. In your case, you'd probably call this "Formal framework".
    This is the big part of your thesis, where you describe e.g. the mathematical modeling.

5. Experimental evaluation. This part you can skip because your work is not experimental.

6. Conclusion: Pretty much the same as the introduction, but this time around assuming the reader has already read the thesis and knows what you did.

I think right now your thesis is not quite conforming to this.

## Methodology

From what I see, "Data format" would be Section 3: Methodology. As you probably have noticed by now, starting with definitions does not quite work well.
Instead, I suggest you start by discussing what design decisions are involved. For example, you can start by saying that the intuitive definition of task is very wide
and includes things like repetitions, task splitting etc. Then you say that instead you decided to interpret tasks as items that must be allocated predetermined contiguous amounts of time,
and that other properties would be reduced to tasks.

Similarly for time: you decided to discretize time into "slots" of an arbitrary length, and to bound the span of time being considered through the horizon etc.

# 20.10.2025

- There's no need to write everything symbolically, it suffices to be unambiguous.
- `T_s` should be moved to the search space or to the semantics.
- Use a special timeslot `u` for "unscheduled" in the assignments, and extend `slot` to a function of signature `T x (N U {u}) --> N U {inf}`.
- Locations and travel are a bit tricky to express. Maybe choose some simplification and move on for purposes of finishing at some point.
- Dependency costs and feasibility should depend on task pair offset.
- You can encode "`t` immediately before `s`" as `a(t) + dur(t) = a(s)`.

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
    "importance": either a JSON integer or the string "Infinity"
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