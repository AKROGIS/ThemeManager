<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/TR/WD-xsl" TYPE="text/javascript">

<!-- An XSL template for displaying metadata in ArcCatalog
	using a tabbed presentation style.

     Copyright (c) 1999-2008, Environmental Systems Research Institute, Inc. All rights reserved.
     	
     Revision History: Created 5/14/99 avienneau
                Updated 3/31/00 avienneau
		        Updated 9/14/00 mlyszkiewicz
                Updated 11/2/00 avienneau
				Updated 12/12/00 mlyszkiewicz
                Updated 1/18/04 avienneau - added geoprocessing history
				Updated 8/30/06 avienneau - added locator and terrain information
-->

<xsl:template match="/">
  <HTML>
    <HEAD>
      <STYLE>
        <!-- body -->
        BODY {background-color:#FBFFFF; margin:0.25in; 
              font-size:10pt; font-family:Arial,sans-serif}
        <!-- title -->
        H1   {margin-left:0.05in; position:relative; top:-6; text-align:center;
              font-weight:bold; font-size:18; font-family:Verdana,sans-serif; color:#006400}
        <!-- data type -->
        H2   {margin-left:0.25in; position:relative; top:-16; text-align:center;
              font-size:13; font-family:Verdana,sans-serif; color:#006400}

        <!-- "tabs" table -->
        <!-- table style -->
        TABLE  {position:relative; top:-10; valign:top; table-layout:fixed; 
                border-collapse:collapse}
        <!-- table row style -->
        TD   {text-align:center}
        <!-- table cell style -->
        TD   {font-weight:bold; font-size:11pt; border-color:#6495ED}
        <!-- selected tab -->
        .tsel  {color:#FFFFFF; background-color:#6495ED}
        <!-- unselected tab -->
        .tun   {color:#00008B; background-color:#B8DEFA}
        <!-- unselected tab hilite -->
        .tover {color:#0000CD; background-color:#B8E6FF; cursor:hand}
        <!-- properties box -->
        .f   {background-color:#FFFFFF; border:'1.5pt solid #6495ED'; 
              position:relative; top:-10}

        <!-- headings -->
        <!-- group heading -->
        .ph1  {color:#2E8B57; font-weight:bold; cursor:}
        <!-- group heading indented -->
        .ph2  {margin-left:0.2in; color:#2E8B57; font-weight:bold; cursor:}
        <!-- group heading hilite -->
        .pover1 {color:#006000; font-weight:bold; cursor:hand}
        <!-- group heading hilite indented -->
        .pover2 {margin-left:0.2in; color:#006000; font-weight:bold; cursor:hand}

        <!-- values -->
        <!-- property name -->
        .pn  {color:#00008B; font-weight:bold}
        <!-- property value -->
        .pv  {font-family:Verdana,sans-serif; line-height:135%;
              color:#191970; margin:0in 0.15in 0.75in 0.15in}
        <!-- expanded properties -->
        .pe1  {margin-left:0.2in}
        <!-- expanded properties entity indent -->
        .pe2  {margin-left:0.25in; font-weight:normal; color:#191970;}
        <!-- expanded long text -->
        .lt  {line-height:115%}
        <!-- lists of comma-separated elements -->
        .lt2  {line-height:115%; margin-bottom:1mm}
        <!-- indented spatial reference parameters -->
        .sr1  {margin-left:0in}
        .sr2  {margin-left:0.2in}
        .sr3  {margin-left:0.4in}
        .srh1  {color:#00008B; font-weight:bold; margin-left:0in}
        .srh2  {color:#00008B; font-weight:bold; margin-left:0.2in}

        <!-- search results -->
        .name   {margin-left:0.05in; position:relative; top:-6; text-align:center;
                 font-weight:bold; font-size:18; font-family:Verdana,sans-serif; color:#006400}
        .sub   {margin-left:0.25in; text-align:center; position:relative; top:3; 
                font-weight:bold; font-size:13; font-family:Verdana,sans-serif; color:#006400}
        .search   {margin:0in 0.15in 0.75in 0.15in; 
                   color:#191970; font-family:Verdana,sans-serif; font-size:13}
        .head   {color:#006400}
      </STYLE>

      <SCRIPT LANGUAGE="JScript"><xsl:comment><![CDATA[

      //changes the color of the tabs or headings that you can click
      //when the mouse hovers over them
      function doHilite()  {
        var e = window.event.srcElement;
        if (e.className == "tun") {
          e.className = "tover";
        }
        else if (e.className == "tover") {
            e.className = "tun";
        }
        else if (e.className == "ph1") {
            e.className = "pover1";
        }
        else if (e.className == "ph2") {
            e.className = "pover2";
        }
        else if (e.className == "pover1") {
            e.className = "ph1";
        }
        else if (e.className == "pover2") {
            e.className = "ph2";
        }

        window.event.cancelBubble = true;
      }

      //changes the style of the selected tab to unselected and hide its text, then 
      //set the style of the tab that was clicked on to selected and show its text
      function changeTab(eRow)  {
        var tabs = eRow.cells;
        for (var i = 0; i < tabs.length; i++) {
          var oldTab = tabs[i];
          if (oldTab.className == "tsel") {
            break;
          }
        }
        oldTab.className = "tun";
        var oldContent = getAssociated(oldTab);
        oldContent.style.display = "none";

        var newTab = window.event.srcElement;
        newTab.className ="tsel";
        var newContent = getAssociated(newTab);
        newContent.style.display = "block";

        window.event.cancelBubble = true;
      }

      //hide or show the text assoicated with the heading that was clicked
      function hideShowGroup(e)  {
        var theGroup = e.children[0];
        if (theGroup.style.display == "none") {
          theGroup.style.display="block";
        }
        else { 
          theGroup.style.display="none";
        }

        window.event.cancelBubble = true;
      }

      //returns the name of the element containing the text associated with each tab
      function getAssociated(e) {
        if (e.tagName == "TD") {
          switch (e.id) {
            case "DescTab":
              return (Description);
            case "SpatialTab": 
              return (Spatial);
            case "AttribTab": 
              return (Attributes);
          }
        }
      }

      //centers the thumbnail
      function position() {
        var e;
        e = document.all("thumbnail");
        if (e != null) {
          b = document.body;
          w1 = b.clientWidth - 80;
          w2 = w1 - thumbnail.width;
          var margin = Math.floor(w2 * .5);
          thumbnail.style.visibility = "hidden";
          thumbnail.style.marginLeft = margin;
          thumbnail.style.visibility = "visible";
        }
      }

      //parse text to respect line breaks added - increases readability.
      //lines beginning with a ">" character are presented with a monospace
      //(fixed-width) font - e.g., so equations will appear correctly
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
        }
        else {
          n = document.createElement("DIV");
          par.appendChild(n);
          n.innerText = e.innerText;
          e.removeNode(true);
        }
      }

      ]]></xsl:comment></SCRIPT>
    </HEAD>
	
    <BODY onload="position();" onresize="position();" oncontextmenu="return true">

    <xsl:choose> 
      <xsl:when test="context()[not(metadata)]"> 

        <xsl:choose> 
          <!-- Show parameters defining the search when the 
               root element of the XML file is SearchResults -->
          <xsl:when test="context()[SearchResults]"> 
            <xsl:apply-templates select="SearchResults" />
          </xsl:when> 

          <!-- If root element isn't metadata or SearchResults, 
               we don't know what data this file contains -->
          <xsl:otherwise> 
            <DIV STYLE="text-align:center; color:#6495ED; margin-left:1in; margin-right:1in">
              <BR/><BR/>
              This document does not contain information that can be displayed 
              with the ESRI stylesheet.
            </DIV>
          </xsl:otherwise> 
        </xsl:choose> 
      </xsl:when> 

      <xsl:otherwise> 

        <!-- Add title and subtitle to the page -->
        <xsl:for-each select="/metadata/idinfo/citation/citeinfo/title[. != '']">
          <H1><xsl:value-of /></H1>
        </xsl:for-each>
        <xsl:for-each select="/metadata/idinfo/natvform[. != '']">
          <H2><xsl:value-of />
		<xsl:if test="/metadata/spdoinfo/rastinfo/rastifor[. != '']">
			- <xsl:value-of select="/metadata/spdoinfo/rastinfo/rastifor" />
		</xsl:if>
		</H2>
        </xsl:for-each>

        <!-- Set up the tabs -->
        <TABLE cols="3" frame="void" rules="cols" width="315" height="28">
          <COL WIDTH="105"/><COL WIDTH="105"/><COL WIDTH="105"/>
          <TR height="28" onmouseover="doHilite()" onmouseout="doHilite()" onclick="changeTab(this)">
            <TD ID="DescTab" CLASS="tsel" TITLE="Click to see a description of the data">Description</TD>
            <TD ID="SpatialTab" CLASS="tun" TITLE="Click for details about the spatial data">Spatial</TD>
            <TD ID="AttribTab" CLASS="tun" TITLE="Click for details about the attribute data">Attributes</TD>
          </TR>
        </TABLE>

        <!-- Define the box which will contain the contents of the current tab -->
        <DIV ID="Group" CLASS="f">

          <!-- Description Tab -->
          <DIV ID="Description" CLASS="pv" STYLE="display:block"><BR/>

            <xsl:choose>
              <xsl:when test="/metadata[($any$ (idinfo/(citation/citeinfo/
                  (origin | pubdate | pubtime | ftname | geoform | onlink | pubinfo/* | 
                  serinfo/*) | timeperd//* | descript/(abstract | purpose | supplinf) | 
                  native | accconst | useconst | keywords/*/(themekey | placekey | 
                  stratkey | tempkey) | browse/(browsen | img/@src) | status/*) | 

                  distinfo/stdorder/digform/(digtinfo/(formname | dssize | transize | 
                  filedec) | digtopt/(onlinopt/(computer/(networka/* | sdeconn/* | 
                  dialinst/(dialtel | dialfile)) | accinstr) | offoptn/offmedia)) | 

                  metainfo/(metd | metrd | metfrd | metstdn | metstdv | mettc | metextns/* | 
                  metc/cntinfo/(cntvoice | cntfax | cntemail | hours | cntinst | */cntper | 
                  */cntorg | cntaddr/(address | city | state | postal | country))) | 

                  Binary/Enclosure/img/@src | Esri/ModDate) != '') or 
                  (Binary/Enclosure/Data/@EsriPropertyType = 'Base64')]">


                <!-- Show contents of Description tab -->
                <xsl:apply-templates select="/metadata//img[(@src != '')]" />

                <xsl:apply-templates select="/metadata/idinfo/keywords
                    [$any$ */(themekey | placekey | stratkey | tempkey) != '']" />

                <xsl:if test="/metadata[($any$ idinfo/(browse/img/@src | 
                    keywords/*/(themekey | placekey | stratkey | tempkey)) != '') 
                    and (($any$ (idinfo/(descript/(abstract | purpose | supplinf) | 
                    browse/browsen) | Binary/Enclosure/img/@src) != '') 
                    or (Binary/Enclosure/Data/@EsriPropertyType = 'Base64'))]">
                  <BR/>
                </xsl:if>

                <xsl:if test="/metadata[($any$ idinfo/(descript/(abstract | purpose | 
                    supplinf) | browse/browsen) != '') or 
                    ($any$ Binary/Enclosure/Data/@EsriPropertyType = 'Base64') or 
                    ($any$ Binary/Enclosure/img/@src != '')]">
                  <DIV CLASS="pn">Description</DIV>
                  <xsl:apply-templates select="/metadata/idinfo/descript[
                      (abstract != '') or (purpose != '') or (supplinf != '')]" />
                  <xsl:apply-templates select="/metadata/Binary
                      [(Enclosure/Data/@EsriPropertyType = 'Base64') or 
                      (Enclosure/img/@src != '')]" />
                  <xsl:if test="/metadata[$any$ idinfo/browse/browsen != '']">
                    <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Links to graphics describing the data
                      <DIV CLASS="pe2" STYLE="display:; position:relative; top:-15; margin-left:-0.05in">
                        <UL><xsl:apply-templates select="/metadata/idinfo/browse[browsen != '']" /></UL>
                      </DIV>
                    </DIV>
                  </xsl:if>
                </xsl:if>

                <xsl:if test="/metadata[($any$ idinfo/(status/* | citation/citeinfo/
                    (origin | pubdate | pubtime | pubinfo/* | serinfo/*) | timeperd//*) != '') 
                    and (($any$ (idinfo/(descript/(abstract | purpose | supplinf) | keywords/*/(themekey | placekey | 
                    stratkey | tempkey) | browse/(img/@src | browsen)) | 
                    Binary/Enclosure/img/@src) != '') or 
                    (Binary/Enclosure/Data/@EsriPropertyType = 'Base64'))]">
                  <DIV STYLE="text-align:center; color:#6495ED">_________________</DIV><BR/>
                </xsl:if>

                <xsl:apply-templates select="/metadata/idinfo/status[$any$ * != '']" />

                <xsl:if test="/metadata[($any$ idinfo/status/* != '') 
                    and ($any$ idinfo/timeperd//* != '')]">
                  <BR/>
                </xsl:if>

                <xsl:apply-templates select="/metadata/idinfo/timeperd[$any$ .//* != '']" />

                <xsl:if test="/metadata[($any$ idinfo/(status/* | timeperd//*) != '') and 
                    ($any$ idinfo/citation/citeinfo/(origin | pubdate | pubtime | 
                    pubinfo/* | serinfo/*) != '')]">
                  <BR/>
                </xsl:if>

                <xsl:apply-templates select="/metadata/idinfo/citation/citeinfo
                    [(origin != '') or (pubdate != '') or (pubtime != '') or 
                    ($any$ pubinfo/* != '') or ($any$ serinfo/* != '')]" />

                <xsl:if test="/metadata[($any$ (idinfo/(citation/citeinfo/(ftname | 
                    geoform | onlink) | native | accconst | useconst) | distinfo/stdorder/
                    digform/(digtinfo/(formname | dssize | transize | filedec) | 
                    digtopt/(onlinopt/(computer/(networka/* | sdeconn/* | 
                    dialinst/(dialtel | dialfile)) | accinstr) | offoptn/offmedia)) | 
                    metainfo/(metd | metrd | metfrd | metstdn | metstdv | mettc | metextns/* |
                    metc/cntinfo/(cntvoice | cntfax | cntemail | hours | cntinst | */cntper | 
                    */cntorg | cntaddr/(address | city | state | postal | country))) | 
                    Esri/ModDate) != '') and 
                    (($any$ (idinfo/(descript/(abstract | purpose | supplinf) | 
                    keywords/*/(themekey | placekey | stratkey | tempkey) | 
                    browse/(img/@src | browsen) | status/* | timeperd//* | 
                    citation/citeinfo/(origin | pubdate | pubtime | pubinfo/* | 
                    serinfo/*)) | Binary/Enclosure/img/@src) != '') or 
                    (Binary/Enclosure/Data/@EsriPropertyType = 'Base64'))]">
                  <DIV STYLE="text-align:center; color:#6495ED">_________________</DIV><BR/>
                </xsl:if>

                <xsl:if test="/metadata[$any$ (idinfo/(citation/citeinfo/(ftname | 
                    geoform | onlink) | native | accconst | useconst) | distinfo/stdorder/
                    digform/(digtinfo/(formname | dssize | transize | filedec) | 
                    digtopt/(onlinopt/(computer/(networka/* | sdeconn/* | 
                    dialinst/(dialtel | dialfile)) | accinstr) | offoptn/offmedia))) != '']">
                  <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Data storage and access information
                    <DIV CLASS="pe2" STYLE="display:none">

                      <xsl:for-each select="/metadata/idinfo/citation/citeinfo/ftname[. != '']">
                        <I>File name: </I><xsl:value-of /><BR/>
                      </xsl:for-each>

                      <xsl:for-each select="/metadata/idinfo/citation/citeinfo/geoform[. != '']">
                        <I>Type of data: </I><xsl:value-of /><BR/>
                      </xsl:for-each>    

                      <xsl:for-each select="/metadata/idinfo/citation/citeinfo/onlink[. != '']">
                        <xsl:if test="context()[0]"><DIV><I>Location of the data: </I></DIV></xsl:if>
                        <DIV><LI STYLE="margin-left:0.2in"><xsl:value-of/></LI></DIV>    
                      </xsl:for-each>    

                      <xsl:for-each select="/metadata/idinfo/native[. != '']">
                        <I>Data processing environment: </I><SPAN CLASS="lt"><xsl:value-of /></SPAN><BR/>
                      </xsl:for-each>

                      <xsl:apply-templates select="/metadata/distinfo/stdorder/digform
                          [($any$ digtinfo/(formname | dssize | transize | filedec) != '') 
                          or ($any$ digtopt/onlinopt/accinstr != '') or ($any$ 
                          digtopt/onlinopt/computer/(networka/* | sdeconn/*) != '') or 
                          ($any$ digtopt/onlinopt/computer/dialinst/(dialtel | dialfile) != '') 
                          or ($any$ digtopt/offoptn/offmedia != '')]" />

                      <xsl:if test="/metadata[$any$ idinfo/(accconst | useconst) != '']">
                        <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Constraints on accessing and using the data
                          <DIV CLASS="pe2" STYLE="display:none">
                            <xsl:apply-templates select="/metadata/idinfo/accconst[. != '']" />
                            <xsl:apply-templates select="/metadata/idinfo/useconst[. != '']" />
                          </DIV>
                        </DIV>
                      </xsl:if>

                    </DIV>
                  </DIV>
                </xsl:if>

                <xsl:if test="/metadata[($any$ (idinfo/(citation/citeinfo/(ftname | 
                    geoform | onlink) | native | accconst | useconst) | distinfo/stdorder/
                    digform/(digtinfo/(formname | dssize | transize | filedec) | 
                    digtopt/(onlinopt/(computer/(networka/* | sdeconn/* | 
                    dialinst/(dialtel | dialfile)) | accinstr) | offoptn/offmedia))) != '') 
                    and ($any$ (Esri/ModDate | metainfo/(metd | metrd | metfrd | metstdn | 
                    metstdv | mettc | metextns/* | metc/cntinfo/(cntvoice | cntfax | 
                    cntemail | hours | cntinst | */cntper | */cntorg | 
                    cntaddr/(address | city | state | postal | country)))) != '')]">
                  <BR/>
                </xsl:if>

                <xsl:if test="/metadata[(Esri/ModDate != '') or 
                    ($any$ metainfo/(metd | metrd | metfrd | metstdn | metstdv | mettc | 
                    metextns/* | metc/cntinfo/(cntvoice | cntfax | cntemail | 
                    hours | cntinst | */cntper | */cntorg | cntaddr/(address | city | 
                    state | postal | country))) != '')]">
                  <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Details about this document
                    <DIV CLASS="pe2" STYLE="display:none">

                      <xsl:choose>
                        <xsl:when test="/metadata[Esri/ModDate != '']">
                          Contents last updated: <xsl:value-of select="/metadata/Esri/ModDate"/>
                          <xsl:if test="/metadata[Esri/ModTime != '']">
                            at time <xsl:value-of select="/metadata/Esri/ModTime"/>
                          </xsl:if>
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:for-each select="/metadata/metainfo/metd[. != '']">
                            <DIV>Contents last updated: <xsl:value-of /></DIV>
                          </xsl:for-each>
                        </xsl:otherwise>
                      </xsl:choose>

                      <xsl:apply-templates select="/metadata/metainfo[(metrd != '') or 
                          (metfrd != '') or (metstdn != '') or (metstdv != '') or 
                          (mettc != '') or ($any$ metextns/* != '') or 
                          (metc/cntinfo/cntvoice != '') or (metc/cntinfo/cntfax != '') or 
                          (metc/cntinfo/cntemail != '') or (metc/cntinfo/hours != '') or 
                          (metc/cntinfo/cntinst != '') or (metc/cntinfo/cntaddr/* != '') or 
                          (metc/cntinfo/*/cntper != '') or (metc/cntinfo/*/cntorg != '')]" />

                    </DIV>
                  </DIV>
                </xsl:if>

              </xsl:when>

              <!-- If nothing to show in Description tab, show message -->
              <xsl:otherwise>
                <BR/>
                <DIV STYLE="text-align:center; color:#6495ED">
                  No descriptive information about the data is available.
                </DIV>
                <BR/>
              </xsl:otherwise>
            </xsl:choose>

            <BR/>
          </DIV>

          <!-- Spatial Tab -->
          <DIV ID="Spatial" class="pv" STYLE="display:none"><BR/>

            <xsl:choose>
              <xsl:when test="/metadata[($any$ (spdoinfo/(ptvctinf/(
                  esriterm/* | sdtsterm/* | vpfterm/(vpflevel | vpfinfo/*)) | 
                  rastinfo/* | netinfo/(nettype | connrule/*)) | 
                  Esri/DataProperties/(topoinfo/*/* | lineage/Process | 
                  Terrain/*) | 
                  
                  Esri/Locator/* | 

                  idinfo/spdom/(bounding/* | lboundng/* | minalti | maxalti) | 

                  spref/(horizsys/(geograph/* | planar//* | local/* | cordsysn/* | 
                  geodetic/*) | vertdef//*) | 

                  dataqual/(lineage/(procstep//* | srcinfo/(srccite/citeinfo/title | 
                  srccitea | typesrc | srcscale | srccontr)) | 
                  posacc/(horizpa/(horizpar | qhorizpa/horizpav) | 
                  vertacc/(vertaccr | qvertpa/vertaccv)))) != '')]">


                <!-- Show contents of Spatial tab -->
                
                <!-- coordinate systems -->
                <xsl:apply-templates select="/metadata/spref/horizsys
                    [$any$ (geograph/* | planar//* | local/* | cordsysn/* | 
                    geodetic/*) != '']" />

                <xsl:if test="/metadata[($any$ spref/horizsys/(geograph/* | 
                    planar//* | local/* | cordsysn/* | geodetic/*) != '') 
                    and ($any$ spref/vertdef//* != '')]"> 
                  <BR/>
                </xsl:if>

                <xsl:apply-templates select="/metadata/spref/vertdef[$any$ */* != '']" />

                <!-- bounding coordinates -->
                <xsl:if test="/metadata[($any$ idinfo/spdom/(bounding/* | 
                    lboundng/* | minalti | maxalti) != '') and ($any$ spref/
                    (horizsys/(geograph/* | planar//* | local/* | cordsysn/* | 
                    geodetic/*) | vertdef//*) != '')]">
                  <DIV STYLE="text-align:center; color:#6495ED">_________________</DIV><BR/>
                </xsl:if>

                <xsl:apply-templates select="/metadata/idinfo/spdom
                    [$any$ (bounding/* | lboundng/* | minalti | maxalti) != '']" />

                <!-- lineage -->
                <xsl:if test="/metadata[($any$ 
                    (Esri/DataProperties/lineage/Process | 
                    dataqual/lineage/(procstep//* | srcinfo/
                    (srccite/citeinfo/title | srccitea | typesrc | srcscale | srccontr))) != '')
                    and ($any$ (idinfo/spdom/
                    (bounding/* | lboundng/* | minalti | maxalti) | 
                    spref/(horizsys/(geograph/* | planar//* | local/* | 
                    cordsysn/* | geodetic/*) | vertdef//*)) != '')]">
                  <DIV STYLE="text-align:center; color:#6495ED">_________________</DIV><BR/>
                </xsl:if>

                <xsl:if test="/metadata[$any$ 
                    (Esri/DataProperties/lineage/Process | 
                    dataqual/lineage/(procstep//* | srcinfo/(srccite/citeinfo/title | 
                    srccitea | typesrc | srcscale | srccontr))) != '']">
                  <DIV CLASS="pn">Lineage</DIV>
                  <xsl:apply-templates select="/metadata/dataqual/lineage[(procstep//* | 
                      srcinfo/(srccite/citeinfo/title | srccitea | typesrc | srcscale | srccontr)) != '']" />
                  <xsl:if test="/metadata[$any$ 
                      (Esri/DataProperties/lineage/Process != '') and
                      (dataqual/lineage/(procstep//* | srcinfo/(srccite/citeinfo/title | 
                      srccitea | typesrc | srcscale | srccontr)) != '')]"> 
                    <BR/>
                  </xsl:if>
                  <xsl:apply-templates select="/metadata/Esri/DataProperties/lineage/Process[. != '']" />
                </xsl:if>

                <!-- data quality -->
                <xsl:if test="/metadata[($any$ dataqual/posacc/(horizpa/
                    (horizpar | qhorizpa/horizpav) | vertacc/(vertaccr | 
                    qvertpa/vertaccv)) != '') and ($any$ (idinfo/spdom/
                    (bounding/* | lboundng/* | minalti | maxalti) | 
                    spref/(horizsys/(geograph/* | planar//* | local/* | 
                    cordsysn/* | geodetic/*) | vertdef//*) | 
                    Esri/DataProperties/lineage/Process | 
                    dataqual/lineage/(procstep//* | srcinfo/(srccite/citeinfo/title | 
                    srccitea | typesrc | srcscale | srccontr))) != '')]">
                  <DIV STYLE="text-align:center; color:#6495ED">_________________</DIV><BR/>
                </xsl:if>  

                <xsl:apply-templates select="/metadata/dataqual/posacc
                    [$any$ (horizpa/(horizpar | qhorizpa/horizpav) | 
                    vertacc/(vertaccr | qvertpa/vertaccv)) != '']" />

                <!-- locator properties -->
                <xsl:if test="/metadata[($any$ Esri/Locator/* != '') and ($any$ 
                    (idinfo/spdom/(bounding/* | lboundng/* | minalti | maxalti) | 
                    spref/(horizsys/(geograph/* | planar//* | local/* | 
                    cordsysn/* | geodetic/*) | vertdef//*) | 
                    Esri/DataProperties/lineage/Process | 
                    dataqual/(lineage/(procstep//* | srcinfo/(srccite/citeinfo/title | 
                    srccitea | typesrc | srcscale | srccontr)) | posacc/
                    (horizpa/(horizpar | qhorizpa/horizpav) | 
                    vertacc/(vertaccr | qvertpa/vertaccv)))) != '')]">
                  <DIV STYLE="text-align:center; color:#6495ED">_________________</DIV><BR/>
                </xsl:if>

                <xsl:if test="/metadata[$any$ Esri/Locator/* != '']">
                  <DIV CLASS="pn">Locator description</DIV>
                </xsl:if>

                <xsl:apply-templates select="/metadata/Esri/Locator[($any$ * != '')]" />

                <!-- vector, network, topology, raster, terrain data properties -->
                <xsl:if test="/metadata[($any$ (spdoinfo/(ptvctinf/(esriterm/* | 
                    sdtsterm/* | vpfterm/(vpflevel | vpfinfo/*)) | rastinfo/* | 
                    netinfo/(nettype | connrule/*)) | 
                    Esri/DataProperties/(topoinfo/*/* | Terrain/*)) != '') and ($any$ 
                    (idinfo/spdom/(bounding/* | lboundng/* | minalti | maxalti) | 
                    spref/(horizsys/(geograph/* | planar//* | local/* | 
                    cordsysn/* | geodetic/*) | vertdef//*) | 
                    Esri/DataProperties/lineage/Process | 
                    dataqual/(lineage/(procstep//* | srcinfo/(srccite/citeinfo/title | 
                    srccitea | typesrc | srcscale | srccontr)) | posacc/
                    (horizpa/(horizpar | qhorizpa/horizpav) | 
                    vertacc/(vertaccr | qvertpa/vertaccv))) | Esri/Locator/*) != '')]">
                  <DIV STYLE="text-align:center; color:#6495ED">_________________</DIV><BR/>
                </xsl:if>

                <xsl:if test="/metadata[$any$ (spdoinfo/(ptvctinf/(esriterm/* | 
                    sdtsterm/* | vpfterm/(vpflevel | vpfinfo/*)) | 
                    netinfo/(nettype | connrule/*) | rastinfo/*) | 
                    Esri/DataProperties/(topoinfo/*/* | Terrain/*)) != '']">
                  <DIV CLASS="pn">Spatial data description</DIV>
                </xsl:if>

                <xsl:apply-templates select="/metadata/spdoinfo/netinfo
                    [(nettype != '') or ($any$ connrule/* != '')]" />

                <xsl:if test="/metadata[($any$ (spdoinfo/netinfo/(nettype |  
                    connrule/*) != '') and ($any$ 
                    Esri/DataProperties/(topoinfo/*/* | Terrain/*) | 
                    spdoinfo/(ptvctinf/(esriterm/* | sdtsterm/* | 
                    vpfterm/(vpflevel | vpfinfo/*))) != ''))]">
                  <BR/>
                </xsl:if>

                <xsl:apply-templates select="/metadata/Esri/DataProperties/topoinfo
                    [($any$ */* != '')]" />

                <xsl:if test="/metadata[($any$ (Esri/DataProperties/topoinfo/*/* != '') 
                    and ($any$ Esri/DataProperties/Terrain/* | 
                    spdoinfo/(ptvctinf/(esriterm/* | 
                    sdtsterm/* | vpfterm/(vpflevel | vpfinfo/*))) != ''))]">
                  <BR/>
                </xsl:if>

                <xsl:apply-templates select="/metadata/Esri/DataProperties/Terrain
                    [($any$ * != '')]" />

                <xsl:if test="/metadata[($any$ (Esri/DataProperties/Terrain/* != '') 
                    and ($any$ spdoinfo/(ptvctinf/(esriterm/* | 
                    sdtsterm/* | vpfterm/(vpflevel | vpfinfo/*))) != ''))]">
                  <BR/>
                </xsl:if>

                <xsl:if test="/metadata[$any$ spdoinfo/ptvctinf/(esriterm/* | 
                    sdtsterm/* | vpfterm/(vpflevel | vpfinfo/*)) != '']">
                  <DIV CLASS="pn" STYLE="margin-left:0.2in">Vector data information</DIV>
                </xsl:if>

                <xsl:apply-templates select="/metadata/spdoinfo/ptvctinf/esriterm
                    [$any$ * != '']" />

                <xsl:if test="/metadata[($any$ spdoinfo/ptvctinf/esriterm/* != '') and 
                    ($any$ spdoinfo/ptvctinf/sdtsterm/* != '')]">
                  <BR/>
                </xsl:if>

                <xsl:if test="/metadata[$any$ spdoinfo/ptvctinf/sdtsterm != '']">
                  <DIV CLASS="ph2" STYLE="margin-left:0.4in" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">SDTS description
                    <DIV CLASS="pe2" STYLE="display:none">
                      <xsl:apply-templates select="
                          /metadata/spdoinfo/ptvctinf/sdtsterm[$any$ * != '']" />
                    </DIV>
                  </DIV>
                </xsl:if>

                <xsl:if test="/metadata[($any$ spdoinfo/ptvctinf/(esriterm/* | 
                    sdtsterm/*) != '') and ($any$ spdoinfo/ptvctinf/vpfterm/(vpflevel | 
                    vpfinfo/*) != '')]">
                  <BR/>
                </xsl:if>

                <xsl:if test="/metadata[$any$ spdoinfo/ptvctinf/vpfterm/(vpflevel | 
                    vpfinfo/*) != '']">
                  <DIV CLASS="ph2" STYLE="margin-left:0.4in" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">VPF description
                    <DIV CLASS="pe2" STYLE="display:none">
                      <xsl:apply-templates select="/metadata/spdoinfo/
                          ptvctinf/vpfterm[(vpflevel != '') or ($any$ vpfinfo/* != '')]" />
                    </DIV>
                  </DIV>
                </xsl:if>

                <xsl:if test="/metadata[($any$ spdoinfo/ptvctinf/(esriterm/* | 
                    sdtsterm/* | vpfterm/(vpflevel | vpfinfo/*)) != '') and 
                    ($any$ spdoinfo/rastinfo != '')]">
                  <BR/>
                </xsl:if>

                <xsl:apply-templates select="/metadata/spdoinfo/rastinfo[$any$ * != '']" />

              </xsl:when>

              <!-- If nothing to show in Spatial tab, show message -->
              <xsl:otherwise>
                <BR/>
                <DIV STYLE="text-align:center; color:#6495ED">
                  No detailed spatial information is is available.
                </DIV>
                <BR/> 
              </xsl:otherwise>
            </xsl:choose>

            <BR/>
          </DIV>

          <!-- Attributes Tab -->
          <DIV ID="Attributes" class="pv" STYLE="display:none"><BR/>

            <xsl:choose>
              <xsl:when test="/metadata[($any$ eainfo/(overview/* | 
                  detailed/(enttyp/(enttypl | enttypt | enttypc | enttypd) | relinfo/* | 
                  attr/(attrlabl | attalias | attrtype | attwidth | atprecis | 
                  atoutwid | atnumdec | atscale | attrdef) | subtype/(stname | 
                  stcode | stfield/(stfldnm | stflddv | stflddd/*)))) != '')]">


                <!-- Show contents of Attributes tab -->
                <xsl:for-each select="metadata/eainfo/detailed[$any$ 
                    (enttyp/(enttypl | enttypt | enttypc | enttypd) | relinfo/* | 
                    attr/(attrlabl | attalias | attrtype | attwidth | atprecis | 
                    atoutwid | atnumdec | atscale | attrdef) | subtype/(stname | 
                    stcode | stfield/(stfldnm | stflddv | stflddd/*))) != '']">

                  <xsl:choose>
                    <xsl:when test="context()[enttyp/enttypl != '']">
                      <DIV CLASS="pn">Details for <xsl:value-of select="enttyp/enttypl"/></DIV>
                    </xsl:when>
                    <xsl:otherwise>
                      <DIV CLASS="pn">Entity details</DIV>
                    </xsl:otherwise>
                  </xsl:choose>

                  <xsl:apply-templates select="enttyp[(enttypt != '') or 
                      (enttypc != '') or (enttypd != '')]" />

                  <xsl:apply-templates select="relinfo[$any$ * != '']" />

                  <xsl:apply-templates select="attr[(attrlabl != '') or 
                      (attalias != '') or (attrtype != '') or (attwidth != '') or 
                      (atprecis != '') or (atoutwid != '') or (atnumdec != '') or 
                      (attscale != '') or (attrdef != '')]" />

                  <xsl:if test="context()[($any$ subtype/(stname | stcode | 
                      stfield/(stfldnm | stflddv | stflddd/*)) != '') and 
                      ($any$ (relinfo/* | attr/(attrlabl | attalias | attrtype | 
                      attwidth | atprecis | atoutwid | atnumdec | atscale | attrdef)) != '')]">
                    <BR/>
                  </xsl:if>

                  <xsl:apply-templates select="subtype[(stname != '') or (stcode != '') or 
                      ($any$ stfield/(stfldnm | stflddv) != '') or  
                      ($any$ stfield/stflddd/* != '')]" />

                  <xsl:if test="context()[not(end())]">
                    <DIV STYLE="text-align:center; color:#6495ED">_________________</DIV><BR/>
                  </xsl:if>

                </xsl:for-each>

                <xsl:if test="/metadata[($any$ eainfo/overview/* != '') and ($any$ 
                    eainfo/detailed/(enttyp/(enttypl | enttypt | enttypc | enttypd) | 
                    relinfo/* | attr/(attrlabl | attalias | attrtype | attwidth | 
                    atprecis | atoutwid | atnumdec | atscale | attrdef) | 
                    subtype/(stname | stcode | stfield/(stfldnm | stflddv | 
                    stflddd/*))) != '')]">
                  <DIV STYLE="text-align:center; color:#6495ED">_________________</DIV><BR/>
                </xsl:if>

                <xsl:apply-templates select="metadata/eainfo/overview[$any$ * != '']" />

              </xsl:when>

              <!-- If nothing to show in Attributes tab, show message -->
              <xsl:otherwise>
                <BR/>
                <DIV STYLE="text-align:center; color:#6495ED">
                  No detailed attribute information is available.
                </DIV>
                <BR/>
              </xsl:otherwise>
            </xsl:choose>

            <BR/>
          </DIV>

        </DIV>

      </xsl:otherwise> 
    </xsl:choose> 
      
    <!-- <CENTER><FONT COLOR="#6495ED">Metadata stylesheets are provided courtesy of ESRI.  Copyright (c) 1999-2004, Environmental Systems Research Institute, Inc.  All rights reserved.</FONT></CENTER> -->

    </BODY>

  </HTML>
</xsl:template>

================================

<!-- DESCRIPTION TAB -->

<!-- Thumbnail -->
<xsl:template match="/metadata/Binary/Thumbnail/img[(@src != '')]">
      <IMG ID="thumbnail" ALIGN="absmiddle" STYLE="height:144; 
          border:'2 outset #FFFFFF'; position:relative">
        <xsl:attribute name="SRC"><xsl:value-of select="@src"/></xsl:attribute>
      </IMG>
      <BR/><BR/>
</xsl:template>
<xsl:template match="/metadata/idinfo/browse/img[@src != '']">
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

<!-- Keywords -->
<xsl:template match="/metadata/idinfo/keywords[$any$ */(themekey | placekey | stratkey | tempkey) != '']">
  <DIV CLASS="pn">Keywords</DIV>
  <xsl:for-each select="theme[$any$ themekey != '']">
    <DIV STYLE="margin-left:0.2in" CLASS="lt2"><SPAN CLASS="pn">Theme: </SPAN>
      <xsl:for-each select="themekey">
        <xsl:choose>
          <xsl:when test="context()[. = 'REQUIRED: Common-use word or phrase used to describe the subject of the data set.']">
            <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="context()[. = 'Common-use word or phrase used to describe the subject of the data set.  REQUIRED.']">
                <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN>
              </xsl:when>
              <xsl:otherwise><xsl:value-of /></xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
      </xsl:for-each>
    </DIV>
  </xsl:for-each>
  <xsl:for-each select="place[$any$ placekey != '']">
    <DIV STYLE="margin-left:0.2in" CLASS="lt2"><SPAN CLASS="pn">Place: </SPAN>
      <xsl:for-each select="placekey">
        <xsl:value-of /><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
      </xsl:for-each>
    </DIV>
  </xsl:for-each>
  <xsl:for-each select="stratum[$any$ stratkey != '']">
    <DIV STYLE="margin-left:0.2in" CLASS="lt2"><SPAN CLASS="pn">Stratum: </SPAN>
      <xsl:for-each select="stratkey">
        <xsl:value-of /><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
      </xsl:for-each>
    </DIV>
  </xsl:for-each>
  <xsl:for-each select="temporal[$any$ tempkey != '']">
    <DIV STYLE="margin-left:0.2in" CLASS="lt2"><SPAN CLASS="pn">Temporal: </SPAN>
      <xsl:for-each select="tempkey">
        <xsl:value-of /><xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
      </xsl:for-each>
    </DIV>
  </xsl:for-each>
</xsl:template>

--------

<!-- Description -->
<xsl:template match="/metadata/idinfo/descript[(abstract != '') or (purpose != '') or 
    (supplinf != '')]">
  <xsl:for-each select="abstract[. != '']">
    <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Abstract
      <DIV CLASS="pe2" STYLE="display:">
        <xsl:choose>
          <xsl:when test="context()[. = 'REQUIRED: A brief narrative summary of the data set.']">
            <SPAN CLASS="lt" STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="context()[. = 'A brief narrative summary of the data set.  REQUIRED.']">
                <SPAN CLASS="lt" STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
              </xsl:when>
              <xsl:otherwise>
                <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN><BR/>
                <SCRIPT>fix(original)</SCRIPT>      
              </xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </DIV>
    </DIV>
  </xsl:for-each>
  <xsl:for-each select="purpose[. != '']">
    <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Purpose
      <DIV ID="Purpose" CLASS="pe2" STYLE="display:">
        <xsl:choose>
          <xsl:when test="context()[. = 'REQUIRED: A summary of the intentions with which the data set was developed.']">
            <SPAN CLASS="lt" STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="context()[. = 'A summary of the intentions with which the data set was developed.  REQUIRED.']">
                <SPAN CLASS="lt" STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
              </xsl:when>
              <xsl:otherwise>
                <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN><BR/>
                <SCRIPT>fix(original)</SCRIPT>      
              </xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </DIV>
    </DIV>
  </xsl:for-each>
  <xsl:for-each select="supplinf[. != '']">
    <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Supplementary Information
      <DIV CLASS="pe2" STYLE="display:">
        <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN><BR/>
        <SCRIPT>fix(original)</SCRIPT>      
      </DIV>
    </DIV>
  </xsl:for-each>
</xsl:template>

--------

<!-- Enclosures -->
<xsl:template match="/metadata/Binary[(Enclosure/Data/@EsriPropertyType = 'Base64') or 
    (Enclosure/img/@src != '')]">
  <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Enclosed files containing additional information
    <DIV CLASS="pe2" STYLE="display:">
      <xsl:for-each select="Enclosure[Data/@EsriPropertyType = 'Base64']">
        <LI><xsl:value-of select="Data/@OriginalFileName"/>: <xsl:value-of select="./Descript"/></LI>
      </xsl:for-each>
      <xsl:for-each select="Enclosure[img/@src != '']">
        <LI><xsl:value-of select="img/@OriginalFileName"/> (Image): <xsl:value-of select="./Descript"/></LI>
      </xsl:for-each>
      <xsl:if test="context()[$any$ Enclosure[img/@src != '']]">
        <DIV CLASS="ph2" STYLE="margin-left=-0.25" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Show thumbnails of images
          <DIV CLASS="pe2" STYLE="display:none">
            <BR/>
            <xsl:for-each select="Enclosure[img/@src != '']">
              <IMG STYLE="height:144; border:'2 outset #FFFFFF'; position:relative">
                <xsl:attribute name="TITLE"><xsl:value-of select="img/@OriginalFileName"/></xsl:attribute>
                <xsl:attribute name="SRC"><xsl:value-of select="img/@src"/></xsl:attribute>
              </IMG>
              <BR/><BR/>
            </xsl:for-each>
          </DIV>
        </DIV>
      </xsl:if>
    </DIV>
  </DIV>
