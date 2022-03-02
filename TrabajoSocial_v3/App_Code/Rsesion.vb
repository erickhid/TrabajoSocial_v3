Imports Microsoft.VisualBasic

Public Class Rsesion
    Public Function RevisaSesion(ByVal conexion As String, ByVal usuario As String) As Boolean
        If conexion = "F" OrElse String.IsNullOrEmpty(conexion) Then
            Return False
        Else
            If String.IsNullOrEmpty(usuario) Then
                Return False
            Else
                Return True
            End If
        End If
    End Function
    Public Function RevisaSesion(ByVal usuario As String) As Boolean
        Return Not String.IsNullOrEmpty(usuario)
    End Function

End Class
