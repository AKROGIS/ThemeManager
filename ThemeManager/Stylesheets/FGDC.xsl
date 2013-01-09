<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/TR/WD-xsl" TYPE="text/javascript">

<!-- An xsl template for displaying FGDC and ESRI metadata in ArcCatalog 
     with similar style to traditional FGDC look but without italics.
     Shows all elements in FGDC standard and ESRI profile, and indicates
     which elements have been automatically updated by ArcCatalog.

     Copyright (c) 2001-2008, Environmental Systems Research Institute, Inc. All rights reserved.
     	
     Revision History: Created 6/21/01 avienneau
                        Updated 1/18/04 avienneau - added geoprocessing history
						Updated 8/30/06 avienneau - added locator and terrain information
						Updated 3/21/08 avienneau - updated link to HTML version of the FGDC CSDGM standard
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
  </HEAD>

  <BODY BGCOLOR="#FFFFFF" ONCONTEXTMENU="return true">
  <FONT COLOR="00008B" SIZE="2" FACE="Verdana">
     

    <A name="Top"/>

    <!-- show metadata summary -->
    <xsl:if test="/metadata[($any$ (idinfo/citation/citeinfo/title/text() | 
          Binary/Thumbnail/img | idinfo/browse/img | idinfo/natvform/text() | 
          idinfo/citation/citeinfo/ftname/text() | spref/horizsys/(geograph/*/text() | 
          planar/(mapproj/mapprojn/text() | gridsys/gridsysn/text() | localp/*/text()) | 
          local/*/text()) | idinfo/keywords/theme/themekey/text() | idinfo/descript/abstract/text()))]">

      <TABLE COLS="2" WIDTH="100%" BGCOLOR="#CCFFCC" CELLPADDING="11" BORDER="0" CELLSPACING="0">

        <!-- show title -->
        <xsl:if test="/metadata/idinfo/citation/citeinfo/title[text()]">
          <TR ALIGN="center" VALIGN="center">
            <TD COLSPAN="2">
              <FONT COLOR="#006400" FACE="Verdana" SIZE="4">
                <xsl:for-each select="/metadata/idinfo/citation/citeinfo/title[text()]">
                  <B><xsl:value-of /></B>
                </xsl:for-each>
              </FONT>
            </TD>
          </TR>
        </xsl:if>

        <xsl:if test="/metadata[($any$ Binary/Thumbnail/img | idinfo/browse/img |
              idinfo/natvform/text() | idinfo/citation/citeinfo/ftname/text() | 
              spref/horizsys/(geograph/*/text() | planar/(mapproj/mapprojn/text() | 
              gridsys/gridsysn/text() | localp/*/text()) | local/*/text()) | 
              idinfo/keywords/theme/themekey/text())]">

          <TR ALIGN="left" VALIGN="top">

            <!-- show thumbnail  -->
            <xsl:if test="/metadata[($any$ Binary/Thumbnail/img | idinfo/browse/img)]">
              <TD>
                <xsl:choose>
                  <xsl:when test="/metadata[($any$ idinfo/natvform/text() |
                        idinfo/citation/citeinfo/ftname/text() | 
                        spref/horizsys/(geograph/text() | planar/(mapproj/mapprojn/text() | 
                        gridsys/gridsysn/text() | localp/*/text()) | local/*/text()) | 
                        idinfo/keywords/theme/themekey/text())]">
                    <xsl:attribute name="WIDTH">210</xsl:attribute>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:attribute name="ALIGN">center</xsl:attribute>
                  </xsl:otherwise>
                </xsl:choose>

                <FONT COLOR="#006400" FACE="Verdana" SIZE="2">
                  <xsl:apply-templates select="/metadata//img[@src]" />
                  <xsl:if test="context()[not(/metadata/idinfo/descript/abstract/text())]"><BR/></xsl:if>
                </FONT>
              </TD>
            </xsl:if>

            <!-- show format, file name, coordinate system, theme keywords  -->
            <xsl:if test="/metadata[($any$ idinfo/natvform/text() | 
                  idinfo/citation/citeinfo/ftname/text() | 
                  spref/horizsys/(geograph/*/text() | planar/(mapproj/mapprojn/text() | 
                  gridsys/gridsysn/text() | localp/*/text()) | local/*/text()) | 
                  idinfo/keywords/theme/themekey/text())]">
              <TD>
                <FONT COLOR="#006400" FACE="Verdana" SIZE="2">
                  <xsl:for-each select="/metadata/idinfo/natvform[text()]">
                    <P>
                      <B>Data format:</B> <xsl:value-of />
                      <xsl:if test="/metadata[(idinfo/natvform = 'Raster Dataset') 
                            and (spdoinfo/rastinfo/rastifor/text())]">
                        - <xsl:value-of select="/metadata/spdoinfo/rastinfo/rastifor" />
                      </xsl:if>
                    </P>
                  </xsl:for-each>
                  
                  <xsl:for-each select="/metadata/idinfo/citation/citeinfo/ftname[text()]">
                    <P><B>File or table name:</B> <xsl:value-of /></P>
                  </xsl:for-each>

                  <xsl:if test="context()[/metadata/spref/horizsys/geograph/*/text() or 
                        /metadata/spref/horizsys/planar/mapproj/mapprojn/text() or 
                        /metadata/spref/horizsys/planar/gridsys/gridsysn/text() or 
                        /metadata/spref/horizsys/planar/localp/*/text() or 
                        /metadata/spref/horizsys/local/*/text() ]">
                    <P><B>Coordinate system:</B> 
                      <xsl:for-each select="/metadata/spref/horizsys/geograph[*/text()]">Geographic </xsl:for-each>
                      <xsl:value-of select="/metadata/spref/horizsys/planar/mapproj/mapprojn[text()]"/>
                      <xsl:value-of select="/metadata/spref/horizsys/planar/gridsys/gridsysn[text()]"/>
                      <xsl:for-each select="/metadata/spref/horizsys/planar/localp[*/text()]">Local planar </xsl:for-each>
                      <xsl:for-each select="/metadata/spref/horizsys/local[*/text()]">Local </xsl:for-each>
                    </P>
                  </xsl:if>

                  <xsl:if test="context()[(/metadata/idinfo/keywords/theme/themekey[(text())
                        and (. != 'REQUIRED: Common-use word or phrase used to describe the subject of the data set.')
                        and (. != 'Common-use word or phrase used to describe the subject of the data set.  REQUIRED.')])]">
                    <P><B>Theme keywords:</B>
                      <xsl:for-each select="/metadata/idinfo/keywords/theme[themekey/text()]">
                        <xsl:for-each select="themekey[text()]">
                          <xsl:value-of /><xsl:if test="context()[not(end()) and (text())]">, </xsl:if>
                        </xsl:for-each><xsl:if test="context()[not(end())]">, </xsl:if>
                      </xsl:for-each>
                    </P>
                  </xsl:if>

<!--                  <xsl:if test="/metadata[idinfo/citation/citeinfo/onlink/text() or 
                      distinfo/stdorder/digform/digtopt/onlinopt/computer/networka/networkr/text()]">
                    <P><B>Location:</B> 
                      <xsl:for-each select="/metadata/idinfo/citation/citeinfo/onlink[text()]"><xsl:value-of /></xsl:for-each>
                      <xsl:for-each select="/metadata/distinfo/stdorder/digform/digtopt/onlinopt/computer/networka/networkr[text()]"><xsl:value-of /></xsl:for-each>
                    </P>
                  </xsl:if> -->

                  <xsl:if test="context()[not( /metadata/idinfo/descript/abstract/text() )]"><P/></xsl:if>
                </FONT>
              </TD>
            </xsl:if>
          </TR>
        </xsl:if>
        <xsl:if test="/metadata/idinfo/descript/abstract[text()]">
          <TR>
            <xsl:for-each select="/metadata/idinfo/descript/abstract[text()]">
              <TD  COLSPAN="2">
                <FONT COLOR="#006400" FACE="Verdana" SIZE="2">
                  <B>Abstract:</B> <xsl:value-of /><BR/><BR/>
                </FONT>
              </TD>
            </xsl:for-each>
          </TR>
        </xsl:if>
      </TABLE>
    </xsl:if>

    <H3>FGDC and ESRI Metadata:</H3>
    <UL>
      <xsl:for-each select="metadata/idinfo">
        <LI><A HREF="#Identification_Information">Identification Information</A></LI>
      </xsl:for-each>
      <xsl:for-each select="metadata/dataqual">
        <LI><A HREF="#Data_Quality_Information">Data Quality Information</A></LI>
      </xsl:for-each>
      <xsl:for-each select="metadata/spdoinfo">
        <LI><A HREF="#Spatial_Data_Organization_Information">Spatial Data Organization Information</A></LI>
      </xsl:for-each>
      <xsl:for-each select="metadata/spref">
        <LI><A HREF="#Spatial_Reference_Information">Spatial Reference Information</A></LI>
      </xsl:for-each>
      <xsl:for-each select="metadata/eainfo">
        <LI><A HREF="#Entity_and_Attribute_Information">Entity and Attribute Information</A></LI>
      </xsl:for-each>
      
      <xsl:if expr="(this.selectNodes('metadata/distinfo').length == 1)">
        <xsl:for-each select="metadata/distinfo">
          <LI><A>
            <xsl:attribute name="HREF">#<xsl:eval>uniqueID(this)</xsl:eval></xsl:attribute>
            Distribution Information
          </A></LI>
        </xsl:for-each>
      </xsl:if>
      <xsl:if expr="(this.selectNodes('metadata/distinfo').length > 1)">
        <LI>Distribution Information</LI>
        <xsl:for-each select="metadata/distinfo">
          <LI STYLE="margin-left:0.3in"><A>
            <xsl:attribute name="HREF">#<xsl:eval>uniqueID(this)</xsl:eval></xsl:attribute>
           Distributor <xsl:eval>formatIndex(childNumber(this), "1")</xsl:eval>
          </A></LI>
        </xsl:for-each>
      </xsl:if>

      <xsl:for-each select="metadata/metainfo">
        <LI><A HREF="#Metadata_Reference_Information">Metadata Reference Information</A></LI>
      </xsl:for-each>
      <xsl:for-each select="metadata/Esri/DataProperties/lineage">
        <LI><A HREF="#Geoprocessing">Geoprocessing History</A></LI>
      </xsl:for-each>
      <xsl:for-each select="metadata/Esri/Locator">
        <LI><A HREF="#Locator">Locator Information</A></LI>
      </xsl:for-each>
      <xsl:for-each select="metadata/Binary">
        <LI><A HREF="#Binary_Enclosures">Binary Enclosures</A></LI>
      </xsl:for-each>
    </UL>

    <BLOCKQUOTE><FONT SIZE="1">
      Metadata elements shown with blue text are defined in the
      Federal Geographic Data Committee's (FGDC) <A TARGET="viewer"
      HREF="http://www.fgdc.gov/metadata/csdgm/index_html">
      <I>Content Standard for Digital Geospatial Metadata (CSDGM)</I></A>.
      Elements shown with <FONT color="#006400">green</FONT>
      text are defined in the <A TARGET="viewer" 
      HREF="http://www.esri.com/metadata/esriprof80.html">
      <I>ESRI Profile of the CSDGM</I>.</A>
      Elements shown with a green asterisk (<FONT color="#006400">*</FONT>)
      will be automatically updated by ArcCatalog.
      ArcCatalog adds hints indicating which FGDC elements are mandatory; 
      these are shown with <FONT color="#999999">gray</FONT> text.
    </FONT></BLOCKQUOTE>

    <xsl:apply-templates select="metadata/idinfo"/>
    <xsl:apply-templates select="metadata/dataqual"/>
    <xsl:apply-templates select="metadata/spdoinfo"/>
    <xsl:apply-templates select="metadata/spref"/>
    <xsl:apply-templates select="metadata/eainfo"/>
    <xsl:apply-templates select="metadata/distinfo"/>
    <xsl:apply-templates select="metadata/metainfo"/>
    <xsl:apply-templates select="metadata/Esri/DataProperties/lineage"/>
    <xsl:apply-templates select="metadata/Esri/Locator"/>
    <xsl:apply-templates select="metadata/Binary"/>

  </FONT>


  <!-- <BR/><BR/><BR/><CENTER><FONT COLOR="#6495ED">Metadata stylesheets are provided courtesy of ESRI.  Copyright (c) 2001-2004, Environmental Systems Research Institute, Inc.  All rights reserved.</FONT></CENTER> -->

  </BODY>
  </HTML>
</xsl:template>

================================

<!-- Thumbnail -->
<xsl:template match="/metadata/Binary/Thumbnail/img[@src]">
      <IMG ID="thumbnail" ALIGN="absmiddle" STYLE="width:217; 
          border:'2 outset #FFFFFF'; position:relative">
        <xsl:attribute name="SRC"><xsl:value-of select="@src"/></xsl:attribute>
      </IMG>