</xsl:template>

--------

<!-- Browse Graphics -->
<xsl:template match="/metadata/idinfo/browse[browsen != '']">
  <LI>
    <xsl:for-each select="browsed[. != '']"><xsl:value-of /></xsl:for-each><xsl:for-each select="browset[. != '']"> (<xsl:value-of/>)</xsl:for-each><xsl:if test="context()[(browset != '') or (browsed != '')]">: </xsl:if>
    <xsl:for-each select="browsen[. != '']">
      <A TARGET="viewer"><xsl:attribute name="HREF"><xsl:value-of/></xsl:attribute><xsl:value-of/></A>
    </xsl:for-each>
  </LI>
</xsl:template>

--------

<!-- Status -->
<xsl:template match="/metadata/idinfo/status[$any$ * != '']">
  <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Status of the data
    <DIV CLASS="pe2" STYLE="display:none">
      <xsl:for-each select="progress[. != '']">
        <xsl:choose>
          <xsl:when test="context()[. = 'REQUIRED: The state of the data set.']">
            <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="context()[. = 'The state of the data set.  REQUIRED.']">
                <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
              </xsl:when>
              <xsl:otherwise><xsl:value-of/><BR/></xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
      <xsl:for-each select="update[. != '']">
        <I>Data update frequency: </I>
        <xsl:choose>
          <xsl:when test="context()[. = 'REQUIRED: The frequency with which changes and additions are made to the data set after the initial data set is completed.']">
            <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="context()[. = 'The frequency with which changes and additions are made to the data set after the initial data set is completed.  REQUIRED.']">
                <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
              </xsl:when>
              <xsl:otherwise><xsl:value-of/><BR/></xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
    </DIV>
  </DIV>
