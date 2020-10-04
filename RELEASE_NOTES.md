## 1.9.0

* R.NET can on all recent R versions from 3.4 onwards, including adjustments for R 4.0.x. While not extensively tested, any of the R 3.X releases should be supported. Note that on Linux only 3.5.y is supported due to a difficulty in detecting R versions on that OS.
* Workaround: The R engine overrides PATH and R_HOME if a .Renviron file is present, at least on on Windows. This led to [Default 'stats' package functions fail to load](https://github.com/rdotnet/rdotnet/issues/127)
* Change on windows: if several versions of R are installed and registered in the windows registry, the most recent is the default R engine loaded.
* Contributions by Luke Rasmussen and Kieren Johnstone are gratefully acknowledged

## 1.8.2

* Fix for [Data frame with character columns: unexpected results with R 3.5+](https://github.com/jmp75/rdotnet/issues/97). Thanks to Luke Rasmussen for this.
* R.NET can on all recent R versions from 3.4 onwards. While not extensively tested, any of the R 3.X releases should be supported. Note that on Linux only 3.5.y is supported due to a difficulty in detecting R versions on that OS.
* R.NET assemblies now target netstandard2.0 and support .NET core (2.0) and .NET framework (4.6.1+ but preferably 4.7.2 see [.NET implementation support](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support) )
* Key contributions by Luke Rasmussen, David Pendray and others are gratefully acknowledged to make this release possible

## 1.8.1

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




