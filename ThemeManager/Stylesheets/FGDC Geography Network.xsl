<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/TR/WD-xsl" TYPE="text/javascript">

<!-- An xsl template for displaying metadata in ArcCatalog with the
     look and feel of "View Details" in the Geography Network

     Copyright (c) 2000-2008, Environmental Systems Research Institute, Inc. All rights reserved.
     	
     Revision History: Created 9/12/00 mlyszkiewicz

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
	<xsl:when test="/metadata[($any$ idinfo/(citation/citeinfo/(title | 
		geoform | origin | pubinfo/pubplace | pubdate) | 
		descript/(abstract | purpose | supplinf) | 
		timeperd/timeinfo/(rngdates/* | sngdate/caldate | mdattim/sngdate/caldate) | 
		status/(progress | update) | 
		spdom/bounding/(westbc | eastbc | northbc | southbc) | 
		keywords/(theme/themekey | place/placekey) | 
		accconst | useconst | natvform) | 
		dataqual/lineage/srcinfo/srcscale | 
		spref/horizsys/(geograph | planar/(mapproj/mapprojn | gridsys/gridsysn | 
		localp) | local) != '')]">	


	<!-- Display Content Citation if it exists -->
	<xsl:if test="/metadata/idinfo/citation/citeinfo[($any$ title | geoform | origin | pubinfo/pubplace | pubdate != '')]">
	<TR><TD COLSPAN="2" CLASS="big">Content Citation</TD></TR>

	<!-- Display the title -->
	<xsl:if test="/metadata/idinfo/citation[citeinfo/title != '']">
	<TR>
		<TD WIDTH="1%">&#160;</TD>
		<TD WIDTH="30%"><b>Title of Content:</b></TD>
		<TD WIDTH="70%"><font color="#4682B4"><b>
			<xsl:value-of select="metadata/idinfo/citation/citeinfo/title"/></b></font>
		</TD>
	</TR>
	</xsl:if>

	<!-- Display the type of content -->
	<xsl:if test="/metadata/distinfo/resdesc[(. != '') and ((. = 'Downloadable Data') or (. = 'Live Data and Maps') or (. = 'Map Files') or (. = 'Offline Data') or (. = 'Static Map Images') or (. = 'Other Documents') or (. = 'Applications') or (. = 'Geographic Services') or (. = 'Clearinghouses') or (. = 'Geographic Activities') )]">
	<TR>
	<TD>&#160;</TD>
	<TD><b>Type of Content:</b></TD>
	<TD>
		<xsl:for-each select="metadata/distinfo/resdesc">	
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>
	
	<!-- Display the publisher -->
	<xsl:if test="/metadata/idinfo/citation[citeinfo/origin != '']">
	<TR>
	<TD>&#160;</TD>
	<TD><b>Content Publisher:</b></TD>
	<TD>
		<xsl:for-each select="metadata/idinfo/citation/citeinfo/origin">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the publishing place -->
	<xsl:if test="/metadata/idinfo/citation[citeinfo/pubinfo/pubplace != '']">
	<TR>
	<TD>&#160;</TD>
	<TD><b>Publication Place:</b></TD>
	<TD>
		<xsl:for-each select="metadata/idinfo/citation/citeinfo/pubinfo/pubplace">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the date of publishing -->
	<xsl:if test="/metadata/idinfo/citation[citeinfo/pubdate != '']">
	<TR>
	<TD>&#160;</TD>
	<TD><b>Publication Date:</b></TD>
	<TD>
		<xsl:for-each select="metadata/idinfo/citation/citeinfo/pubdate">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Content Citation -->

	
	<!-- Display Content Description if it exists -->
	<xsl:if test="/metadata/idinfo/descript[($any$ abstract | purpose | supplinf != '')]">
	
	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Content Description</TD></TR>

	<!-- Display the abstract -->
	<xsl:if test="/metadata/idinfo[descript/abstract != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top" colspan="2"><b>Content Summary:</b>
		<xsl:for-each select="metadata/idinfo/descript/abstract">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the purpose -->
	<xsl:if test="/metadata/idinfo[descript/purpose != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top" colspan="2"><b>Content Purpose:</b>
		<xsl:for-each select="metadata/idinfo/descript/purpose">	
			<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>
	
	<!-- Display the Supplemental information -->
	<xsl:if test="/metadata/idinfo[descript/supplinf != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top" colspan="2"><b>Supplemental Information:</b>
	
		<xsl:for-each select="metadata/idinfo/descript/supplinf">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Content Description -->


	<!-- Display Time Period of Content if it exists -->
	<xsl:if test="/metadata/idinfo/timeperd/timeinfo[($any$ rngdates/begdate | rngdates/enddate != '')
			or (sngdate/caldate != '') or (mdattim/sngdate/caldate != '') ]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Time Period of Content</TD></TR>

	<!-- check to see if the dates are a single date, multiple dates, or a range -->
	<xsl:choose>
	
	<!-- Display a single date -->
	<xsl:when test="/metadata/idinfo/timeperd/timeinfo[sngdate/caldate != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Date:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/timeperd/timeinfo/sngdate/caldate">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:when>
	
	<xsl:otherwise>
	
	<xsl:choose>
	
	<!-- Display multiple dates -->
	<xsl:when test="/metadata/idinfo/timeperd/timeinfo/mdattim[sngdate/caldate != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Date:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/timeperd/timeinfo/mdattim/sngdate">
		<xsl:value-of select="caldate" /><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:when>
	<xsl:otherwise>
	
	<!-- Display a range of dates (the beginning date) -->
	<xsl:if test="/metadata/idinfo/timeperd/timeinfo[rngdates/begdate != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Beginning Date:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/timeperd/timeinfo/rngdates/begdate">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>
	
	<!-- the ending date of the range -->
	<xsl:if test="/metadata/idinfo/timeperd/timeinfo[rngdates/enddate != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Ending Date:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/timeperd/timeinfo/rngdates/enddate">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>
	</xsl:otherwise>
	</xsl:choose>
	</xsl:otherwise>
	</xsl:choose>
	
	</xsl:if> <!-- End Time Period Of Content -->


	<!-- Display Content Status if it exists -->
	<xsl:if test="/metadata/idinfo/status[($any$ progress | update != '')]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Content Status</TD></TR>

	<!-- Display the progress -->	
	<xsl:if test="/metadata/idinfo[status/progress != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Progress:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/status/progress">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the update frequency -->
	<xsl:if test="/metadata/idinfo[status/update != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Update Frequency:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/status/update">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Content Status -->


	<!-- Display Spatial Domain if it exists -->
	<xsl:if test="/metadata/idinfo/spdom/bounding[($any$ westbc | eastbc | northbc | southbc != '')
		or (keywords/place/placekey != '')]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Spatial Domain</TD></TR>

	<!-- Display the west coordinate -->
	<xsl:if test="/metadata/idinfo/spdom[bounding/westbc != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>West Coordinate:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/spdom/bounding/westbc">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the east coordinate -->
	<xsl:if test="/metadata/idinfo/spdom[bounding/eastbc != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>East Coordinate:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/spdom/bounding/eastbc">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the north coordinate -->
	<xsl:if test="/metadata/idinfo/spdom[bounding/northbc != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>North Coordinate:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/spdom/bounding/northbc">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the south coordinate -->
	<xsl:if test="/metadata/idinfo/spdom[bounding/southbc != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>South Coordinate:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/spdom/bounding/southbc">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the place/area -->
	<xsl:if test="/metadata/idinfo/keywords[place/placekey != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Coverage Area:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/keywords/place/placekey">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Spatial Domain -->


	<!-- Display Content Keywords if it exists -->
	<xsl:if test="/metadata/idinfo/keywords[(theme/themekey != '') or (place/placekey != '')]">


	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Content Keywords</TD></TR>

	<!-- Display theme keywords -->
	<xsl:if test="/metadata/idinfo/keywords[theme/themekey != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Theme Keywords:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/keywords/theme">
			<xsl:for-each select="themekey">
				<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
			</xsl:for-each>
			<xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display place keywords -->
	<xsl:if test="/metadata/idinfo/keywords[place/placekey != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Place Keywords:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/keywords/place">
			<xsl:for-each select="placekey">
				<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
			</xsl:for-each>
			<xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if>  <!-- End Content Keywords -->

	
	<!-- Display Spatial Data Information if it exists -->

	<xsl:if test="/metadata[($any$ idinfo/(citation/citeinfo/geoform | natvform) | 
		spref/horizsys/(geograph | planar/(mapproj/mapprojn | gridsys/gridsysn | 
		localp) | local) != '')]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Spatial Data Information</TD></TR>

	<!-- Display the data type -->
	<xsl:if test="/metadata/idinfo/citation[citeinfo/geoform != '']">
	<TR>
	<TD>&#160;</TD>
	<TD><b>Data Type:</b></TD>
	<TD>
		<xsl:for-each select="metadata/idinfo/citation/citeinfo/geoform">	
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the data format -->
	<xsl:if test="/metadata[idinfo/natvform != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Data Format:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/natvform">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display the coordinate system if it is specified under the following elements:
			 geograph, mapproj, gridsys, localp or local -->
	<xsl:if test="/metadata/spref/horizsys[(geograph != '') or 
			(planar/mapproj/mapprojn != '') or 
			(planar/gridsys/gridsysn != '') or 
			(planar/localp != '') or (local != '')]">	
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Data Projection:</b></TD> 
	<TD valign="top">
	
		<!-- Display geographic coordinate systems -->
		<xsl:for-each select="metadata/spref/horizsys/geograph">
			Geographic<xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
		
		<!-- Separate projections specified under different metadata elements by a comma -->
		<xsl:if test="metadata/spref/horizsys[ (geograph != '') and 
			((planar/mapproj/mapprojn != '') or 
			(planar/gridsys/gridsysn != '') or (planar/localp != '') or 
			(local != ''))]">, </xsl:if>

		<!-- Display projections -->
		<xsl:for-each select="metadata/spref/horizsys/planar">	
			<!-- Display projections defined under the mapproj element -->
			<xsl:for-each select="mapproj/mapprojn">
				<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
			</xsl:for-each>

			<!-- Separate projections specified under different metadata elements by a comma -->
			<xsl:if test="context()[(mapproj/mapprojn != '') and ((gridsys/gridsysn != '') or 
				(localp != ''))]">, </xsl:if>

			<!-- Display projections defined under the gridsys element -->		
			<xsl:for-each select="gridsys">
				<xsl:value-of select="gridsysn" /><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
			</xsl:for-each>

			<!-- Separate projections specified under different metadata elements by a comma -->
			<xsl:if test="context()[((mapproj/mapprojn != '') or (gridsys/gridsysn != '')) 
				and (localp != '')]">, </xsl:if>
			
			<!-- Display projections defined under the localp element -->
			<xsl:for-each select="localp">
				Local Planar Coordinate System<xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
			</xsl:for-each>

			<xsl:if test="context()[not(end()) and . != '']">, </xsl:if>		
		</xsl:for-each>
		
		<!-- Separate projections specified under different metadata elements by a comma -->
		<xsl:if test="metadata/spref/horizsys[ ((geograph != '') or 
			(planar/mapproj/mapprojn != '') or (planar/gridsys/gridsysn != '') or 
			(planar/localp != '')) and (local != '')]">, </xsl:if>
		
		<!-- Display local coordinate systems -->
		<xsl:for-each select="metadata/spref/horizsys/local">
			Local Coordinate System<xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>

	</xsl:if>
	
	</xsl:if>  <!-- End Spatial Data Information -->


	<!-- Display the source scale -->
	<xsl:if test="/metadata/dataqual/lineage[srcinfo/srcscale != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Data Scale:</b></TD>
	<TD valign="top">
		<xsl:value-of select="metadata/dataqual/lineage/srcinfo/srcscale"/>
	</TD>
	</TR>
	</xsl:if>


	<!-- Display Access and Usage Information if it exists -->
	<xsl:if test="/metadata/idinfo[($any$ accconst | useconst != '')]">

	<TR><TD>&#160;</TD></TR>
	<TR><TD COLSPAN="2" CLASS="big">Access and Usage Information</TD></TR>

	<!-- Display access constraints -->
	<xsl:if test="/metadata[idinfo/accconst != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Access Constraints:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/accconst">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	<!-- Display usage constraints -->
	<xsl:if test="/metadata[idinfo/useconst != '']">
	<TR>
	<TD>&#160;</TD>
	<TD valign="top"><b>Use Constraints:</b></TD>
	<TD valign="top">
		<xsl:for-each select="metadata/idinfo/useconst">
		<xsl:eval>this.text</xsl:eval><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
		</xsl:for-each>
	</TD>
	</TR>
	</xsl:if>

	</xsl:if> <!-- End Access and Usage Information -->


	</xsl:when> <!-- End showing information that this stylesheet can display -->


	<!-- If the document does not contain information that this stylesheet can display, 
		show a nice message -->
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

	<!-- <BR/><BR/><BR/><CENTER><FONT COLOR="#6495ED">Metadata stylesheets are provided courtesy of ESRI.  Copyright (c) 2000-2004, Environmental Systems Research Institute, Inc.  All rights reserved.</FONT></CENTER> -->

  </BODY>
  </HTML>
</xsl:template>


</xsl:stylesheet>