</xsl:template>

--------

<!-- Time Period of the Data -->
<xsl:template match="/metadata/idinfo/timeperd[$any$ .//* != '']">
  <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Time period for which the data is relevant
    <DIV CLASS="pe2" STYLE="display:none">
      <xsl:apply-templates select="timeinfo/sngdate[$any$ * != '']"/>
      <xsl:apply-templates select="timeinfo/mdattim/sngdate[$any$ * != '']"/>
      <xsl:apply-templates select="timeinfo/rngdates[$any$ * != '']"/>
      <xsl:for-each select="current[. != '']">
        <DIV>
          <I>Description: </I>
          <xsl:choose>
            <xsl:when test="context()[. = 'REQUIRED: The basis on which the time period of content information is determined.']">
              <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN>
            </xsl:when>
            <xsl:otherwise>
              <xsl:choose>
                <xsl:when test="context()[. = 'The basis on which the time period of content information is determined.  REQUIRED.']">
                  <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN>
                </xsl:when>
                <xsl:otherwise>
                  <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN><BR/>
                  <SCRIPT>fix(original)</SCRIPT>      
                </xsl:otherwise>
              </xsl:choose>
            </xsl:otherwise>
          </xsl:choose>
        </DIV>
      </xsl:for-each>
    </DIV>
  </DIV>
</xsl:template>

--------

