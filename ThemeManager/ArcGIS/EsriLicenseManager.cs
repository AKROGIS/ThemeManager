using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using NPS.AKRO.ThemeManager.Properties;
using NPS.AKRO.ThemeManager.UI.Forms;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    internal delegate void LmCallback();

    static class EsriLicenseManager
    {
        //Uncomment the first for Version ArcGIS 9.3, uncomment the second for ArcGIS 10
        //private static EsriLicenseInitializer _AOLicenseInitializer = new EsriLicenseInitializer();
        private static readonly LicenseInitializer _AOLicenseInitializer = new LicenseInitializer();

        public static string Message
        {
            get { return _message; }
            private set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged("Message");
                }
            }
        }
        private static string _message;

        public static bool Running
        {
            get { return _running; }
            private set
            {
                if (_running != value)
                {
                    _running = value;
                    OnPropertyChanged("Running");
                }
            }
        }
        private static bool _running;

        public static bool Start(bool showDialog)
        {
            if (showDialog)
                return StartWithDialog();
            else
                return Start();
        }

        /// <summary>
        /// Checks out ESRI Licenses while showing a modal progress dialog with a cancel button
        /// </summary>
        public static bool StartWithDialog()
        {
            LoadingForm _dialog = new LoadingForm();
            _dialog.Message = "Checking for an ArcGIS license.";
            _dialog.Command = _dialog.LoadLicense;
            _dialog.ShowDialog();
            return Running;
        }

        /// <summary>
        /// Checks out ESRI Product and Extension Licenses in the background.
        /// </summary>
        /// <remarks>
        /// Subscribe to the PropertyChanged event to monitor the status of the Running and
        /// Message property.  If the license is obtained, then Running will change, if a
        /// license cannot be obtained, then Running will not change, but Message will.
        /// If this is called when a license is already obtained, then no messages will be sent.
        /// </remarks>
        public static void StartAsync()
        {
            Thread thread = new Thread(new ThreadStart(PrivateStart));
            thread.Start();
        }

        /// <summary>
        /// Checks out ESRI Product and Extension Licenses.
        /// </summary>
        /// <remarks>
        /// If false is returned, check the Message property to see why.
        /// </remarks>
        /// <returns>Returns true if license was obtained.</returns>
        public static bool Start()
        {
            PrivateStart();
            return Running;
        }

        /// <summary>
        /// Checks out ESRI Product and Extension Licenses.
        /// </summary>
        /// <remarks>
        /// This method is ultimately called by all other start methods.
        /// It contains all the ESRI specific code. and will block until a license is returned,
        /// or the license cannot be obtained.  The time to complete can vary greatly on the speed
        /// of the user's connection to a license server.
        /// </remarks>
        private static void PrivateStart()
        {
            lock (_AOLicenseInitializer) //To protect get/set of Running from multiple threads
            {
                if (!Running)
                {
                    Trace.TraceInformation("{0}: Begin Get ArcGIS License", DateTime.Now); Stopwatch time = Stopwatch.StartNew();
                    //version 10 change:
                    RuntimeManager.Bind(ProductCode.Desktop);

                    _AOLicenseInitializer.InitializeLowerProductFirst = Settings.Default.CheckForArcViewBeforeArcInfo;

                    //Uncomment Additional license/extensions you want to check out
#if ARCGIS_10_1PLUS            
                    esriLicenseProductCode[] products = new[]
                                                            {
                                                                esriLicenseProductCode.esriLicenseProductCodeAdvanced,     //60
                                                                esriLicenseProductCode.esriLicenseProductCodeStandard,   //50
                                                                esriLicenseProductCode.esriLicenseProductCodeBasic,     //40
                                                                //esriLicenseProductCode.esriLicenseProductCodeArcServer,   //30
                                                                //esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB, //20
                                                                //esriLicenseProductCode.esriLicenseProductCodeEngine,      //10
                                                            };
#else
                    esriLicenseProductCode[] products = new[]
                                                            {
                                                                esriLicenseProductCode.esriLicenseProductCodeArcInfo,     //60
                                                                esriLicenseProductCode.esriLicenseProductCodeArcEditor,   //50
                                                                esriLicenseProductCode.esriLicenseProductCodeArcView,     //40
                                                                //esriLicenseProductCode.esriLicenseProductCodeArcServer,   //30
                                                                //esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB, //20
                                                                //esriLicenseProductCode.esriLicenseProductCodeEngine,      //10
                                                            };
#endif
                    esriLicenseExtensionCode[] extensions = new esriLicenseExtensionCode[]
                                                                { 
                                                                    //esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst,
                                                                    //esriLicenseExtensionCode.esriLicenseExtensionCodeCOGO,
                                                                    //esriLicenseExtensionCode.esriLicenseExtensionCodeGeoStats,
                                                                    //esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork,
                                                                    //esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst,
                                                                    //esriLicenseExtensionCode.esriLicenseExtensionCodeSurvey,
                                                                    //...
                                                                };
                    bool initSucceeded = _AOLicenseInitializer.InitializeApplication(products, extensions);

                    if (!initSucceeded)
                    {
                        Message = _AOLicenseInitializer.LicenseMessage();
                        Running = false;
                    }
                    else
                    {
                        Message = null;
                        Running = true;
                    }

                    time.Stop(); Trace.TraceInformation("{0}: End   Get ArcGIS License, total time {1}sec{2}ms", DateTime.Now, time.Elapsed.Seconds, time.Elapsed.Milliseconds);
                }
            }
        }


        /// <summary>
        /// Returns all Checked out licenses and Extensions.
        /// Do not make any call to ArcObjects after ShutDownApplication()
        /// It is safe to call ShutdownApplication() even if checkout was not successful
        /// A license can NOT be re-started once it is stopped.
        /// </summary>
        public static void Stop()
        {
            lock (_AOLicenseInitializer) //To protect get/set of Running from multiple threads
            {
                if (Running)
                {
                    _AOLicenseInitializer.ShutdownApplication();
                    Running = false;
                }
            }
        }

        #region INotifyPropertyChanged

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handle = PropertyChanged;
            if (handle != null)
                handle(null, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
