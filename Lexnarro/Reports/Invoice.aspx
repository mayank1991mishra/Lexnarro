<%@ Page Language="C#" CodeBehind="Invoice.aspx.cs" Inherits="Lexnarro.EmailTemplate.Invoice" %>

<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>LEXNARRO INVOICE</title>
    <style type="text/css">
        .auto-style1 {
            height: 21px;
        }
    </style>
</head>
<body>
    <object data="Lexnarro.svg" type="image/svg+xml">
        <img src="yourfallback.jpg" />
    </object>

    <%--<img src="Lexnarro.svg" />--%>
    <form>
        <!--<p style="text-align: justify;">&nbsp;</p>-->
        <p>
            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Attention:
        &nbsp;&ldquo;<asp:Label ID="lbl_name" runat="server"><%=dt.Rows[0]["FirstName"].ToString() +" "+ dt.Rows[0]["LastName"].ToString()%></asp:Label>&ldquo;
        </p>

        <p style="margin-left: 0px;">
            <font size="6">
            &nbsp; &nbsp;
            INVOICE
        </font>
            <br />

            <span style="color: #7cb8d6">&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
            <asp:Label ID="lbl_invoiceNo1" runat="server"><%=dt.Rows[0]["Invoice_No"].ToString() %></asp:Label>
            </span>
            <br />

            &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
        <a href="#" style="color: #7cb8d6">admin@lexnarro.com.au</a>&nbsp; &nbsp; &nbsp; &nbsp;
        Date:
            <asp:Label ID="lbl_InvoiceDate" runat="server"><%=dt.Rows[0]["Payment_Date"].ToString() %></asp:Label>
        </p>
        &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
        &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
        &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
        &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
        &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
        &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
        &nbsp; &nbsp;&nbsp; 
        Invoice No:
            <asp:Label ID="Label1" runat="server"><%=dt.Rows[0]["Invoice_No"].ToString() %></asp:Label>

        <p>
            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       <%-- Invoice Number:
        <br />
            <asp:Label ID="lbl_invoiceNo2" runat="server"></asp:Label>
            <br />--%>
        Terms: <%=term %> Days
        </p>

        <table style="margin-left: auto; margin-right: auto;" border="1" width="620" cellpadding="0" cellspacing="0">
            <thead style="background-color: #357ca2; font-family: Arial; color: white; font-weight: bold; text-align: center">
                <tr style="">
                    <td width="247">
                        <p style="font-family: Arial; color: white; font-weight: bold">Description</p>
                    </td>
                    <td width="78">
                        <p>Quantity</p>
                    </td>
                    <td width="78">
                        <p>Unit Price</p>
                    </td>
                    <td width="78">
                        <p>Cost</p>
                    </td>
                </tr>
            </thead>

            <tbody style="text-align: center">
                <%
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {%>
                <tr>
                    <td width="247">Lex Narro <%=dt.Rows[i]["Plan"].ToString() %> Subscription
                    </td>
                    <td width="78">
                        <p>1</p>
                    </td>
                    <td width="78">
                        <span>$
                            <%=dt.Rows[i]["Amount"].ToString() %></span>
                    </td>
                    <td width="78">
                        <span>$
                            <%=dt.Rows[i]["Amount"].ToString() %></span>
                    </td>
                </tr>
                <% } %>

                <tr>
                    <td width="247" class="auto-style1">
                        <p>&nbsp;</p>
                    </td>
                    <td width="78" class="auto-style1">
                        <p>&nbsp;</p>
                    </td>
                    <td width="78" class="auto-style1">
                        <p>Subtotal</p>
                    </td>
                    <td width="78" class="auto-style1">
                        <span>$
                            <%=dt.Rows[0]["Amount"].ToString() %></span>
                    </td>
                </tr>
                <tr>
                    <td width="247">
                        <p>&nbsp;</p>
                    </td>
                    <td width="78">
                        <p>&nbsp;</p>
                    </td>
                    <td width="78">
                        <p>&nbsp;</p>
                    </td>
                    <td width="78"></td>
                </tr>
                <tr>
                    <td width="247">
                        <p>&nbsp;</p>
                    </td>
                    <td width="78">
                        <p>&nbsp;</p>
                    </td>
                    <td width="78">
                        <p>Total</p>
                    </td>
                    <td width="78">
                        <span>$
                            <%=dt.Rows[0]["Amount"].ToString() %>
                        </span>
                    </td>
                </tr>
            </tbody>
        </table>

        <p>
            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;
        </p>

        <p>
            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Thank you for your business.
        </p>

        <p>
            <br />
            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Sincerely yours,
        </p>

        <p>
            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Team at Lex Narro
        </p>

        <p>&nbsp;</p>
        <p>&nbsp;</p>
        <p>&nbsp;</p>
        <p>&nbsp;</p>
    </form>
</body>
</html>
