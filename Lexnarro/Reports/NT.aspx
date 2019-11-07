<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NT.aspx.cs" Inherits="Lexnarro.Reports.NT" %>

<!doctype html>
<html>
<head>
    <title>NT</title>
    <script type="text/javascript">
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
    </script>
</head>
<body>
    <div style="width: auto; margin: 0 4%;">
        <center><h1>Annual CPD Certificate</h1></center>
        <form>

            <p>Approved form pursuant to clause 16(1) Schedule 2 Legal Profession Regulations 2007 (as amended).</p>

            <p>This Certificate should be provided to the Law Society Northern Territory no later than 31 March by practitioners who hold, or have held, a practising certificate during the CPD year from 1 April to 31 March.</p>

            <p>For the purpose of this Certificate the following legend applies: Substantive Law competency (SL); Core competency in Professional Ethics and Responsibilities (CA); Core competency in Practice Management and Business Skills (CB); Core competency in Professional Skills in Legal Practice (CC).</p>



            <ol>
                <li>I <u><b><% =data.Tables[0].Rows[0]["FirstName"].ToString()+" "+data.Tables[0].Rows[0]["LastName"].ToString()%></b></u> of <u><b><%=data.Tables[0].Rows[0]["Firm"].ToString()%></b></u> have completed the following CPD activities to satisfy
					 my obligations pursuant to Schedule 2 of the Legal Professional Rules 2007 for the <%=data.Tables[0].Rows[0]["Financial_Year"].ToString()%> CPD year:</li>
                <b>
                    <p>Points carried forward from January 1 to 31 March of previous CPD year</p>
                </b>
                <table style="font-family: arial, sans-serif; border-collapse: collapse; width: 100%;font-size:small">
                    <tr style="border: 1px solid #dddddd; text-align: left; padding: 8px;">
                        <th style="border: 1px solid; text-align: left; padding: 8px;"></th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Date</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">CPD activity</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Provider</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Points claimed</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Competency:
						SL, CA, CB, CC</th>

                    </tr>
                    <% 
                        if (data.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
                            {
                                if (data.Tables[0].Rows[i]["Financial_Year"].ToString() == previousFinacialYear)
                                {
                                %>
                    <tr>
                        <td style="border: 1px solid; text-align: left; padding: 8px;"><%=i + 1 %></td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">
                            <% =data.Tables[0].Rows[i]["Date"].ToString().Split(' ')[0]%>
                        </td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">
                            <% =data.Tables[0].Rows[i]["Descrption"].ToString()%> (<% =data.Tables[0].Rows[i]["Activity Name"].ToString()%>)
                        </td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">
                            <% =data.Tables[0].Rows[i]["Provider"].ToString()%>
                        </td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">
                            <% =data.Tables[0].Rows[i]["Units_Done"].ToString()%>
                        </td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">
                            <% if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "SL")
                                {%>
                            <%= data.Tables[0].Rows[i]["CategShortName"].ToString()%>
                            <%} %>

                             <% if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "EP")
                                {%>
                            CA
                            <%} %>

                            <% if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "PM")
                                {%>
                            CB
                            <%} %>

                            <% if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "PS")
                                {%>
                            CC
                            <%} %>
                        </td>
                    </tr>
                    <%}
                        }
                    } %>
                </table>

                <b>
                    <p>Points accrued in current CPD year from 1 April to 31 March</p>
                </b>

                <table style="font-family: arial, sans-serif; border-collapse: collapse; width: 100%;font-size:small">
                    <tr style="border: 1px solid #dddddd; text-align: left; padding: 8px;">
                        <th style="border: 1px solid; text-align: left; padding: 8px;"></th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Date</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">CPD activity</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Provider</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Points claimed</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Competency:
						SL, CA, CB, CC</th>

                    </tr>
                    <% 
                        if (data.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
                            {%>
                                        <tr>
                        <td style="border: 1px solid; text-align: left; padding: 8px;"><%=i + 1 %></td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">
                            <% =data.Tables[0].Rows[i]["Date"].ToString().Split(' ')[0]%>
                        </td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">
                            <% =data.Tables[0].Rows[i]["Descrption"].ToString()%> (<% =data.Tables[0].Rows[i]["Activity Name"].ToString()%>)
                        </td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">
                            <% =data.Tables[0].Rows[i]["Provider"].ToString()%>
                        </td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">
                            <% =data.Tables[0].Rows[i]["Units_Done"].ToString()%>
                        </td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">
                            <% if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "SL")
                                {%>
                            <%= data.Tables[0].Rows[i]["ShortName"].ToString()%>
                            <%} %>

                             <% if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "LE")
                                {%>
                            CA
                            <%} %>

                            <% if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "PM")
                                {%>
                            CB
                            <%} %>

                            <% if (data.Tables[0].Rows[i]["CategShortName"].ToString() == "PS")
                                {%>
                            CC
                            <%} %>
                        </td>
                    </tr>

                    <%}
                        } %>

                    <tr style="border: 1px solid #dddddd; text-align: left; padding: 8px;">
                        <td style="border: 1px solid; text-align: left; padding: 8px;"></td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;"></td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;"></td>
                        <%  decimal units = 0;
                            for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
                            {
                                units = units + Convert.ToDecimal(data.Tables[0].Rows[i]["Units_Done"]);
                            }
                        %>
                        <td style="border: 1px solid; text-align: left; padding: 8px;">Total Points claimed</td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;"><%=units %></td>
                        <td style="border: 1px solid; text-align: left; padding: 8px;"></td>

                    </tr>
                </table>

                <p>
                    <li>I wish to carry forward the following points accrued between 1 January and 31 March in this CPD year towards next year's CPD obligation (only complete if applicable)</li>
                </p>

                <table style="font-family: arial, sans-serif; border-collapse: collapse; width: 100%;font-size:small">
                    <tr style="border: 1px solid #dddddd; text-align: left; padding: 8px;">
                        <th style="border: 1px solid; text-align: left; padding: 8px;"></th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Date</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">CPD activity</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Provider</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Points accrued</th>
                        <th style="border: 1px solid; text-align: left; padding: 8px;">Competency:
						SL, CA, CB, CC</th>

                    </tr>
                    <%--<% for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
						{%>

					<tr style="border: 1px solid #dddddd; text-align: left; padding: 8px;">
						<td style="border: 1px solid; text-align: left; padding: 8px;">i+1</td>
						<td style="border: 1px solid; text-align: left; padding: 8px;">
							<% =data.Tables[0].Rows[i]["Date"].ToString().Split(' ')[0]%>
						</td>
						<td style="border: 1px solid; text-align: left; padding: 8px;">
							<% =data.Tables[0].Rows[i]["Activity Name"].ToString()%>
						</td>
						<td style="border: 1px solid; text-align: left; padding: 8px;">
							<% =data.Tables[0].Rows[i]["Provider"].ToString()%>
						</td>
						<td style="border: 1px solid; text-align: left; padding: 8px;">
							<% =data.Tables[0].Rows[i]["Units_Done"].ToString()%>
						</td>
						<td style="border: 1px solid; text-align: left; padding: 8px;">
							<% =data.Tables[0].Rows[i]["ShortName"].ToString()%>
						</td>
					</tr>
					<%} %>--%>
                </table>

                <p>
                    <li>I declare that I  have /  have not (tick applicable box) accrued sufficient CPD points in a substantive law competency and each of the core competencies to fulfil my CPD obligations for the <%=data.Tables[0].Rows[0]["Financial_Year"].ToString()%> CPD year.</li>
                </p>

                <p>
                    <li>Complete if full CPD obligation has not been satisfied:
				<br>
                        I intend accrue points after 31 March claiming these towards my CPD obligation for <%=data.Tables[0].Rows[0]["Financial_Year"].ToString()%>
                        <br>
                        I will submit a Supplementary Certificate by 30 June <%=yearTo %></li>
                </p>

                <li>I declare that this information is true and correct:</li>
            </ol>
            <p><b>Signature: ___________________ Date: __________________</b></p>

            <p>Once completed this Annual CPD Certificate should be signed (an electronic signature is not acceptable) and provided to the Law Society Northern Territory by:</p>

            <p><b>Faxing</b> to 08 8941 1623</p>
            <p><b>Emailing</b> to licenseofficer@lawsocietynt.asn.au with 'Annual CPD Certificate' in the subject line</p>
            <p><b>Hand delivering</b> it to Level 3, 9 Cavenagh Street, Darwin, 0800</p>
            <p><b>Posting</b> it to GPO Box 2388, Darwin NT, 0801</p>

            <p>OFFICE USE ONLY</p>
            <table style="font-family: arial, sans-serif; border-collapse: collapse; width: 40%;">
                <tr style="border: 1px solid #dddddd; text-align: left; padding: 8px;">
                    <th style="border: 1px solid; text-align: left; padding: 8px;">Date received</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">Recorded in database</th>
                </tr>
                <tr style="border: 1px solid #dddddd; text-align: left; padding: 8px;">
                    <td style="border: 1px solid; text-align: left; padding: 8px;"></td>
                    <td style="border: 1px solid; text-align: left; padding: 8px;"></td>
                </tr>
            </table>
        </form>
    </div>
    <%--<center><input id="printpagebutton" type="button" value="Print" onclick="printpage()" /></center>--%>
</body>
</html>
