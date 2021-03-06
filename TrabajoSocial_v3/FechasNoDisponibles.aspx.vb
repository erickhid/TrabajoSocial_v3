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
                txt_fecha.Text = Date.Today()
                LlenaGv()
            End If
        End If
    End Sub

    Private Sub LlenaGv()
        Dim errmsg As String = String.Empty
        GV_fechasnd.DataSource = Nothing
        GV_fechasnd.DataBind()
        lbl_tlistado.Text = String.Empty
        If txt_fecha.Text <> String.Empty Then
            If IsDate(txt_fecha.Text) Then
                Try
                    db.Cn1 = cn1
                    Dim fechasnd As DataTable = db.ObtieneFechasNoDisponibles(txt_fecha.Text.ToString(), usuario)
                    GV_fechasnd.DataSource = fechasnd
                    GV_fechasnd.DataBind()
                    Dim fecha As Date = New Date(txt_fecha.Text.Substring(6, 4), txt_fecha.Text.Substring(3, 2), txt_fecha.Text.Substring(0, 2))
                    lbl_tlistado.Text = "FECHAS NO DISPONIBLES PARA CITAS"

                Catch ex As Exception
                    errmsg = "Hubo un error al mostrar listado de fechas no disponibles."
                    errores = (usuario & "|FechasNoDisponibles.LLenaGv()|" & ex.ToString() & "|") + ex.Message
                    db.GrabarErrores(errores)
                    ScriptManager.RegisterStartupScript(Me, Page.GetType(), "alert", "alert('" + errmsg + "');", True)
                End Try
            Else
                errmsg = "No es una fecha válida!"
                ScriptManager.RegisterStartupScript(Me, Page.GetType(), "alert", "alert('" + errmsg + "');", True)
            End If
        Else
            errmsg = "Ingrese fecha!"
            ScriptManager.RegisterStartupScript(Me, Page.GetType(), "alert", "alert('" + errmsg + "');", True)
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
        Dim x As String = String.Empty
        If iv = String.Empty Then
            x = "tbl_estatusN"
            'x = "images/n.png"
        Else
            x = "tbl_estatusV"
            'x = "images/v.png"
        End If
        Return x
    End Function

    Protected Sub btn_generar_Click(sender As Object, e As EventArgs) Handles btn_generar.Click
        db.Cn1 = cn1
        Dim iusuario As Integer = Convert.ToInt32(Session("iusuario").ToString())
        Dim res As String = String.Empty
        Dim resb As String()
        Dim resg As String = String.Empty

        Try
            res = db.ValidaExisteFecha(txt_fecha.Text, usuario)
            resb = res.Split("|")

            If resb(1) = "0" Then

                resg = db.GrabaFechaND(txt_fecha.Text, txt_descri.Text, iusuario, usuario)

                txt_descri.Text = String.Empty

                ScriptManager.RegisterStartupScript(Me, Page.GetType(), "alert", "alert('" + resg + "');", True)

            Else

                'lbl_error.Text = "Fecha ya existe, elija otra fecha"
                resg = "Fecha ya existe, elija otra fecha"
                ScriptManager.RegisterStartupScript(Me, Page.GetType(), "alert", "alert('" + resg + "');", True)

            End If

        Catch ex As Exception
            resg = "Hubo un error al mostrar listado de fechas no disponibles."
            errores = (usuario & "|ConsultaV.LLenaGv()|" & ex.ToString() & "|") + ex.Message
            db.GrabarErrores(errores)

            ScriptManager.RegisterStartupScript(Me, Page.GetType(), "alert", "alert('" + resg + "');", True)
        End Try


        LlenaGv()
        limpiacampos()

    End Sub

    Protected Sub limpiacampos()
        txt_descri.Text = String.Empty
        txt_fecha.Text = Date.Today()
    End Sub

    Protected Sub btn_Eliminar_Click(sender As Object, e As EventArgs) Handles btn_Eliminar.Click
        db.Cn1 = cn1
        usuario = Session("usuario").ToString()
        Dim msjresult As String = String.Empty
        Dim i As Integer

        For i = 0 To GV_fechasnd.Rows.Count - 1
            Dim CheckBoxEliminar As CheckBox = CType(GV_fechasnd.Rows(i).FindControl("chk_eliminar"), CheckBox)
            If CheckBoxEliminar.Checked Then
                Dim IdFechaND As String = CType(GV_fechasnd.Rows(i).FindControl("lbl_IdFechaND"), Label).Text

                msjresult = db.EliminaFechaNoDisponible(IdFechaND, usuario)

                ScriptManager.RegisterStartupScript(Me, Page.GetType(), "alert", "alert('" + msjresult + "');", True)
            End If
        Next

        LlenaGv()
        limpiacampos()

    End Sub
End Class
