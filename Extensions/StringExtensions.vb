Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Module StringExtensions

    <Extension> Public Function ToBool(Str As String) As Boolean?
        If Str = "1" Or Str.ToLower = "true" Then
            Return True
        ElseIf Str = "0" Or Str.ToLower = "false" Then
            Return False
        Else
            Return Nothing
        End If
    End Function

    <Extension> Public Function IsHexColor(Str As String) As Boolean
        Dim CST As String = Str.ToLower.Trim(" ")
        If CST.Length = 4 Or CST.Length = 9 Then
            Dim ShortHex As Boolean = Regex.IsMatch(CST, "#[0-9a-f]{3}")
            Dim LongHex As Boolean = Regex.IsMatch(CST, "#[0-9a-f]{8}")
            Return ShortHex Or LongHex
        Else
            Return False
        End If
    End Function

    <Extension> Public Function IsUri(Str As String) As Boolean
        Dim FakeObj As New Uri("", UriKind.RelativeOrAbsolute)
        Return Uri.TryCreate(Str, UriKind.RelativeOrAbsolute, FakeObj)
    End Function

End Module
