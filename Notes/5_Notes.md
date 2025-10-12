# Notes of week 5 (6.11.-12.11.)

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
    "tasks" a JSON object of type dictionary with a JSON string as a key and a <task> object as a value

<task> a JSON object containing exactly the following fields:
    "name": a JSON string
    "duration": a JSON integer
    "importance": a JSON integer
    "urgendy": a JSON integer
    "timeSlots" a JSON array of type <interval>
    "dependencies" a JSON array of type <dependency>
    "location" a JSON object of type dictionary with a JSON integer as key and a JSON integer as value

<interval> a JSON object with exactly the following fields:
    "interval": a JSON array of type JSON integer of exactly two numbers
    "cost": a JSON integer

<dependency> a JSON object with exactly the following fields:
    "task": a JSON string
    "intervals" a JSON array of type <interval>
    "unmetCost" a JSON integer
```
For clarity everything in this JSON has to conform to the JSON standard (a valid JSON file). Also when "exactly the following fields" is written - it means exactly the listed fields, with exactly the specified types.

## Mathematical Representation
### Data Definiition
* `tasks`: $T$ is a finite set
* `horizon`: $H \in \N_0$
* `time slot function` $t: T \times \N_0 \longrightarrow \N_0 \cup \infty$
  * maps every task at a given time slot to a cost
* `dependencies` $\textit{dep}: T \times T \longrightarrow \N_0 \longrightarrow \N_0 \cup \infty$
* `location` $\textit{loc}: L \times L \rightarrow \N_0$

To avoid having to deal with indices we changed our definition of `tasks` $T$ to be a finite set of any kind. Additionally, we changed `dependencies` $dep$ ot avoid having to deal with indecies and get away from a set of partial functions to a more convenient definition. 

### Search Space
* `assignment`: $a: T \longrightarrow N_0$
    * maps every task to its start time slot
### Semantics