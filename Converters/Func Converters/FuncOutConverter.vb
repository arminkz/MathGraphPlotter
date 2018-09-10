Public Class FuncOutConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert

        Dim EQT As EquationType = value
        Select Case EQT
            Case EquationType.None
                Return "y" 'Temperory Solution (Must Fix)
            Case EquationType.CartesianY
                Return "y"
            Case EquationType.CartesianX
                Return "x"
            Case EquationType.CartesianZ
                Return "z"
            Case EquationType.PolarR
                Return "r"
            Case EquationType.PolarT
                Return "t"
            Case EquationType.PolarP
                Return "p"
                'Case EquationType.ParametricU, EquationType.ParametricUV, EquationType.Parametric4D
                '    Return "u"
            Case Else
                Return ""
                'Throw New Exception("Invalid Type. #AKP")
        End Select

    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class
