// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: AssemblyTitle("Microsoft.Phone.Controls.Toolkit")]
[assembly: AssemblyDescription("Windows Phone Toolkit")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyProduct("Microsoft® Windows Phone")]
[assembly: AssemblyCopyright("© Microsoft Corporation.  All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("280ba6f6-7236-4067-a258-6e8510f629ad")]

[assembly: AssemblyVersion("7.0.1.0")]
[assembly: AssemblyFileVersion("7.0.1.0")]

[assembly: CLSCompliant(false)] // IApplicationBar is not CLS-compliant, but its use matches the type of the platform's PhoneApplicationPage.ApplicationBar property
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: XmlnsPrefix("clr-namespace:Microsoft.Phone.Controls.Updated;assembly=Microsoft.Phone.Controls.Toolkit.Updated", "toolkit")]
[assembly: XmlnsDefinition("clr-namespace:Microsoft.Phone.Control.Updateds;assembly=Microsoft.Phone.Controls.Toolkit.Updated", "Microsoft.Phone.Controls")]
[assembly: XmlnsPrefix("clr-namespace:Microsoft.Phone.Controls.Updated.Primitives;assembly=Microsoft.Phone.Controls.Toolkit.Updated", "toolkitPrimitives")]
[assembly: XmlnsDefinition("clr-namespace:Microsoft.Phone.Controls.Updated.Primitives;assembly=Microsoft.Phone.Controls.Toolkit.Updated", "Microsoft.Phone.Controls.Primitives")]
