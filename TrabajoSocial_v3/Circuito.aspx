<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Circuito.aspx.vb" Inherits="Circuito" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div style="width: 770px; border: solid 1px #5d7b9d; text-align:left;">
         <div class="floating-menu">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <asp:Button ID="btnGrabar" runat="server" Text="" CssClass="grabar" />
                    <%-- <asp:Button ID="btnEditar" runat="server" Text="" CssClass="editar" />--%>
                    <asp:Button ID="btnCancelar" runat="server" Text="" cssClass="cancelar" PostBackUrl="~/Circuito.aspx" />
                   
                </ContentTemplate>
        </asp:UpdatePanel>
       <%-- <div class="floating-menu">
            <asp:Button ID="btnGrabar" runat="server" Text="" CssClass="grabar" />
           <%-- <asp:Button ID="btnAgregar" runat="server" Text="" onclick="btnAgregar_Click" CssClass="grabar" />--%>
<%--            <asp:Button ID="btnModificar" runat="server" Text="" onclick="btnModificar_Click" CssClass="actualizar" />
            <asp:Button ID="btnLimpiar" runat="server" Text="" onclick="btnLimpiar_Click" cssClass="cancelar" />--%>
        </div>
        <asp:UpdatePanel ID="up_basales" runat="server">
            <ContentTemplate>
                <asp:Label ID="lbl_error" runat="server" CssClass="error"></asp:Label>
                <asp:Label ID="lbl_id_circuito_ts" runat="server"></asp:Label>
                <table id="tblcircuito" border="0" cellpadding="1" cellspacing="1" style="width:770px;">

                    <tr>
                        <th colspan="6" class="theader">INFORMACIÓN BASAL</th>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Número CFLAG:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1; padding:0px;">
                            <table id="tblNHC" border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txt_asi" runat="server" CssClass="NHC" MaxLength="7" Width="64px"
                                            TabIndex="1" AutoPostBack="True"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="txt_asi_FilteredTextBoxExtender" runat="server"
                                            TargetControlID="txt_asi" ValidChars="0123456789Pp">
                                        </cc1:FilteredTextBoxExtender>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="btn_buscar" runat="server" ToolTip="BUSCAR" CausesValidation="False" ImageUrl="~/images/search.png" TabIndex="2"   AutoPostBack="True"/>
                                        <asp:ImageButton ID="btn_editar" runat="server" ToolTip="EDITAR" CausesValidation="false" ImageUrl="~/images/file_edit.png" Visible="False" />
                                        <asp:ImageButton ID="btn_agregar" runat="server" ToolTip="AGREGAR" CausesValidation="false" ImageUrl="~/images/add.png" Visible="False" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            NHC Hospitalaria:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_hospitalaria" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Número DPI:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_documento" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Nombre:
                        </td>
                        <td colspan="3" style="background-color: #e9ecf1;">
                            <input id="hd_idpaciente" type="hidden" runat="server" />
                            <asp:Label ID="lbl_nombre" runat="server" CssClass="paciente"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Estatus MANGUA:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_estatus" runat="server" CssClass="paciente"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Género:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_genero" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Fecha Nacimiento:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_nacimiento" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Edad:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_edad" runat="server"></asp:Label>
                        </td>
                     </tr>
                   <%-- <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Pais Nacimiento:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_paisnac" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Depto. Nacimiento:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_deptonac" runat="server"></asp:Label>
                        </td>
                         <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Mun. Nacimiento:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_munnac" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Pais Residencia:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_paisres" runat="server"></asp:Label>
                        </td>--%>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Etnia:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_etnia" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Depto. Residencia:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;" colspan="3">
                            <asp:Label ID="lbl_deptores" runat="server"></asp:Label>
                        </td>
                       <%-- <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Mun. Residencia:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_munres" runat="server"></asp:Label>
                        </td>
                    </tr>--%>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Dirección:
                        </td>
                        <td colspan="5" style="background-color: #e9ecf1;">
                            <asp:Label ID="lbl_direccion" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Teléfono:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_telefono" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Móvil:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;" colspan="3">
                            <asp:Label ID="lbl_movil" runat="server"></asp:Label>
                        </td>
                        <%--<td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Estado Civil:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_estadocivil" runat="server"></asp:Label>
                        </td>
                    </tr>--%>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Nivel Educativo:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_niveleducativo" runat="server"></asp:Label>
                        </td>
                       <%-- <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Años Completos:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_añoscompletos" runat="server"></asp:Label>
                        </td>--%>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Orientación Sexual:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;" colspan ="3">
                            <asp:Label ID="lbl_orientacionsex" runat="server"></asp:Label>
                        </td>
                    </tr>
                   <%-- <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Ocupación u Oficio:
                        </td>
                        <td colspan="3" style="background-color: #e9ecf1;">
                            <asp:Label ID="lbl_ocupacion" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #f18103; color: #ffffff;">
                            CIRCUITO:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:DropDownList ID="ddl_circuito" runat="server" CssClass="ddl">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="0">NO</asp:ListItem>
                                <asp:ListItem Value="1">SI</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">
                            Fecha Estudio SocioEconómico
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1; padding:0px;" colspan ="5">
                            <asp:TextBox ID="txt_fecha_ES" runat="server" Width="70px"></asp:TextBox>
                            <cc1:TextBoxWatermarkExtender ID="txt_fecha_ES_TBWE" runat="server" TargetControlID="txt_fecha_ES" WatermarkCssClass="wm" WatermarkText="dd/mm/aaaa">
                            </cc1:TextBoxWatermarkExtender>
                            <cc1:CalendarExtender ID="txt_txt_fecha_ES_CE" runat="server" Format="dd/MM/yyyy" TargetControlID="txt_fecha_ES">
                            </cc1:CalendarExtender>
                        </td>
                    </tr>--%>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div>
            <asp:Panel ID="pnl_CircuitoAdherencia" runat="server">
                <table id="Table10" border="0" cellpadding="0" cellspacing="1" style="width:770px;">
                    <tr>
                        <td>
                            <table id="Table11" border="0" cellpadding="0" cellspacing="0" style="width:768px;">
                                <tr>
                                    <%--<th class="theader5a" style="width:20px;"><asp:ImageButton ID="ib_CircuitoAdherencia" ImageUrl="~/images/plus2.png" runat="server" BorderWidth="0"  Height="10px" Width="10px"/></th>--%>
                                    <th class="theader5" style="padding-left:4px">CIRCUITO DE ADHERENCIA GRUPAL</th>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
        <div>
            <asp:Panel ID="pnl_circuito_general" runat="server">
                <asp:UpdatePanel ID="up_circuito_general" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table id="tbl_circ_general" border="0" cellpadding="0" cellspacing="1" style="width:770px;">
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left:3px;">
                                    Circuito:
                                </td>
                                         <td style="width: 136px; background-color: #e9ecf1;" colspan="10">
                                            <asp:DropDownList ID="ddl_circuito" runat="server" CssClass="datos">
                                        </asp:DropDownList>
                                </td>

                            </tr>
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left:3px;">
                                    Año Circuito:
                                </td>
                                         <td style="width: 136px; background-color: #e9ecf1;" colspan="1">
                                            <asp:DropDownList ID="ddl_año_circuito" runat="server" CssClass="datos">
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem Value="2015"></asp:ListItem>
                                                <asp:ListItem value="2016"></asp:ListItem>
                                        </asp:DropDownList>
                                </td>

                                 <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left:3px;" colspan="1">
                                    Período Circuito:
                                </td>
                                         <td style="width: 136px; background-color: #e9ecf1;" colspan="5">
                                            <asp:DropDownList ID="ddl_periodo_cir" runat="server" CssClass="datos">
                                        </asp:DropDownList>
                                </td>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left:3px;" colspan="1">
                                    Grupo:
                                </td>
                                         <td style="width: 136px; background-color: #e9ecf1;" colspan="5">
                                            <asp:DropDownList ID="ddl_grupo_cir" runat="server" CssClass="datos">
<%--                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem Value="1">GRUPO 1</asp:ListItem>
                                                <asp:ListItem value="2">GRUPO 2</asp:ListItem>
                                                <asp:ListItem value="3">GRUPO 3</asp:ListItem>
                                                <asp:ListItem value="4">GRUPO 4</asp:ListItem>--%>

                                        </asp:DropDownList>
                                </td>

                            </tr>
<%--                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left:3px;">
                                    Observaciones:
                                </td>
                                <td style="width: 136px; background-color: #e9ecf1; padding:0px;" colspan ="10">
                                    <asp:TextBox ID="txt_observaciones" runat="server" Width="600px"  MaxLength="100"></asp:TextBox>
                                </td>
                            </tr>--%>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>

</asp:Content>

