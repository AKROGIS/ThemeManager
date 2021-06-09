using System.Collections.Generic;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    /// <summary>
    /// A generic interface to GIS data layers. It was originally only used for the ArcObjects data structure in a layer
    /// `*.lyr` file, but was expanded to include ArcObject map documents, and Pro layer `*.lyrx` files. ArcGIS Pro uses
    /// different values for thing like Workspace Type and doesn't use care about some values like `workspaceProgId`
    /// However for compatibility, all Pro values are translated to equivalent ArcObject values when possible.
    /// Blame the ambiguous and similar names on a poor understanding of the ArcObjects data model.
    /// </summary>
    public interface IGisLayer
    {
        /// <summary>
        /// A descriptive label for the GIS Layer as it should appear in the UI
        /// </summary>
        string Name { get; }
        /// <summary>
        /// This is the type of the GIS Layer, not the data in the layer
        /// </summary>
        string DataType { get; }
        /// <summary>
        /// The URL or full path to the data; often Join(WorkspacePath, Container, DataSourceName)
        /// </summary>
        string DataSource { get; }
        /// <summary>
        /// A GIS system specific string to a container of GIS data, could be a URL or file system path, or Database connnection string.
        /// </summary>
        string WorkspacePath { get; }
        /// <summary>
        /// The class name of the workspace, required to query some GIS systems (i.e. ArcObjects)
        /// </summary>
        string WorkspaceProgId { get; }
        /// <summary>
        /// The type of the workspace, used for interpreting the Workspace Path.
        /// </summary>
        string WorkspaceType { get; }
        /// <summary>
        /// A container for data sources in a workspace.  Also known as a feature dataset
        /// </summary>
        string Container { get; }
        /// <summary>
        /// What type of container is this?
        /// </summary>
        string ContainerType { get; }
        /// <summary>
        /// 
        /// </summary>
        string DataSourceName { get; }
        /// <summary>
        /// 
        /// </summary>
        string DataSetName { get; }
        /// <summary>
        /// This is the type of the GIS data in the GIS Layer (could be null for a group layer).
        /// </summary>
        string DataSetType { get; }
        /// <summary>
        /// Is this a "group" GIS Layer; i.e. is it the parent for a set of GIS (sub) Layers
        /// </summary>
        bool IsGroup { get; }
        /// <summary>
        /// A collection of GIS Layers when this GIS Layer is a group
        /// </summary>
        IEnumerable<IGisLayer> SubLayers { get; }

        /// <summary>
        /// Close the map or layer file when done processing it.
        /// FIXME: Do not hold the map or layer file open.
        /// </summary>
        void Close();
    }

    public class GisLayer: IGisLayer
    {
        public GisLayer() { }

        public string Name { get; protected set; }
        public string DataType { get; protected set; }
        public string DataSource { get; protected set; }
        public string WorkspacePath { get; protected set; }
        public string WorkspaceProgId { get; protected set; }
        public string WorkspaceType { get; protected set; }
        public string Container { get; protected set; }
        public string ContainerType { get; protected set; }
        public string DataSourceName { get; protected set; }
        public string DataSetName { get; protected set; }
        public string DataSetType { get; protected set; }
        public bool IsGroup { get; protected set; }
        public virtual IEnumerable<IGisLayer> SubLayers => new GisLayer[] { };

        public virtual void Close() { }
    }
}
