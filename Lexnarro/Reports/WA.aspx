<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WA.aspx.cs" Inherits="Lexnarro.Reports.WA" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
    <title>WA</title>
    <meta http-equiv="Content-Type" content="text/html; charset=Windows-1252">
    <meta http-equiv="Content-Style-Type" content="text/css">
    <style type="text/css">
        <!--
        body {
            margin: 120px 65px 120px 30px;
            background-color: #ffffff;
        }
        /* ========== Text Styles ========== */
        hr {
            color: #000000;
        }

        body, table, span.rvts0 /* Font Style */ {
            font-size: 12pt;
            font-family: 'Times New Roman', 'Times', serif;
            font-style: normal;
            font-weight: normal;
            color: #000000;
            text-decoration: none;
        }

        span.rvts1 {
            font-size: 10pt;
            font-family: 'Arial', 'Helvetica', sans-serif;
            font-weight: bold;
        }

        span.rvts2 {
            font-size: 8pt;
            font-family: 'Arial', 'Helvetica', sans-serif;
            font-weight: bold;
        }

        span.rvts3 {
            font-size: 8pt;
            font-family: 'Arial', 'Helvetica', sans-serif;
        }

        span.rvts4 {
            font-size: 8pt;
            font-family: 'Arial Bold';
            font-weight: bold;
        }

        span.rvts5 {
            font-size: 9pt;
            font-family: 'Arial', 'Helvetica', sans-serif;
            font-weight: bold;
        }

        span.rvts6 {
            font-family: 'Arial', 'Helvetica', sans-serif;
            font-style: italic;
        }

        span.rvts7 {
            font-family: 'Arial', 'Helvetica', sans-serif;
        }

        span.rvts8 {
            font-family: 'Arial', 'Helvetica', sans-serif;
            font-style: italic;
            font-weight: bold;
        }

        span.rvts9 {
            font-family: 'Arial', 'Helvetica', sans-serif;
            font-weight: bold;
        }

        span.rvts10 {
            font-size: 2pt;
            font-family: 'Arial', 'Helvetica', sans-serif;
        }
        /* ========== Para Styles ========== */
        p, ul, ol /* Paragraph Style */ {
            text-align: center;
            text-indent: 0px;
            widows: 2;
            orphans: 2;
            padding: 0px 0px 0px 0px;
            margin: 0px 0px 0px 0px;
        }

        .rvps1 {
            text-align: center;
            widows: 2;
            orphans: 2;
        }

        .rvps2 {
            /*text-align: center;
            widows: 2;
            orphans: 2;*/
            /*margin: 0px 50px 0px 50px;*/
        }
        -->
    </style>
