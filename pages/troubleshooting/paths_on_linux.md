---
title: Paths on Linux
permalink: /ts_paths_on_linux/
tags: troubleshooting
audience: user
keywords: troubleshooting
last_updated: 
summary: Troubleshooting issues preventing R.NET from finding R paths on Linux
---

{% include linkrefs.html %} 

## Issue 

### Description

```C#
```

### Diagnosis


```sh
~/src/github_jm/rdotnet-onboarding/ReportInfo/bin/Release$ ./ReportInfo.exe 
```

```
Is this process 64 bits? True
Info: caller provided rPath=null, rHome=null
Info: R.NET looked for preset R_HOME env. var. Found null
Info: R.NET looked for platform-specific way (e.g. win registry). Found /usr/lib/R
Info: R.NET trying to find rPath based on rHome; Deduced /usr/lib/R/lib
```

### Solution

See also: {{ts_paths_on_windows}}


