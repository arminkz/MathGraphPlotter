Public NotInheritable Class BrushReader

    Shared Function ReadBrush(Text As String) As Brush
        Dim Result As Brush
        Dim EsIndex As Integer = Text.IndexOf(":")
        Dim Part1 As String = Text.Substring(0, EsIndex)
        Dim Part2 As String = Text.Substring(EsIndex + 1)
        If Part1 <> "" And Part2 <> "" Then
            Select Case Part1
                Case "sc"
                    'If Not Part2.IsHexColor Then MsgBox("Warning : Not Standard Hex Color !")
                    Result = New SolidColorBrush(CType(ColorConverter.ConvertFromString(Part2), Color))
                Case "uri"
                    Result = New ImageBrush(New BitmapImage(New Uri(Part2, UriKind.RelativeOrAbsolute)))
                Case "res"
                    Result = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Examples/Example Textures/" & Part2, UriKind.RelativeOrAbsolute)))
                Case Else
                    Throw New Exception("Invalid Brush Identifier : " & Part1)
            End Select
        Else
            Throw New Exception("Invalid MGP Brush #AKP.")
        End If
        Return Result
    End Function

    Shared Function ReadColor(Text As String) As Color
        Dim Result As Color
        Dim EsIndex As Integer = Text.IndexOf(":")
        Dim Part1 As String = Text.Substring(0, EsIndex)
        Dim Part2 As String = Text.Substring(EsIndex + 1)
        If Part1 <> "" And Part2 <> "" Then
            Select Case Part1
                Case "sc"
                    'If Not Part2.IsHexColor Then MsgBox("Warning : Not Standard Hex Color !")
                    Result = CType(ColorConverter.ConvertFromString(Part2), Color)
                Case Else
                    Throw New Exception("Invalid Color Identifier : " & Part1)
            End Select
        Else
            Throw New Exception("Invalid MGP Color #AKP.")
        End If
        Return Result
    End Function

End Class
