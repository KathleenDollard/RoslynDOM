RoslynDOM
=========
This provides programmer friendly view of your application via the Roslyn AST

Available on NuGet
---
[Install-Package RoslynDOM -Pre](https://www.nuget.org/packages/RoslynDOM/)

Examples
---
```C#
    string cSharpCode = File.ReadAllText("something.cs");
    SyntaxTree tree = CSharpSyntaxTree.ParseText(cSharpCode);
    IRoot root = new KFactory().CreateTreeWrapper(tree).Root ;
    INamespace nameSpace = root.Namespaces.First();
    string name = nameSpace.Name;
    
```

## LICENSE
[Apache 2.0 License](https://github.com/SignalR/SignalR/blob/master/LICENSE.md)

Questions
---
Ask on Twitter: [@kathleendollard](https://twitter.com/kathleendollard)