<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="calendario.aspx.vb" Inherits="calendario" ValidateRequest="false" EnableEventValidation="false" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="content_head" ContentPlaceHolderID="head_contentholder" runat="server">


    <link href="CSS/bootstrap.css" rel="stylesheet" />
    <link href="CSS/Custom-Cs.css" rel="stylesheet" />
    <link href="CSS/app.css" rel="stylesheet" />
    <link href="CSS/PACTS.css" rel="stylesheet" />
    
    <style type="text/css">
        .auto-style1 {
            height: 34px;
        }
    </style>
    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" ScriptMode="Release">
                <Scripts>
    <%--                 <asp:ScriptReference Path="Scripts/cancelPostBack.js" />
	--%>
                </Scripts>
    </asp:ScriptManager>
	
    <div class="contenedorprincipalcalendario" >


        <center>
            <table id="tblbasal" border="0" cellpadding="2" cellspacing="1" class="tablaprincipalcalendario" >
                <tr>
                    <th colspan="2" class="theader">CITAS POR CALENDARIO</th>
                    <asp:TextBox   ID="controlID" style="display:none" runat="server" ></asp:TextBox>                   
                    <asp:HiddenField  ID="lblnhc"  runat="server"/>
                    <asp:HiddenField ID="lblUsuario" runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="lblUnidadAtencion" runat="server"></asp:HiddenField>                    
                </tr>
                <tr style="text-align: right">
                    <td colspan="2">          
                        <asp:ImageButton ID="ib_ad" runat="server" ToolTip="ADULTOS" CausesValidation="False" ImageUrl="~/images/Adultos.png" />
                        <asp:ImageButton ID="ib_ped" runat="server" ToolTip="PEDIATRÍA" CausesValidation="False" ImageUrl="~/images/Pediatria.png" Visible="False" />
                    </td>
                </tr>
                <tr>
                    <td class="columnaunocalendario" >
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="pnl_calendar" runat="server" CssClass="calendar">
                                    <asp:Panel ID="pnl_monthSelector" runat="server" CssClass="calendarMonthSelector">
                                        <asp:DropDownList ID="ddl_month" runat="server" />
                                        <asp:DropDownList ID="ddl_year" runat="server" />&nbsp;
                                    <asp:Button runat="server" ID="btn_generar" Text="BUSCAR" CssClass="button" />
                                    </asp:Panel>
                                    <asp:Repeater ID="rpt_calendar" runat="server">
                                        <ItemTemplate>
                                            <asp:Panel ID="pnl_calendarDay" runat="server" CssClass="calendarDay">
                                                <asp:LinkButton ID="lnk_dayLink" runat="server" CssClass="linkday" />
                                                <asp:Literal ID="ltl_dayEvents" runat="server" />
                                            </asp:Panel>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <div style="clear: both; height: 0; overflow: hidden">&nbsp;</div>
                                    <!-- This is needed to force the container (inc. background) around all the days if Days are floated with CSS -->

                                    <div id="div_no_citas" runat="server" visible="false">
                                        <table class="tablahorariocitascalendario" >
                                            <tr>
                                                <td colspan="6" class="calendarMonthSelector" style="height: 24px; color: #ffffff; font-size: 10pt; text-align: center;">HORARIOS CITAS
                                                    <asp:Label ID="lbl_dia_horario" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="columnahorariolistacitas" >
                                                    <asp:Label ID="lbl_BH_header" runat="server" Text="Horario"></asp:Label>
                                                </td>
                                                <td class="columnaclinica2alistacitas" >
                                                    <asp:Label ID="lbl_BH_header2" runat="server" Text="2. AVE"></asp:Label>
                                                </td>
                                                <td class="columnadispoclinica2alistacitas" >
                                                    <asp:Label ID="lbl_BH_header3" runat="server" Text="Status Horario"></asp:Label>
                                                </td>
                                                <td class="columnaclinica1alistacitas" >
                                                    <asp:Label ID="lbl_BH_header4" runat="server" Text="1. AVE"></asp:Label>
                                                </td>
                                                <td class="columnadispoclinica1alistacitas" >
                                                    <asp:Label ID="lbl_BH_header5" runat="server" Text="Status Horario"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="coldatoshoralistacitas">
                                                    <asp:Label ID="lbl_B_1" runat="server" Text="07:00 - 08:00"></asp:Label>
                                                </td>
                                                <td class="coldatoscantidadpaclistacitas">
                                                    <asp:Label ID="lbl_B_1_citas" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_1_citas_status" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorarioclinica1alistacitas">
                                                    <asp:Label ID="lbl_B_1_citas_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_1_citas_status_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="coldatoshoralistacitas">
                                                    <asp:Label ID="lbl_B_2" runat="server" Text="08:00 - 09:00"></asp:Label>
                                                </td>
                                                <td class="coldatoscantidadpaclistacitas">
                                                    <asp:Label ID="lbl_B_2_citas" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_2_citas_status" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorarioclinica1alistacitas">
                                                    <asp:Label ID="lbl_B_2_citas_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_2_citas_status_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="coldatoshoralistacitas">
                                                    <asp:Label ID="lbl_B_3" runat="server" Text="09:00 - 10:00"></asp:Label>
                                                </td>
                                                <td class="coldatoscantidadpaclistacitas">
                                                    <asp:Label ID="lbl_B_3_citas" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_3_citas_status" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorarioclinica1alistacitas">
                                                    <asp:Label ID="lbl_B_3_citas_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_3_citas_status_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="coldatoshoralistacitas" >
                                                    <asp:Label ID="lbl_B_4" runat="server" Text="10:00 - 11:00"></asp:Label>
                                                </td>
                                                <td class="coldatoscantidadpaclistacitas" >
                                                    <asp:Label ID="lbl_B_4_citas" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas" >
                                                    <asp:Label ID="lbl_B_4_citas_status" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorarioclinica1alistacitas" >
                                                    <asp:Label ID="lbl_B_4_citas_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas" >
                                                    <asp:Label ID="lbl_B_4_citas_status_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="coldatoshoralistacitas">
                                                    <asp:Label ID="lbl_B_5" runat="server" Text="11:00 - 12:00"></asp:Label>
                                                </td>
                                                <td class="coldatoscantidadpaclistacitas">
                                                    <asp:Label ID="lbl_B_5_citas" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_5_citas_status" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorarioclinica1alistacitas">
                                                    <asp:Label ID="lbl_B_5_citas_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_5_citas_status_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="coldatoshoralistacitas">
                                                    <asp:Label ID="lbl_B_6" runat="server" Text="12:00 - 13:00"></asp:Label>
                                                </td>
                                                <td class="coldatoscantidadpaclistacitas">
                                                    <asp:Label ID="lbl_B_6_citas" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_6_citas_status" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorarioclinica1alistacitas";">
                                                    <asp:Label ID="lbl_B_6_citas_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_6_citas_status_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="coldatoshoralistacitas">
                                                    <asp:Label ID="lbl_B_7" runat="server" Text="13:00 - 14:00"></asp:Label>
                                                </td>
                                                <td class="coldatoscantidadpaclistacitas">
                                                    <asp:Label ID="lbl_B_7_citas" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_7_citas_status" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorarioclinica1alistacitas">
                                                    <asp:Label ID="lbl_B_7_citas_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_7_citas_status_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="coldatoshoralistacitas">
                                                    <asp:Label ID="lbl_B_8" runat="server" Text="14:00 - 15:00"></asp:Label>
                                                </td>
                                                <td class="coldatoscantidadpaclistacitas">
                                                    <asp:Label ID="lbl_B_8_citas" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_8_citas_status" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorarioclinica1alistacitas">
                                                    <asp:Label ID="lbl_B_8_citas_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_8_citas_status_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="coldatoshoralistacitas">
                                                    <asp:Label ID="lbl_B_9" runat="server" Text="15:00 - 16:00"></asp:Label>
                                                </td>
                                                <td class="coldatoscantidadpaclistacitas">
                                                    <asp:Label ID="lbl_B_9_citas" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_9_citas_status" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorarioclinica1alistacitas">
                                                    <asp:Label ID="lbl_B_9_citas_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td class="colstatushorariolistacitas">
                                                    <asp:Label ID="lbl_B_9_citas_status_hosp" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>

                                        </table>
                                    </div>
                                </asp:Panel>
                                <asp:Literal runat="server" ID="ltl_output" />
                                </td>
                   
             
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="txt_fecha" EventName="TextChanged" />
                                <asp:AsyncPostBackTrigger ControlID="txt_asi" EventName="TextChanged" />
                                <asp:AsyncPostBackTrigger ControlID="btn_buscar" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btn_limpiar" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
               



               
                    <td class="columnadoscalendario" >
                        <div id="div_Adultos" runat="server" visible="false" class="cita">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                <ContentTemplate>
                                    <table border="0" cellpadding="2" cellspacing="1" class="cita">

                                        <tr>
                                            <td colspan="5" class="calendarMonthSelector" style="height: 24px; color: #ffffff; font-size: 10pt; text-align: center;">INGRESO PROXIMA CITA ADULTOS
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnacodigoasititulo" >Número ASI:
                                            </td>
                                            <td colspan="4" style="background-color: #e9ecf1; padding: 0px; text-align: left;">
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
                                                            <asp:ImageButton ID="btn_buscar" runat="server" ToolTip="BUSCAR" CausesValidation="False" ImageUrl="~/images/search.png" TabIndex="2" />
                                                            <asp:Button ID="btn_search" runat="server" Text="Button" Visible="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr style="height: 24px;">
                                            <td class="columnacodigoasititulo" >Nombre:
                                            </td>
                                            <td colspan="4" style="background-color: #e9ecf1;">
                                                <input id="hd_idpaciente" type="hidden" runat="server" />
                                                <input id="hd_idsignosvitales" type="hidden" runat="server" />
                                                <input id="hd_idcitas" type="hidden" runat="server" />
                                                <asp:Label ID="lbl_nombre" runat="server" CssClass="paciente1"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnacodigoasititulo">DPI:

                                            </td>
                                            <td class="colgenerodatospaciente">
                                                <asp:Label ID="lbl_cedula" runat="server" ></asp:Label>
                                            </td>
                                            <td class="coledaddatospaciente">NHC Hospital:

                                            </td>
                                            <td class="colgenerodatospaciente">
                                                <asp:Label ID="lbl_numhopitalia" runat="server" ></asp:Label>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chb_DPI" runat="server" Text="Entrego DPI" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnacodigoasititulo" >Género:
                                            </td>
                                            <td class="colgenerodatospaciente" >
                                                <asp:Label ID="lbl_genero" runat="server"></asp:Label>
                                            </td>
                                            <td class="coledaddatospaciente" >Edad:
                                            </td>
                                            <td class="coledadtxtdatospaciente" colspan="2">
                                                <asp:Label ID="lbl_edad" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnacodigoasititulo">Telefono(s):
                                            </td>
                                            <td colspan="4" style="background-color: #e9ecf1;">
                                                <asp:Label ID="lbl_telefono" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr style="height: 46px;">
                                            <td class="columnacodigoasititulo">Dirección:
                                            </td>
                                            <td colspan="4" style="background-color: #e9ecf1;">
                                                <asp:Label ID="lbl_direccion" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr style="height: 25px;">
                                            <td class="columnacodigoasititulo">TyT:
                                            </td>
                                            <td colspan="4" style="background-color: #e9ecf1;">
                                                <asp:Label ID="lbl_tyt_pac" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5" style="padding-top: 2px; padding-bottom: 2px; padding-right: 2px; text-align: right;">
                                                <%-- *****************Botones de actualizacion de datos***************** --%>
                                                <asp:Button ID="BtnCitas" runat="server" Text="Citas" CssClass="button" />
                                                <asp:Button ID="BtnActDir" runat="server" Text="Dirección" CssClass="button" /> 
                                                <asp:Button ID="BtnActTel" runat="server" Text="Teléfonos" CssClass="button" />
                                                <asp:Button ID="BtnActTodo" runat="server" Text="Ambos" CssClass="button" />
                                            </td>
                                        </tr>
                                        <div id="DivResGD_DirTel_exito" runat="server" visible="false" >
                                        <tr>
                                            <td colspan="5">
                                            <asp:Label ID="lblResGD_DirTel" runat="server" Text="" CssClass="divgd_dirtelexito"></asp:Label>
                                            </td>
                                        </tr>
                                        </div>
                                        <div id="DivResGD_DirTel_error" runat="server" visible="false" >
                                        <tr>
                                            <td colspan="5">
                                            <asp:Label ID="lblResGD_DirTel_Error" runat="server" Text="" CssClass="divgd_dirtelerror"></asp:Label>
                                            </td>
                                        </tr>
                                        </div>
                                        <div id="divdir" runat="server" visible="false" class="divdirclass" >
                                        <%-- *********************Inicia Campos de Direcciones************************** --%>
                                            <tr>
                                               <td>
                                                   <asp:Label ID="lbldirecciontxt" runat="server" Text="Direccion" ></asp:Label>
                                               </td>
                                                <td colspan="4" >
                                                    <asp:TextBox ID="txtdireccion" runat="server" CssClass="txtdireccionclass"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblpais" runat="server" Text="Pais" CssClass="lbl_etiquetatelefonospaciente"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ListaPaises" runat="server" AutoPostBack="true" CssClass="ddl_direccioncascadapaciete" ></asp:DropDownList>

                                                </td>
                                                <td>
                                                    <asp:Label ID="lbldepartamento" runat="server" Text="Departamento"></asp:Label>
                                                </td>
                                                <td colspan="2">
                                                    <asp:DropDownList ID="ddl_ListaDepartamentos" runat="server" AutoPostBack="true" CssClass="ddl_direccioncascadapaciete"></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblmunicipio" runat="server" Text="Municipio"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddl_ListaMunicipios" runat="server" AutoPostBack="true" CssClass="ddl_direccioncascadapaciete"></asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblzona" runat="server" Text="Zonas"></asp:Label>
                                                </td>
                                                <td  colspan="2">
                                                    <asp:DropDownList ID="ddl_ListaZonas" runat="server" CssClass="ddl_direccioncascadapaciete"></asp:DropDownList>
                                                </td>
                                            </tr>
                                        </div>
                                        <div id="BtnsGuardaDir" runat="server" visible="false">
                                            <%-- ******************Botones para Guardar solo Direcciones******************** --%>
                                                <tr>
                                                    <td colspan="5" style="background-color: #544e41; padding-top: 2px; padding-bottom: 2px; padding-right: 2px; text-align: right;">
                                                        <asp:Button ID="Btn_DirCargaDatosActuales_Dir" runat="server" Text="Cargar Datos" CssClass="button" />
                                                        <asp:Button ID="Btn_DirGuardar" runat="server" Text="Guardar" CssClass="button" />
                                                        <asp:Button ID="Btn_DirCancelar" runat="server" Text="Cancelar" CssClass="button" />
                                                    </td>
                                                </tr>
                                            </div>
                                        <%-- *********************Finaliza Campos de Direcciones************************ --%>
                                            
                                        <div id="divtels" runat="server" visible="false" class="divdirclass">
                                        <%-- *********************Inicia Campos de Telefonos**************************** --%>    
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbltelefono1" runat="server" Text="Telefono 1" CssClass="lbl_etiquetatelefonospaciente"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txttelefono1" runat="server" CssClass="coltelefonosdatospaciente" MaxLength="8"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblpertenecetel1" runat="server" Text="Pertenece" CssClass="lbl_etiquetatelefonospaciente"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddl_pertenecetel1" runat="server" CssClass="ddl_pertenecedir" ></asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chbconocetx1" runat="server" Text="Conoce DX:" TextAlign="Left" CssClass="chkb_conocedx" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbltelefono2" runat="server" Text="Telefono 2" CssClass="lbl_etiquetatelefonospaciente"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txttelefono2" runat="server" CssClass="coltelefonosdatospaciente" MaxLength="8"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblpertenecetel2" runat="server" Text="Pertenece" CssClass="lbl_etiquetatelefonospaciente"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddl_pertenecetel2" runat="server" CssClass="ddl_pertenecedir"></asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chbconocetx2" runat="server" Text="Conoce DX:" TextAlign="Left" CssClass="chkb_conocedx" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbltelefono3" runat="server" Text="Telefono 3" CssClass="lbl_etiquetatelefonospaciente"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txttelefono3" runat="server" CssClass="coltelefonosdatospaciente" MaxLength="8"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblpertenecetel3" runat="server" Text="Pertenece" CssClass="lbl_etiquetatelefonospaciente"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddl_pertenecetel3" runat="server" CssClass="ddl_pertenecedir"></asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chbconocetx3" runat="server" Text="Conoce DX:" TextAlign="Left" CssClass="chkb_conocedx" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbltelefono4" runat="server" Text="Telefono 4" CssClass="lbl_etiquetatelefonospaciente"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txttelefono4" runat="server" CssClass="coltelefonosdatospaciente" MaxLength="8"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblpertenecetel4" runat="server" Text="Pertenece" CssClass="lbl_etiquetatelefonospaciente"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddl_pertenecetel4" runat="server" CssClass="ddl_pertenecedir"></asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chbconocetx4" runat="server" Text="Conoce DX:" TextAlign="Left" CssClass="chkb_conocedx" />
                                                    </td>
                                                </tr>
                                                <div id="BtnsGuardarTels" runat="server" visible="false">
                                                <%-- ******************Botones para Guardar solo Telefonos*************** --%>
                                                <tr>
                                                    <td colspan="5" style="background-color: #544e41; padding-top: 2px; padding-bottom: 2px; padding-right: 2px; text-align: right;" class="auto-style1">
                                                        <asp:Button ID="Btn_CargaDatosAct_Tel" runat="server" Text="Carga Datos" CssClass="button" />
                                                        <asp:Button ID="Btn_GuardarTel" runat="server" Text="Guardar" CssClass="button" />
                                                        <asp:Button ID="Btn_CancelarTel" runat="server" Text="Cancelar" CssClass="button" />
                                                    </td>
                                                </tr>
                                                </div>
                                                <div id="BtnsGuardarTodo" runat="server" visible="false">
                                                <%-- ******************Botones para Guardar Direcciones y Telefonos*************** --%>
                                                <tr>
                                                    <td colspan="5" style="background-color: #544e41; padding-top: 2px; padding-bottom: 2px; padding-right: 2px; text-align: right;">
                                                        <asp:Button ID="Btn_CargaDatosAct_Tod" runat="server" Text="Carga Datos" CssClass="button" />
                                                        <asp:Button ID="Btn_GuardarTodo" runat="server" Text="Guardar" CssClass="button" />
                                                        <asp:Button ID="Btn_CancelarTodo" runat="server" Text="Cancelar" CssClass="button" />
                                                    </td>
                                                </tr>
                                                </div>
                                        <%-- *********************Finaliza Campos de Telefonos*********************** --%>    
                                        </div>
                                        <div id="Div_Citas" runat="server" visible="false">
                                        <tr> 
                                            <td class="coltitulosdatoscitas" >Ultima:</td>
                                            <td style="border-top: solid 1px #f18103;">
                                                <asp:Label ID="lbl_visita" runat="server" CssClass="fechavisita"></asp:Label>
                                            </td>
                                            <td class="coltitulosdatoscitas" >*&nbsp;Próxima:</td>
                                            <td style="border-top: solid 1px #f18103; text-align: left;" colspan="2">
                                                <%--    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                                    <ContentTemplate>--%>
                                                <table border="0" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td style="background-color: #e9ecf1; text-align: center;">
                                                            <asp:TextBox ID="txt_fecha" runat="server" Style="width: 7.5em" AutoPostBack="True"></asp:TextBox>
                                                        </td>
                                                        <td style="background-color: #e9ecf1; text-align: left;">
                                                            <cc1:CalendarExtender ID="CalendarExtender1" runat="server" PopupButtonID="ibtn_calendario" TargetControlID="txt_fecha" Format="dd/MM/yyyy" CssClass="ajax__calendar"></cc1:CalendarExtender>
                                                            <asp:ImageButton ID="ibtn_calendario" runat="server" ImageUrl="~/images/datePickerPopupHover.gif" CausesValidation="False" BorderWidth="0" />
                                                            <asp:Label ID="lbl_feriado" runat="server" Visible="false" ></asp:Label>
                                                            <asp:Label ID="lbl_fechaNoDisponible" runat="server" Visible="false" ></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>

                                                <%--                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="coltitulosdatoscitas">

                                            </td>
                                            <td>

                                            </td>
                                            <td class="coltitulosdatoscitas"> *&nbsp;Mangua                                                
                                            </td>
                                            <td colspan="2" style="border-top: solid 1px #f18103;">                                                
                                                <asp:Label ID="lbl_fechaproximavisitaMangua" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="coltitulosdatoscitas" >*&nbsp;Día:</td>
                                            <td style="padding-left: 5px;" colspan="1">
                                                <asp:Label ID="lbl_dia_cita" runat="server" Text="" AutoPostBack="True"></asp:Label></td>
                                            <td class="coltitulosdatoscitas" >*&nbsp;Horario:</td>
                                            <td colspan="2">
                                                <asp:DropDownList ID="ddl_horario_cita" runat="server" AppendDataBoundItems="True" AutoPostBack="True">
                                                </asp:DropDownList>

                                                <asp:ImageButton ID="ib_check_horario" runat="server" ToolTip="VERIFICAR HORARIO" CausesValidation="False" ImageUrl="~/images/yes.png" Style="width: 16px" />

                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="coltitulosdatoscitas" >*&nbsp;Jornada:</td>
                                            <td>
                                                <asp:DropDownList ID="ddl_jornada" runat="server" AppendDataBoundItems="True">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="coltitulosdatoscitas" >*&nbsp;Clínica:</td>
                                            <td colspan="2">
                                                <asp:DropDownList ID="ddl_clinica" runat="server" AppendDataBoundItems="True">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <div id="divcircuito" runat="server" visible="false" >  
                                            <tr>
                                                <td class="coltitulosdatoscitas" >&nbsp;Circuito G:</td>
                                                <td style="padding-left: 5px; font-weight: bold;" colspan="4">
                                                    <asp:Label ID="lbl_circuito_est" runat="server" Text=""></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td class="coltitulosdatoscitas">&nbsp;Grupo:</td>
                                                <td style="padding-left: 5px; font-weight: bold;" colspan="1">
                                                    <asp:Label ID="lbl_circ_grupo" runat="server" Text=""></asp:Label></td>

                                                <td class="coltitulosdatoscitas">&nbsp;Periodo:</td>
                                                <td style="padding-left: 5px; font-weight: bold;" colspan="3">
                                                    <asp:Label ID="lbl_circ_periodo" runat="server" Text=""></asp:Label></td>
                                            </tr>
                                        </div>
                                        <tr>
                                            <td style="padding-left: 5px; font-weight: bold;" colspan="5">
                                                <asp:Label ID="lbl_horario_asignado" runat="server" Text="" Visible="False"></asp:Label>
                                            </td>
                                        </tr>



                                        <tr>

                                            <td colspan="5" style="background-color: #544e41; padding-top: 2px; padding-bottom: 2px; padding-right: 2px; text-align: right;">
                                                <asp:Button ID="btn_agregar" runat="server"
                                                    Text="Agregar" CssClass="button" />
                                              
                                                <asp:Button ID="btn_grabarcita" runat="server" CssClass="button" Visible="false" />
                                                <asp:Button ID="btn_limpiar" runat="server"
                                                    Text="Limpiar" CausesValidation="False"
                                                    CssClass="button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Literal runat="server" ID="ltl_error" /></td>
                                            <td colspan="3">
                                                <asp:Literal runat="server" ID="ltl_error2" /></td>
                                        </tr>
                                        </div>
                                    </table>
                                    
                                </ContentTemplate>
                                <%--    <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="txt_fecha" EventName="TextChanged" />
                                                </Triggers>--%>
                            </asp:UpdatePanel>
                        </div>
                    </td>

                </tr>
                <tr>
                    <td class="tdareapediatrica" ></td>
                </tr>
            </table>

            <div id="div_Pediatria" runat="server" visible="false">
            </div>

            <asp:UpdatePanel ID="prueba" runat="server">
                <ContentTemplate>
                    <asp:Button ID="BtnShowPopup" runat="server" Style="display: none;" />
                    <cc1:ModalPopupExtender ID="MPE_Alerta_CV" runat="server"
                        TargetControlID="BtnShowPopup"
                        PopupControlID="PnlPopup"
                        BehaviorID="poppop"
                        BackgroundCssClass="modalBackground">
                    </cc1:ModalPopupExtender>

                    <asp:Panel ID="PnlPopup" runat="server" CssClass="modalPopup" Style="display: block;">
                        <asp:UpdatePanel ID="UpdPnlDetalle" runat="server">
                            <ContentTemplate>
                                <div class="tddosareapediatricaprueba" >
                                    <table>
                                        <tr>
                                            <td style="text-align: center; font-size: 16px;">
                                                <asp:Label ID="lbl_alerta_cv_pac" runat="server" Text="¡ALERTA! Carga Viral" CssClass="error" Font-Bold="true" Font-Size="18px"></asp:Label>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: center">
                                                <asp:Button ID="Button3" runat="server"
                                                    Text="Aceptar" CssClass="button" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>


        </center>
    </div>

    <script type="text/javascript" src="Scripts/jquery-3.2.1.min.js"></script>
    <script type="text/javascript" src="Scripts/shim.js"></script>
    <script type="text/javascript" src="Scripts/websdk.client.bundle.min.js"></script>
    <script type="text/javascript" src="Scripts/fingerprint.sdk.min.js"></script>
    <script type="text/javascript" src="Scripts/identapp.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/bootbox.min.js"></script>
    <script type="text/javascript" src="Scripts/modalwindow.js"></script>
    <script type="text/javascript" src="Scripts/beforeCancelPostBack.js"></script>


    <script type="text/jscript">

        function copyData() {
            debugger;
            $("input[name*='lblnhc']")[0].value = $("input[name*='txt_asi']")[0].value;
        }

    </script>

    <script type="text/jscript"> 

        $(".NHC").on('change'  ,function(){       
             debugger;
            copyData();
        });
    </script>

    <script type="text/jscript"> 
        //$("input[type='image']").click(function ()
         $(document).on('click','input[type="image"]',function(ev)
         {
             debugger;
             if (!$(ev.target).is('#ib_check_horario')) {
                 debugger;
                 copyData();                   
              }             
        });
    </script>

     <script type="text/jscript">   
         $(document).on('click',"input[name*='btn_agregar']",function(ev)
         {
             debugger;
             copyData();                                         
        });
    </script>

    <script type="text/jscript">

        Sys.Browser.WebKit = {};

        debugger;
        if (navigator.userAgent.indexOf('WebKit/') > -1) {
            Sys.Browser.agent = Sys.Browser.WebKit;
            Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
            Sys.Browser.name = 'WebKit';
        }
    </script>
</asp:Content>
