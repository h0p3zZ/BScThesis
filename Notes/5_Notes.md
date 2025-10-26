# Notes of week 5 (6.10.-12.10.)

we stick with the same JSON format as it for now seems to be (almost) complete - good enough for now.

## JSON file
(no changes just the same as last week)
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

### JSON data descriptino
As having a JSON and defining mathematical notations on it is quite ineffcient, we now use the JSON format to our advantage and redo the data descriptino from last week. Instead of defining everything by a simple mathematical notation, we now define everything by the JSON standard.

```
<file> a JSON object containing exactly the following fields:
    "horizon" a JSON integer
    "tasks" a JSON object of type dictionary with a JSON string as a key and a JSON object of type <task> as a value

<task> a JSON object containing exactly the following fields:
    "name": a JSON string
    "duration": a JSON integer
    "importance": a JSON integer
    "urgency": a JSON integer
    "timeSlots" a JSON array of type <interval>
    "dependencies" a JSON array of type <dependency>
    "location" a JSON object of type <location>

<interval> a JSON object with exactly the following fields:
    "interval": a JSON array of type JSON integer of exactly two numbers
    "cost": a JSON integer

<dependency> a JSON object with exactly the following fields:
    "task": a JSON string
    "intervals" a JSON array of type <interval>
    "unmetCost" a JSON integer

<location> a JSON object with exactly the following fields:
    "id": a JSON integer
    "unmetCost" a JSON integer
```
For clarity everything in this JSON has to conform to the JSON standard (a valid JSON file). Also when "exactly the following fields" is written - it means exactly the listed fields, with exactly the specified types.

## Mathematical Representation
### Data Definiition
* `tasks`: $T$ is a finite set
* `horizon`: $H \in \N_0$
* `time slot function` $slot: T \times \Z \longrightarrow \N_0 \cup \infty$
  * maps every task at a given time slot to a cost
* `dependencies` $\textit{dep}: T \times T \longrightarrow \Z \longrightarrow \N_0 \cup \infty$
* `locations` $\textit{loc}: L \times L \rightarrow \N_0 \cup \infty$

To avoid having to deal with indices we changed our definition of `tasks` $T$ to be a finite set of any kind. Additionally, we changed `dependencies` $dep$ ot avoid having to deal with indecies and get away from a set of partial functions to a more convenient definition. 

### Search Space
* `assignments`: $A: T \longrightarrow \N_0 \cup -1$
  * maps every task to its start time slot

### Semantics
An assignment $a \in A$ is valid iff:
* $\forall t_1, t_2 \in T: a(t_1) \neq a(t_2)$
  * no two tasks can have the same starting time
  * Thoughts: would also be satisfied by $slot$ function
* $\forall t \in T: a(t) \lt H$
* $\forall t \in T: slot(t, a(t)) \in \N_0$
* $\forall t_1, t_2 \in T: dep(t_1, t_2)(a(t_2)) \in \N_0$
  * Thoughts: Where we can specify that e.g. $dep(t, t)(n) = 0 | t \in T, n \in \N$, meaning any task produces 0 cost when the dependency is checked on itself. Also, if the task is checked against a task where no direct depencency exists this is the most logical response.
* $\forall t_1, t_2 \in T: loc(t_1, t_2) \in \N_0$

#### Comparing by cost
* `cost` $c: A \longrightarrow \N_0 \cup \infty$
* $c(a) = \sum_{t_1,t_2 \in T} slot(t_1, a(t_1)) + dep(t_1, t_2)(a(t_2)) + loc(t_1, t_2)$

For any two given solutions (assignments) $a_1$ and $a_2$, $a_1$ is better iff:
* $c(a_1) < c(a_2)$

We determine the best sollution by:
* $\min(c(a)) | a \in A$

Thoughts: Something has to be changed for location but I'm at this point not sure what. It needs some notion of the time slot assigned to the tasks to be able to give a cost.

Unsure on how to handle the time slot with the existing appraoch on location (what time slot does it refer to?).

# Meeting notes
Whe handling location A after B can reduce cost of x
but B after A can reduce cost of y > x.

Add `duration` $d$
Fix `dependencies` and  

### `Assignments`
When using $\N_0 \cup - 1$ you have to handle that seperately in the assignment/duration semantics

### `Dependencies`
Instead of using $\in N_0 just write \geq 0$

### `Cost`
Fix sum in cost function

### `Locations`
Maybe just define a set of locations?