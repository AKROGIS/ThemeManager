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
        /// The name of the container in a workspace that holds the data. Could be the name of a feature dataset
        /// in a geodatabase or coverage, or the name of a raster with multiple bands of data.
        /// This is a property of the data pointed to by a layer. It may be null. The nullity should match ContainerType
        /// It is persisted in the Container property of ThemeData (in the Data property of a TmNode)
        /// It is used in many places in Metadata.cs to Path.Combine with WorkspacePath to look for metadata.
        /// For some types of data, it is part of the DataSource property.
        /// </summary>
        string Container { get; }

        /// <summary>
        /// A string for the type of container.  Historically (from ArcObjects), this was either "RasterDataset" 
        /// or "FeatureDataset".
        /// This is a property of the data pointed to by a layer. It may be null. The nullity should match Container
        /// It is persisted in the ContainerType property of ThemeData (in the Data property of a TmNode)
        /// It is not used by Theme Manager.
        /// </summary>
        string ContainerType { get; }

        /// <summary>
        /// One of three names for the Data. It is unused. See also DataSourceName and Name
        /// Historically (ArcObjects) this is the name property of the layer as an IDataset interface
        /// https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#IDataset.htm
        /// It was typically the same as the Name, but blank for Group layers, Map Frames, and non-GIS themes
        /// It might differ for services, if the data manger edited the name, or if the layer file name was
        /// different than the layer name for a single dataset layer file.
        /// Stating with Pro, this is more commonly the same as the DataSourceName.
        /// This is a property of the layer. It may be null. The nullity should match DataSetType
        /// It is persisted in the DataSetName property of ThemeData (in the Data property of a TmNode)
        /// It is not used by Theme Manager.
        /// </summary>
        string DataSetName { get; }

        /// <summary>
        /// This is a string describing the type of the data in the layer.  It could be null (e.g. group layers) 
        /// 
        /// Historically (in ArcObjects) this was the type of a layer that implemented IDataset. It would be the string value
        /// of one of the enumerations in https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#esriDatasetType.htm
        /// with "esriDT" removed. Starting with support for Pro, the additional types in
        /// https://pro.arcgis.com/en/pro-app/latest/sdk/api-reference/#topic95.html are also recognized.
        /// This is a property of the data pointed to by a layer. It may be null. The nullity should match DataSetName
        /// It is persisted in the DataSetType property of ThemeData (in the Data property of a TmNode)
        /// It is used once in ThemeData.cs to clasify the Data object when the value is "RasterBand".
        /// </summary>
        string DataSetType { get; }

        /// <summary>
        /// This is the "path" to the data in the layer. It is usually a URL or the full path to the data; often
        ///  by combining WorkspacePath, Container, and DataSourceName.
        /// This is a property of the data pointed to by a layer. It may be null.
        /// It is persisted in the DataSource property of ThemeData (in the Data property of a TmNode)
        /// It is presented and editable in the UI as the Data Source.  It is an observed property.
        /// It is used in TMNode to help determine the Iconography if the string starts with "http".
        /// It is used in ThemeData.cs to classify the Data object when the string starts with "http" and ends with one of "/MapServer", "FeatureServer", or "/ImageServer".
        /// It is used in Metadata.cs to find and set the metadata path (in a geodatabase, it is the metadata path)
        /// </summary>
        string DataSource { get; }

        /// <summary>
        /// One of three names for the Data. See also DataSetName and Name
        /// The name (without the workspace) of the data in the layer. It is typically the last component in the DataSource.
        /// Historically (ArcObjects) this is the name property of the IDataLayer layer's DataSourceName (IDatasetName) property
        /// https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#IDatasetName.htm
        /// Starting with Pro it is the DataConnection's DataSet property.  Typically the same as ArcObjects.
        /// This is a property of the data pointed to by a layer. It may be null.
        /// It is persisted in the DataSourceName property of ThemeData (in the Data property of a TmNode)
        /// It is used once in Metadata.cs to remove itself from the DataSource for RasterBand Metadata path.
        /// </summary>
        string DataSourceName { get; }

        /// <summary>
        /// This is a them description: a combination of the "type" of the GIS Layer and the "type" of the data in the layer.
        /// It is a comma separated collection values use to classify the layer.
        /// See the ArcObjects GetLayerDescriptionFromLayer in LayerUtilities.cs to see how it has historically been built.
        /// It may contain an "error" value if the "type" cannot be determined.
        /// It is a property of the layer and the data in the layer. It can be null, but rarely.
        /// It is persisted in the Type property of ThemeData (in the Data property of a TmNode)
        /// It is presented and editable in the UI as the Data Type.  It is an observed property.
        /// It is used in TMNode as the primary source for determining the icon for the theme.
        /// It is used in ThemeData.cs to classify the Data object for determining the metadata path in Metadata.cs.
        /// It is usually classified by seeing if it contains keywords - a really poor strategy.
        /// It is checked in AdminReports.cs for a value of "Error"
        /// </summary>
        string DataType { get; }

        /// <summary>
        /// Is this a "group" GIS Layer; i.e. is it the parent for a set of GIS (sub) Layers
        /// default is false.
        /// This value is only used by ThemeBuilder to create a "Theme" type TmNode for holding other TmNodes.
        /// </summary>
        bool IsGroup { get; }

        /// <summary>
        /// One of three names for the Data. See also DataSetName and DataSourceName
        /// This is name of the layer as it appears in a map's table of contents.
        /// It will be persisted in the Name property on a TmNode.
        /// It will be used in the Theme Manager UI as the theme name.
        /// It is purely descriptive, and not used in any conditional expressions.
        /// The theme list manager can edit the name as needed.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A collection of GIS Layers that are children (sub layers) of this layer
        /// The collection will be empty if IsGroup is true and not empty otherwise.
        /// This is only used by ThemeBuilder to create "Theme" type TmNodes that are children of another "Theme" Node.
        /// Default is an empty list
        /// </summary>
        IEnumerable<IGisLayer> SubLayers { get; }

        /// <summary>
        /// A GIS system specific string to a container of GIS data. It could be a URL, file system path,
        /// a database connnection string, or even some other custom string.
        /// Historically (in ArcObjects) it was a URL or filesystem path. A remote database workspace
        /// was referenced by a file system path to a *.sde file with connection properties.
        /// This is a property of the data pointed to by a layer. It may be null if there is no data
        /// It is persisted in the WorkspacePath property of ThemeData (in the Data property of a TmNode)
        /// It is used in Metadata.cs to build a metadata path when it is not null.
        /// It is used in ThemeData.cs to classify the Data object for determining the metadata path in Metadata.cs.
        /// </summary>
        string WorkspacePath { get; }

        /// <summary>
        /// The string the defines the "meaningful" type of the workspace.
        /// Historically (in ArcObjects) it is the value of the IDatasetName.WorkspaceName.WorkspaceFactoryProgId string.
        /// Typical values are listed in
        /// https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#IWorkspaceName_WorkspaceFactoryProgID.htm
        /// Values may also have a ".1" appended if the OS is 64bit.
        /// I have not been able to find an exhaustive list of WorkspaceFactoryProgId alues, but I tried in the MapFixer
        /// repo at https://github.com/AKROGIS/MapFixer/blob/c7ff2451977bfd400850b8628089f6a6efb44293/MovesDatabase/Moves.cs#L574-L641
        /// Starting With ArcGIS Pro, This may also include the string representation of the WorkspaceFacotry enum
        /// https://github.com/Esri/cim-spec/blob/master/docs/v2/CIMVectorLayers.md#enumeration-workspacefactory
        /// This is a property of the data pointed to by a layer. It may be null if there is no data
        /// It is persisted in the WorkspaceProgId property of ThemeData (in the Data property of a TmNode)
        /// It is used in ThemeData.cs to classify the Data object for determining the metadata path in Metadata.cs.
        /// </summary>
        string WorkspaceProgId { get; }

        /// <summary>
        /// The type of the workspace (e.g. file system, local database, remote database).
        /// Historically (in ArcObjects) it is one of the three text strings from the enumeration in
        /// https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#esriWorkspaceType.htm
        /// This is a property of the data pointed to by a layer. It may be null if there is no data
        /// It is persisted in the WorkspaceType property of ThemeData (in the Data property of a TmNode)
        /// It is not used by Theme Manager.
        /// </summary>
        string WorkspaceType { get; }

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
