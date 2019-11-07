<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QLD.aspx.cs" Inherits="Lexnarro.Reports.QLD" %>

<!doctype html>
<html>
<head>
    <title>QLD</title>

</head>
<body>
    <div style="width: auto; margin: 0 10%;">
        <center><h1></h1></center>
        <form>
            <b>
                <h2 style="margin-left: 50px;">Name - <%=data.Tables[0].Rows[0]["FirstName"].ToString()+" "+ 
                                                                    data.Tables[0].Rows[0]["LastName"].ToString()%></h2>
            </b>
            <b>
                <h2 style="margin-left: 50px;">Member Number - <%=data.Tables[0].Rows[0]["LawSocietyNumber"].ToString()%></h2>
            </b>
            <center><h2 style="background-color: black; color: white; padding: 10px; width: 88%;">CPD Scheme Record</h2><center>


                        <% decimal totalRows = data.Tables[0].Rows.Count;
                            decimal s = totalRows / 10m;
                            decimal pageNumber = Math.Ceiling(s);

                            var rows = 0;

                            //for (int i = 0; i < pageNumber; i++)
                            //{
                            //    rows = 0;
                             %>
				<table style="font-family: arial, sans-serif; border-collapse: collapse; width: 90%;">
				<tr style="border: 1px solid #dddddd ; text-align: left; padding: 8px;">
					<th style="border: 1px solid; text-align: left; padding: 8px;">Date</th>
					<th style="border: 1px solid; text-align: left; padding: 8px;">Event Title</th>
					<th style="border: 1px solid; text-align: left; padding: 8px;">Provider</th>
					<th style="border: 1px solid; text-align: left; padding: 8px;">Core area</th>
					<th style="border: 1px solid; text-align: left; padding: 8px;">Format</th>
					<th style="border: 1px solid; text-align: left; padding: 8px;">Units</th>
				</tr>
				<%--<tr>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">Date of the event</td>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">Insert title of seminar or activity</td>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">Insert the provider of the event, e.g QLS, firm name, external provider name.</td>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">Insert the core area that applies, if any, to the event.</td>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">Advise the format of the session e.g. seminar, DVD, conference</td>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">Insert the units that apply</td>
				</tr>--%>
				
                    <%for (int j = 0; j < data.Tables[0].Rows.Count; j++)
                        {%>
                   <tr>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">
                        <% =data.Tables[0].Rows[j]["Date"].ToString().Split(' ')[0]%>
					</td>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">
                        <% =data.Tables[0].Rows[j]["Descrption"].ToString()%>
					</td>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">
                        <% =data.Tables[0].Rows[j]["Provider"].ToString()%>
					</td>

					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">
                        <% =data.Tables[0].Rows[j]["Category Name"].ToString()%>
					</td>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">
                        <% =data.Tables[0].Rows[j]["Activity Name"].ToString()%>
					</td>
					<td style="border: 1px solid; text-align: left; padding: 8px; font-size:10px">
                        <% =data.Tables[0].Rows[j]["Units_Done"].ToString()%>
					</td>
				</tr>
                        <%}
                            
                         %>				
							
				</table>
                        <%//}
                        decimal units = 0;
                        for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                        {
                            units = units + Convert.ToDecimal(data.Tables[0].Rows[i]["Units_Done"]);
                        }
                     %>
					<h5><b>Total Units: <%=units %></b></h5>
					<h5 style="background-color: black; color: white; padding: 2px; width: 90%;"><b>
                        Year : <% =data.Tables[0].Rows[0]["Financial_Year"].ToString()%></b></h5>

        </form>
    </div>
</body>
</html>
