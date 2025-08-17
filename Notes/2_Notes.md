# Notes of week 2 (11.08.-17.08.)

## Problems from previous week
From the previous week (1_Notes.md) we have spotted several problems with the previous approach:

- Priorities can take several forms: urgency != importance
- Let the front-end deal with dates and holidays
- You need to find a good way to make this work with reoccurrences.
- Currently, you can only express order dependencies (i.e. X happens before Y), but sometimes you also want time dependencies (X happens at most 1 week after and at least 2 days after Y).
- Instead of location, you could consider clustering constraints where some tasks happen preferably in clusters with each other.
- Duration can have many variations (variable duration, task splitting, preferred duration). You should think about which of these you can already express within your language, and which ones would need new features.

This means: we need to reiterate the IR language:

### Urgency != Importance
As urgency restricts us in terms of how far in the future the task can be scheduled, importance gives us a metric of how important it is to schedule this task correctly (or at all).

### Dates and Holidays
For our previous approach we tried to define Dates and Holidays in simple terms which is plainly not possible. Therefore, we outsource this functionality to the Frontend (the layer before the intermediate representation). For this we need the frontend to define a set of time slots, which represent the given dates and holidays.

In this case we suggest an interval instead of the previously used binary array as it would be most likely a very sparse binary array (a lot of 0 entries). If we now take an example like: In two days (or on 21.08.2025) we let the frontend calculate the time slots until this day starts. We get the following code example in c#. This gives as the interval of valid time slots.
```c#
var currDate = DateTime.Now;
var startOfTargetDay = new DateTime(2025, 8, 18).Date;

int timeSlotLength = 60; // In minutes

TimeSpan timeDiff = startOfTargetDay - currDate;
int timeslotsUntilDate = (int)Math.Ceil(timeDiff.TotalMinutes / timeSlotLength); // Ceil to ensure the time slot is already on the selected date

DateTime endOfTargetDay = startOfTargetDay.AddDays(1).AddSeconds(-1);
TimeSpan dayInterval = endOfTargetDay - startOfTargetDay;
int timeslosOfTargetDate = (int)(dayInterval.TotalMinutes / timeSlotLength);

Console.WriteLine($"[{timeslotsUntilDate}:{timeslotsUntilDate + timeslosOfTargetDate}]");
```

For a task like I have to pay the electricity bill until September 1st (01.09.2025)
```c#
var now = DateTime.Now;
var target = new DateTime(2025, 9, 1).Date;

TimeSpan timeUntilTarget = target-now;
int timeslotsUntilTarget = (int)Math.Ceiling(timeUntilTarget.TotalMinutes / timeSlotLength);

Console.WriteLine($"[:{timeslotsUntilTarget}]");
```

Additionally, one needs to keep in mind that there are also excluding dates, which would be very inefficiently defined with the previous approach of including time slots. For this we propose an `includingDate` interval and an `excludingDate` interval. For clarity `excludingDate` takes precedence. This is needed for excluding holidays, weekends and simple dates. 

### Reoccurrences
As reoccurrences are tricky to be defined in general and especially tricky if they have to be simple. Therefore, we surpass this issue by scheduling multiple tasks in the given time horizon. If one wants to go shopping every week we schedule x different tasks - where x is the weeks contained in the given time horizon - with each task containing the `includingDate` of the respective week (excluding local restrictions when shops are closed on holidays or weekends). 

This leaves us with the problem that formulations like "at least twice a week" and "maximum once per month" have to be smartly created as the twice a week could mean 2 tasks with high importance and then infinitely many with low importance/zero importance. "Maximum once per month" on the other hand seems to be clearly 1 Task per month with low-medium importance. 

### Improved dependencies
Task dependencies should contain time horizon in which the follow-up-task has to be scheduled. This can easily be represented by two simple additional properties `minAfter` and `maxAfter`, these constrain the following task by a count of minimum and maximum time slots it should be after the task finished. 

### Handling location
The tasks in common location should be - in the best case - scheduled together to be the most time efficient. As this is just an IR language we do not have the capacity to handle locations. Therefore, we again outsource this issue to be handled by the frontend. It can check locations, additionally check opening-hours if applicable and add them to the include/exclude time slots. When checking such a location it can also look up all the other locations of the other existing tasks and in the best case schedule/link them together via `dependencies`.

This can become an issue though, as the dependencies have a specific order, as there cannot be cyclic dependencies or anything like that. In general such clustering of tasks should not maintain on ordering of some sort and this might become an issue later on.

### Current Representation
```json
"TaskA": {
    "duration": 2,
    /* "priority": 100, ~crossed out~ */
    "importance": 1000,
    "urgency": 50,
    "timeWindow": [0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0],
    "preferedTimeWindow": [0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0],
    /*"minAmount": {
        "perDay": 0,
        "perWeek": 0,
        "perMonth": 0,
        "perYear": 0
    },
    "maxAmount": {
        "perDay": 0,
        "perWeek": 2,
        "perMonth": 0,
        "perYear": 0
    }, ~crossed out~ */
    /* "date": {
        "daysOfWeek": [1,0,0,1,1,1,1],
        "daysOfMonth": null,
        "weeksOfMonth": null,
        "monthsOfYear": null,
        "everyYears": 1
    }, ~crossed out~ */
    /* "location": null, ~crossed out~ */
    "dates": {
        "included": [12:24],
        "excluded": [7:8]
    },
    "dependencies": [
        { "task": "TaskB", "minAfter": 10, "maxAfter": 20 }
    ]
}
```

This leaves the problem that we either have `dates` (both included and excluded) as either time slots or days. Now to the issue: if we now have the `timeWindow` property, how can we use/combine them? 