<!-- Publication Info -->
<xsl:template match="/metadata/idinfo/citation/citeinfo[(origin != '') or (pubdate != '') or 
    (pubtime != '') or ($any$ pubinfo/* != '') or ($any$ serinfo/* != '')]">
  <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Publication Information
    <DIV CLASS="pe2" STYLE="display:none"><SPAN CLASS="lt2">
      <xsl:for-each select="origin[$any$ . != '']">
        <xsl:if test="context()[0]"><I>Who created the data: </I></xsl:if>
        <xsl:choose>
          <xsl:when test="context()[. = 'REQUIRED: The name of an organization or individual that developed the data set.']">
            <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="context()[. = 'The name of an organization or individual that developed the data set.  REQUIRED.']">
                <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN>
              </xsl:when>
              <xsl:otherwise><xsl:if test="context()[. != '']"><xsl:value-of/></xsl:if></xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
        <xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
        <xsl:if test="context()[end()]"><BR/></xsl:if>
      </xsl:for-each></SPAN>
      <xsl:if test="context()[(pubdate != '') or (pubtime != '')]">
        <DIV><I>Date and time: </I>
          <xsl:choose>
            <xsl:when test="context()[pubdate = 'REQUIRED: The date when the data set is published or otherwise made available for release.']">
              <SPAN STYLE="color:#999999"><xsl:value-of select="pubdate"/></SPAN>
            </xsl:when>
            <xsl:otherwise>
              <xsl:choose>
                <xsl:when test="context()[pubdate = 'The date when the data set is published or otherwise made available for release.  REQUIRED']">
                  <SPAN STYLE="color:#999999"><xsl:value-of select="pubdate"/></SPAN>
                </xsl:when>
                <xsl:otherwise><xsl:value-of select="pubdate"/></xsl:otherwise>
              </xsl:choose>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:if test="context()[pubtime != '']"> at time <xsl:value-of select="pubtime"/></xsl:if>
        </DIV>
      </xsl:if>
      <xsl:for-each select="pubinfo[$any$ * != '']">
        <DIV><I>Publisher and place: </I>
          <xsl:value-of select="publish"/><xsl:if test="context()[publish != '' and pubplace != '']">, </xsl:if>
          <xsl:value-of select="pubplace"/>
        </DIV>
      </xsl:for-each>
      <xsl:for-each select="serinfo/sername[. != '']">
        <DIV><I>Series name: </I><xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="serinfo/issue[. != '']">
        <DIV><I>Series issue: </I><xsl:value-of/></DIV>
      </xsl:for-each>
    </DIV>
  </DIV>
</xsl:template>

--------

<!-- Distribution Info -->
<xsl:template match="/metadata/distinfo/stdorder/digform[($any$ digtinfo/(formname | 
    dssize | transize | filedec) != '') or ($any$ digtopt/onlinopt/accinstr != '') or 
    ($any$ digtopt/onlinopt/computer/(networka/* | sdeconn/*) != '') or 
    ($any$ digtopt/onlinopt/computer/dialinst/(dialtel | dialfile) != '') or 
    ($any$ digtopt/offoptn/offmedia != '')]">
  <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Accessing the data
    <DIV CLASS="pe2" STYLE="display:none">
      <xsl:for-each select="digtinfo/formname[. != '']">
        <I>Data format: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="digtinfo/dssize[. != '']">
        <I>Size of the data: </I><xsl:value-of /> MB<BR/>
      </xsl:for-each>
      <xsl:for-each select="digtinfo/transize[. != '']">
        <I>Data transfer size: </I><xsl:value-of /> MB<BR/>
      </xsl:for-each>
      <xsl:for-each select="digtinfo/filedec[. != '']">
        <I>How to decompress the file: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="digtopt/onlinopt"> 
        <xsl:for-each select="computer/networka/networkr[. != '']">
          <xsl:if test="context()[0]"><DIV CLASS="pn">Network location:</DIV></xsl:if>
          <DIV CLASS="pe2">
            <xsl:for-each select="context()[. != '']">
              <LI><xsl:value-of /></LI>
            </xsl:for-each>
          </DIV>
        </xsl:for-each>
        <xsl:for-each select="computer/sdeconn[$any$ * != '']">
          <DIV CLASS="pn">SDE connection:</DIV>
          <DIV CLASS="pe2">
            <LI>Server: <xsl:value-of select="server"/></LI>
            <LI>Instance: <xsl:value-of select="instance"/></LI>
            <LI>Version: <xsl:value-of select="version"/></LI>
            <LI>Username: <xsl:value-of select="user"/></LI>
          </DIV>
        </xsl:for-each>
        <xsl:for-each select="computer/dialinst[($any$ dialtel != '') or ($any$ dialfile != '')]">
          <xsl:if test="context()[0]"><DIV CLASS="pn">Dialup instructions:</DIV></xsl:if>
          <DIV CLASS="pe2">
            <xsl:for-each select="dialtel[. != '']">
              <LI><xsl:value-of /></LI>
            </xsl:for-each>
            <xsl:for-each select="dialfile[. != '']">
              <LI><xsl:value-of /></LI>
            </xsl:for-each>
          </DIV>
        </xsl:for-each>
        <xsl:for-each select="accinstr[. != '']">
          <DIV CLASS="pe2"><I>Access instructions: </I><xsl:value-of /></DIV>
        </xsl:for-each>
      </xsl:for-each>
      <xsl:for-each select="digtopt/offoptn/offmedia[. != '']">
        <xsl:if test="context()[0]"><I>Available media: </I></xsl:if>
        <xsl:if test="context()[. != '']"><xsl:value-of/></xsl:if>
        <xsl:if test="context()[not(end()) and . != '']">, </xsl:if>
        <xsl:if test="context()[end()]"><BR/></xsl:if>
      </xsl:for-each>
      <xsl:if test="/metadata[($any$ distinfo/stdorder/digform/(digtinfo/(formname | 
          dssize | transize | filedec) | digtopt/(onlinopt/(computer/(networka/* | 
          sdeconn/* | dialinst/(dialtel | dialfile)) | accinstr | offoptn/offmedia))) != '')
          and ($any$ idinfo/(accconst | useconst) != '')]">
        <BR/>
      </xsl:if>
    </DIV>
  </DIV>
</xsl:template>

--------

<!-- Data access constraints -->
<xsl:template match="/metadata/idinfo/accconst[. != '']">
  <I>Access constraints: </I>
  <xsl:choose>
    <xsl:when test="context()[. = 'REQUIRED: Restrictions and legal prerequisites for accessing the data set.']">
      <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
    </xsl:when>
    <xsl:otherwise>
      <xsl:choose>
        <xsl:when test="context()[. = 'Restrictions and legal prerequisites for accessing the data set.  REQUIRED.']">
          <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
        </xsl:when>
        <xsl:otherwise><SPAN CLASS="lt"><xsl:value-of /><BR/></SPAN></xsl:otherwise>
      </xsl:choose>
    </xsl:otherwise>
  </xsl:choose>
</xsl:template>

--------

<!-- Data use constraints -->
<xsl:template match="/metadata/idinfo/useconst[. != '']">
  <DIV>
    <I>Use constraints: </I>
    <xsl:choose>
      <xsl:when test="context()[. = 'REQUIRED: Restrictions and legal prerequisites for using the data set after access is granted.']">
        <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="context()[. = 'Restrictions and legal prerequisites for using the data set after access is granted.  REQUIRED.']">
            <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
          </xsl:when>
          <xsl:otherwise>
            <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN><BR/>
            <SCRIPT>fix(original)</SCRIPT>      
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
  </DIV>
</xsl:template>

--------

<!-- Metadata Info -->
<xsl:template match="/metadata/metainfo[(metrd != '') or (metfrd != '') or (metstdn != '') or 
    (metstdv != '') or (mettc != '') or ($any$ metextns/* != '') or (metc/cntinfo/cntvoice != '') or 
    (metc/cntinfo/cntfax != '') or (metc/cntinfo/cntemail != '') or (metc/cntinfo/hours != '') or 
    (metc/cntinfo/cntinst != '') or (metc/cntinfo/cntaddr/* != '') or 
    (metc/cntinfo/*/cntper != '') or (metc/cntinfo/*/cntorg != '')]">
  <xsl:for-each select="metrd[. != '']">
    <DIV>Contents last reviewed: <xsl:value-of /></DIV>
  </xsl:for-each>
  <xsl:for-each select="metfrd[. != '']">
    <DIV>Contents to be reviewed: <xsl:value-of /></DIV>
  </xsl:for-each>
  <xsl:if test="context()[($any$ metc/cntinfo/(cntvoice | cntfax | cntemail | hours | cntinst | 
      */cntper | */cntorg | cntaddr/(address | city | state | postal | country)) != '')]">
    <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Who completed this document
      <DIV CLASS="pe2" STYLE="display:none">
        <xsl:apply-templates select="metc/cntinfo"/>
      </DIV>
    </DIV>
  </xsl:if>
  <xsl:if test="context()[(metstdn != '') or (metstdv != '') or (mettc != '') or 
      ($any$ metextns/* != '')]">
    <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Standards used to create this document
      <DIV CLASS="pe2" STYLE="display:none">
        <xsl:for-each select="metstdn[. != '']">
          <I>Standard name: </I><xsl:value-of /><BR/>
        </xsl:for-each>
        <xsl:for-each select="metstdv[. != '']">
          <I>Standard version: </I><xsl:value-of /><BR/>
        </xsl:for-each>
        <xsl:for-each select="mettc[. != '']">
          <I>Time convention used in this document: </I><xsl:value-of /><BR/>
        </xsl:for-each>
        <xsl:for-each select="metextns[(metprof != '') or (onlink != '')]">
          <xsl:if test="context()[0]">Metadata profiles defining additonal information</xsl:if>
          <LI STYLE="margin-left:0.2in">
            <xsl:for-each select="metprof[. != '']"><xsl:value-of/></xsl:for-each><xsl:if test="context()[(metprof != '') and (onlink != '')]">: </xsl:if>
            <xsl:for-each select="onlink[. != '']">
              <A TARGET="viewer"><xsl:attribute name="HREF"><xsl:value-of/>
                </xsl:attribute><xsl:value-of/>
              </A>
              <xsl:if test="context()[not(end())]">, </xsl:if>
            </xsl:for-each>
          </LI>
        </xsl:for-each>
      </DIV>
    </DIV>
  </xsl:if>
</xsl:template>


================================


<!-- SPATIAL TAB -->

<!-- Horizontal Coordinate Systems -->
<xsl:template match="/metadata/spref/horizsys[$any$ (geograph/* | 
    planar//* | local/* | cordsysn/* | geodetic/*) != '']">
  <DIV CLASS="pn">Horizontal coordinate system</DIV>
  <xsl:if test="context()[$any$ cordsysn/* != '']">
    <xsl:for-each select="cordsysn/projcsn[. != '']">
      <DIV STYLE="margin-left:0.2in"><I>Projected coordinate system name: </I><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="cordsysn/geogcsn[. != '']">
      <DIV STYLE="margin-left:0.2in"><I>Geographic coordinate system name: </I><xsl:value-of/></DIV>
    </xsl:for-each>
  </xsl:if>
  <xsl:if test="context()[$any$ (geograph/* | planar//* | local/* | geodetic/*) != '']">
    <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Details
      <DIV CLASS="pe2" STYLE="display:none">

        <xsl:apply-templates select="geograph[$any$ * != '']"/>
        <xsl:apply-templates select="planar[$any$ .//* != '']"/>
        <xsl:apply-templates select="local[$any$ * != '']"/>

        <xsl:if test="context()[($any$ (geograph/* | planar//* | local/*) != '') and 
            ($any$ geodetic/* != '')]">
          <BR/>
        </xsl:if>

        <xsl:apply-templates select="geodetic[$any$ * != '']"/>

      </DIV>
    </DIV>
  </xsl:if>
</xsl:template>

--------

<!-- Vertical Coordinate Systems -->
<xsl:template match="/metadata/spref/vertdef[$any$ .//* != '']">
  <DIV CLASS="pn">Vertical coordinate system</DIV>
  <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Details
    <DIV CLASS="pe2" STYLE="display:none">
      <xsl:apply-templates select="context()"/>
    </DIV>
  </DIV>
</xsl:template>

--------

<!-- Bounding Coordinates -->
<xsl:template match="/metadata/idinfo/spdom[$any$ (bounding/* | lboundng/* | 
    minalti | maxalti) != '']">
  <DIV CLASS="pn">Bounding coordinates</DIV>
  <xsl:if test="context()[$any$ (bounding/* | lboundng/*) != '']">
    <DIV STYLE="margin-left:0.2in" CLASS="pn">Horizontal</DIV>
    <xsl:for-each select="bounding[$any$ * != '']">
      <DIV STYLE="margin-left:0.4in" CLASS="pn">In decimal degrees</DIV>
      <DIV STYLE="margin-left:0.6in"><I>West: </I>
        <xsl:choose>
          <xsl:when test="context()[westbc = 'REQUIRED: Western-most coordinate of the limit of coverage expressed in longitude.']">
            <SPAN STYLE="color:#999999"><xsl:value-of select="westbc"/></SPAN>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="context()[westbc = 'Western-most coordinate of the limit of coverage expressed in longitude.  REQUIRED.']">
                <SPAN STYLE="color:#999999"><xsl:value-of select="westbc"/></SPAN>
              </xsl:when>
              <xsl:otherwise><xsl:value-of select="westbc"/></xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </DIV>
      <DIV STYLE="margin-left:0.6in"><I>East: </I>
        <xsl:choose>
          <xsl:when test="context()[eastbc = 'REQUIRED: Eastern-most coordinate of the limit of coverage expressed in longitude.']">
            <SPAN STYLE="color:#999999"><xsl:value-of select="eastbc"/></SPAN>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="context()[eastbc = 'Eastern-most coordinate of the limit of coverage expressed in longitude.  REQUIRED.']">
                <SPAN STYLE="color:#999999"><xsl:value-of select="eastbc"/></SPAN>
              </xsl:when>
              <xsl:otherwise><xsl:value-of select="eastbc"/></xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </DIV>
      <DIV STYLE="margin-left:0.6in"><I>North: </I>
        <xsl:choose>
          <xsl:when test="context()[northbc = 'REQUIRED: Northern-most coordinate of the limit of coverage expressed in latitude.']">
            <SPAN STYLE="color:#999999"><xsl:value-of select="northbc"/></SPAN>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="context()[northbc = 'Northern-most coordinate of the limit of coverage expressed in latitude.  REQUIRED.']">
                <SPAN STYLE="color:#999999"><xsl:value-of select="northbc"/></SPAN>
              </xsl:when>
              <xsl:otherwise><xsl:value-of select="northbc"/></xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </DIV>
      <DIV STYLE="margin-left:0.6in"><I>South: </I>
        <xsl:choose>
          <xsl:when test="context()[southbc = 'REQUIRED: Southern-most coordinate of the limit of coverage expressed in latitude.']">
            <SPAN STYLE="color:#999999"><xsl:value-of select="southbc"/></SPAN>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="context()[southbc = 'Southern-most coordinate of the limit of coverage expressed in latitude.  REQUIRED.']">
                <SPAN STYLE="color:#999999"><xsl:value-of select="southbc"/></SPAN>
              </xsl:when>
              <xsl:otherwise><xsl:value-of select="southbc"/></xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </DIV>
    </xsl:for-each>
    <xsl:for-each select="lboundng[$any$ * != '']">
      <DIV STYLE="margin-left:0.4in" CLASS="pn">In projected or local coordinates</DIV>
      <DIV STYLE="margin-left:0.6in"><I>Left: </I><xsl:value-of select="leftbc"/></DIV>
      <DIV STYLE="margin-left:0.6in"><I>Right: </I><xsl:value-of select="rightbc"/></DIV>
      <DIV STYLE="margin-left:0.6in"><I>Top: </I><xsl:value-of select="topbc"/></DIV>
      <DIV STYLE="margin-left:0.6in"><I>Bottom: </I><xsl:value-of select="bottombc"/></DIV>
    </xsl:for-each>
  </xsl:if>

  <xsl:if test="context()[($any$ (bounding/* | lboundng/*) != '') and 
      ($any$ (minalti | maxalti) !='')]">
    <BR/>
  </xsl:if>

  <xsl:if test="context()[$any$ (minalti | maxalti) != '']">
    <DIV STYLE="margin-left:0.2in" CLASS="pn">Vertical</DIV>
    <xsl:for-each select="minalti[. != '']">
      <DIV STYLE="margin-left:0.4in"><I>Minimum elevation: </I>
        <xsl:if test="context()[. != '1.#QNAN0']"><xsl:value-of /></xsl:if>
        <xsl:if test="context()[. = '1.#QNAN0']">Unknown</xsl:if>
      </DIV>
    </xsl:for-each>
    <xsl:for-each select="maxalti[. != '']">
      <DIV STYLE="margin-left:0.4in"><I>Maximum elevation: </I>
        <xsl:if test="context()[. != '1.#QNAN0']"><xsl:value-of /></xsl:if>
        <xsl:if test="context()[. = '1.#QNAN0']">Unknown</xsl:if>
      </DIV>
    </xsl:for-each>
  </xsl:if>
</xsl:template>

--------

<!-- Data Quality -->
<xsl:template match="/metadata/dataqual/posacc[$any$ (horizpa/(horizpar | 
    qhorizpa/horizpav) | vertacc/(vertaccr | qvertpa/vertaccv)) != '']">
  <DIV CLASS="pn">Spatial data quality</DIV>
  <xsl:for-each select="horizpa[(horizpar != '') or (qhorizpa/horizpav != '')]">
    <DIV CLASS="ph2" STYLE="margin-left:0.2in" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Horizontal positional accuracy
      <DIV CLASS="pe2" STYLE="margin-left:0.2in; display:none">
        <xsl:for-each select="horizpar[. != '']">
          <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN>
          <SCRIPT>fix(original)</SCRIPT>      
        </xsl:for-each>
        <xsl:for-each select="qhorizpa[$any$ horizpav != '']">
          <xsl:for-each select="horizpav[. != '']">
            <DIV STYLE="margin-left:0.2in"><I>Estimated accuracy: </I><xsl:value-of /></DIV>
          </xsl:for-each>
          <xsl:for-each select="horizpae[. != '']">
            <DIV STYLE="margin-left:0.2in">
              <I>How this value was determined: </I>
              <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN>
              <SCRIPT>fix(original)</SCRIPT>
            </DIV>      
          </xsl:for-each>
        </xsl:for-each>
      </DIV>
    </DIV>
  </xsl:for-each>

  <xsl:if test="context()[($any$ horizpa/(horizpar | qhorizpa/horizpav) != '')
      and ($any$ vertacc/(vertaccr | qvertpa/vertaccv) != '')]">
    <BR/>
  </xsl:if>

  <xsl:for-each select="vertacc[(vertaccr != '') or (qvertpa/vertaccv != '')]">
    <DIV CLASS="ph2" STYLE="margin-left:0.2in" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Vertical positional accuracy
      <DIV CLASS="pe2" STYLE="margin-left:0.2in; display:none">
        <xsl:for-each select="vertaccr[. != '']">
          <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN>
          <SCRIPT>fix(original)</SCRIPT>      
        </xsl:for-each>
        <xsl:for-each select="qvertpa[$any$ vertaccv != '']">
          <xsl:for-each select="vertaccv[. != '']">
            <DIV STYLE="margin-left:0.2in"><I>Estimated accuracy: </I><xsl:value-of /></DIV>
          </xsl:for-each>
          <xsl:for-each select="vertacce[. != '']">
            <DIV STYLE="margin-left:0.2in">
              <I>How this value was determined: </I>
              <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN>
              <SCRIPT>fix(original)</SCRIPT>
            </DIV>      
          </xsl:for-each>
        </xsl:for-each>
      </DIV>
    </DIV>
  </xsl:for-each>
</xsl:template>

--------

<!-- FGDC lineage -->
<xsl:template match="/metadata/dataqual/lineage[(procstep//* | 
    srcinfo/(srccite/citeinfo/title | srccitea | typesrc | srcscale | srccontr)) != '']">
  <DIV CLASS="pn" STYLE="margin-left:0.2in">FGDC lineage</DIV>
  <xsl:for-each select="procstep[.//* != '']">
    <DIV CLASS="ph2" STYLE="margin-left:0.4in" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Process step <xsl:eval>formatIndex(childNumber(this), "1")</xsl:eval>
      <DIV CLASS="pe2" STYLE="margin-left:0.2in; display:none">
          <xsl:for-each select="procdesc[. != '']">
            <I>Process description: </I><xsl:value-of /><BR/>
          </xsl:for-each>
          <xsl:for-each select="procsv[. != '']">
            <I>Process software and version: </I><xsl:value-of /><BR/>
          </xsl:for-each>
          <xsl:for-each select="srcused[. != '']">
            <I>Source used: </I><xsl:value-of /><BR/>
          </xsl:for-each>
          <xsl:for-each select="srcprod[. != '']">
            <I>Source produced: </I><xsl:value-of /><BR/>
          </xsl:for-each>
          <xsl:if test="procdate[. != '']">
            <I>Process date: </I><xsl:value-of select="procdate" />
            <xsl:if test="proctime[. != '']">at time <xsl:value-of select="proctime" /></xsl:if><BR/>
          </xsl:if>
          <xsl:if test="context()[($any$ proccont/cntinfo/(cntvoice | cntfax | cntemail | hours | cntinst | 
              */cntper | */cntorg | cntaddr/(address | city | state | postal | country)) != '')]">
            <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Who did this process
              <DIV CLASS="pe2" STYLE="display:none">
                <xsl:apply-templates select="proccont/cntinfo"/>
              </DIV>
            </DIV>
          </xsl:if>
          <xsl:if test="context()[not(end())]"><BR/></xsl:if>
      </DIV>
    </DIV>
  </xsl:for-each>
  <xsl:if test="srcinfo[$any$ (srccite/citeinfo/title | 
      srccitea | typesrc | srcscale | srccontr) != '']">
    <DIV CLASS="pn" STYLE="margin-left:0.4in">Sources</DIV>
    <xsl:for-each select="srcinfo[(srccite/citeinfo/title | 
      srccitea | typesrc | srcscale | srccontr) != '']">
      <DIV CLASS="ph2" STYLE="margin-left:0.6in" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Source <xsl:eval>formatIndex(childNumber(this), "1")</xsl:eval><xsl:if test="context()[(srccite/citeinfo/title | srccitea) != '']">:
          <xsl:value-of select="srccite/citeinfo/title"/>
          <xsl:if test="srccitea[. != '']">(<xsl:value-of select="srccitea"/>)</xsl:if>
        </xsl:if>
        <DIV CLASS="pe2" STYLE="margin-left:0.2in; display:none">
          <xsl:for-each select="typesrc[. != '']">
            <I>Media: </I><xsl:value-of /><BR/>
          </xsl:for-each>
          <xsl:for-each select="srcscale[. != '']">
            <I>Scale denominator: </I><xsl:value-of /><BR/>
          </xsl:for-each>
          <xsl:for-each select="srccontr[. != '']">
            <I>Contribution: </I><xsl:value-of /><BR/>
          </xsl:for-each>
          <xsl:for-each select="srctime[.//* != '']">
            <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Currentness of this source
              <DIV CLASS="pe2" STYLE="display:none">
                <xsl:apply-templates select="timeinfo/sngdate[$any$ * != '']"/>
                <xsl:apply-templates select="timeinfo/mdattim/sngdate[$any$ * != '']"/>
                <xsl:apply-templates select="timeinfo/rngdates[$any$ * != '']"/>
                <xsl:for-each select="current[. != '']">
                  <DIV>
                    <I>Description: </I>
                    <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN><BR/>
                    <SCRIPT>fix(original)</SCRIPT>      
                  </DIV>
                </xsl:for-each>
              </DIV>
            </DIV>
          </xsl:for-each>
          <xsl:if test="context()[not(end())]"><BR/></xsl:if>
        </DIV>
      </DIV>
    </xsl:for-each>
  </xsl:if>
