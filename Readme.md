# Theme Manager

The National Park Service Theme Manager is a GIS application for browsing
GIS data sets by theme, previewing the datasets and reading metadata.
It provides an intuitive interface with advanced search capabilities.
It is intended as an easy to use alternative to ArcCatalog.

The database of themes it uses are typically created and updated by a GIS
data manager and made available to all the user's in an organization,
however it also supports private theme lists created and managed by the user.

It is a Windows application and uses the .NET Framework 4.5, and WinForms.  To
enable GIS features, you must also have ArcObjects 10.x and/or Pro 2.5+.

## Build

* Clone this repository to your local file system.
* Install the version of Microsoft Visual Studio Community supported by your
version of ArcGIS. You may need a different version for Pro and for 10.x.
* For ArcGIS 10.x support:
  * Install the ArcObjects SDK (comes with ArcGIS Desktop 10.x).
  * Copy the file `C:\Program Files (x86)\ArcGIS\Desktop10.8\bin\ESRI.ArcGIS.MetadataEditor.dll`
    (or similar) to the `ArcGis10x` folder.
* For Pro 2.5+ support:
  * You must have ArcGIS Pro installed.
  * Copy the file `C:\Program Files\ArcGIS\Pro\bin\Extensions\Metadata\ArcGIS.Desktop.Metadata.dll`
    to the `ArcGisPro` folder.
  * Download and extract the [File Geodatabase API](https://github.com/Esri/file-geodatabase-api).
    Copy `Esri.FileGDBAPI.dll`, `FileGDBAPI.dll`, and `FileGDBAPID.dll` from
    the `64bit` folder to `ArcGisPro` folder.  See `ArcGisPro\Fgdb.cs` or the
    readme with the download for details.
  * Install the Pro SDK as a visual studio extension (see
  <https://github.com/Esri/arcgis-pro-sdk/wiki/ProGuide-Installation-and-Upgrade>)
* Open `ThemeManager.sln` in Visual Studio.
* Select `Build -> Build Solution` from the Visual Studio menu.  If you are not
supporting both ArcGIS Pro and 10.x, then you may need to build only the supported
projects.
* If the file `Docs/Help.md` is edited then update the files
`ThemeManager/Html/Help.html` by using a markdown to html converter like
[pandoc](https://pandoc.org), an online convertor, or the
[atom](https://atom.io) editor which includes github styling.
(This should be done before building to ensure a copy ends up in the
`bin/release` folder)

## Deploy

### Theme Manager for 10.x

Detailed installation instructions are in the
[Help Documentation](https://github.com/AKROGIS/ThemeManager/blob/master/ThemeManager/Docs/Help.md#installation),
however they assumes that you have downloaded Theme Manager from
[IRMA](https://irma.nps.gov/DataStore/Reference/Profile/2188597).
The following instructions are how to build the zip file from the repo.

* Build a release version
* Delete the file `ThemeManager10x/bin/release/ThemeManager.pdb`
* **For 10.x only**, Copy `ArcGis10x/bin/release/ArcGis10x.dll` and
  ``ArcGis10x/ESRI.ArcGIS.MetadataEditor.dll` to `ThemeManager10x/bin/release`
* Copy the `ThemeManager10x/bin/release`
folder to a new location renamed `Theme Manager 3.x for ArcGIS 10.y`
(editing `x` and `y` as appropriate).
* Zip up `Theme Manager 3.x for ArcGIS 10.y`
* Distribute

## Using

See the
[Help document](https://github.com/AKROGIS/ThemeManager/blob/master/Docs/Help.md#using-theme-manager).

## Projects

The Visual Studio solution file `ThemeManager.sln` contains several projects.

### Library `ArcGis10x`

Implements a `GisInterface` class with a small set of static methods that
implement all of the functions that require support from an external GIS
system.  In this case the ArcObjects 10.x libraries. This library also vends
a custom implementation of the IGisLayer object to provide details about the
GIS objects it supports.

### Library `ArcGisPro`

Same as the `ArcGis10x` library but providing support for ArcGIS Pro.

### Library `NoGis`

Same as the `ArcGis10x` library but provides no GIS support (also does not
require the user to have any GIS software installed).

### WinForm Application `ThemeManager10x`

Classic Theme Manager which links with the `ArcGis10x` library. It builds an
app that expects the user to have ArcGIS 10.x installed and licensed.

### WinForm Application `ThemeManagerPro`

Builds a version of Theme Manager which links with the `ArcGisPro` library. It
expects the user to have ArcGIS Pro installed and licensed. This project has
no code of it's own -- it links to the code in `ThemeManager10x`. A separate
project is required because the Pro version requires 64bit and .net framework
4.8 both of which are incompatible with the 10.x version.  This project is not
required to build the Pro version, but makes it similar to build both projects
without switching build parameters back and forth.

### WinForm Application `ThemeManagerNoGis`

Builds a version of Theme Manager which links with the `NoGis` library. It does
not expect the user to have any GIS installed or licensed. This project has
no code of it's own -- it links to the code in `ThemeManager10x` and provide
the different build instructions.

### Command Line Utility `tm`

This project is a command line utility which
when run with a theme list, i.e. `$ tm \path\to\themelist.tml`,
will reload all the themes in the theme list, and re-sync all
of the metadata.

This can be handy if there has been a major
reorganization and update of the themes (layer files).  However,
it should be used with the understanding that it may overwrite any
custom attribution that you might have added.  For example if a data
source has default (empty) metadata, then the re-sync will clear the
tags, summary, description, etc, even if you have hand entered data for
those fields.
See [Issue 12](https://github.com/AKROGIS/ThemeManager/issues/12) for details.

This project is configured to link with `ArcGis10x` and build a tool
that expects ArcGIS 10.x support. By changing the .net framework to 4.8 (from
4.5), the target platform to x64 (from x86), and referencing `ArcGisPro` in lieu
of `ArcGIS10x` it can be run with ArcGIS Pro.  Note that only the 10.x version
will be able to reload themes based on `*.lyr` or `*.mxd` files, and only the
Pro version will be able to reload themes based on a `*.lyrx` file.

### Command Line Utility `tmpro`

This project also includes a command line utility called `tmpro` which
when run will print a CSV report to the standard output listing all the data
sources in the `*.lyrx` files in a folder tree. This report is useful to make
sure that all the pro data sources are correctly interpreted by the Theme
Manager code.  The output can be compared with the _DataSources_ report in the
administrative reports menu.

See the discussion for the `tm` utility for how to build this app with support
for reading a folder of *.lyr` files.
