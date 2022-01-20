# Calendar Pattern

This [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) library for .NET Core 2+, .NET Framework 4.6.1+, Mono 5.4+, and others helps to calculate next and previous date & times of user-defined patterns, e.g. day must be the 17th of any month.

## Problems Solved

Have you ever wanted to implement a calendar to automate actions based on sophisticated rules instead of just predefined or user-defined fixed absolute points in time? Think of something like [crontab](https://pubs.opengroup.org/onlinepubs/9699919799/utilities/crontab.html) or the [Microsoft Windows Task Scheduler](https://docs.microsoft.com/en-us/windows/win32/taskschd/about-the-task-scheduler).

Easy, huh?

But we don't want polling-like processing, e.g. continuously checking every minute, whether the current date & time matches the configured patterns. Also we want to be able to provide a preview of upcoming events to our users without wasting processing power, no matter how distant in the future the next event will be.

No worries, huh?

Have you considered that not every month has the same amount of days?

Have you considered that there are leap years?

Have you considered that there are undefined and duplicate time ranges while daylight saving time zone transitions?

I bet! But think no further, so I have, too! And I have spent quite some time in a project on this problem to come up with a reasonable solution. This code is a complete rewrite from my memories and only shares some concepts and lessons learnt.

## Usage

### Patterns

First of all, the date & time patterns must be specified all calculations are based upon. The patterns can be extended by the library user, however these are predefined and ready to use:

* Year
* Month
* Day
* DayOfWeek
* Hour
* Minute
* Second

Example:

```csharp
var monthPattern = new MonthPattern(2);
var dayPattern = new DayPattern(14);
```

### Calculator

Using the static default implementation `CalendarPattern.Calculator`'s  instance `Calculator.Default` or any other instance of the calculator can now be used to perform calculations. The `Calculator` class implements the interface `ICalculator` to allow for easier unit test mocking.

Example:

```csharp
var next = Calculator.Next(new IDateTimePattern[] { monthPattern, dayPattern }, DateTime.Now, TimeZoneInfo.Local);
```

Remember that the `Next` and `Previous` methods always find next and previous matches of the patterns after or before the start date & time and never the start date & time itself. If calculation is impossible as the pattern is impossible to fulfill, these methods return `null`.

### Time zones

It is required that the caller tells upon calculating the next or previous date & time match, which time zone the start date & time relates to. Depending on the `DateTimeKind` of the start date & time, it might get adjusted to the specified time zone. That means that a UTC start date & time will be converted to the specified time zone if it is not UTC or remains unchanged if the specified time zone is also UTC. A local or unspecified start date & time will be converted to UTC time if the specified time zone is UTC or remains unchanged if the specified time zone is also local.

Date & time calculations are based upon the given time zone, i.e. checking whether a potential calculation result is valid at all due to daylight saving transitions. If the automatic conversion of the start date & time to the specified time zone is not wanted, make sure both are UTC values or both are local values.

### Edge alignment

When calculating the next match for a date & time pattern, the result (if any) will have all date & time components, which are ranked lower than the lowest specified pattern component, set to their minimum values, which are typically zeros, e.g. any 12h wanted will have 0m 0s. In contrast, when calculating the previous match for a date & time pattern, the result (if any) will have all date & time components, which are ranked lower than the lowest specified pattern component, set to their maximum values, which can be derived from 31th December, 23h 59m 59s. E.g. any 4th will have 23h 59m 59s. In addition, the currently unavailable pattern compoents for milliseconds and ticks are also set to their maximum values which are 999ms and 9999 additional ticks. This makes up the absolute maximum these date & time components can take by the .NET `DateTime` data type.

For various reasons, one might want either the beginning or the end of the date & time range specified the patterns, no matter whether calculating the next or previous match. Therefore, the methods `Next` and `Previous` of the `Calculator` class have overloads which accept an argument specifying the wanted edge, `Beginning` or `End`. Why beginning and end? When specifying a pattern, the pattern always specifies a range instead of a point in time. E.g. even a "full" specification with all currently available pattern components down to the second specifies a range, because the milliseconds or even ticks could be different for multiple points in time with the same components from year to second. These are in total `0.9999999` seconds which currently cannot be specified by any pattern and thus make up a range. The edge specification then selects whether to calculate a date & time at the beginning (all zeros) or end of this range lower ranked than all specified date & time pattern components.
