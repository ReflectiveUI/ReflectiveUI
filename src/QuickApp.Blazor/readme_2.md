
Talk Synopsis

Thoughts on the Application Programmer Interface
Zach Starkebaum

I've been thinking a lot about how to write APIs that are easy to use and easy to understand. While I certainly don't have any clear-cut conclusions on a topic this complicated and nuanced, I'll share some of my own experiments in API design, hopefully sparking some discussion along the way; and possibly inspiring you to think a little differently about the APIs you create.

Demonstration code will be in C#, though I hope you'll join us even if that isn't your language of choice. The concepts we discuss should cover just about any language.

https://search.openverse.engineering/image/ca9ed539-8d5b-4998-81a5-79a0d94cb223
https://wordpress.org/openverse/image/1a039726-48b0-42d0-af8c-375d48667505
https://wordpress.org/openverse/image/b2691801-ab83-447a-a5cb-a7a8b99a9094
https://wordpress.org/openverse/image/8335bafb-1dbe-4638-b078-82d5a98ed699
https://search.openverse.engineering/image/c450a0c3-4ec6-4696-b78a-49696bd42640
https://search.openverse.engineering/image/c5cc4f36-e0c2-483c-a3ce-a2a5936495fc
https://search.openverse.engineering/image/fcccad3f-fb48-4d34-8dae-73c2ae4323eb
https://search.openverse.engineering/image/b308ef8c-29f5-40df-b722-9ae6c3026314
https://search.openverse.engineering/image/283d76b5-12a3-4a57-b64e-415abea15c0d
https://search.openverse.engineering/image/55fb0b3d-2ec7-4f59-b20f-83705c37d127
https://search.openverse.engineering/image/05cf5c9e-a6eb-4c50-b73f-937c825c90e7
https://search.openverse.engineering/image/94986164-2534-46c2-b71e-888902d96a76
https://search.openverse.engineering/image/9e69bbdd-eb68-473a-8ffd-6524d333d770
https://search.openverse.engineering/image/fefe1c38-8bf4-44ce-978c-44ecc7ac8c71
https://search.openverse.engineering/image/2ea69879-0913-48d9-b60e-fac35b46fe3e
https://search.openverse.engineering/image/913c3686-8ec3-4668-8537-53598bb131f6
https://wordpress.org/openverse/image/da3e3c38-1494-4a93-a478-1ee5b7df6c38
https://search.openverse.engineering/image/fc6ab3ee-3a9e-4f6a-afd5-8a764fcdfb88
https://search.openverse.engineering/image/74a09f7a-3ff2-4b92-8ee1-31107e188290
https://search.openverse.engineering/image/205023e3-ac15-4d67-96d3-98240aa7a49a

https://www.thespacetechie.com/apollo-guidance-computer/ dskey photo

https://github.com/rebelzach/Arduino-Tempo-Box

Talk Outline:

- I'm going to try to turn up the snark here because I think it makes the talk more
  fun, but I'll be one of the first to admit, I'm wrong, a lot. Like, a lot a lot.
  - Please interject if you want, but I may also have to pause a discussion if it gets long.
- What do I mean by API for the purposes of this talk?
	- Everytime you write a function you are creating an interface.
- But I'm the only one who uses this API
	- Show a little compassion for yourself.
- Some basic concepts
	- Cognitive Load
	- Obviousness
	- Constraints
		- Constructors
	- Bounded Context and Layers
		- Who consumes your code.
		- All abstractions are leaky, but that doesn't mean you have to be the one that sinks the ship.
		- A database is a bounded context but its only concerned with making sure tables and rows follow the predefined rules.
	- Abstraction
		- I should de-duplicate everything right?
			- You should de-duplicate twice as often as you are now; and only half as often as you think you should.
		- Good abstractions often evolve out of a lot of trial and error (in my experience)
			- So try things, but be willing to rip it out if it isn't working.
		- The Inner Platform Effect
			- "Thats weird, I just created a class named 'Class'."
			- I find that this goes hand-in-hand with my desire to write The Next Great American API.
			- Again we want the software to be obvious, not clever.
	- Emergent design
		- Pull in patterns when its obvious they will help you.