</xsl:template>

--------

<!-- ESRI geoprocessing history -->
<xsl:template match="/metadata/Esri/DataProperties/lineage/Process[. != '']">
  <xsl:if test="context()[0]"> 
    <DIV STYLE="margin-left:0.2in" CLASS="pn">ESRI geoprocessing history</DIV>
  </xsl:if>
  <DIV CLASS="ph2" STYLE="margin-left:0.4in" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)"><xsl:eval>formatIndex(childNumber(this), "1")</xsl:eval>. <xsl:choose>
      <xsl:when test="@Name[. != '']">
        <xsl:value-of select="@Name"/>
      </xsl:when>
      <xsl:otherwise>Process</xsl:otherwise>
    </xsl:choose>
    <DIV CLASS="pe2" STYLE="display:none">
      <xsl:if test="@Date[. != '']">
        <I>Date and time:</I> <xsl:value-of select="@Date"/>
      <xsl:if test="@Time[. != '']"> at time <xsl:value-of select="@Time"/></xsl:if><BR/>
      </xsl:if>
      <xsl:if test="@ToolSource">
        <I>Tool location:</I> <xsl:value-of select="@ToolSource"/><BR/>
      </xsl:if>
      <DIV CLASS="srh1">Command issued</DIV>
      <DIV CLASS="sr2"><xsl:value-of/></DIV>
      <xsl:if test="context()[not(end())]"><BR/></xsl:if>
    </DIV>
  </DIV>
</xsl:template>

--------

<!-- ESRI feature description -->
<xsl:template match="/metadata/spdoinfo/ptvctinf/esriterm[$any$ * != '']">
  <xsl:if test="context()[0]"> 
    <DIV STYLE="margin-left:0.4in" CLASS="pn">ESRI description</DIV>
  </xsl:if>
  <DIV CLASS="ph2" STYLE="margin-left:0.6in" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">
    <xsl:choose>
      <xsl:when test="context()[@Name != '']">
        <xsl:value-of select="@Name"/>
      </xsl:when>
      <xsl:otherwise>
        Feature class
      </xsl:otherwise>
    </xsl:choose>
    <DIV CLASS="pe2" STYLE="display:none">
      <xsl:for-each select="efeatyp[. != '']">
        <I>ESRI feature type: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="efeageom[. != '']">
        <I>Geometry type: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="featdesc[. != '']">
        <I>Feature description: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="esritopo[. != '']">
        <I>Topology: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="efeacnt[. != '']">
        <I>Feature count: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="spindex[. != '']">
        <I>Spatial Index: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="linrefer[. != '']">
        <I>Linear referencing: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="netwrole[. != '']">
        <I>Network role: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:if test="context()[($any$ XYRank | ZRank | topoWeight | validateEvents | partTopoRules)]"><BR/></xsl:if> 
      <xsl:for-each select="XYRank[. != '']">
        <I>XYRank: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="ZRank[. != '']">
        <I>ZRank: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="topoWeight[. != '']">
        <I>Topology weight: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="validateEvents[. != '']">
        <I>Events on validation: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="partTopoRules[. != '']">
        <I>Participates in topology rules: </I>
        <xsl:for-each select="topoRuleID[. != '']"><xsl:value-of /><xsl:if test="context()[not(end())]">, </xsl:if></xsl:for-each><BR/>
      </xsl:for-each>
      <xsl:if test="context()[not(end())]"><BR/></xsl:if>
    </DIV>
  </DIV>
</xsl:template>

--------

<!-- SDTS feature description -->
<xsl:template match="/metadata/spdoinfo/ptvctinf/sdtsterm[$any$ * != '']">
  <xsl:if test="context()[0]">
    <DIV>Feature class: SDTS feature type, feature count</DIV>
  </xsl:if>
  <DIV STYLE="margin-left:0.2in">
    <LI>
      <xsl:choose>
        <xsl:when test="context()[@Name != '']">
          <xsl:value-of select="@Name"/>: 
        </xsl:when>
        <xsl:otherwise>
          Feature class: 
        </xsl:otherwise>
      </xsl:choose>
      <xsl:value-of select="sdtstype"/><xsl:if test="context()[sdtstype != '' and ptvctcnt != '']">, </xsl:if>
      <xsl:value-of select="ptvctcnt"/>
    </LI>
  </DIV>
</xsl:template>

--------

<!-- VPF feature description -->
<xsl:template match="/metadata/spdoinfo/ptvctinf/vpfterm[(vpflevel != '') or ($any$ vpfinfo/* != '')]">
  <xsl:for-each select="vpflevel[. != '']">
    <DIV><I>Level of topology: </I><xsl:value-of /></DIV>
  </xsl:for-each>
  <xsl:for-each select="vpfinfo[$any$ * != '']">
    <xsl:if test="context()[0]">
      <DIV>Feature class: VPF feature type, feature count</DIV>
    </xsl:if>
    <DIV STYLE="margin-left:0.2in">
      <LI>
        <xsl:choose>
          <xsl:when test="context()[@Name != '']">
            <xsl:value-of select="@Name"/>: 
          </xsl:when>
          <xsl:otherwise>
            Feature class: 
          </xsl:otherwise>
        </xsl:choose>
        <xsl:value-of select="vpftype"/><xsl:if test="context()[vpftype != '' and ptvctcnt != '']">, </xsl:if>
        <xsl:value-of select="ptvctcnt"/>
      </LI>
    </DIV>
  </xsl:for-each>
</xsl:template>

--------

<!-- Geometric Network Information -->
<xsl:template match="/metadata/spdoinfo/netinfo[(nettype != '') or ($any$ connrule/* != '')]">
  <DIV CLASS="pn" STYLE="margin-left:0.2in">Geometric network information</DIV>
  <xsl:for-each select="nettype[. != '']">
    <DIV STYLE="margin-left:0.4in"><I>Network type: </I><xsl:value-of /></DIV><BR/>
  </xsl:for-each>
  <xsl:if test="context()[$any$ connrule/* != '']">
    <DIV CLASS="pn" STYLE="margin-left:0.4in">Connectivity rules 
        <DIV CLASS="pe2">In the list below, feature classes are listed followed by the subtype code to which the rule applies.</DIV><BR/>
        <xsl:for-each select="connrule[$any$ * != '']">
          <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)"><xsl:eval>formatIndex(childNumber(this), "1")</xsl:eval>. <xsl:choose>
              <xsl:when test="ruletype[. != '']">
                <xsl:value-of select="ruletype"/> rule 
              </xsl:when>
              <xsl:otherwise>
                Connectivity rule 
              </xsl:otherwise>
            </xsl:choose>
            <DIV CLASS="pe2" STYLE="display:none">
              <xsl:for-each select="rulehelp[. != '']">
                <I>Rule description: </I><xsl:value-of /><BR/>
              </xsl:for-each>
<!--              <xsl:for-each select="rulecat[. != '']">
                <I>Rule category: </I><xsl:value-of /><BR/>
              </xsl:for-each> -->
              <xsl:if test="context()[(rulefeid != '') or (rulefest != '')]">
                <I>From edge:</I> 
                <xsl:value-of select="rulefeid"/><xsl:if test="context()[rulefeid != '' and rulefest != '']">: </xsl:if>
                <xsl:value-of select="rulefest"/><BR/>
              </xsl:if>
              <xsl:if test="context()[(ruleteid != '') or (ruletest != '')]">
                <I>To edge:</I> 
                <xsl:value-of select="ruleteid"/><xsl:if test="context()[ruleteid != '' and ruletest != '']">: </xsl:if>
                <xsl:value-of select="ruletest"/><BR/>
              </xsl:if>
              <xsl:if test="context()[(ruleeid != '') or (ruleest != '') or 
                  (ruleemnc != '') or (ruleemxc != '')]">
                <I>Edge: </I> 
                <xsl:value-of select="ruleeid"/><xsl:if test="context()[ruleeid != '' and ruleest != '']">: </xsl:if>
                <xsl:value-of select="ruleest"/><BR/>
                <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Cardinality
                  <DIV CLASS="pe2" STYLE="display:none">
                    <xsl:for-each select="ruleemnc[. != '']">
                      <I>Minimum: </I><xsl:value-of /><BR/>
                    </xsl:for-each>
                    <xsl:for-each select="ruleemxc[. != '']">
                      <I>Maximum: </I><xsl:value-of /><BR/>
                    </xsl:for-each>
                  </DIV>
                 </DIV>
              </xsl:if>
              <xsl:if test="context()[(rulejid != '') or (rulejst != '') or 
                  (rulejmnc != '') or (rulejmxc != '')]">
                <I>Junction: </I> 
                <xsl:value-of select="rulejid"/><xsl:if test="context()[rulejid != '' and rulejst != '']">: </xsl:if>
                <xsl:value-of select="rulejst"/><BR/>
                <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Cardinality
                  <DIV CLASS="pe2" STYLE="display:none">
                    <xsl:for-each select="rulejmnc[. != '']">
                      <I>Minimum: </I><xsl:value-of /><BR/>
                    </xsl:for-each>
                    <xsl:for-each select="rulejmxc[. != '']">
                      <I>Maximum: </I><xsl:value-of /><BR/>
                    </xsl:for-each>
                  </DIV>
                </DIV>
              </xsl:if>
              <xsl:if test="context()[(ruledjid != '') or (ruledjst != '')]">
                <DIV>
                  <I>Default junction:</I>
                  <xsl:value-of select="ruledjid"/><xsl:if test="context()[ruledjid != '' and ruledjst != '']">: </xsl:if>
                  <xsl:value-of select="ruledjst"/>
                </DIV>
              </xsl:if>
              <xsl:if test="rulejunc[(junctid != '') or (junctst != '')]">
                <DIV CLASS="ph1" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Available junctions
                  <DIV CLASS="pe2" STYLE="display:none">
                    <xsl:for-each select="rulejunc[(junctid != '') or (junctst != '')]">
                      <LI>
                        <xsl:value-of select="junctid"/><xsl:if test="context()[junctid != '' and junctst != '']">: </xsl:if>
                        <xsl:value-of select="junctst"/>
                      </LI>
                    </xsl:for-each>
                  </DIV>
                </DIV>
              </xsl:if>
              <BR/>
            </DIV>
          </DIV>
        </xsl:for-each>
    </DIV>
  </xsl:if>
</xsl:template>

--------

