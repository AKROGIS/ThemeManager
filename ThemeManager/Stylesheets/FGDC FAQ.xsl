<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/TR/WD-xsl" TYPE="text/javascript">

<!-- An xsl template for displaying metadata in ArcCatalog with the
     traditional FGDC look and feel created by mp

     Copyright (c) 2000-2008, Environmental Systems Research Institute, Inc. All rights reserved.
     	
     Revision History: Created 03/17/00 avienneau

-->

<xsl:template match="/">
<HTML>
  <HEAD>
    <SCRIPT LANGUAGE="JScript"><xsl:comment><![CDATA[
      function fix(e) {
        var par = e.parentNode;
        e.id = "";
        var pos = e.innerText.indexOf("\n");
        if (pos > 0) {
          while (pos > 0) {
            var t = e.childNodes(0);
            var n = document.createElement("PRE");
            var s = t.splitText(pos);
            e.insertAdjacentElement("afterEnd", n);
            n.appendChild(s);
            e = n;
            pos = e.innerText.indexOf("\n");
          }
          var count = (par.children.length);
          for (var i = 0; i < count; i++) {
            e = par.children(i);
            if (e.tagName == "PRE") {
              pos = e.innerText.indexOf(">");
              if (pos != 0) {
                n = document.createElement("DIV");
                e.insertAdjacentElement("afterEnd", n);
                n.innerText = e.innerText;
                e.removeNode(true);
              }
            }
          }
          if (par.children.tags("PRE").length > 0) {
            count = (par.childNodes.length);
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
          if (par.tagName == "SPAN") {
            e = par.children(par.children.length - 2);
            if (e.tagName == "DIV") {
              n = document.createElement("SPAN");
              e.insertAdjacentElement("afterEnd", n);
              n.innerText = e.innerText + " ";
              e.removeNode(true);
            }
          }
        }
        else {
          n = document.createElement("DIV");
          par.appendChild(n);
          n.innerText = e.innerText;
          e.removeNode(true);
          if (par.tagName == "SPAN") {
            e = par.lastChild;
            if (e.tagName == "DIV") {
              n = document.createElement("SPAN");
              e.insertAdjacentElement("afterEnd", n);
              n.innerText = e.innerText + " ";
              e.removeNode(true);
            }
          }
        }
      }
    ]]></xsl:comment></SCRIPT>
  </HEAD>

  <BODY oncontextmenu="return true">

    <A name="Top"/>
    <xsl:for-each select="metadata/idinfo/citation/citeinfo/title[. != '']">
      <H3><xsl:value-of/></H3>
    </xsl:for-each>

    <H4>Frequently-asked questions:</H4>
    <UL>
      <LI>
        <A href="#what">What does this data set describe?</A> 
        <OL>
          <LI><A href="#what.1">How should this data set be cited?</A></LI>
          <LI><A href="#what.2">What geographic area does the data set cover?</A></LI>
          <LI><A href="#what.3">What does it look like?</A></LI>
          <LI><A href="#what.4">Does the data set describe conditions during a particular time period?</A></LI>
          <LI><A href="#what.5">What is the general form of this data set?</A></LI>
          <LI><A href="#what.6">How does the data set represent geographic features?</A></LI>
          <LI><A href="#what.7">How does the data set describe geographic features?</A></LI>
        </OL>
      </LI>
      <LI>
        <A href="#who">Who produced the data set?</A> 
        <OL>
          <LI><A href="#who.1">Who are the originators of the data set?</A></LI>
          <LI><A href="#who.2">Who also contributed to the data set?</A></LI>
          <LI><A href="#who.3">To whom should users address questions about the data?</A></LI>
        </OL>
      </LI>
      <LI><A href="#why">Why was the data set created?</A></LI>
      <LI>
        <A href="#how">How was the data set created?</A> 
        <OL>
          <LI><A href="#how.1">Where did the data come from?</A></LI>
          <LI><A href="#how.2">What changes have been made?</A></LI>
        </OL>
      </LI>
      <LI>
        <A href="#quality">How reliable are the data; what problems remain in the data set?</A> 
        <OL>
          <LI><A href="#quality.1">How well have the observations been checked?</A></LI>
          <LI><A href="#quality.2">How accurate are the geographic locations?</A></LI>
          <LI><A href="#quality.3">How accurate are the heights or depths?</A></LI>
          <LI><A href="#quality.4">Where are the gaps in the data? What is missing?</A></LI>
          <LI><A href="#quality.5">How consistent are the relationships among the data, including topology?</A></LI>
        </OL>
      </LI>
      <LI>
        <A href="#getacopy">How can someone get a copy of the data set?</A> 
        <OL>
          <LI><A href="#getacopy.0">Are there legal restrictions on access or use of the data?</A></LI>
          <LI><A href="#getacopy.1">Who distributes the data?</A></LI>
          <LI><A href="#getacopy.2">What's the catalog number I need to order this data set?</A></LI>
          <LI><A href="#getacopy.3">What legal disclaimers am I supposed to read?</A></LI>
          <LI><A href="#getacopy.4">How can I download or order the data?</A></LI>
          <LI><A href="#getacopy.5">Is there some other way to get the data?</A></LI>
          <LI><A href="#getacopy.6">What hardware or software do I need in order to use the data set?</A></LI>
        </OL>
      </LI>
      <LI><A href="#metaref">Who wrote the metadata?</A></LI>
    </UL>
    <HR/>

    <A name="what"><H3>What does this data set describe?</H3></A>
    <DIV STYLE="margin-left:'0.2in'">
      <xsl:for-each select="metadata/idinfo/citation/citeinfo/title[. != '']">
        <DIV><I>Title:</I></DIV>
        <DIV STYLE="margin-left:'0.4in'"><xsl:value-of/></DIV>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="metadata/idinfo/descript/abstract[. != '']">
        <DIV><I>Abstract:</I></DIV>
        <DIV STYLE="margin-left:'0.4in'">
          <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
          <SCRIPT>fix(original)</SCRIPT>      
        </DIV>
        <BR/>
      </xsl:for-each>

      <xsl:for-each select="metadata/idinfo/descript/supplinf[. != '']">
        <DIV><I>Supplemental information:</I></DIV>
        <DIV STYLE="margin-left:'0.4in'">
          <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
          <SCRIPT>fix(original)</SCRIPT>      
        </DIV>
        <BR/>
      </xsl:for-each>
    </DIV>

    <OL>
      <LI>
        <A name="what.1"><B>How should this data set be cited?</B></A>
        <BR/><BR/>
        <xsl:apply-templates select="metadata/idinfo/citation/citeinfo"/>
      </LI>

      <LI>
        <A name="what.2"><B>What geographic area does the data set cover?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/idinfo/spdom/bounding[$any$ * != '']">
          <DIV STYLE="margin-left:'0.2in'">
            <DIV>Bounding coordinates:</DIV>
            <DIV STYLE="margin-left:'0.4in'">
              <xsl:for-each select="westbc[. != '']"><DIV><I>West:</I> <xsl:value-of/></DIV></xsl:for-each>
              <xsl:for-each select="eastbc[. != '']"><DIV><I>East:</I> <xsl:value-of/></DIV></xsl:for-each>
              <xsl:for-each select="northbc[. != '']"><DIV><I>North:</I> <xsl:value-of/></DIV></xsl:for-each>
              <xsl:for-each select="southbc[. != '']"><DIV><I>South:</I> <xsl:value-of/></DIV></xsl:for-each>
            </DIV>
          </DIV>
          <BR/>
        </xsl:for-each>
      </LI>

      <LI>
        <A name="what.3"><B>What does it look like?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/idinfo/browse[(browsen != '') or (browset != '') or 
            (browsed != '')]">
          <DIV STYLE="margin-left:'0.2in'">
            <DIV>
              <xsl:for-each select="browsen[. != '']">
                <A TARGET="viewer"><xsl:attribute name="HREF"><xsl:value-of/></xsl:attribute><xsl:value-of/></A>
              </xsl:for-each>
              <xsl:for-each select="browset[. != '']">
                (<xsl:value-of/>)
              </xsl:for-each>
            </DIV>
            <xsl:for-each select="browsed[. != '']">
              <DIV STYLE="margin-left:'0.4in'"><xsl:value-of/></DIV>
            </xsl:for-each>
          </DIV>
          <BR/>
        </xsl:for-each>
      </LI>

      <LI>
        <A name="what.4"><B>Does the data set describe conditions during a particular time period?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/idinfo/timeperd[(.//caldate != '') or 
            (timeinfo/rngdates/* != '') or (current != '')]">
          <xsl:apply-templates select="timeinfo"/>
          <xsl:for-each select="current[. != '']">
            <DIV STYLE="margin-left:'0.2in'"><I>Currentness reference:</I></DIV>
            <DIV STYLE="margin-left:'0.6in'">
              <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
              <SCRIPT>fix(original)</SCRIPT>      
            </DIV>
          </xsl:for-each>
          <BR/>
        </xsl:for-each>
     </LI>

      <LI>
        <A name="what.5"><B>What is the general form of this data set?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/idinfo/citation/citeinfo/geoform[. != '']">
          <DIV STYLE="margin-left:'0.2in'"><I>Geospatial data presentation form:</I> <xsl:value-of/></DIV>
          <BR/>
        </xsl:for-each>
      </LI>

      <LI>
        <A name="what.6"><B>How does the data set represent geographic features?</B></A>
        <BR/><BR/>
        <OL TYPE="a">
          <LI>
            <B>How are geographic features stored in the data set?</B>
            <BR/><BR/>
            <xsl:for-each select="metadata/spdoinfo/indspref[. != '']">
              <DIV STYLE="margin-left:'0.2in'">
                <I>Indirect spatial reference:</I>
                <DIV STYLE="margin-left:'0.4in'"><xsl:value-of/></DIV>
              </DIV>
              <BR/>
            </xsl:for-each>
            <xsl:if test="metadata/spdoinfo[(direct != '') or ($any$ ptvctinf/sdtsterm/* != '') or 
                (ptvctinf/vpfterm//* != '') or  (rastinfo/colcount != '') or 
                (rastinfo/rowcount != '') or (rastinfo/vrtcount != '') or 
                (rastinfo/rasttype != '')]">
              <DIV STYLE="margin-left:'0.2in'">
                <xsl:for-each select="metadata/spdoinfo/direct[. != '']">
                  This is a <xsl:value-of/> data set.
                </xsl:for-each>
                <xsl:if test="metadata/spdoinfo/ptvctinf[$any$ sdtsterm/* != '']">
                  It contains the following vector data types (SDTS terminology):
                  <UL>
                    <xsl:for-each select="metadata/spdoinfo/ptvctinf/sdtsterm[$any$ * != '']">
                      <LI><xsl:for-each select="sdtstype[. != '']"><xsl:value-of/> </xsl:for-each><xsl:for-each select="ptvctcnt[. != '']">(<xsl:value-of/>)</xsl:for-each></LI>
                    </xsl:for-each>
                  </UL>
                </xsl:if>
                <xsl:if test="metadata/spdoinfo/ptvctinf[vpfterm//* != '']">
                  <xsl:for-each select="metadata/spdoinfo/ptvctinf/vpfterm/vpflevel[. != '']">
                    The VPF topology level is <xsl:value-of/>.
                  </xsl:for-each>
                  <xsl:if test="metadata/spdoinfo/ptvctinf/vpfterm[$any$ vpfinfo/* != '']">
                    It contains the following vector data types (VPF terminology):
                    <UL>
                      <xsl:for-each select="metadata/spdoinfo/ptvctinf/vpfterm/vpfinfo[$any$ * != '']">
                        <LI><xsl:for-each select="vpftype[. != '']"><xsl:value-of/> </xsl:for-each><xsl:for-each select="ptvctcnt[. != '']">(<xsl:value-of/>)</xsl:for-each></LI>
                      </xsl:for-each>
                    </UL>
                  </xsl:if>
                </xsl:if>
                <xsl:if test="metadata/spdoinfo/rastinfo[(rowcount != '') or 
                    (colcount != '') or (vrtcount != '') or (rasttype != '')]">
                  It contains the following raster data types:
                  <UL>
                    <xsl:for-each select="metadata/spdoinfo/rastinfo">
                      <LI>
                        <xsl:if test="context()[(rowcount != '') or (colcount != '') or 
                            (vrtcount != '')]">
                          Dimensions <xsl:value-of select="rowcount[. != '']"/> x <xsl:value-of select="colcount[. != '']"/><xsl:for-each select="vrtcount[. != '']"> x <xsl:value-of/></xsl:for-each>
                        </xsl:if><xsl:if test="context()[((rowcount != '') or 
                            (colcount != '') or (vrtcount != '')) and (rasttype != '')]">, </xsl:if>
                        <xsl:for-each select="rasttype[. != '']">type <xsl:value-of/></xsl:for-each>
                      </LI>
                    </xsl:for-each>
                  </UL>
                </xsl:if>
              </DIV>
              <BR/>
            </xsl:if>
          </LI>

          <LI>
            <B>What coordinate system is used to represent geographic features?</B>
            <BR/><BR/>
            <xsl:if test="metadata/spref[$any$ .//* != '']">
              <DIV STYLE="margin-left:'0.2in'">
                <xsl:for-each select="metadata/spref/horizsys/geograph[$any$ * != '']">
                  <DIV>
                    Horizontal positions are specified in geographic coordinates, that is, latitude and longitude. 
                    <xsl:for-each select="latres[. != '']">Latitudes are given to the nearest <xsl:value-of />. </xsl:for-each>
                    <xsl:for-each select="longres[. != '']">Longitudes are given to the nearest <xsl:value-of />. </xsl:for-each>
                    <xsl:for-each select="geogunit[. != '']">Latitude and longitude values are specified in <xsl:value-of />. </xsl:for-each>
                  </DIV>
                  <BR/>
                </xsl:for-each>
                <xsl:for-each select="metadata/spref/horizsys/planar[$any$ .//* != '']">
                  <xsl:for-each select="mapproj[$any$ .//* != '']">
                    <DIV>The map projection used is <xsl:value-of select="mapprojn[. != '']"/>.</DIV>
                    <BR/>
                    <DIV>Projection parameters:</DIV>
                    <xsl:apply-templates select="*"/>
                    <BR/>
                  </xsl:for-each>
                  <xsl:for-each select="gridsys[$any$ .//* != '']">
                    <DIV>The grid coordinate system used is <xsl:value-of select="gridsysn[. != '']"/></DIV>
                    <BR/>
                    <xsl:apply-templates select="*"/>
                    <BR/>
                  </xsl:for-each>
                  <xsl:for-each select="localp[$any$ * != '']">
                    <DIV>
                      Horizontal coordinates are specified using a local planar system.
                      <xsl:for-each select="localpd[. != '']"><xsl:value-of/></xsl:for-each>
                    </DIV>
                    <BR/>
                    <xsl:for-each select="localpgi[. != '']"><DIV><xsl:value-of/></DIV><BR/></xsl:for-each>
                  </xsl:for-each>
                  <xsl:for-each select="planci[$any$ * != '']">
                    <xsl:for-each select="plance[. != '']"><DIV>Planar coordinates are encoded using <xsl:value-of/>.</DIV></xsl:for-each>
                    <xsl:for-each select="coordrep/absres[. != '']"><DIV>Abscissae (x-coordinates) are specified to the nearest <xsl:value-of/>.</DIV></xsl:for-each>
                    <xsl:for-each select="coordrep/ordres[. != '']"><DIV>Ordinates (y-coordinates) are specified to the nearest <xsl:value-of/>.</DIV></xsl:for-each>
                    <xsl:for-each select="distbrep[$any$ * != '']">
                      <DIV>Planar coordinates are specified using distance and bearing values.</DIV>
                      <xsl:for-each select="distres[. != '']"><DIV>Resolution of distance values: <xsl:value-of/></DIV></xsl:for-each>
                      <xsl:for-each select="bearres[. != '']"><DIV>Resolution of bearing values: <xsl:value-of/></DIV></xsl:for-each>
                      <xsl:for-each select="bearunit[. != '']"><DIV>Bearing is specified in units of <xsl:value-of/>.</DIV></xsl:for-each>
                      <xsl:for-each select="bearrefd[. != '']"><DIV>Bearing is measured <xsl:value-of/>.</DIV></xsl:for-each>
                      <xsl:for-each select="bearrefm[. != '']"><DIV>Bearing is measured from the <xsl:value-of/> meridian.</DIV></xsl:for-each>
                    </xsl:for-each>
                    <xsl:for-each select="plandu[. != '']"><DIV>Planar coordinates are specified in <xsl:value-of/>.</DIV></xsl:for-each>
                    <BR/>
                  </xsl:for-each>
                </xsl:for-each>
                <xsl:for-each select="metadata/spref/horizsys/local[$any$ * != '']">
                  This local coordinate system was used: <xsl:value-of select="localdes[. != '']"/>.
                  <BR/><BR/>
                  <xsl:for-each select="localgeo[. != '']"><xsl:value-of/></xsl:for-each>
                  <BR/><BR/>
                </xsl:for-each>
                <xsl:for-each select="metadata/spref/horizsys/geodetic[$any$ * != '']">
                  <xsl:for-each select="horizdn[. != '']"><DIV>The horizontal datum used is <xsl:value-of/>.</DIV></xsl:for-each>
                  <xsl:for-each select="ellips[. != '']"><DIV>The ellipsoid used is <xsl:value-of/>.</DIV></xsl:for-each>
                  <xsl:for-each select="semiaxis[. != '']"><DIV>The semi-major axis of the ellipsoid used is <xsl:value-of/>.</DIV></xsl:for-each>
                  <xsl:for-each select="denflat[. != '']"><DIV>The flattening of the ellipsoid used is 1/<xsl:value-of/>.</DIV></xsl:for-each>
                  <BR/>
                </xsl:for-each>
                <xsl:for-each select="metadata/spref/vertdef[$any$ .//* != '']">
                  Vertical coordinate system definition:
                  <DIV STYLE="margin-left:'0.2in'">
                    <xsl:for-each select="altsys[$any$ * != '']">
                      Altitude system definition:
                      <DIV STYLE="margin-left:'0.4in'">
                        <xsl:for-each select="altdatum[. != '']"><DIV><I>Altitude datum name:</I>  <xsl:value-of/></DIV></xsl:for-each>
                        <xsl:for-each select="altres[. != '']"><DIV><I>Altitude resolution:</I>  <xsl:value-of/></DIV></xsl:for-each>
                        <xsl:for-each select="altunits[. != '']"><DIV><I>Altitude distance units:</I>  <xsl:value-of/></DIV></xsl:for-each>
                        <xsl:for-each select="altenc[. != '']"><DIV><I>Altitude encoding method:</I>  <xsl:value-of/></DIV></xsl:for-each>
                      </DIV>
                    </xsl:for-each>
                    <xsl:if test="context()[($any$ altsys/* != '') and ($any$ depthsys/* != '')]">
                      <BR/>
                    </xsl:if>
                    <xsl:for-each select="depthsys[$any$ * != '']">
                      Depth system definition:
                      <DIV STYLE="margin-left:'0.4in'">
                        <xsl:for-each select="depthdn[. != '']"><DIV><I>Depth datum name:</I>  <xsl:value-of/></DIV></xsl:for-each>
                        <xsl:for-each select="depthres[. != '']"><DIV><I>Depth resolution:</I>  <xsl:value-of/></DIV></xsl:for-each>
                        <xsl:for-each select="depthdu[. != '']"><DIV><I>Depth distance units:</I>  <xsl:value-of/></DIV></xsl:for-each>
                        <xsl:for-each select="depthem[. != '']"><DIV><I>Depth encoding method:</I>  <xsl:value-of/></DIV></xsl:for-each>
                      </DIV>
                    </xsl:for-each>
                  </DIV>
                  <BR/>
                </xsl:for-each>
              </DIV>
            </xsl:if>
          </LI>
        </OL>
      </LI>

      <LI>
        <A name="what.7"><B>How does the data set describe geographic features?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/eainfo/detailed[enttyp/enttypl != '']">
          <DIV STYLE="margin-left:'0.2in'">
            <DIV><B><TT STYLE="font-size:15"><xsl:value-of select="enttyp/enttypl"/></TT></B></DIV>
            <DIV STYLE="margin-left:'0.4in'">
              <xsl:for-each select="enttyp[(enttypd != '') or (enttypds != '')]">
                <DIV><xsl:for-each select="enttypd[. != '']"><SPAN><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE><SCRIPT>fix(original)</SCRIPT></SPAN></xsl:for-each><xsl:for-each select="enttypds[. != '']">  (Source: <SPAN><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE><SCRIPT>fix(original)</SCRIPT></SPAN>)</xsl:for-each></DIV>
                <BR/>
              </xsl:for-each>
              <xsl:for-each select="attr[attrlabl != '']">
                <DIV><B><TT><xsl:value-of select="attrlabl"/></TT></B></DIV>
                <xsl:for-each select="context()[(attrdef != '') or (attrdefs != '')]">
                  <DIV STYLE="margin-left:'0.4in'"><xsl:for-each select="attrdef[. != '']"><SPAN><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE><SCRIPT>fix(original)</SCRIPT></SPAN></xsl:for-each><xsl:for-each select="attrdefs[. != '']">  (Source: <SPAN><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE><SCRIPT>fix(original)</SCRIPT></SPAN>)</xsl:for-each></DIV>
                  <BR/>
                </xsl:for-each>
                <xsl:for-each select="attrmfrq[. != '']">
                  <DIV STYLE="margin-left:'0.4in'"><I>Frequency of measurement:</I> <xsl:value-of/></DIV>
                  <BR/>
                </xsl:for-each>
                <xsl:for-each select="attrdomv/udom[. != '']">
                  <DIV STYLE="margin-left:'0.4in'"><I><xsl:value-of/></I></DIV>
                  <BR/>
                </xsl:for-each>
                <xsl:if test="attrdomv/edom[(edomv != '') or (edomvd != '')]">
                  <TABLE STYLE="margin-left:'0.4in'" BORDER="1" CELLPADDING="4">
                    <TR>
                      <TH>Value</TH>
                      <TH>Definition</TH>
                    </TR>
                    <xsl:for-each select="attrdomv/edom[(edomv != '') or (edomvd != '')]">
                      <TR>
                        <TD VALIGN="top"><xsl:value-of select="edomv"/></TD>
                        <TD>
                          <DIV>
                            <xsl:choose>
                              <xsl:when test="edomvd[. != '']">
                                <xsl:for-each select="edomvd[. != '']">
                                  <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
                                  <SCRIPT>fix(original)</SCRIPT>
                                </xsl:for-each>
                              </xsl:when>
                              <xsl:otherwise xml:space="preserve">
                                <DIV> [not provided] </DIV></xsl:otherwise></xsl:choose>
                          </DIV>
                        </TD>
                      </TR>
                    </xsl:for-each>
                  </TABLE>
                  <BR/>
                </xsl:if>
                <xsl:if test="attrdomv/rdom[$any$ * != '']">
                  <TABLE STYLE="margin-left:'0.4in'" BORDER="1" CELLPADDING="4">
                    <TR>
                      <TH COLSPAN="2">Range of values</TH>
                    </TR>
                    <xsl:for-each select="attrdomv/rdom/rdommin[. != '']">
                      <TR>
                        <TH>Minimum:</TH>
                        <TD><xsl:value-of/></TD>
                      </TR>
                    </xsl:for-each>
                    <xsl:for-each select="attrdomv/rdom/rdommax[. != '']">
                      <TR>
                        <TH>Maximum:</TH>
                        <TD><xsl:value-of/></TD>
                      </TR>
                    </xsl:for-each>
                    <xsl:for-each select="attrdomv/rdom/attrunit[. != '']">
                      <TR>
                        <TH>Units:</TH>
                        <TD><xsl:value-of/></TD>
                      </TR> 
                    </xsl:for-each>
                    <xsl:for-each select="attrdomv/rdom/attrmres[. != '']">
                      <TR>
                        <TH>Resolution:</TH>
                        <TD><xsl:value-of/></TD>
                      </TR>
                    </xsl:for-each>
                  </TABLE>
                  <BR/>
                </xsl:if>
                <xsl:if test="attrdomv/codesetd[$any$ * != '']">
                  <TABLE STYLE="margin-left:'0.4in'" BORDER="1" CELLPADDING="4">
                    <TR>
                      <TH COLSPAN="2">Formal codeset</TH>
                    </TR>
                    <xsl:for-each select="attrdomv/codesetd/codesetn[. != '']">
                      <TR>
                        <TH>Codeset name:</TH>
                        <TD><xsl:value-of/></TD>
                      </TR>
                    </xsl:for-each>
                    <xsl:for-each select="attrdomv/codesetd/codesets[. != '']">
                      <TR> 
                        <TH>Codeset source:</TH>
                        <TD><xsl:value-of/></TD>
                      </TR>
                    </xsl:for-each>
                  </TABLE>
                  <BR/>
                </xsl:if>
              </xsl:for-each>
            </DIV>
          </DIV>
        </xsl:for-each>
        <xsl:for-each select="metadata/eainfo/overview[$any$ * != '']">
          <DIV STYLE="margin-left:'0.2in'">
            <xsl:for-each select="eaover[. != '']">
              <DIV><I>Entity and attribute overview:</I></DIV>
              <DIV STYLE="margin-left:'0.4in'">
                <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
                <SCRIPT>fix(original)</SCRIPT>      
              </DIV>
            </xsl:for-each>
            <xsl:if test="context()[(eaover != '') and ($any$ eadetcit != '')]">
              <BR/>
            </xsl:if>
            <xsl:for-each select="eadetcit[. != '']">
              <DIV><I>Entity and attribute detail citation:</I></DIV>
              <DIV STYLE="margin-left:'0.4in'">
                <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
                <SCRIPT>fix(original)</SCRIPT>      
              </DIV>
            </xsl:for-each>
            <xsl:if test="context()[not(end())]">
              <BR/>
            </xsl:if>
          </DIV>
        </xsl:for-each>
      </LI>
    </OL>
    <A HREF="#Top">Back to Top</A>
    <HR/>

    <A name="who"><H3>Who produced the data set?</H3></A>
    <OL>
      <LI>
        <A name="who.1"><B>Who are the originators of the data set?</B> (may include formal authors, digital compilers, and editors)</A>
        <BR/><BR/>
        <xsl:if test="metadata/idinfo/citation/citeinfo/origin[. != '']">
          <UL>
            <xsl:for-each select="metadata/idinfo/citation/citeinfo/origin[. != '']">
              <LI><xsl:value-of/></LI>
            </xsl:for-each>
          </UL>
          <BR/>
        </xsl:if>
      </LI>

      <LI>
       <A name="who.2"><B>Who also contributed to the data set?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/idinfo/datacred[. != '']">
          <DIV STYLE="margin-left:'0.2in'">
            <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
            <SCRIPT>fix(original)</SCRIPT>      
          </DIV>
          <BR/>
        </xsl:for-each>
     </LI>

      <LI>
        <A name="who.3"><B>To whom should users address questions about the data?</B></A>
        <BR/><BR/>
        <xsl:apply-templates select="metadata/idinfo/ptcontac/cntinfo"/>
      </LI>
    </OL>
    <A HREF="#Top">Back to Top</A>
    <HR/>

    <A name="why"><H3>Why was the data set created?</H3></A>
    <xsl:for-each select="metadata/idinfo/descript/purpose[. != '']">
      <DIV STYLE="margin-left:'0.2in'">
        <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
        <SCRIPT>fix(original)</SCRIPT>      
      </DIV>
      <BR/>
    </xsl:for-each>
    <A HREF="#Top">Back to Top</A>
    <HR/>
 
    <A name="how"><H3>How was the data set created?</H3></A>
    <OL>
      <LI>
        <A name="how.1"><B>Where did the data come from?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/dataqual/lineage/srcinfo[(srccitea != '') or 
            (typesrc != '') or (srcscale != '') or (srccontr != '') or 
            (srccite/citeinfo/origin != '') or (srccite/citeinfo/pubdate != '') or 
            (srccite/citeinfo/title != '') or (srccite/citeinfo/serinfo/* != '') or 
            (srccite/citeinfo/pubinfo/* != '') or (srccite/citeinfo/onlink != '') or 
            (srccite/citeinfo/ othercit != '')]">
          <DIV STYLE="margin-left:'0.2in'">
            <DIV>
              <xsl:for-each select="srccitea[. != '']"><B><xsl:value-of/></B></xsl:for-each>
              (source <xsl:eval>formatIndex(childNumber(this), "1")</xsl:eval> of <xsl:eval>formatIndex(this.selectNodes("../srcinfo").length, "1")</xsl:eval>)
            </DIV>
            <xsl:if test="srccite/citeinfo[(origin != '') or (pubdate != '') or 
                (title != '') or (serinfo/* != '') or (pubinfo/* != '') or 
                (onlink != '') or (othercit != '')]">
              <BR/>
              <DIV STYLE="margin-left:'0.2in'">
                <xsl:apply-templates select="srccite/citeinfo"/>
              </DIV>
            </xsl:if>
            <xsl:for-each select="typesrc[. != '']">
              <DIV STYLE="margin-left:'0.4in'"><I>Type of source media:</I> <xsl:value-of/></DIV>
            </xsl:for-each>
            <xsl:for-each select="srcscale[. != '']">
              <DIV STYLE="margin-left:'0.4in'"><I>Source scale denominator:</I> <xsl:value-of/></DIV>
            </xsl:for-each>
            <xsl:for-each select="srccontr[. != '']">
              <DIV STYLE="margin-left:'0.4in'"><I>Source contribution:</I></DIV>
              <DIV STYLE="margin-left:'0.8in'">
                <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
                <SCRIPT>fix(original)</SCRIPT>      
              </DIV>
            </xsl:for-each>
          </DIV>
          <BR/>
        </xsl:for-each>
      </LI>

      <LI>
        <A name="how.2"><B>What changes have been made?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/dataqual/lineage/procstep[(procdate != '') or 
            (procdesc != '') or (srcused != '') or (srcprod != '') or 
            (proccont/cntinfo/cntperp/* != '') or (proccont/cntinfo/cntorgp/* != '') or 
            (proccont/cntinfo/cntaddr/address != '') or (proccont/cntinfo/cntaddr/city != '') or 
            (proccont/cntinfo/cntaddr/state != '') or (proccont/cntinfo/cntaddr/postal != '') or 
            (proccont/cntinfo/cntaddr/country != '') or (proccont/cntinfo/cntvoice != '') or 
            (proccont/cntinfo/cntfax != '') or (proccont/cntinfo/cntemail != '') or 
            (proccont/cntinfo/hours != '') or (proccont/cntinfo/cntinst != '')]">
          <DIV STYLE="margin-left:'0.2in'">
            <DIV>
              <xsl:for-each select="procdate[. != '']"><I>Date:</I> <xsl:value-of/></xsl:for-each>
              (change <xsl:eval>formatIndex(childNumber(this), "1")</xsl:eval> of <xsl:eval>formatIndex(this.selectNodes("../procstep").length, "1")</xsl:eval>)
            </DIV>
            <xsl:for-each select="procdesc[. != '']">
              <DIV STYLE="margin-left:'0.4in'">
                <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
                <SCRIPT>fix(original)</SCRIPT>      
              </DIV>
              <BR/>
            </xsl:for-each>
            <xsl:if test="proccont/cntinfo[(cntperp/* != '') or (cntorgp/* != '') or 
                (cntaddr/address != '') or (cntaddr/city != '') or (cntaddr/state != '') or 
                (cntaddr/postal != '') or (cntaddr/country != '') or (cntvoice != '') or 
                (cntfax != '') or (cntemail != '') or (hours != '') or (cntinst != '')]">
              <DIV STYLE="margin-left:'0.4in'">
                <I>Person responsible for change:</I><BR/>
                <DIV STYLE="margin-left:'0.2in'"><xsl:apply-templates select="proccont/cntinfo"/></DIV>
              </DIV>
              <BR/>
            </xsl:if>
            <xsl:if test="srcused[. != '']">
              <DIV STYLE="margin-left:'0.4in'">
                <I>Data sources used in this process:</I>
                <UL>
                  <xsl:for-each select="srcused[. != '']">
                    <LI TYPE="disc"><xsl:value-of/></LI>
                  </xsl:for-each>
                </UL>
              </DIV>
              <BR/>
            </xsl:if>
            <xsl:if test="srcprod[. != '']">
              <DIV STYLE="margin-left:'0.4in'">
                <I>Data sources produced in this process:</I>
                <UL>
                  <xsl:for-each select="srcprod[. != '']">
                    <LI TYPE="disc"><xsl:value-of/></LI>
                  </xsl:for-each>
                </UL>
              </DIV>
              <BR/>
            </xsl:if>
          </DIV>
        </xsl:for-each>
      </LI>
    </OL>
    <A HREF="#Top">Back to Top</A>
    <HR/>

    <A name="quality"><H3>How reliable are the data; what problems remain in the data set?</H3></A>
    <OL>
      <LI>
        <A name="quality.1"><B>How well have the observations been checked?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/dataqual/attracc/attraccr[. != '']">
          <DIV STYLE="margin-left:'0.2in'">
            <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
            <SCRIPT>fix(original)</SCRIPT>      
          </DIV>
          <BR/>
        </xsl:for-each>
      </LI>

      <LI>
        <A name="quality.2"><B>How accurate are the geographic locations?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/dataqual/posacc/horizpa/horizpar[. != '']">
          <DIV STYLE="margin-left:'0.2in'">
            <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
            <SCRIPT>fix(original)</SCRIPT>      
          </DIV>
          <BR/>
        </xsl:for-each>
      </LI>

      <LI>
        <A name="quality.3"><B>How accurate are the heights or depths?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/dataqual/posacc/vertacc/vertaccr[. != '']">
          <DIV STYLE="margin-left:'0.2in'">
            <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
            <SCRIPT>fix(original)</SCRIPT>      
          </DIV>
          <BR/>
        </xsl:for-each>
      </LI>

      <LI>
        <A name="quality.4"><B>Where are the gaps in the data? What is missing?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/dataqual/complete[. != '']">
          <DIV STYLE="margin-left:'0.2in'">
            <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
            <SCRIPT>fix(original)</SCRIPT>      
          </DIV>
          <BR/>
        </xsl:for-each>
      </LI>

      <LI>
        <A name="quality.5"><B>How consistent are the relationships among the observations, including topology?</B></A>
        <BR/><BR/>
        <xsl:for-each select="metadata/dataqual/logic[. != '']">
          <DIV STYLE="margin-left:'0.2in'">
            <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
            <SCRIPT>fix(original)</SCRIPT>      
          </DIV>
          <BR/>
        </xsl:for-each>
      </LI>
    </OL>
    <A HREF="#Top">Back to Top</A>
    <HR/>

    <A name="getacopy"><H3>How can someone get a copy of the data set?</H3></A>
    <DIV STYLE="margin-left:'0.2in'">
      <A name="getacopy.0"><B>Are there legal restrictions on access or use of the data?</B></A>
      <BR/><BR/>
      <xsl:for-each select="metadata/idinfo/accconst[. != '']">
        <DIV STYLE="margin-left:'0.2in'"><I>Access constraints:</I> <xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="metadata/idinfo/useconst[. != '']">
        <DIV STYLE="margin-left:'0.2in'"><I>Use constraints:</I></DIV>
        <DIV STYLE="margin-left:'0.6in'">
          <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
          <SCRIPT>fix(original)</SCRIPT>      
        </DIV>
      </xsl:for-each>
      <xsl:if test="metadata/idinfo[(accconst != '') or (useconst != '')]">
        <BR/>
      </xsl:if>
    </DIV>
    <xsl:for-each select="metadata/distinfo">
      <A><xsl:attribute name="name">Distrib<xsl:eval>formatIndex(childNumber(this), "1")</xsl:eval></xsl:attribute></A>
      <DIV STYLE="margin-left:'0.2in'">
        <B>Distributor <xsl:eval>formatIndex(childNumber(this), "1")</xsl:eval></B> of <xsl:eval>formatIndex(this.selectNodes("../distinfo").length, "1")</xsl:eval>
        <xsl:if expr="(childNumber(this) > 1)">
          &lt;<A><xsl:attribute name="HREF">#Distrib<xsl:eval>formatIndex((childNumber(this)-1), "1")</xsl:eval></xsl:attribute>Back</A>&gt;
        </xsl:if>
        <xsl:if test="context()[not(end())]">
          &lt;<A><xsl:attribute name="HREF">#Distrib<xsl:eval>formatIndex((childNumber(this)+1), "1")</xsl:eval></xsl:attribute>Next</A>&gt;
        </xsl:if>
      </DIV>
      <BR/>
      <OL>
        <DIV STYLE="margin-left:'0.2in'">
          <LI>
            <A name="getacopy.1"><B>Who distributes the data set?</B></A>
            <BR/><BR/>
            <xsl:apply-templates select="distrib/cntinfo"/>
            <xsl:if test="distrib/cntinfo[(cntperp/* != '') or (cntorgp/* != '') or 
                (cntaddr/address != '') or (cntaddr/city != '') or (cntaddr/state != '') or 
                (cntaddr/postal != '') or (cntaddr/country != '') or (cntvoice != '') or 
                (cntfax != '') or (cntemail != '') or (hours != '') or (cntinst != '')]">
              <BR/>
            </xsl:if>
          </LI>

          <LI>
            <A name="getacopy.2"><B>What's the catalog number I need to order this data set?</B></A>
            <BR/><BR/>
            <xsl:for-each select="resdesc[. != '']">
              <DIV STYLE="margin-left:'0.2in'"><xsl:value-of/></DIV>
              <BR/>
            </xsl:for-each>
          </LI>

          <LI>
            <A name="getacopy.3"><B>What legal disclaimers am I supposed to read?</B></A>
            <BR/><BR/>
            <xsl:for-each select="distliab[. != '']">
              <DIV STYLE="margin-left:'0.2in'">
                <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
                <SCRIPT>fix(original)</SCRIPT>      
              </DIV>
              <BR/>
            </xsl:for-each>
          </LI>

          <LI>
            <A name="getacopy.4"><B>How can I download or order the data?</B></A>
            <BR/><BR/>
            <xsl:if test="stdorder[(nondig != '') or (digform/digtinfo/formcont != '') or 
                      (digform/digtinfo/formname != '') or (digform/digtinfo/formvern != '') or 
                      (digform/digtinfo/formspec != '') or (digform/digtinfo/transize != '') or 
                      (digform/digtopt/onlinopt/computer/networka/networkr != '') or 
                      (digform/digtopt/offoptn//* != '') or (fees != '') or 
                      (ordering != '') or (turnarnd != '')]">
              <xsl:for-each select="stdorder">
                <UL>
                  <xsl:for-each select="nondig[. != '']">
                    <LI><B>Availability in non-digital form:</B>
                      <BR/><BR/>
                      <DIV STYLE="margin-left:'0.2in'"><xsl:value-of/></DIV>
                      <BR/>
                    </LI>
                  </xsl:for-each>
                  <xsl:if test="digform[(digtinfo/formcont != '') or 
                      (digtinfo/formname != '') or (digtinfo/formvern != '') or 
                      (digtinfo/formspec != '') or (digtinfo/transize != '') or 
                      (digtopt/onlinopt/computer/networka/networkr != '') or 
                      (digtopt/offoptn//* != '')]">
                    <LI><B>Availability in digital form:</B></LI>
                    <BR/><BR/>
                    <DIV STYLE="margin-left:'0.2in'">
                      <xsl:for-each select="digform[(digtinfo/formcont != '') or 
                          (digtinfo/formname != '') or (digtinfo/formvern != '') or 
                          (digtinfo/formspec != '') or (digtinfo/transize != '') or 
                          (digtopt/onlinopt/computer/networka/networkr != '') or 
                          (digtopt/offoptn//* != '')]">
                        <TABLE BORDER="0" CELLPADDING="2">
                          <TBODY>
                            <xsl:if test="digtinfo[(formcont != '') or (formname != '') or 
                                (formvern != '') or (formspec != '') or (transize != '')]">
                              <TR>
                                <TH ALIGN="left" VALIGN="top"><PRE STYLE="font-family:Times; font-size:12pt">Data format:</PRE></TH>
                                <TD VALIGN="top">
                                  <xsl:for-each select="digtinfo">
                                    <xsl:for-each select="formcont[. != '']"><SPAN><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE><SCRIPT>fix(original)</SCRIPT></SPAN> </xsl:for-each>
                                    <xsl:for-each select="formname[. != '']">in format <xsl:value-of/> </xsl:for-each>
                                    <xsl:for-each select="formvern[. != '']">(version <xsl:value-of/>) </xsl:for-each>
                                    <xsl:for-each select="formspec[. != '']"><SPAN><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE><SCRIPT>fix(original)</SCRIPT></SPAN> </xsl:for-each>
                                    <xsl:for-each select="transize[. != '']">Size: <xsl:value-of/></xsl:for-each>
                                    <xsl:if test="context()[not(end())]">
                                      <BR/><BR/>
                                    </xsl:if>
                                  </xsl:for-each>
                                </TD>
                              </TR>
                            </xsl:if>
                            <xsl:if test="digtopt/onlinopt/computer/networka/networkr[. != '']">
                              <TR>
                                <TH ALIGN="left" VALIGN="top"><PRE STYLE="font-family:Times; font-size:12pt">Network links:</PRE></TH>
                                <TD VALIGN="top">
                                  <xsl:for-each select="digtopt/onlinopt/computer/networka/networkr[. != '']">
                                    <A><xsl:attribute name="HREF"><xsl:value-of/></xsl:attribute><xsl:value-of/></A>
                                    <xsl:if test="context()[not(end())]">
                                      <BR/>
                                    </xsl:if>
                                  </xsl:for-each>
                                </TD>
                              </TR>
                            </xsl:if>
                            <xsl:if test="digtopt/offoptn[$any$ .//* != '']">
                              <TR>
                                <TH ALIGN="left" VALIGN="top"><PRE STYLE="font-family:Times; font-size:12pt">Media you can order:</PRE></TH>
                                <TD VALIGN="top">
                                  <xsl:for-each select="digtopt/offoptn[$any$ .//* != '']">
                                    <xsl:value-of select="offmedia"/> 
                                    <xsl:for-each select="reccap[$any$ * != '']">(Density <xsl:value-of select="recden"/> <xsl:value-of select="recdenu"/>) </xsl:for-each>
                                    <xsl:for-each select="recfmt[. != '']">(format <xsl:value-of/>)</xsl:for-each>
                                    <xsl:for-each select="compat[. != '']"><BR/><BR/>Note: <xsl:value-of/></xsl:for-each>
                                    <xsl:if test="context()[not(end())]">
                                      <BR/><BR/>
                                    </xsl:if>
                                  </xsl:for-each>
                                </TD>
                              </TR>
                            </xsl:if>
                          </TBODY>
                        </TABLE>
                        <xsl:if test="context()[not(end())]">
                          <BR/>
                        </xsl:if>
                      </xsl:for-each>
                    </DIV>
                    <BR/>
                  </xsl:if>
                  <xsl:for-each select="fees[. != '']">
                    <LI><B>Cost to order the data:</B> <xsl:value-of/></LI>
                    <BR/><BR/>
                  </xsl:for-each>
                  <xsl:for-each select="ordering[. != '']">
                    <LI><B>Special instructions:</B>
                      <BR/><BR/>
                      <DIV STYLE="margin-left:'0.2in'">
                        <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
                        <SCRIPT>fix(original)</SCRIPT>      
                      </DIV>
                      <BR/>
                    </LI>
                  </xsl:for-each>
                  <xsl:for-each select="turnarnd[. != '']">
                    <LI><B>How long will it take to get the data?</B>
                      <BR/><BR/>
                      <DIV STYLE="margin-left:'0.2in'"><xsl:value-of/></DIV>
                      <BR/>
                    </LI>
                  </xsl:for-each>
                </UL>
              </xsl:for-each>
            </xsl:if>
          </LI>

          <LI>
            <A name="getacopy.5"><B>Is there some other way to get the data?</B></A>
            <BR/><BR/>
            <xsl:for-each select="custom[. != '']">
              <DIV STYLE="margin-left:'0.2in'">
                <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
                <SCRIPT>fix(original)</SCRIPT>      
              </DIV>
              <BR/>
            </xsl:for-each>
          </LI>

          <LI>
            <A name="getacopy.6"><B>What hardware or software do I need in order to use the data set?</B></A>
            <BR/><BR/>
            <xsl:for-each select="techpreq[. != '']">
              <DIV STYLE="margin-left:'0.2in'"><xsl:value-of/></DIV>
              <BR/>
            </xsl:for-each>
          </LI> 
        </DIV>
      </OL>
      <xsl:if test="context()[not(end())]">
        <BR/>
      </xsl:if>
    </xsl:for-each>
    <xsl:if test="metadata[not(distinfo)]">
      <DIV STYLE="margin-left:'0.2in'"><B>Distributor 0</B> of 0</DIV>
      <BR/>
      <OL>
        <DIV STYLE="margin-left:'0.2in'">
          <LI><A name="getacopy.1"><B>Who distributes the data set?</B></A><BR/><BR/></LI>
          <LI><A name="getacopy.2"><B>What's the catalog number I need to order this data set?</B></A><BR/><BR/></LI>
          <LI><A name="getacopy.3"><B>What legal disclaimers am I supposed to read?</B></A><BR/><BR/></LI>
          <LI><A name="getacopy.4"><B>How can I download or order the data?</B></A><BR/><BR/></LI>
          <LI><A name="getacopy.5"><B>Is there some other way to get the data?</B></A><BR/><BR/></LI>
          <LI><A name="getacopy.6"><B>What hardware or software do I need in order to use the data set?</B></A><BR/><BR/></LI> 
        </DIV>
      </OL>
    </xsl:if>
    <A HREF="#Top">Back to Top</A>
    <HR/>

    <A name="metaref"><H3>Who wrote the metadata?</H3></A>
    <xsl:if test="metadata/metainfo[(metd != '') or (metrd != '') or (metfrd != '')]">
      <DIV STYLE="margin-left:'0.2in'">
        Dates:<BR/>
        <DIV STYLE="margin-left:'0.4in'">
          <xsl:for-each select="metadata/metainfo/metd[. != '']"><DIV><I>Last modified:</I> <xsl:value-of/></DIV></xsl:for-each>
          <xsl:for-each select="metadata/metainfo/metrd[. != '']"><DIV><I>Last reviewed:</I> <xsl:value-of/></DIV></xsl:for-each>
          <xsl:for-each select="metadata/metainfo/metfrd[. != '']"><DIV><I>To be reviewed:</I> <xsl:value-of/></DIV></xsl:for-each>
        </DIV>
      </DIV>
      <BR/>
    </xsl:if>
    <xsl:if test="metadata/metainfo/metc/cntinfo[(cntperp/* != '') or (cntorgp/* != '') or 
                (cntaddr/address != '') or (cntaddr/city != '') or (cntaddr/state != '') or 
                (cntaddr/postal != '') or (cntaddr/country != '') or (cntvoice != '') or 
                (cntfax != '') or (cntemail != '') or (hours != '') or (cntinst != '')]">
      <DIV STYLE="margin-left:'0.2in'">
        <I>Metadata author:</I><BR/>
        <DIV STYLE="margin-left:'0.2in'"><xsl:apply-templates select="metadata/metainfo/metc/cntinfo"/></DIV>
      </DIV>
      <BR/>
    </xsl:if>
    <xsl:if test="metadata/metainfo[(metstdn != '') or (metstdv != '')]">
      <DIV STYLE="margin-left:'0.2in'">
        <I>Metadata standard:</I><BR/>
        <DIV STYLE="margin-left:'0.4in'">
          <xsl:value-of select="metadata/metainfo/metstdn[. != '']"/>
          <xsl:for-each select="metadata/metainfo/metstdv[. != '']">(<xsl:value-of/>)</xsl:for-each>
        </DIV>
      </DIV>
      <BR/>
    </xsl:if>
    <xsl:if test="metadata/metainfo/metextns/onlink[. != '']">
      <DIV STYLE="margin-left:'0.2in'">
        <I>Metadata extensions used:</I><BR/>
        <xsl:for-each select="metadata/metainfo/metextns/onlink[. != '']">
          <LI STYLE="margin-left:'0.3in'"><A TARGET="viewer"><xsl:attribute name="HREF"><xsl:value-of/></xsl:attribute><xsl:value-of/></A></LI>
        </xsl:for-each>
      </DIV>
      <BR/>
    </xsl:if>
    <A HREF="#Top">Back to Top</A>
    <BR/><BR/>

  <!-- <BR/><BR/><BR/><CENTER><FONT COLOR="#6495ED">Metadata stylesheets are provided courtesy of ESRI.  Copyright (c) 2000-2004, Environmental Systems Research Institute, Inc.  All rights reserved.</FONT></CENTER> -->

  </BODY>
</HTML>
</xsl:template>


<!-- Contact Information -->
<xsl:template match="cntinfo">
  <xsl:if test="context()[(cntperp/* != '') or (cntorgp/* != '') or (cntaddr/address != '') or 
      (cntaddr/city != '') or (cntaddr/state != '') or (cntaddr/postal != '') or 
      (cntaddr/country != '') or (cntvoice != '') or (cntfax != '') or (cntemail != '') or 
      (hours != '') or (cntinst != '')]"> 
    <DIV STYLE="margin-left:'0.2in'">
      <xsl:for-each select="*/cntper[. != '']"><DIV><xsl:value-of/></DIV></xsl:for-each>
      <xsl:for-each select="*/cntorg[. != '']"><DIV><xsl:value-of/></DIV></xsl:for-each>
      <xsl:for-each select="cntpos[. != '']"><DIV><xsl:value-of/></DIV></xsl:for-each>
      <xsl:for-each select="cntaddr">
        <xsl:for-each select="address[. != '']">
          <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
          <SCRIPT>fix(original)</SCRIPT>      
        </xsl:for-each>
        <xsl:if test="context()[((city != '') or (state != '') or (postal != ''))]">
          <DIV>
            <xsl:for-each select="city[. != '']">
                <xsl:value-of/></xsl:for-each><xsl:if test="context()[(city != '') and (state != '')]">, </xsl:if><xsl:for-each select="state[. != '']">
                <xsl:value-of/></xsl:for-each><xsl:if test="context()[((city != '') or (state != '')) and (postal != '')]" xml:space="preserve"> </xsl:if>
                <xsl:for-each select="postal[. != '']"><xsl:value-of/></xsl:for-each>
          </DIV>
        </xsl:if>
        <xsl:for-each select="country[. != '']"><DIV><xsl:value-of/></DIV></xsl:for-each>
        <xsl:if test="context()[not(end())]">
          <BR/>
        </xsl:if>
      </xsl:for-each>
      <xsl:if test="context()[(($any$ cntaddr/address != '') or (cntaddr/city != '') or 
          (cntaddr/state != '') or (cntaddr/postal != '') or (cntaddr/country != '')) 
          and ((cntvoice != '') or (cntfax != '') or (cntemail != '') or 
          (hours != '') or (cntinst != ''))]">
        <BR/>
      </xsl:if>
      <xsl:for-each select="cntvoice[. != '']"><DIV><xsl:value-of/> (voice)</DIV></xsl:for-each>
      <xsl:for-each select="cntfax[. != '']"><DIV><xsl:value-of/> (FAX)</DIV></xsl:for-each>
      <xsl:for-each select="cntemail[. != '']"><DIV><xsl:value-of/></DIV></xsl:for-each>
      <xsl:for-each select="hours[. != '']"><DIV><I>Hours of Service:</I> <xsl:value-of/></DIV></xsl:for-each>
      <xsl:for-each select="cntinst[. != '']">
        <DIV><I>Contact Instructions:</I></DIV>
        <DIV STYLE="margin-left:'0.4in'">
          <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
          <SCRIPT>fix(original)</SCRIPT>
        </DIV>
      </xsl:for-each>
    </DIV>
  </xsl:if>
</xsl:template>


<!-- Citation Information -->
<xsl:template match="citeinfo">
  <xsl:if test="context()[(origin != '') or (pubdate != '') or (title != '') or 
      (serinfo/* != '') or (pubinfo/*)]"> 
    <DIV STYLE="margin-left:'0.2in'; margin-right:'0.4in'">
      <xsl:for-each select="origin[. != '']"><xsl:value-of/>, </xsl:for-each>
      <xsl:for-each select="pubdate[. != '']"><xsl:value-of/>, </xsl:for-each>
      <xsl:for-each select="title[. != '']"><xsl:value-of/></xsl:for-each>
      <xsl:if test="context()[((origin != '') or (pubdate != '') or (title != '')) and 
          ((serinfo/* != '') or (pubinfo/* != ''))]">: </xsl:if>
      <xsl:for-each select="serinfo/sername[. != '']"><xsl:value-of/> </xsl:for-each>
      <xsl:for-each select="serinfo/issue[. != '']"> <xsl:value-of/></xsl:for-each>
      <xsl:if test="context()[($any$ serinfo/* != '') and ($any$ pubinfo/* != '')]">, </xsl:if>
      <xsl:for-each select="pubinfo/publish[. != '']"><xsl:value-of/>, </xsl:for-each>
      <xsl:for-each select="pubinfo/pubplace[. != '']"><xsl:value-of/></xsl:for-each>.
    </DIV>
    <BR/>
  </xsl:if>
  <xsl:if test="onlink[. != '']"> 
    <DIV STYLE="margin-left:'0.2in'">
      <I>Online links:</I>
      <UL>
        <xsl:for-each select="onlink[. != '']">
          <LI TYPE="disc"><A TARGET="viewer"><xsl:attribute name="HREF"><xsl:value-of/></xsl:attribute><xsl:value-of/></A></LI>
        </xsl:for-each>
      </UL>
      <BR/>
    </DIV>
  </xsl:if>
  <xsl:for-each select="othercit[. != '']">
    <DIV STYLE="margin-left:'0.2in'">
      <I>Other citation details:</I>
      <DIV STYLE="margin-left:'0.4in'"><xsl:value-of/></DIV>
      <BR/>
    </DIV>
  </xsl:for-each>
  <xsl:for-each select="lworkcit[.//* != '']">
    <DIV STYLE="margin-left:'0.2in'">
      <DIV>This is part of the following larger work:</DIV>
      <BR/>
      <DIV STYLE="margin-left:'0.4in'"><xsl:apply-templates select="citeinfo"/></DIV>
    </DIV>
  </xsl:for-each>
</xsl:template>


<!-- Time Period Information -->
<xsl:template match="timeinfo">
  <xsl:for-each select=".//caldate[. != '']">
    <DIV STYLE="margin-left:'0.2in'"><I>Calendar date:</I> <xsl:value-of/></DIV>
  </xsl:for-each>
  <xsl:for-each select="rngdates">
    <DIV STYLE="margin-left:'0.2in'">
      <xsl:for-each select="begdate[. != '']"><DIV><I>Beginning date:</I> <xsl:value-of/></DIV></xsl:for-each>
      <xsl:for-each select="begtime[. != '']"><DIV><I>Beginning time:</I> <xsl:value-of/></DIV></xsl:for-each>
      <xsl:for-each select="enddate[. != '']"><DIV><I>Ending date:</I> <xsl:value-of/></DIV></xsl:for-each>
      <xsl:for-each select="endtime[. != '']"><DIV><I>Ending time:</I> <xsl:value-of/></DIV></xsl:for-each>
    </DIV>
  </xsl:for-each>
</xsl:template>


<!-- Grid Projection Systems -->
<xsl:template match="utm">
  <DIV>
    <xsl:for-each select="utmzone">
      <DIV><I>UTM zone number:</I>  <xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="transmer">
      <DIV>Transverse Mercator projection parameters:</DIV>
    </xsl:for-each>
    <xsl:apply-templates select="transmer"/>
  </DIV>
</xsl:template>

<xsl:template match="ups">
  <DIV>
    <xsl:for-each select="upszone">
      <DIV><I>UPS zone identifier:</I> <xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="polarst">
      <DIV>Polar stereographic projection parameters:</DIV>
    </xsl:for-each>
    <xsl:apply-templates select="polarst"/>
  </DIV>
</xsl:template>

<xsl:template match="spcs">
  <DIV>
    <xsl:for-each select="spcszone">
      <DIV><I>SPCS zone identifier:</I> <xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="lambertc">
      <DIV>Lambert conformal conic projection parameters:</DIV>
    </xsl:for-each>
    <xsl:apply-templates select="lambertc"/>
    <xsl:for-each select="transmer">
      <DIV>Transverse mercator projection parameters:</DIV>
    </xsl:for-each>
    <xsl:apply-templates select="transmer"/>
    <xsl:for-each select="obqmerc">
      <DIV>Oblique mercator projection parameters:</DIV>
    </xsl:for-each>
    <xsl:apply-templates select="obqmerc"/>
    <xsl:for-each select="polycon">
      <DIV>Polyconic projection parameters:</DIV>
    </xsl:for-each>
    <xsl:apply-templates select="polycon"/>
  </DIV>
</xsl:template>

<xsl:template match="arcsys">
  <DIV>
    <xsl:for-each select="arczone">
      <DIV><I>ARC system zone identifier:</I> <xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="equirect">
      <DIV>Equirectangular projection parameters:</DIV>
    </xsl:for-each>
    <xsl:apply-templates select="equirect"/>
    <xsl:for-each select="azimequi">
      <DIV>Azimuthal equidistant projection parameters:</DIV>
    </xsl:for-each>
    <xsl:apply-templates select="azimequi"/>
  </DIV>
</xsl:template>

<xsl:template match="othergrd">
  <DIV><I>Other grid system's definition:</I></DIV>
  <DIV STYLE="margin-left:'0.4in'"><xsl:value-of/></DIV>
</xsl:template>


<!-- Map Projections -->
<xsl:template match="albers | equicon | lambertc">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:apply-templates select="stdparll"/>
    <xsl:apply-templates select="longcm"/>
    <xsl:apply-templates select="latprjo"/>
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>

<xsl:template match="gnomonic | lamberta | orthogr | stereo | gvnsp">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:for-each select="../gvnsp">
      <xsl:apply-templates select="heightpt"/>
    </xsl:for-each>
    <xsl:apply-templates select="longpc"/>
    <xsl:apply-templates select="latprjc"/>
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>

<xsl:template match="azimequi | polycon | transmer">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:for-each select="../transmer">
      <xsl:apply-templates select="sfctrmer"/>
    </xsl:for-each>
    <xsl:apply-templates select="longcm"/>
    <xsl:apply-templates select="latprjo"/>
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>

<xsl:template match="miller | sinusoid | vdgrin">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:apply-templates select="longcm"/>
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>

<xsl:template match="equirect">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:apply-templates select="stdparll"/>
    <xsl:apply-templates select="longcm"/>
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>

<xsl:template match="mercator">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:apply-templates select="stdparll"/>
    <xsl:apply-templates select="sfequat"/>
    <xsl:apply-templates select="longcm"/>
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>

<xsl:template match="polarst">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:apply-templates select="svlong"/>
    <xsl:apply-templates select="stdparll"/>
    <xsl:apply-templates select="sfprjorg"/>
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>

<xsl:template match="obqmerc">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:apply-templates select="sfctrlin"/>
    <xsl:apply-templates select="obqlazim"/>
    <xsl:apply-templates select="obqlpt"/>
    <xsl:apply-templates select="latprjo"/>
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>

<xsl:template match="modsak">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>

<xsl:template match="robinson">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:apply-templates select="longpc"/>
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>

<xsl:template match="spaceobq">
  <DIV STYLE="margin-left:'0.4in'">
    <xsl:apply-templates select="landsat"/>
    <xsl:apply-templates select="pathnum"/>
    <xsl:apply-templates select="feast"/>
    <xsl:apply-templates select="fnorth"/>
  </DIV>
</xsl:template>


<!-- Projection Parameters -->
<xsl:template match="stdparll">
  <DIV><I>Standard parallel:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="longcm">
  <DIV><I>Longitude of central meridian:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="latprjo">
  <DIV><I>Latitude of projection origin:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="feast">
  <DIV><I>False easting:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="fnorth">
  <DIV><I>False northing:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="sfequat">
  <DIV><I>Scale factor at equator:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="heightpt">
  <DIV><I>Height of perspective point above surface:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="longpc">
  <DIV><I>Longitude of projection center:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="latprjc">
  <DIV><I>Latitude of projection center:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="sfctrlin">
  <DIV><I>Scale factor at center line:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="obqlazim">
  <DIV><I>Oblique line azimuth:</I> <xsl:value-of/></DIV>
  <xsl:for-each select="azimangl">
    <DIV STYLE="margin-left:'0.2in'"><I>Azimuthal angle:</I> <xsl:value-of/></DIV>
  </xsl:for-each>
  <xsl:for-each select="azimptl">
    <DIV STYLE="margin-left:'0.2in'"><I>Azimuthal measure point longitude:</I> <xsl:value-of/></DIV>
  </xsl:for-each>
</xsl:template>

<xsl:template match="obqlpt">
  <DIV><I>Oblique line point:</I> <xsl:value-of/></DIV>
  <xsl:for-each select="obqllat">
    <DIV STYLE="margin-left:'0.2in'"><I>Oblique line latitude:</I> <xsl:value-of/></DIV>
  </xsl:for-each>
  <xsl:for-each select="obqllong">
    <DIV STYLE="margin-left:'0.2in'"><I>Oblique line longitude:</I> <xsl:value-of/></DIV>
  </xsl:for-each>
</xsl:template>

<xsl:template match="svlong">
  <DIV><I>Straight vertical longitude from pole:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="sfprjorg">
  <DIV><I>Scale factor at projection origin:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="landsat">
  <DIV><I>Landsat number:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="pathnum">
  <DIV><I>Path number:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="sfctrmer">
  <DIV><I>Scale factor at central meridian:</I> <xsl:value-of/></DIV>
</xsl:template>

<xsl:template match="otherprj">
  <DIV><I>Other projection's definition:</I></DIV>
  <DIV STYLE="margin-left:'0.4in'"><xsl:value-of/></DIV>
</xsl:template>


</xsl:stylesheet>