- I should make my interfaces generic and open-ended right?
	- No, unless you are writing the Next Great American API.
	- Your interface should directly serve the code that depends on it.
	- This doesn't mean that you should forfit your abstraction or bounded contexts.
- When to use a capital I Interface/Protocol/Duck
	- I would argue that not everything should be an interface
		- It might need to be an interface if:
			- Its an external dependency to your bounded context.
- DTOs
	- Don't get too hung up on how the data is passed to the application.
	- Unless it's causing you to leak abstractions.
	- You might be able to get away with your ORM models if you are careful. Aggregate roots can help with this.
		- Use IDs at the edges of your aggregates
- Entrypoint
	- DI has this idea of a composition root.
- Deep classes and deep interfaces
	- "It is more important for a module to have a simple interface than a simple implementation."
	  John Ousterhout, A Philosophy of Sofware Design p55
	- John O. counters this idea that classes should be small with the idea that classes should be deep
	  rather than shallow.
	- Are you implementing validation in your User Interface and In your lower level classes.
- Command and Query Responsibility Separation
	- As long as you don't google it, this is a really good principle.
	- The really high level here is: Usually the way that we create and persist data is not the same way 
	  that we want to get it out of the system.
	- So I should use commands for everything right?
		- I mean, isn't is a method
- Events and async programming
	- Be wary of things like: 
		- In process messaging.
		- The mediatr library. (Not to be confused with the mediator pattern)
		- Domain events. (Different than integration events)
	- These often are tight coupling in disguise.
	- Experiment with composition before employing these tools.
	- Exceptions/Distinctions
		- Integration events can be an awesome way to decouple things.
	  Please take a deep breath and,
	  "You'll Remember You Don't Believe In Any Of This Fate Crap. You're In Control Of Your Own Life, Remember?" - The Oracle (The Matrix)
		- 
- Documentation
	- For the love of all that is good in this world please write more comments!
	- Add comments to your public interfaces. Most IDEs read the comments on interfaces and use those comments for tooltips.
	- In C# inheritdoc is your friend for implementation in Interface implementations
- Tools that might help
	- Discriminated Unions 
- Testing?
	
- Domain driven design
	- Learn about aggregate roots, even if you don't employ them.
- Alignment with the problem domain
- Thinking about how the api maps to the domain
	- Consistent terminology
- For a while the web had the effect of cutting us off from a lot of the features
  and patterns that had been evolving.
- I have yet to work on an app that is "just CRUD"
- 
- Experiment demo intro
	- How would you use code differently if you were using it in a REPL vs using it from code.
	- When it comes to a front end it often feels like a huge undertaking to make something interactive

- Further study
	- Read about SOLID principles and then take a point of view of deep skepticism.
		- Like all concepts and guidelines, they were were in a particular setting
		  and the often the intent of the author is lost or skewed over time.
	- Learn a little lisp. The parens won't hurt you.

- Consider excavating Form Tools PDF
- WTFs per minute graphic


Types and Immediate Children

- Root : IInstanceNode
	- IMemberNode[]

- Property : IMemberNode
	- IInstanceNode

- Method : IMemberNode
	- ParameterList
	- LastExecutionResult

- ParameterList
	- Parameter[]

- LastExecutionResult
	- IInstanceNode

- Object : IInstanceNode
	- IMemberNode[]

- Value : IInstanceNode

Value is a value type or object that doesn't expose any additional children, usually intended just for display.
	
- Enumerable : IInstanceNode
	- TypeInfo
	- EnumerableItem[]

- EnumerableItem[]
	- IInstanceNode

- TypeInfo
	- TypeMemberInfo[]

- Source Generation
	- Build the InteractNode graph in code (runtime variance may make this hard)
	- Generate razor component base classes that can be implemented for different behavior

- Ideas
	- Component templating with strongly typed references
		- Could use code generator to create models for method parameters
	- Button to generate a new template component