<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/REC-html40">
	<!-- AKSO XSL template for displaying metadata in Theme Manager.
     Created: 2/4/04 Greg Daniels, NPS AKSO.-->
	<!-- The "parameter" below is a variable that points to the directory where
     ArcGIS has been installed. The NPS Metadata Extension will try to
     automatically change this value. If for some reason that doesn't work, you
		 can change the value in the 'select=' to where Arc8 is installed on your
		 system. For example: "'C:\arcgis\arcexe82'" -->
	<xsl:param name="arcInstallDir" select="'C:\arcgis\arcexe83'"/>
	<!-- begin templates -->
	<xsl:template match="/">
		<HTML>
			<HEAD>
				<STYLE>
			blockquote{margin-top:0.0in;margin-bottom:0.0in;margin-left:0.25in}
    	body      {font-size:11pt; font-family:Arial,sans-serif}
      .title    {margin-left:0.00in; position:relative; top:0; text-align:left;
                font-size:20; font-family:Arial,Verdana,sans-serif;
                color:#570303}
      .name     {margin-left:0.00in; position:relative; top:0; text-align:left;
                font-weight:bold; font-size:11; font-family:Verdana,sans-serif;
								color:#000000}
			.sub			{margin-left:0.15in; position:relative; top:0; text-align:left;
                font-weight:bold; font-size:11; font-family:Verdana,sans-serif;
								color:#000000}
      .value    {margin-left:0.05in; position:relative; top:0; text-align:left;
                font-size:11; font-family:Verdana,sans-serif; color:#000000;
								margin-bottom: 4}
			.attrib   {text-indent:0.25in; position:relative; top:0; text-align:left;
                font-weight:bold; font-style:italic; font-size:11;
								font-family:Verdana,sans-serif; color:#000000}
			.titlebar			{background-color:#f5f0dd;
								border-right:solid 0px Black;
								border-left:solid 0px Black;
								border-bottom:solid 0px Black;
								border-top:solid 0px Black;
								padding:5px 5px 5px 5px;
								}
			.main			{background-color:#ffffff;
								border-right:solid 0px #d6d5d0;
								border-left:solid 0px #d6d5d0;
								border-bottom:solid 0px #d6d5d0;
								padding:5px 5px 5px 5px;
    </STYLE>
			</HEAD>
			<BODY link="#666600" vlink="#999900" alink="#FF6666" >
				<table  width="500" cellpadding="0" cellspacing="0" border="0" >
					<tr>
						<td colspan="2" class="header">
							<img border="0" src="AKSO_header.GIF" alt="National Park Service, U.S. Department of the Interior"  height="95" width="500"/>
						</td>
					</tr>
					<tr>
						<td class="titlebar">
							<DIV CLASS="title">
        							 Title: <xsl:value-of select="metadata/idinfo/citation/citeinfo/title"/>
							</DIV>
						</td>
					</tr>
					<tr align="left">
						<td width="700" class="main" >
							<DIV CLASS="value">
								<SPAN CLASS="name">Abstract: </SPAN>
								<xsl:value-of select="metadata/idinfo/descript/abstract"/>
							</DIV>
							<DIV CLASS="value">
								<SPAN CLASS="name">Purpose: </SPAN>
								<xsl:value-of select="metadata/idinfo/descript/purpose"/>
							</DIV>
							<DIV CLASS="value">
								<SPAN CLASS="name">Supplemental: </SPAN>
								<xsl:value-of select="metadata/idinfo/descript/supplinf"/>
							</DIV>
							<DIV CLASS="value">
								<SPAN CLASS="name">Publication Date: </SPAN>
								<xsl:value-of select="metadata/idinfo/citation/citeinfo/pubdate"/>
							</DIV>				
							<DIV CLASS="value">
								<SPAN CLASS="name">Originator: </SPAN>
								<xsl:value-of select="metadata/idinfo/citation/citeinfo/origin"/>
							</DIV>
							<xsl:if test="metadata/idinfo/citation/citeinfo/onlink[. != '']">
								<DIV CLASS="value">
									<SPAN CLASS="name">Online citation: </SPAN>
									<xsl:value-of select="metadata/idinfo/citation/citeinfo/onlink"/>
								</DIV>
							</xsl:if>
							<DIV CLASS="value">
								<SPAN CLASS="name">Bounding coordinates:</SPAN>
								<br/>
								<SPAN CLASS="sub">West: </SPAN>
								<xsl:value-of select="metadata/idinfo/spdom/bounding/westbc"/>
								<br/>
								<SPAN CLASS="sub">East: </SPAN>
								<xsl:value-of select="metadata/idinfo/spdom/bounding/eastbc"/>
								<br/>
								<SPAN CLASS="sub">North: </SPAN>
								<xsl:value-of select="metadata/idinfo/spdom/bounding/northbc"/>
								<br/>
								<SPAN CLASS="sub">South: </SPAN>
								<xsl:value-of select="metadata/idinfo/spdom/bounding/southbc"/>
							</DIV>
							<DIV CLASS="value">
								<SPAN CLASS="name">Place Keywords: </SPAN>
								<xsl:for-each select="metadata/idinfo/keywords/place/placekey">
									<xsl:value-of select="."/>
									<xsl:if test="position()!=last()">, </xsl:if>
								</xsl:for-each>
							</DIV>
							<DIV CLASS="value">
								<SPAN CLASS="name">Theme Keywords: </SPAN>
								<xsl:for-each select="metadata/idinfo/keywords/theme/themekey">
									<xsl:value-of select="."/>
									<xsl:if test="position()!=last()">, </xsl:if>
								</xsl:for-each>
							</DIV>
							<xsl:if test="metadata/spref/horizsys/cordsysn/projcsn[. != '']">
								<DIV CLASS="value">
									<SPAN CLASS="name">Projection: </SPAN>
									<xsl:value-of select="metadata/spref/horizsys/cordsysn/projcsn"/>
								</DIV>
							</xsl:if>
							<xsl:if test="metadata/spref/horizsys/cordsysn/geogcsn[. != '']">
								<DIV CLASS="value">
									<SPAN CLASS="name">Geographic: </SPAN>
									<xsl:value-of select="metadata/spref/horizsys/cordsysn/geogcsn"/>
								</DIV>
							</xsl:if>
							<!-- Dataset Attributes: repeats Attribute Template for each file -->
							<xsl:for-each select="metadata/eainfo/detailed[attr]">
								<DIV CLASS="value">
									<SPAN CLASS="name">Attributes of: </SPAN>
									<xsl:value-of select="enttyp/enttypl"/>
								</DIV>
								<xsl:apply-templates select="attr"/>
							</xsl:for-each>
							<DIV CLASS="value">
								<SPAN CLASS="name">Metadata date: </SPAN>
								<xsl:value-of select="metadata/metainfo/metd"/>
							</DIV>
							<p/>
							<p/>
						</td>
					</tr>
					<tr>
						<td width="500">
							<font size="1"  color="Black" face="Arial, Helvetica, sans-serif"><i>AKSO Theme Manager Metadata Stylesheet</i></font>
						</td>
					</tr>
				</table>
			</BODY>
		</HTML>
	</xsl:template>
	<!-- Attribute Template -->
	<xsl:template match="metadata/eainfo/detailed/attr">
		<!-- Attribute Name -->
		<DIV CLASS="attrib">
			<xsl:choose>
				<xsl:when test="attrlabl[. != '']">
					<xsl:value-of select="attrlabl"/>
				</xsl:when>
				<xsl:otherwise>
    		Attribute
    	</xsl:otherwise>
			</xsl:choose>
		</DIV>
		<!-- Attribute Properties -->
		<blockquote>
			<DIV CLASS="value">
				<xsl:if test="attalias[. != '']">
					<SPAN CLASS="sub">Alias: </SPAN>
					<xsl:value-of select="attalias"/>
					<br/>
				</xsl:if>
				<xsl:if test="attrtype[. != '']">
					<SPAN CLASS="sub">Data type: </SPAN>
					<xsl:value-of select="attrtype"/>
					<br/>
				</xsl:if>
				<xsl:if test="attwidth[. != '']">
					<SPAN CLASS="sub">Width: </SPAN>
					<xsl:value-of select="attwidth"/>
					<br/>
				</xsl:if>
				<xsl:if test="atoutwid[. != '']">
					<SPAN CLASS="sub">Display width: </SPAN>
					<xsl:value-of select="atoutwid"/>
					<br/>
				</xsl:if>
				<xsl:if test="atprecis[. != '']">
					<SPAN CLASS="sub">Precision: </SPAN>
					<xsl:value-of select="atprecis"/>
					<br/>
				</xsl:if>
				<xsl:if test="attscale[. != '']">
					<SPAN CLASS="sub">Scale: </SPAN>
					<xsl:value-of select="attscale"/>
					<br/>
				</xsl:if>
				<xsl:if test="atnumdec[. != '']">
					<SPAN CLASS="sub">Number of decimals: </SPAN>
					<xsl:value-of select="atnumdec"/>
					<br/>
				</xsl:if>
				<xsl:if test="attrdef[. != '']">
					<SPAN CLASS="sub">Description: </SPAN>
					<xsl:value-of select="attrdef"/>
				</xsl:if>
			</DIV>
		</blockquote>
	</xsl:template>
</xsl:stylesheet>