</xsl:template>
<xsl:template match="/metadata/idinfo/browse/img[@src]">
  <xsl:if test="context()[not (/metadata/Binary/Thumbnail/img)]">
      <xsl:if test="../@BrowseGraphicType[. = 'Thumbnail']">
        <IMG ID="thumbnail" ALIGN="absmiddle" STYLE="height:144; 
            border:'2 outset #FFFFFF'; position:relative">
          <xsl:attribute name="SRC"><xsl:value-of select="@src"/></xsl:attribute>
        </IMG>
        <BR/><BR/>
      </xsl:if>
      <xsl:if test="context()[not (../../browse/@BrowseGraphicType)]">
        <IMG ID="thumbnail" ALIGN="absmiddle" STYLE="height:144; 
            border:'2 outset #FFFFFF'; position:relative">
          <xsl:attribute name="SRC"><xsl:value-of select="@src"/></xsl:attribute>
        </IMG>
        <BR/><BR/>
      </xsl:if>
  </xsl:if>
</xsl:template>

--------

<!-- Identification -->
<xsl:template match="idinfo">
  <A name="Identification_Information"><HR/></A>
  <DL>
    <DT><FONT color="#0000AA" size="3"><B>Identification Information:</B></FONT></DT>
    <BR/><BR/>
    <DD>
    <DL>
      <xsl:for-each select="citation">
        <DT><FONT color="#0000AA"><B>Citation:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:apply-templates select="citeinfo"/>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="descript">
        <DT><FONT color="#0000AA"><B>Description:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="abstract">
            <DIV>
              <DT><FONT color="#0000AA"><B>Abstract:</B></FONT></DT>
              <xsl:choose>
                <xsl:when test="context()[. = 'REQUIRED: A brief narrative summary of the data set.']">
                  <DD><FONT color="#999999"><xsl:value-of /></FONT></DD>
                </xsl:when>
                <xsl:when test="context()[. = 'A brief narrative summary of the data set.  REQUIRED.']">
                  <DD><FONT color="#999999"><xsl:value-of /></FONT></DD>
                </xsl:when>
                <xsl:otherwise>
                  <PRE ID="original"><xsl:value-of /></PRE>
                  <SCRIPT>fix(original)</SCRIPT>      
                </xsl:otherwise>
              </xsl:choose>
            </DIV><BR/>
          </xsl:for-each>

          <xsl:for-each select="purpose">
            <DIV>
              <DT><FONT color="#0000AA"><B>Purpose:</B></FONT></DT>
              <xsl:choose>
                <xsl:when test="context()[. = 'REQUIRED: A summary of the intentions with which the data set was developed.']">
                  <DD><FONT color="#999999"><xsl:value-of /></FONT></DD>
                </xsl:when>
                <xsl:when test="context()[. = 'A summary of the intentions with which the data set was developed.  REQUIRED.']">
                  <DD><FONT color="#999999"><xsl:value-of /></FONT></DD>
                </xsl:when>
                <xsl:otherwise>
                  <PRE ID="original"><xsl:value-of /></PRE>
                  <SCRIPT>fix(original)</SCRIPT>      
                </xsl:otherwise>
              </xsl:choose>
            </DIV><BR/>
          </xsl:for-each>

          <xsl:for-each select="supplinf">
            <DIV>
              <DT><FONT color="#0000AA"><B>Supplemental information:</B></FONT></DT>
              <PRE ID="original"><xsl:value-of /></PRE>
              <SCRIPT>fix(original)</SCRIPT>      
            </DIV><BR/>
          </xsl:for-each>

          <!-- ESRI Profile element  -->
          <xsl:for-each select="langdata">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Language of dataset:</B></FONT> 
                <xsl:value-of/></DT><BR/><BR/>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="timeperd">
        <DT><FONT color="#0000AA"><B>Time period of content:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:apply-templates select="timeinfo"/>
          
          <xsl:for-each select="current">
            <DIV>
              <DT><FONT color="#0000AA"><B>Currentness reference:</B></FONT></DT>
              <xsl:choose>
                <xsl:when test="context()[. = 'REQUIRED: The basis on which the time period of content information is determined.']">
                  <DD><FONT color="#999999"><xsl:value-of /></FONT></DD>
                </xsl:when>
                <xsl:when test="context()[. = 'The basis on which the time period of content information is determined.  REQUIRED.']">
                  <DD><FONT color="#999999"><xsl:value-of /></FONT></DD>
                </xsl:when>
                <xsl:otherwise>
                  <PRE ID="original"><xsl:value-of /></PRE>
                  <SCRIPT>fix(original)</SCRIPT>      
                </xsl:otherwise>
              </xsl:choose>
            </DIV><BR/>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="status">
        <DT><FONT color="#0000AA"><B>Status:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="progress">
            <DT>
              <FONT color="#0000AA"><B>Progress:</B></FONT>
              <xsl:choose>
                <xsl:when test="context()[. = 'REQUIRED: The state of the data set.']">
                  <FONT color="#999999"><xsl:value-of /></FONT>
                </xsl:when>
                <xsl:when test="context()[. = 'The state of the data set.  REQUIRED.']">
                  <FONT color="#999999"><xsl:value-of /></FONT>
                </xsl:when>
                <xsl:otherwise><xsl:value-of/></xsl:otherwise>
              </xsl:choose>
            </DT>
          </xsl:for-each>
          <xsl:for-each select="update">
            <DT>
              <FONT color="#0000AA"><B>Maintenance and update frequency:</B></FONT> 
              <xsl:choose>
                <xsl:when test="context()[. = 'REQUIRED: The frequency with which changes and additions are made to the data set after the initial data set is completed.']">
                  <FONT color="#999999"><xsl:value-of /></FONT>
                </xsl:when>
                <xsl:when test="context()[. = 'The frequency with which changes and additions are made to the data set after the initial data set is completed.  REQUIRED.']">
                  <FONT color="#999999"><xsl:value-of /></FONT>
                </xsl:when>
                <xsl:otherwise><xsl:value-of/></xsl:otherwise>
              </xsl:choose>
            </DT>
          </xsl:for-each>
        </DL>
        </DD>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="spdom">
        <DT><FONT color="#0000AA"><B>Spatial domain:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="bounding">
            <DT><FONT color="#0000AA"><B>Bounding coordinates:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="westbc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>West bounding coordinate:</B></FONT>
                  <xsl:choose>
                    <xsl:when test="context()[. = 'REQUIRED: Western-most coordinate of the limit of coverage expressed in longitude.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:when test="context()[. = 'Western-most coordinate of the limit of coverage expressed in longitude.  REQUIRED.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:otherwise><xsl:value-of /></xsl:otherwise>
                  </xsl:choose>
                </DT>
              </xsl:for-each>
              <xsl:for-each select="eastbc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>East bounding coordinate:</B></FONT>
                  <xsl:choose>
                    <xsl:when test="context()[. = 'REQUIRED: Eastern-most coordinate of the limit of coverage expressed in longitude.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:when test="context()[. = 'Eastern-most coordinate of the limit of coverage expressed in longitude.  REQUIRED.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:otherwise><xsl:value-of /></xsl:otherwise>
                  </xsl:choose>
                </DT>
              </xsl:for-each>
              <xsl:for-each select="northbc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>North bounding coordinate:</B></FONT>
                  <xsl:choose>
                    <xsl:when test="context()[. = 'REQUIRED: Northern-most coordinate of the limit of coverage expressed in latitude.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:when test="context()[. = 'Northern-most coordinate of the limit of coverage expressed in latitude.  REQUIRED.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:otherwise><xsl:value-of /></xsl:otherwise>
                  </xsl:choose>
                </DT>
              </xsl:for-each>
              <xsl:for-each select="southbc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>South bounding coordinate:</B></FONT>
                  <xsl:choose>
                    <xsl:when test="context()[. = 'REQUIRED: Southern-most coordinate of the limit of coverage expressed in latitude.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:when test="context()[. = 'Southern-most coordinate of the limit of coverage expressed in latitude.  REQUIRED.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:otherwise><xsl:value-of /></xsl:otherwise>
                  </xsl:choose>
                </DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>
          
          <!-- ESRI Profile element  -->
          <xsl:for-each select="lboundng">
            <DT><FONT color="#006400"><B>Local bounding coordinates:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="leftbc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Left bounding coordinate:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="rightbc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Right bounding coordinate:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="topbc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Top bounding coordinate:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="bottombc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Bottom bounding coordinate:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>

          <xsl:for-each select="dsgpoly">
            <DT><FONT color="#0000AA"><B>Data set G-polygon:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="dsgpolyo">
                <DT><FONT color="#0000AA"><B>Data set G-polygon outer G-ring:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:apply-templates select="grngpoin"/>
                  <xsl:apply-templates select="gring"/>
                </DL>
                </DD>
              </xsl:for-each>
              <xsl:for-each select="dsgpolyx">
                <DT><FONT color="#0000AA"><B>Data set G-polygon exclusion G-ring:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:apply-templates select="grngpoin"/>
                  <xsl:apply-templates select="gring"/>
                </DL>
                </DD>
              </xsl:for-each>
            </DL>
            </DD>
          </xsl:for-each>

          <!-- ESRI Profile element  -->
          <xsl:for-each select="minalti">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Minimum altitude:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="maxalti">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Maximum altitude:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="altunits">
            <DT><FONT color="#006400"><B>Altitude units:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:if test="context()[($any$ minalti | maxalti | altunits)]"><BR/><BR/></xsl:if>

          <!-- ESRI Profile element  -->
          <xsl:for-each select="eframes">
            <DT><FONT color="#006400"><B>Data frames:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="framect">
                <DT><FONT color="#006400"><B>Data frame count:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="framenam">
                <DT><FONT color="#006400"><B>Data frame name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="keywords">
        <DT><FONT color="#0000AA"><B>Keywords:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="theme">
            <DT><FONT color="#0000AA"><B>Theme:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:if test="context()[themekey/text()]">
                <DT>
                <xsl:for-each select="themekey[text()]">
                  <xsl:if test="context()[0]"><FONT color="#0000AA"><B>Theme keywords:</B></FONT> </xsl:if>
                  <xsl:choose>
                    <xsl:when test="context()[. = 'REQUIRED: Common-use word or phrase used to describe the subject of the data set.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:when test="context()[. = 'Common-use word or phrase used to describe the subject of the data set.  REQUIRED.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:otherwise><xsl:value-of /></xsl:otherwise>
                  </xsl:choose><xsl:if test="context()[not(end())]">, </xsl:if>
                </xsl:for-each>
                </DT>
              </xsl:if>
              <xsl:for-each select="themekt">
                <DT>
                  <FONT color="#0000AA"><B>Theme keyword thesaurus:</B></FONT> 
                  <xsl:choose>
                    <xsl:when test="context()[. = 'REQUIRED: Reference to a formally registered thesaurus or a similar authoritative source of theme keywords.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:when test="context()[. = 'Reference to a formally registered thesaurus or a similar authoritative source of theme keywords.  REQUIRED.']">
                      <FONT color="#999999"><xsl:value-of /></FONT>
                    </xsl:when>
                    <xsl:otherwise><xsl:value-of /></xsl:otherwise>
                  </xsl:choose>
                </DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>

          <xsl:for-each select="place">
            <DT><FONT color="#0000AA"><B>Place:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:if test="context()[placekey/text()]">
                <DT>
                <xsl:for-each select="placekey[text()]">
                  <xsl:if test="context()[0]"><FONT color="#0000AA"><B>Place keywords:</B></FONT> </xsl:if>
                  <xsl:value-of /><xsl:if test="context()[not(end())]">, </xsl:if>
                </xsl:for-each>
                </DT>
              </xsl:if>
              <xsl:for-each select="placekt">
                <DT><FONT color="#0000AA"><B>Place keyword thesaurus:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>

          <xsl:for-each select="stratum">
            <DT><FONT color="#0000AA"><B>Stratum:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:if test="context()[stratkey/text()]">
                <DT>
                <xsl:for-each select="stratkey[text()]">
                  <xsl:if test="context()[0]"><FONT color="#0000AA"><B>Stratum keywords:</B></FONT> </xsl:if>
                  <xsl:value-of /><xsl:if test="context()[not(end())]">, </xsl:if>
                </xsl:for-each>
                </DT>
              </xsl:if>
              <xsl:for-each select="stratkt">
                <DT><FONT color="#0000AA"><B>Stratum keyword thesaurus:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>
 
          <xsl:for-each select="temporal">
            <DT><FONT color="#0000AA"><B>Temporal:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:if test="context()[tempkey/text()]">
                <DT>
                <xsl:for-each select="tempkey[text()]">
                  <xsl:if test="context()[0]"><FONT color="#0000AA"><B>Temporal keywords:</B></FONT> </xsl:if>
                  <xsl:value-of /><xsl:if test="context()[not(end())]">, </xsl:if>
                </xsl:for-each>
                </DT>
              </xsl:if>
              <xsl:for-each select="tempkt">
                <DT><FONT color="#0000AA"><B>Temporal keyword thesaurus:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="accconst">
        <DT><FONT color="#0000AA"><B>Access constraints:</B></FONT>
          <xsl:choose>
            <xsl:when test="context()[. = 'REQUIRED: Restrictions and legal prerequisites for accessing the data set.']">
              <FONT color="#999999"><xsl:value-of /></FONT>
            </xsl:when>
            <xsl:when test="context()[. = 'Restrictions and legal prerequisites for accessing the data set.  REQUIRED.']">
              <FONT color="#999999"><xsl:value-of /></FONT>
            </xsl:when>
            <xsl:otherwise><xsl:value-of /></xsl:otherwise>
          </xsl:choose>
        </DT>
      </xsl:for-each>
      <xsl:for-each select="useconst">
        <DIV>
          <DT><FONT color="#0000AA"><B>Use constraints:</B></FONT></DT>
          <xsl:choose>
            <xsl:when test="context()[. = 'REQUIRED: Restrictions and legal prerequisites for using the data set after access is granted.']">
              <DD><FONT color="#999999"><xsl:value-of /></FONT></DD>
            </xsl:when>
            <xsl:when test="context()[. = 'Restrictions and legal prerequisites for using the data set after access is granted.  REQUIRED.']">
              <DD><FONT color="#999999"><xsl:value-of /></FONT></DD>
            </xsl:when>
            <xsl:otherwise>
              <PRE ID="original"><xsl:value-of /></PRE>
              <SCRIPT>fix(original)</SCRIPT>      
            </xsl:otherwise>
          </xsl:choose>
        </DIV>
      </xsl:for-each>
      <xsl:if test="context()[($any$ accconst | useconst)]"><BR/></xsl:if>
      <xsl:if test="context()[($any$ accconst | useconst) and not ($any$ useconst)]"><BR/></xsl:if>

      <xsl:for-each select="ptcontac">
        <DT><FONT color="#0000AA"><B>Point of contact:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:apply-templates select="cntinfo"/>
        </DL>
        </DD>
        <xsl:if test="context()[not (cntinfo/*)]"><BR/></xsl:if>
      </xsl:for-each>

      <xsl:for-each select="browse">
        <DT><FONT color="#0000AA"><B>Browse graphic:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="browsen">
            <DT><FONT color="#0000AA"><B>Browse graphic file name:</B></FONT> <A TARGET="viewer">
              <xsl:attribute name="HREF"><xsl:value-of/></xsl:attribute>
              <xsl:value-of/></A>
            </DT>
          </xsl:for-each>
          <xsl:for-each select="browsed">
            <DT><FONT color="#0000AA"><B>Browse graphic file description:</B></FONT></DT>
            <DD><xsl:value-of/></DD>
          </xsl:for-each>
          <xsl:for-each select="browset">
            <DT><FONT color="#0000AA"><B>Browse graphic file type:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="img">
            <DT><FONT color="#006400"><B>Browse graphic embedded:</B></FONT></DT>
            <DD>
            <DL>
              <DT><FONT color="#006400"><B>Enclosure type:</B></FONT> Picture</DT>
              <BR/><BR/>
              <IMG ID="thumbnail" ALIGN="absmiddle" STYLE="height:144; 
                  border:'2 outset #FFFFFF'; position:relative">
                <xsl:attribute name="SRC"><xsl:value-of select="@src"/></xsl:attribute>
              </IMG>
            </DL>
            </DD>
          </xsl:for-each>
        </DL>
        </DD>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="datacred">
        <DIV>
          <DT><FONT color="#0000AA"><B>Data set credit:</B></FONT></DT>
          <PRE ID="original"><xsl:value-of/></PRE>      
          <SCRIPT>fix(original)</SCRIPT>      
        </DIV>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="secinfo">
        <DT><FONT color="#0000AA"><B>Security information:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="secsys">
            <DT><FONT color="#0000AA"><B>Security classification system:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:for-each select="secclass">
            <DT><FONT color="#0000AA"><B>Security classification:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:for-each select="sechandl">
            <DT><FONT color="#0000AA"><B>Security handling description:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
        </DL>
        </DD>
        <BR/>
      </xsl:for-each>

      <!-- ESRI Profile element  -->
      <xsl:for-each select="natvform">
        <DT><xsl:if test="context()[@Sync = 'TRUE']">
            <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Native dataset format:</B></FONT> <xsl:value-of/></DT>
      </xsl:for-each>
      <xsl:for-each select="native">
        <DT><xsl:if test="context()[@Sync = 'TRUE']">
            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Native data set environment:</B></FONT></DT>
        <DD><xsl:value-of/></DD>
      </xsl:for-each>
      <xsl:if test="context()[($any$ natvform | native)]"><BR/><BR/></xsl:if>

      <xsl:for-each select="crossref">
        <DT><FONT color="#0000AA"><B>Cross reference:</B></FONT></DT>
        <DD>
        <DL>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="assndesc">
            <DT><FONT color="#006400"><B>Association description:</B></FONT> <xsl:value-of/></DT>
            <BR/><BR/>
          </xsl:for-each>
          <xsl:apply-templates select="citeinfo"/>
        </DL>
        </DD>
      </xsl:for-each>

    </DL>
    </DD>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

--------

<!-- Data Quality -->
<xsl:template match="dataqual">
  <A name="Data_Quality_Information"><HR/></A>
  <DL>
    <DT><FONT color="#0000AA" size="3"><B>Data Quality Information:</B></FONT></DT>
    <BR/><BR/>
    <DD>
    <DL>
      <xsl:for-each select="attracc">
        <DT><FONT color="#0000AA"><B>Attribute accuracy:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="attraccr">
            <DIV>
              <DT><FONT color="#0000AA"><B>Attribute accuracy report:</B></FONT></DT>
              <PRE ID="original"><xsl:value-of/></PRE>      
              <SCRIPT>fix(original)</SCRIPT>      
            </DIV>
            <BR/>
          </xsl:for-each>
          <xsl:for-each select="qattracc">
            <DT><FONT color="#0000AA"><B>Quantitative attribute accuracy assessment:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="attraccv">
                <DT><FONT color="#0000AA"><B>Attribute accuracy value:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="attracce">
                <DIV>
                  <DT><FONT color="#0000AA"><B>Attribute accuracy explanation:</B></FONT></DT>
                  <PRE ID="original"><xsl:value-of/></PRE>      
                  <SCRIPT>fix(original)</SCRIPT>      
                </DIV>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="logic">
        <DIV>
          <DT><FONT color="#0000AA"><B>Logical consistency report:</B></FONT></DT>
          <PRE ID="original"><xsl:value-of/></PRE>      
          <SCRIPT>fix(original)</SCRIPT>      
        </DIV>
        <BR/>
      </xsl:for-each>
      <xsl:for-each select="complete">
        <DIV>
          <DT><FONT color="#0000AA"><B>Completeness report:</B></FONT></DT>
          <PRE ID="original"><xsl:value-of/></PRE>      
          <SCRIPT>fix(original)</SCRIPT>      
        </DIV>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="posacc">
        <DT><FONT color="#0000AA"><B>Positional accuracy:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="horizpa">
            <DT><FONT color="#0000AA"><B>Horizontal positional accuracy:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="horizpar">
                <DIV>
                  <DT><FONT color="#0000AA"><B>Horizontal positional accuracy report:</B></FONT></DT>
                  <PRE ID="original"><xsl:value-of/></PRE>      
                  <SCRIPT>fix(original)</SCRIPT>      
                </DIV>
                <BR/>
              </xsl:for-each>
              <xsl:for-each select="qhorizpa">
                <DT><FONT color="#0000AA"><B>Quantitative horizontal positional accuracy assessment:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="horizpav">
                    <DT><FONT color="#0000AA"><B>Horizontal positional accuracy value:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="horizpae">
                    <DIV>
                      <DT><FONT color="#0000AA"><B>Horizontal positional accuracy explanation:</B></FONT></DT>
                      <PRE ID="original"><xsl:value-of/></PRE>      
                      <SCRIPT>fix(original)</SCRIPT>      
                    </DIV>
                  </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>
            </DL>
            </DD>
          </xsl:for-each>
          <xsl:for-each select="vertacc">
            <DT><FONT color="#0000AA"><B>Vertical positional accuracy:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="vertaccr">
                <DIV>
                  <DT><FONT color="#0000AA"><B>Vertical positional accuracy report:</B></FONT></DT>
                  <PRE ID="original"><xsl:value-of/></PRE>      
                  <SCRIPT>fix(original)</SCRIPT>      
                </DIV>
                <BR/>
              </xsl:for-each>
              <xsl:for-each select="qvertpa">
                <DT><FONT color="#0000AA"><B>Quantitative vertical positional accuracy assessment:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="vertaccv">
                    <DT><FONT color="#0000AA"><B>Vertical positional accuracy value:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="vertacce">
                    <DIV>
                      <DT><FONT color="#0000AA"><B>Vertical positional accuracy explanation:</B></FONT></DT>
                      <PRE ID="original"><xsl:value-of/></PRE>      
                      <SCRIPT>fix(original)</SCRIPT>      
                    </DIV>
                  </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>
            </DL>
            </DD>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="lineage">
        <DT><FONT color="#0000AA"><B>Lineage:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="srcinfo">
            <DT><FONT color="#0000AA"><B>Source information:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="srccite">
                <DT><FONT color="#0000AA"><B>Source citation:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:apply-templates select="citeinfo"/>
                </DL>
                </DD>
              </xsl:for-each>

              <xsl:for-each select="srcscale">
                <DT><FONT color="#0000AA"><B>Source scale denominator:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="typesrc">
                <DT><FONT color="#0000AA"><B>Type of source media:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="srccitea">
                <DT><FONT color="#0000AA"><B>Source citation abbreviation:</B></FONT></DT>
                <DD><xsl:value-of/></DD>
              </xsl:for-each>
              <xsl:for-each select="srccontr">
                <DIV>
                  <DT><FONT color="#0000AA"><B>Source contribution:</B></FONT></DT>
                  <PRE ID="original"><xsl:value-of/></PRE>      
                  <SCRIPT>fix(original)</SCRIPT>      
                </DIV>
              </xsl:for-each>
              <xsl:if test="context()[($any$ srcscale | typesrc | srccitea | srccontr)]"><BR/>  </xsl:if>

              <xsl:for-each select="srctime">
                <DT><FONT color="#0000AA"><B>Source time period of content:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:apply-templates select="timeinfo"/>
                  <xsl:for-each select="srccurr">
                    <DT><FONT color="#0000AA"><B>Source currentness reference:</B></FONT></DT>
                    <DD><xsl:value-of/></DD>
                  </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>
            </DL>
            </DD>
          </xsl:for-each>

          <xsl:for-each select="procstep">
            <DT><FONT color="#0000AA"><B>Process step:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="procdesc">
                <DIV>
                  <DT><FONT color="#0000AA"><B>Process description:</B></FONT></DT>
                  <PRE ID="original"><xsl:value-of/></PRE>      
                  <SCRIPT>fix(original)</SCRIPT>      
                </DIV>
                <BR/>
              </xsl:for-each>

              <!-- ESRI Profile element  -->
              <xsl:for-each select="procsv">
                <DT><FONT color="#006400"><B>Process software and version:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="procdate">
                <DT><FONT color="#0000AA"><B>Process date:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="proctime">
                <DT><FONT color="#0000AA"><B>Process time:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ procsv | procdate | proctime)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="srcused">
                <DT><FONT color="#0000AA"><B>Source used citation abbreviation:</B></FONT></DT>
                <DD><xsl:value-of/></DD>
              </xsl:for-each>
              <xsl:for-each select="srcprod">
                <DT><FONT color="#0000AA"><B>Source produced citation abbreviation:</B></FONT></DT>
                <DD><xsl:value-of/></DD>
              </xsl:for-each>
              <xsl:if test="context()[($any$ srcused | srcprod)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="proccont">
                <DT><FONT color="#0000AA"><B>Process contact:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:apply-templates select="cntinfo"/>
                </DL>
                </DD>
                <xsl:if test="context()[not (cntinfo/*)]"><BR/></xsl:if>
              </xsl:for-each>
            </DL>
            </DD>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>
      <xsl:for-each select="cloud">
        <DT><FONT color="#0000AA"><B>Cloud cover:</B></FONT> <xsl:value-of/></DT>
      </xsl:for-each>
    </DL>
    </DD>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

--------

<!-- Spatial Data Organization -->
<xsl:template match="spdoinfo">
  <A name="Spatial_Data_Organization_Information"><HR/></A>
  <DL>
    <DT><FONT color="#0000AA" size="3"><B>Spatial Data Organization Information:</B></FONT></DT>
    <BR/><BR/>
    <DD>
    <DL>
      <xsl:for-each select="indspref">
        <DT><FONT color="#0000AA"><B>Indirect spatial reference method:</B></FONT></DT>
        <DD><xsl:value-of/></DD>
        <BR/><BR/>
      </xsl:for-each>

      <xsl:for-each select="direct">
        <DT><xsl:if test="context()[@Sync = 'TRUE']">
            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Direct spatial reference method:</B></FONT> <xsl:value-of/></DT>
        <BR/><BR/>
      </xsl:for-each>

    <!-- ESRI Profile element  -->
      <!-- Topologies -->
      <xsl:for-each select="../Esri/DataProperties/topoinfo">
        <DT><FONT color="#006400"><B>Topology information:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="topoProps">
            <DT><FONT color="#006400"><B>Topology properties:</B></FONT></DT>
            <DD>
            <DL>
	          <xsl:for-each select="topoName">
	            <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Topology name:</B></FONT> <xsl:value-of/></DT>
	          </xsl:for-each>
	          <xsl:for-each select="clusterTol">
	            <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Cluster tolerance:</B></FONT> <xsl:value-of/></DT>
	          </xsl:for-each>
	          <xsl:for-each select="trustedArea/trustedPolygon">
	            <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Trusted area coordinates:</B></FONT> <xsl:value-of/></DT>
	          </xsl:for-each>
	          <xsl:for-each select="notTrusted">
	            <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Nothing trusted:</B></FONT> <xsl:value-of/></DT>
	          </xsl:for-each>
	          <xsl:for-each select="maxErrors">
	            <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Maximum error count:</B></FONT> <xsl:value-of/></DT>
	          </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>
          
          <xsl:for-each select="topoRule">
            <DT><FONT color="#006400"><B>Topology rule:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="topoRuleName">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Rule name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="topoRuleID">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Rule identifier:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="topoRuleType">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Rule type:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="rulehelp">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Rule help:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ topoRuleName | topoRuleID | topoRuleType | rulehelp)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="topoRuleOrigin">
                <DT><FONT color="#006400"><B>Rule origin:</B></FONT></DT>
                <DD>
                <DL>
	              <xsl:for-each select="fcname">
	                <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Feature class name:</B></FONT> <xsl:value-of/></DT>
	              </xsl:for-each>
	              <xsl:for-each select="stcode">
	                <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Subtype code:</B></FONT> <xsl:value-of/></DT>
	              </xsl:for-each>
	              <xsl:for-each select="stname">
	                <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Subtype name:</B></FONT> <xsl:value-of/></DT>
	              </xsl:for-each>
	              <xsl:for-each select="allOriginSubtypes">
	                <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Rule applies to all origin subtypes:</B></FONT> <xsl:value-of/></DT>
	              </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>
              
              <xsl:for-each select="topoRuleDest">
                <DT><FONT color="#006400"><B>Rule destination:</B></FONT></DT>
                <DD>
                <DL>
	              <xsl:for-each select="fcname">
	                <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Feature class name:</B></FONT> <xsl:value-of/></DT>
	              </xsl:for-each>
	              <xsl:for-each select="stcode">
	                <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Subtype code:</B></FONT> <xsl:value-of/></DT>
	              </xsl:for-each>
	              <xsl:for-each select="stname">
	                <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Subtype name:</B></FONT> <xsl:value-of/></DT>
	              </xsl:for-each>
	              <xsl:for-each select="allDestSubtypes">
	                <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Rule applies to all destination subtypes:</B></FONT> <xsl:value-of/></DT>
	              </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>

           </DL>
           </DD>
         </xsl:for-each>
		 <BR/>
         </DL>
         </DD>
       </xsl:for-each>

      <!-- ESRI Profile element  -->
      <!-- Terrains -->
      <xsl:for-each select="../Esri/DataProperties/Terrain">
        <DT><FONT color="#006400"><B>Terrain information:</B></FONT></DT>
        <DD>
        <DL>
		  <xsl:for-each select="totalPts">
			<DT><FONT color="#006400"><B>Total number of points:</B></FONT> <xsl:value-of/></DT>
		  </xsl:for-each>
		<BR/><BR/>
		</DL>
		</DD>
      </xsl:for-each>

      <!-- ESRI Profile element  -->
      <!-- Geometric networks -->
      <xsl:for-each select="netinfo">
        <DT><FONT color="#006400"><B>Geometric network information:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="nettype">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Network type:</B></FONT> <xsl:value-of/></DT>
            <BR/><BR/>
          </xsl:for-each>
          <xsl:for-each select="connrule">
            <DT><FONT color="#006400"><B>Connectivity rule:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="ruletype">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Rule type:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="rulecat">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Rule category:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="rulehelp">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Rule help:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ ruletype | rulecat | rulehelp)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="rulefeid">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>From edge feature class:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="rulefest">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>From edge subtype:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="ruleteid">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>To edge feature class:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="ruletest">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>To edge subtype:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ rulefeid | rulefest | ruleteid | ruletest)]"><BR/><BR/></xsl:if>
              
              <xsl:for-each select="ruledjid">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Default junction feature class:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="ruledjst">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Default junction subtype:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="rulejunc">
                <DT><FONT color="#006400"><B>Available junctions:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="junctid">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Junction feature class:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="junctst">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Junction subtype:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>
              <xsl:if test="context()[($any$ rulefeid | rulefest) and not (rulejunc)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="ruleeid">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Edge feature class:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="ruleest">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Edge subtype:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="ruleemnc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Edge minimum cardinality:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="ruleemxc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Edge maximum cardinality:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ ruleeid | ruleest | ruleemnc | ruleemxc)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="rulejid">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Junction feature class:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="rulejst">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Junction subtype:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="rulejmnc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Junction minimum cardinality:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="rulejmxc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Junction maximum cardinality:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ rulejid | rulejst | rulejmnc | rulejmxc)]"><BR/><BR/></xsl:if>
            </DL>
            </DD>
          </xsl:for-each>
          
          <xsl:for-each select="elemcls">
            <DT><FONT color="#006400"><B>Network element:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="roletype">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Ancillary role:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="rolefld">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Ancillary role attribute:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="enabfld">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Enabled attribute:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="ptvctinf">
        <DT><FONT color="#0000AA"><B>Point and vector object information:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="sdtsterm">
            <DT><FONT color="#0000AA"><B>SDTS terms description:</B></FONT></DT>
            <DD>
            <DL>
              <!-- ESRI Profile element  -->
              <xsl:for-each select="@Name">
                <DT><FONT color="#006400">*<B>Name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="sdtstype">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>SDTS point and vector object type:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="ptvctcnt">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Point and vector object count:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>

          <xsl:for-each select="vpfterm">
            <DT><FONT color="#0000AA"><B>VPF terms description:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="vpflevel">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>VPF topology level:</B></FONT> <xsl:value-of/></DT>
                <BR/><BR/>
              </xsl:for-each>
              <xsl:for-each select="vpfinfo">
                <DT><FONT color="#0000AA"><B>VPF point and vector object information:</B></FONT></DT>
                <DD>
                <DL>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="@Name">
                    <DT><FONT color="#006400">*<B>Name:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="vpftype">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>VPF point and vector object type:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="ptvctcnt">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Point and vector object count:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>
            </DL>
            </DD>
          </xsl:for-each>

          <!-- ESRI Profile element  -->
          <xsl:for-each select="esriterm">
            <DT><FONT color="#006400"><B>ESRI terms description:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="@Name">
                <DT><FONT color="#006400">*<B>Name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="efeatyp">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>ESRI feature type:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="efeageom">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>ESRI feature geometry:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="esritopo">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>ESRI topology:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="efeacnt">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>ESRI feature count:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="spindex">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Spatial index:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="linrefer">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Linear referencing:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="netwrole">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Network role:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="featdesc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Feature description:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ @Name | efeatyp | efeageom | esritopo | efeacnt | spindex | linrefer | linrefer | netwrole | featdesc)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="XYRank">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>XY rank:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="ZRank">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Z rank:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="topoWeight">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Topology weight:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="validateEvents">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Events on validation:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
	        <xsl:for-each select="partTopoRules">
	          <DT><FONT color="#006400"><B>Participates in topology rules:</B></FONT></DT>
	          <DD>
	          <DL>
	            <xsl:for-each select="topoRuleID">
	              <DT><xsl:if test="context()[@Sync = 'TRUE']">
	                  <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Rule identifier:</B></FONT> <xsl:value-of />	</DT>
	            </xsl:for-each>
	          </DL>
	          </DD>
	        </xsl:for-each>
            <xsl:if test="context()[($any$ XYRank | ZRank | topoWeight | validateEvents | partTopoRules)]"><BR/><BR/></xsl:if>
            </DL>
            </DD>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="rastinfo">
        <DT><FONT color="#0000AA"><B>Raster object information:</B></FONT></DT>
        <DD>
        <DL>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastifor">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Image format:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastityp">
            <DT><FONT color="#006400"><B>Image type:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastband">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Number of bands:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:if test="context()[($any$ rastifor | rastityp | rastband)]"><BR/><BR/></xsl:if>

          <xsl:for-each select="rowcount">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Row count:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:for-each select="colcount">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Column count:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:for-each select="vrtcount">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Vertical count:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:if test="context()[($any$ rowcount | colcount | vrtcount)]"><BR/><BR/></xsl:if>

          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastxsz">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Cell size X direction:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastxu">
            <DT><FONT color="#006400"><B>Cell size X units:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastysz">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Cell size Y direction:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastyu">
            <DT><FONT color="#006400"><B>Cell size Y units:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:if test="context()[($any$ rastxsz | rastxu | rastysz | rastyu)]"><BR/><BR/></xsl:if>

          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastbpp">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Bits per pixel:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastnodt">
            <DT><FONT color="#006400"><B>Background nodata value:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastplyr">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Pyramid layers:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastcmap">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Image colormap:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastcomp">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Compression type:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:if test="context()[($any$ rastbpp | rastnodt | rastplyr | rastcmap | rastcomp)]"><BR/><BR/></xsl:if>

          <xsl:for-each select="rasttype">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Raster object type:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastdtyp">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Raster display type:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="rastorig">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Raster origin:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:if test="context()[($any$ rasttype | rastdtyp | rastorig)]"><BR/><BR/></xsl:if>
        </DL>
        </DD>
      </xsl:for-each>

      </DL>
    </DD>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

