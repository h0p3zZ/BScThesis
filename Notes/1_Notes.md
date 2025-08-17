# Notes of week 1 (29.07.-05.08.)

The definition only handles Tasks from today into the future, no backward tasking (does not make any sense)

So a basic constraint (not really a constraint maybe) is that any task must start no sooner than today.

For the task representation we chose JSON as it is the markup language we are most common and comfortable with. The Intermediate Representation language is just a subset of JSON -  a dialect. 

## Simple Constraints
We came up with the following constraints which tend to give a lot of freedom but still might not cover all possible situations. Still we restrict this thesis on the following constraints as we find it expressive enough.
* Duration (int, the amount of (uninterrupted?) time required to be allocated to the task)
* Urgency (int?, the higher the number the more urgent sorts tasks by x>y>z ...)
* After (x > int?, the task must be concluded after the given timestamp)
* Before (x > int?, the task must be concluded before the given timestamp)
* Dependency (x => y, the task must be scheduled after the given task y)
* Reoccuring (x, has to be scheduled every month/week/tuesday/...)
* ...
* (x) Time frame (int? <= x <= int?, the task must be concluded between the given timestamps) - already covered with before and after

```json
"TaskA": {
        "duration": 1,
        "earliestStart": 0,
        "latestStart": 0,
        "latestEnd": 0,
        "priority": 100,
        "reoccuring": {
            "daysOfWeek": [1,1,1,1,1,1,1],
            "daysOfMonth": [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
            "weeksOfMonth": [1, 1, 1, 1, 1],
            "monthsOfYear": [1,1,1,1,1,1,1,1,1,1,1,1,1],
            "everyYears": 5
        },
        "dependencies": []
    },
```
In every scenario we take the priority as penalts, the higher the priority the higher the penalty. For the same tasks to be scheduled the schedule with the lowest cost is the solution which is the most appropriate - or at least meets all the criteria of the scheduling task.

This was the first attempt on a minimal language without too many values - this is still very complex to parse/very complex to get every e.g. repetition and, in general, some are not even possible (for example: every 1st tueasday in March, every year). This cannot be expressed by the number arrays in the example JSON.
> Note: this can now be done when using the new property "weeksOfMonth".

Actually we also thought about the conditions that need to be met in regards of visiting a store. For this one has to store additional information on the store/the location itself such as:
* Opening hours 
* Opening days
* Seasonal restrictions (only open from October-February)
* Open on national holidays?
* Location (to be able to handle proximities for scheduling nearby tasks together)

For this we introduce a new idea. Substracting a day into timeslots of 5/10/15 minutes. For simplicity we take 60 minutes at the beginning as it is easier to handle 24 slots/indices than handling 5 minute slots which would be 24*60/5 = 24*12 = 264 slots.
```JSON
{
    "storeA": {
        "openingHours": [0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0],
        "openingDays": [1,1,1,1,1,1,0],
        "openingM$onths": [1,1,1,1,1,1,1,1,1,1,1,1],
        "openHolidays": false,
        "location": {
            "lat": 48.337040,
            "long": 14.319919
        }
    }
}
```
The example above would therefore mean a store by the name of "storeA" open from 07:00-19:00 every day but Sunday (Mo-Sa), which is not open on days of national holidays and located in the JKU Uniteich.

Using this approach one could also redefine our previous example and just use the split into timeslots:
```JSON
{
    "TaskA": {
        "duration": 1,
        "earliestStart": [0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
        "latestStart": null,
        "latestEnd": [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0],
        "priority": 100,
        "reoccuring": {
            "daysOfWeek": [0,1,0,0,0,0,0],
            "daysOfMonth": null,
            "weeksOfMonth": null,
            "monthsOfYear": null,
            "everyYears": 0
        },
        "dependencies": []
    }
}
```
Additionally to the timeslots approach we have replaced the [1,1,...,1] arrays by null, meaning "don't care".

This would now be a Task that takes 1 hour and has to be done $\textit{earliestStart} \cap \textit{latestEnd} = \text{possibleSlots}$. With the priority 100 (cost of failing to schedule properly is 100). Additionally, this task has to be scheduled every second day of the week (tuesday). TaskA has no dependencies on tasks that have to be finished beforehand.

