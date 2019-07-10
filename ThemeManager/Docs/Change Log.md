Theme Manager Change Log
========================

After Theme Manager 3.0.12, this change log was not actively maintained.  It still contains some of the more important changes.  See the [Github repository](https://github.com/regan-sarwas/ThemeManager) for details of ongoing development.

# Closed Issues

## General

- Need quick start tutorial in startup screen
- Shortcut keys (ctrl-c, ctrl-x, del, ctrl-v) do not work on selected text in the properties tab when editing a theme/category.  Instead they operate on the treeview.  Work around – use right click menu for copy paste, delete in text boxes in properties tab.
- Do not allow multiple copies to run simultaneously.  I.E.  make TM a single instance application (see [http://sanity-free.org/143/csharp\_dotnet\_single\_instance\_application.html](http://sanity-free.org/143/csharp_dotnet_single_instance_application.html))
- Initial startup with no saved settings/session loads ArcMap license un-necessarily and can't find startup html (next start is fine)
- Can't scroll long descriptions in read-only theme list property panel
- In some situations, It is possible to create categories in the root of theme list tree (i.e. not in any theme list).
  * **I have been unable to repeat this bug.  It is possible that it is from erroneous multi-select code.**
- If you add a read-only theme to favorites you cannot move/delete it (it is still read-only), until the next start.
- Save As…  on a category item is not implemented
- When deleting, next item should be selected
- Theme/Category tooltip text is not displaying
- No Icon for a ArcMap Document Data Frame
- Preview Pane should say **Loading…** after click but before map is displayed (particularly when waiting for the license server).
- Occasional crash on exit - Lost connection to license server??
  * **I have been unable to repeat this bug.**
- On Loading License... dialog provide a progress bar and cancel button
  * **(Cancel button not provided, there are two calls to ESRI Library that take all the time, so a cancel will not be responsive).**
- Data source and related properties such as workspace type (not displayed) are not being saved.

## Multi-Select

- There is a timing issue in multi-select.  Items not getting unselected when new item is clicked.
  * **Closed, duplicate of the following issue.**
- If you click and drag, the previously selected item is not unselected, and the newly selected item is not highlighted.  Both items will be added to the map if that is the drop target.  If you stop dragging and click-drag a new (non-highlighted) item will be added to the selected set.  The work around is to click, make sure the item is highlighted, then click and drag.
- It seems that the selected item (or selection set) is not always correct after  Drag 'n' Drop / Copy 'n' Paste
  * **Closed, issues discussed in detail below.**

## Drag 'n' Drop / Copy 'n' Paste

- Drag and drop of a file on theme filename text box should trigger a property update (to re-draw metadata and preview tabs)
  * **Closed, dup of Preview/Metadata display does not change listed above.**
- If you copy and item with drag and drop, then clicking on one item will select both the new and original.
  * **This problem could not be repeated, and was probably corrected as a side effect of another issue.**
- Cut and delete do not work in search results, menu is available. Sometimes this works???
  * **This was due to the read only nature of items copied to the search results.  This has been corrected.**
- Drag and drop multiple themes/categories does not work
- When copying a single item with drag and drop, the original is still selected, the new item is selected, and the parent item of the new item are all selected.

## Metadata

- Creating a new theme does not find data source metadata in a geodatabase
- Creating a new theme does not find data source metadata in a coverage or grid
- When reloading a theme, or changing a theme's source, the metadata field will get cleared or reset.  This should not be done if the existing metadata is valid, and the new is blank or invalid.
- Reloading a theme does not clear/reset broken metadata.
- Accessing geodatabase metadata is randomly slow.
- Manually changing the metadata path in properties panel to an xml file or a geodatabase data source does not find the metadata.
- Searching metadata or syncing metadata attributes can cause an out of memory error
  * **problem appears to be fixed with changes to caching.**
- Cannot format X:\Albers\parks\kefj\Shapes\Avg\_Number\_Spp\LANDBIRDS\_AVG.shp.xml.
  * **error in metadata file was corrected.**
- Regional theme list does not have Theme description/tags/summary synced with metadata when metadata is in a geodatabase
- Syncing takes too long and windows thinks it is dead.
- On Syncing... dialog provide progress bar and cancel button
- Description and other fields are cleared if the Metadata property is changed to an invalid or unfound metadata file.
- Random text will invoke catalog to find metadata.  This will often result in an error displayed in the loading ArcCatalog dialog.
  * **turned off display of dialog**

## Esoteric (programmer related)

- Items may not be cloned correctly when copied/dragged

# Open Issues (also see the help document, and GitHub Issues)

## Issues not listed in the help document

- Remove top line in context menu when **Save As..** is not enabled
- Multi-select needs much better stress testing.
- Drag and drop need much better stress testing.

## Other To Do items.

- Implement and test a deployment package
- Do not let winforms swallow exceptions. Provide message box during beta testing.
- Review `FIXME`s in code
- Implement automated unit testing
- Code cleanup – remove dead code, document public interface
- Refactor `TmNode` into derived classes: `ThemeList`, `Category`, `Theme`, `SubTheme`, and `Shortcut`
- Draw dependency diagram and simplify public interfaces
- Microsoft bug 554759 (corrupt resx for imagelist)
  - if the mainform.rex is modified, then change j00L to j0yL in the first line of the serialized treeViewImageList

# Change Log

## 3.0.0.8 ⇨ 3.0.0.9

- Reload button on properties tab had an incorrect tool tip
- Corrected the spelling of the **Syncronize All Metadata**  button.
- Added Reload Themes and Synchronize Metadata buttons to Category properties tab, to reload/synchronize all themes within a category.
- Clarified the wording on the tool tip for all the synchronize buttons on the properties tab
- Clarified the wording on the tool tip for all the reload buttons on the properties tab
- Added tool tips to the browse buttons on the properties panels
- Expanded Theme List file text box in properties panel to space left by (removed) browse button
- Added error trapping for when layer's data source cannot be found.
- Do not replace existing valid metadata is no metadata is found when reloading a theme.
- Worked on metadata validation logic (_**still needs work – see comments in code**_)
- Made all public methods internal in Metadata.
- Added logic to find metadata for multiple theme and sub-theme  types: file based, geodatabase based, grid/coverage based (_**Works for all data sources now in the regional theme list.  But has not tested for all data sources.**_)
- Added a Data Source text field to the Theme Property tab.   Data source is the path to GIS data in the layer.  A layer file with a single data layer will have a path (to the layer file), and a data source (path to the data). A group layer will have a path (to the layer file), but not have a Data Source (see the layer's sub-themes).  A sub-theme will have a data source, but not a path.
- Moved RequiredThemeLists and HelpUrl Settings from user to application space
- Moved several user settings from local to roaming
- Improved the speed of loading metadata in file/personal geodatabases (Major changes in `ESRIMetadata.cs`) (_**Works for all data sources now in the regional theme list.  But has not tested for all data sources.**_)
- Removed **reload theme** button on sub-theme property panel.
- Cleaned up caching of metadata, caching file based metadata is optional and turned off by default (load times were fast enough).  Added mandatory caching of ESRI-based (geodatabase, or non-file based) metadata.
- Added code to make sure the dragged item was selected (and other selected items are unselected unless ctrl or shift was pressed).
- Cut/Copy/Paste/Delete menu commands and the key equivalents now work on active text box or active treeview correctly.

## 3.0.0.9 ⇨ 3.0.0.10

- Changed wording on dialogs in response to reload/sync themes button on properties panel.
- When validating unknown metadata added logic to URL.   Check if the URL is file, then it is valid only if the file exists.  Removed false positives on the URL type, resulting in invalid metadata types.  Added check for empty path while validating.
- Added code to `Program.cs` and `MainForm.cs` to guarantee that only one instance of the application can be running on a computer.
- Reworked the BuildThemeFromId code in MdbStore, so that favorites from the registry did not invoke a metadata re-sync due to a path change.
- Removed obnoxious/confusing exception message generated when a favorite could not be loaded because database could not be found.  Message for database is generated, so favorites in that database are silently ignored.
- When drawing the properties panels, if item is read only, I set all text boxes to read only, instead of disabling them.  The text is now black (which I can't change), but it is selectable and scrollable.)
- Fixed add to favorites and drag/drop of read only themes.  The copy is no longer read only.  (There was a similar problem with search results) This was done by replacing the node cloning algorithm with serialization/re-serialization of the object graph.  Cloning was keeping reference pointers to existing objects, and not creating true clones.
- Modified TreeView.Delete() to select the next visible item after the last item deleted.
- Added check to make sure that a selected item is not added to the selected set (multi-select) more than once.
- Added a path separator to the check for removing descendent nodes from the selected set when doing multi-selection.  (some siblings  were being erroneously removed because they a had the same prefix as the newly selected item).
- Added ability to save a category as a new theme list.
- Added an icon for a ArcMap DataFrame sub theme.
- Found property on treeview to turn on node tool tips (it was off).
- Added advanced option to show description as a tool tip.  This option is off by default
- Preview pane displays **Loading Preview Image…** while loading ArcGIS license.
- Removed the code in Add To Favorites that selected the newly added items.
- Added the Nonserialized attribute to the IsSelected property of TmNode.  Therefore all copied items will not appear to be selected, when they were not.
- Added DataSource Properties from ThemeData object to XML Serialization code, so that these properties will be saved.
- Removed unused functions `Workspace()` and `GetDataSource()` from `ThemeData`.
- Code Cleanup in `ThemeBuilder.cs` and `LayerUtilities.cs` to ensure that any open files get closed, and to remove duplicate code.
- Added check for **Region** in data type when looking for coverage metadata
- Added trace calls around calls to `ThemeBuilder` methods
- Move Metadata reload/sync out of theme reload, unless it is done by a path change from the UI.
- Massive reorganization of `ESRILicenseManager` to remove old aborted attempts to load license in background, and re-wrote with event  notifications.
- Re-wrote the loading dialog to be generic for any long task, and to support a progress bar and cancel button.
- Added `reloadNode` event to `TMNode`, so that a tmnode tree that was updated in the background could request the UI tree to update itself based on the new tmnode tree.
- Does syncing and reloading in the background with progress and cancel buttons on the loading dialog.
- Fixed theme metadata syncing to also sync su-themes.
- Fixed administrative reports to cancel correctly, and display incomplete results if canceled.
- Added an exception catch to when loading a layer file, so an ArcObjects error will not halt loading the rest of the layer files.
- Added additional code to determine URL from WMS/IMS/AGS web service data sources.
- Added ability to Drag and Drop Multiple items.
- Changed Icons from Image Server and WMS Group Layers

## 3.0.0.10 ⇨ 3.0.0.11

- Add access to the Quick Start tutorial from the help menu
- Display nothing in lieu of **!Error Not a Data Set** for data set name and data set type in administrative report when layer data source is not a data set (i.e. a TIN)
- Added Save As… ability to categories in favorites and search results.
- Fixed bug wherein search results when searching other search results were not being displayed.
- Fixed bug wherein user was not prompted to save (save was not even allowed – only save as..) newly created theme lists.  Changes were lost unless the user remembered to do a save as…  Save button is now enabled on new theme lists, and user is prompted to save unsaved changes to new theme lists when quitting.
- Gave the SaveAs Dialog box a default file name based on the item being saved.
- Changed New Themelist to New Theme List in new document pop-up menu.
- Remove HasData Assertion from `TmNode.SaveAs()` method
- Added `Type == null` check in `ThemeData.IsShapefile()` and similar to remove error when adding non-ESRI themes.
- Turned off display of **Loading ArcCatalog** dialog when checking unknown metadata type
- Added check for Valid Metadata before Syncing description etc. when the Metadata changes.

## 3.0.0.11 ⇨ 3.0.0.12

- Removed dead code in `TMNode`, reduced visibility (public ⭢ internal/private) of functions, cleaned up comments, and regrouped methods/properties.
- Added private set to `TMNode.ImageKey` (moved code from `UpdateImageIndex()` to `ImageKey`)
- Fixed bug where Saving a category as a themelist was not copying the nodes (just referencing the same node).
- Fixed bug wherein tree node editing (advanced option) was not updating the name of the item in the properties panel.
- Removed reference to `ESRI.ArcGIS.ADF.Local` - it is not used, and is not available in 9.3
- Changed reference to `ESRI.ARcGIS.GISClient` from specific `Version = true` to `false` (all others were false), and I need it false to compile against 9.3.
- Added comments for compiling with ArcGIS 9.3
- Set the ****DontShowBetaWarning** setting to true
- Added `try/catch` around license manager checks in main form and loading form to protect against cases where ESRI libraries were not found.
- Added code to ensure that a new theme is shown and selected when created.
- Added code to ensure that new themes and categories are shown (but not selected) when created.

## 3.0.0.12 ⇨ 3.0.1.3

- Fixed bug that could have allowed a mal-formed sub-theme to be launched.
- Added ability to find metadata abstract, purpose, tags, etc from Esri Metadata.
- Sort categories above all other items.
- Disallow copy of SubThemes
- Fixed license code to work with changes in ArcGIS 10.1+ SDK.
- Fixed bug that allowed drag and drop of subthemes and theme lists.
- Added icons for word and pdf; more services now get wms icon
- Recompiled for ArcGIS 10.1 to 10.6.

## 3.0.1.3 ⇨ 3.1.00

_Only major changes are shown here.  See Github repo for all changes._

- Now uses real [W3C specification for XSLT version 1.0](https://www.w3.org/TR/xslt-10) stylesheets.
- Removes dependency on vulnerable MSXML2.0 and the obscure MS draft XSL transformation.
- Can use ESRI ArcMap 10.0+, or ArcGIS Pro metadata stylesheets (if ArcGIS is installed).
- Attributes can be extracted from ISO (19139) metadata.
- HTML tags are stripped from extracted metadata attributes when adding to Theme node.
- Can extract attributes, search and display Metadata for ArcGIS services (Map, Image, and Feature).
- Better logic for finding metadata for data sources.
- Removed option (in preferences to preload metadata).
- Metadata is always guaranteed fresh, no caching.
- Reading and saving Author data on the Themelist is supported.
- Cleaner formating of display when metadata is missing or has errors.
