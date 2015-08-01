#!/bin/bash

E_BADARGS=85

function sample_usage ()
{
  echo "Usage: `basename $0` [--help] target"
  echo "installs SWIFT shared library dependencies into a library folder"
  echo "  Example:"
  echo "`basename $0` Tests"
  echo "  Example:"
  echo "`basename $0` NuGet"
}

if [ -n "$1" ]
then
  if [ "$1" == "--help" ] || [ "$1" == "-h" ]
  then
    sample_usage
    exit 0
  fi
fi  

if [ ! -e ./.paket/paket.bootstrapper.exe ] ; then
  echo "FATAL: paket.bootstrapper.exe not found" ;
  exit 1;
fi;

./.paket/paket.bootstrapper.exe

if [ $? == 0 ]; then
  echo "paket.bootstrapper.exe OK";
else
  echo "paket.bootstrapper.exe FAILED";
  exit 1;
fi

chmod gu+x ./.paket/paket.exe

mono ./.paket/paket.exe restore

if [ $? == 0 ]; then
  echo "paket.exe restore OK";
else
  echo "paket.exe restore FAILED";
  exit 1;
fi

mono ./packages/FAKE/tools/FAKE.exe $* --fsiargs -d:MONO build.fsx 

if [ $? == 0 ]; then
  echo "Fake.exe OK";
else
  echo "Fake.exe FAILED";
  exit 1;
fi


# "./packages/NuGet.CommandLine/tools/NuGet.exe" push bin/R.NET.Community.1.6.2.nupkg
exit 0