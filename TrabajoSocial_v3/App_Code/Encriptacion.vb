Imports Microsoft.VisualBasic

Public Class Encriptacion
    'Public Enum eEncrypt
    '    eDesencriptar = 0
    '    eEncriptar
    'End Enum

    'Private m_Accion As eEncrypt

    'Public m_Accion As Boolean = True

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
        ' Esta es una función que llamará directamente a ConvertirClave
        'm_Accion = eDesencriptar
        'm_Accion = False
        DesEncriptar = ConvertirClave(sOriginal, sClave, False)
    End Function

    Public Function Encriptar(ByVal sOriginal As String, ByVal sClave As String) As String
        ' Esta es una función que llamará directamente a ConvertirClave
        'm_Accion = eEncriptar
        'm_Accion = True
        Encriptar = ConvertirClave(sOriginal, sClave, True)
    End Function

    'Public Property Get CadenaOriginal() As String
    '    CadenaOriginal = m_sOriginal
    'End Property

    'Public Property Let CadenaOriginal(ByVal NewValue As String)
    '    ' Sólo asignar si la cadena tiene algún contenido
    '    If Len(NewValue) Then
    '        m_sOriginal = NewValue
    '    Else
    '' Devolver un error, si así se ha indicado
    '        If m_RaiseError Then
    '            With Err
    '                .Description = "Se debe asignar algún contenido a la cadena a encryptar / desencriptar"
    '                .Number = 13
    '                .Source = "cEncrypt::CadenaOriginal"
    '                .Raise .Number
    '            End With
    '        End If
    '    End If
    'End Property

    'Public Property Get Clave() As String
    '    Clave = m_sClave
    'End Property

    'Public Property Let Clave(ByVal NewValue As String)
    '    ' Sólo asignar si la cadena tiene algún contenido
    '    If Len(NewValue) Then
    '        m_sClave = NewValue
    '    Else
    '' Devolver un error, si así se ha indicado
    '        If m_RaiseError Then
    '            With Err
    '                .Description = "Se debe asignar algún contenido a la cadena a usar como clave para encriptar / desencriptar"
    '                .Number = 13
    '                .Source = "cEncrypt::Clave"
    '                .Raise .Number
    '            End With
    '        Else
    '' Si no, devolver el valor por defecto
    '            m_sClave = mc_sClave
    '        End If
    '    End If
    'End Property

    'Public Property Get RaiseError() As Boolean
    '    RaiseError = m_RaiseError
    'End Property

    'Public Property Let RaiseError(ByVal NewValue As Boolean)
    '    m_RaiseError = NewValue
    'End Property

    'Public Property Get Accion() As eEncrypt
    '    Accion = m_Accion
    'End Property

    'Public Property Let Accion(ByVal NewValue As eEncrypt)
    '    ' Si el valor indicado es 0 será Descencriptar,
    '    ' si es cualquier otro valor, será encriptar
    '    ' De esta forma se aceptarán valores boolenos
    '    If NewValue = 0 Then
    '        m_Accion = eDesencriptar
    '    Else
    '        m_Accion = eEncriptar
    '    End If
    'End Property

End Class
