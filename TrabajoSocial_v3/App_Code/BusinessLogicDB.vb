Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.IO
Imports System.Data
Imports System.Data.SqlTypes
Imports PacientesTS

Public Class BusinessLogicDB
    Private _cn1 As String
    Private _cn2 As String
    Private _cn3 As String
    Private _error As String
    Private _page As String = ""
    Private _pageO As String = ""
    Private Const TimeoutDB As Integer = 600
    Public ResultadoG As String
    Public ResultadoG2 As String

    Public Property PageName() As String
        Get
            Return _page
        End Get
        Set(ByVal value As String)
            _page = value
        End Set
    End Property

    Public ReadOnly Property DB_Error() As String
        Get
            Return _error
        End Get
    End Property

    Public Property Cn1() As String
        Get
            Return _cn1
        End Get
        Set(ByVal value As String)
            _cn1 = value
        End Set
    End Property

    Public Property Cn2() As String
        Get
            Return _cn2
        End Get
        Set(ByVal value As String)
            _cn2 = value
        End Set
    End Property

    Public Property Cn3() As String
        Get
            Return _cn3
        End Get
        Set(ByVal value As String)
            _cn3 = value
        End Set
    End Property

    '*Login, cifrador y decifrador*//

    Public Function LoginValidation(ByVal UserName As String, ByVal Password As String, ByVal app As String) As String
        _page = "LoginValidation"
        Dim x As String = ""
        Dim v As Boolean = False
        Dim idperfil As String = ""
        Dim edicion As String = ""
        Dim claveE As String = Encriptar(Password, "Acuario") 'EncString(Password)
        Try
            Using connection1 As New SqlConnection(_cn3)
                connection1.Open()
                Dim strSQL As String = String.Format("SELECT NomUsuario, IdBDAcceso, IdPerfil, Edicion, Estatus FROM USUARIO WHERE NomUsuario = '{0}'", UserName)
                Dim command As New SqlCommand(strSQL, connection1)
                Dim Dr As SqlDataReader = Nothing
                Dr = command.ExecuteReader()
                If Dr.HasRows Then
                    While Dr.Read()
                        If Dr("IdBDAcceso").ToString() = app Then
                            If Dr("Estatus").ToString() = "A" Then
                                v = True
                                idperfil = Dr("IdPerfil").ToString()
                                edicion = Dr("Edicion").ToString()
                                Exit While
                            Else
                                x = "False|Usuario Desactivado."
                                v = False
                            End If
                        Else
                            x = "False|Usuario NO autorizado."
                            v = False
                        End If
                    End While
                Else
                    x = "False|Usuario NO autorizado."
                End If
                Dr.Close()
                connection1.Close()
            End Using
            If v = True Then
                Using connection As New SqlConnection(_cn2)
                    connection.Open()
                    Dim strSQL As String = String.Format("SELECT IdUsuario, NomUsuario, Clave FROM USUARIO WHERE NomUsuario = '{0}'", UserName)
                    Dim command As New SqlCommand(strSQL, connection)
                    Dim Dr As SqlDataReader = Nothing
                    Dr = command.ExecuteReader()
                    If Dr.HasRows Then
                        While Dr.Read()
                            If claveE = Dr("Clave").ToString() Then
                                x = "True|" & Dr("IdUsuario").ToString() & "|" & Dr("NomUsuario").ToString() & "|" & idperfil & "|" & edicion
                                Exit While
                            Else
                                x = "False|Usuario o Clave Incorrectos, favor intente de Nuevo."
                            End If
                        End While
                    Else
                        x = "False|Usuario o Clave Incorrectos, favor intente de Nuevo."
                    End If
                    Dr.Close()
                End Using
            End If
            Return x
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(UserName & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            x = "False|Hubo un Error al Iniciar Sesion, intente de nuevo."
            Return x
        End Try
    End Function

    Public Function ConvertirClave(ByVal sOriginal As String, ByVal sClave As String, ByVal vAccion As Boolean) As String
        Dim x As String = ""
        Dim LenOri As Long
        Dim LenClave As Long
        Dim i As Long
        Dim j As Long
        Dim cO As Long
        Dim cC As Long
        Dim k As Long
        Dim v As String

        LenOri = Len(sOriginal)
        LenClave = Len(sClave)

        v = Space(LenOri)
        i = 0
        For j = 1 To LenOri
            i = i + 1
            If i > LenClave Then
                i = 1
            End If
            cO = Asc(Mid(sOriginal, j, 1))
            cC = Asc(Mid(sClave, i, 1))
            If vAccion Then
                k = cO + cC
                If k > 255 Then
                    k = k - 255
                End If
            Else
                k = cO - cC
                If k < 0 Then
                    k = k + 255
                End If
            End If
            Mid(v, j, 1) = Chr(k)
        Next
        ConvertirClave = v
    End Function

    Public Function DesEncriptar(ByVal sOriginal As String, ByVal sClave As String) As String
        DesEncriptar = ConvertirClave(sOriginal, sClave, False)
    End Function

    Public Function Encriptar(ByVal sOriginal As String, ByVal sClave As String) As String
        Encriptar = ConvertirClave(sOriginal, sClave, True)
    End Function

    Public Sub GrabaSesion(ByVal id As String, ByVal tipo As String, ByVal ip As String, ByVal usuario As String)
        _page = "db.GrabaSesion"
        Dim sql As String = String.Format("INSERT INTO UActividad (IdUsuario, Tipo, IP) VALUES({0}, '{1}', '{2}')", id, tipo, ip)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Function Desconectar(ByVal id As String, ByVal ip As String, ByVal usuario As String) As Boolean
        _page = "db.Desconectar"
        Dim sql As String = String.Format("INSERT INTO UActividad (IdUsuario, Tipo, IP) VALUES({0}, '{1}', '{2}')", id, "O", ip)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return True
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return False
        End Try
    End Function

    '*Citas*//

    Public Function ConteoCitas(ByVal m As String, ByVal y As String, ByVal usuario As String) As DataTable
        _page = "db.ConteoCitas"
        Dim Query As String = String.Empty
        'Query = "SELECT CONVERT(VARCHAR,FechaProximaVisita,103) AS 'Fecha', COUNT(DISTINCT IdPaciente) AS 'Conteo' "
        'Query += String.Format("FROM SIGNOSVITALES WHERE Month(FechaProximaVisita) = {0} And Year(FechaProximaVisita) = {1} ", m, y)
        'Query += "GROUP BY FechaProximaVisita "
        Query = "SELECT CONVERT(VARCHAR,S.FechaProximaVisita,103) AS 'Fecha', COUNT(DISTINCT S.IdPaciente) AS 'Conteo', "
        Query += String.Format("(SELECT COUNT(DISTINCT C.IdSignosVitales) FROM dbo.PAC_CITAS AS C WHERE Month(C.FechaProximaVisita) = {0} And Year(C.FechaProximaVisita) = {1} AND C.FechaProximaVisita = S.FechaProximaVisita AND C.Clinica IN (0,1)) AS 'Cli2',  ", m, y)
        Query += String.Format("(SELECT COUNT(DISTINCT C.IdSignosVitales) FROM dbo.PAC_CITAS AS C WHERE Month(C.FechaProximaVisita) = {0} And Year(C.FechaProximaVisita) = {1} AND C.FechaProximaVisita = S.FechaProximaVisita AND C.Clinica = 2) AS 'Cli1', ", m, y)
        Query += String.Format("(SELECT COUNT(DISTINCT C.IdSignosVitales) FROM dbo.PAC_CITAS AS C WHERE Month(C.FechaProximaVisita) = {0} And Year(C.FechaProximaVisita) = {1} AND C.FechaProximaVisita = S.FechaProximaVisita AND C.Jornada = 2) AS 'JT'  ", m, y)
        Query += String.Format("FROM SIGNOSVITALES AS S WHERE Month(S.FechaProximaVisita) = {0} And Year(S.FechaProximaVisita) = {1}  ", m, y)
        Query += "GROUP BY S.FechaProximaVisita "

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & m & "_" & y
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Visitas*//

    Public Function ConteoVisitas(ByVal m As String, ByVal y As String, ByVal usuario As String) As DataTable
        _page = "db.ConteoVisitas"
        Dim Query As String = String.Empty
        Query = "SELECT CONVERT(VARCHAR,FechaVisita,103) AS 'Fecha', COUNT(DISTINCT IdPaciente) AS 'Conteo' "
        Query += String.Format("FROM SIGNOSVITALES WHERE Month(FechaVisita) = {0} And Year(FechaVisita) = {1} ", m, y)
        Query += "GROUP BY FechaVisita "
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & m & "_" & y
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Feriados*//

    Public Function ConteoFeriados(ByVal m As String, ByVal usuario As String) As DataTable
        _page = "db.ConteoFeriados"
        Dim Query As String = String.Empty
        Query = "SELECT DiaFeriado, Descripcion FROM CITAS_FERIADOS "
        Query += String.Format("WHERE MesFeriado = {0} ", m)
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & m
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ConteoFeriados(ByVal usuario As String) As DataTable
        _page = "db.ConteoFeriados"
        Dim Query As String = String.Empty
        Query = "SELECT DiaFeriado, MesFeriado, Descripcion FROM CITAS_FERIADOS "
        'Query += String.Format("WHERE MesFeriado = {0} ", m)
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Reportes*//
    Public Function RepMensualSIGPRO(ByVal fecha As String, ByVal usuario As String) As DataTable
        _page = "db.RepMensualSIGPRO"
        Dim Q As New StringBuilder()
        Q.Append("SELECT E.IdEsquema AS 'ID', E.Descripcion AS 'ESQUEMA', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 10, 14, 1, E.IdEsquema, '" & fecha & "')) AS 'M1014', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 10, 14, 2, E.IdEsquema, '" & fecha & "')) AS 'F1014', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 10, 14, 3, E.IdEsquema, '" & fecha & "')) AS 'T1014', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 15, 24, 1, E.IdEsquema, '" & fecha & "')) AS 'M1524', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 15, 24, 2, E.IdEsquema, '" & fecha & "')) AS 'F1524', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 15, 24, 3, E.IdEsquema, '" & fecha & "')) AS 'T1524', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 25, 49, 1, E.IdEsquema, '" & fecha & "')) AS 'M2549', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 25, 49, 2, E.IdEsquema, '" & fecha & "')) AS 'F2549', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 25, 49, 3, E.IdEsquema, '" & fecha & "')) AS 'T2549', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 50, 100, 1, E.IdEsquema, '" & fecha & "')) AS 'M50', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 50, 100, 2, E.IdEsquema, '" & fecha & "')) AS 'F50', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(2, 50, 100, 3, E.IdEsquema, '" & fecha & "')) AS 'T50', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_2(2, 1, E.IdEsquema, '" & fecha & "')) AS 'MT', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_2(2, 2, E.IdEsquema, '" & fecha & "')) AS 'FT', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_2(2, 3, E.IdEsquema, '" & fecha & "')) AS 'TT', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_3(2, E.IdEsquema, '" & fecha & "')) AS 'SUBT1', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(3, 15, 24, 2, E.IdEsquema, '" & fecha & "')) AS 'PP1524', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(3, 25, 49, 2, E.IdEsquema, '" & fecha & "')) AS 'PP2549', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(3, 50, 100, 2, E.IdEsquema, '" & fecha & "')) AS 'PP50', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_3(3, E.IdEsquema, '" & fecha & "')) AS 'SUBT2', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(1, 15, 24, 2, E.IdEsquema, '" & fecha & "')) AS 'EMB1524', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(1, 25, 49, 2, E.IdEsquema, '" & fecha & "')) AS 'EMB2549', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_1(1, 50, 100, 2, E.IdEsquema, '" & fecha & "')) AS 'EMB50', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_3(1, E.IdEsquema, '" & fecha & "')) AS 'SUBT3', ")
        Q.Append("(SELECT * FROM fn_RepARVXEsquema_4(E.IdEsquema, '" & fecha & "')) AS 'TOTAL' ")
        Q.Append("FROM Esquemas AS E ")
        'Q.Append("WHERE (E.IdEsquema BETWEEN 1 AND 34) ")
        Q.Append("ORDER BY E.IdEsquema ASC")
        Dim Query As String = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & fecha
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ReportesMensual(ByVal tipo As String, ByVal fechaI As String, ByVal fechaF As String, ByVal usuario As String) As DataTable
        _page = "db.ReportesMensual"
        Dim Query As String = String.Empty
        Select Case tipo
            Case "1" 'EMBARAZADAS
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A INNER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                'Query += "WHERE A.NHC NOT LIKE '%P%' AND A.Embarazo = '1' AND A.Med1_ARVEstatus NOT IN (6, 7, 12, 21) "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.Embarazo = '1' AND A.EsquemaEstatus NOT IN (6, 7, 12, 21) "
                Query += "AND A.IdCCARV = (SELECT TOP(1) B.IdCCARV FROM ControlARV AS B "
                Query += "WHERE B.NHC = A.NHC AND B.FechaEntrega = (SELECT TOP(1) C.FechaEntrega FROM ControlARV AS C "
                Query += "WHERE C.NHC = B.NHC AND C.FechaEntrega <= '" & fechaF & " 23:59:59.999' ORDER BY C.FechaEntrega DESC) "
                Query += "ORDER BY B.IdCCARV DESC)"
            Case "1P"
                Query = ""
            Case "2" 'POSTPARTOS
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A INNER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                'Query += "WHERE A.NHC NOT LIKE '%P%' AND A.Embarazo = '3' AND A.Med1_ARVEstatus NOT IN (6, 7, 12, 21) "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.Embarazo = '3' AND A.EsquemaEstatus NOT IN (6, 7, 12, 21) "
                Query += "AND A.IdCCARV = (SELECT TOP(1) B.IdCCARV FROM ControlARV AS B "
                Query += "WHERE B.NHC = A.NHC AND B.FechaEntrega = (SELECT TOP(1) C.FechaEntrega FROM ControlARV AS C "
                Query += "WHERE C.NHC = B.NHC AND C.FechaEntrega <= '" & fechaF & " 23:59:59.999' ORDER BY C.FechaEntrega DESC) "
                Query += "ORDER BY B.IdCCARV DESC)"
            Case "2P"
                Query = ""
            Case "3" 'FALLECIDOS
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A INNER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                'Query += "WHERE A.NHC NOT LIKE '%P%' AND A.Med1_ARVEstatus = 12 "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.EsquemaEstatus = 12 "
                Query += "AND A.IdCCARV = (SELECT TOP(1) B.IdCCARV FROM ControlARV AS B "
                Query += "WHERE B.NHC = A.NHC AND B.FechaEntrega = (SELECT TOP(1) C.FechaEntrega FROM ControlARV AS C "
                Query += "WHERE C.NHC = B.NHC AND (C.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999') ORDER BY C.FechaEntrega DESC) "
                Query += "ORDER BY B.IdCCARV DESC)"
            Case "3P" 'FALLECIDOS PEDIATRICO
                Query = "SELECT A.NHC, C.Genero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A INNER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                'Query += "WHERE A.NHC LIKE '%P%' AND A.Med1_ARVEstatus = 12 "
                Query += "WHERE A.NHC LIKE '%P%' AND A.EsquemaEstatus = 12 "
                Query += "AND A.IdCCARV = (SELECT TOP(1) B.IdCCARV FROM ControlARV AS B "
                Query += "WHERE B.NHC = A.NHC AND B.FechaEntrega = (SELECT TOP(1) C.FechaEntrega FROM ControlARV AS C "
                Query += "WHERE C.NHC = B.NHC AND (C.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999') ORDER BY C.FechaEntrega DESC) "
                Query += "ORDER BY B.IdCCARV DESC)"
            Case "4" 'ABANDONOS
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A INNER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                'Query += "WHERE A.NHC NOT LIKE '%P%' AND A.Med1_ARVEstatus = 6 "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.EsquemaEstatus = 6 "
                Query += "AND A.IdCCARV = (SELECT TOP(1) B.IdCCARV FROM ControlARV AS B "
                Query += "WHERE B.NHC = A.NHC AND B.FechaEntrega = (SELECT TOP(1) C.FechaEntrega FROM ControlARV AS C "
                Query += "WHERE C.NHC = B.NHC AND (C.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999') ORDER BY C.FechaEntrega DESC) "
                Query += "ORDER BY B.IdCCARV DESC)"
            Case "4P" 'ABANDONOS PEDIATRICO
                Query = "SELECT A.NHC, C.Genero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A INNER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                'Query += "WHERE A.NHC LIKE '%P%' AND A.Med1_ARVEstatus = 6 "
                Query += "WHERE A.NHC LIKE '%P%' AND A.EsquemaEstatus = 6 "
                Query += "AND A.IdCCARV = (SELECT TOP(1) B.IdCCARV FROM ControlARV AS B "
                Query += "WHERE B.NHC = A.NHC AND B.FechaEntrega = (SELECT TOP(1) C.FechaEntrega FROM ControlARV AS C "
                Query += "WHERE C.NHC = B.NHC AND (C.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999') ORDER BY C.FechaEntrega DESC) "
                Query += "ORDER BY B.IdCCARV DESC)"
            Case "5" 'TRASLADOS
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A INNER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                'Query += "WHERE A.NHC NOT LIKE '%P%' AND A.Med1_ARVEstatus = 7 "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.EsquemaEstatus = 7 "
                Query += "AND A.IdCCARV = (SELECT TOP(1) B.IdCCARV FROM ControlARV AS B "
                Query += "WHERE B.NHC = A.NHC AND B.FechaEntrega = (SELECT TOP(1) C.FechaEntrega FROM ControlARV AS C "
                Query += "WHERE C.NHC = B.NHC AND (C.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999') ORDER BY C.FechaEntrega DESC) "
                Query += "ORDER BY B.IdCCARV DESC)"
            Case "5P" 'TRASLADOS PEDIATRICO
                Query = "SELECT A.NHC, C.Genero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A INNER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                'Query += "WHERE A.NHC LIKE '%P%' AND A.Med1_ARVEstatus = 7 "
                Query += "WHERE A.NHC LIKE '%P%' AND A.EsquemaEstatus = 7 "
                Query += "AND A.IdCCARV = (SELECT TOP(1) B.IdCCARV FROM ControlARV AS B "
                Query += "WHERE B.NHC = A.NHC AND B.FechaEntrega = (SELECT TOP(1) C.FechaEntrega FROM ControlARV AS C "
                Query += "WHERE C.NHC = B.NHC AND (C.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999') ORDER BY C.FechaEntrega DESC) "
                Query += "ORDER BY B.IdCCARV DESC)"
            Case "6" 'INICIOS
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                'Query += "WHERE A.NHC NOT LIKE '%P%' AND A.Med1_ARVEstatus = 2 "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.EsquemaEstatus = 2 "
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
            Case "6P" 'INICIOS PEDIATRICO
                Query = "SELECT A.NHC, C.Genero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                'Query += "WHERE A.NHC LIKE '%P%' AND A.Med1_ARVEstatus = 2 "
                Query += "WHERE A.NHC LIKE '%P%' AND A.EsquemaEstatus = 2 "
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
            Case "7" 'REINICIOS
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                'Query += "WHERE A.NHC NOT LIKE '%P%' AND A.Med1_ARVEstatus = 3 "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.EsquemaEstatus = 3 "
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
            Case "7P" 'REINICIOS PEDIATRICO
                Query = "SELECT A.NHC, C.Genero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A INNER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                'Query += "WHERE A.NHC LIKE '%P%' AND A.Med1_ARVEstatus = 3 "
                Query += "WHERE A.NHC LIKE '%P%' AND A.EsquemaEstatus = 3 "
                Query += "AND A.IdCCARV = (SELECT TOP(1) B.IdCCARV FROM ControlARV AS B "
                Query += "WHERE B.NHC = A.NHC AND B.FechaEntrega = (SELECT TOP(1) C.FechaEntrega FROM ControlARV AS C "
                Query += "WHERE C.NHC = B.NHC AND (C.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999') ORDER BY C.FechaEntrega DESC) "
                Query += "ORDER BY B.IdCCARV DESC)"
            Case "8" 'CAMBIOS
                'Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                'Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                'Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                'Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                'Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                'Query += "WHERE A.NHC NOT LIKE '%P%' AND (A.Med1_ARVEstatus IN (18, 14) OR A.Med2_ARVEstatus IN (18, 14) OR A.Med3_ARVEstatus IN (18, 14) OR A.Med4_ARVEstatus IN (18, 14) OR A.Med5_ARVEstatus IN (18, 14)) "
                'Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                'Query += "ORDER BY A.FechaEntrega DESC"
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.EsquemaEstatus IN (18, 14) "
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
            Case "8P" 'CAMBIOS PEDIATRICO
                'Query = "SELECT A.NHC, C.Genero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                'Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                'Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                'Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                'Query += "WHERE A.NHC LIKE '%P%' AND (A.Med1_ARVEstatus IN (18, 14) OR A.Med2_ARVEstatus IN (18, 14) OR A.Med3_ARVEstatus IN (18, 14) OR A.Med4_ARVEstatus IN (18, 14) OR A.Med5_ARVEstatus IN (18, 14) OR A.Med6_ARVEstatus IN (18, 14) OR A.Med7_ARVEstatus IN (18, 14) OR A.Med8_ARVEstatus IN (18, 14)) "
                'Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                'Query += "ORDER BY A.FechaEntrega DESC"
                Query = "SELECT A.NHC, C.Genero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                Query += "WHERE A.NHC LIKE '%P%' AND A.EsquemaEstatus IN (18, 14, 25)"
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
            Case "9" 'REFERIDOS
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                'Query += "WHERE A.NHC NOT LIKE '%P%' AND (A.Med1_ARVEstatus IN (18, 14) OR A.Med2_ARVEstatus IN (18, 14) OR A.Med3_ARVEstatus IN (18, 14) OR A.Med4_ARVEstatus IN (18, 14) OR A.Med5_ARVEstatus IN (18, 14)) "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.EsquemaEstatus = 13 "
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
            Case "9P" 'REFERIDOS PEDIATRICO
                Query = "SELECT A.NHC, C.Genero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                'Query += "WHERE A.NHC LIKE '%P%' AND (A.Med1_ARVEstatus IN (18, 14) OR A.Med2_ARVEstatus IN (18, 14) OR A.Med3_ARVEstatus IN (18, 14) OR A.Med4_ARVEstatus IN (18, 14) OR A.Med5_ARVEstatus IN (18, 14) OR A.Med6_ARVEstatus IN (18, 14) OR A.Med7_ARVEstatus IN (18, 14) OR A.Med8_ARVEstatus IN (18, 14)) "
                Query += "WHERE A.NHC LIKE '%P%' AND A.EsquemaEstatus = 13 "
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
            Case "10" 'REINGRESOS
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                'Query += "WHERE A.NHC NOT LIKE '%P%' AND (A.Med1_ARVEstatus IN (18, 14) OR A.Med2_ARVEstatus IN (18, 14) OR A.Med3_ARVEstatus IN (18, 14) OR A.Med4_ARVEstatus IN (18, 14) OR A.Med5_ARVEstatus IN (18, 14)) "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.EsquemaEstatus = 24 "
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
            Case "10P" 'REINGRESOS PEDIATRICO
                Query = "SELECT A.NHC, C.Genero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                'Query += "WHERE A.NHC LIKE '%P%' AND (A.Med1_ARVEstatus IN (18, 14) OR A.Med2_ARVEstatus IN (18, 14) OR A.Med3_ARVEstatus IN (18, 14) OR A.Med4_ARVEstatus IN (18, 14) OR A.Med5_ARVEstatus IN (18, 14) OR A.Med6_ARVEstatus IN (18, 14) OR A.Med7_ARVEstatus IN (18, 14) OR A.Med8_ARVEstatus IN (18, 14)) "
                Query += "WHERE A.NHC LIKE '%P%' AND A.EsquemaEstatus = 24 "
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
            Case "11" 'CAMBIOS FORMA FARMACEUTICA
                Query = "SELECT A.NHC, C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND A.EsquemaEstatus = 20 "
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
            Case "11P" 'CAMBIOS FORMA FARMACEUTICA PEDIATRICO
                Query = "SELECT A.NHC, C.Genero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', A.IdEsquema "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                Query += "WHERE A.NHC LIKE '%P%' AND A.EsquemaEstatus = 20"
                Query += "AND A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999' "
                Query += "ORDER BY A.FechaEntrega DESC"
        End Select
        Dim Ds As New DataSet()
        If Query <> "" Then
            Try
                Using connection As New SqlConnection(_cn1)
                    connection.Open()
                    Dim adapter As New SqlDataAdapter()
                    adapter.SelectCommand = New SqlCommand(Query, connection)
                    adapter.SelectCommand.CommandTimeout = TimeoutDB
                    adapter.Fill(Ds, _page)
                    adapter.Dispose()
                    connection.Dispose()
                    connection.Close()
                End Using
                Return Ds.Tables(0)
            Catch ex As SqlException
                _error = ex.Message
                _pageO = _page & "_" & tipo
                GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
                Return Nothing
            End Try
        Else
            Return Nothing
        End If
    End Function

    Public Function ReportesMensualIOS_ITS(ByVal tipo As String, ByVal fechaI As String, ByVal fechaF As String, ByVal usuario As String) As DataTable
        _page = "db.ReportesMensualIOS_ITS"
        Dim Query As String = String.Empty
        Select Case tipo
            Case "1" 'REPORTE IOS
                Query = "SELECT  E.NomEnfermedad, E.Enfermedad, "
                Query += "SUM(CASE WHEN P.IdGenero = 1 THEN 1 ELSE 0 END) AS 'M', "
                Query += "SUM(CASE WHEN P.IdGenero = 2 THEN 1 ELSE 0 END) AS 'F', "
                Query += "SUM(CASE WHEN P.IdGenero = 3 THEN 1 ELSE 0 END) AS 'T', "
                Query += "COUNT(E.Enfermedad) AS TOTAL "
                Query += "FROM ENFERMEDAD_PAC AS E INNER JOIN "
                Query += "PAC_BASALES AS P ON E.IdPaciente = P.IdPaciente "
                Query += "WHERE (E.FechaEnfermedad BETWEEN '" & fechaI & "' AND '" & fechaF & "') AND (E.Enfermedad LIKE 'ED%' OR E.Enfermedad LIKE 'ER%' OR "
                Query += "E.Enfermedad IN ('D50', 'B37.3+', 'D07.2', 'A87.2', 'A07.2', 'L21', 'N87.0', 'N87.1', 'B86', 'B16', 'B16.0', 'B16.1', 'B16.9', 'B16.2', 'B17.1', 'B17.2', 'B18', 'B02', 'B39', 'B39.4', 'B39.9', 'B17.0', 'A60', 'B39.5', 'B39.3', 'B39.0', 'B39.1', 'B39.2', 'B00', 'A81.2', 'G03.0', 'G00', 'G00.9', 'G03.1', 'A87.1+', 'B37.5+', 'B38.4+', 'B02.1+', 'B02.1+', 'G03', 'B01.0+', 'G03.8', 'G01*', 'G02.0*', 'G02.1*', 'G02*', 'G02.8*', 'A87.0', 'G00.3', 'G00.2', 'B00.3+', 'A39.0+', 'G00.1', 'G00.0', 'B26.1+', 'A20.3', 'G03.2', 'A17.0+', 'A17.0+', 'A87', 'A87.9', 'A32.1', 'G03.9', 'B08.1', 'J15', 'J15.9', 'J13', 'J12', 'J12.9', 'J18.9', 'B17', 'B17.8', 'G00.8', 'A87.8', 'J15.8', 'J18.8', 'A19.8', 'B97.7', 'B05.1', 'A50', 'A50.1', 'A50.2', 'A50.0', 'A59', 'A59.8', 'A59.9', 'A18', 'A18.8', 'A19.9', 'N76.0', 'L30.9', 'D50.9', 'D51', 'B16', 'B16.0', 'B16.1', 'B16.2', 'B16.9', 'N77.1*', 'N73', 'N73.8', 'N76', 'A53.9', 'A51.3', 'A51.2', 'A18.3')) "
                Query += "GROUP BY E.Enfermedad, E.NomEnfermedad "
                Query += "ORDER BY TOTAL DESC"
            Case "2" 'LISTA PX IOS
                Query = "SELECT I.NHC, DATEPART(YEAR, CONVERT(DATETIME, CONVERT(DATE, '01/01/' + SUBSTRING(I.NHC, 5, 2)))) AS 'Cohorte', "
                Query += "CONVERT(INTEGER, dbo.fn_ObtieneEdad2(P.FechaNacimiento, '" & fechaF & "')) AS 'Edad', P.IdGenero, "
                Query += "CONVERT(VARCHAR, E.FechaEnfermedad, 103) AS 'FechaEnfermedad', E.Enfermedad, E.NomEnfermedad "
                Query += "FROM ENFERMEDAD_PAC AS E INNER JOIN "
                Query += "PAC_BASALES AS P ON E.IdPaciente = P.IdPaciente INNER JOIN "
                Query += "PAC_ID AS I ON E.IdPaciente = I.IdPaciente "
                Query += "WHERE (E.FechaEnfermedad BETWEEN '" & fechaI & "' AND '" & fechaF & "') AND (Enfermedad LIKE 'ED%' OR Enfermedad LIKE 'ER%' OR "
                Query += "E.Enfermedad IN ('D50', 'B37.3+', 'D07.2', 'A87.2', 'A07.2', 'L21', 'N87.0', 'N87.1', 'B86', 'B16', 'B16.0', 'B16.1', 'B16.9', 'B16.2', 'B17.1', 'B17.2', 'B18', 'B02', 'B39', 'B39.4', 'B39.9', 'B17.0', 'A60', 'B39.5', 'B39.3', 'B39.0', 'B39.1', 'B39.2', 'B00', 'A81.2', 'G03.0', 'G00', 'G00.9', 'G03.1', 'A87.1+', 'B37.5+', 'B38.4+', 'B02.1+', 'B02.1+', 'G03', 'B01.0+', 'G03.8', 'G01*', 'G02.0*', 'G02.1*', 'G02*', 'G02.8*', 'A87.0', 'G00.3', 'G00.2', 'B00.3+', 'A39.0+', 'G00.1', 'G00.0', 'B26.1+', 'A20.3', 'G03.2', 'A17.0+', 'A17.0+', 'A87', 'A87.9', 'A32.1', 'G03.9', 'B08.1', 'J15', 'J15.9', 'J13', 'J12', 'J12.9', 'J18.9', 'B17', 'B17.8', 'G00.8', 'A87.8', 'J15.8', 'J18.8', 'A19.8', 'B97.7', 'B05.1', 'A50', 'A50.1', 'A50.2', 'A50.0', 'A59', 'A59.8', 'A59.9', 'A18', 'A18.8', 'A19.9', 'N76.0', 'L30.9', 'D50.9', 'D51', 'B16', 'B16.0', 'B16.1', 'B16.2', 'B16.9', 'N77.1*', 'N73', 'N73.8', 'N76', 'A53.9', 'A51.3', 'A51.2', 'A18.3'))"
            Case "3" 'REPORTE ITS
                Query = "SELECT A.NomAgenteITS AS 'ITS', "
                Query += "SUM(CASE WHEN P.IdGenero = 1 THEN 1 ELSE 0 END) AS 'M', "
                Query += "SUM(CASE WHEN P.IdGenero = 2 THEN 1 ELSE 0 END) AS 'F', "
                Query += "SUM(CASE WHEN P.IdGenero = 3 THEN 1 ELSE 0 END) AS 'T', "
                Query += "COUNT(E.AgenteITS) AS TOTAL "
                Query += "FROM ITS AS E INNER JOIN "
                Query += "ITS_M_AGENTE AS A ON A.IdAgenteITS = E.AgenteITS INNER JOIN "
                Query += "PAC_BASALES AS P ON E.IdPaciente = P.IdPaciente "
                Query += "WHERE (E.FechaITS BETWEEN '" & fechaI & "' AND '" & fechaF & "') "
                Query += "GROUP BY E.AgenteITS, A.NomAgenteITS "
                Query += "ORDER BY TOTAL DESC"
            Case "4" 'LISTA PX ITS
                Query = "SELECT I.NHC, DATEPART(YEAR, CONVERT(DATETIME, CONVERT(DATE, '01/01/' + SUBSTRING(I.NHC, 5, 2)))) AS 'Cohorte', "
                Query += "CONVERT(INTEGER, dbo.fn_ObtieneEdad2(P.FechaNacimiento, '" & fechaF & "')) AS 'Edad', P.IdGenero, "
                Query += "CONVERT(VARCHAR, E.FechaITS, 103) AS 'FechaITS', E.AgenteITS, A.NomAgenteITS AS 'ITS' "
                Query += "FROM ITS AS E INNER JOIN "
                Query += "ITS_M_AGENTE AS A ON A.IdAgenteITS = E.AgenteITS INNER JOIN "
                Query += "PAC_BASALES AS P ON E.IdPaciente = P.IdPaciente INNER JOIN "
                Query += "PAC_ID AS I ON E.IdPaciente = I.IdPaciente "
                Query += "WHERE (E.FechaITS BETWEEN '" & fechaI & "' AND '" & fechaF & "') "
        End Select
        Dim Ds As New DataSet()
        If Query <> "" Then
            Try
                Using connection As New SqlConnection(_cn2)
                    connection.Open()
                    Dim adapter As New SqlDataAdapter()
                    adapter.SelectCommand = New SqlCommand(Query, connection)
                    adapter.SelectCommand.CommandTimeout = TimeoutDB
                    adapter.Fill(Ds, _page)
                    adapter.Dispose()
                    connection.Dispose()
                    connection.Close()
                End Using
                Return Ds.Tables(0)
            Catch ex As SqlException
                _error = ex.Message
                _pageO = _page & "_" & tipo
                GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
                Return Nothing
            End Try
        Else
            Return Nothing
        End If
    End Function

    Public Function ReportesMensualNoARV(ByVal tipo As String, ByVal fechaI As String, ByVal fechaF As String, ByVal usuario As String) As DataTable
        _page = "db.ReportesMensualNoARV"
        Dim Query As String = String.Empty
        Select Case tipo
            Case "1" 'NUEVOS
                Query = String.Format("SELECT NHC, Cohorte, IdGenero, Edad, Paciente FROM dbo.fn_NARV_Nuevos('{0}','{1}')", fechaI, fechaF)
            Case "2" 'REINGRESOS
                Query = String.Format("SELECT NHC, Cohorte, IdGenero, Edad, Paciente FROM dbo.fn_NARV_Reinicio('{0}','{1}')", fechaI, fechaF)
            Case "3" 'ABANDONOS
                Query = String.Format("SELECT NHC, Cohorte, IdGenero, Edad, Paciente FROM dbo.fn_NARV_Abandono('{0}','{1}')", fechaI, fechaF)
            Case "4" 'FALLECIDOS
                Query = String.Format("SELECT NHC, Cohorte, IdGenero, Edad, Paciente FROM dbo.fn_NARV_Fallecidos('{0}','{1}')", fechaI, fechaF)
            Case "5" 'TRASLADOS
                Query = String.Format("SELECT NHC, Cohorte, IdGenero, Edad, Paciente FROM dbo.fn_NARV_Traslados('{0}','{1}')", fechaI, fechaF)
            Case "6" 'CAMBIO EDAD
                Query = "SELECT NHC, Cohorte, IdGenero, CONVERT(VARCHAR,FechaNacimiento,103) AS 'FechaNacimiento', Edad, EdadAnterior, Paciente "
                Query += String.Format("FROM dbo.fn_ListaPacientesNARV2('{0}','{1}') ", fechaI, fechaF)
                Query += "WHERE EdadAnterior <> Edad AND EdadAnterior IN (14,24,49)"
            Case "7" 'INICIAN TARV
                Query = String.Format("SELECT NHC, Cohorte, IdGenero, Edad, Paciente FROM dbo.fn_NARV_InicianTARV('{0}','{1}')", fechaI, fechaF)
            Case "8" 'TOTAL ACTIVOS
                Query = String.Format("SELECT NHC, Cohorte, IdGenero, Edad, Paciente FROM dbo.fn_ListaPacientesNARV2('{0}','{1}') WHERE IdPaciente IN (SELECT DISTINCT S.idpaciente FROM SIGNOSVITALES AS S WHERE S.FechaVisita <= CONVERT(DATE,'{1}'))", fechaI, fechaF)
            Case "9" 'NO TARV ACTIVO
                Query = "SELECT I.NHC, DATEPART(YEAR, CONVERT(DATETIME, CONVERT(DATE, '01/01/' + SUBSTRING(I.NHC, 5, 2)))) AS 'Cohorte', "
                Query += String.Format("B.IdGenero, dbo.fn_ObtieneEdad2(CONVERT(DATE, B.FechaNacimiento), '{0}') AS 'Edad', ", fechaF)
                Query += "LTRIM(RTRIM(B.PrimerNombre)) + (CASE WHEN B.SegundoNombre IS NULL THEN '' WHEN B.SegundoNombre = 'SSN' THEN '' ELSE ' ' + "
                Query += "LTRIM(RTRIM(B.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(B.PrimerApellido)) + (CASE WHEN B.SegundoApellido IS NULL THEN '' "
                Query += "WHEN B.SegundoApellido = 'SSA' THEN '' ELSE ' ' + LTRIM(RTRIM(B.SegundoApellido)) END) AS 'Paciente' "
                Query += String.Format("FROM dbo.fn_PacientesNoARVActual('{0}','{1}') AS A INNER JOIN ", fechaI, fechaF)
                Query += "dbo.PAC_ID AS I ON A.IdPaciente = I.Idpaciente INNER JOIN "
                Query += "dbo.PAC_BASALES AS B ON A.IdPaciente = B.IdPaciente"
            Case "10" 'NO TARV/CONSULTA
                Query = String.Format("SELECT NHC, Cohorte, IdGenero, Edad, Paciente FROM dbo.fn_ListaPacientesNARV2('{0}','{1}') WHERE IdPaciente NOT IN (SELECT DISTINCT idpaciente FROM SIGNOSVITALES)", fechaI, fechaF)
        End Select
        Dim Ds As New DataSet()
        If Query <> "" Then
            Try
                Dim adapter As New SqlDataAdapter()
                If tipo = "9" Then
                    Using connection As New SqlConnection(_cn1)
                        connection.Open()
                        adapter.SelectCommand = New SqlCommand(Query, connection)
                        adapter.SelectCommand.CommandTimeout = TimeoutDB
                        adapter.Fill(Ds, _page)
                        adapter.Dispose()
                        connection.Dispose()
                        connection.Close()
                    End Using
                Else
                    Using connection As New SqlConnection(_cn2)
                        connection.Open()
                        adapter.SelectCommand = New SqlCommand(Query, connection)
                        adapter.SelectCommand.CommandTimeout = TimeoutDB
                        adapter.Fill(Ds, _page)
                        adapter.Dispose()
                        connection.Dispose()
                        connection.Close()
                    End Using
                End If
                Return Ds.Tables(0)
            Catch ex As SqlException
                _error = ex.Message
                _pageO = _page & "_" & tipo
                GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
                Return Nothing
            End Try
        Else
            Return Nothing
        End If
    End Function

    Public Function ReportesMensualNoARVResumen(ByVal tipo As String, ByVal fechaI As String, ByVal fechaF As String, ByVal usuario As String) As DataTable
        _page = "db.ReportesMensualNoARVResumen"
        Dim Query As String = String.Empty
        Select Case tipo
            Case "1" 'NUEVOS
                Query = "SELECT G.NomGenero, "
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Nuevos(G.IdGenero, 10, 14,'{0}','{1}')),0) AS 'R1014', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Nuevos(G.IdGenero, 15, 24,'{0}','{1}')),0) AS 'R1524', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Nuevos(G.IdGenero, 25, 49,'{0}','{1}')),0) AS 'R2549', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Nuevos(G.IdGenero, 50, 100,'{0}','{1}')),0) AS 'R50', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Nuevos(G.IdGenero, 10, 100,'{0}','{1}')),0) AS 'total' ", fechaI, fechaF)
                Query += "FROM PAC_M_GENERO AS G"
            Case "2" 'REINGRESOS
                Query = "SELECT G.NomGenero, "
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Reinicio(G.IdGenero, 10, 14,'{0}','{1}')),0) AS 'R1014', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Reinicio(G.IdGenero, 15, 24,'{0}','{1}')),0) AS 'R1524', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Reinicio(G.IdGenero, 25, 49,'{0}','{1}')),0) AS 'R2549', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Reinicio(G.IdGenero, 50, 100,'{0}','{1}')),0) AS 'R50', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Reinicio(G.IdGenero, 10, 100,'{0}','{1}')),0) AS 'total' ", fechaI, fechaF)
                Query += "FROM PAC_M_GENERO AS G"
            Case "3" 'ABANDONOS
                Query = "SELECT G.NomGenero, "
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Abandono(G.IdGenero, 10, 14,'{0}','{1}')),0) AS 'R1014', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Abandono(G.IdGenero, 15, 24,'{0}','{1}')),0) AS 'R1524', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Abandono(G.IdGenero, 25, 49,'{0}','{1}')),0) AS 'R2549', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Abandono(G.IdGenero, 50, 100,'{0}','{1}')),0) AS 'R50', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Abandono(G.IdGenero, 10, 100,'{0}','{1}')),0) AS 'total' ", fechaI, fechaF)
                Query += "FROM PAC_M_GENERO AS G"
            Case "4" 'FALLECIDOS
                Query = "SELECT G.NomGenero, "
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Fallecidos(G.IdGenero, 10, 14,'{0}','{1}')),0) AS 'R1014', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Fallecidos(G.IdGenero, 15, 24,'{0}','{1}')),0) AS 'R1524', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Fallecidos(G.IdGenero, 25, 49,'{0}','{1}')),0) AS 'R2549', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Fallecidos(G.IdGenero, 50, 100,'{0}','{1}')),0) AS 'R50', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Fallecidos(G.IdGenero, 10, 100,'{0}','{1}')),0) AS 'total' ", fechaI, fechaF)
                Query += "FROM PAC_M_GENERO AS G"
            Case "5" 'TRASLADOS
                Query = "SELECT G.NomGenero, "
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Traslados(G.IdGenero, 10, 14,'{0}','{1}')),0) AS 'R1014', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Traslados(G.IdGenero, 15, 24,'{0}','{1}')),0) AS 'R1524', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Traslados(G.IdGenero, 25, 49,'{0}','{1}')),0) AS 'R2549', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Traslados(G.IdGenero, 50, 100,'{0}','{1}')),0) AS 'R50', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_Traslados(G.IdGenero, 10, 100,'{0}','{1}')),0) AS 'total' ", fechaI, fechaF)
                Query += "FROM PAC_M_GENERO AS G"
            Case "6" 'CAMBIO EDAD
                Query = "SELECT G.NomGenero, "
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_SCambioEdad(G.IdGenero, 14,'{0}','{1}')),0) AS 'S1014', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_ECambioEdad(G.IdGenero, 10,'{0}','{1}')),0) AS 'E1014', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_SCambioEdad(G.IdGenero, 24,'{0}','{1}')),0) AS 'S1524', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_ECambioEdad(G.IdGenero, 15,'{0}','{1}')),0) AS 'E1524', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_SCambioEdad(G.IdGenero, 49,'{0}','{1}')),0) AS 'S2549', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_ECambioEdad(G.IdGenero, 25,'{0}','{1}')),0) AS 'E2549', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_SCambioEdad(G.IdGenero, 100,'{0}','{1}')),0) AS 'S50', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_ECambioEdad(G.IdGenero, 50,'{0}','{1}')),0) AS 'E50' ", fechaI, fechaF)
                Query += "FROM PAC_M_GENERO AS G"
            Case "7" 'INICIAN TARV
                Query = "SELECT G.NomGenero, "
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_InicianTARV(G.IdGenero, 10, 14,'{0}','{1}')),0) AS 'R1014', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_InicianTARV(G.IdGenero, 15, 24,'{0}','{1}')),0) AS 'R1524', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_InicianTARV(G.IdGenero, 25, 49,'{0}','{1}')),0) AS 'R2549', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_InicianTARV(G.IdGenero, 50, 100,'{0}','{1}')),0) AS 'R50', ", fechaI, fechaF)
                Query += String.Format("ISNULL((SELECT * FROM dbo.fn_RepNARV_InicianTARV(G.IdGenero, 10, 100,'{0}','{1}')),0) AS 'total' ", fechaI, fechaF)
                Query += "FROM PAC_M_GENERO AS G"
            Case "8" 'TOTAL ACTIVOS
                'Query = String.Format("SELECT NHC, Cohorte, IdGenero, Edad, Paciente FROM dbo.fn_ListaPacientesNARV2('{0}','{1}') WHERE IdPaciente IN (SELECT DISTINCT S.idpaciente FROM SIGNOSVITALES AS S WHERE S.FechaVisita <= CONVERT(DATE,'{1}'))", fechaI, fechaF)
            Case "9" 'NO TARV ACTIVO
                'Query = "SELECT I.NHC, DATEPART(YEAR, CONVERT(DATETIME, CONVERT(DATE, '01/01/' + SUBSTRING(I.NHC, 5, 2)))) AS 'Cohorte', "
                'Query += String.Format("B.IdGenero, dbo.fn_ObtieneEdad2(CONVERT(DATE, B.FechaNacimiento), '{0}') AS 'Edad', ", fechaF)
                'Query += "LTRIM(RTRIM(B.PrimerNombre)) + (CASE WHEN B.SegundoNombre IS NULL THEN '' WHEN B.SegundoNombre = 'SSN' THEN '' ELSE ' ' + "
                'Query += "LTRIM(RTRIM(B.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(B.PrimerApellido)) + (CASE WHEN B.SegundoApellido IS NULL THEN '' "
                'Query += "WHEN B.SegundoApellido = 'SSA' THEN '' ELSE ' ' + LTRIM(RTRIM(B.SegundoApellido)) END) AS 'Paciente' "
                'Query += String.Format("FROM dbo.fn_PacientesNoARVActual('{0}','{1}') AS A INNER JOIN ", fechaI, fechaF)
                'Query += "dbo.PAC_ID AS I ON A.IdPaciente = I.Idpaciente INNER JOIN "
                'Query += "dbo.PAC_BASALES AS B ON A.IdPaciente = B.IdPaciente"
            Case "10" 'NO TARV/CONSULTA
                'Query = String.Format("SELECT NHC, Cohorte, IdGenero, Edad, Paciente FROM dbo.fn_ListaPacientesNARV2('{0}','{1}') WHERE IdPaciente NOT IN (SELECT DISTINCT idpaciente FROM SIGNOSVITALES)", fechaI, fechaF)
        End Select
        Dim Ds As New DataSet()
        If Query <> "" Then
            Try
                Dim adapter As New SqlDataAdapter()
                If tipo = "9" Then
                    Using connection As New SqlConnection(_cn1)
                        connection.Open()
                        adapter.SelectCommand = New SqlCommand(Query, connection)
                        adapter.SelectCommand.CommandTimeout = TimeoutDB
                        adapter.Fill(Ds, _page)
                        adapter.Dispose()
                        connection.Dispose()
                        connection.Close()
                    End Using
                Else
                    Using connection As New SqlConnection(_cn2)
                        connection.Open()
                        adapter.SelectCommand = New SqlCommand(Query, connection)
                        adapter.SelectCommand.CommandTimeout = TimeoutDB
                        adapter.Fill(Ds, _page)
                        adapter.Dispose()
                        connection.Dispose()
                        connection.Close()
                    End Using
                End If
                Return Ds.Tables(0)
            Catch ex As SqlException
                _error = ex.Message
                _pageO = _page & "_" & tipo
                GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
                Return Nothing
            End Try
        Else
            Return Nothing
        End If
    End Function

    Public Function ReporteConsumos(ByVal tipo As String, ByVal fechaA As String, ByVal fechaM As String, ByVal ultimodia As String, ByVal usuario As String) As DataTable
        _page = "db.ReporteConsumos"
        Dim Query As String = String.Empty
        Dim fechaI As String = fechaA.ToString() & "-" & fechaM.ToString() & "-01"
        Dim fechaF As String = fechaA.ToString() & "-" & fechaM.ToString() & "-" & ultimodia
        Dim fecha As String = fechaA.ToString() & "-" & fechaM.ToString() & "-"
        Dim diafin As Integer = CInt(ultimodia)
        Select Case tipo
            Case "1" 'ARVS
                Query = "SELECT A.IdFFARV, '0' + A.Codigo AS 'Codigo', B.NomARV, C.NomFF, A.Concentracion, "
                For d As Integer = 1 To diafin
                    Query += "(SELECT * FROM dbo.fn_ObtieneCantidadMedFechas(A.IdFFARV,'" & fecha & d & "')) AS '" & Right("0" + d.ToString(), 2) & "', "
                Next
                Query += "(SELECT * FROM dbo.fn_ObtieneCantidadMed(A.IdFFARV,'" & fechaI & "','" & fechaF & "')) AS 'Total' "
                Query += "FROM FFARV AS A INNER JOIN "
                Query += "MedARV AS B ON A.IdARV = B.IdARV INNER JOIN "
                Query += "FormaFarmaceutica AS C ON A.IdFF = C.IdFF "
                Query += "ORDER BY A.Codigo"
            Case "2" 'PROFILAXIS
                Query = "SELECT A.IdFFProf, A.Codigo, B.NomProfilaxis, C.NomFF, A.Concentracion, "
                For d As Integer = 1 To diafin
                    Query += "(SELECT * FROM dbo.fn_ObtieneCantidadMedProfFechas(A.IdFFProf,'" & fecha & d & "')) AS '" & Right("0" + d.ToString(), 2) & "', "
                Next
                Query += "(SELECT * FROM dbo.fn_ObtieneCantidadMedProf(A.IdFFProf,'" & fechaI & "','" & fechaF & "')) AS 'Total' "
                Query += "FROM FFProf AS A INNER JOIN "
                Query += "MedProf AS B ON A.IdProf = B.IdProf INNER JOIN "
                Query += "FormaFarmaceutica AS C ON A.IdFF = C.IdFF "
                Query += "ORDER BY A.Codigo"
        End Select
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & tipo
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ReporteProyeccion(ByVal tipo As String, ByVal fechaA As String, ByVal fechaM As String, ByVal ultimodia As String, ByVal usuario As String) As DataTable
        _page = "db.ReporteProyeccion"
        Dim Query As String = String.Empty
        Dim fechaI As String = fechaA.ToString() & "-" & fechaM.ToString() & "-01"
        Dim fechaF As String = fechaA.ToString() & "-" & fechaM.ToString() & "-" & ultimodia
        Dim fecha As String = fechaA.ToString() & "-" & fechaM.ToString() & "-"
        Dim diafin As Integer = CInt(ultimodia)
        Select Case tipo
            Case "1" 'ARVS
                Query = "SELECT A.IdFFARV, '0' + A.Codigo AS 'Codigo', B.NomARV, C.NomFF, A.Concentracion, "
                For d As Integer = 1 To diafin
                    Query += "(SELECT * FROM dbo.fn_ObtieneCantidadPxMedFechas(A.IdFFARV,'" & fecha & d & "')) AS '" & Right("0" + d.ToString(), 2) & "', "
                Next
                Query += "(SELECT * FROM dbo.fn_ObtieneCantidadPxMed(A.IdFFARV,'" & fechaI & "','" & fechaF & "')) AS 'Total' "
                Query += "FROM FFARV AS A INNER JOIN "
                Query += "MedARV AS B ON A.IdARV = B.IdARV INNER JOIN "
                Query += "FormaFarmaceutica AS C ON A.IdFF = C.IdFF "
                Query += "ORDER BY A.Codigo"
            Case "2" 'PROFILAXIS
                'Query = "SELECT A.IdFFProf, A.Codigo, B.NomProfilaxis, C.NomFF, A.Concentracion, "
                'For d As Integer = 1 To diafin
                '    Query += "(SELECT * FROM dbo.fn_ObtieneCantidadMedProfFechas(A.IdFFProf,'" & fecha & d & "')) AS '" & Right("0" + d.ToString(), 2) & "', "
                'Next
                'Query += "(SELECT * FROM dbo.fn_ObtieneCantidadMedProf(A.IdFFProf,'" & fechaI & "','" & fechaF & "')) AS 'Total' "
                'Query += "FROM FFProf AS A INNER JOIN "
                'Query += "MedProf AS B ON A.IdProf = B.IdProf INNER JOIN "
                'Query += "FormaFarmaceutica AS C ON A.IdFF = C.IdFF "
                'Query += "ORDER BY A.Codigo"
        End Select
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & tipo
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function RepPxConsumo(ByVal tipo As String, ByVal fechaA As String, ByVal fechaM As String, ByVal ultimodia As String, ByVal usuario As String) As DataTable
        _page = "db.RepPxConsumo"
        Dim Query As String = String.Empty
        Dim fechaI As String = fechaA.ToString() & "-" & fechaM.ToString() & "-01"
        Dim fechaF As String = fechaA.ToString() & "-" & fechaM.ToString() & "-" & ultimodia
        Select Case tipo
            Case "1" 'ARVS
                Query = "SELECT A.NHC, LTRIM(RTRIM(C.PrimerNombre)) + (CASE WHEN C.SegundoNombre IS NULL THEN '' WHEN C.SegundoNombre = 'SSN' THEN '' ELSE ' ' "
                Query += "+ LTRIM(RTRIM(C.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(C.PrimerApellido)) "
                Query += "+ (CASE WHEN C.SegundoApellido IS NULL THEN '' WHEN C.SegundoApellido = 'SSA' THEN '' ELSE ' ' "
                Query += "+ LTRIM(RTRIM(C.SegundoApellido)) END) AS 'Paciente', C.IdGenero, "
                Query += "dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', CONVERT(VARCHAR,A.FechaRetorno, 103) AS 'FechaRetorno', "
                Query += "A.IdEsquema, dbo.fn_ObtieneCodigoSEsquema(A.IdSEsquema) AS 'SEsquema', dbo.fn_ObtieneEstatusMed(A.EsquemaEstatus) AS 'Estatus', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med1_Codigo) AS 'M1', A.Med1_Cantidad AS 'MC1', dbo.fn_ObtieneEstatusMed(A.Med1_ARVEstatus) AS 'ME1', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med2_Codigo) AS 'M2', A.Med2_Cantidad AS 'MC2', dbo.fn_ObtieneEstatusMed(A.Med2_ARVEstatus) AS 'ME2', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med3_Codigo) AS 'M3', A.Med3_Cantidad AS 'MC3', dbo.fn_ObtieneEstatusMed(A.Med3_ARVEstatus) AS 'ME3', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med4_Codigo) AS 'M4', A.Med4_Cantidad AS 'MC4', dbo.fn_ObtieneEstatusMed(A.Med4_ARVEstatus) AS 'ME4', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med5_Codigo) AS 'M5', A.Med5_Cantidad AS 'MC5', dbo.fn_ObtieneEstatusMed(A.Med5_ARVEstatus) AS 'ME5', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med6_Codigo) AS 'M6', A.Med6_Cantidad AS 'MC6', dbo.fn_ObtieneEstatusMed(A.Med6_ARVEstatus) AS 'ME6', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med7_Codigo) AS 'M7', A.Med7_Cantidad AS 'MC7', dbo.fn_ObtieneEstatusMed(A.Med7_ARVEstatus) AS 'ME7', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med8_Codigo) AS 'M8', A.Med8_Cantidad AS 'MC8', dbo.fn_ObtieneEstatusMed(A.Med8_ARVEstatus) AS 'ME8' "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND (A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999') "
                Query += "UNION "
                Query += "SELECT A.NHC, LTRIM(RTRIM(C.PrimerNombre)) + (CASE WHEN C.SegundoNombre IS NULL THEN '' WHEN C.SegundoNombre = 'SSN' THEN '' ELSE ' ' "
                Query += "+ LTRIM(RTRIM(C.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(C.PrimerApellido)) "
                Query += "+ (CASE WHEN C.SegundoApellido IS NULL THEN '' WHEN C.SegundoApellido = 'SSA' THEN '' ELSE ' ' "
                Query += "+ LTRIM(RTRIM(C.SegundoApellido)) END) AS 'Paciente', C.Genero, "
                Query += "dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', '' AS 'Embarazo', "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', CONVERT(VARCHAR,A.FechaRetorno, 103) AS 'FechaRetorno', "
                Query += "A.IdEsquema, dbo.fn_ObtieneCodigoSEsquema(A.IdSEsquema) AS 'SEsquema', dbo.fn_ObtieneEstatusMed(A.EsquemaEstatus) AS 'Estatus', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med1_Codigo) AS 'M1', A.Med1_Cantidad AS 'MC1', dbo.fn_ObtieneEstatusMed(A.Med1_ARVEstatus) AS 'ME1', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med2_Codigo) AS 'M2', A.Med2_Cantidad AS 'MC2', dbo.fn_ObtieneEstatusMed(A.Med2_ARVEstatus) AS 'ME2', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med3_Codigo) AS 'M3', A.Med3_Cantidad AS 'MC3', dbo.fn_ObtieneEstatusMed(A.Med3_ARVEstatus) AS 'ME3', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med4_Codigo) AS 'M4', A.Med4_Cantidad AS 'MC4', dbo.fn_ObtieneEstatusMed(A.Med4_ARVEstatus) AS 'ME4', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med5_Codigo) AS 'M5', A.Med5_Cantidad AS 'MC5', dbo.fn_ObtieneEstatusMed(A.Med5_ARVEstatus) AS 'ME5', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med6_Codigo) AS 'M6', A.Med6_Cantidad AS 'MC6', dbo.fn_ObtieneEstatusMed(A.Med6_ARVEstatus) AS 'ME6', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med7_Codigo) AS 'M7', A.Med7_Cantidad AS 'MC7', dbo.fn_ObtieneEstatusMed(A.Med7_ARVEstatus) AS 'ME7', "
                Query += "'0' + dbo.fn_ObtieneCodMed('A', A.Med8_Codigo) AS 'M8', A.Med8_Cantidad AS 'MC8', dbo.fn_ObtieneEstatusMed(A.Med8_ARVEstatus) AS 'ME8' "
                Query += "FROM ControlARV AS A LEFT OUTER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                Query += "WHERE A.NHC LIKE '%P%' AND (A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999') "
            Case "2" 'PROFILAXIS
                Query = "SELECT A.NHC, LTRIM(RTRIM(C.PrimerNombre)) + (CASE WHEN C.SegundoNombre IS NULL THEN '' WHEN C.SegundoNombre = 'SSN' THEN '' ELSE ' ' "
                Query += "+ LTRIM(RTRIM(C.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(C.PrimerApellido)) "
                Query += "+ (CASE WHEN C.SegundoApellido IS NULL THEN '' WHEN C.SegundoApellido = 'SSA' THEN '' ELSE ' ' "
                Query += "+ LTRIM(RTRIM(C.SegundoApellido)) END) AS 'Paciente', C.IdGenero, "
                Query += "dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', CONVERT(VARCHAR,A.FechaRetorno, 103) AS 'FechaRetorno', "
                Query += "dbo.fn_ObtieneCodMed('P', A.Prof1_Codigo) AS 'P1', A.Prof1_Cantidad AS 'PC1', dbo.fn_ObtieneEstatusMed(A.Prof1_MedEstatus) AS 'PE1', "
                Query += "dbo.fn_ObtieneCodMed('P', A.Prof2_Codigo) AS 'P2', A.Prof2_Cantidad AS 'PC2', dbo.fn_ObtieneEstatusMed(A.Prof2_MedEstatus) AS 'PE2', "
                Query += "dbo.fn_ObtieneCodMed('P', A.Prof3_Codigo) AS 'P3', A.Prof3_Cantidad AS 'PC3', dbo.fn_ObtieneEstatusMed(A.Prof3_MedEstatus) AS 'PE3', "
                Query += "dbo.fn_ObtieneCodMed('P', A.Prof4_Codigo) AS 'P4', A.Prof4_Cantidad AS 'PC4', dbo.fn_ObtieneEstatusMed(A.Prof4_MedEstatus) AS 'PE4' "
                Query += "FROM ControlPROF AS A LEFT OUTER JOIN "
                Query += "PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN "
                Query += "PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente "
                Query += "WHERE A.NHC NOT LIKE '%P%' AND (A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999') "
                Query += "UNION "
                Query += "SELECT A.NHC, LTRIM(RTRIM(C.PrimerNombre)) + (CASE WHEN C.SegundoNombre IS NULL THEN '' WHEN C.SegundoNombre = 'SSN' THEN '' ELSE ' ' "
                Query += "+ LTRIM(RTRIM(C.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(C.PrimerApellido)) "
                Query += "+ (CASE WHEN C.SegundoApellido IS NULL THEN '' WHEN C.SegundoApellido = 'SSA' THEN '' ELSE ' ' "
                Query += "+ LTRIM(RTRIM(C.SegundoApellido)) END) AS 'Paciente', C.Genero, "
                Query += "dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', '' AS 'Embarazo', "
                Query += "CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', CONVERT(VARCHAR,A.FechaRetorno, 103) AS 'FechaRetorno', "
                Query += "dbo.fn_ObtieneCodMed('P', A.Prof1_Codigo) AS 'P1', A.Prof1_Cantidad AS 'PC1', dbo.fn_ObtieneEstatusMed(A.Prof1_MedEstatus) AS 'PE1', "
                Query += "dbo.fn_ObtieneCodMed('P', A.Prof2_Codigo) AS 'P2', A.Prof2_Cantidad AS 'PC2', dbo.fn_ObtieneEstatusMed(A.Prof2_MedEstatus) AS 'PE2', "
                Query += "dbo.fn_ObtieneCodMed('P', A.Prof3_Codigo) AS 'P3', A.Prof3_Cantidad AS 'PC3', dbo.fn_ObtieneEstatusMed(A.Prof3_MedEstatus) AS 'PE3', "
                Query += "dbo.fn_ObtieneCodMed('P', A.Prof4_Codigo) AS 'P4', A.Prof4_Cantidad AS 'PC4', dbo.fn_ObtieneEstatusMed(A.Prof4_MedEstatus) AS 'PE4' "
                Query += "FROM ControlPROF AS A LEFT OUTER JOIN "
                Query += "BasalesPediatria AS C ON C.NHC = A.NHC "
                Query += "WHERE A.NHC LIKE '%P%' AND (A.FechaEntrega BETWEEN '" & fechaI & " 00:00:00.000' AND '" & fechaF & " 23:59:59.999')"
        End Select
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & tipo
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function Reporte90dias(ByVal fechaA As String, ByVal fechaM As String, ByVal ultimodia As String, ByVal usuario As String) As DataTable
        _page = "db.Reporte90dias"
        Dim fecha As String = ultimodia & "/" & fechaM.ToString() & "/" & fechaA.ToString()
        Dim Query As String = "SELECT NHC, Cohorte, Paciente, UltimaFechaEntrega, FechaRetorno, ME, Dias "
        Query += "FROM dbo.fn_PacientesAbandono('" & fecha & "') "
        Query += "WHERE ME NOT IN (6, 7, 12, 21) AND FechaRetorno <= '" & fecha & "' AND Dias <= -90"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & fecha
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ReporteVisitas(ByVal fecha As String, ByVal usuario As String) As DataTable
        _page = "db.ReporteVisitas"
        Dim Query As String = "SELECT * FROM dbo.fn_VisitasFarmacia('" & fecha & "') ORDER BY Cohorte, NHC"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & fecha
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ReporteNoARV(ByVal tipo As String, ByVal fechaA As String, ByVal fechaM As String, ByVal ultimodia As String, ByVal usuario As String) As DataTable
        _page = "db.ReporteNoARV"
        Dim Query As String = String.Empty
        Dim fechaI As String = fechaA.ToString() & "-" & fechaM.ToString() & "-01"
        Dim fechaF As String = fechaA.ToString() & "-" & fechaM.ToString() & "-" & ultimodia
        Dim fecha As String = fechaA.ToString() & "-" & fechaM.ToString() & "-"
        Dim diafin As Integer = CInt(ultimodia)
        Select Case tipo
            Case "1" 'Reporte
                Query = "SELECT * FROM dbo.fn_ReportePacientesNARV('" & fechaF & "')"
            Case "2" 'Listado
                Query = "SELECT Paciente, NHC, Cohorte, IdGenero, Edad FROM dbo.fn_ListaPacientesNARV('" & fechaF & "')"
        End Select
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & tipo
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ReporteNoARV1(ByVal tipo As String, ByVal fechaA As String, ByVal fechaM As String, ByVal ultimodia As String, ByVal fechaAA As String, ByVal fechaMA As String, ByVal ultimodiaA As String, ByVal usuario As String) As DataTable
        _page = "db.ReporteNoARV1"
        Dim Query As String = String.Empty
        Dim fechaI As String = fechaA.ToString() & "-" & fechaM.ToString() & "-01"
        Dim fechaF As String = fechaA.ToString() & "-" & fechaM.ToString() & "-" & ultimodia
        Dim fecha As String = fechaA.ToString() & "-" & fechaM.ToString() & "-"
        Dim diafin As Integer = CInt(ultimodia)
        Dim fechaAnterior As String = fechaAA.ToString() & "-" & fechaMA.ToString() & "-" & ultimodiaA
        Select Case tipo
            Case "1014"
                Query = "SELECT G.NomGenero, "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Nuevos(G.IdGenero, 10, 14,'" & fechaI & "','" & fechaF & "')),0) AS 'Nuevos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Reinicio(G.IdGenero, 10, 14,'" & fechaI & "','" & fechaF & "')),0) AS 'Reingresos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Abandono(G.IdGenero, 10, 14,'" & fechaI & "','" & fechaF & "')),0) AS 'Abandonos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Fallecidos(G.IdGenero, 10, 14,'" & fechaI & "','" & fechaF & "')),0) AS 'Fallecidos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Traslados(G.IdGenero, 10, 14,'" & fechaI & "','" & fechaF & "')),0) AS 'Traslados', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_SCambioEdad(G.IdGenero, 14,'" & fechaAnterior & "','" & fechaF & "')),0) AS 'SalenCambioEdad', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_ECambioEdad(G.IdGenero, 10,'" & fechaAnterior & "','" & fechaF & "')),0) AS 'EntranCambioEdad', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_InicianTARV(G.IdGenero, 10, 14,'" & fechaI & "','" & fechaF & "')),0) AS 'InicianTARV' "
                Query += "FROM PAC_M_GENERO AS G"
            Case "1524"
                Query = "SELECT G.NomGenero, "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Nuevos(G.IdGenero, 15, 24,'" & fechaI & "','" & fechaF & "')),0) AS 'Nuevos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Reinicio(G.IdGenero, 15, 24,'" & fechaI & "','" & fechaF & "')),0) AS 'Reingresos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Abandono(G.IdGenero, 15, 24,'" & fechaI & "','" & fechaF & "')),0) AS 'Abandonos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Fallecidos(G.IdGenero, 15, 24,'" & fechaI & "','" & fechaF & "')),0) AS 'Fallecidos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Traslados(G.IdGenero, 15, 24,'" & fechaI & "','" & fechaF & "')),0) AS 'Traslados', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_SCambioEdad(G.IdGenero, 24,'" & fechaAnterior & "','" & fechaF & "')),0) AS 'SalenCambioEdad', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_ECambioEdad(G.IdGenero, 15,'" & fechaAnterior & "','" & fechaF & "')),0) AS 'EntranCambioEdad', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_InicianTARV(G.IdGenero, 15, 24,'" & fechaI & "','" & fechaF & "')),0) AS 'InicianTARV' "
                Query += "FROM PAC_M_GENERO AS G"
            Case "2549"
                Query = "SELECT G.NomGenero, "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Nuevos(G.IdGenero, 25, 49,'" & fechaI & "','" & fechaF & "')),0) AS 'Nuevos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Reinicio(G.IdGenero, 25, 49,'" & fechaI & "','" & fechaF & "')),0) AS 'Reingresos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Abandono(G.IdGenero, 25, 49,'" & fechaI & "','" & fechaF & "')),0) AS 'Abandonos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Fallecidos(G.IdGenero, 25, 49,'" & fechaI & "','" & fechaF & "')),0) AS 'Fallecidos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Traslados(G.IdGenero, 25, 49,'" & fechaI & "','" & fechaF & "')),0) AS 'Traslados', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_SCambioEdad(G.IdGenero, 49,'" & fechaAnterior & "','" & fechaF & "')),0) AS 'SalenCambioEdad', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_ECambioEdad(G.IdGenero, 25,'" & fechaAnterior & "','" & fechaF & "')),0) AS 'EntranCambioEdad', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_InicianTARV(G.IdGenero, 25, 49,'" & fechaI & "','" & fechaF & "')),0) AS 'InicianTARV' "
                Query += "FROM PAC_M_GENERO AS G"
            Case "50"
                Query = "SELECT G.NomGenero, "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Nuevos(G.IdGenero, 50, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'Nuevos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Reinicio(G.IdGenero, 50, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'Reingresos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Abandono(G.IdGenero, 50, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'Abandonos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Fallecidos(G.IdGenero, 50, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'Fallecidos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Traslados(G.IdGenero, 50, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'Traslados', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_SCambioEdad(G.IdGenero, 100,'" & fechaAnterior & "','" & fechaF & "')),0) AS 'SalenCambioEdad', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_ECambioEdad(G.IdGenero, 50,'" & fechaAnterior & "','" & fechaF & "')),0) AS 'EntranCambioEdad', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_InicianTARV(G.IdGenero, 50, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'InicianTARV' "
                Query += "FROM PAC_M_GENERO AS G"
            Case "Total"
                Query = "SELECT G.NomGenero, "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Nuevos(G.IdGenero, 10, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'Nuevos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Reinicio(G.IdGenero, 10, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'Reingresos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Abandono(G.IdGenero, 10, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'Abandonos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Fallecidos(G.IdGenero, 10, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'Fallecidos', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_Traslados(G.IdGenero, 10, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'Traslados', "
                Query += "ISNULL((SELECT * FROM dbo.fn_RepNARV_InicianTARV(G.IdGenero, 10, 100,'" & fechaI & "','" & fechaF & "')),0) AS 'InicianTARV' "
                Query += "FROM PAC_M_GENERO AS G"
        End Select
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & tipo
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ReporteConsultasPX(ByVal fechaA As String, ByVal fechaM As String, ByVal ultimodia As String, ByVal usuario As String) As DataTable
        _page = "db.ReporteConsultasPX"
        Dim Query As String = String.Empty
        Dim fechaI As String = "01/" & fechaM.ToString().PadLeft(2, "0") & "/" & fechaA.ToString()
        Dim fechaF As String = ultimodia & "/" & fechaM.ToString().PadLeft(2, "0") & "/" & fechaA.ToString()
        Dim fechaT As String = fechaM.ToString().PadLeft(2, "0") & "/" & fechaA.ToString().Substring(2, 2)
        Query = String.Format("SELECT * FROM fn_TotalConsultasPacienteXMES2('{0}','{1}','{2}')", fechaT, fechaI, fechaF)
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & fechaT
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ListaConsultasPX(ByVal fechaA As String, ByVal fechaM As String, ByVal ultimodia As String, ByVal usuario As String) As DataTable
        _page = "db.ListaConsultasPX"
        Dim Query As String = String.Empty
        Dim fechaI As String = "01/" & fechaM.ToString().PadLeft(2, "0") & "/" & fechaA.ToString()
        Dim fechaF As String = ultimodia & "/" & fechaM.ToString().PadLeft(2, "0") & "/" & fechaA.ToString()
        Query = String.Format("SELECT NHC, Cohorte, CONVERT(VARCHAR,FechaVisita,103) AS 'FechaVisita', Edad, IdGenero, Embarazo, ARV FROM fn_ConsultaPacienteXFechas2('{0}','{1}') ORDER BY Cohorte, NHC ASC", fechaI, fechaF)
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ReporteCambioEdad(ByVal fechaI As String, ByVal fechaF As String, ByVal usuario As String) As DataTable
        _page = "db.ReporteCambioEdad"
        Dim Q As New StringBuilder()
        Q.Append("SELECT Z.* FROM ")
        Q.Append("(SELECT A.NHC, LTRIM(RTRIM(C.PrimerNombre)) + (CASE WHEN C.SegundoNombre IS NULL THEN '' WHEN C.SegundoNombre = 'SSN' THEN '' ELSE ' ' ")
        Q.Append("+ LTRIM(RTRIM(C.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(C.PrimerApellido)) ")
        Q.Append("+ (CASE WHEN C.SegundoApellido IS NULL THEN '' WHEN C.SegundoApellido = 'SSA' THEN '' ELSE ' ' ")
        Q.Append("+ LTRIM(RTRIM(C.SegundoApellido)) END) AS 'Paciente', ")
        Q.Append("C.IdGenero, CONVERT(DATE, C.FechaNacimiento) AS 'FechaNacimiento', ")
        Q.Append("dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaI & "')) AS 'EdadAnterior', ")
        Q.Append("dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad' ")
        Q.Append("FROM ControlARV AS A INNER JOIN ")
        Q.Append("PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN ")
        Q.Append("PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente ")
        Q.Append("WHERE A.NHC NOT LIKE '%P%' AND A.Med1_ARVEstatus NOT IN (6, 7, 12, 21) ")
        Q.Append("AND A.IdCCARV = (SELECT TOP(1) X.IdCCARV FROM ControlARV AS X ")
        Q.Append("WHERE X.NHC = A.NHC AND X.FechaEntrega = (SELECT TOP(1) Y.FechaEntrega FROM ControlARV AS Y ")
        Q.Append("WHERE Y.NHC = X.NHC AND Y.FechaEntrega <= '" & fechaF & " 23:59:59.999' ORDER BY Y.FechaEntrega DESC) ")
        Q.Append("ORDER BY X.IdCCARV DESC)) AS Z ")
        Q.Append("WHERE Z.EdadAnterior <> Z.Edad AND Z.EdadAnterior IN (14,24,49)")
        Dim Query As String = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function RPxActivosTMP(ByVal fechaA As String, ByVal fechaM As String, ByVal ultimodia As String, ByVal usuario As String) As DataTable
        _page = "db.RPxActivosTMP"
        Dim Query As String = String.Empty
        Dim fechaI As String = fechaA.ToString() & "-" & fechaM.ToString() & "-01"
        Dim fechaF As String = fechaA.ToString() & "-" & fechaM.ToString() & "-" & ultimodia
        Dim fecha As String = fechaA.ToString() & "-" & fechaM.ToString() & "-"
        Dim diafin As Integer = CInt(ultimodia)
        Query = "SELECT * FROM dbo.fn_ListaPXActivosTMP('" & fechaF & "') ORDER BY NHC"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function RPActivosMes(ByVal fechaA As String, ByVal fechaM As String, ByVal ultimodia As String, ByVal usuario As String) As DataTable
        _page = "db.RPActivosMes"
        Dim fechaI As String = fechaA.ToString() & "-" & fechaM.ToString() & "-01"
        Dim fechaF As String = fechaA.ToString() & "-" & fechaM.ToString() & "-" & ultimodia
        Dim fecha As String = fechaA.ToString() & "-" & fechaM.ToString() & "-"
        Dim diafin As Integer = CInt(ultimodia)
        Dim Q As New StringBuilder()
        Q.Append("SELECT A.NHC, LTRIM(RTRIM(C.PrimerNombre)) + (CASE WHEN C.SegundoNombre IS NULL THEN '' WHEN C.SegundoNombre = 'SSN' THEN '' ELSE ' ' ")
        Q.Append("+ LTRIM(RTRIM(C.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(C.PrimerApellido)) ")
        Q.Append("+ (CASE WHEN C.SegundoApellido IS NULL THEN '' WHEN C.SegundoApellido = 'SSA' THEN '' ELSE ' ' ")
        Q.Append("+ LTRIM(RTRIM(C.SegundoApellido)) END) AS 'Paciente', ")
        Q.Append("C.IdGenero, dbo.fn_ObtieneEdad(CONVERT(DATE, C.FechaNacimiento), CONVERT(DATE, '" & fechaF & "')) AS 'Edad', A.Embarazo, ")
        Q.Append("CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', CONVERT(VARCHAR,A.FechaRetorno, 103) AS 'FechaRetorno', A.IdEsquema, ")
        Q.Append("A.IdSEsquema, A.EsquemaEstatus, ")
        Q.Append("dbo.fn_ObtieneMed(A.Med1_Codigo) AS 'M1', A.Med1_ARVEstatus AS 'ME1', dbo.fn_ObtieneMed(A.Med2_Codigo) AS 'M2', A.Med2_ARVEstatus AS 'ME2', ")
        Q.Append("dbo.fn_ObtieneMed(A.Med3_Codigo) AS 'M3', A.Med3_ARVEstatus AS 'ME3', dbo.fn_ObtieneMed(A.Med4_Codigo) AS 'M4', A.Med4_ARVEstatus AS 'ME4', ")
        Q.Append("dbo.fn_ObtieneMed(A.Med5_Codigo) AS 'M5', A.Med5_ARVEstatus AS 'ME5', dbo.fn_ObtieneMed(A.Med6_Codigo) AS 'M6', A.Med6_ARVEstatus AS 'ME6', ")
        Q.Append("dbo.fn_ObtieneMed(A.Med7_Codigo) AS 'M7', A.Med7_ARVEstatus AS 'ME7', dbo.fn_ObtieneMed(A.Med8_Codigo) AS 'M8', A.Med8_ARVEstatus AS 'ME8' ")
        Q.Append("FROM ControlARV AS A INNER JOIN ")
        Q.Append("PAC_ID AS B ON A.NHC = B.NHC LEFT OUTER JOIN ")
        Q.Append("PAC_BASALES AS C ON C.IdPaciente = B.IdPaciente ")
        Q.Append("WHERE A.NHC NOT LIKE '%P%' AND A.EsquemaEstatus NOT IN (6, 7, 12, 21) ")
        Q.Append("AND A.IdCCARV = (SELECT TOP(1) X.IdCCARV FROM ControlARV AS X ")
        Q.Append("WHERE X.NHC = A.NHC AND X.FechaEntrega = (SELECT TOP(1) Y.FechaEntrega FROM ControlARV AS Y ")
        Q.Append("WHERE Y.NHC = X.NHC AND Y.FechaEntrega <= '" & fechaF & " 23:59:59.999' ORDER BY Y.FechaEntrega DESC) ")
        Q.Append("ORDER BY X.IdCCARV DESC)")
        Dim Query As String = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & fechaF
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Lista Pacientes*//
    Public Function ListaPacientes(ByVal tipo As String, ByVal estatus As String, ByVal usuario As String) As DataTable
        _page = "db.ListaPacientes"
        Dim Query As String = ""
        If tipo = "A" Then
            Select Case estatus
                Case Is = "AC"
                    'todos los pacientes adultos activos
                    Query = "SELECT NHC, Cohorte, Paciente, UltimaFechaEntrega, FechaRetorno, Dias FROM v_ListaPacientes WHERE TIPO = 'A' AND ME NOT IN (6, 7, 12, 21)"
                Case Is = "PP"
                    'todos los pacientes Post-parto
                    Query = "SELECT NHC, Cohorte, Paciente, UltimaFechaEntrega, FechaRetorno, Dias FROM v_ListaPacientes WHERE TIPO = 'A' AND ME = 21"
                Case Is = "AB"
                    'todos los pacientes adultos abandonos
                    Query = "SELECT NHC, Cohorte, Paciente, UltimaFechaEntrega, FechaRetorno, Dias FROM v_ListaPacientes WHERE TIPO = 'A' AND ME = 6"
                Case Is = "TR"
                    'todos los pacientes adultos traslados		
                    Query = "SELECT NHC, Cohorte, Paciente, UltimaFechaEntrega, FechaRetorno, Dias FROM v_ListaPacientes WHERE TIPO = 'A' AND ME = 7"
                Case Is = "FA"
                    'todos los pacientes adultos fallecidos		
                    Query = "SELECT NHC, Cohorte, Paciente, UltimaFechaEntrega, FechaRetorno, Dias FROM v_ListaPacientes WHERE TIPO = 'A' AND ME = 12"
            End Select
        ElseIf tipo = "P" Then
            Select Case estatus
                Case Is = "AC"
                    'todos los pacientes pediatria activos	
                    Query = "SELECT NHC, Cohorte, Paciente, UltimaFechaEntrega, FechaRetorno, Dias FROM v_ListaPacientes WHERE TIPO = 'P' AND ME NOT IN (6, 7, 12)"
                Case Is = "AB"
                    'todos los pacientes pediatria abandonos		
                    Query = "SELECT NHC, Cohorte, Paciente, UltimaFechaEntrega, FechaRetorno, Dias FROM v_ListaPacientes WHERE TIPO = 'P' AND ME = 6"
                Case Is = "TR"
                    'todos los pacientes pediatria traslados		
                    Query = "SELECT NHC, Cohorte, Paciente, UltimaFechaEntrega, FechaRetorno, Dias FROM v_ListaPacientes WHERE TIPO = 'P' AND ME = 7"
                Case Is = "FA"
                    'todos los pacientes pediatria fallecidos		
                    Query = "SELECT NHC, Cohorte, Paciente, UltimaFechaEntrega, FechaRetorno, Dias FROM v_ListaPacientes WHERE TIPO = 'P' AND ME = 12"
            End Select
        End If

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function RegistrosPacienteARV(ByVal nhc As String, ByVal usuario As String) As DataTable
        _page = "db.RegistrosPacienteARV"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Q.Append("SELECT A.NHC,A.IdCCARV,CONVERT(VARCHAR,A.FechaEntrega,103) AS 'FechaEntrega',CONVERT(VARCHAR,A.FechaRetorno,103) AS 'FechaRetorno', ")
        Q.Append("dbo.fn_ObtieneCodMed('A', A.Med1_Codigo) AS 'M1C',A.Med1_Cantidad AS 'M1N',dbo.fn_ObtieneEstatusMed(A.Med1_ARVEstatus) AS 'M1E', ")
        Q.Append("dbo.fn_ObtieneCodMed('A', A.Med2_Codigo) AS 'M2C',A.Med2_Cantidad AS 'M2N',dbo.fn_ObtieneEstatusMed(A.Med2_ARVEstatus) AS 'M2E', ")
        Q.Append("dbo.fn_ObtieneCodMed('A', A.Med3_Codigo) AS 'M3C',A.Med3_Cantidad AS 'M3N',dbo.fn_ObtieneEstatusMed(A.Med3_ARVEstatus) AS 'M3E', ")
        Q.Append("dbo.fn_ObtieneCodMed('A', A.Med4_Codigo) AS 'M4C',A.Med4_Cantidad AS 'M4N',dbo.fn_ObtieneEstatusMed(A.Med4_ARVEstatus) AS 'M4E', ")
        Q.Append("dbo.fn_ObtieneCodMed('A', A.Med5_Codigo) AS 'M5C',A.Med5_Cantidad AS 'M5N',dbo.fn_ObtieneEstatusMed(A.Med5_ARVEstatus) AS 'M5E', ")
        Q.Append("dbo.fn_ObtieneCodMed('A', A.Med6_Codigo) AS 'M6C',A.Med6_Cantidad AS 'M6N',dbo.fn_ObtieneEstatusMed(A.Med6_ARVEstatus) AS 'M6E', ")
        Q.Append("dbo.fn_ObtieneCodMed('A', A.Med7_Codigo) AS 'M7C',A.Med7_Cantidad AS 'M7N',dbo.fn_ObtieneEstatusMed(A.Med7_ARVEstatus) AS 'M7E', ")
        Q.Append("dbo.fn_ObtieneCodMed('A', A.Med8_Codigo) AS 'M8C',A.Med8_Cantidad AS 'M8N',dbo.fn_ObtieneEstatusMed(A.Med8_ARVEstatus) AS 'M8E', ")
        Q.Append("S.SCodigo, E.Descripcion, dbo.fn_ObtieneEstatusMed(A.EsquemaEstatus) AS 'EsquemaEstatus', A.TiempoTARV, A.Embarazo, A.CD4, A.CV ")
        Q.Append("FROM ControlARV AS A LEFT OUTER JOIN ")
        Q.Append("SubEsquemas AS S ON A.IdSEsquema = S.IdSEsquema LEFT OUTER JOIN ")
        Q.Append("Esquemas AS E ON A.IdEsquema = E.IdEsquema ")
        Q.Append("WHERE A.NHC = '" & nhc & "' ")
        Q.Append("ORDER BY A.FechaEntrega DESC, A.IdCCARV DESC")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ObtieneRegARV(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ObtieneRegARV"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT IdCCARV, FechaEntrega, IdEsquema, IdSEsquema, EsquemaEstatus, Med1_Codigo, Med1_Cantidad, Med1_Dosis, Med1_Frecuencia, Med1_UExCantidad, Med1_ARVEstatus, Med1_DevCantidad, ")
        Q.Append("Med2_Codigo, Med2_Cantidad, Med2_Dosis, Med2_Frecuencia, Med2_UExCantidad, Med2_ARVEstatus, Med2_DevCantidad, ")
        Q.Append("Med3_Codigo, Med3_Cantidad, Med3_Dosis, Med3_Frecuencia, Med3_UExCantidad, Med3_ARVEstatus, Med3_DevCantidad, ")
        Q.Append("Med4_Codigo, Med4_Cantidad, Med4_Dosis, Med4_Frecuencia, Med4_UExCantidad, Med4_ARVEstatus, Med4_DevCantidad, ")
        Q.Append("Med5_Codigo, Med5_Cantidad, Med5_Dosis, Med5_Frecuencia, Med5_UExCantidad, Med5_ARVEstatus, Med5_DevCantidad, ")
        Q.Append("Med6_Codigo, Med6_Cantidad, Med6_Dosis, Med6_Frecuencia, Med6_UExCantidad, Med6_ARVEstatus, Med6_DevCantidad, ")
        Q.Append("Med7_Codigo, Med7_Cantidad, Med7_Dosis, Med7_Frecuencia, Med7_UExCantidad, Med7_ARVEstatus, Med7_DevCantidad, ")
        Q.Append("Med8_Codigo, Med8_Cantidad, Med8_Dosis, Med8_Frecuencia, Med8_UExCantidad, Med8_ARVEstatus, Med8_DevCantidad, ")
        Q.Append("FechaRetorno, TiempoTARV, CitaMedica, CitaFarmacia, Embarazo, TiempoRetorno, Adherencia, CD4, CV, Observaciones ")
        Q.Append("FROM ControlARV ")
        Q.Append("WHERE IdCCARV = " & id)
        Query = Q.ToString()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdCCARV").ToString() + "|" + reader("FechaEntrega").ToString() + "|" + reader("Med1_Codigo").ToString() + "|" + reader("Med1_Cantidad").ToString() + "|" + reader("Med1_Dosis").ToString() + "|" + reader("Med1_Frecuencia").ToString() + "|" + reader("Med1_UExCantidad").ToString() + "|" + reader("Med1_ARVEstatus").ToString() + "|" + reader("Med1_DevCantidad").ToString() + "|" + reader("Med2_Codigo").ToString() + "|" + reader("Med2_Cantidad").ToString() + "|" + reader("Med2_Dosis").ToString() + "|" + reader("Med2_Frecuencia").ToString() + "|" + reader("Med2_UExCantidad").ToString() + "|" + reader("Med2_ARVEstatus").ToString() + "|" + reader("Med2_DevCantidad").ToString() + "|" + reader("Med3_Codigo").ToString() + "|" + reader("Med3_Cantidad").ToString() + "|" + reader("Med3_Dosis").ToString() + "|" + reader("Med3_Frecuencia").ToString() + "|" + reader("Med3_UExCantidad").ToString() + "|" + reader("Med3_ARVEstatus").ToString() + "|" + reader("Med3_DevCantidad").ToString() + "|" + reader("Med4_Codigo").ToString() + "|" + reader("Med4_Cantidad").ToString() + "|" + reader("Med4_Dosis").ToString() + "|" + reader("Med4_Frecuencia").ToString() + "|" + reader("Med4_UExCantidad").ToString() + "|" + reader("Med4_ARVEstatus").ToString() + "|" + reader("Med4_DevCantidad").ToString() + "|" + reader("Med5_Codigo").ToString() + "|" + reader("Med5_Cantidad").ToString() + "|" + reader("Med5_Dosis").ToString() + "|" + reader("Med5_Frecuencia").ToString() + "|" + reader("Med5_UExCantidad").ToString() + "|" + reader("Med5_ARVEstatus").ToString() + "|" + reader("Med5_DevCantidad").ToString() + "|" + reader("Med6_Codigo").ToString() + "|" + reader("Med6_Cantidad").ToString() + "|" + reader("Med6_Dosis").ToString() + "|" + reader("Med6_Frecuencia").ToString() + "|" + reader("Med6_UExCantidad").ToString() + "|" + reader("Med6_ARVEstatus").ToString() + "|" + reader("Med6_DevCantidad").ToString() + "|" + reader("Med7_Codigo").ToString() + "|" + reader("Med7_Cantidad").ToString() + "|" + reader("Med7_Dosis").ToString() + "|" + reader("Med7_Frecuencia").ToString() + "|" + reader("Med7_UExCantidad").ToString() + "|" + reader("Med7_ARVEstatus").ToString() + "|" + reader("Med7_DevCantidad").ToString() + "|" + reader("Med8_Codigo").ToString() + "|" + reader("Med8_Cantidad").ToString() + "|" + reader("Med8_Dosis").ToString() + "|" + reader("Med8_Frecuencia").ToString() + "|" + reader("Med8_UExCantidad").ToString() + "|" + reader("Med8_ARVEstatus").ToString() + "|" + reader("Med8_DevCantidad").ToString() + "|" + reader("FechaRetorno").ToString() + "|" + reader("TiempoTARV").ToString() + "|" + reader("CitaMedica").ToString() + "|" + reader("CitaFarmacia").ToString() + "|" + reader("Embarazo").ToString() + "|" + reader("TiempoRetorno").ToString() + "|" + reader("Adherencia").ToString() + "|" + reader("CD4").ToString() + "|" + reader("CV").ToString() + "|" + reader("Observaciones").ToString() + "|" + reader("IdEsquema").ToString() + "|" + reader("IdSEsquema").ToString() + "|" + reader("EsquemaEstatus").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function RegistrosPacienteProf(ByVal nhc As String, ByVal usuario As String) As DataTable
        _page = "db.RegistrosPacienteProf"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Q.Append("SELECT NHC,IdCCProf,CONVERT(DATE,FechaEntrega) AS 'FechaEntrega', CD4, ")
        Q.Append("dbo.fn_ObtieneCodMed('P', Prof1_Codigo) AS 'P1',Prof1_Cantidad AS 'P1C', Prof1_Tipo AS 'P1T', Prof1_TipoTratamiento AS 'P1TT', dbo.fn_ObtieneEstatusMed(Prof1_Estatus) AS 'P1E', ")
        Q.Append("dbo.fn_ObtieneCodMed('P', Prof2_Codigo) AS 'P2',Prof2_Cantidad AS 'P2C', Prof2_Tipo AS 'P2T', Prof2_TipoTratamiento AS 'P2TT', dbo.fn_ObtieneEstatusMed(Prof2_Estatus) AS 'P2E', ")
        Q.Append("dbo.fn_ObtieneCodMed('P', Prof3_Codigo) AS 'P3',Prof3_Cantidad AS 'P3C', Prof3_Tipo AS 'P3T', Prof3_TipoTratamiento AS 'P3TT', dbo.fn_ObtieneEstatusMed(Prof3_Estatus) AS 'P3E', ")
        Q.Append("dbo.fn_ObtieneCodMed('P', Prof4_Codigo) AS 'P4',Prof4_Cantidad AS 'P4C', Prof4_Tipo AS 'P4T', Prof4_TipoTratamiento AS 'P4TT', dbo.fn_ObtieneEstatusMed(Prof4_Estatus) AS 'P4E', ")
        Q.Append("dbo.fn_ObtieneCodMed('P', Prof5_Codigo) AS 'P5',Prof5_Cantidad AS 'P5C', Prof5_Tipo AS 'P5T', Prof5_TipoTratamiento AS 'P5TT', dbo.fn_ObtieneEstatusMed(Prof5_Estatus) AS 'P5E', ")
        Q.Append("dbo.fn_ObtieneCodMed('P', Prof6_Codigo) AS 'P6',Prof6_Cantidad AS 'P6C', Prof6_Tipo AS 'P6T', Prof6_TipoTratamiento AS 'P6TT', dbo.fn_ObtieneEstatusMed(Prof6_Estatus) AS 'P6E' ")
        Q.Append("FROM ControlProf ")
        Q.Append("WHERE NHC = '" & nhc & "' ")
        'Q.Append("ORDER BY CONVERT(DATE,FechaRetorno) DESC")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ObtieneRegProf(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ObtieneRegProf"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT C.IdCCPROF, C.NHC, C.FechaEntrega, C.TipoPaciente, ")
        Q.Append("C.Prof1_Codigo, dbo.fn_ObtieneNomProf(C.Prof1_Codigo) AS 'NomProf1', C.Prof1_Cantidad, C.Prof1_Dosis, C.Prof1_VIA, C.Prof1_Frecuencia, C.Prof1_Tipo, C.Prof1_TipoTratamiento, C.Prof1_Estatus, C.Prof1_TTMed, C.Prof1_Observaciones, ")
        Q.Append("C.Prof2_Codigo, dbo.fn_ObtieneNomProf(C.Prof2_Codigo) AS 'NomProf2', C.Prof2_Cantidad, C.Prof2_Dosis, C.Prof2_VIA, C.Prof2_Frecuencia, C.Prof2_Tipo, C.Prof2_TipoTratamiento, C.Prof2_Estatus, C.Prof2_TTMed, C.Prof2_Observaciones, ")
        Q.Append("C.Prof3_Codigo, dbo.fn_ObtieneNomProf(C.Prof3_Codigo) AS 'NomProf3', C.Prof3_Cantidad, C.Prof3_Dosis, C.Prof3_VIA, C.Prof3_Frecuencia, C.Prof3_Tipo, C.Prof3_TipoTratamiento, C.Prof3_Estatus, C.Prof3_TTMed, C.Prof3_Observaciones, ")
        Q.Append("C.Prof4_Codigo, dbo.fn_ObtieneNomProf(C.Prof4_Codigo) AS 'NomProf4', C.Prof4_Cantidad, C.Prof4_Dosis, C.Prof4_VIA, C.Prof4_Frecuencia, C.Prof4_Tipo, C.Prof4_TipoTratamiento, C.Prof4_Estatus, C.Prof4_TTMed, C.Prof4_Observaciones, ")
        Q.Append("C.Prof5_Codigo, dbo.fn_ObtieneNomProf(C.Prof5_Codigo) AS 'NomProf5', C.Prof5_Cantidad, C.Prof5_Dosis, C.Prof5_VIA, C.Prof5_Frecuencia, C.Prof5_Tipo, C.Prof5_TipoTratamiento, C.Prof5_Estatus, C.Prof5_TTMed, C.Prof5_Observaciones, ")
        Q.Append("C.Prof6_Codigo, dbo.fn_ObtieneNomProf(C.Prof6_Codigo) AS 'NomProf6', C.Prof6_Cantidad, C.Prof6_Dosis, C.Prof6_VIA, C.Prof6_Frecuencia, C.Prof6_Tipo, C.Prof6_TipoTratamiento, C.Prof6_Estatus, C.Prof6_TTMed, C.Prof6_Observaciones, ")
        Q.Append("C.CD4 ")
        Q.Append("FROM ControlProf AS C ")
        Q.Append("WHERE C.IdCCProf = " & id)
        Query = Q.ToString()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdCCPROF").ToString() + "|" + reader("FechaEntrega").ToString() + "|" + reader("TipoPaciente").ToString() + "|"
                        Str += reader("Prof1_Codigo").ToString() + "|" + reader("NomProf1").ToString() + "|" + reader("Prof1_Cantidad").ToString() + "|" + reader("Prof1_Dosis").ToString() + "|" + reader("Prof1_VIA").ToString() + "|" + reader("Prof1_Frecuencia").ToString() + "|" + reader("Prof1_Tipo").ToString() + "|" + reader("Prof1_TipoTratamiento").ToString() + "|" + reader("Prof1_Estatus").ToString() + "|" + reader("Prof1_TTMed").ToString() + "|" + reader("Prof1_Observaciones").ToString() + "|"
                        Str += reader("Prof2_Codigo").ToString() + "|" + reader("NomProf2").ToString() + "|" + reader("Prof2_Cantidad").ToString() + "|" + reader("Prof2_Dosis").ToString() + "|" + reader("Prof2_VIA").ToString() + "|" + reader("Prof2_Frecuencia").ToString() + "|" + reader("Prof2_Tipo").ToString() + "|" + reader("Prof2_TipoTratamiento").ToString() + "|" + reader("Prof2_Estatus").ToString() + "|" + reader("Prof2_TTMed").ToString() + "|" + reader("Prof2_Observaciones").ToString() + "|"
                        Str += reader("Prof3_Codigo").ToString() + "|" + reader("NomProf3").ToString() + "|" + reader("Prof3_Cantidad").ToString() + "|" + reader("Prof3_Dosis").ToString() + "|" + reader("Prof3_VIA").ToString() + "|" + reader("Prof3_Frecuencia").ToString() + "|" + reader("Prof3_Tipo").ToString() + "|" + reader("Prof3_TipoTratamiento").ToString() + "|" + reader("Prof3_Estatus").ToString() + "|" + reader("Prof3_TTMed").ToString() + "|" + reader("Prof3_Observaciones").ToString() + "|"
                        Str += reader("Prof4_Codigo").ToString() + "|" + reader("NomProf4").ToString() + "|" + reader("Prof4_Cantidad").ToString() + "|" + reader("Prof4_Dosis").ToString() + "|" + reader("Prof4_VIA").ToString() + "|" + reader("Prof4_Frecuencia").ToString() + "|" + reader("Prof4_Tipo").ToString() + "|" + reader("Prof4_TipoTratamiento").ToString() + "|" + reader("Prof4_Estatus").ToString() + "|" + reader("Prof4_TTMed").ToString() + "|" + reader("Prof4_Observaciones").ToString() + "|"
                        Str += reader("Prof5_Codigo").ToString() + "|" + reader("NomProf5").ToString() + "|" + reader("Prof5_Cantidad").ToString() + "|" + reader("Prof5_Dosis").ToString() + "|" + reader("Prof5_VIA").ToString() + "|" + reader("Prof5_Frecuencia").ToString() + "|" + reader("Prof5_Tipo").ToString() + "|" + reader("Prof5_TipoTratamiento").ToString() + "|" + reader("Prof5_Estatus").ToString() + "|" + reader("Prof5_TTMed").ToString() + "|" + reader("Prof5_Observaciones").ToString() + "|"
                        Str += reader("Prof6_Codigo").ToString() + "|" + reader("NomProf6").ToString() + "|" + reader("Prof6_Cantidad").ToString() + "|" + reader("Prof6_Dosis").ToString() + "|" + reader("Prof6_VIA").ToString() + "|" + reader("Prof6_Frecuencia").ToString() + "|" + reader("Prof6_Tipo").ToString() + "|" + reader("Prof6_TipoTratamiento").ToString() + "|" + reader("Prof6_Estatus").ToString() + "|" + reader("Prof6_TTMed").ToString() + "|" + reader("Prof6_Observaciones").ToString() + "|"
                        Str += reader("CD4").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function ConsultaRegistroARV(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ConsultaRegistroARV"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT A.IdCCARV, CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', ")
        Q.Append("(CASE WHEN A.Med1_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med1_Codigo) ELSE '' END) AS 'Med1_Codigo', ")
        Q.Append("(CASE WHEN A.Med2_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med2_Codigo) ELSE '' END) AS 'Med2_Codigo', ")
        Q.Append("(CASE WHEN A.Med3_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med3_Codigo) ELSE '' END) AS 'Med3_Codigo', ")
        Q.Append("(CASE WHEN A.Med4_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med4_Codigo) ELSE '' END) AS 'Med4_Codigo', ")
        Q.Append("(CASE WHEN A.Med5_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med5_Codigo) ELSE '' END) AS 'Med5_Codigo', ")
        Q.Append("(CASE WHEN A.Med6_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med6_Codigo) ELSE '' END) AS 'Med6_Codigo', ")
        Q.Append("(CASE WHEN A.Med7_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med7_Codigo) ELSE '' END) AS 'Med7_Codigo', ")
        Q.Append("(CASE WHEN A.Med8_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med8_Codigo) ELSE '' END) AS 'Med8_Codigo' ")
        Q.Append("FROM ControlARV AS A ")
        Q.Append("WHERE A.IdCCARV = " & id)
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdCCARV").ToString() + "|" + reader("FechaEntrega").ToString() + "|" + reader("Med1_Codigo").ToString() + "|" + reader("Med2_Codigo").ToString() + "|" + reader("Med3_Codigo").ToString() + "|" + reader("Med4_Codigo").ToString() + "|" + reader("Med5_Codigo").ToString() + "|" + reader("Med6_Codigo").ToString() + "|" + reader("Med7_Codigo").ToString() + "|" + reader("Med8_Codigo").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe última fecha."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function Esquemas(ByVal usuario As String) As DataTable
        _page = "db.Esquemas"
        'Dim Query As String = "SELECT IdEsquema, Descripcion, Codigos FROM Esquemas ORDER BY IdEsquema"
        Dim Query As String = "SELECT E.IdEsquema, E.Descripcion, E.Codigos, (SELECT COUNT(*) FROM SubEsquemas AS S WHERE S.IdEsquema = E.IdEsquema) AS 'SubEsquemas' FROM Esquemas AS E ORDER BY E.IdEsquema"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ListaEsquemas(ByVal usuario As String) As DataTable
        _page = "db.ListaEsquemas"
        Dim Query As String = "SELECT IdEsquema FROM Esquemas ORDER BY IdEsquema"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ListaSEsquemas(ByVal idse As String, ByVal usuario As String) As DataTable
        _page = "db.ListaSEsquemas"
        Dim Query As String = String.Format("SELECT IdSEsquema, SCodigo FROM dbo.SubEsquemas WHERE IdEsquema = {0} ORDER BY SCodigo ASC", idse)
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ObtieneEsquema(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ObtieneEsquema"
        Dim Query As String = String.Format("SELECT Descripcion FROM Esquemas WHERE IdEsquema = {0}", id)
        Dim Str As String = ""
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = reader("Descripcion").ToString()
                        Exit While
                    End While
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Str = String.Empty
        End Try
        Return Str
    End Function

    Public Function SEsquemas(ByVal id As String, ByVal usuario As String) As DataTable
        _page = "db.SEsquemas"
        Dim Query As String = String.Format("SELECT IdSEsquema, SCodigo, Descripcion, Codigos FROM SubEsquemas WHERE IdEsquema = {0} ORDER BY IdSEsquema ASC", id)
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Basales Paciente*//


    Public Function ObtieneGrupoFamiliar(ByVal IdBasalesTS As String, ByVal usuario As String) As DataTable
        _page = "db.ObtieneGrupoFamiliar"
        Dim Query As String
        Query = "SELECT ROW_NUMBER() OVER (ORDER BY IdGrupoFamiliar) as IdGrupoFamiliar_Indice, IdGrupoFamiliar, Nombre, dbo.fn_ObtieneTipoRelacion(IdTipoRelacion) AS 'TipoRelacion', "
        Query += "Edad, dbo.fn_ObtieneNivelEducativo(IdNivelEducativo) AS 'NivelEducativo', "
        Query += "dbo.fn_ObtieneSituacionLaboral(IdSituacionLaboral) AS 'SituacionLaboral', Ingreso, "
        Query += "dbo.fn_ObtieneConoceDx(ConoceDx) AS 'NomConoceDx' "
        Query += String.Format("FROM dbo.PAC_GRUPOFAMILIAR WHERE IdBasalesTS = {0} AND Estatus = 1", IdBasalesTS)
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function listasGrupoFamiliar(ByVal op As Integer, ByVal usuario As String) As DataTable
        _page = "db.listasGrupoFamiliar"
        Dim Query As String = String.Empty
        Select Case op
            Case 1 'tipo relacion
                Query = "SELECT IdTipoRelacion, NomTipoRelacion FROM dbo.PAC_TIPO_RELACION ORDER BY IdTipoRelacion ASC"
            Case 2 'nivel educativo
                Query = "SELECT IdNivelEducativo, NomNivelEducativo FROM dbo.PAC_NIVELEDUCATIVO ORDER BY IdNivelEducativo ASC"
            Case 3 'situacion laboral
                Query = "SELECT IdSituacionLaboral, NomSituacionLaboral FROM dbo.PAC_SITUACIONLABORAL ORDER BY IdSituacionLaboral ASC"
            Case 4 'conoce dx
                Query = "SELECT IdConoceDx, NomConoceDx FROM dbo.PAC_CONOCEDX ORDER BY IdConoceDx ASC"
            Case 5 'quienes conocen dx
                Query = "SELECT IdPersonasDx, NomPersonaDX FROM PAC_PERSONASDX ORDER BY IdPersonasDx ASC"
            Case 6 'tipo de vivienda
                Query = "SELECT IdTipoVivienda, NomTipoVivienda FROM PAC_TIPO_VIVIENDA ORDER BY IdTipoVivienda ASC"
            Case 7 'servicios
                Query = "SELECT IdServicios, NomServicios FROM PAC_SERVICIOS ORDER BY IdServicios ASC"
            Case 8 'tipo de construccion
                Query = "SELECT IdTipoConstruccion, NomTipoConstruccion FROM PAC_TIPOCONSTRUCCION ORDER BY IdTipoConstruccion ASC"
            Case 9 'problemas identificados
                Query = "SELECT IdProbIdentificados, NomProbIdentificados FROM PAC_PROBIDENTIFICADOS ORDER BY IdProbIdentificados ASC"
        End Select
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function listasProximaCitaTS(ByVal op As Integer, ByVal usuario As String) As DataTable
        _page = "db.listasProximaCitaTS"
        Dim Query As String = String.Empty
        Select Case op
            Case 1 'Jornada
                Query = "SELECT IdJornada, Jornada FROM dbo.PAC_JORNADA ORDER BY IdJornada ASC"
            Case 2 'Clinica
                Query = "SELECT IdClinica, Clinica FROM dbo.PAC_CLINICA ORDER BY IdClinica ASC"
        End Select
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ExisteNHC(ByVal nhc As String, ByVal usuario As String) As Integer
        _page = "db.ExisteNHC"
        Dim Query As String = "SELECT COUNT(*) AS 'Existe' FROM PAC_BASALES_TS WHERE NHC ='" & nhc & "'"
        Dim valor As Integer
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        valor = reader("Existe")
                        Exit While
                    End While
                End If
                'If Str() = String.Empty Then
                'Str = "False|No se Encontró Información."
                'End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            'Str = "False|" + ex.Message
        End Try
        Return valor
    End Function

    Public Function ExisteNHC_circuito(ByVal nhc As String, ByVal usuario As String) As Integer
        _page = "db.ExisteNHC_circuito"
        Dim Query As String = "SELECT COUNT(*) AS 'Existe' FROM PAC_CIRCUITO_ADHERENCIA WHERE NHC ='" & nhc & "'"
        Dim valor As Integer
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        valor = reader("Existe")
                        Exit While
                    End While
                End If
                'If Str() = String.Empty Then
                'Str = "False|No se Encontró Información."
                'End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            'Str = "False|" + ex.Message
        End Try
        Return valor
    End Function




    Public Function InsertarGrupoFamiliar(ByVal idbts As String, ByVal idpaciente As String, ByVal nombre As String, ByVal idtiporelacion As String, ByVal edad As String, ByVal idniveleducativo As String, ByVal idsituacionlaboral As String, ByVal ingreso As String, ByVal conocedx As String, ByVal estatus As String, ByVal usuario As String) As String
        _page = "db.InsertarGrupoFamiliar"
        Dim X As String = Nothing
        Dim sql As String = "INSERT INTO PAC_GRUPOFAMILIAR (IdBasalesTS, IdPaciente, Nombre, IdTipoRelacion, Edad, IdNivelEducativo, IdSituacionLaboral, Ingreso, ConoceDx, Estatus, Usuario) "
        sql += String.Format("VALUES({0}, {1}, '{2}', {3}, {4}, {5}, {6}, '{7}', {8}, '{9}', '{10}')", idbts, idpaciente, nombre, idtiporelacion, edad, idniveleducativo, idsituacionlaboral, ingreso, conocedx, estatus, usuario)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
                X = "True|Grupo Familiar Ingresado Correctamente."
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = "False|" & ex.Message
        End Try
        Return X
    End Function

    Public Function ModificarGrupoFamiliar(ByVal idGrupoFamiliar As String, ByVal nombre As String, ByVal TipoRelacion As String, ByVal Edad As String, ByVal NivelEducativo As String, ByVal SituacionLaboral As String, ByVal Ingreso As String, ByVal ConoceDx As String, ByVal usuario As String) As String
        _page = "db.ModificarGrupoFamiliar"
        Dim X As String = Nothing
        Dim sql As String = String.Format("UPDATE PAC_GRUPOFAMILIAR SET Nombre = '{1}', IdTipoRelacion = {2}, Edad = {3}, IdNivelEducativo = {4}, IdSituacionLaboral = {5}, Ingreso = '{6}', ConoceDx = {7}, FechaModificacion = '{8}' WHERE IdGrupoFamiliar = {0} ", idGrupoFamiliar, nombre, TipoRelacion, Edad, NivelEducativo, SituacionLaboral, Ingreso, ConoceDx, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            X = "True|Grupo Familiar Modificado Correctamente."
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = "False|" & ex.Message
        End Try
        Return X
    End Function

    Public Function ContenidoGrupoFamiliar(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ContenidoGrupoFamiliar"
        Dim str As String = ""
        Dim Query As String = String.Format("SELECT Nombre, IdTipoRelacion, Edad, IdNivelEducativo, IdSituacionLaboral, Ingreso, ConoceDx FROM PAC_GRUPOFAMILIAR WHERE IdGrupoFamiliar = {0}", id)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        str = "True|" & Convert.ToString(reader("Nombre")) & "|" & Convert.ToString(reader("IdTipoRelacion")) & "|" & Convert.ToString(reader("Edad")) & "|" & Convert.ToString(reader("IdNivelEducativo")) & "|" & Convert.ToString(reader("IdSituacionLaboral")) & "|" & Convert.ToString(reader("Ingreso")) & "|" & Convert.ToString(reader("ConoceDx"))
                        Exit While
                    End While
                Else
                    str = "False|No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            str = "False|" & ex.Message
        End Try
        Return str
    End Function

    Public Function DesactivarGrupoFamiliar(ByVal id As String, ByVal usuario As String) As String
        _page = "db.DesactivarGrupoFamiliar"
        Dim X As String = Nothing
        Dim sql As String = String.Format("UPDATE PAC_GRUPOFAMILIAR SET Estatus = '{1}', Usuario = '{2}', FechaModificacion = '{3}' WHERE IdGrupoFamiliar = {0}", id, "0", usuario, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            X = "True|El Grupo Familiar fue Desactivado."
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = "False|" & ex.Message
        End Try
        Return X
    End Function

    Public Function ObtienePacientes(ByVal fecha As String, ByVal usuario As String) As DataTable
        _page = "db.ObtienePacientes"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Q.Append("SELECT ROW_NUMBER() OVER( ORDER BY X.Paciente ASC ) AS 'nro', X.Paciente, X.NHC, X.NomGenero AS Genero, X.Edad, X.TVisita, ")
        Q.Append("(SELECT DISTINCT 'V' FROM PAC_BASALES AS A1 INNER JOIN SIGNOSVITALES AS B1 ON A1.IdPaciente = B1.IdPaciente ")
        Q.Append("WHERE A1.IdPaciente = X.IdPaciente AND CONVERT(VARCHAR, B1.FechaVisita, 103) = '" & fecha & "') AS Visita ")
        Q.Append("FROM ")
        Q.Append("(SELECT DISTINCT A.IdPaciente, (LTRIM(RTRIM(A.PrimerNombre)) + (CASE WHEN A.SegundoNombre IS NULL THEN '' ELSE ' ' + LTRIM(RTRIM(A.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(A.PrimerApellido)) + (CASE WHEN A.SegundoApellido IS NULL THEN '' ELSE ' ' + LTRIM(RTRIM(A.SegundoApellido)) END)) AS Paciente, ")
        Q.Append("B.NHC, C.NomGenero, dbo.fn_ObtieneEdad2(A.FechaNacimiento, GETDATE()) AS Edad, ")
        Q.Append("(SELECT TOP 1 T.NomTipoVisita FROM SIGNOSVITALES AS D1 LEFT OUTER JOIN SIGNOSVIT_M_TIPOVISITA AS T ON D1.TipoVisita = T.IdTipoVisita WHERE (D1.IdPaciente = A.IdPaciente) AND (CONVERT(VARCHAR, D1.FechaVisita, 103) = '" & fecha & "')) AS 'TVisita' ")
        Q.Append("FROM PAC_BASALES AS A LEFT OUTER JOIN ")
        Q.Append("PAC_ID AS B ON A.IdPaciente = B.IdPaciente LEFT OUTER JOIN ")
        Q.Append("PAC_M_GENERO AS C ON A.IdGenero = C.IdGenero LEFT OUTER JOIN ")
        Q.Append("SIGNOSVITALES AS D ON B.IdPaciente = D.IdPaciente LEFT OUTER JOIN ")
        Q.Append("SIGNOSVIT_M_TIPOVISITA AS T ON D.TipoVisita = T.IdTipoVisita ")
        Q.Append("WHERE CONVERT(VARCHAR, D.FechaProximaVisita, 103) = '" & fecha & "' OR (CONVERT(VARCHAR, D.FechaVisita, 103) = '" & fecha & "')) AS X ")
        Q.Append("ORDER BY X.Paciente")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & fecha
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message & "|" & Query)
            Return Nothing
        End Try
    End Function

    Public Function InsertarFechaProximaVisita(ByVal t As String, ByVal idsignosvitales As String, ByVal fechaproximavisita As String, ByVal idpaciente As String, ByVal iusuario As Integer, ByVal usuario As String) As String
        _page = "db.InsertarFechaProximaVisita"
        Dim X As String = Nothing
        Dim sql As String = String.Format("UPDATE SIGNOSVITALES SET FechaProximaVisita = CONVERT(DATETIME,'{1}') WHERE IdSignosVitales = {0}", idsignosvitales, fechaproximavisita)
        Dim isql As String = String.Format("INSERT INTO X_Log (IdUsuario, IdPaciente, Tabla, Id, Fecha, Accion) VALUES ({0}, {1}, 'Signosvitale', {2}, GETDATE(), 'Update')", iusuario, idpaciente, idsignosvitales)
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                Dim command2 As New SqlCommand(isql, connection)
                command2.ExecuteNonQuery()
                command2.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Select Case t
                Case "1"
                    X = "True|MANGUA: Fecha Próxima Visita Ingresada."
                Case "2"
                    X = "True|MANGUA: Fecha Próxima Visita Modificada."
            End Select
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & idsignosvitales
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = "False|" & ex.Message
        End Try
        Return X
    End Function

    Public Function InsertarFechaProximaVisitaTS(ByVal idpaciente As String, ByVal idsignosvitales As String, ByVal fecha As String, ByVal jornada As String, ByVal clinica As String, ByVal usuario As String, ByVal horario As String) As String
        _page = "db.InsertarFechaProximaVisitaTS"
        Dim X As String = Nothing
        Dim sql As String = String.Format("INSERT INTO PAC_CITAS (IdSignosVitales, IdPaciente, FechaProximaVisita, Jornada, Clinica, Usuario, IdHorario) VALUES ({0}, {1}, Convert(date,'{2}',103), {3}, {4}, '{5}', {6})", idsignosvitales, idpaciente, fecha, jornada, clinica, usuario, horario)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            X = "True|TS_DB: Fecha Próxima Visita Ingresada."
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & idpaciente
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = "False|" & ex.Message
        End Try
        Return X
    End Function

    Public Function ActualizarFechaProximaVisitaTS(ByVal idsignosvitales As String, ByVal fecha As String, ByVal jornada As String, ByVal clinica As String, ByVal usuario As String, ByVal horario As String) As String
        _page = "db.ActualizarFechaProximaVisitaTS"
        Dim X As String = Nothing

        Dim sql As String = String.Format("UPDATE PAC_CITAS SET FechaProximaVisita = Convert(DateTime,'{1}') , Jornada = {2}, Clinica = {3}, Usuario = '{4}', FechaModificacion = '{5}', IdHorario = {6}  WHERE IdSignosVitales = {0}", idsignosvitales, fecha, jornada, clinica, usuario, DateTime.Now.ToString("yyyyMMdd HH:mm:ss"), horario)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            X = "True|TS_DB: Fecha Próxima Visita Modificada."
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & idsignosvitales
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = "False|" & ex.Message
        End Try
        Return X
    End Function

    Public Function ExisteFechaProximaVisitaTS(ByVal idsignosvitales As String, ByVal usuario As String) As Integer
        _page = "db.ExisteFechaProximaVisitaTS"
        Dim Query As String = "SELECT COUNT(*) AS 'Existe' FROM PAC_CITAS WHERE IdSignosVitales ='" & idsignosvitales & "'"
        Dim valor As Integer
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        valor = reader("Existe")
                        Exit While
                    End While
                End If
                'If Str() = String.Empty Then
                'Str = "False|No se Encontró Información."
                'End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & idsignosvitales
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            'Str = "False|" + ex.Message
        End Try
        Return valor
    End Function

    Public Function ObtieneFechaVisita(ByVal idpaciente As String, ByVal usuario As String) As String
        _page = "db.ObtieneFechaVisita"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT X.IdSignosVitales, X.IdPaciente, CONVERT(VARCHAR,X.FechaVisita,103) AS 'FechaVisita', ")
        Q.Append("CONVERT(VARCHAR,X.FechaProximaVisita,103) AS 'FechaProximaVisita' FROM ")
        Q.Append("(SELECT TOP 1 IdSignosVitales, IdPaciente, FechaVisita, FechaProximaVisita ")
        Q.Append("FROM SIGNOSVITALES ")
        Q.Append("WHERE idpaciente = " & idpaciente & " ")
        Q.Append("ORDER BY FechaVisita DESC) AS X ")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdSignosVitales").ToString() + "|" + reader("FechaVisita").ToString() + "|" + reader("FechaProximaVisita").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|MANGUA: No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & idpaciente
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function ObtieneFechaVisitaTS(ByVal idsignosvitales As String, ByVal usuario As String) As String
        _page = "db.ObtieneFechaVisita"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT IdCitas, FechaProximaVisita, Jornada, Clinica, IdHorario FROM dbo.PAC_CITAS WHERE IdSignosVitales = " & idsignosvitales)
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdCitas").ToString() + "|" + reader("FechaProximaVisita").ToString() + "|" + reader("Jornada").ToString() + "|" + reader("Clinica").ToString() + "|" + reader("IdHorario").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|TS_DB: No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & idsignosvitales
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function ObtieneBasales(ByVal nhc As String, ByVal usuario As String) As String
        _page = "db.ObtieneBasales"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT Z.IdPaciente, Z.NumHospitalaria, Z.Cedula, Z.NomGenero, Z.Paciente, Z.Telefono,Z.Movil, Z.FechaNacimiento, Z.Edad, Z.PaisNacimiento, Z.DeptoNacimiento, Z.MuniNacimiento, ")
        Q.Append("Z.PaisResidencia, Z.DeptoResidencia, Z.MuniResidencia, Z.Direccion, Z.EstadoCivil, Z.NivelEducativo, Z.AñosCompletos, Z.OrientacionSexual, Z.SituacionLaboral, ")
        Q.Append("(CASE WHEN LTRIM(RTRIM(Z.NomMotivoBaja)) IS NULL THEN 'Activo' ELSE LTRIM(RTRIM(Z.NomMotivoBaja)) END) AS 'MotivoBaja', Z.Etnia FROM ")
        Q.Append("(SELECT A.IdPaciente, B.NumHospitalaria, B.Cedula, dbo.fn_ObtieneGenero(B.IdGenero) AS 'NomGenero', LTRIM(RTRIM(B.PrimerNombre)) + (CASE WHEN B.SegundoNombre IS NULL ")
        Q.Append("THEN '' WHEN B.SegundoNombre = 'SSN' THEN '' ELSE ' ' + LTRIM(RTRIM(B.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(B.PrimerApellido)) ")
        Q.Append("+ (CASE WHEN B.SegundoApellido IS NULL THEN '' WHEN B.SegundoApellido = 'SSA' THEN '' ELSE ' ' + LTRIM(RTRIM(B.SegundoApellido)) END) AS 'Paciente', ")
        Q.Append("LTRIM(RTRIM(B.TelefonoFijo)) AS 'Telefono', ")
        Q.Append("LTRIM(RTRIM(B.TelefonoMovil)) AS 'Movil', ")
        Q.Append("CONVERT(VARCHAR,B.FechaNacimiento,103) AS 'FechaNacimiento', ")
        Q.Append("dbo.fn_ObtieneEdad(B.FechaNacimiento, GETDATE()) AS 'Edad', ")
        Q.Append("dbo.fn_ObtieneNomPais(B.PaisNacimiento) AS 'PaisNacimiento', dbo.fn_ObtieneNomDepto(B.DepartamentoNacimiento) AS 'DeptoNacimiento', ")
        Q.Append("dbo.fn_ObtieneNomMun(B.MunicipioNacimiento) AS 'MuniNacimiento', ")
        Q.Append("LTRIM(RTRIM(B.Direccion)) AS 'direccion', ")
        Q.Append("dbo.fn_ObtieneNomPais(B.PaisResidencia) AS 'PaisResidencia', dbo.fn_ObtieneNomDepto(B.DepartamentoResidencia) AS 'DeptoResidencia', ")
        Q.Append("dbo.fn_ObtieneNomMun(B.MunicipioResidencia) AS 'MuniResidencia', ")
        Q.Append("(SELECT TOP 1 dbo.fn_ObtieneEstadoCivil(S.EstadoCivil) FROM PAC_SOCIODEMOG AS S WHERE S.IdPaciente = A.IdPaciente ORDER BY S.FechaSociodemog ASC) AS 'EstadoCivil', ")
        Q.Append("(SELECT TOP 1 dbo.fn_ObtieneNivelEducativo(S.NivelEducativo) FROM PAC_SOCIODEMOG AS S WHERE S.IdPaciente = A.IdPaciente ORDER BY S.FechaSociodemog ASC) AS 'NivelEducativo', ")
        Q.Append("(SELECT TOP 1 S.AnosCompletos FROM PAC_SOCIODEMOG AS S WHERE S.IdPaciente = A.IdPaciente ORDER BY S.FechaSociodemog ASC) AS 'AñosCompletos', ")
        Q.Append("(SELECT TOP 1 dbo.fn_ObtieneOrientacion(V.OrientacionSexual) FROM PAC_VIDASEXUAL AS V WHERE V.IdPaciente = A.IdPaciente ORDER BY V.FechaVidaSexual ASC) AS 'OrientacionSexual', ")
        Q.Append("(SELECT TOP 1 dbo.fn_ObtieneSituacionLaboral(S.SituacionLaboral) FROM PAC_SOCIODEMOG AS S WHERE S.IdPaciente = A.IdPaciente ORDER BY S.FechaSociodemog ASC) AS 'SituacionLaboral', ")
        Q.Append("(SELECT TOP 1 N.NomMotivoBaja FROM PAC_BAJA AS M LEFT OUTER JOIN PAC_ID AS O ON M.IdPaciente = O.IdPaciente LEFT OUTER JOIN PAC_M_MOTIVOBAJA AS N ON M.MotivoBaja = N.IdMotivoBaja WHERE M.IdPaciente = A.IdPaciente AND O.Baja = 1 ORDER BY M.FechaBaja DESC) AS 'NomMotivoBaja', ")
        Q.Append("dbo.fn_ObtieneEtnia(B.Etnia) AS 'Etnia' ")
        Q.Append("FROM PAC_ID AS A LEFT OUTER JOIN ")
        Q.Append("PAC_BASALES AS B ON A.IdPaciente = B.IdPaciente ")
        Q.Append("WHERE (A.NHC = '" & nhc & "')) AS Z")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("NumHospitalaria").ToString() + "|" + reader("Cedula").ToString() + "|" + reader("NomGenero").ToString() + "|" + reader("Paciente").ToString() + "|" + reader("Telefono").ToString() + "|" + reader("Movil").ToString() + "|" + reader("FechaNacimiento").ToString() + "|" + reader("Edad").ToString() + "|" + reader("PaisNacimiento").ToString() + "|" + reader("DeptoNacimiento").ToString() + "|" + reader("MuniNacimiento").ToString() + "|" + reader("PaisResidencia").ToString() + "|" + reader("DeptoResidencia").ToString() + "|" + reader("MuniResidencia").ToString() + "|" + reader("Direccion").ToString() + "|" + reader("EstadoCivil").ToString() + "|" + reader("NivelEducativo").ToString() + "|" + reader("AñosCompletos").ToString() + "|" + reader("OrientacionSexual").ToString() + "|" + reader("SituacionLaboral").ToString() + "|" + reader("MotivoBaja").ToString() + "|" + reader("IdPaciente").ToString() + "|" + reader("Etnia").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function ObtieneBasales1(ByVal nhc As String, ByVal usuario As String) As String
        _page = "db.ObtieneBasales1"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT Z.NomGenero, Z.Paciente, Z.Telefono, Z.FechaNacimiento, Z.Edad, Z.Direccion, Z.IdPaciente, ")
        Q.Append("(CASE WHEN LTRIM(RTRIM(Z.NomMotivoBaja)) IS NULL THEN 'Activo' ELSE LTRIM(RTRIM(Z.NomMotivoBaja)) END) AS 'MotivoBaja', Z.Clasificación_Pac FROM ")
        Q.Append("(SELECT A.IdPaciente, C.NomGenero, LTRIM(RTRIM(B.PrimerNombre)) + (CASE WHEN B.SegundoNombre IS NULL ")
        Q.Append("THEN '' WHEN B.SegundoNombre = 'SSN' THEN '' ELSE ' ' + LTRIM(RTRIM(B.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(B.PrimerApellido)) ")
        Q.Append("+ (CASE WHEN B.SegundoApellido IS NULL THEN '' WHEN B.SegundoApellido = 'SSA' THEN '' ELSE ' ' + LTRIM(RTRIM(B.SegundoApellido)) END) AS 'Paciente', ")
        Q.Append("(CASE WHEN LTRIM(RTRIM(B.TelefonoFijo)) IS NOT NULL THEN LTRIM(RTRIM(B.TelefonoFijo)) ELSE '' END) + ")
        Q.Append("(CASE WHEN LTRIM(RTRIM(B.TelefonoMovil)) IS NOT NULL THEN ', ' + LTRIM(RTRIM(B.TelefonoMovil)) ELSE '' END) AS 'Telefono', ")
        Q.Append("CONVERT(VARCHAR,B.FechaNacimiento,103) AS 'FechaNacimiento', ")
        Q.Append("dbo.fn_ObtieneEdad(B.FechaNacimiento, GETDATE()) AS 'Edad', ")
        Q.Append("(CASE WHEN LTRIM(RTRIM(B.Direccion)) IS NOT NULL THEN LTRIM(RTRIM(B.Direccion)) ELSE '' END) + ")
        Q.Append("(CASE WHEN LTRIM(RTRIM(E.NomMunicipio)) IS NOT NULL THEN ', ' + LTRIM(RTRIM(E.NomMunicipio)) ELSE '' END) + ")
        Q.Append("(CASE WHEN LTRIM(RTRIM(F.NomDepartamento)) IS NOT NULL THEN ', ' + LTRIM(RTRIM(F.NomDepartamento)) ELSE '' END) + ")
        Q.Append("(CASE WHEN LTRIM(RTRIM(D.NomPais)) IS NOT NULL THEN ', ' + LTRIM(RTRIM(D.NomPais)) ELSE '' END) AS 'Direccion', ")
        Q.Append("(SELECT TOP 1 N.NomMotivoBaja FROM PAC_BAJA AS M LEFT OUTER JOIN PAC_ID AS O ON M.IdPaciente = O.IdPaciente LEFT OUTER JOIN PAC_M_MOTIVOBAJA AS N ON M.MotivoBaja = N.IdMotivoBaja WHERE M.IdPaciente = A.IdPaciente AND O.Baja = 1 ORDER BY M.FechaBaja DESC) AS 'NomMotivoBaja', ")
        Q.Append("(SELECT TOP 1 dbo.fn_ObtieneClasificacion_pac(PEP.Id_Clasificacion_Pac) FROM PSOEP AS PEP WHERE PEP.NHC = A.NHC AND PEP.Id_Clasificacion_Pac IS NOT NULL  ORDER BY PEP.FechaFicha DESC) AS 'Clasificación_Pac' ")
        Q.Append("FROM PAC_ID AS A LEFT OUTER JOIN ")
        'Q.Append("PAC_BAJA AS G ON A.IdPaciente = G.IdPaciente LEFT OUTER JOIN ")
        'Q.Append("PAC_M_MOTIVOBAJA AS H ON G.MotivoBaja = H.IdMotivoBaja LEFT OUTER JOIN ")
        Q.Append("PAC_BASALES AS B ON A.IdPaciente = B.IdPaciente LEFT OUTER JOIN ")
        Q.Append("PAC_M_GENERO AS C ON B.IdGenero = C.IdGenero LEFT OUTER JOIN ")
        Q.Append("PAC_M_PAIS AS D ON B.PaisNacimiento = D.IdPais AND B.PaisResidencia = D.IdPais LEFT OUTER JOIN ")
        Q.Append("M_MUNICIPIO AS E ON B.MunicipioNacimiento = E.IdMunicipio AND B.MunicipioResidencia = E.IdMunicipio LEFT OUTER JOIN ")
        Q.Append("M_DEPARTAMENTO AS F ON B.DepartamentoNacimiento = F.IdDepartamento AND ")
        Q.Append("B.DepartamentoResidencia = F.IdDepartamento AND E.Departamento = F.IdDepartamento AND ")
        Q.Append("E.Departamento = F.IdDepartamento ")
        Q.Append("WHERE (A.NHC = '" & nhc & "')) AS Z")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("NomGenero").ToString() + "|" + reader("Paciente").ToString() + "|" + reader("Telefono").ToString() + "|" + reader("FechaNacimiento").ToString() + "|" + reader("Edad").ToString() + "|" + reader("Direccion").ToString() + "|" + reader("MotivoBaja").ToString() + "|" + reader("IdPaciente").ToString() + "|" + reader("Clasificación_Pac").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function ObtieneBasalesP(ByVal nhc As String, ByVal usuario As String) As String
        _page = "db.ObtieneBasalesP"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT B.NomGenero, LTRIM(RTRIM(A.PrimerNombre)) + (CASE WHEN A.SegundoNombre IS NULL ")
        Q.Append("THEN '' WHEN A.SegundoNombre = 'SSN' THEN '' ELSE ' ' + LTRIM(RTRIM(A.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(A.PrimerApellido)) ")
        Q.Append("+ (CASE WHEN A.SegundoApellido IS NULL THEN '' WHEN A.SegundoApellido = 'SSA' THEN '' ELSE ' ' + LTRIM(RTRIM(A.SegundoApellido)) END) AS 'Paciente', ")
        'Q.Append("(CASE WHEN A.FechaNacimiento = '' THEN '' ELSE CONVERT(VARCHAR, CONVERT(DATE, (SUBSTRING(A.FechaNacimiento,4,2)+'/'+SUBSTRING(A.FechaNacimiento,1,2)+'/'+SUBSTRING(A.FechaNacimiento,7,4))),103) END) AS 'FechaNacimiento', A.Telefono, A.Direccion, ")
        'Q.Append("(CASE WHEN A.FechaNacimiento = '' THEN '' ELSE CONVERT(VARCHAR, CONVERT(DATE, (SUBSTRING(A.FechaNacimiento,1,2)+'/'+SUBSTRING(A.FechaNacimiento,4,2)+'/'+SUBSTRING(A.FechaNacimiento,7,4))),103) END) AS 'FechaNacimiento', A.Telefono, A.Direccion, ")
        Q.Append("(CASE WHEN A.FechaNacimiento = '' THEN '' ELSE A.FechaNacimiento END) AS 'FechaNacimiento', A.Telefono, A.Direccion, ")
        Q.Append("(CASE WHEN LTRIM(RTRIM(C.NomMotivoBaja)) IS NULL THEN '' ELSE LTRIM(RTRIM(C.NomMotivoBaja)) END) AS 'MotivoBaja' ")
        Q.Append("FROM BasalesPediatria AS A LEFT OUTER JOIN ")
        Q.Append("PAC_M_GENERO AS B ON A.Genero = B.IdGenero LEFT OUTER JOIN ")
        Q.Append("PAC_M_MOTIVOBAJA AS C ON A.IdBaja = C.IdMotivoBaja ")
        Q.Append("WHERE (A.NHC = '" & nhc & "')")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("NomGenero") + "|" + reader("Paciente") + "|" + reader("Telefono") + "|" + reader("FechaNacimiento") + "|" + reader("Direccion") + "|" + reader("MotivoBaja")
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function ObtieneBasalesP2(ByVal nhc As String, ByVal usuario As String) As String
        _page = "db.ObtieneBasalesP2"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT A.Genero, A.PrimerNombre, A.SegundoNombre, A.PrimerApellido, A.SegundoApellido, ")
        'Q.Append("(CASE WHEN A.FechaNacimiento = '' THEN '' ELSE CONVERT(VARCHAR, CONVERT(DATE, (SUBSTRING(A.FechaNacimiento,4,2)+'/'+SUBSTRING(A.FechaNacimiento,1,2)+'/'+SUBSTRING(A.FechaNacimiento,7,4))),103) END) AS 'FechaNacimiento', A.Telefono, A.Direccion, IdBaja ")
        Q.Append("(CASE WHEN A.FechaNacimiento = '' THEN '' ELSE A.FechaNacimiento END) AS 'FechaNacimiento', A.Telefono, A.Direccion, IdBaja ")
        Q.Append("FROM BasalesPediatria AS A ")
        Q.Append("WHERE (A.NHC = '" & nhc & "')")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("Genero").ToString() + "|" + reader("PrimerNombre").ToString() + "|" + reader("SegundoNombre").ToString() + "|" + reader("PrimerApellido").ToString() + "|" + reader("SegundoApellido").ToString() + "|" + reader("FechaNacimiento").ToString() + "|" + reader("Telefono").ToString() + "|" + reader("Direccion").ToString() + "|" + reader("IdBaja").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function



    Public Function ObtieneBasalesPed(ByVal nhc As String, ByVal usuario As String) As String
        _page = "db.ObtieneBasalesPed"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT P.NHC, LTRIM(RTRIM(P.PrimerNombre)) + (CASE WHEN P.SegundoNombre IS NULL THEN '' WHEN P.SegundoNombre = 'SSN' THEN '' ELSE ' ' + ")
        Q.Append("LTRIM(RTRIM(P.SegundoNombre)) END) + ' ' + LTRIM(RTRIM(P.PrimerApellido)) + (CASE WHEN P.SegundoApellido IS NULL THEN '' ")
        Q.Append("WHEN P.SegundoApellido = 'SSA' THEN '' ELSE ' ' + LTRIM(RTRIM(P.SegundoApellido)) END) AS 'Paciente', dbo.fn_ObtieneGenero(P.Genero) AS 'Genero', ")
        Q.Append("dbo.fn_ObtieneEdad(P.FechaNacimiento,getdate()) AS 'Fecha_Nac', P.Telefono , P.Direccion ")
        Q.Append("FROM dbo.BasalesPediatria AS P ")
        Q.Append("WHERE (P.NHC = '" & nhc & "')")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("NHC").ToString() + "|" + reader("Paciente").ToString() + "|" + reader("Genero").ToString() + "|" + reader("Fecha_Nac").ToString() + "|" + reader("Telefono").ToString() + "|" + reader("Direccion").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    '*Obtiene Profilaxis y dosis*//
    Public Function ObtieneMED(ByVal codigo As String, ByVal usuario As String) As String
        _page = "db.ObtieneMED"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT M.NomProfilaxis, F.Concentracion ")
        Q.Append("FROM FFProf AS F INNER JOIN ")
        Q.Append("MedProf AS M ON F.IdProf = M.IdProf ")
        Q.Append("WHERE F.IdFFProf = '" & codigo & "'")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("NomProfilaxis") + "|" + reader("Concentracion")
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No se Encontró Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & codigo
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    '*Lista FFProf-FFARV*//
    Public Function busquedaMedProf(ByVal tipo As Integer, ByVal usuario As String) As DataTable
        _page = "db.busquedaMedProf"
        Dim Query As String = ""
        If tipo = "1" Then
            Query = "SELECT A.IdFFARV, A.Codigo, B.NomARV, C.NomFF, A.Concentracion "
            Query += "FROM FFARV AS A LEFT OUTER JOIN "
            Query += "MedARV AS B ON A.IdARV = B.IdARV LEFT OUTER JOIN "
            Query += "FormaFarmaceutica AS C ON A.IdFF = C.IdFF "
            Query += "ORDER BY A.Codigo ASC"
        ElseIf tipo = "2" Then
            Query = "SELECT A.IdFFProf, A.Codigo, B.NomProfilaxis, C.NomFF, A.Concentracion "
            Query += "FROM FFProf AS A LEFT OUTER JOIN "
            Query += "MedProf AS B ON A.IdProf = B.IdProf LEFT OUTER JOIN "
            Query += "FormaFarmaceutica AS C ON A.IdFF = C.IdFF "
            Query += "ORDER BY A.Codigo ASC"
        End If
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ContenidoFFARV(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ContenidoFFARV"
        Dim Q As New StringBuilder()
        Dim Query As String = "SELECT IdARV, IdFF, Concentracion FROM FFARV WHERE IdFFARV = " & id
        Dim Str As String = ""
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdARV").ToString() + "|" + reader("IdFF").ToString() + "|" + reader("Concentracion").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function ContenidoFFProf(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ContenidoFFProf"
        Dim Q As New StringBuilder()
        Dim Query As String = "SELECT IdProf, IdFF, Concentracion FROM FFProf WHERE IdFFProf = " & id
        Dim Str As String = ""
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdProf").ToString() + "|" + reader("IdFF").ToString() + "|" + reader("Concentracion").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function busquedaCod(ByVal tipo As Integer, ByVal usuario As String) As DataTable
        _page = "db.busquedaCod"
        Dim Query As String = ""
        If tipo = "1" Then
            Query = "SELECT IdARV, NomARV, NomCorto FROM MedARV ORDER BY IdARV"
        ElseIf tipo = "2" Then
            Query = "SELECT IdProf, NomProfilaxis FROM MedProf ORDER BY IdProf"
        End If
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ContenidoCodARV(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ContenidoCodARV"
        Dim Q As New StringBuilder()
        Dim Query As String = "SELECT IdARV, NomARV, NomCorto FROM MedARV WHERE IdARV = " & id
        Dim Str As String = ""
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdARV").ToString() + "|" + reader("NomARV").ToString() + "|" + reader("NomCorto").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function ContenidoCodProf(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ContenidoCodProf"
        Dim Q As New StringBuilder()
        Dim Query As String = "SELECT IdProf, NomProfilaxis FROM MedProf WHERE IdProf = " & id
        Dim Str As String = ""
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdProf").ToString() + "|" + reader("NomProfilaxis").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    '*Lista FF*//
    Public Function busquedaff(ByVal usuario As String) As DataTable
        _page = "db.busquedaff"
        Dim Query As String = ""
        Query = "SELECT IdFF, NomFF FROM FormaFarmaceutica ORDER BY IdFF"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Graba FF*//
    Public Sub GrabaFF(ByVal ff As String, ByVal usuario As String)
        _page = "db.GrabaFF"
        Dim sql As String = String.Format("INSERT INTO FormaFarmaceutica (NomFF) VALUES ('{0}')", ff)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Function ContenidoFF(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ContenidoFF"
        Dim Q As New StringBuilder()
        Dim Query As String = "SELECT IdFF, NomFF FROM FormaFarmaceutica WHERE IdFF = " & id
        Dim Str As String = ""
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdFF").ToString() + "|" + reader("NomFF").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Sub ActualizaFF(ByVal id As String, ByVal datos As String, ByVal usuario As String)
        _page = "db.ActualizaFF"
        'Dim d As String() = datos.Split("|")
        Dim sql As String = String.Format("UPDATE FormaFarmaceutica SET NomFF = '{1}', FechaModificacion = GETDATE() WHERE IdFF = '{0}'", id, datos.ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page + "_" + id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    '*Lista Estatus*//
    Public Function busquedaEstatus(ByVal usuario As String) As DataTable
        _page = "db.busquedaEstatus"
        Dim Query As String = ""
        Query = "SELECT IdEstatus, Codigo, Descripcion FROM Estatus ORDER BY IdEstatus"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Graba Estatus*//
    Public Sub GrabaEstatus(ByVal codigo As String, ByVal descripcion As String, ByVal usuario As String)
        _page = "db.GrabaEstatus"
        Dim sql As String = String.Format("INSERT INTO Estatus (Codigo, Descripcion) VALUES ('{0}', '{1}')", codigo, descripcion)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Function ContenidoEstatus(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ContenidoEstatus"
        Dim Q As New StringBuilder()
        Dim Query As String = "SELECT IdEstatus, Codigo, Descripcion FROM Estatus WHERE IdEstatus = " & id
        Dim Str As String = ""
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdEstatus").ToString() + "|" + reader("Codigo").ToString() + "|" + reader("Descripcion").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Sub ActualizaEstatus(ByVal id As String, ByVal codigo As String, ByVal descripcion As String, ByVal usuario As String)
        _page = "db.ActualizaEstatus"
        'Dim d As String() = datos.Split("|")
        Dim sql As String = String.Format("UPDATE Estatus SET Codigo = '{1}', Descripcion = '{2}' WHERE IdEstatus = '{0}'", id, codigo, descripcion)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page + "_" + id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    '*Lista Frecuencia*//
    Public Function busquedaFx(ByVal usuario As String) As DataTable
        _page = "db.busquedaFx"
        Dim Query As String = ""
        Query = "SELECT IdFrecuencia, Codigo, Descripcion FROM Frecuencia ORDER BY IdFrecuencia"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Graba Frecuencia*//
    Public Sub GrabaFx(ByVal codigo As String, ByVal descripcion As String, ByVal usuario As String)
        _page = "db.GrabaFx"
        Dim sql As String = String.Format("INSERT INTO Frecuencia (Codigo, Descripcion) VALUES ('{0}', '{1}')", codigo, descripcion)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Function ContenidoFx(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ContenidoFx"
        Dim Q As New StringBuilder()
        Dim Query As String = "SELECT IdFrecuencia, Codigo, Descripcion FROM Frecuencia WHERE IdFrecuencia = " & id
        Dim Str As String = ""
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdFrecuencia").ToString() + "|" + reader("Codigo").ToString() + "|" + reader("Descripcion").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Sub ActualizaFx(ByVal id As String, ByVal codigo As String, ByVal descripcion As String, ByVal usuario As String)
        _page = "db.ActualizaFx"
        'Dim d As String() = datos.Split("|")
        Dim sql As String = String.Format("UPDATE Frecuencia SET Codigo = '{1}', Descripcion = '{2}' WHERE IdFrecuencia = '{0}'", id, codigo.ToString(), descripcion.ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page + "_" + id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    '*Codigo MedARV*//
    Public Function ObtieneMedARVProf(ByVal tipo As String, ByVal usuario As String) As DataTable
        _page = "db.ObtieneMedARVProf"
        Dim Query As String = ""
        If tipo = "1" Then
            Query = "SELECT IdARV, NomARV FROM MedARV ORDER BY NomARV ASC"
        ElseIf tipo = "2" Then
            Query = "SELECT IdProf, NomProfilaxis FROM MedProf ORDER BY NomProfilaxis ASC"
        End If

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ObtieneMedARV(ByVal id As String, ByVal usuario As String) As String
        _page = "db.ObtieneMedARV"
        Dim Query As String = ""
        Query = "SELECT NomCorto FROM MedARV WHERE IdARV = " & id
        Dim Ds As New DataSet()
        Dim Str As String = ""
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("NomCorto").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe Información."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    '*Codigo FF*//
    Public Function ObtieneFF(ByVal usuario As String) As DataTable
        _page = "db.ObtieneFF"
        Dim Query As String = "SELECT IdFF, NomFF FROM FormaFarmaceutica ORDER BY NomFF ASC"

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Codigo ARV/Medicamentos*//
    Public Function ObtieneARVMedicamento(ByVal tipo As String, ByVal usuario As String) As DataTable
        _page = "db.ObtieneARVMedicamento"
        Dim Query As String = ""
        If tipo = "1" Then
            Query = "SELECT IdFFARV, Codigo FROM FFARV ORDER BY Codigo ASC"
        ElseIf tipo = "2" Then
            Query = "SELECT IdFFProf, Codigo FROM FFProf WHERE IdFFProf NOT IN (21,22,23) ORDER BY Codigo ASC"
        End If

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ObtieneARVMedicamentoXid(ByVal id As String, ByVal usuario As String) As DataTable
        _page = "db.ObtieneARVMedicamentoXid"
        Dim Query As String = ""
        Query = String.Format("dbo.sp_ListaMedXEsquema {0}", id)
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                Dim coDetalle As New SqlCommand
                coDetalle.CommandText = "sp_ListaMedXEsquema"
                coDetalle.CommandType = CommandType.StoredProcedure
                coDetalle.Connection = connection  'Previamente definida
                'El Adaptador y su SelectCommand
                Dim daDetalle As New SqlDataAdapter
                daDetalle.SelectCommand = coDetalle
                'Parámetros si hubieran
                Dim miParam As New SqlParameter("@esquema", SqlDbType.Int)
                miParam.Direction = ParameterDirection.Input
                coDetalle.Parameters.Add(miParam)
                coDetalle.Parameters("@esquema").Value = id
                'Llenar el DataSet
                'Al llenar el DataSet se ejecuta el Store Procedure
                'el SQLCommand del SQLDataAdapter se especificó del tipo StoreProcedure
                daDetalle.Fill(Ds, _page)
                daDetalle.Dispose()
                'connection.Dispose()
                'connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ObtieneARVMedicamentoXidSE(ByVal id As String, ByVal usuario As String) As DataTable
        _page = "db.ObtieneARVMedicamentoXidSE"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                Dim coDetalle As New SqlCommand
                coDetalle.CommandText = "sp_ListaMedXEsquema4"
                coDetalle.CommandType = CommandType.StoredProcedure
                coDetalle.Connection = connection  'Previamente definida
                'El Adaptador y su SelectCommand
                Dim daDetalle As New SqlDataAdapter
                daDetalle.SelectCommand = coDetalle
                'Parámetros si hubieran
                Dim miParam As New SqlParameter("@sesquema", SqlDbType.Int)
                miParam.Direction = ParameterDirection.Input
                coDetalle.Parameters.Add(miParam)
                coDetalle.Parameters("@sesquema").Value = id
                'Llenar el DataSet
                'Al llenar el DataSet se ejecuta el Store Procedure
                'el SQLCommand del SQLDataAdapter se especificó del tipo StoreProcedure
                daDetalle.Fill(Ds, _page)
                daDetalle.Dispose()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ObtieneARVMedicamentoXid3(ByVal id As String, ByVal usuario As String) As DataTable
        _page = "db.ObtieneARVMedicamentoXid3"
        Dim Query As String = ""
        Query = String.Format("dbo.sp_ListaMedXEsquema3 {0}", id)
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                Dim coDetalle As New SqlCommand
                coDetalle.CommandText = "sp_ListaMedXEsquema3"
                coDetalle.CommandType = CommandType.StoredProcedure
                coDetalle.Connection = connection  'Previamente definida
                'El Adaptador y su SelectCommand
                Dim daDetalle As New SqlDataAdapter
                daDetalle.SelectCommand = coDetalle
                'Parámetros si hubieran
                Dim miParam As New SqlParameter("@esquema", SqlDbType.Int)
                miParam.Direction = ParameterDirection.Input
                coDetalle.Parameters.Add(miParam)
                coDetalle.Parameters("@esquema").Value = id
                'Llenar el DataSet
                'Al llenar el DataSet se ejecuta el Store Procedure
                'el SQLCommand del SQLDataAdapter se especificó del tipo StoreProcedure
                daDetalle.Fill(Ds, _page)
                daDetalle.Dispose()
                'connection.Dispose()
                'connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Codigo Estatus*//
    Public Function ObtieneEstatus(ByVal usuario As String) As DataTable
        _page = "db.ObtieneEstatus"
        Dim Query As String = "SELECT IdEstatus, Codigo FROM Estatus ORDER BY Codigo ASC"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ObtieneEstatusProf(ByVal usuario As String) As DataTable
        _page = "db.ObtieneEstatusProf"
        Dim Query As String = "SELECT IdEstatus, Codigo FROM Estatus WHERE IdEstatus IN (2,1,8,3) ORDER BY Codigo ASC"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Via Administración*//
    Public Function ObtieneVIA(ByVal usuario As String) As DataTable
        _page = "db.ObtieneVIA"
        Dim Query As String = "SELECT IdViaAdministracion, NomViaAdministracion FROM VIAADMINISTRACION ORDER BY IdViaAdministracion ASC"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Codigo Frecuencia*//
    Public Function ObtieneFrecuenia(ByVal usuario As String) As DataTable
        _page = "db.ObtieneFrecuenia"
        Dim Query As String = "SELECT IdFrecuencia FROM Frecuencia ORDER BY IdFrecuencia ASC"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Codigo Embarazo*//
    Public Function ObtieneEmbarazo(ByVal usuario As String) As DataTable
        _page = "db.ObtieneEmbarazo"
        Dim Query As String = "SELECT IdEmbarazo FROM Embarazo ORDER BY IdEmbarazo ASC"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Codigo Baja*//
    Public Function ObtieneBaja(ByVal usuario As String) As DataTable
        _page = "db.ObtieneBaja"
        Dim Query As String = "SELECT IdMotivoBaja, NomMotivoBaja FROM PAC_M_MOTIVOBAJA ORDER BY IdMotivoBaja ASC"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    '*Ultima Fecha Entrega*//
    Public Function ObtieneUltimoReg(ByVal nhc As String, ByVal usuario As String) As String
        _page = "db.ObtieneUltimoReg"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT A.IdCCARV, CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', (E.Codigo + ' - ' + E.Descripcion) AS 'Estatus', ")
        Q.Append("(CASE WHEN A.Med1_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med1_Codigo) ELSE '' END) AS 'Med1_Codigo', ")
        Q.Append("(CASE WHEN A.Med2_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med2_Codigo) ELSE '' END) AS 'Med2_Codigo', ")
        Q.Append("(CASE WHEN A.Med3_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med3_Codigo) ELSE '' END) AS 'Med3_Codigo', ")
        Q.Append("(CASE WHEN A.Med4_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med4_Codigo) ELSE '' END) AS 'Med4_Codigo', ")
        Q.Append("(CASE WHEN A.Med5_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med5_Codigo) ELSE '' END) AS 'Med5_Codigo', ")
        Q.Append("(CASE WHEN A.Med6_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med6_Codigo) ELSE '' END) AS 'Med6_Codigo', ")
        Q.Append("(CASE WHEN A.Med7_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med7_Codigo) ELSE '' END) AS 'Med7_Codigo', ")
        Q.Append("(CASE WHEN A.Med8_Codigo <> 0 THEN (SELECT Codigo FROM FFARV WHERE IdFFARV = A.Med8_Codigo) ELSE '' END) AS 'Med8_Codigo' ")
        Q.Append("FROM ControlARV AS A LEFT OUTER JOIN ")
        Q.Append("Estatus AS E ON E.IdEstatus = A.EsquemaEstatus ")
        Q.Append("WHERE A.NHC = '" & nhc & "' ")
        Q.Append("AND A.IdCCARV = (SELECT TOP(1) B.IdCCARV FROM ControlARV AS B WHERE B.NHC = A.NHC AND B.FechaEntrega = (SELECT DISTINCT MAX(C.FechaEntrega) FROM ControlARV AS C WHERE C.NHC = B.NHC) ORDER BY B.IdCCARV DESC) ")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdCCARV").ToString() + "|" + reader("FechaEntrega").ToString() + "|" + reader("Med1_Codigo").ToString() + "|" + reader("Med2_Codigo").ToString() + "|" + reader("Med3_Codigo").ToString() + "|" + reader("Med4_Codigo").ToString() + "|" + reader("Med5_Codigo").ToString() + "|" + reader("Med6_Codigo").ToString() + "|" + reader("Med7_Codigo").ToString() + "|" + reader("Med8_Codigo").ToString() + "|" + reader("Estatus").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe última fecha."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function ObtieneUltimoRegProf(ByVal nhc As String, ByVal usuario As String) As String
        _page = "db.ObtieneUltimoRegProf"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT A.IdCCPROF, CONVERT(VARCHAR,A.FechaEntrega, 103) AS 'FechaEntrega', ")
        Q.Append("(CASE WHEN A.Prof1_Codigo <> 0 THEN (SELECT Codigo FROM FFProf WHERE IdFFProf = A.Prof1_Codigo) ELSE '' END) AS 'Prof1_Codigo', ")
        Q.Append("(CASE WHEN A.Prof2_Codigo <> 0 THEN (SELECT Codigo FROM FFProf WHERE IdFFProf = A.Prof2_Codigo) ELSE '' END) AS 'Prof2_Codigo', ")
        Q.Append("(CASE WHEN A.Prof3_Codigo <> 0 THEN (SELECT Codigo FROM FFProf WHERE IdFFProf = A.Prof3_Codigo) ELSE '' END) AS 'Prof3_Codigo', ")
        Q.Append("(CASE WHEN A.Prof4_Codigo <> 0 THEN (SELECT Codigo FROM FFProf WHERE IdFFProf = A.Prof4_Codigo) ELSE '' END) AS 'Prof4_Codigo' ")
        Q.Append("FROM ControlProf AS A ")
        Q.Append("WHERE A.FechaEntrega = (SELECT DISTINCT MAX(B.FechaEntrega) FROM ControlProf AS B WHERE B.NHC = '" & nhc & "') ")
        Q.Append("AND A.NHC = '" & nhc & "'")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("IdCCPROF").ToString() + "|" + reader("FechaEntrega").ToString() + "|" + reader("Prof1_Codigo").ToString() + "|" + reader("Prof2_Codigo").ToString() + "|" + reader("Prof3_Codigo").ToString() + "|" + reader("Prof4_Codigo").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No existe última fecha."
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    '*Graba Controles*//
    Public Sub GrabaUFechaControlARV(ByVal datos As String, ByVal usuario As String)
        _page = "db.GrabaUFechaControlARV"
        Dim d As String() = datos.Split("|")
        Dim sql As String = String.Format("UPDATE ControlARV SET Med1_DevCantidad = {1}, Med2_DevCantidad = {2}, Med3_DevCantidad = {3}, Med4_DevCantidad = {4}, Med5_DevCantidad = {5}, Med6_DevCantidad = {6}, Med7_DevCantidad = {7}, Med8_DevCantidad = {8}, Adherencia = {9}, TiempoRetorno = {10}, FechaModificacion = GETDATE() WHERE IdCCARV = {0}", d(0).ToString(), d(1).ToString(), d(2).ToString(), d(3).ToString(), d(4).ToString(), d(5).ToString(), d(6).ToString(), d(7).ToString(), d(8).ToString(), d(9).ToString(), d(10).ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub GrabaControlARV(ByVal datos As String, ByVal usuario As String)
        _page = "db.GrabaControlARV"
        Dim sql As String = ""
        Dim d As String() = datos.Split("|")
        'NHC, FechaEntrega, Med1_Codigo, Med1_Cantidad, Med1_Dosis, Med1_Frecuencia, Med1_UExCantidad, Med1_ARVEstatus, Med2_Codigo, Med2_Cantidad, Med2_Dosis, Med2_Frecuencia, Med2_UExCantidad, Med2_ARVEstatus, Med3_Codigo, Med3_Cantidad, Med3_Dosis, Med3_Frecuencia, Med3_UExCantidad, Med3_ARVEstatus, Med4_Codigo, Med4_Cantidad, Med4_Dosis, Med4_Frecuencia, Med4_UExCantidad, Med4_ARVEstatus, Med5_Codigo, Med5_Cantidad, Med5_Dosis, Med5_Frecuencia, Med5_UExCantidad, Med5_ARVEstatus, Med6_Codigo, Med6_Cantidad, Med6_Dosis, Med6_Frecuencia, Med6_UExCantidad, Med6_ARVEstatus, Med7_Codigo, Med7_Cantidad, Med7_Dosis, Med7_Frecuencia, Med7_UExCantidad, Med7_ARVEstatus, Med8_Codigo, Med8_Cantidad, Med8_Dosis, Med8_Frecuencia, Med8_UExCantidad, Med8_ARVEstatus, FechaRetorno, TiempoTARV, CitaMedica, CitaFarmacia, Embarazo, TiempoRetorno, CD4, CV, Observaciones, NomUsuario
        sql = "INSERT INTO ControlARV (NHC, FechaEntrega, IdEsquema, IdSEsquema, EsquemaEstatus, Med1_Codigo, Med1_Cantidad, Med1_Dosis, Med1_Frecuencia, Med1_UExCantidad, Med1_ARVEstatus, Med2_Codigo, Med2_Cantidad, Med2_Dosis, Med2_Frecuencia, Med2_UExCantidad, Med2_ARVEstatus, Med3_Codigo, Med3_Cantidad, Med3_Dosis, Med3_Frecuencia, Med3_UExCantidad, Med3_ARVEstatus, Med4_Codigo, Med4_Cantidad, Med4_Dosis, Med4_Frecuencia, Med4_UExCantidad, Med4_ARVEstatus, Med5_Codigo, Med5_Cantidad, Med5_Dosis, Med5_Frecuencia, Med5_UExCantidad, Med5_ARVEstatus, Med6_Codigo, Med6_Cantidad, Med6_Dosis, Med6_Frecuencia, Med6_UExCantidad, Med6_ARVEstatus, Med7_Codigo, Med7_Cantidad, Med7_Dosis, Med7_Frecuencia, Med7_UExCantidad, Med7_ARVEstatus, Med8_Codigo, Med8_Cantidad, Med8_Dosis, Med8_Frecuencia, Med8_UExCantidad, Med8_ARVEstatus, FechaRetorno, TiempoTARV, CitaMedica, CitaFarmacia, Embarazo, CD4, CV, Observaciones, NomUsuario) "
        sql += String.Format("VALUES('{0}', CONVERT(date,'{1}'), {2}, {3}, {4}, {5}, {6}, '{7}', {8}, {9}, {10}, {11}, {12}, '{13}', {14}, {15}, {16}, {17}, {18}, '{19}', {20}, {21}, {22}, {23}, {24}, '{25}', {26}, {27}, {28}, {29}, {30}, '{31}', {32}, {33}, {34}, {35}, {36}, '{37}', {38}, {39}, {40}, {41}, {42}, '{43}', {44}, {45}, {46}, {47}, {48}, '{49}', {50}, {51}, {52}, CONVERT(date,'{53}'), {54}, '{55}', '{56}', '{57}', '{58}', '{59}', '{60}', '{61}')", d(0).ToString(), d(1).ToString(), d(2).ToString(), d(3).ToString(), d(4).ToString(), d(5).ToString(), d(6).ToString(), d(7).ToString(), d(8).ToString(), d(9).ToString(), d(10).ToString(), d(11).ToString(), d(12).ToString(), d(13).ToString(), d(14).ToString(), d(15).ToString(), d(16).ToString(), d(17).ToString(), d(18).ToString(), d(19).ToString(), d(20).ToString(), d(21).ToString(), d(22).ToString(), d(23).ToString(), d(24).ToString(), d(25).ToString(), d(26).ToString(), d(27).ToString(), d(28).ToString(), d(29).ToString(), d(30).ToString(), d(31).ToString(), d(32).ToString(), d(33).ToString(), d(34).ToString(), d(35).ToString(), d(36).ToString(), d(37).ToString(), d(38).ToString(), d(39).ToString(), d(40).ToString(), d(41).ToString(), d(42).ToString(), d(43).ToString(), d(44).ToString(), d(45).ToString(), d(46).ToString(), d(47).ToString(), d(48).ToString(), d(49).ToString(), d(50).ToString(), d(51).ToString(), d(52).ToString(), d(53).ToString(), d(54).ToString(), d(55).ToString(), d(56).ToString(), d(57).ToString(), d(58).ToString(), d(59).ToString(), d(60).ToString(), usuario)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub ActualizaControlARV(ByVal datos As String, ByVal usuario As String)
        _page = "db.ActualizaControlARV"
        Dim sql As String = ""
        Dim d As String() = datos.Split("|")
        sql = String.Format("UPDATE ControlARV SET Med1_Cantidad = {0}, Med1_Dosis = '{1}', Med1_Frecuencia = '{2}', Med1_UExCantidad = {3}, Med1_DevCantidad = {4}, ", d(1).ToString(), d(2).ToString(), d(3).ToString(), d(4).ToString(), d(5).ToString())
        sql += String.Format("Med2_Cantidad = {0}, Med2_Dosis = '{1}', Med2_Frecuencia = '{2}', Med2_UExCantidad = {3}, Med2_DevCantidad = {4}, ", d(6).ToString(), d(7).ToString(), d(8).ToString(), d(9).ToString(), d(10).ToString())
        sql += String.Format("Med3_Cantidad = {0}, Med3_Dosis = '{1}', Med3_Frecuencia = '{2}', Med3_UExCantidad = {3}, Med3_DevCantidad = {4}, ", d(11).ToString(), d(12).ToString(), d(13).ToString(), d(14).ToString(), d(15).ToString())
        sql += String.Format("Med4_Cantidad = {0}, Med4_Dosis = '{1}', Med4_Frecuencia = '{2}', Med4_UExCantidad = {3}, Med4_DevCantidad = {4}, ", d(16).ToString(), d(17).ToString(), d(18).ToString(), d(19).ToString(), d(20).ToString())
        sql += String.Format("Med5_Cantidad = {0}, Med5_Dosis = '{1}', Med5_Frecuencia = '{2}', Med5_UExCantidad = {3}, Med5_DevCantidad = {4}, ", d(21).ToString(), d(22).ToString(), d(23).ToString(), d(24).ToString(), d(25).ToString())
        sql += String.Format("Med6_Cantidad = {0}, Med6_Dosis = '{1}', Med6_Frecuencia = '{2}', Med6_UExCantidad = {3}, Med6_DevCantidad = {4}, ", d(26).ToString(), d(27).ToString(), d(28).ToString(), d(29).ToString(), d(30).ToString())
        sql += String.Format("Med7_Cantidad = {0}, Med7_Dosis = '{1}', Med7_Frecuencia = '{2}', Med7_UExCantidad = {3}, Med7_DevCantidad = {4}, ", d(31).ToString(), d(32).ToString(), d(33).ToString(), d(34).ToString(), d(35).ToString())
        sql += String.Format("Med8_Cantidad = {0}, Med8_Dosis = '{1}', Med8_Frecuencia = '{2}', Med8_UExCantidad = {3}, Med8_DevCantidad = {4}, ", d(36).ToString(), d(37).ToString(), d(38).ToString(), d(39).ToString(), d(40).ToString())
        sql += String.Format("FechaRetorno = '{0}', TiempoTARV = {1}, CitaMedica = '{2}', CitaFarmacia = '{3}', Embarazo = '{4}', ", d(41).ToString(), d(42).ToString(), d(43).ToString(), d(44).ToString(), d(45).ToString())
        sql += String.Format("TiempoRetorno = {0}, Adherencia = {1}, CD4 = '{2}', CV = '{3}', Observaciones = '{4}' ", d(46).ToString(), d(47).ToString(), d(48).ToString(), d(49).ToString(), d(50).ToString())
        sql += String.Format("WHERE IdCCARV = {0}", d(0).ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub ActualizaControlProf(ByVal datos As String, ByVal usuario As String)
        _page = "db.ActualizaControlProf"
        Dim sql As String = ""
        Dim d As String() = datos.Split("|")
        sql = String.Format("UPDATE ControlProf SET TipoPaciente = {0}, ", d(2).ToString())
        sql += String.Format("Prof1_Cantidad = {0}, Prof1_Dosis = '{1}', Prof1_VIA = '{2}', Prof1_Frecuencia = '{3}', Prof1_Tipo = {4}, Prof1_TipoTratamiento = {5}, Prof1_Estatus = {6}, Prof1_TTMed = {7}, Prof1_Observaciones = '{8}', ", d(3).ToString(), d(4).ToString(), d(5).ToString(), d(6).ToString(), d(7).ToString(), d(8).ToString(), d(9).ToString(), d(10).ToString(), d(11).ToString())
        sql += String.Format("Prof2_Cantidad = {0}, Prof2_Dosis = '{1}', Prof2_VIA = '{2}', Prof2_Frecuencia = '{3}', Prof2_Tipo = {4}, Prof2_TipoTratamiento = {5}, Prof2_Estatus = {6}, Prof2_TTMed = {7}, Prof2_Observaciones = '{8}', ", d(12).ToString(), d(13).ToString(), d(14).ToString(), d(15).ToString(), d(16).ToString(), d(17).ToString(), d(18).ToString(), d(19).ToString(), d(20).ToString())
        sql += String.Format("Prof3_Cantidad = {0}, Prof3_Dosis = '{1}', Prof3_VIA = '{2}', Prof3_Frecuencia = '{3}', Prof3_Tipo = {4}, Prof3_TipoTratamiento = {5}, Prof3_Estatus = {6}, Prof3_TTMed = {7}, Prof3_Observaciones = '{8}', ", d(21).ToString(), d(22).ToString(), d(23).ToString(), d(24).ToString(), d(25).ToString(), d(26).ToString(), d(27).ToString(), d(28).ToString(), d(29).ToString())
        sql += String.Format("Prof4_Cantidad = {0}, Prof4_Dosis = '{1}', Prof4_VIA = '{2}', Prof4_Frecuencia = '{3}', Prof4_Tipo = {4}, Prof4_TipoTratamiento = {5}, Prof4_Estatus = {6}, Prof4_TTMed = {7}, Prof4_Observaciones = '{8}', ", d(30).ToString(), d(31).ToString(), d(32).ToString(), d(33).ToString(), d(34).ToString(), d(35).ToString(), d(36).ToString(), d(37).ToString(), d(38).ToString())
        sql += String.Format("Prof5_Cantidad = {0}, Prof5_Dosis = '{1}', Prof5_VIA = '{2}', Prof5_Frecuencia = '{3}', Prof5_Tipo = {4}, Prof5_TipoTratamiento = {5}, Prof5_Estatus = {6}, Prof5_TTMed = {7}, Prof5_Observaciones = '{8}', ", d(39).ToString(), d(40).ToString(), d(41).ToString(), d(42).ToString(), d(43).ToString(), d(44).ToString(), d(45).ToString(), d(46).ToString(), d(47).ToString())
        sql += String.Format("Prof6_Cantidad = {0}, Prof6_Dosis = '{1}', Prof6_VIA = '{2}', Prof6_Frecuencia = '{3}', Prof6_Tipo = {4}, Prof6_TipoTratamiento = {5}, Prof6_Estatus = {6}, Prof6_TTMed = {7}, Prof6_Observaciones = '{8}', ", d(48).ToString(), d(49).ToString(), d(50).ToString(), d(51).ToString(), d(52).ToString(), d(53).ToString(), d(54).ToString(), d(55).ToString(), d(56).ToString())
        sql += String.Format("CD4 = '{0}', NomUsuario = '{1}', FechaModificacion = GETDATE() ", d(1).ToString(), d(57).ToString())
        sql += String.Format("WHERE IdCCPROF = {0}", d(0).ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub GrabaUFechaControlPROF(ByVal datos As String, ByVal usuario As String)
        _page = "db.GrabaUFechaControlPROF"
        Dim d As String() = datos.Split("|")
        Dim sql As String = String.Format("UPDATE ControlPROF SET Prof1_DevCantidad = {1}, Prof2_DevCantidad = {2}, Prof3_DevCantidad = {3}, Prof4_DevCantidad = {4}, FechaModificacion = GETDATE() WHERE IdCCProf = {0}", d(0).ToString(), d(1).ToString(), d(2).ToString(), d(3).ToString(), d(4).ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub GrabaControlPROF(ByVal datos As String, ByVal usuario As String)
        _page = "db.GrabaControlPROF"
        Dim sql As String = ""
        Dim d As String() = datos.Split("|")
        'NHC, FechaEntrega, Prof1_Codigo, Prof1_Cantidad, Prof1_Dosis, Prof1_Frecuencia, Prof1_MedEstatus, Prof2_Codigo, Prof2_Cantidad, Prof2_Dosis, Prof2_Frecuencia, Prof2_MedEstatus, Prof3_Codigo, Prof3_Cantidad, Prof3_Dosis, Prof3_Frecuencia, Prof3_MedEstatus, Prof4_Codigo, Prof4_Cantidad, Prof4_Dosis, Prof4_Frecuencia, Prof4_MedEstatus, FechaRetorno, TiempoTMed, CitaMedica, CitaFarmacia, Embarazo, TiempoRetorno, CD4, CV, Observaciones
        sql = "INSERT INTO ControlPROF (NHC, FechaEntrega, TipoPaciente, "
        sql += "Prof1_Codigo, Prof1_Cantidad, Prof1_Dosis, Prof1_VIA, Prof1_Frecuencia, Prof1_Tipo, Prof1_TipoTratamiento, Prof1_Estatus, Prof1_TTMed, Prof1_Observaciones, "
        sql += "Prof2_Codigo, Prof2_Cantidad, Prof2_Dosis, Prof2_VIA, Prof2_Frecuencia, Prof2_Tipo, Prof2_TipoTratamiento, Prof2_Estatus, Prof2_TTMed, Prof2_Observaciones, "
        sql += "Prof3_Codigo, Prof3_Cantidad, Prof3_Dosis, Prof3_VIA, Prof3_Frecuencia, Prof3_Tipo, Prof3_TipoTratamiento, Prof3_Estatus, Prof3_TTMed, Prof3_Observaciones, "
        sql += "Prof4_Codigo, Prof4_Cantidad, Prof4_Dosis, Prof4_VIA, Prof4_Frecuencia, Prof4_Tipo, Prof4_TipoTratamiento, Prof4_Estatus, Prof4_TTMed, Prof4_Observaciones, "
        sql += "Prof5_Codigo, Prof5_Cantidad, Prof5_Dosis, Prof5_VIA, Prof5_Frecuencia, Prof5_Tipo, Prof5_TipoTratamiento, Prof5_Estatus, Prof5_TTMed, Prof5_Observaciones, "
        sql += "Prof6_Codigo, Prof6_Cantidad, Prof6_Dosis, Prof6_VIA, Prof6_Frecuencia, Prof6_Tipo, Prof6_TipoTratamiento, Prof6_Estatus, Prof6_TTMed, Prof6_Observaciones, "
        sql += "CD4, NomUsuario)"
        sql += String.Format("VALUES ('{0}', CONVERT(date,'{1}'), {2}, {3}, {4}, '{5}', {6}, {7}, {8}, {9}, {10}, {11}, '{12}', {13}, {14}, '{15}', {16}, {17}, {18}, {19}, {20}, {21}, '{22}', {23}, {24}, '{25}', {26}, {27}, {28}, {29}, {30}, {31}, '{32}', {33}, {34}, '{35}', {36}, {37}, {38}, {39}, {40}, {41}, '{42}', {43}, {44}, '{45}', {46}, {47}, {48}, {49}, {50}, {51}, '{52}', {53}, {54}, '{55}', {56}, {57}, {58}, {59}, {60}, {61}, '{62}', '{63}', '{64}')", d(0).ToString(), d(1).ToString(), d(2).ToString(), d(3).ToString(), d(4).ToString(), d(5).ToString(), d(6).ToString(), d(7).ToString(), d(8).ToString(), d(9).ToString(), d(10).ToString(), d(11).ToString(), d(12).ToString(), d(13).ToString(), d(14).ToString(), d(15).ToString(), d(16).ToString(), d(17).ToString(), d(18).ToString(), d(19).ToString(), d(20).ToString(), d(21).ToString(), d(22).ToString(), d(23).ToString(), d(24).ToString(), d(25).ToString(), d(26).ToString(), d(27).ToString(), d(28).ToString(), d(29).ToString(), d(30).ToString(), d(31).ToString(), d(32).ToString(), d(33).ToString(), d(34).ToString(), d(35).ToString(), d(36).ToString(), d(37).ToString(), d(38).ToString(), d(39).ToString(), d(40).ToString(), d(41).ToString(), d(42).ToString(), d(43).ToString(), d(44).ToString(), d(45).ToString(), d(46).ToString(), d(47).ToString(), d(48).ToString(), d(49).ToString(), d(50).ToString(), d(51).ToString(), d(52).ToString(), d(53).ToString(), d(54).ToString(), d(55).ToString(), d(56).ToString(), d(57).ToString(), d(58).ToString(), d(59).ToString(), d(60).ToString(), d(61).ToString(), d(62).ToString(), d(63).ToString(), usuario)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    '*Esquemas y SubEsquemas*//
    Public Sub GrabaEsquema(ByVal descripcion As String, ByVal codigos As String, ByVal usuario As String)
        _page = "db.GrabaEsquema"
        Dim sql As String = ""
        sql = "INSERT INTO Esquemas (Descripcion, Codigos) "
        sql += String.Format("VALUES('{0}', '{1}')", descripcion, codigos)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub GrabaSEsquemas(ByVal IdEsquema As String, ByVal Descripcion As String, ByVal Codigos As String, ByVal usuario As String)
        _page = "db.GrabaSEsquemas"
        Dim sql As String = ""
        sql = "INSERT INTO SubEsquemas (IdEsquema, Descripcion, Codigos, SCodigo) "
        sql += String.Format("VALUES({0}, '{1}', '{2}', (SELECT RIGHT('000' + CAST({0} AS VARCHAR(3)),3) + '-' + RIGHT('00' + CAST(COUNT(IdEsquema) + 1 AS VARCHAR(2)),2) FROM SubEsquemas WHERE IdEsquema = {0}))", IdEsquema, Descripcion, Codigos)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    '*Graba FFProf-FFARV*//
    Public Sub GrabaFFARV(ByVal datos As String, ByVal usuario As String)
        _page = "db.GrabaFFARV"
        Dim sql As String = ""
        Dim d As String() = datos.Split("|")
        sql = "INSERT INTO FFARV (IdARV, IdFF, Concentracion, Codigo) "
        'sql += String.Format("VALUES({0}, {1}, '{2}', (SELECT (CASE WHEN LEN({0}) = 1 THEN ('0' + CONVERT(VARCHAR, {0})) ELSE CONVERT(VARCHAR, {0}) END) +'-'+ (CASE WHEN LEN(COUNT(Codigo)+1) = 1 THEN ('0' + CONVERT(VARCHAR, COUNT(Codigo)+1)) ELSE CONVERT(VARCHAR, COUNT(Codigo)+1) END) FROM FFARV WHERE IdARV = {0}))", d(0).ToString(), d(1).ToString(), d(2).ToString())
        sql += String.Format("VALUES({0}, {1}, '{2}', (SELECT RIGHT('00' + CAST({0} AS VARCHAR(2)),2) + '-' + RIGHT('00' + CAST(COUNT(Codigo) + 1 AS VARCHAR(2)),2) FROM FFARV WHERE IdARV = {0}))", d(0).ToString(), d(1).ToString(), d(2).ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub ActualizaFFARV(ByVal id As String, ByVal datos As String, ByVal usuario As String)
        _page = "db.ActualizaFFARV"
        'Dim d As String() = datos.Split("|")
        Dim sql As String = String.Format("UPDATE FFARV SET Concentracion = '{1}', FechaModificacion = GETDATE() WHERE IdFFARV = '{0}'", id, datos.ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page + "_" + id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub GrabaFFProf(ByVal datos As String, ByVal usuario As String)
        _page = "db.GrabaFFProf"
        Dim sql As String = ""
        Dim d As String() = datos.Split("|")
        sql = "INSERT INTO FFProf (IdProf, IdFF, Concentracion, Codigo) "
        'sql += String.Format("VALUES({0}, {1}, '{2}', (SELECT (CASE WHEN LEN({0}) = 1 THEN ('0' + CONVERT(VARCHAR, {0})) ELSE CONVERT(VARCHAR, {0}) END) +'-'+ (CASE WHEN LEN(COUNT(Codigo)+1) = 1 THEN ('0' + CONVERT(VARCHAR, COUNT(Codigo)+1)) ELSE CONVERT(VARCHAR, COUNT(Codigo)+1) END) FROM FFProf WHERE IdProf = {0}))", d(0).ToString(), d(1).ToString(), d(2).ToString())
        sql += String.Format("VALUES({0}, {1}, '{2}', (SELECT RIGHT('000' + CAST({0} AS VARCHAR(3)),3) + '-' + RIGHT('00' + CAST(COUNT(Codigo) + 1 AS VARCHAR(2)),2) FROM FFProf WHERE IdProf = {0}))", d(0).ToString(), d(1).ToString(), d(2).ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub ActualizaFFProf(ByVal id As String, ByVal datos As String, ByVal usuario As String)
        _page = "db.ActualizaFFProf"
        'Dim d As String() = datos.Split("|")
        Dim sql As String = String.Format("UPDATE FFProf SET Concentracion = '{1}', FechaModificacion = GETDATE() WHERE IdFFProf = '{0}'", id, datos.ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page + "_" + id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    '*Graba LProf-LARV*//
    Public Sub GrabaLARV(ByVal nombre As String, ByVal ncorto As String, ByVal usuario As String)
        _page = "db.GrabaLARV"
        Dim sql As String = String.Format("INSERT INTO MedARV (NomARV, NomCorto) VALUES('{0}', '{1}')", nombre, ncorto)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub ActualizaLARV(ByVal id As String, ByVal nombre As String, ByVal ncorto As String, ByVal usuario As String)
        _page = "db.ActualizaLARV"
        Dim sql As String = String.Format("UPDATE MedARV SET NomARV = '{1}', NomCorto = '{2}', FechaModificacion = GETDATE() WHERE IdARV = '{0}'", id, nombre.ToString(), ncorto.ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page + "_" + id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub GrabaLProf(ByVal datos As String, ByVal usuario As String)
        _page = "db.GrabaLProf"
        Dim sql As String = String.Format("INSERT INTO MedProf (NomProfilaxis) VALUES('{0}')", datos)
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    Public Sub ActualizaLProf(ByVal id As String, ByVal datos As String, ByVal usuario As String)
        _page = "db.ActualizaLProf"
        Dim sql As String = String.Format("UPDATE MedProf SET NomProfilaxis = '{1}', FechaModificacion = GETDATE() WHERE IdProf = '{0}'", id, datos.ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page + "_" + id
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    '*Actualiza Basal Pediatrico*//
    Public Sub ActualizaBPediatrico(ByVal nhc As String, ByVal datos As String, ByVal usuario As String)
        _page = "db.ActualizaBPediatrico"
        Dim d As String() = datos.Split("|")
        Dim sql As String = String.Format("UPDATE BasalesPediatria SET PrimerNombre = '{1}', SegundoNombre = '{2}', PrimerApellido = '{3}', SegundoApellido = '{4}', Genero = {5}, FechaNacimiento = '{6}', Telefono = '{7}', Direccion = '{8}', IdBaja = {9} WHERE NHC = '{0}'", d(0).ToString(), d(1).ToString(), d(2).ToString(), d(3).ToString(), d(4).ToString(), d(5).ToString(), d(6).ToString(), d(7).ToString(), d(8).ToString(), d(9).ToString())
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page + "_" + nhc
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
        End Try
    End Sub

    '*Agrega Basal Pediatrico*//
    Public Function AgregaBPediatrico(ByVal nhc As String, ByVal datos As String, ByVal usuario As String) As String
        _page = "db.AgregaBPediatrico"
        Dim sql As String
        Dim str As String
        Dim x As Boolean = False
        sql = "SELECT NHC FROM BasalesPediatria WHERE NHC = '" & nhc & "'"
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                'If reader IsNot Nothing Then
                If reader.HasRows Then
                    x = True
                Else
                    x = False
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            str = "False|" + ex.Message
        End Try
        If x Then
            str = "False|El NHC pediatrico ya existe, revise porfavor."
        Else
            Dim d As String() = datos.Split("|")
            sql = String.Format("INSERT INTO BasalesPediatria (NHC, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, Genero, FechaNacimiento, Telefono, Direccion, IdBaja) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', {5}, '{6}', '{7}', '{8}', {9})", d(0).ToString(), d(1).ToString(), d(2).ToString(), d(3).ToString(), d(4).ToString(), d(5).ToString(), d(6).ToString(), d(7).ToString(), d(8).ToString(), d(9).ToString())
            Try
                Using connection As New SqlConnection(_cn1)
                    connection.Open()
                    Dim command As New SqlCommand(sql, connection)
                    command.ExecuteNonQuery()
                    command.Dispose()
                    connection.Dispose()
                    connection.Close()
                End Using
                str = "True|Ok"
            Catch ex As SqlException
                _error = ex.Message
                _pageO = _page + "_" + nhc
                GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
                str = "False|" + ex.Message
            End Try
        End If
        Return str
    End Function

    '*Graba Errores*//
    Public Sub GrabarErrores(ByVal Errores As String)
        Try
            Dim r As String() = Errores.Split("|")
            Dim r3 As String = r(3).Replace("'", "").ToString()
            Using connection As New SqlConnection(ConfigurationManager.ConnectionStrings("conStringTS").ConnectionString)
                Dim sql As String = ""
                sql = String.Format("insert into logEBD(IdUsuario,Pagina,Error,ErrorMsg) values('{0}', '{1}', '{2}', '{3}')", r(0), r(1), r(2), r(3))
                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            Dim r As String() = Errores.Split("|")
            Dim narchivo As String = "TS_EBD"
            Dim path As String = ConfigurationManager.AppSettings("LogEBD")
            Dim archivo As String = (path & narchivo) & System.DateTime.Now.Year.ToString() & System.DateTime.Now.Month.ToString("00") & System.DateTime.Now.Day.ToString("00") & ".txt"
            Dim sw As New StreamWriter(archivo, True)
            sw.WriteLine(System.DateTime.Now.ToLongTimeString() & " - " & r(0) & " - Página: " & r(1))
            sw.WriteLine("Error No.: " & r(2))
            sw.WriteLine("Error Mensaje: " & r(3))
            sw.WriteLine("---------------------------------------------------")
            sw.WriteLine("Error Base de Datos al Grabar")
            sw.WriteLine("Error: " & ex.Number)
            sw.WriteLine("Error Mensaje: " & ex.Message)
            sw.WriteLine("---------------------------------------------------")
            sw.Close()
        End Try
    End Sub

    Public Function Grabar_basal_ts(ByVal idpac As String, ByVal nhc As String, ByVal fechaestudiose As String, ByVal observaciones As String, ByVal circuito As String, ByVal usuario As String) As String
        _page = "db.GrabaBasal"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                Using insertCommand As SqlCommand = connection.CreateCommand()

                    Q.Append("INSERT INTO dbo.PAC_BASALES_TS (IdPaciente, NHC, Fecha_estudioSE,  Observaciones, Circuito, Usuario)")
                    Query = Q.ToString() + String.Format("VALUES ({0}, '{1}', convert(datetime,'{2}',103), '{3}', {4},'{5}')", idpac, nhc, fechaestudiose, If(String.IsNullOrEmpty(observaciones.ToString()), "", observaciones.ToString()), If(String.IsNullOrEmpty(circuito.ToString()), "Null", circuito.ToString()), usuario)

                    insertCommand.CommandText = Query + " SET @ID = SCOPE_IDENTITY()"
                    Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                    IDParameter.Direction = ParameterDirection.Output
                    insertCommand.Parameters.Add(IDParameter)
                    insertCommand.Connection.Open()
                    insertCommand.ExecuteNonQuery()
                    id = IDParameter.Value

                End Using
            End Using
            X = id
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = String.Empty
        End Try
        Return X
    End Function

    Public Function Grabar_personaDX(ByVal idbasalests As String, ByVal personadx As String, ByVal usuario As String) As String
        _page = "db.Grabar_personaDX"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                Using insertCommand As SqlCommand = connection.CreateCommand()

                    Q.Append("INSERT INTO dbo.PAC_BASALES_PERSONASDX (IdBasalesTS, PersonasDx, Usuario)")
                    Query = Q.ToString() + String.Format("VALUES ({0}, {1}, '{2}')", idbasalests, personadx, usuario)

                    insertCommand.CommandText = Query + " SET @ID = SCOPE_IDENTITY()"
                    Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                    IDParameter.Direction = ParameterDirection.Output
                    insertCommand.Parameters.Add(IDParameter)
                    insertCommand.Connection.Open()
                    insertCommand.ExecuteNonQuery()
                    id = IDParameter.Value

                End Using
            End Using
            X = id
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = String.Empty
        End Try
        Return X
    End Function

    Public Function Condiciones_vivienda(ByVal idbasalests As String, ByVal numambientes As String, ByVal usuario As String) As String
        _page = "db.Condiciones_vivienda"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                Using insertCommand As SqlCommand = connection.CreateCommand()

                    Q.Append("INSERT INTO dbo.PAC_CONDICIONESVIVIENDA (IdBasalesTS, NumAmbientes, Usuario)")
                    Query = Q.ToString() + String.Format("VALUES ({0}, {1}, '{2}')", idbasalests, If(String.IsNullOrEmpty(numambientes.ToString()), "Null", numambientes.ToString()), usuario)

                    insertCommand.CommandText = Query + " SET @ID = SCOPE_IDENTITY()"
                    Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                    IDParameter.Direction = ParameterDirection.Output
                    insertCommand.Parameters.Add(IDParameter)
                    insertCommand.Connection.Open()
                    insertCommand.ExecuteNonQuery()
                    id = IDParameter.Value

                End Using
            End Using
            X = id
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = String.Empty
        End Try
        Return X
    End Function

    Public Function Tipo_vivienda(ByVal idbasalests As String, ByVal idcondicionesvivienda As String, ByVal tipovivienda As String, ByVal usuario As String) As String
        _page = "db.Tipo_vivienda"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                Using insertCommand As SqlCommand = connection.CreateCommand()

                    Q.Append("INSERT INTO dbo.PAC_CONDICIONES_TIPOVIVIENDA (IdBasalesTS, IdCondicionesVivienda, TipoVivienda, Usuario)")
                    Query = Q.ToString() + String.Format("VALUES ({0}, {1}, {2}, '{3}')", idbasalests, idcondicionesvivienda, tipovivienda, usuario)

                    insertCommand.CommandText = Query + " SET @ID = SCOPE_IDENTITY()"
                    Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                    IDParameter.Direction = ParameterDirection.Output
                    insertCommand.Parameters.Add(IDParameter)
                    insertCommand.Connection.Open()
                    insertCommand.ExecuteNonQuery()
                    id = IDParameter.Value

                End Using
            End Using
            X = id
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = String.Empty
        End Try
        Return X
    End Function

    Public Function Tipo_construccion(ByVal idbasalests As String, ByVal idcondicionesvivienda As String, ByVal tipoconstruccion As String, ByVal usuario As String) As String
        _page = "db.Tipo_construccion"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                Using insertCommand As SqlCommand = connection.CreateCommand()

                    Q.Append("INSERT INTO dbo.PAC_CONDICIONES_TIPOCONSTRUCCION (IdBasalesTS, IdCondicionesVivienda, TipoConstruccion, Usuario)")
                    Query = Q.ToString() + String.Format("VALUES ({0}, {1}, {2}, '{3}')", idbasalests, idcondicionesvivienda, tipoconstruccion, usuario)

                    insertCommand.CommandText = Query + " SET @ID = SCOPE_IDENTITY()"
                    Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                    IDParameter.Direction = ParameterDirection.Output
                    insertCommand.Parameters.Add(IDParameter)
                    insertCommand.Connection.Open()
                    insertCommand.ExecuteNonQuery()
                    id = IDParameter.Value

                End Using
            End Using
            X = id
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = String.Empty
        End Try
        Return X
    End Function

    Public Function Servicios_vivienda(ByVal idbasalests As String, ByVal idcondicionesvivienda As String, ByVal servicios As String, ByVal usuario As String) As String
        _page = "db.Servicios_vivienda"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                Using insertCommand As SqlCommand = connection.CreateCommand()

                    Q.Append("INSERT INTO dbo.PAC_CONDICIONES_SERVICIOS (IdBasalesTS, IdCondicionesVivienda, Servicios, Usuario)")
                    Query = Q.ToString() + String.Format("VALUES ({0}, {1}, {2}, '{3}')", idbasalests, idcondicionesvivienda, servicios, usuario)

                    insertCommand.CommandText = Query + " SET @ID = SCOPE_IDENTITY()"
                    Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                    IDParameter.Direction = ParameterDirection.Output
                    insertCommand.Parameters.Add(IDParameter)
                    insertCommand.Connection.Open()
                    insertCommand.ExecuteNonQuery()
                    id = IDParameter.Value

                End Using
            End Using
            X = id
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = String.Empty
        End Try
        Return X
    End Function

    Public Function Prob_identificados(ByVal idbasalests As String, ByVal probidentificados As String, ByVal usuario As String) As String
        _page = "db.Prob_identificados"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                Using insertCommand As SqlCommand = connection.CreateCommand()

                    Q.Append("INSERT INTO dbo.PAC_BASALES_PROBIDENTIFICADOS (IdBasalesTS, ProbIdentificados, Usuario)")
                    Query = Q.ToString() + String.Format("VALUES ({0}, {1}, '{2}')", idbasalests, probidentificados, usuario)

                    insertCommand.CommandText = Query + " SET @ID = SCOPE_IDENTITY()"
                    Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                    IDParameter.Direction = ParameterDirection.Output
                    insertCommand.Parameters.Add(IDParameter)
                    insertCommand.Connection.Open()
                    insertCommand.ExecuteNonQuery()
                    id = IDParameter.Value

                End Using
            End Using
            X = id
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = String.Empty
        End Try
        Return X
    End Function




    Public Function Insert_ingresos(ByVal datos As String, ByVal usuario As String) As String

        _page = "db.Insert_ingresos"
        Dim sql As String = String.Empty
        Dim X As String = String.Empty
        Dim d As String() = datos.Split("|")
        Dim id As String
        ' Dim sqldatenull As SqlDateTime
        ' sqldatenull = SqlDateTime.Null
        Dim sqldatenull As SqlDateTime
        sqldatenull = SqlDateTime.Null
        Dim intnull As DBNull
        intnull = Nothing
        Try
            Using connection As New SqlConnection(_cn1)
                Using insertCommand As SqlCommand = connection.CreateCommand()
                    Dim Q As New StringBuilder()
                    ''Q.Append("INSERT INTO dbo.TV_Basal (CodigoOrientadorPre, RegistroOrientacion, FechaOrientacion, NoHC, IdTipoUsuario, IdCohorte, IdServicio, Cama, VecesVisitaEMA, TiempoUltimaVisita, IdEstadoConciencia, MotivoVisitaHospital, Iniciales, ZonaResindencia, IdMunicipioResidencia, IdDepartamentoResidencia, IdPaisResidencia, IdPaisNacimiento, IdDepartamentoNacimiento, FechaNacimiento, EdadAños, EdadMeses, IdEtnia, OtroEtnia, IdComunidadLinguistica, IdGenero, IdEstadoCivil, Hijos, CuantosHijos, EdadHijoMayor, EdadHijoMenor, IdReligion, OtraReligion, SabeLeer, SabeEscribir, IdEscolaridad, GradosDeEstudio, Carrera, Ocupacion, TrabajaActualmente, Salario, QuienAportaSalario, IngresoTotalHogar, IdSituacionLaboral, IdOrientacionSexual, TiempoUltimaRelacion, NumeroParejasSexuales, ParejaOcasional, IdCitaParejaOcasional, HaTenidoUnaITS, RiesgoInfecVIH, RiesgoInfecITS, EnfermedadCronica, CualEnfCronica, CuandoEnfCronica, PruebaAnteriorVIH, FechaPruebaAnterior, PreOrientacionHoy, Tamizaje1, FechaTamizaje1, Tamizaje2, FechaTamizaje2, ELISA, FechaELISA, WesternBlot, FechaWesternBlot, ResultadoFinal, FechaResultadoFinal, PostOrientacionHoy, POPorque, CodigoPostOrientador, PacienteVinculadoTratamiento, LugarVinculoTratamiento, TerapiaPreviaARV, LugarTerapiaPreviaARV, NoASIAnterior)")
                    'Q.Append("INSERT INTO TVC_Basal (CodigoOrientadorPre, RegistroOrientacion, FechaOrientacion, NoHC, IdTipoUsuario, IdCohorte, IdServicio, Cama, VecesVisitaEMA, TiempoUltimaVisita, IdEstadoConciencia, ")
                    'Q.Append("MotivoVisitaHospital, Iniciales, ZonaResindencia, IdMunicipioResidencia, IdDepartamentoResidencia, IdPaisResidencia, IdPaisNacimiento, IdDepartamentoNacimiento, FechaNacimiento, EdadAños, ")
                    'Q.Append("EdadMeses, IdEtnia, OtroEtnia, IdComunidadLinguistica, IdGenero, IdEstadoCivil, Hijos, CuantosHijos, EdadHijoMayor, EdadHijoMenor, ")
                    'Q.Append("IdReligion, OtraReligion, SabeLeer, SabeEscribir, IdEscolaridad, GradosDeEstudio, Carrera, Ocupacion, TrabajaActualmente, Salario, ")
                    'Q.Append("QuienAportaSalario, IngresoTotalHogar, IdSituacionLaboral, IdOrientacionSexual, TiempoUltimaRelacion, NumeroParejasSexuales, ParejaOcasional, IdCitaParejaOcasional, HaTenidoUnaITS, RiesgoInfecVIH, ")
                    'Q.Append("RiesgoInfecITS, EnfermedadCronica, CualEnfCronica, CuandoEnfCronica, PruebaAnteriorVIH, FechaPruebaAnterior, PreOrientacionHoy, Tamizaje1, FechaTamizaje1, Tamizaje2, ")
                    'Q.Append("FechaTamizaje2, ELISA, FechaELISA, WesternBlot, FechaWesternBlot, ResultadoFinal, FechaResultadoFinal, PostOrientacionHoy, POPorque, CodigoPostOrientador, ")
                    'Q.Append("PacienteVinculadoTratamiento, LugarVinculoTratamiento, TerapiaPreviaARV, LugarTerapiaPreviaARV, NoASIAnterior, Usuario) ")
                    'sql = Q.ToString() & String.Format("VALUES({0}, '{1}', '{2}', '{3}', {4}, {5}, {6}, {7}, {8}, '{9}', {10}, {11}, '{12}', {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, '{23}', {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}, '{32}', {33}, {34}, {35}, {36}, '{37}', {38}, {39}, {40}, '{41}', {42}, {43}, {44}, '{45}', {46}, {47}, {48}, {49}, {50}, {51}, {52}, '{53}', '{54}', {55}, {56}, {57}, {58}, {59}, {60}, {61}, {62}, {63}, {64}, {65}, {66}, {67}, {68}, '{69}', {70}, {71}, '{72}', {73}, '{74}', '{75}', '{76}')", d(0).ToString(), d(1).ToString(), d(2).ToString(), d(3).ToString(), d(4).ToString(), d(5).ToString(), d(6).ToString(), If(String.IsNullOrEmpty(d(7).ToString()), "Null", d(7).ToString()), If(String.IsNullOrEmpty(d(8).ToString()), "Null", d(8).ToString()), d(9).ToString(), d(10).ToString(), d(11).ToString(), d(12).ToString(), If(String.IsNullOrEmpty(d(13).ToString()), "Null", d(13).ToString()), d(14).ToString(), d(15).ToString(), d(16).ToString(), d(17).ToString(), d(18).ToString(), IIf(String.IsNullOrEmpty(d(19).ToString()), sqldatenull, "'" & d(19).ToString() & "'"), If(String.IsNullOrEmpty(d(20).ToString()), "Null", d(20).ToString()), If(String.IsNullOrEmpty(d(21).ToString()), "Null", d(21).ToString()), d(22).ToString(), d(23).ToString(), d(24).ToString(), d(25).ToString(), d(26).ToString(), d(27).ToString(), If(String.IsNullOrEmpty(d(28).ToString()), "Null", d(28).ToString()), If(String.IsNullOrEmpty(d(29).ToString()), "Null", d(29).ToString()), If(String.IsNullOrEmpty(d(30).ToString()), "Null", d(30).ToString()), d(31).ToString(), d(32).ToString(), d(33).ToString(), d(34).ToString(), d(35).ToString(), If(String.IsNullOrEmpty(d(36).ToString()), "Null", d(36).ToString()), d(37).ToString(), d(38).ToString(), d(39).ToString(), If(String.IsNullOrEmpty(d(40).ToString()), "Null", d(40).ToString()), d(41).ToString(), If(String.IsNullOrEmpty(d(42).ToString()), "Null", d(42).ToString()), d(43).ToString(), d(44).ToString(), d(45).ToString(), If(String.IsNullOrEmpty(d(46).ToString()), "Null", d(46).ToString()), d(47).ToString(), d(48).ToString(), d(49).ToString(), d(50).ToString(), d(51).ToString(), d(52).ToString(), d(53).ToString(), d(54).ToString(), d(55).ToString(), IIf(String.IsNullOrEmpty(d(56).ToString()), sqldatenull, "'" & d(56).ToString() & "'"), d(57).ToString(), d(58).ToString(), IIf(String.IsNullOrEmpty(d(59).ToString()), sqldatenull, "'" & d(59).ToString() & "'"), d(60).ToString(), IIf(String.IsNullOrEmpty(d(61).ToString()), sqldatenull, "'" & d(61).ToString() & "'"), d(62).ToString(), IIf(String.IsNullOrEmpty(d(63).ToString()), sqldatenull, "'" & d(63).ToString() & "'"), d(64).ToString(), IIf(String.IsNullOrEmpty(d(65).ToString()), sqldatenull, "'" & d(65).ToString() & "'"), d(66).ToString(), IIf(String.IsNullOrEmpty(d(67).ToString()), sqldatenull, "'" & d(67).ToString() & "'"), d(68).ToString(), d(69).ToString(), If(String.IsNullOrEmpty(d(70).ToString()), "Null", d(70).ToString()), d(71).ToString(), d(72).ToString(), d(73).ToString(), d(74).ToString(), If(String.IsNullOrEmpty(d(75).ToString()), "Null", d(75).ToString()), d(76).ToString())
                    Q.Append("INSERT INTO dbo.PAC_INGRESOS (IdBasalesTS, Ingresos, IngresosHogar, EgresoVivienda, EgresoEE, EgresoAgua, EgresoCable, ")
                    Q.Append("EgresoTelefono, EgresoAlimentacion, EgresoTransporte, EgresoEducacion, EgresoBasura, EgresoOtros, TotalIngresos, TotalEgresos, ")
                    Q.Append("DeficitSuperavit, Usuario)")
                    sql = Q.ToString() & String.Format("VALUES({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}' )", d(0).ToString(), d(1).ToString(), d(2).ToString(), d(3).ToString(), d(4).ToString(), d(5).ToString(), d(6).ToString(), d(7).ToString(), d(8).ToString(), d(9).ToString(), d(10).ToString(), d(11).ToString(), d(12).ToString(), d(13).ToString(), d(14).ToString(), d(15).ToString(), d(16).ToString())

                    connection.Open()
                    Dim command As New SqlCommand(sql, connection)
                    command.ExecuteNonQuery()
                    command.Dispose()
                    connection.Dispose()
                    connection.Close()
                    'insertCommand.CommandText = sql + " SET @ID = SCOPE_IDENTITY()"
                    'Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                    'IDParameter.Direction = ParameterDirection.Output
                    'insertCommand.Parameters.Add(IDParameter)
                    'insertCommand.Connection.Open()
                    'insertCommand.ExecuteNonQuery()
                    'id = IDParameter.Value
                End Using
            End Using
            'X = id
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = String.Empty
        End Try
        Return X
    End Function


    'Public Function Search_BasalTS(ByRef op As Integer, ByRef idTVC As String, ByVal usuario As String) As DataTable
    '    _page = "db.Tipo_its"
    '    Dim Q As String
    '    Dim Ds As New DataSet()
    '    Select Case op
    '        Case 1
    '            Q = String.Format("SELECT I.IdITS  FROM dbo.TVC_Basal_Registros AS B LEFT OUTER JOIN  dbo.ITS AS I ON B.IdOp = I.IdITS  WHERE B.IdTVCBasal = {0} AND B.IdS_TB = 4", idTVC)
    '        Case 2
    '            Q = String.Format("SELECT B.Descripcion FROM dbo.TVC_Basal_Registros AS B LEFT OUTER JOIN dbo.ITS AS I ON B.IdOp = I.IdITS WHERE B.IdTVCBasal = {0} AND B.IdS_TB = 4 AND I.IdITS = 5", idTVC)
    '    End Select

    '    Try
    '        Using connection As New SqlConnection(Me._cn1)
    '            connection.Open()
    '            Dim adapter As New SqlDataAdapter()
    '            adapter.SelectCommand = New SqlCommand(Q, connection)
    '            adapter.SelectCommand.CommandTimeout = TimeoutDB
    '            adapter.Fill(Ds, _page)
    '            adapter.Dispose()
    '            connection.Dispose()
    '            connection.Close()
    '        End Using
    '        Return Ds.Tables(0)
    '    Catch ex As SqlException
    '        _error = ex.Message
    '        _pageO = _page
    '        GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
    '        Return Nothing
    '    End Try
    'End Function

    Public Function Search_BasalTS(ByRef idts As String, ByVal usuario As String) As DataTable
        _page = "db.Search_BasalTS"
        Dim Q As String
        Dim Ds As New DataSet()
        Q = String.Format("SELECT B.IdBasalesTS , convert(VARCHAR,b.Fecha_estudioSE ,103) AS 'Fecha_estudioSE', B.Observaciones, B.Circuito  FROM PAC_BASALES_TS AS B WHERE B.IdBasalesTS = {0} ", idts)
        Try
            Using connection As New SqlConnection(Me._cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Q, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function Search_Ingresos(ByRef idts As String, ByVal usuario As String) As DataTable
        _page = "db.Search_Ingresos"
        Dim Q As String
        Dim Ds As New DataSet()
        'Q = String.Format("SELECT I.IdBasalesTS, CAST(I.Ingresos  AS DECIMAL (17,2)) AS 'Ingresos', CAST(I.IngresosHogar AS DECIMAL (17,2)) AS 'IngresosHogar' , CAST(I.EgresoVivienda AS DECIMAL (17,2)) AS 'EgresoVivienda', CAST(I.EgresoEE AS DECIMAL (17,2)) AS 'EgresoEE' , CAST(I.EgresoAgua AS DECIMAL (17,2)) AS 'EgresoAgua' , CAST(I.EgresoCable AS DECIMAL (17,2)) AS 'EgresoCable' , CAST(I.EgresoTelefono AS DECIMAL (17,2)) AS 'EgresoTelefono',  CAST(I.EgresoAlimentacion AS DECIMAL (17,2)) AS 'EgresoAlimientacion', CAST(I.EgresoTransporte AS DECIMAL (17,2))AS 'EgresoTransporte' , CAST(I.EgresoEducacion AS DECIMAL (17,2)) AS 'EgresoEducacion', CAST(I.EgresoBasura AS DECIMAL (17,2)) AS 'EgresoBasura' , CAST(I.EgresoOtros AS DECIMAL (17,2)) AS 'EgresoOtros' , CAST(I.TotalIngresos AS DECIMAL (17,2)) AS 'TotalIngresos' , CAST(I.TotalEgresos AS DECIMAL (17,2)) AS 'TotalEgresos' , CAST(I.DeficitSuperavit AS DECIMAL (17,2)) AS 'DeficitSuperavit' FROM PAC_INGRESOS AS I WHERE I.IdBasalesTS =  {0}", idts)
        Q = String.Format("SELECT I.IdBasalesTS, CAST(I.Ingresos  AS DECIMAL (17,2)), CAST(I.IngresosHogar AS DECIMAL (17,2)), CAST(I.EgresoVivienda AS DECIMAL (17,2)), CAST(I.EgresoEE AS DECIMAL (17,2)), CAST(I.EgresoAgua AS DECIMAL (17,2)), CAST(I.EgresoCable AS DECIMAL (17,2)), CAST(I.EgresoTelefono AS DECIMAL (17,2)),  CAST(I.EgresoAlimentacion AS DECIMAL (17,2)), CAST(I.EgresoTransporte AS DECIMAL (17,2)), CAST(I.EgresoEducacion AS DECIMAL (17,2)), CAST(I.EgresoBasura AS DECIMAL (17,2)), CAST(I.EgresoOtros AS DECIMAL (17,2)), CAST(I.TotalIngresos AS DECIMAL (17,2)), CAST(I.TotalEgresos AS DECIMAL (17,2)), CAST(I.DeficitSuperavit AS DECIMAL (17,2)) FROM PAC_INGRESOS AS I WHERE I.IdBasalesTS =  {0}", idts)
        Try
            Using connection As New SqlConnection(Me._cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Q, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function ProbIden(ByVal idts As String, ByVal usuario As String) As DataTable
        _page = "db.ProbIden"
        Dim Q As String
        Dim Ds As New DataSet()

        Q = String.Format("SELECT IdBasalesTS, ProbIdentificados FROM PAC_BASALES_PROBIDENTIFICADOS WHERE IdBasalesTS = {0}", idts)

        Try
            Using connection As New SqlConnection(Me._cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Q, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function CondVivi(ByVal idts As String, ByVal usuario As String) As DataTable
        _page = "db.CondVivi"
        Dim Q As String
        Dim Ds As New DataSet()

        Q = String.Format("SELECT IdBasalesTS, NumAmbientes FROM PAC_CONDICIONESVIVIENDA WHERE IdBasalesTS = {0}", idts)

        Try
            Using connection As New SqlConnection(Me._cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Q, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function TipoVivi(ByVal idts As String, ByVal usuario As String) As DataTable
        _page = "db.TipoVivi"
        Dim Q As String
        Dim Ds As New DataSet()

        Q = String.Format("SELECT IdBasalesTS, IdCondicionesVivienda , TipoVivienda FROM PAC_CONDICIONES_TIPOVIVIENDA  WHERE IdBasalesTS = {0}", idts)

        Try
            Using connection As New SqlConnection(Me._cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Q, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function Servicios_vi(ByVal idts As String, ByVal usuario As String) As DataTable
        _page = "db.Servicios_vi"
        Dim Q As String
        Dim Ds As New DataSet()

        Q = String.Format("SELECT IdBasalesTS, IdCondicionesVivienda, Servicios FROM PAC_CONDICIONES_SERVICIOS WHERE IdBasalesTS = {0}", idts)

        Try
            Using connection As New SqlConnection(Me._cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Q, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function Tipo_constru(ByVal idts As String, ByVal usuario As String) As DataTable
        _page = "db.Tipo_constru"
        Dim Q As String
        Dim Ds As New DataSet()

        Q = String.Format("SELECT IdBasalesTS, IdCondicionesVivienda, TipoConstruccion FROM PAC_CONDICIONES_TIPOCONSTRUCCION WHERE IdBasalesTS = {0}", idts)

        Try
            Using connection As New SqlConnection(Me._cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Q, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function Sabe_diag(ByVal idts As String, ByVal usuario As String) As DataTable
        _page = "db.Sabe_diag"
        Dim Q As String
        Dim Ds As New DataSet()

        Q = String.Format("SELECT IdBasalesTS, PersonasDx  FROM PAC_BASALES_PERSONASDX  WHERE IdBasalesTS = {0}", idts)

        Try
            Using connection As New SqlConnection(Me._cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Q, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Sub Insert_Ingresos_cambios(ByVal datos As String, ByVal idts As String, ByVal usuario As String)
        _page = "db.Insert_Ingresos_cambios"
        Dim sql As String = ""
        'Dim X As String = String.Empty
        Dim d As String() = datos.Split("|")
        'Dim id As String
        Dim sqldatenull As SqlDateTime
        sqldatenull = SqlDateTime.Null
        'Dim intnull As DBNull
        'intnull = Nothing
        Try
            Using connection As New SqlConnection(_cn1)
                'Using insertCommand As SqlCommand = connection.CreateCommand()

                'Using insertCommand As SqlCommand = connection.CreateCommand()
                ' Dim Q As New StringBuilder()
                'Q.Append("INSERT INTO dbo.TV_Basal (CodigoOrientadorPre, RegistroOrientacion, FechaOrientacion, NoHC, IdTipoUsuario, IdCohorte, IdServicio, Cama, VecesVisitaEMA, TiempoUltimaVisita, IdEstadoConciencia, MotivoVisitaHospital, Iniciales, ZonaResindencia, IdMunicipioResidencia, IdDepartamentoResidencia, IdPaisResidencia, IdPaisNacimiento, IdDepartamentoNacimiento, FechaNacimiento, EdadAños, EdadMeses, IdEtnia, OtroEtnia, IdComunidadLinguistica, IdGenero, IdEstadoCivil, Hijos, CuantosHijos, EdadHijoMayor, EdadHijoMenor, IdReligion, OtraReligion, SabeLeer, SabeEscribir, IdEscolaridad, GradosDeEstudio, Carrera, Ocupacion, TrabajaActualmente, Salario, QuienAportaSalario, IngresoTotalHogar, IdSituacionLaboral, IdOrientacionSexual, TiempoUltimaRelacion, NumeroParejasSexuales, ParejaOcasional, IdCitaParejaOcasional, HaTenidoUnaITS, RiesgoInfecVIH, RiesgoInfecITS, EnfermedadCronica, CualEnfCronica, CuandoEnfCronica, PruebaAnteriorVIH, FechaPruebaAnterior, PreOrientacionHoy, Tamizaje1, FechaTamizaje1, Tamizaje2, FechaTamizaje2, ELISA, FechaELISA, WesternBlot, FechaWesternBlot, ResultadoFinal, FechaResultadoFinal, PostOrientacionHoy, POPorque, CodigoPostOrientador, PacienteVinculadoTratamiento, LugarVinculoTratamiento, TerapiaPreviaARV, LugarTerapiaPreviaARV, NoASIAnterior)")
                sql = String.Format("UPDATE dbo.PAC_INGRESOS SET Ingresos = '{0}', IngresosHogar = '{1}', EgresoVivienda = '{2}', EgresoEE = '{3}', EgresoAgua = '{4}', EgresoCable = '{5}', EgresoTelefono = '{6}', EgresoAlimentacion = '{7}', EgresoTransporte = '{8}', EgresoEducacion = '{9}', EgresoBasura = '{10}', ", d(0).ToString(), d(1).ToString(), d(2).ToString(), d(3).ToString(), d(4).ToString(), d(5).ToString(), d(6).ToString(), d(7).ToString(), d(8).ToString(), d(9).ToString(), d(10).ToString())
                sql += String.Format("EgresoOtros = '{0}', TotalIngresos = '{1}', TotalEgresos = '{2}', DeficitSuperavit = '{3}', Usuario = '{4}'", d(11).ToString(), d(12).ToString(), d(13).ToString(), d(14).ToString(), d(15).ToString())
                sql += String.Format("WHERE IdBasalesTS = {0}", idts)

                connection.Open()
                Dim command As New SqlCommand(sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()

                'insertCommand.CommandText = sql + " SET @ID = SCOPE_IDENTITY()"
                'Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                'IDParameter.Direction = ParameterDirection.Output
                'insertCommand.Parameters.Add(IDParameter)
                'insertCommand.Connection.Open()
                'insertCommand.ExecuteNonQuery()
                'id = IDParameter.Value
            End Using
            'End Using
            'X = id
            'X = "1"
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            'X = String.Empty
            'X = "0"
        End Try
        'Return X
    End Sub


    Public Function delete_registros_GrupoFamiliar(ByVal idts As String, ByVal usuario As String) As String
        _page = "db.delete_registros_GrupoFamiliar"
        'Dim Q As String
        'Dim Ds As New DataSet()
        Dim Sql As String = String.Empty
        'Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                ' Using insertCommand As SqlCommand = connection.CreateCommand()
                Dim Q As New StringBuilder()
                Q.Append("DELETE FROM dbo.PAC_GRUPOFAMILIAR WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOVIVIENDA WHERE IdBasalesTS = '" & idts & "'")
                ' Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOCONSTRUCCION WHERE IdBasalesTS = '" & idts & "'")
                ' Q.Append("DELETE FROM dbo.PAC_CONDICIONES_SERVICIOS WHERE IdBasalesTS = '" & idts & "'")
                ' Q.Append("DELETE FROM dbo.PAC_BASALES_PROBIDENTIFICADOS WHERE IdBasalesTS = '" & idts & "'")
                Sql = Q.ToString()
                'insertCommand.CommandText = Sql + " SET @ID = SCOPE_IDENTITY()"
                'Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                'IDParameter.Direction = ParameterDirection.Output
                'insertCommand.Parameters.Add(IDParameter)
                'insertCommand.Connection.Open()
                'insertCommand.ExecuteNonQuery()
                'id = IDParameter.Value
                connection.Open()
                Dim command As New SqlCommand(Sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            'End Using
            'X = id

        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
        Return X
    End Function


    Public Function delete_registros_personadx(ByVal idts As String, ByVal usuario As String) As String
        _page = "db.delete_registros_personadx"
        'Dim Q As String
        'Dim Ds As New DataSet()
        Dim Sql As String = String.Empty
        'Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                ' Using insertCommand As SqlCommand = connection.CreateCommand()
                Dim Q As New StringBuilder()
                Q.Append("DELETE FROM dbo.PAC_BASALES_PERSONASDX WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOVIVIENDA WHERE IdBasalesTS = '" & idts & "'")
                ' Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOCONSTRUCCION WHERE IdBasalesTS = '" & idts & "'")
                ' Q.Append("DELETE FROM dbo.PAC_CONDICIONES_SERVICIOS WHERE IdBasalesTS = '" & idts & "'")
                ' Q.Append("DELETE FROM dbo.PAC_BASALES_PROBIDENTIFICADOS WHERE IdBasalesTS = '" & idts & "'")
                Sql = Q.ToString()
                'insertCommand.CommandText = Sql + " SET @ID = SCOPE_IDENTITY()"
                'Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                'IDParameter.Direction = ParameterDirection.Output
                'insertCommand.Parameters.Add(IDParameter)
                'insertCommand.Connection.Open()
                'insertCommand.ExecuteNonQuery()
                'id = IDParameter.Value
                connection.Open()
                Dim command As New SqlCommand(Sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            'End Using
            'X = id

        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
        Return X
    End Function
    Public Function delete_registros_tipovi(ByVal idts As String, ByVal usuario As String) As String
        _page = "db.delete_registros_tipovi"
        'Dim Q As String
        'Dim Ds As New DataSet()
        Dim Sql As String = String.Empty
        'Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                ' Using insertCommand As SqlCommand = connection.CreateCommand()
                Dim Q As New StringBuilder()
                'Q.Append("DELETE FROM dbo.PAC_BASALES_PERSONASDX WHERE IdBasalesTS = '" & idts & "'")
                Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOVIVIENDA WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOCONSTRUCCION WHERE IdBasalesTS = '" & idts & "'")
                ' Q.Append("DELETE FROM dbo.PAC_CONDICIONES_SERVICIOS WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_BASALES_PROBIDENTIFICADOS WHERE IdBasalesTS = '" & idts & "'")
                Sql = Q.ToString()
                'insertCommand.CommandText = Sql + " SET @ID = SCOPE_IDENTITY()"
                'Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                'IDParameter.Direction = ParameterDirection.Output
                'insertCommand.Parameters.Add(IDParameter)
                'insertCommand.Connection.Open()
                'insertCommand.ExecuteNonQuery()
                'id = IDParameter.Value
                connection.Open()
                Dim command As New SqlCommand(Sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            'End Using
            'X = id

        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
        Return X
    End Function

    Public Function delete_registros_tipoconst(ByVal idts As String, ByVal usuario As String) As String
        _page = "db.delete_registros_tipoconst"
        'Dim Q As String
        'Dim Ds As New DataSet()
        Dim Sql As String = String.Empty
        'Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                ' Using insertCommand As SqlCommand = connection.CreateCommand()
                Dim Q As New StringBuilder()
                'Q.Append("DELETE FROM dbo.PAC_BASALES_PERSONASDX WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOVIVIENDA WHERE IdBasalesTS = '" & idts & "'")
                Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOCONSTRUCCION WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_SERVICIOS WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_BASALES_PROBIDENTIFICADOS WHERE IdBasalesTS = '" & idts & "'")
                Sql = Q.ToString()
                'insertCommand.CommandText = Sql + " SET @ID = SCOPE_IDENTITY()"
                'Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                'IDParameter.Direction = ParameterDirection.Output
                'insertCommand.Parameters.Add(IDParameter)
                'insertCommand.Connection.Open()
                'insertCommand.ExecuteNonQuery()
                'id = IDParameter.Value
                connection.Open()
                Dim command As New SqlCommand(Sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            'End Using
            'X = id

        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
        Return X
    End Function
    Public Function delete_registros_servicios(ByVal idts As String, ByVal usuario As String) As String
        _page = "db.delete_registros_servicios"
        'Dim Q As String
        'Dim Ds As New DataSet()
        Dim Sql As String = String.Empty
        'Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                ' Using insertCommand As SqlCommand = connection.CreateCommand()
                Dim Q As New StringBuilder()
                'Q.Append("DELETE FROM dbo.PAC_BASALES_PERSONASDX WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOVIVIENDA WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOCONSTRUCCION WHERE IdBasalesTS = '" & idts & "'")
                Q.Append("DELETE FROM dbo.PAC_CONDICIONES_SERVICIOS WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_BASALES_PROBIDENTIFICADOS WHERE IdBasalesTS = '" & idts & "'")
                Sql = Q.ToString()
                'insertCommand.CommandText = Sql + " SET @ID = SCOPE_IDENTITY()"
                'Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                'IDParameter.Direction = ParameterDirection.Output
                'insertCommand.Parameters.Add(IDParameter)
                'insertCommand.Connection.Open()
                'insertCommand.ExecuteNonQuery()
                'id = IDParameter.Value
                connection.Open()
                Dim command As New SqlCommand(Sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            'End Using
            'X = id

        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
        Return X
    End Function
    Public Function delete_registros_prob(ByVal idts As String, ByVal usuario As String) As String
        _page = "db.delete_registros_prob"
        'Dim Q As String
        'Dim Ds As New DataSet()
        Dim Sql As String = String.Empty
        'Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                ' Using insertCommand As SqlCommand = connection.CreateCommand()
                Dim Q As New StringBuilder()
                ' Q.Append("DELETE FROM dbo.PAC_BASALES_PERSONASDX WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOVIVIENDA WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOCONSTRUCCION WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_SERVICIOS WHERE IdBasalesTS = '" & idts & "'")
                Q.Append("DELETE FROM dbo.PAC_BASALES_PROBIDENTIFICADOS WHERE IdBasalesTS = '" & idts & "'")
                Sql = Q.ToString()
                'insertCommand.CommandText = Sql + " SET @ID = SCOPE_IDENTITY()"
                'Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                'IDParameter.Direction = ParameterDirection.Output
                'insertCommand.Parameters.Add(IDParameter)
                'insertCommand.Connection.Open()
                'insertCommand.ExecuteNonQuery()
                'id = IDParameter.Value
                connection.Open()
                Dim command As New SqlCommand(Sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            'End Using
            'X = id

        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
        Return X
    End Function

    Public Function delete_registros_condicionesvi(ByVal idts As String, ByVal usuario As String) As String
        _page = "db.delete_registros_condicionesvi"
        'Dim Q As String
        'Dim Ds As New DataSet()
        Dim Sql As String = String.Empty
        'Dim id As String
        Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                ' Using insertCommand As SqlCommand = connection.CreateCommand()
                Dim Q As New StringBuilder()
                ' Q.Append("DELETE FROM dbo.PAC_BASALES_PERSONASDX WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOVIVIENDA WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_TIPOCONSTRUCCION WHERE IdBasalesTS = '" & idts & "'")
                'Q.Append("DELETE FROM dbo.PAC_CONDICIONES_SERVICIOS WHERE IdBasalesTS = '" & idts & "'")
                ' Q.Append("DELETE FROM dbo.PAC_BASALES_PROBIDENTIFICADOS WHERE IdBasalesTS = '" & idts & "'")
                Q.Append("DELETE FROM dbo.PAC_CONDICIONESVIVIENDA WHERE IdBasalesTS ='" & idts & "'")
                Sql = Q.ToString()
                'insertCommand.CommandText = Sql + " SET @ID = SCOPE_IDENTITY()"
                'Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                'IDParameter.Direction = ParameterDirection.Output
                'insertCommand.Parameters.Add(IDParameter)
                'insertCommand.Connection.Open()
                'insertCommand.ExecuteNonQuery()
                'id = IDParameter.Value
                connection.Open()
                Dim command As New SqlCommand(Sql, connection)
                command.ExecuteNonQuery()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            'End Using
            'X = id

        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try
        Return X
    End Function

    Public Sub Grabar_cambios_basal_ts(ByRef idts As String, ByVal idpac As String, ByVal nhc As String, ByVal fechaestudiose As String, ByVal observaciones As String, ByVal circuito As String, ByVal usuario As String)
        _page = "db.Grabar_cambios_basal_ts"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim sql As String = ""
        Dim Str As String = ""
        'Dim id As String
        'Dim X As String = String.Empty

        Try
            Using connection As New SqlConnection(_cn1)
                Using insertCommand As SqlCommand = connection.CreateCommand()

                    'Q.Append("INSERT INTO dbo.PAC_BASALES_TS (IdPaciente, NHC, Fecha_estudioSE,  Observaciones, Circuito, Usuario)")
                    'Query = Q.ToString() + String.Format("VALUES ({0}, '{1}', '{2}', '{3}', {4},'{5}')", idpac, nhc, fechaestudiose, If(String.IsNullOrEmpty(observaciones.ToString()), "", observaciones.ToString()), If(String.IsNullOrEmpty(circuito.ToString()), "Null", circuito.ToString()), usuario)
                    'Q.Append("INSERT INTO dbo.TV_Basal (CodigoOrientadorPre, RegistroOrientacion, FechaOrientacion, NoHC, IdTipoUsuario, IdCohorte, IdServicio, Cama, VecesVisitaEMA, TiempoUltimaVisita, IdEstadoConciencia, MotivoVisitaHospital, Iniciales, ZonaResindencia, IdMunicipioResidencia, IdDepartamentoResidencia, IdPaisResidencia, IdPaisNacimiento, IdDepartamentoNacimiento, FechaNacimiento, EdadAños, EdadMeses, IdEtnia, OtroEtnia, IdComunidadLinguistica, IdGenero, IdEstadoCivil, Hijos, CuantosHijos, EdadHijoMayor, EdadHijoMenor, IdReligion, OtraReligion, SabeLeer, SabeEscribir, IdEscolaridad, GradosDeEstudio, Carrera, Ocupacion, TrabajaActualmente, Salario, QuienAportaSalario, IngresoTotalHogar, IdSituacionLaboral, IdOrientacionSexual, TiempoUltimaRelacion, NumeroParejasSexuales, ParejaOcasional, IdCitaParejaOcasional, HaTenidoUnaITS, RiesgoInfecVIH, RiesgoInfecITS, EnfermedadCronica, CualEnfCronica, CuandoEnfCronica, PruebaAnteriorVIH, FechaPruebaAnterior, PreOrientacionHoy, Tamizaje1, FechaTamizaje1, Tamizaje2, FechaTamizaje2, ELISA, FechaELISA, WesternBlot, FechaWesternBlot, ResultadoFinal, FechaResultadoFinal, PostOrientacionHoy, POPorque, CodigoPostOrientador, PacienteVinculadoTratamiento, LugarVinculoTratamiento, TerapiaPreviaARV, LugarTerapiaPreviaARV, NoASIAnterior)")
                    sql = String.Format("UPDATE dbo.PAC_BASALES_TS SET IdPaciente = {0}, NHC = '{1}', Fecha_estudioSE = CONVERT(DATE,'{2}'), Observaciones = '{3}', Circuito = {4}, Usuario = '{5}'", idpac, nhc, fechaestudiose, If(String.IsNullOrEmpty(observaciones.ToString()), "", observaciones.ToString()), If(String.IsNullOrEmpty(circuito.ToString()), "Null", circuito.ToString()), usuario)
                    ' Sql += String.Format("MotivoVisitaHospital = {0}, Iniciales = '{1}', ZonaResindencia = {2}, IdMunicipioResidencia = {3}, IdDepartamentoResidencia = {4}, IdPaisResidencia = {5}, IdPaisNacimiento = {6}, IdDepartamentoNacimiento = {7}, FechaNacimiento = CONVERT(DATE,{8}), EdadAños = {9}, ", d(11).ToString(), d(12).ToString(), If(String.IsNullOrEmpty(d(13).ToString()), "Null", d(13).ToString()), d(14).ToString(), d(15).ToString(), d(16).ToString(), d(17).ToString(), d(18).ToString(), IIf(String.IsNullOrEmpty(d(19).ToString()), sqldatenull, "'" & d(19).ToString() & "'"), If(String.IsNullOrEmpty(d(20).ToString()), "Null", d(20).ToString()))
                    ' Sql += String.Format("EdadMeses = {0}, IdEtnia = {1}, OtroEtnia = '{2}', IdComunidadLinguistica = {3}, IdGenero = {4}, IdEstadoCivil = {5}, Hijos = {6}, CuantosHijos = {7}, EdadHijoMayor = {8}, EdadHijoMenor = {9}, ", If(String.IsNullOrEmpty(d(21).ToString()), "Null", d(21).ToString()), d(22).ToString(), d(23).ToString(), d(24).ToString(), d(25).ToString(), d(26).ToString(), d(27).ToString(), If(String.IsNullOrEmpty(d(28).ToString()), "Null", d(28).ToString()), If(String.IsNullOrEmpty(d(29).ToString()), "Null", d(29).ToString()), If(String.IsNullOrEmpty(d(30).ToString()), "Null", d(30).ToString()))
                    ' Sql += String.Format("IdReligion = {0}, OtraReligion = '{1}', SabeLeer = {2}, SabeEscribir = {3}, IdEscolaridad = {4}, GradosDeEstudio = {5}, Carrera = '{6}', Ocupacion = {7}, TrabajaActualmente = {8}, Salario = {9}, ", d(31).ToString(), d(32).ToString(), d(33).ToString(), d(34).ToString(), d(35).ToString(), If(String.IsNullOrEmpty(d(36).ToString()), "Null", d(36).ToString()), d(37).ToString(), d(38).ToString(), d(39).ToString(), If(String.IsNullOrEmpty(d(40).ToString()), "Null", d(40).ToString()))
                    ' Sql += String.Format("QuienAportaSalario = '{0}', IngresoTotalHogar = {1}, IdSituacionLaboral = {2}, IdOrientacionSexual = {3}, TiempoUltimaRelacion = '{4}', NumeroParejasSexuales = {5}, ParejaOcasional = {6}, IdCitaParejaOcasional = {7}, HaTenidoUnaITS = {8}, RiesgoInfecVIH = {9}, ", d(41).ToString(), If(String.IsNullOrEmpty(d(42).ToString()), "Null", d(42).ToString()), d(43).ToString(), d(44).ToString(), d(45).ToString(), If(String.IsNullOrEmpty(d(46).ToString()), "Null", d(46).ToString()), d(47).ToString(), d(48).ToString(), d(49).ToString(), d(50).ToString())
                    ' Sql += String.Format("RiesgoInfecITS = {0}, EnfermedadCronica = {1}, CualEnfCronica = '{2}', CuandoEnfCronica = '{3}', PruebaAnteriorVIH = {4}, FechaPruebaAnterior = CONVERT(DATE,{5}), PreOrientacionHoy = {6}, Tamizaje1 = {7}, FechaTamizaje1 = CONVERT(DATE,{8}), Tamizaje2 = {9}, ", d(51).ToString(), d(52).ToString(), d(53).ToString(), d(54).ToString(), d(55).ToString(), IIf(String.IsNullOrEmpty(d(56).ToString()), sqldatenull, "'" & d(56).ToString() & "'"), d(57).ToString(), d(58).ToString(), IIf(String.IsNullOrEmpty(d(59).ToString()), sqldatenull, "'" & d(59).ToString() & "'"), d(60).ToString())
                    ' Sql += String.Format("FechaTamizaje2 = CONVERT(DATE,{0}), ELISA = {1}, FechaELISA = CONVERT(DATE,{2}), WesternBlot = {3}, FechaWesternBlot = CONVERT(DATE,{4}), ResultadoFinal = {5}, FechaResultadoFinal = CONVERT(DATE,{6}), PostOrientacionHoy = {7}, POPorque = '{8}', CodigoPostOrientador = {9}, ", IIf(String.IsNullOrEmpty(d(61).ToString()), sqldatenull, "'" & d(61).ToString() & "'"), d(62).ToString(), IIf(String.IsNullOrEmpty(d(63).ToString()), sqldatenull, "'" & d(63).ToString() & "'"), d(64).ToString(), IIf(String.IsNullOrEmpty(d(65).ToString()), sqldatenull, "'" & d(65).ToString() & "'"), d(66).ToString(), IIf(String.IsNullOrEmpty(d(67).ToString()), sqldatenull, "'" & d(67).ToString() & "'"), d(68).ToString(), d(69).ToString(), If(String.IsNullOrEmpty(d(70).ToString()), "Null", d(70).ToString()))
                    ' Sql += String.Format("PacienteVinculadoTratamiento = {0}, LugarVinculoTratamiento = '{1}', TerapiaPreviaARV = {2}, LugarTerapiaPreviaARV = '{3}', NoASIAnterior = '{4}', Usuario = '{5}' ", d(71).ToString(), d(72).ToString(), d(73).ToString(), d(74).ToString(), If(String.IsNullOrEmpty(d(75).ToString()), "Null", d(75).ToString()), d(76).ToString())
                    sql += String.Format("WHERE IdBasalesTS = {0}", idts)

                    'insertCommand.CommandText = Query + " SET @ID = SCOPE_IDENTITY()"
                    'Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                    'IDParameter.Direction = ParameterDirection.Output
                    'insertCommand.Parameters.Add(IDParameter)
                    'insertCommand.Connection.Open()
                    'insertCommand.ExecuteNonQuery()
                    'id = IDParameter.Value

                End Using
            End Using
            'X = id
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            'X = String.Empty
        End Try
        'Return X
    End Sub


    Public Function CATALOGOS_CIRCUITO(ByVal op As String, ByVal usuario As String) As DataTable
        _page = "db.CATALOGOS_CIRCUITO"
        Dim Query As String = String.Empty


        Select Case op
            'status circuito paciente
            Case 1
                Query = "SELECT IdStatus, Nom_Status FROM dbo.STATUS_CIRCUITO ORDER BY IdStatus ASC"
                'PERIODO CIRCUITO
            Case 2
                Query = "SELECT IdPeriodo, Descripcion_Periodo FROM dbo.PERIODO_CIRCUITO ORDER BY IdPeriodo ASC "
            Case 3
                Query = "SELECT IdGrupo, Nom_Grupo FROM dbo.GRUPO_CIRCUITO ORDER BY IdGrupo ASC "

        End Select


        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function Ob_varios_circuito(ByVal nhc As String, ByVal usuario As String) As String
        _page = "db.Ob_varios_circuito"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT E.NomEtnia FROM ")
        Q.Append("PAC_ID AS I LEFT OUTER JOIN  ")
        Q.Append("PAC_BASALES  AS B  ON B.IdPaciente = I.IdPaciente  LEFT OUTER JOIN ")
        Q.Append("PAC_M_ETNIA AS E ON E.IdEtnia = B.Etnia  ")
        Q.Append("WHERE I.NHC = '" & nhc & "'")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn2)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                'Dim reader As SqlDataReader = command.ExecuteReader()
                'If reader IsNot Nothing Then
                '    While reader.Read()
                '        Str = "True|" + reader("NumHospitalaria").ToString() + "|" + reader("Cedula").ToString() + "|" + reader("NomGenero").ToString() + "|" + reader("Paciente").ToString() + "|" + reader("Telefono").ToString() + "|" + reader("Movil").ToString() + "|" + reader("FechaNacimiento").ToString() + "|" + reader("Edad").ToString() + "|" + reader("PaisNacimiento").ToString() + "|" + reader("DeptoNacimiento").ToString() + "|" + reader("MuniNacimiento").ToString() + "|" + reader("PaisResidencia").ToString() + "|" + reader("DeptoResidencia").ToString() + "|" + reader("MuniResidencia").ToString() + "|" + reader("Direccion").ToString() + "|" + reader("EstadoCivil").ToString() + "|" + reader("NivelEducativo").ToString() + "|" + reader("AñosCompletos").ToString() + "|" + reader("OrientacionSexual").ToString() + "|" + reader("SituacionLaboral").ToString() + "|" + reader("MotivoBaja").ToString() + "|" + reader("IdPaciente").ToString()
                '        Exit While
                '    End While
                'End If
                'If Str = String.Empty Then
                '    Str = "False|No se Encontró Información."
                'End If
                'reader.Dispose()
                'reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function Graba_Circuito(ByVal idpac As String, ByVal nhc As String, ByVal status_circuito As String, ByVal año As String, ByVal periodo As String, ByVal grupo As String, ByVal fecha_ingreso As String, ByVal usuario As String) As String
        _page = "db.Graba_Circuito"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Dim id As String
        Dim X As String = String.Empty


        Try
            Using connection As New SqlConnection(_cn1)
                Using insertCommand As SqlCommand = connection.CreateCommand()

                    Q.Append("INSERT INTO dbo.PAC_CIRCUITO_ADHERENCIA (IdPaciente, NHC, Status, Año, Periodo, Grupo, Fecha_Ingreso, Usuario)")
                    Query = Q.ToString() + String.Format("VALUES ({0}, '{1}', {2}, {3}, {4},{5}, Convert(DateTime,'{6}',21),'{7}' )", idpac, nhc, status_circuito, año, periodo, grupo, fecha_ingreso, usuario)

                    insertCommand.CommandText = Query + " SET @ID = SCOPE_IDENTITY()"
                    Dim IDParameter As New SqlParameter("@ID", SqlDbType.Int)
                    IDParameter.Direction = ParameterDirection.Output
                    insertCommand.Parameters.Add(IDParameter)
                    insertCommand.Connection.Open()
                    insertCommand.ExecuteNonQuery()
                    id = IDParameter.Value

                End Using
            End Using
            X = id
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message)
            X = String.Empty
        End Try
        Return X
    End Function

    Public Function obtiene_circuito_info(ByVal nhc As String, ByVal usuario As String) As String
        _page = "db.obtiene_circuito_info"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Dim Str As String = ""
        Q.Append("SELECT C.NHC, C.Status, S.Nom_Status, G.Nom_Grupo, P.Descripcion_Periodo ")
        Q.Append("FROM dbo.PAC_CIRCUITO_ADHERENCIA AS C LEFT OUTER JOIN ")
        Q.Append("dbo.STATUS_CIRCUITO AS S ON S.IdStatus = C.Status LEFT OUTER JOIN  ")
        Q.Append("dbo.PERIODO_CIRCUITO AS P ON P.IdPeriodo = C.Periodo LEFT OUTER JOIN  ")
        Q.Append("GRUPO_CIRCUITO AS G ON G.IdGrupo = C.Grupo   ")
        Q.Append("WHERE C.NHC = '" & nhc & "'")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    While reader.Read()
                        Str = "True|" + reader("NHC").ToString() + "|" + reader("Status").ToString() + "|" + reader("Nom_Status").ToString() + "|" + reader("Nom_Grupo").ToString() + "|" + reader("Descripcion_Periodo").ToString()
                        Exit While
                    End While
                End If
                If Str = String.Empty Then
                    Str = "False|No se encuentra en Circuito"
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_" & nhc
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function Llena_horarios_citas(ByVal op As String, ByVal usuario As String) As DataTable
        _page = "db.Llena_horarios_citas"
        Dim Query As String = String.Empty


        Select Case op

            Case 1
                'Seleccion horarios asignados lunes -jueves
                Query = "SELECT H.Id_Bloque_H, H.Desc_Horario FROM HORARIOS_JORNADA_CITAS AS H WHERE h.Id_Bloque_H IN ('1','2', '3','4','5','7','8','9') ORDER BY H.Id_Bloque_H ASC "

            Case 2
                'Seleccion horarios asignados viernes
                Query = "SELECT H.Id_Bloque_H, H.Desc_Horario FROM HORARIOS_JORNADA_CITAS AS H WHERE h.Id_Bloque_H IN ('1','2', '3','7','8','9') ORDER BY H.Id_Bloque_H ASC "

        End Select


        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function


    Public Function Revisa_horarios_disponibles(ByVal prox_cita As String, ByVal horario As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.Revisa_horarios_disponibles"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'Seleccion horarios asignados lunes -jueves
        Query = "SELECT Count(C.IdCitas) AS 'No_Citas' FROM  PAC_CITAS AS C  WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '" & horario & "' AND C.Clinica = '" & clinica & "' "

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str



        '    Using connection As New SqlConnection(_cn1)
        '        connection.Open()
        '        Dim adapter As New SqlDataAdapter()
        '        adapter.SelectCommand = New SqlCommand(Query, connection)
        '        adapter.SelectCommand.CommandTimeout = TimeoutDB
        '        adapter.Fill(Ds, _page)
        '        adapter.Dispose()
        '        connection.Dispose()
        '        connection.Close()
        '    End Using
        '    Return Ds.Tables(0)
        'Catch ex As SqlException
        '    _error = ex.Message
        '    _pageO = _page
        '    GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
        '    Return Nothing
        'End Try
    End Function
    Public Function Ultimo_Horario_Asignado(ByVal idpac As String, ByVal usuario As String) As String
        _page = "db.Ultimo_Horario_Asignado"
        Dim Query As String = String.Empty
        Dim Str As String = ""



        Query = "SELECT DISTINCT(C.IdPaciente), (SELECT TOP(1) dbo.fn_ObtieneHorarioCita(CI.IdHorario) AS 'HorarioCita' FROM PAC_CITAS AS CI WHERE CI.IdPaciente = C.IdPaciente  ORDER BY CI.Fecha DESC) AS 'Horario'  FROM PAC_CITAS AS C  WHERE C.IdPaciente = '" & idpac & "' "


        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("horario_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str

    End Function


    Public Function R_no_citas_horario1(ByVal prox_cita As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.R_no_citas_horario1"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'seleccion horario 1 
        Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '1' AND C.Clinica = '" & clinica & "' "

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str


    End Function

    Public Function R_no_citas_horario2(ByVal prox_cita As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.R_no_citas_horario2"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'seleccion horario 2 
        Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '2' AND C.Clinica = '" & clinica & "' "

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function R_no_citas_horario3(ByVal prox_cita As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.R_no_citas_horario3"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'seleccion horario 3
        Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '3' AND C.Clinica = '" & clinica & "' "

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function R_no_citas_horario4(ByVal prox_cita As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.R_no_citas_horario4"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'seleccion horario 4 
        Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '4' AND C.Clinica = '" & clinica & "' "

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function R_no_citas_horario5(ByVal prox_cita As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.R_no_citas_horario5"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'seleccion horario 5
        Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '5' AND C.Clinica = '" & clinica & "'"

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function R_no_citas_horario6(ByVal prox_cita As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.R_no_citas_horario6"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'seleccion horario 1 
        Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '6' AND C.Clinica = '" & clinica & "'"

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function R_no_citas_horario7(ByVal prox_cita As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.R_no_citas_horario7"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'seleccion horario 1 
        Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '7' AND C.Clinica = '" & clinica & "' "

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function R_no_citas_horario8(ByVal prox_cita As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.R_no_citas_horario8"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'seleccion horario 1 
        Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '8' AND C.Clinica = '" & clinica & "' "

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function R_no_citas_horario9(ByVal prox_cita As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.R_no_citas_horario9"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'seleccion horario 1 
        Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '9' AND C.Clinica = '" & clinica & "' "

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function R_no_citas_horario_hosp(ByVal opc As String, ByVal prox_cita As String, ByVal clinica As String, ByVal usuario As String) As String
        _page = "db.R_no_citas_horario_hosp"
        Dim Query As String = String.Empty
        Dim Str As String = ""

        Select Case opc
            Case "1"
                Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '1' AND C.Clinica = '" & clinica & "' "

            Case "2"
                Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '2' AND C.Clinica = '" & clinica & "' "

            Case "3"
                Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '3' AND C.Clinica = '" & clinica & "' "

            Case "4"
                Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '4' AND C.Clinica = '" & clinica & "' "

            Case "5"
                Query = "SELECT Count(C.IdCitas) AS 'No_Citas'  FROM  PAC_CITAS AS C WHERE convert(VARCHAR, C.FechaProximaVisita, 103) = '" & prox_cita & "' AND C.IdHorario = '4' AND C.Clinica = '" & clinica & "' "

        End Select


        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = reader("No_Citas").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function conteo_dias_CV(ByVal nhc As String, ByVal usuario As String) As String
        _page = "db.conteo_dias_CV"
        Dim Query As String = String.Empty
        Dim Str As String = ""


        'seleccion horario 1 
        Query = "SELECT  I.NHC, (SELECT TOP 1 z.Id_Clasificacion_Pac  FROM PSOEP AS Z WHERE I.NHC = Z.NHC ORDER BY Z.FechaFicha DESC ) AS 'Clasificacion_pac',(SELECT TOP 1 convert(DATE,C.FechaEntrega)  FROM ControlARV  AS C WHERE I.NHC = C.NHC AND C.EsquemaEstatus = '2' ORDER BY C.FechaEntrega ASC) AS 'Fecha_Inicio_ARV',datediff(day, (SELECT TOP 1 convert(DATE,C.FechaEntrega)  FROM ControlARV  AS C WHERE I.NHC = C.NHC AND C.EsquemaEstatus = '2' ORDER BY C.FechaEntrega ASC) , convert(DATE,getdate())) AS 'dias'FROM PAC_ID AS I WHERE I.NHC = '" & nhc & "'"

        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim command As New SqlCommand(Query, connection)
                command.CommandTimeout = TimeoutDB
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader IsNot Nothing Then
                    reader.Read()
                    Str = "True|" & reader("NHC").ToString() & "|" & reader("Clasificacion_pac").ToString() & "|" & reader("dias").ToString()
                End If
                reader.Dispose()
                reader.Close()
                command.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page & "_"
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Str = "False|" + ex.Message
        End Try
        Return Str
    End Function

    Public Function ReporteGrupoGamiliar(ByVal NHC As String, ByVal usuario As String) As DataTable
        _page = "db.ReporteGrupoGamiliar"

        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Q.Append("SELECT G.IdGrupoFamiliar, G.Nombre ,T.NomTipoRelacion TipoRelacion ,G.Edad ,N.NomNivelEducativo NivelEducativo, ")
        Q.Append("S.NomSituacionLaboral SituacionLaboral, G.Ingreso, CASE WHEN G.ConoceDx = 1 THEN 'Si' WHEN  G.ConoceDx = 2  THEN 'No' END  AS NomConoceDx ")
        Q.Append("FROM ")
        Q.Append("PAC_GRUPOFAMILIAR G LEFT OUTER JOIN  ")
        Q.Append("(SELECT TOP 1 NHC, IdPaciente, IdBasalesTS FROM PAC_BASALES_TS ORDER BY IdBasalesTS desc) AS B ON  g.IdPaciente = b.IdPaciente LEFT OUTER JOIN ")
        Q.Append("PAC_TIPO_RELACION T ON G.IdTipoRelacion = T.IdTipoRelacion LEFT OUTER JOIN ")
        Q.Append("PAC_NIVELEDUCATIVO N ON G.IdNivelEducativo = N.IdNivelEducativo LEFT OUTER JOIN ")
        Q.Append("PAC_SITUACIONLABORAL S ON G.IdSituacionLaboral = S.IdSituacionLaboral ")
        Q.Append("WHERE B.NHC = '" & NHC & "' ")

        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message & "|" & Query)
            Return Nothing
        End Try
    End Function




    Public Function Buscar_Estudios(ByVal nhc As String, ByVal usuario As String) As DataTable
        _page = "db.Buscar_Estudios"
        Dim Q As New StringBuilder()
        Dim Query As String = ""
        Q.Append("select IdBasalesTS, NHC, convert(VARCHAR,Fecha_estudioSE,103) as 'FechaEstudio', Circuito, Observaciones  ")
        Q.Append("from PAC_BASALES_TS  ")
        Q.Append("where NHC = '" & nhc & "' order by IdBasalesTS desc  ")
        Query = Q.ToString()
        Dim Ds As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(Query, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Ds, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Ds.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _page & "|" & ex.Number & "|" & ex.Message & "|" & Query)
            Return Nothing
        End Try
    End Function

    Public Function CATALOGOS_DIRECCION(ByVal op As String, ByVal usuario As String, ByVal IdParam As Integer) As DataTable
        _page = "db.CATALOGOS_DIRECCION"
        Dim QueryDir As String = String.Empty

        Select Case op
            'Carga pais
            Case 1
                QueryDir = String.Format("select IdPais, PaisDescripcion from PAC_Pais")
            'Carga Departamento
            Case 2
                QueryDir = String.Format("select IdDepartamento,DepartamentoDescripcion from PAC_Departamentos where IdPais = {0}", IdParam)
            'Carga Municipio
            Case 3
                QueryDir = String.Format("select IdMunicipio,MunicipioDescripcion from PAC_Municipios where IdDepartamento = {0}", IdParam)
            'Carga Zona
            Case 4
                QueryDir = String.Format("select IdZona,ZonaDescripcion from PAC_Zonas where IdMunicipio = {0}", IdParam)
            Case 5
                QueryDir = String.Format("select PerteneceTAbrevia,PerteneceTdescripcion from pac_perteneceTelefono")

        End Select


        Dim Dsd As New DataSet()
        Try
            Using connection As New SqlConnection(_cn1)
                connection.Open()
                Dim adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(QueryDir, connection)
                adapter.SelectCommand.CommandTimeout = TimeoutDB
                adapter.Fill(Dsd, _page)
                adapter.Dispose()
                connection.Dispose()
                connection.Close()
            End Using
            Return Dsd.Tables(0)
        Catch ex As SqlException
            _error = ex.Message
            _pageO = _page
            GrabarErrores(usuario & "|" & _pageO & "|" & ex.Number & "|" & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Sub GuardaDireccion(ByVal param As PacientesTS, ByVal GuardaDTT As String)
        _page = "db.GuardarDireccion"
        Dim QGuarda As String = String.Empty
        Dim TelMangua1 As String = String.Empty
        Dim TelMangua2 As String = String.Empty

        Select Case param.PerTel1
            Case "PER"
                TelMangua1 = param.Tel1 & " Personal, " & param.ConTx1 & " Conoce dx"
            Case "FAM"
                TelMangua1 = param.Tel1 & " familiar, " & param.ConTx1 & " Conoce dx"
            Case "AMI"
                TelMangua1 = param.Tel1 & " amigo, " & param.ConTx1 & " Conoce dx"
            Case "VEC"
                TelMangua1 = param.Tel1 & " vecino, " & param.ConTx1 & " Conoce dx"
            Case "CON"
                TelMangua1 = param.Tel1 & " conocido, " & param.ConTx1 & " Conoce dx"
        End Select

        If String.IsNullOrEmpty(param.Tel2) Then
            TelMangua2 = String.Empty
            param.ConTx2 = String.Empty
        Else
            Select Case param.PerTel2
                Case "PER"
                    TelMangua2 = param.Tel2 & " Personal, " & param.ConTx2 & " Conoce dx"
                Case "FAM"
                    TelMangua2 = param.Tel2 & " familiar, " & param.ConTx2 & " Conoce dx"
                Case "AMI"
                    TelMangua2 = param.Tel2 & " amigo, " & param.ConTx2 & " Conoce dx"
                Case "VEC"
                    TelMangua2 = param.Tel2 & " vecino, " & param.ConTx2 & " Conoce dx"
                Case "CON"
                    TelMangua2 = param.Tel2 & " conocido, " & param.ConTx2 & " Conoce dx"
            End Select
        End If

        If String.IsNullOrEmpty(param.Tel3) Then
            param.ConTx3 = String.Empty
        End If

        If String.IsNullOrEmpty(param.Tel4) Then
            param.ConTx4 = String.Empty
        End If

        Try

            Using cnn1 As New SqlConnection(_cn1)
                'Dim cmd As SqlCommand = New SqlCommand
                Dim cmd As SqlCommand = cnn1.CreateCommand()
                cmd.CommandText = "SP_GuardaDireccion"
                cmd.CommandType = CommandType.StoredProcedure
                'cmd.Connection = cnn1

                cmd.Parameters.AddWithValue("@IdPaciente", SqlDbType.Int).Value = param.IdPacTS
                cmd.Parameters.AddWithValue("@Nhc", SqlDbType.VarChar).Value = param.NhcTS
                cmd.Parameters.AddWithValue("@IdPais", SqlDbType.Int).Value = param.IdPaisTS
                cmd.Parameters.AddWithValue("@IdDepartamento", SqlDbType.Int).Value = param.IdDeptoTS
                cmd.Parameters.AddWithValue("@IdMunicipio", SqlDbType.Int).Value = param.IdMupioTS
                cmd.Parameters.AddWithValue("@IdZona", SqlDbType.Int).Value = param.IdZonaTS
                cmd.Parameters.AddWithValue("@Direccion", SqlDbType.VarChar).Value = param.DireccionTS
                cmd.Parameters.AddWithValue("@Tel1", SqlDbType.VarChar).Value = param.Tel1
                cmd.Parameters.AddWithValue("@Tel2", SqlDbType.VarChar).Value = param.Tel2
                cmd.Parameters.AddWithValue("@Tel3", SqlDbType.VarChar).Value = param.Tel3
                cmd.Parameters.AddWithValue("@Tel4", SqlDbType.VarChar).Value = param.Tel4
                cmd.Parameters.AddWithValue("@PerTel1", SqlDbType.VarChar).Value = param.PerTel1
                cmd.Parameters.AddWithValue("@PerTel2", SqlDbType.VarChar).Value = param.PerTel2
                cmd.Parameters.AddWithValue("@PerTel3", SqlDbType.VarChar).Value = param.PerTel3
                cmd.Parameters.AddWithValue("@PerTel4", SqlDbType.VarChar).Value = param.PerTel4
                cmd.Parameters.AddWithValue("@ConTX1", SqlDbType.VarChar).Value = param.ConTx1
                cmd.Parameters.AddWithValue("@ConTX2", SqlDbType.VarChar).Value = param.ConTx2
                cmd.Parameters.AddWithValue("@ConTX3", SqlDbType.VarChar).Value = param.ConTx3
                cmd.Parameters.AddWithValue("@ConTX4", SqlDbType.VarChar).Value = param.ConTx4
                cmd.Parameters.AddWithValue("@TelMg1", SqlDbType.VarChar).Value = TelMangua1
                cmd.Parameters.AddWithValue("@TelMg2", SqlDbType.VarChar).Value = TelMangua2
                cmd.Parameters.AddWithValue("@Usuario", SqlDbType.VarChar).Value = param.Usua
                cmd.Parameters.AddWithValue("@GuardarDTT", SqlDbType.VarChar).Value = GuardaDTT
                cmd.Parameters.Add("@resultado", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output

                cnn1.Open()
                cmd.ExecuteScalar()

                ResultadoG = cmd.Parameters("@resultado").Value
                ResultadoG2 = "EXITO"

            End Using
        Catch ex As SqlException
            ResultadoG2 = "ERROR"
            _error = ex.Message
            _pageO = _page
            GrabarErrores(param.Usua & "|" & _page & "|" & ex.Number & "|" & ex.Message)
        End Try


    End Sub


End Class