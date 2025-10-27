# Notes of week 7 (20.10.-26.10.)

## Goals for this week
* Cleaning up mathematical description
    * Get rid of bloating math symbols and use natural language
* Clean up definition of location
* Carry on but cross out
  * urgency (do later if time)
  * travel (do later if time - probably not)

## Mathematical Representation
We define:
* $\N_u = \N_0 \cup u$
  * u means unassigned
### Data Definiition
* `tasks`: $T$ is a finite set
* `horizon`: $H \in \N_0$
* `duration` $\textit{dur}: T \longrightarrow \N_0$
* `time slot function` $slot: T \times \N_u \longrightarrow \N_0 \cup \infty$
  * maps every task at a given time slot to a cost
* `dependencies` $\textit{dep}: T \times T \longrightarrow (\Z \cup u) \longrightarrow \N_0 \cup \infty$
* `locations`: $L$ is a finite set
* `locations` $\textit{loc}: T \longrightarrow L \times (\N_0 \cup \infty)$
  * $\textit{loc}(t) = (\textit{loc}_L(t), \textit{loc}_c(t))$
* ~~`travel` $\sout{\textit{trav}: T \times T \longrightarrow \N_0 \cup \infty}$~~

### Search Space
* `assignments`: $a: T \longrightarrow \N_u$
  * A is the set of all possible functions $T \longrightarrow N_u$
  * maps every task to its start time slot

### Semantics
We define:
* $T_s \sube T$ as the subset of scheduled Tasks ($a(t_s) \geq 0$) 
* `relDist`: $\N_u \times \N_u \longrightarrow \Z \cup u$
  * $\operatorname{relDist}(n_1, n_2) = \begin{cases} n_1 - n_2 & \text{for }n_1, n_2 \in \N_0 \\ u & \text{else}\end{cases}$
  * For readability we define use the infix notation: $n_1 - n_2 = \operatorname{relDist}(n_1, n_2)$

An assignment $a \in A$ is valid iff:
* For all pairs of distinct assigned tasks $t_1$, $t_2$ the durations must not overlap:
  *  $a(t_1) + \textit{dur}(t_1) \leq a(t_2) \lor a(t_2) + \textit{dur}(t_2) \leq a(t_1)$
* For every task assigned task $t$ the assigned time slots must not exceed the horizon.
  * $a(t) + \textit{dur}(t) \lt H$
* For every task $t$ the cost of using the suggested assignment must be finite.
  * $slot(t, a(t)) \in \N_0$
* For all pairs of tasks $t_1$, $t_2$ the
  * $dep(t_1, t_2)(a(t_2) - a(t_1)) \in \N_0$
* For all assigned tasks $t$ which are the first (grouped) at a location ($loc_L(t)$) the cost $loc_c(t)$ has to be finite.
  * For all assigned tasks $t$ where there is no assigned task $s$: $a(s) + dur(s) = a(t): loc_c(t) \in \N_0$ 
* ~~$\sout{\forall t_1, t_2 \in T_s | (a(t_1) < a(t_2) \land \forall t \in T_s: a(t) > a(t_1) \implies a(t) > a(t_2)): \textit{trav}(t_1, t_2) \in N_0}$~~
  * ~~If both tasks are scheduled, and $t_2$ is the next task after $t_1$ the travel function should return a number (not $\sout{\infty}$).~~

#### Comparing by cost
* `cost` $c: A \longrightarrow \N_0 \cup \infty$
* $c(a) = \sum_{t \in T} slot(t, a(t)) +\\
\sum_{t_1, t_2 \in T} dep(t_1, t_2)(a(t_2) - a(t_1)) + \\
\sum_{t \in T_s \text{where } \nexists s \in T_s: a(s) + dur(s) = a(t)} \textit{loc}_c(t)$
  * Summing up the $slot$ for all tasks $t$
  * Summing up the dependency costs for all pairs of tasks
  * Summing up all location costs for every first task of a block of grouped location tasks


For any two given solutions (assignments) $a_1$ and $a_2$, $a_1$ is better iff:
* $c(a_1) < c(a_2)$

We determine the best sollution by:
* $\min(c(a)) | a \in A$


### Meeting notes

* Instead of the current definition for task emmediatly before (grouped) task at a location use:
  * \sum_{t_1, t_2 \in T_s} loc_C(t_2) * (1 - \delta(loc_L(t_1), loc_L(t_2))) * \delta(a(t_1) + dur(t_1), a(t_2))
  * This does not allow tasks to have a gap between them, think of a way to schedule optional placeholder tasks between them to have a notion of "nothing to do" such that one can easily use the above notation to find if the task was scheduled at the same location.

#### TODO
* Right solution verifier - tells you if the soluation is valid (for the problem) and additionally calculates the cost.
* Set up a latex folder in the github repo such that Adrian can take a look at it (instead of overleaf).