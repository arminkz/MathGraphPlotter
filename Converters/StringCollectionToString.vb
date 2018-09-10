Public Class StringCollectionToString
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim Result As String = ""
        Dim DC As List(Of String) = CType(value, List(Of String))
        If Not (DC.Count = 0 And DC Is Nothing) Then
            For i = 0 To DC.Count - 1
                If i = DC.Count - 1 Then
                    Result += DC(i)
                Else
                    Result += DC(i) & ","
                End If
            Next
        End If
        Return Result
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Dim Result As New List(Of String)
        Dim InputString As String = TryCast(value, String)
        If Not InputString = Nothing Then
            Dim SplitedString() As String = InputString.Split(",")

            For Each SS As String In SplitedString
                If Not SS Is "" Then Result.Add(SS)
            Next
        End If

        Return Result
    End Function
End Class
