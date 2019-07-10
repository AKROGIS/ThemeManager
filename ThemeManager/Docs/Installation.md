Theme Manager 3.1 - Installation Instructions
=================

# Contents
  * [Installation](#installation)
  * [Requirements](#requirements)
  * [Using ESRI Metadata Stylesheets](#using-esri-metadata-stylesheets)
  * [Using Theme Manager on a Network](#using-theme-manager-on-a-network)
  * [Accessing Theme Manager from ArcMap](#accessing-theme-manager-from-arcmap)

# Installation

Theme Manager is delivered as a zip file that can be extracted to any convenient location[^1]. Theme Manager does not write to the registry, or make any changes to your system, so no administrative privileges are required to install or run it.

To run Theme Manager, just double click on the executable file (Theme Manager.exe) in the folder where you extracted the download.

The executable file (Theme Manager.exe) relies on various files and sub-folders that come with it, and it expects to find them in the same relative locations.  This means that if you want to move or copy the application, you must move or copy the entire folder.  Putting a shortcut to the executable file on your desktop is fine (use ctrl-shift-drag or right click, create shortcut), but do not copy just Theme Manager.exe to your desktop and expect it to run correctly.

# Requirements

Theme Manager 3.1 requires the Microsoft .Net Framework 4.5 or higher (This is also a requirement for ArcGIS 10.4+ and other software, and has been a core component of Windows software since 2013, so you probably have it.  If not, it can be downloaded for free from Microsoft.

Theme Manager can be used on a computer without any GIS software, but to get full access to all the features you must have ArcGIS Desktop version 10.5 (or higher) installed (and licensed) on your computer.  There is a different version of Theme Manager for each version for ArcGIS. If you have installed mismatched versions, Theme Manager will run but you will get error and you will not have access to any of the features that require ArcGIS.  Theme Manager will work with ArcGIS Pro, but some features (mainly previewing themes and loading/searching/displaying metadata in a geodatabase) do not work.

Theme Manager 3.0 is available for ArcGIS 9.3 through ArcGIS 10.6.

# Using ESRI Metadata Stylesheets

Stylesheets are required to stylize the metadata in XML format to an easier to read HTML format.  Esri provides Stylesheets which work with FGDC, ISO, and ESRI formatted XML metadata, however
these stylesheets require the use of a esri code library not generally available to custom
applications like Theme Manager.  Nevertheless, if you have ArcGIS 10.4+ installed, you can use the ESRI stylesheets with Theme Manager if you follow [these instructions](https://github.com/regan-sarwas/ThemeManager/blob/master/ThemeManager/Docs/Stylesheets.md).

# Using Theme Manager on a Network

A single install of Theme Manager can be used by multiple users if the application is unzipped to a shared folder on a network drive.  If the domain administrators have not designated the server or shared folder as a trusted location, you will be prompted to accept a security warning when you launch the application.  In some cases you may be denied the ability to run the application from the network. This is a site specific domain security issue beyond the programmer's control.  If it is a problem at your site, contact your domain administrator, or copy the application to a local (trusted) location and execute it from there.   If you have administrative control on your computer there are also ways to run off this security warning, but that is beyond the scope of this document – consult the Google.

# Accessing Theme Manager from ArcMap

Theme Manager is a stand-alone windows application.  It is not an ArcGIS extension.  It is typically launched via a shortcut on your desktop.

To launch the application from ArcMap requires installing an ArcMap Add-In, installing Theme Manager in a well known location, and putting that location into the code for the Add-In.  An Add-In may be available for your site (contact your GIS Specialist, or the programmer), if not an Add-In can be created using the ESRI tutorials.  Put the following code in the `OnClick()` method of the tool you create.

``` c#
    //Change the following path to your installation location
    string directory = "X:\Path\To\ThemeManager\Folder";
    System.Diagnostics.ProcessStartInfo startInfo =
        newSystem.Diagnostics.ProcessStartInfo(directory + "\ThemeManager.exe");
    startInfo.WorkingDirectory = directory;
    Process.Start(startInfo);
```
[^1]: Provided you have write permission in that location – However, you do not need write permission to the installation location in order to run Theme Manager.
