using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("R.NET")]
[assembly: AssemblyDescription(".NET bindings for the official R statistical computing language")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("R.NET")]
[assembly: AssemblyCopyright("Copyright © Copyright 2011-2014 RecycleBin, Copyright © 2014-2017 CSIRO")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("9ecd1b81-67cd-417a-8b4d-77847c3353d4")]
#if NETSTANDARD2_0
[assembly: InternalsVisibleTo("RDotNet.Tests.x")]
#else
[assembly: InternalsVisibleTo("RDotNet.Tests")]
#endif