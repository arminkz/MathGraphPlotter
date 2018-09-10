Imports System.Collections.Generic
Imports System.Linq
Imports System.Globalization
Imports System.Collections.ObjectModel


Public Class MathParser

    Public Sub New(Optional LoadPreDefinedFunctions As Boolean = True, Optional LoadPreDefinedOperators As Boolean = True, Optional LoadPreDefinedVariables As Boolean = True)

        If LoadPreDefinedOperators Then
            ' by default, we will load basic arithmetic operators.
            ' please note, its possible to do it either inside the constructor,
            ' or outside the class. the lowest value will be executed first!
            OperatorList.Add("%")
            OperatorList.Add("^")
            'OperatorList.Add(":")
            OperatorList.Add("/")
            OperatorList.Add("*")
            OperatorList.Add("-")
            OperatorList.Add("+")
            OperatorList.Add(">")
            OperatorList.Add("<")
            OperatorList.Add("=")

            ' when an operator is executed, the parser needs to know how.
            ' this is how you can add your own operators. note, the order
            ' in this list does not matter.
            _operatorAction.Add("%", Function(A, B) A Mod B)
            _OperatorAction.Add("^", Function(A, B) CDbl(Math.Pow(CDbl(A), CDbl(B))))
            _operatorAction.Add(":", Function(A, B) A / B)
            _operatorAction.Add("/", Function(A, B) A / B)
            _operatorAction.Add("*", Function(A, B) A * B)
            _operatorAction.Add("+", Function(A, B) A + B)
            _operatorAction.Add("-", Function(A, B) A - B)

            _operatorAction.Add(">", Function(A, B) If(A > B, 1, 0))
            _operatorAction.Add("<", Function(A, B) If(A < B, 1, 0))
            _operatorAction.Add("=", Function(A, B) If(A = B, 1, 0))
        End If


        If LoadPreDefinedFunctions Then
            ' these are the basic functions you might be able to use.
            ' as with operators, localFunctions might be adjusted, i.e.
            ' you can add or remove a function.
            ' please open the "MathosTest" project, and find MathParser.cs
            ' in "CustomFunction" you will see three ways of adding 
            ' a new function to this variable!
            ' EACH FUNCTION MAY ONLY TAKE ONE PARAMETER, AND RETURN ONE
            ' VALUE. THESE VALUES SHOULD BE IN "Double FORMAT"!
            LocalFunctions.Add("abs", Function(x) Math.Abs(CDbl(x(0))))

            LocalFunctions.Add("cos", Function(x) Math.Cos(CDbl(x(0))))
            LocalFunctions.Add("cosh", Function(x) Math.Cosh(CDbl(x(0))))
            LocalFunctions.Add("arccos", Function(x) Math.Acos(CDbl(x(0))))
            LocalFunctions.Add("acos", Function(x) CDbl(Math.Acos(CDbl(x(0)))))

            LocalFunctions.Add("sin", Function(x) Math.Sin(CDbl(x(0))))
            LocalFunctions.Add("sinh", Function(x) Math.Sinh(CDbl(x(0))))
            LocalFunctions.Add("arcsin", Function(x) Math.Asin(CDbl(x(0))))
            LocalFunctions.Add("asin", Function(x) CDbl(Math.Asin(CDbl(x(0)))))

            LocalFunctions.Add("tan", Function(x) Math.Tan(CDbl(x(0))))
            LocalFunctions.Add("tanh", Function(x) Math.Tanh(CDbl(x(0))))
            LocalFunctions.Add("arctan", Function(x) Math.Atan(CDbl(x(0))))
            LocalFunctions.Add("atan", Function(x) CDbl(Math.Atan(CDbl(x(0)))))
            'LocalFunctions.Add("arctan2", x => (Double)Math.Atan2((double)x[0], (double)x[1]));

            LocalFunctions.Add("cot", Function(x) 1 / Math.Tan(CDbl(x(0))))
            'LocalFunctions.Add("coth", Function(x) Math.Tanh(CDbl(x(0))))
            LocalFunctions.Add("arccot", Function(x) Math.Atan(CDbl(1 / x(0))))

            LocalFunctions.Add("sqr", Function(x) Math.Sqrt(CDbl(x(0))))
            LocalFunctions.Add("sqrt", Function(x) Math.Sqrt(CDbl(x(0))))
            LocalFunctions.Add("rem", Function(x) Math.IEEERemainder(CDbl(x(0)), CDbl(x(1))))
            LocalFunctions.Add("root", Function(x) Math.Pow(CDbl(x(0)), 1.0 / CDbl(x(1))))

            LocalFunctions.Add("pow", Function(x) Math.Pow(CDbl(x(0)), CDbl(x(1))))

            LocalFunctions.Add("exp", Function(x) CDbl(Math.Exp(CDbl(x(0)))))
            'LocalFunctions.Add("log", x => (Double)Math.Log((double)x[0]));
            'LocalFunctions.Add("log10", x => (Double)Math.Log10((double)x[0]));
            ' input[0] is the number
            ' input[1] is the base

            LocalFunctions.Add("log", Function(Input As Double())
                                          If Input.Length = 1 Then
                                              Return Math.Log10(CDbl(Input(0)))
                                          ElseIf Input.Length = 2 Then
                                              Return Math.Log(CDbl(Input(0)), CDbl(Input(1)))
                                          Else
                                              'Error
                                              Return 0
                                          End If
                                      End Function)
            LocalFunctions.Add("ln", Function(x) CDbl(Math.Log(CDbl(x(0)))))

            LocalFunctions.Add("round", Function(x) Math.Round(CDbl(x(0))))
            LocalFunctions.Add("truncate", Function(x) Math.Truncate(CDbl(x(0))))

            LocalFunctions.Add("floor", Function(x) Math.Floor(CDbl(x(0))))
            LocalFunctions.Add("int", Function(x) Math.Floor(CDbl(x(0))))

            LocalFunctions.Add("ceiling", Function(x) Math.Ceiling(CDbl(x(0))))
            LocalFunctions.Add("ceil", Function(x) Math.Floor(CDbl(x(0))))

            LocalFunctions.Add("sign", Function(x) Math.Sign(CDbl(x(0))))
            LocalFunctions.Add("sgn", Function(x) Math.Sign(CDbl(x(0))))
        End If

        If LoadPreDefinedVariables Then

            LocalVariables.Add("pi", 3.14159265358979)
            LocalVariables.Add("e", 2.71828182845905)
            LocalVariables.Add("phi", 1.61803398874989)

            'LocalVariables.Add("pi2", CDec(6.28318530717959))
            'LocalVariables.Add("pi05", CDec(1.5707963267949))
            'LocalVariables.Add("pi025", CDec(0.785398163397448))
            'LocalVariables.Add("pi0125", CDec(0.392699081698724))
            'LocalVariables.Add("pitograd", CDec(57.2957795130823))
            'LocalVariables.Add("piofgrad", CDec(0.0174532925199433))
            'LocalVariables.Add("major", CDec(0.618033988749895))
            'LocalVariables.Add("minor", CDec(0.381966011250105))

        End If
    End Sub


    Private _OperatorList As New List(Of String)
    Public Property OperatorList() As List(Of String)
        Get
            Return _operatorList
        End Get
        Set(value As List(Of String))
            _operatorList = value
        End Set
    End Property

    Private _OperatorAction As New Dictionary(Of String, Func(Of Double, Double, Double))()
    Public Property OperatorAction() As Dictionary(Of String, Func(Of Double, Double, Double))
        Get
            Return _OperatorAction
        End Get
        Set(value As Dictionary(Of String, Func(Of Double, Double, Double)))
            _OperatorAction = value
        End Set
    End Property

    Private _LocalFunctions As New Dictionary(Of String, Func(Of Double(), Double))()
    Public Property LocalFunctions() As Dictionary(Of String, Func(Of Double(), Double))
        Get
            Return _LocalFunctions
        End Get
        Set(value As Dictionary(Of String, Func(Of Double(), Double)))
            _LocalFunctions = value
        End Set
    End Property

    Private _LocalVariables As New Dictionary(Of String, Double)()
    Public Property LocalVariables() As Dictionary(Of String, Double)
        Get
            Return _LocalVariables
        End Get
        Set(value As Dictionary(Of String, Double))
            _LocalVariables = value
        End Set
    End Property

    'Private _CULTURE_INFO As CultureInfo = CultureInfo.InvariantCulture
    Public ReadOnly Property CULTURE_INFO() As CultureInfo
        Get
            Return CultureInfo.InvariantCulture
        End Get
    End Property


    Public Function Parse(MathExpression As String) As Double
        'MsgBox("Pass In Value = " & MathExpression)
        Return MathParserLogic(Tokenize(Expr:=MathExpression))
    End Function

    Public Function Parse(MathExpression As ReadOnlyCollection(Of String)) As Double
        Return MathParserLogic(_Tokens:=New List(Of String)(MathExpression))
    End Function

    Public Function ProgrammaticallyParse(MathExpression As String, Optional CorrectExpression As Boolean = True, Optional IdentifyComments As Boolean = True) As Double
        If IdentifyComments Then
            ' Delete Comments #{Comment}#
            ' Delete Comments #Comment
            MathExpression = System.Text.RegularExpressions.Regex.Replace(MathExpression, "#\{.*?\}#", "")
            MathExpression = System.Text.RegularExpressions.Regex.Replace(MathExpression, "#.*$", "")
        End If

        If CorrectExpression Then
            MathExpression = Correction(MathExpression)
        End If

        If MathExpression.Contains("let") Then
            Dim VarName As String
            If MathExpression.Contains("be") Then
                VarName = MathExpression.Substring(MathExpression.IndexOf("let") + 3, MathExpression.IndexOf("be") - MathExpression.IndexOf("let") - 3)
                MathExpression = MathExpression.Replace(VarName & "be", "")
            Else
                VarName = MathExpression.Substring(MathExpression.IndexOf("let") + 3, MathExpression.IndexOf("=") - MathExpression.IndexOf("let") - 3)
                MathExpression = MathExpression.Replace(VarName & "=", "")
            End If

            VarName = VarName.Replace(" ", "")
            MathExpression = MathExpression.Replace("let", "")
            Dim VarValue As Double = Parse(MathExpression)

            If LocalVariables.ContainsKey(VarName) Then
                LocalVariables(VarName) = VarValue
            Else
                LocalVariables.Add(VarName, VarValue)
            End If
            Return VarValue
        ElseIf MathExpression.Contains(":=") Then
            Dim VarName As String = MathExpression.Substring(0, MathExpression.IndexOf(":="))
            MathExpression = MathExpression.Replace(VarName & ":=", "")

            Dim VarValue As Double = Parse(MathExpression)
            VarName = VarName.Replace(" ", "")
            If LocalVariables.ContainsKey(VarName) Then
                LocalVariables(VarName) = VarValue
            Else
                LocalVariables.Add(VarName, VarValue)
            End If

            Return VarValue
        Else
            Return Parse(MathExpression)
        End If
    End Function

    Public Function GetTokens(MathExpression As String) As ReadOnlyCollection(Of String)
        Return Tokenize(Expr:=MathExpression).AsReadOnly()
    End Function

    'Not for Now
    Private Function Correction(Input As String) As String
        ' Word corrections

        'Input = System.Text.RegularExpressions.Regex.Replace(Input, "\b(sqr|sqrt)\b", "sqr", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        'Input = System.Text.RegularExpressions.Regex.Replace(Input, "\b(atn|atan|arctan)\b", "arctan", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        'Input = System.Text.RegularExpressions.Regex.Replace(Input, "\b(asin|arcsin)\b", "arcsin", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        'Input = System.Text.RegularExpressions.Regex.Replace(Input, "\b(acos|arccos)\b", "arccos", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        'Input = System.Text.RegularExpressions.Regex.Replace(Input, "\b(atn2|atan2|arctan2)\b", "arctan2", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        '... and more

        Return input
    End Function

    Private Function Tokenize(Expr As String) As List(Of String)

        Dim _Tokens As New List(Of String)()
        Dim _Vector As String = ""


        Expr = Expr.Replace("+-", "-")
        Expr = Expr.Replace("-+", "-")
        Expr = Expr.Replace("--", "+")

        For i As Integer = 0 To Expr.Length - 1
            Dim Ch As Char = Expr.Chars(i)

            If Char.IsWhiteSpace(Ch) Then
                'do nothing

            ElseIf Char.IsLetter(Ch) Then
                If i <> 0 AndAlso (Char.IsDigit(Expr(i - 1)) OrElse Char.IsDigit(Expr(i - 1)) OrElse Expr(i - 1) = ")"c) Then
                    _Tokens.Add("*")
                End If

                _Vector = _Vector & Ch
                While i < (Expr.Length - 1) AndAlso Char.IsLetterOrDigit(Expr(i + 1))
                    i += 1
                    _Vector = _Vector & Expr(i)
                End While
                If _Vector IsNot Nothing Then
                    _Tokens.Add(_Vector)
                    _Vector = ""
                End If

            ElseIf Char.IsDigit(Ch) Then

                _Vector = _Vector & Ch
                While i < (Expr.Length - 1) AndAlso (Char.IsDigit(Expr(i + 1)) OrElse Expr(i + 1) = "."c)
                    i += 1
                    _Vector = _Vector & Expr(i)
                End While
                If _Vector IsNot Nothing Then
                    _Tokens.Add(_Vector)
                    _Vector = ""
                End If

            ElseIf i < (Expr.Length - 1) AndAlso (Ch = "-"c OrElse Ch = "+"c) AndAlso Char.IsDigit(Expr(i + 1)) AndAlso (i = 0 OrElse OperatorList.IndexOf(Expr(i - 1).ToString) <> -1 OrElse (i > 1 AndAlso Expr(i - 1) = "("c)) Then
                'تمایز بین منفی یکانی و عملگر منفی
                _Vector = _Vector & Ch
                While i < (Expr.Length - 1) AndAlso (Char.IsDigit(Expr(i + 1)) OrElse Expr(i + 1) = "."c)
                    i += 1
                    _Vector = _Vector & Expr(i)
                End While
                If _Vector IsNot Nothing Then
                    _Tokens.Add(_Vector)
                    _Vector = ""
                End If

            ElseIf ((i < (Expr.Length - 1)) AndAlso (Ch = "-"c OrElse Ch = "+"c) AndAlso (Char.IsLetter(Expr(i + 1)) OrElse Expr(i + 1) = "("c)) Then 'AndAlso (i = 0 OrElse OperatorList.IndexOf(Expr(i - 1).ToString) <> -1 OrElse (i > 1 AndAlso Expr(i - 1) = "("c))
                'MsgBox("Alghorithm Worked !")
                If Ch = "-"c Then
                    _Tokens.Add("-1")
                    _Tokens.Add("*")
                End If

            ElseIf Ch = "("c Then
                If i <> 0 AndAlso (Char.IsDigit(Expr(i - 1)) OrElse Char.IsDigit(Expr(i - 1)) OrElse Expr(i - 1) = ")"c) Then
                    _Tokens.Add("*")
                    ' if we remove this line, we would be able to have numbers in function names. however, then we can't parser 3(2+2)
                    _Tokens.Add("(")
                Else
                    _Tokens.Add("(")
                End If
            Else
                _Tokens.Add(Ch.ToString())
            End If
        Next

        Return _Tokens
    End Function

    Private Function MathParserLogic(_Tokens As List(Of String)) As Double
        ' CALCULATING THE EXPRESSIONS INSIDE THE BRACKETS
        ' IF NEEDED, EXECUTE A FUNCTION

        'Variables Replacement
        For I As Integer = 0 To _Tokens.Count - 1
            If LocalVariables.Keys.Contains(_Tokens(I)) Then
                _Tokens(I) = LocalVariables(_Tokens(I)).ToString(CULTURE_INFO)
            End If
        Next

        While _Tokens.IndexOf("(") <> -1
            ' getting data between "(", ")"
            Dim Open As Integer = _Tokens.LastIndexOf("(")
            Dim Close As Integer = _Tokens.IndexOf(")", Open)
            ' in case open is -1, i.e. no "(" // , open == 0 ? 0 : open - 1
            If Open >= Close Then
                ' if there is no closing bracket, throw a new exception
                Throw New ArithmeticException("پارانتز باز شده بسته نشده است.")
            End If
            Dim RoughExpr As New List(Of String)()
            For J As Integer = Open + 1 To Close - 1
                RoughExpr.Add(_Tokens(J))
            Next

            Dim Result As Double = 0
            ' the temporary result is stored here
            Dim FunctionName As String = _Tokens(If(Open = 0, 0, Open - 1))
            Dim _Args As Double() = New Double(-1) {}
            If LocalFunctions.Keys.Contains(FunctionName) Then
                If RoughExpr.Contains(",") Then
                    'Converting all arguments into a Double array
                    For i As Integer = 0 To RoughExpr.Count - 1
                        Dim FirstCommaOrEndOfExpression As Integer = If(RoughExpr.IndexOf(",", i) <> -1, RoughExpr.IndexOf(",", i), RoughExpr.Count)

                        Dim DefaultExpr As New List(Of String)()
                        While i < FirstCommaOrEndOfExpression
                            DefaultExpr.Add(RoughExpr(i))
                            i += 1
                        End While

                        ' changing the size of the array of arguments
                        Array.Resize(_Args, _Args.Length + 1)
                        If DefaultExpr.Count = 0 Then
                            _Args(_Args.Length - 1) = 0
                        Else
                            _Args(_Args.Length - 1) = BasicArithmeticalExpression(DefaultExpr)
                        End If
                    Next

                    'Finnaly, passing the arguments to the given function
                    Result = Double.Parse(LocalFunctions(FunctionName)(_Args).ToString(CULTURE_INFO), CULTURE_INFO)
                Else

                    'But if we only have one argument, then we pass it directly to the function
                    Result = Double.Parse(LocalFunctions(FunctionName)(New [Double]() {BasicArithmeticalExpression(RoughExpr)}).ToString(CULTURE_INFO), CULTURE_INFO)
                End If
            Else
                ' if no function is need to execute following expression, pass it
                ' to the "BasicArithmeticalExpression" method.
                Result = BasicArithmeticalExpression(RoughExpr)
            End If

            ' when all the calculations have been done
            ' we replace the "opening bracket with the result"
            ' and removing the rest.
            _Tokens(open) = result.ToString(CULTURE_INFO)
            _Tokens.RemoveRange(Open + 1, Close - Open)
            If LocalFunctions.Keys.Contains(FunctionName) Then
                ' if we also executed a function, removing
                ' the function name as well.
                _Tokens.RemoveAt(Open - 1)
            End If
        End While

        ' at this point, we should have replaced all brackets
        ' with the appropriate values, so we can simply
        ' calculate the expression. it's not so complex
        ' any more!
        Return BasicArithmeticalExpression(_Tokens)
    End Function

    Private Function BasicArithmeticalExpression(_Tokens As List(Of String)) As Double
        ' PERFORMING A BASIC ARITHMETICAL EXPRESSION CALCULATION
        ' THIS METHOD CAN ONLY OPERATE WITH NUMBERS AND OPERATORS
        ' AND WILL NOT UNDERSTAND ANYTHING BEYOND THAT.

        If _Tokens.Count = 1 Then
            Return Double.Parse(_Tokens(0), CULTURE_INFO)

        ElseIf _Tokens.Count = 2 Then
            Dim Op As String = _Tokens(0)
            If Op = "-" OrElse Op = "+" Then
                Return Double.Parse((If(Op = "+", "", "-")) & _Tokens(1), CULTURE_INFO)
            Else
                Return OperatorAction(Op)(0, Double.Parse(_Tokens(1), CULTURE_INFO))
            End If

        ElseIf _Tokens.Count = 0 Then
            Return 0
        End If

        For Each Op As String In OperatorList
            While _Tokens.IndexOf(Op) <> -1
                Dim OpPlace As Integer = _Tokens.IndexOf(Op)

                Dim NumberA As Double = Convert.ToDouble(_Tokens(OpPlace - 1), CULTURE_INFO)
                Dim NumberB As Double = Convert.ToDouble(_Tokens(OpPlace + 1), CULTURE_INFO)

                Dim Result As Double = OperatorAction(Op)(NumberA, NumberB)

                _Tokens(OpPlace - 1) = Result.ToString(CULTURE_INFO)
                _Tokens.RemoveRange(OpPlace, 2)
            End While
        Next
        Return Convert.ToDouble(_Tokens(0), CULTURE_INFO)
    End Function
End Class