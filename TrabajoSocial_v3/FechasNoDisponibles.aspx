<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="FechasNoDisponibles.aspx.vb" Inherits="ConsultaV" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="content_head" ContentPlaceHolderID="head_contentholder" runat="server">


    <link href="CSS/bootstrap.css" rel="stylesheet" />
    <link href="CSS/Custom-Cs.css" rel="stylesheet" />
    <link href="CSS/app.css" rel="stylesheet" />
    <link href="CSS/PACTS.css" rel="stylesheet" /> 
    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"> 
    </asp:ScriptManager>
    <div style="text-align:center; width:700px; margin-left:auto; margin-right:auto;">
        <table id="Table1" runat="server" width="700px" style="text-align:left" border="0" cellpadding="2" cellspacing="1">
            <tr>
                <th class="theader" colspan="5" >Fechas No Disponibles para Citas</th>
            </tr>
            <tr>
                <td style="width:75px; padding-left:10px; background-color: #5d7b9d; color: #ffffff;">
                Fecha:
                </td>
                <td style="width:100px; background-color: #e9ecf1; text-align:Left;">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <table border="0" cellpadding="0" cellspacing="0" width="450px">
                                <tr>
                                    <td style="width:80px; background-color: #e9ecf1; text-align:center;">
                                        <asp:TextBox ID="txt_fecha" runat="server" style="width:60px"></asp:TextBox>
                                    </td>
                                    <td style="background-color: #e9ecf1; text-align:left;" >
                                        <cc1:CalendarExtender ID="CalendarExtender1" runat="server" PopupButtonID="ibtn_calendario" TargetControlID="txt_fecha" Format="dd/MM/yyyy" CssClass="ajax__calendar"></cc1:CalendarExtender>
                                        <asp:ImageButton ID="ibtn_calendario" runat="server" ImageUrl="~/images/datePickerPopupHover.gif" CausesValidation="False"/>
                                    </td>
                                    <td style="width:75px; padding-left:10px; background-color: #5d7b9d; color: #ffffff;">
                                        Descripción:
                                    </td>
                                    <td style="width:250px; background-color: #e9ecf1; text-align:Left;">
                                        <asp:TextBox ID="txt_descri" runat="server" Width="100%" ></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td style="width:110px; text-align:center;">
                    <asp:Button runat="server" ID="btn_generar" Text="AGREGAR" CssClass="button" TabIndex="2" />
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
                                                <asp:Panel ID="pl_pacientes" runat="server" ScrollBars="Vertical" Width="700px">
                                                    <asp:GridView ID="GV_fechasnd" runat="server" 
                                                        CellPadding="1" 
                                                        ForeColor="#333333" 
                                                        BorderColor="#A0ACC0" 
                                                        BorderStyle="Solid" 
                                                        BorderWidth="1px" 
                                                        Width="680px"
                                                        DataKeyNames="Descripcion" 
                                                        AutoGenerateColumns ="False" >
                                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Seleccionar">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chk_eliminar" runat="server" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="20px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="No">
                                                                <ItemTemplate>
                                                                        <asp:Label ID="lbl_No" runat="server" Text='<%# Bind("nro")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="20px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Descripción">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_descripcion" runat="server" Text='<%# Bind("Descripcion") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="280px" HorizontalAlign="Left" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="280px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Fecha">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_fecha" runat="server" Text='<%# Bind("FechaNoDisponible") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="IdFechaND" Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_IdFechaND" runat="server" Text='<%# Bind("IdCitaNoDisponible") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
                                                                <HeaderStyle Width="80px" HorizontalAlign="Center" CssClass="GV_rowpad" />
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
                                    <div>
                                        <table>
                                            <tr>
                                                <td colspan="5" align="right" style="width:650px;">
                                                        <asp:Button ID="btn_Eliminar" runat="server" Text="Eliminar" CssClass="button" />   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="5">
                                                    <asp:Label ID="lbl_error" runat="server" CssClass="divgd_dirtelerror"></asp:Label>
                                                </td>
                                            </tr>                                           
                                        </table>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btn_generar" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btn_Eliminar" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </td> 
            </tr>
         </table>
    </div>
</asp:Content>