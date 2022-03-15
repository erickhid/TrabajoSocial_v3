Imports System.Data.SqlClient
Imports System.Data
Imports System.Threading
Imports System.Globalization

Partial Class RepDiarioV
    Inherits System.Web.UI.Page
    Private revisar As New Rsesion()
    Private db As New BusinessLogicDB()
    Public usuario As String = ""
    Public errores As String = ""
    Private Const TimeoutDB As Integer = 600
    Public con As String = ConfigurationManager.ConnectionStrings("conString").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
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
                If Not String.IsNullOrEmpty(Session("diareporte").ToString()) Then
                    txt_fecha.Text = Session("diareporte").ToString()
                    generar()
                Else
                    txt_fecha.Text = Date.Today()
                End If
            End If
        End If
    End Sub

    Protected Sub btn_generar_Click(sender As Object, e As EventArgs) Handles btn_generar.Click
        generar()
    End Sub

    Sub generar()
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        lbl_titulo.Text = String.Empty
        If txt_fecha.Text <> String.Empty Then
            If IsDate(txt_fecha.Text) Then
                Dim tbpacA As DataTable = ObtieneVisitas(txt_fecha.Text.ToString())
                GridView1.DataSource = tbpacA
                GridView1.DataBind()
				
                'Dim fecha As Date = New Date(txt_fecha.Text.Substring(6, 4), txt_fecha.Text.Substring(3, 2), txt_fecha.Text.Substring(0, 2))
                'lbl_titulo.Text = diasemana(fecha.DayOfWeek) & " " & fecha.Day & " de " & mes(fecha.Month) & " de " & fecha.Year
				If txt_fecha.Text.Length >= 10 Then
                    Dim fecha As Date = New Date(txt_fecha.Text.Substring(6, 4), txt_fecha.Text.Substring(3, 2), txt_fecha.Text.Substring(0, 2))
                    lbl_titulo.Text = diasemana(fecha.DayOfWeek) & " " & fecha.Day & " de " & mes(fecha.Month) & " de " & fecha.Year
                    lbl_error.Text = "Error en fecha: " + txt_fecha.Text
                Else

                End If

			Else
                lbl_error.Text = "No es una fecha válida!"
            End If
        Else
            lbl_error.Text = "Ingrese fecha!"
        End If
    End Sub

    Function diasemana(ByVal ndia As Integer) As String
        Dim x As String = String.Empty
        Select Case ndia
            Case 0
                x = "Domingo"
            Case 1
                x = "Lunes"
            Case 2
                x = "Martes"
            Case 3
                x = "Miércoles"
            Case 4
                x = "Jueves"
            Case 5
                x = "Viernes"
            Case 6
                x = "Sábado"
        End Select
        Return x
    End Function

    Function mes(ByVal nmes As Integer) As String
        Dim x As String = String.Empty
        Select Case nmes
            Case 1
                x = "Enero"
            Case 2
                x = "Febrero"
            Case 3
                x = "Marzo"
            Case 4
                x = "Abril"
            Case 5
                x = "Mayo"
            Case 6
                x = "Junio"
            Case 7
                x = "Julio"
            Case 8
                x = "Agosto"
            Case 9
                x = "Septiembre"
            Case 10
                x = "Octubre"
            Case 11
                x = "Noviembre"
            Case 12
                x = "Diciembre"
        End Select
        Return x
    End Function

    Function ObtieneVisitas(ByVal fecha As String) As DataTable
        Dim Query As String = String.Empty
        Query = "SELECT ROW_NUMBER() OVER( ORDER BY X.Horario ASC ) AS 'nro', X.Nombre, X.Genero, X.NumHospitalaria, X.NHC, X.Edad, X.Ultima_Visita, X.Tiempo_Ultima_Visita, X.Tiempo_Dias, dbo.fn_ObtieneJornadaTS(Y.Jornada) AS 'Jornada', dbo.fn_ObtieneHorarioCita(X.Horario) AS 'Horario',  dbo.fn_ObtieneClinicaTS(Y.Clinica) AS 'Clinica',X.FechaUltimoCD4, X.UltimoCD4, X.Clasificación_Pac "
        Query += "FROM (SELECT DISTINCT A.IdPaciente, (LTRIM(RTRIM(A.PrimerNombre)) + (CASE WHEN A.SegundoNombre IS NULL THEN '' ELSE ' ' + LTRIM(RTRIM(A.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(A.PrimerApellido)) + (CASE WHEN A.SegundoApellido IS NULL THEN '' ELSE ' ' + LTRIM(RTRIM(A.SegundoApellido)) END)) AS Nombre, "
        Query += "dbo.fn_ObtieneGenero(A.IdGenero) AS 'Genero', A.NumHospitalaria, B.NHC, "
        Query += "dbo.fn_ObtieneEdad2(A.FechaNacimiento, GETDATE()) AS Edad, "
        Query += String.Format("(SELECT TOP 1 IdSignosVitales FROM SIGNOSVITALES AS D1 WHERE (D1.IdPaciente = A.IdPaciente) AND D1.FechaVisita < CONVERT(DATE, '{0}', 103) ORDER BY D1.FechaVisita DESC) AS 'IdSignosVitales', ", fecha.ToString())
        Query += String.Format("(SELECT TOP 1 IdSignosVitales FROM SIGNOSVITALES AS D1 WHERE (D1.IdPaciente = A.IdPaciente) AND D1.FechaProximaVisita = CONVERT(DATE, '{0}', 103) ORDER BY D1.FechaVisita DESC) AS 'IdSignosVitales2', ", fecha.ToString())
        Query += String.Format("(SELECT TOP 1 CONVERT(VARCHAR, D1.FechaVisita, 103) FROM SIGNOSVITALES AS D1 WHERE (D1.IdPaciente = A.IdPaciente) AND D1.FechaVisita < CONVERT(DATE, '{0}', 103) ORDER BY D1.FechaVisita DESC) AS 'Ultima_Visita', ", fecha.ToString())
        Query += String.Format("(SELECT TOP 1 dbo.fn_ObtieneEdad(D1.FechaVisita, CONVERT(DATE, '{0}', 103)) FROM SIGNOSVITALES AS D1 WHERE (D1.IdPaciente = A.IdPaciente) AND D1.FechaVisita < CONVERT(DATE, '{0}', 103) ORDER BY D1.FechaVisita DESC) AS 'Tiempo_Ultima_Visita', ", fecha.ToString())
        Query += String.Format("(SELECT TOP 1 dbo.fn_ObtieneFechaDias(D1.FechaVisita, CONVERT(DATE, '{0}', 103)) FROM SIGNOSVITALES AS D1 WHERE (D1.IdPaciente = A.IdPaciente) AND D1.FechaVisita < CONVERT(DATE, '{0}', 103) ORDER BY D1.FechaVisita DESC) AS 'Tiempo_Dias', ", fecha.ToString())
        Query += String.Format("(SELECT TOP 1 C.IdHorario FROM BDTrabajoSocial.dbo.PAC_CITAS AS C WHERE C.IdPaciente = A.IdPaciente AND C.FechaProximaVisita = CONVERT(date, '{0}', 103)) AS 'Horario', ", fecha.ToString())
        Query += String.Format("(SELECT TOP(1) convert(VARCHAR,A1.FechaAnalitica,103) FROM ANA_ESPECIAL AS A1 WHERE A1.IdPaciente = B.IdPaciente AND A1.CD4 IS NOT NULL ORDER BY A1.FechaAnalitica DESC) AS 'FechaUltimoCD4', ", fecha.ToString())
        Query += String.Format("(SELECT TOP(1) A1.CD4 FROM ANA_ESPECIAL AS A1 WHERE A1.IdPaciente = B.IdPaciente AND A1.CD4 IS NOT NULL ORDER BY A1.FechaAnalitica DESC) AS 'UltimoCD4', ", fecha.ToString())
        Query += " (SELECT TOP 1 dbo.fn_ObtieneClasificacion_pac(PEP.Id_Clasificacion_Pac) FROM BDEvolucionPX.dbo.PSOEP AS PEP WHERE PEP.NHC = B.NHC AND PEP.Id_Clasificacion_Pac IS NOT NULL  ORDER BY PEP.FechaFicha DESC) AS 'Clasificación_Pac' "
        Query += "FROM PAC_BASALES AS A LEFT OUTER JOIN "
        Query += "PAC_ID AS B ON A.IdPaciente = B.IdPaciente LEFT OUTER JOIN "
        Query += "SIGNOSVITALES AS D ON B.IdPaciente = D.IdPaciente "
        Query += String.Format("WHERE CONVERT(VARCHAR, D.FechaProximaVisita, 103) = '{0}') AS X LEFT OUTER JOIN ", fecha.ToString())
        Query += "BDTrabajoSocial.dbo.PAC_CITAS AS Y ON X.IdSignosVitales2 = Y.IdSignosVitales "
        Query += "ORDER BY X.Horario"

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(con)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            lbl_error.Text = ex.Number & "|" & ex.Message & "| query: " & Query
            Return Nothing
        End Try
    End Function

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    End Sub

    Protected Sub IB_exportar_Click(sender As Object, e As ImageClickEventArgs) Handles IB_exportar.Click
        Dim nombre As String = "Reporte_de_visitas_del_" & lbl_titulo.Text()
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