<!-- Topology Information -->
<xsl:template match="/metadata/Esri/DataProperties/topoinfo[($any$ */* != '')]">
  <DIV CLASS="pn" STYLE="margin-left:0.2in">Topology information</DIV>
  <xsl:for-each select="topoProps[. != '']">
    <DIV STYLE="margin-left:0.4in">
      <xsl:for-each select="clusterTol[. != '']">
        <I>Cluster tolerance: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="maxErrors[. != '']">
        <I>Maximum number of errors: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="notTrusted[. != '']">
        <I>Nothing trusted: </I><xsl:value-of /><BR/>
      </xsl:for-each>
      <xsl:for-each select="trustedArea/trustedPolygon[. != '']">
        <I>Trusted area: </I><xsl:value-of /><BR/>
      </xsl:for-each>
    </DIV>
    <xsl:if test="context()[$any$ ../topoRule//* != '']"><BR/></xsl:if>
  </xsl:for-each>
  <xsl:if test="context()[$any$ topoRule//* != '']">
    <DIV CLASS="pn" STYLE="margin-left:0.4in">Topology rule information
        <xsl:for-each select="topoRule[$any$ .//* != '']">
          <DIV CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Topology rule #<xsl:value-of select="topoRuleID[. != '']"/>
            <DIV CLASS="pe2" STYLE="display:none">
              <xsl:for-each select="topoRuleName[. != '']">
                <I>Rule name: </I><xsl:value-of /><BR/>
              </xsl:for-each>
              <xsl:for-each select="rulehelp[. != '']">
                <I>Rule description: </I><xsl:value-of /><BR/>
              </xsl:for-each>
              <xsl:for-each select="topoRuleType[. != '']">
                <I>Rule type: </I><xsl:value-of /><BR/>
              </xsl:for-each>
              <xsl:for-each select="topoRuleOrigin[* != '']">
                <SPAN CLASS="pn">Rule origin:</SPAN> 
                <DIV STYLE="margin-left:0.2in">
                  <xsl:for-each select="fcname[. != '']">
                    <I>Feature class: </I><xsl:value-of /><BR/>
                  </xsl:for-each>
                  <xsl:for-each select="allOriginSubtypes[. != '']">
                    <I>Rule applies to all subtypes: </I><xsl:value-of /><BR/>
                  </xsl:for-each>
                  <xsl:if test="context()[(allOriginSubtypes = 'FALSE') and ((stname !='') or (stcode != ''))]">
                    <I>Subtype name and code: </I>
                    <xsl:value-of select="stname"/><xsl:if test="context()[stname != '' and stcode != '']">: </xsl:if>
                    <xsl:value-of select="stcode"/><BR/>
                  </xsl:if>
                </DIV>
              </xsl:for-each>
              <xsl:for-each select="topoRuleDest[* != '']">
                <SPAN CLASS="pn">Rule destination:</SPAN> 
                <DIV STYLE="margin-left:0.2in">
                  <xsl:for-each select="fcname[. != '']">
                    <I>Feature class: </I><xsl:value-of /><BR/>
                  </xsl:for-each>
                  <xsl:for-each select="allDestSubtypes[. != '']">
                    <I>Rule applies to all subtypes: </I><xsl:value-of /><BR/>
                  </xsl:for-each>
                  <xsl:if test="context()[(allDestSubtypes = 'FALSE') and ((stname !='') or (stcode != ''))]">
                    <I>Subtype name and code: </I>
                    <xsl:value-of select="stname"/><xsl:if test="context()[stname != '' and stcode != '']">: </xsl:if>
                    <xsl:value-of select="stcode"/><BR/>
                  </xsl:if>
                </DIV>
                <BR/>
              </xsl:for-each>
            </DIV>
          </DIV>
        </xsl:for-each>
    </DIV>
  </xsl:if>
</xsl:template>

--------

<!-- Terrain Information -->
<xsl:template match="/metadata/Esri/DataProperties/Terrain[($any$ totalPts != '')]">
  <DIV CLASS="pn" STYLE="margin-left:0.2in">Terrain information</DIV>
  <xsl:for-each select="totalPts[. != '']">
    <DIV STYLE="margin-left:0.4in"><I>Total number of points: </I>
      <xsl:value-of />
    </DIV>
  </xsl:for-each>
</xsl:template>

--------

<!-- Locator Information -->
<xsl:template match="/metadata/Esri/Locator[($any$ (Style | Properties/(Fallback | 
    FieldAliases | FileMat | FileSTN | IntFileMat | IntFileSTN)) != '')]">
  <xsl:for-each select="Style[. != '']">
    <DIV STYLE="margin-left:0.2in"><I>Address locator style: </I>
      <xsl:value-of />
    </DIV>
  </xsl:for-each>
  <xsl:if test="context()[$any$ Properties/FieldAliases != '']">
    <DIV STYLE="margin-left:0.2in" CLASS="pn">Input fields</DIV>
    <xsl:for-each select="Properties/FieldAliases[. != '']">
      <DIV STYLE="margin-left:0.4in">- <xsl:value-of /></DIV>
    </xsl:for-each>
  </xsl:if>
  <xsl:if test="context()[$any$ Properties/(FileMAT | FileSTN | 
      IntFileMAT | IntFileSTN) != '']">
    <DIV STYLE="margin-left:0.2in" CLASS="pn">Geocoding rule bases</DIV>
    <xsl:for-each select="Properties/FileMAT[. != '']">
      <DIV STYLE="margin-left:0.4in"><I>Match rules: </I><xsl:value-of /></DIV>
    </xsl:for-each>
    <xsl:for-each select="Properties/FileSTN[. != '']">
      <DIV STYLE="margin-left:0.4in"><I>Standardization rules: </I><xsl:value-of /></DIV>
    </xsl:for-each>
    <xsl:for-each select="Properties/IntFileMAT[. != '']">
      <DIV STYLE="margin-left:0.4in"><I>Intersection match rules: </I><xsl:value-of /></DIV>
    </xsl:for-each>
    <xsl:for-each select="Properties/IntFileSTN[. != '']">
      <DIV STYLE="margin-left:0.4in"><I>Intersection standardization rules: </I><xsl:value-of /></DIV>
    </xsl:for-each>
  </xsl:if>
  <xsl:for-each select="Properties/Fallback[. != '']">
    <DIV STYLE="margin-left:0.2in"><I>Fallback matching: </I>
      <xsl:value-of />
    </DIV>
  </xsl:for-each>
  </xsl:template>

--------

<!-- Raster Dataset Information -->
<xsl:template match="/metadata/spdoinfo/rastinfo[$any$ * != '']">
  <DIV CLASS="pn" STYLE="margin-left:0.2in">Raster dataset information</DIV>
  <xsl:for-each select="rastifor[. != '']">
    <DIV STYLE="margin-left:0.4in"><I>Raster format: </I><xsl:value-of /></DIV>
  </xsl:for-each>
  <xsl:for-each select="rasttype[. != '']">
    <DIV STYLE="margin-left:0.4in"><I>SDTS raster type: </I><xsl:value-of /></DIV>
  </xsl:for-each>
  <xsl:for-each select="rastband[. != '']">
    <DIV STYLE="margin-left:0.4in"><I>Number of raster bands: </I><xsl:value-of /></DIV>
  </xsl:for-each>
  <xsl:if test="context()[(rastorig != '') or (rastplyr != '') or (rastcmap != '') or 
      (rastcomp != '') or (rastdtyp != '')]">
    <DIV CLASS="ph2" STYLE="margin-left:0.4in" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Raster properties
      <DIV CLASS="pe2" STYLE="display:none">
        <xsl:for-each select="rastorig[. != '']">
          <DIV><I>Origin location: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="rastplyr[. != '']">
          <DIV><I>Has pyramids: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="rastcmap[. != '']">
          <DIV><I>Has colormap: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="rastcomp[. != '']">
          <DIV><I>Data compression type: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="rastdtyp[. != '']">
          <DIV><I>Display type: </I><xsl:value-of /></DIV>
        </xsl:for-each>
      </DIV>
    </DIV>
  </xsl:if>
  <xsl:if test="context()[(rastxsz != '') or (rastysz != '') or (rastbpp != '') or 
      (vrtcount != '') or (rowcount != '') or (colcount != '')]">
    <DIV CLASS="ph2" STYLE="margin-left:0.4in" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Cell information
      <DIV CLASS="pe2" STYLE="display:none">
        <xsl:for-each select="colcount[. != '']">
          <DIV><I>Number of cells on x-axis: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="rowcount[. != '']">
          <DIV><I>Number of cells on y-axis: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="vrtcount[. != '']">
          <DIV><I>Number of cells on z-axis: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="rastbpp[. != '']">
          <DIV><I>Number of bits per cell: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:if test="context()[(rastxsz != '') or (rastysz != '')]">
          <DIV CLASS="pn">Cell Size</DIV>
          <xsl:for-each select="rastxsz[. != '']">
            <DIV STYLE="margin-left:0.2in"><I>X distance: </I><xsl:value-of /></DIV>
          </xsl:for-each>
          <xsl:for-each select="rastysz[. != '']">
            <DIV STYLE="margin-left:0.2in"><I>Y distance: </I><xsl:value-of /></DIV>
          </xsl:for-each>
        </xsl:if>
      </DIV>
    </DIV>
  </xsl:if>
  <xsl:if test="context()[not(end())]"><BR/></xsl:if>
</xsl:template>


================================


<!-- ATTRIBUTES TAB -->

<!-- Entity type -->
<xsl:template match="/metadata/eainfo/detailed/enttyp[(enttypt != '') or (enttypc != '') or 
    (enttypd != '')]">
  <xsl:for-each select="enttypt[. != '']">
    <DIV STYLE="margin-left:0.2in"><I>Type of object: </I><xsl:value-of /></DIV>
  </xsl:for-each>
  <xsl:for-each select="enttypc[. != '']">
    <DIV STYLE="margin-left:0.2in"><I>Number of records: </I><xsl:value-of /></DIV>
  </xsl:for-each>
  <xsl:if test="context()[enttypd != '']">
    <DIV STYLE="margin-left:0.2in" CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Description
      <DIV CLASS="pe2" STYLE="display:none">
        <SPAN CLASS="lt"><xsl:value-of select="enttypd"/></SPAN><BR/>
        <xsl:for-each select="enttypds[. != '']">
          <SPAN CLASS="lt"><I>Source: </I><xsl:value-of /></SPAN><BR/>
        </xsl:for-each><BR/>
      </DIV>
    </DIV>
  </xsl:if>
</xsl:template>

--------

<!-- Relationship Information -->
<xsl:template match="/metadata/eainfo/detailed/relinfo[$any$ * != '']">
  <DIV STYLE="margin-left:0.2in" CLASS="pn">Relationship information</DIV>
  <xsl:for-each select="relcomp[. != '']">
    <DIV STYLE="margin-left:0.4in"><I>Type of relationship: </I>
      <xsl:choose>
        <xsl:when test="context()[. = 'TRUE']">Composite</xsl:when>
        <xsl:otherwise>Simple</xsl:otherwise>
      </xsl:choose>
    </DIV>
  </xsl:for-each>
  <xsl:for-each select="relcard[. != '']">
    <DIV STYLE="margin-left:0.4in"><I>Cardinality of the relationship: </I><xsl:value-of /></DIV>
  </xsl:for-each>
  <xsl:for-each select="relattr[. != '']">
    <DIV STYLE="margin-left:0.4in"><I>Has attributes: </I><xsl:value-of /></DIV>
  </xsl:for-each>
  <xsl:if test="context()[(otfcname != '') or (otfcpkey != '') or (otfcfkey != '')]">
    <DIV STYLE="margin-left:0.4in" CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Origin information
      <DIV CLASS="pe2" STYLE="display:none">
        <xsl:for-each select="otfcname[. != '']">
          <DIV><I>Origin name: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="otfcpkey[. != '']">
          <DIV><I>Primary key: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="otfcfkey[. != '']">
          <DIV><I>Foreign key: </I><xsl:value-of /></DIV>
        </xsl:for-each>
      </DIV>
    </DIV>
  </xsl:if>
  <xsl:if test="context()[(dtfcname != '') or (dtfcpkey != '') or (dtfcfkey != '')]">
    <DIV STYLE="margin-left:0.4in" CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Destination information
      <DIV CLASS="pe2" STYLE="display:none">
        <xsl:for-each select="dtfcname[. != '']">
          <DIV><I>Destination name: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="dtfcpkey[. != '']">
          <DIV><I>Primary key: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="dtfcfkey[. != '']">
          <DIV><I>Foreign key: </I><xsl:value-of /></DIV>
        </xsl:for-each>
      </DIV>
    </DIV>
  </xsl:if>
  <xsl:if test="context()[(relnodir != '') or (relflab != '') or (relblab != '')]">
    <DIV STYLE="margin-left:0.4in" CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Notification information
      <DIV CLASS="pe2" STYLE="display:none">
        <xsl:for-each select="relnodir[. != '']">
          <DIV><I>Notification direction: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="relflab[. != '']">
          <DIV><I>Forward label: </I><xsl:value-of /></DIV>
        </xsl:for-each>
        <xsl:for-each select="relblab[. != '']">
          <DIV><I>Backward label: </I><xsl:value-of /></DIV>
        </xsl:for-each>
      </DIV>
    </DIV>
  </xsl:if>
</xsl:template>

--------

<!-- Attribute Information -->
<xsl:template match="/metadata/eainfo/detailed/attr[(attrlabl != '') or (attalias != '') or 
    (attrtype != '') or (attwidth != '') or (atprecis != '') or (atoutwid != '') or 
    (atnumdec != '') or (attscale != '') or (attrdef != '')]">
  <DIV STYLE="margin-left:0.2in" CLASS="pn">
    <xsl:if test="context()[0]">Attributes</xsl:if>
    <xsl:choose> 
      <xsl:when test="context()[(attalias != '') or (attrtype != '') or (attwidth != '') or 
          (atprecis != '') or (atoutwid != '') or (atnumdec != '') or (attscale != '') or 
          (attrdef != '')]">
        <DIV STYLE="margin-left:0.25in" CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">
          <xsl:choose>
            <xsl:when test="context()[attrlabl != '']">
              <xsl:value-of select="attrlabl"/>
            </xsl:when>
            <xsl:otherwise>Attribute</xsl:otherwise>
          </xsl:choose>
          <DIV CLASS="pe2" STYLE="display:none">
            <xsl:for-each select="attalias[. != '']">
              <I>Alias: </I><xsl:value-of /><BR/>
            </xsl:for-each>
            <xsl:for-each select="attrtype[. != '']">
              <I>Data type: </I><xsl:value-of /><BR/>
            </xsl:for-each>
            <xsl:for-each select="attwidth[. != '']">
              <I>Width: </I><xsl:value-of /><BR/>
            </xsl:for-each>
            <xsl:for-each select="atoutwid[. != '']">
              <I>Output width: </I><xsl:value-of /><BR/>
            </xsl:for-each>
            <xsl:for-each select="atnumdec[. != '']">
              <I>Number of decimals: </I><xsl:value-of /><BR/>
            </xsl:for-each>
            <xsl:for-each select="atprecis[. != '']">
              <I>Precision: </I><xsl:value-of /><BR/>
            </xsl:for-each>
            <xsl:for-each select="attscale[. != '']">
              <I>Scale: </I><xsl:value-of /><BR/>
            </xsl:for-each>
            <xsl:for-each select="attrdef[. != '']">
              <!--<SPAN CLASS="lt"><I>Definition: </I><xsl:value-of /></SPAN><BR/>-->
              <SPAN CLASS="lt"><I>Definition: </I><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN>
              <SCRIPT>fix(original)</SCRIPT>      
            </xsl:for-each>
            <xsl:for-each select="attrdefs[. != '']">
              <!--<SPAN CLASS="lt"><I>Definition Source: </I><xsl:value-of /></SPAN><BR/>-->
              <SPAN CLASS="lt"><I>Definition Source: </I><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN>
              <SCRIPT>fix(original)</SCRIPT>      
            </xsl:for-each><BR/>
          </DIV>
        </DIV>
      </xsl:when>
      <xsl:otherwise>
        <xsl:if test="context()[attrlabl != '']">
          <DIV STYLE="margin-left:0.25in" CLASS="pe2"><xsl:value-of select="attrlabl"/></DIV>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>
  </DIV>
</xsl:template>

--------

