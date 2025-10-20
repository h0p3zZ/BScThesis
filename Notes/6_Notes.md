# Notes of week 6 (13.10.-19.10.)

## Problems of previous week
* Defining `dep` to be a total function instead of a partial one
* Fixing `loc` as the standing definition does not provide any meaningful functionality and is asymmetric. 
* Adding `dur` for duration to have more expressivity

## Mathematical Representation
### Data Definiition
* `tasks`: $T$ is a finite set
  * we define $T_s \sube T$ as the subset of scheduled Tasks ($a(t_s) \geq 0$)
* `horizon`: $H \in \N_0$
* `duration` $\textit{dur}: T \longrightarrow \N_0$
* `time slot function` $slot: T \times \Z \longrightarrow \N_0 \cup \infty$
  * maps every task at a given time slot to a cost
* `dependencies` $\textit{dep}: T \times T \longrightarrow \Z \longrightarrow \N_0 \cup \infty$
* `locations`: $L$ is a finite set 
  * ~~Thoughts: Is this actually needed?~~
* ~~`locations` $\sout{\textit{loc}: T \times T \longrightarrow \N \longrightarrow \N_0 \cup \infty}$~~
  * ~~Idea: Tasks with the same location give a trivial function that takes the absolute value of their distance (in time slots) and therefore gives a cost. Meaning: When tasks that should be scheduled together, are not, it will increase the cost.~~
* `locations` $\textit{loc}: T \longrightarrow L \times (\N_0 \cup \infty)$
  * $\textit{loc}(t) = (\textit{loc}_L(t), \textit{loc}_N(t))$
* `travel` $\textit{trav}: T \times T \longrightarrow \N_0 \cup \infty$ 
  * Idea: instead of looking at locations as groups of tasks at the same place, we now take it as a form of travel distance between two tasks.

### Search Space
* `assignments`: $A: T \longrightarrow \N_0 \cup \infty$
  * maps every task to its start time slot

### Semantics
An assignment $a \in A$ is valid iff:
* $\forall t_1, t_2 \in T_s | t_1 \neq t_2: a(t_1) + \textit{dur}(t_1) \leq a(t_2) \lor a(t_2) + \textit{dur}(t_2) \leq a(t_1)$
* $\forall t \in T_s: a(t) + \textit{dur}(t) \lt H$
* $\forall t \in T: slot(t, a(t)) \in \N_0$
* $\forall t_1, t_2 \in T: dep(t_1, t_2)(a(t_2)) \in \N_0$
  * or $\forall t_1, t_2 \in T: dep(t_1, t_2)(a(t_2) - a(t_1)) \in \N_0$ if the relative time slot is needed
* ~~$\sout{\forall t_1, t_2 \in T | t_1 \neq t_2: loc(t_1, t_2)(min(|a(t_1) + \textit{dur}(t_1) - a(t_2)|, |a(t_2) + \textit{dur}(t_2) - a(t_1)|)) \in \N_0}$~~
  * ~~$\sout{min(|a(t_1) + \textit{dur}(t_1) - a(t_2)|, |a(t_2) + \textit{dur}(t_2) - a(t_1)|)}$ should get the relative, absolute difference between the two tasks.~~
  * ~~With the distance one can tell how far appart the tasks are and therefore one has a messure on whether these two tasks should have been scheduled together or not~~
  * Problem with this approach:
    * When defining a cost, when is this cost appended, if one does not schedule together with another task of same location
* $\forall t_1, t_2 \in T_s | (a(t_1) < a(t_2) \land \forall t \in T_s: a(t) > a(t_1) \implies a(t) > a(t_2)): \textit{trav}(t_1, t_2) \in N_0$
  * If both tasks are scheduled, and $t_2$ is the next task after $t_1$ the travel function should return a number (not $\infty$).

#### Comparing by cost
* `cost` $c: A \longrightarrow \N_0 \cup \infty$
* $c(a) = \sum_{t \in T} slot(t, a(t)) +\\
\sum_{t_1, t_2 \in T} dep(t_1, t_2)(a(t_2) - a(t_1)) + \\
\sum_{t_1, t_2 \in T_s | (a(t_1) < a(t_2) \land \forall t \in T: a(t) > a(t_1) \implies a(t) > a(t_2))} \textit{trav}(t_1, t_2)$

For any two given solutions (assignments) $a_1$ and $a_2$, $a_1$ is better iff:
* $c(a_1) < c(a_2)$

We determine the best sollution by:
* $\min(c(a)) | a \in A$


## Meetings
* remove $\infty$ in assignment an replace by another symbol
* fix `slot` to be a function $\longrightarrow \N_0 \cup \text{symbol}$ instead of $\Z$