<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ACT.aspx.cs" Inherits="Lexnarro.Reports.ACT" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
    <title>ACT</title>
    <meta http-equiv="Content-Type" content="text/html; charset=Windows-1252">
    <meta http-equiv="Content-Style-Type" content="text/css">
    <style type="text/css">
        <!--
        body {
            margin: 130px 72px 95px 72px;
            background-color: #ffffff;
        }
        /* ========== Text Styles ========== */
        hr {
            color: #000000;
        }

        body, table, span.rvts0 /* Font Style */ {
            font-size: 11pt;
            font-family: 'Cambria';
            font-style: normal;
            font-weight: normal;
            color: #000000;
            text-decoration: none;
        }

        span.rvts1 {
            font-size: 18pt;
            font-family: 'Calibri';
            color: #365f91;
        }

        span.rvts2 {
            font-size: 9pt;
            font-family: 'Calibri';
            color: #365f91;
        }

        span.rvts3 {
            font-style: italic;
        }

        span.rvts4 {
        }

        span.rvts5 {
            font-family: 'Times New Roman', 'Times', serif;
        }

        span.rvts6 {
            font-size: 10pt;
            font-family: 'Calibri';
            color: #365f91;
        }

        span.rvts7 {
            font-size: 7pt;
            font-family: 'Times New Roman', 'Times', serif;
            font-weight: bold;
            color: #365f91;
            vertical-align: super;
        }

        span.rvts8 {
            font-size: 7pt;
            font-family: 'Times New Roman', 'Times', serif;
            vertical-align: super;
        }

        span.rvts9 {
            font-size: 10pt;
        }

        span.rvts10 {
            font-size: 7pt;
            font-family: 'Times New Roman', 'Times', serif;
            color: #365f91;
            vertical-align: super;
        }

        span.rvts11 {
            font-family: 'Times New Roman', 'Times', serif;
            font-weight: bold;
        }

        span.rvts12 {
            font-family: 'Wingdings 2';
        }
        /* ========== Para Styles ========== */
        p, ul, ol /* Paragraph Style */ {
            text-align: left;
            text-indent: 0px;
            widows: 2;
            orphans: 2;
            padding: 0px 0px 0px 0px;
            margin: 0px 0px 8px 0px;
        }

        .rvps1 {
            text-align: left;
            text-indent: 0px;
            page-break-inside: avoid;
            page-break-after: avoid;
            widows: 2;
            orphans: 2;
            padding: 0px 0px 0px 0px;
            margin: 16px 0px 8px 0px;
        }

        .rvps2 {
            widows: 2;
            orphans: 2;
            margin: 8px 0px 8px 0px;
        }

        .rvps3 {
            widows: 2;
            orphans: 2;
            margin: 0px 0px 8px 1px;
        }

        .rvps4 {
            widows: 2;
            orphans: 2;
            margin: 0px 0px 0px 0px;
        }

        .rvps5 {
            widows: 2;
            orphans: 2;
        }

        .rvps6 {
            text-align: left;
            text-indent: 0px;
            page-break-inside: avoid;
            page-break-after: avoid;
            widows: 2;
            orphans: 2;
            padding: 0px 0px 0px 0px;
            margin: 3px 0px 0px 0px;
        }

        .rvps7 {
            widows: 2;
            orphans: 2;
            margin: 0px 0px 0px 0px;
        }
        -->
    </style>

    <%--<script type="text/javascript">
        function printpage() {
            //Get the print button and put it into a variable
            var printButton = document.getElementById("printpagebutton");
            //Set the print button visibility to 'hidden' 
            printButton.style.visibility = 'hidden';
            //Print the page content
            window.print()
            //Set the print button to 'visible' again 
            //[Delete this line if you want it to stay hidden after printing]
            printButton.style.visibility = 'visible';
        }
    </script>--%>
