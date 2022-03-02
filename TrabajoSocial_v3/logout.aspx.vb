
Partial Class logout
    Inherits System.Web.UI.Page
    Private revisar As New Rsesion()
    Private db As New BusinessLogicDB()
    Public cn1 As String = ConfigurationManager.ConnectionStrings("conStringTS").ConnectionString
    Public cn2 As String = ConfigurationManager.ConnectionStrings("conString").ConnectionString

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Response.Buffer = True
        Response.ExpiresAbsolute = DateTime.Now.AddDays(-1.0)
        Response.Expires = -1500
        Response.CacheControl = "no-cache"
        If Not revisar.RevisaSesion(Session("conexion").ToString(), Session("usuario").ToString()) Then
            Response.Redirect("~/inicio.aspx", False)
        Else
            db.Cn1 = cn1
            If db.Desconectar(Session("iusuario").ToString(), Session("ip").ToString(), Session("usuario").ToString()) Then
                Session("conexion") = "F"
                Session("usuario") = ""
                Session("nusuario") = ""
                Session("pusuario") = ""
                Response.Redirect("~/inicio.aspx", False)
            Else
                Session("conexion") = "F"
                Session("usuario") = ""
                Session("nusuario") = ""
                Session("pusuario") = ""
                Response.Redirect("~/inicio.aspx", False)
            End If
        End If
    End Sub
End Class
