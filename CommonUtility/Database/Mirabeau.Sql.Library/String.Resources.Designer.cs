﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mirabeau.Sql.Library
{
    using System;


    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class String_Resources
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal String_Resources()
        {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Mirabeau.Sql.Library.String.Resources", typeof(String_Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to cannot be null.
        /// </summary>
        internal static string CannotbeNull
        {
            get
            {
                return ResourceManager.GetString("CannotbeNull", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to cannot be null or empty.
        /// </summary>
        internal static string CannotbeNullOrEmpty
        {
            get
            {
                return ResourceManager.GetString("CannotbeNullOrEmpty", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to [Errorcode: {0}] {1}.
        /// </summary>
        internal static string errorCodeFormatString
        {
            get
            {
                return ResourceManager.GetString("errorCodeFormatString", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to No errorcode available for errorcode: {0}..
        /// </summary>
        internal static string NoErrorCodePresent
        {
            get
            {
                return ResourceManager.GetString("NoErrorCodePresent", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Parameter {0} cannot be null..
        /// </summary>
        internal static string ParameterCannotBeNull
        {
            get
            {
                return ResourceManager.GetString("ParameterCannotBeNull", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Parameter count does not match Parameter Value count..
        /// </summary>
        internal static string ParameterCountDoesNotMatch
        {
            get
            {
                return ResourceManager.GetString("ParameterCountDoesNotMatch", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Required version of assembly {0} is {1}. Found verions is {2}..
        /// </summary>
        internal static string RequiredAssemblyNotFound
        {
            get
            {
                return ResourceManager.GetString("RequiredAssemblyNotFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Wrong format: {0}.
        /// </summary>
        internal static string WrongFormat
        {
            get
            {
                return ResourceManager.GetString("WrongFormat", resourceCulture);
            }
        }
    }
}