</head>
<body style="margin: 120px 75px 120px 57px">
    <div>
        <p lang="en-AU" align="CENTER" style="margin-bottom: 0.32in">
            <font size="5">
                <u>
                    <b>CPD
                            RECORD CARD
                    </b>
                </u>
            </font>
        </p>
    </div>

    <%decimal totalUnitsPoint = 0; %>

    <div class="rvps2">
        <table width="945" border="1" cellpadding="4" cellspacing="-1" style="border-width: 0px; border-collapse: collapse; margin: 0px auto;">
            <tr valign="top">
                <td width="81" valign="middle" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p class="rvps1"><span class="rvts1">DATE</span></p>
                </td>
                <td width="118" valign="middle" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p class="rvps1"><span class="rvts1">PROVIDER</span></p>
                </td>
                <td width="288" valign="middle" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p class="rvps1"><span class="rvts1">ACTIVITY &amp; TOPIC</span></p>
                </td>
                <td width="73" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p><span class="rvts2">YOUR ROLE: </span><span class="rvts3"></span></p>
                </td>
                <td width="73" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p><span class="rvts2">ROLE: </span><span class="rvts3">Participant Presenter Commentator Chair </span></p>
                </td>
                <td width="69" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p><span class="rvts4">Competency Area 1</span><span class="rvts2"> </span></p>
                    <p><span class="rvts3">Practice Management</span></p>
                </td>
                <td width="69" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p><span class="rvts4">Competency Area 2</span><span class="rvts2"> </span></p>
                    <p><span class="rvts3">Professional Skills</span></p>
                </td>
                <td width="73" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p><span class="rvts4">Competency Area 3</span><span class="rvts2"> </span></p>
                    <p><span class="rvts3">Ethics or Professional Responsibility</span></p>
                </td>
                <td width="69" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p><span class="rvts4">Competency Area 4</span><span class="rvts2"> </span></p>
                    <p><span class="rvts3">Substantive Law</span></p>
                </td>
                <td width="54" valign="middle" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p class="rvps1"><span class="rvts5">POINTS</span></p>
                </td>
            </tr>

            <%  if (data.Tables[0].Rows.Count > 0)
                {                    
                    for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
                    {%>
            <tr valign="top" style="font-size:smaller">
                <td width="81" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts6"><%=data.Tables[0].Rows[i]["Date"].ToString().Split(' ')[0] %>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="118" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts6"><%=data.Tables[0].Rows[i]["Provider"].ToString() %>
                            <br>
                        </span>
                    </p>
                </td>
                <td width="288" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts6"><%=data.Tables[0].Rows[i]["Descrption"].ToString()%> (<%=data.Tables[0].Rows[i]["Activity Name"].ToString() %>)
                        <br>
                        </span>
                    </p>
                </td>
                <td width="73" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts6">
                            <br>
                            <%=data.Tables[0].Rows[i]["SubActivity Name"].ToString() %>
                        </span>
                    </p>
                </td>
                
                <td width="73" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts6">
                            <br>
                            <%=data.Tables[0].Rows[i]["Your_Role"].ToString()%>
                        </span>
                    </p>
                </td>
                <td width="69" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts6">
                            <% totalUnits = Convert.ToDecimal(data.Tables[0].Rows[i]["Units_Done"].ToString());%>
                            <%if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "PM")
                                {
                            %>
                            <br>
                            <%=data.Tables[0].Rows[i]["Units_Done"].ToString()%>
                            <% } %>
                        </span>
                    </p>
                </td>
                <td width="69" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts6">
                            <% totalUnits = Convert.ToDecimal(data.Tables[0].Rows[i]["Units_Done"].ToString());%>
                            <%if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "PS")
                                {
                            %>
                            <br>
                            <%=data.Tables[0].Rows[i]["Units_Done"].ToString()%>
                            <% } %>
                        </span>
                    </p>
                </td>
                <td width="73" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts6">
                            <% totalUnits = Convert.ToDecimal(data.Tables[0].Rows[i]["Units_Done"].ToString());%>
                            <%if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "EP")
                                {
                            %>
                            <br>
                            <%=data.Tables[0].Rows[i]["Units_Done"].ToString()%>
                            <% } %>
                        </span>
                    </p>
                </td>
                <td width="69" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts6">
                            <% totalUnits = Convert.ToDecimal(data.Tables[0].Rows[i]["Units_Done"].ToString());%>
                            <%if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "SL")
                                {
                            %>
                            <br>
                            <%=data.Tables[0].Rows[i]["Units_Done"].ToString()%>
                            <% } %>
                        </span>
                    </p>
                </td>
                <td width="54" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px;">
                    <p>
                        <span class="rvts7">
                            <br>
                            <%=totalUnits%>
                            <%totalUnitsPoint = totalUnitsPoint + totalUnits;%>
                        </span>
                    </p>
                </td>
            </tr>
            <%}
            }%>
            <tr valign="top">
                <td width="81" valign="middle" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p class="rvps1"><span class="rvts1"></span></p>
                </td>
                <td width="118" valign="middle" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p class="rvps1"><span class="rvts1"></span></p>
                </td>
                <td width="288" valign="middle" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p class="rvps1"><span class="rvts1">Total Points</span></p>
                </td>
                <td width="73" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    
                </td>
                <td width="73" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    
                </td>
                <td width="69" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p>
                        <span class="rvts2"></span><span class="rvts6">
                            <% decimal totalPm = 0;
                                for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
                                {
                                    if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "PM")
                                    {
                                        totalPm = totalPm + Convert.ToDecimal(data.Tables[0].Rows[i]["Units_Done"].ToString());
                                    }
                                } %>
                            <%=totalPm %>
                        </span>
                    </p>
                    
                </td>
                <td width="69" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p>
                        <span class="rvts2"></span><span class="rvts6">
                            <% decimal totalPs = 0;
                                for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
                                {
                                    if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "PS")
                                    {
                                        totalPs = totalPs + Convert.ToDecimal(data.Tables[0].Rows[i]["Units_Done"].ToString());
                                    }
                                } %>
                            <%=totalPs %>
                        </span>
                    </p>
                </td>
                <td width="73" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p>
                        <span class="rvts2"></span><span class="rvts6">
                            <% decimal totalEp = 0;
                                for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
                                {
                                    if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "EP")
                                    {
                                        totalEp = totalEp + Convert.ToDecimal(data.Tables[0].Rows[i]["Units_Done"].ToString());
                                    }
                                } %>
                            <%=totalEp %>
                        </span>
                    </p>
                </td>
                <td width="69" valign="top" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p>
                        <span class="rvts2"></span><span class="rvts6">
                            <% decimal totalSl = 0;
                                for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
                                {
                                    if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "SL")
                                    {
                                        totalSl = totalSl + Convert.ToDecimal(data.Tables[0].Rows[i]["Units_Done"].ToString());
                                    }
                                } %>
                            <%=totalSl %>
                        </span>
                    </p>
                </td>
                <td width="54" valign="middle" style="border-width: 1px; border-color: #000000; border-style: solid; padding: 0px 7px; background-color: #e6e6e6;">
                    <p>
                        <span class="rvts2"></span><span class="rvts6">                            
                            <%=totalUnitsPoint %>
                        </span>
                    </p>
                </td>
            </tr>
        </table>


    </div>
    <p>
        <span class="rvts10">
            <br>
        </span>
    </p>
    <hr />
    <footer>
        <div>I declare this is a true and accurate record </div>
       <br />
        <label><b>NAME - <b><u><%= data.Tables[0].Rows[0]["FirstName"].ToString() + " " + data.Tables[0].Rows[0]["LastName"].ToString()%></u></b>
            SIGNATURE -______________________DATE________________ </b></label>
    </footer>
</body>
</html>