```json
{
    "TaskA": {
        "...": "...",
        "availibility": {
            "daysOfWeek": ["size7"],
            "daysOfMonth": ["size31"],
            "weeksOfMonth": ["size5"],
            "monthsOfYear": ["size12"],
            "everyYear": "every x years (x is an unsigned integer)"
        }
    }
}
```
We want to use this as the frame of possible assignments.
```json
{
    "nationalHolidays": {
        "easter": {
            "daysOfWeek": ["size7"],
            "daysOfMonth": ["size31"],
            "weeksOfMonth": ["size5"],
            "monthsOfYear": ["size12"],
            "everyYear": 1,
            "generator": "has to be calculated using code (not possible statically)"
        },
        "newYearsDay": {
            "daysOfWeek": null,
            "daysOfMonth": [1,0,0,0,...,0],
            "weeksOfMonth": null,
            "monthsOfYear": [1,0,0,0,...,0],
            "everyYear": 1
        }
    }
}
```
As easter is defined to be on the First Sunday after the first full moon on or after the vernal equinox (March 21), it can not be expressed. New years day on the other hand (01.01.) can easily be defined by this approach. For Austria national holidays are either based on date or relative to easter. This has the consequence that all the national holidays (of Austria) can be defined by 
* $e(x) | x \in \N, e \in \{0,1,2,...,366\} \text{where e is the index of the day of easter sunday in the year x}$ (= e)
* Some relatives indices to the output of $e(x)$ including 0 t o ensure easter sunday itself is included (= R)
* And finally some static dates defined by the above boolean arrays with the summed size of 55 bits and a 8 bit uint (= S)
When we have those we can easily define the holidays of the year x by:
$[1 \text{ for every day of the year } \in e(x)+R] \cup [\text{concat}(\textit{daysOfMonth})\circ \textit{monthsOfYear}]$ where $\circ$ is the tensor operation ($x \circ y$ - put x into every position $y_i$ multiplied by $y_i$) and concat is just the function that reduces the *daysOfMonth* to the appropriate length (Jan=31,Feb=28/29,April=30). So the actual signature for the concat function would be $\text{concat}(x,m,y) | x \in \{0,1\}^31, m \in \{0,1,...,11\}, y \in \N$. Meaning x is the array of length 31 (*daysOfMonth*) m is the index of the month and y is the year (for leap year calculations).

With this appraoch we can represent most scheduled Task imaginable.
The problem we now face, is that natural language expressiosn like: "needs to be done twice a year", is more vague and can be object to a lot of interpretation. A task defining "I need to clean the windows twice a year, it takes about two hours" can either be scheduled as: 01.02.2026, 17:00-19:00, "Clean Windows" and 01.07.2026 17:00-19:00, "Clean windows". But also as: 01.01.2026, 17:00-19:00, "Clean windows" and 01.01.2026, 19:00-21:00 "Clean windows", which are very different solutions and only looking at the text input, there is no real cost difference for either of those schedules. 

## Detailed Constraints
The above section handles most of the basic scheduling needs, but for this thesis we need a broader definition of a task, and more variants of defining in what ways a task can be properly scheduled acoording to user needs.

* Duration (int, the amount of (uninterrupted?) time required to be allocated to the task)
* Urgency (int?, the higher the number the more urgent sorts tasks by x>y>z ...)
* After (x > int?, the task must be concluded after the given timestamp)
* Before (x > int?, the task must be concluded before the given timestamp)
* Dependency (x => y, the task must be scheduled after the given task y)
* Reoccuring (obj, has to be scheduled every month/week/tuesday/...)

These previously defined constraints fail at tasks like:

- I want to read a book at most two times a week. I prefer to do that right before going to sleep. Don’t schedule it on Saturdays, usually I go partying.
- My flat is an mess because I keep putting off cleaning. Remind me to clean when I am at home and I have nothing else scheduled.

So how does one schedule or even quantify such a task into any proper format?