--------

<!-- Spatial Reference -->
<xsl:template match="spref">
  <A name="Spatial_Reference_Information"><HR/></A>
  <DL>
    <DT><FONT color="#0000AA" size="3"><B>Spatial Reference Information:</B></FONT></DT>
    <BR/><BR/>
    <DD>
    <DL>
      <xsl:for-each select="horizsys">
        <DT><FONT color="#0000AA"><B>Horizontal coordinate system definition:</B></FONT></DT>
        <DD>
        <DL>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="cordsysn">
            <DT><FONT color="#006400"><B>Coordinate system name:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="projcsn">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Projected coordinate system name:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
              <xsl:for-each select="geogcsn">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Geographic coordinate system name:</B></FONT> <xsl:value-of /></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>

          <xsl:for-each select="geograph">
            <DT><FONT color="#0000AA"><B>Geographic:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="latres">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Latitude resolution:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="longres">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Longitude resolution:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="geogunit">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Geographic coordinate units:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>

          <xsl:for-each select="planar">
            <DT><FONT color="#0000AA"><B>Planar:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="mapproj">
                <DT><FONT color="#0000AA"><B>Map projection:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="mapprojn">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Map projection name:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>

                  <xsl:for-each select="albers">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Albers conical equal area:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="azimequi">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Azimuthal equidistant:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="equicon">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Equidistant conic:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="equirect">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Equirectangular:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="gvnsp">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>General vertical near-sided perspective:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="gnomonic">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Gnomonic:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="lamberta">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Lambert azimuthal equal area:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="lambertc">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Lambert conformal conic:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="mercator">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Mercator:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="modsak">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Modified stereographic for alaska:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="miller">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Miller cylindrical:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="obqmerc">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Oblique mercator:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="orthogr">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Orthographic:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="polarst">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Polar stereographic:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="polycon">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Polyconic:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="robinson">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Robinson:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="sinusoid">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Sinusoidal:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="spaceobq">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Space oblique mercator (Landsat):</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="stereo">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Stereographic:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="transmer">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Transverse mercator:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="vdgrin">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>van der Grinten:</B></FONT></DT>
                  </xsl:for-each>
                  <xsl:for-each select="mapprojp">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Map projection parameters:</B></FONT></DT>
                  </xsl:for-each>

                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="behrmann">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Behrmann:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="bonne">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Bonne:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="cassini">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Cassini:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="eckert1">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Eckert I:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="eckert2">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Eckert II:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="eckert3">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Eckert III:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="eckert4">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Eckert IV:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="eckert5">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Eckert V:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="eckert6">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Eckert VI:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="gallster">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Gall stereographic:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="loximuth">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Loximuthal:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="mollweid">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Mollweide:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="quartic">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Quartic authalic:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="winkel1">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Winkel I:</B></FONT></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="winkel2">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Winkel II:</B></FONT></DT>
                  </xsl:for-each>

                  <xsl:apply-templates select="*"/>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>

              <xsl:for-each select="gridsys">
                <DT><FONT color="#0000AA"><B>Grid coordinate system:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="gridsysn">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Grid coordinate system name:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>

                  <xsl:for-each select="utm">
                    <DT><FONT color="#0000AA"><B>Universal Transverse Mercator:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="utmzone">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>UTM zone number:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="transmer">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Transverse mercator:</B></FONT></DT>
                      </xsl:for-each>
                      <xsl:apply-templates select="transmer"/>
                    </DL>
                    </DD>
                  </xsl:for-each>

                  <xsl:for-each select="ups">
                    <DT><FONT color="#0000AA"><B>Universal Polar Stereographic:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="upszone">
                        <DT><FONT color="#0000AA"><B>UPS zone identifier:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="polarst">
                        <DT><FONT color="#0000AA"><B>Polar stereographic:</B></FONT></DT>
                      </xsl:for-each>
                      <xsl:apply-templates select="polarst"/>
                    </DL>
                    </DD>
                  </xsl:for-each>

                  <xsl:for-each select="spcs">
                    <DT><FONT color="#0000AA"><B>State Plane Coordinate System:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="spcszone">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>SPCS zone identifier:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="lambertc">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Lambert conformal conic:</B></FONT></DT>
                      </xsl:for-each>
                      <xsl:apply-templates select="lambertc"/>
                      <xsl:for-each select="transmer">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Transverse mercator:</B></FONT></DT>
                      </xsl:for-each>
                      <xsl:apply-templates select="transmer"/>
                      <xsl:for-each select="obqmerc">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Oblique mercator:</B></FONT></DT>
                      </xsl:for-each>
                      <xsl:apply-templates select="obqmerc"/>
                      <xsl:for-each select="polycon">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Polyconic:</B></FONT></DT>
                      </xsl:for-each>
                      <xsl:apply-templates select="polycon"/>
                    </DL>
                    </DD>
                  </xsl:for-each>

                  <xsl:for-each select="arcsys">
                    <DT><FONT color="#0000AA"><B>ARC coordinate system:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="arczone">
                        <DT><FONT color="#0000AA"><B>ARC system zone identifier:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="equirect">
                        <DT><FONT color="#0000AA"><B>Equirectangular:</B></FONT></DT>
                      </xsl:for-each>
                      <xsl:apply-templates select="equirect"/>
                      <xsl:for-each select="azimequi">
                        <DT><FONT color="#0000AA"><B>Azimuthal equidistant:</B></FONT></DT>
                      </xsl:for-each>
                      <xsl:apply-templates select="azimequi"/>
                    </DL>
                    </DD>
                  </xsl:for-each>

                  <xsl:for-each select="othergrd">
                    <DT><FONT color="#0000AA"><B>Other grid system's definition:</B></FONT></DT>
                    <DD><xsl:value-of/></DD>
                  </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>

              <xsl:for-each select="localp">
                <DT><FONT color="#0000AA"><B>Local planar:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="localpd">
                    <DT><FONT color="#0000AA"><B>Local planar description:</B></FONT></DT>
                    <DD><xsl:value-of/></DD>
                  </xsl:for-each>
                  <xsl:for-each select="localpgi">
                    <DT><FONT color="#0000AA"><B>Local planar georeference information:</B></FONT></DT>
                    <DD><xsl:value-of/></DD>
                  </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>

              <xsl:for-each select="planci">
                <DT><FONT color="#0000AA"><B>Planar coordinate information:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="plance">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Planar coordinate encoding method:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="coordrep">
                    <DT><FONT color="#0000AA"><B>Coordinate representation:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="absres">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Abscissa resolution:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="ordres">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Ordinate resolution:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                    </DL>
                    </DD>
                  </xsl:for-each>
                  <xsl:for-each select="distbrep">
                    <DT><FONT color="#0000AA"><B>Distance and bearing representation:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="distres">
                        <DT><FONT color="#0000AA"><B>Distance resolution:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="bearres">
                        <DT><FONT color="#0000AA"><B>Bearing resolution:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="bearunit">
                        <DT><FONT color="#0000AA"><B>Bearing units:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="bearrefd">
                        <DT><FONT color="#0000AA"><B>Bearing reference direction:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="bearrefm">
                        <DT><FONT color="#0000AA"><B>Bearing reference meridian:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                    </DL>
                    </DD>
                  </xsl:for-each>
                  <xsl:for-each select="plandu">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Planar distance units:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>
            </DL>
            </DD>
          </xsl:for-each>

          <xsl:for-each select="local">
            <DT><FONT color="#0000AA"><B>Local:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="localdes">
                <DT><FONT color="#0000AA"><B>Local description:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="localgeo">
                <DT><FONT color="#0000AA"><B>Local georeference information:</B></FONT></DT>
                <DD><xsl:value-of/></DD>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>

          <xsl:for-each select="geodetic">
            <DT><FONT color="#0000AA"><B>Geodetic model:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="horizdn">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Horizontal datum name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="ellips">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Ellipsoid name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="semiaxis">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Semi-major axis:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="denflat">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Denominator of flattening ratio:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="vertdef">
        <DT><FONT color="#0000AA"><B>Vertical coordinate system definition:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="altsys">
            <DT><FONT color="#0000AA"><B>Altitude system definition:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="altdatum">
                <DT><FONT color="#0000AA"><B>Altitude datum name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="altres">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Altitude resolution:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="altunits">
                <DT><FONT color="#0000AA"><B>Altitude distance units:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="altenc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Altitude encoding method:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>

          <xsl:for-each select="depthsys">
            <DT><FONT color="#0000AA"><B>Depth system definition:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="depthdn">
                <DT><FONT color="#0000AA"><B>Depth datum name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="depthres">
                <DT><FONT color="#0000AA"><B>Depth resolution:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="depthdu">
                <DT><FONT color="#0000AA"><B>Depth distance units:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="depthem">
                <DT><FONT color="#0000AA"><B>Depth encoding method:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>
    </DL>
    </DD>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

