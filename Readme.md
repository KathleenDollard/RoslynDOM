RoslynDom
=========
This provides programmer friendly view of your application via the .NET Compiler Platform (code named Roslyn) from Microsoft. 

- The best thing about Roslyn is that it exposes the syntactic and semantic trees of the compiler
- The worst thing about Roslyn is that it exposes the syntactic and semantic trees of the compiler

It's a freakin' awesome compiler, but you don't think like that. No one thinks like that. 

You might want to use this tool, RoslynDom to 

- Perform complex refactoring that isn't provided by the services of Roslyn or easily created via an Analyzer. For example I added the property assigning constructors to RoslynDom using RoslynDom.
- More easily do most of the things you would do with a syntax walker
- Probably, get better performance in iterative updates that require multiple rewriter passes in Roslyn
- Do things with the compiler information without having to think like a compiler

Components
--
It's probably easiest to understand RoslynDom by understanding it's components

RoslynDom
---
A set of mutable (yes, mutable) classes that represent your code as close to how you think about code as I've been able to accomplish so far. That's sort of an "average you" so you may not love everything, but I'm kind of a geek on the human/programming language interface and I worked hard and watched closely how we talk about code, which is a good clue about how we think about code. 

These classes are built on a set of characteristic based interfaces.

I planned to be VB/C# language agnostic and to be friendly to the idea of interacting with other languages in parallel trees. 

RoslynDom C# Factories
---
There is pretty much a one-to-one relationship between the RoslynDom classes and a C# factory.

The C# factories take The .NET Compiler Factories, Roslyn, syntax and semantic trees and sort through them to load RoslynDom. It's messy work sometimes.

The C# factories also take RoslynDom classes and recreate The .NET Compiler Platform, Roslyn syntax trees. 

The future RoslynDom VB Factories
---
Not yet created. 

For certain scenarios, this will allow large scale VB/C# conversions. I say for certain scenarios because VB and C# have some significant differences. The thing about this approach is that we can reason about the code, look for problem areas, refactor, etc. in the common tree. 

The Characteristic Interfaces
---
One of the frustrating things about The .NET Compiler Platform, Roslyn is that it has a very shallow syntax tree, which means very little polymorphism. There are very good reasons to do that in the compiler, and I'm also not a fan of deep hierarchies for anything, particularly where the commonality is as fluid as in the syntax tree. 

So, I create an interface to correspond to each entity in RoslynDom, and a series of interfaces for characteristics. Sometimes these characteristic interfaces are quite small with only one property. 

There are two important benefits of these small interfaces. First, you can go look up an entities interface and wander around the tree to find out what it can do. I want some visualization of this, but I'm not there yet. 

Second, as you're working with the tree, you can simply attempt a cast (an as cast) and see if you have that property to work with. No need to understand any other aspect of the entity. 

Supporting Players
---
There's a fair amount of sometimes messy code that supports all this. 

Testing
---
There's over 600 passing unit tests, and about 28 tests that are inconclusive because I know I need them and havne't gotten to it. 

There is a lot of testing still to do. I'm only at 87% coverage and I think a core library should be at near 100%. You might notice that I have a tendency to increase what's at near 100%, while postponing testing in other areas. I think it makes it easier to know where to still be careful. 

Why did I do this?
---
It's been an enormous effort. 

I didn't do it to do it. I did it because I have two generation projects - CodeFirstMetadata and ExpansionFirstTemplates that entirely rely on it. The prototypes of those projects used The .NET Compiler Platform, Roslyn directly. The complexity that arose from combining the concerns of compile detail and quirks with trying to get the generation job done made for exceedingly complex code. 

And in a workshop last spring I said "I'm sure the community will build a wrapper for Roslyn" and a few decades of experience echoed in my head "you are community."

My only real feedback to date has been those projects. I turn to them,make some headway and turn back to make RoslynDom better. The vast majority of my work has been to make RoslynDom good (it's not done, and could use refactoring, etc, but I'm very happy with the core). As a result, those two projects, which tackle much more difficult problems are relatively small and as easy to understand as complex mapping problems can be.

So, here you go. I am community! You are community, and what I most need right now from community is feedback. This is still a young project and I'm still available to answer questions like "is RoslynDom a good idea to do xxx." 



Available on NuGet
---
[Install-Package RoslynDOM -Pre](https://www.nuget.org/packages/RoslynDOM/)

More info
---
You can find more info on [my blog] (http://msmvps.com/blogs/kathleen/default.aspx)

Examples
---
```C#
    string cSharpCode = File.ReadAllText("something.cs");
    IRoot root = RDomFactory.GetRootFromString(csharpCode);
    INamespace nameSpace = root.Namespaces.First();
    string name = nameSpace.Name;
    
```

## LICENSE
[Apache 2.0 License](https://github.com/SignalR/SignalR/blob/master/LICENSE.md)

Questions
---
Ask on Twitter: [@kathleendollard](https://twitter.com/kathleendollard)