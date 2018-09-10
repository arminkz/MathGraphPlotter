Imports System.Text.RegularExpressions
Imports System.Runtime.CompilerServices

Public Module EquationTools

    'پاکسازی توابع ریاضی
    Public Function CleanMathFunctions(InputFormula As String)
        Dim ExperStr = InputFormula.ToLower
        '' Disable Functions
        'Invers TRIG
        ExperStr = Regex.Replace(ExperStr, "arcsin", "??????")
        ExperStr = Regex.Replace(ExperStr, "asin", "????")
        ExperStr = Regex.Replace(ExperStr, "arccos", "??????")
        ExperStr = Regex.Replace(ExperStr, "acos", "????")
        ExperStr = Regex.Replace(ExperStr, "arctan", "??????")
        ExperStr = Regex.Replace(ExperStr, "atan", "????")
        ExperStr = Regex.Replace(ExperStr, "atn", "???")
        'TRIG
        ExperStr = Regex.Replace(ExperStr, "sin", "???")
        ExperStr = Regex.Replace(ExperStr, "cos", "???")
        ExperStr = Regex.Replace(ExperStr, "tan", "???")
        'LOG
        ExperStr = Regex.Replace(ExperStr, "log", "???")
        ExperStr = Regex.Replace(ExperStr, "ln", "??")
        'ROUND
        ExperStr = Regex.Replace(ExperStr, "int", "???")
        ExperStr = Regex.Replace(ExperStr, "floor", "?????")
        ExperStr = Regex.Replace(ExperStr, "ceil", "????")
        'ABS - SGN
        ExperStr = Regex.Replace(ExperStr, "abs", "???")
        ExperStr = Regex.Replace(ExperStr, "sgn", "???")
        'CONST
        ExperStr = Regex.Replace(ExperStr, "pi", "??")
        ExperStr = Regex.Replace(ExperStr, "e", "?")
        'Other ....
        Return ExperStr
    End Function

    Public Function Replace(Input As String, Pattern As String, Replacement As Double) As String
        Return Regex.Replace(Input, "(?<![a-zA-Z]+)" + Pattern + "(?![a-zA-Z]+)", CStr(Replacement))
    End Function

    Public Function Replace(Input As String, Pattern As String, Replacement As String) As String
        Return Regex.Replace(Input, "(?<![a-zA-Z]+)" + Pattern + "(?![a-zA-Z]+)", Replacement)
    End Function

    <Extension>
    Public Function ContainsP(Input As String, Pattern As String)
        Return Regex.IsMatch(Input, "(?<![a-zA-Z]+)" + Pattern + "(?![a-zA-Z]+)")
    End Function

    Public Sub FixNotWrittenMul(ByRef Exp As String)
        Dim FixedStr As String = Exp
        Dim InsertedTimes As Integer = 0
        For Each MAT As Match In Regex.Matches(Exp, "[0-9]+[a-zA-Z]+")
            Dim NumMAT As Match = Regex.Match(MAT.Value, "[0-9]+")
            Dim AfterNumberIndex As Integer = MAT.Index + NumMAT.Index + NumMAT.Value.Length + InsertedTimes
            FixedStr = FixedStr.Insert(AfterNumberIndex, "*")
            InsertedTimes += 1
        Next
        Exp = FixedStr
    End Sub

    'تشخیص نوع معادله
    Public Sub DetectEqType(ByRef Eq As Equation, Optional DrawMode As String = "2D")
        Dim EqExp As String = Eq.Expression.ToLower

        Dim IsParametric As Boolean = EqExp.Contains(",") 'معادله پارامتریک
        Dim IsImplicit As Boolean = False 'تابع ضمنی
        Dim OutUnknown As Boolean = False

        Dim Is2D As Boolean = (DrawMode = "2D")
        Dim Is3D As Boolean = (DrawMode = "3D")

        'ورودی کارتزین
        Dim InX As Boolean = False
        Dim InY As Boolean = False
        Dim InZ As Boolean = False
        Dim InN As Boolean = False

        'ورودی قطبی
        Dim InT As Boolean = False
        Dim InR As Boolean = False
        Dim InP As Boolean = False

        'خروجی کارتزین
        Dim OutX As Boolean = False
        Dim OutY As Boolean = False
        Dim OutZ As Boolean = False

        Dim OutW As Boolean = False

        'خروجی قطبی
        Dim OutT As Boolean = False
        Dim OutR As Boolean = False
        Dim OutP As Boolean = False

        If Not (EqExp Is Nothing Or EqExp = "") Then

            If IsParametric Then 'معادله پارامتریک

                Dim Param() As String = EqExp.Split(",")
                If Param.Length = 2 Then
                    If Param(0).Contains("=") And Param(1).Contains("=") Then
                        Eq.Type = EquationType.ParametricU
                    End If
                ElseIf Param.Length = 3 Then
                    If Param(0).Contains("=") And Param(1).Contains("=") And Param(2).Contains("=") Then
                        Eq.Type = EquationType.ParametricU
                        If EqExp.ContainsP("u") And EqExp.ContainsP("v") Then
                            Eq.Type = EquationType.ParametricUV
                        End If
                    End If
                End If
                If Param.Length = 4 Then
                    Eq.Type = EquationType.Parametric4D
                End If

            Else 'تابع غیر پارامتریک

                If EqExp.Contains("=") Then 'خروجی مشخص

                    Dim Func() As String = EqExp.Split("=")
                    Dim FuncFrist As String = Func(0).Trim(" ")
                    Dim FuncSecond As String = Func(1).Trim(" ")

                    If FuncFrist.Length = 1 And (Not IsNumeric(FuncFrist)) Then

                        Select Case FuncFrist.ToLower
                            Case "x"
                                OutX = True
                            Case "y"
                                OutY = True
                            Case "z"
                                OutZ = True
                            Case "w"
                                OutW = True
                            Case "t"
                                OutT = True
                            Case "r"
                                OutR = True
                            Case "p"
                                OutP = True
                        End Select

                        InX = FuncSecond.ContainsP("x")
                        InY = FuncSecond.ContainsP("y")
                        InZ = FuncSecond.ContainsP("z")
                        InN = FuncSecond.ContainsP("n")

                        InT = FuncSecond.ContainsP("t")
                        InR = FuncSecond.ContainsP("r")
                        InP = FuncSecond.ContainsP("p")

                    ElseIf FuncSecond.Length = 1 And (Not IsNumeric(FuncSecond)) Then ' تابع واقعی با جابجایی

                        Select Case FuncSecond.ToLower
                            Case "x"
                                OutX = True
                            Case "y"
                                OutY = True
                            Case "z"
                                OutZ = True
                            Case "w"
                                OutW = True
                            Case "t"
                                OutT = True
                            Case "r"
                                OutR = True
                            Case "p"
                                OutP = True
                        End Select

                        InX = FuncFrist.ContainsP("x")
                        InY = FuncFrist.ContainsP("y")
                        InZ = FuncFrist.ContainsP("z")
                        InZ = FuncFrist.ContainsP("n")

                        InT = FuncFrist.ContainsP("t")
                        InR = FuncFrist.ContainsP("r")
                        InP = FuncFrist.ContainsP("p")

                    Else   ' تابع ضمنی

                        IsImplicit = True

                        Eq.HasError = True
                        Eq.ErrorMsg = " خروجی تابع باید برحسب x یا y یا z (کارتزین) ویا برحسب r یا t یا p (قطبی) باشد." & vbCrLf & _
                            "لطفا به بخش راهنما مراجعه کنید."
                    End If

                Else 'خروجی نامشخص
                    OutUnknown = True

                    InX = EqExp.ContainsP("x")
                    InY = EqExp.ContainsP("y")
                    InZ = EqExp.ContainsP("z")
                    InN = EqExp.ContainsP("n")

                    InT = EqExp.ContainsP("t")
                    InR = EqExp.ContainsP("r")
                    InP = EqExp.ContainsP("p")

                End If

            End If

        Else 'ضابطه تابع تایپ نشده

            'Empty Equation
            Eq.Type = EquationType.None

        End If

        If Not OutUnknown Then

            If OutY Then
                If Not InN Then
                    Eq.Type = EquationType.CartesianY
                    If InZ And DrawMode = "2D" Then
                        Eq.HasError = True
                        Eq.ErrorMsg = ".لطفا حالت رسم را به 3 بعدی تغییر بدهید" & vbCrLf & "در حالت 2 بعدی متغییر Z نادیده گرفته می شود."
                    End If
                    Return
                Else
                    Eq.Type = EquationType.CartesianN
                    Return
                End If
            End If

            If OutX Then
                Eq.Type = EquationType.CartesianX
                If InZ And DrawMode = "2D" Then
                    Eq.HasError = True
                    Eq.ErrorMsg = ".لطفا حالت رسم را به 3 بعدی تغییر بدهید" & vbCrLf & "در حالت 2 بعدی متغییر Z نادیده گرفته می شود."
                End If
                Return
            End If


            If OutZ Then
                Eq.Type = EquationType.CartesianZ
                If DrawMode = "2D" Then
                    Eq.HasError = True
                    Eq.ErrorMsg = "لطفا حالت رسم را به 3 بعدی تغییر بدهید"
                End If
                Return
            End If

            If OutW Then
                Eq.Type = EquationType.CartesianW
                If DrawMode <> "4D" Then
                    Eq.HasError = True
                    Eq.ErrorMsg = "لطفا حالت رسم را به 4 بعدی تغییر بدهید"
                End If
                Return
            End If


            If OutR Then
                Eq.Type = EquationType.PolarR
                If InP And DrawMode = "2D" Then
                    Eq.HasError = True
                    Eq.ErrorMsg = ".لطفا حالت رسم را به 3 بعدی تغییر بدهید" & vbCrLf & "در حالت 2 بعدی متغییر P نادیده گرفته می شود."
                End If
                Return
            End If

            If OutT Then
                Eq.Type = EquationType.PolarT
                If InP And DrawMode = "2D" Then
                    Eq.HasError = True
                    Eq.ErrorMsg = ".لطفا حالت رسم را به 3 بعدی تغییر بدهید" & vbCrLf & "در حالت 2 بعدی متغییر P نادیده گرفته می شود."
                End If
                Return
            End If

            If OutP Then
                Eq.Type = EquationType.PolarP
                If DrawMode = "2D" Then
                    Eq.HasError = True
                    Eq.ErrorMsg = "لطفا حالت رسم را به 3 بعدی تغییر بدهید"
                End If
                Return
            End If

        Else

            If InX And InY And Not InZ Then 'z = F(x,y)
                Eq.Expression = "z=" & Eq.Expression
                Eq.Type = EquationType.CartesianZ
                If DrawMode = "2D" Then
                    Eq.HasError = True
                    Eq.ErrorMsg = ".لطفا حالت رسم را به 3 بعدی تغییر بدهید"
                End If
                Return
            End If

            If InX And Not InY Then 'y = F(x)
                Eq.Expression = "y=" & Eq.Expression
                Eq.Type = EquationType.CartesianY
                If InZ And DrawMode = "2D" Then  'y = F(x,z)
                    Eq.HasError = True
                    Eq.ErrorMsg = ".لطفا حالت رسم را به 3 بعدی تغییر بدهید" & vbCrLf & "در حالت 2 بعدی متغییر Z نادیده گرفته می شود."
                End If
                Return
            End If

            If InY And Not InX Then 'x = F(y)
                Eq.Expression = "x=" & Eq.Expression
                Eq.Type = EquationType.CartesianX
                If InZ And DrawMode = "2D" Then  'x = F(y,z)
                    Eq.HasError = True
                    Eq.ErrorMsg = ".لطفا حالت رسم را به 3 بعدی تغییر بدهید" & vbCrLf & "در حالت 2 بعدی متغییر Z نادیده گرفته می شود."
                End If
                Return
            End If

            If InX And InY And InZ Then
                Eq.Expression = "w=" & Eq.Expression
                Eq.Type = EquationType.CartesianW
                If DrawMode <> "4D" Then  'w = F(x,y,z)
                    Eq.HasError = True
                    Eq.ErrorMsg = ".لطفا حالت رسم را به 4 بعدی تغییر بدهید"
                End If
                Return
            End If

            If InN Then
                Eq.Expression = "y=" & Eq.Expression
                Eq.Type = EquationType.CartesianN
                If DrawMode <> "2D" Then  'w = F(x,y,z)
                    Eq.HasError = True
                    Eq.ErrorMsg = "دنباله ها فقط در حالت 2 بعدی قابل رسم هستند."
                End If
                Return
            End If

            If InR And InT Then 'p = F(r,t)
                Eq.Expression = "p=" & Eq.Expression
                Eq.Type = EquationType.PolarP
                If DrawMode = "2D" Then
                    Eq.HasError = True
                    Eq.ErrorMsg = ".لطفا حالت رسم را به 3 بعدی تغییر بدهید"
                End If
                Return
            End If

            If InT And Not InR Then 'r=F(t)
                Eq.Expression = "r=" & Eq.Expression
                Eq.Type = EquationType.PolarR
                If InP And DrawMode = "2D" Then  'r=F(t,p)
                    Eq.HasError = True
                    Eq.ErrorMsg = ".لطفا حالت رسم را به 3 بعدی تغییر بدهید" & vbCrLf & "در حالت 2 بعدی متغییر P نادیده گرفته می شود."
                End If
                Return
            End If

            If InR And Not InT Then 't = F(r)
                Eq.Expression = "t=" & Eq.Expression
                Eq.Type = EquationType.PolarT
                If InP And DrawMode = "2D" Then  't = F(r,p)
                    Eq.HasError = True
                    Eq.ErrorMsg = ".لطفا حالت رسم را به 3 بعدی تغییر بدهید" & vbCrLf & "در حالت 2 بعدی متغییر P نادیده گرفته می شود."
                End If
                Return
            End If


        End If
    End Sub

    Public Function GetSimplifiedEquation(Eq As Equation) As String

        Return 0
    End Function

    Public Function GetParameter(Eq As Equation, ParamOut As String) As String
        If Eq.Type = EquationType.ParametricU Or EquationType.ParametricUV Or EquationType.Parametric4D Then
            Dim EqExp As String = Eq.Expression.ToLower
            Dim Param() As String = EqExp.Split(",")
            If Param.Count > 1 Then
                For Each P As String In Param
                    If P.Contains("=") Then
                        Dim PED() As String = P.Split("=")
                        If PED(0) = ParamOut.ToLower Then Return PED(1)
                        If PED(1) = ParamOut.ToLower Then Return PED(0)
                    Else
                        Eq.HasError = True
                        Eq.ErrorMsg = "پارامتر ها به خوبی مشخص نشده اند."
                    End If
                Next
            Else
                Eq.HasError = True
                Eq.ErrorMsg = "معادله ی پارامتری حداقل باید دو خروجی داشته باشد"
            End If
        Else
            Throw New Exception("Equation Type Is Not Parametric. #AKP" & vbCrLf & "Use Detect Type Frist!")
        End If
        Return ""
    End Function

End Module