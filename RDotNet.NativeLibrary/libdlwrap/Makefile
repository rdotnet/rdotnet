# Builds a thin wrapper around the "libdl" shared library and four of its functions
# On at lease one instance of one Linux flavour (CentOS), R.NET calls the 
# dlopen function in "libdl" via P/Invoke to try to load libR.so, but it fails and 'dlerror'
# returns the message 'invalid caller'. See https://rdotnet.codeplex.com/workitem/73. 
# Why this is an invalid caller could not be determined.
# Why the exact cause if this failure is unclear, a thin wrapper library around libdl.so works around this issue.
# Credits/acknowledgements:
# The author of this workaround is Daniel Collins, CSIRO, Australia. 

all: 
	gcc -Wall -fPIC -c libdlwrap.c
	gcc -shared -o libdlwrap.so libdlwrap.o
	
install: all
	cp libdlwrap.so ../bin/Debug/

clean:
	rm -f *.o *.so makefile~