<!-- Subtype Information -->
<xsl:template match="/metadata/eainfo/detailed/subtype[(stname != '') or (stcode != '') or 
    ($any$ stfield/(stfldnm | stflddv) != '') or ($any$ stfield/stflddd/* != '')]">
  <xsl:if test="context()[0]">
    <DIV STYLE="margin-left:0.2in" CLASS="pn">Subtype Information</DIV>
    <DIV STYLE="margin-left:0.4in" CLASS="pe2">In the following list the subtype code is followed by the subtype name.</DIV>
    <BR/>
  </xsl:if>
  <DIV STYLE="margin-left:0.4in" CLASS="pn"><xsl:value-of select="stcode"/>: <xsl:value-of select="stname"/></DIV>
  <xsl:if test="stfield[$any$ * != '']">
    <DIV STYLE="margin-left:0.7in" CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">Attributes
      <DIV STYLE="display:none">
      <xsl:for-each select="stfield[$any$ * != '']">
      <xsl:choose> 
        <xsl:when test="context()[(stflddv != '') or ($any$ stflddd/* != '')]">
          <DIV STYLE="margin-left:0.25in" CLASS="ph2" onmouseover="doHilite()" onmouseout="doHilite()" onclick="hideShowGroup(this)">
            <xsl:choose>
              <xsl:when test="context()[stfldnm != '']">
                <xsl:value-of select="stfldnm"/>
              </xsl:when>
              <xsl:otherwise>Subtype field</xsl:otherwise>
            </xsl:choose>
            <DIV CLASS="pe2" STYLE="display:none">
              <xsl:for-each select="stflddv[. != '']">
                <I>Default value: </I><xsl:value-of /><BR/>
              </xsl:for-each>
              <xsl:for-each select="stflddd[$any$ * != '']">
                <DIV><SPAN CLASS="pn">Domain: </SPAN><xsl:value-of select="domname"/></DIV>
                <DIV STYLE="margin-left:0.2in">
                  <xsl:for-each select="domdesc[. != '']">
                    <I>Description: </I><xsl:value-of /><BR/>
                  </xsl:for-each>
                  <xsl:for-each select="domfldtp[. != '']">
                    <I>Field type: </I><xsl:value-of /><BR/>
                  </xsl:for-each>
                  <xsl:for-each select="domtype[. != '']">
                    <I>Domain type: </I><xsl:value-of /><BR/>
                  </xsl:for-each>
                  <xsl:for-each select="mrgtype[. != '']">
                    <I>Merge rule: </I><xsl:value-of /><BR/>
                  </xsl:for-each>
                  <xsl:for-each select="splttype[. != '']">
                    <I>Split rule: </I><xsl:value-of /><BR/>
                  </xsl:for-each>
                  <xsl:for-each select="domowner[. != '']">
                    <I>Domain owner: </I><xsl:value-of /><BR/>
                  </xsl:for-each>
                </DIV>
              </xsl:for-each>
              <xsl:if test="context()[not(end())]"><BR/></xsl:if>
            </DIV>
          </DIV>
        </xsl:when>
        <xsl:otherwise>
          <xsl:if test="context()[stfldnm != '']">
            <DIV STYLE="margin-left:0.25in" CLASS="pe2"><xsl:value-of select="stfldnm"/></DIV>
          </xsl:if>
        </xsl:otherwise>
      </xsl:choose> 
      </xsl:for-each>
      <xsl:if test="context()[not(end())]"><BR/></xsl:if>
      </DIV>
    </DIV>
  </xsl:if>
</xsl:template>

--------

<!-- Overview info -->
<xsl:template match="/metadata/eainfo/overview[(dsoverv != '') or (eaover != '') or ($any$ eadetcit != '')]">
 <xsl:for-each select="dsoverv[. != '']">
    <DIV CLASS="srh1">Overview Description</DIV>
    <DIV STYLE="margin-left:0.2in">
      <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN>
      <SCRIPT>fix(original)</SCRIPT><BR/>     
    </DIV>
  </xsl:for-each>
  <xsl:for-each select="eaover[. != '']">
    <DIV CLASS="srh1">Overview</DIV>
    <DIV STYLE="margin-left:0.2in">
      <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN>
      <SCRIPT>fix(original)</SCRIPT><BR/>     
    </DIV>
  </xsl:for-each>
  <xsl:for-each select="eadetcit[. != '']">
    <xsl:if test="context()[0]"><DIV CLASS="srh1">Overview citation</DIV></xsl:if>
    <DIV STYLE="margin-left:0.2in">
      <SPAN CLASS="lt"><PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE></SPAN><BR/>
      <SCRIPT>fix(original)</SCRIPT>      
    </DIV>
  </xsl:for-each>
</xsl:template>


================================


<!-- SUPPORTING TEMPLATES -->

<!-- Time Period Information -->

<!-- Single or Multiple Date/Time -->
<xsl:template match="timeinfo//sngdate[(caldate != '') or (time != '')]">
  <DIV><I>Date and time: </I>
    <xsl:choose>
      <xsl:when test="context()[caldate = 'REQUIRED: The year (and optionally month, or month and day) for which the data set corresponds to the ground.']">
        <SPAN STYLE="color:#999999"><xsl:value-of select="caldate"/></SPAN>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="context()[caldate = 'The year (and optionally month, or month and day) for which the data set corresponds to the ground.  REQUIRED.']">
            <SPAN STYLE="color:#999999"><xsl:value-of select="caldate"/></SPAN>
          </xsl:when>
          <xsl:otherwise><xsl:value-of select="caldate"/></xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:if test="context()[time != '']"> at time <xsl:value-of select="time"/></xsl:if>
  </DIV>
</xsl:template>

<!-- Range of Dates/Times -->
<xsl:template match="timeinfo/rngdates[$any$ * != '']">
  <DIV><I>Beginning date and time: </I>
    <xsl:value-of select="begdate"/>
    <xsl:if test="context()[begtime != '']"> at time <xsl:value-of select="begtime"/></xsl:if>
  </DIV>
  <DIV><I>Ending date and time: </I>
    <xsl:value-of select="enddate"/>
    <xsl:if test="context()[endtime != '']"> at time <xsl:value-of select="endtime"/></xsl:if>
  </DIV>
</xsl:template>

--------

<!-- Contact Information -->
<xsl:template match="cntinfo[(cntvoice != '') or (cntfax != '') or (cntemail != '') or 
    (hours != '') or (cntinst != '') or (cntaddr/* != '') or (*/cntper != '') or 
    (*/cntorg != '')]">
  <xsl:for-each select="*/cntper[. != '']">
    <xsl:choose>
      <xsl:when test="context()[. = 'REQUIRED: The person responsible for the metadata information.']">
        <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="context()[. = 'The person responsible for the metadata information.  REQUIRED.']">
            <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
          </xsl:when>
          <xsl:otherwise><xsl:value-of /><BR/></xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:for-each>
  <xsl:for-each select="*/cntorg[. != '']">
    <xsl:choose>
      <xsl:when test="context()[. = 'REQUIRED: The organization responsible for the metadata information.']">
        <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="context()[. = 'The organization responsible for the metadata information.  REQUIRED.']">
            <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
          </xsl:when>
          <xsl:otherwise><xsl:value-of /><BR/></xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:for-each>
  <xsl:for-each select="cntaddr[($any$ address != '') or (city != '') or (state != '') or (postal != '') or (country != '')]">
    <xsl:choose>
      <xsl:when test="addrtype[. = 'REQUIRED: The mailing and/or physical address for the organization or individual.']">
        <SPAN STYLE="color:#999999"><I><xsl:value-of select="addrtype"/>:</I></SPAN><BR/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="addrtype[. = 'The mailing and/or physical address for the organization or individual.  REQUIRED.']">
            <SPAN STYLE="color:#999999"><I><xsl:value-of select="addrtype"/>:</I></SPAN><BR/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="addrtype[. != '']"><I><xsl:value-of select="addrtype"/>:</I><BR/></xsl:when>
              <xsl:otherwise><I>Address</I><BR/></xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:if test="context()[((address != '') or (city != '') or (state != '') or 
        (postal != '') or (country != ''))]">
      <DIV STYLE="margin-left:0.3in">
        <xsl:for-each select="address[. != '']">
          <DIV CLASS="lt">
            <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
            <SCRIPT>fix(original)</SCRIPT>
          </DIV>      
        </xsl:for-each>
        <xsl:if test="context()[((city != '') or (state != '') or (postal != ''))]">
          <DIV>
            <xsl:for-each select="city[. != '']">
              <xsl:choose>
                <xsl:when test="context()[. = 'REQUIRED: The city of the address.']">
                  <SPAN STYLE="color:#999999"><xsl:value-of /><xsl:if test="context()[../state != '']">, </xsl:if></SPAN></xsl:when>
                <xsl:otherwise>
                  <xsl:choose>
                    <xsl:when test="context()[. = 'The city of the address.  REQUIRED.']">
                      <SPAN STYLE="color:#999999"><xsl:value-of /><xsl:if test="context()[../state != '']">, </xsl:if></SPAN></xsl:when>
                    <xsl:otherwise><xsl:value-of /><xsl:if test="context()[../state != '']">, </xsl:if></xsl:otherwise>
                  </xsl:choose>
                </xsl:otherwise>
              </xsl:choose></xsl:for-each><xsl:for-each select="state[. != '']">
              <xsl:choose>
                <xsl:when test="context()[. = 'REQUIRED: The state or province of the address.']">
                  <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN></xsl:when>
                <xsl:otherwise>
                  <xsl:choose>
                    <xsl:when test="context()[. = 'The state or province of the address.  REQUIRED.']">
                      <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN></xsl:when>
                    <xsl:otherwise><xsl:value-of /></xsl:otherwise>
                  </xsl:choose>
                </xsl:otherwise>
              </xsl:choose></xsl:for-each><xsl:if test="context()[((city != '') or (state != '')) and (postal != '')]" xml:space="preserve"> </xsl:if>
              <xsl:for-each select="postal[. != '']">
              <xsl:choose>
                <xsl:when test="context()[. = 'REQUIRED: The ZIP or other postal code of the address.']">
                  <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN></xsl:when>
                <xsl:otherwise>
                  <xsl:choose>
                    <xsl:when test="context()[. = 'The ZIP or other postal code of the address.  REQUIRED.']">
                      <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN></xsl:when>
                    <xsl:otherwise><xsl:value-of /></xsl:otherwise>
                  </xsl:choose>
                </xsl:otherwise>
              </xsl:choose></xsl:for-each>
          </DIV>
        </xsl:if>
        <xsl:for-each select="country[. != '']"><DIV><xsl:value-of/></DIV></xsl:for-each>
        <xsl:if test="context()[not(end())]">
          <BR/>
        </xsl:if>
      </DIV>
    </xsl:if>
  </xsl:for-each>
  <xsl:if test="context()[(($any$ cntaddr/address != '') or (cntaddr/city != '') or 
      (cntaddr/state != '') or (cntaddr/postal != '') or (cntaddr/country != '')) 
      and ((cntvoice != '') or (cntfax != '') or (cntemail != ''))]">
    <BR/>
  </xsl:if>
  <xsl:for-each select="cntvoice[. != '']">
    <xsl:choose>
      <xsl:when test="context()[. = 'REQUIRED: The telephone number by which individuals can speak to the organization or individual.']">
        <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="context()[. = 'The telephone number by which individuals can speak to the organization or individual.  REQUIRED.']">
            <SPAN STYLE="color:#999999"><xsl:value-of /></SPAN><BR/>
          </xsl:when>
          <xsl:otherwise><xsl:value-of /> (voice)<BR/></xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:for-each>
  <xsl:for-each select="cntfax[. != '']"><xsl:value-of/> (fax)<BR/></xsl:for-each>
  <xsl:for-each select="cntemail[. != '']"><xsl:value-of/><BR/></xsl:for-each>
  <xsl:if test="context()[(($any$ cntaddr/address != '') or (cntaddr/city != '') or 
      (cntaddr/state != '') or (cntaddr/postal != '') or (cntaddr/country != '') or 
      (cntvoice != '') or (cntfax != '') or (cntemail != '')) 
      and ((hours != '') or (cntinst != ''))]">
    <BR/>
  </xsl:if>
  <xsl:for-each select="hours[. != '']"><DIV><I>Hours of service:</I> <xsl:value-of/></DIV></xsl:for-each>
  <xsl:for-each select="cntinst[. != '']">
    <DIV><I>Contact Instructions:</I></DIV>
    <DIV STYLE="margin-left:0.3in">
      <PRE ID="original"><xsl:eval>this.text</xsl:eval></PRE>
      <SCRIPT>fix(original)</SCRIPT>
    </DIV>      
  </xsl:for-each>
  <BR/>
</xsl:template>

--------

<!-- Horizontal Coordinate Systems -->

<!-- Geographic Coordinate System -->
<xsl:template match="/metadata/spref/horizsys/geograph[$any$ * != '']">
  <DIV CLASS="srh1">Geographic Coordinate System</DIV>
  <xsl:for-each select="latres[. != '']">
    <DIV CLASS="sr2"><I>Latitude Resolution: </I><xsl:value-of/></DIV>
  </xsl:for-each>
  <xsl:for-each select="longres[. != '']">
    <DIV CLASS="sr2"><I>Longitude Resolution: </I><xsl:value-of/></DIV>
  </xsl:for-each>
  <xsl:for-each select="geogunit[. != '']">
    <DIV CLASS="sr2"><I>Geographic Coordinate Units: </I><xsl:value-of/></DIV>
  </xsl:for-each>
</xsl:template>