--------

<!-- Entity and Attribute -->
<xsl:template match="eainfo">
  <A name="Entity_and_Attribute_Information"><HR/></A>
  <DL>
    <DT><FONT color="#0000AA" size="3"><B>Entity and Attribute Information:</B></FONT></DT>
    <BR/><BR/>
    <DD>
    <DL>
      <xsl:for-each select="detailed">
        <DT><FONT color="#0000AA"><B>Detailed description:</B></FONT></DT>
        <DD>
        <DL>
          <!-- ESRI Profile element  -->
          <xsl:for-each select="@Name">
            <DT><FONT color="#006400">*<B>Name:</B></FONT> <xsl:value-of/></DT>
            <BR/><BR/>
          </xsl:for-each>

          <xsl:for-each select="enttyp">
            <DT><FONT color="#0000AA"><B>Entity type:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="enttypl">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Entity type label:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <!-- ESRI Profile element  -->
              <xsl:for-each select="enttypt">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Entity type type:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <!-- ESRI Profile element  -->
              <xsl:for-each select="enttypc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Entity type count:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="enttypd">
                <DT><FONT color="#0000AA"><B>Entity type definition:</B></FONT></DT>
                <DD><xsl:value-of/></DD>
              </xsl:for-each>
              <xsl:for-each select="enttypds">
                <DT><FONT color="#0000AA"><B>Entity type definition source:</B></FONT></DT>
                <DD><xsl:value-of/></DD>
              </xsl:for-each>
            </DL>
            </DD>
            <BR/>
          </xsl:for-each>

          <xsl:for-each select="attr">
            <DT><FONT color="#0000AA"><B>Attribute:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="attrlabl">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Attribute label:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <!-- ESRI Profile element  -->
              <xsl:for-each select="attalias">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Attribute alias:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="attrdef">
                <DIV>
                  <DT><xsl:if test="context()[@Sync = 'TRUE']">
                      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Attribute definition:</B></FONT></DT>
                  <PRE ID="original"><xsl:value-of/></PRE>      
                  <SCRIPT>fix(original)</SCRIPT>      
                </DIV>
              </xsl:for-each>
              <xsl:for-each select="attrdefs">
                <DIV>
                  <DT><xsl:if test="context()[@Sync = 'TRUE']">
                      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Attribute definition source:</B></FONT></DT>
                  <PRE ID="original"><xsl:value-of/></PRE>      
                  <SCRIPT>fix(original)</SCRIPT>      
                </DIV>
              </xsl:for-each>
              <xsl:if test="context()[($any$ attrlabl | attalias | attrdef | attrdefs)]"><BR/></xsl:if>
              <xsl:if test="context()[($any$ attrlabl | attalias) and not(attrdef | attrdefs)]"><BR/></xsl:if>

              <!-- ESRI Profile element  -->
              <xsl:for-each select="attrtype">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Attribute type:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <!-- ESRI Profile element  -->
              <xsl:for-each select="attwidth">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Attribute width:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <!-- ESRI Profile element  -->
              <xsl:for-each select="atprecis">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Attribute precision:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <!-- ESRI Profile element  -->
              <xsl:for-each select="attscale">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Attribute scale:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <!-- ESRI Profile element  -->
              <xsl:for-each select="atoutwid">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Attribute output width:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <!-- ESRI Profile element  -->
              <xsl:for-each select="atnumdec">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Attribute number of decimals:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <!-- ESRI Profile element  -->
              <xsl:for-each select="atindex">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Attribute indexed:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ attrtype | attwidth | atprecis | attscale | atoutwid | atnumdec | atindex)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="attrdomv">
                <DT><FONT color="#0000AA"><B>Attribute domain values:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="edom">
                    <DT><FONT color="#0000AA"><B>Enumerated domain:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="edomv">
                        <DT><FONT color="#0000AA"><B>Enumerated domain value:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="edomvd">
                        <DIV>
                          <DT><FONT color="#0000AA"><B>Enumerated domain value definition:</B></FONT></DT>
                          <PRE ID="original"><xsl:value-of/></PRE>      
                          <SCRIPT>fix(original)</SCRIPT>      
                        </DIV>
                      </xsl:for-each>
                      <xsl:for-each select="edomvds">
                        <DIV>
                          <DT><FONT color="#0000AA"><B>Enumerated domain value definition source:</B></FONT></DT>
                          <PRE ID="original"><xsl:value-of/></PRE>      
                          <SCRIPT>fix(original)</SCRIPT>      
                        </DIV>
                      </xsl:for-each>
                      <xsl:for-each select="attr">
                        <DT><FONT color="#0000AA"><B>Attribute:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                    </DL>
                    </DD>
                    <BR/>
                  </xsl:for-each>

                  <xsl:for-each select="rdom">
                    <DT><FONT color="#0000AA"><B>Range domain:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="rdommin">
                        <DT><FONT color="#0000AA"><B>Range domain minimum:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="rdommax">
                        <DT><FONT color="#0000AA"><B>Range domain maximum:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <!-- ESRI Profile element  -->
                      <xsl:for-each select="rdommean">
                        <DT><FONT color="#006400"><B>Range domain mean:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <!-- ESRI Profile element  -->
                      <xsl:for-each select="rdomstdv">
                        <DT><FONT color="#006400"><B>Range domain standard deviation:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="attrunit">
                        <DT><FONT color="#0000AA"><B>Attribute units of measure:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="attrmres">
                        <DT><FONT color="#0000AA"><B>Attribute measurement resolution:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="attr">
                        <DT><FONT color="#0000AA"><B>Attribute:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                    </DL>
                    </DD>
                    <BR/>
                  </xsl:for-each>

                  <xsl:for-each select="codesetd">
                    <DT><FONT color="#0000AA"><B>Codeset Ddomain:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="codesetn">
                        <DT><FONT color="#0000AA"><B>Codeset name:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="codesets">
                        <DT><FONT color="#0000AA"><B>Codeset source:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                    </DL>
                    </DD>
                    <BR/>
                  </xsl:for-each>

                  <xsl:for-each select="udom">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Unrepresentable domain:</B></FONT></DT>
                    <DD><xsl:value-of/></DD>
                    <BR/><BR/>
                  </xsl:for-each>
                </DL>
                </DD>
              </xsl:for-each>

              <xsl:for-each select="begdatea">
                <DT><FONT color="#0000AA"><B>Beginning date of attribute values:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="enddatea">
                <DT><FONT color="#0000AA"><B>Ending date of attribute values:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ begdatea | enddatea)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="attrvai">
                <DT><FONT color="#0000AA"><B>Attribute value accuracy information:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="attrva">
                    <DT><FONT color="#0000AA"><B>Attribute value accuracy:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="attrvae">
                    <DT><FONT color="#0000AA"><B>Attribute value accuracy explanation:</B></FONT></DT>
                    <DD><xsl:value-of/></DD>
                  </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>
              <xsl:for-each select="attrmfrq">
                <DT><FONT color="#0000AA"><B>Attribute measurement frequency:</B></FONT></DT>
                <DD><xsl:value-of/></DD>
                <BR/><BR/>
              </xsl:for-each>
            </DL>
            </DD>
          </xsl:for-each>

          <!-- ESRI Profile element  -->
          <xsl:for-each select="subtype">
            <DT><FONT color="#006400"><B>Subtype information:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="stname">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Subtype name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="stcode">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Subtype code:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ stname | stcode)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="stfield">
                <DT><FONT color="#006400"><B>Subtype attribute:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="stfldnm">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Subtype attribute name:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="stflddv">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Subtype default value:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="stflddd">
                    <DT><FONT color="#006400"><B>Attribute defined domain:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="domname">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Domain name:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="domdesc">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Domain description:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="domowner">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Domain owner:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="domfldtp">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Attribute type:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="domtype">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Domain type:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="mrgtype">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Merge rule:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="splttype">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Split rule:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                    </DL>
                    </DD>
                    <BR/>
                  </xsl:for-each>
                  <xsl:if test="context()[($any$ stfldnm | stflddv) and not (stflddd)]"><BR/><BR/></xsl:if>
                </DL>
                </DD>
              </xsl:for-each>
            </DL>
            </DD>
          </xsl:for-each>

          <!-- ESRI Profile element  -->
          <xsl:for-each select="relinfo">
            <DT><FONT color="#006400"><B>Relationship information:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="reldesc">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Description of relationship:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="relcard">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Relationship cardinality:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="relattr">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Attributed relationship:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="relcomp">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Composite relationship:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="relnodir">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Notification direction:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ reldesc | relcard | relattr | relcomp | relnodir)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="otfcname">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Origin table feature class name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="otfcpkey">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Origin table feature class primary key:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="otfcfkey">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Origin table feature class foreign key:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="dtfcname">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Destination table feature class name:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="dtfcpkey">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Destination table feature class primary key:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="dtfcfkey">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Destination table feature class foreign key:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ otfcname | otfcpkey | otfcfkey | dtfcname | dtfcpkey | dtfcfkey)]"><BR/><BR/></xsl:if>

              <xsl:for-each select="relflab">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Relationship forward label:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:for-each select="relblab">
                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Relationship backward label:</B></FONT> <xsl:value-of/></DT>
              </xsl:for-each>
              <xsl:if test="context()[($any$ relflab | relblab)]"><BR/><BR/></xsl:if>
            </DL>
            </DD>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="overview">
        <DT><FONT color="#0000AA"><B>Overview description:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="dsoverv">
            <DIV>
              <DT><FONT color="#0000AA"><B>Dataset overview:</B></FONT></DT>
              <PRE ID="original"><xsl:value-of/></PRE>      
              <SCRIPT>fix(original)</SCRIPT>      
            </DIV>
            <BR/>
          </xsl:for-each>
          <xsl:for-each select="eaover">
            <DIV>
              <DT><FONT color="#0000AA"><B>Entity and attribute overview:</B></FONT></DT>
              <PRE ID="original"><xsl:value-of/></PRE>      
              <SCRIPT>fix(original)</SCRIPT>      
            </DIV>
            <BR/>
          </xsl:for-each>
          <xsl:for-each select="eadetcit">
            <DIV>
              <DT><FONT color="#0000AA"><B>Entity and attribute detail citation:</B></FONT></DT>
              <PRE ID="original"><xsl:value-of/></PRE>      
              <SCRIPT>fix(original)</SCRIPT>      
            </DIV>
            <BR/>
          </xsl:for-each>
        </DL>
        </DD>
      </xsl:for-each>
    </DL>
    </DD>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

