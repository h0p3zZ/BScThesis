I think now the issues with presentation are more clear we can work on how to present the theoretical contribution in a more step-by-step way.
You should not assume the reader knows the boundaries between different "realms" of your modeling.
What I mean by that is that at each point if should be clear if a notion you're introducing is either:
* A real world phenomenon you are aiming to model.
* A mathematical concept that is intended to model some real world feature.
* A derived mathematical expression that is intended to fit one of those mathematical concepts into the formalisms you are working with (say, ILP or SAT).
The difference between these is sometimes subtle or fuzzy, but right now these realms are quite mixed.

I propose you separate your presentation into the following rough stages. You might want to use sectioning only for some of them; use your own judgment there.

1. A description of the real world problem you're tackling. To some extent, this is already in the introduction, but here you can go in more detail,
    explaining what kind of situations you're trying to capture (e.g. repeating/split tasks, allowed time intervals, preferred time intervals).
    Here you do not yet specify exactly what each term means, but you keep it to "general population" language.

2. A presentation of the terminology you will be using. Here's where you explain e.g. that you are dividing time in discrete units called blocks,
    that you're only considering blocks up to a horizon, the notion of windows and so on.
    Note that at this point you're only introducing *concepts*. You're not yet explaining how these concepts relate to each other.
    For example, you can talk about windows without talking about tasks themselves.

3. A mathematical description of the problem. That is, you present what you need to know in order to even have *something* to solve.
    In more CS terms, this is the mathematical equivalent to declaring a class that contains an API request. So, for example,
    if you think of a SAT solver, here I would simply say that a SAT problem is a list of clauses, where each clause is itself a
    list of positive or negative variables. Note that I've said *nothing* about how to solve this problem or what a "solution" even is.
    The goal here is for the reader to understand what the problem *depends* on.

4. A mathematical description of the search space for a given problem from (3). Here's where you'd finally introduce the objects you're trying
    to find, namely mappings T x B --> {0, 1}.

5. A mathematical description of the semantics of solutions. That is, when an element from (4) would be a feasible solution of (3),
    and when one such feasible solution is prefered over another solution. Note that here you haven't even started talking about
    how you'll be solving this, so you still have access to the full expressivity of math, and right now you're not using that.
    For example, some of the definitions you currently introduce in 3.3.1. are basically capturing the ideas of contiguous intervals of time,
    and of gaps between two consecutive intervals. Here you are trying to express these ideas in, say, the LaTeX version of MiniZinc.
    Instead, you could say something like, for example:
    for each task `t` and each block `b` where `M(t, b) = 1`, there is an interval `I` contained in `B` such that
    
    * `I` has length at least `t.minExecutionSpan`.
    * `b` is in `I`.
    * `M(t, b') = 1` for all `b'` in `I`.

    Note that writing this in MiniZinc is difficult, but for a reader it is *far easier* to understand what the semantics are if you write them like this.
    Your goals here are, on the one hand, that the reader understands the connection between the math and the real world phenomenon you are
    modeling; and on the other hand, that what is a solution and what isn't is clear beyond any ambiguity.

6. Finally, a section on how to encode the mathematical conditions in (5) to constraints that are amenable to be implemented in MiniZinc.
    This part would contain most of what you currently write in Section 3.3. But now the situation is different:
    * The reader now knows where you're going with this and what you're trying to achieve.
    * Now the flow would not be interrupted by what the constraints intend to capture.
    * Most importantly, you are now free to explain that huge detour with helper functions while knowing the reader didn't get lost.