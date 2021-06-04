# Theme Manager and ArcGIS Pro

In simple terms, Theme Manager is just a program that organizes files in a
hierarchy of catagories, where a category is like a folder in a file system.
The files can be double clicked to open them in their associated application.
Each file can have associated properties (summary, description, tags and
publication date) in addition to a path to an XML metadata file. For more
information about this file. Theme Manager does not need any GIS software
installed to provide these features.

However, Theme Manager has a special relationship with ArcGIS 10.x and two
files used by ArcGIS 10.x: layer files `*.lyr` and map documents `*.mxd`.
If a user has ArcGIS 10.x installed and licensed, then Theme Manager can
provide the following three additional features:

1 When adding one of those files to a theme list, Theme Manager will use
  ArcGIS 10.x to open and interrogate the internal structure of these files
  so that it can represent the hierarchy of GIS data sources within.  Theme
  Manager will also ask ArcGIS 10.x to get the metadata for each data source and
  populate the properties mentioned above with information from the metadata.
2 When the metadata for a data source is not in an XML side car file, e.g. in
  a file geodatabase, then Theme Manager will ask ArcGIS 10.x to get the
  metadata for display to the user.
3 Theme Manager can use ArcGIS 10.x to provide a preview of the data in
  a `*.lyr` or `*.mxd` file.

Theme Manager is built for a specific version of ArcGIS 10.x. It will still run
without ArcGIS 10.x or with a mis-matched version of 10.x, however the three
additional features listed above will be unavailable.

## Using Theme Manager with ArcGIS Pro

Theme Manager built for ArcGIS 10.x will work, in a limited way, with ArcGIS
Pro.  This version of Theme Manager does not consider the ArcGIS Pro files,
`*.lyrx`, or `*.aprx` special and cannot get any information from them like
it can with `*.lyr` and `*.mxd` files.  Nevertheless, it will open those files
with ArcGIS Pro.

### Theme Manager for ArcGIS 10.x and `*.lyrx` files

There are two ways to add `*.lyrx` files to a theme list with a version of
Theme Manager built for ArcGIS 10.x.

1) Drag and drop a `*.lyrx` file onto a theme category in Theme Manager.
2) Click `File -> New Theme` in the menu. In the properties tab, click on the
   browse button adjacent to the `File` text box and select a `*.lyrx` file.

Since Theme Manager for ArcGIS 10.x cannot extract details from a `*.lyrx` file,
you will need to edit the Properties tab manually.  If you have a `*.xml`
metadata file for the layer file or the datasets therein, you can browse to the
file and sync to populate the summary, description, tags and publication date.

If you have Pro installed, then double clicking on the `*.lyrx` will launch Pro
with a new empty map and add the layer file to the map. To add the theme to an
existing map, drag and drop from Theme Manager to your Pro map pane.

### With ArcGIS 10.x

If you have ArcGIS Pro installed and ArcGIS 10.x and a matching version of Theme
Manager for ArcGIS 10.x, Then Theme Manager will have all of it's functionality.
If there is a `*.lyrx` file in the theme list, it can be added to a Pro map as
described above. To add a theme based on a `*.lyr` file (typical), you will need
to drag and drop the theme in an ArcGIS Pro map pane, because double clicking it
will open it with ArcMap.

If you want to import a `*.mxd` from Theme Manager to Pro, look at the
properties tab to get the path to the `*.mxd`, and then from the Pro import
map dialog, browse to that path to select the `*.mxd`.

### Without ArcGIS 10.x

Themes can be browses, searched for and added to an ArcGIS Pro project,
however the three additional features listed above will be unavailable.

## Theme Manager for ArcGIS Pro

