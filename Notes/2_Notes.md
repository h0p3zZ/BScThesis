# Notes of week 2 (11.08.-17.08.)

From the previous week (1_Notes.md) we have spotted several problems with the previous approach:

- Priorities can take several forms: urgency != importance
- Let the front-end deal with dates and holidays
- You need to find a good way to make this work with reoccurrences.
- Currently, you can only express order dependencies (i.e. X happens before Y), but sometimes you also want time dependencies (X happens at most 1 week after and at least 2 days after Y).
- Instead of location, you could consider clustering constraints where some tasks happen preferably in clusters with each other.
- Duration can have many variations (variable duration, task splitting, preferred duration). You should think about which of these you can already express within your language, and which ones would need new features.

This means: we need to reiterate the IR language:

## Urgency != Importance
As urgency restricts us in terms of how far in the future the task can be scheduled, importance gives us a metric of how important it is to schedule this task correctly (or at all).

## Dates and Holidays
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

## Reoccurrences
As reoccurrences are tricky to be defined in general and especially tricky if they have to be simple. Therefore, we surpass this issue by scheduling multiple tasks in the given time horizon. If one wants to go shopping every week we schedule x different tasks - where x is the weeks contained in the given time horizon - with each task containing the `includingDate` of the respective week (excluding local restrictions when shops are closed on holidays or weekends). 

## Improved dependencies
Task dependencies should contain time horizon in which the follow-up-task has to be scheduled.

## Handling location
The tasks in common location should be - in the best case - scheduled together to be the most time efficient. 