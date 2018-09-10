Public Class MGPImporter

    Private m_text As String
    Public Sub New(FileText As String)
        m_text = FileText
    End Sub

    Public Sub BeginImport()
        If ValidateInput() Then
            CType(My.Application.MainWindow, MainWindow).DeleteAll()
            ImportEquation()
            ImportPoint()
            ImportVector()
            ImportShape()
            ImportVariable()
            CType(My.Application.MainWindow, MainWindow).RestoreDefaults()
            ImportSetting()
            CType(My.Application.MainWindow, MainWindow).AutoRedraw()
            CType(My.Application.MainWindow, MainWindow).AutoplayVariables()
        End If
    End Sub

    'تایید نوع فایل و ورژن
    Public Function ValidateInput() As Boolean
        Dim Result As Boolean = False
        If m_text <> "" Then
            If m_text.StartsWith("<AKPMGP>") Then
                Dim RV As Version = New Version(TagReader.ReadTag(m_text, "rv"))
                If RV < New Version("10.4") Then
                    Dim D As New Dialog("این فایل مخصوص نسخه های قدیمی تر برنامه است.", "Error")
                    D.ShowDialog()
                    Return Result
                End If
                If My.Application.Info.Version >= RV Then
                    Result = True
                Else
                    Dim D As New Dialog("این فایل با ورژن فعلی سازگاری ندارد" & vbCrLf & "ورژن مورد نیاز : " & RV.ToString, "Error")
                    D.ShowDialog()
                End If
            End If
        End If
        Return Result
    End Function

    'ورود توابع از فایل
    Public Sub ImportEquation()
        Try
            Dim EqInput As String = TagReader.ReadTag(m_text, "eq")
            If EqInput <> "" Then
                For i = 0 To EqInput.Split(vbCrLf).Count - 2
                    Dim Line As String = EqInput.Split(vbCrLf)(i)
                    If Line <> "" And Line <> vbCrLf Then
                        Dim EqInfo() As String = Line.Substring(1).Split(";")
                        Dim Exp As String = EqInfo(0) '0
                        Dim Type As Integer = CInt(EqInfo(1)) '1
                        Dim EqDomain As String = EqInfo(2) '2
                        Dim EqDs() As String = EqDomain.Split("[")
                        Dim EqD1 As String = EqDs(1).Trim("[", "]")
                        Dim EqD2 As String = "-inf,+inf"
                        If EqDs.Count > 2 Then
                            EqD2 = EqDs(2).Trim("[", "]")
                        End If
                        Dim EqD1s() As String = EqD1.Split(",")
                        Dim EqD2s() As String = EqD2.Split(",")
                        Dim MSC As New MathSymbolConverter
                        Dim DS As Double = MSC.ConvertBack(EqD1s(0), GetType(Double), Nothing, Nothing) '2.1
                        Dim DE As Double = MSC.ConvertBack(EqD1s(1), GetType(Double), Nothing, Nothing) '2.2
                        Dim DS2 As Double = MSC.ConvertBack(EqD2s(0), GetType(Double), Nothing, Nothing) '2.3
                        Dim DE2 As Double = MSC.ConvertBack(EqD2s(1), GetType(Double), Nothing, Nothing) '2.4
                        Dim EqExcept As String = EqInfo(3) '3
                        EqExcept = EqExcept.Trim("{", "}")
                        Dim DC2S As New DoubleCollectionToString
                        Dim DExcept As DoubleCollection = DC2S.ConvertBack(EqExcept, GetType(DoubleCollection), Nothing, Nothing)
                        Dim EqBrush As Brush = BrushReader.ReadBrush(EqInfo(4)) '4
                        Dim ThickN As Integer = CDbl(EqInfo(5)) '5
                        Dim Opacity As Integer = CDbl(EqInfo(6)) '6
                        Dim Visibility As Boolean = EqInfo(7).ToBool
                        CType(My.Application.MainWindow, MainWindow).AddFunction(Exp, Type, DS, DE, DS2, DE2, DExcept, EqBrush, ThickN, Opacity, Visibility)
                    End If
                Next
            End If
        Catch Ex As Exception
            MsgBox("Error Reading File Equations." & vbCrLf & Ex.Message)
        End Try
    End Sub

    'ورود متغییر ها از فایل
    Public Sub ImportVariable()
        Try
            Dim VarInput As String = TagReader.ReadTag(m_text, "var")
            If VarInput <> "" Then
                For i = 0 To VarInput.Split(vbCrLf).Count - 2
                    Dim Line As String = VarInput.Split(vbCrLf)(i)
                    If Line <> "" And Line <> vbCrLf Then
                        Dim VarInfo() As String = Line.Substring(1).Split(";")
                        Dim VarMain As String = VarInfo(0) '0
                        Dim VM() As String = VarMain.Split("=")
                        Dim Chr As String = VM(0) '0.0
                        Dim Value As Integer = CDbl(VM(1)) '0.1
                        Dim MinMax As String = VarInfo(1) '1
                        MinMax = MinMax.Trim("[", "]")
                        Dim MM() As String = MinMax.Split(",")
                        Dim MSC As New MathSymbolConverter
                        Dim Min As Double = MSC.ConvertBack(MM(0), GetType(Double), Nothing, Nothing) '1.0
                        Dim Max As Double = MSC.ConvertBack(MM(1), GetType(Double), Nothing, Nothing) '1.1
                        Dim By As Double = CDbl(VarInfo(2)) '2
                        Dim Duration As Double = CDbl(VarInfo(3)) '3
                        Dim RMN As Integer = CInt(VarInfo(4)) '4
                        Dim AP As Boolean = VarInfo(5).ToBool
                        CType(My.Application.MainWindow, MainWindow).AddVariable(Chr, Value, Min, Max, By, Duration, RMN, AP)
                    End If
                Next
            End If
        Catch Ex As Exception
            MsgBox("Error Reading File Variables." & vbCrLf & Ex.Message)
        End Try
    End Sub

    'ورود نقطه ها از فایل
    Public Sub ImportPoint()
        Try
            Dim PointInput As String = TagReader.ReadTag(m_text, "point")
            If PointInput <> "" Then
                For i = 0 To PointInput.Split(vbCrLf).Count - 2
                    Dim Line As String = PointInput.Split(vbCrLf)(i)
                    If Line <> "" And Line <> vbCrLf Then
                        Dim ViewerInfo() As String = Line.Substring(1).Split(";")
                        Dim PointName As String = ViewerInfo(0).Split("(")(0) '0.0
                        Dim PointInfo() As String = TagReader.ReadParenthesis(Line.Substring(1)).Split(",") '0.1
                        Dim PointX As String = PointInfo(0) '0.1.0
                        Dim PointY As String = PointInfo(1) '0.1.1
                        Dim PointZ As String = PointInfo(2) '0.1.2
                        Dim PointColor As Color = BrushReader.ReadColor(ViewerInfo(1)) '1
                        Dim Visiblity As Boolean = ViewerInfo(2).ToBool '2
                        Dim IsLight As Boolean = ViewerInfo(3).ToBool '3
                        CType(My.Application.MainWindow, MainWindow).AddPoint(PointName, PointX, PointY, PointZ, PointColor, Visiblity, IsLight)
                    End If
                Next
            End If
        Catch Ex As Exception
            MsgBox("Error Reading File Points." & vbCrLf & Ex.Message)
        End Try
    End Sub

    'ورود بردار ها از فایل
    Public Sub ImportVector()
        'Try
        Dim VectInput As String = TagReader.ReadTag(m_text, "vect")
        If VectInput <> "" Then
            For i = 0 To VectInput.Split(vbCrLf).Count - 2
                Dim Line As String = VectInput.Split(vbCrLf)(i)
                If Line <> "" And Line <> vbCrLf Then
                    Dim ViewerInfo() As String = Line.Substring(1).Split(";")
                    Dim VectorName As String = ViewerInfo(0).Split("(")(0) '0.0
                    Dim VectorInfo() As String = TagReader.ReadParenthesis(ViewerInfo(0)).Split(",") '0.1
                    Dim VectorI As String = VectorInfo(0) '0.1.0
                    Dim VectorJ As String = VectorInfo(1) '0.1.1
                    Dim VectorK As String = VectorInfo(2) '0.1.2
                    Dim VectorOPN As String = ViewerInfo(1) '1
                    Dim VectorColor As Color = BrushReader.ReadColor(ViewerInfo(2)) '2
                    Dim Visiblity As Boolean = ViewerInfo(3).ToBool '3
                    CType(My.Application.MainWindow, MainWindow).AddVector(VectorName, VectorI, VectorJ, VectorK, VectorOPN, VectorColor, Visiblity)
                End If
            Next
        End If
        'Catch Ex As Exception
        '    MsgBox("Error Reading File Vectors." & vbCrLf & Ex.Message)
        'End Try
    End Sub

    'ورود شکل ها از فایل
    Public Sub ImportShape()
        Try
            Dim ShapeInput As String = TagReader.ReadTag(m_text, "shape")
            If ShapeInput <> "" Then
                For i = 0 To ShapeInput.Split(vbCrLf).Count - 2
                    Dim Line As String = ShapeInput.Split(vbCrLf)(i)
                    If Line <> "" And Line <> vbCrLf Then
                        Dim ShapeInfo() As String = Line.Substring(1).Split(";")
                        Dim ShapeName As String = ShapeInfo(0).Split("(")(0) '0.0
                        Dim ShapeTypeInt As Integer = CInt(TagReader.ReadParenthesis(ShapeInfo(0))) '0.1
                        Dim ShapeOrigin As String = TagReader.ReadParenthesis(ShapeInfo(1)) '1
                        Dim ShapeDir As String = TagReader.ReadParenthesis(ShapeInfo(2)) '2
                        Dim ShapeL As String = ShapeInfo(3) '3
                        Dim ShapeW As String = ShapeInfo(4) '4
                        Dim ShapeH As String = ShapeInfo(5) '5
                        Dim ShapeRX As String = ShapeInfo(6) '6
                        Dim ShapeRY As String = ShapeInfo(7) '7
                        Dim ShapeRZ As String = ShapeInfo(8) '8
                        Dim ShapeFaceCount As Integer = CInt(ShapeInfo(9)) '9
                        Dim ShapeCubeFace As Integer = CInt(ShapeInfo(10)) '10
                        Dim ShapeBaseCap As Boolean = ShapeInfo(11).ToBool '11
                        Dim ShapeTopCap As Boolean = ShapeInfo(12).ToBool '12
                        Dim ShapeThetaDiv As Integer = CInt(ShapeInfo(13)) '13
                        Dim ShapePhiDiv As Integer = CInt(ShapeInfo(14)) '14 
                        Dim ShapeBrush As Brush = BrushReader.ReadBrush(ShapeInfo(15)) '15
                        Dim Visiblity As Boolean = ShapeInfo(16).ToBool '16
                        CType(My.Application.MainWindow, MainWindow).AddShape(ShapeName, ShapeTypeInt, ShapeOrigin, ShapeDir, ShapeL, ShapeW, ShapeH, ShapeRX, ShapeRY, ShapeRZ, ShapeFaceCount, ShapeCubeFace,
                                                                              ShapeBaseCap, ShapeTopCap, ShapeThetaDiv, ShapePhiDiv, ShapeBrush, Visiblity)
                    End If
                Next
            End If
        Catch Ex As Exception
            MsgBox("Error Reading File Shapes." & vbCrLf & Ex.Message)
        End Try
    End Sub

    'ورود تنظیمات از فایل
    Public Sub ImportSetting()
        Dim SetInput As String = TagReader.ReadTag(m_text, "set")
        If SetInput <> "" Then
            Dim SetSS() As String = SetInput.Split(vbCrLf)
            For i = 0 To SetInput.Split(vbCrLf).Count - 2
                Dim Line As String = SetSS(i)
                If Line <> "" Then
                    Dim CmdStr As String = "set " + Line.Substring(1).Replace("=", " ")
                    CType(My.Application.MainWindow, MainWindow).AKPTerminal.Term.RemoteCommand(CmdStr)
                End If
            Next
        End If
    End Sub

End Class
