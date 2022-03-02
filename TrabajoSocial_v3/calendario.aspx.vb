﻿Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports PacientesTS

Partial Class calendario
    Inherits System.Web.UI.Page
    Private revisar As New Rsesion()
    Private db As New BusinessLogicDB()
    Public cn1 As String = ConfigurationManager.ConnectionStrings("conStringTS").ConnectionString
    Public cn2 As String = ConfigurationManager.ConnectionStrings("conString").ConnectionString
    Public usuario As String = ""
    Public iusuario As Integer
    Public errores As String = ""
    Public spais As Integer
    Public sdepto As Integer
    Public smupio As Integer
    Public existeID As Boolean

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
                iusuario = Session("iusuario").ToString()
                lblUsuario.Value = usuario
                lblUnidadAtencion.Value = Session("ua").ToString()
                txt_asi.Focus()
                LoadMonths()
                LoadYears()
                llenaJornada()
                llenaClinica()
                'LoadMyImportantDates() ' load our important dates before the calendar is loaded '
                LoadCitas()
                LoadVisitas()
                DiaFeriados()
                LoadCalendar()
                btn_agregar.Visible = True

                llenapais()
                llenaPerTel1()
                llenaPerTel2()
                llenaPerTel3()
                llenaPerTel4()

            End If
        End If
    End Sub

    Private Property _dtMyDates As DataTable
        Get
            Return ViewState("_MyDates")
        End Get
        Set(value As DataTable)
            ViewState("_MyDates") = value
        End Set
    End Property

    Protected Sub LoadMonths()
        For i As Integer = 1 To 12
            Dim li As New ListItem
            li.Text = MonthName(i) ' MonthName() gets the month name based on the users computer settings (their local month name rather than English default) '
            li.Value = i.ToString
            ddl_month.Items.Add(li)
            If li.Value = DateTime.Now.Month.ToString Then
                li.Selected = True
            End If
        Next
    End Sub

    Protected Sub LoadYears()
        Dim yearsBack As Integer = 2
        Dim yearsForward As Integer = 3
        For i As Integer = DateTime.Now.AddYears(-yearsBack).Year To DateTime.Now.AddYears(yearsForward).Year
            Dim li As New ListItem
            li.Text = i.ToString
            li.Value = i.ToString
            ddl_year.Items.Add(li)
            If li.Value = DateTime.Now.Year Then
                li.Selected = True
            End If
        Next
    End Sub
    Private Sub LlenaCircuito()
        Try
            db.Cn1 = cn1
            Dim nhc As String = txt_asi.Text.ToString()
            Dim circuito As String = db.obtiene_circuito_info(nhc, usuario)
            Dim rp As String() = circuito.Split("|")
            If rp(0).ToString() = "True" Then
                lbl_circuito_est.Text = String.Empty
                lbl_circuito_est.Text = rp(3).ToString()
                lbl_circ_grupo.Text = rp(4).ToString()
                lbl_circ_periodo.Text = rp(5).ToString()
            ElseIf rp(0).ToString() = "False" Then
                lbl_circuito_est.Text = String.Empty
                lbl_circuito_est.Text = "No se encuentra en Circuito Grupal"
                lbl_circ_grupo.Text = String.Empty
                lbl_circ_periodo.Text = String.Empty

            End If
        Catch ex As Exception
            errores = (usuario & "|consulta_paciente.circuito()|" & ex.ToString() & "|") + ex.Message
            db.GrabarErrores(errores)

        End Try
    End Sub
    Private Sub llenaJornada()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasProximaCitaTS(1, usuario)
        ddl_jornada.DataSource = datos
        ddl_jornada.DataTextField = "Jornada"
        ddl_jornada.DataValueField = "IdJornada"
        ddl_jornada.DataBind()
        ddl_jornada.Items.Insert(0, New ListItem("", "0"))
        ddl_jornada.SelectedIndex = 0
    End Sub

    Private Sub llenaClinica()
        db.Cn1 = cn1
        Dim datos As DataTable = db.listasProximaCitaTS(2, usuario)

        ddl_clinica.DataSource = datos
        ddl_clinica.DataTextField = "Clinica"
        ddl_clinica.DataValueField = "IdClinica"
        ddl_clinica.DataBind()
        ddl_clinica.Items.Insert(0, New ListItem("", "0"))
        ddl_clinica.SelectedIndex = 0
    End Sub

    Private Sub llenahorarios1()
        db.Cn1 = cn1
        ddl_horario_cita.Items.Clear()
        Dim datos As DataTable = db.Llena_horarios_citas(1, usuario)

        ddl_horario_cita.DataSource = datos
        ddl_horario_cita.DataTextField = "Desc_Horario"
        ddl_horario_cita.DataValueField = "Id_Bloque_H"
        ddl_horario_cita.DataBind()
        ddl_horario_cita.Items.Insert(0, New ListItem("", "0"))
        ddl_horario_cita.SelectedIndex = 0
    End Sub
    Private Sub llenahorarios2()
        db.Cn1 = cn1
        ddl_horario_cita.Items.Clear()
        Dim datos As DataTable = db.Llena_horarios_citas(2, usuario)

        ddl_horario_cita.DataSource = datos
        ddl_horario_cita.DataTextField = "Desc_Horario"
        ddl_horario_cita.DataValueField = "Id_Bloque_H"
        ddl_horario_cita.DataBind()
        ddl_horario_cita.Items.Insert(0, New ListItem("", "0"))
        ddl_horario_cita.SelectedIndex = 0
    End Sub

    Protected Sub LoadCitas()
        Try
            db.Cn2 = cn2
            usuario = Session("usuario").ToString()
            Dim m As Integer = Convert.ToInt16(ddl_month.SelectedItem.Value)
            Dim y As Integer = Convert.ToInt16(ddl_year.SelectedItem.Value)
            Dim tbpacA As DataTable = db.ConteoCitas(m, y, usuario)
            Session("dspacA") = tbpacA
        Catch ex As Exception
            'lbl_error.Text = "Hubo un error al mostrar las Propiedades."
            errores = (usuario & "|calendario.LoadCitas()|" & ex.ToString() & "|") + ex.Message
            db.GrabarErrores(errores)
        End Try
    End Sub

    Protected Sub LoadVisitas()
        Try
            db.Cn2 = cn2
            usuario = Session("usuario").ToString()
            Dim m As Integer = Convert.ToInt16(ddl_month.SelectedItem.Value)
            Dim y As Integer = Convert.ToInt16(ddl_year.SelectedItem.Value)
            Dim dspacP As DataTable = db.ConteoVisitas(m, y, usuario)
            Session("dspacP") = dspacP
        Catch ex As Exception
            'lbl_error.Text = "Hubo un error al mostrar las Propiedades."
            errores = (usuario & "|calendario.LoadVisitas()|" & ex.ToString() & "|") + ex.Message
            db.GrabarErrores(errores)
        End Try
    End Sub

    Protected Sub DiaFeriados()
        Try
            db.Cn1 = cn1
            usuario = Session("usuario").ToString()
            Dim m As Integer = Convert.ToInt16(ddl_month.SelectedItem.Value)
            Dim y As Integer = Convert.ToInt16(ddl_year.SelectedItem.Value)
            Dim feriados As DataTable = db.ConteoFeriados(m, usuario)
            'Dim feriados As DataTable = db.ConteoFeriados(usuario)
            Session("feriados") = feriados
            CalculaSemanaSanta(m, y)
        Catch ex As Exception
            'lbl_error.Text = "Hubo un error al mostrar las Propiedades."
            errores = (usuario & "|calendario.DiaFeriados()|" & ex.ToString() & "|") + ex.Message
            db.GrabarErrores(errores)
        End Try
    End Sub

    Protected Sub CalculaSemanaSanta(ByVal m As Integer, ByVal y As Integer)
        Dim ss As New SemanaSanta(y)
        Dim dt2 As DataTable = TryCast(Session("feriados"), DataTable) ' returns the results as a temporary DataTable '
        Dim dr2 As DataRow
        If m = ss.MiercolesSanto.Month Then
            dr2 = dt2.NewRow()
            dr2.Item("DiaFeriado") = ss.MiercolesSanto.Day
            'dr2.Item("MesFeriado") = ss.MiercolesSanto.Month
            dr2.Item("Descripcion") = "Miercoles Santo"
            dt2.Rows.Add(dr2)
        End If
        If m = ss.JuevesSanto.Month Then
            dr2 = dt2.NewRow()
            dr2.Item("DiaFeriado") = ss.JuevesSanto.Day
            'dr2.Item("MesFeriado") = ss.JuevesSanto.Month
            dr2.Item("Descripcion") = "Jueves Santo"
            dt2.Rows.Add(dr2)
        End If
        If m = ss.ViernesSanto.Month Then
            dr2 = dt2.NewRow()
            dr2.Item("DiaFeriado") = ss.ViernesSanto.Day
            'dr2.Item("MesFeriado") = ss.ViernesSanto.Month
            dr2.Item("Descripcion") = "Viernes Santo"
            dt2.Rows.Add(dr2)
        End If
        Session("feriados") = dt2
    End Sub

    ' Creates a bunch of random event dates and adds them into a page-level DataTable variable '
    Protected Sub LoadMyImportantDates()
        Me._dtMyDates = New DataTable
        Me._dtMyDates.Columns.Add(New DataColumn("MyDate", GetType(System.DateTime))) ' This will hold the event date '
        Me._dtMyDates.Columns.Add(New DataColumn("MyEvent", GetType(System.String))) ' This will hold the event details '
        Me._dtMyDates.Columns.Add(New DataColumn("Important", GetType(System.Boolean)))    ' This will hold a True/False value based on importance '
        Dim dr As DataRow = Me._dtMyDates.NewRow()
        dr.Item("MyDate") = New DateTime(2014, 9, 1)
        dr.Item("MyEvent") = "First day of the month - celebrate!"
        dr.Item("Important") = False
        Me._dtMyDates.Rows.Add(dr)
        dr = Me._dtMyDates.NewRow()
        dr.Item("MyDate") = New DateTime(2014, 9, 3)
        dr.Item("MyEvent") = "Pester boss for huge payrise based on this beautiful piece of code"
        dr.Item("Important") = False
        Me._dtMyDates.Rows.Add(dr)
        dr = Me._dtMyDates.NewRow()
        dr.Item("MyDate") = New DateTime(2014, 9, 3)
        dr.Item("MyEvent") = "Sulk about boss's decision"
        dr.Item("Important") = False
        Me._dtMyDates.Rows.Add(dr)
        dr = Me._dtMyDates.NewRow()
        dr.Item("MyDate") = New DateTime(2014, 9, 9)
        dr.Item("MyEvent") = "Take over the company"
        dr.Item("Important") = False
        Me._dtMyDates.Rows.Add(dr)
        dr = Me._dtMyDates.NewRow()
        dr.Item("MyDate") = New DateTime(2014, 9, 10)
        dr.Item("MyEvent") = "Fire previous boss for refusing to give me that payrise"
        dr.Item("Important") = True
        Me._dtMyDates.Rows.Add(dr)
        dr = Me._dtMyDates.NewRow()
        dr.Item("MyDate") = New DateTime(2014, 9, 12)
        dr.Item("MyEvent") = "Kick back and soak up the sun at my new company-funded luxury villa"
        dr.Item("Important") = False
        Me._dtMyDates.Rows.Add(dr)
    End Sub

    Protected Sub btn_generar_Click(sender As Object, e As EventArgs) Handles btn_generar.Click
        ltl_output.Text = String.Empty
        LoadCitas()
        LoadVisitas()
        DiaFeriados()
        LoadCalendar()
    End Sub

    Protected Sub LoadCalendar()
        Dim m As Integer = Convert.ToInt16(ddl_month.SelectedItem.Value)
        Dim y As Integer = Convert.ToInt16(ddl_year.SelectedItem.Value)
        Dim dates As New List(Of DateTime)
        For i As Integer = 1 To Date.DaysInMonth(y, m)
            Dim d As New DateTime(y, m, i)
            dates.Add(d)
        Next
        rpt_calendar.DataSource = dates
        rpt_calendar.DataBind()
    End Sub

    Protected Sub rpt_calendar_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rpt_calendar.ItemCreated
        Dim lnk_dayLink As LinkButton = CType(e.Item.FindControl("lnk_dayLink"), LinkButton)
        AddHandler lnk_dayLink.Command, AddressOf DayLinkClicked ' tell ASP.NET to fire DayLinkClicked() when the linkbutton is clicked '
    End Sub

    Protected Sub rpt_calendar_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpt_calendar.ItemDataBound
        ' Find controls '
        Dim pnl_calendarDay As Panel = CType(e.Item.FindControl("pnl_calendarDay"), Panel)
        Dim lnk_dayLink As LinkButton = CType(e.Item.FindControl("lnk_dayLink"), LinkButton)
        Dim ltl_dayEvents As Literal = CType(e.Item.FindControl("ltl_dayEvents"), Literal)
        ' Set values '
        Dim d As DateTime = CType(e.Item.DataItem, DateTime)
        ' Here we set the day value for each day entry within the calendar ' 
        Dim sb As New StringBuilder
        sb.Append(d.Day.ToString)
        sb.Append(" ")
        'sb.Append(d.ToString("dddd").Substring(0, 3))    ' gets the day name based on the users computer settings (their local day name rather than English default) '
        sb.Append(d.ToString("ddd"))    ' gets the day name based on the users computer settings (their local day name rather than English default) '
        lnk_dayLink.Text = sb.ToString
        lnk_dayLink.CommandArgument = d     ' used to read the date value when the link is clicked ' 
        Dim dt As DataTable = TryCast(Session("dspacA"), DataTable) ' returns the results as a temporary DataTable '
        If dt.Rows.Count > 0 Then ' if any dates are found '
            Dim sb_day As New StringBuilder
            For Each dr As DataRow In dt.Rows
                If d.ToShortDateString() = dr.Item("Fecha").ToString Then
                    'sb_day.Append("<br/><div style=""display:inline;"" class="" event tooltip"">")
                    sb_day.Append(dr.Item("Conteo").ToString)
                    sb_day.Append("<div class=""form-group"">")
                    sb_day.Append("<span data-toggle=""tooltip"" data-html=""true"" aria-describedby=""passHelp"" class=""badge badge-secondary"" data-toggle=""tooltip"" data-placement=""right"" ")
                    sb_day.Append("title =""The password must be:<ul><li>8 characters long</li> <li>Start With capital letter</li><li>Contains a special character</li></ul>""> ")

                    sb_day.Append("</span>")
                    sb_day.Append("</div>")
                    'sb_day.Append("<span class=""classic""><em>" & dr.Item("Fecha").ToString() & "</em>")
                    'sb_day.Append("<div class=""Cli2"">CFLAG 2a. Ave.: " & String.Format(CultureInfo.InvariantCulture, "{0:00}", dr.Item("Cli2").ToString()) & "</div>")
                    'sb_day.Append("<div class=""Cli1"">CFLAG 1a. Ave.: " & String.Format(CultureInfo.InvariantCulture, "{0:00}", dr.Item("Cli1").ToString()) & "</div>")
                    'sb_day.Append("<div class=""JT"">Jornada Tarde: " & String.Format(CultureInfo.InvariantCulture, "{0:00}", dr.Item("JT").ToString()) & "</div>")
                    'sb_day.Append("</span>")
                    'sb_day.Append("</div>")
                End If
            Next
            ltl_dayEvents.Text = sb_day.ToString
        End If
        Dim dt1 As DataTable = TryCast(Session("dspacP"), DataTable) ' returns the results as a temporary DataTable '
        If dt1.Rows.Count > 0 Then ' if any dates are found '
            Dim sb_day1 As New StringBuilder
            For Each dr1 As DataRow In dt1.Rows
                If d.ToShortDateString() = dr1.Item("Fecha").ToString Then
                    sb_day1.Append("<span class=""event1"">-")
                    sb_day1.Append(dr1.Item("Conteo").ToString)
                    sb_day1.Append("</span>")
                End If
            Next
            ltl_dayEvents.Text += sb_day1.ToString
        End If

        If d.DayOfWeek = DayOfWeek.Saturday Or d.DayOfWeek = DayOfWeek.Sunday Then
            pnl_calendarDay.CssClass = "calendarDay2"
        Else
            Dim dt2 As DataTable = TryCast(Session("feriados"), DataTable) ' returns the results as a temporary DataTable '
            If dt2.Rows.Count > 0 Then ' if any dates are found '
                For Each dr2 As DataRow In dt2.Rows
                    If d.Day = dr2.Item("DiaFeriado").ToString() Then
                        pnl_calendarDay.CssClass = "calendarDay3"
                    End If
                Next
            Else
                pnl_calendarDay.CssClass = "calendarDay"
            End If
        End If
    End Sub

    Protected Sub DayLinkClicked(sender As Object, e As CommandEventArgs) ' Handles set by rpt_calendar_ItemCreated() '
        Dim d As DateTime = Convert.ToDateTime(e.CommandArgument)
        'ltl_output.Text = "<p>" & d.ToShortDateString() & "</p>"
        Session("diareporte") = d.ToShortDateString
        Response.Redirect("~/RepDiarioV.aspx", False)
    End Sub

    Public Function CrearCita(ByVal mensaje As String) As String

        db.Cn1 = cn1
        db.Cn2 = cn2
        Dim x As String
        Dim y As String
        Dim msj As String = String.Empty
        Dim rp As String()
        Dim rpy As String()
        usuario = Session("usuario").ToString()
        iusuario = Convert.ToInt32(Session("iusuario").ToString())
        Dim fechan As Date = txt_fecha.Text

        Dim fechab As String = fechan.ToString("yyyyMMdd")

        lblref1.Text = fechab

        If btn_agregar.Text = "Agregar" Then
            If txt_fecha.Text.ToString() <> String.Empty And ddl_clinica.SelectedValue.ToString() <> 0 And ddl_jornada.SelectedValue.ToString() <> 0 Then
                'MANGUA
                x = db.InsertarFechaProximaVisita("1", hd_idsignosvitales.Value, fechab, hd_idpaciente.Value, iusuario, usuario)
                rp = x.Split("|")
                'TS
                y = db.InsertarFechaProximaVisitaTS(hd_idpaciente.Value, hd_idsignosvitales.Value, fechab, ddl_jornada.SelectedValue.ToString(), ddl_clinica.SelectedValue.ToString(), usuario, ddl_horario_cita.SelectedValue.ToString())
                rpy = y.Split("|")
                msj = "<span class='error'>" & rp(1).ToString() & "<br />" & rpy(1).ToString() & "</span>"
                limpiardatos()
            Else
                msj = "<span class='error'>Debe ingresar los campos requeridos para GRABAR.</span>"
            End If
        ElseIf btn_agregar.Text = "Modificar" Then
            If txt_fecha.Text.ToString() <> String.Empty And ddl_clinica.SelectedValue.ToString() <> 0 And ddl_jornada.SelectedValue.ToString() <> 0 Then
                'MANGUA
                x = db.InsertarFechaProximaVisita("2", hd_idsignosvitales.Value, fechab, hd_idpaciente.Value, iusuario, usuario)
                rp = x.Split("|")
                'TS
                y = db.ActualizarFechaProximaVisitaTS(hd_idsignosvitales.Value, fechab, ddl_jornada.SelectedValue.ToString(), ddl_clinica.SelectedValue.ToString(), usuario, ddl_horario_cita.SelectedValue.ToString())
                rpy = y.Split("|")
                msj = "<span class='error'>" & rp(1).ToString() & "<br />" & rpy(1).ToString() & "</span>"
                limpiardatos()
            Else
                msj = "<span class='error'>Debe ingresar los campos requeridos para MODIFICAR.</span>"
            End If
        End If
        div_no_citas.Visible = False
        mensaje = msj

        Return mensaje

    End Function


    Protected Sub btn_agregar_Click(sender As Object, e As EventArgs) Handles btn_agregar.Click

        Dim message As String = ""
        CrearCita(message)
        ltl_error.Text = message

    End Sub

    Function msjerror(ByVal mj As String) As String
        Dim x As String
        Select Case mj
            Case "1"
                x = ""

        End Select
        Return x
    End Function

    Protected Sub btn_limpiar_Click(sender As Object, e As EventArgs) Handles btn_limpiar.Click

        limpiardatos()
        txt_asi.Text = String.Empty
        div_Adultos.Visible = True
        div_Pediatria.Visible = False
        llenahorarios1()
    End Sub

    Protected Sub btn_buscar_Click(sender As Object, e As ImageClickEventArgs) Handles btn_buscar.Click
        buscaNHC()
        LlenaCircuito()
        'obtiene nombre día visita
        Obtienediacita()
        If txt_fecha.Text <> String.Empty Then
            ObtieneNoCitasHorarios()
        Else
            ltl_error2.Text = "<span class='error'>" & "Ingrese Fecha Próxima Cita" & "</span>"
        End If
        Dim nhc As String = txt_asi.Text.ToString()
        Dim x As String = db.conteo_dias_CV(nhc, usuario)
        Dim rp As String() = x.Split("|")
        If rp(0).ToString() = "True" Then
            If rp(2).ToString() = 1 Then

                If rp(3).ToString() > 24 Then
                    If rp(3).ToString() < 32 Then
                        MPE_Alerta_CV.Show()
                    End If
                End If

                If rp(3).ToString() > 86 Then
                    If rp(3).ToString() < 94 Then
                        MPE_Alerta_CV.Show()
                    End If
                End If

                If rp(3).ToString() > 176 Then
                    If rp(3).ToString() < 184 Then
                        MPE_Alerta_CV.Show()
                    End If
                End If
            End If

        End If

    End Sub

    Protected Sub txt_asi_TextChanged(sender As Object, e As EventArgs) Handles txt_asi.TextChanged

        buscaNHC()
        Obtienediacita()
        LlenaCircuito()

        'ObtieneNoCitasHorarios()
        If txt_fecha.Text <> String.Empty Then
            ObtieneNoCitasHorarios()
        Else
            ltl_error2.Text = "<span class='error'>" & "Ingrese Fecha Próxima Cita" & "</span>"
        End If



        Dim dia_lbl As String
        dia_lbl = lbl_dia_cita.Text.ToString()

        If dia_lbl <> "viernes" Then

            llenahorarios1()
        ElseIf dia_lbl = "viernes" Then

            llenahorarios2()
        End If
    End Sub

    Sub buscaNHC()
        'Dim nhc As String = txt_asi.Text.ToString()

        If txt_asi.Text.ToUpper().Trim <> String.Empty Then
            limpiardatos()
            llenadatos(txt_asi.Text.ToUpper())
        Else
            limpiardatos()
        End If
        'If nhc.Substring(1, 1).ToUpper.ToString() = "P" Then
        '    limpiardatos()
        '    llenadatosPed(txt_asi.Text.ToUpper())
        'ElseIf txt_asi.Text.ToUpper().Trim <> String.Empty Then
        '    limpiardatos()
        '    llenadatos(txt_asi.Text.ToUpper())
        'Else
        '    limpiardatos()
        'End If
    End Sub

    Sub limpiardatos()

        div_no_citas.Visible = False
        lbl_genero.Text = String.Empty
        lbl_nombre.Text = String.Empty
        lbl_edad.Text = String.Empty
        lbl_telefono.Text = String.Empty
        lbl_direccion.Text = String.Empty
        lbl_visita.Text = String.Empty
        txt_fecha.Text = String.Empty
        lbl_circuito_est.Text = String.Empty
        lbl_circ_grupo.Text = String.Empty
        lbl_circ_periodo.Text = String.Empty
        lbl_dia_cita.Text = String.Empty
        ddl_clinica.SelectedIndex = 0
        ddl_jornada.SelectedIndex = 0
        ddl_horario_cita.Items.Clear()
        ltl_error.Text = String.Empty
        ltl_error2.Text = String.Empty

        txt_asi.Focus()
        btn_agregar.Text = "Agregar"
        btn_agregar.Visible = False
    End Sub

    Sub llenadatos(ByVal nhc As String)
        'llenahorarios1()
        usuario = Session("usuario").ToString()
        db.Cn2 = cn2
        Try
            Dim x As String = db.ObtieneBasales1(nhc, usuario)
            Dim rp As String() = x.Split("|")
            If rp(0).ToString() = "True" Then
                Session("nhc") = nhc
                lbl_nombre.Text = String.Empty
                lbl_genero.Text = String.Empty
                lbl_edad.Text = String.Empty
                lbl_telefono.Text = String.Empty
                lbl_direccion.Text = String.Empty
                hd_idpaciente.Value = String.Empty
                'asignacion
                lbl_nombre.Text = rp(2).ToString()
                lbl_genero.Text = rp(1).ToString()
                lbl_edad.Text = rp(5).ToString()
                lbl_telefono.Text = rp(3).ToString()
                lbl_direccion.Text = rp(6).ToString()
                hd_idpaciente.Value = rp(8).ToString()
                lbl_tyt_pac.Text = rp(9).ToString()
                ltl_error.Text = String.Empty
            Else
                ltl_error.Text = "<span class='error'>" & rp(1) & "</span>"
            End If
            'visita
            Dim y As String = db.ObtieneFechaVisita(hd_idpaciente.Value, usuario)
            Dim rpy As String() = y.Split("|")
            If rpy(0).ToString() = "True" Then
                lbl_visita.Text = String.Empty
                txt_fecha.Text = String.Empty
                'asignacion
                hd_idsignosvitales.Value = rpy(1).ToString()
                lbl_visita.Text = rpy(2).ToString()
                txt_fecha.Text = rpy(3).ToString()
                ltl_error.Text = String.Empty
            Else
                ltl_error.Text = "<span class='error'>" & rpy(1) & "</span>"
            End If
            'visitaTS
            Dim z As String = db.ObtieneFechaVisitaTS(hd_idsignosvitales.Value, usuario)
            Dim rpz As String() = z.Split("|")
            If rpz(0).ToString() = "True" Then
                'llenahorarios1()
                ddl_jornada.SelectedIndex = 0
                ddl_clinica.SelectedIndex = 0
                'ddl_horario_cita.SelectedIndex = 0
                'asignacion
                hd_idcitas.Value = rpz(1).ToString()
                ddl_jornada.SelectedValue = rpz(3).ToString()
                ddl_clinica.SelectedValue = rpz(4).ToString()
                'ddl_horario_cita.SelectedValue = rpz(5).ToString()
                ltl_error.Text = String.Empty
            Else
                ltl_error.Text = "<span class='error'>" & rpz(1) & "</span>"
            End If
            Dim existe As Integer
            existe = Existe_ID(rpy(1).ToString())
            If existe <> 0 Then
                existeID = True
                btn_agregar.Text = "Modificar"
            ElseIf existe = 0 Then
                existeID = False
                btn_agregar.Text = "Agregar"
            End If
        Catch ex As Exception
            errores = (usuario & "|consulta_paciente.calendario()|" & ex.ToString() & "|") + ex.Message
            db.GrabarErrores(errores)

        End Try
    End Sub


    Sub llenadatosPed(ByVal nhc As String)
        usuario = Session("usuario").ToString()
        db.Cn1 = cn1
        Dim x As String = db.ObtieneBasalesPed(nhc, usuario)
        Dim rp As String() = x.Split("|")
        If rp(0).ToString() = "True" Then
            Session("nhc") = nhc
            lbl_nombre.Text = String.Empty
            lbl_genero.Text = String.Empty
            lbl_edad.Text = String.Empty
            lbl_telefono.Text = String.Empty
            lbl_direccion.Text = String.Empty
            hd_idpaciente.Value = String.Empty
            'asignacion
            lbl_nombre.Text = rp(2).ToString()
            lbl_genero.Text = rp(3).ToString()
            lbl_edad.Text = rp(4).ToString()
            lbl_telefono.Text = rp(5).ToString()
            lbl_direccion.Text = rp(6).ToString()
            hd_idpaciente.Value = rp(1).ToString()
            ltl_error.Text = String.Empty
        Else
            ltl_error.Text = "<span class='error'>" & rp(1) & "</span>"
        End If
        'visita
        'Dim y As String = db.ObtieneFechaVisita(hd_idpaciente.Value, usuario)
        'Dim rpy As String() = y.Split("|")
        'If rpy(0).ToString() = "True" Then
        '    lbl_visita.Text = String.Empty
        '    txt_fecha.Text = String.Empty
        '    'asignacion
        '    hd_idsignosvitales.Value = rpy(1).ToString()
        '    lbl_visita.Text = rpy(2).ToString()
        '    txt_fecha.Text = rpy(3).ToString()
        '    ltl_error.Text = String.Empty
        'Else
        '    ltl_error.Text = "<span class='error'>" & rpy(1) & "</span>"
        'End If
        ''visitaTS
        'Dim z As String = db.ObtieneFechaVisitaTS(hd_idsignosvitales.Value, usuario)
        'Dim rpz As String() = z.Split("|")
        'If rpz(0).ToString() = "True" Then
        '    ddl_jornada.SelectedIndex = 0
        '    ddl_clinica.SelectedIndex = 0
        '    'asignacion
        '    hd_idcitas.Value = rpz(1).ToString()
        '    ddl_jornada.SelectedValue = rpz(3).ToString()
        '    ddl_clinica.SelectedValue = rpz(4).ToString()
        '    ltl_error.Text = String.Empty
        'Else
        '    ltl_error.Text = "<span class='error'>" & rpz(1) & "</span>"
        'End If
        'Dim existe As Integer
        'existe = Existe_ID(rpy(1).ToString())
        'If existe <> 0 Then
        '    existeID = True
        '    btn_agregar.Text = "Modificar"
        'ElseIf existe = 0 Then
        '    existeID = False
        '    btn_agregar.Text = "Agregar"
        'End If
    End Sub

    Function Existe_ID(ByVal id As String) As Integer
        db.Cn1 = cn1
        Return db.ExisteFechaProximaVisitaTS(id, usuario)
    End Function


    Function diaSemana(ByVal fecha As String) As String
        Dim A As Integer = Convert.ToInt32(fecha.Substring(6, 4))
        Dim M As Integer = Convert.ToInt32(fecha.Substring(3, 2))
        Dim D As Integer = Convert.ToInt32(fecha.Substring(0, 2))
        Dim diaFecha As New DateTime(A, M, D)
        Return diaFecha.ToString("dddd", New CultureInfo("es-ES"))

    End Function


    Sub Obtienediacita()
        If txt_fecha.Text <> String.Empty Then
            Dim dia As String
            dia = diaSemana(txt_fecha.Text)
            lbl_dia_cita.Text = dia.ToString()
        Else
            ltl_error2.Text = "<span class='error'>" & "Ingrese Fecha Próxima Cita" & "</span>"
        End If

    End Sub


    Sub ObtieneNoCitasHorarios()
        div_no_citas.Visible = True
        db.Cn1 = cn1

        lbl_B_1_citas.Visible = True
        lbl_B_1.Visible = True
        lbl_B_1_citas_status.Visible = True
        lbl_B_1_citas_hosp.Visible = True
        lbl_B_1_citas_status_hosp.Visible = True


        lbl_B_2_citas.Visible = True
        lbl_B_2.Visible = True
        lbl_B_2_citas_status.Visible = True
        lbl_B_2_citas_hosp.Visible = True
        lbl_B_2_citas_status.Visible = True


        lbl_B_3_citas.Visible = True
        lbl_B_3.Visible = True
        lbl_B_3_citas_status.Visible = True
        lbl_B_3_citas_hosp.Visible = True
        lbl_B_3_citas_status_hosp.Visible = True


        lbl_B_4_citas.Visible = True
        lbl_B_4.Visible = True
        lbl_B_4_citas_status.Visible = True
        lbl_B_4_citas_hosp.Visible = True
        lbl_B_4_citas_status_hosp.Visible = True


        lbl_B_5_citas.Visible = True
        lbl_B_5.Visible = True
        lbl_B_5_citas_status.Visible = True
        lbl_B_5_citas_hosp.Visible = True
        lbl_B_5_citas_status_hosp.Visible = True


        lbl_B_6_citas.Visible = True
        lbl_B_6.Visible = True
        lbl_B_6_citas_status.Visible = True

        lbl_B_7_citas.Visible = True
        lbl_B_7.Visible = True
        lbl_B_7_citas_status.Visible = True

        lbl_B_8_citas.Visible = True
        lbl_B_8.Visible = True
        lbl_B_8_citas_status.Visible = True

        lbl_B_9_citas.Visible = True
        lbl_B_9.Visible = True
        lbl_B_9_citas_status.Visible = True


        Dim clinica As String
        Dim hospital As String
        clinica = "1"
        hospital = "2"

        Dim dia As String = txt_fecha.Text.ToString()
        lbl_dia_horario.Text = dia

        Dim h1 As String = db.R_no_citas_horario1(dia, clinica, usuario)
        lbl_B_1_citas.Text = h1.ToString()

        If h1 >= 12 Then
            lbl_B_1_citas_status.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_1_citas_status.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        Dim h2 As String = db.R_no_citas_horario2(dia, clinica, usuario)
        lbl_B_2_citas.Text = h2.ToString()
        If h2 >= 14 Then
            lbl_B_2_citas_status.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_2_citas_status.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        Dim h3 As String = db.R_no_citas_horario3(dia, clinica, usuario)
        lbl_B_3_citas.Text = h3.ToString()
        If h3 >= 13 Then
            lbl_B_3_citas_status.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_3_citas_status.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        Dim h4 As String = db.R_no_citas_horario4(dia, clinica, usuario)
        lbl_B_4_citas.Text = h4.ToString()
        If h4 >= 13 Then
            lbl_B_4_citas_status.Text = "<span class='error_hc'> Completo </span>"
        ElseIf lbl_dia_cita.Text = "viernes" Then
            'lbl_B_4_citas_status.Text = "<span class='error_hc'>No Disponible </span>"
            lbl_B_4_citas.Visible = False
            lbl_B_4.Visible = False
            lbl_B_4_citas_status.Visible = False
        Else
            lbl_B_4_citas_status.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        Dim h5 As String = db.R_no_citas_horario5(dia, clinica, usuario)
        lbl_B_5_citas.Text = h5.ToString()
        If h5 >= 13 Then
            lbl_B_5_citas_status.Text = "<span class='error_hc'> Completo </span>"
        ElseIf lbl_dia_cita.Text = "viernes" Then
            'lbl_B_5_citas_status.Text = "<span class='error_hc'>No Disponible </span>"
            lbl_B_5_citas.Visible = False
            lbl_B_5.Visible = False
            lbl_B_5_citas_status.Visible = False
        Else
            lbl_B_5_citas_status.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        lbl_B_6.Visible = False
        lbl_B_6_citas.Visible = False
        lbl_B_6_citas_status.Visible = False

        Dim h7 As String = db.R_no_citas_horario7(dia, clinica, usuario)
        lbl_B_7_citas.Text = h7.ToString()
        If h7 >= 15 Then
            lbl_B_7_citas_status.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_7_citas_status.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        Dim h8 As String = db.R_no_citas_horario8(dia, clinica, usuario)
        lbl_B_8_citas.Text = h8.ToString()
        If h8 >= 10 Then
            lbl_B_8_citas_status.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_8_citas_status.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        Dim h9 As String = db.R_no_citas_horario9(dia, clinica, usuario)
        lbl_B_9_citas.Text = h9.ToString()
        If h9 >= 30 Then
            lbl_B_9_citas_status.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_9_citas_status.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        'CITAS  1AVE HOSPITAL


        Dim h1_hosp As String = db.R_no_citas_horario_hosp(1, dia, hospital, usuario)
        lbl_B_1_citas_hosp.Text = h1_hosp.ToString()
        If h1_hosp >= 1 Then
            lbl_B_1_citas_status_hosp.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_1_citas_status_hosp.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        Dim h2_hosp As String = db.R_no_citas_horario_hosp(2, dia, hospital, usuario)
        lbl_B_2_citas_hosp.Text = h2_hosp.ToString()
        If h2_hosp >= 1 Then
            lbl_B_2_citas_status_hosp.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_2_citas_status_hosp.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        Dim h3_hosp As String = db.R_no_citas_horario_hosp(3, dia, hospital, usuario)
        lbl_B_3_citas_hosp.Text = h3_hosp.ToString()
        If h3_hosp >= 1 Then
            lbl_B_3_citas_status_hosp.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_3_citas_status_hosp.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        Dim h4_hosp As String = db.R_no_citas_horario_hosp(4, dia, hospital, usuario)
        lbl_B_4_citas_hosp.Text = h4_hosp.ToString()
        If h4_hosp >= 1 Then
            lbl_B_4_citas_status_hosp.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_4_citas_status_hosp.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If

        Dim h5_hosp As String = db.R_no_citas_horario_hosp(5, dia, hospital, usuario)
        lbl_B_5_citas_hosp.Text = h5_hosp.ToString()
        If h5_hosp >= 1 Then
            lbl_B_5_citas_status_hosp.Text = "<span class='error_hc'> Completo </span>"
        Else
            lbl_B_5_citas_status_hosp.Text = "<span class='status_BH_disponible'> Disponible </span>"
        End If



    End Sub



    Protected Sub txt_fecha_TextChanged(sender As Object, e As EventArgs) Handles txt_fecha.TextChanged
        Obtienediacita()
        ObtieneNoCitasHorarios()

        Dim dia_lbl As String
        dia_lbl = lbl_dia_cita.Text.ToString()

        If dia_lbl <> "viernes" Then

            llenahorarios1()
        ElseIf dia_lbl = "viernes" Then

            llenahorarios2()
        End If

    End Sub


    Protected Sub ib_check_horario_Click(sender As Object, e As ImageClickEventArgs) Handles ib_check_horario.Click
        Dim pox_cita As String
        Dim horario As String
        Dim clinica As String
        Dim d As String


        pox_cita = txt_fecha.Text.ToString()
        horario = ddl_horario_cita.SelectedValue.ToString()
        clinica = ddl_clinica.SelectedValue.ToString()
        db.Cn1 = cn1
        d = db.Revisa_horarios_disponibles(pox_cita, horario, clinica, usuario)

        ltl_error2.Text = String.Empty

        If clinica = 1 Then

            Select Case horario
                Case 1
                    If d > 12 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"
                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If

                Case 2
                    If d > 14 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"

                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If
                Case 3
                    If d > 13 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"

                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If
                Case 4
                    If d > 13 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"

                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If
                Case 5
                    If d > 13 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"

                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If

                Case 7
                    If d > 15 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"

                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If
                Case 8
                    If d > 10 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"

                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If
                Case 9
                    If d > 20 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"

                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If
            End Select
        Else
            Select Case horario
                Case 1
                    If d >= 1 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"
                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If

                Case 2
                    If d >= 1 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"
                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If

                Case 3
                    If d >= 1 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"
                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If

                Case 4
                    If d >= 1 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"
                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If

                Case 5
                    If d >= 1 Then
                        ltl_error.Text = "<span class='error_hc'> Horario lleno, Reasigne </span>"
                    Else
                        ltl_error.Text = "<span class='error_hc1'> Horario Disponible </span>"
                        btn_agregar.Visible = True
                    End If
            End Select
        End If
    End Sub

    Protected Sub ddl_horario_cita_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddl_horario_cita.SelectedIndexChanged
        btn_agregar.Visible = False
    End Sub

    Protected Sub ib_ad_Click(sender As Object, e As ImageClickEventArgs) Handles ib_ad.Click
        limpiardatos()
        txt_asi.Text = String.Empty
        div_Adultos.Visible = True
        div_Pediatria.Visible = False
        llenahorarios1()

    End Sub

    Protected Sub ib_ped_Click(sender As Object, e As ImageClickEventArgs) Handles ib_ped.Click
        div_Pediatria.Visible = True
        div_Adultos.Visible = False
        limpiardatos()

    End Sub

    Protected Sub btn_search_Click(sender As Object, e As EventArgs) Handles btn_search.Click

        buscaNHC()
        Obtienediacita()
        LlenaCircuito()
        If txt_fecha.Text <> String.Empty Then
            ObtieneNoCitasHorarios()
        Else
            ltl_error2.Text = "<span class='error'>" & "Ingrese Fecha Próxima Cita" & "</span>"
        End If

        Dim dia_lbl As String
        dia_lbl = lbl_dia_cita.Text.ToString()

        If dia_lbl <> "viernes" Then

            llenahorarios1()
        ElseIf dia_lbl = "viernes" Then

            llenahorarios2()
        End If

    End Sub

    Protected Sub btn_grabarcita_Click(sender As Object, e As EventArgs) Handles btn_grabarcita.Click

        Dim message As String = ""
        CrearCita(message)
        ltl_error.Text = message

    End Sub

    Private Sub llenaPerTel1()
        Dim ipais As Integer

        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_DIRECCION(5, usuario, ipais)
        ddl_pertenecetel1.DataSource = datos
        ddl_pertenecetel1.DataTextField = "PerteneceTdescripcion"
        ddl_pertenecetel1.DataValueField = "PerteneceTAbrevia"
        ddl_pertenecetel1.DataBind()
        ddl_pertenecetel1.Items.Insert(0, New ListItem("Seleccione...", "0"))
    End Sub

    Private Sub llenaPerTel2()
        Dim ipais As Integer

        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_DIRECCION(5, usuario, ipais)
        ddl_pertenecetel2.DataSource = datos
        ddl_pertenecetel2.DataTextField = "PerteneceTdescripcion"
        ddl_pertenecetel2.DataValueField = "PerteneceTAbrevia"
        ddl_pertenecetel2.DataBind()
        ddl_pertenecetel2.Items.Insert(0, New ListItem("Seleccione...", "0"))
    End Sub

    Private Sub llenaPerTel3()
        Dim ipais As Integer

        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_DIRECCION(5, usuario, ipais)
        ddl_pertenecetel3.DataSource = datos
        ddl_pertenecetel3.DataTextField = "PerteneceTdescripcion"
        ddl_pertenecetel3.DataValueField = "PerteneceTAbrevia"
        ddl_pertenecetel3.DataBind()
        ddl_pertenecetel3.Items.Insert(0, New ListItem("Seleccione...", "0"))
    End Sub

    Private Sub llenaPerTel4()
        Dim ipais As Integer

        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_DIRECCION(5, usuario, ipais)
        ddl_pertenecetel4.DataSource = datos
        ddl_pertenecetel4.DataTextField = "PerteneceTdescripcion"
        ddl_pertenecetel4.DataValueField = "PerteneceTAbrevia"
        ddl_pertenecetel4.DataBind()
        ddl_pertenecetel4.Items.Insert(0, New ListItem("Seleccione...", "0"))
    End Sub

    Private Sub llenapais()
        Dim ipais As Integer

        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_DIRECCION(1, usuario, ipais)
        ListaPaises.DataSource = datos
        ListaPaises.DataTextField = "paisdescripcion"
        ListaPaises.DataValueField = "idpais"
        ListaPaises.DataBind()
        ListaPaises.Items.Insert(0, New ListItem("Seleccione...", "0"))
    End Sub

    Private Sub llenaDeptos(ByVal ipais As Integer)
        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_DIRECCION(2, usuario, ipais)
        ddl_ListaDepartamentos.DataSource = datos
        ddl_ListaDepartamentos.DataTextField = "DepartamentoDescripcion"
        ddl_ListaDepartamentos.DataValueField = "IdDepartamento"
        ddl_ListaDepartamentos.DataBind()
        ddl_ListaDepartamentos.Items.Insert(0, New ListItem("Seleccione...", "0"))
    End Sub

    Private Sub llenaMunicipios(ByVal idepto As Integer)
        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_DIRECCION(3, usuario, idepto)
        ddl_ListaMunicipios.DataSource = datos
        ddl_ListaMunicipios.DataTextField = "MunicipioDescripcion"
        ddl_ListaMunicipios.DataValueField = "IdMunicipio"
        ddl_ListaMunicipios.DataBind()
        ddl_ListaMunicipios.Items.Insert(0, New ListItem("Seleccione...", "0"))
    End Sub

    Private Sub llenaZonas(ByVal imupio As Integer)
        db.Cn1 = cn1
        Dim datos As DataTable = db.CATALOGOS_DIRECCION(4, usuario, imupio)
        ddl_ListaZonas.DataSource = datos
        ddl_ListaZonas.DataTextField = "ZonaDescripcion"
        ddl_ListaZonas.DataValueField = "IdZona"
        ddl_ListaZonas.DataBind()
        ddl_ListaZonas.Items.Insert(0, New ListItem("Seleccione...", "0"))
    End Sub

    Protected Sub ListaPaises_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListaPaises.SelectedIndexChanged
        ddl_ListaDepartamentos.ClearSelection()
        ddl_ListaMunicipios.ClearSelection()
        ddl_ListaZonas.ClearSelection()
        llenaDeptos(ListaPaises.SelectedValue)
    End Sub

    Protected Sub ddl_ListaDepartamentos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddl_ListaDepartamentos.SelectedIndexChanged
        ddl_ListaMunicipios.ClearSelection()
        ddl_ListaZonas.ClearSelection()
        llenaMunicipios(ddl_ListaDepartamentos.SelectedValue)

    End Sub

    Protected Sub ddl_ListaMunicipios_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddl_ListaMunicipios.SelectedIndexChanged
        ddl_ListaZonas.ClearSelection()
        llenaZonas(ddl_ListaMunicipios.SelectedValue)
    End Sub

    Function AsignaDatosPacientesTS(ByVal DTT As String) As Object
        Dim PacientesTS As New PacientesTS
        Dim Con_tx1 As String = String.Empty
        Dim Con_tx2 As String = String.Empty
        Dim Con_tx3 As String = String.Empty
        Dim Con_tx4 As String = String.Empty

        If DTT = "TOD" Then
            PacientesTS.IdPacTS = hd_idpaciente.Value
            PacientesTS.NhcTS = txt_asi.Text
            PacientesTS.IdPaisTS = ListaPaises.SelectedValue
            PacientesTS.IdDeptoTS = ddl_ListaDepartamentos.SelectedValue
            PacientesTS.IdMupioTS = ddl_ListaMunicipios.SelectedValue
            PacientesTS.IdZonaTS = ddl_ListaZonas.SelectedValue
            PacientesTS.DireccionTS = txtdireccion.Text
            PacientesTS.Tel1 = txttelefono1.Text
            PacientesTS.Tel2 = txttelefono2.Text
            PacientesTS.Tel3 = txttelefono3.Text
            PacientesTS.Tel4 = txttelefono4.Text
            PacientesTS.PerTel1 = ddl_pertenecetel1.SelectedValue
            PacientesTS.PerTel2 = ddl_pertenecetel2.SelectedValue
            PacientesTS.PerTel3 = ddl_pertenecetel3.SelectedValue
            PacientesTS.PerTel4 = ddl_pertenecetel4.SelectedValue
            PacientesTS.ConTx1 = If(chbconocetx1.Checked, "SI", "NO")
            PacientesTS.ConTx2 = If(chbconocetx2.Checked, "SI", "NO")
            PacientesTS.ConTx3 = If(chbconocetx3.Checked, "SI", "NO")
            PacientesTS.ConTx4 = If(chbconocetx4.Checked, "SI", "NO")
            PacientesTS.Usua = Convert.ToInt32(Session("iusuario").ToString())
        ElseIf DTT = "TEL" Then
            PacientesTS.IdPacTS = hd_idpaciente.Value
            PacientesTS.NhcTS = txt_asi.Text
            PacientesTS.IdPaisTS = 0
            PacientesTS.IdDeptoTS = 0
            PacientesTS.IdMupioTS = 0
            PacientesTS.IdZonaTS = 0
            PacientesTS.DireccionTS = ""
            PacientesTS.Tel1 = txttelefono1.Text
            PacientesTS.Tel2 = txttelefono2.Text
            PacientesTS.Tel3 = txttelefono3.Text
            PacientesTS.Tel4 = txttelefono4.Text
            PacientesTS.PerTel1 = ddl_pertenecetel1.SelectedValue
            PacientesTS.PerTel2 = ddl_pertenecetel2.SelectedValue
            PacientesTS.PerTel3 = ddl_pertenecetel3.SelectedValue
            PacientesTS.PerTel4 = ddl_pertenecetel4.SelectedValue
            PacientesTS.ConTx1 = If(chbconocetx1.Checked, "SI", "NO")
            PacientesTS.ConTx2 = If(chbconocetx2.Checked, "SI", "NO")
            PacientesTS.ConTx3 = If(chbconocetx3.Checked, "SI", "NO")
            PacientesTS.ConTx4 = If(chbconocetx4.Checked, "SI", "NO")
            PacientesTS.Usua = Convert.ToInt32(Session("iusuario").ToString())
        ElseIf DTT = "DIR" Then
            PacientesTS.IdPacTS = hd_idpaciente.Value
            PacientesTS.NhcTS = txt_asi.Text
            PacientesTS.IdPaisTS = ListaPaises.SelectedValue
            PacientesTS.IdDeptoTS = ddl_ListaDepartamentos.SelectedValue
            PacientesTS.IdMupioTS = ddl_ListaMunicipios.SelectedValue
            PacientesTS.IdZonaTS = ddl_ListaZonas.SelectedValue
            PacientesTS.DireccionTS = txtdireccion.Text
            PacientesTS.Tel1 = txttelefono1.Text
            PacientesTS.Tel2 = txttelefono2.Text
            PacientesTS.Tel3 = txttelefono3.Text
            PacientesTS.Tel4 = txttelefono4.Text
            PacientesTS.PerTel1 = ddl_pertenecetel1.SelectedValue
            PacientesTS.PerTel2 = ddl_pertenecetel2.SelectedValue
            PacientesTS.PerTel3 = ddl_pertenecetel3.SelectedValue
            PacientesTS.PerTel4 = ddl_pertenecetel4.SelectedValue
            PacientesTS.ConTx1 = If(chbconocetx1.Checked, "SI", "NO")
            PacientesTS.ConTx2 = If(chbconocetx2.Checked, "SI", "NO")
            PacientesTS.ConTx3 = If(chbconocetx3.Checked, "SI", "NO")
            PacientesTS.ConTx4 = If(chbconocetx4.Checked, "SI", "NO")
            PacientesTS.Usua = Convert.ToInt32(Session("iusuario").ToString())
        End If

        Return PacientesTS

    End Function

    Function LimpiaDatosPacientesTS(ByVal Tipo As String) As String
        Dim devolucionDTT As String = String.Empty

        If Tipo = "TOD" Then
            ListaPaises.ClearSelection()
            ddl_ListaDepartamentos.ClearSelection()
            ddl_ListaMunicipios.ClearSelection()
            ddl_ListaZonas.ClearSelection()
            txtdireccion.Text = ""
            txttelefono1.Text = ""
            txttelefono2.Text = ""
            txttelefono3.Text = ""
            txttelefono4.Text = ""
            ddl_pertenecetel1.ClearSelection()
            ddl_pertenecetel2.ClearSelection()
            ddl_pertenecetel3.ClearSelection()
            ddl_pertenecetel4.ClearSelection()
            chbconocetx1.Checked = False
            chbconocetx2.Checked = False
            chbconocetx3.Checked = False
            chbconocetx4.Checked = False

            devolucionDTT = "Se limpio formulaio de Direcciones y telefonos"
        End If
        If Tipo = "DIR" Then
            ListaPaises.ClearSelection()
            ddl_ListaDepartamentos.ClearSelection()
            ddl_ListaMunicipios.ClearSelection()
            ddl_ListaZonas.ClearSelection()
            txtdireccion.Text = ""

            devolucionDTT = "Se limpio formulaio de Direcciones"
        End If
        If Tipo = "TEL" Then
            txttelefono1.Text = ""
            txttelefono2.Text = ""
            txttelefono3.Text = ""
            txttelefono4.Text = ""
            ddl_pertenecetel1.ClearSelection()
            ddl_pertenecetel2.ClearSelection()
            ddl_pertenecetel3.ClearSelection()
            ddl_pertenecetel4.ClearSelection()
            chbconocetx1.Checked = False
            chbconocetx2.Checked = False
            chbconocetx3.Checked = False
            chbconocetx4.Checked = False

            devolucionDTT = "Se limpio formulaio de telefonos"
        End If

        Return devolucionDTT

    End Function

    Protected Sub Btn_DirGuardar_Click(sender As Object, e As EventArgs) Handles Btn_DirGuardar.Click
        'Guarda la direccion
        db.GuardaDireccion(AsignaDatosPacientesTS("DIR"), "DIR")

        Dim resultadoD As String = db.ResultadoG

        ltl_error2.Text = "<span class='error'>" & resultadoD & "</span>"

        LimpiaDatosPacientesTS("DIR")

    End Sub

    Protected Sub Btn_GuardarTel_Click(sender As Object, e As EventArgs) Handles Btn_GuardarTel.Click
        'Guarda los telefonos
        db.GuardaDireccion(AsignaDatosPacientesTS("TEL"), "TEL")

        Dim resultadoD As String = db.ResultadoG

        ltl_error2.Text = "<span class='error'>" & resultadoD & "</span>"

        LimpiaDatosPacientesTS("TEL")
    End Sub
    Protected Sub Btn_GuardarTodo_Click(sender As Object, e As EventArgs) Handles Btn_GuardarTodo.Click
        'Guarda los telefonos
        db.GuardaDireccion(AsignaDatosPacientesTS("TOD"), "TOD")

        Dim resultadoD As String = db.ResultadoG

        ltl_error2.Text = "<span class='error'>" & resultadoD & "</span>"

        ErrorMessage(Me, resultadoD, "Resultado")

        LimpiaDatosPacientesTS("TOD")
    End Sub
    Protected Sub BtnActDir_Click(sender As Object, e As EventArgs) Handles BtnActDir.Click
        LimpiaDatosPacientesTS("DIR")
        divdir.Visible = True
        BtnsGuardaDir.Visible = True
        divtels.Visible = False
        BtnsGuardarTels.Visible = False
    End Sub
    Protected Sub BtnActTel_Click(sender As Object, e As EventArgs) Handles BtnActTel.Click
        LimpiaDatosPacientesTS("TEL")
        divtels.Visible = True
        BtnsGuardarTels.Visible = True
        divdir.Visible = False
        BtnsGuardaDir.Visible = False
    End Sub
    Protected Sub BtnActTodo_Click(sender As Object, e As EventArgs) Handles BtnActTodo.Click
        LimpiaDatosPacientesTS("TOD")
        divdir.Visible = True
        divtels.Visible = True
        BtnsGuardarTodo.Visible = True
        BtnsGuardaDir.Visible = False
        BtnsGuardarTels.Visible = False
    End Sub

    Public Sub ErrorMessage(ByVal Control As UI.Control, ByVal Message As String, Optional ByVal Title As String = "Alert", Optional ByVal callback As String = "")
        Try
            ScriptManager.RegisterStartupScript(Control, Control.GetType, "Script", "swal('" + Title + "','" + Message + "','message');", True)
        Catch ex As Exception
        End Try
    End Sub
End Class
