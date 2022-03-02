Imports System.Data

Partial Class ConsultaV
    Inherits System.Web.UI.Page
    Private db As New BusinessLogicDB()
    Private revisar As New Rsesion()
    Public cn1 As String = ConfigurationManager.ConnectionStrings("conStringTS").ConnectionString
    Public cn2 As String = ConfigurationManager.ConnectionStrings("conString").ConnectionString
    Public usuario As String = ""
    'Public fecha As String = Convert.ToString(Date.Today.ToString("dd/MM/yyyy"))
    Public errores As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Response.Buffer = True
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1.0)
            Response.Expires = -1500
            Response.CacheControl = "no-cache"
            If Not revisar.RevisaSesion(Session("conexion").ToString(), Session("usuario").ToString()) Then
                Response.Redirect("~/inicio.aspx", False)
            Else
                usuario = Session("usuario").ToString()
                'Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-ES")
                'Thread.CurrentThread.CurrentUICulture = New CultureInfo("es-ES")
                txt_fecha.Text = Date.Today()
            End If
        End If
        '*****
        'usuario = "GUEST"
        'lbl_tlistado.Text = "PACIENTES VISITA MEDICA - " & fecha
        'LlenaGv(fecha)
    End Sub

    Private Sub LlenaGv()
        GV_pacientes.DataSource = Nothing
        GV_pacientes.DataBind()
        lbl_tlistado.Text = String.Empty
        If txt_fecha.Text <> String.Empty Then
            If IsDate(txt_fecha.Text) Then
                Try
                    db.Cn2 = cn2
                    Dim pacientes As DataTable = db.ObtienePacientes(txt_fecha.Text.ToString(), usuario)
                    GV_pacientes.DataSource = pacientes
                    GV_pacientes.DataBind()
                    Dim fecha As Date = New Date(txt_fecha.Text.Substring(6, 4), txt_fecha.Text.Substring(3, 2), txt_fecha.Text.Substring(0, 2))
                    lbl_tlistado.Text = "PACIENTES VISITA MEDICA - " & fecha
                    lbl_gv.Text = pacientes.Rows.Count.ToString() + " Programado(s), "
                    Dim visitas As DataRow() = pacientes.Select("Visita = 'V'")
                    lbl_gv.Text += visitas.Length.ToString() + " Atendido(s)."
                Catch ex As Exception
                    lbl_error.Text = "Hubo un error al mostrar listado de Pacientes."
                    errores = (usuario & "|ConsultaV.LLenaGv()|" & ex.ToString() & "|") + ex.Message
                    db.GrabarErrores(errores)
                End Try
            Else
                lbl_error.Text = "No es una fecha válida!"
            End If
        Else
            lbl_error.Text = "Ingrese fecha!"
        End If
    End Sub

   Function imgVisita(ByVal iv As String) As String
        Dim x As String
        If iv = String.Empty Then
            x = "▫"
        Else
            x = "▪"
        End If
        Return x
    End Function

    Function imgVisita1(ByVal iv As String) As String
        Dim x As String
        If iv = String.Empty Then
            x = "tbl_estatusN"
            'x = "images/n.png"
        Else
            x = "tbl_estatusV"
            'x = "images/v.png"
        End If
        Return x
    End Function

    'Protected Sub GV_pacientes_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GV_pacientes.RowCommand
    '    If e.CommandName = "Editar" Then
    '        Try
    '            Dim gv As GridView = DirectCast(sender, GridView)
    '            Dim rowIndex As Int32 = Convert.ToInt32(e.CommandArgument.ToString())
    '            Dim nhc As String = gv.DataKeys(rowIndex)(0).ToString()
    '            Session("nhc") = nhc
    '            Response.Redirect("~/consulta_paciente.aspx?P=" & nhc, False)
    '        Catch ex As Exception
    '            lbl_error.Text = ex.Message
    '        End Try
    '    End If
    'End Sub

    'Protected Sub GV_pacientes_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles GV_pacientes.PreRender
    '    Dim n As Integer = 0
    '    For Each nrow As GridViewRow In GV_pacientes.Rows
    '        For columnIndex As Integer = n To Convert.ToInt32(GV_pacientes.Rows.Count)
    '            Dim irow1 As ImageButton = DirectCast(nrow.FindControl("IB_editar"), ImageButton)
    '            irow1.CommandArgument = Convert.ToString(n)
    '            Dim irow2 As LinkButton = DirectCast(nrow.FindControl("lkb_paciente"), LinkButton)
    '            irow2.CommandArgument = Convert.ToString(n)
    '            Dim irow3 As LinkButton = DirectCast(nrow.FindControl("lkb_nhc"), LinkButton)
    '            irow3.CommandArgument = Convert.ToString(n)
    '        Next
    '        n += 1
    '    Next
    'End Sub

    Protected Sub btn_generar_Click(sender As Object, e As EventArgs) Handles btn_generar.Click
        LlenaGv()
    End Sub

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    End Sub

    Protected Sub IB_exportar_Click(sender As Object, e As ImageClickEventArgs) Handles IB_exportar.Click
        Dim nombre As String = "Consulta_de_visitas_del_" & lbl_tlistado.Text()
        Response.Clear()
        Response.AddHeader("content-disposition", "attachment;filename=" & nombre & ".xls")
        Response.Charset = "UTF-8"
        Response.ContentEncoding = Encoding.Default
        Response.ContentType = "application/vnd.xls"
        Dim stringWrite As System.IO.StringWriter = New System.IO.StringWriter
        Dim htmlWrite As System.Web.UI.HtmlTextWriter = New HtmlTextWriter(stringWrite)
        tbl_reporte.RenderControl(htmlWrite)
        Response.Write(stringWrite.ToString)
        Response.End()
    End Sub
End Class
