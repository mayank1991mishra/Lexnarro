<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SA.aspx.cs" Inherits="Lexnarro.Reports.SA" %>

<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>SA</title>
    <!--<style type="text/css">
            /*.boldtable, .boldtable td, .boldtable th { font-family: sans-serif; font-size: 20pt; color: white; background-color: navy; }*/
            h1 {
                text-align: center;
                background-color: #000000;
                color: #fff;
                padding: 4px 30px 4px 8px;
            }

            table {
                font-family: arial, sans-serif;
                border-collapse: collapse;
                width: 100%;
            }

            td, th {
                border: 1px solid #dddddd;
                text-align: left;
                padding: 8px;
            }

            /*tr:nth-child(even) {
        background-color: #dddddd;
        }*/
    </style>-->
</head>
<body>
    <form style="font-size:x-small">
        <div style="width: auto; margin: 0 10%;">
            <table style="font-family: arial, sans-serif;
    border-collapse: collapse;
    width: 100%;">
                <TR>
                    <TH COLSPAN="8">
                        <h1 style="text-align: center;
                background-color: #000000;
                color: #fff;
                padding: 4px 30px 4px 8px;">CPD Rectification Plan Under Rule 8(2)</h1>
                        <p>Name: <%=data.Tables[0].Rows[0]["FirstName"].ToString()+" "+ data.Tables[0].Rows[0]["LastName"].ToString()%>  <br />
                            Address: <%=data.Tables[0].Rows[0]["StreetNumber"].ToString() %>, <%=data.Tables[0].Rows[0]["StreetName"].ToString() %>, <%=data.Tables[0].Rows[0]["Suburb"].ToString() %>, <%=data.Tables[0].Rows[0]["PostCode"].ToString() %> &nbsp;</p>
                        <br><p>Practitioner No.&nbsp; <u><%=data.Tables[0].Rows[0]["LawSocietyNumber"].ToString()%></u> Firm:&nbsp; <u><%=data.Tables[0].Rows[0]["Firm"].ToString()%></u> <br />
                            Email: <u><%=data.Tables[0].Rows[0]["EmailAddress"].ToString() %></u> Contact No. <u><%=data.Tables[0].Rows[0]["PhoneNumber"].ToString() %></u></p></br>
                        <BR><H9>Please list below the CPD activities you intend to complete within 90 days from the date of submission of this Plan </H9></BR>
                    </TH>
                </TR>
                <tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <th style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">Date</th>
                    <th style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">Details of CPD Activity</th>
                    <th style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">provider</th>
                    <th style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">Formats</th>
                    <th style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">Unit</th>
                </tr>

                <%--<tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">Insert the date activity to be undertaken</td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">Insert the title of the seminar/course. For example "Trust Law - The Role Of Executors"</td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">Insert the name of the provider or publisher of the activity. For eample. "Training"</td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">For example: workshop, studygroup</td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">Insert the unit that apply</td>
                </tr>--%>

                      <%  if (data.Tables[0].Rows.Count > 0)
    {
        for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
        {%>

                <tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"><%=data.Tables[0].Rows[i]["Date"].ToString().Split(' ')[0] %></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"><%=data.Tables[0].Rows[i]["Descrption"].ToString() %> (<%=data.Tables[0].Rows[i]["Category Name"].ToString() %>)</td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"><%=data.Tables[0].Rows[i]["Provider"].ToString() %></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"><%=data.Tables[0].Rows[i]["Activity Name"].ToString() %></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"><%=data.Tables[0].Rows[i]["Units_Done"].ToString() %></td>
                </tr><%}
    }%>

                <%--<tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                </tr>

                <tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                </tr>

                <tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                </tr>

                <tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>               
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                </tr>

                <tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                </tr>

                <tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                </tr>

                <tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                </tr>

                <tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                </tr>

                <tr style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;">Total Units</td>
                    <td style="border: 1px solid #dddddd ;
                      text-align: left;
                       padding: 8px;"></td>
                </tr>--%>


            </table>
            <p>Please note: if you do not complete 1 CPD unit in each of the compulsory field as required under rule 5.2 of the LIV Continuing Professional Developement Rules 2008 or Rule 8 of the Victorian bar Continuing Professional Developement Rules[whichever apply].you must include in your Rectification Plan CPD activites that will fullfill those compulsory field</p>
            <p><b>Date:____________________________  Signature:________________________  </b></p>
        </div>
    </form>


</body>
</html>
