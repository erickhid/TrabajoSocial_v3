<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="acceso.aspx.vb" Inherits="acceso" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div style="width: 700px; height:500px; border: solid 1px #5d7b9d; text-align:left;">
        <br />
        <br />
        <br />
        <br />
        <table border="0" cellpadding="0" cellspacing="0" width="700px">
            <tr>
                <td style="text-align:center;">
                    <center>
                    <div style="display:block; background-color:#CCCCCC; padding:12px; width:500px; border: 2px double #666666;">
                        <div style="border-bottom:solid 1px #FFFFFF; padding:4px;">
                            <img src="images/err.png" alt="" />&nbsp;<asp:label ID="lbl_errorT" runat="server" style="color:#231F20; font-size:14pt; font-weight:bold;">PAGINA NO AUTORIZADA</asp:label>
                        </div>
                        <div style="padding:4px;">
                            <asp:label ID="lbl_errorD" runat="server" style="color:#231F20; font-size:10pt; font-weight:bold;">Comuníquese con el Administrador.</asp:label>
                        </div>
                    </div>  
                    </center>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <br />
        <br />
    </div>
</asp:Content>

