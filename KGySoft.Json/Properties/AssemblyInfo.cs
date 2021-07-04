using System;
using System.Reflection;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly:AssemblyTitle("KGySoft.Json")]
[assembly: AssemblyDescription("KGy SOFT JSON Libraries")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("KGy SOFT")]
[assembly: AssemblyProduct("KGy SOFT Libraries")]
[assembly: AssemblyCopyright("Copyright © KGy SOFT. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: CLSCompliant(true)]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]

[assembly: AllowPartiallyTrustedCallers]

#if !NET35
[assembly: SecurityRules(SecurityRuleSet.Level2, SkipVerificationInFullTrust = true)]
#endif
