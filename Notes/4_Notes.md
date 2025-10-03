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
* Changed Object inside the dependencies array to accommodate multiple different costs for different intervals.
  * Instead of `minAfter` and `maxAfter`, we now use `interval`, as it is basically the same as the `interval` of `timeSlots` and therefore this way is more consistent. 
* Added cost for the location (still only one location allowed per task, but the cost may very when not scheduling the same locations together)

## Data Description
* `horizon` the horizon at which tasks can be scheduled (total amount of time slots)\
$\textit{horizon} \in \N_0$\
From now on the object we look at is the `tasks` dictionary:
* `duration` amount of uninterrupted time slots allocated to a task\
  $\textit{duration}_t \in \N_0$
* `importance` cost for not scheduling the task\
  $\textit{importance}_t \in \N_0 \cup \infty$
* `urgency` cost factor multiplied by the default late-cost\
  $\textit{urgency}_t \in \N_0$
* `timeSlots` all time slots at which the task can be scheduled
  * `interval` ($i \in \{1,2,\dots,count(intervals)\}$) an interval at which the task can be scheduled\
    $[a,b] | a,b \in \N_0$
  * `cost` cost applied when the task is scheduled at this particular interval\
    $\textit{cost} \in \N_0$
* `depencendies` specifying what task should/have to be scheduled beforehand
  * `task` id of the task that has to be scheduled beforehand\
    $\textit{task} \in \Sigma^n$ where $\Sigma$ is the alphabet and $n \in \N_0$ is the length of the word/string.
  * `intervals`
    * `interval` interval that defines at which time slots the task should be scheduled relative to the dependency\
    $[a,b]_{t,i} | a,b \in \N_0$
    * `cost` cost for this particular interval\
      $\textit{cost} \in \N_0$
  * `unmetCost` the cost applied if none of the intervals defined in `intervals` is met\
  $\textit{unmetCost}_{t,i} \in \N_0$ 
* `location` ($l \in \{1,2,\dots,count(locations)\}$)
  * `id` id of the location the task should be scheduled at\
    $\textit{id}_{t,l} \in \N_0$
  * `unmetCost` cost applied when not scheduling this task with other tasks of this location (if present)\
    $\textit{unmetCost}_{t,l} \in \N_0$

## Mathematical Representation

### Data Definition
We now need to define the data which describes our problem to have something to solve. 

  * `tasks`: $T = \{1,\dots, m\}$
  * `horizon`: $H \in \N_0$
  * `time slot function` $t: T \times \N_0 \longrightarrow \N_0 \cup \infty$
    * maps every task at a given time slot to a cost
  * `dependencie` $\textit{dep}_{a,b}: \N_0 \longrightarrow \N_0 \cup \infty$
  * `location` $\textit{loc}: L \times L \rightarrow \N_0$

Very unsure about this - was a short in the dark to see if I'm anywhere near what would work.

Thoughts:
* Tasks is needed anyways - without it there is nothing to solve 
* Same goes for horizon as some form of time-constraint (horizon is needed)
* `time slot function` combines: 
  * importance
  * urgency
  * duration
  * and time slots to one function that maps to cost.
* `dependencies` are functions between Tasks a and b. It calculates a cost based on how many time slots b is scheduled after a.
* `locations` maps two locations to a number of times lots needed between two tasks as travel time.
* Using several functions (mappings) to cost will enable ILP to find the best solution - at least that's my theory.

### Search Space
We have the description of the data and therefore a set of acceptable languages that meet the requirements of the Intermediate Representation. 

We now want to specify the search space, meaning the space of possible solutions, which is a subset of the set of acceptable languages. 

```cs
public class Solution {
  public class Task {
    public int startIndex;
    public int duration;
  }
}
```