What about building a version of Theme Manager specifically for ArcGIS Pro?
The special ArcGIS 10.x features in Theme Manager can theoretically be swapped
out for similar functionality in ArcGIS pro. Indeed, ArcGIS Pro comes with a
software development kit (SDK) for adding functionality to ArcGIS Pro. Most of
the functionality in the SDK is limited to in-process tasks via AddIns, plugins,
and customizations. The SDK only supports limited features in a stand alone
application via the [CoreHost](https://github.com/esri/arcgis-pro-sdk/wiki/proconcepts-CoreHost)
library - primarily reading and writing geodatabases and geometries.
Specifically it does not provide support for previewing `*lyrx` files or
for extracting metadata from a data source. It also cannot read `*.lyr`
or `*.mxd` files.  It can read `*.lyrx` files, but since those are just
[Cartographic Information Model (CIM)](https://github.com/Esri/cim-spec) files
in a JSON text file, there is nothing special about that.  However this does
open the door to supporting other CIM documents (maps, layouts and reports)
zipped up in the `*.aprx` file.

## Preview with Pro

* Pro provides no stand alone "preview" capability similar to that in
  the ArcGIS 10.x version of Theme Manager.
* In a Pro AddIn, the closest thing is to create a new empty map based on
  the layer file. However, this is no different than using stand alone Theme
  Manger to open the layer file in ArcGIS Pro.
* ArcGIS Pro "CoreHost" applications do not support graphic or UI functions.

## Layer File Interrogation

* 10.x (map, python, and arcObjects) can read `*.lyr` but not `*.lyrx`.
* Pro application can read `*.lyr` and `*.lyrx`.
* Pro AddIns can read `*.lyrx` but not `*.lyr` files.
* Pro Python can read `*.lyr` and `*.lyrx`, but can only save as `*.lyrx`.
* Pro "CoreHost" can read `*.lyrx`, but not `*.lyr` files.

## Metadata with Pro

* Pro CoreHost does not provide an interface for obtaining metadata from a
  data source.
* In Pro, metadata can be retrieved from any item data source path with Python
  and the arcpy module.
* The File Geodatabase API can be used to retrieve metadata from any data source
  in a file geodatabase without needing ArcGIS Pro or 10.x.

## Other Options

### Using ArcEngine

* Would support all existing Theme Manager functions without needing ArcMap
  or Pro to be installed.
* Requires admin install of ArcEngine and licensing.
* Does not support *.lyrx files (except as noted above)

### Runtime SDK

These SDKs are intended for working with content and map documents on ArcGIS
Online. They do not support layer files, or local map documents or projects

### ArcPy

`arcpy` for Python and Pro can read `*.lyr` as well as `*.mxd` and save and
`*.lyrx` and `*.mapx` files. `arcpy.mp.Layer` has various methods for querying
a layer like `isGroup`, `longName`, and `datasource`, all of which map nicely to
the properties Theme Manager needs. In addition,  arcpy can also read metadata
from an item to an internal Python object that can be saved as XML:

```python
item_path = r'C:\Data\LocalArea.gdb\Streets'
metadata = arcpy.metadata.Metadata(item_path)
metadata.exportMetadata(export_19139_path) # no option to save as ArcGIS format
# or convert to html; note does not work with esri provided stylesheets
metadata.exportMetadata('myfile.html', 'CUSTOM', 'REMOVE_ALL_SENSITIVE_INFO',`custom_xml2html.xlst`)
```

The mapping module could also be used to create a blank project, load a layer
file, and export hte map to a snapshot jpeg image, which could be displayed to
the user as a static (no zooming) preview image.

However, This is all done as an external process reading and writing temporary
files to disk. Loading arcpy as an external process takes several seconds each
time a task is required making it too slow without some optimizing work. Even if
arcpy can be preloaded into Theme manager (I doubt it), it is still
significantly slower than internal processes.

### File Geodatabase API

Esri provides a library for reading file geodatabases in third party
applications without a ArcGIS license.  This can be used to extract metadata
from items in a fgdb without using Pro (or ArcMap 10.x).  The library adds
about 20MB to the size of Theme Manager (only about 1MB), and only works with
FGDBs, not SDE, PGDB (not supported by Pro), or file based metadata files.
so different techniques will need to be used and may not be able to retrieve
metadata in all cases.  However, the FGDB is the primary data source.
