Public NotInheritable Class TagReader

    Shared Function ReadTag(Source As String, TagName As String) As String
        Dim TagStart As Integer
        Dim TagEnd As Integer
        If Source.Contains("<" + TagName + ">") Then
            TagStart = Source.IndexOf("<" + TagName + ">") + TagName.Length + 2
            If Source.Substring(TagStart).Contains("</" + TagName + ">") Then
                TagEnd = Source.IndexOf("</" + TagName + ">")
            Else
                Return ""
            End If
        Else
            Return ""
        End If
        Return Source.Substring(TagStart, TagEnd - TagStart)
    End Function

    Shared Function ReadParenthesis(Source As String) As String
        Dim PraStart As Integer = Source.IndexOf("(") + 1
        Dim PraEnd As Integer = Source.LastIndexOf(")")
        Return Source.Substring(PraStart, PraEnd - PraStart)
    End Function

End Class
