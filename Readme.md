RoslynDOM
=========
This provides programmer friendly view of your application via the Roslyn AST, currently in a very preliminary form

Available on NuGet
---
[Install-Package RoslynDOM -Pre](https://www.nuget.org/packages/RoslynDOM/)

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