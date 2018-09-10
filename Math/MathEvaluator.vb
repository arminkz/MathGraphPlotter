Imports System.Text.RegularExpressions

Public Class MathEvaluator

    Public Property EvaluatorMethod As EvalMethod = EvalMethod.MSSC

    Public Enum EvalMethod
        MathParser
        MathParserAuto
        MSSC
    End Enum

    Dim MP As MathParser
    Dim SC As MSScriptControl.ScriptControl

    Dim Result As Object

    Public Sub New()
        'Initialize AKP Math Parser
        MP = New MathParser
        'Initialize MSSC v1.0
        SC = New MSScriptControl.ScriptControl 'Add COM Refrence to Project  Microsoft Script Control
        SC.Language = "VBSCRIPT"
    End Sub

    Public Function Eval(Formula As Object, ByRef Success As Boolean, Optional IsCritical As Boolean = False) As Double
        Success = True

        If EvaluatorMethod = EvalMethod.MathParser Then
            Return MP.Parse(CStr(Formula))
            'Try

            'Catch Ex As Exception
            '    Success = False
            '    Return 0
            'End Try
        End If

        If EvaluatorMethod = EvalMethod.MathParserAuto Then
            Try
                Return MP.ProgrammaticallyParse(CStr(Formula))
            Catch Ex As Exception
                Success = False
                Return 0
            End Try
        End If

        If EvaluatorMethod = EvalMethod.MSSC Then
            Try
                Formula = MathToScript(Formula.ToLower)
            Catch
                Success = False
                If IsCritical Then
                    Return 0
                Else
                    Return Double.NaN
                End If
            End Try
            Try
                Result = Convert.ToDouble(SC.Eval(Formula))
            Catch Ex As Exception
                Success = False
                If IsCritical Then
                    Return 0
                Else
                    Return Double.NaN
                End If
            End Try
            Return Result
        End If

        Return Double.NaN
    End Function

    Public Function MathToScript(Formula As String) As String


        'If Formula.Contains("asin") Then
        '    Dim ArcSinRE As New Regex("asin\(.*\)")
        '    Dim ArcSinEvaluator As MatchEvaluator = New MatchEvaluator(AddressOf ReplaceFunc)

        '    Formula = ArcSinRE.Replace(Formula, ArcSinEvaluator)
        '    If Formula = Nothing Then Return "Error"
        'End If



        'ArcSin
        If Formula.Contains("arcsin") Then
            Formula = Regex.Replace(Formula, "arcsin", "asin")
        End If
        If Formula.Contains("asin") Then
            Dim ArcSinIn As Double = GetFunctionInput(Formula, "asin")
            If Not Double.IsNaN(Math.Asin(ArcSinIn)) And Not Double.IsNaN(ArcSinIn) Then
                Replace(Formula, "asin\(.*\)", Math.Asin(ArcSinIn).ToString)
                Formula = Regex.Replace(Formula, "asin\(.*\)", Math.Asin(ArcSinIn).ToString)
            Else
                Return "Error"
            End If
        End If

        'ArcCos
        If Formula.Contains("arccos") Then
            Formula = Regex.Replace(Formula, "arccos", "acos")
        End If
        If Formula.Contains("acos") Then
            Dim ArcCosIn As Double = GetFunctionInput(Formula, "acos")
            If Not Double.IsNaN(Math.Acos(ArcCosIn)) And Not Double.IsNaN(ArcCosIn) Then
                Formula = Regex.Replace(Formula, "acos\(.*\)", Math.Acos(ArcCosIn).ToString)
            Else
                Return "Error"
            End If
        End If

        'ArcTan
        If Formula.Contains("atan") Then
            Formula = Regex.Replace(Formula, "atan", "atn")
        End If
        If Formula.Contains("arctan") Then
            Formula = Regex.Replace(Formula, "arctan", "atn")
        End If

        'Log
        'If Formula.Contains("log") Then
        '    Dim LogIn As Double = GetFunctionInput(Formula, "log")
        '    If Not Double.IsNaN(Math.Log10(LogIn)) And Not Double.IsNaN(LogIn) Then
        '        Formula = Regex.Replace(Formula, "log\(.*\)", Math.Log10(LogIn).ToString)
        '    Else
        '        Return "Error"
        '    End If
        'End If

        'Ln (Neperian Logarithm)
        'If Formula.Contains("ln") Then
        '    Dim LnIn As Double = GetFunctionInput(Formula, "ln")
        '    If Not Double.IsNaN(Math.Log(LnIn)) And Not Double.IsNaN(LnIn) Then
        '        Formula = Regex.Replace(Formula, "ln\(.*\)", Math.Log(LnIn).ToString)
        '    Else
        '        Return "Error"
        '    End If
        'End If

        'Floor
        If Formula.Contains("floor") Then
            Dim FloorIn As Double = GetFunctionInputValue(Formula, "floor")
            If Not Double.IsNaN(Math.Floor(FloorIn)) And Not Double.IsNaN(FloorIn) Then
                Formula = Regex.Replace(Formula, "floor\(.*\)", Math.Floor(FloorIn).ToString)
            Else
                Return "Error"
            End If
        End If

        'Ceiling
        If Formula.Contains("ceil") Then
            Dim CeilingIn As Double = GetFunctionInputValue(Formula, "ceil")
            If Not Double.IsNaN(Math.Ceiling(CeilingIn)) And Not Double.IsNaN(CeilingIn) Then
                Formula = Regex.Replace(Formula, "ceil\(.*\)", Math.Ceiling(CeilingIn).ToString)
            Else
                Return "Error"
            End If
        End If

        'Sign
        If Formula.Contains("sgn") Then
            Dim SignIn As Double = GetFunctionInputValue(Formula, "sgn")
            If Not Double.IsNaN(Math.Sign(SignIn)) And Not Double.IsNaN(SignIn) Then
                Formula = Regex.Replace(Formula, "sgn\(.*\)", Math.Sign(SignIn).ToString)
            Else
                Return "Error"
            End If
        End If

        'PI
        If Formula.Contains("pi") Then
            Formula = Regex.Replace(Formula, "pi", Math.PI.ToString)
        End If

        'E (Neper)
        If Formula.Contains("e") Then
            Formula = Regex.Replace(Formula, "e", Math.E.ToString)
        End If

        Return Formula.ToString
    End Function

    Public Function GetFunctionInputValue(Exp As String, FuncName As String) As Double
        Dim ST As Integer = Exp.IndexOf("(", Exp.IndexOf(FuncName) + FuncName.Length) + 1
        Dim EN As Integer = Exp.IndexOf(")", ST)
        Dim Str As String = Exp.Substring(ST, EN - ST)
        Dim Val As Double = Convert.ToDouble(SC.Eval(MathToScript(Str)))
        Return Val
    End Function

    Shared Function GetFunctionInput(Exp As String, FuncName As String) As String
        Dim ST As Integer = Exp.IndexOf("(", Exp.IndexOf(FuncName) + FuncName.Length) + 1
        Dim EN As Integer = Exp.IndexOf(")", ST) + 1
        Dim Str As String = Exp.Substring(ST, EN - ST)
        'Dim Val As Double = Convert.ToDouble(SC.Eval(MathToScript(Str)))
        Return Str
    End Function

    Public Function ReplaceFunc(m As Match) As String
        If m.Value.StartsWith("asin") Then
            Dim FI As Double = GetFunctionInput(m.Value, "asin")
            Dim FIV As Double = Math.Asin(FI)
            If Not Double.IsNaN(FI) And Not Double.IsNaN(FIV) Then
                Return FIV
            End If
        End If
        Return Nothing
    End Function

End Class
