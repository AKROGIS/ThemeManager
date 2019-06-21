Stylesheets
===========

Theme Manager comes with two stylesheets to convert XML metadata into a html file suitable for display.
Esri provides a few more comprehensive stylesheets that Theme Manager users may want to use.
The Stylesheets require processing by an Esri library (dll) that is not part of the developers SDK,
so it is not available for general access by Theme Manager.

If you want to add the Esri Stylesheets to Theme Manager, you must have ArcGIS Desktop or ArcGIS Pro
installed on your computer.  Then follow these steps.

1. Copy the file **ESRI.ArcGIS.MetadataEditor.dll** to the location where Theme Manager is installed.
The dll must be in the same folder as ThemeManager.exe.  This file is typically found in
**C:\Program Files (x86)\ArcGIS\Desktop10.5\bin** or similar for Desktop, or
**FIXME** for ArcGIS Pro

2. Create a folder called **Esri** in Theme Manager's StyleSheet folder.  The StyleSheet folder
is typicall called **StyleSheets** in the folder where Theme Manager is installed.  However, it
can be given a differnt name by editing the **StyleSheetDirectory** setting in **ThemeManager.exe.config**

3. Desktop: Copy the Esri Stylesheets **FIXME** and **FIXME** from the folder
   **FIXME** to the **StyleSheets\Esri** folder.  The files can be renamed to something
   more intuitive for display in the Stylesheet picker in Theme Manager.  The files must
   have a **.xslt** extension.  You also need to copy the **ArcGIS_Imports** folder (and all
   it contents) from **FIXME** to **StyleSheets\Esri**.  These files are referenced by the
   main files, and need to remain in the **ArcGIS_Imports** folder with the original names.

4. Pro: Copy the Esri Stylesheets **FIXME** and **FIXME** from the folder
   **FIXME** to the **StyleSheets\Esri** folder.  The files can be renamed to something
   more intuitive for display in the Stylesheet picker in Theme Manager.  The files must
   have a **.xslt** extension.  You also need to copy the **ArcGIS_Imports** folder (and all
   it contents) from **FIXME** to **StyleSheets\Esri**.  These files are referenced by the
   main files, and need to remain in the **ArcGIS_Imports** folder with the original names.

5. You can use both Pro and Desktop style sheets at the same time, provided the names do
   not clash.  Fortunately, the names of the files in ArcGIS_Imports are the same, or have
   different names.\, so you can combine both sets of files in **ArcGIS_Imports**.
