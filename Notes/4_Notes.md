# Notes of week 4 (15.09.-21.09.)

Again we summarize the problems of the previous week:
* `dependencies` needs to be more expressive
* `locations` are missing cost
* `horizon` needs to be included in the language as a global property
*  Specify semantics in the following levels
    1. What data defines a scheduling problem?
    2. What data is a potential solution for a fixed scheduling problem? (i.e. the search space)
    3. When is a potential solution an actual solution of a scheduling problem? (i.e. feasible solutions)
    4. When is an solution preferable to another solution (i.e. optimal solutions)

## Updated language

```json
{
    "horizon": 100,
    "tasks": {
        "SomeTaskId": {
            "name": "Some Cleartext name",
            "duration": 5,
            "importance": 100,
            "urgency": 5,
            "timeSlots": [
                { "interval": [5,12], "cost": 5},
                { "interval": [20,22], "cost": 0},
                { "interval": [20,28], "cost": 25},
            ],
            "dependencies": [
                { 
                    "task": "AnotherTaskId",
                    "intervals": [
                        { "interval": [10, 20], "cost": 5},
                        { "interval": [20, 1110], "cost": 15}
                    ],
                    "unmetCost": 100
                }
            ],
            "location": { "id": 1, "unmetCost": 150 }
        }
    }
}
```

The changes in short are:
* Changed Object inside of the dependencies array to accomodate multiple different costs for different intervals.
  * Instead of `minAfter` and `maxAfter`, we now use `interval`, as it is basically the same as the `interval` of `timeSlots` and therefore this way is more consistent. 
* Added cost for the location (still only one location allowed per task, but the cost may very when not scheduling the same locations together)

## Data Description
* `horizon` the horizon at which tasks can be scheduled (total amount of time slots)
From now on the object we look at is the `tasks` dictionary:
* `duration` amount of uninterrupted time slots allocated to a task
* `importance` cost for not scheduling the task
* `urgency` cost factor multiplied by the default late-cost
* `timeSlots` all time slots at which the task can be scheduled
  * `interval` on interval at which the task can be scheduled
  * `cost` cost applied when the task is scheduled at this particular interval
* `depencendies` specifying what task should/have to be scheduled beforehand
  * `task` id of the task that has to be scheduled beforehand
  * `intervals`
    * `interval` interval that defines at which time slots the task should be scheduled relative to the dependency
    * `cost` cost for this particular interval
  * `unmetCost` the cost applied if none of the intervals defined in `intervals` is met
* `location`
  * `id` id of the location the task should be scheduled at
  * `unmetCost` cost applied when not scheduling this task with other tasks of this location (if present)

## Mathematical Representation

### Data Description
* `horizon` $\textit{horizon} \in \N_0$\
From now on the object we look at is the `tasks` dictionary:
* `duration` $\textit{duration} \in \N_0$
* `importance` $\textit{importance} \in \N_0 \cup \infty$
* `urgency` $\textit{urgency} \in \N_0$
* `timeSlots`
  * `interval` $[a,b] | a,b \in \N_0$
  * `cost` $\textit{cost} \in \N_0$
* `depencendies` specifying what task should/have to be scheduled beforehand
  * `task` $\textit{task} \in \Sigma^n$ where $\Sigma$ is the alphabet and $n \in \N_0$ is the length of the word/string.
  * `intervals`
    * `interval` $[a,b] | a,b \in \N_0$
    * `cost` $\textit{cost} \in \N_0$
  * `unmetCost` $\textit{unmetCost} \in \N_0$ 
* `location` 
    * `id` $\textit{id} \in \N_0$
    * `unmetCost` $\textit{unmetCost} \in \N_0$

This now describes in computer science terms the basic data structure of the language and what types the containing variables have. In other words it defines the basic parsable/acceptable language.

### Search Space
We have the description of the data and therefore a set of acceptable languages that meet the requirements of the Intermediate Representation. 

We now want to specify the search space, meaning the space of possible solutions, which is a subset of the set of acceptable languages. 

* `horizon` $\textit{horizon} \in \N_0$\
From now on the object we look at is the `tasks` dictionary:
* `duration` $\textit{duration} \in \N_0$
* `importance` $\textit{importance} \in \N_0 \cup \infty$
* `urgency` $\textit{urgency} \in \N_0$
* `timeSlots` $\textit{timeSlots} = [[a,b] [b,c], ...] \rightarrow \textit{possibleSlots} = \bigcup\limits_{i \in \textit{timeSlots}} i$
* `depencendies` 
  * `task` $\textit{task.intervals} = [[a,b] [b,c], ...] \rightarrow \textit{specifiedIntervals} = \bigcup\limits_{i \in \textit{timeSlots}} i$\
  $\textit{unmetInterval} = [\textit{taskB.actualIntervalEnd}, \textit{horizon}]$\
  $\textit{possibleIntervals} = \textit{specifiedIntervals} \cup \textit{unmetInterval}$
* `location` 
    * `id` $\textit{id} \in \N_0$
    * `unmetCost` $\textit{unmetCost} \in \N_0$