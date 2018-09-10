Public Class VisibilityIfAvaliable
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim P As String = CStr(parameter)
        Select Case P
            Case "Text"
                If CStr(value) = "" Then
                    Return Visibility.Hidden
                Else
                    Return Visibility.Visible
                End If
            Case Else
                Return Visibility.Hidden
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class