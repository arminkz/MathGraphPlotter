
Public Class FuncInConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert

        ' TODO Fix FuncIn Converter

        Dim EQT As EquationType = value
        Dim P As String = CStr(parameter)

        Select Case EQT
            Case EquationType.None 'Temperory Solution
                Return "x"
            Case EquationType.CartesianY
                If Not P = "Second" Then
                    Return "x"
                Else
                    Return "z"
                End If
            Case EquationType.CartesianX
                If Not P = "Second" Then
                    Return "y"
                Else
                    Return "z"
                End If
            Case EquationType.CartesianZ
                If Not P = "Second" Then
                    Return "x"
                Else
                    Return "y"
                End If
            Case EquationType.CartesianN
                Return "Felan sabr"
            Case EquationType.PolarR
                If Not P = "Second" Then
                    Return "t"
                Else
                    Return "p"
                End If
            Case EquationType.PolarT
                If Not P = "Second" Then
                    Return "r"
                Else
                    Return "p"
                End If
            Case EquationType.PolarP
                If Not P = "Second" Then
                    Return "r"
                Else
                    Return "t"
                End If
            Case EquationType.ParametricU
                If Not P = "Second" Then
                    Return "u"
                Else
                    Return "Error : EqType2Visiblity"
                End If
            Case EquationType.ParametricUV
                If Not P = "Second" Then
                    Return "u"
                Else
                    Return "v"
                End If
            Case EquationType.CartesianW
                If Not P = "Second" Then
                    Return "x"
                Else
                    Return "y"
                End If
            Case EquationType.Parametric4D
                If Not P = "Second" Then
                    Return "u"
                Else
                    Return "v"
                End If
            Case Else
                Throw New Exception("Invalid Type.")
        End Select
        Return "x"

    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class