<!-- Planar Coordinate System -->
<xsl:template match="/metadata/spref/horizsys/planar[$any$ .//* != '']">
  <xsl:for-each select="mapproj">
    <xsl:for-each select="mapprojn[. != '']">
      <DIV CLASS="sr1"><SPAN CLASS="pn">Map Projection Name: </SPAN><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:if test="context()[$any$ .//* != '']">
      <DIV CLASS="sr2"><xsl:apply-templates select="*"/></DIV>
    </xsl:if>
    <xsl:if test="context()[not(end())]"><BR/></xsl:if>
  </xsl:for-each>

  <xsl:for-each select="gridsys">
    <xsl:for-each select="gridsysn[. != '']">
      <DIV CLASS="sr1"><SPAN CLASS="pn">Grid Coordinate System Name: </SPAN><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="utm">
      <xsl:for-each select="utmzone[. != '']">
        <DIV CLASS="sr2"><I>UTM Zone Number: </I><xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="transmer[$any$ * != '']">
        <DIV CLASS="srh2">Transverse Mercator Projection</DIV>
      </xsl:for-each>
      <DIV CLASS="sr3"><xsl:apply-templates select="transmer"/></DIV>
    </xsl:for-each>
    <xsl:for-each select="ups">
      <xsl:for-each select="upszone[. != '']">
        <DIV CLASS="sr2"><I>UPS Zone Identifier: </I><xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="polarst[$any$ * != '']">
        <DIV CLASS="srh2">Polar Stereographic Projection</DIV>
      </xsl:for-each>
      <DIV CLASS="sr3"><xsl:apply-templates select="polarst"/></DIV>
    </xsl:for-each>
    <xsl:for-each select="spcs">
      <xsl:for-each select="spcszone[. != '']">
        <DIV CLASS="sr2"><I>SPCS Zone Identifier: </I><xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="lambertc[$any$ * != '']">
        <DIV CLASS="srh2">Lambert Conformal Conic Projection</DIV>
      </xsl:for-each>
      <xsl:for-each select="transmer[$any$ * != '']">
        <DIV CLASS="srh2">Transverse Mercator Projection</DIV>
      </xsl:for-each>
      <xsl:for-each select="obqmerc[$any$ * != '']">
        <DIV CLASS="srh2">Oblique Mercator Projection</DIV>
      </xsl:for-each>
      <xsl:for-each select="polycon[$any$ * != '']">
        <DIV CLASS="srh2">Polyconic Projection</DIV>
      </xsl:for-each>
      <DIV CLASS="sr3"><xsl:apply-templates select="*"/></DIV>
    </xsl:for-each>
    <xsl:for-each select="arcsys">
      <xsl:for-each select="arczone[. != '']">
        <DIV CLASS="sr2"><I>ARC System Zone Identifier: </I><xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="equirect[$any$ * != '']">
        <DIV CLASS="srh2">Equirectangular Projection</DIV>
      </xsl:for-each>
      <xsl:for-each select="azimequi[$any$ * != '']">
        <DIV CLASS="srh2">Azimuthal Equidistant Projection</DIV>
      </xsl:for-each>
      <DIV CLASS="sr3"><xsl:apply-templates select="*"/></DIV>
    </xsl:for-each>
    <xsl:for-each select="othergrd[. != '']">
      <DIV CLASS="srh2">Other Grid System's Definition</DIV>
      <DIV CLASS="sr3"><xsl:value-of/></DIV>
    </xsl:for-each>
  </xsl:for-each>

  <xsl:for-each select="localp">
    <xsl:if test="context()[$any$ * != '']">
      <DIV CLASS="srh1">Local Planar Coordinate System</DIV>
    </xsl:if>
    <xsl:for-each select="localpd[. != '']">
      <DIV CLASS="sr2"><I>Description: </I><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="localpgi[. != '']">
      <DIV CLASS="srh2">Georeference Information</DIV>
      <DIV CLASS="sr3"><SPAN CLASS="lt"><xsl:value-of/></SPAN></DIV>
    </xsl:for-each>
  </xsl:for-each>

  <xsl:if test="context()[($any$ (mapproj//* | gridsys//* | localp/*) != '') and 
      ($any$ planci//* != '')]">
    <BR/>
  </xsl:if>

  <xsl:for-each select="planci">
    <DIV CLASS="sr1"><SPAN CLASS="pn">Planar Coordinate Information</SPAN></DIV>
    <xsl:for-each select="plandu[. != '']">
      <DIV CLASS="sr2"><I>Planar Distance Units: </I><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="plance[. != '']">
      <DIV CLASS="sr2"><I>Coordinate Encoding Method: </I><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="coordrep">
      <xsl:if test="context()[$any$ * != '']">
        <DIV CLASS="srh2">Coordinate Representation</DIV>
      </xsl:if>
      <xsl:for-each select="absres[. != '']">
        <DIV CLASS="sr3"><I>Abscissa Resolution: </I><xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="ordres[. != '']">
        <DIV CLASS="sr3"><I>Ordinate Resolution: </I><xsl:value-of/></DIV>
      </xsl:for-each>
    </xsl:for-each>
    <xsl:for-each select="distbrep">
      <xsl:if test="context()[$any$ * != '']">
        <DIV CLASS="srh2">Distance and Bearing Representation</DIV>
      </xsl:if>
      <xsl:for-each select="distres[. != '']">
        <DIV CLASS="sr3"><I>Distance Resolution: </I><xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="bearres[. != '']">
        <DIV CLASS="sr3"><I>Bearing Resolution: </I><xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="bearunit[. != '']">
        <DIV CLASS="sr3"><I>Bearing Units: </I><xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="bearrefd[. != '']">
        <DIV CLASS="sr3"><I>Bearing Reference Direction: </I><xsl:value-of/></DIV>
      </xsl:for-each>
      <xsl:for-each select="bearrefm[. != '']">
        <DIV CLASS="sr3"><I>Bearing Reference Meridian: </I><xsl:value-of/></DIV>
      </xsl:for-each>
    </xsl:for-each>
  </xsl:for-each>

  <xsl:if test="context()[not(end())]">
    <BR/>
  </xsl:if>
</xsl:template>

<!-- Local Coordinate System -->
<xsl:template match="/metadata/spref/horizsys/local[$any$ * != '']">
  <DIV CLASS="srh1">Local Coordinate System</DIV>
  <xsl:for-each select="localdes[. != '']">
    <DIV CLASS="sr2"><I>Description: </I><xsl:value-of/></DIV>
  </xsl:for-each>
  <xsl:for-each select="localgeo[. != '']">
    <DIV CLASS="srh2">Georeference Information</DIV>
    <DIV CLASS="sr3"><SPAN CLASS="lt"><xsl:value-of/></SPAN></DIV>
  </xsl:for-each>
</xsl:template>

<!-- Geodetic Model -->
<xsl:template match="/metadata/spref/horizsys/geodetic[$any$ * != '']">
  <DIV CLASS="srh1">Geodetic Model</DIV>
  <xsl:for-each select="horizdn[. != '']">
    <DIV CLASS="sr2"><I>Horizontal Datum Name: </I><xsl:value-of/></DIV>
  </xsl:for-each>
  <xsl:for-each select="ellips[. != '']">
    <DIV CLASS="sr2"><I>Ellipsoid Name: </I><xsl:value-of/></DIV>
  </xsl:for-each>
  <xsl:for-each select="semiaxis[. != '']">
    <DIV CLASS="sr2"><I>Semi-major Axis: </I><xsl:value-of/></DIV>
  </xsl:for-each>
  <xsl:for-each select="denflat[. != '']">
    <DIV CLASS="sr2"><I>Denominator of Flattening Ratio: </I><xsl:value-of/></DIV>
  </xsl:for-each>
</xsl:template>

--------

<!-- Map Projections -->
<!-- Projections explicitly supported in the FGDC standard -->
<xsl:template match="albers | azimequi | equicon | equirect | gnomonic | gvnsp | lamberta | 
    lambertc | mercator | miller | modsak | obqmerc | orthogr | polarst | polycon | robinson | 
    sinusoid | spaceobq | stereo | transmer | vdgrin">
  <xsl:apply-templates select="*"/>
</xsl:template>

<!-- Projections defined in the 8.0 ESRI Profile -->
<xsl:template match="behrmann | bonne | cassini | eckert1 | eckert2 | eckert3 | eckert4 | 
    eckert5 | eckert6 | gallster | loximuth | mollweid | quartic | winkel1 | winkel2">
  <xsl:apply-templates select="*"/>
</xsl:template>

<!-- For projections not explicitly supported, FGDC standard places parameters in mapprojp; used by Catalog at 8.1 -->
<xsl:template match="mapprojp">
  <xsl:apply-templates select="*"/>
</xsl:template>

--------

<!-- Map Projection Parameters -->
<xsl:template match="stdparll[. != '']">
  <I>Standard Parallel: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="longcm[. != '']">
  <I>Longitude of Central Meridian: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="latprjo[. != '']">
  <I>Latitude of Projection Origin: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="feast[. != '']">
  <I>False Easting: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="fnorth[. != '']">
  <I>False Northing: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="sfequat[. != '']">
  <I>Scale Factor at Equator: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="heightpt[. != '']">
  <I>Height of Perspective Point Above Surface: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="longpc[. != '']">
  <I>Longitude of Projection Center: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="latprjc[. != '']">
  <I>Latitude of Projection Center: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="sfctrlin[. != '']">
  <I>Scale_Factor at Center Line: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="obqlazim[. != '']">
  <I>Oblique Line Azimuth: </I><BR/>
    <xsl:for-each select="azimangl[. != '']">
      <DD><I>Azimuthal Angle: </I><xsl:value-of/></DD><BR/>
    </xsl:for-each>
    <xsl:for-each select="azimptl[. != '']">
      <DD><I>Azimuthal Measure Point Longitude: </I><xsl:value-of/></DD><BR/>
    </xsl:for-each>
</xsl:template>

<xsl:template match="obqlpt[. != '']">
  <I>Oblique Line Point: </I><BR/>
    <xsl:for-each select="obqllat[. != '']">
      <DD><I>Oblique Line Latitude: </I><xsl:value-of/></DD><BR/>
    </xsl:for-each>
    <xsl:for-each select="obqllong[. != '']">
      <DD><I>Oblique Line Longitude: </I><xsl:value-of/></DD><BR/>
    </xsl:for-each>
</xsl:template>

<xsl:template match="svlong[. != '']">
  <I>Straight Vertical Longitude from Pole: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="sfprjorg[. != '']">
  <I>Scale Factor at Projection Origin: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="landsat[. != '']">
  <I>Landsat Number: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="pathnum[. != '']">
  <I>Path Number: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="sfctrmer[. != '']">
  <I>Scale Factor at Central Meridian: </I><xsl:value-of/><BR/>
</xsl:template>

<xsl:template match="otherprj[. != '']">
  <I>Other Projection's Definition: </I><xsl:value-of/><BR/>
</xsl:template>

--------

<!-- Vertical Coordinate Systems -->
<xsl:template match="vertdef">
  <xsl:for-each select="altsys">
    <xsl:if test="context()[$any$ * != '']">
      <DIV CLASS="srh1">Altitude System Definition</DIV>
    </xsl:if>
    <xsl:for-each select="altdatum[. != '']">
      <DIV CLASS="sr2"><I>Datum Name: </I><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="altres[. != '']">
      <DIV CLASS="sr2"><I>Resolution: </I><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="altunits[. != '']">
      <DIV CLASS="sr2"><I>Distance Units: </I><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="altenc[. != '']">
      <DIV CLASS="sr2"><I>Encoding Method: </I><xsl:value-of/></DIV>
    </xsl:for-each>
  </xsl:for-each>

  <xsl:if test="context()[($any$ altsys/* != '') and ($any$ depthsys/* != '')]">
    <BR/>
  </xsl:if>

  <xsl:for-each select="depthsys">
    <xsl:if test="context()[$any$ * != '']">
      <DIV CLASS="srh1">Depth System Definition</DIV>
    </xsl:if>
    <xsl:for-each select="depthdn[. != '']">
      <DIV CLASS="sr2"><I>Datum Name: </I><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="depthres[. != '']">
      <DIV CLASS="sr2"><I>Resolution: </I><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="depthdu[. != '']">
      <DIV CLASS="sr2"><I>Distance Units: </I><xsl:value-of/></DIV>
    </xsl:for-each>
    <xsl:for-each select="depthem[. != '']">
      <DIV CLASS="sr2"><I>Encoding Method: </I><xsl:value-of/></DIV>
    </xsl:for-each>
  </xsl:for-each>
</xsl:template>


================================


<!-- SEARCH RESULTS TEMPLATE -->

<xsl:template match="SearchResults">
  <xsl:for-each select="QueryName">
    <DIV CLASS="name"><xsl:value-of/></DIV>
  </xsl:for-each>
  <DIV CLASS="sub">Search Results</DIV>
  <BR/>

  <DIV CLASS="search">
    <DIV>
      <xsl:for-each select="DatasetName">
        This search looks for data named "<xsl:value-of/>". 
      </xsl:for-each>
      <xsl:choose>
        <xsl:when test="context()[DatasetType]">
          <xsl:for-each select="DatasetType">
            <xsl:if test="context()[0]">
              It retrieves the following types of data:
            </xsl:if>
            <DIV STYLE="margin-left:0.3in">
              <LI><xsl:value-of select="@Description"/></LI>
            </DIV>
          </xsl:for-each>
        </xsl:when>
        <xsl:otherwise>
          It retrieves all types of data.
        </xsl:otherwise>
      </xsl:choose>
    </DIV>
    <BR/>

    <xsl:if test="context()[Envelope]">
      <DIV CLASS="head">Geographic criteria</DIV>
      <DIV>
        Data
        <xsl:if test="EnvelopeOperator[. = '0']">located within</xsl:if>
        <xsl:if test="EnvelopeOperator[. = '1']">that overlaps</xsl:if>
        the following area will be retrieved by this search: 
      </DIV>
      <DIV STYLE="margin-left:0.3in">
        <xsl:for-each select="Envelope/XMin"><DIV>Minimum X: <xsl:value-of/></DIV></xsl:for-each>
        <xsl:for-each select="Envelope/YMin"><DIV>Minimum Y: <xsl:value-of/></DIV></xsl:for-each>
        <xsl:for-each select="Envelope/XMax"><DIV>Maximum X: <xsl:value-of/></DIV></xsl:for-each>
        <xsl:for-each select="Envelope/YMax"><DIV>Maximum Y: <xsl:value-of/></DIV></xsl:for-each>
      </DIV>
      <BR/>
    </xsl:if>

    <xsl:if test="context()[DateType]">
      <DIV CLASS="head">Temporal criteria</DIV>
      <DIV>
        Data
        <xsl:if test="DateType[. = '1']">describing the time period </xsl:if>
        <xsl:if test="DateType[. = '2']">published </xsl:if>
        <xsl:if test="DateType[. = '3']">whose metadata was updated </xsl:if>
        <xsl:if test="DateType[. = '4']">modified </xsl:if>
        <xsl:if test="DateOperator[. = '0']">during the previous <xsl:value-of select="Date1"/> days </xsl:if>
        <xsl:if test="DateOperator[. > '0']">
          <xsl:if test="DateOperator[. = '1']">before </xsl:if>
          <xsl:if test="DateOperator[. = '2']">before or during </xsl:if>
          <xsl:if test="DateOperator[. = '3']">during </xsl:if>
          <xsl:if test="DateOperator[. = '4']">equal to </xsl:if>
          <xsl:if test="DateOperator[. = '5']">after or during </xsl:if>
          <xsl:if test="DateOperator[. = '6']">after </xsl:if>
          <xsl:value-of select="Date1"/>
          <xsl:if test="context()[Date2]">through <xsl:value-of select="Date2"/> </xsl:if>
        </xsl:if>
        will be retrieved by this search. 
      </DIV>
      <BR/>
    </xsl:if>

    <xsl:for-each select="FieldQuery">
      <xsl:if test="context()[0]">
        <DIV CLASS="head">Keyword criteria</DIV>
        <DIV>
          Data whose metadata satisfies the following criteria, 
          which are <xsl:if test="../IsCaseSensitive[. = '0']">not </xsl:if>case-sensitive,
          will be retrieved by this search:
        </DIV>
      </xsl:if>
      <DIV>
        <LI STYLE="margin-left:0.3in">
          <xsl:if test="FieldType[. = '0']">Full text </xsl:if>
          <xsl:if test="FieldType[. = '1']">Title </xsl:if>
          <xsl:if test="FieldType[. = '2']">Edition </xsl:if>
          <xsl:if test="FieldType[. = '3']">Originator </xsl:if>
          <xsl:if test="FieldType[. = '4']">Source agency </xsl:if>
          <xsl:if test="FieldType[. = '5']">Abstract </xsl:if>
          <xsl:if test="FieldType[. = '6']">Purpose </xsl:if>
          <xsl:if test="FieldType[. = '7']">Geospatial data presentation form </xsl:if>
          <xsl:if test="FieldType[. = '8']">Theme keyword </xsl:if>
          <xsl:if test="FieldType[. = '9']">Place keyword </xsl:if>
          <xsl:if test="FieldType[. = '10']">Stratum keyword </xsl:if>
          <xsl:if test="FieldType[. = '11']">Temporal keyword </xsl:if>
          <xsl:if test="FieldType[. = '12']">Entity type label </xsl:if>
          <xsl:if test="FieldType[. = '13']">Attribute label </xsl:if>
          <xsl:if test="FieldType[. = '14']">Lineage </xsl:if>
          <xsl:if test="FieldType[. = '15']">Source scale </xsl:if>
          <xsl:if test="FieldType[. = '16']">Cloud cover </xsl:if>
          <xsl:if test="FieldType[. = '17']">Progress</xsl:if>
          <xsl:if test="FieldOperator[. = '0']">includes </xsl:if>
          <xsl:if test="FieldOperator[. = '1']">equals </xsl:if>
          <xsl:if test="FieldOperator[. = '2']">exists</xsl:if>
          <xsl:if test="context()[FieldValue != '']">"<xsl:value-of select="FieldValue"/>"</xsl:if>.
        </LI>
      </DIV>
      <xsl:if test="context()[end()]">
        <BR/>
      </xsl:if>
    </xsl:for-each>

    <xsl:if test="*[@Engine = 'File system']">
      <DIV CLASS="head">When searching the File system</DIV>
      <DIV>
        Data located under "<xsl:value-of select="FileSystemLocation"/>" will be retrieved. 
        <xsl:for-each select="IncludeSubFolders[@Engine = 'File system']">
          Sub-folders will <xsl:if test="IncludeSubFolders[. = '0']">not </xsl:if>be searched.
        </xsl:for-each>
      </DIV>
      <BR/>
    </xsl:if>
    <xsl:if test="*[@Engine = 'Catalog']">
      <DIV CLASS="head">When searching the Catalog</DIV>
      <DIV>Data located under "<xsl:value-of select="CatalogLocation"/>" will be retrieved.</DIV>
      <BR/>
    </xsl:if>
  </DIV>
</xsl:template>


</xsl:stylesheet>