---
title: Recommended practices with R.NET
permalink: /tut_recommended_practices/
tags: tutorial
audience: user
keywords: overview
last_updated: 
summary: Recommended practices with R.NET
---

{% include linkrefs.html %} 

### Calling R scripts

To reuse whole scripts, the simplest method is to use the 'source' function in R. Better yet, consider creating R packages, but the comment is valid irrespective of R.NET.

```C#
engine.Evaluate("source('c:/src/path/to/myscript.r')");
```

