#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonModule.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Runtime.CompilerServices;

#endregion

namespace KGySoft.Json
{
    /// <summary>
    /// Represents the <c>KGySoft.Json</c> module.
    /// </summary>
    public static class JsonModule
    {
        #region Methods

        /// <summary>
        /// Initializes the <c>KGySoft.Json</c> module. It initializes the resource manager for string resources and registers its central management
        /// in the <a href="http://docs.kgysoft.net/corelibraries/?topic=html/T_KGySoft_LanguageSettings.htm" target="_blank">LanguageSettings</a> class.
        /// <br/>See the <strong>Remarks</strong> section for details.
        /// </summary>
        /// <remarks>
        /// <note>The module initializer is executed automatically when any member is accessed in the module for the first time. This method is public to able
        /// to trigger module initialization without performing any other operation. Normally you don't need to call it explicitly but it can be useful if you use
        /// the KGySoft JSON Libraries in an application and you want to configure resource management on starting the application via
        /// the <a href="http://docs.kgysoft.net/corelibraries/?topic=html/T_KGySoft_LanguageSettings.htm" target="_blank">LanguageSettings</a> class.
        /// In such case you can call this method before configuring language settings to make sure that the resources of
        /// the <c>KGySoft.Json.dll</c> are also affected by the settings.</note>
        /// </remarks>
        [ModuleInitializer]
#if NET6_0_OR_GREATER
        [SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute should not be used in libraries",
            Justification = "See the description, it is intended and required, and besides it's public so can be explicitly called from an application startup.")] 
#endif
        public static void Initialize()
        {
            // Just referencing Res in order to trigger its static constructor and initialize the project resources.
            Res.EnsureInitialized();
        }

        #endregion
    }
}
