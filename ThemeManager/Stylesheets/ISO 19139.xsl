<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:gmd="http://www.isotc211.org/2005/gmd"
	xmlns:gco="http://www.isotc211.org/2005/gco"
	xmlns:gml="http://www.opengis.net/gml">

<!-- An xsl template for displaying ISO 19139 metadata in ArcCatalog 
     with similar style to "FGDC" and "ISO" style sheets. Shows only
     ISO 19139 standard elements. Will not show any FGDC or 
     ESRI-ISO elements, or any ESRI-defined content such as 
	 thumbnails or enclosures, or any FGDC-defined content such as
	 attribute descriptions.

     Copyright (c) 2006-2008, Environmental Systems Research Institute, Inc. All rights reserved.
	
     Revision History: Created 3/10/06 jcupp
		Revised: 8/25/06 avienneau
		Revised: 3/21/08 avienneau - added or fixed display of dataSetURI, spatialResolution, datasetLanguage,
			 discipline keywords, and security handling description elements
-->

<xsl:template match="/">
  <HTML>
  <HEAD>
    <SCRIPT LANGUAGE="JScript"><xsl:comment><![CDATA[

function test() {
  var ua = window.navigator.userAgent
  var msie = ua.indexOf ( "MSIE " )
  if ( msie == -1 ) 
    document.write("<P>" + "Netscape")
}

      function fix(e) {
        var par = e.parentNode;
        e.id = "";
        e.style.marginLeft = "0.42in";
        var pos = e.innerText.indexOf("\n");
        if (pos > 0) {
          while (pos > 0) {
            var t = e.childNodes(0);
            var n = document.createElement("PRE");
            var s = t.splitText(pos);
            e.insertAdjacentElement("afterEnd", n);
            n.appendChild(s);
            n.style.marginLeft = "0.42in";
            e = n;
            pos = e.innerText.indexOf("\n");
          }
          var count = (par.children.length);
          for (var i = 0; i < count; i++) {
            e = par.children(i);
            if (e.tagName == "PRE") {
              pos = e.innerText.indexOf(">");
              if (pos != 0) {
                n = document.createElement("DD");
                e.insertAdjacentElement("afterEnd", n);
                n.innerText = e.innerText;
                e.removeNode(true);
              }
            }
          }
          if (par.children.tags("PRE").length > 0) {
            count = (par.children.length);
            for (i = 0; i < count; i++) {
              e = par.children(i);
              if (e.tagName == "PRE") {
                e.id = "";
                if (i < (count-1)) {
                  var e2 = par.children(i + 1);
                  if (e2.tagName == "PRE") {
                    e.insertAdjacentText("beforeEnd", e2.innerText+"\n");
                    e2.removeNode(true);
                    count = count-1;
                    i = i-1;
                  }
                }
              }
            }
          }
        }
        else {
          n = document.createElement("DD");
          par.appendChild(n);
          n.innerText = e.innerText;
          e.removeNode(true);
        }
      }

    ]]></xsl:comment></SCRIPT>
		
		<STYLE><xsl:comment><![CDATA[
		
		BODY { font-family: "verdana", sans-serif ; color: #00008B; font-size: 10pt; background-color: white; }
		H1 {font-size: medium; font-weight: bold; }
		H2 {font-size: small; font-weight: bold; color: #0000AA; }
		.header { color: #006400; background-color: #CCFFCC; font-size: 10pt; }
		
		]]></xsl:comment></STYLE>
		
  </HEAD>
   
  <BODY ONCONTEXTMENU="return true">
	
  <!-- SHOW METADATA SUMMARY -->
  
  <!-- summary doesn't include natvform and file name - ESRI extended elements
        uses distribution format and location instead -->
				
  <xsl:variable name="title" select="/gmd:MD_Metadata/gmd:identificationInfo[1]/gmd:MD_DataIdentification/gmd:citation/gmd:CI_Citation/gmd:title"/>
  <xsl:variable name="refSysID" select="/gmd:MD_Metadata/gmd:referenceSystemInfo[1]/gmd:MD_ReferenceSystem/gmd:referenceSystemIdentifier/gmd:RS_Identifier/gmd:code"/>
  <xsl:variable name="abstract" select="/gmd:MD_Metadata/gmd:identificationInfo[1]/gmd:MD_DataIdentification/gmd:abstract"/>
  <xsl:variable name="theme-keywords" select="/gmd:MD_Metadata/gmd:identificationInfo[1]/gmd:MD_DataIdentification/gmd:descriptiveKeywords/gmd:MD_Keywords[gmd:type/gmd:MD_KeywordTypeCode = '002']/gmd:keyword"/>
  <xsl:variable name="linkage1" select="/gmd:MD_Metadata/gmd:distributionInfo/gmd:MD_Distribution/gmd:transferOptions/gmd:MD_DigitalTransferOptions/gmd:onLine/gmd:CI_OnlineResource/gmd:linkage"/>
  <xsl:variable name="linkage2" select="/gmd:MD_Metadata/gmd:distributionInfo/gmd:MD_Distribution/gmd:distributor/gmd:MD_Distributor/gmd:distributorTransferOptions/gmd:MD_DigitalTransferOptions/gmd:onLine/gmd:CI_OnlineResource/gmd:linkage"/>
  <xsl:variable name="format1" select="/gmd:MD_Metadata/gmd:distributionInfo/gmd:MD_Distribution/gmd:distributionFormat/gmd:MD_Format/gmd:name"/>
  <xsl:variable name="format2" select="/gmd:MD_Metadata/gmd:distributionInfo/gmd:MD_Distribution/gmd:distributor/gmd:MD_Distributor/gmd:distributorFormat/gmd:MD_Format/gmd:name"/>
  
	<xsl:if test="$title | $refSysID | $abstract | $theme-keywords | $linkage1 | $linkage2 | $format1 | $format2 ">

    <TABLE class="header" COLS="2" WIDTH="100%" CELLPADDING="24" BORDER="0" CELLSPACING="0">
	<TR ALIGN="left" VALIGN="top">
	<TD  COLSPAN="2">

		<!-- title -->
		<xsl:for-each select="$title[1]">
			<H1><xsl:call-template name="CharacterString"/></H1>
		</xsl:for-each>
	
		<!-- title -->
		<xsl:if test="($format1 != '') or ($format2 != '')">
			<P>
				<B>Data format(s):</B>
				<xsl:for-each select="$format1"><xsl:call-template name="CharacterString"/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
				<xsl:if test="($format1 != '') and ($format2 != '')">, </xsl:if>
				<xsl:for-each select="$format2"><xsl:call-template name="CharacterString"/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
			</P>
			<!-- FIXME: nowhere to get/put the image format
			<xsl:if test="/metadata[(idinfo/natvform = 'Raster Dataset') 
				and (spdoinfo/rastinfo/rastifor/text())]">
					- <xsl:value-of select="/metadata/spdoinfo/rastinfo/rastifor" />
			</xsl:if>-->
		</xsl:if>

		<!-- coordinate system -->
		<xsl:if test="$refSysID">
			<P>
				<B>Coordinate system:</B> 
				<xsl:for-each select="$refSysID">
					<xsl:call-template name="CharacterString"/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
			</P>
		</xsl:if>
						
		<!-- theme keywords -->
		<xsl:if test="$theme-keywords">
		  <P><B>Theme keyword(s):</B>
			<xsl:for-each select="$theme-keywords">
				<xsl:call-template name="CharacterString"/>
				<xsl:if test="not(position()=last())">, </xsl:if>
			</xsl:for-each>
		  </P>
		</xsl:if>
	
		<!-- online linkage -->
		<xsl:if test="($linkage1 != '') or ($linkage2 != '')">
			<P>
				<B>Location(s):</B>
				<xsl:for-each select="$linkage1"><xsl:value-of select="."/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
				<xsl:if test="($linkage1 != '') and ($linkage2 != '')">, </xsl:if>
				<xsl:for-each select="$linkage2"><xsl:value-of select="."/>
					<xsl:if test="not(position()=last())">, </xsl:if>
				</xsl:for-each>
			</P>
		</xsl:if>
	
		<!-- abstract -->
		<xsl:if test="$abstract">
			<P>
			  <xsl:for-each select="$abstract">
				<B>Abstract:</B><xsl:call-template name="CharacterString"/>
			  </xsl:for-each>
			</P>
		</xsl:if>
		  
	</TD>
	</TR>
    </TABLE>
  </xsl:if>


  <!-- BUILD THE TOC  -->

  <A name="Top"/>
  <H2>ISO-19139 Metadata:</H2>

	<xsl:variable name="metadata-sections" select="
		/gmd:MD_Metadata/gmd:fileIdentifier |
		/gmd:MD_Metadata/gmd:language |
		/gmd:MD_Metadata/gmd:characterSet |
		/gmd:MD_Metadata/gmd:parentIdentifier |
		/gmd:MD_Metadata/gmd:hierarchyLevel |
		/gmd:MD_Metadata/gmd:hierarchyLevelName |
		/gmd:MD_Metadata/gmd:contact |
		/gmd:MD_Metadata/gmd:dateStamp |
		/gmd:MD_Metadata/gmd:metadataStandardName |
		/gmd:MD_Metadata/gmd:metadataStandardVersion |
		/gmd:MD_Metadata/gmd:dataSetURI |
		/gmd:MD_Metadata/gmd:metadataMaintenance |
		/gmd:MD_Metadata/gmd:metadataConstraints"/>
	
	<xsl:variable name="dataIdInfo" select="/gmd:MD_Metadata/gmd:identificationInfo"/>
	<xsl:variable name="spatRepInfo" select="/gmd:MD_Metadata/gmd:spatialRepresentationInfo"/>
	<xsl:variable name="contInfo" select="/gmd:MD_Metadata/gmd:contentInfo"/>
	<xsl:variable name="refSysInfo" select="/gmd:MD_Metadata/gmd:referenceSystemInfo"/>
	<xsl:variable name="dqInfo" select="/gmd:MD_Metadata/gmd:dataQualityInfo"/>
	<xsl:variable name="porCatInfo" select="/gmd:MD_Metadata/gmd:portrayalCatalogueInfo"/>
	<xsl:variable name="distInfo" select="/gmd:MD_Metadata/gmd:distributionInfo"/>
	<xsl:variable name="appSchInfo" select="/gmd:MD_Metadata/gmd:applicationSchemaInfo"/>
	<xsl:variable name="mdExtInfo" select="/gmd:MD_Metadata/gmd:metadataExtensionInfo"/>
		
  <UL>
    <!-- Metadata Identification -->
    <!-- Root node "metadata" will always exist. Only add to TOC if it contains elements
          that describe the metadata. -->
			
    <xsl:if test="count($metadata-sections) &gt; 0">
			<LI><A HREF="#Metadata_Information">Metadata Information</A></LI>
    </xsl:if>

    <!-- Resource Identification -->
    <!-- DIS version of the DTD doesn't account for data and service subclasses of MD_Identification. 
          This template assumes service elements may appear in metadata/dataIdInfo even though
          those elements aren't in the DTD at all. If subclasses were included, the structure would be
          similar to spatial representation. -->
					
		<xsl:call-template name="TOC-HEADING">
			<xsl:with-param name="nodes" select="$dataIdInfo"/>
			<xsl:with-param name="label">Resource Identification Information</xsl:with-param>
			<xsl:with-param name="sub-label">Resource</xsl:with-param>
		</xsl:call-template>

    <!-- Spatial Representation Information  -->
		<xsl:call-template name="TOC-HEADING">
			<xsl:with-param name="nodes" select="$spatRepInfo"/>
			<xsl:with-param name="label">Spatial Representation Information</xsl:with-param>
			<xsl:with-param name="sub-label">Representation</xsl:with-param>
		</xsl:call-template>

    <!-- Content Information  -->
		<xsl:call-template name="TOC-HEADING">
			<xsl:with-param name="nodes" select="$contInfo"/>
			<xsl:with-param name="label">Content Information</xsl:with-param>
			<xsl:with-param name="sub-label">Description</xsl:with-param>
		</xsl:call-template>

    <!-- Reference System Information  -->
		<xsl:call-template name="TOC-HEADING">
			<xsl:with-param name="nodes" select="$refSysInfo"/>
			<xsl:with-param name="label">Reference System Information</xsl:with-param>
			<xsl:with-param name="sub-label">Reference System</xsl:with-param>
		</xsl:call-template>
		
    <!-- Data Quality Information -->
		<xsl:call-template name="TOC-HEADING">
			<xsl:with-param name="nodes" select="$dqInfo"/>
			<xsl:with-param name="label">Data Quality Information</xsl:with-param>
			<xsl:with-param name="sub-label">Data Quality</xsl:with-param>
		</xsl:call-template>

    <!-- Distribution Information  -->
		<xsl:call-template name="TOC-HEADING">
			<xsl:with-param name="nodes" select="$distInfo"/>
			<xsl:with-param name="label">Distribution Information</xsl:with-param>
			<xsl:with-param name="sub-label">Distribution</xsl:with-param>
		</xsl:call-template>

    <!-- Portrayal Catalogue Reference  -->
		<xsl:call-template name="TOC-HEADING">
			<xsl:with-param name="nodes" select="$porCatInfo"/>
			<xsl:with-param name="label">Portrayal Catalogue Reference</xsl:with-param>
			<xsl:with-param name="sub-label">Catalogue</xsl:with-param>
		</xsl:call-template>

    <!-- Application Schema Information  -->
		<xsl:call-template name="TOC-HEADING">
			<xsl:with-param name="nodes" select="$appSchInfo"/>
			<xsl:with-param name="label">Application Schema Information</xsl:with-param>
			<xsl:with-param name="sub-label">Schema</xsl:with-param>
		</xsl:call-template>
		
    <!-- Metadata Extension Information  -->
		<xsl:call-template name="TOC-HEADING">
			<xsl:with-param name="nodes" select="$mdExtInfo"/>
			<xsl:with-param name="label">Metadata Extension Information</xsl:with-param>
			<xsl:with-param name="sub-label">Extension</xsl:with-param>
		</xsl:call-template>

    </UL>

		<!-- PUT METADATA CONTENT ON THE HTML PAGE  -->

    <!-- Metadata Information -->
    <!-- Root node "metadata" will always exist. Only apply template if it contains elements
          that describe the metadata. -->
    <xsl:if test="count($metadata-sections) &gt; 0">
      <xsl:apply-templates select="gmd:MD_Metadata"/>
    </xsl:if>

    <!-- Resource Identification -->
    <xsl:apply-templates select="$dataIdInfo/*"/>

    <!-- Spatial Representation Information -->
    <xsl:apply-templates select="$spatRepInfo/*"/>

    <!-- Content Information -->
    <xsl:apply-templates select="$contInfo"/> <!-- NOTE: special case, see template -->

    <!-- Reference System Information -->
    <xsl:apply-templates select="$refSysInfo/*"/>

    <!-- Data Quality Information -->
    <xsl:apply-templates select="$dqInfo/*"/>

    <!-- Distribution Information -->
    <xsl:apply-templates select="$distInfo/*"/>

    <!-- Portrayal Catalogue Reference -->
    <xsl:apply-templates select="$porCatInfo/*"/>

    <!-- Application Schema Information -->
    <xsl:apply-templates select="$appSchInfo/*"/>

    <!-- Metadata Extension Information -->
    <xsl:apply-templates select="$mdExtInfo/*"/>

   <!-- <BR/><BR/><BR/><CENTER><FONT COLOR="#6495ED">Metadata stylesheets are provided courtesy of ESRI.  Copyright (c) 2001-2004, Environmental Systems Research Institute, Inc.  All rights reserved.</FONT></CENTER> -->

  </BODY>
  </HTML>
</xsl:template>

<!-- Generic template for displaying the TOC headings and links -->
<xsl:template name="TOC-HEADING">
	<xsl:param name="nodes"/>
	<xsl:param name="label"/>
	<xsl:param name="sub-label"/>
	
	<xsl:if test="count($nodes) = 1">
		<xsl:for-each select="$nodes">
			<LI><A>
				<xsl:attribute name="HREF">#<xsl:value-of select="generate-id(./*[1])"/></xsl:attribute>
				<xsl:value-of select="$label"/>
			</A></LI>
		</xsl:for-each>
	</xsl:if>
	<xsl:if test="count($nodes) &gt; 1">
		<LI><xsl:value-of select="$label"/></LI>
		<xsl:for-each select="$nodes">
			<LI STYLE="margin-left:0.5in"><A>
				<xsl:attribute name="HREF">#<xsl:value-of select="generate-id(./*[1])"/></xsl:attribute>
				<xsl:value-of select="$sub-label"/>&#x20;<xsl:value-of select="position()"/>
			</A></LI>
		</xsl:for-each>
	</xsl:if>
	
</xsl:template>

<!-- TEMPLATES FOR METADATA UML CLASSES -->

<!-- Metadata Information (B.2.1 MD_Metadata - line 1) -->
<xsl:template match="gmd:MD_Metadata">
  <A name="Metadata_Information"><HR/></A>
  <DL>
  <DT><H2>Metadata Information:</H2></DT>
  <DL>
  <DD>
    <xsl:for-each select="gmd:language">
      <DT><B>Metadata language:</B>
        <xsl:call-template name="CharacterString"/>
      </DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:characterSet/gmd:MD_CharacterSetCode">
      <DT><B>Metadata character set:</B>
        <xsl:call-template name="AnyCode"/>
      </DT>
    </xsl:for-each>
    <xsl:if test="gmd:language | gmd:characterSet"><BR/><BR/></xsl:if>
    
    <xsl:for-each select="gmd:dateStamp">
      <DT><B>Last update:</B> <xsl:call-template name="Date_PropertyType"/></DT>
    </xsl:for-each>
    <xsl:apply-templates select="gmd:metadataMaintenance"/>
    <xsl:if test="gmd:dateStamp | gmd:metadataMaintenance"><BR/><BR/></xsl:if>

    <xsl:for-each select="gmd:metadataConstraints">
      <DT><B>Metadata constraints:</B></DT>
      <DL>
				<!-- 
						NOTE: will match sub-class templates:
						MD_LegalConstraints, MD_SecurityConstraints
				-->
        <xsl:apply-templates select="*"/>
      </DL>
    </xsl:for-each>

    <xsl:apply-templates select="gmd:contact/gmd:CI_ResponsibleParty"/>
    
    <xsl:for-each select="gmd:hierarchyLevel/gmd:MD_ScopeCode">
      <DT><B>Scope of the data described by the metadata:</B>
        <xsl:call-template name="AnyCode" />
      </DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:hierarchyLevelName">
      <DT><B>Scope name:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:hierarchyLevel | gmd:hierarchyLevelName"><BR/><BR/></xsl:if>
 
    <xsl:for-each select="gmd:metadataStandardName">
      <DT><B>Name of the metadata standard used:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:metadataStandardVersion">
      <DT><B>Version of the metadata standard:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:metadataStandardName | gmd:metadataStandardVersion"><BR/><BR/></xsl:if>

    <xsl:for-each select="gmd:fileIdentifier">
      <DT><B>Metadata identifier:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:parentIdentifier">
      <DT><B>Parent identifier:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:dataSetURI">
      <DT><B>URI of the data described by the metadata:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:fileIdentifier | gmd:parentIdentifier | gmd:dataSetURI"><BR/><BR/></xsl:if>
  </DD>
  </DL>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

<!-- 2 letter language code list from ISO 639 : 1988, in alphabetic order by code -->
<xsl:template match="language">
	<xsl:for-each select="gco:CharacterString">
    <xsl:choose>
        <xsl:when test="(.='aa')">Afar</xsl:when>
        <xsl:when test="(.='ab')">Abkhazian</xsl:when>
        <xsl:when test="(.='af')">Afrikaans</xsl:when>
        <xsl:when test="(.='am')">Amharic</xsl:when>
        <xsl:when test="(.='ar')">Arabic</xsl:when>
        <xsl:when test="(.='as')">Assamese</xsl:when>
        <xsl:when test="(.='ay')">Aymara</xsl:when>
        <xsl:when test="(.='az')">Azerbaijani</xsl:when>
        
        <xsl:when test="(.='ba')">Bashkir</xsl:when>
        <xsl:when test="(.='be')">Byelorussian</xsl:when>
        <xsl:when test="(.='bg')">Bulgarian</xsl:when>
        <xsl:when test="(.='bh')">Bihari</xsl:when>
        <xsl:when test="(.='bi')">Bislama</xsl:when>
        <xsl:when test="(.='bn')">Bengali, Bangla</xsl:when>
        <xsl:when test="(.='bo')">Tibetan</xsl:when>
        <xsl:when test="(.='br')">Breton</xsl:when>
        
        <xsl:when test="(.='ca')">Catalan</xsl:when>
        <xsl:when test="(.='co')">Corsican</xsl:when>
        <xsl:when test="(.='cs')">Czech</xsl:when>
        <xsl:when test="(.='cy')">Welsh</xsl:when>
        
        <xsl:when test="(.='da')">Danish</xsl:when>
        <xsl:when test="(.='de')">German</xsl:when>
        <xsl:when test="(.='dz')">Bhutani</xsl:when>
        
        <xsl:when test="(.='el')">Greek</xsl:when>
        <xsl:when test="(.='en')">English</xsl:when>
        <xsl:when test="(.='eo')">Esperanto</xsl:when>
        <xsl:when test="(.='es')">Spanish</xsl:when>
        <xsl:when test="(.='et')">Estonian</xsl:when>
        <xsl:when test="(.='eu')">Basque</xsl:when>
        
        <xsl:when test="(.='fa')">Persian</xsl:when>
        <xsl:when test="(.='fi')">Finnish</xsl:when>
        <xsl:when test="(.='fj')">Fiji</xsl:when>
        <xsl:when test="(.='fo')">Faroese</xsl:when>
        <xsl:when test="(.='fr')">French</xsl:when>
        <xsl:when test="(.='fy')">Frisian</xsl:when>
        
        <xsl:when test="(.='ga')">Irish</xsl:when>
        <xsl:when test="(.='gd')">Scots Gaelic</xsl:when>
        <xsl:when test="(.='gl')">Galician</xsl:when>
        <xsl:when test="(.='gn')">Guarani</xsl:when>
        <xsl:when test="(.='gu')">Gujarati</xsl:when>
        
        <xsl:when test="(.='ha')">Hausa</xsl:when>
        <xsl:when test="(.='hi')">Hindi</xsl:when>
        <xsl:when test="(.='hr')">Croatian</xsl:when>
        <xsl:when test="(.='hu')">Hungarian</xsl:when>
        <xsl:when test="(.='hy')">Armenian</xsl:when>
        
        <xsl:when test="(.='ia')">Interlingua</xsl:when>
        <xsl:when test="(.='ie')">Interlingue</xsl:when>
        <xsl:when test="(.='ik')">Inupiak</xsl:when>
        <xsl:when test="(.='in')">Indonesian</xsl:when>
        <xsl:when test="(.='is')">Icelandic</xsl:when>
        <xsl:when test="(.='it')">Italian</xsl:when>
        <xsl:when test="(.='iw')">Hebrew</xsl:when>
        
        <xsl:when test="(.='ja')">Japanese</xsl:when>
        <xsl:when test="(.='ji')">Yiddish</xsl:when>
        <xsl:when test="(.='jw')">Javanese</xsl:when>
        
        <xsl:when test="(.='ka')">Georgian</xsl:when>
        <xsl:when test="(.='kk')">Kazakh</xsl:when>
        <xsl:when test="(.='kl')">Greenlandic</xsl:when>
        <xsl:when test="(.='km')">Cambodian</xsl:when>
        <xsl:when test="(.='kn')">Kannada</xsl:when>
        <xsl:when test="(.='ko')">Korean</xsl:when>
        <xsl:when test="(.='ks')">Kashmiri</xsl:when>
        <xsl:when test="(.='ku')">Kurdish</xsl:when>
        <xsl:when test="(.='ky')">Kirghiz</xsl:when>
        
        <xsl:when test="(.='la')">Latin</xsl:when>
        <xsl:when test="(.='ln')">Lingala</xsl:when>
        <xsl:when test="(.='lo')">Laothian</xsl:when>
        <xsl:when test="(.='lt')">Lithuanian</xsl:when>
        <xsl:when test="(.='lv')">Latvian, Lettish</xsl:when>
        
        <xsl:when test="(.='mg')">Malagasy</xsl:when>
        <xsl:when test="(.='mi')">Maori</xsl:when>
        <xsl:when test="(.='mk')">Macedonian</xsl:when>
        <xsl:when test="(.='ml')">Malayalam</xsl:when>
        <xsl:when test="(.='mn')">Mongolian</xsl:when>
        <xsl:when test="(.='mo')">Moldavian</xsl:when>
        <xsl:when test="(.='mr')">Marathi</xsl:when>
        <xsl:when test="(.='ms')">Malay</xsl:when>
        <xsl:when test="(.='mt')">Maltese</xsl:when>
        <xsl:when test="(.='my')">Burmese</xsl:when>
        
        <xsl:when test="(.='na')">Nauru</xsl:when>
        <xsl:when test="(.='ne')">Nepali</xsl:when>
        <xsl:when test="(.='nl')">Dutch</xsl:when>
        <xsl:when test="(.='no')">Norwegian</xsl:when>
        
        <xsl:when test="(.='oc')">Occitan</xsl:when>
        <xsl:when test="(.='om')">(Afan) Oromo</xsl:when>
        <xsl:when test="(.='or')">Oriya</xsl:when>
        
        <xsl:when test="(.='pa')">Punjabi</xsl:when>
        <xsl:when test="(.='pl')">Polish</xsl:when>
        <xsl:when test="(.='ps')">Pashto, Pushto</xsl:when>
        <xsl:when test="(.='pt')">Portugese</xsl:when>
        
        <xsl:when test="(.='qu')">Quechua</xsl:when>
        
        <xsl:when test="(.='rm')">Rhaeto-Romance</xsl:when>
        <xsl:when test="(.='rn')">Kirundi</xsl:when>
        <xsl:when test="(.='ro')">Romanian</xsl:when>
        <xsl:when test="(.='ru')">Russian</xsl:when>
        <xsl:when test="(.='rw')">Kinyarwanda</xsl:when>
        
        <xsl:when test="(.='sa')">Sanskrit</xsl:when>
        <xsl:when test="(.='sd')">Sindhi</xsl:when>
        <xsl:when test="(.='sg')">Sangho</xsl:when>
        <xsl:when test="(.='sh')">Serbo-Croatian</xsl:when>
        <xsl:when test="(.='si')">Singhalese</xsl:when>
        <xsl:when test="(.='sk')">Slovak</xsl:when>
        <xsl:when test="(.='sl')">Slovenian</xsl:when>
        <xsl:when test="(.='sm')">Samoan</xsl:when>
        <xsl:when test="(.='sn')">Shona</xsl:when>
        <xsl:when test="(.='so')">Somali</xsl:when>
        <xsl:when test="(.='sq')">Albanian</xsl:when>
        <xsl:when test="(.='sr')">Serbian</xsl:when>
        <xsl:when test="(.='ss')">Siswati</xsl:when>
        <xsl:when test="(.='st')">Sesotho</xsl:when>
        <xsl:when test="(.='su')">Sundanese</xsl:when>
        <xsl:when test="(.='sv')">Swedish</xsl:when>
        <xsl:when test="(.='sw')">Swahili</xsl:when>
        
        <xsl:when test="(.='ta')">Tamil</xsl:when>
        <xsl:when test="(.='te')">Telugu</xsl:when>
        <xsl:when test="(.='tg')">Tajik</xsl:when>
        <xsl:when test="(.='th')">Thai</xsl:when>
        <xsl:when test="(.='ti')">Tigrinya</xsl:when>
        <xsl:when test="(.='tk')">Turkmen</xsl:when>
        <xsl:when test="(.='tl')">Tagalog</xsl:when>
        <xsl:when test="(.='tn')">Setswana</xsl:when>
        <xsl:when test="(.='to')">Tonga</xsl:when>
        <xsl:when test="(.='tr')">Turkish</xsl:when>
        <xsl:when test="(.='ts')">Tsonga</xsl:when>
        <xsl:when test="(.='tt')">Tatar</xsl:when>
        <xsl:when test="(.='tw')">Twi</xsl:when>
        
        <xsl:when test="(.='uk')">Ukrainian</xsl:when>
        <xsl:when test="(.='ur')">Urdu</xsl:when>
        <xsl:when test="(.='uz')">Uzbek</xsl:when>
        
        <xsl:when test="(.='vi')">Vietnamese</xsl:when>
        <xsl:when test="(.='vo')">Volapuk</xsl:when>
        
        <xsl:when test="(.='wo')">Wolof</xsl:when>
        
        <xsl:when test="(.='xh')">Xhosa</xsl:when>
        
        <xsl:when test="(.='yo')">Yoruba</xsl:when>
        
        <xsl:when test="(.='zh')">Chinese</xsl:when>
        <xsl:when test="(.='zu')">Zulu</xsl:when>
        
        <xsl:otherwise><xsl:value-of select="."/></xsl:otherwise>
    </xsl:choose>
	</xsl:for-each>
</xsl:template>

<!-- Scope code list (B.2.25 MD_ScopeCode) -->
<xsl:template match="gmd:MD_ScopeCode">
	<xsl:call-template name="AnyCode"/>
</xsl:template>

<!-- Maintenance Information (B.2.5 MD_MaintenanceInformation - line142) -->
<xsl:template match="gmd:MD_MaintenanceInformation"> <!-- match="(mdMaint | resMaint)"> -->
    <DD>
    <xsl:choose>
      <xsl:when test="../gmd:resourceMaintenance">
        <DT><B>Resource maintenance:</B></DT>
      </xsl:when>
      <xsl:otherwise>
        <DT><B>Maintenance:</B></DT>
      </xsl:otherwise>
    </xsl:choose>

    <DD>
    <DL>
      <xsl:for-each select="gmd:dateOfNextUpdate">
        <DT><B>Date of next update:</B> <xsl:call-template name="Date_PropertyType"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:maintenanceAndUpdateFrequency">
        <DT><B>Update frequency:</B>
        <xsl:for-each select="gmd:MD_MaintenanceFrequencyCode">
					<xsl:call-template name="AnyCode"/>
        </xsl:for-each>
        </DT>
      </xsl:for-each>
      <xsl:apply-templates select="gmd:userDefinedMaintenanceFrequency"/>
      <xsl:if test="gmd:dateOfNextUpdate | gmd:maintenanceAndUpdateFrequency | gmd:userDefinedMaintenanceFrequency"><BR/><BR/></xsl:if>

      <xsl:for-each select="gmd:updateScope/gmd:MD_ScopeCode">
        <DT><B>Scope of the updates:</B>
            <xsl:apply-templates select="gmd:updateScope/gmd:MD_ScopeCode" />
        </DT>
      </xsl:for-each>
      <xsl:apply-templates select="gmd:updateScopeDescription"/>
      <xsl:if test="gmd:updateScope | gmd:updateScopeDescription"><BR/><BR/></xsl:if>

      <xsl:for-each select="gmd:maintenanceNote">
        <DT><B>Other maintenance requirements:</B> <xsl:call-template name="CharacterString"/></DT>
        <BR/><BR/>
      </xsl:for-each>
    </DL>
    </DD>
    </DD>
</xsl:template>

<!-- Time Period Information (from 19103 information in 19115 DTD) -->
<xsl:template match="usrDefFreq">
  <DD>
  <DT><B>Time period between updates:</B></DT>
  <DD>
  <DL>
    <xsl:for-each select="designator">
      <DT><B>Time period designator:</B> <xsl:value-of select="."/></DT>
    </xsl:for-each>
    <xsl:for-each select="years">
      <DT><B>Years:</B> <xsl:value-of select="."/></DT>
    </xsl:for-each>
    <xsl:for-each select="months">
      <DT><B>Months:</B> <xsl:value-of select="."/></DT>
    </xsl:for-each>
    <xsl:for-each select="days">
      <DT><B>Days:</B> <xsl:value-of select="."/></DT>
    </xsl:for-each>
    <xsl:for-each select="timeIndicator">
      <DT><B>Time indicator:</B> <xsl:value-of select="."/></DT>
    </xsl:for-each>
    <xsl:for-each select="hours">
      <DT><B>Hours:</B> <xsl:value-of select="."/></DT>
    </xsl:for-each>
    <xsl:for-each select="minutes">
      <DT><B>Minutes:</B> <xsl:value-of select="."/></DT>
    </xsl:for-each>
    <xsl:for-each select="seconds">
      <DT><B>Seconds:</B> <xsl:value-of select="."/></DT>
    </xsl:for-each>
  </DL>
  </DD>
  </DD>
  <BR/>
</xsl:template>

<!-- Scope Description Information (B.2.5.1 MD_ScopeDescription - line149) -->
<xsl:template match="scpLvlDesc | upScpDesc">
<DD>
	<DT><B>Scope description:</B></DT>
	
  <xsl:for-each select="attribSet">
    <DD><B>Attributes:</B> <xsl:value-of select="."/></DD>
  </xsl:for-each>
  <xsl:for-each select="featSet">
    <DD><B>Features:</B> <xsl:value-of select="."/></DD>
  </xsl:for-each>
  <xsl:for-each select="featIntSet">
    <DD><B>Feature instances:</B> <xsl:value-of select="."/></DD>
  </xsl:for-each>
  <xsl:for-each select="attribIntSet">
    <DD><B>Attribute instances:</B> <xsl:value-of select="."/></DD>
  </xsl:for-each>
  <xsl:for-each select="datasetSet">
    <DD><B>Dataset:</B> <xsl:value-of select="."/></DD>
  </xsl:for-each>
  <xsl:for-each select="other">
    <DD><B>Other:</B> <xsl:value-of select="."/></DD>
  </xsl:for-each>

  <xsl:if test="count (following-sibling::*) = 0"><BR/><BR/></xsl:if>
  </DD>
</xsl:template>

<!-- General Constraint Information (B.2.3 MD_Constraints - line67) -->
<xsl:template match="gmd:MD_Constraints">
  <DD>
    <DT><B>Constraints:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:useLimitation">
        <DT><B>Limitations of use:</B> <xsl:call-template name="CharacterString"/></DT>
        <BR/><BR/>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- Legal Constraint Information (B.2.3 MD_LegalConstraints - line69) -->
<xsl:template match="gmd:MD_LegalConstraints">
  <DD>
    <DT><B>Legal constraints:</B></DT>
    <DD>
    <DL>
      <xsl:if test="gmd:accessConstraints">
        <DT><B>Access constraints:</B>
        <xsl:for-each select="gmd:accessConstraints">
					<xsl:apply-templates select="gmd:MD_RestrictionCode" /> 
          <xsl:if test="following-sibling::*">, </xsl:if>
        </xsl:for-each>
        </DT>
        <BR/><BR/>
      </xsl:if>

      <xsl:if test="gmd:useConstraints">
        <DT><B>Use constraints:</B>
        <xsl:for-each select="useConsts">
        	<xsl:apply-templates select="gmd:MD_RestrictionCode" /> 
          <xsl:if test="following-sibling::*">, </xsl:if>
        </xsl:for-each>
        </DT>
        <BR/><BR/>
      </xsl:if>

      <xsl:for-each select="othConsts">
        <DT><B>Other constraints:</B></DT>
        <PRE ID="original"><xsl:value-of select="."/></PRE>
        <SCRIPT>fix(original)</SCRIPT>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="gmd:otherConstraints">
        <DT><B>Limitations of use:</B> <xsl:call-template name="CharacterString"/></DT>
        <BR/><BR/>
      </xsl:for-each>
			
			<xsl:for-each select="gmd:useLimiation">
        <DT><B>Limitations of use:</B> <xsl:call-template name="CharacterString"/></DT>
        <BR/><BR/>
      </xsl:for-each>
			
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- Restrictions code list (B.5.24 MD_RestrictionCode) -->
<xsl:template match="gmd:MD_RestrictionCode">
	<xsl:call-template name="AnyCode"/>
</xsl:template>

<!-- Security Constraint Information (B.2.3 MD_SecurityConstraints - line73) -->
<xsl:template match="gmd:MD_SecurityConstraints">
  <DD>
    <DT><B>Security constraints:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:classification/gmd:MD_ClassificationCode">
        <DT><B>Classification:</B>
        	<xsl:call-template name="AnyCode"/>
        </DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:classificationSystem">
        <DT><B>Classification system:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:classification | gmd:classificationSystem"><BR/><BR/></xsl:if>

      <xsl:for-each select="gmd:userNote">
        <DT><B>Legal constraints:</B> <xsl:call-template name="CharacterString"/></DT>
        <BR/><BR/>
      </xsl:for-each>

      <xsl:for-each select="gmd:handlingDescription">
        <DT><B>Additional restrictions on handling the resource:</B> <xsl:call-template name="CharacterString"/></DT>
        <BR/><BR/>
      </xsl:for-each>

      <xsl:for-each select="gmd:useLimiation">
        <DT><B>Limitations of use:</B> <xsl:call-template name="CharacterString"/></DT>
        <BR/><BR/>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- RESOURCE IDENTIFICATION -->

<!-- Resource Identification Information (B.2.2 MD_Identification - line23, including MD_DataIdentification - line36) -->
<!-- DTD doesn't account for data and service subclasses of MD_Identification -->
<xsl:template match="gmd:MD_DataIdentification">
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
	<xsl:if test="count (../../*[gmd:MD_DataIdentification]) =  1">
  	<DT><H2>Resource Identification Information:</H2></DT>
  </xsl:if>
  <xsl:if test="count (../../*[gmd:MD_DataIdentification]) &gt; 1">
		<DT><H2>Resource Identification Information - Resource 
			<xsl:for-each select="..">
				<xsl:value-of select="position()"/>:
			</xsl:for-each></H2>
		</DT>
  </xsl:if>
  <DL>
  <DD>
    <xsl:apply-templates select="gmd:citation/gmd:CI_Citation"/>

    <xsl:if test="gmd:topicCategory[gmd:MD_TopicCategoryCode]">
      <DT><B>Themes or categories of the resource: </B>
      <xsl:for-each select="gmd:topicCategory">
      	<xsl:value-of select="gmd:MD_TopicCategoryCode"/>
		<xsl:if test="not(position()=last())">, </xsl:if>        
      </xsl:for-each>
      </DT>
      <xsl:if test="count (following-sibling::*) = 0"><BR/><BR/></xsl:if>
    </xsl:if>

    <xsl:apply-templates select="gmd:descriptiveKeywords/gmd:MD_Keywords"/>
    
    <xsl:for-each select="gmd:abstract">
      <DT><B>Abstract:</B></DT>
      <PRE ID="original"><xsl:call-template name="CharacterString"/></PRE>
      <SCRIPT>fix(original)</SCRIPT>
      <BR/>
    </xsl:for-each>

    <xsl:for-each select="gmd:purpose">
      <DT><B>Purpose:</B></DT>
      <PRE ID="original"><xsl:call-template name="CharacterString"/></PRE>
      <SCRIPT>fix(original)</SCRIPT>
      <BR/>
    </xsl:for-each>

    <xsl:apply-templates select="gmd:graphicOverview/gmd:MD_BrowseGraphic"/>
    
    <xsl:for-each select="gmd:language">
      <DT><B>Dataset language: </B>
          <xsl:apply-templates select="." />
      </DT>
    </xsl:for-each>
       
    <xsl:for-each select="gmd:characterSet/gmd:MD_CharacterSetCode">
      <DT><B>Dataset character set:</B><xsl:call-template name="AnyCode"/>
      </DT>
    </xsl:for-each>
    <xsl:if test="gmd:language | gmd:characterSet"><BR/><BR/></xsl:if>

		<!-- TODO: not yet supporting service description 
    <xsl:for-each select="serType">
      <DT><B>Type of service:</B> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:for-each select="typeProps">
      <DT><B>Attributes of the service:</B> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:if test="context()[($any$ serType | typeProps)]"><BR/><BR/></xsl:if>
		-->
		
    <xsl:for-each select="gmd:status/gmd:MD_ProgressCode">
      <DT><B>Status:</B>
        <xsl:call-template name="AnyCode"/>
      </DT>
    </xsl:for-each>
    <xsl:apply-templates select="gmd:resourceMaintenance"/>
<!--    <xsl:if test="gmd:status | gmd:resourceMaintenance"><BR/><BR/></xsl:if> -->

    <xsl:for-each select="gmd:resourceConstraints">
      <DT><B>Resource constraints:</B></DT>
      <DL>
        <xsl:apply-templates select="gmd:MD_Constraints"/>
        <xsl:apply-templates select="gmd:MD_LegalConstraints"/>
        <xsl:apply-templates select="gmd:MD_SecurityConstraints"/>
      </DL>
    </xsl:for-each>

    <xsl:apply-templates select="gmd:resourceSpecificUsage/gmd:MD_Usage"/>

    <xsl:for-each select="gmd:spatialRepresentationType/gmd:MD_SpatialRepresentationTypeCode">
      <DT><B>Spatial representation type:</B>
        <xsl:call-template name="AnyCode"/>
      </DT>
    </xsl:for-each>
    <xsl:apply-templates select="gmd:resourceFormat/gmd:MD_Format"/>
    <xsl:if test="gmd:spatialRepresentationType | gmd:resourceFormat"><BR/><BR/></xsl:if>

    <xsl:for-each select="gmd:environmentDescription">
      <DT><B>Processing environment:</B> <xsl:call-template name="CharacterString"/></DT>
      <BR/><BR/>
    </xsl:for-each>
    
    <xsl:apply-templates select="gmd:spatialResolution/gmd:MD_Resolution"/> 
    
	<xsl:apply-templates select="gmd:extent/gmd:EX_Extent"/>

    <xsl:for-each select="gmd:supplementalInformation">
      <DT><B>Supplemental information:</B></DT>
      <PRE ID="original"><xsl:call-template name="CharacterString"/></PRE>
      <SCRIPT>fix(original)</SCRIPT>
      <BR/>
    </xsl:for-each>

    <xsl:for-each select="gmd:credit">
      <DT><B>Credits:</B></DT>
      <PRE ID="original"><xsl:call-template name="CharacterString"/></PRE>
      <SCRIPT>fix(original)</SCRIPT>
      <BR/>
    </xsl:for-each>
    
    <xsl:apply-templates select="gmd:pointOfContact/gmd:CI_ResponsibleParty"/>
  </DD>
  </DL>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

<!-- Keyword Information (B.2.2.2 MD_Keywords - line52)-->
<xsl:template match="gmd:MD_Keywords">
  <DD>
   <xsl:choose>
     <xsl:when test="gmd:type/gmd:MD_KeywordTypeCode[@codeListValue='001' or @codeListValue='discipline']">
        <DT><B>Discipline keywords:</B></DT>
     </xsl:when>
     <xsl:when test="gmd:type/gmd:MD_KeywordTypeCode[@codeListValue='002' or @codeListValue='place']">
        <DT><B>Place keywords:</B></DT>
     </xsl:when>
     <xsl:when test="gmd:type/gmd:MD_KeywordTypeCode[@codeListValue='003' or @codeListValue='stratum']">
        <DT><B>Stratum keywords:</B></DT>
     </xsl:when>
     <xsl:when test="gmd:type/gmd:MD_KeywordTypeCode[@codeListValue='004' or @codeListValue='temporal']">
        <DT><B>Temporal keywords:</B></DT>
     </xsl:when>
     <xsl:when test="gmd:type/gmd:MD_KeywordTypeCode[@codeListValue='005' or @codeListValue='theme']">
        <DT><B>Theme keywords:</B></DT>
     </xsl:when>
     <xsl:otherwise>
        <DT><B>Descriptive keywords - <xsl:value-of select="gmd:type/gmd:MD_KeywordTypeCode/@codeListValue"/>:</B></DT>
     </xsl:otherwise>
   </xsl:choose>
    <DD>
    <DL>
      <xsl:if test="gmd:keyword">
        <DT>
        <xsl:for-each select="gmd:keyword">
          <xsl:if test="position() = 1"><B>Keywords:</B> </xsl:if>
          <xsl:call-template name="CharacterString"/><xsl:if test="not(position()=last())">, </xsl:if>
        </xsl:for-each>
        </DT>
        <BR/><BR/>
      </xsl:if>

      <xsl:apply-templates select="gmd:thesaurusName/gmd:CI_Citation"/>
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- Browse Graphic Information (B.2.2.1 MD_BrowseGraphic - line48) -->
<xsl:template match="gmd:MD_BrowseGraphic">
  <DD>
    <DT><B>Graphic overview:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:fileName">
        <DT><B>File name:</B><xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:fileType">
        <DT><B>File type:</B><xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:fileDescription">
        <DT><B>File description:</B><xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
  <BR/>
</xsl:template>

<!-- Usage Information (B.2.2.5 MD_Usage - line62) -->
<xsl:template match="gmd:MD_Usage">
  <DD>
    <DT><B>How the resource is used:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:usageDateTime">
        <DT><B>Date and time of use:</B> <xsl:call-template name="Date_PropertyType"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:specificUsage">
        <DT><B>Description:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:usageDateTime | gmd:specificUsage"><BR/><BR/></xsl:if>

      <xsl:for-each select="gmd:userDeterminedLimitations">
        <DT><B>How the resource must not be used:</B> <xsl:call-template name="CharacterString"/></DT>
        <BR/><BR/>
      </xsl:for-each>

      <xsl:apply-templates select="gmd:userContactInfo/gmd:CI_ResponsibleParty"/>
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- Resolution Information (B.2.2.4 MD_Resolution - line59) -->
<xsl:template match="gmd:MD_Resolution">
  <DD>
  <DT><B>Spatial resolution:</B></DT>
    <DD>
    <DL>
      <xsl:apply-templates select="gmd:equivalentScale/gmd:MD_RepresentativeFraction"/>

      <xsl:for-each select="gmd:distance/gco:Distance">
        <DT><B>Ground sample distance:</B> <xsl:apply-templates select="."/></DT>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- Representative Fraction Information (B.2.2.3 MD_RepresentativeFraction - line56) -->
<xsl:template match="gmd:MD_RepresentativeFraction">
  <DT><B>Dataset's scale:</B></DT>
  <DD>
  <DL>
    <xsl:for-each select="gmd:denominator">
      <DT><B>Scale denominator:</B><xsl:call-template name="Integer"/></DT>
    </xsl:for-each>
  </DL>
  </DD>
  <BR/>
</xsl:template>

<!-- SPATIAL REPRESENTATION -->

<!-- Spatial Representation Information (B.2.6  MD_SpatialRepresentation - line156) -->
    <!--
<xsl:template match="gmd:spatialRepresentationInfo">
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
    <xsl:choose>
      <xsl:when test="gmd:MD_GridSpatialRepresentation">
        <DT><H2>Grid:</H2></DT>
      </xsl:when>
      <xsl:when test="gmd:MD_VectorSpatialRepresentation">
        <DT><H2>Vector:</H2></DT>
      </xsl:when>
			<xsl:when test="gmd:MD_Georectified">
        <DT><H2>Georectified Grid:</H2></DT>
      </xsl:when>
      <xsl:when test="gmd:MD_Georeferenceable">
        <DT><H2>Georeferenceable Grid:</H2></DT>
      </xsl:when>
    -->
			<!-- NOTE: abstract -->
    <!--
      <xsl:otherwise>
        <DT><FONT color="#0000AA" size="3"><B>Spatial Representation:</B></FONT></DT>
      </xsl:otherwise>
    </xsl:choose>
    -->
    <!--  
  <DL>
    <DD>
      <xsl:apply-templates select="*"/>
    </DD>
  </DL>
  </DL> 
  <A HREF="#Top">Back to Top</A>
</xsl:template>
	-->

<!-- Grid Information (B.2.6  MD_GridSpatialRepresentation - line157 -->
<xsl:template name="MD_GridSpatialRepresentation"> 
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
  <DT><H2>Spatial Representation - Grid:</H2></DT>
  <DL>
  <DD>
   	<xsl:for-each select="gmd:numberOfDimensions">
      <DT><B>Number of dimensions:</B><xsl:call-template name="Integer"/></DT>
    </xsl:for-each>
	<DD>
		<DT><B>Axis dimensions properties:</B></DT>
		<DD>
		<DL>
			<xsl:apply-templates select="gmd:axisDimensionProperties/gmd:MD_Dimension"/>
		</DL>
		</DD>
	</DD>

	<xsl:for-each select="gmd:cellGeometry/gmd:MD_CellGeometryCode">
    	<DT><B>Cell geometry:</B>
			<xsl:call-template name="AnyCode"/>
		</DT>
    </xsl:for-each>
		
	<xsl:for-each select="gmd:transformationParameterAvailability">
		<DT><B>Transformation parameters are available:</B>
			<xsl:call-template name="Boolean"/>
		</DT>      
	</xsl:for-each>
		
	<xsl:if test="gmd:numberOfDimensions and not (gmd:axisDimensionProperties)"><BR/><BR/></xsl:if>
		
  </DD>
  </DL>
  </DL>
</xsl:template>

<!-- Grid Information (B.2.6  MD_GridSpatialRepresentation - line157 -->
<xsl:template match="gmd:MD_GridSpatialRepresentation">
		<xsl:call-template name="MD_GridSpatialRepresentation"/>
</xsl:template>

<xsl:template match="gmd:MD_Georectified">
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
  <DT><H2>Spatial Representation - Georectified:</H2></DT>
  <DL>
  <DD>
	<xsl:call-template name="MD_GridSpatialRepresentation"/>
	
	<xsl:for-each select="gmd:pointInPixel/gmd:MD_PixelOrientationCode">
		<DT><B>Point in pixel:</B><xsl:call-template name="AnyCode"/></DT>
	</xsl:for-each>
	<xsl:if test="gmd:cellGeometry | gmd:pointInPixel"><BR/><BR/></xsl:if>
	
	<xsl:for-each select="gmd:transformationDimensionDescription">
		<DT><B>Transformation dimension description:</B><xsl:call-template name="CharacterString"/></DT>
	</xsl:for-each>
	<xsl:for-each select="gmd:transformationDimensionMapping">
		<DT><B>Tranformation dimension mapping:</B><xsl:call-template name="CharacterString"/></DT>
	</xsl:for-each>
	<xsl:if test="gmd:transformationParameterAvailability | gmd:transformationDimensionDescription | gmd:transformationDimensionMapping"><BR/><BR/></xsl:if>
    
	<xsl:for-each select="gmd:checkPointAvailability">
		<DT><B>Check points are available:</B><xsl:call-template name="Boolean"/></DT>      
	</xsl:for-each>
	<xsl:for-each select="gmd:checkPointDescription">
		<DT><B>Check point description:</B><xsl:call-template name="CharacterString"/></DT>
	</xsl:for-each>
	<xsl:for-each select="gmd:cornerPoints">
		<DT><B>Corner points:</B></DT>
		<DD>
		<DL>
			<xsl:for-each select="gml:Point">
				<DT><B>Coordinates:</B><xsl:apply-templates select="."/></DT>
				<BR/><BR/>
			</xsl:for-each>
		</DL>
		</DD>
	</xsl:for-each>
	<xsl:for-each select="gmd:centerPoint">
		<DT><B>Center point:</B></DT>
		<DD>
		<DL>
			<xsl:for-each select="gml:Point">
				<DT><B>Coordinates:</B><xsl:apply-templates select="."/></DT>
				<BR/><BR/>
			</xsl:for-each>
		</DL>
		</DD>
	</xsl:for-each>

	<xsl:if test="gmd:checkPointAvailability | gmd:checkPointDescription | gmd:cornerPoints | gmd:centerPoint"><BR/><BR/></xsl:if>

  </DD>
  </DL>
  </DL>
</xsl:template>

<xsl:template match="gmd:MD_Georeferenceable">
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
  <DT><H2>Spatial Representation - Georeferenceable:</H2></DT>
  <DL>
  <DD>
	<xsl:call-template name="MD_GridSpatialRepresentation"/>
	
      <xsl:for-each select="gmd:controlPointAvailability">
        <DT><B>Control points are available:</B><xsl:call-template name="Boolean"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:orientationParameterAvailability">
        <DT><B>Orientation parameters are available:</B><xsl:call-template name="Boolean"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:orientationParameterDescription">
        <DT><B>Orientation parameter description:</B><xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:controlPointAvailability | gmd:orientationParameterAvailability | gmd:orientationParameterDescription"><BR/><BR/></xsl:if>

      <xsl:for-each select="gmd:georeferencedParameters">
        <DT><B>Georeferencing parameters:</B><xsl:call-template name="Record"/></DT>
      </xsl:for-each>
      <xsl:apply-templates select="gmd:parameterCitation/gmd:CI_Citation"/>
      <xsl:if test="gmd:georeferencedParameters and not (gmd:parameterCitation)"><BR/><BR/></xsl:if>
		
  </DD>
  </DL>
  </DL>
</xsl:template> 

<!-- Dimension Information (B.2.6.1 MD_Dimension - line179) -->
<!-- DataType -->
<xsl:template match="gmd:MD_Dimension">
  
      <!--<xsl:for-each select="gmd:MD_Dimension">-->
				<DT><B>Dimension:</B></DT>
				<DD>
				<DL>
					<xsl:for-each select="gmd:dimensionName/gmd:MD_DimensionNameTypeCode">
						<DT><B>Dimension name:</B>
							<xsl:call-template name="AnyCode"/>
						</DT>
					</xsl:for-each>
					<xsl:for-each select="gmd:dimensionSize">
						<DT><B>Dimension size:</B><xsl:call-template name="Integer"/></DT>
					</xsl:for-each>
					<xsl:for-each select="gmd:resolution/gco:Measure">
						<DT><B>Resolution:</B></DT>
						<DL>
						<DT><B>Distance:</B><xsl:apply-templates select="."/></DT>
						</DL>
					</xsl:for-each>
				</DL>
				</DD>
				<BR/>
      <!--</xsl:for-each>-->
 
</xsl:template>

<!-- Vector Information (B.2.6  MD_VectorSpatialRepresentation - line176) -->
<xsl:template match="gmd:MD_VectorSpatialRepresentation">
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
  <DT><H2>Spatial Representation - Vector:</H2></DT>
  <DL>
  <DD>
      <xsl:for-each select="gmd:topologyLevel/gmd:MD_TopologyLevelCode">
        <DT><B>Level of topology for this dataset:</B>
			<xsl:call-template name="AnyCode"/>
        </DT>
      </xsl:for-each>
      <xsl:apply-templates select="gmd:geometricObjects/gmd:MD_GeometricObjects"/>
      <xsl:if test="gmd:topologyLevel and not (gmd:geometricObjects)"><BR/><BR/></xsl:if>
		
  </DD>
  </DL>
  </DL>
</xsl:template>

<!-- Geometric Object Information (B.2.6.2 MD_GeometricObjects - line183) -->
<!-- Data Type -->
<xsl:template match="gmd:MD_GeometricObjects">
  <DD>
    <DT><B>Geometric objects:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:geometricObjectType/gmd:MD_GeometricObjectTypeCode">
        <DT><B>Object type:</B><xsl:call-template name="AnyCode"/>
        </DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:geometricObjectCount">
        <DT><B>Object count:</B><xsl:call-template name="Integer"/></DT>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
  <BR/>
</xsl:template>

<!-- Identifier Information (B.2.7.2 MD_Identifier - line205) -->
<xsl:template match="gmd:MD_Identifier">
	<xsl:call-template name="MD_Identifier"/>
</xsl:template>

<xsl:template match="gmd:RS_Identifier">
	<xsl:call-template name="MD_Identifier"/>
	<DD>
	<DL>
	<xsl:for-each select="gmd:codeSpace">
		<DT><B>Code space:</B> <xsl:call-template name="CharacterString"/></DT>
	</xsl:for-each>
	<xsl:for-each select="gmd:version">
		<DT><B>Version:</B> <xsl:call-template name="CharacterString"/></DT>
	</xsl:for-each>
	</DL>
	</DD>
</xsl:template>

<xsl:template name="MD_Identifier"> <!-- NOTE: was match="geoId | refSysID | projection | ellipsoid | datum | refSysName | 
      MdIdent | RS_Identifier | datumID"> -->
  <DD>
  <xsl:choose>
    <xsl:when test="../gmd:geographicIdentifier">
        <DT><B>Geographic identifier:</B></DT>
    </xsl:when>
    <!-- don't include an xsl:otherwise so the identCode value will appear correctly indented under the heading -->
  </xsl:choose>
	<DD>
	<DL>
		<xsl:for-each select="gmd:code">
			<DT><B>Value:</B> <xsl:call-template name="CharacterString"/></DT>
		</xsl:for-each>

		<xsl:apply-templates select="gmd:authority/gmd:CI_Citation"/>
		
		<xsl:if test="(gmd:code) and not (gmd:authority)"><BR/><BR/></xsl:if>
	</DL>
	</DD>
  </DD>
</xsl:template>


<!-- CONTENT INFORMATION -->

<!-- Content Information (B.2.8 MD_ContentInformation - line232) -->
<xsl:template match="gmd:contentInfo"> <!-- TODO: change reference to this -->
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
    <xsl:choose>
      <xsl:when test="gmd:MD_FeatureCatalogueDescription">
        <DT><H2>Content Information - Feature Catalogue Description:</H2></DT>
      </xsl:when>
      <xsl:when test="gmd:MD_CoverageDescription">
        <DT><H2>Content Information - Coverage Description:</H2></DT>
      </xsl:when>
      <xsl:when test="gmd:MD_ImageDescription">
        <DT><H2>Content Information - Image Description:</H2></DT>
      </xsl:when>
      <xsl:otherwise>
        <DT><H2>Content Information:</H2></DT>
      </xsl:otherwise>
    </xsl:choose>
  <DL>
    <DD>
        <xsl:apply-templates />
    </DD>
    </DL>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

<!-- Feature Catalogue Description (B.2.8 MD_FeatureCatalogueDescription - line233) -->
<xsl:template match="gmd:MD_FeatureCatalogueDescription">
      <xsl:for-each select="gmd:language">
        <DT><B>Feature catalogue's language:</B>
            <xsl:call-template name="CharacterString"/>
        </DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:includedWithDataset">
        <DT><B>Catalogue accompanies the dataset:</B>
        	<xsl:call-template name="Boolean"/>
        </DT>      
      </xsl:for-each>
      <xsl:for-each select="gmd:complianceCode">
        <DT><B>Catalogue complies with ISO 19110:</B>
        	<xsl:call-template name="Boolean"/>
        </DT>      
      </xsl:for-each>
      <xsl:if test="gmd:language | gmd:includedWithDataset | gmd:complianceCode"><BR/><BR/></xsl:if>

      <xsl:for-each select="gmd:featureTypes">
        <DT><B>Feature types in the dataset:</B></DT>
        <xsl:apply-templates /> <!-- NOTE: will match gco:LocalName and gco:ScopedName -->
      </xsl:for-each>

     <xsl:apply-templates select="gmd:featureCatalogueCitation/CI_Citation"/>
</xsl:template>

<!-- Coverage Description (B.2.8 MD_CoverageDescription - line239) -->
<xsl:template match="gmd:MD_CoverageDescription">
      <xsl:for-each select="gmd:contentType">
        <DT><B>Type of information:</B>
        <xsl:for-each select="gmd:MD_CoverageContentTypeCode">
        	<xsl:call-template name="AnyCode"/>
        </xsl:for-each>
        </DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:attributeDescription">
        <DT><B>Attribute described by cell values:</B> <xsl:call-template name="RecordType"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:contentType | gmd:attributeDescription"><BR/><BR/></xsl:if>

      <xsl:apply-templates select="gmd:dimension"/>
</xsl:template>

<!-- Range dimension Information (B.2.8.1 MD_RangeDimension - line256) -->
<xsl:template match="gmd:dimension"> <!-- NOTE: not matching class because there is one subclass -->
    <DD>
    <xsl:choose>
      <xsl:when test="gmd:MD_RangeDimension">
        <DT><B>Range of cell values:</B></DT>
      </xsl:when>
      <xsl:when test="gmd:MD_Band">
        <DT><B>Band information:</B></DT>
      </xsl:when>
			<!-- don't see any way that this could be anything but the two options above -->	
			<!--
      <xsl:otherwise>
        <DT><B>Cell value information:</B></DT>
      </xsl:otherwise> -->
    </xsl:choose>

    <DD>
    <DL>
      <xsl:for-each select="*/gmd:descriptor">
        <DT><B>Minimum and maximum values:</B><xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="*/gmd:sequenceIdentifier/gco:MemberName">
        <DT><B>Band identifier:</B></DT>
        <xsl:apply-templates select="." />
      </xsl:for-each>
      <xsl:if test="*/gmd:descriptor | */gmd:sequenceIdentifier"><BR/><BR/></xsl:if>

    <xsl:for-each select="gmd:MD_Band">
      <xsl:for-each select="gmd:maxValue">
        <DT><B>Longest wavelength:</B><xsl:call-template name="Real"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:minValue">
        <DT><B>Shortest wavelength:</B><xsl:call-template name="Real"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:peakResponse">
        <DT><B>Peak response wavelength:</B><xsl:call-template name="Real"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:units">
        <DT><B>Wavelength units:</B></DT>
        <xsl:apply-templates select="gmd:units/gml:UnitDefinition"/>
      </xsl:if>
      <xsl:if test="(gmd:maxValue | gmd:minValue | gmd:peakResponse) and not (gmd:units)"><BR/><BR/></xsl:if>

      <xsl:for-each select="gmd:bitsPerValue">
        <DT><B>Number of bits per value:</B><xsl:call-template name="Integer"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:toneGradation">
        <DT><B>Number of discrete values:</B><xsl:call-template name="Integer"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:scaleFactor">
        <DT><B>Scale factor applied to values:</B><xsl:call-template name="Real"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:offset">
        <DT><B>Offset of values from zero:</B><xsl:call-template name="Real"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:bitsPerValue | gmd:toneGradation | gmd:scaleFactor | gmd:offset"><BR/><BR/></xsl:if>
    </xsl:for-each>
    </DL>
    </DD>
    </DD>
</xsl:template>

<!-- Type Name -->
<!-- TODO: is this in 19115? 
<xsl:template match="TypeName">
    <DD>
    <DL>
      <xsl:for-each select="scope">
        <DT><B>Scope:</B> <xsl:value-of /></DT>
      </xsl:for-each>
      <xsl:for-each select="aName">
        <DT><B>Name:</B> <xsl:value-of/></DT>
      </xsl:for-each>
    </DL>
    </DD>
    <BR/>
</xsl:template>-->

<!-- Member Name -->
<xsl:template match="gco:MemberName">
    <DD>
    <DL>
      <!-- TODO: is this in 19115? <xsl:for-each select="scope">
        <DT><B>Scope:</B> <xsl:value-of /></DT>
      </xsl:for-each>-->
      <xsl:for-each select="gco:aName">
        <DT><B>Name:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gco:attributeType/gco:TypeName">
        <DT><B>Attribute type:</B></DT>
        <DD>
        <DL>
          <!-- TODO: is this in 19115? <xsl:for-each select="scope">
            <DT><B>Scope:</B> <xsl:value-of /></DT>
          </xsl:for-each>-->
          <xsl:for-each select="gco:aName">
            <DT><B>Name:</B> <xsl:call-template name="CharacterString"/></DT>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>
    </DL>
    </DD>
    <BR/>
</xsl:template>

<!-- Image Description (B.2.8 MD_ImageDescription - line243) -->
<xsl:template match="gmd:MD_ImageDescription">

			<!-- TODO: create a common template for MD_CoverageDescription so dup content is not here -->
      <xsl:for-each select="gmd:contentType">
        <DT><B>Type of information:</B>
        <xsl:for-each select="gmd:MD_CoverageContentTypeCode">
        	<xsl:call-template name="AnyCode"/>
        </xsl:for-each>
        </DT>
      </xsl:for-each>
			<xsl:for-each select="gmd:attributeDescription">
        <DT><B>Attribute described by cell values:</B><xsl:call-template name="RecordType"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:contentType | gmd:attributeDescription"><BR/><BR/></xsl:if>

      <xsl:apply-templates select="gmd:dimension"/>

      <xsl:for-each select="gmd:illuminationElevationAngle">
        <DT><B>Illumination elevation angle:</B><xsl:call-template name="Real"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:illuminationAzimuthAngle">
        <DT><B>Illumination azimuth angle:</B><xsl:call-template name="Real"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:imagingCondition/gmd:MD_ImagingConditionCode">
        <DT><B>Imaging condition:</B>
       		<xsl:call-template name="AnyCode"/>
        </DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:cloudCoverPercentage">
        <DT><B>Percent cloud cover:</B><xsl:call-template name="Real"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:illuminationElevationAngle | gmd:illuminationAzimuthAngle | gmd:imagingCondition | gmd:cloudCoverPercentage"><BR/><BR/></xsl:if>

      <xsl:for-each select="gmd:imageQualityCode/gmd:MD_Identifier">
        <DT><B>Image quality code:</B></DT>
        <xsl:apply-templates />
      </xsl:for-each>

      <xsl:for-each select="gmd:processingLevelCode/gmd:MD_Identifier">
        <DT><B>Processing level code:</B></DT>
        <xsl:apply-templates />
      </xsl:for-each>

      <xsl:for-each select="gmd:compressionGenerationQuantity">
        <DT><B>Number of lossy compression cycles:</B><xsl:call-template name="Integer"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:triangulationIndicator">
        <DT><B>Triangulation has been performed:</B>
          <xsl:call-template name="Boolean"/>
        </DT>      
      </xsl:for-each>
      <xsl:for-each select="gmd:radiometricCalibrationDataAvailability">
        <DT><B>Radiometric calibration is available:</B>
					<xsl:call-template name="Boolean"/>
        </DT>      
      </xsl:for-each>
      <xsl:for-each select="gmd:cameraCalibrationInformationAvailability">
        <DT><B>Camera calibration is available:</B>
					<xsl:call-template name="Boolean"/>
        </DT>      
      </xsl:for-each>
      <xsl:for-each select="gmd:filmDistortionInformationAvailability">
        <DT><B>Film distortion information is available:</B>
					<xsl:value-of select="."/>
        </DT>      
      </xsl:for-each>
      <xsl:for-each select="gmd:lensDistortionInformationAvailability">
        <DT><B>Lens distortion information is available:</B>
					<xsl:call-template name="Boolean"/>
        </DT>      
      </xsl:for-each>
      <xsl:if test="gmd:compressionGenerationQuantity | gmd:triangulationIndicator | 
				gmd:radiometricCalibrationDataAvailability | gmd:cameraCalibrationInformationAvailability |  
				gmd:filmDistortionInformationAvailability | gmd:lensDistortionInformationAvailability"><BR/><BR/></xsl:if>
</xsl:template>


<!-- REFERENCE SYSTEM -->

<!-- Reference System Information (B.2.7 MD_ReferenceSystem - line186) -->
<xsl:template match="gmd:MD_ReferenceSystem"> 
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
  <xsl:if test="count (../../gmd:referenceSystemInfo) = 1">
      <DT><H2>Reference System Information:</H2></DT>
  </xsl:if>
  <xsl:if test="count (../../gmd:referenceSystemInfo) &gt; 1">
      <DT><H2>
        Reference System Information - System <xsl:value-of select="position()"/>:
      </H2></DT>
  </xsl:if>
	<DL>
	<DD>
		 <xsl:if test="gmd:referenceSystemIdentifier">
			<DT><B>Reference system identifier:</B></DT>
			<xsl:apply-templates select="gmd:referenceSystemIdentifier/gmd:RS_Identifier"/>
		</xsl:if>

		<xsl:if test="count (following-sibling::*) = 0"><BR/></xsl:if>

		<!-- no support in the DIS DTD for RS_ReferenceSystem information and TMRefSys, SIRefSys, SCRefSys -->
	</DD>
	</DL>
	</DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

<!-- DATA QUALITY -->

<!-- Data Quality Information  (B.2.4 DQ_DataQuality - line78) -->
<xsl:template match="gmd:DQ_DataQuality">
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
	<xsl:if test="count (../../*[gmd:DQ_DataQuality]) =  1">
  	<DT><H2>Data Quality Information:</H2></DT>
  </xsl:if>
  <xsl:if test="count (../../*[gmd:DQ_DataQuality]) &gt; 1">
		<DT><H2>Data Quality - Description 
			<xsl:for-each select="..">
				<xsl:value-of select="position()"/>:
			</xsl:for-each></H2>
		</DT>
  </xsl:if>
  <DL>
  <DD>
    <xsl:apply-templates select="gmd:scope/gmd:DQ_Scope"/>

    <xsl:apply-templates select="gmd:lineage/gmd:LI_Lineage"/>

    <xsl:for-each select="gmd:report/*"> <!-- NOTE: will select sub-classes -->
      <xsl:apply-templates select="."/>
    </xsl:for-each>
  </DD>
  </DL>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

<!-- Scope Information (B.2.4.4 DQ_Scope - line138) -->
<xsl:template match="gmd:DQ_Scope">
    <DD>
    <DT><B>Scope of quality information:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:level">
        <DT><B>Level of the data:</B>
            <xsl:apply-templates select="gmd:MD_ScopeCode" /> <!-- TODO: make sure there's a global template for this -->
        </DT>
      </xsl:for-each>
      <xsl:apply-templates select="gmd:levelDescription"/>
      <xsl:if test="(gmd:level) and not (gmd:levelDescription)"><BR/><BR/></xsl:if>

      <xsl:apply-templates select="gmd:extent/gmd:EX_Extent"/> <!-- TODO: make sure there's a global template for this -->
    </DL>
    </DD>
    </DD>
</xsl:template>

<!-- Lineage Information (B.2.4.1 LI_Lineage - line82) -->
<xsl:template match="gmd:LI_Lineage">
  <DD>
  <DT><B>Lineage:</B></DT>
  <DD>
  <DL>
    <xsl:for-each select="gmd:statement">
      <DT><B>Lineage statement:</B></DT>
      <PRE ID="original"><xsl:call-template name="CharacterString"/></PRE>
      <SCRIPT>fix(original)</SCRIPT>
      <BR/>
    </xsl:for-each>

    <xsl:apply-templates select="gmd:processStep/gmd:LI_ProcessStep"/>

    <xsl:apply-templates select="gmd:source/gmd:LI_Source"/>
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Process Step Information (B.2.4.1.1 LI_ProcessStep - line86) -->
<xsl:template match="gmd:LI_ProcessStep"> <!-- NOTE: was match="(prcStep | srcStep)"> -->
  <DD>
  <DT><B>Process step:</B></DT>
  <DD>
  <DL>
    <xsl:for-each select="gmd:dateTime">
      <DT><B>When the process occurred:</B> <xsl:call-template name="Date_PropertyType"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:description">
      <DT><B>Description:</B><xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:rationale">
      <DT><B>Rationale:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:dateTime | gmd:description |gmd:rationale"><BR/><BR/></xsl:if>

    <xsl:apply-templates select="gmd:processor/gmd:CI_ResponsibleParty"/>

		<!-- TODO: review this -->
    <xsl:if test="not (../gmd:sourceStep)">
      <xsl:apply-templates select="gmd:source/gmd:LI_Source"/>
    </xsl:if>
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Source Information (B.2.4.1.2 LI_Source - line92) -->
<xsl:template match="gmd:LI_Source"> <!-- TODO: make sure there are callers of this template -->
  <DD>
  <DT><B>Source data:</B></DT>
  <DD>
  <DL>
      <xsl:for-each select="gmd:description">
        <DT><B>Level of the source data:</B> <xsl:call-template name="CharacterString"/></DT>
        <BR/><BR/>
      </xsl:for-each>
      
      <xsl:apply-templates select="gmd:scaleDenominator/gmd:MD_RepresentativeFraction"/>
      
      <xsl:apply-templates select="gmd:sourceCitation/gmd:CI_Citation"/>
      
      <xsl:for-each select="gmd:sourceReferenceSystem">
        <DT><B>Source reference system:</B></DT>
	    <DD>
	    <DL>
	      <xsl:apply-templates select="gmd:MD_ReferenceSystem"/>
	    </DL>
	    </DD>
      </xsl:for-each>
      
      <xsl:apply-templates select="gmd:sourceExtent/gmd:EX_Extent"/>

			<!-- TODO: review this -->
      <xsl:if test="not (../gmd:source)">
        <xsl:apply-templates select="gmd:sourceStep"/>
      </xsl:if>
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Data Quality Element Information (B.2.4.2 DQ_Element - line99) -->
<xsl:template match="gmd:DQ_CompletenessOmission | gmd:DQ_CompletenessCommission | 
	gmd:DQ_TopologicalConsistency | gmd:DQ_FormatConsistency |
	gmd:DQ_DomainConsistency | gmd:DQ_ConceptualConsistency |
	gmd:DQ_RelativeInternalPositionalAccuracy | gmd:DQ_GriddedDataPositionalAccuracy |
	gmd:DQ_AbsoluteExternalPositionalAccuracy | gmd:DQ_QuantitativeAttributeAccuracy |
	gmd:DQ_NonQuantitativeAttributeAccuracy | gmd:DQ_ThematicClassificationCorrectness |
	gmd:DQ_TemporalValidity | gmd:DQ_TemporalConsistency | gmd:DQ_AccuracyOfATimeMeasurement">
  <DD>
  <xsl:choose>
		<!-- NOTE: this is abstract
    <xsl:when test="../DQComplete">
        <DT><B>Data quality report - Completeness:</B></DT>
    </xsl:when>-->
    <xsl:when test="local-name(.) = 'DQ_CompletenessCommission'">
        <DT><B>Data quality report - Completeness commission:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_CompletenessOmission'">
        <DT><B>Data quality report - Completeness omission:</B></DT>
    </xsl:when>
		<!-- NOTE: this is abstract
    <xsl:when test="../DQLogConsis">
        <DT><B>Data quality report - Logical consistency:</B></DT>
    </xsl:when>-->
    <xsl:when test="local-name(.) = 'DQ_ConceptualConsistency'">
        <DT><B>Data quality report - Conceptual consistency:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_DomainConsistency'">
        <DT><B>Data quality report - Domain consistency:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_FormatConsistency'">
        <DT><B>Data quality report - Formal consistency:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_TopologicalConsistency'">
        <DT><B>Data quality report - Topological consistency:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_AbsoluteExternalPositionalAccuracy'">
        <DT><B>Data quality report - Absolute external positional accuracy:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_GriddedDataPositionalAccuracy'">
        <DT><B>Data quality report - Gridded data positional accuracy:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_RelativeInternalPositionalAccuracy'">
        <DT><B>Data quality report - Relative internal positional accuracy:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_AccuracyOfATimeMeasurement'">
        <DT><B>Data quality report - Accuracy of a time measurement:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_TemporalConsistency'">
        <DT><B>Data quality report - Temporal consistency:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_TemporalValidity'">
        <DT><B>Data quality report - Temporal validity:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_ThematicClassificationCorrectness'">
        <DT><B>Data quality report - Thematic classification correctness:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_NonQuantitativeAttributeAccuracy'">
        <DT><B>Data quality report - Non quantitative attribute accuracy:</B></DT>
    </xsl:when>
    <xsl:when test="local-name(.) = 'DQ_QuantitativeAttributeAccuracy'">
        <DT><B>Data quality report - Quantitative attribute accuracy:</B></DT>
    </xsl:when>
  </xsl:choose>

  <DD>
  <DL>
    <xsl:for-each select="gmd:nameOfMeasure">
      <DT><B>Name of the test:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:evaluationMethodType/gmd:DQ_EvaluationMethodTypeCode">
      <DT><B>Type of test:</B>
				<xsl:call-template name="AnyCode"/>
      </DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:dateTime">
      <DT><B>Date of the test:</B> <xsl:call-template name="Date_PropertyType"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:nameOfMeasure | gmd:evaluationMethodType | gmd:dateTime"><BR/><BR/></xsl:if>

    <xsl:for-each select="gmd:measureDescription">
      <DT><B>Measure produced by the test:</B> <xsl:call-template name="CharacterString"/></DT>
      <BR/><BR/>
    </xsl:for-each>

    <xsl:for-each select="gmd:evaluationMethodDescription">
      <DT><B>Evaluation method:</B> <xsl:call-template name="CharacterString"/></DT>
      <BR/><BR/>
    </xsl:for-each>

    <xsl:for-each select="gmd:measureIdentification/gmd:MD_Identifier">
      <DT><B>Registered standard procedure:</B></DT>
      <xsl:apply-templates select="."/>
    </xsl:for-each>

    <xsl:apply-templates select="gmd:evaluationProcedure/gmd:CI_Citation"/>

    <xsl:for-each select="gmd:result">
				<!-- NOTE: this will select the sub-classes:
					DQ_ConformanceResult, DQ_QuantitativeResult
				-->
        <xsl:apply-templates select="*"/>
    </xsl:for-each>
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Conformance Result Information (B.2.4.3 DQ_ConformanceResult - line129) -->
<xsl:template match="gmd:DQ_ConformanceResult">
  <DD>
  <DT><B>Conformance test results:</B> </DT>
  <DD>
  <DL>
    <xsl:for-each select="gmd:pass">
      <DT><B>Test passed:</B>
      	<xsl:call-template name="Boolean"/>
      </DT>      
    </xsl:for-each>
    <xsl:for-each select="gmd:explanation">
      <DT><B>Meaning of the result:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:pass | gmd:explanation"><BR/><BR/></xsl:if>

    <xsl:apply-templates select="gmd:specification/gmd:CI_Citation"/>
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Quantitative Result Information (B.2.4.3 DQ_QuantitativeResult - line133) -->
<xsl:template match="gmd:DQ_QuantitativeResult">
  <DD>
  <DT><B>Quality test results:</B> </DT>
  <DD>
  <DL>
    <xsl:for-each select="gmd:valueType">
      <DT><B>Values required for conformance:</B><xsl:call-template name="RecordType"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:valueUnit">
      <DT><B>Value units:</B></DT>
      <DD>
      <DL>
          <!-- value will be shown regardless of the subelement Integer, Real, or Decimal -->
					<!-- TODO: not sure if this is in GML or GMD?
          <xsl:for-each select="value">
            <DT><B>Precision:</B> <xsl:value-of select="."/></DT>
          </xsl:for-each>-->

          <xsl:apply-templates select="gml:UnitDefinition"/>

          <xsl:if test="count (following-sibling::*) = 0"><BR/><BR/></xsl:if>
      </DL>
      </DD>
    </xsl:for-each>
    <xsl:if test="(gmd:valueType) and not (gmd:valueUnit)"><BR/><BR/></xsl:if>

    <xsl:for-each select="gmd:errorStatistic">
      <DT><B>Statistical method used to determine values:</B> <xsl:call-template name="CharacterString"/></DT>
      <BR/><BR/>
    </xsl:for-each>

    <xsl:if test="gmd:value">
      <xsl:for-each select="gmd:value">
        <DT><B>Result value:</B><xsl:call-template name="Record"/></DT>
      </xsl:for-each>
      <BR/><BR/>
    </xsl:if>
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- DISTRIBUTION INFORMATION -->

<!-- Distribution Information (B.2.10 MD_Distribution - line270) -->
<xsl:template match="gmd:MD_Distribution">
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
	<xsl:if test="count (../../gmd:distributionInfo) = 1">
		<DT><H2>Distribution Information:</H2></DT>
  </xsl:if>
  <xsl:if test="count (../../gmd:distributionInfo) &gt; 1">
      <DT><H2>
        Distribution Information - Distribution <xsl:value-of select="position()"/>:
      </H2></DT>
  </xsl:if>
  <DL>
  <DD>
      <xsl:apply-templates select="gmd:distributor/gmd:MD_Distributor"/>

      <xsl:apply-templates select="gmd:distributionFormat/gmd:MD_Format"/>

      <xsl:apply-templates select="gmd:transferOptions/gmd:MD_DigitalTransferOptions"/>
  </DD>
  </DL>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>


<!-- Distributor Information (B.2.10.2 MD_Distributor - line279) -->
<xsl:template match="gmd:MD_Distributor"> <!-- NOTE: was (distributor | formatDist)"> -->
  <DD>
  <DT><B>Distributor:</B></DT>
  <DD>
  <DL>
    <xsl:apply-templates select="gmd:distributorContact/gmd:CI_ResponsibleParty"/>
    <!-- NOTE: removed tests for recursion <xsl:if test="context()[not ((../../dsFormat) or (../../distorFormat) or (../../distFormat))]">
      <xsl:apply-templates select="gmd:distributorFormat/gmd:MD_Format"/> 
    </xsl:if>-->
		<xsl:apply-templates select="gmd:distributorFormat/gmd:MD_Format"/>
		
    <xsl:apply-templates select="gmd:distributionOrderProcess/gmd:MD_StandardOrderProcess"/>

    <xsl:apply-templates select="gmd:distributorTransferOptions/gmd:MD_DigitalTransferOptions"/>
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Format Information (B.2.10.3 MD_Format - line284) -->
<xsl:template match="gmd:MD_Format">
  <DD>
  <xsl:choose>
    <xsl:when test="../gmd:resourceFormat">
        <DT><B>Resource format:</B></DT>
    </xsl:when>
		<!-- TODO: is there an "available format"? -->
    <xsl:when test="../gmd:distributorFormat">
        <DT><B>Distribution format:</B></DT>
    </xsl:when>
    <xsl:otherwise>
        <DT><B>Format:</B></DT>
    </xsl:otherwise>
  </xsl:choose>

  <DD>
  <DL>
    <xsl:for-each select="gmd:name">
      <DT><B>Format name:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:version">
      <DT><B>Format version:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:amendmentNumber">
      <DT><B>Format amendment number:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:specification">
      <DT><B>Format specification:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:fileDecompressionTechnique">
      <DT><B>File decompression technique:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:name | gmd:version | gmd:amendmentNumber | gmd:specification | gmd:fileDecompressionTechnique"><BR/><BR/></xsl:if>

    <!-- NOTE: removed <xsl:if test="context()[not ((../../distributor) or (../../formatDist))]">
      <xsl:apply-templates select="formatDist"/>
    </xsl:if>-->
		<xsl:apply-templates select="gmd:formatDistributor/gmd:MD_Distributor"/>
		
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Standard Order Process Information (B.2.10.5 MD_StandardOrderProcess - line298) -->
<xsl:template match="gmd:MD_StandardOrderProcess">
  <DD>
  <DT><B>Ordering process:</B></DT>
  <DD>
  <DL>
    <xsl:for-each select="gmd:fees">
      <DT><B>Terms and fees:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:plannedAvailableDateTime">
      <DT><B>Date of availability:</B> <xsl:call-template name="Date_PropertyType"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:orderingInstructions">
      <DT><B>Turnaround time:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:turnaround">
      <DT><B>Instructions:</B></DT>
      <PRE ID="original"><xsl:call-template name="CharacterString"/></PRE>
      <SCRIPT>fix(original)</SCRIPT>
    </xsl:for-each>
  </DL>
  </DD>
  </DD>
  <BR/>
</xsl:template>

<!-- Digital Transfer Options Information (B.2.10.1 MD_DigitalTransferOptions - line274) -->
<xsl:template match="gmd:MD_DigitalTransferOptions"> <!-- NOTE: was (distorTran | distTranOps)"> -->
  <DD>
  <DT><B>Transfer options:</B></DT>

  <DD>
  <DL>
    <xsl:for-each select="gmd:transferSize">
      <DT><B>Transfer size:</B><xsl:call-template name="Real"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:unitsOfDistribution">
      <DT><B>Units of distribution (e.g., tiles):</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:transferSize | gmd:unitsOfDistribution"><BR/><BR/></xsl:if>

    <xsl:apply-templates select="gmd:onLine/gmd:CI_OnlineResource"/>

    <xsl:apply-templates select="gmd:offLine/gmd:MD_Medium"/>
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Medium Information (B.2.10.4 MD_Medium - line291) -->
<xsl:template match="gmd:MD_Medium">
  <DD>
  <DT><B>Medium of distribution:</B></DT>
  <DD>
  <DL>
    <xsl:for-each select="gmd:name">
      <DT><B>Medium name:</B>
        <xsl:for-each select="gmd:MD_MediumNameCode">
          <xsl:call-template name="AnyCode"/>
        </xsl:for-each>
      </DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:volumes">
      <DT><B>Number of media items:</B><xsl:call-template name="Integer"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:name | gmd:volumes"><BR/><BR/></xsl:if>

    <xsl:for-each select="gmd:mediumFormat">
      <DT><B>How the medium is written:</B>
        <xsl:for-each select="gmd:MD_MediumFormatCode">
        	<xsl:call-template name="AnyCode"/>
        </xsl:for-each>
      </DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:density">
      <DT><B>Recording density:</B><xsl:call-template name="Real"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:densityUnits">
      <DT><B>Density units of measure:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:mediumFormat | gmd:density | gmd:densityUnits"><BR/><BR/></xsl:if>

    <xsl:for-each select="gmd:mediumNote">
      <DT><B>Limitations for using the medium:</B> <xsl:call-template name="CharacterString"/></DT>
      <xsl:if test="count (following-sibling::*) = 0"><BR/><BR/></xsl:if>
    </xsl:for-each>
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Portrayal Catalogue Reference (B.2.9 MD_PortrayalCatalogueReference - line268) -->
<xsl:template match="gmd:MD_PortrayalCatalogueReference">
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
	<!-- removed 
  <xsl:if test="(this.selectNodes('/metadata/porCatInfo').length == 1)">
      <DT><FONT color="#0000AA" size="3"><B>Portrayal Catalogue Reference:</B></FONT></DT>
  </xsl:if>
  <xsl:if test="(this.selectNodes('/metadata/porCatInfo').length > 1)">
      <DT><FONT color="#0000AA" size="3"><B>
        Portrayal Catalogue Reference - Catalogue <xsl:value-of select="position()"/>:
      </B></FONT></DT>
  </xsl:if>-->
	<DT><H2>Portrayal Catalogue Reference:</H2></DT>
  <DL>
  <DD>
    <xsl:apply-templates select="gmd:portrayalCatalogueCitation/gmd:CI_Citation"/>
  </DD>
  </DL>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

<!-- APPLICATION SCHEMA -->

<!-- Application schema Information (B.2.12 MD_ApplicationSchemaInformation - line320) -->
<xsl:template match="gmd:MD_ApplicationSchemaInformation">
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
  <xsl:if test="count (../../*[gmd:MD_ApplicationSchemaInformation]) =  1">
      <DT><H2>Application Schema Information:</H2></DT>
  </xsl:if>
  <xsl:if test="count (../../*[gmd:MD_ApplicationSchemaInformation]) &gt; 1">
      <DT><H2>Application Schema Information - Schema 
				<xsl:for-each select="..">
					<xsl:value-of select="position()"/>:
				</xsl:for-each></H2>
			</DT>
  </xsl:if>
	<DL>
	<DD>
	<xsl:apply-templates select="gmd:name/gmd:CI_Citation"/>

	<xsl:for-each select="gmd:schemaLanguage">
		<DT><B>Schema language used:</B> <xsl:call-template name="CharacterString"/></DT>
	</xsl:for-each>
	<xsl:for-each select="gmd:constraintLanguage">
		<DT><B>Formal language used in schema:</B> <xsl:call-template name="CharacterString"/></DT>
	</xsl:for-each>
	<xsl:if test="gmd:schemaLanguage | gmd:constraintLanguage"><BR/><BR/></xsl:if>

	<xsl:for-each select="gmd:schemaAscii">
		<DT><B>Schema ASCII file:</B> <xsl:call-template name="CharacterString"/></DT>
	</xsl:for-each>
	<xsl:for-each select="gmd:graphicsFile">
		<DT><B>Schema graphics file:</B> <xsl:call-template name="Boolean"/></DT>
	</xsl:for-each>
	<xsl:for-each select="gmd:softwareDevelopmentFile">
		<DT><B>Schema software development file:</B> <xsl:call-template name="Boolean"/></DT>
	</xsl:for-each>
	<xsl:for-each select="gmd:softwareDevelopmentFileFormat">
		<DT><B>Software development file format:</B> <xsl:call-template name="CharacterString"/></DT>
	</xsl:for-each>
	<xsl:if test="gmd:schemaAscii | gmd:graphicsFile | 
		gmd:softwareDevelopmentFile | gmd:softwareDevelopmentFileFormat"><BR/><BR/></xsl:if>

	</DD>
	</DL>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

<!-- Spatial Attribute Supplement Information (B.2.12.2 MD_SpatialAttributeSupplement - line332) -->
<!-- NOTE: not in ISO 19139 schemas
<xsl:template match="featCatSup">
  <DD>
    <DT><B>Feature catalogue supplement:</B></DT>
    <DD>
    <DL>
      <xsl:apply-templates select="featTypeList"/>
    </DL>
    </DD>
  </DD>
</xsl:template>-->

<!-- Feature Type List Information (B.2.12.1 MD_FeatureTypeList - line329 -->
<!-- NOTE: not in ISO 19139 schemas
<xsl:template match="featTypeList">
  <DD>
    <DT><B>Feature type list:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="spatObj">
        <DT><B>Spatial object:</B> <xsl:value-of/></DT>
      </xsl:for-each>
      <xsl:for-each select="spatSchName">
        <DT><B>Spatial schema name:</B> <xsl:value-of/></DT>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
  <BR/>
</xsl:template>-->

<!-- METADATA EXTENSIONS -->

<!-- Metadata Extension Information (B.2.11 MD_MetadataExtensionInformation - line303) -->
<xsl:template match="gmd:MD_MetadataExtensionInformation">
  <A><xsl:attribute name="NAME"><xsl:value-of select="generate-id(.)"/></xsl:attribute><HR/></A>
  <DL>
	<DT><H2>Metadata extension information:</H2></DT>
	<DL>
	<DD>
		<xsl:apply-templates select="gmd:extensionOnLineResource/gmd:CI_OnlineResource"/>
		
		<xsl:apply-templates select="gmd:extendedElementInformation/gmd:MD_ExtendedElementInformation"/>
	</DD>
	</DL>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

<!-- Extended Element Information (B.2.11.1 MD_ExtendedElementInformation - line306) -->
<xsl:template match="gmd:MD_ExtendedElementInformation">
    <DD>
    <DT><B>Extended element information:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:name">
        <DT><B>Element name:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:shortName">
        <DT><B>Short name:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:domainCode">
        <DT><B>Codelist value:</B> <xsl:call-template name="Integer"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:definition">
        <DT><B>Definition:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:name | gmd:shortName | gmd:domainCode | gmd:definition"><BR/><BR/></xsl:if>

      <xsl:for-each select="gmd:obligation/gmd:MD_ObligationCode">
        <DT><B>Obligation:</B>
					<xsl:call-template name="AnyCode"/>
        </DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:condition">
        <DT><B>Condition:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:maximumOccurrence">
        <DT><B>Maximum occurrence:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:dataType/gmd:MD_DatatypeCode">
        <DT><B>Data type:</B>
					<xsl:call-template name="AnyCode"/>
        </DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:domainValue">
        <DT><B>Domain:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:obligation | gmd:condition | gmd:maximumOccurrence | gmd:dataType | gmd:domainValue"><BR/><BR/></xsl:if>

      <xsl:for-each select="gmd:parentEntity">
        <DT><B>Parent element:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:rule">
        <DT><B>Relationship to existing elements:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:rationale">
        <DT><B>Why the element was created:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:parentEntity | gmd:rule | gmd:rationale"><BR/><BR/></xsl:if>

      <xsl:apply-templates select="gmd:source/gmd:CI_ResponsibleParty"/>
    </DL>
    </DD>
    </DD>
</xsl:template>

<!-- TEMPLATES FOR DATA TYPE CLASSES -->

<!-- CITATION AND CONTACT INFORMATION -->

<!-- Citation Information (B.3.2 CI_Citation - line359) -->
<xsl:template match="gmd:CI_Citation">
  <DD>
  <xsl:choose>
    <xsl:when test="../gmd:citation">
      <DT><B>Citation:</B></DT>
    </xsl:when>
    <xsl:when test="../gmd:thesaurusName">
      <DT><B>Thesaurus name:</B></DT>
    </xsl:when>
    <xsl:when test="../gmd:authority"> 
      <DT><B>Authority that defines the value:</B></DT>
    </xsl:when>
    <xsl:when test="../gmd:sourceCitation">
      <DT><B>Source citation:</B></DT>
    </xsl:when>
    <xsl:when test="../gmd:evaluationProcedure">
      <DT><B>Description of evaluation procedure:</B></DT>
    </xsl:when>
    <xsl:when test="../gmd:specification">
      <DT><B>Description of conformance requirements:</B></DT>
    </xsl:when>
    <xsl:when test="../gmd:parameterCitation">
      <DT><B>Georeferencing parameters citation:</B></DT>
    </xsl:when>
    <xsl:when test="../gmd:portrayalCatalogueCitation">
      <DT><B>Portrayal catalogue citation:</B></DT>
    </xsl:when>
    <xsl:when test="../gmd:featureCatalogueCitation">
      <DT><B>Feature catalogue citation:</B></DT>
    </xsl:when>
    <xsl:when test="../gmd:name">
      <DT><B>Application schema name:</B></DT>
    </xsl:when>
    <xsl:otherwise>
      <DT><B>Citation:</B></DT>
    </xsl:otherwise>
  </xsl:choose>

  <DD>
  <DL>
    <xsl:for-each select="gmd:title">
      <DT><B>Title:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:if test="gmd:alternateTitle">
	    <DT><B>Alternate titles:</B> 
	      <xsl:for-each select="gmd:alternateTitle">
	       	<xsl:call-template name="CharacterString"/><xsl:if test="following-sibling::*">, </xsl:if>
	      </xsl:for-each>
	    </DT>
    </xsl:if>
    <xsl:if test="gmd:title | gmd:alternateTitle"><BR/><BR/></xsl:if>

    <xsl:apply-templates select="gmd:date/gmd:CI_Date"/>

    <xsl:for-each select="gmd:edition">
      <DT><B>Edition:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:editionDate">
      <DT><B>Edition date:</B> <xsl:call-template name="Date_PropertyType"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:presentationForm">
      <DT><B>Presentation format:</B>
        <xsl:for-each select="gmd:CI_PresentationFormCode">
        	<xsl:call-template name="AnyCode"/>
        </xsl:for-each>
        </DT>
    </xsl:for-each>
    <xsl:if test="gmd:edition | gmd:editionDate | gmd:presentationForm"><BR/><BR/></xsl:if>
    
    <xsl:for-each select="gmd:collectiveTitle">
      <DT><B>Collection title:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:apply-templates select="gmd:series/gmd:CI_Series"/>
    <xsl:if test="gmd:collectiveTitle | gmd:series"><BR/><BR/></xsl:if>

    <xsl:for-each select="gmd:ISBN">
      <DT><B>ISBN:</B><xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:ISSN">
      <DT><B>ISSN:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:identifier/gmd:code">
      <DT><B>Unique resource identifier:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>

    <xsl:if test="gmd:ISBN | gmd:ISSN | gmd:identifier"><BR/><BR/></xsl:if>

    <xsl:for-each select="gmd:otherCitationDetails">
      <DT><B>Other citation details:</B> <xsl:call-template name="CharacterString"/></DT>
      <BR/><BR/>
    </xsl:for-each>
    
    <xsl:apply-templates select="gmd:citedResponsibleParty/gmd:CI_ResponsibleParty"/>

    <!-- NOTE: removed <xsl:if test="context()[not (text()) and not(*)]"><BR/></xsl:if>-->
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Date Information (B.3.2.3 CI_Date) -->
<xsl:template match="gmd:CI_Date">
  <DD>
    <DT><B>Reference date<xsl:for-each select="gmd:dateType/gmd:CI_DateTypeCode">
      -  <xsl:call-template name="AnyCode"/>
      </xsl:for-each>:</B>
      <xsl:for-each select="gmd:date">
        <xsl:call-template name="Date_PropertyType"/>
      </xsl:for-each>
    </DT>
  </DD>
  <xsl:if test="position()=last()"><BR/><BR/></xsl:if>
</xsl:template>

<!-- Responsible Party Information (B.3.2 CI_ResponsibleParty - line374) -->
<xsl:template match="gmd:CI_ResponsibleParty"> <!-- TODO: (gmd:contact | gmd:pointOfContact | gmd:userContactInfo | stepProc | distorCont | 
      citRespParty | extEleSrc)"> -->
  <DD>
  <DT><B>
  <xsl:choose>
    <xsl:when test="../../gmd:contact">
      Metadata contact
    </xsl:when>
    <xsl:when test="../../gmd:pointOfContact"> 
      Point of contact
    </xsl:when>
    <xsl:when test="../../gmd:userContactInfo"> 
      Party using the resource
    </xsl:when>
    <xsl:when test="../../gmd:processor">
      Process contact
    </xsl:when>
    <xsl:when test="../../gmd:distributorContact">
      Distributor information
    </xsl:when>
    <xsl:when test="../../gmd:citedResponsibleParty">
      Party responsible for the resource
    </xsl:when>
    <!-- gmd:source?? -->
    <xsl:when test="../gmd:source">
      Extension source
    </xsl:when>
    <xsl:otherwise>
      Contact
    </xsl:otherwise>
  </xsl:choose> - 
  <xsl:for-each select="gmd:role/gmd:CI_RoleCode">
	  <xsl:call-template name="AnyCode"/>
  </xsl:for-each>:</B>
  </DT>
  <DD>
  <DL>
		
		<xsl:for-each select="gmd:individualName">
			<DT><B>Individual's name:</B> <xsl:call-template name="CharacterString"/></DT>
		</xsl:for-each>
		<xsl:for-each select="gmd:organisationName">
			<DT><B>Organization's name:</B> <xsl:call-template name="CharacterString"/></DT>
		</xsl:for-each>
		<xsl:for-each select="gmd:positionName">
			<DT><B>Contact's position:</B> <xsl:call-template name="CharacterString"/></DT>
		</xsl:for-each>
		
		<xsl:if test="gmd:individualName | gmd:organisationName | gmd:positionName"><BR/><BR/></xsl:if>
		<xsl:apply-templates select="gmd:contactInfo/gmd:CI_Contact"/>
		
  </DL>
  </DD>
  </DD>
</xsl:template>

<!-- Contact Information (B.3.2.2 CI_Contact - line387) -->
<xsl:template match="gmd:CI_Contact">
  <DD>
    <DT><B>Contact information:</B></DT>
    <DD>
    <DL>
      <xsl:apply-templates select="gmd:phone/gmd:CI_Telephone"/>

      <xsl:apply-templates select="gmd:address/gmd:CI_Address"/>

      <xsl:apply-templates select="gmd:onlineResource/gmd:CI_OnlineResource"/>

      <xsl:for-each select="gmd:hoursOfService">
        <DT><B>Hours of service:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:contactInstructions">
        <DT><B>Contact instructions:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:hoursOfService | gmd:contactInstructions"><BR/><BR/></xsl:if>
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- Telephone Information (B.3.2.6 CI_Telephone - line407) -->
<xsl:template match="gmd:CI_Telephone">
  <DD>
    <DT><B>Phone:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:voice">
        <DT><B>Voice:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:facsimile">
        <DT><B>Fax:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
  <BR/>
</xsl:template>

<!-- Address Information (B.3.2.1 CI_Address - line380) -->
<xsl:template match="gmd:CI_Address">
  <DD>
    <DT><B>Address:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:deliveryPoint">
        <DT><B>Delivery point:</B></DT>
        <PRE ID="original"><xsl:call-template name="CharacterString"/></PRE>
        <SCRIPT>fix(original)</SCRIPT>
      </xsl:for-each>
      <xsl:for-each select="gmd:city">
        <DT><B>City:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:administrativeArea">
        <DT><B>Administrative area:</B><xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:postalCode">
        <DT><B>Postal code:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:country/gco:CharacterString">
        <DT><B>Country:</B>
          <!-- 2 letter country codes from ISO 3661-1, in alphabetical order by name -->
          <xsl:choose>
            <xsl:when test="(. = 'af') or (. = 'AF')">Afghanistan</xsl:when>
            <xsl:when test="(. = 'al') or (. = 'AL')">Albania</xsl:when>
            <xsl:when test="(. = 'dz') or (. = 'DZ')">Algeria</xsl:when>
            <xsl:when test="(. = 'as') or (. = 'AS')">American Samoa</xsl:when>
            <xsl:when test="(. = 'ad') or (. = 'AD')">Andorra</xsl:when>
            <xsl:when test="(. = 'ao') or (. = 'AO')">Angola</xsl:when>
            <xsl:when test="(. = 'ai') or (. = 'AI')">Anguilla</xsl:when>
            <xsl:when test="(. = 'aq') or (. = 'AQ')">Antarctica</xsl:when>
            <xsl:when test="(. = 'ag') or (. = 'AG')">Antigua and Barbuda</xsl:when>
            <xsl:when test="(. = 'ar') or (. = 'AR')">Argentina</xsl:when>
            <xsl:when test="(. = 'am') or (. = 'AM')">Armenia</xsl:when>
            <xsl:when test="(. = 'aw') or (. = 'AW')">Aruba</xsl:when>
            <xsl:when test="(. = 'au') or (. = 'AU')">Australia</xsl:when>
            <xsl:when test="(. = 'at') or (. = 'AT')">Austria</xsl:when>
            <xsl:when test="(. = 'az') or (. = 'AZ')">Azerbaijan</xsl:when>
            
            <xsl:when test="(. = 'bs') or (. = 'BS')">Bahamas</xsl:when>
            <xsl:when test="(. = 'bh') or (. = 'BH')">Bahrain</xsl:when>
            <xsl:when test="(. = 'bd') or (. = 'BD')">Bangladesh</xsl:when>
            <xsl:when test="(. = 'bb') or (. = 'BB')">Barbados</xsl:when>
            <xsl:when test="(. = 'by') or (. = 'BY')">Belarus</xsl:when>
            <xsl:when test="(. = 'be') or (. = 'BE')">Belgium</xsl:when>
            <xsl:when test="(. = 'bz') or (. = 'BZ')">Belize</xsl:when>
            <xsl:when test="(. = 'bj') or (. = 'BJ')">Benin</xsl:when>
            <xsl:when test="(. = 'bm') or (. = 'BM')">Bermuda</xsl:when>
            <xsl:when test="(. = 'bt') or (. = 'BT')">Bhutan</xsl:when>
            <xsl:when test="(. = 'bo') or (. = 'BO')">Bolivia</xsl:when>
            <xsl:when test="(. = 'ba') or (. = 'BA')">Bosnia and Herzegovina</xsl:when>
            <xsl:when test="(. = 'bw') or (. = 'BW')">Botswana</xsl:when>
            <xsl:when test="(. = 'bv') or (. = 'BV')">Bouvet Island</xsl:when>
            <xsl:when test="(. = 'br') or (. = 'BR')">Brazil</xsl:when>
            <xsl:when test="(. = 'io') or (. = 'IO')">British Indian Ocean Territory</xsl:when>
            <xsl:when test="(. = 'bn') or (. = 'BN')">Brunei Darussalam</xsl:when>
            <xsl:when test="(. = 'bg') or (. = 'BG')">Bulgaria</xsl:when>
            <xsl:when test="(. = 'bf') or (. = 'BF')">Burkina Faso</xsl:when>
            <xsl:when test="(. = 'bi') or (. = 'BI')">Burundi</xsl:when>
            
            <xsl:when test="(. = 'kh') or (. = 'KH')">Cambodia</xsl:when>
            <xsl:when test="(. = 'cm') or (. = 'CM')">Cameroon</xsl:when>
            <xsl:when test="(. = 'ca') or (. = 'CA')">Canada</xsl:when>
            <xsl:when test="(. = 'cv') or (. = 'CV')">Cape Verde</xsl:when>
            <xsl:when test="(. = 'ky') or (. = 'KY')">Cayman Islands</xsl:when>
            <xsl:when test="(. = 'cf') or (. = 'CF')">Central African Republic</xsl:when>
            <xsl:when test="(. = 'td') or (. = 'TD')">Chad</xsl:when>
            <xsl:when test="(. = 'cl') or (. = 'CL')">Chile</xsl:when>
            <xsl:when test="(. = 'cn') or (. = 'CN')">China</xsl:when>
            <xsl:when test="(. = 'cx') or (. = 'CX')">Christmas Island</xsl:when>
            <xsl:when test="(. = 'cc') or (. = 'CC')">Cocos (Keeling) Islands</xsl:when>
            <xsl:when test="(. = 'co') or (. = 'CO')">Colombia</xsl:when>
            <xsl:when test="(. = 'km') or (. = 'KM')">Comoros</xsl:when>
            <xsl:when test="(. = 'cg') or (. = 'CG')">Congo</xsl:when>
            <xsl:when test="(. = 'cd') or (. = 'CD')">Congo, Democratic Republic of the</xsl:when>
            <xsl:when test="(. = 'ck') or (. = 'CK')">Cook Islands</xsl:when>
            <xsl:when test="(. = 'cr') or (. = 'CR')">Costa Rica</xsl:when>
            <xsl:when test="(. = 'ci') or (. = 'CI')">Cote D'Ivoire</xsl:when>
            <xsl:when test="(. = 'hr') or (. = 'HR')">Croatia</xsl:when>
            <xsl:when test="(. = 'cu') or (. = 'CU')">Cuba</xsl:when>
            <xsl:when test="(. = 'cy') or (. = 'CY')">Cyprus</xsl:when>
            <xsl:when test="(. = 'cz') or (. = 'CZ')">Czech Republic</xsl:when>
            
            <xsl:when test="(. = 'dk') or (. = 'DK')">Denmark</xsl:when>
            <xsl:when test="(. = 'dj') or (. = 'DJ')">Djibouti</xsl:when>
            <xsl:when test="(. = 'dm') or (. = 'DM')">Dominica</xsl:when>
            <xsl:when test="(. = 'do') or (. = 'DO')">Dominican Republic</xsl:when>
            
            <xsl:when test="(. = 'tp') or (. = 'TP')">East Timor</xsl:when>
            <xsl:when test="(. = 'ec') or (. = 'EC')">Ecuador</xsl:when>
            <xsl:when test="(. = 'eg') or (. = 'EG')">Egypt</xsl:when>
            <xsl:when test="(. = 'sv') or (. = 'SV')">El Salvador</xsl:when>
            <xsl:when test="(. = 'gq') or (. = 'GQ')">Equatorial Guinea</xsl:when>
            <xsl:when test="(. = 'er') or (. = 'ER')">Eritrea</xsl:when>
            <xsl:when test="(. = 'ee') or (. = 'EE')">Estonia</xsl:when>
            <xsl:when test="(. = 'et') or (. = 'ET')">Ethiopia</xsl:when>
            
            <xsl:when test="(. = 'fk') or (. = 'FK')">Falkland Islands (Malvinias)</xsl:when>
            <xsl:when test="(. = 'fo') or (. = 'FO')">Faroe Islands</xsl:when>
            <xsl:when test="(. = 'fj') or (. = 'FJ')">Fiji</xsl:when>
            <xsl:when test="(. = 'fi') or (. = 'FI')">Finland</xsl:when>
            <xsl:when test="(. = 'fr') or (. = 'FR')">France</xsl:when>
            <xsl:when test="(. = 'gf') or (. = 'GF')">French Guiana</xsl:when>
            <xsl:when test="(. = 'pf') or (. = 'PF')">French Polynesia</xsl:when>
            <xsl:when test="(. = 'tf') or (. = 'TF')">French Southern Territories</xsl:when>
            
            <xsl:when test="(. = 'ga') or (. = 'GA')">Gabon</xsl:when>
            <xsl:when test="(. = 'gm') or (. = 'GM')">Gambia</xsl:when>
            <xsl:when test="(. = 'ge') or (. = 'GE')">Georgia</xsl:when>
            <xsl:when test="(. = 'de') or (. = 'DE')">Germany</xsl:when>
            <xsl:when test="(. = 'gh') or (. = 'GH')">Ghana</xsl:when>
            <xsl:when test="(. = 'gi') or (. = 'GI')">Gibraltar</xsl:when>
            <xsl:when test="(. = 'gr') or (. = 'GR')">Greece</xsl:when>
            <xsl:when test="(. = 'gl') or (. = 'GL')">Greenland</xsl:when>
            <xsl:when test="(. = 'gd') or (. = 'GD')">Grenada</xsl:when>
            <xsl:when test="(. = 'gp') or (. = 'GP')">Guadeloupe</xsl:when>
            <xsl:when test="(. = 'gu') or (. = 'GU')">Guam</xsl:when>
            <xsl:when test="(. = 'gt') or (. = 'GT')">Guatemala</xsl:when>
            <xsl:when test="(. = 'gn') or (. = 'GN')">Guinea</xsl:when>
            <xsl:when test="(. = 'gw') or (. = 'GW')">Guinea-Bissau</xsl:when>
            <xsl:when test="(. = 'gy') or (. = 'GY')">Guyana</xsl:when>
            
            <xsl:when test="(. = 'ht') or (. = 'HT')">Haiti</xsl:when>
            <xsl:when test="(. = 'hm') or (. = 'HM')">Heard Island and McDonald Islands</xsl:when>
            <xsl:when test="(. = 'va') or (. = 'VA')">Holy See / Vatican City State</xsl:when>
            <xsl:when test="(. = 'hn') or (. = 'HN')">Honduras</xsl:when>
            <xsl:when test="(. = 'hk') or (. = 'HK')">Hong Kong</xsl:when>
            <xsl:when test="(. = 'hu') or (. = 'HU')">Hungary</xsl:when>
            
            <xsl:when test="(. = 'is') or (. = 'IS')">Iceland</xsl:when>
            <xsl:when test="(. = 'in') or (. = 'IN')">India</xsl:when>
            <xsl:when test="(. = 'id') or (. = 'ID')">Indonesia</xsl:when>
            <xsl:when test="(. = 'ir') or (. = 'IR')">Iran, Islamic Republic of</xsl:when>
            <xsl:when test="(. = 'iq') or (. = 'IQ')">Iraq</xsl:when>
            <xsl:when test="(. = 'ie') or (. = 'IE')">Ireland</xsl:when>
            <xsl:when test="(. = 'il') or (. = 'IL')">Israel</xsl:when>
            <xsl:when test="(. = 'it') or (. = 'IT')">Italy</xsl:when>
            
            <xsl:when test="(. = 'jm') or (. = 'JM')">Jamaica</xsl:when>
            <xsl:when test="(. = 'jp') or (. = 'JP')">Japan</xsl:when>
            <xsl:when test="(. = 'jo') or (. = 'JO')">Jordan</xsl:when>
            
            <xsl:when test="(. = 'kz') or (. = 'KZ')">Kazakstan</xsl:when>
            <xsl:when test="(. = 'ke') or (. = 'KE')">Kenya</xsl:when>
            <xsl:when test="(. = 'ki') or (. = 'KI')">Kiribati</xsl:when>
            <xsl:when test="(. = 'kp') or (. = 'KP')">Korea, Democratic People's Republic of</xsl:when>
            <xsl:when test="(. = 'kr') or (. = 'KR')">Korea, Republic of</xsl:when>
            <xsl:when test="(. = 'kw') or (. = 'KW')">Kuwait</xsl:when>
            <xsl:when test="(. = 'kg') or (. = 'KG')">Kyrgyzstan</xsl:when>
            
            <xsl:when test="(. = 'la') or (. = 'LA')">Lao People's Demoratic Republic</xsl:when>
            <xsl:when test="(. = 'lv') or (. = 'LV')">Latvia</xsl:when>
            <xsl:when test="(. = 'lb') or (. = 'LB')">Lebanon</xsl:when>
            <xsl:when test="(. = 'ls') or (. = 'LS')">Lesotho</xsl:when>
            <xsl:when test="(. = 'lr') or (. = 'LR')">Liberia</xsl:when>
            <xsl:when test="(. = 'ly') or (. = 'LY')">Libyan Arab Jamahiriya</xsl:when>
            <xsl:when test="(. = 'li') or (. = 'LI')">Liechtenstein</xsl:when>
            <xsl:when test="(. = 'lt') or (. = 'LT')">Lithuania</xsl:when>
            <xsl:when test="(. = 'lu') or (. = 'LU')">Luxembourg</xsl:when>
            
            <xsl:when test="(. = 'mo') or (. = 'MO')">Macau</xsl:when>
            <xsl:when test="(. = 'mk') or (. = 'MK')">Macedonia, The Former Yugoslav Republic of</xsl:when>
            <xsl:when test="(. = 'mg') or (. = 'MG')">Madagascar</xsl:when>
            <xsl:when test="(. = 'mw') or (. = 'MW')">Malawi</xsl:when>
            <xsl:when test="(. = 'my') or (. = 'MY')">Malaysia</xsl:when>
            <xsl:when test="(. = 'mv') or (. = 'MV')">Maldives</xsl:when>
            <xsl:when test="(. = 'ml') or (. = 'ML')">Mali</xsl:when>
            <xsl:when test="(. = 'mt') or (. = 'MT')">Malta</xsl:when>
            <xsl:when test="(. = 'mh') or (. = 'MH')">Marshall Islands</xsl:when>
            <xsl:when test="(. = 'mq') or (. = 'MQ')">Martinique</xsl:when>
            <xsl:when test="(. = 'mr') or (. = 'MR')">Mauritania</xsl:when>
            <xsl:when test="(. = 'mu') or (. = 'MU')">Mauritius</xsl:when>
            <xsl:when test="(. = 'yt') or (. = 'YT')">Mayotte</xsl:when>
            <xsl:when test="(. = 'mx') or (. = 'MX')">Mexico</xsl:when>
            <xsl:when test="(. = 'fm') or (. = 'FM')">Micronesia, Federated States of</xsl:when>
            <xsl:when test="(. = 'md') or (. = 'MD')">Moldova, Republic of</xsl:when>
            <xsl:when test="(. = 'mc') or (. = 'MC')">Monaco</xsl:when>
            <xsl:when test="(. = 'mn') or (. = 'MN')">Mongolia</xsl:when>
            <xsl:when test="(. = 'ms') or (. = 'MS')">Montserrat</xsl:when>
            <xsl:when test="(. = 'ma') or (. = 'MA')">Morocco</xsl:when>
            <xsl:when test="(. = 'mz') or (. = 'MZ')">Mozambique</xsl:when>
            <xsl:when test="(. = 'mm') or (. = 'MM')">Myanmar</xsl:when>
            
            <xsl:when test="(. = 'na') or (. = 'NA')">Namibia</xsl:when>
            <xsl:when test="(. = 'nr') or (. = 'NR')">Nauru</xsl:when>
            <xsl:when test="(. = 'np') or (. = 'NP')">Nepal</xsl:when>
            <xsl:when test="(. = 'nl') or (. = 'NL')">Netherlands</xsl:when>
            <xsl:when test="(. = 'an') or (. = 'AN')">Netherlands Antilles</xsl:when>
            <xsl:when test="(. = 'nc') or (. = 'NC')">New Caledonia</xsl:when>
            <xsl:when test="(. = 'nz') or (. = 'NZ')">New Zealand</xsl:when>
            <xsl:when test="(. = 'ni') or (. = 'NI')">Nicaragua</xsl:when>
            <xsl:when test="(. = 'ne') or (. = 'NE')">Niger</xsl:when>
            <xsl:when test="(. = 'ng') or (. = 'NG')">Nigeria</xsl:when>
            <xsl:when test="(. = 'nu') or (. = 'NU')">Niue</xsl:when>
            <xsl:when test="(. = 'nf') or (. = 'NF')">Norfolk Island</xsl:when>
            <xsl:when test="(. = 'mp') or (. = 'MP')">Northern Mariana Islands</xsl:when>
            <xsl:when test="(. = 'no') or (. = 'NO')">Norway</xsl:when>
            
            <xsl:when test="(. = 'om') or (. = 'OM')">Oman</xsl:when>
            
            <xsl:when test="(. = 'pk') or (. = 'PK')">Pakistan</xsl:when>
            <xsl:when test="(. = 'pw') or (. = 'PW')">Palau</xsl:when>
            <xsl:when test="(. = 'ps') or (. = 'PS')">Palestinian Territory, Occupied</xsl:when>
            <xsl:when test="(. = 'pa') or (. = 'PA')">Panama</xsl:when>
            <xsl:when test="(. = 'pg') or (. = 'PG')">Papua New Guinea</xsl:when>
            <xsl:when test="(. = 'py') or (. = 'PY')">Paraguay</xsl:when>
            <xsl:when test="(. = 'pe') or (. = 'PE')">Peru</xsl:when>
            <xsl:when test="(. = 'ph') or (. = 'PH')">Phillippines</xsl:when>
            <xsl:when test="(. = 'pn') or (. = 'PN')">Pitcairn</xsl:when>
            <xsl:when test="(. = 'pl') or (. = 'PL')">Poland</xsl:when>
            <xsl:when test="(. = 'pt') or (. = 'PT')">Portugal</xsl:when>
            <xsl:when test="(. = 'pr') or (. = 'PR')">Puerto Rico</xsl:when>
            
            <xsl:when test="(. = 'qa') or (. = 'QA')">Qatar</xsl:when>
            
            <xsl:when test="(. = 're') or (. = 'RE')">Reunion</xsl:when>
            <xsl:when test="(. = 'ro') or (. = 'RO')">Romania</xsl:when>
            <xsl:when test="(. = 'ru') or (. = 'RU')">Russian Federation</xsl:when>
            <xsl:when test="(. = 'rw') or (. = 'RW')">Rwanda</xsl:when>
            
            <xsl:when test="(. = 'sh') or (. = 'SH')">Saint Helena</xsl:when>
            <xsl:when test="(. = 'kn') or (. = 'KN')">Saint Kitts and Nevis</xsl:when>
            <xsl:when test="(. = 'lc') or (. = 'LC')">Saint Lucia</xsl:when>
            <xsl:when test="(. = 'pm') or (. = 'PM')">Saint Pierre and Miquelon</xsl:when>
            <xsl:when test="(. = 'vc') or (. = 'VC')">Saint Vincent and the Grenadines</xsl:when>
            <xsl:when test="(. = 'ws') or (. = 'WS')">Samoa</xsl:when>
            <xsl:when test="(. = 'sm') or (. = 'SM')">San Marino</xsl:when>
            <xsl:when test="(. = 'st') or (. = 'ST')">Sao Tome and Principe</xsl:when>
            <xsl:when test="(. = 'sa') or (. = 'SA')">Saudi Arabia</xsl:when>
            <xsl:when test="(. = 'sn') or (. = 'SN')">Senegal</xsl:when>
            <xsl:when test="(. = 'sc') or (. = 'SC')">Seychelles</xsl:when>
            <xsl:when test="(. = 'sl') or (. = 'SL')">Sierra Leone</xsl:when>
            <xsl:when test="(. = 'sg') or (. = 'SG')">Singapore</xsl:when>
            <xsl:when test="(. = 'sk') or (. = 'SK')">Slovakia</xsl:when>
            <xsl:when test="(. = 'si') or (. = 'SI')">Slovenia</xsl:when>
            <xsl:when test="(. = 'sb') or (. = 'SB')">Solomon Islands</xsl:when>
            <xsl:when test="(. = 'so') or (. = 'S0')">Somalia</xsl:when>
            <xsl:when test="(. = 'za') or (. = 'ZA')">South Africa</xsl:when>
            <xsl:when test="(. = 'gs') or (. = 'GS')">South Georgia and the South Sandwich Islands</xsl:when>
            <xsl:when test="(. = 'es') or (. = 'ES')">Spain</xsl:when>
            <xsl:when test="(. = 'lk') or (. = 'LK')">Sri Lanka</xsl:when>
            <xsl:when test="(. = 'sd') or (. = 'SD')">Sudan</xsl:when>
            <xsl:when test="(. = 'sr') or (. = 'SR')">Suriname</xsl:when>
            <xsl:when test="(. = 'sj') or (. = 'SJ')">Svalbard and Jan Mayen</xsl:when>
            <xsl:when test="(. = 'sz') or (. = 'SZ')">Swaziland</xsl:when>
            <xsl:when test="(. = 'se') or (. = 'SE')">Sweden</xsl:when>
            <xsl:when test="(. = 'ch') or (. = 'CH')">Switzerland</xsl:when>
            <xsl:when test="(. = 'sy') or (. = 'SY')">Syrian Arab Republic</xsl:when>
            
            <xsl:when test="(. = 'tw') or (. = 'TW')">Taiwan, Province of China</xsl:when>
            <xsl:when test="(. = 'tj') or (. = 'TJ')">Tajikistan</xsl:when>
            <xsl:when test="(. = 'tz') or (. = 'TZ')">Tanzania, United Republic of</xsl:when>
            <xsl:when test="(. = 'th') or (. = 'TH')">Thailand</xsl:when>
            <xsl:when test="(. = 'tg') or (. = 'TG')">Togo</xsl:when>
            <xsl:when test="(. = 'tk') or (. = 'TK')">Tokelau</xsl:when>
            <xsl:when test="(. = 'to') or (. = 'TO')">Tonga</xsl:when>
            <xsl:when test="(. = 'tt') or (. = 'TT')">Trinidad and Tobago</xsl:when>
            <xsl:when test="(. = 'tn') or (. = 'TN')">Tunisia</xsl:when>
            <xsl:when test="(. = 'tr') or (. = 'TR')">Turkey</xsl:when>
            <xsl:when test="(. = 'tm') or (. = 'TM')">Turkmenistan</xsl:when>
            <xsl:when test="(. = 'tc') or (. = 'TC')">Turks and Caicos Islands</xsl:when>
            <xsl:when test="(. = 'tv') or (. = 'TV')">Tuvalu</xsl:when>
            
            <xsl:when test="(. = 'ug') or (. = 'UG')">Uganda</xsl:when>
            <xsl:when test="(. = 'ua') or (. = 'UA')">Ukraine</xsl:when>
            <xsl:when test="(. = 'ae') or (. = 'AE')">United Arab Emirates</xsl:when>
            <xsl:when test="(. = 'gb') or (. = 'GB')">United Kingdom</xsl:when>
            <xsl:when test="(. = 'us') or (. = 'US')">United States</xsl:when>
            <xsl:when test="(. = 'um') or (. = 'UM')">United States Minor Outlying Islands</xsl:when>
            <xsl:when test="(. = 'uy') or (. = 'UY')">Uruguay</xsl:when>
            <xsl:when test="(. = 'uz') or (. = 'UZ')">Uzbekistan</xsl:when>
            
            <xsl:when test="(. = 'vu') or (. = 'VU')">Vanuatu</xsl:when>
            <xsl:when test="(. = 've') or (. = 'VE')">Venezuela</xsl:when>
            <xsl:when test="(. = 'vn') or (. = 'VN')">Viet Nam</xsl:when>
            <xsl:when test="(. = 'vg') or (. = 'VG')">Virgin Islands, British</xsl:when>
            <xsl:when test="(. = 'vi') or (. = 'VI')">Virgin Islands, U.S.</xsl:when>
            
            <xsl:when test="(. = 'wf') or (. = 'WF')">Wallis and Futuna</xsl:when>
            <xsl:when test="(. = 'eh') or (. = 'EH')">Western Sahara</xsl:when>
            
            <xsl:when test="(. = 'ye') or (. = 'YE')">Yemen</xsl:when>
            <xsl:when test="(. = 'yu') or (. = 'YU')">Yugoslavia</xsl:when>
            
            <xsl:when test="(. = 'zm') or (. = 'ZM')">Zambia</xsl:when>
            <xsl:when test="(. = 'zw') or (. = 'ZW')">Zimbabwe</xsl:when>
            
            <xsl:otherwise><xsl:value-of select="."/></xsl:otherwise>
	   </xsl:choose>
        </DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:electronicMailAddress">
        <DT><B>e-mail address:</B> <xsl:value-of select="."/></DT>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
  <BR/>
</xsl:template>

<!-- Online Resource Information (B.3.2.4 CI_OnlineResource - line396) -->
<xsl:template match="gmd:CI_OnlineResource">
  <DD>
	<DT><B>Online resource:</B></DT>
  <DD>
  <DL>
    <xsl:for-each select="gmd:name">
      <DT><B>Name of resource:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:linkage/gmd:URL">
      <DT><B>Online location:</B> <xsl:value-of select="."/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:protocol">
      <DT><B>Connection protocol:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:function/gmd:CI_OnLineFunctionCode">
      <DT><B>Function performed:</B> <xsl:call-template name="AnyCode"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:description">
      <DT><B>Description:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
    <xsl:for-each select="gmd:applicationProfile">
      <DT><B>Application profile:</B> <xsl:call-template name="CharacterString"/></DT>
    </xsl:for-each>
  </DL>
  </DD>
  </DD>
  <BR/>
</xsl:template>

<!-- Series Information (B.3.2.5 CI_Series - line403) -->
<xsl:template match="gmd:CI_Series">
  <DD>
    <DT><B>Series:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:name">
        <DT><B>Name:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:issueIdentification">
        <DT><B>Issue:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:page">
        <DT><B>Pages:</B> <xsl:call-template name="CharacterString"/></DT>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
  <BR/>
</xsl:template>
	
<!-- EXTENT INFORMATION -->

<!-- Extent Information (B.3.1 EX_Extent - line334) -->
<xsl:template match="gmd:EX_Extent"><!-- NOTE: was (dataExt | scpExt | srcExt)">-->
  <DD>
	<!-- TODO: show more descriptive text
  <xsl:choose>
    <xsl:when test="../dataExt">
      <DT><B>Other extent information:</B></DT>
    </xsl:when>
    <xsl:when test="../scpExt">
      <DT><B>Scope extent:</B></DT>
    </xsl:when>
    <xsl:when test="../srcExt">
      <DT><B>Extent of the source data:</B></DT>
    </xsl:when>
    <xsl:otherwise>
      <DT><B>Extent:</B></DT>
    </xsl:otherwise>
  </xsl:choose>-->
	<DT><B>Extent:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:description">
        <DT><B>Extent description:</B></DT>
        <PRE ID="original"><xsl:call-template name="CharacterString"/></PRE>
        <SCRIPT>fix(original)</SCRIPT>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="gmd:geographicElement">
        <DT><B>Geographic extent:</B></DT>
        <DD>
          <DD>
          <DL>
						<!-- 
								NOTE: can call sub-classes' templates:
								EX_BoundingPolygon, EX_GeographicBoundingBox, EX_GeographicDescription
						-->
						<xsl:apply-templates select="*"/>
          </DL>
          </DD>
        </DD>
<!--        <xsl:if test="not (following-sibling::*)"><BR/></xsl:if> -->
      </xsl:for-each>

      <xsl:for-each select="gmd:temporalElement">
				<!-- 
						NOTE: can call sub-classes' templates:
						EX_TemporalExtent, EX_SpatialTemporalExtent
				-->
				<xsl:apply-templates select="*"/>
				<BR/><BR/>
      </xsl:for-each>

      <xsl:apply-templates select="gmd:verticalElement/gmd:EX_VerticalExtent"/>
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- Bounding Polygon Information (B.3.1.1 EX_BoundingPolygon - line341) -->
<xsl:template match="gmd:EX_BoundingPolygon">
  <DD>
  <DT><B>Bounding polygon:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:extentTypeCode">
        <DT><B>Extent contains the resource:</B>
        	<xsl:call-template name="Boolean"/>
        </DT>      
      </xsl:for-each>
			<xsl:for-each select="gmd:polygon/*">
        <DT><B>Geometry:</B> <xsl:call-template name="AbstractGeometry"/></DT>
      </xsl:for-each>
      <xsl:if test="gmd:extentTypeCode | gmd:polygon"><BR/><BR/></xsl:if>
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- Bounding Box Information (B.3.1.1 EX_GeographicBoundingBox - line343) -->
<xsl:template match="gmd:EX_GeographicBoundingBox">
  <DD>
	<DT><B>Bounding rectangle:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:extentTypeCode">
        <DT><B>Extent contains the resource:</B>
        	<xsl:call-template name="Boolean"/>
        </DT>      
      </xsl:for-each>
      <xsl:for-each select="gmd:westBoundLongitude">
        <DT><B>West longitude:</B> <xsl:call-template name="Decimal"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:eastBoundLongitude">
        <DT><B>East longitude:</B> <xsl:call-template name="Decimal"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:northBoundLatitude">
        <DT><B>North latitude:</B> <xsl:call-template name="Decimal"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:southBoundLatitude">
        <DT><B>South latitude:</B> <xsl:call-template name="Decimal"/></DT>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
  <BR/>
</xsl:template>

<!-- Geographic Description Information (B.3.1.1 EX_GeographicDescription - line348) -->
<xsl:template match="EX_GeographicDescription">
  <DD>
	<DT><B>Geographic description:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:extentTypeCode">
        <DT><B>Extent contains the resource:</B>
        	<xsl:call-template name="Boolean"/>
        </DT>      
      </xsl:for-each>
      <xsl:apply-templates select="gmd:geographicIdentifier/gmd:MD_Identifier"/>
      <xsl:if test="(gmd:extentTypeCode) and not (gmd:geographicIdentifier)"><BR/><BR/></xsl:if>
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- Temporal Extent Information (B.3.1.2 EX_TemporalExtent - line350) -->
<xsl:template match="gmd:EX_TemporalExtent">
  <DD>
		<DT><B>Temporal extent:</B></DT>
		<!-- NOTE: select sub-classes of gml:AbstractTimePrimitive -->
		<xsl:apply-templates select="gmd:extent/*"/>
  </DD>
</xsl:template>


<!-- temporal extent Information from ISO 19103 as defined is DTD -->
<xsl:template match="gml:TimePeriod">
  <DD>
  <DL>
	<xsl:for-each select="gml:beginPosition">
		<DT><B>Beginning date:</B><xsl:call-template name="TimeInstant"/></DT>
	</xsl:for-each>
	<xsl:for-each select="gml:begin/gml:TimeInstant">
		<DT><B>Beginning date:</B><xsl:call-template name="TimeInstant"/></DT>
	</xsl:for-each>
	<xsl:for-each select="gml:endPosition">
		<DT><B>Ending date:</B><xsl:call-template name="TimeInstant"/></DT>
	</xsl:for-each>
	<xsl:for-each select="gml:end/gml:TimeInstant">
		<DT><B>Ending date:</B><xsl:call-template name="TimeInstant"/></DT>
	</xsl:for-each>
	</DL>
  </DD>
  <BR/>
</xsl:template>

<!-- temporal extent Information from ISO 19103 as defined is DTD -->
<xsl:template match="gml:TimeInstant">
  <DD>
  <DL>
  <xsl:for-each select="gml:timePosition">
		<DT><B>Date/Time:</B>
			<xsl:call-template name="TimeInstant"/>
		</DT>
  </xsl:for-each>
  </DL>
  </DD>
  <BR/>
</xsl:template>

<!-- Spatial Temporal Extent Information (B.3.1.2 EX_SpatialTemporalExtent - line352) -->
<xsl:template match="gmd:EX_SpatialTemporalExtent">
  <DD>
  <DT><B>Spatial and temporal extent:</B></DT>
    <DD>
    <DL>
			<!-- NOTE: select sub-classes of gml:AbstractTimePrimitive -->
			<xsl:apply-templates select="gmd:extent/*"/>-->

      <xsl:for-each select="gmd:spatialExtent">
        <DT><B>Spatial extent:</B></DT>
        <DD>
          <DD>
          <DL>
						<!-- 
								NOTE: can call sub-classes' templates:
								EX_BoundingPolygon, EX_GeographicBoundingBox, EX_GeographicDescription
						-->
						<xsl:apply-templates select="*"/>
          </DL>
          </DD>
        </DD>
      </xsl:for-each>
    </DL>
    </DD>
  </DD>
</xsl:template>

<!-- Vertical Extent Information (B.3.1.3 EX_VerticalExtent - line354) -->
<xsl:template match="gmd:EX_VerticalExtent">
  <DD>
  <DT><B>Vertical extent:</B></DT>
    <DD>
    <DL>
      <xsl:for-each select="gmd:minimumValue">
        <DT><B>Minimum value:</B><xsl:call-template name="Real"/></DT>
      </xsl:for-each>
      <xsl:for-each select="gmd:maximumValue">
        <DT><B>Maximum value:</B><xsl:call-template name="Real"/></DT>
      </xsl:for-each>
			
			<!-- 19139 uses GML here instead of ISO 19115's unitOfMeasure (UomLength) -->
			<xsl:if test="gmd:verticalCRS">
				<xsl:for-each select="gmd:verticalCRS/*">
					<!-- TODO: review this -->
					<DT><B>Coordinate reference system: </B> <xsl:call-template name="AbstractCRS"/></DT>
				</xsl:for-each>
			</xsl:if>
	
      <xsl:if test="(gmd:minimumValue | gmd:maximumValue) and not (gmd:verticalCRS)"><BR/><BR/></xsl:if>

    </DL>
    </DD>
  </DD>
  <xsl:if test="not (*)"><BR/></xsl:if>
</xsl:template>

<!-- gml:TimeInstant -->
<xsl:template name="TimeInstant">
	<!-- NOTE: ignoring attributes: frame, calendarEraName, indeterminatePosition -->
	<xsl:text>&#x20;</xsl:text><xsl:value-of select="."/>
</xsl:template>

<!-- gco:Boolean -->
<xsl:template name="Boolean">
	<xsl:for-each select="gco:Boolean">
		<xsl:text>&#x20;</xsl:text><xsl:value-of select="."/>
	</xsl:for-each>
</xsl:template>

<!-- gco:CodeListValue_Type -->
<xsl:template name="AnyCode">
	<xsl:text>&#x20;</xsl:text><xsl:value-of select="(@codeListValue | text())[1]"/>
</xsl:template>

<!-- gco:Measure -->
<xsl:template match="gco:Measure">
	<!-- NOTE: uom attribute supressed -->
	<xsl:text>&#x20;</xsl:text><xsl:value-of select="."/>
</xsl:template>

<!-- gco:Integer -->
<xsl:template name="Integer">
	<xsl:for-each select="gco:Integer">
		<xsl:text>&#x20;</xsl:text><xsl:value-of select="."/>
	</xsl:for-each>
</xsl:template>

<!-- gco:Real -->
<xsl:template name="Real">
	<xsl:for-each select="gco:Real">
		<xsl:text>&#x20;</xsl:text><xsl:value-of select="."/>
	</xsl:for-each>
</xsl:template>

<!-- gco:Decimal -->
<xsl:template name="Decimal">
	<xsl:text>&#x20;</xsl:text><xsl:value-of select="gco:Decimal"/>
</xsl:template>

<!-- gco:Date_PropertyType -->
<xsl:template name="Date_PropertyType">
	<xsl:text>&#x20;</xsl:text><xsl:value-of select="(gco:Date | gco:DateTime)[1]"/>
</xsl:template>

<!-- gco:CharacterString , gco:FreeText -->
<xsl:template name="CharacterString">
	<xsl:for-each select="*">
		<xsl:choose>
			<xsl:when test="local-name(.) = 'CharacterString'">
				<xsl:text>&#x20;</xsl:text><xsl:value-of select="."/>
			</xsl:when>
			<xsl:when test="local-name(.) = 'PT_FreeText'">
				<xsl:text>&#x20;</xsl:text><xsl:value-of select="gmd:textGroup/gmd:LocalisedCharacterString"/>
			</xsl:when>
		</xsl:choose>
	</xsl:for-each>
</xsl:template>

<!-- gco:Record -->
<xsl:template name="Record">
	<!-- NOTE: this has no content model in the ISO 19139 schemas -->
	<xsl:for-each select="gco:Record">
		<xsl:apply-templates select="@* | *"/>
	</xsl:for-each>
</xsl:template>

<!-- gml:Point -->
<xsl:template match="gml:Point">
	<!-- NOTE: ignoring attribute-group gml:SRSReferenceGroup -->
	<xsl:text>&#x20;</xsl:text><xsl:value-of select="(gml:pos | gml:coordinates)[1]"/>
</xsl:template>

<!-- gml:UnitDefinitionType -->
<xsl:template match="gml:UnitDefinition">
	<!-- NOTE: there are lots of elements and attributes not included -->
	<DD>
  <DL>
    <xsl:for-each select="gml:name">
       <DT><B>Units:</B><xsl:text>&#x20;</xsl:text><xsl:value-of select="."/></DT>
    </xsl:for-each>
  </DL>
  </DD>
  <BR/>
</xsl:template>

<!-- gml:AbstractGeometry -->
<xsl:template name="AbstractGeometry">
	<!-- NOTE: no implementation -->
</xsl:template>

<!-- gml:AbstractCRS -->
<xsl:template name="AbstractCRS">
	<!-- NOTE: no implementation -->
</xsl:template>

<!-- gco:Distance -->
<xsl:template match="gco:Distance">
	<xsl:text>&#x20;</xsl:text><xsl:value-of select="."/>
</xsl:template>

<!-- gco:RecordType -->
<xsl:template name="RecordType">
	<!-- NOTE: attribute-group xlink:simpleLink ignored -->
	<xsl:for-each select="gco:RecordType">
		<xsl:text>&#x20;</xsl:text><xsl:value-of select="."/> 
	</xsl:for-each>
</xsl:template>

<!-- gco:LocalName, gco:ScopedName -->
<xsl:template match="gco:LocalName | gco:ScopedName">
	<!-- NOTE: supressing codeSpace attribute -->	
	<DD>
	<DL>
	<DT><B>Scope:</B><xsl:text>&#x20;</xsl:text><xsl:value-of select="."/></DT>
  </DL>
	</DD>
	<BR/>
</xsl:template>

</xsl:stylesheet>