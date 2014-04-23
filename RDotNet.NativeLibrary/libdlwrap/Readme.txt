
You normally do not need to install this workaround on most Linux platform. 

If, however, you get a message 'invalid caller' message from the dlerror function, read on.

On Linux, R.NET calls the dlopen function in "libdl" via P/Invoke to try to load libR.so, On at least one instance of one Linux flavour (CentOS), it fails and 'dlerror' returns the message 'invalid caller'. See https://rdotnet.codeplex.com/workitem/73 for detailed information. Why this is an "invalid caller" could not be determined. While the exact cause if this failure is unclear, a thin wrapper library around libdl.so works around this issue.

You build and install this workaround with the following commands:

	RDOTNET_BIN_DIR=~/my/path/to/rdotnetbin

in the directory of the present Readme.txt file:

	make
	less sample.config # skim the comments, for information
	cp sample.config $RDOTNET_BIN_DIR/RDotNet.NativeLibrary.dll.config
	cp libdlwrap.so  $RDOTNET_BIN_DIR/


Credits/acknowledgements:
The author of this workaround is Daniel Collins, CSIRO, Perth, Australia. 
