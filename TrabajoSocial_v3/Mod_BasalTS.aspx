<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Mod_BasalTS.aspx.vb" Inherits="Mod_BasalTS" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head_contentholder">
    <style type="text/css">
        .auto-style1 {
            height: 19px;
        }
    </style>
    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">



   

    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/FixFocus.js" />
        </Scripts>
    </asp:ScriptManager>


   

   

    <div style="width: 770px; border: solid 1px #5d7b9d; text-align: left;">
        <div class="floating-menu">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <asp:Button ID="btnGrabar" runat="server" Text="" CssClass="grabar" />
                    <%-- <asp:Button ID="btnEditar" runat="server" Text="" CssClass="editar" />--%>
                    <asp:Button ID="btnExport" runat="server" Text="" OnClick="btnExport_Click" CssClass="generaPDF" /><asp:Button ID="btn_abrir" runat="server" Text="" OnClientClick="" Visible="False" CssClass="imprimePDF" /><br />
                    <asp:Button ID="btnCancelar" runat="server" Text="" CssClass="cancelar" PostBackUrl="~/Mod_BasalTS.aspx" />

                </ContentTemplate>
            </asp:UpdatePanel>
            <%--                                                  <td style="background-color:#5D7B9D; font-weight:bold; color:#ffffff; text-align:center; width:162px;">Nivel Educativo</td>
                                                    <td style="background-color:#5D7B9D; font-weight:bold; color:#ffffff; text-align:center; width:150px;">Situación Laboral</td>
                                                    <td style="background-color:#5D7B9D; font-weight:bold; color:#ffffff; text-align:center; width:55px;">Ingreso</td>
                                                    <td style="background-color:#5D7B9D; font-weight:bold; color:#ffffff; text-align:center; width:75px;">Conoce Dx</td>--%>            <%--                                                <td style="text-align:center; width:35px;" class="GV_rowpad">
                                                    <asp:TextBox ID="TextBox2" runat="server" Width="20px" CssClass="texto2" TabIndex="6"></asp:TextBox>
                                                </td>--%>
        </div>
        <asp:UpdatePanel ID="up_basales" runat="server">
            <ContentTemplate>
                <asp:Label ID="lbl_error" runat="server" CssClass="error"></asp:Label>
                <asp:Label ID="lbl_id_basal_ts" runat="server" ForeColor="White"></asp:Label>
                <asp:Label ID="lbl_Grupo_Fam" runat="server" ForeColor="White"></asp:Label>
                <table id="tblbasal" runat="server" border="0" cellpadding="1" cellspacing="1" style="width: 770px;">
                    <tr>
                        <th colspan="6" style="background-color: #f18103; color: #ffffff; text-align: center; font-size: 13px;">CAMBIOS BASALES</th>
                    </tr>
                    <tr>
                        <th colspan="6" class="theader">BASALES TRABAJO SOCIAL</th>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Número ASI:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1; padding: 0px;">
                            <table id="tblNHC" runat="server" border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txt_asi" runat="server" CssClass="NHC" MaxLength="7" Width="64px"
                                            TabIndex="1" AutoPostBack="True"></asp:TextBox>
                                        <cc1:FilteredTextBoxExtender ID="txt_asi_FilteredTextBoxExtender" runat="server"
                                            TargetControlID="txt_asi" ValidChars="0123456789Pp">
                                        </cc1:FilteredTextBoxExtender>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="btn_buscar" runat="server" ToolTip="BUSCAR" CausesValidation="False" ImageUrl="~/images/search.png" TabIndex="2" AutoPostBack="True" Style="width: 20px" />
                                        <asp:ImageButton ID="btn_editar" runat="server" ToolTip="EDITAR" CausesValidation="false" ImageUrl="~/images/file_edit.png" Visible="False" />
                                        <asp:ImageButton ID="btn_agregar" runat="server" ToolTip="AGREGAR" CausesValidation="false" ImageUrl="~/images/add.png" Visible="False" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">NHC Hospitalaria:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_hospitalaria" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Número DPI:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_documento" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Nombre:
                        </td>
                        <td colspan="3" style="background-color: #e9ecf1;">
                            <input id="hd_idpaciente" type="hidden" runat="server" />
                            <asp:Label ID="lbl_nombre" runat="server" CssClass="paciente"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Estatus MANGUA:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_estatus" runat="server" CssClass="paciente"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Género:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_genero" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Fecha Nacimiento:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_nacimiento" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Edad:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_edad" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Pais Nacimiento:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_paisnac" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Depto. Nacimiento:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_deptonac" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Mun. Nacimiento:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_munnac" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Pais Residencia:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_paisres" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Depto. Residencia:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_deptores" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Mun. Residencia:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_munres" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Dirección:
                        </td>
                        <td colspan="5" style="background-color: #e9ecf1;">
                            <asp:Label ID="lbl_direccion" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Teléfono:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_telefono" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Móvil:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_movil" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Estado Civil:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_estadocivil" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Nivel Educativo:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_niveleducativo" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Años Completos:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_añoscompletos" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Orientación Sexual:
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1;">
                            <asp:Label ID="lbl_orientacionsex" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Ocupación u Oficio:
                        </td>
                        <td colspan="3" style="background-color: #e9ecf1;">
                            <asp:Label ID="lbl_ocupacion" runat="server"></asp:Label>
                        </td>
                        <td style="width: 120px; background-color: #f18103; color: #ffffff;">CIRCUITO:
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
                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff;">Fecha Estudio SocioEconómico
                        </td>
                        <td style="width: 136px; background-color: #e9ecf1; padding: 0px;" colspan="5">
                            <asp:TextBox ID="txt_fecha_ES" runat="server" Width="70px"></asp:TextBox>
                            <cc1:TextBoxWatermarkExtender ID="txt_fecha_ES_TBWE" runat="server" TargetControlID="txt_fecha_ES" WatermarkCssClass="wm" WatermarkText="dd/mm/aaaa">
                            </cc1:TextBoxWatermarkExtender>
                            <cc1:CalendarExtender ID="txt_txt_fecha_ES_CE" runat="server" Format="dd/MM/yyyy" TargetControlID="txt_fecha_ES">
                            </cc1:CalendarExtender>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>




        <div>


            <td>
                <div style="border: solid 1px #5d7b9d;">
                    <%--                                                <td style="text-align:center; width:146px;" class="GV_rowpad">
                                                    <asp:DropDownList ID="DropDownList3" runat="server" CssClass="datos" 
                                                        AutoPostBack="true" TabIndex="8">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align:center; width:55px;" class="GV_rowpad">
                                                    <asp:TextBox ID="TextBox3" runat="server" Width="40px" CssClass="texto2" TabIndex="9">0</asp:TextBox>
                                                    <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txt_Ingreso" ValidChars="0123456789.,">
                                                    </cc1:FilteredTextBoxExtender>
                                                </td>--%>
                    <asp:Panel ID="pnl_SE" runat="server">
                        <asp:UpdatePanel ID="up_pn1_SE" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <table id="tblpnl_SE" border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                                    <tr>
                                        <td colspan="6" class="theader">Estudios Socio Economicos </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: center">









                                            <asp:GridView ID="GV_pnl_SE" runat="server" ForeColor="#333333"
                                                EmptyDataText="No se existe estudio."
                                                Font-Names="Trebuchet MS" Font-Size="8pt" GridLines="None"
                                                CellPadding="0" CellSpacing="1" Width="770px" AutoGenerateColumns="False"
                                                ShowFooter="False" DataKeyNames="IdBasalesTS" TabIndex="3">
                                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Wrap="False" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="IdBasalesTS" ShowHeader="False" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="IdBasalesTS" runat="server" Text='<%# Bind("IdBasalesTS")%>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="NHC">
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblNHC" runat="server" Text='<%# Bind("NHC")%>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Fecha Estudio">
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblFechaEstudio" runat="server" Text='<%# Bind("FechaEstudio")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" Width="164px" CssClass="GV_rowpad" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Circuito">
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblCircuito" runat="server" Text='<%# Bind("Circuito")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" Width="92px" CssClass="GV_rowpad" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Observaciones">
                                                        <ItemTemplate>
                                                            <asp:Label ID="LblObservaciones" runat="server" Text='<%# Bind("Observaciones")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" Width="35px" CssClass="GV_rowpad" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Modificar">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="Check_box" runat="server" />
                                                        </ItemTemplate>
                                                        <ItemStyle CssClass="GV_rowpad" HorizontalAlign="Center" Width="50px" />
                                                    </asp:TemplateField>

                                                </Columns>
                                                <EmptyDataRowStyle Font-Bold="True" />
                                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                                                <EditRowStyle BackColor="#999999" />
                                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                            </asp:GridView>















                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </div>
            </td>


        </div>





        <%--<div>
            <asp:Panel ID="pnl_CircuitoAdherencia" runat="server">
                <table id="Table10" border="0" cellpadding="0" cellspacing="1" style="width: 770px;">
                    <tr>
                        <td>
                            <table id="Table11" border="0" cellpadding="0" cellspacing="0" style="width: 768px;">
                                <tr>
                                    <th class="theader5a" style="width: 20px;">
                                        <asp:ImageButton ID="ib_CircuitoAdherencia" ImageUrl="~/images/plus2.png" runat="server" BorderWidth="0" Height="10px" Width="10px" /></th>
                                    <th class="theader5">CIRCUITO DE ADHERENCIA</th>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>--%>
        <%--  <div>
            <asp:Panel ID="pnl_CircuitoAdherencia2" runat="server">
                <asp:UpdatePanel ID="up_CircuitoAdherencia" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table id="Table12" border="0" cellpadding="0" cellspacing="0" style="width: 770px;">
                            <tr>
                                <td>
                                    <asp:GridView ID="GV_CircuitoAdherencia" runat="server" ForeColor="#333333"
                                        EmptyDataText="No tiene Circuito de Adherencia"
                                        Font-Names="Trebuchet MS" Font-Size="8pt" GridLines="None"
                                        CellPadding="0" CellSpacing="1" Width="770px" AutoGenerateColumns="False"
                                        ShowFooter="False" DataKeyNames="IdCircuitoAdherencia" TabIndex="3">
                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Wrap="False" />
                                        <Columns>
                                            <asp:TemplateField ShowHeader="False" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_IdGrupoFamiliar" runat="server" Text='<%# Bind("IdGrupoFamiliar")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Fecha Circuito">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_fecha_circuit" runat="server" Text='<%# Bind("FechaCircuito")%>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="164px" CssClass="GV_rowpad" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Tema">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_tema_circuit" runat="server" Text='<%# Bind("TemaCircuito")%>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="92px" CssClass="GV_rowpad" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Problemas Identificados">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_prob_ident" runat="server" Text='<%# Bind("ProblemasIdentificados")%>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="35px" CssClass="GV_rowpad" />
                                            </asp:TemplateField>--%>
        <%-- <asp:TemplateField HeaderText="Nivel Educativo">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_NivelEducativo" runat="server" Text='<%# Bind("NivelEducativo")%>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="158px" CssClass="GV_rowpad" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Situación Laboral">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_SituacionLaboral" runat="server" Text='<%# Bind("SituacionLaboral")%>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="150px" CssClass="GV_rowpad" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Ingreso">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Ingreso" runat="server" Text='<%# Bind("Ingreso")%>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="55px" CssClass="GV_rowpad" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Conoce Dx">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_ConoceDx" runat="server" Text='<%# Bind("NomConoceDx")%>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="75px" CssClass="GV_rowpad" />
                                            </asp:TemplateField>--%>
        <%-- <asp:TemplateField ShowHeader="False">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="ibtEditar" runat="server" ImageUrl="~/images/file_edit.png" CommandName="Editar" ToolTip="Editar" />
                                                    <asp:ImageButton ID="ibtBorrar" runat="server" ImageUrl="~/images/delete.png" CommandName="Borrar" ToolTip="Borrar" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="right" Width="41px" CssClass="GV_rowpad" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataRowStyle Font-Bold="True" />
                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                                        <EditRowStyle BackColor="#999999" />
                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                        <EmptyDataTemplate>
                                            <table border="0" cellspacing="1" cellpadding="0" width="770px">
                                                <tr>
                                                    <td colspan="8" style="font-weight: bold; color: #333333; text-align: Left;">No tiene Circuito de Adherencia.</td>
                                                </tr>
                                                <tr>
                                                    <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 157px;">Fecha Circuito</td>
                                                    <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 95px;">Tema</td>
                                                    <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 80px;">Problemas Identificados</td>
        --%>  <%--                                                  <td style="background-color:#5D7B9D; font-weight:bold; color:#ffffff; text-align:center; width:162px;">Nivel Educativo</td>
                                                    <td style="background-color:#5D7B9D; font-weight:bold; color:#ffffff; text-align:center; width:150px;">Situación Laboral</td>
                                                    <td style="background-color:#5D7B9D; font-weight:bold; color:#ffffff; text-align:center; width:55px;">Ingreso</td>
                                                    <td style="background-color:#5D7B9D; font-weight:bold; color:#ffffff; text-align:center; width:75px;">Conoce Dx</td>--%>
        <%--<td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 41px;"></td>
                                                </tr>
                                            </table>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td style="border-top: 2px solid #5d7b9d; border-bottom: 2px solid #5d7b9d; background-color: #e9ecf1;">
                                    <div id="divingresoCircuito" runat="server" visible="false">
                                        <table border="0" cellspacing="1" cellpadding="0" width="770px">
                                            <tr>
                                                <td style="text-align: center; width: 159px" class="GV_rowpad">
                                                    <asp:TextBox ID="txt_fecha_circuit" runat="server" Width="85px" CssClass="texto" TabIndex="4"></asp:TextBox>
                                                </td>
                                                <td style="text-align: center; width: 75px;" class="GV_rowpad">
                                                    <asp:DropDownList ID="ddl_Tema_circuit" runat="server" CssClass="datos"
                                                        AutoPostBack="true" TabIndex="5">
                                                    </asp:DropDownList>
                                                </td>--%>
        <%--                                                <td style="text-align:center; width:35px;" class="GV_rowpad">
                                                    <asp:TextBox ID="TextBox2" runat="server" Width="20px" CssClass="texto2" TabIndex="6"></asp:TextBox>
                                                </td>--%>
        <%--<td style="text-align: center; width: 30px;" class="GV_rowpad">
                                                    <asp:DropDownList ID="ddl_problemas" runat="server" CssClass="datos"
                                                        AutoPostBack="true" TabIndex="7">
                                                    </asp:DropDownList>
                                                </td>--%>
        <%--                                                <td style="text-align:center; width:146px;" class="GV_rowpad">
                                                    <asp:DropDownList ID="DropDownList3" runat="server" CssClass="datos" 
                                                        AutoPostBack="true" TabIndex="8">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align:center; width:55px;" class="GV_rowpad">
                                                    <asp:TextBox ID="TextBox3" runat="server" Width="40px" CssClass="texto2" TabIndex="9">0</asp:TextBox>
                                                    <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txt_Ingreso" ValidChars="0123456789.,">
                                                    </cc1:FilteredTextBoxExtender>
                                                </td>--%>
        <%--                                                <td style="text-align:center; width:75px;" class="GV_rowpad">
                                                        <asp:DropDownList ID="DropDownList4" runat="server" CssClass="datos" 
                                                        AutoPostBack="true" TabIndex="10">
                                                    </asp:DropDownList>
                                                </td>--%>
        <%--<td style="text-align: right; width: 49px;" class="GV_rowpad">
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/add.png" OnClick="ibt_Agregar_Click" ToolTip="Agregar" TabIndex="11" Style="width: 16px; height: 16px;" />
                                                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/edit.png" OnClick="ibt_Modificar_Click" ToolTip="Modificar" Visible="false" TabIndex="12" />
                                                    <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/images/no.png" OnClick="ibt_Cancelar_Click" ToolTip="Cancelar" TabIndex="13" Style="width: 16px" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btn_buscar" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="txt_asi" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>--%>
        <div>
            <asp:Panel ID="pnl_GrupoFamiliar" runat="server">
                <table id="Table7" border="0" cellpadding="0" cellspacing="1" style="width: 770px;">
                    <tr>
                        <td>
                            <table id="Table8" border="0" cellpadding="0" cellspacing="0" style="width: 768px;">
                                <tr>
                                    <th class="theader" style="width: 20px;">
                                        <asp:ImageButton ID="ib_GrupoFamiliar" ImageUrl="~/images/plus2.png" runat="server" BorderWidth="0" Height="10px" Width="10px" /></th>
                                    <th class="theader4">GRUPO FAMILIAR CON QUIEN VIVE</th>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
        <div>
            <asp:Panel ID="pnl_GrupoFamiliar2" runat="server">
                <asp:UpdatePanel ID="up_grupofamiliar" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table id="tblgfamiliar" border="0" cellpadding="0" cellspacing="0" style="width: 770px;">
                            <tr>
                                <td>
                                    <div id="Scroll_Grupo">
                                        <asp:GridView ID="GV_GrupoFamiliar" runat="server" ForeColor="#333333"
                                            EmptyDataText="Vive solo o no se ha reportado Grupo Familiar."
                                            Font-Names="Trebuchet MS" Font-Size="8pt" GridLines="None"
                                            CellPadding="0" CellSpacing="1" Width="755px" AutoGenerateColumns="False"
                                            ShowFooter="False" DataKeyNames="IdGrupoFamiliar_Indice" TabIndex="3">
                                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Wrap="False" />
                                            <Columns>
                                                <asp:TemplateField ShowHeader="False" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_IdGrupoFamiliar_Indice" runat="server" Text='<%# Bind("IdGrupoFamiliar_Indice")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ShowHeader="False" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_IdGrupoFamiliar" runat="server" Text='<%# Bind("IdGrupoFamiliar")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nombre">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_Nombre" runat="server" Text='<%# Bind("Nombre") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="164px" CssClass="GV_rowpad" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo Relación">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_NomTipoRelacion" runat="server" Text='<%# Bind("TipoRelacion")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="92px" CssClass="GV_rowpad" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Edad">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_Edad" runat="server" Text='<%# Bind("Edad") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="35px" CssClass="GV_rowpad" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nivel Educativo">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_NivelEducativo" runat="server" Text='<%# Bind("NivelEducativo")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="158px" CssClass="GV_rowpad" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Situación Laboral">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_SituacionLaboral" runat="server" Text='<%# Bind("SituacionLaboral")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="150px" CssClass="GV_rowpad" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ingreso">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_Ingreso" runat="server" Text='<%# Bind("Ingreso")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="55px" CssClass="GV_rowpad" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Conoce Dx">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_ConoceDx" runat="server" Text='<%# Bind("NomConoceDx")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="75px" CssClass="GV_rowpad" />
                                                </asp:TemplateField>
                                                <asp:TemplateField ShowHeader="False">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="ibtEditar" runat="server" ImageUrl="~/images/file_edit.png" CommandName="Editar" ToolTip="Editar" />
                                                        <asp:ImageButton ID="ibtBorrar" runat="server" ImageUrl="~/images/delete.png" CommandName="Borrar" ToolTip="Borrar" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="right" Width="41px" CssClass="GV_rowpad" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataRowStyle Font-Bold="True" />
                                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                                            <EditRowStyle BackColor="#999999" />
                                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                            <EmptyDataTemplate>
                                                <table border="0" cellspacing="1" cellpadding="0" width="770px">
                                                    <tr>
                                                        <td colspan="8" style="font-weight: bold; color: #333333; text-align: Left;">Vive solo o no se ha reportado Grupo Familiar.</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 157px;">Nombre</td>
                                                        <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 95px;">Tipo Relación</td>
                                                        <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 35px;">Edad</td>
                                                        <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 162px;">Nivel Educativo</td>
                                                        <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 150px;">Situación Laboral</td>
                                                        <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 55px;">Ingreso</td>
                                                        <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 75px;">Conoce Dx</td>
                                                        <td style="background-color: #5D7B9D; font-weight: bold; color: #ffffff; text-align: center; width: 41px;"></td>
                                                    </tr>
                                                </table>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="border-top: 2px solid #5d7b9d; border-bottom: 2px solid #5d7b9d; background-color: #e9ecf1;">
                                    <div id="divingresoGF" runat="server" visible="false">
                                        <table border="0" cellspacing="1" cellpadding="0" width="770px">
                                            <tr>
                                                <td style="text-align: center; width: 155px;" class="GV_rowpad">
                                                    <asp:TextBox ID="txt_Nombre" runat="server" Width="140px" CssClass="texto" TabIndex="5"></asp:TextBox>
                                                </td>
                                                <td style="text-align: center; width: 95px;" class="GV_rowpad">
                                                    <asp:DropDownList ID="DDL_TipoRelacion" runat="server" CssClass="datos"
                                                        AutoPostBack="true" TabIndex="6">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: center; width: 35px;" class="GV_rowpad">
                                                    <asp:TextBox ID="txt_Edad" runat="server" Width="20px" CssClass="texto2" TabIndex="7"></asp:TextBox>
                                                </td>
                                                <td style="text-align: center; width: 158px;" class="GV_rowpad">
                                                    <asp:DropDownList ID="DDL_NivelEducativo" runat="server" CssClass="datos"
                                                        AutoPostBack="true" TabIndex="8">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: center; width: 146px;" class="GV_rowpad">
                                                    <asp:DropDownList ID="DDL_SituacionLaboral" runat="server" CssClass="datos"
                                                        AutoPostBack="true" TabIndex="9">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: center; width: 55px;" class="GV_rowpad">
                                                    <asp:TextBox ID="txt_Ingreso" runat="server" Width="40px" CssClass="texto2" TabIndex="10">0</asp:TextBox>
                                                    <cc1:FilteredTextBoxExtender ID="txt_Ingreso_FilteredTextBoxExtender" runat="server" TargetControlID="txt_Ingreso" ValidChars="0123456789.,">
                                                    </cc1:FilteredTextBoxExtender>
                                                </td>
                                                <td style="text-align: center; width: 75px;" class="GV_rowpad">
                                                    <asp:DropDownList ID="DDL_ConoceDx" runat="server" CssClass="datos"
                                                        AutoPostBack="true" TabIndex="11">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: right; width: 49px;" class="GV_rowpad">
                                                    <asp:ImageButton ID="ibt_Agregar" runat="server" ImageUrl="~/images/add.png" ToolTip="Agregar" TabIndex="11" Style="height: 16px" />
                                                    <asp:ImageButton ID="ibt_Modificar" runat="server" ImageUrl="~/images/edit.png" OnClick="ibt_Modificar_Click" ToolTip="Modificar" Visible="false" TabIndex="12" />
                                                    <asp:ImageButton ID="ibt_Cancelar" runat="server" ImageUrl="~/images/no.png" OnClick="ibt_Cancelar_Click" ToolTip="Cancelar" TabIndex="13" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btn_buscar" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="txt_asi" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>
        <table id="Table9" border="0" cellpadding="0" cellspacing="1" style="width: 770px;">
            <tr>
                <td>
                    <asp:Panel ID="pnl_sabe_diag" runat="server">
                        <asp:UpdatePanel ID="upd_pnl_sabe_diag" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table border="0" cellspacing="1" cellpadding="0" width="770px">
                                    <tr>
                                        <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Quienes saben su Dx.:</td>
                                        <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                            <asp:CheckBoxList ID="cbl_qsDx" runat="server" CellPadding="1" CellSpacing="1" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="8" TabIndex="14"></asp:CheckBoxList>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btn_buscar" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="GridView1" runat="server" ForeColor="#333333"
                        EmptyDataText="Vive solo o no se ha reportado Grupo Familiar."
                        Font-Names="Trebuchet MS" Font-Size="8pt" GridLines="None"
                        CellPadding="0" CellSpacing="1" Width="770px" AutoGenerateColumns="False"
                        ShowFooter="False" DataKeyNames="IdGrupoFamiliar" TabIndex="3">
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Wrap="False" />
                        <Columns>
                            <asp:TemplateField ShowHeader="False" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_IdGrupoFamiliar" runat="server" Text='<%# Bind("IdGrupoFamiliar")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Nombre">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_Nombre" runat="server" Text='<%# Bind("Nombre") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="164px" CssClass="GV_rowpad" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tipo Relación">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_NomTipoRelacion" runat="server" Text='<%# Bind("TipoRelacion")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="92px" CssClass="GV_rowpad" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Edad">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_Edad" runat="server" Text='<%# Bind("Edad") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="35px" CssClass="GV_rowpad" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Nivel Educativo">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_NivelEducativo" runat="server" Text='<%# Bind("NivelEducativo")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="158px" CssClass="GV_rowpad" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Situación Laboral">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_SituacionLaboral" runat="server" Text='<%# Bind("SituacionLaboral")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="150px" CssClass="GV_rowpad" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ingreso">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_Ingreso" runat="server" Text='<%# Bind("Ingreso")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="55px" CssClass="GV_rowpad" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Conoce Dx">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_ConoceDx" runat="server" Text='<%# Bind("NomConoceDx")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="75px" CssClass="GV_rowpad" />
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="False">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ibtEditar" runat="server" ImageUrl="~/images/file_edit.png" CommandName="Editar" ToolTip="Editar" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="right" Width="41px" CssClass="GV_rowpad" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataRowStyle Font-Bold="True" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                        <EditRowStyle BackColor="#999999" />
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <div>
            <asp:Panel ID="pnl_CondicionesVivienda" runat="server">
                <table id="Table5" border="0" cellpadding="0" cellspacing="1" style="width: 770px;">
                    <tr>
                        <td>
                            <table id="Table6" border="0" cellpadding="0" cellspacing="0" style="width: 768px;">
                                <tr>
                                    <th class="theader" style="width: 20px;">
                                        <asp:ImageButton ID="ib_CondicionesVivienda" ImageUrl="~/images/plus2.png" runat="server" BorderWidth="0" Height="10px" Width="10px" /></th>
                                    <th class="theader4">CONDICIONES DE VIVIENDA</th>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
        <div>
            <asp:Panel ID="pnl_CondicionesVivienda2" runat="server">
                <asp:UpdatePanel ID="up_pnl_cond_vivienda" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table id="Table1" border="0" cellpadding="1" cellspacing="1" style="width: 770px;">
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Tipo Vivienda:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <asp:CheckBoxList ID="cbl_tipovivienda" runat="server" CellPadding="1" CellSpacing="1" RepeatDirection="Horizontal" RepeatColumns="0" TabIndex="15"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Num. de Ambientes:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <asp:TextBox ID="txt_nambientes" runat="server" Width="20px" CssClass="texto2" TabIndex="16"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Servicios:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <asp:CheckBoxList ID="cbl_servicios" runat="server" CellPadding="1" CellSpacing="1" RepeatDirection="Horizontal" RepeatColumns="0" TabIndex="17"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Tipo Construcción:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <asp:CheckBoxList ID="cbl_tipoconstruccion" runat="server" CellPadding="1" CellSpacing="1" RepeatDirection="Horizontal" RepeatColumns="7" TabIndex="18"></asp:CheckBoxList>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btn_buscar" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>
        <div>
            <asp:Panel ID="pnl_IngresosEgresos" runat="server">
                <table id="Table3" border="0" cellpadding="0" cellspacing="1" style="width: 770px;">
                    <tr>
                        <td>
                            <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="width: 768px;">
                                <tr>
                                    <th class="theader" style="width: 20px;">
                                        <asp:ImageButton ID="ib_IngresosEgresos" ImageUrl="~/images/plus2.png" runat="server" BorderWidth="0" Height="10px" Width="10px" /></th>
                                    <th class="theader4">INGRESOS Y EGRESOS</th>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
        <div>
            <asp:Panel ID="pnl_IngresosEgresos2" runat="server">
                <asp:UpdatePanel ID="up_IngresosEgresos" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table id="Table2" border="0" cellpadding="1" cellspacing="1" style="width: 770px;">
                            <tr>
                                <td colspan="6" style="padding-left: 2px; text-align: left; font-weight: bold;" class="auto-style1">INGRESOS:</td>
                            </tr>
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Personal:</td>
                                <td style="width: 136px; background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_ingpersonal" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="19"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_ingresos_FilteredTextBoxExtender" runat="server" TargetControlID="txt_ingpersonal" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Hogar:</td>
                                <td colspan="3" style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_inghogar" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="20"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_inghogar_FilteredTextBoxExtender" runat="server" TargetControlID="txt_inghogar" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="6" style="padding-left: 2px; text-align: left; font-weight: bold;">EGRESOS:</td>
                            </tr>
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Vivienda:</td>
                                <td style="width: 136px; background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_egvivienda" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="21"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_egvivienda_FilteredTextBoxExtender" runat="server" TargetControlID="txt_egvivienda" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Energía Eléctrica:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_egeelectrica" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="22"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_egeelectrica_FilteredTextBoxExtender" runat="server" TargetControlID="txt_egeelectrica" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Agua Potable:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_egagua" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="23"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_egagua_FilteredTextBoxExtender" runat="server" TargetControlID="txt_egagua" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Cable:</td>
                                <td style="width: 136px; background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_egcable" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="24"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_egcable_FilteredTextBoxExtender" runat="server" TargetControlID="txt_egcable" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Teléfono:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_egtelefono" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="25"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_egtelefono_FilteredTextBoxExtender" runat="server" TargetControlID="txt_egtelefono" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Alimentación:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_egalimentacion" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="26"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_egalimentacion_FilteredTextBoxExtender" runat="server" TargetControlID="txt_egalimentacion" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Transporte:</td>
                                <td style="width: 136px; background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_egtransporte" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="27"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_egtransporte_FilteredTextBoxExtender" runat="server" TargetControlID="txt_egtransporte" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Educación:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_egeducacion" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="28"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_egeducacion_FilteredTextBoxExtender" runat="server" TargetControlID="txt_egeducacion" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Basura:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_egbasura" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="29"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_egbasura_FilteredTextBoxExtender" runat="server" TargetControlID="txt_egbasura" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Otros:</td>
                                <td colspan="5" style="width: 136px; background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="txt_egotros" runat="server" Width="75px" CssClass="texto4" AutoPostBack="True" TabIndex="30"></asp:TextBox>
                                                <cc1:FilteredTextBoxExtender ID="txt_egotros_FilteredTextBoxExtender" runat="server" TargetControlID="txt_egotros" ValidChars="0123456789.,">
                                                </cc1:FilteredTextBoxExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="padding-left: 2px; text-align: left; font-weight: bold;">ANÁLISIS FINANCIERO:</td>
                            </tr>
                            <tr>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Total Ingresos:</td>
                                <td style="width: 136px; background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:Label ID="lbl_totalingresos" runat="server" CssClass="texto3"></asp:Label></td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Total Egresos:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:Label ID="lbl_totalegresos" runat="server" CssClass="texto3"></asp:Label></td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Déficit/Súperavit:</td>
                                <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>Q.&nbsp;</td>
                                            <td>
                                                <asp:Label ID="lbl_deficit_superavit" runat="server" CssClass="texto3"></asp:Label></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <br />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btn_buscar" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </asp:Panel>
        </div>

        <asp:Panel ID="pnl_observaciones" runat="server">
            <asp:UpdatePanel ID="up_pnl_obs" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table border="0" cellspacing="1" cellpadding="1" width="770px" style="border-top: 2px solid #5d7b9d;">
                        <tr>
                            <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Problemas Identificados:</td>
                            <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                <asp:CheckBoxList ID="cbl_probidentificados" runat="server" CellPadding="1" CellSpacing="1" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="5" TabIndex="30"></asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <%--                <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left:4px;">Observaciones:</td>
                <td style="background-color: #e9ecf1; padding-left:2px; text-align:left;">
                    <asp:TextBox ID="txt_observaciones" runat="server" CssClass="texto" TextMode="MultiLine" Height="45" Width="630px"  AutoPostBack="True"></asp:TextBox>
                </td>--%>
                            <td style="width: 120px; background-color: #5d7b9d; color: #ffffff; padding-left: 4px;">Observaciones:</td>
                            <td style="background-color: #e9ecf1; padding-left: 2px; text-align: left;">
                                <asp:TextBox ID="txt_observaciones" runat="server" CssClass="texto" Height="45" Width="630px" TabIndex="30" MaxLength="150"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btn_buscar" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </asp:Panel>
        <%--    <table border="0" cellspacing="1" cellpadding="1" width="770px" >
            <tr>
                <td style="background-color:#544e41; padding-top:4px; padding-bottom:4px; padding-right:4px; text-align:right;">
                    <asp:Button ID="Button1" runat="server" 
                        Text="Agregar" onclick="btn_agregar_Click" 
                        TabIndex="31" CssClass="button" />&nbsp;
                    <asp:Button ID="btn_limpiar" runat="server" 
                        Text="Limpiar" CausesValidation="False" 
                        onclick="btn_limpiar_Click" CssClass="button" TabIndex="32" />
                </td>
            </tr>
        </table>--%>
        <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server"
            SuppressPostBack="true" ExpandedImage="~/images/minus2.png" TargetControlID="pnl_IngresosEgresos2"
            CollapseControlID="pnl_IngresosEgresos" ExpandControlID="pnl_IngresosEgresos" CollapsedImage="~/images/plus2.png"
            Collapsed="true" ImageControlID="ib_IngresosEgresos">
        </cc1:CollapsiblePanelExtender>
        <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender2" runat="server"
            SuppressPostBack="true" ExpandedImage="~/images/minus2.png" TargetControlID="pnl_CondicionesVivienda2"
            CollapseControlID="pnl_CondicionesVivienda" ExpandControlID="pnl_CondicionesVivienda" CollapsedImage="~/images/plus2.png"
            Collapsed="true" ImageControlID="ib_CondicionesVivienda">
        </cc1:CollapsiblePanelExtender>
        <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender3" runat="server"
            SuppressPostBack="true" ExpandedImage="~/images/minus2.png" TargetControlID="pnl_GrupoFamiliar2"
            CollapseControlID="pnl_GrupoFamiliar" ExpandControlID="pnl_GrupoFamiliar" CollapsedImage="~/images/plus2.png"
            Collapsed="true" ImageControlID="ib_GrupoFamiliar">
        </cc1:CollapsiblePanelExtender>
        <%-- <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender4" runat="server"
            SuppressPostBack="true" ExpandedImage="~/images/minus2.png" TargetControlID="pnl_CircuitoAdherencia2"
            CollapseControlID="pnl_CircuitoAdherencia" ExpandControlID="pnl_CircuitoAdherencia" CollapsedImage="~/images/plus2.png"
            Collapsed="true" ImageControlID="ib_CircuitoAdherencia">
        </cc1:CollapsiblePanelExtender>--%>
    </div>


    <script type="text/javascript" src="Scripts/jquery-3.2.1.min.js"></script>

   <script type="text/jscript">   
         $(document).on('click',"input[name*='Check_box']",function(ev)
         {         
             __doPostBack('ctl00$ContentPlaceHolder1$btn_buscar', '');                                       
        });
    </script>

</asp:Content>




