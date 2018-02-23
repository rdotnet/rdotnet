@echo off
dotnet build ../R.NET/RDotNet.Core.csproj -c release -f netstandard2.0 -o ../build/RCore.NET/release/netstandard2.0
dotnet build ../R.NET/RDotNet.Core.csproj -c release -f net40 -o ../build/RCore.NET/release/net40
dotnet build ../R.NET/RDotNet.Core.csproj -c release -f net46 -o ../build/RCore.NET/release/net46

nuget pack ../nuget/RDotNet.Core.nuspec

echo 构建完成
pause