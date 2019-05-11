
## 1.8.0

* R.NET can now also run on all recent R versions in the 3.4 and 3.5 series. While not extensively tested any of the R 3.X series should be supported. Note that on Linux only 3.5.y is supported due to a difficulty in detecting R versions on that OS.
* R.NET assemblies now target netstandard2.0 and support .NET core (2.0) and .NET framework (4.6.1+ but preferably 4.7.2 see [.NET implementation support](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support) )
* Key contributions by Luke Rasmussen, David Pendray and others are gratefully acknowledged to make this release possible

## Older versions

* 1.7.0 - Add support for unicode strings - Thanks to Wei Lu (https://github.com/Wei-Lu) for the contributions. Minor maintenance enhancements: depends on .NET 4.5.2.
* 1.6.5 - Improve discovery of windows registry, to cater for situations arising from side by side installations of R
* 1.6.4 - Fixed github #17 (do not crash when R_Visible is not exported).
* 1.6.3 - Fixed github #14 (failing to parse things such as cat('this has # hash');). Known limitation: multi-line R character strings still not parsed correctly. Use Paket for dependency management. Use FAKE. Fix incorrect dependency for R.NET.Fsharp
* 1.6.2 - Fixed github #14 (failing to parse things such as cat('this has # hash');). Known limitation: multi-line R character strings still not parsed correctly. Use Paket for dependency management. Use FAKE.
* 1.6.0 - Fix issue where some code commented out was still executed.