--------

<!-- Distribution -->
<xsl:template match="distinfo">
  <A>
    <xsl:attribute name="NAME"><xsl:eval>uniqueID(this)</xsl:eval></xsl:attribute>
    <HR/>
  </A>
  <DL>
    <DT><FONT color="#0000AA" size="3"><B>Distribution Information:</B></FONT> </DT>
    <BR/><BR/>
    <DD>
    <DL>
      <xsl:for-each select="distrib">
        <DT><FONT color="#0000AA"><B>Distributor:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:apply-templates select="cntinfo"/>
        </DL>
        </DD>
        <xsl:if test="context()[not (cntinfo/*)]"><BR/></xsl:if>
      </xsl:for-each>

      <xsl:for-each select="resdesc">
        <DT><FONT color="#0000AA"><B>Resource description:</B></FONT> <xsl:value-of/></DT>
        <BR/><BR/>
      </xsl:for-each>
      <xsl:for-each select="distliab">
        <DIV>
          <DT><FONT color="#0000AA"><B>Distribution liability:</B></FONT></DT>
          <PRE ID="original"><xsl:value-of/></PRE>      
          <SCRIPT>fix(original)</SCRIPT>      
        </DIV>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="stdorder">
        <DT><FONT color="#0000AA"><B>Standard order process:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="nondig">
            <DT><FONT color="#0000AA"><B>Non-digital form:</B></FONT></DT>
            <DD><xsl:value-of/></DD>
            <BR/><BR/>
          </xsl:for-each>
          <xsl:for-each select="digform">
            <DT><FONT color="#0000AA"><B>Digital form:</B></FONT></DT>
            <DD>
            <DL>
              <xsl:for-each select="digtinfo">
                <DT><FONT color="#0000AA"><B>Digital transfer information:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="formname">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Format name:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="formvern">
                    <DT><FONT color="#0000AA"><B>Format version number:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="formverd">
                    <DT><FONT color="#0000AA"><B>Format version date:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="formspec">
                    <DIV>
                      <DT><FONT color="#0000AA"><B>Format specification:</B></FONT></DT>
                      <PRE ID="original"><xsl:value-of/></PRE>      
                      <SCRIPT>fix(original)</SCRIPT>      
                    </DIV>
                  </xsl:for-each>
                  <xsl:for-each select="formcont">
                    <DIV>
                      <DT><FONT color="#0000AA"><B>Format information content:</B></FONT></DT>
                      <PRE ID="original"><xsl:value-of/></PRE>      
                      <SCRIPT>fix(original)</SCRIPT>      
                    </DIV>
                  </xsl:for-each>
                  <xsl:for-each select="filedec">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>File decompression technique:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <xsl:for-each select="transize">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Transfer size:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                  <!-- ESRI Profile element  -->
                  <xsl:for-each select="dssize">
                    <DT><xsl:if test="context()[@Sync = 'TRUE']">
                        <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Dataset size:</B></FONT> <xsl:value-of/></DT>
                  </xsl:for-each>
                </DL>
                </DD>
                <BR/>
              </xsl:for-each>

              <xsl:for-each select="digtopt">
                <DT><FONT color="#0000AA"><B>Digital transfer option:</B></FONT></DT>
                <DD>
                <DL>
                  <xsl:for-each select="onlinopt">
                    <DT><FONT color="#0000AA"><B>Online option:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="computer">
                        <DT><FONT color="#0000AA"><B>Computer contact information:</B></FONT></DT>
                        <DD>
                        <DL>
                          <xsl:for-each select="networka">
                            <DT><FONT color="#0000AA"><B>Network address:</B></FONT></DT>
                            <DD>
                            <DL>
                              <xsl:for-each select="networkr">
                                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Network resource name:</B></FONT> <A TARGET="viewer">
                                  <xsl:attribute name="HREF"><xsl:value-of/></xsl:attribute>
                                  <xsl:value-of/></A>
                                </DT>
                              </xsl:for-each>
                            </DL>
                            </DD>
                            <BR/>
                          </xsl:for-each>

                          <xsl:for-each select="dialinst">
                            <DT><FONT color="#0000AA"><B>Dialup instructions:</B></FONT></DT>
                            <DD>
                            <DL>
                              <xsl:for-each select="lowbps">
                                <DT><FONT color="#0000AA"><B>Lowest BPS:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="highbps">
                                <DT><FONT color="#0000AA"><B>Highest BPS:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="numdata">
                                <DT><FONT color="#0000AA"><B>Number databits:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="numstop">
                                <DT><FONT color="#0000AA"><B>Number stopbits:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="parity">
                                <DT><FONT color="#0000AA"><B>Parity:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="compress">
                                <DT><FONT color="#0000AA"><B>Compression support:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="dialtel">
                                <DT><FONT color="#0000AA"><B>Dialup telephone:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="dialfile">
                                <DT><FONT color="#0000AA"><B>Dialup file name:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                            </DL>
                            </DD>
                            <BR/>
                          </xsl:for-each>

                          <!-- ESRI Profile element  -->
                          <xsl:for-each select="sdeconn">
                            <DT><FONT color="#006400"><B>SDE connection information:</B></FONT></DT>
                            <DD>
                            <DL>
                              <xsl:for-each select="server">
                                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Server name:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="instance">
                                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Instance name:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="database">
                                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Database name:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="user">
                                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>User name:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                              <xsl:for-each select="version">
                                <DT><xsl:if test="context()[@Sync = 'TRUE']">
                                    <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Version name:</B></FONT> <xsl:value-of/></DT>
                              </xsl:for-each>
                            </DL>
                            </DD>
                            <BR/>
                          </xsl:for-each>
                        </DL>
                        </DD>
                      </xsl:for-each>
                      <xsl:for-each select="accinstr">
                        <DT><xsl:if test="context()[@Sync = 'TRUE']">
                            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Access instructions:</B></FONT></DT>
                        <DD><xsl:value-of/></DD>
                      </xsl:for-each>
                      <xsl:for-each select="oncomp">
                        <DT><FONT color="#0000AA"><B>Online computer and operating system:</B></FONT></DT>
                        <DD><xsl:value-of/></DD>
                      </xsl:for-each>
                      <xsl:if test="context()[($any$ accinstr | oncomp)]"><BR/><BR/></xsl:if>
                    </DL>
                    </DD>
                  </xsl:for-each>

                  <xsl:for-each select="offoptn">
                    <DT><FONT color="#0000AA"><B>Offline option:</B></FONT></DT>
                    <DD>
                    <DL>
                      <xsl:for-each select="offmedia">
                        <DT><FONT color="#0000AA"><B>Offline media:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="reccap">
                        <DT><FONT color="#0000AA"><B>Recording capacity:</B></FONT></DT>
                        <DD>
                        <DL>
                          <xsl:for-each select="recden">
                            <DT><FONT color="#0000AA"><B>Recording density:</B></FONT> <xsl:value-of/></DT>
                          </xsl:for-each>
                          <xsl:for-each select="recdenu">
                            <DT><FONT color="#0000AA"><B>Recording density Units:</B></FONT> <xsl:value-of/></DT>
                          </xsl:for-each>
                        </DL>
                        </DD>
                      </xsl:for-each>
                      <xsl:for-each select="recfmt">
                        <DT><FONT color="#0000AA"><B>Recording format:</B></FONT> <xsl:value-of/></DT>
                      </xsl:for-each>
                      <xsl:for-each select="compat">
                        <DT><FONT color="#0000AA"><B>Compatibility information:</B></FONT></DT>
                        <DD><xsl:value-of/></DD>
                      </xsl:for-each>
                    </DL>
                    </DD>
                    <BR/>
                  </xsl:for-each>
                </DL>
                </DD>
              </xsl:for-each>
            </DL>
            </DD>
          </xsl:for-each>

          <xsl:for-each select="fees">
            <DT><FONT color="#0000AA"><B>Fees:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:for-each select="ordering">
            <DIV>
              <DT><FONT color="#0000AA"><B>Ordering instructions:</B></FONT></DT>
              <PRE ID="original"><xsl:value-of/></PRE>      
              <SCRIPT>fix(original)</SCRIPT>      
            </DIV>
          </xsl:for-each>
          <xsl:for-each select="turnarnd">
            <DT><FONT color="#0000AA"><B>Turnaround:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:if test="context()[($any$ fees | ordering | turnarnd)]"><BR/><BR/></xsl:if>
        </DL>
        </DD>
      </xsl:for-each>

      <xsl:for-each select="custom">
        <DIV>
          <DT><FONT color="#0000AA"><B>Custom order process:</B></FONT></DT>
          <PRE ID="original"><xsl:value-of/></PRE>      
          <SCRIPT>fix(original)</SCRIPT>      
        </DIV>
      </xsl:for-each>
      <xsl:for-each select="techpreq">
        <DT><FONT color="#0000AA"><B>Technical prerequisites:</B></FONT></DT>
        <DD><xsl:value-of/></DD>
      </xsl:for-each>
      <xsl:if test="context()[($any$ custom | techpreq)]"><BR/><BR/></xsl:if>

      <xsl:for-each select="availabl">
        <DT><FONT color="#0000AA"><B>Available time period:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:apply-templates select="timeinfo"/>
        </DL>
        </DD>
      </xsl:for-each>
    </DL>
    </DD>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

