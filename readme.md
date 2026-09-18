# BenchmarkDotNet iOS Test

New project builds fine.

Adding `BenchmarkDotNet` nuget and now it fails with the following:

```
BenchmarkDotNetiOSTest net9.0-ios failed with 1 error(s) (8.2s) → BenchmarkDotNetiOSTest/bin/Release/net9.0-ios/ios-arm64/BenchmarkDotNetiOSTest.dll
    /usr/local/share/dotnet/packs/Microsoft.iOS.Sdk.net9.0_18.4/18.4.9288/targets/Xamarin.Shared.Sdk.targets(917,3): error : ILStrip failed for /Users/beeradmoore/.nuget/packages/microsoft.diagnostics.tracing.traceevent/3.1.8/build/native/arm64/KernelTraceControl.dll: Invalid PE file

Build failed with 1 error(s) in 8.3s
```

This can be fixed with the following changes to `BenchmarkDotNetiOSTest.csproj`. However these builds will likely not be accepted by the AppStore.

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <EnableAssemblyILStripping>false</EnableAssemblyILStripping>
</PropertyGroup>
```

However 