Additional constraints:
* Duration (int, the amount of (uninterrupted?) timeslots required to be allocated to the task)
* Priority (int, the higher the number ($0<x\leq100$) the higher the priority)
* After ($x \geq int$, the timeslot(s) must be allocated after the given timeindex)
* Before ($x \leq int$, the timeslot(s) must be allocated before the given timeindex)
* Location (the timeslot must be allocated at this location or when another event at this location is planned)
* PreferedAfter ($x \geq int$, the timeslot(s) can be allocated after the given timeindex but don't have to be (cost))
* PreferedBefore ($x \leq int$, the timelot(s) can be allocated after the given timeindex but don't have to be (cost))
* MinAmount (obj, the amount of minimum events per week one has to schedule)
* MaxAmount (obj, the amount of maximum events per week one has to schedule)
* Dependency ([], the array of tasks that have to be finished before this one can start)
* Date (obj, date of scheduling (possible dates)) 
  * Recurring can be embedded with the date

For simplicity we stick with the division of a day into 24 timeslots of equal length (e.g. 1 hour).
```json
{
    "TaskA": {
        "duration": 2,
        "priority": 100,
        "afterTime": 7,
        "beforeTime": 20,
        "preferedTimeWindow": [0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0],
        "minAmount": {
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
        },
        "date": {
            "daysOfWeek": [1,0,0,1,1,1,1],
            "daysOfMonth": null,
            "weeksOfMonth": null,
            "monthsOfYear": null,
            "everyYears": 1
        },
        "location": null,
        "dependency": null
    }
}
```
*afterTime* and *beforeTime* can be combined into a single *timeWindow* which is a binary array, for this case:
```json
"timeWindow": [0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0]
```

So what the example of TaskA above actually says is: I want to do something schedule something twice a week that takes approximately two hours, it should be scheduled a maximum of twice a week between 07:00 and 20:00, but preferebly after 14:00. But don't schedule it on Tuesday or Wednesday.


Value description:
```json
{
    "TaskDescription": {
        "duration": 2,      /* An unsigned integer between 0 and the amount of simeslots-1 */
        "priority": 100,    /* An unsigned integer between 1 and 100 - quantifies the cost of not meeting */
        "afterTime": 7,     /* An unsigned integer - the index of earliest begining of the task */
        "beforeTime": 20,   /* An unsigned integer - the index of latest end of the task */
        "preferedTimeWindow": [0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0], /* A boolean array - timeframe of perefered assignment */
        "minAmount": {
            "perDay": 0,    /* An unsigned integer - minimum amount of this task scheduled per day */
            "perWeek": 0,   /* An unsigned integer - minimum amount of this task scheduled per week */
            "perMonth": 0,  /* An unsigned integer - minimum amount of this task scheduled per month */
            "perYear": 0    /* An unsigned integer - minimum amount of this task scheduled per year */
        },
        "maxAmount": {
            "perDay": 0,    /* An unsigned integer - maximum amount of this task scheduled per day */
            "perWeek": 0,   /* An unsigned integer - maximum amount of this task scheduled per week */
            "perMonth": 0,  /* An unsigned integer - maximum amount of this task scheduled per month */
            "perYear": 0    /* An unsigned integer - maximum amount of this task scheduled per year */
        },
        "date": {
            "daysOfWeek": [],   /* A boolean array of length 7 - depicts days of week */
            "daysOfMonth": [],  /* A boolean array of length 31 - depicts days of month */
            "weeksOfMonth": [], /* A boolean array of length 5 - depicts weeks of month */
            "monthsOfYear": [], /* A boolean array of length 12 - depicts months of year */
            "everyYears": 1     /* An unsigned integer - depicts repititions every x years */
        },
        "location": null,   /* (double, double) - depicts the location of the task */
        "dependency": null  /* An array of tasks/task-ids - depicts which tasks have to be finished first before this one can be scheduled */
    }
}
```

### Meeting notes?
Urgency is a task that has to happen soon?
Important is a task that HAS to be scheduled and might kick out not-important tasks
Think about what to do with reoccuring - maybe instead of modelling them, just schedule a lot of seperate tasks (from the frontend)
Think about how to handle interrupted tasks (schedule multiple tasks?, just don't allow interrupted tasks?)