# Theme Manager

The National Park Service Theme Manager is a GIS application for browsing
GIS data sets by theme, previewing the datasets and reading metadata.
It provides an intuitive interface with advanced search capabilities.
It is intended as an easy to use alternative to ArcCatalog.

The database of themes it uses are typically created and updated by a GIS
data manager and made available to all the user's in an organization,
however it also supports private theme lists created and managed by the user.

It is a Windows application and uses the .NET Framework 4.5, WinForms, and
ArcObjects 10.x.

## Build

* Clone this repository to your local file system.
* Copy the file `C:\Program Files (x86)\ArcGIS\Desktop10.8\bin\ESRI.ArcGIS.MetadataEditor.dll`
(or similar) to the folder containing `ThemeManager.csproj`.
* Install the version of Microsoft Visual Studio Community supported by your
version of ArcGIS.
* Install the ArcObjects SDK (comes with ArcGIS Desktop 10.x).
* Open `ThemeManager.sln` in Visual Studio.
* Select `Build -> Build Solution` from the Visual Studio menu.
* If the file `ThemeManager/Docs/Help.md` is edited then update the files
`ThemeManager/Html/Help.html` by using a markdown to html converter like
[pandoc](https://pandoc.org), an online convertor, or the
[atom](https://atom.io) editor which includes github styling.
(This should be done before building to ensure a copy ends up in the
`bin/release` folder)

## Deploy

Detailed installation instructions are in the
[Help Documentation](https://github.com/AKROGIS/ThemeManager/blob/master/ThemeManager/Docs/Help.md#installation),
however they assumes that you have downloaded Theme Manager from
[IRMA](https://irma.nps.gov/DataStore/Reference/Profile/2188597).
The following instructions are how to build the zip file from the repo.

* Build a release version
* Delete the file `ThemeManager/bin/release/ThemeManager.pdb`
* Copy the `ThemeManager/bin/release`
folder to a new location renamed `Theme Manager 3.x for ArcGIS 10.y`
(editing `x` and `y` as appropriate).
* Zip up `Theme Manager 3.x for ArcGIS 10.y`
* Distribute

## Using

See the
[Help document](https://github.com/AKROGIS/ThemeManager/blob/master/ThemeManager/Docs/Help.md#using-theme-manager).

# Command Line Utility `tm`

This repo also includes a command line utility called `tm` which
when run with a theme list, i.e. `$ tm \path\to\themelist.tml`,
will reload all the themes in the theme list, and re-sync all
of the metadata.  This can be handy if there has been a major
reorganization and update of the themes (layer files).  However,
it should be used with the understanding that it may overwrite any
custom attribution that you might have added.  For example if a data
source has default (empty) metadata, then the re-sync will clear the
tags, summary, description, etc, even if you have hand entered data for
those fields.
See [Issue 12](https://github.com/AKROGIS/ThemeManager/issues/12) for details.
