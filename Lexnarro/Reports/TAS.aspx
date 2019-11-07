<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TAS.aspx.cs" Inherits="Lexnarro.Reports.TAS" %>

<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>TAS</title>
    <!--<style type="text/css">
        /*table {
            font-family: arial, sans-serif;
            border-collapse: collapse;
            width: 80%;
        }*/

        /*td, th {
            border: 2px solid #808080;
            text-align: left;
            padding: 8px;
        }*/

        /*tr:nth-child(even) {
            /*background-color: #dddddd;*/
        }
        */

        /*#container{
        width:500px;
        margin-left:auto;
        margin-right:auto;
        text-align:left;*/
        }
    </style>-->
</head>
<body>
    <form>
        <div style="width: auto; margin: 0 15%;">
            <h3>MY CPD RECORD</h3>
            <p>CPD YEAR: <%=data.Tables[0].Rows[0]["Financial_Year"].ToString() %></p>
            <br />
            <p><b>Name :</b> <%=data.Tables[0].Rows[0]["FirstName"].ToString()+" "+ data.Tables[0].Rows[0]["LastName"].ToString()%><b>  
                Film/entity: </b><%=data.Tables[0].Rows[0]["Firm"].ToString()%> <b>Telephone:</b> <%=data.Tables[0].Rows[0]["PhoneNumber"].ToString() %></p>

            <table style="font-family: arial, sans-serif;
                   border-collapse: collapse;
                   width: 100%;" cellpadding="40" cellspacing="90">
                <tr style="border: 2px solid #808080;
                    text-align :left;
                    padding: 8px;">
                    <th  style="border: 2px solid #808080;
                    text-align :left; font-size:x-small; font-weight:bold;
                    padding: 8px;">CPD Activity (and Event Name if applicable)</th>
                    <th style="border: 2px solid #808080;
                    text-align :left;font-size:x-small; font-weight:bold;
                    padding: 27px;">Date</th>
                    <th style="border: 2px solid #808080;
                    text-align :left;font-size:x-small; font-weight:bold;
                    padding: 8px;">Points  and Mandatory Category if applicable</th>
                    <th style="border: 2px solid #808080;
                    text-align :left;font-size:x-small; font-weight:bold;
                    padding: 8px;";>Provider / Entity</th>
                    <th style="border: 2px solid #808080;
                    text-align :left;font-size:x-small; font-weight:bold;
                    padding: 8px;">
                        Description/Comments

                        Supporting evidence attached?
                    </th>
                </tr>

                  <%  if (data.Tables[0].Rows.Count > 0)
    {
        for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
        {%>
                <tr  style="border: 2px solid #808080;
                    text-align :left;
                    padding: 8px;font-size:x-small;">
                    <td  style="border: 2px solid #808080;
                    text-align :left;
                    padding: 8px;"><%=data.Tables[0].Rows[i]["Activity Name"].ToString() %></td>
                    <td  style="border: 2px solid #808080;
                    text-align :left;
                    padding: 8px;"><%=data.Tables[0].Rows[i]["Date"].ToString().Split(' ')[0] %></td>
                    <td  style="border: 2px solid #808080;
                    text-align :left;
                    padding: 8px;"><%=data.Tables[0].Rows[i]["Units_Done"].ToString() + "/" + data.Tables[0].Rows[i]["Category Name"].ToString() %></td>
                    <td  style="border: 2px solid #808080;
                    text-align :left;
                    padding: 8px;"><%=data.Tables[0].Rows[i]["Provider"].ToString() %></td>
                    <td  style="border: 2px solid #808080;
                    text-align :left;
                    padding: 8px;"><%=data.Tables[0].Rows[i]["Descrption"].ToString() %> 
                        <%if (data.Tables[0].Rows[i]["UploadedFileName"].ToString() != string.Empty)
                            { %>
                        (<%=data.Tables[0].Rows[i]["UploadedFileName"].ToString() %>)
                        <%} %>
                    </td>
                </tr>
                <%}    }%>              
            </table>

            <p><b>Signature of audited practitioner:………………………………. <br /><br />Date:…………………………………</b></p>
        </div>

    </form>

</body>
</html>
