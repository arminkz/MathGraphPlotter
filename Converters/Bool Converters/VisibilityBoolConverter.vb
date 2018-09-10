
Public Class VisibilityBoolConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If targetType <> GetType(Visibility) Then
            Throw New InvalidOperationException("The Target Must Be Visibility Type. #AKP")
        End If

        Dim B As Byte = CBool(value)
        If parameter IsNot Nothing Then
            Dim P As String = CStr(parameter)
            If P.ToLower = "not" Then B = Not B
        End If

        If B Then
            Return Visibility.Visible
        Else
            Return Visibility.Collapsed
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function

End Class