</head>
<body style="font-size:smaller;">
    <%if (data.Tables["Report_ACT"].Rows.Count > 0)
        { %>
    <h1 class="rvps1"><span class="rvts0"><span class="rvts1">Record of CPD activities</span></span><span class="rvts0"><span class="rvts2"> </span></span></h1>
    <p><span class="rvts3">(Practitioners do not need to submit this record to the Society unless requested to do so)</span></p>
    <div class="rvps3">
        <table border="1" cellpadding="4" cellspacing="-1" style="border-width: 0px; border-collapse: collapse;">
            <tr valign="top">
                <td width="102" height="27" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p class="rvps2"><span class="rvts4"><b>Full Name:</b></span></p>
                </td>
                <td width="519" height="27" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p class="rvps2">

                        <span class="rvts4"><%=data.Tables["Report_ACT"].Rows[0]["FirstName"].ToString() + " " + data.Tables["Report_ACT"].Rows[0]["LastName"].ToString()%><br>
                        </span>
                    </p>
                </td>
            </tr>
            <tr valign="top">
                <td width="102" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p class="rvps2"><span class="rvts4"><b>Firm/Agency:</b></span></p>
                </td>
                <td width="519" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p class="rvps2">
                        <span class="rvts4">
                            <%=data.Tables["Report_ACT"].Rows[0]["Firm"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
            </tr>
            <tr valign="top">
                <td width="102" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p class="rvps2"><span class="rvts4"><b>Postal Address:</b></span></p>
                </td>
                <td width="519" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p class="rvps2">
                        <span class="rvts4">
                            <br>
                            <%=data.Tables["Report_ACT"].Rows[0]["StreetNumber"].ToString() + "," + data.Tables["Report_ACT"].Rows[0]["StreetName"].ToString()%></span>
                    </p>
                    <p class="rvps2">
                        <span class="rvts4"><%=data.Tables["Report_ACT"].Rows[0]["Suburb"].ToString()+", "+data.Tables["Report_ACT"].Rows[0]["PostCode"].ToString()%><br>
                            <span class="rvts4"><%=data.Tables["Report_ACT"].Rows[0]["State Name"].ToString()+", "+data.Tables["Report_ACT"].Rows[0]["Country Name"].ToString()%><br>

                        </span>
                    </p>
                </td>
            </tr>
            <tr valign="top">
                <td width="102" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p class="rvps2"><span class="rvts4"><b>Email: </b></span></p>
                </td>
                <td width="519" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p class="rvps2">
                        <span class="rvts4"><%=data.Tables["Report_ACT"].Rows[0]["EmailAddress"].ToString()%><br>
                        </span>
                    </p>
                </td>
            </tr>
            <tr valign="top">
                <td width="102" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p class="rvps2"><span class="rvts4"><b>CPD Year:</b></span></p>
                </td>
                <td width="519" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p class="rvps2"><span class="rvts4">1 April <%=yearFrom %>&nbsp; to 31 March <%=yearTo %></span></p>
                </td>
            </tr>
        </table>
    </div>
    <p>
        <span class="rvts5">
            <br>
        </span>
    </p>
    <p><span class="rvts4">Practitioners must complete a minimum of 10 CPD units each CPD year, with at least one unit in each of the four core areas:</span></p>
    <p class="rvps4">
        <span class="rvts4">1.</span><span class="rvts4"> &nbsp; &nbsp; &nbsp; &nbsp;</span><span class="rvts4">Legal ethics and professional responsibility</span><br>
        <span class="rvts4">2.</span><span class="rvts4"> &nbsp; &nbsp; &nbsp; &nbsp;</span><span class="rvts4">Practice management and business skills</span>
    </p>
    <p class="rvps4"><span class="rvts4">3.</span><span class="rvts4"> &nbsp; &nbsp; &nbsp; &nbsp;</span><span class="rvts4">Professional skills</span></p>
    <p class="rvps4"><span class="rvts4">4.</span><span class="rvts4"> &nbsp; &nbsp; &nbsp; &nbsp;</span><span class="rvts4">Substantive law and procedural law</span></p>
    <p class="rvps4">
        <span class="rvts5">
            <br>
        </span>
    </p>
    <p class="rvps5"><span class="rvts4">For more information please see the </span><span class="rvts3">CPD Guidelines </span><span class="rvts4">or contact the Society on 6247 5700.</span></p>
    <p>
        <span class="rvts5">
            <br>
        </span>
    </p>

    <br />
    <br />
    <div class="rvps3">
        <%int z = 0, k = 0; %>

        <% while (n > 0)
            { %>
                <%if (n <= 3)
                {%>

        <table width="100%" border="1" cellpadding="4" cellspacing="-1" style="border-width: 0px; border-collapse: collapse;">
            <tr valign="top">
                <td width="52" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Date</span></span></h2>
                </td>
                <td width="147" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Title of activity</span></span></h2>
                </td>
                <td width="121" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Provider</span></span><span class="rvts0"><span class="rvts7"><a href="#_footnote0">1</a></span></span></h2>
                </td>
                <td width="86" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Format</span></span><span class="rvts0"><span class="rvts10"><a href="#_footnote1">2</a></span></span></h2>
                </td>
                <td width="54" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Your role</span></span><span class="rvts0"><span class="rvts10"><a href="#_footnote2">3</a></span></span></h2>
                </td>
                <td width="46" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Core area</span></span></h2>
                </td>
                <td width="46" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Units claimed</span></span></h2>
                </td>
            </tr>

            <% int q = k + n; %>
            <% for (; k < q; k++)
                {%>

            <tr valign="top">
                <td width="52" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Date"].ToString().Split(' ')[0]%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="147" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Descrption"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="121" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Provider"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="86" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Activity Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="54" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["SubActivity Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="46" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Category Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="46" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Units_Done"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
            </tr>
            <%} %>
        </table>
        <br />
        <br />
        <br />
        <br />

        <p>
            <span class="rvts11">
                <br>
            </span>
        </p>
        <p><span class="rvts12">£</span><span class="rvts4"> I have completed 10 or more units</span></p>
        <p><span class="rvts12">£</span><span class="rvts4"> I have completed at least one unit in each of the four core areas</span></p>
        <p>
            <span class="rvts5">
                <br>
            </span>
        </p>
        <hr noshade style="color: #c0c0c0; border-width: 0px" width="30%" align="left" size="2">
        <a name="_footnote0"></a>
        <p class="rvps7"><span class="rvts8">1</span><span class="rvts9"> e.g. ACTLS, firm name, commercial provider name</span></p>
        <a name="_footnote1"></a>
        <p class="rvps7"><span class="rvts8">2</span><span class="rvts9"> e.g. seminar, DVD, publication</span></p>
        <a name="_footnote2"></a>
        <p class="rvps7"><span class="rvts8">3</span><span class="rvts9"> e.g. attendee, presenter, author</span></p>


        <% return;
            } %>

        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />




        <%if (z == 0)
            {%>

        <table width="100%" border="1" cellpadding="4" cellspacing="-1" style="border-width: 0px; border-collapse: collapse;">
            <tr valign="top">
                <td width="52" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Date</span></span></h2>
                </td>
                <td width="147" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Title of activity</span></span></h2>
                </td>
                <td width="121" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Provider</span></span><span class="rvts0"><span class="rvts7"><a href="#_footnote0">1</a></span></span></h2>
                </td>
                <td width="86" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Format</span></span><span class="rvts0"><span class="rvts10"><a href="#_footnote1">2</a></span></span></h2>
                </td>
                <td width="54" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Your role</span></span><span class="rvts0"><span class="rvts10"><a href="#_footnote2">3</a></span></span></h2>
                </td>
                <td width="46" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Core area</span></span></h2>
                </td>
                <td width="46" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Units claimed</span></span></h2>
                </td>
            </tr>
            <% for (; z < 3; z++)
                {%>

            <tr valign="top">
                <td width="52" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[z]["Date"].ToString().Split(' ')[0]%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="147" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[z]["Descrption"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="121" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[z]["Provider"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="86" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[z]["Activity Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="54" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[z]["SubActivity Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="46" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[z]["Category Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="46" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[z]["Units_Done"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
            </tr>


            <%} %>
        </table>

        <br />
        <br />
        <br />
        <br />
        <%n = n - 3;%>
        <% k = z;%>
        <% }%>

        <%else
            {%>

        <% if (n > 7)
            {%>
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <table width="100%" border="1" cellpadding="4" cellspacing="-1" style="border-width: 0px; border-collapse: collapse;">
            <tr valign="top">
                <td width="52" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Date</span></span></h2>
                </td>
                <td width="147" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Title of activity</span></span></h2>
                </td>
                <td width="121" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Provider</span></span><span class="rvts0"><span class="rvts7"><a href="#_footnote0">1</a></span></span></h2>
                </td>
                <td width="86" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Format</span></span><span class="rvts0"><span class="rvts10"><a href="#_footnote1">2</a></span></span></h2>
                </td>
                <td width="54" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Your role</span></span><span class="rvts0"><span class="rvts10"><a href="#_footnote2">3</a></span></span></h2>
                </td>
                <td width="46" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Core area</span></span></h2>
                </td>
                <td width="46" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Units claimed</span></span></h2>
                </td>
            </tr>
            <% int p = k + 5; %>
            <% for (; k < p; k++)
                {%>

            <tr valign="top">
               <td width="52" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Date"].ToString().Split(' ')[0]%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="147" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Descrption"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="121" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Provider"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="86" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Activity Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="54" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["SubActivity Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="46" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Category Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="46" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Units_Done"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
            </tr>


            <%} %>
        </table>
        <br />
        <br />
        <br />
        <br />

        <% n = n - 5;%>

        <% }%>

        <% else
            {%>

        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />

        <table width="100%" border="1" cellpadding="4" cellspacing="-1" style="border-width: 0px; border-collapse: collapse;">
            <tr valign="top">
                <td width="52" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Date</span></span></h2>
                </td>
                <td width="147" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Title of activity</span></span></h2>
                </td>
                <td width="121" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Provider</span></span><span class="rvts0"><span class="rvts7"><a href="#_footnote0">1</a></span></span></h2>
                </td>
                <td width="86" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Format</span></span><span class="rvts0"><span class="rvts10"><a href="#_footnote1">2</a></span></span></h2>
                </td>
                <td width="54" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Your role</span></span><span class="rvts0"><span class="rvts10"><a href="#_footnote2">3</a></span></span></h2>
                </td>
                <td width="46" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Core area</span></span></h2>
                </td>
                <td width="46" height="30" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <h2 class="rvps6"><span class="rvts0"><span class="rvts6">Units claimed</span></span></h2>
                </td>
            </tr>

            <% int q = k + n; %>
            <% for (; k < q; k++)
                {%>

            <tr valign="top">
                <td width="52" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Date"].ToString().Split(' ')[0]%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="147" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Descrption"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="121" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Provider"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="86" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Activity Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="54" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["SubActivity Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="46" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Category Name"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="46" height="53" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts5"><% =data.Tables["Report_ACT"].Rows[k]["Units_Done"].ToString()%>
                            <br>
                        </span>
                    </p>
                </td>
            </tr>


            <%} %>
        </table>
        <br />
        <br />
        <br />
        <br />
        <% n = n - n;%>

        <% }%>


        <% }%>



        <% }%>
    </div>


    <p>
        <span class="rvts11">
            <br>
        </span>
    </p>
    <p><span class="rvts12">£</span><span class="rvts4"> I have completed 10 or more units</span></p>
    <p><span class="rvts12">£</span><span class="rvts4"> I have completed at least one unit in each of the four core areas</span></p>
    <p>
        <span class="rvts5">
            <br>
        </span>
    </p>
    <hr noshade style="color: #c0c0c0; border-width: 0px" width="30%" align="left" size="2">
    <a name="_footnote0"></a>
    <p class="rvps7"><span class="rvts8">1</span><span class="rvts9"> e.g. ACTLS, firm name, commercial provider name</span></p>
    <a name="_footnote1"></a>
    <p class="rvps7"><span class="rvts8">2</span><span class="rvts9"> e.g. seminar, DVD, publication</span></p>
    <a name="_footnote2"></a>
    <p class="rvps7"><span class="rvts8">3</span><span class="rvts9"> e.g. attendee, presenter, author</span></p>


    <%--<center><input id="printpagebutton" type="button" value="Print" onclick="printpage()" /></center>--%>

    <% }%>
</body>
</html>
