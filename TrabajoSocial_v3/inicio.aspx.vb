
Partial Class inicio
    Inherits System.Web.UI.Page
    Private db As New BusinessLogicDB()
    Public cn1 As String = ConfigurationManager.ConnectionStrings("conStringTS").ConnectionString
    Public cn2 As String = ConfigurationManager.ConnectionStrings("conString").ConnectionString
    Public cn3 As String = ConfigurationManager.ConnectionStrings("conStringAdminUsr").ConnectionString
    Public app As String = ConfigurationManager.AppSettings("app").ToString
    Public errores As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Session("usuario") = ""
            Session("conexion") = "F"
            FailureText.Text = ""
            Session("ip") = Request.UserHostAddress
            Session("ua") = "TRASOC"
            UserName.Focus()
        Else
            Session("usuario") = ""
            Session("conexion") = "F"
            FailureText.Text = ""
            Session("ip") = Request.UserHostAddress
            UserName.Focus()
        End If
    End Sub

    Protected Sub LoginButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LoginButton.Click
        Session("usuario") = ""
        Session("conexion") = "F"
        FailureText.Text = ""
        db.Cn1 = cn1
        db.Cn2 = cn2
        db.Cn3 = cn3
        Try
            Dim valida As String = db.LoginValidation(UserName.Text, Password.Text, app)
            Dim v As String() = valida.Split("|")
            If v(0).ToString() = "True" Then
                Session("usuario") = UserName.Text
                Session("iusuario") = v(1).ToString()
                Session("nusuario") = v(2).ToString()
                Session("pusuario") = v(3).ToString()
                Session("edicion") = v(4).ToString()
                Session("conexion") = "T"
                Session("ip") = Request.UserHostAddress
                db.GrabaSesion(Session("iusuario").ToString(), "I", Session("ip").ToString(), Session("usuario").ToString())
                UserName.Text = ""
                Password.Text = ""
                Select Case Session("pusuario").ToString()
                    Case "1", "2", "3" 'Master, Administrador, Digitador
                        'Response.Redirect("~/BasalTS.aspx", False)
                        Response.Redirect("~/calendario.aspx", False)
                    Case "4", "5", "6" 'Consulta, Reportes, Supervisor
                        Response.Redirect("~/blank.aspx", False)
                End Select
            Else
                FailureText.Text = v(1).ToString()
                UserName.Text = ""
                Password.Text = ""
                Session("conexion") = "F"
                Session("usuario") = ""
                Session("iusuario") = ""
                Session("nusuario") = ""
                Session("pusuario") = ""
                Session("edicion") = ""
            End If
        Catch ex As Exception
            errores = (UserName.Text + "|inicio.Login()|" & ex.ToString() & "|") + ex.Message
            db.GrabarErrores(errores)
            UserName.Text = ""
            Password.Text = ""
            'lblmsgerror.Text = "Hubo problema al iniciar sesion, intente de nuevo."
        End Try
    End Sub
End Class