--------

<!-- Metadata -->
<xsl:template match="metainfo">
  <A name="Metadata_Reference_Information"><HR/></A>
  <DL>
    <DT><FONT color="#0000AA" size="3"><B>Metadata Reference Information:</B></FONT></DT>
    <BR/><BR/>
    <DD>
    <DL>
      <xsl:for-each select="metd">
        <DT><xsl:if test="context()[@Sync = 'TRUE']">
            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Metadata date:</B></FONT> <xsl:value-of/></DT>
      </xsl:for-each>
      <xsl:for-each select="metrd">
        <DT><FONT color="#0000AA"><B>Metadata review date:</B></FONT> <xsl:value-of/></DT>
      </xsl:for-each>
      <xsl:for-each select="metfrd">
        <DT><FONT color="#0000AA"><B>Metadata future review date:</B></FONT> <xsl:value-of/></DT>
      </xsl:for-each>
      <xsl:if test="context()[($any$ metd | metrd | metfrd)]"><BR/><BR/></xsl:if>

      <!-- ESRI Profile element  -->
      <xsl:for-each select="langmeta">
        <DT><xsl:if test="context()[@Sync = 'TRUE']">
            <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Language of metadata:</B></FONT> <xsl:value-of/></DT>
        <BR/><BR/>
      </xsl:for-each>

      <xsl:for-each select="metc">
        <DT><FONT color="#0000AA"><B>Metadata contact:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:apply-templates select="cntinfo"/>
        </DL>
        </DD>
        <xsl:if test="context()[not (cntinfo/*)]"><BR/></xsl:if>
      </xsl:for-each>

      <xsl:for-each select="metstdn">
        <DT><xsl:if test="context()[@Sync = 'TRUE']">
            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Metadata standard name:</B></FONT> <xsl:value-of/></DT>
      </xsl:for-each>
      <xsl:for-each select="metstdv">
        <DT><xsl:if test="context()[@Sync = 'TRUE']">
            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Metadata standard version:</B></FONT> <xsl:value-of/></DT>
      </xsl:for-each>
      <xsl:for-each select="mettc">
        <DT><xsl:if test="context()[@Sync = 'TRUE']">
            <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Metadata time convention:</B></FONT> <xsl:value-of/></DT>
      </xsl:for-each>
      <xsl:if test="context()[($any$ metstdn | metstdv | mettc)]"><BR/><BR/></xsl:if>

      <xsl:for-each select="metac">
        <DT><FONT color="#0000AA"><B>Metadata access constraints:</B></FONT> <xsl:value-of/></DT>
      </xsl:for-each>
      <xsl:for-each select="metuc">
        <DT><FONT color="#0000AA"><B>Metadata use constraints:</B></FONT></DT>
        <DD><xsl:value-of/></DD>
      </xsl:for-each>
      <xsl:if test="context()[($any$ metac | metuc)]"><BR/><BR/></xsl:if>

      <xsl:for-each select="metsi">
        <DT><FONT color="#0000AA"><B>Metadata security information:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="metscs">
            <DT><FONT color="#0000AA"><B>Metadata security classification system:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:for-each select="metsc">
            <DT><FONT color="#0000AA"><B>Metadata security classification:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:for-each select="metshd">
            <DT><FONT color="#0000AA"><B>Metadata security handling description:</B></FONT></DT>
            <DD><xsl:value-of/></DD>
          </xsl:for-each>
        </DL>
        </DD>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="metextns">
        <DT><FONT color="#0000AA"><B>Metadata extensions:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="onlink">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Online linkage:</B></FONT> <A TARGET="viewer">
              <xsl:attribute name="HREF"><xsl:value-of/></xsl:attribute>
              <xsl:value-of/></A>
            </DT>
          </xsl:for-each>
          <xsl:for-each select="metprof">
            <DT><xsl:if test="context()[@Sync = 'TRUE']">
                <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Profile name:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
        </DL>
        </DD>
        <BR/>
      </xsl:for-each>
    </DL>
    </DD>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

