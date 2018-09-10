Public Class MathSymbolConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim ValueString As String = value.ToString

        If value = Double.PositiveInfinity Then Return "+∞"

        If value = Double.NegativeInfinity Then Return "-∞"

        If Math.Abs(value) > 3 Then 'مضارب صحیح پی
            Dim IntPart As Integer = Fix(value / Math.PI)
            Dim DecPart As Double = (value / Math.PI) - Fix(value / Math.PI)

            If (Math.Abs(DecPart) < 1) And (Math.Abs(DecPart) > 0.99) Then
                Dim DivRem As Integer = IntPart + Math.Sign(DecPart)
                Dim DRS As String = DivRem.ToString
                If DRS = "1" Then DRS = ""
                If DRS = "-1" Then DRS = "-"
                Return DRS & "π"

            ElseIf Math.Abs(DecPart) > 1 And Math.Abs(DecPart) < 1.01 Then
                Dim DivRem As Integer = IntPart
                Dim DRS As String = DivRem.ToString
                If DRS = "1" Then DRS = ""
                If DRS = "-1" Then DRS = "-"
                Return DRS & "π"

            Else
                'MsgBox("NOT PI !")
            End If

        ElseIf ValueString.Substring(ValueString.IndexOf(".") + 1).Length > 1 Then 'مقسوم علیه های گویا پی

            Dim Neg As Boolean = (value < 0)
            Dim DRS As String = ""
            Dim DivByPI As Double = Math.Abs(value) / Math.PI
            For n = 2 To 12
                If Math.Abs(DivByPI - 1 / n) < 0.01 Then DRS = "/" & n.ToString
            Next
            If DRS <> "" Then
                Return If(Neg, "-", "") & "π" & DRS
            Else
                'MsgBox("NOT PI !")
            End If

        End If

        Return Math.Round(value, 2, MidpointRounding.AwayFromZero)

    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Dim Input As String = CStr(value).ToLower.Trim()

        Dim Neg As Boolean = False

        If Input.StartsWith("+") Then
            Input = Input.TrimStart("+")
        ElseIf Input.StartsWith("-") Then
            Neg = True
            Input = Input.TrimStart("-")
        End If

        If Input = "inf" Then
            Return If(Neg, -1, 1) * Double.PositiveInfinity
        End If

        If Input = "∞" Then
            Return If(Neg, -1, 1) * Double.PositiveInfinity
        End If

        If Input.Contains("pi") Then
            Dim PiIndex As Integer = Input.IndexOf("pi")
            Dim Prefix As String = Input.Substring(0, PiIndex)
            Dim Suffix As String = Input.Substring(PiIndex + 2)

            Dim PrefixInt As Double = 1
            If Prefix <> "" Then
                PrefixInt = CDbl(Prefix)
            End If
            Dim SuffixInt As Double = 1
            If Suffix <> "" AndAlso Suffix.StartsWith("/") Then
                SuffixInt = CDbl(Suffix.TrimStart("/"))
            End If

            Return If(Neg, -1, 1) * PrefixInt * Math.PI / SuffixInt
        End If

        If Input.Contains("π") Then
            Dim PiIndex As Integer = Input.IndexOf("π")
            Dim Prefix As String = Input.Substring(0, PiIndex)
            Dim Suffix As String = Input.Substring(PiIndex + 1)

            Dim PrefixInt As Double = 1
            If Prefix <> "" Then
                PrefixInt = CDbl(Prefix)
            End If
            Dim SuffixInt As Double = 1
            If Suffix <> "" AndAlso Suffix.StartsWith("/") Then
                SuffixInt = CDbl(Suffix.TrimStart("/"))
            End If

            Return If(Neg, -1, 1) * PrefixInt * Math.PI / SuffixInt
        End If

        Try
            Return CDbl(value)
        Catch
            'Null (WPF Detects As Error)
        End Try
    End Function

    Public Function ConvertForExport(value As Object) As String
        Select Case value
            Case Is = Double.PositiveInfinity
                Return "+inf"
            Case Is = Double.NegativeInfinity
                Return "-inf"
            Case Is = Math.PI
                Return "pi"
            Case Is = -Math.PI
                Return "-pi"
            Case Is = Math.PI / 2
                Return "pi/2"
            Case Is = -Math.PI
                Return "-pi/2"
            Case Is = 2 * Math.PI
                Return "2pi"
            Case Is = -2 * Math.PI
                Return "-2pi"
            Case Else
                Return CStr(value)
        End Select
    End Function

End Class
