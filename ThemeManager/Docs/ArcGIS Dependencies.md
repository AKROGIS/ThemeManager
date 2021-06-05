# ArcGIS Dependencies

Most ArcGIS dependent code is in the `ThemeManager\ArcGIS` folder in the
`NPS.AKRO.ThemeManager.ArcGIS` namepspace. However all of this code is dependent
on ArcObjects. This namespace should be broken out into a class libary and a
similar version of the library built for Pro, and for no GIS.

**Note:** The line numbers are for files circa May 2021 (commit `a757bb0`)

**TODO:** Figure out how to have one project link or load a different version of
a library based on runtime conditions.  Developer will need to have both the
Pro and the ArcObjects development libraries available.

## Files using `NPS.AKRO.ThemeManager.ArcGIS`

* `ThemeManager\Model\Metadata.cs`
  * `EsriMetadata.GetContentsAsXml(Path)` on line 958
* `ThemeManager\Model\TmNode.cs`
  * `ThemeBuilder.BuildSubThemesForMapDocument(this)` on line 636
  * `ThemeBuilder.BuildThemesForLayerFile(this)` on line 642
* `ThemeManager\UI\Forms\LoadingForm.cs`
  * `EsriMetadata.LoadWithCatalog(path)` on lines 199
  * `EsriLicenseManager.Start()` on lines 212
  * `EsriLicenseManager.Message` on lines 215
* `ThemeManager\UI\Forms\MainForm.cs`
  * `EsriLicenseManager.Start()` on lines 1206
  * `EsriLicenseManager.Running` on lines 1205, 1207 and 1488
  * `EsriLicenseManager.Message` on lines 1208
  * `EsriLicenseManager.Stop()` on lines 1489
* `tm\Program.cs`
  * `EsriLicenseManager.Start()` on lines 16
  * `EsriLicenseManager.Running` on lines 17
  * `EsriLicenseManager.Message` on lines 19

## StyleSheets

`ThemeManager\Model\StyleSheets.cs` references the ArcGIS metadata
library directly on lines 108 to 120. This should be moved to the
ArcGIS namespace.

* ArcObjects

  ```c#
  var esri = new ESRI.ArcGIS.Metadata.Editor.XsltExtensionFunctions();
  return ESRI.ArcGIS.Metadata.Editor.XsltExtensionFunctions.GetResString(match.Groups[1].Value);
  ```

* Pro 2.5

  ```c#
  var esri = new ArcGIS.Desktop.Metadata.Editor.XsltExtensionFunctions();
  return ArcGIS.Desktop.Metadata.Editor.XsltExtensionFunctions.GetResString(match.Groups[1].Value);
  ```

* Pro 2.8

  ```c#
  var esri = new ArcGIS.Desktop.Internal.Metadata.XsltExtFunctions();
  return ArcGIS.Desktop.Internal.Metadata.XsltExtFunctions.GetResString(match.Groups[1].Value);
  ```

## Map Controls

`ThemeManager\UI\Forms\MainForm.cs` has the following ArcObjects
references. This should be moved to the ArcGIS namespace.

* using ESRI.ArcGIS.Controls;
  * `AxToolbarControl()` at 1212
  * `AxMapControl()` at line 1222
* using ESRI.ArcGIS.SystemUI;
  * `esriCommandStyles` on lines 1234 to 1239