When we take the days we can multiply them (tensor product) - if day is 1 insert the `timeWindow` as is, else insert zeros with the same length as `timeWindow`.

For time slots we have the problem that we do not know where to put the `timeWindow` mask.
For the day slots we have the problem that for today (a day that has already started) we have to shorten the time slots as some of them are in the past. Additionally, when we have the day slots, one cannot express Tasks like "I have to do x until 1st of September 2025 1pm" because we do not have the time factor present. 

This leaves us with three options:
- Outsource the masking of `timeWindow` into the frontend
- Use `availableDates` and use the tensor product when trying to figure out the final available slots
  - Cut the first time slots that are in the past of the current day.
  - Con: cannot represent the previously mentioned example
- Use `availableSlots` and move the mask such that the time slots match with the `timeWindow` mask
  - Uses binary operations as each of the arrays (`timeWindow`, `availableSlots`, `blockedSlots`) is of type binary.

As we can still simply outsource this task to the frontend later anyway we now proceed with the third option as it is the most expressive without loosing any expressiveness of the language.


### New Representation
```json
"TaskA": {
    "duration": 2,
    "importance": 1000,
    "urgency": 50,
    "timeWindow": [(10,21)],
    "preferedTimeWindow": [(15,20)],
    "timeslots": {
        "included": [79:102],
        "excluded": null,
        "shift": 19
    },
    "dependencies": [
        { "task": "TaskB", "minAfter": 10, "maxAfter": 20 }
    ]
}
```

#### Semantics
- `duration`: the number (consecutive) of time slots occupied by this task (integer)
- `importance`: a factor of cost which is added when scheduling of this task is not satisfactory (integer)
- `urgency`: a factor of cost which is added when scheduling takes longer (increases per time slot) (integer)
- `timeWindow`: defines the time of day the task can be scheduled at (if not met → importance) ([interval])
- `preferedtimeWindow`: defines the time of day the task should be schedule (if not met → low cost) ([interval]) - has to be a subset of `timeWindow`
- `timeSlots`: defines the time slots in which the task can be scheduled (dates, exclusion of holidays/weekends, ...) (if not met → importance) ([interval])
- `dependencies`: defines what tasks have to be finished before this task can be scheduled and in what time frame it has to be scheduled.

For the mathematical semantics we first define:\
$T$ to be the set of time slots in the horizon $t \in T | 0 \leq t \leq n$ where n ($n$ is the last time slot) is the last slot in the horizon\
$[a:b]$ will be used as an integer interval meaning $[a:b] = \{a, a+1, a+2, \dots, b-1, b\}$\
if one leaves out $a$ (the first limit) it means $a = 0$; if one leaves out $b$ (the second limit) it means $b = \inf$\
we now use $\tau$ as a short for a task

We now define possible time slots:\
$W_{\text{shifted}} = [(a - \textit{timeslots.shift} + 24k,b - \textit{timeslots.shift} + 24k) | k \in \{0, 1, \dots, ceil(n / 24)\}]$\
$P \sube (\textit{timeslots.included} - \textit{timeslots.excluded}) \cap W_{\text{shifte}}$

We now have the set/array of intervals $P$ to be the points where this task is allowed to be scheduled at. Now we go forth with the duration. Per definition the duration is a (at this point for simplicity reasons continuous) block of time. Which has to fit into one of the available intervals.\
$V = \{s, s+1, s+2, \dots, s+duration\} = (s,s+duration) | s \in T \land (s,s+duration) \in P$\
V is now the Set of valid task positions.

Now we need to fulfill the following two constraints:
1. For a set of tasks $T$\
   $\forall \tau \in T: \forall d \in \mathit{dependencies}(\tau): V(\tau) = V(\tau) \cap (s(\textit{d.task}) + \mathit{duration}(\textit{d.task}) + \textit{d.minAfter}, \mathit{maxAfter}(s(\textit{d.task}) + \mathit{duration}(\textit{d.task}), \textit{d.maxAfter}))$\
   Where $\mathit{maxAfter}(end, after) = \begin{cases} end + after & \text{if } after \neq null \\ \inf & \text{else}\end{cases}$ and where
   $s(\tau)$ is the scheduled (actual) starting point.\
   Meaning: Each scheduled task has to be a successor of all tasks defined in `dependencies` with the time constraints `minAfter` and `maxAfter` accounted for.
   
2. For a set of tasks $T$\
   $\forall \tau, t \in T | \tau \neq t: (s(\tau), s(\tau) + \mathit{duration}(\tau)) \cup (s(t), s(t) + \mathit{duration}(t)) = \empty$\
   Meaning: Each scheduled task has to be free of conflicts/crossovers with other tasks.


### Duration
Defining durations of tasks seems easy at first glance, but when one takes a deeper dive into the topic it must be defined what is meant by duration:
- One consecutive chunk of time
- One or more junks of time in a reasonable interval with each having a minimum length
- One consecutive chunk of time which has preferred length n, but it must not be this exact length
- One or more junks of time but the more junks of time the longer the task overall will take
- ...

With our current approach we can tackle two of those examples above:
- One consecutive chunk of time (duration: int)
- More junks of time with time split between those junks ([duration: int])

The frontend can generate multiple tasks (as with reoccurrences) and split the total amount of time needed between them. When doing so, it additionally can add dependencies between those tasks to not make them shift apart too far. 

#### Questions
- Is a task which is scheduled but not with the full duration (e.g. 10 slots instead of 11) deemed completed?
- What is reasonable addition of overhead for each junk the task is split in. 
- Is a task that is split up actually only one task, or is it possible to separate those task beforehand.