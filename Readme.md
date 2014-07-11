RoslynDom
=========
This provides programmer friendly view of your application via the .NET Compiler Platform (code named Roslyn) from Microsoft. This is an early, experimental release.

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