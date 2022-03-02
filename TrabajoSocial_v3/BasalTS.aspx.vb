Imports System.Data
Imports System.Text
Imports System.IO
Imports itextsharp.text
Imports itextsharp.text.html.simpleparser
Imports itextsharp.text.pdf
Imports itextsharp
'Imports System.Linq

Partial Class BasalTS
    Inherits System.Web.UI.Page
    Private revisar As New Rsesion()
    Private db As New BusinessLogicDB()
    Public cn1 As String = ConfigurationManager.ConnectionStrings("conStringTS").ConnectionString
    Public cn2 As String = ConfigurationManager.ConnectionStrings("conString").ConnectionString
    Public usuario As String = ""
    Public errores As String = ""
    Public strnhc As String
    Public existenhc As Boolean
    Public ufecha As Boolean
    Public idufecha As String

    'Declaracion de variables para generar el pdf
    Public nhc As String = String.Empty

    'Declaracion de variables para los datos basales
    Public FechaSocioeconomico, Nombre, Genero, Edad, PaisResidencia, DeptoResidencia, MunResidencia, Direccion, Telefono As String
    'Declaracion de variables para grupo familiar
    Public GrupoFamiliar As DataTable
    Public NombreEs, TipoRelacion, EdadES, NivelEducativo, NomSitLaboral, Ingreso, Dx As String
    'Condiciones de vivienda
    Public TipoVivienda, NumAmbiente, Servicios, TipoConstruccion As String
    'Condiciones Ingresos y egresos
    Public Personal, Hogar, Vivienda, EnergiaElectrica, AguaPotable, Cable, ETelefono, Alimentacion, Transporte, Educacion, Basura, Otros, TotalIngreso, TotalEgreso, DefSup, ProblemasIdentificados, Observaciones As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Response.Buffer = True
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1.0)
            Response.Expires = -1500
            Response.CacheControl = "no-cache"
            If Not revisar.RevisaSesion(Session("conexion").ToString(), Session("usuario").ToString()) Then
                Response.Redirect("~/inicio.aspx", False)
            Else
                usuario = Session("usuario").ToString()
                ufecha = False
                txt_asi.Focus()
                'setcampos(0)
                llenaTipoRelacion()
                llenaNivelEducativo()
                llenaSituacionLaboral()
                llenaConoceDx()
                llenaQuienesConocenDx()
                llenaTipoVivienda()
                llenaServicios()
                llenaTipoConstruccion()
                calculaIngresosEgresos()
                llenaProblemasIdentificados()
                pnl_CircuitoAdherencia.Visible = False ''panel de adherencia oculto
                Dim dt As New DataTable
                dt.Columns.Add("IdGrupoFamiliar")
                dt.Columns.Add("Nombre")
                dt.Columns.Add("TipoRelacion")
                dt.Columns.Add("Edad")
                dt.Columns.Add("NivelEducativo")
                dt.Columns.Add("SituacionLaboral")
                dt.Columns.Add("Ingreso")
                dt.Columns.Add("NomConoceDx")
                'dt.Rows.Add(0, "", "", "", "", "", "", "")
                Session("datosGF") = dt
            End If
            'ElseIf Page.IsPostBack Then
            '    Dim wcICausedPostBack As WebControl = CType(GetControlThatCausedPostBack(TryCast(sender, Page)), WebControl)
            '    Dim indx As Integer = wcICausedPostBack.TabIndex
            '    Dim ctrl = From control In wcICausedPostBack.Parent.Controls.OfType(Of WebControl)() Where control.TabIndex > indx Order By control.TabIndex Select control
            '    ctrl.DefaultIfEmpty(wcICausedPostBack).First().Focus()
        End If
    End Sub

    'Protected Function GetControlThatCausedPostBack(page As Page) As Control
    '    Dim control As Control = Nothing
    '    Dim ctrlname As String = page.Request.Params.[Get]("__EVENTTARGET")
    '    If ctrlname IsNot Nothing AndAlso ctrlname <> String.Empty Then
    '        control = page.FindControl(ctrlname)
    '    Else
    '        For Each ctl As String In page.Request.Form
    '            Dim c As Control = page.FindControl(ctl)
    '            If TypeOf c Is System.Web.UI.WebControls.Button OrElse TypeOf c Is System.Web.UI.WebControls.ImageButton Then
    '                control = c
    '                Exit For
    '            End If
    '        Next
    '    End If
    '    Return control
    'End Function

    Sub llenadatos(ByVal nhc As String)
        Dim tipo As String
        usuario = Session("usuario").ToString()
        If nhc.Substring(1, 1).ToUpper.ToString() = "P" Then
            tipo = "P"
        Else
            tipo = "A"
        End If
        If tipo = "A" Then
            db.Cn2 = cn2
            Dim x As String = db.ObtieneBasales(nhc, usuario)
            Dim rp As String() = x.Split("|")
            If rp(0).ToString() = "True" Then
                strnhc = nhc
                Session("nhc") = nhc
                existenhc = True
                lbl_documento.Text = String.Empty
                lbl_hospitalaria.Text = String.Empty
                lbl_nombre.Text = String.Empty
                lbl_genero.Text = String.Empty
                lbl_nacimiento.Text = String.Empty
                lbl_edad.Text = String.Empty
                lbl_paisnac.Text = String.Empty
                lbl_deptonac.Text = String.Empty
                lbl_munnac.Text = String.Empty
                lbl_paisres.Text = String.Empty
                lbl_deptores.Text = String.Empty
                lbl_munres.Text = String.Empty
                lbl_direccion.Text = String.Empty
                lbl_telefono.Text = String.Empty
                lbl_movil.Text = String.Empty
                lbl_niveleducativo.Text = String.Empty
                lbl_añoscompletos.Text = String.Empty
                lbl_orientacionsex.Text = String.Empty
                lbl_estadocivil.Text = String.Empty
                lbl_orientacionsex.Text = String.Empty
                lbl_estatus.Text = String.Empty
                hd_idpaciente.Value = String.Empty
                'asignacion
                lbl_documento.Text = rp(2).ToString()
                lbl_hospitalaria.Text = rp(1).ToString()
                lbl_nombre.Text = rp(4).ToString()
                lbl_genero.Text = rp(3).ToString()
                lbl_nacimiento.Text = rp(7).ToString()
                lbl_edad.Text = rp(8).ToString()
                lbl_paisnac.Text = rp(9).ToString()
                lbl_deptonac.Text = rp(10).ToString()
                lbl_munnac.Text = rp(11).ToString()
                lbl_paisres.Text = rp(12).ToString()
                lbl_deptores.Text = rp(13).ToString()
                lbl_munres.Text = rp(14).ToString()
                lbl_direccion.Text = rp(15).ToString()
                lbl_telefono.Text = rp(5).ToString()
                lbl_movil.Text = rp(6).ToString()
                lbl_niveleducativo.Text = rp(17).ToString()
                lbl_añoscompletos.Text = rp(18).ToString()
                lbl_orientacionsex.Text = rp(19).ToString()
                lbl_estadocivil.Text = rp(16).ToString()
                lbl_ocupacion.Text = rp(20).ToString()
                lbl_estatus.Text = rp(21).ToString()
                hd_idpaciente.Value = rp(22).ToString()
                lbl_error.Text = String.Empty
                btn_editar.Visible = False
                btn_agregar.Visible = False
                'Busca si posee Basales TS
                Dim existe As Integer
                existe = Existe_NHC(txt_asi.Text.ToString())
                If existe <> 0 Then
                    'lbl_error.Text = "Existe NHC en Basal TS"
                    'condicion para bloquear el boto de grabar!
                ElseIf existe = 0 Then
                    lbl_error.Text = "El paciente NO existe en el Basal de Trabajo Social"
                End If
            Else
                lbl_error.Text = rp(1)
                existenhc = False
                btn_editar.Visible = False
                btn_agregar.Visible = False
            End If
        ElseIf tipo = "P" Then
            db.Cn1 = cn1
            Dim x As String = db.ObtieneBasalesP(nhc, usuario)
            Dim rpP As String() = x.Split("|")
            If rpP(0).ToString() = "True" Then
                strnhc = nhc.ToUpper()
                Session("nhc") = nhc.ToUpper()
                existenhc = True
                lbl_genero.Text = String.Empty
                lbl_nombre.Text = String.Empty
                lbl_telefono.Text = String.Empty
                lbl_nacimiento.Text = String.Empty
                lbl_direccion.Text = String.Empty
                lbl_estatus.Text = String.Empty
                lbl_genero.Text = rpP(1).ToString()
                lbl_nombre.Text = rpP(2).ToString()
                lbl_telefono.Text = rpP(3).ToString()
                lbl_nacimiento.Text = rpP(4).ToString()
                lbl_direccion.Text = rpP(5).ToString()
                lbl_estatus.Text = rpP(6).ToString()
                lbl_error.Text = String.Empty
                btn_editar.Visible = True
                btn_agregar.Visible = False
            Else
                lbl_error.Text = rpP(1)
                existenhc = False
                btn_editar.Visible = False
                btn_agregar.Visible = True
            End If
        End If
    End Sub

    Sub llenaGrupoFamiliar(ByVal idpaciente As String)
        Try
            db.Cn1 = cn1
            Dim GrupoFamiliar As DataTable = db.ObtieneGrupoFamiliar(idpaciente, usuario)
            Session("datosGF") = GrupoFamiliar
            GV_GrupoFamiliar.DataSource = GrupoFamiliar
            GV_GrupoFamiliar.DataBind()
            'If GrupoFamiliar.Rows.Count > 0 Then
            '    llenafecha(1, "M")
            '    llenaARV(1)
            'Else
            '    llenafecha(0, "M")
            '    llenaARV(0)
            'End If
            'llenaMOTIVOCAMBIO()
        Catch ex As Exception
            'lbl_error.Text = "Hubo un error al mostrar listado de Pacientes."
            errores = (usuario & "|BasalTS.llenaGrupoFamiliar()|" & ex.ToString() & "|") + ex.Message
            db.GrabarErrores(errores)
        End Try
    End Sub

    Private Sub llenaTipoRelacion()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasGrupoFamiliar(1, usuario)
        DDL_TipoRelacion.DataSource = datos
        DDL_TipoRelacion.DataTextField = "NomTipoRelacion"
        DDL_TipoRelacion.DataValueField = "IdTipoRelacion"
        DDL_TipoRelacion.DataBind()
        DDL_TipoRelacion.SelectedIndex = 0
    End Sub

    Private Sub llenaNivelEducativo()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasGrupoFamiliar(2, usuario)
        DDL_NivelEducativo.DataSource = datos
        DDL_NivelEducativo.DataTextField = "NomNivelEducativo"
        DDL_NivelEducativo.DataValueField = "IdNivelEducativo"
        DDL_NivelEducativo.DataBind()
        DDL_NivelEducativo.SelectedIndex = 0
    End Sub

    Private Sub llenaSituacionLaboral()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasGrupoFamiliar(3, usuario)
        DDL_SituacionLaboral.DataSource = datos
        DDL_SituacionLaboral.DataTextField = "NomSituacionLaboral"
        DDL_SituacionLaboral.DataValueField = "IdSituacionLaboral"
        DDL_SituacionLaboral.DataBind()
        DDL_SituacionLaboral.SelectedIndex = 0
    End Sub

    Private Sub llenaConoceDx()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasGrupoFamiliar(4, usuario)
        DDL_ConoceDx.DataSource = datos
        DDL_ConoceDx.DataTextField = "NomConoceDx"
        DDL_ConoceDx.DataValueField = "IdConoceDx"
        DDL_ConoceDx.DataBind()
        DDL_ConoceDx.SelectedIndex = 0
    End Sub

    Private Sub llenaQuienesConocenDx()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasGrupoFamiliar(5, usuario)
        cbl_qsDx.DataSource = datos
        cbl_qsDx.DataTextField = "NomPersonaDX"
        cbl_qsDx.DataValueField = "IdPersonasDx"
        cbl_qsDx.DataBind()
        'cbl_qsDx.SelectedIndex = 0
    End Sub

    Private Sub llenaTipoVivienda()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasGrupoFamiliar(6, usuario)
        cbl_tipovivienda.DataSource = datos
        cbl_tipovivienda.DataTextField = "NomTipoVivienda"
        cbl_tipovivienda.DataValueField = "IdTipoVivienda"
        cbl_tipovivienda.DataBind()
        'cbl_tipovivienda.SelectedIndex = 0
    End Sub

    Private Sub llenaServicios()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasGrupoFamiliar(7, usuario)
        cbl_servicios.DataSource = datos
        cbl_servicios.DataTextField = "NomServicios"
        cbl_servicios.DataValueField = "IdServicios"
        cbl_servicios.DataBind()
        'cbl_servicios.SelectedIndex = 0
    End Sub

    Private Sub llenaTipoConstruccion()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasGrupoFamiliar(8, usuario)
        cbl_tipoconstruccion.DataSource = datos
        cbl_tipoconstruccion.DataTextField = "NomTipoConstruccion"
        cbl_tipoconstruccion.DataValueField = "IdTipoConstruccion"
        cbl_tipoconstruccion.DataBind()
        'cbl_tipoconstruccion.SelectedIndex = 0
    End Sub

    Private Sub llenaProblemasIdentificados()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasGrupoFamiliar(9, usuario)
        cbl_probidentificados.DataSource = datos
        cbl_probidentificados.DataTextField = "NomProbIdentificados"
        cbl_probidentificados.DataValueField = "IdProbIdentificados"
        cbl_probidentificados.DataBind()
        'cbl_probidentificados.SelectedIndex = 0
    End Sub

    Private Sub calculaIngresosEgresos()
        'validacion de textos
        txt_ingpersonal.Text = ValoresTxt(txt_ingpersonal.Text.ToString())
        txt_inghogar.Text = ValoresTxt(txt_inghogar.Text.ToString())
        txt_egvivienda.Text = ValoresTxt(txt_egvivienda.Text.ToString())
        txt_egeelectrica.Text = ValoresTxt(txt_egeelectrica.Text.ToString())
        txt_egagua.Text = ValoresTxt(txt_egagua.Text.ToString())
        txt_egcable.Text = ValoresTxt(txt_egcable.Text.ToString())
        txt_egtelefono.Text = ValoresTxt(txt_egtelefono.Text.ToString())
        txt_egalimentacion.Text = ValoresTxt(txt_egalimentacion.Text.ToString())
        txt_egtransporte.Text = ValoresTxt(txt_egtransporte.Text.ToString())
        txt_egeducacion.Text = ValoresTxt(txt_egeducacion.Text.ToString())
        txt_egbasura.Text = ValoresTxt(txt_egbasura.Text.ToString())
        txt_egotros.Text = ValoresTxt(txt_egotros.Text.ToString())
        'asignacion a variables
        Dim ing_personal, ing_hogar, eg_vivienda, eg_eelectrica, eg_agua, eg_cable, eg_telefono, eg_alimentacion, eg_transporte, eg_educacion, eg_basura, eg_otros As Double
        ing_personal = Valores(txt_ingpersonal.Text.ToString())
        ing_hogar = Valores(txt_inghogar.Text.ToString())
        eg_vivienda = Valores(txt_egvivienda.Text.ToString())
        eg_eelectrica = Valores(txt_egeelectrica.Text.ToString())
        eg_agua = Valores(txt_egagua.Text.ToString())
        eg_cable = Valores(txt_egcable.Text.ToString())
        eg_telefono = Valores(txt_egtelefono.Text.ToString())
        eg_alimentacion = Valores(txt_egalimentacion.Text.ToString())
        eg_transporte = Valores(txt_egtransporte.Text.ToString())
        eg_educacion = Valores(txt_egeducacion.Text.ToString())
        eg_basura = Valores(txt_egbasura.Text.ToString())
        eg_otros = Valores(txt_egotros.Text.ToString())
        'calculos
        Dim total_ingresos, total_egresos, deficit_superavit As Double
        total_ingresos = ing_personal + ing_hogar
        total_egresos = eg_vivienda + eg_eelectrica + eg_agua + eg_cable + eg_telefono + eg_alimentacion + eg_transporte + eg_educacion + eg_basura + eg_otros
        deficit_superavit = total_ingresos - total_egresos
        lbl_totalingresos.Text = FormatNumber(CDbl(total_ingresos), 2)
        lbl_totalegresos.Text = FormatNumber(CDbl(total_egresos), 2)
        lbl_deficit_superavit.Text = FormatNumber(CDbl(deficit_superavit), 2)
        If deficit_superavit < 0 Then
            lbl_deficit_superavit.CssClass = "texto3b"
        Else
            lbl_deficit_superavit.CssClass = "texto3a"
        End If
    End Sub

    Function Valores(ByVal v As String) As Double
        Dim x As Double
        If v.ToString() = "" Or String.IsNullOrEmpty(v.ToString()) Then
            x = 0.0R
        Else
            x = Convert.ToDouble(v)
        End If
        Return x
    End Function

    Function ValoresTxt(ByVal v As String) As String
        Dim x As String
        If v.ToString() = "" Or String.IsNullOrEmpty(v.ToString()) Then
            x = "0.00"
        Else
            x = FormatNumber(CDbl(v), 2)
        End If
        Return x
    End Function

    Sub buscaNHC()
        If txt_asi.Text.ToUpper().Trim <> String.Empty Then
            llenadatos(txt_asi.Text.ToUpper())
            llenaestudios(txt_asi.Text.ToUpper())
            'llenaGrupoFamiliar(hd_idpaciente.Value)
            Session("idPaciente") = hd_idpaciente.Value
            divingresoGF.Visible = True
            divingresoCircuito.Visible = False


            GV_GrupoFamiliar.DataSource = Session("datosGF")
            GV_GrupoFamiliar.DataBind()

            'GV_pnl_SE.DataSource = Nothing
            'GV_pnl_SE.DataBind()

            'If existenhc Then
            '    If ufecha Then
            '        divingreso.Visible = True
            '        txt_retornodias.Focus()
            '        'txt_devcant1.Focus()
            '    Else
            '        divingreso.Visible = True
            '        txt_fe_dd.Focus()
            '    End If
            'Else
            '    lbl_genero.Text = String.Empty
            '    lbl_nombre.Text = String.Empty
            '    lbl_telefono.Text = String.Empty
            '    lbl_nacimiento.Text = String.Empty
            '    lbl_domicilio.Text = String.Empty
            '    lbl_estatus.Text = String.Empty
            '    divingreso.Visible = False
            '    txt_asi.Focus()
            'End If
        Else
            GV_pnl_SE.DataSource = Nothing
            GV_pnl_SE.DataBind()

            lbl_genero.Text = String.Empty
            lbl_nombre.Text = String.Empty
            lbl_telefono.Text = String.Empty
            lbl_nacimiento.Text = String.Empty
            lbl_direccion.Text = String.Empty
            lbl_estatus.Text = String.Empty
            'divingreso.Visible = False
            txt_asi.Focus()
        End If
    End Sub

    Protected Sub ibt_Agregar_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles ibt_Agregar.Click
        If Not revisar.RevisaSesion(Session("conexion").ToString(), Session("usuario").ToString()) Then
            Response.Redirect("~/inicio.aspx", False)
        Else
            Dim dt As DataTable = Session("datosGF")
            Dim cont As Integer = 0


            For Each row As DataRow In dt.Rows
                cont = Convert.ToInt32(row("IdGrupoFamiliar").ToString())
            Next



            If txt_Nombre.Text.ToString() <> "" Then
                If txt_Edad.Text.ToString() <> "" Then
                    dt.Rows.Add(cont + 1, txt_Nombre.Text.ToString(), DDL_TipoRelacion.SelectedItem.Text.ToString(), txt_Edad.Text.ToString(), DDL_NivelEducativo.SelectedItem.Text.ToString(), DDL_SituacionLaboral.SelectedItem.Text.ToString(), txt_Ingreso.Text.ToString(), DDL_ConoceDx.SelectedItem.Text.ToString())
                    Session("datosGF") = dt
                    GV_GrupoFamiliar.DataSource = dt
                    GV_GrupoFamiliar.DataBind()

                    CancelarGF()
                Else
                    lbl_error.Text = "Ingresar edad"
                End If
            Else
                lbl_error.Text = "Ingresar nombre"
            End If


        End If
    End Sub

    Protected Sub ibt_Modificar_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)
        If Not revisar.RevisaSesion(Session("conexion").ToString(), Session("usuario").ToString()) Then
            Response.Redirect("~/inicio.aspx", False)
        Else

            Dim idGF As String = Session("idGF_Mod")


            Dim dt As DataTable = Session("datosGF")
            Dim indice As Integer = 0
            Dim indiceModificar As Integer = -1
            For Each row As DataRow In dt.Rows

                If idGF = row("IdGrupoFamiliar").ToString() Then

                    indiceModificar = indice

                End If
                indice += 1
            Next



            dt.Rows(indiceModificar)("Nombre") = txt_Nombre.Text
            dt.Rows(indiceModificar)("TipoRelacion") = DDL_TipoRelacion.SelectedItem.ToString()
            dt.Rows(indiceModificar)("Edad") = txt_Edad.Text
            dt.Rows(indiceModificar)("NivelEducativo") = DDL_NivelEducativo.SelectedItem.ToString()
            dt.Rows(indiceModificar)("SituacionLaboral") = DDL_SituacionLaboral.SelectedItem.ToString()
            dt.Rows(indiceModificar)("Ingreso") = txt_Ingreso.Text
            dt.Rows(indiceModificar)("NomConoceDx") = DDL_ConoceDx.SelectedItem.ToString()


            Session("datosGF") = dt

            GV_GrupoFamiliar.DataSource = Nothing
            GV_GrupoFamiliar.DataBind()

            GV_GrupoFamiliar.DataSource = dt
            GV_GrupoFamiliar.DataBind()


            CancelarGF()

        End If


    End Sub

    Protected Sub ibt_Cancelar_Click(sender As Object, e As ImageClickEventArgs)
        CancelarGF()
    End Sub

    Private Sub CancelarGF()
        txt_Nombre.Text = String.Empty
        txt_Edad.Text = String.Empty
        txt_Ingreso.Text = String.Empty
        DDL_TipoRelacion.SelectedIndex = 0
        DDL_NivelEducativo.SelectedIndex = 0
        DDL_SituacionLaboral.SelectedIndex = 0
        DDL_ConoceDx.SelectedIndex = 0
        ibt_Agregar.Visible = True
        ibt_Modificar.Visible = False
        Session("idGF_Mod") = 0
    End Sub

    Protected Sub GV_GrupoFamiliar_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GV_GrupoFamiliar.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim ib As ImageButton = DirectCast(e.Row.FindControl("ibtBorrar"), ImageButton)
            ib.Attributes.Add("onclick", "javascript:return confirm('Esta seguro que quiere Eliminar el familiar?')")
        End If
    End Sub

    Protected Sub GV_CircuitoAdherencia_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GV_CircuitoAdherencia.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim ib As ImageButton = DirectCast(e.Row.FindControl("ibtBorrar"), ImageButton)
            ib.Attributes.Add("onclick", "javascript:return confirm('Esta seguro que quiere Eliminar Circuito?')")
        End If

    End Sub

    Protected Sub GV_GrupoFamiliar_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GV_GrupoFamiliar.RowCommand
        If Not revisar.RevisaSesion(Session("conexion").ToString(), Session("usuario").ToString()) Then
            Response.Redirect("~/inicio.aspx", False)
        Else
            If e.CommandName = "Borrar" Then
                Dim gv As GridView = DirectCast(sender, GridView)
                Dim rowIndex As Int32 = Convert.ToInt32(e.CommandArgument.ToString())
                Dim idGF As String = gv.DataKeys(rowIndex)(0).ToString()


                Dim dt As DataTable = Session("datosGF")
                Dim indice As Integer = 0
                Dim indiceEliminar As Integer = -1
                For Each row As DataRow In dt.Rows

                    If idGF = row("IdGrupoFamiliar").ToString() Then

                        indiceEliminar = indice

                    End If
                    indice += 1
                Next


                dt.Rows.RemoveAt(indiceEliminar)




                Session("datosGF") = dt

                GV_GrupoFamiliar.DataSource = Nothing
                GV_GrupoFamiliar.DataBind()

                GV_GrupoFamiliar.DataSource = dt
                GV_GrupoFamiliar.DataBind()

            End If
            If e.CommandName = "Editar" Then
                Dim gv As GridView = DirectCast(sender, GridView)
                Dim rowIndex As Int32 = Convert.ToInt32(e.CommandArgument.ToString())
                Dim idGF As String = gv.DataKeys(rowIndex)(0).ToString()

                lbl_error.Text = idGF.ToString()
                Dim dt As DataTable = Session("datosGF")
                Dim indice As Integer = 0
                Dim indiceModificar As Integer = -1
                For Each row As DataRow In dt.Rows

                    If idGF = row("IdGrupoFamiliar").ToString() Then

                        indiceModificar = indice

                    End If
                    indice += 1
                Next




                txt_Nombre.Text = dt.Rows(indiceModificar)("Nombre")

                Dim c As Integer = -1
                For i As Integer = 0 To DDL_TipoRelacion.Items.Count - 1
                    DDL_TipoRelacion.SelectedIndex = i
                    If DDL_TipoRelacion.SelectedItem.ToString() = dt.Rows(indiceModificar)("TipoRelacion") Then
                        c = i
                    End If
                Next
                DDL_TipoRelacion.SelectedIndex = c


                txt_Edad.Text = dt.Rows(indiceModificar)("Edad")

                c = -1
                For i As Integer = 0 To DDL_NivelEducativo.Items.Count - 1
                    DDL_NivelEducativo.SelectedIndex = i
                    If DDL_NivelEducativo.SelectedItem.ToString() = dt.Rows(indiceModificar)("NivelEducativo") Then
                        c = i
                    End If
                Next
                DDL_NivelEducativo.SelectedIndex = c

                c = -1
                For i As Integer = 0 To DDL_SituacionLaboral.Items.Count - 1
                    DDL_SituacionLaboral.SelectedIndex = i
                    If DDL_SituacionLaboral.SelectedItem.ToString() = dt.Rows(indiceModificar)("SituacionLaboral") Then
                        c = i
                    End If
                Next
                DDL_SituacionLaboral.SelectedIndex = c


                txt_Ingreso.Text = dt.Rows(indiceModificar)("Ingreso")

                c = -1
                For i As Integer = 0 To DDL_ConoceDx.Items.Count - 1
                    DDL_ConoceDx.SelectedIndex = i
                    If DDL_ConoceDx.SelectedItem.ToString() = dt.Rows(indiceModificar)("NomConoceDx") Then
                        c = i
                    End If
                Next
                DDL_ConoceDx.SelectedIndex = c



                txt_Nombre.Focus()
                ibt_Agregar.Visible = False
                ibt_Modificar.Visible = True
                ibt_Cancelar.Visible = True

                Session("idGF_Mod") = idGF

            End If
        End If
    End Sub

    Protected Sub GV_GrupoFamiliar_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles GV_GrupoFamiliar.PreRender
        Dim n As Integer = 0
        For Each nrow As GridViewRow In GV_GrupoFamiliar.Rows
            For columnIndex As Integer = n To Convert.ToInt32(GV_GrupoFamiliar.Rows.Count)
                Dim irow1 As ImageButton = DirectCast(nrow.FindControl("ibtBorrar"), ImageButton)
                irow1.CommandArgument = Convert.ToString(n)
                Dim irow2 As ImageButton = DirectCast(nrow.FindControl("ibtEditar"), ImageButton)
                irow2.CommandArgument = Convert.ToString(n)
            Next
            n += 1
        Next
    End Sub

    Protected Sub IngresosEgresos_TextChanged(sender As Object, e As EventArgs) Handles txt_ingpersonal.TextChanged, txt_inghogar.TextChanged, txt_egvivienda.TextChanged, txt_egeelectrica.TextChanged, txt_egagua.TextChanged, txt_egcable.TextChanged, txt_egtelefono.TextChanged, txt_egalimentacion.TextChanged, txt_egtransporte.TextChanged, txt_egeducacion.TextChanged, txt_egbasura.TextChanged, txt_egotros.TextChanged
        calculaIngresosEgresos()
    End Sub

    Protected Sub btn_buscar_Click(sender As Object, e As ImageClickEventArgs) Handles btn_buscar.Click
        buscaNHC()
    End Sub

    Protected Sub txt_asi_TextChanged(sender As Object, e As EventArgs) Handles txt_asi.TextChanged
        buscaNHC()
    End Sub

    'Protected Sub btn_agregar_Click(sender As Object, e As EventArgs) Handles Button1.Click
    '    db.Cn1 = cn1
    '    Dim datos As String
    '    usuario = Session("usuario").ToString()
    '    'Dim NHC As String = txt_asi.Text.ToString()
    '    'Dim X As String = db.ExisteNHC(NHC, usuario)

    '    Dim existe As Integer
    '    existe = Existe_NHC(txt_asi.Text.ToString())
    '    If existe <> 0 Then
    '        'lbl_error.Text = "Existe NHC en Basal TS"
    '    ElseIf existe = 0 Then
    '        lbl_error.Text = "El paciente NO existe en el Basal de Trabajo Social"
    '    End If

    '    ' Fecha en que se modifico
    '    ' Dim fechamod As String = (Date)  

    '    If X <> "Existe" Then
    '        db.Grabar_basal_ts(Session("idPaciente").ToString(), txt_asi.Text.ToString(), txt_fecha_ES.Text.ToString(), txt_observaciones.Text.ToString(), str(ddl_circuito.SelectedValue.ToString()), usuario, fechamod)


    '    End If


    'End Sub
    Function str(ByVal x As String) As String
        Dim z As String
        If x = String.Empty Then
            z = 0
        Else
            z = x
        End If
        Return z
    End Function



    Function Existe_NHC(ByVal nhc As String) As Integer
        db.Cn1 = cn1
        Return db.ExisteNHC(nhc, usuario)
    End Function


    'Protected Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click

    '    If Not String.IsNullOrEmpty(txt_asi.Text.ToString()) Then
    '        Dim existe As Integer
    '        existe = Existe_NHC(txt_asi.Text.ToString())
    '        If existe <> 0 Then
    '            lbl_error.Text = "Existe NHC en Basal TS"
    '        ElseIf existe = 0 Then
    '            lbl_error.Text = "NO existe NHC en Basal TS"
    '        End If
    '    End If
    'End Sub


    Protected Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click

        db.Cn1 = cn1
        usuario = Session("usuario").ToString()
        Dim datos As String

        lbl_error.Text = ""
        'Dim existe As Integer

        'existe = Existe_NHC(txt_asi.Text.ToString())
        Try
            'If existe <> 0 Then
            '    lbl_error.Text = "Paciente ya Existe en datos basales"
            'ElseIf existe = 0 Then
            ' lbl_error.Text = "NO existe NHC en Basal TS"
            If txt_fecha_ES.Text <> Nothing Then

                Dim idbasalests As String = db.Grabar_basal_ts(Session("idPaciente").ToString(), txt_asi.Text.ToString(), txt_fecha_ES.Text.ToString(), txt_observaciones.Text.ToString(), ddl_circuito.SelectedValue.ToString(), usuario)

                If idbasalests <> String.Empty Then



                    Dim datos2 As String
                    Dim estado As String = "1"

                    Dim dt As DataTable = Session("datosGF")


                    Dim TipoRelacion As DataTable = db.listasGrupoFamiliar(1, usuario)
                    Dim NivelEducativo As DataTable = db.listasGrupoFamiliar(2, usuario)
                    Dim SituacionLaboral As DataTable = db.listasGrupoFamiliar(3, usuario)
                    Dim ConoceDx As DataTable = db.listasGrupoFamiliar(4, usuario)



                    For Each row As DataRow In dt.Rows

                        Dim TR As String = Nothing
                        Dim NE As String = Nothing
                        Dim SL As String = Nothing
                        Dim CD As String = Nothing

                        For Each row1 As DataRow In TipoRelacion.Rows
                            If (row("TipoRelacion").ToString() = row1("NomTipoRelacion").ToString()) Then
                                TR = row1("IdTipoRelacion").ToString()
                            End If
                        Next
                        For Each row1 As DataRow In NivelEducativo.Rows
                            If (row("NivelEducativo").ToString() = row1("NomNivelEducativo").ToString()) Then
                                NE = row1("IdNivelEducativo").ToString()
                            End If
                        Next
                        For Each row1 As DataRow In SituacionLaboral.Rows
                            If (row("SituacionLaboral").ToString() = row1("NomSituacionLaboral").ToString()) Then
                                SL = row1("IdSituacionLaboral").ToString()
                            End If
                        Next
                        For Each row1 As DataRow In ConoceDx.Rows
                            If (row("NomConoceDx").ToString() = row1("NomConoceDx").ToString()) Then
                                CD = row1("IdConoceDx").ToString()
                            End If
                        Next

                        datos2 = db.InsertarGrupoFamiliar(idbasalests, Session("idPaciente").ToString(), row("Nombre").ToString(), TR, row("Edad").ToString(), NE, SL, row("Ingreso").ToString(), CD, estado, Session("usuario").ToString())

                    Next














                    For Each li As System.Web.UI.WebControls.ListItem In cbl_qsDx.Items
                            If li.Selected Then
                                db.Grabar_personaDX(idbasalests, li.Value.ToString(), usuario)
                            End If
                        Next

                        Dim idcondiciones_vivienda As String = db.Condiciones_vivienda(idbasalests, txt_nambientes.Text.ToString(), usuario)

                        If idcondiciones_vivienda <> 0 Then

                            For Each li As System.Web.UI.WebControls.ListItem In cbl_tipovivienda.Items
                                If li.Selected Then
                                    db.Tipo_vivienda(idbasalests, idcondiciones_vivienda, li.Value.ToString(), usuario)
                                End If
                            Next

                            For Each li As System.Web.UI.WebControls.ListItem In cbl_tipoconstruccion.Items
                                If li.Selected Then
                                    db.Tipo_construccion(idbasalests, idcondiciones_vivienda, li.Value.ToString(), usuario)
                                End If
                            Next

                            For Each li As System.Web.UI.WebControls.ListItem In cbl_servicios.Items
                                If li.Selected Then
                                    db.Servicios_vivienda(idbasalests, idcondiciones_vivienda, li.Value.ToString(), usuario)
                                End If
                            Next
                        End If

                        For Each li As System.Web.UI.WebControls.ListItem In cbl_probidentificados.Items
                            If li.Selected Then
                                db.Prob_identificados(idbasalests, li.Value.ToString(), usuario)
                            End If
                        Next

                        datos = idbasalests + "|" + txt_ingpersonal.Text.ToString() + "|" + txt_inghogar.Text.ToString() + "|" + txt_egvivienda.Text.ToString() + "|" + txt_egeelectrica.Text.ToString() + "|"
                        datos += txt_egagua.Text.ToString() + "|" + txt_egcable.Text.ToString() + "|" + txt_egtelefono.Text.ToString() + "|" + txt_egalimentacion.Text.ToString() + "|" + txt_egtransporte.Text.ToString() + "|" + txt_egeducacion.Text.ToString() + "|"
                        datos += txt_egbasura.Text.ToString() + "|" + txt_egotros.Text.ToString() + "|" + lbl_totalingresos.Text.ToString() + "|" + lbl_totalegresos.Text.ToString() + "|" + lbl_deficit_superavit.Text.ToString() + "|" + usuario.ToString()

                        db.Insert_ingresos(datos, usuario)
                        Response.Redirect("~/BasalTS.aspx", False)
                    End If
                Else
                lbl_error.Text = "Ingresar Fecha de Estudio Socio Económico"
            End If
            'End If

        Catch ex As Exception
            lbl_error.Text = ex.Message
        End Try
    End Sub


    Sub datosGV()
        'Declaracion de variables locales dentro de la funcion
        Dim NHCB As String = txt_asi.Text.ToString()
        GV_GrupoFamiliar.DataSource = Nothing
        GV_GrupoFamiliar.DataBind()
        usuario = Session("usuario").ToString()
        db.Cn1 = cn1
        Dim GrupoFamiliar As DataTable = db.ReporteGrupoGamiliar(NHCB, usuario)
        Try
            Session("dspcA") = GrupoFamiliar
            GV_GrupoFamiliar.DataSource = GrupoFamiliar
            GV_GrupoFamiliar.DataBind()
        Catch ex As Exception
            lbl_error.Text = "Hubo un errror al mostrar datos del gridview"
        End Try
    End Sub
    'CREANDO EL PDF
    'Funcion para generar pdf
    Sub GeneraPDF()
        'Datos Basales
        nhc = DirectCast(tblbasal.FindControl("txt_asi"), TextBox).Text.ToString()
        FechaSocioeconomico = DirectCast(tblbasal.FindControl("txt_fecha_ES"), TextBox).Text.ToString()
        Nombre = DirectCast(tblbasal.FindControl("lbl_nombre"), Label).Text.ToString()
        Genero = DirectCast(tblbasal.FindControl("lbl_genero"), Label).Text.ToString()
        Edad = DirectCast(tblbasal.FindControl("lbl_edad"), Label).Text.ToString()
        PaisResidencia = DirectCast(tblbasal.FindControl("lbl_paisres"), Label).Text.ToString()
        DeptoResidencia = DirectCast(tblbasal.FindControl("lbl_deptores"), Label).Text.ToString()
        MunResidencia = DirectCast(tblbasal.FindControl("lbl_munres"), Label).Text.ToString()
        Direccion = DirectCast(tblbasal.FindControl("lbl_direccion"), Label).Text.ToString()
        Telefono = DirectCast(tblbasal.FindControl("lbl_telefono"), Label).Text.ToString()

        datosGV()

        'Datos condiciones de vivienda
        'Tipo Vivienda
        For i = 0 To cbl_tipovivienda.Items.Count - 1
            If cbl_tipovivienda.Items(i).Selected Then
                TipoVivienda += cbl_tipovivienda.Items(i).Text & "." & "  "
            End If
        Next
        'Numero de ambiente
        NumAmbiente = DirectCast(tblbasal.FindControl("txt_nambientes"), TextBox).Text.ToString()
        'Servicios
        For i = 0 To cbl_servicios.Items.Count - 1
            If cbl_servicios.Items(i).Selected Then
                Servicios += cbl_servicios.Items(i).Text & "." & "  "
            End If
        Next
        'Tipo Construcción
        For i = 0 To cbl_tipoconstruccion.Items.Count - 1
            If cbl_tipoconstruccion.Items(i).Selected Then
                TipoConstruccion += cbl_tipoconstruccion.Items(i).Text & "." & "  "
            End If
        Next

        'Ingresos y Egresos
        'Ingresos
        Personal = DirectCast(tblbasal.FindControl("txt_ingpersonal"), TextBox).Text.ToString()
        Hogar = DirectCast(tblbasal.FindControl("txt_inghogar"), TextBox).Text.ToString()
        'Egresos
        Vivienda = DirectCast(tblbasal.FindControl("txt_egvivienda"), TextBox).Text.ToString()
        EnergiaElectrica = DirectCast(tblbasal.FindControl("txt_egeelectrica"), TextBox).Text.ToString()
        AguaPotable = DirectCast(tblbasal.FindControl("txt_egagua"), TextBox).Text.ToString()
        Cable = DirectCast(tblbasal.FindControl("txt_egcable"), TextBox).Text.ToString()
        ETelefono = DirectCast(tblbasal.FindControl("txt_egtelefono"), TextBox).Text.ToString()
        Alimentacion = DirectCast(tblbasal.FindControl("txt_egalimentacion"), TextBox).Text.ToString()
        Transporte = DirectCast(tblbasal.FindControl("txt_egtransporte"), TextBox).Text.ToString()
        Educacion = DirectCast(tblbasal.FindControl("txt_egeducacion"), TextBox).Text.ToString()
        Basura = DirectCast(tblbasal.FindControl("txt_egbasura"), TextBox).Text.ToString()
        Otros = DirectCast(tblbasal.FindControl("txt_egotros"), TextBox).Text.ToString()
        TotalIngreso = DirectCast(tblbasal.FindControl("lbl_totalingresos"), Label).Text.ToString()
        TotalEgreso = DirectCast(tblbasal.FindControl("lbl_totalegresos"), Label).Text.ToString()
        DefSup = DirectCast(tblbasal.FindControl("lbl_deficit_superavit"), Label).Text.ToString()

        'Problemas identificados y observaciones
        'Problemas identificados
        For i = 0 To cbl_probidentificados.Items.Count - 1
            If cbl_probidentificados.Items(i).Selected Then
                ProblemasIdentificados += cbl_probidentificados.Items(i).Text & "." & "  "
            End If
        Next
        'Observaciones
        Observaciones = DirectCast(tblbasal.FindControl("txt_observaciones"), TextBox).Text.ToString()
    End Sub

    'Funcion para exportar el documento a pdf
    Sub ExportarPDF(ByVal usr As String, ByVal archivo As String, ByVal narchivo As String)
        Dim Documento As New Document(PageSize.LETTER, 20.0F, 20.0F, 20.0F, 20.0F) 'Declaracion del documento tamaño carta
        Dim Parrafo As New Paragraph 'Declaracion de variables para uso de parrafos
        Dim tblTITULO As New PdfPTable(1) 'declara la tabla con 1 Columna
        Dim tblFooter As New PdfPTable(1) 'declara la tabla con 1 Columna
        Dim tblDATOSGENERALES As New PdfPTable(6) 'declara la tabla con 6 Columnas
        Dim tblDATOSG As New PdfPTable(6) 'declara la tabla con 6 Columnas
        Dim tblCONDICIONESVIVIENDA As New PdfPTable(4) 'declara la tabla con 4 Columnas
        Dim tblINGRESOEGRESO As New PdfPTable(6) 'declara la tabla con 6 Columnas
        Dim tblAnalisisFinanciero As New PdfPTable(1) 'declara la tabla con 6 Columnas
        Dim tblTAnalisisFinanciero As New PdfPTable(6) 'declara la tabla con 6 Columnas
        Dim tblPROBLEMASOBSERVACIONES As New PdfPTable(2) 'declara la tabla con 4 Columnas
        Dim tblGrupoFamiliar As New PdfPTable(7)






        'Llamando a la funcion GeneraPDF
        GeneraPDF()

        Dim fs As New FileStream(archivo, FileMode.Create)
        PdfWriter.GetInstance(Documento, fs)

        Documento.Open() 'Se abre el documento para escribir en el
        tblTITULO.DefaultCell.Padding = 8
        tblTITULO.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER
        tblTITULO.SetWidthPercentage(New Single() {600}, PageSize.LETTER) 'Ajusta el tamaño de cada columna
        tblTITULO.AddCell(New Paragraph("ESTUDIO SOCIOECONÓMICO", FontFactory.GetFont("Arial", 10, Element.ALIGN_CENTER)))
        Documento.Add(tblTITULO) 'Agrega la tabla al documento

        'Llenado de tabla de Datos Generales
        Parrafo.Alignment = Element.ALIGN_LEFT 'Alinea el parrafo para que sea centrado o justificado
        Parrafo.Font = FontFactory.GetFont("Arial", 11, Font.BOLD) 'Asigan fuente
        Parrafo.Add("    Datos Generales") 'Texto que se insertara
        Documento.Add(Parrafo) 'Agrega el parrafo al documento
        Parrafo.Clear() 'Limpia el parrafo para que despues pueda ser utilizado nuevamente
        'Documento.Add(New Paragraph(" ")) 'Salto de linea
        tblDATOSGENERALES.DefaultCell.Border = 0
        tblDATOSGENERALES.DefaultCell.Padding = 1
        tblDATOSGENERALES.SetWidthPercentage(New Single() {55, 150, 50, 50, 50, 245}, PageSize.LETTER) 'Ajusta el tamaño de cada columna
        'DatosG
        tblDATOSG.DefaultCell.Border = 0
        tblDATOSG.DefaultCell.Padding = 1
        tblDATOSG.SetWidthPercentage(New Single() {125, 75, 125, 100, 100, 75}, PageSize.LETTER) 'Ajusta el tamaño de cada columna

        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("   NHC:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblDATOSGENERALES.AddCell(New Paragraph(nhc, FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("   Nombre:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblDATOSGENERALES.AddCell(New Paragraph(Nombre, FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("   Género:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblDATOSGENERALES.AddCell(New Paragraph(Genero, FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("   Edad:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblDATOSGENERALES.AddCell(New Paragraph(Edad, FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("   Dirección:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblDATOSGENERALES.AddCell(New Paragraph(Direccion, FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSGENERALES.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("   Pais residencia:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblDATOSG.AddCell(New Paragraph(PaisResidencia, FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("   Departamento residencia:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblDATOSG.AddCell(New Paragraph(DeptoResidencia, FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("  Municipio residencia:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblDATOSG.AddCell(New Paragraph(MunResidencia, FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("   Fecha socioeconómica:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblDATOSG.AddCell(New Paragraph(FechaSocioeconomico, FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblDATOSG.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        Documento.Add(tblDATOSGENERALES)
        Documento.Add(tblDATOSG)

        'GRUPO FAMILIAR 
        Dim NHCB As String = txt_asi.Text.ToString()
        GV_GrupoFamiliar.DataSource = Nothing
        GV_GrupoFamiliar.DataBind()
        usuario = Session("usuario").ToString()
        db.Cn1 = cn1
        GrupoFamiliar = db.ReporteGrupoGamiliar(NHCB, usuario)
        Try
            Session("dspcA") = GrupoFamiliar
            GV_GrupoFamiliar.DataSource = GrupoFamiliar
            GV_GrupoFamiliar.DataBind()
        Catch ex As Exception
            lbl_error.Text = "Hubo un errror al mostrar datos del gridview"
        End Try

        'Estructura de la tabla
        Dim cells() As PdfPCell = New PdfPCell() {New PdfPCell(New Paragraph("Nombre", FontFactory.GetFont("Arial", 8, Font.BOLD))), New PdfPCell(New Paragraph("TipoRelacion", FontFactory.GetFont("Arial", 8, Font.BOLD))), New PdfPCell(New Paragraph("Edad", FontFactory.GetFont("Arial", 8, Font.BOLD))), New PdfPCell(New Paragraph("NivelEducativo", FontFactory.GetFont("Arial", 8, Font.BOLD))), New PdfPCell(New Paragraph("SituacionLaboral", FontFactory.GetFont("Arial", 8, Font.BOLD))), New PdfPCell(New Paragraph("Ingresos", FontFactory.GetFont("Arial", 8, Font.BOLD))), New PdfPCell(New Paragraph("Dx", FontFactory.GetFont("Arial", 8, Font.BOLD)))}
        Dim rowsEncabezadoPDF As PdfPRow = New PdfPRow(cells)
        tblGrupoFamiliar.Rows.Add(rowsEncabezadoPDF)


        'recorremos la tabla y la llenamos
        For Each rows As DataRow In GrupoFamiliar.Rows

            NombreEs = CStr(rows("Nombre"))
            TipoRelacion = CStr(rows("TipoRelacion"))
            EdadES = CStr(rows("Edad"))
            NivelEducativo = CStr(rows("NivelEducativo"))
            NomSitLaboral = CStr(rows("SituacionLaboral"))
            Ingreso = CStr(rows("Ingreso"))
            Dx = CStr(rows("NomConoceDx"))

            'Estructura de llenado de datos
            Dim cells1() As PdfPCell = New PdfPCell() {New PdfPCell(New Paragraph(NombreEs, FontFactory.GetFont("Arial", 8))), New PdfPCell(New Paragraph(TipoRelacion, FontFactory.GetFont("Arial", 8))), New PdfPCell(New Paragraph(EdadES, FontFactory.GetFont("Arial", 8))), New PdfPCell(New Paragraph(NivelEducativo, FontFactory.GetFont("Arial", 8))), New PdfPCell(New Paragraph(NomSitLaboral, FontFactory.GetFont("Arial", 8))), New PdfPCell(New Paragraph(Ingreso, FontFactory.GetFont("Arial", 8))), New PdfPCell(New Paragraph(Dx, FontFactory.GetFont("Arial", 8)))}
            Dim rowsPDF As PdfPRow = New PdfPRow(cells1)
            tblGrupoFamiliar.Rows.Add(rowsPDF)
        Next


        'Llenado de tabla de Grupo Familiar
        Parrafo.Alignment = Element.ALIGN_LEFT 'Alinea el parrafo para que sea centrado o justificado
        Parrafo.Font = FontFactory.GetFont("Arial", 11, Font.BOLD) 'Asigan fuente
        Parrafo.Add("    Grupo Familiar") 'Texto que se insertara
        Parrafo.Add(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        Documento.Add(Parrafo) 'Agrega el parrafo al documento
        Parrafo.Clear() 'Limpia el parrafo para que despues pueda ser utilizado nuevamente
        tblGrupoFamiliar.DefaultCell.Border = 0
        tblGrupoFamiliar.DefaultCell.Padding = 1
        tblGrupoFamiliar.SetWidthPercentage(New Single() {150, 100, 60, 100, 100, 60, 30}, PageSize.LETTER) 'Ajusta el tamaño de cada columna
        Documento.Add(tblGrupoFamiliar)


        Parrafo.Add("    Condiciones de vivienda") 'Texto que se insertara
        Documento.Add(Parrafo) 'Agrega el parrafo al documento
        Parrafo.Clear() 'Limpia el parrafo para que despues pueda ser utilizado nuevamente
        'Documento.Add(New Paragraph(" ")) 'Salto de linea
        tblCONDICIONESVIVIENDA.DefaultCell.Border = 0
        tblCONDICIONESVIVIENDA.DefaultCell.Padding = 1
        tblCONDICIONESVIVIENDA.SetWidthPercentage(New Single() {75, 225, 100, 200}, PageSize.LETTER) 'Ajusta el tamaño de cada columna
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("   Tipo vivienda:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph(TipoVivienda, FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("   Num. de ambientes:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph(NumAmbiente, FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("   Servicios:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph(Servicios, FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("   Tipo construcción:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph(TipoConstruccion, FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblCONDICIONESVIVIENDA.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        Documento.Add(tblCONDICIONESVIVIENDA)

        Parrafo.Add("    INGRESOS Y EGRESOS") 'Texto que se insertara
        Documento.Add(Parrafo) 'Agrega el parrafo al documento
        Parrafo.Clear() 'Limpia el parrafo para que despues pueda ser utilizado nuevamente
        tblINGRESOEGRESO.DefaultCell.Border = 0
        tblINGRESOEGRESO.DefaultCell.Padding = 1
        tblINGRESOEGRESO.SetWidthPercentage(New Single() {75, 50, 100, 50, 75, 250}, PageSize.LETTER) 'Ajusta el tamaño de cada columna
        'Analisis Financiero
        tblAnalisisFinanciero.DefaultCell.Border = 0
        tblAnalisisFinanciero.DefaultCell.Padding = 1
        tblAnalisisFinanciero.SetWidthPercentage(New Single() {600}, PageSize.LETTER) 'Ajusta el tamaño de cada columna
        'Total Analisis Financiero
        tblTAnalisisFinanciero.DefaultCell.Border = 0
        tblTAnalisisFinanciero.DefaultCell.Padding = 1
        tblTAnalisisFinanciero.SetWidthPercentage(New Single() {75, 50, 100, 50, 85, 240}, PageSize.LETTER) 'Ajusta el tamaño de cada columna
        'ProblemasObservaciones
        tblPROBLEMASOBSERVACIONES.DefaultCell.Border = 0
        tblPROBLEMASOBSERVACIONES.DefaultCell.Padding = 1
        tblPROBLEMASOBSERVACIONES.SetWidthPercentage(New Single() {125, 475}, PageSize.LETTER) 'Ajusta el tamaño de cada columna

        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("  INGRESOS:", FontFactory.GetFont("Arial", 9, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   Personal:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(Personal, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   Hogar:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(Hogar, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("  EGRESOS:", FontFactory.GetFont("Arial", 9, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   Vivienda:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(Vivienda, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   	Energía Eléctrica:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(EnergiaElectrica, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   Agua Potable:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(AguaPotable, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   Cable:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(Cable, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   	Teléfono:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(ETelefono, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   Alimentación:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(Alimentacion, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   Transporte:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(Transporte, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   	Educación:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(Educacion, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   Basura:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(Basura, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("   Otros:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblINGRESOEGRESO.AddCell(New Paragraph(Otros, FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblINGRESOEGRESO.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblAnalisisFinanciero.AddCell(New Paragraph("  ANÁLISIS FINANCIERO:", FontFactory.GetFont("Arial", 9, Font.BOLD)))
        tblTAnalisisFinanciero.AddCell(New Paragraph("   Total Ingresos:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblTAnalisisFinanciero.AddCell(New Paragraph(TotalIngreso, FontFactory.GetFont("Arial", 8)))
        tblTAnalisisFinanciero.AddCell(New Paragraph("   	Total Egresos:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblTAnalisisFinanciero.AddCell(New Paragraph(TotalEgreso, FontFactory.GetFont("Arial", 8)))
        tblTAnalisisFinanciero.AddCell(New Paragraph("   Déficit/Súperavit:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblTAnalisisFinanciero.AddCell(New Paragraph(DefSup, FontFactory.GetFont("Arial", 8)))
        tblTAnalisisFinanciero.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblTAnalisisFinanciero.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblTAnalisisFinanciero.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblTAnalisisFinanciero.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblTAnalisisFinanciero.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblTAnalisisFinanciero.AddCell(New Paragraph("                  ", FontFactory.GetFont("Arial", 8)))
        tblPROBLEMASOBSERVACIONES.AddCell(New Paragraph("   	Problemas Identificados:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblPROBLEMASOBSERVACIONES.AddCell(New Paragraph(ProblemasIdentificados, FontFactory.GetFont("Arial", 8)))
        tblPROBLEMASOBSERVACIONES.AddCell(New Paragraph("   Observaciones:", FontFactory.GetFont("Arial", 8, Font.BOLD)))
        tblPROBLEMASOBSERVACIONES.AddCell(New Paragraph(Observaciones, FontFactory.GetFont("Arial", 8)))
        Documento.Add(tblINGRESOEGRESO)
        Documento.Add(tblAnalisisFinanciero)
        Documento.Add(tblTAnalisisFinanciero)
        Documento.Add(tblPROBLEMASOBSERVACIONES)

        'tblFooter.DefaultCell.Padding = 8
        'tblFooter.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER
        'tblFooter.SetWidthPercentage(New Single() {600}, PageSize.LETTER) 'Ajusta el tamaño de cada columna
        'tblFooter.AddCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, Element.ALIGN_CENTER)))
        'Documento.Add(tblFooter) 'Agrega la tabla al documento

        Documento.Close() 'Cierra el documento
        System.Diagnostics.Process.Start(archivo) 'Abre el archivo DEMO.PDF
        btnExport.Visible = False
        Dim link As String = "TS/" & narchivo
        Dim script As String = "javascript:window.open(""" & link & """)"
        btn_abrir.Attributes.Add("OnClientClick", script)
        btn_abrir.OnClientClick = script
        'Response.Redirect(link)
        btn_abrir.Visible = True
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        If Not revisar.RevisaSesion(Session("conexion").ToString(), Session("usuario").ToString()) Then
            Response.Redirect("~/inicio.aspx", False)
        Else
            'Genera nombre de archivo a exportar
            nhc = DirectCast(tblbasal.FindControl("txt_asi"), TextBox).Text.ToString()
            Dim fecha As String = Now.Year.ToString() & Now.Month.ToString("00") & Now.Day.ToString("00")
            Dim hora As String = Now.Hour.ToString("00") & Now.Minute.ToString("00") & Now.Second.ToString("00")
            Dim farchivo As String = fecha & hora
            Dim narchivo As String = "EstudioS_" & nhc & "_" & farchivo & ".PDF"
            Dim path As String = ConfigurationManager.AppSettings("TS")
            Dim archivo As String = (path & narchivo)
            ExportarPDF(Session("usuario").ToString(), archivo, narchivo)
        End If
    End Sub


    Private Sub llenaestudios(ByVal nhc As String)
        db.Cn2 = cn2

        GV_pnl_SE.DataSource = Nothing
        GV_pnl_SE.DataBind()

        usuario = Session("usuario").ToString()

        Dim datos As DataTable = db.Buscar_Estudios(nhc, usuario)

        Try
            Session("dspcA") = datos
            GV_pnl_SE.DataSource = datos
            GV_pnl_SE.DataBind()


        Catch ex As Exception



        End Try


    End Sub



End Class
