Theme Manager Help
==================

# Contents
* [Introduction](#introduction)
* [Quick Start](#quick-start)
* [Installation](#installation)
  * [Requirements](#requirements)
  * [Using Esri Metadata Stylesheets](#using-esri-metadata-stylesheets)
  * [Using Theme Manager on a Network](#using-theme-manager-on-a-network)
  * [Accessing Theme Manager from ArcMap](#accessing-theme-manager-from-arcmap)
* [What's New](#whats-new)
  * [Theme Manager 3.1](#theme-manager-31)
  * [Theme Manager 3.0](#theme-manager-30)
* [Using Theme Manager](#using-theme-manager)
  * [Browsing](#browsing)
    * [Theme Lists](#theme-lists)
    * [Themes Tab](#themes-tab)
    * [Favorites Tab](#favorites-tab)
    * [Search Results Tab](#search-results-tab)
    * [Metadata Tab](#metadata-tab)
    * [Preview Panel](#preview-panel)
    * [Properties Panel](#properties-panel)
  * [Adding Themes to ArcMap](#adding-themes-to-arcmap)
  * [Searching](#searching)
  * [Favorites](#favorites)
* [Managing Theme Lists](#managing-theme-lists)
  * [Creating Theme Lists](#creating-theme-lists)
  * [Adding Themes](#adding-themes)
  * [Organizing Themes](#organizing-themes)
  * [Editing Properties](#editing-properties)
  * [Reloading Themes](#reloading-themes)
  * [Synchronizing Themes with Metadata](#synchronizing-themes-with-metadata)
  * [Saving](#saving)
  * [Administrative Reports](#administrative-reports)
* [Advanced Topics](#advanced-topics)
  * [Startup](#startup)
  * [Configuration Files](#configuration-files)
    * [Application Defaults](#application-defaults)
    * [User Preferences](#user-preferences)
    * [Session Information](#session-information)
    * [Contents of ThemeManager.exe.config](#contents-of-thememanagerexeconfig)
  * [Multi-select](#multi-select)
  * [Advanced Options](#advanced-options)
    * [General Options](#general-options)
    * [Tree View](#tree-view)
    * [Menus](#menus)
    * [Other](#other)
* [Known Issues](#known-issues)
* [Ideas for Future Versions](#ideas-for-future-versions)
* [Contact Information](#contact-information)

# Introduction

Theme Manager simplifies access to GIS data. It provides the most benefit to occasional GIS users, and those in organizations with large data holdings. It can be used on a computer without any GIS software, but to get full access to all the features you must have ArcGIS Desktop version 10 installed (and licensed) on your computer. It is an alternative to the _file based_ ArcCatalog method of data management. Theme Manager uses an XML database to organize layer in a hierarchical structure while not affecting the underlying data storage structure. As a result, users are presented a simple, consistent, and quick way to browse, search and access symbolized datasets

# Quick Start

A separate [Quick Start Tutorial](Startup.html) is available from the help menu

# Installation

Theme Manager is delivered as a zip file that can be extracted to any convenient location[^1]. Theme Manager does not write to the registry, or make any changes to your system, so no administrative privileges are required to install or run it.

To run Theme Manager, just double click on the executable file (Theme Manager.exe) in the folder where you extracted the download.

The executable file (Theme Manager.exe) relies on various files and sub-folders that come with it, and it expects to find them in the same relative locations. This means that if you want to move or copy the application, you must move or copy the entire folder. Putting a shortcut to the executable file on your desktop is fine (use ctrl-shift-drag or right click, create shortcut), but do not copy just Theme Manager.exe to your desktop and expect it to run correctly.

## Requirements

Theme Manager 3.1 requires the Microsoft .Net Framework 4.5 or higher (This is also a requirement for ArcGIS 10.4+ and other software, and has been a core component of Windows software since 2013, so you probably have it. If not, it can be downloaded for free from Microsoft.

Theme Manager can be used on a computer without any GIS software, but to get full access to all the features you must have ArcGIS Desktop version 10.5 (or higher) installed (and licensed) on your computer. There is a different version of Theme Manager for each version for ArcGIS. If you have installed mismatched versions, Theme Manager will run but you will get error and you will not have access to any of the features that require ArcGIS. Theme Manager will work with ArcGIS Pro, but some features (mainly previewing themes and loading/searching/displaying metadata in a geodatabase) do not work.

Theme Manager 3.0 is available for ArcGIS 9.3 through ArcGIS 10.6.

## Using Esri Metadata Stylesheets

Theme Manager comes with two stylesheets to convert XML metadata into an HTML file suitable for display.
Esri provides a few more comprehensive stylesheets that Theme Manager users may want to use.
Unfortunately, the Esri stylesheets require processing by an Esri library (dll) that is not part of the developer's SDK,
so it is not available for general access by Theme Manager.

If you want to use the Esri stylesheets with Theme Manager, you must have ArcGIS Desktop or ArcGIS Pro
installed on your computer. Then follow these steps:

1. Copy the dll to Theme Manager

  * **Desktop**: Copy the file `C:\Program Files (x86)\ArcGIS\Desktop10.5\bin\ESRI.ArcGIS.MetadataEditor.dll` (or similar) to the location where Theme Manager is installed. The dll must be in the same folder as ThemeManager.exe.

  * **Pro**: Copy `C:\Program Files\ArcGIS\Pro\bin\Extensions\Metadata\ArcGIS.Desktop.Metadata.dll` to the location where Theme Manager is installed. The dll must be in the same folder as ThemeManager.exe. **Note**: You will need a special build of Theme Manager specifically for Pro.

2. Create a folder called `Esri` in Theme Manager's stylesheet folder. The stylesheet folder
   is typically called `StyleSheets` in the folder where Theme Manager is installed. However, it
   can be given a different name by editing the `StyleSheetDirectory` setting in `ThemeManager.exe.config`

3. Copy the Esri Stylesheets to Theme Manager

  * **Desktop**: Copy the files `ArcGIS.xsl` and/or `ArcGIS_ItemDescription.xsl` from the folder
   `C:\Program Files (x86)\ArcGIS\Desktop10.5\Metadata\Stylesheets` (or similar) to Theme Manager's
   `StyleSheets\Esri` folder. The files can be renamed to something more intuitive for display
   in the stylesheet picker in Theme Manager, but the files must retain the `.xsl` extension.
   You also need to copy the folder `C:\Program Files (x86)\ArcGIS\Desktop10.5\Metadata\Stylesheets\ArcGIS_Imports` (or similar, and all it contents) to `StyleSheets\Esri\ArcGIS_Imports`. These files are referenced by the main files, and cannot be renamed or put in a different sub folder.

   * **Pro**: Copy the files `ArcGISPro.xsl` and/or `ArcGISProFull.xsl` from the folder
   `C:\Program Files\ArcGIS\Pro\Resources\Metadata\Stylesheets` to Theme Manager's
   `StyleSheets\Esri` folder. The files can be renamed to something more intuitive for display
   in the stylesheet picker in Theme Manager, but the files must retain the `.xsl` extension.
   You also need to copy the folder `C:\Program Files\ArcGIS\Pro\Resources\Metadata\Stylesheets\sArcGIS_Imports` (and all it contents) to `StyleSheets\Esri\ArcGIS_Imports`. These files are referenced by the main files, and cannot be renamed or put in a different sub folder.

   * You can use Pro stylesheets with Desktop and visa-versa. Fortunately, the names of the files in `ArcGIS_Imports` have different names for the different versions, except `ArcGIS_Imports\XML.xslt` which is the same in both versions.

## Using Theme Manager on a Network

A single install of Theme Manager can be used by multiple users if the application is unzipped to a shared folder on a network drive. If the domain administrators have not designated the server or shared folder as a trusted location, you will be prompted to accept a security warning when you launch the application. In some cases you may be denied the ability to run the application from the network. This is a site specific domain security issue beyond the programmer's control. If it is a problem at your site, contact your domain administrator, or copy the application to a local (trusted) location and execute it from there. If you have administrative control on your computer there are also ways to run off this security warning, but that is beyond the scope of this document – consult the Google.

## Accessing Theme Manager from ArcMap

Theme Manager is a stand-alone windows application. It is not an ArcGIS extension. It is typically launched via a shortcut on your desktop.

To launch the application from ArcMap requires installing an ArcMap Add-In, installing Theme Manager in a well known location, and putting that location into the code for the Add-In. An Add-In may be available for your site (contact your GIS Specialist, or the programmer), if not an Add-In can be created using the Esri tutorials. Put the following code in the `OnClick()` method of the tool you create.

``` c#
    //Change the following path to your installation location
    string directory = "X:\Path\To\ThemeManager\Folder";
    System.Diagnostics.ProcessStartInfo startInfo =
        newSystem.Diagnostics.ProcessStartInfo(directory + "\ThemeManager.exe");
    startInfo.WorkingDirectory = directory;
    Process.Start(startInfo);
```

[^1]: Provided you have write permission in that location – However, you do not need write permission to the installation location in order to run Theme Manager.

# What's New

This section assumes you have used previous versions of Theme Manager. If you are a new user you can ignore it.

## Theme Manager 3.1

- Now uses real [W3C specification for XSLT version 1.0](https://www.w3.org/TR/xslt-10) stylesheets.
- Removes dependency on vulnerable MSXML2.0 and the obscure MS draft XSL transformation.
- Can use Esri ArcMap 10.0+, or ArcGIS Pro metadata stylesheets (if ArcGIS is installed).
- Attributes can be extracted from ISO (19139) metadata.
- HTML tags are stripped from extracted metadata attributes when adding to Theme node.
- Can extract attributes, search and display Metadata for ArcGIS services (Map, Image, and Feature).
- Better logic for finding metadata for data sources.
- Removed option `KeepMetaDataInMemory` (this was a half baked idea, and never fully implemented).
- Metadata is always guaranteed fresh, no caching.
- Reading and saving Author data on the Themelist is supported.
- Cleaner formatting of display when metadata is missing or has errors.
- Improvements to reloading, syncing and saving theme lists.
- Resize text boxes on theme properties form to show more metadata details.


## Theme Manager 3.0
Theme Manager 3.0 is a complete re-write of the previous versions of Theme Manager. As a result, there is a completely new look and feel to the application. Most of the core functionality is the same but it may be accessed in a slightly different manner. This section will not provide a complete cross walk of all the new ways to access previous functionality. Chances are that you have already discovered most of what you need to know from experimenting. Only the major differences are listed below.

- Theme Manager 2.2 and earlier used a Microsoft Access database to store the theme list. Theme Manager 3.0 saves theme lists in an XML formatted text file. Theme Manager 3.0 can read theme list from previous versions, but it cannot modify those theme lists, or save new theme lists in the Microsoft Access format. This means that if you have a mixed environment, you will need to edit your theme lists in a previous version of Theme Manager until all your users can upgrade. We encourage you to upgrade as soon as possible in order to take advantage of several of the new enhancements.
- In order to improve the speed of searching, several of the key metadata fields can be harvested from the metadata and stored in the theme list file. There is a Synchronize Metadata button available on the properties panel to do this. Theme Manager does not keep track of changes to your metadata, so you will need to synchronize when the metadata changes. You can only synchronize if you have write permissions to the theme list, and the theme list is not stored in a Microsoft Access database.
- New XML based theme list format (faster loading, searching and saving)
- All Theme properties are managed on the properties tab in the main form.
- Quick search from the toolbar.
- Search a subset of themes.
- Most operations (i.e. copy/paste, add to favorites, add to ArcMap, etc.) will work with multiple themes. Use ctrl-click and/or shift-click to select multiple themes.
- Drag and drop multiple themes to add them to ArcMap.
- Drag and drop multiple layer files to add them to a theme list.
- Double clicking on a theme will open that theme (typically in ArcMap).
- A theme can be any document (but only Esri layer files (\*.lyr) and Map documents (\*.mxd, \*.mxt) provide sub-theme expansion and automatic metadata discovery.
- ArcMap document when added as themes will reveal all their layers in the tree view
- Themes can be reloaded if the source document has changed.
- Layers/Maps can have group layers nested arbitrarily deep.
- Themes can be sort ascending, descending, or unsorted (by turning on an advanced option)
- Various administrative reports are available (through an advanced option). These reports provide diagnostic lists on the state of a selected theme list.
- Themes can be previewed with an embedded map viewer.
- Theme lists can be edited with cut/copy/paste and well as drag and drop.
- Favorites can be organized into categories.
- Any category (including those in search results or favorites) can be saved as a new theme list.
- Changes to a theme list are not permanent until you explicitly save. Changes can be discarded by quitting without saving changes.

# Using Theme Manager

## Browsing

Theme Manager is used like a windows file browser. You can open and close folders. Folders can contain themes or more nested folders. Themes are typically ArcMap documents like layer files or map documents. Double clicking on a theme will perform the open action for that document type (same as double clicking the document in windows explorer. For example if the theme is a layer file (typical) it will be added to the current open Arc Map document (a new one will be opened if there is no current document). If the theme is a map document a new session of ArcMap will be started with the new map document.

To assist in browsing for new items, some items will have a red **N** added to their icon. These are items that have been published with a user specified time period. Theme lists and categories are also flagged as new if items somewhere below them are new. You can change the specified time period with the drop down list on the toolbar.

### Theme Lists

Themes and categories are organized and saved in theme lists. Theme lists (and their contents) are added to the Themes tab when you use the open theme list command. Theme Manager has hopefully opened with a theme list that you can start using right away. Theme Manager will use any theme lists you were using with the previous version, or an administrator can set the application configuration file to load a default theme lists. Some theme lists may be read only (all theme lists from previous versions of Theme Manager), and those that are marked as read only, or stored in a location to which you do not have permission to write. Read only theme lists can be identified with a lock icon on the Theme list icon.

### Themes Tab

The themes tab shows all the theme lists that you currently have open. Theme lists can be added with the open command, or the New Theme List command. Deleting a theme list from this tab will only remove it from the list of open theme lists it will not delete the actual theme list file. When you quit, Theme Manager will remember which theme lists you had open and restore them when you restart Theme Manager

### Favorites Tab

Themes and categories of themes can be added to the favorites tab. New categories can be created in the favorites tab, and existing items in the favorites tab can be added to the new category by using copy and paste or drag and drop. Items can be removed from the favorites with the delete command. Categories in the favorites tab can be searched with the right click search command, and only themes in that category will be searched. All themes and categories in the favorites tab will be automatically remembered between sessions of Theme Manager. However, if you want to save a category in the favorites tab as a theme list (and thereby added to the themes tab) you can use the Save As command for that.

### Search Results Tab

The search results tab creates a new category for each collection of themes that are found when you request a search. Search Results can be organized, deleted, searched and saved the same as the favorites tab.

### Metadata Tab

Each theme list, category, theme and sub-theme can have associated metadata. Metadata is either a URL to a web page, a file path to an XML document, or an ArcCatalog path to a data source in a file geodatabase that has embedded metadata for that data source. If the associated metadata can be found and is a valid file, then it will be displayed in the metadata tab. The format of XML based metadata can be changed by picking a different style sheet from the pick list on the toolbar. The style sheets are included in a subfolder with the application. Style sheets in this folder can be removed, edited or added to in order to change the choices or display of the metadata. The style sheets provided are based on FGDC and Esri's 9.3 metadata tags. Some of the tags used in the Esri's version 10 metadata are not recognized by the style sheets.

URL and file based metadata do not require an ArcGIS license to display, but metadata in a geodatabase requires an ArcGIS license.

### Preview Panel

Themes based on Esri files (\*.lyr and \*.mxd) can be displayed in the preview panel with the same symbology as they would in ArcMap. Simple navigation tools (zoom, pan) are available in the toolbar. You can also use the mouse to zoom and pan. The preview panel requires access to the ArcGIS libraries.

### Properties Panel

The attributes of the item (theme list, category, theme or sub theme) currently selected in the active tree view are displayed in the properties tab. The properties tab provides a means of editing these items. For properties that contain file names/paths, a browse button is available to find the file, or the text field will accept a file dragged/dropped from Windows file explorer. Some properties cannot be edited because they are controlled by Theme Manager, or because the item is in a read only theme list.

Most properties for themes and sub-themes are populated automatically when the theme is added to Theme Manager; however, these properties can become out of date if the theme's layer file changes. The property panel provides a **reload** button that will reread a theme's layer file and update the data source, data type, and any sub-themes. The property panels for theme lists and categories also provide a reload button which will reload all the themes contained below them.

Several of the properties for a theme can be harvested from the metadata (if found). However, the properties can be edited independent of the metadata if you wish. If you wish to populate the fields based on the metadata, you will need to click the **sync** button.

## Adding Themes to ArcMap

Themes (based on \*.mxd or \*.lyr files) can be added to ArcMap in several ways.

1. Drag and drop the selected themes onto the ArcMap table of contents or map canvas.
2. Double click the theme (or ctrl click multiple themes).
3. Select one or more themes, and then click **Open Theme(s)** from the **Tools** menu.

If the theme is an ArcMap document, then a new copy of ArcMap will be opened, leaving any existing map documents as they were. If the theme is a layer file, and ArcMap is not open, then ArcMap will be launched, and the layer file will be added to a new empty map document. . If the theme is a layer file, and there are one or more sessions of ArcMap running, then theme will be added to the most recently active ArcMap document.

## Searching

There are four ways to search:

1. Enter text in the search text box on the toolbar and press the enter key
2. Press the Search button on the toolbar.
3. Click Find in the Edit Menu (also available with the keyboard shortcuts Alt, E, F, or Ctrl-F)
4. Right click in one of the trees of Themes, Favorite, or Search Results

Option 1 is known as the quick search, and it should return results quickly. It will search all theme lists (but not favorites or previous search results) for all themes that match the search request and all themes that have a sub-theme that matches the request. A theme (or sub-theme) matches if all the unique words[^2] in the search box can be found in the combination of the text in the following four properties: Theme, Description, Summary, or Tags. It will also return themes which have sub-themes that match the search request.

Option 2 is the advanced search; it presents the search form to allow you the greatest degree of control on how the search will be conducted. The Advanced search will search all theme lists. The default search text will be any text that was in the search box on the toolbar, but that can be changed on the search form. If you press the search button, the default behavior will be the same as the quick search.

The search form allows you to search for themes based on publication date, or to not search some of the properties such as description. The search form also allows you to search the metadata directly. This can be very time consuming, as each metadata file will need to be opened and searched in turn, but it is the only way to search for text in the metadata that is not harvested into one of the theme properties.

The search form also allows you to combine these various search methods together so that a match only occurs if all the conditions are met (AND), or if any one of the conditions are met (OR).

The best way to learn the search form is to experiment with it, searching well known themes to see if you get the results you are expecting.

Option 3 invokes the same behavior as option 2

Option 4 is known as the selective search. It is will only search a portion of the available themes. If the right click happens on an item in one of the three trees, then only that item, and all items below it will be searched, using the advanced search, for a match. See the Option 2 for more information on the advanced search. If the right click occurs below the last theme, that is the right click was not on any item in the tree, then all items in that tree (i.e. all theme lists if in the Themes tab, or all favorites if in the favorites tab, or all prior search results if in the **Search Results** tab) will be searched.

[^2]: The text in the search box is broken into words at spaces and tabs, but not punctuation like commas or hyphens. Search will not be able to find a **word** with a space in it, but it can find a word with punctuation in it.

## Favorites

Any theme, category or group of themes and/or categories can be added to the favorites tab. The selected themes/categories are added by clicking the star icon on the toolbar, or clicking Add to Favorites in the Tools menu, or by right clicking on a theme and selecting Add to Favorites in the pop-up context menu. See the discussion in Favorites Tab above for more information on managing your favorites.

# Managing Theme Lists

## Creating Theme Lists

A new empty theme list can be created by selecting New Theme List from the File menu. It can also be created by selecting the new document icon on the toolbar, and then selecting the New Theme List command in the pop-up menu. In both cases, an empty theme list called New Theme List will be added to the theme tab. The themes tab will be made active, and the New Theme List will be selected. The name and other properties of the new theme list can be edited in the properties panel when the theme list is selected. The file name can only be specified by selecting the Save As command. The Save As command will be automatically invoked by the save command if the theme list has been modified. You will be prompted to save when you quit Theme Manager.

You can also create a new theme list from an existing theme list, or an existing category of themes by selecting the item, and then clicking the File then Save As in the main menu, or by right clicking on the item and selecting Save As on the pop-up context menu.

## Adding Themes

Themes can be added to a theme list with the File ⭢ Add Theme command in the main menu, or by selecting the new document icon in the toolbar, and then select New Theme. You can only create a new theme if you have a theme list or category selected, in the themes tab, and If the theme list is not read only. The New Theme command will create an empty theme with the name **New Theme** as a child of the selected item. The name and other properties of the theme can be edited by selecting the new theme and then making sure the Properties tab is active. Usually the only properties that need to be edited are the Theme name, and the File. Use the browse (…) button to select an Esri layer file (\*.lyr), and the rest of the properties will be automatically populated.

If you have a layer file already selected in the windows file explorer, then you can drag the file and drop it on the file text box in the theme's property tab.

You can also select one or more files (of any type) in the windows file explorer, and then drag and drop them onto a theme or category in the themes tab, and a new theme will be created for each of the files that were selected.

## Organizing Themes

Categories (folders for organizing themes) can be created in any of all of the tabs (Themes, Favorites, and Search Results). Create a category with the File ⭢ Add Category command in the main menu, or by selecting the new document icon in the toolbar, and then select New Category. You can only create a new category in the themes tab if you have a theme list or category selected, and the theme list is not read only. You cannot create a category in any of the tabs if you have a theme or sub-theme selected.

The name, description, and URL/Metadata for a category can be edited by selecting it and then making sure the Properties tab is active.

Themes and Categories can be moved/copied from one location to another with cut/copy/paste, and with drag and drop. With dragging and dropping, move is the default action, pressing the control key will dragging will change the action to a copy. You cannot move a theme/category in a read only theme list, but you can copy.

Drag and drop as well as cut/copy/paste will work with multiple items selected. In most cases it doesn't make sense to perform these operations on a sub-theme or a theme list.

All items except sub-themes and items in a read only theme lists can be deleted with the delete command, or by pressing the delete key.

## Editing Properties

See the discussion in Properties Panel above.

## Reloading Themes

See the discussion in Properties Panel above.

## Synchronizing Themes with Metadata

See the discussion in Properties Panel above. The properties match the metadata elements as follows:

- **Publication Date**: the XML element `/metadata/idinfo/citation/citeinfo/pubdate` used by FGDC or `/metadata/dataIdInfo/idCitation/date/pubdate` used by ArcGIS version 10+
- **Description**: the XML element `/metadata/idinfo/descript/abstract` used by FGDC and ArcGIS
- **Summary**: the XML element `/metadata/idinfo/descript/purpose` used by FGDC and ArcGIS
- **Tags**: the FGDC XML elements `/metadata/idinfo/keywords/*/*key` (where \* = theme, place, strat, temp) or the ArcGIS version 10 XML elements `/metadata/dataIdInfo/*/Keys/keyword` (where \* = desc, other, place, temp, disc, strat, search, theme).

## Saving

There are two ways to save changes to your theme list.

1. Save

This command is available from the main menu (File ⭢ Save), or the toolbar. This will save modifications to you have made to your theme lists to disk. If there are no changes, this command is not available. If you do not save, then all changes will be lost when you quit Theme Manager, however you will be prompted first. Read-only theme lists cannot be modified, so they will not be saved. There is no option to save changes in some theme list, but discard changes in other theme lists (this only applies if you have changes to more than one theme list). If a theme has been created in this session and does not have a file name associated with it, the save command will invoke the Save As… command on that theme list. There is also no provision in this command to save the changes under a new file. See the following command.

1. Save As…

This command is not available on the tool bar. You must select it from the main file menu, or the right click menu of a theme list or category. This command will save the currently selected item (theme list or category only) as a new theme list. You will be prompted for a filename, and then the item will be added to the theme lists tree. This is the simplest method of creating a copy of a read-only theme list. This command will only work on a single selected item. If multiple items are selected when this command is invoked, then last selected item will be used.

## Administrative Reports

Administrative reports are useful for generating a list of the themes in your theme list, or for checking the status of all themes in a theme list. To access the administrative reports, first enable the menu item by opening the options panel (Tools ⭢ Options…), then click the Advanced Options button. Next click the Menus tab, and put a check box in the option to **Show administrative tools in main menu**. Once the reports are enabled, they can be invoked from the Tools menu with the Administrative Reports… command.

To use the reporting tool, first select the theme list that you check, then select the report you wish to receive, then click the Go button. Some reports may take a while to complete, so they can be canceled after the Go button changes to a Stop button.

A description for each report is shown in the status bar. Several of the reports are not functional with this release, but may be available with a soon.

The report **List missing themes** will check all themes, and list those themes that it cannot find the file that they point to. This is helpful for finding themes that are pointing to layer files that have been moved or deleted from the file system.

The report **List (sub)theme by workspace/dataset** will list all the workspaces and data sets of all the data sources in all the layer files in the selected theme list. This is helpful for determining if any themes are missing their data sources, or if the layer files or data sources are corrupt. This report does not read all the layer files (which is a time consuming operation). It assumes that that you have loaded (or reloaded) all the themes with Theme Manager 3.0. Theme lists from version 2.2 or earlier will not have the information to make this report useful. Theme Manager will store this information until the theme is deleted or reloaded. The reload command should be run on themes when they change, or periodically on the entire theme list.

# Advanced Topics

## Startup

During startup, Theme Manager goes through the following steps to load your session information, i.e. the set of theme lists you want to use.

1. If you have a session stored from a previous execution of the current version of the program, then that is loaded. This is the typical situation. The location of the session file is discussed in Session information above.
2. If this is the first execution of a new version, than the previous check will fail, so Theme Manager looks for a session file from a previous version, and attempt to read it.
3. If no session file is found, Theme Manager will check the windows registry for settings left behind from version 2.2 or earlier of Theme Manager. The registry may have paths to theme lists and favorites.
4. If no registry information is available, then the application setting of `DefaultThemeLists` (a list of semicolon separated file path strings) is checked. All theme lists listed here are loaded

After any previously known theme lists have been loaded, the application settings of `ObsoleteThemeLists` and `RequiredThemeLists` (lists of semicolon separated file path strings) are checked and corresponding theme lists are removed or added to your session. The removing of obsolete and adding of required theme lists is check on each start up, and is used to encourage organizational consistency. Users can override this behavior in the advanced preferences panel.

The application settings of `DefaultThemeLists` and `RequiredThemeLists` seem redundant, however they are both required for the situation where an organization does not want to require standard theme lists, but does want to provide default theme lists to first time users.

## Configuration Files

Theme Manager follows the new Microsoft recommended configuration management strategy (it is the default behavior for .Net applications even though Microsoft and others do not always follow it). Settings are stored in XML files, so they can be viewed in Internet explorer, or viewed/edited in any text or XML editor. Theme Manager 3.0 does not use the registry except to check for information left behind by Theme Manager v2.2 or earlier. The folder structure of organization/application/version, which you will see below, should be typical for newer applications.

There are basically three configuration files:

1. Application defaults
2. User Preferences
3. Session information


### Application Defaults

This is the primary configuration file. It is named `{application}.config` and it must be adjacent to the `.exe` in order to be found. This file is read only (since it is on the X: drive) unless you make a local copy of the Theme Manager application folder. It contains 1) default user preferences, and 2) application settings which are either pieces of information that I don't want to embed in the code, like directory or registry paths, or settings that we might want to change without recompiling the program, i.e. the path to the application icon, or the help file.

The application configuration file can be found at:

``` bash
    X:\GIS\ThemeMgrApp\Ver10.x\ThemeManager.exe.config
```

The settings available in that file are subject to change until the development stabilizes.

### User Preferences

On XP there is only one active configuration file, while on Windows7 there are two. User settings are defined as either local or roaming. The local settings are machine specific settings, typically things like window size/location, which may vary based on the size and resolution of the current display. The roaming settings are user specific settings which, in the future, may automatically travel from machine to machine with the user. Unfortunately, there is nothing in the application defaults file to distinguish between roaming and local user settings, however the application knows the difference, and the system will save them in different files.

These files are created when the user changes a default setting by, for example, clicking a check box in the preferences panel or moving the window. Only settings that are modified from the default value in the application configuration file are saved in the user configuration file. Therefore you may not see all available settings by looking at a user's configuration file.

The user configuration files can be found in the following locations:

* XP: `C:\Documents and Settings\{USERNAME}\LocalSettings\ApplicationData\National_Park_Service\ThemeManager.exe{###}\3.0.0.0\user.config`

* Windows 7+ (local): `C:\Users\{USERNAME}\AppData\Local\National_Park_Service\ThemeManager.exe{###}\3.0.0.0\user.config`

* Windows 7+ (roaming): `C:\Users\{USERNAME}\AppData\Roaming\National_Park_Service\ThemeManager.exe{###}\3.0.0.0\user.config`

Where `{USERNAME}` is the user's domain login name, and `{###}` is usually a lot of gobbledygook. The `{###}` gobbledygook is created by the system, not me. Windows seems to create a new folder if the exe changes or the path to the exe changes (i.e. running a local copy, or accessing the application via a UNC vs. drive letter). Only the most recent (which can be determined by looking at the modification date on the file not the folder) is the active configuration file.

The version string **3.0.0.0** will likely change as the versions move forward.

I am not sure how settings migrate when the exe, path or version changes, but it looks like the user starts over with the application defaults each time. I may need to mitigate this in a future version.

### Session Information

In addition to the settings discussed above, each application has it's own folder in user's subdirectories. This folder is for saving additional application state that does not fit well into the automated settings system. Theme Manager uses this folder to store information about the user's session, i.e. the collection of the theme lists they are using, their favorites, and search results. This file can be found in the following location:

* XP: `C:\Documents and Settings\{USERNAME}\ApplicationData\National Park Service\ThemeManager\3.0.0.0\session.xml`

* Windows7: `C:\Users\{USERNAME}\AppData\Roaming\National Park Service\ThemeManager.exe\3.0.0.0\session.xml`

This file is automatically saved only when Theme Manager exits normally. It is not saved if Theme Manager crashes, or you kill it. If this file does not exist, Theme Manager tries to find a previous version (not applicable until there are versions beyond 3.0.0.0). If no previous version is found, Theme Manager searches the registry for information save by a Theme Manager 2.2 or earlier. If nothing is found in the registry, a list of default theme lists are loaded. This behavior will change before the final release to ensure that the default theme list is always loaded.

If Theme Manager runs into problems when starting up, the session may not be properly restored, however, when you quit the program, this incomplete session will be saved, overwriting your previous session. This can result in a saved session with no theme lists, obviously confusing the casual user. If this happens, the session file should be deleted to allow Theme Manager to recreate it by searching the registry. This problem is partially corrected in versions after 10/20/2010 which will not overwrite your session file if you have no theme lists attached.

I may implement automatically saving backups of this file in a future version.

### Contents of ThemeManager.exe.config

The following is a list of the settings and valid values found in the configuration file. Users and/or administrators can edit this file to change the default behavior of Theme Manager.

| Application Settings | Description | Valid Values |
| --- | --- | --- |
| StylesheetSubDir | No longer used |  |
| RegistryArcDir | No longer used |  |
| RegistryArc8Dir | No longer used |  |
| ArcDir | No longer used |  |
| Arc64Dir | No longer used |  |
| RegistryArcKey | No longer used |  |
| RegistryUserDatabases | Registry key with of paths to Theme Manager2.2 databases | Registry path |
| RegistryUserFavorites | Registry path to stored codes for favorites (v2.2) | Registry path |
| RegistryMachineDatabases | Registry key with of paths to Theme Manager1.0 databases | Registry path |
| RegistryMachineFavorites | Registry path to stored codes for favorites (v1.0) | Registry path |
| SavedSessionFile | Name of the file to state of Theme Manager between sessions. | valid file name |
| AppName | Changes the title bar in the main form | Any text |
| AppIcon | Changes the icon that shows in each form, and the taskbar, but not the application icon that shows in windows file viewer. | Relative path to \*.ico file from executable |
| DefaultDatabases | Theme list to load for a first time user that has no prior theme lists | Semi colon separated list of full path names of theme lists |
| HTMLNoInitialData | Web page to shown in the metadata tab if the user has no theme lists loaded. | Relative path to html file from exe |
| HTMLNoMetadata | Web page to show in the metadata panel if select item has no metadata. | Relative path to html file from exe |
| DefaultHtml | web page to show in metadata panel at startup and when no item is selected. | Relative path to html file from exe |
| MinimumVisibleSplitterWidth | Width of metadata/preview tab below which we do not bother refreshing the content. | integer |
| StyleSheetDirectory | Location of metadata stylesheets | Relative path to folder containing stylesheets |
| ObsoleteThemeLists | List of theme lists user may have opened previously that the administration considers obsolete, and should be unload and not used. | Semi colon separated list of full path names of theme lists |
| RequiredThemeLists | List of theme lists that administration would like every user to have loaded. | Semi colon separated list of full path names of theme lists |
| HelpUrl | Web page to show in the metadata panel when the user selects help. | Relative path to html file from exe |

| User Settings | Description | Valid Values |
| --- | --- | --- |
| mainFormSize | Size of main windows when quitting | Width and height in pixels (x,y) |
| mainFormLocation | Location of main window on the screen | window coordinates in pixels (x,y) |
| mainFormState | Main window minimized/maximized status | Normal, Minimized, Maximized |
| mainFormTabpage1 | Select themes tab | 0=Themes, 1=Favorites, 2=SearchResults |
| mainFormSplitter | Width of left side of main form in pixels | Integer for 0 to width of window |
| StyleSheetIndex | Selected style sheet | 0 to number of style sheets in style sheets folder |
| StayOnTop | The user wants Theme Manager to stay above all other windows | true/false |
| AgeInDays | Days corresponding to selection in the AgeComboBox | Positive integer |
| KeepSearchFormOpen | See Advanced Options | true/false |
| DisplayDefaultHtml | User prefers a blank metadata panel to the default html page | true/false |
| AgeComboIndex | Current pick list choice | 0 (1 Day) to 10 (5 Years) |
| InPlaceNodeEditing | See Advanced Options | true/false |
| CheckForArcViewBeforeArcInfo | See Advanced Options | true/false |
| mainFormTabpage2 | Selected info tab | 0=Metadata, 1=Preview, 2=Properties |
| Toolstrip1Location | Location of toolbar1 (mostly icons) | window coordinates in pixels (x,y) |
| Toolstrip2Location | Location of toolbar2 (pick lists and search box) | window coordinates in pixels (x,y) |
| Toolstrip1ParentName | Location of toolbar1 | TopToolStripPanel, BottomToolStripPanel, RightToolStripPanel, LeftToolStripPanel |
| Toolstrip2ParentName | Location of toolbar2 | TopToolStripPanel, BottomToolStripPanel, RightToolStripPanel, LeftToolStripPanel |
| RightClickSelectsNode | See Advanced Options | true/false |
| DontSelectFirstNode | See Advanced Options | true/false |
| ClickToClearNodeSelection | See Advanced Options | true/false |
| FocusTreeviewOnTabChange | See Advanced Options | true/false |
| DontShowBetaWarning | Turns on a warning for testers and early adopters | true/false |
| ShowHiddenThemes | Not used | true/false |
| LoadRequiredThemeLists | See Advanced Options | true/false |
| RemoveObsoleteThemeLists | See Advanced Options | true/false |
| ShowMainMenu | See Advanced Options | true/false |
| ShowToolbar1 | See Advanced Options | true/false |
| ShowToolbar2 | See Advanced Options | true/false |
| DefaultSortOrder | See Advanced Options | -1 = Descending, 0 = Unsorted, 1 = Ascending |
| AllowUnsortedOrder | See Advanced Options | true/false |
| ShowAdministrativeTools | See Advanced Options | true/false |
| ShowThemeDescriptionToolTip | See Advanced Options | true/false |

## Multi-select

The standard windows tree view does not allow multi-selection. The correct and expected behavior is therefore not well defined. This topic describes the multi-select behavior as implemented by Theme Manager. Some Theme Manager actions will only operate on a single selected item, while others will operate on a collection of selected items. The last (or only) selected item (identified with a slightly different color from other selected items) is the item used when only a single item is allowed. Selecting an item has the effect of selecting all the sub-items. It is not possible to select an item and one of its sub-items; If a sub-item is selected, then the parent item is unselected, conversely if a ancestor (parent item) is selected, then the original item (which is now a sub item) is unselected. In addition, you cannot select sub-themes, they are a display only property of a theme.

| Action | Behavior |
| --- | --- |
| Click on a item | Clears all previously selected items, adds new item to selected list |
| Click not on a item | Clears all previously selected items (or no-op) - user pref. |
| Ctrl-click on a item | Add (if not already selected or removes (if already selected) item from selected list |
| Ctrl-click not on a item | no-op (adds or removes nothing from the selected list) |
| Shift-click on a item | if sibling item is selected, select all to closest sibling, if no sibling is selected Add or remove from list |
| Shift-click not on a item | no-op (adds or removes nothing from the selected list) |
| [^3]-Click on a item | same as click on a item |
| [^3]-Click not on a item | same as click not on a item |
| Right-Click[^4] on a item | Has behavior of single click (with modifiers) or no-op (user preference), then opens the context menu for item. |
| Right-Click[^4] not on a item | open context menu for tree |
| Double Click[^4] on a item | Has behavior of single click (with modifiers), then launches the selected set of items. |
| Double Click[^4] not on a item | no-op - do not change selection, do not launch any items |

[^3]: Any other modifier keys (i.e. Alt), or any combination of modifier keys (i.e. Ctrl-Shift)

[^4]: With any combination of modifier keys (i.e. Ctrl, Shift, Alt)

## Advanced Options

The advanced options are available by clicking the advanced options button on the options panel. Most users can safely ignore these options, and they are here for those that like to customize their environment, and are willing to deal with the inconvenience of their mistakes.

### General Options

| Setting | Default | Description |
| --- | --- | --- |
| Keep Theme Manager on top of other windows | No | With yes, Theme Manager application will float above all other windows on the screen |
| Display introduction when no theme is selected | Yes | With yes, the quick start tutorial (or whatever is in starup.html) will display in the metadata panel at startup and whenever a nothing is selected. |
| Check for ArcView before ArcInfo License | No | It is faster to check for the license you know you have first. With yes the ArcView is checked first. Having an ArcInfo license does not imply you have a ArcView license. |
| Keep search form open after results are displayed | No | With no, the search form will close after results are displayed. With yes, the search form will stay open so similar searches can be done quickly. |

### Tree View

| Setting | Default | Description |
| --- | --- | --- |
| Enable theme/category name editing in tree view | No | With yes, the name of items can be edited directly in the tree view, in addition to the properties panel. It can be annoying to enter edit mode when you only wanted to select the item. |
| Tab selection focuses tree view | Yes | With yes, clicking on one of the three tabs with the themes trees in them will put keyboard focus on the tree. No is default windows behavior. |
| Click in tree view clears selection | Yes | With yes, if you click on a blank spot in the tree view, all nodes will be unselected. With no, the selection will not change. No is default windows behavior |
| Don't select the first node if no node is selected | Yes | With No, if no node is selected in the tree view, then the first node in the tree is selected. No is default windows behavior. |
| Right click selects node | Yes | With no, right click will invoke the context menu, but the selected node will not change. No is default windows behavior. |
| Show theme description as a pop-up tool tip | No | With no, the theme name will be the pop-up tool tip if the entire name cannot be displayed. |

### Menus

| Setting | Default | Description |
| --- | --- | --- |
| Show main menu | Yes | Turn the main menu on/off |
| Show main toolbar | Yes | Turn this toolbar on/off |
| Show pick list and search toolbar | Yes | Turn this toolbar on/off |
| Show status bar | No | Adds the status bar at the bottom of the window. Currently the status bar is not used for anything. |
| Show administrative tools in main menu | No | Adds a menu option in the Tools menu |

### Other

| Setting | Default | Description |
| --- | --- | --- |
| Load Required theme lists | Yes | With no user will ignore administrative request to add specified theme lists at startup. |
| Remove obsolete theme lists | Yes | With no user will ignore administrative request to remove obsolete theme lists at startup. |
| Allow Unsorted Order | No | With no, sort button toggles between Ascending and Descending. With yes, sort button toggles between Ascending and No Sort and Descending |
| Default Sort Order | Ascending | Sort order to impose at start up |

# Known Issues

- If you run Theme Manager on a computer without ArcGIS or with a different version of ArcGIS than it was built for, you will get a confusing _Unhandled Exception_ error dialog whenever you try to do an operation requiring ArcGIS (preview a theme, reading or rendering metadata in a geodatabase, adding or reloading a theme based on a layer file or map document). The error dialog should clearly state the problem or be replaced with a clear explanation in the preview or metadata tabs. **Note** previewing metadata does not show the error dialog, but the explanation in the display should be clearer.
- If Theme Manager is unable to obtain an ArcGIS license, even temporarily, some actions will fail with a confusing and misleading error dialog. Workaround is reconnect to a license manager and restart Theme Manager.
- You cannot paste an item to the root of favorites. Workaround: use **Add to Favorites**
- If the user adjust the system font size, labels and text boxes do not adjust their position accordingly and they may overlap.
- Toolbar icons are fuzzy on high resolution displays.
- Preview display does not update if theme properties (i.e. path to layer file) are changed. Work around: refresh the preview by clicking on another theme, then click back.
- Metadata display does not update if theme properties (i.e. path to metadata) are changed. Work around: refresh the metadata by clicking on another theme, then click back.
- Sometimes when a theme or category is deleted a new Theme or Category is not selected. In this case, the metadata/preview/properties panel is not updated. Work around: refresh the panel by clicking on another theme or category.
- If the [session files](#session-information) are moved, or incorrectly altered, Theme Manager may not behave properly until you quit and save new session information. If you lost session information (i.e. favorites), you can recreate it or manually edit the session configuration files with a text editor.
- Setting some advanced options causes the toolbars to disappear. Workaround: quit and restart Theme Manager, or turn the toolbars off, then on again in the advanced options.
- Occasional cross threading and null reference errors occur when reloading themes or syncing metadata. This was introduced when reloading and syncing were done in the background to provide a progress bar and cancel button. The problem is rare, sporadic, and a challenge to fix. In the meantime it can usually be cleared by trying again, or quitting/restarting Theme Manager.

# Ideas for Future Versions

The following is a brainstorming list and not a planning document. Some of the ideas are so easy they may get slipped into a patch before the next major version, whereas some are so hard or of limited use that they may never get implemented. If you have any other ideas please [contact us](#contact-information).

- Save/Restore a list of categories to hide (general solution needs shortcuts)
- Load a default list of categories to hide based on organization (park) affiliation.
- Store favorites and search results as **shortcuts**, not clones
- Search results to show pointer back to source (need **shortcuts**)
- Additional admin reports
- When browsing for theme metadata, use the Esri ArcCatalog browser and not the windows file browser. This will allow finding feature classes (and the associated metadata) as well as xml file based metadata.
- Check out license in background at startup.
- Save/restore the xml theme list (\*.tml) in a compressed format (\*.tmz)
- Allow theme/category drop on favorites tab (== add to favorites)
- Try to get data source modification date for geodatabase sources
- New Logo
- Undo/Redo when editing theme lists
- Ability to filter arbitrary portions of a read only theme list
- Improve searching (speed, flexibility and simplicity, more google like)
- Allow a theme list to use relative paths to themes/metadata
- Allow drag and drop in between siblings – for item reordering
- Use fly-out and dock-able windows (instead of tab panels)
- Use floating, dock-able, closeable toolbars
- Preferences for font color choices for different item types.
- Drop a theme list onto Themes tab to add a theme list
- Save the state (which items are expanded/closed) of the trees between sessions.
- Expand WMS Group Layers in tree view

# Contact Information

Alaska Region GIS Team  
National Park Service  
240 West 5th Avenue  
Anchorage Alaska 99501  
[akro_gis_helpdesk@nps.gov](mailto:akro_gis_helpdesk@nps.gov?subject=ThemeManager3.0)
