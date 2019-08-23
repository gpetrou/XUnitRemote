This is an example of running an xUnit.net test in a separate process.

Projects: 

- XUnitRemote: Contains the xUnit.net classes that are used for running a RemoteHostFact.
- XUnitRemoteHost: Contains a small program that can be used to run a method from a test dll.
- XUnitRemoteTests: Contains an example of a RemoteHostFact usage.

Current Status:

- When using ReSharper runner to execute the net452 version, it works.
- When using ReSharper runner to execute the netcoreapp3.0 version, it outputs `XUnitRemoteTests.UnitTest1.RemoteHostFactAttributeTest was left pending after its run completion.`
- When using the `dotnet test` or `dotnet vstest` it outputs `No test is available in PATH_TO\XUnitRemote\XUnitRemote\XUnitRemoteTests\bin\Debug\netcoreapp3.0\XUnitRemoteTests.dll. Make sure that test discoverer & executors are registered and platform & framework version settings are appropriate and try again.`
- When debugging the net452 and netcoreapp3.0, both of them reach the `return tcs.Task;`, `runSummary` contains the same values and exitCode is equal to 0.
- When using the Test Explorer from Visual Studio to execute net452 and netcoreapp3.0, both output `1 tests found ` but `0 tests run`.