Imports System.Data
Partial Class Circuito
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
                llenaStatusCircuito()
                llenaPeriodoCircuito()
                llenaGrupoCircuito()
            End If

        End If
    End Sub
    Sub buscaNHC()
        If txt_asi.Text.ToUpper().Trim <> String.Empty Then
            llenadatos(txt_asi.Text.ToUpper())

            Session("idPaciente") = hd_idpaciente.Value

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

    Protected Sub btn_buscar_Click(sender As Object, e As ImageClickEventArgs) Handles btn_buscar.Click
        buscaNHC()
    End Sub

    Protected Sub txt_asi_TextChanged(sender As Object, e As EventArgs) Handles txt_asi.TextChanged
        buscaNHC()
    End Sub
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
                lbl_deptores.Text = String.Empty
                lbl_direccion.Text = String.Empty
                lbl_telefono.Text = String.Empty
                lbl_movil.Text = String.Empty
                lbl_niveleducativo.Text = String.Empty
                lbl_orientacionsex.Text = String.Empty
                lbl_estatus.Text = String.Empty
                hd_idpaciente.Value = String.Empty
                lbl_etnia.Text = String.Empty
                'asignacion
                lbl_documento.Text = rp(2).ToString()
                lbl_hospitalaria.Text = rp(1).ToString()
                lbl_nombre.Text = rp(4).ToString()
                lbl_genero.Text = rp(3).ToString()
                lbl_nacimiento.Text = rp(7).ToString()
                lbl_edad.Text = rp(8).ToString()
                lbl_deptores.Text = rp(13).ToString()
                lbl_direccion.Text = rp(15).ToString()
                lbl_orientacionsex.Text = rp(19).ToString()
                lbl_telefono.Text = rp(5).ToString()
                lbl_movil.Text = rp(6).ToString()
                lbl_niveleducativo.Text = rp(17).ToString()
                lbl_estatus.Text = rp(21).ToString()
                hd_idpaciente.Value = rp(22).ToString()
                lbl_etnia.Text = rp(23).ToString()
                lbl_error.Text = String.Empty
                btn_editar.Visible = False
                btn_agregar.Visible = False
                'Busca si posee Basales TS
                Dim existe As Integer
                existe = Existe_NHC_Circuito(txt_asi.Text.ToString())
                If existe <> 0 Then
                    btnGrabar.Visible = False
                    lbl_error.Text = "El paciente YA existe en Circuito"
                    'lbl_error.Text = "Existe NHC en Basal TS"
                    'condicion para bloquear el boto de grabar!
                ElseIf existe = 0 Then
                    lbl_error.Text = "El paciente NO existe en Circuito"
                    btnGrabar.Visible = True
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


    Function str(ByVal x As String) As String
        Dim z As String
        If x = String.Empty Then
            z = 0
        Else
            z = x
        End If
        Return z
    End Function



    Function Existe_NHC_Circuito(ByVal nhc As String) As Integer
        db.Cn1 = cn1
        Return db.ExisteNHC_circuito(nhc, usuario)
    End Function


    Private Sub llenaStatusCircuito()
        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_CIRCUITO(1, usuario)
        ddl_circuito.DataSource = datos
        ddl_circuito.DataTextField = "Nom_Status"
        ddl_circuito.DataValueField = "IdStatus"
        ddl_circuito.DataBind()
        ddl_circuito.Items.Insert(0, New ListItem("", "0"))
    End Sub

    Private Sub llenaPeriodoCircuito()
        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_CIRCUITO(2, usuario)
        ddl_periodo_cir.DataSource = datos
        ddl_periodo_cir.DataTextField = "Descripcion_Periodo"
        ddl_periodo_cir.DataValueField = "IdPeriodo"
        ddl_periodo_cir.DataBind()
        ddl_periodo_cir.Items.Insert(0, New ListItem("", "0"))
    End Sub

    Private Sub llenaGrupoCircuito()
        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_CIRCUITO(3, usuario)
        ddl_grupo_cir.DataSource = datos
        ddl_grupo_cir.DataTextField = "Nom_Grupo"
        ddl_grupo_cir.DataValueField = "IdGrupo"
        ddl_grupo_cir.DataBind()
        ddl_grupo_cir.Items.Insert(0, New ListItem("", "0"))
    End Sub

    Protected Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        db.Cn1 = cn1
        usuario = Session("usuario").ToString()
        Dim existe As Integer
        existe = Existe_NHC_Circuito(txt_asi.Text.ToString())
        If existe = 0 Then

            Dim fecha_ing As String = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            Dim idcircuitots As String = db.Graba_Circuito(Session("idPaciente").ToString(), txt_asi.Text.ToString(), ddl_circuito.SelectedValue.ToString(), ddl_año_circuito.SelectedValue.ToString(), ddl_periodo_cir.SelectedValue.ToString(), ddl_grupo_cir.SelectedValue.ToString(), fecha_ing, usuario)
            Response.Redirect("~/Circuito.aspx", False)
        ElseIf existe <> 0 Then

            lbl_error.Text = "El paciente YA existe en Circuito"
        End If


    End Sub

 
    Protected Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Response.Redirect("~/Circuito.aspx", False)
    End Sub
End Class
