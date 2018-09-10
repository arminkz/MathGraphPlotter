Public Class FuncRuleConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim FuncExp As String = CStr(value)
        If FuncExp = "" Then
            Return "No Equation"
        Else
            If FuncExp.Contains(",") Then
                Dim Result As String = ""
                Dim Param() As String = FuncExp.Split(",")
                For Each P As String In Param
                    Dim PE() As String = P.Split("=")
                    If PE.Count = 2 Then
                        Result += PE(0).Trim(" ") & " = " & PE(1).Trim(" ") & vbCrLf
                    End If
                Next
                Return Result
            Else
                If FuncExp.Contains("=") Then
                    Dim Func() As String = FuncExp.Split("=")
                    Dim FuncFrist As String = Func(0).Trim(" ")
                    Dim FuncSecond As String = Func(1).Trim(" ")
                    Return FuncFrist + " = " + FuncSecond
                Else
                    Return FuncExp
                End If
            End If
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class