--------

<!-- ESRI Profile element -->
<!-- Geoprocessing history -->
<xsl:template match="Esri/DataProperties/lineage">
  <A name="Geoprocessing"><HR/></A>
  <DL>
    <DT><FONT color="#006400" size="3"><B>Geoprocessing History:</B></FONT></DT>
    <BR/><BR/>
    <DD>
    <DL>
      <xsl:for-each select="Process">
        <DT><FONT color="#006400"><B>Process:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:if test="@Name">
            <DT><FONT color="#006400">*<B>Process name:</B></FONT> <xsl:value-of select="@Name"/></DT>
          </xsl:if>
          <xsl:if test="@Date">
            <DT><FONT color="#006400">*<B>Date:</B></FONT> <xsl:value-of select="@Date"/></DT>
          </xsl:if>
          <xsl:if test="@Time">
            <DT><FONT color="#006400">*<B>Time:</B></FONT> <xsl:value-of select="@Time"/></DT>
          </xsl:if>
          <xsl:if test="@ToolSource">
            <DT><FONT color="#006400">*<B>Tool location:</B></FONT> <xsl:value-of select="@ToolSource"/></DT>
          </xsl:if>
          <DT><FONT color="#006400">*<B>Command issued:</B></FONT> <xsl:value-of /></DT>
          <BR/><BR/>
        </DL>
        </DD>
      </xsl:for-each>
    </DL>
    </DD>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

--------

<!-- ESRI Profile element  -->
<!-- Locators -->
<xsl:template match="Esri/Locator">
  <A name="Locator"><HR/></A>
  <DL>
    <DT><FONT color="#006400" size="3"><B>Locator Information:</B></FONT></DT>
    <BR/><BR/>
	<DD>
	<DL>
	  <xsl:for-each select="Style">
		<DT><FONT color="#006400"><B>Address locator style:</B></FONT> <xsl:value-of/></DT>
		<BR/><BR/>
	  </xsl:for-each>
	  <xsl:for-each select="Properties">
		<xsl:if expr="(this.selectNodes('FieldAliases').length > 0)">
		  <DT><FONT color="#006400"><B>Input fields:</B></FONT></DT>
		  <DD>
		  <DL>
			<xsl:for-each select="FieldAliases">
			  <DT><FONT color="#006400"><B>-</B></FONT> <xsl:value-of /></DT>
			</xsl:for-each>
		  </DL>
		  </DD>
		</xsl:if>
		<xsl:if test="context()[($any$ FileMAT | FileSTN | IntFileMAT | IntFileSTN)]">
		  <BR/>
		  <DT><FONT color="#006400"><B>Geocoding rule bases:</B></FONT></DT>
		  <DD>
		  <DL>
			<xsl:for-each select="FileMAT">
			  <DT><FONT color="#006400"><B>Match rules:</B></FONT> <xsl:value-of /></DT>
			</xsl:for-each>
			<xsl:for-each select="FileSTN">
			  <DT><FONT color="#006400"><B>Standardization rules:</B></FONT> <xsl:value-of /></DT>
			</xsl:for-each>
			<xsl:for-each select="IntFileMAT">
			  <DT><FONT color="#006400"><B>Intersection match rules:</B></FONT> <xsl:value-of /></DT>
			</xsl:for-each>
			<xsl:for-each select="IntFileSTN">
			  <DT><FONT color="#006400"><B>Intersection standardization rules:</B></FONT> <xsl:value-of /></DT>
			</xsl:for-each>
		  </DL>
		  </DD>
		</xsl:if>
		<xsl:for-each select="Fallback">
		  <BR/>
		  <DT><FONT color="#006400"><B>Fallback matching:</B></FONT> <xsl:value-of/></DT>
		</xsl:for-each>
	  </xsl:for-each>
	</DL>
	</DD>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>


--------

<!-- ESRI Profile element  -->
<!-- Binary enclosures -->
<xsl:template match="Binary">
  <A name="Binary_Enclosures"><HR/></A>
  <DL>
    <DT><FONT color="#006400" size="3"><B>Binary Enclosures:</B></FONT></DT>
    <BR/><BR/>
    <DD>
    <DL>
      <xsl:for-each select="Thumbnail">
        <DT><FONT color="#006400"><B>Thumbnail:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="img">
            <DT><FONT color="#006400"><B>Enclosure type:</B></FONT> Picture</DT>
            <BR/><BR/>
            <IMG ID="thumbnail" ALIGN="absmiddle" STYLE="height:144; 
                border:'2 outset #FFFFFF'; position:relative">
              <xsl:attribute name="SRC"><xsl:value-of select="@src"/></xsl:attribute>
            </IMG>
          </xsl:for-each>
        </DL>
        </DD>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="Enclosure">
        <DT><FONT color="#006400"><B>Enclosure:</B></FONT></DT>
        <DD>
        <DL>
          <xsl:for-each select="*/@EsriPropertyType">
            <DT><FONT color="#006400"><B>Enclosure type:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:for-each select="img">
            <DT><FONT color="#006400"><B>Enclosure type:</B></FONT> Image</DT>
           </xsl:for-each>
          <xsl:for-each select="*/@OriginalFileName">
            <DT><FONT color="#006400"><B>Original file name:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:for-each select="Descript">
            <DT><FONT color="#006400"><B>Description of enclosure:</B></FONT> <xsl:value-of/></DT>
          </xsl:for-each>
          <xsl:for-each select="img">
            <DD>
              <BR/>
              <IMG STYLE="height:144; border:'2 outset #FFFFFF'; position:relative">
                <xsl:attribute name="TITLE"><xsl:value-of select="img/@OriginalFileName"/></xsl:attribute>
                <xsl:attribute name="SRC"><xsl:value-of select="@src"/></xsl:attribute>
              </IMG>
            </DD>
           </xsl:for-each>
        </DL>
        </DD>
        <BR/>
      </xsl:for-each>
    </DL>
    </DD>
  </DL>
  <A HREF="#Top">Back to Top</A>
</xsl:template>

--------

<!-- Citation -->
<xsl:template match="citeinfo">
  <DT><FONT color="#0000AA"><B>Citation information:</B></FONT></DT>
  <DD>
  <DL>
    <xsl:if test="context()[origin/text()]">
      <DT>
      <xsl:for-each select="origin[text()]">
        <xsl:if test="context()[0]"><FONT color="#0000AA"><B>Originators:</B></FONT> </xsl:if>
        <xsl:choose>
          <xsl:when test="context()[. = 'REQUIRED: The name of an organization or individual that developed the data set.']">
            <FONT color="#999999"><xsl:value-of /></FONT>
          </xsl:when>
          <xsl:when test="context()[. = 'The name of an organization or individual that developed the data set.  REQUIRED.']">
            <FONT color="#999999"><xsl:value-of /></FONT>
          </xsl:when>
          <xsl:otherwise><xsl:value-of /></xsl:otherwise>
        </xsl:choose><xsl:if test="context()[not(end())]">, </xsl:if>
      </xsl:for-each>
      </DT>
    </xsl:if>
    <xsl:if test="context()[origin]"><BR/><BR/></xsl:if>

    <xsl:for-each select="title">
      <DT><xsl:if test="context()[@Sync = 'TRUE']">
          <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Title:</B></FONT></DT>
      <DD><xsl:value-of/></DD>
    </xsl:for-each>
    <!-- ESRI Profile element  -->
    <xsl:for-each select="ftname">
      <DT><xsl:if test="context()[@Sync = 'TRUE']">
          <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>File or table name:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:if test="context()[($any$ title | ftname)]"><BR/><BR/></xsl:if>

    <xsl:for-each select="pubdate">
      <DT>
        <FONT color="#0000AA"><B>Publication date:</B></FONT> 
        <xsl:choose>
          <xsl:when test="context()[. = 'REQUIRED: The date when the data set is published or otherwise made available for release.']">
            <FONT color="#999999"><xsl:value-of /></FONT>
          </xsl:when>
          <xsl:when test="context()[. = 'The date when the data set is published or otherwise made available for release.  REQUIRED']">
            <FONT color="#999999"><xsl:value-of /></FONT>
          </xsl:when>
          <xsl:otherwise><xsl:value-of/></xsl:otherwise>
        </xsl:choose>
      </DT>
    </xsl:for-each>
    <xsl:for-each select="pubtime">
      <DT><FONT color="#0000AA"><B>Publication time:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:for-each select="edition">
      <DT><FONT color="#0000AA"><B>Edition:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:for-each select="geoform">
      <DT><xsl:if test="context()[@Sync = 'TRUE']">
          <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Geospatial data presentation form:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:if test="context()[($any$ pubdate | pubtime | edition | geoform)]"><BR/><BR/></xsl:if>

    <xsl:for-each select="serinfo">
      <DT><FONT color="#0000AA"><B>Series information:</B></FONT></DT>
      <DD>
      <DL>
        <xsl:for-each select="sername">
          <DT><FONT color="#0000AA"><B>Series name:</B></FONT> <xsl:value-of/></DT>
        </xsl:for-each>
        <xsl:for-each select="issue">
          <DT><FONT color="#0000AA"><B>Issue identification:</B></FONT> <xsl:value-of/></DT>
        </xsl:for-each>
      </DL>
      </DD>
      <BR/>
    </xsl:for-each>

    <xsl:for-each select="pubinfo">
      <DT><FONT color="#0000AA"><B>Publication information:</B></FONT></DT>
      <DD>
      <DL>
        <xsl:for-each select="pubplace">
          <DT><FONT color="#0000AA"><B>Publication place:</B></FONT> <xsl:value-of/></DT>
        </xsl:for-each>
        <xsl:for-each select="publish">
          <DT><FONT color="#0000AA"><B>Publisher:</B></FONT> <xsl:value-of/></DT>
        </xsl:for-each>
      </DL>
      </DD>
      <BR/>
    </xsl:for-each>

    <xsl:for-each select="othercit">
      <DT><FONT color="#0000AA"><B>Other citation details:</B></FONT></DT>
      <DD><xsl:value-of/></DD>
      <BR/><BR/>
    </xsl:for-each>

    <xsl:for-each select="onlink">
      <DT><xsl:if test="context()[@Sync = 'TRUE']">
          <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Online linkage:</B></FONT> <A TARGET="viewer">
        <xsl:attribute name="HREF"><xsl:value-of/></xsl:attribute>
        <xsl:value-of/></A>
      </DT>
    </xsl:for-each>
    <xsl:if test="context()[onlink]"><BR/><BR/></xsl:if>

    <xsl:for-each select="lworkcit">
      <DT><FONT color="#0000AA"><B>Larger work citation:</B></FONT></DT>
      <DD>
      <DL>
        <xsl:apply-templates select="citeinfo"/>
      </DL>
      </DD>
    </xsl:for-each>
  </DL>
  </DD>
</xsl:template>

--------

