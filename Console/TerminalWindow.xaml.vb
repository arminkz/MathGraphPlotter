Imports System.Media
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Windows.Media.Media3D

Public Class TerminalWindow

    Public Property IsLogger As Boolean = False
    Public Property RealTextStartIndex As Integer

    Public Enum TextStyle
        Normal
        [Error]
        Success
    End Enum

    Private Sub Terminal_Loaded(sender As Object, e As RoutedEventArgs)
        Term.Text += "AKP Terminal v0.9" + vbCrLf
        Term.Text += "-----------------" + vbCrLf + vbCrLf
        RealTextStartIndex = Term.Text.Length
        Term.InsertNewPrompt()
        Term.ScrollToEnd()

        Term.RegisteredCommands.Add("help")
        Term.RegisteredCommands.Add("fam")
        Term.RegisteredCommands.Add("debug3d")
        Term.RegisteredCommands.Add("log")
        Term.RegisteredCommands.Add("redraw")
        Term.RegisteredCommands.Add("cubecor")
        Term.RegisteredCommands.Add("clear")
        'Term.RegisteredCommands.Add("simplfy")
        Term.RegisteredCommands.Add("anim")
        Term.RegisteredCommands.Add("get")
        Term.RegisteredCommands.Add("set")
        Term.RegisteredCommands.Add("snapshot")
        Term.RegisteredCommands.Add("boxfaces")
        Term.RegisteredCommands.Add("info")
        Term.RegisteredCommands.Add("reset")
        Term.RegisteredCommands.Add("about")
        Term.RegisteredCommands.Add("akp")
        Term.RegisteredCommands.Add("exit")

    End Sub

    'Public Sub DebugCommand()
    '    Dim Cmd As String = InputBox("Input Command", "Debug")
    '    Select Case Cmd
    '        Case "debug3d"
    '            ActivePlaneMode = PlaneMode.DebugMode
    '        Case "frenzy"
    '            Dim DT As New DispatcherTimer
    '            DT.Interval = TimeSpan.FromMilliseconds(100)
    '            AddHandler DT.Tick, AddressOf ChangeColor
    '            DT.Start()
    '    End Select
    'End Sub

    Private Sub Terminal_CommandEntered(sender As Object, e As Terminal.CommandEventArgs) Handles Term.CommandEntered

        If e.Command.Name.Length > 0 Then
            Select Case e.Command.Name.ToLower

                'Case "msc"
                '    Dim value As Double = CDbl(e.Command.Args(0))
                '    Term.InsertNewPrompt()

                Case "help"
                    If e.Command.Args.Length = 0 Then
                        Term.Text += vbCrLf + "Available Commands Are: " + vbCrLf + vbCrLf
                        For Each S As String In Term.RegisteredCommands
                            Term.Text += "    " + S + vbCrLf
                        Next
                        Term.Text += vbCrLf
                        Term.InsertNewPrompt()
                    End If

                Case "fam" ' Full Admin Mode
                        If e.Command.Args.Length > 0 Then
                            If e.Command.Args(0) = "alpine" Then
                                MainWindow.UnlockBetaFeatures = True
                                PrintText("Full Admin Mode Enabled - Beta Features Unlocked !")
                            Else
                                PrintText("Wrong Password !")
                            End If
                        Else
                            PrintText("please enter your password in this form :" + vbCrLf + "fam pass")
                    End If

                Case "debug3d"
                    TryCast(Owner, MainWindow).ActivePlaneMode = MainWindow.PlaneMode.DebugMode
                    TryCast(Owner, MainWindow).AutoRedrawCM()
                    Term.InsertNewPrompt()

                Case "log"
                        If e.Command.Args.Length > 0 Then
                            Select Case e.Command.Args(0)
                                Case "0"
                                    'Deactive
                                Case "1"
                                    ActivateLogger()
                                Case "save"
                                    SaveLog()
                                    Term.InsertNewPrompt()
                            End Select
                        End If

                Case "redraw"
                        TryCast(Owner, MainWindow).AutoRedrawCM()
                        PrintText("AutoRedrawCM Executed. Mode : " & TryCast(Owner, MainWindow).DrawMode)

                Case "cubecor"
                        If TryCast(Owner, MainWindow).DrawMode = "3D" Then
                            TryCast(Owner, MainWindow).DrawCoordinateCube()
                            PrintText("DrawCoordinateCube  Executed.")
                        Else
                            PrintText("DrawMode Must Be 3D. Change DrawMode Frist !")
                        End If

                Case "clear"
                        If e.Command.Args.Length > 0 Then
                            Select Case e.Command.Args(0)
                                Case "help"
                                    Term.Text += vbCrLf + "Available Commands Are: " + vbCrLf + vbCrLf
                                    Term.Text += "    " + "clear help" + vbCrLf
                                    Term.Text += "    " + "clear cor" + vbCrLf
                                    Term.Text += "    " + "clear cor2d" + vbCrLf
                                    Term.Text += "    " + "clear cor3d" + vbCrLf
                                    Term.Text += vbCrLf
                                    Term.InsertNewPrompt()
                                Case "cor"
                                    TryCast(Owner, MainWindow).Cor2D_Behind.Children.Clear()
                                    TryCast(Owner, MainWindow).VisualCorLines.Children.Clear()
                                    PrintText("Cleared Cor2D & Cor3D.")
                                Case "cor2d"
                                    TryCast(Owner, MainWindow).Cor2D_Behind.Children.Clear()
                                    PrintText("Cleared Cor2D.")
                                Case "cor3d"
                                    TryCast(Owner, MainWindow).VisualCorLines.Children.Clear()
                                    PrintText("Cleared Cor3D.")
                            End Select
                        End If

                    'Case "simplfy"
                    '        If e.Command.Args.Length > 0 Then
                    '            Dim FuncExp As String = e.Command.Args(0)
                    '            Dim var As String = If(e.Command.Args.Length > 1, e.Command.Args(1), "a")

                    '            Term.Text += vbCrLf + "Detect Variable :" + vbCrLf
                    '            PrintText(Regex.Replace(FuncExp, "(?<![a-zA-Z]+)" + var + "(?![a-zA-Z]+)", "<VAR>"))

                    '            Term.Text += "Fix Not Written Mul :" + vbCrLf
                    '            Dim FixedStr As String = FuncExp
                    '            Dim InsertedTimes As Integer = 0
                    '            For Each MAT As Match In Regex.Matches(FuncExp, "[0-9]+[a-zA-Z]+")
                    '                Dim NumMAT As Match = Regex.Match(MAT.Value, "[0-9]+")
                    '                Dim AfterNumberIndex As Integer = MAT.Index + NumMAT.Index + NumMAT.Value.Length + InsertedTimes
                    '                FixedStr = FixedStr.Insert(AfterNumberIndex, "*")
                    '                InsertedTimes += 1
                    '            Next
                    '            PrintText(FixedStr)
                    '    End If

                    'Case "vec4"
                    '    PrintText("Testing Vector 4D Cross Product ... ")
                    '    Dim X As New Vector4D(1, 0, 0, 0)
                    '    Dim Y As New Vector4D(0, 1, 0, 0)
                    '    Dim Z As New Vector4D(0, 0, 1, 0)
                    '    Dim W As Vector4D = Vector4D.CrossProduct(X, Y, Z)
                    '    MsgBox("X = " & W.X & "   Y = " & W.Y & "    Z = " & W.Z & "   W = " & W.W)

                Case "der"
                    If e.Command.Args.Length > 0 Then
                        Dim Formula As String = e.Command.Args(0)
                        If Formula.Contains("d/d") Then
                            Const Epsilon As Double = 0.001
                            Dim DerIndex As Integer = Formula.IndexOf("d/d")
                            Dim DerPer As String = Formula.Substring(DerIndex + 3, 1)
                            Dim DerIn As String = MathEvaluator.GetFunctionInput(Formula, "d/d" & DerPer)
                            MsgBox(DerIn)
                            Dim DerInEp1 As String = EquationTools.Replace(DerIn, DerPer, "(" & DerPer & "+" & Epsilon.ToString & ")")
                            Dim DerInEp2 As String = EquationTools.Replace(DerIn, DerPer, "(" & DerPer & "-" & Epsilon.ToString & ")")
                            Dim NewFormula As String = "(" & DerInEp1 & "-" & DerInEp2 & ") / " & (2 * Epsilon).ToString
                            MsgBox(NewFormula)

                            Term.InsertNewPrompt()
                            'Dim DerInEps As String = EquationTools.
                        End If
                    End If

                Case "anim"
                    If e.Command.Args.Length > 0 Then
                        Select Case e.Command.Args(0)
                            Case "stop"
                                TryCast(Owner, MainWindow).RotateTransformDT.Stop()
                            Case "x"
                                TryCast(Owner, MainWindow).RotateTransformAxis = New Vector3D(1, 0, 0)
                                TryCast(Owner, MainWindow).RotateTransformDT.Start()
                                PrintText("Animation Started !")
                            Case "y"
                                TryCast(Owner, MainWindow).RotateTransformAxis = New Vector3D(0, 1, 0)
                                TryCast(Owner, MainWindow).RotateTransformDT.Start()
                                PrintText("Animation Started !")
                            Case "z"
                                TryCast(Owner, MainWindow).RotateTransformAxis = New Vector3D(0, 0, 1)
                                TryCast(Owner, MainWindow).RotateTransformDT.Start()
                                PrintText("Animation Started !")
                            Case "speed"
                                If e.Command.Args.Length > 1 Then
                                    TryCast(Owner, MainWindow).RotateTransformSpeed = e.Command.Args(1)
                                    PrintText("Speed Changed To " & e.Command.Args(1) & " !")
                                End If
                            Case Else
                                If e.Command.Args.Length > 2 AndAlso IsNumeric(e.Command.Args(0)) And IsNumeric(e.Command.Args(1)) And IsNumeric(e.Command.Args(2)) Then
                                    Dim X As Double = CDbl(e.Command.Args(0))
                                    Dim Y As Double = CDbl(e.Command.Args(1))
                                    Dim Z As Double = CDbl(e.Command.Args(2))
                                    TryCast(Owner, MainWindow).RotateTransformAxis = New Vector3D(X, Y, Z)
                                    TryCast(Owner, MainWindow).RotateTransformDT.Start()
                                    PrintText("Custom Animation Started !")
                                Else
                                    PrintText("Unknown Animation.")
                                End If
                        End Select
                    End If

                    'Case "xjoy"
                    '    TryCast(Owner, MainWindow).Joystick.IntroVibrate()
                    '    PrintText("Connected XInput Joystick !")

                Case "get"
                    If e.Command.Args.Length > 0 Then
                        Select Case e.Command.Args(0)

                            Case "help"
                                Term.Text += vbCrLf + "Available Commands Are: " + vbCrLf + vbCrLf
                                Term.Text += "    " + "get help" + vbCrLf
                                Term.Text += "    " + "get drawmode" + vbCrLf
                                Term.Text += "    " + "get acc" + vbCrLf
                                Term.Text += "    " + "get acc3d" + vbCrLf
                                Term.Text += "    " + "get cordetail" + vbCrLf
                                Term.Text += "    " + "get coloringmode" + vbCrLf
                                Term.Text += "    " + "get lenvp" + vbCrLf
                                Term.Text += "    " + "get mat" + vbCrLf
                                Term.Text += "    " + "get debug" + vbCrLf
                                Term.Text += vbCrLf
                                Term.InsertNewPrompt()

                            Case "drawmode"
                                PrintText("DrawMode Is : " & TryCast(Owner, MainWindow).DrawMode)

                            Case "acc"
                                PrintText("Accuracy Is : " & TryCast(Owner, MainWindow).Acc)

                            Case "acc3d"
                                PrintText("Accuracy 3D Is : " & TryCast(Owner, MainWindow).Acc3D)

                            Case "cordetail"
                                PrintText("CorDetail Is : " & TryCast(Owner, MainWindow).CorDetail)

                            Case "coloringmode"
                                PrintText("Coloring Mode Is : " & TryCast(Owner, MainWindow).ColoringMethod)

                            Case "lenvp"
                                PrintText("LenVP :" & vbCrLf _
                                          & "X =" & TryCast(Owner, MainWindow).LenVPX _
                                          & "Y =" & TryCast(Owner, MainWindow).LenVPY _
                                          & "Z =" & TryCast(Owner, MainWindow).LenVPZ)

                            Case "mat"
                                Dim M As Matrix3D = TryCast(Owner, MainWindow).Transform3D.Value
                                PrintText("Transform Matrix Viewer" & vbCrLf & vbCrLf & _
                                          "| " & M.M11.ToString & " , " & M.M12.ToString & " , " & M.M13.ToString & " |" & vbCrLf & _
                                          "| " & M.M21.ToString & " , " & M.M22.ToString & " , " & M.M23.ToString & " |" & vbCrLf & _
                                          "| " & M.M31.ToString & " , " & M.M32.ToString & " , " & M.M33.ToString & " |")

                            Case "debug"
                                PrintText("Debug Level Is :" & TryCast(Owner, MainWindow).DebugLevel)

                            Case Else
                                PrintText("var '" + e.Command.Args(0) + "' not recognized")

                        End Select
                    End If

                Case "set"
                    If e.Command.Args.Length > 0 Then
                        Select Case e.Command.Args(0)

                            Case "help"
                                Term.Text += vbCrLf + "Available Commands Are: " + vbCrLf + vbCrLf
                                Term.Text += "    " + "set help" + vbCrLf
                                Term.Text += "    " + "set drawmode" + vbCrLf
                                Term.Text += "    " + "set acc" + vbCrLf
                                Term.Text += "    " + "set acc3d" + vbCrLf
                                Term.Text += "    " + "set cordetail" + vbCrLf
                                Term.Text += "    " + "set coloringmode" + vbCrLf
                                Term.Text += "    " + "set corbackground" + vbCrLf
                                Term.Text += "    " + "set lenvp" + vbCrLf
                                Term.Text += "    " + "set debug" + vbCrLf
                                Term.Text += vbCrLf
                                Term.InsertNewPrompt()

                            Case "drawmode"
                                If e.Command.Args.Length > 1 Then
                                    If e.Command.Args(1) = "help" Then
                                        PrintText(vbCrLf + "set drawmode <value>" + vbCrLf + "<value> {2D / 3D / 4D}")
                                    ElseIf e.Command.Args(1) = "2D" Or e.Command.Args(1) = "3D" Or e.Command.Args(1) = "4D" Then
                                        TryCast(Owner, MainWindow).DrawMode = e.Command.Args(1)
                                        If Not e.Command.IsRemote Then PrintText("Mode Changed To " + e.Command.Args(1))
                                    Else
                                        If Not e.Command.IsRemote Then PrintText("Draw Mode Must Be : 2D / 3D / 4D")
                                    End If
                                Else
                                    If Not e.Command.IsRemote Then PrintText("Please Enter A Value.")
                                End If

                            Case "acc"
                                If e.Command.Args.Length > 1 Then
                                    If e.Command.Args(1) = "help" Then
                                        PrintText(vbCrLf + "set acc <value>" + vbCrLf + "<value> Is An Integer Type")
                                    ElseIf IsNumeric(e.Command.Args(1)) Then
                                        Dim Acc As Integer = CInt(e.Command.Args(1))
                                        TryCast(Owner, MainWindow).Acc = Acc
                                        If Not e.Command.IsRemote Then PrintText("Successfully Seted Accuracy To " + Acc.ToString)
                                    Else
                                        If Not e.Command.IsRemote Then PrintText("Invalid Input !")
                                    End If
                                Else
                                    If Not e.Command.IsRemote Then PrintText("Please Enter A Value.")
                                End If

                            Case "acc3d"
                                If e.Command.Args.Length > 1 Then
                                    If e.Command.Args(1) = "help" Then
                                        PrintText(vbCrLf + "set acc3d <value>" + vbCrLf + "<value> Is An Integer Type")
                                    ElseIf IsNumeric(e.Command.Args(1)) Then
                                        Dim Acc3D As Integer = CInt(e.Command.Args(1))
                                        TryCast(Owner, MainWindow).Acc3D = Acc3D
                                        If Not e.Command.IsRemote Then PrintText("Successfully Seted Accuracy (3D) To " + Acc3D.ToString)
                                    Else
                                        If Not e.Command.IsRemote Then PrintText("Invalid Input !")
                                    End If
                                Else
                                    If Not e.Command.IsRemote Then PrintText("Please Enter A Value.")
                                End If

                            Case "cordetail"
                                If e.Command.Args.Length > 1 Then
                                    If e.Command.Args(1) = "help" Then
                                        PrintText(vbCrLf + "set cordetail <value>" + vbCrLf + "<value> Is An Integer Type Between 0 And 3")
                                    ElseIf IsNumeric(e.Command.Args(1)) Then
                                        Dim CD As Integer = CInt(e.Command.Args(1))
                                        If CD >= 0 And CD <= 3 Then
                                            TryCast(Owner, MainWindow).CorDetail = CD
                                            If Not e.Command.IsRemote Then PrintText("Successfully Seted CorDetail To " + CD.ToString)
                                        Else
                                            If Not e.Command.IsRemote Then PrintText("CorDetail Must Be Between 0 And 3.")
                                        End If
                                    Else
                                        If Not e.Command.IsRemote Then PrintText("Invalid Input !")
                                    End If
                                Else
                                    If Not e.Command.IsRemote Then PrintText("Please Enter A Value.")
                                End If

                            Case "coloringmode"
                                If e.Command.Args.Length > 1 Then
                                    If e.Command.Args(1) = "help" Then
                                        PrintText(vbCrLf + "set coloringmode <brush / bycor>")
                                    ElseIf e.Command.Args(1) = "brush" Then
                                        TryCast(Owner, MainWindow).ColoringMethod = MainWindow.ColorMethod.Brush
                                        If Not e.Command.IsRemote Then PrintText("Successfully Seted ColoringMethod To Brush.")
                                    ElseIf e.Command.Args(1) = "bycor" Then
                                        TryCast(Owner, MainWindow).ColoringMethod = MainWindow.ColorMethod.ByCor
                                        If Not e.Command.IsRemote Then PrintText("Successfully Seted ColoringMethod To ByCor.")
                                    Else
                                        If Not e.Command.IsRemote Then PrintText("Input Must Be Brush Or ByCor")
                                    End If
                                Else
                                    If Not e.Command.IsRemote Then PrintText("Please Enter A Value.")
                                End If

                            Case "corbackground"
                                If e.Command.Args.Length > 1 Then
                                    Try
                                        TryCast(Owner, MainWindow).CorBackground = BrushReader.ReadBrush(e.Command.Args(1))
                                        If Not e.Command.IsRemote Then PrintText("CorBackground Changed Successfully!")
                                    Catch ex As Exception
                                        If Not e.Command.IsRemote Then PrintText(ex.Message)
                                    End Try
                                End If

                            Case "lenvp"
                                If e.Command.Args.Length > 1 Then
                                    If e.Command.Args(1) = "help" Then
                                        PrintText(vbCrLf + "set lenvp <x / y / z> <value>" + vbCrLf + "<value> Is An Double Type")
                                    ElseIf e.Command.Args(1) = "x" Then
                                        If IsNumeric(e.Command.Args(2)) Then
                                            Dim LenX As Integer = CInt(e.Command.Args(2))
                                            TryCast(Owner, MainWindow).LenVPX = LenX
                                            TryCast(Owner, MainWindow).AutoRedrawCM()
                                            If Not e.Command.IsRemote Then PrintText("Successfully Seted LenVPX To " & LenX.ToString)
                                        Else
                                            If Not e.Command.IsRemote Then PrintText("Invalid Value !")
                                        End If
                                    ElseIf e.Command.Args(1) = "y" Then
                                        If IsNumeric(e.Command.Args(2)) Then
                                            Dim LenY As Integer = CInt(e.Command.Args(2))
                                            TryCast(Owner, MainWindow).LenVPY = LenY
                                            TryCast(Owner, MainWindow).AutoRedrawCM()
                                            If Not e.Command.IsRemote Then PrintText("Successfully Seted LenVPY To " & LenY.ToString)
                                        Else
                                            If Not e.Command.IsRemote Then PrintText("Invalid Value !")
                                        End If
                                    ElseIf e.Command.Args(1) = "z" Then
                                        If IsNumeric(e.Command.Args(2)) Then
                                            Dim LenZ As Integer = CInt(e.Command.Args(2))
                                            TryCast(Owner, MainWindow).LenVPZ = LenZ
                                            TryCast(Owner, MainWindow).AutoRedrawCM()
                                            If Not e.Command.IsRemote Then PrintText("Successfully Seted LenVPZ To " & LenZ.ToString)
                                        Else
                                            If Not e.Command.IsRemote Then PrintText("Invalid Value !")
                                        End If
                                    End If
                                Else
                                    If Not e.Command.IsRemote Then PrintText("Please Enter A Value.")
                                End If

                            Case "debug"
                                If e.Command.Args.Length > 1 Then
                                    If e.Command.Args(1) = "help" Then
                                        PrintText(vbCrLf + "set debug <debug level>" + vbCrLf + "Debug Level : 0~3")
                                    ElseIf IsNumeric(e.Command.Args(1)) Then
                                        Dim DL As Integer = CInt(e.Command.Args(1))
                                        TryCast(Owner, MainWindow).DebugLevel = DL
                                        If Not e.Command.IsRemote Then PrintText("Successfully Seted Debug Level To " & DL)
                                    Else
                                        If Not e.Command.IsRemote Then PrintText("Invalid Input !")
                                    End If
                                Else
                                    If Not e.Command.IsRemote Then PrintText("Please Enter A Value.")
                                End If

                            Case Else
                                If Not e.Command.IsRemote Then PrintText("var '" + e.Command.Args(0) + "' not recognized")

                        End Select
                    Else
                        PrintText("Invalid Args!")
                    End If

                Case "snapshot"
                    Dim IsClip As Boolean = False
                    If e.Command.Args.Length > 0 AndAlso e.Command.Args(0) = "clipboard" Then IsClip = True
                    TryCast(Owner, MainWindow).TakeSnapshot(IsClip)
                    Term.InsertNewPrompt()

                Case "boxfaces"
                    Term.Text += vbCrLf
                    Term.Text += "Top : " & BoxFaces.Top & vbCrLf
                    Term.Text += "Bottom : " & BoxFaces.Bottom & vbCrLf
                    Term.Text += "Left : " & BoxFaces.Left & vbCrLf
                    Term.Text += "Right : " & BoxFaces.Right & vbCrLf
                    Term.Text += "Front : " & BoxFaces.Front & vbCrLf
                    Term.Text += "Back : " & BoxFaces.Back & vbCrLf
                    Term.Text += "All : " & BoxFaces.All & vbCrLf
                    Term.Text += vbCrLf
                    Term.InsertNewPrompt()

                Case "shutdownanim"
                    TryCast(Owner, MainWindow).DevCan.Visibility = Windows.Visibility.Collapsed

                Case "info"
                    Term.Text += vbCrLf
                    Term.Text += "Update Enabled : " + My.Settings.UpdateEnabled.ToString + vbCrLf
                    Term.Text += "Last Update Check : " + My.Settings.CheckForUpdateLast.ToLongDateString + vbCrLf
                    Term.Text += "Checks For Update every " + My.Settings.CheckForUpdateDays.ToString + " day(s)." + vbCrLf
                    Term.Text += "Times Launched : " + My.Settings.TimesLaunched.ToString + vbCrLf
                    Term.Text += vbCrLf
                    Term.InsertNewPrompt()

                Case "reset"
                    If e.Command.Args.Length > 0 Then
                        Select Case e.Command.Args(0)
                            Case "upd"
                                My.Settings.CheckForUpdateLast = Date.MinValue
                                My.Settings.Save()
                                Term.Text += "Update Timer Reseted !"
                                Term.Text += vbCrLf + vbCrLf
                                Term.InsertNewPrompt()
                        End Select
                    End If

                Case "about"
                    SystemSounds.Hand.Play()
                    Term.Text += vbCrLf
                    Term.Text += "AKP Math Graph Plotter   v" & AppInfo.Version
                    Term.Text += vbCrLf
                    Term.Text += "Developed & Programed By : Armin Kazemi"
                    Term.Text += vbCrLf + vbCrLf
                    Term.InsertNewPrompt()


                Case "akp"
                    Term.Text += vbCrLf
                    Term.Text += "       d88888     d8P  8888888b.  " + vbCrLf
                    Term.Text += "      d888888    d8P   888   Y88b " + vbCrLf
                    Term.Text += "     d88P 888   d8P    888    888 " + vbCrLf
                    Term.Text += "    d88P  888 d88K     888   d88P " + vbCrLf
                    Term.Text += "   d88P   888 8888b    8888888P'  " + vbCrLf
                    Term.Text += "  d88P8888888   Y88b   888        " + vbCrLf
                    Term.Text += " d888     888    Y88b  888        " + vbCrLf
                    Term.Text += "d88P      888     Y88b 888        " + vbCrLf
                    Term.Text += vbCrLf + vbCrLf
                    Term.InsertNewPrompt()

                Case "exit"
                    Me.Hide()

                Case Else
                    PrintText("command '" + e.Command.Name + "' not recognized")
            End Select
        End If
    End Sub

    Public Sub PrintText(Str As String, Optional TextStyle As TextStyle = 0)
        Term.Text += Str + vbCrLf + vbCrLf
        Term.InsertNewPrompt()
    End Sub

    Public Sub Clear()
        Term.Text = "AKP Terminal v0.9" + vbCrLf
        Term.Text += "-----------------" + vbCrLf + vbCrLf
        RealTextStartIndex = Term.Text.Length
        Term.InsertNewPrompt()
        Term.ScrollToEnd()
    End Sub

    Public Sub ActivateLogger()
        Term.Text = "AKP Terminal v0.9 - Logger Mode (Press F3 To Cancel)" + vbCrLf
        Term.Text += "-----------------" + vbCrLf + vbCrLf
        Term.Prompt = ":: "
        Term.InsertNewPrompt()
        RealTextStartIndex = Term.Text.Length
        Term.ScrollToEnd()
        IsLogger = True
    End Sub

    Public Sub DeactivateLogger()
        IsLogger = False
        Term.Prompt = "> "
        Clear()
    End Sub

    Public Sub LogWriteLine(Log As String)
        If IsLogger Then
            Term.Text += Term.Prompt + Log + vbCrLf
        End If
    End Sub

    Public Sub SaveLog()
        Dim SFD As New Microsoft.Win32.SaveFileDialog
        SFD.DefaultExt = ".txt"
        SFD.Filter = "AKP Log File|*.txt"

        Dim Res As Nullable(Of Boolean) = SFD.ShowDialog

        If Res = True Then
            Dim FILE_NAME As String = SFD.FileName
            Dim ObjWriter As New System.IO.StreamWriter(FILE_NAME)

            ObjWriter.WriteLine("AKP Math Graph Plotter Log File")
            ObjWriter.WriteLine("_______________________________")
            ObjWriter.WriteLine("Version : " + My.Application.Info.Version.ToString)
            ObjWriter.WriteLine("Terminal Version : " + "0.9 Beta")
            ObjWriter.WriteLine("")
            ObjWriter.Write(Term.Text.Substring(RealTextStartIndex))
            ObjWriter.Close()
            MsgBox("Log Saved To Text File !")

        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub Window_KeyDown(sender As Object, e As KeyEventArgs)
        If IsLogger And e.Key = Key.F3 Then
            DeactivateLogger()
        ElseIf (Not IsLogger) And e.Key = Key.F3 Then
            ActivateLogger()
        End If
    End Sub
End Class
