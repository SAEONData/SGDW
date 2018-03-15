<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="sgdw._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <br />

        <table border="1" style="border-collapse: collapse">
            <tr>
                <!--
                <td>id</td>
                <td>parent</td>
                -->
                <td>type</td>
                <td>name</td>
                <td>description</td>
                <td>link</td>
            </tr>
            <% WriteFiles(); %>
        </table>
        
    </div>
    </form>
</body>
</html>
