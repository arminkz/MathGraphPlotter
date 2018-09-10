
Public Class Toggle3D
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim IsVisible As Boolean

        Dim DM As String = CStr(value)
        Dim Mode As Integer
        If DM = "2D" Then
            Mode = 2
        ElseIf DM = "3D" Then
            Mode = 3
        ElseIf DM = "4D" Then
            Mode = 4
        Else
            Throw New Exception("DrawMode Is Wrong !")
        End If

        Dim CorType As String = CStr(parameter)

        If CorType = "TwoD" Then
            IsVisible = If(Mode = 2, True, False)
        ElseIf CorType = "ThreeD" Then
            IsVisible = If(Mode = 3, True, False)
        ElseIf CorType = "FourD" Then
            IsVisible = If(Mode = 4, True, False)
        ElseIf CorType = "ThreeFourD" Then
            IsVisible = If(Mode = 3 Or Mode = 4, True, False)
        Else
            Throw New Exception("Parameter Must Be 2D Or 3D Or 4D !")
        End If

        Dim Result As Visibility
        If IsVisible Then
            Result = Visibility.Visible
        Else
            Result = Visibility.Collapsed
        End If

        Return Result
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException("Not Supported #AKP")
    End Function
End Class