# Valued Time

What's it do?

- Shows how your time is being spread across your values
- 

$env:DOTNET_WATCH_RESTART_ON_RUDE_EDIT = "True"


Aggregate Entities

- LifeValue
	- guid Id
	- string Name

- Activity
	- guid Id
	- string Name
	- DateTime CreatedTime
	- ActivityStatus (Enum)
	- DateOnly? CompletedDate
	- ExpectedLifeValues
		- guid LifeValueId

- ActivityLog (Per Day)
	- guid Id
	- DateOnly Date
	- PlannedActivities
		- ActivityId
	- CurrentActivity?
		- ActivityId
		- DateTime StartTime
		- TimeSpan? Estimated
	- TimeEntry[] TimeEntries
		- guid Id
		- DateTime StartTime
		- DateTime EndTime
		- TimeSpan? EstimatedDuration
		- TimeSpan ActualDuration
		- ActivityId
		- LifeValueAllocations
			- guid LifeValueId
			- TimeSpan? Duration

Ideas

- QuickApp
	- Generate test code

Tasks:

- [X] Scaffold ActivityLog entities
- [X] Add service calls and tests for adding and retrieving the activity log. (maybe won't need DTOs for everything?)
- [X] Run tests in build and publish script
- [X] Seed some data on startup and list events for current day on the page
- [X] Show add activity button
- [X] Start in progress activity
- [X] Define an estimate for new activity
- [X] Set start time anytime between now and the start of last task
- [X] Consistency check the time entries to make sure they don't collide.
- [X] Delete time entries
- [X] List activities when no current activity is pending
- [X] Start existing activity
- [X] Life value add new when starting in-progress entry
- [X] Life value selection controls when starting in-progress
- [X] Show end time
- [ ] Edit time entries
	- [ ] Edit button
	- [ ] Add Edit component
- [ ] Confirm delete time entry
- [ ] Show blank time slots
- [ ] Life value list screen
- [ ] Life value color selection control
- [ ] Dot visualization for duration per activity
- [ ] Basic day life value summary
- [ ] Setting activities as recurring
- [ ] Big Feature, needs more design: Show how much available time you have after factoring in recurring activity estimates.
- [ ]

- QuickApp
	- [X] Disclosure to objects
	- [X] Func properties
	- [X] Editable Properties
		- [X] Need to bind the existing value
		- [X] Try implementing start/stop timer with a ""creator"" object
	- [ ] Move supression into the blazor side
	- [ ] Better type name for generic types
	- [ ] Editable parameters
	- [ ] 

ReflectiveUI

Lunch 'n Learn: ReflectiveUI
Description: Zach Starkebaum will demo an experiment that's now become a brand new open source project. ReflectiveUI is a .NET library that leverages reflection to quickly create simple useful applications and interactive tools.