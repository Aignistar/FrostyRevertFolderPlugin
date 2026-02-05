using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostySdk.Ebx;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace RevertFolderPlugin
{
    public class RevertFolderContextMenuItem : DataExplorerContextMenuExtension
    {
        // Shows the button in the main toolbar
        public override string ContextItemName => "Revert Folder & Subfolders";

        public override ImageSource Icon =>
            new ImageSourceConverter().ConvertFromString(
                "pack://application:,,,/FrostyEditor;component/Images/Revert.png"
            ) as ImageSource;

        public override RelayCommand ContextItemClicked => new RelayCommand((o) =>
        {
            AssetEntry entry = App.SelectedAsset as AssetEntry;
            if (entry == null) return;

            // Create a temporary ContextMenu
            System.Windows.Controls.ContextMenu menu = new System.Windows.Controls.ContextMenu();

            var headerItem1 = new MenuItem
            {
                Header = "With UI Refresh (goes back to root)",                
                Foreground = Brushes.Orange, // Font Color
                FontWeight = FontWeights.Bold, // Bold
                IsHitTestVisible = false
            };

            var headerItem2 = new MenuItem
            {
                Header = "Without UI Refresh (keeps file in focus)",
                Foreground = Brushes.Orange, // Font Color
                FontWeight = FontWeights.Bold, // Bold
                IsHitTestVisible = false
            };

            // Option 1: Revert + Refresh
            var item1 = new MenuItem
            {
                Header = "Revert",
                Icon = new Image
                {
                    Source = new ImageSourceConverter().ConvertFromString(
                        "pack://application:,,,/FrostyEditor;component/Images/Revert.png"
                    ) as ImageSource,
                    Width = 16,
                    Height = 16
                }
            };
            item1.Click += (s, e) => RevertFolder(entry.Path, true, false);

            // Option 2: Revert Only
            var item2 = new MenuItem
            {
                Header = "Revert",
                Icon = new Image
                {
                    Source = new ImageSourceConverter().ConvertFromString(
                        "pack://application:,,,/FrostyEditor;component/Images/Revert.png"
                    ) as ImageSource,
                    Width = 16,
                    Height = 16
                }
            };
            item2.Click += (s, e) => RevertFolder(entry.Path, false, false);

            // Option 3: Revert & all References (With Folder Refresh)
            var item3 = new MenuItem
            {
                Header = "Revert (including references)",
                Icon = new Image
                {
                    Source = new ImageSourceConverter().ConvertFromString(
                        "pack://application:,,,/FrostyEditor;component/Images/Revert.png"
                    ) as ImageSource,
                    Width = 16,
                    Height = 16
                }
            };
            item3.Click += (s, e) => RevertFolder(entry.Path, true, true);

            // Option 4: Revert & all References (Without Folder Refresh)
            var item4 = new MenuItem
            {
                Header = "Revert (including references)",
                Icon = new Image
                {
                    Source = new ImageSourceConverter().ConvertFromString(
                        "pack://application:,,,/FrostyEditor;component/Images/Revert.png"
                    ) as ImageSource,
                    Width = 16,
                    Height = 16
                }
            };
            item4.Click += (s, e) => RevertFolder(entry.Path, false, true);

            // Optional separator
            var separator1 = new System.Windows.Controls.Separator();
            var separator2 = new System.Windows.Controls.Separator();
            var separator3 = new System.Windows.Controls.Separator();
            var separator4 = new System.Windows.Controls.Separator();

            menu.Items.Add(separator1);
            menu.Items.Add(headerItem1);
            menu.Items.Add(separator2);
            menu.Items.Add(item1);
            menu.Items.Add(item3);
            menu.Items.Add(separator3);
            menu.Items.Add(headerItem2);
            menu.Items.Add(separator4);
            menu.Items.Add(item2);
            menu.Items.Add(item4);

            // Open the menu under the button
            menu.IsOpen = true;
        });
                
        //public void RevertFolder(AssetEntry entry, bool refreshExplorer)
        //{
        //    if (entry == null)
        //    {
        //        FrostyMessageBox.Show("No valid asset selected.", "Revert Folder");
        //        return;
        //    }

        //    string folderPath = entry.Path;
        //    if (string.IsNullOrEmpty(folderPath))
        //    {
        //        FrostyMessageBox.Show("Please select an asset inside a folder.", "Revert Folder");
        //        return;
        //    }

        //    // Close tabs beforehand (only for modified assets)
        //    var assetsToRevert = GetAllAssetsInFolder(folderPath);

        //    // Log list
        //    List<string> logLines = new List<string>();
        //    int totalReverted = 0;

        //    foreach (var asset in assetsToRevert)
        //    {
        //        if (asset.IsModified)
        //        {
        //            CloseModifiedTab(asset);
        //        }
        //    }

        //    // Execute revert in a TaskWindow
        //    FrostyTaskWindow.Show("Reverting Assets", $"Reverting all assets in folder: {folderPath}", (task) =>
        //    {
        //        foreach (var asset in assetsToRevert)
        //        {
        //            if (asset.IsModified)
        //            {
        //                App.AssetManager.RevertAsset(asset, suppressOnModify: false);
        //                totalReverted++;
        //                logLines.Add($"Reverted: {asset.Name}");
        //            }                        
        //        }
        //    });

        //    // Show log in a message box
        //    if (logLines.Count > 0)
        //    {
        //        string logMessage = string.Join(Environment.NewLine, logLines);
        //        FrostyMessageBox.Show(
        //            $"Total reverted assets: {totalReverted}\n\n{logMessage}",
        //            "Revert Folder"
        //        );
        //    }
        //    else
        //    {
        //        FrostyMessageBox.Show("No modified assets found to revert.", "Revert Folder");
        //    }

        //    // Update UI
        //    App.EditorWindow.DataExplorer.RefreshItems();

        //    if (!refreshExplorer)
        //        return; // Do not adjust the view

        //    App.EditorWindow.DataExplorer.ItemsSource = App.AssetManager.EnumerateEbx();

        //    // Check if there are still assets in the folder
        //    var remainingAssets = new List<AssetEntry>();
        //    foreach (var asset in App.AssetManager.EnumerateEbx())
        //    {
        //        if (asset.Path.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase))
        //            remainingAssets.Add(asset);
        //    }

        //    if (remainingAssets.Count > 0)
        //    {
        //        App.EditorWindow.DataExplorer.SelectAsset(remainingAssets[0]);
        //    }
        //}
        
        //private List<AssetEntry> GetAllAssetsInFolder(string folderPath)
        //{
        //    var result = new List<AssetEntry>();
        //    foreach (var asset in App.AssetManager.EnumerateEbx())
        //    {
        //        if (asset.Path.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase))
        //            result.Add(asset);
        //    }

        //    return result;
        //}

        private void RevertFolder(string folderPath, bool refreshExplorer, bool includeAllReferences)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                FrostyMessageBox.Show(
                    "Please select an asset inside a folder.",
                    "Revert Folder"
                );
                return;
            }

            // Log list
            //List<string> logLines = new List<string>();
            int totalReverted = 0;

            var assetsToRevert = new HashSet<AssetEntry>();

            // Collect all modified assets inside the folder
            foreach (var ebx in App.AssetManager.EnumerateEbx())
            {
                if (ebx.IsModified &&
                    ebx.Path.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase))
                {
                    assetsToRevert.Add(ebx);
                }
            }

            if (assetsToRevert.Count == 0)
                return;

            // Expand selection by reverse references
            if (includeAllReferences)
            {
                var reverseIndex = BuildReverseReferenceIndex();
                var expanded = new HashSet<AssetEntry>();

                foreach (var asset in assetsToRevert)
                {
                    CollectReverseDependencies(asset, expanded, reverseIndex);
                }

                assetsToRevert = expanded;
            }

            // Close open editor tabs
            foreach (var asset in assetsToRevert)
            {
                if (asset.IsModified)
                    CloseModifiedTab(asset);
            }

            // Execute revert
            FrostyTaskWindow.Show(
                "Reverting Assets",
                $"Reverting {assetsToRevert.Count} assets…",
                task =>
                {
                    foreach (var asset in assetsToRevert)
                    {
                        if (asset.IsModified)
                        {
                            App.AssetManager.RevertAsset(asset, suppressOnModify: false);
                            totalReverted++;
                            //logLines.Add($"Reverted: {asset.Name}");
                        }
                    }
                });

            // Show log in a message box
            //if (logLines.Count > 0)
            if (totalReverted > 0)
            {
                //string logMessage = string.Join(Environment.NewLine, logLines);
                //FrostyMessageBox.Show(
                //    $"Total reverted assets: {totalReverted}\n\n{logMessage}",
                //    "Revert Folder"
                //);
                //FrostyMessageBox.Show(
                //    $"Total reverted assets: {totalReverted}",
                //    "Revert Folder"
                //);
                App.Logger.Log($"[Revert Folder] Total reverted assets: {totalReverted}");
            }
            else
            {
                App.Logger.LogWarning($"[Revert Folder] No modified assets found to revert.");
                //FrostyMessageBox.Show("No modified assets found to revert.", "Revert Folder");
            }

            App.EditorWindow.DataExplorer.RefreshItems();

            // Refresh UI
            if (!refreshExplorer)
                return; // Do not adjust the view            

            App.EditorWindow.DataExplorer.ItemsSource = App.AssetManager.EnumerateEbx();

            // Check if there are still assets in the folder
            var remainingAssets = new List<AssetEntry>();
            foreach (var asset in App.AssetManager.EnumerateEbx())
            {
                if (asset.Path.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase))
                    remainingAssets.Add(asset);
            }

            if (remainingAssets.Count > 0)
            {
                App.EditorWindow.DataExplorer.SelectAsset(remainingAssets[0]);
            }
        }

        // ------------------------------------------------------------
        // Reverse reference handling
        // ------------------------------------------------------------
        private Dictionary<AssetEntry, HashSet<AssetEntry>> BuildReverseReferenceIndex()
        {
            var index = new Dictionary<AssetEntry, HashSet<AssetEntry>>();

            foreach (var ebx in App.AssetManager.EnumerateEbx())
            {
                if (!ebx.IsModified)
                    continue;

                var dataObject = ebx.ModifiedEntry?.DataObject;
                if (dataObject == null)
                    continue;

                foreach (var referenced in ExtractReferencedAssets(dataObject))
                {
                    if (!index.TryGetValue(referenced, out var list))
                    {
                        list = new HashSet<AssetEntry>();
                        index[referenced] = list;
                    }

                    list.Add(ebx);
                }
            }

            return index;
        }

        private void CollectReverseDependencies(
            AssetEntry asset,
            HashSet<AssetEntry> result,
            Dictionary<AssetEntry, HashSet<AssetEntry>> reverseIndex)
        {
            if (!result.Add(asset))
                return;

            if (reverseIndex.TryGetValue(asset, out var dependents))
            {
                foreach (var dep in dependents)
                {
                    CollectReverseDependencies(dep, result, reverseIndex);
                }
            }
        }

        // ------------------------------------------------------------
        // EBX object reference walker
        // ------------------------------------------------------------
        private IEnumerable<AssetEntry> ExtractReferencedAssets(object root)
        {
            var result = new HashSet<AssetEntry>();
            var visited = new HashSet<object>();

            void Walk(object obj)
            {
                if (obj == null || visited.Contains(obj))
                    return;

                visited.Add(obj);

                // EBX pointer reference
                if (obj is PointerRef pointer && pointer.External != null)
                {
                    var guid = pointer.External.FileGuid;
                    var entry = App.AssetManager.GetEbxEntry(guid);
                    if (entry != null)
                        result.Add(entry);

                    return;
                }

                // Collections
                if (obj is System.Collections.IEnumerable enumerable && !(obj is string))
                {
                    foreach (var item in enumerable)
                        Walk(item);
                    return;
                }

                // Walk properties
                var type = obj.GetType();
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!prop.CanRead)
                        continue;

                    try
                    {
                        Walk(prop.GetValue(obj));
                    }
                    catch
                    {
                        // ignore broken getters
                    }
                }
            }

            Walk(root);
            return result;
        }

        // ------------------------------------------------------------
        // Editor tab handling
        // ------------------------------------------------------------
        private void CloseModifiedTab(AssetEntry entry)
        {
            if (!entry.IsModified)
                return;

            // Access the EditorWindow instance
            var editorWindow = App.EditorWindow;

            // Use reflection to get the private TabControl
            var tabControlField = editorWindow.GetType().GetField("TabControl", BindingFlags.Instance | BindingFlags.NonPublic);
            if (tabControlField == null)
                return;

            TabControl tabControl = tabControlField.GetValue(editorWindow) as TabControl;
            if (tabControl == null)
                return;

            // Iterate through the tabs and close the matching one
            for (int i = 1; i < tabControl.Items.Count; i++)
            {
                FrostyTabItem tabItem = tabControl.Items[i] as FrostyTabItem;
                if (tabItem != null && tabItem.TabId == entry.Name)
                {
                    // Call Editor-Closed methods
                    (tabItem.Content as FrostyBaseEditor)?.Closed();
                    (tabItem.Content as FrostyAssetEditor)?.Closed();

                    // Remove the tab
                    tabControl.Items.Remove(tabItem);

                    // If only the first (default) tab remains
                    if (tabControl.Items.Count == 1)
                    {
                        FrostyTabItem defaultTab = tabControl.Items[0] as FrostyTabItem;
                        defaultTab.Visibility = System.Windows.Visibility.Visible;
                        defaultTab.IsSelected = true;
                    }
                    break;
                }
            }
        }
    }
}