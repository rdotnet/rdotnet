
## 2020-09

Testing .NET 5 RC as a target, on linux

after buiding dynamic-interop

`dotnet nuget list source`

`dotnet nuget add source ${HOME}/src/github_jm/dynamic-interop-dll/nupkgs`


```sh
dotnet list RDotNet.Tests.sln package
```

doing a full txt search to replace ">netstandard2.0<" and ">netcoreapp2.0<" with ">net5.0<". Note that `dotnet list RDotNet.Tests.sln package` may still show outdated target fw not the new one.


```sh
dotnet list RDotNet.Tests.sln package


dotnet add R.NET/RDotNet.csproj package --prerelease DynamicInterop 
dotnet add R.NET/RDotNet.csproj package --prerelease Microsoft.CSharp 
dotnet add R.NET/RDotNet.csproj package --prerelease Microsoft.Win32.Registry 


dotnet add RDotNet.TestBase/RDotNet.TestBase.csproj package --prerelease NETStandard.Library 
dotnet add RDotNet.TestBase/RDotNet.TestBase.csproj package --prerelease xunit 


dotnet add RDotNet.Tests/RDotNet.Tests.csproj package --prerelease Microsoft.NET.Test.Sdk   
dotnet add RDotNet.Tests/RDotNet.Tests.csproj package --prerelease Microsoft.NETCore.App    
dotnet add RDotNet.Tests/RDotNet.Tests.csproj package --prerelease xunit                    
dotnet add RDotNet.Tests/RDotNet.Tests.csproj package --prerelease xunit.runner.visualstudio


dotnet add RDotNet.FSharp/RDotNet.FSharp.fsproj package --prerelease DynamicInterop            
# dotnet add RDotNet.FSharp/RDotNet.FSharp.fsproj package --prerelease FSharp.Core               
dotnet add RDotNet.FSharp/RDotNet.FSharp.fsproj package --prerelease NETStandard.Library


dotnet add RDotNet.FSharp.Tests/RDotNet.FSharp.Tests.fsproj package --prerelease DynamicInterop            
# dotnet add RDotNet.FSharp.Tests/RDotNet.FSharp.Tests.fsproj package --prerelease FSharp.Core               
dotnet add RDotNet.FSharp.Tests/RDotNet.FSharp.Tests.fsproj package --prerelease Microsoft.NET.Test.Sdk    
dotnet add RDotNet.FSharp.Tests/RDotNet.FSharp.Tests.fsproj package --prerelease Microsoft.NETCore.App     
dotnet add RDotNet.FSharp.Tests/RDotNet.FSharp.Tests.fsproj package --prerelease xunit                     
dotnet add RDotNet.FSharp.Tests/RDotNet.FSharp.Tests.fsproj package --prerelease xunit.runner.visualstudio 
```





