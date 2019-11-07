<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NSW.aspx.cs" Inherits="Lexnarro.Reports.NSW" %>

<!DOCTYPE html>


<html>
<head>
    <title>NSW</title>
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
    <div style="width: auto; margin: 0 20%;">
        <form>
            <h2 style="color: #ffaf1a;">RECORD OF CPD ACTIVITIES</h2>

            <hr style="color: #ffa500; background-color: #ffa500; height: 5px;">
            <p style="color: #ffa500;">Please retain this for your records.</p>
            <hr style="color: #ffa500; background-color: #ffa500; height: 5px;">

            <p>Record of CPD activities for the CPD year from 1 April <%=yearFrom %> to 31 March <%=yearTo %></p>

            <hr style="color: #ffa500; background-color: #ffa500; height: 2px;">

            <b>
                <p style="color: #ffa500;">1.PRACTITIONER DETAILS</p>
            
            </b>&nbsp;

            First Name&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;Last Name<br />
            <input style="width: 48%; padding: 12px 20px; margin: 8px 0; display: inline-block; 
            border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box;" type="text" id="fname" name="SurName" placeholder="First Name:">
            <input style="width: 48%; padding: 12px 20px; margin: 8px 0; display: inline-block; border: 1px solid #ccc; 
            border-radius: 4px; box-sizing: border-box;" type="text" id="lname" name="FirstName" placeholder="Last Name:">
            <br />  &nbsp;Other Name&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Law Society Number<br />

            <input style="width: 48%; padding: 12px 20px; margin: 8px 0; display: inline-block; border: 1px solid #ccc; 
            border-radius: 4px; box-sizing: border-box;" type="text" id="oname" name="otherName">
            <input style="width: 48%; padding: 12px 20px; margin: 8px 0; display: inline-block; border: 1px solid #ccc; 
            border-radius: 4px; box-sizing: border-box;" type="text" id="lnumber" name="law">


            <table style="font-family: arial, sans-serif; border-collapse: collapse; width: 100%;">
                <tr style="border: 1px solid #dddddd; text-align: left; padding: 8px;font-size:small;">
                    <th style="border: 1px solid; text-align: left; padding: 8px;">Date</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">DESCRIPTION/TITLE OF ACTIVITY</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">CODE*</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">PROVIDER</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">UNITS CLAIMED</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">FIELD**</th>
                </tr>

                <% 
                    if (data.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i <= data.Tables[0].Rows.Count - 1; i++)
                        {%>
                <tr>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["Date"].ToString().Split(' ')[0]%>
                    </td>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["Descrption"].ToString()%>
                    </td>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["ActivityShortName"].ToString()%>
                    </td>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["Provider"].ToString()%>
                    </td>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["Units_Done"].ToString()%>
                    </td>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["Category Name"].ToString()%>
                    </td>
                </tr>
                <%}
                    } %>
            </table>
            <hr>
            <b>
                <p style="color: #ffa500;">2.CARRYOVER FROM PREVIOUS CPD YEAR</p>
            </b>
            <table style="font-family: arial, sans-serif; border-collapse: collapse; width: 100%;">
                <tr style="border: 1px solid #dddddd; text-align: left; padding: 8px;font-size:small;">
                    <th style="border: 1px solid; text-align: left; padding: 8px;">Date</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">DESCRIPTION/TITLE OF ACTIVITY</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">CODE*</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">PROVIDER</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">UNITS CLAIMED</th>
                    <th style="border: 1px solid; text-align: left; padding: 8px;">FIELD**</th>
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
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["Date"].ToString().Split(' ')[0]%>
                    </td>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["Activity Name"].ToString()%>
                    </td>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["ActivityShortName"].ToString()%>
                    </td>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["Provider"].ToString()%>
                    </td>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["Units_Done"].ToString()%>
                    </td>
                    <td style="border: 1px solid; text-align: left; padding: 8px; font-size: 10px">
                        <% =data.Tables[0].Rows[i]["Category Name"].ToString()%>
                    </td>
                </tr>
                <%
                    }%>

                <%}
                    } %>
            </table>
            <table style="font-size:smaller;">
                <tr>
                    <td>
                        <div>
                            <b>
                                <p>CODES* COURSES/ACTIVITIES</p>
                            </b>
                            <hr>
                            <b>CS	Seminar, workshop, lecture, conference</b> (1 unit per hour)
                          <hr>
                            <b>DG	In-house seminars/discussion group</b> (1 unit per hour)
                          <hr>
                            <b>O	Multimedia or web-based program</b> (1 unit per hour)
                          <hr>
                            <b>VA	Private study of audio/visual material</b> (1 unit per hour to maximum of 5 CPD units)
                          <hr>
                            <b>PP	Preparation and/or presentation of written or oral material to be used in a CPD activity or in other forms of education provided to solicitors</b> (1 unit per hour to maximum of 5 CPD units)
                          <hr>
                            <b>PU	Publications – research, preparation or editing of an article published or published Law Reports</b>
                            (1 unit for every 1000 words of the article and a maximum of 5 CPD units)
                         <hr>
                            <b>PG	Postgraduate studies relevant to a solicitor’s practice needs</b> (1 unit per hour)
                            <hr>
                            <b>CM	Reasonable attendance as a committee member of professional association, designated local authority or the Law Council of Australia or of other committees</b> (1 unit per 2 hours and a maximum of 3 CPD units)

                        </div>
                    </td>
                    <td></td>
                    <td></td>

                    <td valign="top">
                        <div>
                            <b>
                                <p>FIELD**</p>
                            </b>
                            <hr />
                            <p>Practitioners must complete 10 CPD units every CPD year including at least one unit every year in each of the following fields:</p>
                            <ol>
                                <li>Ethics and Professional Responsibility</li>

                                <li>Practice Management and Business Skills</li>

                                <li>Professional Skills</li>

                                <li>Substantive Law</li>
                            </ol>
                        </div>
                    </td>
                </tr>

                <tr>
                </tr>
            </table>
            <p style="font-size:small;">
                <b>ENQUIRIES:</b> LAW SOCIETY REGISTRY, The Law Society of New South Wales, 170 Phillip Street, Sydney NSW 2000 or DX 362 Sydney<br />
                <b>T:</b> +61 2 9926 0156 |<b>F:</b> +61 2 9926 0257 | <b>E:</b>registry@lawsociety.com.au | <b>W:</b>lawsociety.com.au | <b>ACN:</b>000 000 699 | <b>ABN:</b> 98 696 304 966
            </p>
        </form>
    </div>
    <script src="https://code.jquery.com/jquery-1.12.4.min.js"></script>
    <script>
        $(function () {
            $('#fname').val('<% = data.Tables[0].Rows[0]["FirstName"].ToString()%>');
            $('#fname').attr("disabled", "disabled");
            $('#lname').val('<% = data.Tables[0].Rows[0]["LastName"].ToString()%>');
            $('#lname').attr("disabled", "disabled");
            $('#oname').val('<% = data.Tables[0].Rows[0]["OtherName"].ToString()%>');
            $('#oname').attr("disabled", "disabled");
            $('#lnumber').val('<% = data.Tables[0].Rows[0]["LawSocietyNumber"].ToString()%>');
            $('#lnumber').attr("disabled", "disabled");
        });
    </script>
</body>
</html>
