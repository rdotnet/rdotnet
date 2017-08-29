* 1.7.0 - Add support for unicode strings - Thanks to Wei Lu (https://github.com/Wei-Lu) for the contributions. Minor maintenance enhancements: depends on .NET 4.5.2.
* 1.6.5 - Improve discovery of windows registry, to cater for situations arising from side by side installations of R
* 1.6.4 - Fixed github #17 (do not crash when R_Visible is not exported).
* 1.6.3 - Fixed github #14 (failing to parse things such as cat('this has # hash');). Known limitation: multi-line R character strings still not parsed correctly. Use Paket for dependency management. Use FAKE. Fix incorrect dependency for R.NET.Fsharp
* 1.6.2 - Fixed github #14 (failing to parse things such as cat('this has # hash');). Known limitation: multi-line R character strings still not parsed correctly. Use Paket for dependency management. Use FAKE.
* 1.6.0 - Fix issue where some code commented out was still executed.
