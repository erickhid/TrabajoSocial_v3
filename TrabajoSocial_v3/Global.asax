<%@ Application Language="VB" %>

<script runat="server">

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
        Session("nhc") = ""
        Session("idufecha") = ""
        Session("ufecha") = False
        Session("dsffarv") = ""
        Session("dsffprof") = ""
        Session("dsff") = ""
        Session("idFFARV") = ""
        Session("usuario") = ""
        Session("iusuario") = ""
        Session("nusuario") = ""
        Session("pusuario") = ""
        Session("edicion") = ""
        Session("conexion") = "F"
        Session("dspacA") = ""
        Session("dspacP") = ""
        Session("idPaciente") = ""
        Session("idGrupoFamiliar") = ""
        Session("dspacresA") = ""
        Session("datosGF") = ""
        Session("diareporte") = ""
        Session("feriados") = ""
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
        Dim db As BusinessLogicDB = New BusinessLogicDB()
        db.Cn1 = ConfigurationManager.ConnectionStrings("conStringTS").ConnectionString
        If db.Desconectar(Session("iusuario").ToString(), Session("ip").ToString(), Session("usuario").ToString()) Then
            Session("nhc") = ""
            Session("idufecha") = ""
            Session("ufecha") = False
            Session("dsffarv") = ""
            Session("dsffprof") = ""
            Session("dsff") = ""
            Session("idFFARV") = ""
            Session("usuario") = ""
            Session("iusuario") = ""
            Session("nusuario") = ""
            Session("pusuario") = ""
            Session("edicion") = ""
            Session("conexion") = "F"
            Session("dspacA") = ""
            Session("dspacP") = ""
            Session("idPaciente") = ""
            Session("idGrupoFamiliar") = ""
            Session("dspacresA") = ""
            Session("datosGF") = ""
            Session("diareporte") = ""
            Session("feriados") = ""
            'Response.Redirect("~/inicio.aspx", False)
        End If
    End Sub

</script>