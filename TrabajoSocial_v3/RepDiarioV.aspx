<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="RepDiarioV.aspx.vb" Inherits="RepDiarioV" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True"></asp:ScriptManager>
    <div style="text-align:center; width:700px; margin-left:auto; margin-right:auto;">
        <table id="Table1" runat="server" width="700px" style="text-align:left" border="0" cellpadding="2" cellspacing="1">
            <tr>
                <th class="theader" colspan="5" >Reporte de Visitas</th>
            </tr>
            <tr>
                <td style="width:75px; padding-left:10px; background-color: #5d7b9d; color: #ffffff;">
                Fecha:
                </td>
                <td style="width:470px; background-color: #e9ecf1; text-align:Left;">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <table border="0" cellpadding="0" cellspacing="0" width="485px">
                                <tr>
                                    <td style="width:80px; background-color: #e9ecf1; text-align:center;">
                                        <asp:TextBox ID="txt_fecha" runat="server" style="width:60px"></asp:TextBox>
                                    </td>
                                    <td style="background-color: #e9ecf1; text-align:left;" >
                                        <cc1:CalendarExtender ID="CalendarExtender1" runat="server" PopupButtonID="ibtn_calendario" TargetControlID="txt_fecha" Format="dd/MM/yyyy" CssClass="ajax__calendar"></cc1:CalendarExtender>
                                        <asp:ImageButton ID="ibtn_calendario" runat="server" ImageUrl="~/images/datePickerPopupHover.gif" CausesValidation="False"/>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td style="width:110px; text-align:center;">
                    <asp:Button runat="server" ID="btn_generar" Text="GENERAR" CssClass="button" TabIndex="2" />
                </td>
                <td style="width:20px; text-align:center;">
                    <asp:UpdateProgress ID="UP_procesar" runat="server" DisplayAfter="100">
                        <ProgressTemplate>
                            <div>
                                <asp:Image ID="iloader1" runat="server" ImageUrl="~/images/ajax-loader1.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                 </td>
                 <td style="width:22px; text-align:center;">
                    <asp:ImageButton ID="IB_exportar" runat="server" ImageUrl="~/images/excel.png" ToolTip="Exportar a Excel" Height="20px" />
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="tbl_reporte" runat="server"> 
                                <asp:Panel ID="pnl_reporte" runat="server" BorderColor="#5d7b9d" BorderStyle="solid" BorderWidth="0px" ScrollBars="Auto" Width="100%">
                                    <table style="border:0px; border-spacing:0px; width:700px;">
                                        <tr>
                                            <th style="text-align:center" class="theader">
                                                <asp:Label ID="lbl_titulo" runat="server"></asp:Label>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Panel ID="pl_pacientes" runat="server" Height="500px" ScrollBars="Both" Width="700px">
                                                    <asp:GridView ID="GridView1" runat="server"  
                                                        CellPadding="0" 
                                                        ForeColor="#333333" 
                                                        AutoGenerateColumns="False"
                                                        GridLines="both" BorderColor="#a0acc0" BorderStyle="solid" BorderWidth="1px" 
                                                        EnableModelValidation="True" Width="1100px" ShowFooter="False">
                                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" CssClass="GV_rowpadREP1"/>
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="No">
                                                                <ItemTemplate>
                                                                     <asp:Label ID="lbl_No" runat="server" Text='<%# Bind("nro")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="20px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Nombre">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_Nombre" runat="server" Text='<%# Bind("Nombre") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="290px" HorizontalAlign="Left" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="290px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Género">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_genero" runat="server" Text='<%# Bind("Genero")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="60px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="60px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="No. Hosptilario">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_no_hosp" runat="server" Text='<%# Bind("NumHospitalaria")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="90px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="90px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="NHC">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_nhc" runat="server" Text='<%# Bind("NHC")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="40px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="40px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Edad">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_edad" runat="server" Text='<%# Bind("Edad")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="40px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="40px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Última V.">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_ultima_visita" runat="server" Text='<%# Bind("Ultima_Visita")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="75px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="75px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Tiempo Ultima V.">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_tiempo_ult_vi" runat="server" Text='<%# Bind("Tiempo_Ultima_Visita")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="140px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="140px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Tiempo Días">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_tiempo_dias" runat="server" Text='<%# Bind("Tiempo_Dias")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Jornada">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_jornada" runat="server" Text='<%# Bind("Jornada")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="60px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="60px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                             <asp:TemplateField HeaderText="TyT">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_tyt_pac" runat="server" Text='<%# Bind("Clasificación_Pac")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="60px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Horario">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_horario" runat="server" Text='<%# Bind("Horario")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="60px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Clínica">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_Clinica" runat="server" Text='<%# Bind("Clinica")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="90px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="90px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="FechaUltimoCD4">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_FechaUltimoCD4" runat="server" Text='<%# Bind("FechaUltimoCD4")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="UltimoCD4">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_UltimoCD4" runat="server" Text='<%# Bind("UltimoCD4")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="50px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="50px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" CssClass="GV_headerREP1" HorizontalAlign="Center" />
                                                        <PagerStyle BackColor="#aba392" ForeColor="#333333" HorizontalAlign="Right" />
                                                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" CssClass="GV_headerREP1" />
                                                        <EditRowStyle BackColor="#999999" />
                                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" CssClass="GV_rowpadREP1" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                           </td>
                                       </tr>
                                   </table>
                                </asp:Panel>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btn_generar" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td> 
            </tr>
            <tr>
                <td colspan="5">
                    <asp:Label ID="lbl_error" runat="server" CssClass="error"></asp:Label>
                </td>
            </tr>
         </table>
    </div>
</asp:Content>
