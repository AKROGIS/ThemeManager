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
   **C:\Program Files (x86)\ArcGIS\Desktop10.5\bin** or similar for Desktop.
   If you have Pro and not desktop, you will need a special build of Theme Manager specifically for
   Pro.  In that case, copy **ArcGIS.Desktop.Metadata.dll** from 
   **C:\Program Files\ArcGIS\Pro\bin\Extensions\Metadata** to the location where Theme Manager is installed.

2. Create a folder called **Esri** in Theme Manager's StyleSheet folder.  The StyleSheet folder
   is typicall called **StyleSheets** in the folder where Theme Manager is installed.  However, it
   can be given a differnt name by editing the **StyleSheetDirectory** setting in **ThemeManager.exe.config**

3. Desktop: Copy the Esri Stylesheets **ArcGIS.xsl** and **ArcGIS_ItemDescription.xsl** from the folder
   **C:\Program Files (x86)\ArcGIS\Desktop10.5\Metadata\Stylesheets** (or similar) to ThemeManager's
   **StyleSheets\Esri** folder.  The files can be renamed to something more intuitive for display
   in the Stylesheet picker in Theme Manager, but the files must retain the **.xsl** extension.
   You also need to copy the **ArcGIS_Imports** folder (and all it contents) from
   **C:\Program Files (x86)\ArcGIS\Desktop10.5\Metadata\Stylesheets** (or similar) to
   **StyleSheets\Esri**.  These files are referenced by the main files, and need to
   remain in a sub folder called **ArcGIS_Imports** with the original names.

4. Pro: Copy the Esri Stylesheets **ArcGISPro.xsl** and **ArcGISProFull.xsl** from the folder
   **C:\Program Files\ArcGIS\Pro\Resources\Metadata\Stylesheets** (or similar) to ThemeManager's
   **StyleSheets\Esri** folder.  The files can be renamed to something more intuitive for display
   in the Stylesheet picker in Theme Manager, but the files must retain the **.xsl** extension.
   You also need to copy the **ArcGIS_Imports** folder (and all it contents) from 
   **C:\Program Files\ArcGIS\Pro\Resources\Metadata\Stylesheets** (or similar) to
   **StyleSheets\Esri**.  These files are referenced by the main files, and need to
   remain in a sub folder called **ArcGIS_Imports** with the original names.

5. You can use both Pro and Desktop style sheets at the same time, provided the names do
   not clash.  Fortunately, the names of the files in ArcGIS_Imports have the same name
   (except ArcGIS_Imports\XML.xslt which is the same) so you can combine both sets of files
   in **StyleSheets\Esri\ArcGIS_Imports**.  And both sets of stylesheets require one of the
   dlls from step 2, but not both.
