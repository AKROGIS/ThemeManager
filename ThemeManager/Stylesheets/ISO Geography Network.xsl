<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/TR/WD-xsl" TYPE="text/javascript">

<!-- An xsl template for displaying ISO metadata in ArcCatalog with the
     look and feel of "View Details" in the Geography Network

     Copyright (c) 2001-2008, Environmental Systems Research Institute, Inc. All rights reserved.
     	
     Revision History: Created 10/22/01 avienneau

-->

<xsl:template match="/">
  <HTML>
  <HEAD>
	<STYLE>
	  BODY {font-size:10pt; font-family:Verdana,Arial,sans-serif}
	  TD   {font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 9pt; color: #333333}  
	  .big   {  font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 11pt; color: #333333; font-weight: bold}
	</STYLE>
  </HEAD>

  <BODY ONCONTEXTMENU="return true">
	<TABLE BORDER="0" CELLPADDING="2" CELLSPACING="2">
	
	<!-- If the document contains information that this stylesheet can display, show it -->
	<xsl:choose>
	<xsl:when test="/metadata[($any$ dataIdInfo[0]/(idCitation/(resTitle | 
		citRespParty[role/RoleCd/@value='010']/rpIndName | 
		citRespParty[role/RoleCd/@value='010']/rpOrgName | 
		citRespParty[role/RoleCd/@value='010']/rpCntInfo/cntAddress/city | 
		resRefDate[refDateType/DateTypCd/@value='002']/refDate | 
		presForm/PresFormCd/@value) | 
		idAbs | idPurp | suppInfo | dataScale/equScale/rfDenom | 
		geoBox/(westBL | eastBL | northBL | southBL) | 
		descKeys[(keyTyp/KeyTypCd/@value = '005') or (keyTyp/KeyTypCd/@value = '002')]/keyword | 
		resConst/LegConsts/(accessConsts/RestrictCd/@value | useConsts/RestrictCd/@value) | 
		idStatus/ProgCd/@value | resMaint/maintFreq/MaintFreqCd/@value | 
		dataExt/tempEle/*/exTemp/TM_GeometricPrimitive/(*/begin | */end | *//calDate)) | 
		distInfo/distributor/(distorTran/onLineSrc/orDesc | distorFormat/formatName) | 
		refSysInfo/*/refSysID/identCode | 
		spatRepInfo/*/axDimProps/Dimen/dimResol/value != '')]">
	
	
	<!-- Display Content Citation if it exists -->
	<xsl:if test="/metadata[($any$ dataIdInfo[0]/idCitation/(resTitle | 
		citRespParty[role/RoleCd/@value='010']/rpIndName | 
		citRespParty[role/RoleCd/@value='010']/rpOrgName | 
		citRespParty[role/RoleCd/@value='010']/rpCntInfo/cntAddress/city | 
		resRefDate[refDateType/DateTypCd/@value='002']/refDate) | 
		distInfo/distributor/distorTran/onLineSrc/orDesc != '')]">
	<TR><TD COLSPAN="2" CLASS="big">Content Citation</TD></TR>

	<!-- Display the title -->
	<xsl:if test="/metadata[dataIdInfo[0]/idCitation/resTitle/text()]">
	<TR>
		<TD WIDTH="1%">&#160;</TD>
		<TD WIDTH="30%"><b>Title of Content:</b></TD>
		<TD WIDTH="70%"><font color="#4682B4"><b>
			<xsl:value-of select="/metadata/dataIdInfo[0]/idCitation/resTitle[text()]"/></b></font>
		</TD>
	</TR>
	</xsl:if>

	<!-- Display the type of content -->
	<xsl:if test="/metadata[distInfo/distributor/distorTran/onLineSrc/orDesc/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD><b>Type of Content:</b></TD>
	<TD>
		<xsl:for-each select="/metadata/distInfo/distributor/distorTran/onLineSrc[orDesc/text()]">
			<xsl:for-each select="orDesc[text()]">
				<xsl:choose>
				<xsl:when test="context()[. = '001']">Live Data and Maps</xsl:when>
				<xsl:when test="context()[. = '002']">Downloadable Data</xsl:when>
				<xsl:when test="context()[. = '003']">Offline Data</xsl:when>
				<xsl:when test="context()[. = '004']">Static Map Images</xsl:when>
				<xsl:when test="context()[. = '005']">Other Documents</xsl:when>
				<xsl:when test="context()[. = '006']">Applications</xsl:when>
				<xsl:when test="context()[. = '007']">Geographic Services</xsl:when>
				<xsl:when test="context()[. = '008']">Clearinghouses</xsl:when>
				<xsl:when test="context()[. = '009']">Map Files</xsl:when>
				<xsl:when test="context()[. = '010']">Geographic Activities</xsl:when>
				<xsl:otherwise><xsl:value-of /></xsl:otherwise>
				</xsl:choose>
			</xsl:for-each><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>
	
	<!-- Display the publisher -->
	<xsl:if test="/metadata/dataIdInfo[0]/idCitation[citRespParty/role/RoleCd/@value='010'][(citRespParty/rpIndName/text()) or (citRespParty/rpOrgName/text())]">
	<TR>
	<TD>&#160;</TD>
	<TD><b>Content Publisher:</b></TD>
	<TD>
		<xsl:for-each select="/metadata/dataIdInfo[0]/idCitation/citRespParty[role/RoleCd/@value='010'][(rpIndName/text()) or (rpOrgName/text())]">
			<xsl:value-of select="rpIndName[text()]"/><xsl:if test="context()[(rpIndName != '') and (rpOrgName != '')]">, </xsl:if>
			<xsl:value-of select="rpOrgName[text()]"/><xsl:if test="context()[not(end())]">; </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the publishing place -->
	<xsl:if test="/metadata/dataIdInfo[0]/idCitation[citRespParty/role/RoleCd/@value='010'][citRespParty/rpCntInfo/cntAddress/city/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD><b>Publication Place:</b></TD>
	<TD>
		<xsl:for-each select="/metadata/dataIdInfo[0]/idCitation/citRespParty[role/RoleCd/@value='010'][rpCntInfo/cntAddress/city/text()]">
			<xsl:value-of select="rpCntInfo/cntAddress/city[text()]"/><xsl:if test="context()[not(end())]">; </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the date of publishing -->
	<xsl:if test="/metadata/dataIdInfo[0]/idCitation/resRefDate[refDateType/DateTypCd/@value='002'][refDate/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD><b>Publication Date:</b></TD>
	<TD>
		<xsl:for-each select="/metadata/dataIdInfo[0]/idCitation/resRefDate[refDateType/DateTypCd/@value='002'][refDate/text()]">
			<xsl:value-of select="refDate[text()]"/><xsl:if test="context()[not(end())]">; </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Content Citation -->

	
	<!-- Display Content Description if it exists -->
	<xsl:if test="/metadata/dataIdInfo[0][($any$ idAbs | idPurp | suppInfo != '')]">
	
	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Content Description</TD></TR>

	<!-- Display the abstract -->
	<xsl:if test="/metadata/dataIdInfo[0][idAbs/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top" colspan="2"><b>Content Summary:</b>
		<xsl:value-of select="/metadata/dataIdInfo[0]/idAbs[text()]"/>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the purpose -->
	<xsl:if test="/metadata/dataIdInfo[0][idPurp/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top" colspan="2"><b>Content Purpose:</b>
		<xsl:value-of select="metadata/dataIdInfo[0]/idPurp[text()]"/>
	</TD>
	</TR>
	</xsl:if>
	
	<!-- Display the Supplemental information -->
	<xsl:if test="/metadata/dataIdInfo[0][suppInfo/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top" colspan="2"><b>Supplemental Information:</b>
		<xsl:value-of select="metadata/dataIdInfo[0]/suppInfo[text()]"/>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Content Description -->


	<!-- Display Time Period of Content if it exists -->
	<xsl:if test="/metadata/dataIdInfo[0][($any$ dataExt/tempEle/*/exTemp/TM_GeometricPrimitive/(*/begin | */end | *//calDate) != '')]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Time Period of Content</TD></TR>

	<!-- Display a single date -->
	<xsl:if test="/metadata/dataIdInfo[0]/dataExt/tempEle[*//calDate/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Date:</b></TD>
	<TD valign="top">
		<xsl:for-each select="/metadata/dataIdInfo[0]/dataExt/tempEle[*/exTemp/TM_GeometricPrimitive/TM_Instant/tmPosition/*/calDate/text()]">
			<xsl:value-of select="*/exTemp/TM_GeometricPrimitive/TM_Instant/tmPosition/*/calDate[text()]"/><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>
	
	<!-- Display a range of dates -->
	<xsl:for-each select="/metadata/dataIdInfo[0]/dataExt/tempEle[($any$ */exTemp/TM_GeometricPrimitive/TM_Period/(begin | end) != '')]">
	
	<!-- the beginning date of the range -->
	<xsl:if test="*/exTemp/TM_GeometricPrimitive/TM_Period[begin/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Beginning Date:</b></TD>
	<TD valign="top">
		<xsl:for-each select="*/exTemp/TM_GeometricPrimitive/TM_Period/begin[text()]">
			<xsl:value-of /><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>
	
	<!-- the ending date of the range -->
	<xsl:if test="*/exTemp/TM_GeometricPrimitive/TM_Period[end/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Ending Date:</b></TD>
	<TD valign="top">
		<xsl:for-each select="*/exTemp/TM_GeometricPrimitive/TM_Period/end[text()]">
			<xsl:value-of /><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>
	
	</xsl:for-each>
		
	</xsl:if> <!-- End Time Period Of Content -->


	<!-- Display Content Status if it exists -->
	<xsl:if test="/metadata/dataIdInfo[0][($any$ idStatus/ProgCd/@value | resMaint/maintFreq/MaintFreqCd/@value != '')]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Content Status</TD></TR>

	<!-- Display the progress -->	
	<xsl:if test="/metadata/dataIdInfo[0][idStatus/ProgCd/@value/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Progress:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/dataIdInfo[0]/idStatus[ProgCd/@value/text()]">
			<xsl:for-each select="ProgCd[@value/text()]">
				<xsl:choose>
				<xsl:when test="context()[@value = '001']">completed</xsl:when>
				<xsl:when test="context()[@value = '002']">historical archive</xsl:when>
				<xsl:when test="context()[@value = '003']">obsolete</xsl:when>
				<xsl:when test="context()[@value = '004']">on-going</xsl:when>
				<xsl:when test="context()[@value = '005']">planned</xsl:when>
				<xsl:when test="context()[@value = '006']">required</xsl:when>
				<xsl:when test="context()[@value = '007']">under development</xsl:when>
				<xsl:otherwise><xsl:value-of select="@value"/></xsl:otherwise>
				</xsl:choose>
			</xsl:for-each><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the update frequency -->
	<xsl:if test="/metadata/dataIdInfo[0][resMaint/maintFreq/MaintFreqCd/@value/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Update Frequency:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/dataIdInfo[0]/resMaint[maintFreq/MaintFreqCd/@value/text()]">
			<xsl:for-each select="maintFreq/MaintFreqCd[@value/text()]">
				<xsl:choose>
				<xsl:when test="context()[@value = '001']">continual</xsl:when>
				<xsl:when test="context()[@value = '002']">daily</xsl:when>
				<xsl:when test="context()[@value = '003']">weekly</xsl:when>
				<xsl:when test="context()[@value = '004']">fortnightly</xsl:when>
				<xsl:when test="context()[@value = '005']">monthly</xsl:when>
				<xsl:when test="context()[@value = '006']">quarterly</xsl:when>
				<xsl:when test="context()[@value = '007']">biannually</xsl:when>
				<xsl:when test="context()[@value = '008']">annually</xsl:when>
				<xsl:when test="context()[@value = '009']">as needed</xsl:when>
				<xsl:when test="context()[@value = '010']">irregular</xsl:when>
				<xsl:when test="context()[@value = '011']">not planned</xsl:when>
				<xsl:when test="context()[@value = '998']">unknown</xsl:when>
				<xsl:otherwise><xsl:value-of select="@value"/></xsl:otherwise>
				</xsl:choose>
			</xsl:for-each><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Content Status -->


	<!-- Display Spatial Domain if it exists -->
	<xsl:if test="/metadata/dataIdInfo[0][($any$ geoBox/(westBL | eastBL | northBL | southBL) != '')
		or (descKeys[keyTyp/KeyTypCd/@value = '002']/keyword/text())]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Spatial Domain</TD></TR>
	
	<!-- Get the first geographic extent that contains the dataset -->
	<xsl:for-each select="/metadata/dataIdInfo[0]/geoBox[(exTypeCode = 1) and (westBL &gt; -180) and (eastBL &lt; 180) and (northBL &lt; 90) and (southBL &gt; -90)][0]">

	<!-- Display the west coordinate -->
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>West Coordinate:</b></TD>
	<TD valign="top">
		<xsl:value-of select="westBL"/>
	</TD>
	</TR>

	<!-- Display the east coordinate -->
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>East Coordinate:</b></TD>
	<TD valign="top">
		<xsl:value-of select="eastBL"/>
	</TD>
	</TR>

	<!-- Display the north coordinate -->
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>North Coordinate:</b></TD>
	<TD valign="top">
		<xsl:value-of select="northBL"/>
	</TD>
	</TR>

	<!-- Display the south coordinate -->
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>South Coordinate:</b></TD>
	<TD valign="top">
		<xsl:value-of select="southBL"/>
	</TD>
	</TR>

	</xsl:for-each> <!-- end geographic extent -->

	<!-- Display the place/area -->
	<xsl:if test="/metadata/dataIdInfo[0][descKeys[keyTyp/KeyTypCd/@value = '002']/keyword/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Coverage Area:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/dataIdInfo[0]/descKeys[keyTyp/KeyTypCd/@value = '002'][keyword/text()]">
			<xsl:for-each select="keyword[text()]">
				<xsl:value-of /><xsl:if test="context()[not(end())]">, </xsl:if>
			</xsl:for-each>
			<xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Spatial Domain -->


	<!-- Display Content Keywords if they exist -->
	<xsl:if test="/metadata/dataIdInfo[0][descKeys[(keyTyp/KeyTypCd/@value = '005') or (keyTyp/KeyTypCd/@value = '002')]/keyword/text()]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Content Keywords</TD></TR>

	<!-- Display theme keywords -->
	<xsl:if test="/metadata/dataIdInfo[0]/descKeys[keyTyp/KeyTypCd/@value = '005'][keyword/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Theme Keywords:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/dataIdInfo[0]/descKeys[keyTyp/KeyTypCd/@value = '005'][keyword/text()]">
			<xsl:for-each select="keyword[text()]">
				<xsl:value-of /><xsl:if test="context()[not(end())]">, </xsl:if>
			</xsl:for-each>
			<xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display place keywords -->
	<xsl:if test="/metadata/dataIdInfo[0]/descKeys[keyTyp/KeyTypCd/@value = '002'][keyword/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Place Keywords:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/dataIdInfo[0]/descKeys[keyTyp/KeyTypCd/@value = '002'][keyword/text()]">
			<xsl:for-each select="keyword[text()]">
				<xsl:value-of /><xsl:if test="context()[not(end())]">, </xsl:if>
			</xsl:for-each>
			<xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if>  <!-- End Content Keywords -->

	
	<!-- Display Spatial Data Information if it exists -->
	<xsl:if test="/metadata[(dataIdInfo[0]/(idCitation/presForm/PresFormCd/@value | 
		dataScale/equScale/rfDenom) | distInfo/distributor/distorFormat/formatName | 
		refSysInfo/*/refSysID/identCode | spatRepInfo/*/axDimProps/Dimen/dimResol/value != '')]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Spatial Data Information</TD></TR>

	<!-- Display the data type -->
	<xsl:if test="/metadata/dataIdInfo[0]/idCitation[presForm/PresFormCd/@value/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD><b>Data Type:</b></TD>
	<TD>
		<xsl:for-each select="metadata/dataIdInfo[0]/idCitation/presForm[PresFormCd/@value/text()]">	
			<xsl:for-each select="PresFormCd">
				<xsl:choose>
				<xsl:when test="context()[@value = '001']">digital document</xsl:when>
				<xsl:when test="context()[@value = '002']">hardcopy document</xsl:when>
				<xsl:when test="context()[@value = '003']">digital image</xsl:when>
				<xsl:when test="context()[@value = '004']">hardcopy image</xsl:when>
				<xsl:when test="context()[@value = '005']">digital map</xsl:when>
				<xsl:when test="context()[@value = '006']">hardcopy map</xsl:when>
				<xsl:when test="context()[@value = '007']">digital model</xsl:when>
				<xsl:when test="context()[@value = '008']">hardcopy model</xsl:when>
				<xsl:when test="context()[@value = '009']">digital profile</xsl:when>
				<xsl:when test="context()[@value = '010']">hardcopy profile</xsl:when>
				<xsl:when test="context()[@value = '011']">digital table</xsl:when>
				<xsl:when test="context()[@value = '012']">hardcopy table</xsl:when>
				<xsl:when test="context()[@value = '013']">digital video</xsl:when>
				<xsl:when test="context()[@value = '014']">hardcopy video</xsl:when>
				<xsl:otherwise><xsl:value-of select="@value"/></xsl:otherwise>
				</xsl:choose>
			</xsl:for-each><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the data format -->
	<xsl:if test="/metadata[distInfo/distributor/distorFormat/formatName/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Data Format:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/distInfo/distributor[distorFormat/formatName/text()]">
			<xsl:for-each select="distorFormat[formatName/text()]">
				<xsl:value-of select="formatName[text()]"/><xsl:if test="context()[not(end())]">, </xsl:if>
			</xsl:for-each><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the reference system name -->
	<xsl:if test="/metadata[refSysInfo/*/refSysID/identCode/text()]">	
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Data Projection:</b></TD> 
	<TD valign="top">
		<xsl:for-each select="metadata/refSysInfo[*/refSysID/identCode/text()]">	
			<xsl:value-of select="*/refSysID/identCode[text()]"/><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the source scale -->
	<xsl:if test="/metadata/dataIdInfo[0][dataScale/equScale/rfDenom/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Data Scale:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/dataIdInfo[0]/dataScale[equScale/rfDenom/text()]">	
			<xsl:value-of select="equScale/rfDenom"/><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the resolution -->
	<xsl:if test="/metadata/spatRepInfo/*/axDimProps/Dimen/dimResol/value/text()">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Resolution:</b></TD>
	<TD valign="top">
		X Axis - <xsl:value-of select="metadata/spatRepInfo/*/axDimProps/Dimen[dimName/DimNameTypCd/@value = '002']/dimResol/value"/>,
		Y Axis - <xsl:value-of select="metadata/spatRepInfo/*/axDimProps/Dimen[dimName/DimNameTypCd/@value = '001']/dimResol/value"/>,
		Units - <xsl:value-of select="metadata/spatRepInfo/*/axDimProps/Dimen[dimName/DimNameTypCd/@value = '002']/dimResol/uom/UomLength/uomName"/>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Spatial Data Information -->


	<!-- Display Access and Usage Information if it exists -->
	<xsl:if test="/metadata/dataIdInfo[0][($any$ resConst/LegConsts/(accessConsts/RestrictCd/@value | 
		useConsts/RestrictCd/@value) != '')]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Access and Usage Information</TD></TR>

	<!-- Display access constraints -->
	<xsl:if test="/metadata/dataIdInfo[0][resConst/LegConsts/accessConsts/RestrictCd/@value/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Access Constraints:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/dataIdInfo[0]/resConst[LegConsts/accessConsts/RestrictCd/@value/text()]">
			<xsl:for-each select="LegConsts/accessConsts[RestrictCd/@value/text()]">
				<xsl:for-each select="RestrictCd">
					<xsl:choose>
					<xsl:when test="context()[@value = '001']">copyright</xsl:when>
					<xsl:when test="context()[@value = '002']">patent</xsl:when>
					<xsl:when test="context()[@value = '003']">patent pending</xsl:when>
					<xsl:when test="context()[@value = '004']">trademark</xsl:when>
					<xsl:when test="context()[@value = '005']">license</xsl:when>
					<xsl:when test="context()[@value = '006']">intellectual property rights</xsl:when>
					<xsl:when test="context()[@value = '007']">restricted</xsl:when>
					<xsl:when test="context()[@value = '008']">other restrictions</xsl:when>
					<xsl:otherwise><xsl:value-of select="@value"/></xsl:otherwise>
					</xsl:choose>
				</xsl:for-each><xsl:if test="context()[not(end())]">, </xsl:if>
			</xsl:for-each><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display usage constraints -->
	<xsl:if test="/metadata/dataIdInfo[0][resConst/LegConsts/useConsts/RestrictCd/@value/text()]">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Use Constraints:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/dataIdInfo[0]/resConst[LegConsts/useConsts/RestrictCd/@value/text()]">
			<xsl:for-each select="LegConsts/useConsts[RestrictCd/@value/text()]">
				<xsl:for-each select="RestrictCd">
					<xsl:choose>
					<xsl:when test="context()[@value = '001']">copyright</xsl:when>
					<xsl:when test="context()[@value = '002']">patent</xsl:when>
					<xsl:when test="context()[@value = '003']">patent pending</xsl:when>
					<xsl:when test="context()[@value = '004']">trademark</xsl:when>
					<xsl:when test="context()[@value = '005']">license</xsl:when>
					<xsl:when test="context()[@value = '006']">intellectual property rights</xsl:when>
					<xsl:when test="context()[@value = '007']">restricted</xsl:when>
					<xsl:when test="context()[@value = '008']">other restrictions</xsl:when>
					<xsl:otherwise><xsl:value-of select="@value"/></xsl:otherwise>
					</xsl:choose>
				</xsl:for-each><xsl:if test="context()[not(end())]">, </xsl:if>
			</xsl:for-each><xsl:if test="context()[not(end())]">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Access and Usage Information -->


	</xsl:when> <!-- End showing information that this stylesheet can display -->


	<!-- If the document does not contain information that this stylesheet can display, show a nice message -->
	<xsl:otherwise> 
		<TR><TD COLSPAN="2" CLASS="big">&#160;</TD></TR>
		<TR>
		<TD WIDTH="15%">&#160;</TD>
		<TD WIDTH="70%" align="center">
			The selected document does not contain information that can be displayed with this stylesheet.
		</TD>
		<TD WIDTH="15%">&#160;</TD>
		</TR>
	</xsl:otherwise> 
	</xsl:choose> 

	</TABLE>


	<!-- <BR/><BR/><BR/><CENTER><FONT COLOR="#6495ED">Metadata stylesheets are provided courtesy of ESRI.  Copyright (c) 2001-2004, Environmental Systems Research Institute, Inc.  All rights reserved.</FONT></CENTER> -->

  </BODY>
  </HTML>
</xsl:template>


</xsl:stylesheet>