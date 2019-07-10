### Theme Manager 3.0

### Installation Instructions

# Installation

Theme Manager 3.0 is delivered as a zip file that can be extracted to any convenient location1.  Theme Manager does not write to the registry, or make any changes to your system, so no administrative privileges are required to install or run it.

To run Theme Manager, just double click on the executable file (Theme Manager.exe) in the folder where you extracted the download.

The executable file (Theme Manager.exe) relies on various files and sub-folders that come with it, and it expects to find them in the same relative locations.  This means that if you want to move or copy the application, you must move or copy the entire folder.  Putting a shortcut to the executable file on your desktop is fine (use ctrl-shift-drag or right click, create shortcut), but do not copy just Theme Manager.exe to your desktop and expect it to run correctly.

# Requirements

Theme Manager requires the Microsoft .Net Framework 3.5 or higher (This is also a requirement for ArcGIS and other software, and has been a core component of Windows software since 2008, so you probably have it.  If not, it can be downloaded for free from Microsoft.

Theme Manager can be used on a computer without any GIS software, but to get full access to all the features you must have ArcGIS Desktop version 9.3 or 10 installed (and licensed) on your computer.  There is a different version of Theme Manager for each version for ArcGIS.  If you have installed mismatched versions, Theme Manager will run but you will not have access to any of the features that require ArcGIS.

# Using Theme Manager on a Network

A single install of Theme Manager can be used by multiple users if the application is unzipped to a shared folder on a network drive.  If the domain administrators have not designated the server or shared folder as a trusted location, you will be prompted to accept a security warning when you launch the application.  In some cases you may be denied the ability to run the application from the network. This is a site specific domain security issue beyond the programmer&#39;s control.  If it is a problem at your site, contact your domain administrator, or copy the application to a local (trusted) location and execute it from there.   If you have administrative control on your computer there are also ways to run off this security warning, but that is beyond the scope of this document – consult the Google.

# Accessing Theme Manager from ArcMap

Theme Manager is a stand-alone windows application.  It is not an ArcGIS extension.  It is typically launched via a shortcut on your desktop.

For ArcGIS Version 10, an add-in to launch the application from ArcMap requires installing Theme Manager in a well known location, and putting that location into the code for the add-in.  An add-in may be available for your site (contact your GIS Specialist, or the programmer), if not an add-in can be created using the ESRI tutorials.  Put the following code in the OnClick() method of the tool you create.

//Change the following path to your installation location

         string directory = @&quot;X:\Path\To\ThemeManager\Folder&quot;;

System.Diagnostics.ProcessStartInfo startInfo = newSystem.Diagnostics.ProcessStartInfo(directory + @&quot;\ThemeManager.exe&quot;);

startInfo.WorkingDirectory = directory;

Process.Start(startInfo);

For ArcGIS Version 9.3 extensions require administrative privileges (to writing to the registry), so that feature has been dropped.

1

#
 Provided you have write permission in that location – However, you do not need write permission to the installation location in order to run Theme Manager.