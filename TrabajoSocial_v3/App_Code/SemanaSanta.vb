Imports Microsoft.VisualBasic

Public Class SemanaSanta

#Region "Campos"
    Private a As Integer
    Private b As Integer
    Private c As Integer
    Private d As Integer
    Private e As Integer
    Private pascuaResurreccion As DateTime

    Private anio As Integer

#End Region

    ''' <summary>
    ''' Constructor de la clase
    ''' </summary>
    ''' <param name="anio">Entero que representa el año del que 
    ''' se quiere calcular la semana santa.</param>
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' Se produce cuando se intenta calcular
    ''' la semana santa de un año no contemplado.</exception>
    Public Sub New(anio As Integer)
        Try
            Me.anio = anio
            calculaDomingoPascua()
        Catch
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Cálculo del domingo de Pascua o domingo de Resurrección.
    ''' </summary>
    Private Sub calculaDomingoPascua()
        Dim p As ParConstantes = getPar(anio)
        a = anio Mod 19
        b = anio Mod 4
        c = anio Mod 7
        d = (19 * a + p.M) Mod 30
        e = (2 * b + 4 * c + 6 * d + p.N) Mod 7

        If d + e < 10 Then
            pascuaResurreccion = New DateTime(anio, 3, d + e + 22)
        Else
            pascuaResurreccion = New DateTime(anio, 4, d + e - 9)
        End If

        ' Excepciones
        If pascuaResurreccion = New DateTime(anio, 4, 26) Then
            pascuaResurreccion = New DateTime(anio, 4, 19)
        End If

        If pascuaResurreccion = New DateTime(anio, 4, 25) AndAlso d = 28 AndAlso e = 6 AndAlso a > 10 Then
            pascuaResurreccion = New DateTime(anio, 4, 18)
        End If
    End Sub

#Region "Constantes cálculo"
    Private Structure ParConstantes
        Public Property M() As Integer
            Get
                Return m_M
            End Get
            Set(value As Integer)
                m_M = Value
            End Set
        End Property
        Private m_M As Integer
        Public Property N() As Integer
            Get
                Return m_N
            End Get
            Set(value As Integer)
                m_N = Value
            End Set
        End Property
        Private m_N As Integer
    End Structure

    Private Function getPar(anio As Integer) As ParConstantes
        Dim p As New ParConstantes()
        If anio < 1583 Then
            Throw New ArgumentOutOfRangeException("El año deberá ser superior a 1583")
        ElseIf anio < 1700 Then
            p.M = 22
            p.N = 2
        ElseIf anio < 1800 Then
            p.M = 23
            p.N = 3
        ElseIf anio < 1900 Then
            p.M = 23
            p.N = 4
        ElseIf anio < 2100 Then
            p.M = 24
            p.N = 5
        ElseIf anio < 2200 Then
            p.M = 24
            p.N = 6
        ElseIf anio < 2299 Then
            p.M = 25
            p.N = 0
        Else
            Throw New ArgumentOutOfRangeException("El año deberá ser inferior a 2299")
        End If
        Return p
    End Function
#End Region

#Region "Propiedades públicas"



    Public ReadOnly Property MiercolesCeniza() As DateTime
        Get
            Return SabadoSanto.AddDays(7 * -6 - 3)
        End Get
    End Property

    Public ReadOnly Property ViernesDolores() As DateTime
        Get
            Return pascuaResurreccion.AddDays(-9)
        End Get
    End Property

    Public ReadOnly Property DomingoRamos() As DateTime
        Get
            Return pascuaResurreccion.AddDays(-7)
        End Get
    End Property

    Public ReadOnly Property MiercolesSanto() As DateTime
        Get
            Return pascuaResurreccion.AddDays(-4)
        End Get
    End Property

    Public ReadOnly Property JuevesSanto() As DateTime
        Get
            Return pascuaResurreccion.AddDays(-3)
        End Get
    End Property

    Public ReadOnly Property ViernesSanto() As DateTime
        Get
            Return pascuaResurreccion.AddDays(-2)
        End Get
    End Property

    Public ReadOnly Property SabadoSanto() As DateTime
        Get
            Return pascuaResurreccion.AddDays(-1)
        End Get
    End Property

    Public ReadOnly Property DomingoResurreccion() As DateTime
        Get
            Return pascuaResurreccion
        End Get
    End Property

#End Region
End Class
