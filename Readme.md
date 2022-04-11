# Battleships


## Build
Ensure you have the .NET 6 SDK installed.

```
dotnet restore
dotnet build
```
### Example output

```
sashan@charon ~/tmp (master)> dotnet build
Microsoft (R) Build Engine version 17.0.0+c9eb9dd64 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored /home/sashan/tmp/battleship.fsproj (in 202 ms).
  battleship -> /home/sashan/tmp/bin/Debug/net6.0/battleship.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.82
sashan@charon ~/tmp (master)> dotnet test
  Determining projects to restore...
  All projects are up-to-date for restore.
sashan@charon ~/tmp (master)> cd test/
sashan@charon ~/t/test (master)> dotnet test
  Determining projects to restore...
  Restored /home/sashan/tmp/test/test.fsproj (in 843 ms).
  1 of 2 projects are up-to-date for restore.
  battleship -> /home/sashan/tmp/bin/Debug/net6.0/battleship.dll
  test -> /home/sashan/tmp/test/bin/Debug/net6.0/test.dll
Test run for /home/sashan/tmp/test/bin/Debug/net6.0/test.dll (.NETCoreApp,Version=v6.0)
Microsoft (R) Test Execution Command Line Tool Version 17.0.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     5, Skipped:     0, Total:     5, Duration: 6 ms - /home/sashan/tmp/test/bin/Debug/net6.0/test.dll (net6.0)
sashan@charon ~/t/test (master)>
```

## Test

```
cd test; dotnet test
```
