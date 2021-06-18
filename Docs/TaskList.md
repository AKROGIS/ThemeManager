Priority User Requested Features
================================
* Filtering
  - Each node would get a hidden flag.  Regular UI would only show nodes which are not hidden.
  - Would need to have a special UI mode, which shows all nodes for editing the hidden flag
  - A simpler solution may be piggy back on the Shortcut feature.
* Shortcuts
  - See last section of this doc.


Priority Admin Features
=======================
* Undo/Redo when editing themelists
* multi-select drag-n'-drop not working


Bugs
=====
* See Help.doc


Testing
=======
* Check with "borrowed license"
* Check with no license


Cleanup/Beautification
======================
* Review the Iserializable classes.  Make sure we are not serializing more than we need.
* Clean up/beautify properties panel
* Preferences Panel Cleanup and Expansion


New features - Editing/Misc
===========================
* Filter Theme list based on user/park preferences
* Select multiple themes
* New Logo
* Undo/Redo when editing themelists
* Store favorites and search results as "shortcuts", not clones
* Allow park based configuration files
* Save user choice of Sort Order for each treeview
* Search results to show pointer back to source
* Take advantage of ArcGIS 10 Indexing ??
* Create my own metadata cache/index  (How do you guarantee cache is not stale - consider metadata in services and Geodatabases)
* Compress the xml themelist
* Save an arbitary tree as a new themelist
* Use today's date as pubdate when creating a new category/themelist
* use layerfile's file date when creating a new theme if there is no pubdate in metadata)
* Check out license in background
* Setting to skip load of old themes from registry
* Setting to always check and load "Standard Theme" (as specified by admin)
* Setting to specify search path and load all themes found in those folders.
* Use editable datagrid for Author attributes.
* searching is confusing.  Make default searching search all.  Make advanced search easier to get to.
* Advanced search panel
	- specify arbitrary metadata element to search.
	- Make the search panel more like a Mac search (graphically build a search string)
* Allow relative paths - use a themelist property
* Allow drag and drop between siblings
 - redo draw behaviour of treeview.
* Allow drop on favorites tab (== add to favorites)
* Restore unsorted order
* Copy/Paste category theme as a theme manager path (text)
* Parallel Linq to speed up searches.
* Allow copy of readonly properties
* Search tree should not contain only a sub-theme (add the full theme)
* provide options to sort just visible, or just selected branches
* provide a default "new" button based on current tree/node
* provide preference so that search from toolbar uses current tab panel/current node as search point
* provide "Cancel" button while doing a Sync All on a theme list
* provide a cancel button while checking for a ArcGIS License
* Create a cache of thumbnail images, store in metadata.


New features - User Interface
=============================
* Use WPF for more elegant UI
* Use flyout/dockable windows (instead of tab panels)
* Separate Toolbar, Tree browser, and metadata windows to allow repositioning.
* Implement floating toolbars
* implement closable toolbars
* context menu on form to add/remove menus/toolbars
* New icon/function should switch to the last used tool.
* Add description to status bar, and/or tooltip
* Search results should show where they came from
* Allow font/color choices for different node types.


Code reorganization (refactoring)
===================================
* If drag over a tab, then switch to that tab.
* The `TMNode._datastore` should be a `ThemeData` Object, which may have a datastore property.


Questions
==================
* Do I want to clone or reference TMNode when copying?
  - Clone: Edits in favorites should not show up in Theme List.
  - Reference:  Favorites should reflect changes in the main themelist.
* Should default search with multiple words be search all or search any?


Improvements in 3.0
===================
* Faster Loading
  - Save and read theme lists as XML
  - Stand alone TM does not connect to license server until it needs to.
* Doesn't require an adminstrative installation
* Doesn't write to the registry
* drag'n'drop/copy'n'paste of themes and categories in all trees
* Create/Edit categories in favorites and searches
* Full editing of searches and favorites
* Saves favorites and searches
* Can drag'n'drop/copy'n'paste any file (i.e kml/mxd) into the themelist and launch
* display/search metadata in SDE feature classes
* layer groups can be nested arbitrarily deep.

Improvements in 3.1
===================
* Now uses real [W3C specification for XSLT version 1.0](https://www.w3.org/TR/xslt-10/) stylesheets
* Removes dependency on vulnerable MSXML2.0 and the obscure MS draft XSL transformation
* Can use esri 10.0+ metadata stylesheets (if ArcGIS is installed)
* Attributes can be extracted from ISO (19139) metadata
* HTML tags are stripped from extracted metadata attributes when adding to Theme node
* Can extract attributes, search and display Metadata for ArcGIS services (Map, Image, and Feature)
* Better logic for finding metadata for data sources.
* Removed option (in preferences to preload metadata).
* Metadata is always guaranteed fresh, no caching.
* Reading and saving Author data on the Themelist is supported
* Sync All metadata on a category or group layer does not stop (with no metadata) and not process children
* Resize text boxes on properties form (for longer metadata details)

Improvements in 3.2?
====================
* Async
* Responsive to retina displays; high resolution icons
* Advanced Search default to sub tree, warning on takes a long time, default to search all, arbitary XPath
* Drop support for ArcGIS 9.x and 10.0 (ThemeManager/ArcGIS/EsriLicenseManager.cs)
* Better Error message when esri library's can be found, or are the wrong version.
* Remove references to save as .mdb

Shortcut Idea
=============
Create a new node type which is a shortcut.
 * Every node gets a persisted GUID on creation (new on copy/paste); a shortcut has a reference to a GUID
 * Favorites and searches can be shortcuts to a any node in a themelist
 * these shortcuts can be organized with categories/sub categorys.
 * Filtered Themelists can be created by organizing shortcuts to only "interesting" categories
 * favorites and searches can only have a categories or shortcuts.
 * short cuts must point to a category or theme (not themelist or subtheme) in a themelist.
 * Shortcuts may become broken if the data they point to is deleted, but not reorganized.
 * In Lieu of a GUID, each shortcut could contain a path to a node (seems much more fragile)
