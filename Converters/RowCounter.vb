Imports System.Collections.ObjectModel

Public Class RowCounter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        Try
            Dim MathObj As MathObject = values(0)
            Dim Coll As ObservableCollection(Of MathObject) = values(1)

            Dim Counter As Integer = 1
            For Each E As Object In Coll
                If E.Equals(MathObj) Then
                    Return Counter.ToString
                End If
                Counter += 1
            Next
        Catch
        End Try
        Return "E" ' Push Error String
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotSupportedException("Not Supported #AKP")
    End Function
End Class