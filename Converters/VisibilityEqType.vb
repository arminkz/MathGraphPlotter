Public Class VisibilityEqType
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert

        Dim Eq As Equation = CType(value, Equation)
        Dim EqType As EquationType = Eq.Type
        Dim EqExp As String = Eq.Expression

        Select Case EqType
            Case EquationType.CartesianY
                If EqExp.Contains("x") And EqExp.Contains("z") Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case EquationType.CartesianX
                If EqExp.Contains("y") And EqExp.Contains("z") Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case EquationType.CartesianZ
                If EqExp.Contains("x") And EqExp.Contains("y") Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case EquationType.CartesianN
                Return Visibility.Collapsed
            Case EquationType.PolarR
                'Dim EqExp As String = EquationTools.GetSimplifiedEquation(Eq)
                If EqExp.Contains("t") And EqExp.Contains("p") Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case EquationType.PolarT
                'Dim EqExp As String = EquationTools.GetSimplifiedEquation(Eq)
                If EqExp.Contains("r") And EqExp.Contains("p") Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case EquationType.PolarP
                'Dim EqExp As String = EquationTools.GetSimplifiedEquation(Eq)
                If EqExp.Contains("r") And EqExp.Contains("t") Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case EquationType.ParametricU
                Return Visibility.Collapsed
            Case EquationType.ParametricUV
                Return Visibility.Visible
            Case Else
                Return Visibility.Collapsed
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class