<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="ConsultaV.aspx.vb" Inherits="ConsultaV" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div style="text-align:center; width:700px; margin-left:auto; margin-right:auto;">
        <table id="Table1" runat="server" width="700px" style="text-align:left" border="0" cellpadding="2" cellspacing="1">
            <tr>
                <th class="theader" colspan="5" >Visitas Realizadas</th>
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
                    <div id="tbl_reporte" runat="server">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:Panel ID="pnl_reporte" runat="server" BorderColor="#5d7b9d" BorderStyle="solid" BorderWidth="0px" ScrollBars="Auto" Width="100%">
                                    <table style="border:0px; border-spacing:0px; width:700px;">
                                        <tr>
                                            <th style="text-align:center" class="theader">
                                                <asp:Label ID="lbl_tlistado" runat="server" Text=""></asp:Label><br />
                                                <asp:Label ID="lbl_gv" runat="server" Text=""></asp:Label>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Panel ID="pl_pacientes" runat="server" Height="475px" ScrollBars="Vertical" Width="700px">
                                                    <asp:GridView ID="GV_pacientes" runat="server" 
                                                        CellPadding="1" 
                                                        ForeColor="#333333" 
                                                        GridLines="both" 
                                                        BorderColor="#a0acc0" 
                                                        BorderStyle="solid" 
                                                        BorderWidth="1px" 
                                                        EnableModelValidation="True" 
                                                        Width="680px" 
                                                        ShowFooter="False"
                                                        DataKeyNames="NHC" 
                                                        AutoGenerateColumns ="false" >
                                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="No">
                                                                <ItemTemplate>
                                                                        <asp:Label ID="lbl_No" runat="server" Text='<%# Bind("nro")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="20px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Paciente">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_paciente" runat="server" Text='<%# Bind("Paciente") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="280px" HorizontalAlign="Left" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="280px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="NHC">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_nhc" runat="server" Text='<%# Bind("NHC") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Género">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_genero" runat="server" Text='<%# Bind("Genero") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Edad">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_edad" runat="server" Text='<%# Bind("Edad") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="40px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="40px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Visita">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_TVisita" runat="server" Text='<%# Bind("TVisita") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="160px" HorizontalAlign="Left" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="160px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ShowHeader="False">
                                                                <ItemTemplate>
                                                                    <div class='<%# imgVisita1(Eval("Visita").ToString())%>'><%# imgVisita(Eval("Visita").ToString())%></div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="20px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                                        <EditRowStyle BackColor="#999999" />
                                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                    </asp:GridView>
                                                </asp:Panel>
                                                <br />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btn_generar" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
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