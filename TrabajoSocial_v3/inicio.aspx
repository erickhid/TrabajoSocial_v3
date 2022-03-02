<%@ Language="VB" AutoEventWireup="false" CodeFile="inicio.aspx.vb" Inherits="inicio" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Unidad de Trabajo Social - ASI</title>
    <link rel="shortcut icon" href="favicon.ico" />
    <link href="CSS/TS.css" rel="stylesheet" type="text/css" />
    <link href="CSS/dropdown.css" rel="stylesheet" media="all" type="text/css" />
    <link href="CSS/Avanzado.css" rel="stylesheet" media="all" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <center>
            <div style="width:762px; border:solid 5px #e8e4db;display:block; position:relative; left:auto; right:auto; background-color:#e8e4db;">
                <div style="padding:8px 5px 8px 5px; width:750px; border:solid 1px #aba392; background-color:#FFFFFF; display:block;">
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <table border="0" cellpadding="2" cellspacing="2" style="background-color:#f7f6f3; padding:4px; border:solid 1px #aba392;">
                        <tr>
                            <td style="text-align:center; background-color:#ffffff;">
                                <asp:Image ID="img_logo" runat="server" ImageUrl="~/images/logo_ASI.png" Width="150px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table border="0" cellpadding="2" cellspacing="2">
                                    <tr>
                                        <td align="center" colspan="4" style="color:White;background-color:#544e41;font-size:10pt;font-weight:bold;height:25px;">
                                        ::. CONTROL UNIDAD DE TRABAJO SOCIAL .::</td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="UserNameLabel" runat="server" Text="Usuario:" Font-Bold="true" />
                                            </td>
                                        <td align="left">
                                            <asp:Label ID="PasswordLabel" runat="server" Text="Clave:" Font-Bold="true" />
                                            </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="UserName" runat="server" CssClass="datos" Width="100px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                                                ControlToValidate="UserName"
                                                ErrorMessage="*" 
                                                ToolTip="Usuario Requerido."/>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="Password" runat="server" CssClass="datos" TextMode="Password" 
                                                Width="100px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                                                ControlToValidate="Password"
                                                ErrorMessage="*" 
                                                ToolTip="Clave Requerida." />
                                        </td>
                                        <td align="right">
                                            <asp:Button ID="LoginButton" runat="server" CssClass="button" Text="CONECTAR" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" style="color:Red;">
                                            <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                </div>
            </div>
        </center>
    </form>
</body>
</html>