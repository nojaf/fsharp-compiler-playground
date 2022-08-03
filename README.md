# FSharpCompiler Playground

This repository contains some example unit tests that I frequently use to debug the F# compiler.  
These work for me and may or may not work for you.

## Getting started

The idea is that you clone this repository on the same level as `dotnet/fsharp`.
So if you have `C:\Users\nojaf\fsharp`, you want to clone to `C:\Users\nojaf\fsharp-compiler-playground`.

Build the sample project initially:

> dotnet build ./FSharpPlaygroundProject/FSharpPlaygroundProject.fsproj

Next, add the `PlaygroundTests.fs` to the `FSharp.Compiler.Service.Tests.fsproj` project.  
(I'm assuming you have opened the `FSharp.Compiler.Service.sln`)

```xml
<Compile Include="..\..\..\fsharp-compiler-playground\PlaygroundTests.fs" />
```

I typically add this just before `<Compile Include="..\service\Program.fs">`.  
Note that you never want to commit this change in your PR to the F# compiler.  
Again, this is just to play around with.

## Update FSharpPlaygroundProject

You can update the code in `Library.fs` according to your needs.
Feel free to add files or references, anything you need really.

Building the project with `-v d` will output more details by MSBuild.

```
Task "Fsc"
         C:\Program Files\dotnet\dotnet.exe "C:\Program Files\dotnet\sdk\6.0.302\FSharp\fsc.dll" -o:obj\Debug\net6.0\FSharpPlaygroundProject.dll
         -g
         --debug:portable
         --noframework
         --define:TRACE
         --define:DEBUG
         --define:NET
         --define:NET6_0
         --define:NETCOREAPP
         --define:NET5_0_OR_GREATER
         --define:NET6_0_OR_GREATER
         --define:NETCOREAPP1_0_OR_GREATER
         --define:NETCOREAPP1_1_OR_GREATER
         --define:NETCOREAPP2_0_OR_GREATER
         --define:NETCOREAPP2_1_OR_GREATER
         --define:NETCOREAPP2_2_OR_GREATER
         --define:NETCOREAPP3_0_OR_GREATER
         --define:NETCOREAPP3_1_OR_GREATER
         --doc:obj\Debug\net6.0\FSharpPlaygroundProject.xml
         --optimize-
         --tailcalls-
         -r:C:\Users\nojaf\.nuget\packages\fsharp.core\6.0.5\lib\netstandard2.1\FSharp.Core.dll
         -r:C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.7\ref\net6.0\Microsoft.CSharp.dll
         -r:C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.7\ref\net6.0\Microsoft.VisualBasic.Core.dll
         ...
         -r:C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.7\ref\net6.0\System.Xml.XPath.dll
         -r:C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.7\ref\net6.0\System.Xml.XPath.XDocument.dll
         -r:C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.7\ref\net6.0\WindowsBase.dll
         --target:library
         --warn:3
         --warnaserror:3239
         --fullpaths
         --flaterrors
         --highentropyva+
         --targetprofile:netcore
         --nocopyfsharpcore
         --deterministic+
         --simpleresolution
         obj\Debug\net6.0\.NETCoreApp,Version=v6.0.AssemblyAttributes.fs
         obj\Debug\net6.0\FSharpPlaygroundProject.AssemblyInfo.fs
         Library.fs
         Microsoft (R) F# Compiler version 12.0.4.0 for F# 6.0
         Copyright (c) Microsoft Corporation. All Rights Reserved.
       Done executing task "Fsc".
```

Any changes you see there, will need to be reflected in the `projectOptions` of `PlaygroundTests` if you wish to continue mimicking the structure.

PS: On Mac/Linux you will need to update `dotnetPacksDirectory` accordingly.  
Similar with `targetFramework`, `fsharpCoreVersion`, ... maybe you have something slightly different on your machine.

## Questions

> "Why not contribute this to dotnet/fsharp" ?

This works for me and I don't want to burden the F# team with this. I'm happy to make this publicly available but that is about it.  
There is no larger plan or anything.