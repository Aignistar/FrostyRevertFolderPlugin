using Frosty.Core.Attributes;
using RevertFolderPlugin;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: ComVisible(false)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly
)]

[assembly: PluginDisplayName("Revert Folder")]
[assembly: PluginAuthor("Aigni")]
[assembly: PluginVersion("1.0.1.0")]

[assembly: RegisterDataExplorerContextMenu(typeof(RevertFolderContextMenuItem))]

[assembly: AssemblyTitle("Revert Folder")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Aigni")]
[assembly: AssemblyProduct("Revert Folder")]
[assembly: AssemblyCopyright("Copyright © Aigni 2026")]

[assembly: AssemblyVersion("1.0.1")]