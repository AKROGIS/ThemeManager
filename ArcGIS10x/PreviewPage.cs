using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    public class PreviewPage
    {

        private AxMapControl mapControl;
        private AxToolbarControl mapToolbar;
        private Control _parent;
        private Control.ControlCollection _controls;
        private Control _defaultControl;
        private Form _form;

        public PreviewPage(Control parent, Form form)
        {
            _parent = parent;
            _controls = parent.Controls;
            _defaultControl = _controls[0];
            _form = form;
        }

        public void ShowText(string text)
        {
            if (_controls[0] != _defaultControl)
            {
                _controls.Clear();
                _controls.Add(_defaultControl);
            }
            _defaultControl.Text = text;
        }

        public async Task ShowMapAsync(string path)
        {
            if (mapControl == null)
            {
                try
                {
                    await CreateMapControlAsync();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to create ESRI mapControl\n" + ex);
                    mapControl = null;
                }
            }

            if (mapControl == null)
            {
                ShowText("Unable to initialize the ArcGIS Viewer.");
            }
            else
            {
                PrepPageForMap();
                string msg = LoadMapFileInPreviewControl(path);
                if (!string.IsNullOrEmpty(msg))
                {
                    ShowText(msg);
                }
            }
        }

        private void PrepPageForMap()
        {
            if (mapControl == null)
                throw new Exception("mapControl is null");
            if (mapToolbar == null)
                throw new Exception("mapToolbar is null");
            if (_controls[0] != mapToolbar)
            {
                _controls.Clear();
                _controls.Add(mapToolbar);
                _controls.Add(mapControl);
            }
        }

        private async Task CreateMapControlAsync()
        {
            Trace.TraceInformation("{0}: Start of CreateMapControl()", DateTime.Now); Stopwatch time = Stopwatch.StartNew();
            ShowText("Loading preview image...");

            await EsriLicense.GetLicenseAsync();

            Trace.TraceInformation("{0}: CreateMapControl()- Got License - Elapsed time: {1}", DateTime.Now, time.Elapsed);

            mapToolbar = new AxToolbarControl();
            ((ISupportInitialize)mapToolbar).BeginInit();
            mapToolbar.Name = "mapToolbar";
            //mapToolbar.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            mapToolbar.Location = new Point(0, 0);
            mapToolbar.Size = new Size(600, 28);
            //AutoScaleDimensions = new System.Drawing.SizeF(6f, 13F);
            //AutoScaleMode = AutoScaleMode.Font;
            ((ISupportInitialize)mapToolbar).EndInit();

            mapControl = new AxMapControl();
            ((ISupportInitialize)mapControl).BeginInit();
            mapControl.Name = "mapControl";
            mapControl.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            mapControl.Location = new Point(0, 28);
            mapControl.Size = new Size(_parent.ClientSize.Width, _parent.ClientSize.Height - 28);
            _form.ResizeBegin += MapViewer_ResizeBegin;
            _form.ResizeEnd += MapViewer_ResizeEnd;
            ((ISupportInitialize)mapControl).EndInit();

            // I can't add items until it is activated in a parent control
            PrepPageForMap();
            mapToolbar.AddItem("esriControls.ControlsMapZoomInTool", -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
            mapToolbar.AddItem("esriControls.ControlsMapZoomOutTool", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            mapToolbar.AddItem("esriControls.ControlsMapPanTool", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            mapToolbar.AddItem("esriControls.ControlsMapFullExtentCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            mapToolbar.AddItem("esriControls.ControlsMapZoomToLastExtentBackCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            mapToolbar.AddItem("esriControls.ControlsMapZoomToLastExtentForwardCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);

            //tocControl.SetBuddyControl(mapControl);
            mapToolbar.SetBuddyControl(mapControl);

            time.Stop(); Trace.TraceInformation("{0}: End of CreateMapControl() - Elapsed time: {1}", DateTime.Now, time.Elapsed);
        }

        private void MapViewer_ResizeBegin(object sender, EventArgs e)
        {
            mapControl.SuppressResizeDrawing(true, 0);
        }

        private void MapViewer_ResizeEnd(object sender, EventArgs e)
        {
            mapControl.SuppressResizeDrawing(false, 0);
        }

        private string LoadMapFileInPreviewControl(string fileName)
        {
            string msg = string.Empty;
            if (File.Exists(fileName))
            {
                string ext = Path.GetExtension(fileName).ToLower();
                if (ext == ".mxd")
                {
                    if (mapControl.CheckMxFile(fileName))
                    {
                        try
                        {
                            mapControl.LoadMxFile(fileName);
                            mapControl.Extent = mapControl.FullExtent;
                        }
                        catch (Exception ex)
                        {
                            msg = "ESRI Map Control generated an error.\nFile: " + fileName + "\nError: " + ex;
                        }
                    }
                    else
                        msg = "Map document not valid: " + fileName;
                }
                else
                    if (ext == ".lyr")
                {
                    try
                    {
                        mapControl.ClearLayers();
                        mapControl.SpatialReference = null;
                        mapControl.AddLayerFromFile(fileName);
                        mapControl.get_Layer(0).Visible = true; //Make sure the layer is visible
                                                                //Set the Spatial Ref to match the current layer, not the previous layer.
                                                                //mapControl.SpatialReference = mapControl.get_Layer(0).SpatialReference;
                        mapControl.Extent = mapControl.FullExtent;
                    }
                    catch (Exception ex)
                    {
                        msg = "ESRI Map Control generated an error.\nFile: " + fileName + "\nError: " + ex;
                    }
                }
                else
                    msg = "File must be a map document (.mxd) or a layer file (.lyr): " + fileName;
            }
            else
                msg = "File not found: " + fileName;
            return msg;
        }
    }
}
