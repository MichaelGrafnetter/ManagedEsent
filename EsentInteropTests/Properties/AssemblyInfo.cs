// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.
// </copyright>
// <summary>
//   Assembly properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EsentInteropTests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("EsentInteropTests")]
[assembly: AssemblyCopyright("Copyright (c) Microsoft. All Rights Reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: CLSCompliant(true)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#if STRONG_NAMED
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EsentCollectionsTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f9c95d579b685ed337ed26b3488d2f036377cb1dbc8ecdecaaf0f0e7ebf79dee332b9b5c5247690746596f414a3f6882a0b471c2f0c0e3536e06d016d3b4a6ef804d61d390c97ed3bef73dc0ff0e3f5859299f384c50b2b9c2a1aabe3d42274f46cbab42a5a7246ec029d2696865cc8d6382f38882ca86e66bde7c9ac7a9d4ae")]
#else
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EsentCollectionsTests")]
#endif