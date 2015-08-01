---
title: Could not find R function
permalink: /ts_could_not_find_function/
tags: troubleshooting
audience: user
keywords: troubleshooting
last_updated: 
summary: Troubleshooting issues preventing R.NET from finding functions
---

{% include linkrefs.html %} 

## Issue 

Error: could not find function "aggregate"

### Description

```C#
engine.Evaluate("library(stats)");
engine.Evaluate("x <- data.frame(alfa = 1:9, beta = rep(1:3, 3))");
engine.Evaluate("aggregate(cbind(alfa) ~ beta, data = x, FUN = function(x) c(gama = mean(x)) )");
```

### Diagnosis

### Solution

See also: {{ts_paths_on_windows}}

