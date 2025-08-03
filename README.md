# ESENT Managed Interop

## DSInternals Fork

This fork has been created to enable publishing of strong name signed assemblies to the NuGet Gallery:

| Package                                         | Original Package                         |
|-------------------------------------------------|------------------------------------------|
| [DSInternals.ManagedEsent.Interop]              | [Microsoft.Database.ManagedEsent]        |
| [DSInternals.ManagedEsent.Isam]                 | [Microsoft.Database.Isam]                |
| [DSInternals.ManagedEsent.PersistentDictionary] | [Microsoft.Database.Collections.Generic] |

[DSInternals.ManagedEsent.Interop]: https://www.nuget.org/packages/DSInternals.ManagedEsent.Interop
[DSInternals.ManagedEsent.Isam]: https://www.nuget.org/packages/DSInternals.ManagedEsent.Isam
[DSInternals.ManagedEsent.PersistentDictionary]: https://www.nuget.org/packages/DSInternals.ManagedEsent.PersistentDictionary
[Microsoft.Database.ManagedEsent]: https://www.nuget.org/packages/Microsoft.Database.ManagedEsent
[Microsoft.Database.Isam]: https://www.nuget.org/packages/Microsoft.Database.Isam
[Microsoft.Database.Collections.Generic]: https://www.nuget.org/packages/Microsoft.Database.Collections.Generic

The intention was to modify as few files as possible. The following files have been added:

| Name                              | Description                           |
|-----------------------------------|---------------------------------------|
| [Directory.Build.props]           | Project file property overrides.      |
| [Directory.Build.targets]         | Additional project file overrides.    |
| [DSInternals.Community.snk]       | Public-private key pair for assembly strong name signing. |
| [.github/workflows/autobuild.yml] | Automated build using GitHub Actions. |
| `*/packages.lock.json`            | Package lock files to enable NuGet package caching at GitHub. |
| [global.json]                     | Specifies the .NET SDK version dependency. |

[Directory.Build.props]: Directory.Build.props
[Directory.Build.targets]: Directory.Build.targets
[DSInternals.Community.snk]: DSInternals.Community.snk
[.github/workflows/autobuild.yml]: .github/workflows/autobuild.yml
[global.json]: global.json

The newly generated public key has been injected into all `AssemblyInfo.cs` files.

## Project Info

ManagedEsent provides managed access to esent.dll, the embeddable database engine native to Windows.

The **[Microsoft.Isam.Esent.Interop](Documentation/ManagedEsentDocumentation.md)** namespace in EsentInterop.dll provides managed access to the basic ESENT API. Use this for applications that want access to the full ESENT feature set.

The **[PersistentDictionary](Documentation/PersistentDictionaryDocumentation.md)** class in EsentCollections.dll provides a persistent, generic dictionary for .NET, with LINQ support. A PersistentDictionary is backed by an ESENT database and can be used to replace a standard Dictionary, HashTable, or SortedList. Use it when you want extremely simple, reliable and fast data persistence.

**esedb** provides both dbm and shelve modules built on top of ESENT IronPython users.