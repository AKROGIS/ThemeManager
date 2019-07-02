using System;
//using System.IO;
using System.Diagnostics;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    public class MapViewer
    {
        private static AxMapControl _mapControl;
        private static AxToolbarControl _mapToolbar;
        private static bool _triedToCreateControl;
        private static bool _triedToBuildToolBar;



        public static string LoadMapFileInControl(string fileName, Control control)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            if (control == null)
                throw new ArgumentNullException("control");

            if (!_triedToCreateControl) {
                try {
                    CreateMapControl (control);
                } catch (Exception ex) {
                    return "Could not create map viewer: " + ex.Message;
                }
            }
            if (_mapControl == null || _mapToolbar == null)
                return "Map viewer could not be loaded";
            
            AddMapControlToControl (control);

            string msg = null;
            if (fileName.EndsWith(".mxd", StringComparison.CurrentCultureIgnoreCase) ||
                fileName.EndsWith(".mxt", StringComparison.CurrentCultureIgnoreCase)) {
                if (_mapControl.CheckMxFile (fileName)) {
                    try {
                        _mapControl.LoadMxFile (fileName);
                        _mapControl.Extent = _mapControl.FullExtent;
                    } catch (Exception ex) {
                        msg = "ESRI Map Control generated an error.\nFile: " + fileName + "\nError: " + ex.Message;
                    }
                } else
                    msg = "Map document not valid: " + fileName;
            }
            if (fileName.EndsWith(".lyr", StringComparison.CurrentCultureIgnoreCase)) {
                try {
                    _mapControl.ClearLayers();
                    _mapControl.SpatialReference = null;
                    _mapControl.AddLayerFromFile (fileName);
                    //Make sure the first layer is visible
                    _mapControl.get_Layer(0).Visible = true;
                    _mapControl.Extent = _mapControl.FullExtent;
                } catch (Exception ex) {
                    msg = "ESRI Map Control generated an error.\nFile: " + fileName + "\nError: " + ex.Message;
                }
            }
            return null;
        }

        public static void ResizeBegin()
        {
            if (_mapControl != null)
                _mapControl.SuppressResizeDrawing(true, 0);
        }

        public static void ResizeEnd()
        {
            if (_mapControl != null)
                _mapControl.SuppressResizeDrawing(false, 0);
        }



        
        private static void AddMapControlToControl(Control control)
        {
            Debug.Assert(_mapControl != null && _mapToolbar != null, "MapControls are null");

            // control may already contain the map control, or it may have something else like a txet box
            if (control.Controls[0] != _mapToolbar)
            {
                control.Controls.Clear();
                control.Controls.Add(_mapControl);
                control.Controls.Add(_mapToolbar);
                // I can't add items to the toolbar until it has been added to a parent control
                // but I only need to do it the first time it is added.
                if (!_triedToBuildToolBar)
                    BuildToolbar();
            }
        }

        private static void CreateMapControl(Control control)
        {
            Trace.TraceInformation ("{0}: Start of CreateMapControl()", DateTime.Now); Stopwatch time = Stopwatch.StartNew ();

            _triedToCreateControl = true;

            // Need to load license before we can initialize the MapViewer Type
            // FIXME - When MapViewer type gets initialized is unspecified.  We are relying on lazy initialization by MSIL
            if (!EsriLicenseManager.Running)
                EsriLicenseManager.Start(true);
            if (!EsriLicenseManager.Running)
                throw new Exception("Could not initialize an ArcGIS license. \n" + EsriLicenseManager.Message);

            Trace.TraceInformation("{0}: CreateMapControl()- Got License - Elapsed time: {1}", DateTime.Now, time.Elapsed);

            _mapToolbar = new AxToolbarControl();
            ((ISupportInitialize)_mapToolbar).BeginInit();
            _mapToolbar.Name = "mapToolbar";
            _mapToolbar.Location = new Point(0, 0);
            _mapToolbar.Size = new Size(600, 28);
            ((ISupportInitialize)_mapToolbar).EndInit();

            _mapControl = new AxMapControl();
            ((ISupportInitialize)_mapControl).BeginInit();
            _mapControl.Name = "mapControl";
            _mapControl.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            _mapControl.Location = new Point(0, 28);
            _mapControl.Size = new Size(control.ClientSize.Width, control.ClientSize.Height - 28);
            ((ISupportInitialize)_mapControl).EndInit();

            time.Stop(); Trace.TraceInformation("{0}: End   of CreateMapControl() - Elapsed time: {1}", DateTime.Now, time.Elapsed);
        }

        private static void BuildToolbar()
        {
            _triedToBuildToolBar = true;
            _mapToolbar.AddItem("esriControls.ControlsMapZoomInTool", -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            _mapToolbar.AddItem("esriControls.ControlsMapZoomOutTool", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            _mapToolbar.AddItem("esriControls.ControlsMapPanTool", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            _mapToolbar.AddItem("esriControls.ControlsMapFullExtentCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            _mapToolbar.AddItem("esriControls.ControlsMapZoomToLastExtentBackCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            _mapToolbar.AddItem("esriControls.ControlsMapZoomToLastExtentForwardCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            _mapToolbar.SetBuddyControl(_mapControl);
        }

    }
}