<!-- Contact -->
<xsl:template match="cntinfo">
  <DT><FONT color="#0000AA"><B>Contact information:</B></FONT></DT>
  <DD>
  <DL>
    <xsl:for-each select="cntperp">
      <DT><FONT color="#0000AA"><B>Contact person primary:</B></FONT></DT>
      <DD>
      <DL>
        <xsl:for-each select="cntper">
          <DT>
            <FONT color="#0000AA"><B>Contact person:</B></FONT>
            <xsl:choose>
              <xsl:when test="context()[. = 'REQUIRED: The person responsible for the metadata information.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:when test="context()[. = 'The person responsible for the metadata information.  REQUIRED.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:otherwise><xsl:value-of /></xsl:otherwise>
            </xsl:choose>
          </DT>
        </xsl:for-each>
        <xsl:for-each select="cntorg">
          <DT>
            <FONT color="#0000AA"><B>Contact organization:</B></FONT>
            <xsl:choose>
              <xsl:when test="context()[. = 'REQUIRED: The organization responsible for the metadata information.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:when test="context()[. = 'The organization responsible for the metadata information.  REQUIRED.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:otherwise><xsl:value-of /></xsl:otherwise>
            </xsl:choose>
          </DT>
        </xsl:for-each>
      </DL>
      </DD>
    </xsl:for-each>
    <xsl:for-each select="cntorgp">
      <DT><FONT color="#0000AA"><B>Contact organization primary:</B></FONT></DT>
      <DD>
      <DL>
        <xsl:for-each select="cntper">
          <DT>
            <FONT color="#0000AA"><B>Contact person:</B></FONT>
            <xsl:choose>
              <xsl:when test="context()[. = 'REQUIRED: The person responsible for the metadata information.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:when test="context()[. = 'The person responsible for the metadata information.  REQUIRED.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:otherwise><xsl:value-of /></xsl:otherwise>
            </xsl:choose>
          </DT>
        </xsl:for-each>
        <xsl:for-each select="cntorg">
          <DT>
            <FONT color="#0000AA"><B>Contact organization:</B></FONT>
            <xsl:choose>
              <xsl:when test="context()[. = 'REQUIRED: The organization responsible for the metadata information.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:when test="context()[. = 'The organization responsible for the metadata information.  REQUIRED.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:otherwise><xsl:value-of /></xsl:otherwise>
            </xsl:choose>
          </DT>
        </xsl:for-each>
      </DL>
      </DD>
    </xsl:for-each>
    <xsl:for-each select="cntpos">
      <DT><FONT color="#0000AA"><B>Contact position:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:if test="context()[($any$ */cntper | */cntorg | cntpos)]"><BR/></xsl:if>
    <xsl:if test="context()[cntpos]"><BR/></xsl:if>

    <xsl:for-each select="cntaddr">
      <DT><FONT color="#0000AA"><B>Contact address:</B></FONT></DT>
      <DD>
      <DL>
        <xsl:for-each select="addrtype">
          <DT>
            <FONT color="#0000AA"><B>Address type:</B></FONT>
            <xsl:choose>
              <xsl:when test="context()[. = 'REQUIRED: The mailing and/or physical address for the organization or individual.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:when test="context()[. = 'The mailing and/or physical address for the organization or individual.  REQUIRED.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:otherwise><xsl:value-of /></xsl:otherwise>
            </xsl:choose>
          </DT>
        </xsl:for-each>
        <xsl:for-each select="address">
          <DIV>
            <DT><FONT color="#0000AA"><B>Address:</B></FONT></DT>
            <PRE ID="original"><xsl:value-of/></PRE>      
            <SCRIPT>fix(original)</SCRIPT>      
          </DIV>
        </xsl:for-each>
        <xsl:for-each select="city">
          <DT>
            <FONT color="#0000AA"><B>City:</B></FONT>
            <xsl:choose>
              <xsl:when test="context()[. = 'REQUIRED: The city of the address.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:when test="context()[. = 'The city of the address.  REQUIRED.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:otherwise><xsl:value-of /></xsl:otherwise>
            </xsl:choose>
          </DT>
        </xsl:for-each>
        <xsl:for-each select="state">
          <DT>
            <FONT color="#0000AA"><B>State or province:</B></FONT>
            <xsl:choose>
              <xsl:when test="context()[. = 'REQUIRED: The state or province of the address.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:when test="context()[. = 'The state or province of the address.  REQUIRED.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:otherwise><xsl:value-of /></xsl:otherwise>
            </xsl:choose>
          </DT>
        </xsl:for-each>
        <xsl:for-each select="postal">
          <DT>
            <FONT color="#0000AA"><B>Postal code:</B></FONT>
            <xsl:choose>
              <xsl:when test="context()[. = 'REQUIRED: The ZIP or other postal code of the address.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:when test="context()[. = 'The ZIP or other postal code of the address.  REQUIRED.']">
                <FONT color="#999999"><xsl:value-of /></FONT>
              </xsl:when>
              <xsl:otherwise><xsl:value-of /></xsl:otherwise>
            </xsl:choose>
          </DT>
        </xsl:for-each>
        <xsl:for-each select="country">
          <DT><FONT color="#0000AA"><B>Country:</B></FONT> <xsl:value-of/></DT>
        </xsl:for-each>
      </DL>
      </DD>
      <BR/>
    </xsl:for-each>

    <xsl:for-each select="cntvoice">
      <DT>
        <FONT color="#0000AA"><B>Contact voice telephone:</B></FONT>
        <xsl:choose>
          <xsl:when test="context()[. = 'REQUIRED: The telephone number by which individuals can speak to the organization or individual.']">
            <FONT color="#999999"><xsl:value-of /></FONT>
          </xsl:when>
          <xsl:when test="context()[. = 'The telephone number by which individuals can speak to the organization or individual.  REQUIRED.']">
            <FONT color="#999999"><xsl:value-of /></FONT>
          </xsl:when>
          <xsl:otherwise><xsl:value-of /></xsl:otherwise>
        </xsl:choose>
      </DT>
    </xsl:for-each>
    <xsl:for-each select="cnttdd">
      <DT><FONT color="#0000AA"><B>Contact TDD/TTY telephone:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:for-each select="cntfax">
      <DT><FONT color="#0000AA"><B>Contact facsimile telephone:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:if test="context()[($any$ cntvoice | cnttdd | cntfax)]"><BR/><BR/></xsl:if>

    <xsl:for-each select="cntemail">
      <DT><FONT color="#0000AA"><B>Contact electronic mail address:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:if test="context()[cntemail]"><BR/><BR/></xsl:if>

    <xsl:for-each select="hours">
      <DT><FONT color="#0000AA"><B>Hours of service:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:for-each select="cntinst">
      <DT><FONT color="#0000AA"><B>Contact instructions:</B></FONT></DT>
      <DIV>
        <PRE ID="original"><xsl:value-of/></PRE>      
        <SCRIPT>fix(original)</SCRIPT>      
      </DIV>
    </xsl:for-each>
    <xsl:if test="context()[($any$ hours | cntinst)]"><BR/></xsl:if>
    <xsl:if test="context()[($any$ hours | cntinst) and not (cntinst)]"><BR/><BR/></xsl:if>
  </DL>
  </DD>
</xsl:template>

--------

<!-- Time Period Info -->
<xsl:template match="timeinfo">
  <DT><FONT color="#0000AA"><B>Time period information:</B></FONT></DT>
  <DD>
  <DL>
    <xsl:apply-templates select="sngdate"/>
    <xsl:apply-templates select="mdattim"/>
    <xsl:apply-templates select="rngdates"/>
  </DL>
  </DD>
  <BR/>
</xsl:template>

<!-- Single Date/Time -->
<xsl:template match="sngdate">
  <DT><FONT color="#0000AA"><B>Single date/time:</B></FONT></DT>
  <DD>
  <DL>
    <xsl:for-each select="caldate">
      <DT>
        <FONT color="#0000AA"><B>Calendar date:</B></FONT>
        <xsl:choose>
          <xsl:when test="context()[. = 'REQUIRED: The year (and optionally month, or month and day) for which the data set corresponds to the ground.']">
            <FONT color="#999999"><xsl:value-of /></FONT>
          </xsl:when>
          <xsl:when test="context()[. = 'The year (and optionally month, or month and day) for which the data set corresponds to the ground.  REQUIRED.']">
            <FONT color="#999999"><xsl:value-of /></FONT>
          </xsl:when>
          <xsl:otherwise><xsl:value-of/></xsl:otherwise>
        </xsl:choose>
      </DT>
    </xsl:for-each>
    <xsl:for-each select="time">
      <DT><FONT color="#0000AA"><B>Time of day:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
  </DL>
  </DD>
</xsl:template>

<!-- Multiple Date/Time -->
<xsl:template match="mdattim">
  <DT><FONT color="#0000AA"><B>Multiple dates/times:</B></FONT></DT>
  <DD>
  <DL>
    <xsl:apply-templates select="sngdate"/>
  </DL>
  </DD>
</xsl:template>

<!-- Range of Dates/Times -->
<xsl:template match="rngdates">
  <DT><FONT color="#0000AA"><B>Range of dates/times:</B></FONT></DT>
  <DD>
  <DL>
    <xsl:for-each select="begdate">
      <DT><FONT color="#0000AA"><B>Beginning date:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:for-each select="begtime">
      <DT><FONT color="#0000AA"><B>Beginning time:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:for-each select="enddate">
      <DT><FONT color="#0000AA"><B>Ending date:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:for-each select="endtime">
      <DT><FONT color="#0000AA"><B>Ending time:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
  </DL>
  </DD>
</xsl:template>

--------

<!-- G-Ring -->
<xsl:template match="grngpoin">
  <DT><FONT color="#0000AA"><B>G-Ring point:</B></FONT></DT>
  <DD>
  <DL>
    <xsl:for-each select="gringlat">
      <DT><FONT color="#0000AA"><B>G-Ring latitude:</B></FONT> <xsl:value-of/></DT>
        </xsl:for-each>
        <xsl:for-each select="gringlon">
      <DT><FONT color="#0000AA"><B>G-Ring longitude:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
  </DL>
  </DD>
  <BR/>
</xsl:template>
<xsl:template match="gring">
  <DT><FONT color="#0000AA"><B>G-Ring:</B></FONT></DT>
  <DD><xsl:value-of/></DD>
  <BR/><BR/>
</xsl:template>

--------

<!-- Map Projections -->
<!-- Projections explicitly supported in the FGDC standard -->
<xsl:template match="albers | azimequi | equicon | equirect | gnomonic | gvnsp | lamberta | 
    lambertc | mercator | miller | modsak | obqmerc | orthogr | polarst | polycon | robinson | 
    sinusoid | spaceobq | stereo | transmer | vdgrin | mapprojp">
  <DL><xsl:apply-templates select="*"/></DL>
</xsl:template>

<!-- Used at 8.0 for projections not explicitly supported in FGDC; mapprojp used at 8.1 -->
<xsl:template match="behrmann | bonne | cassini | eckert1 | eckert2 | eckert3 | eckert4 | 
    eckert5 | eckert6 | gallster | loximuth | mollweid | quartic | winkel1 | winkel2">
  <DL><xsl:apply-templates select="*"/></DL>
</xsl:template>

<!-- Map Projection Parameters -->
<xsl:template match="stdparll">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Standard parallel:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="longcm">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Longitude of central meridian:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="latprjo">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Latitude of projection origin:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="feast">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>False easting:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="fnorth">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>False northing:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="sfequat">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Scale factor at equator:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="heightpt">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Height of perspective point above surface:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="longpc">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Longitude of projection center:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="latprjc">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Latitude of projection center:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="sfctrlin">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Scale factor at center line:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="obqlazim">
  <DT><FONT color="#0000AA"><B>Oblique line azimuth:</B></FONT></DT>
  <DD>
  <DL>
    <xsl:for-each select="azimangl">
      <DT><xsl:if test="context()[@Sync = 'TRUE']">
          <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Azimuthal angle:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:for-each select="azimptl">
      <DT><xsl:if test="context()[@Sync = 'TRUE']">
          <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Azimuthal measure point longitude:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
  </DL>
  </DD>
</xsl:template>

<xsl:template match="obqlpt">
  <DT><FONT color="#0000AA"><B>Oblique line point:</B></FONT></DT>
  <DD>
  <DL>
    <xsl:for-each select="obqllat">
      <DT><xsl:if test="context()[@Sync = 'TRUE']">
          <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Oblique line latitude:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
    <xsl:for-each select="obqllong">
      <DT><xsl:if test="context()[@Sync = 'TRUE']">
          <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Oblique line longitude:</B></FONT> <xsl:value-of/></DT>
    </xsl:for-each>
  </DL>
  </DD>
</xsl:template>

<xsl:template match="svlong">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Straight vertical longitude from pole:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="sfprjorg">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Scale factor at projection origin:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="landsat">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Landsat number:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="pathnum">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Path number:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="sfctrmer">
  <DT><xsl:if test="context()[@Sync = 'TRUE']">
      <FONT color="#006400">*</FONT></xsl:if><FONT color="#0000AA"><B>Scale factor at central meridian:</B></FONT> <xsl:value-of/></DT>
</xsl:template>

<xsl:template match="otherprj">
  <xsl:choose>
    <xsl:when test="context()[*]">
      <DT><xsl:if test="context()[@Sync = 'TRUE']">
          <FONT color="#006400">*</FONT></xsl:if><FONT color="#006400"><B>Other ESRI projection:</B></FONT></DT>
      <DL><xsl:apply-templates select="*"/></DL>
    </xsl:when>
    <xsl:otherwise>
      <DT><FONT color="#0000AA"><B>Other projection's definition:</B></FONT></DT>
      <DD><xsl:value-of/></DD>
    </xsl:otherwise>
  </xsl:choose>
</xsl:template>

</xsl:stylesheet>