# Notes of week 3 (25.08.-31.08.)

Summarizing the previous meeting we decided:
* urgency and importance are not important at this very moment
* Splitting tasks will be handled via
  * Creating multiple tasks
  * Connect via contstraint (preferebly location)

## Location
We now have get a notioon of how to handle locations. The following is a simple definition for location:
> Location is a place (can be fictional), which allows us to schedule tasks with the same location together and apply a certain cost to solutions, which are not scheduled together

We have to watch out for:
* How to define such locations
* How to calculate cost 
  * A task which is not scheduled with other task(s) that have the same location
  * A task should not creat cost, if it is not possible to schedule it with other tasks of that location!

### Suggestion on how to handle location:
There are two usecases for location, either for *split tasks* or for actual *locations*.

Meaing, if one wants to specify a given location: "I want to go to the shopping mall" and "I want to go to the movies (in the shopping mall)" then these two tasks should be scheduled together to optimize travel times. Also if we allow task splitting (not tasks like: "Read a book twice a week") go to the Gym 3 hours per week and one sitting has to have at least 1 hour. This can be scheduled together (and in best case should be) but does not have to be if there is a better solution (in terms of cost).

Therefor, we again rely on our trusty frontend telling us what location a given task has (this can be handled via a simple map/id). Meaning we have:

```json
"location": 1
```

This can be done because our cost calculator does not rely on knowing what location it is, but rather what tasks are at the same location. Hence, if we keep Map of location and ids in our frontend we are done.

## Timslots

We realised that using an array of binary values is very hard to read. Additionally, intervals are as expressive:
> A union/array of intervals can easily be converted to a binary array

Also, instead of using `preferedTimeslots` we merge it into one array with the option to append cost to a given interval. This allows us to give a notion of at what time the task should preferebly be scheduled.
```json
"timeslots": [
    { "interval": [5,12], "cost": 5},
    { "interval": [20,22], "cost": 0},
    { "interval": [20,28], "cost": 25},
]
```
Additionally, we removed `timeWindow`. As this has to be handled by the Frontend anyways - given that locations like restaurants have to be looked up/handled by the forntend too. This also get ridd of `shift` inside of `timeslots`.

## Resulting representation

```json
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
            "minAfter": 10, 
            "maxAfter": 20,
            "unmetCost": 0,
        }
    ],
    "location": 1
}
```

## Semantics
These changes clearly make the semantical explanation a lot easier.

* `duration` amount of uninterrupted time slots allocated to a task\
  $\textit{duration} \in \N_0 | \textit{duration} < \textit{horizon}$
* `importance` cost for not scheduling the task\
  $\textit{importance} \in \N_0 \cup \infty$
* `urgency` cost factor multiplied by the default late-cost\
  $\textit{urgency} \in \N_0$
* `timeSlots` all time slots at which the task can be scheduled\
  $\textit{timeSlots} = [[a,b] [b,c], ...] \rightarrow \textit{possibleSlots} = \bigcup\limits_{i \in \textit{timeSlots}} i$
* `depencendies` specifying what task should/have to be scheduled beforehand\
  *task* id of the task that has to be scheduled beforehand\
  $\textit{taskB.ActualInterval} = [a,b]\rightarrow$\
  $\textit{dependencyInterval} = \begin{cases}[b+\textit{minAfter}, b+\textit{maxAfter}] & \text{if } \textit{maxAfter} > \textit{minAfter} \\ [b+\textit{minAfter},\infty) & \text{else}\end{cases} \rightarrow$\
  $\textit{dependencyPossibleSlots} = \textit{dependencyInterval} \cap \textit{possibleSlots}$\
  \
  $\textit{minAfter} \in \N_0$\
  $\textit{maxAfter} \in \N_0$\
  $\textit{maxAfter} > \textit{minAfter} \implies \textit{maxAfter} - \textit{minAfter} \geq duration$\
  \
  *unmetCost* the cost applied when the *minAfter*/*maxAfter* requirement is not met
* `location` id of the location the task should be scheduled at

With late-cost mentioned above, we mean the cost increas which "naturally" occurs when tasks are scheduled later. Scheduling tasks later in general is more costly than scheduling them now. This is a direct consequence of the requirement that tasks should - in best case - be scheduled towards the beginning of the time horizon. 
> Naturally, tasks should be scheduled towards the beginning of the time horizon as most tasks will be knonwn rather later than sooner, which makes them easier to fit into the existing schedule.

`horizon` is the last timeslot