
Public Class EnumBoolConverter
    Implements IValueConverter

    ' Convert از کد به رابط کاربری
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim ParameterString As String = TryCast(parameter, String)
        If ParameterString Is Nothing Then
            Throw New InvalidOperationException("Parameter String Cannot Be Null. #AKP")
        End If

        Dim IsDef As Boolean = [Enum].IsDefined(value.GetType, value)
        If IsDef = False Then
            Throw New InvalidOperationException("Invalid Enum. #AKP")
        End If

        Dim ParameterValue As Object = [Enum].Parse(value.GetType, ParameterString)
        Return ParameterValue.Equals(value)
    End Function

    'Convert Back از رابط کاربری به کد
    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Dim ParameterString As String = TryCast(parameter, String)
        If ParameterString Is Nothing Then
            Throw New InvalidOperationException("Parameter String Cannot Be Null. #AKP")
        End If

        If CBool(value) Then
            Return [Enum].Parse(targetType, ParameterString)
        Else
            Return DependencyProperty.UnsetValue
        End If

    End Function
End Class