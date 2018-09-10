Imports System.Collections.ObjectModel

Public Class MGPExporter

    Private m_objlist As ObservableCollection(Of MathObject)
    Private m_savesetting As Boolean

    Public Sub New(MathObj As ObservableCollection(Of MathObject), Optional SaveSetting As Boolean = True)
        m_objlist = MathObj
        m_text = ""
        m_savesetting = SaveSetting
    End Sub

    Private m_text As String

    Private CEq As Boolean = False
    Private CVar As Boolean = False
    Private CPoint As Boolean = False
    Private CVect As Boolean = False
    Private CShape As Boolean = False

    Public Sub WriteToFile(ByVal Location As String, Optional ByVal Overwrite As Boolean = True)
        If IO.File.Exists(Location) Then
            If Overwrite = True Then
                IO.File.Delete(Location)
            End If
        End If

        WriteVersion()
        For Each MO As MathObject In m_objlist
            Select Case MO.Type
                Case 0
                    CEq = True
                Case 1
                    CVar = True
                Case 2
                    CPoint = True
                Case 3
                    CVect = True
                Case 4
                    CShape = True
            End Select
        Next

        If m_savesetting Then
            m_text += "<set>" + vbCrLf
            WriteSetting()
            m_text += "</set>" + vbCrLf
        End If

        If CEq Then
            m_text += "<eq>" + vbCrLf
            For Each MO As MathObject In m_objlist
                If MO.Type = 0 Then WriteEquation(MO.Inner)
            Next
            m_text += "</eq>" + vbCrLf
        End If

        If CVar Then
            m_text += "<var>" + vbCrLf
            For Each MO As MathObject In m_objlist
                If MO.Type = 1 Then WriteVariable(MO.Inner)
            Next
            m_text += "</var>" + vbCrLf
        End If

        If CPoint Then
            m_text += "<point>" + vbCrLf
            For Each MO As MathObject In m_objlist
                If MO.Type = 2 Then WritePoint(MO.Inner)
            Next
            m_text += "</point>" + vbCrLf
        End If

        If CVect Then
            m_text += "<vect>" + vbCrLf
            For Each MO As MathObject In m_objlist
                If MO.Type = 3 Then WriteVector(MO.Inner)
            Next
            m_text += "</vect>" + vbCrLf
        End If

        If CShape Then
            m_text += "<shape>" + vbCrLf
            For Each MO As MathObject In m_objlist
                If MO.Type = 4 Then WriteShape(MO.Inner)
            Next
            m_text += "</shape>" + vbCrLf
        End If

        IO.File.WriteAllText(Location, m_text)
    End Sub

    'ذخیره اطلاعات ورژن
    Private Sub WriteVersion()
        m_text += "<AKPMGP>" + vbCrLf
        m_text += "<rv>" & AppInfo.Version & "</rv>" & vbCrLf
    End Sub

    'ذخیره تابع
    Public Sub WriteEquation(Eq As Equation)
        'Brush
        Dim BrushText As String
        If Eq.Brush.GetType() = GetType(SolidColorBrush) Then
            BrushText = "sc:" & DirectCast(Eq.Brush, SolidColorBrush).Color.ToString
        End If
        If Eq.Brush.GetType() = GetType(ImageBrush) Then
            BrushText = "uri:" & TryCast(DirectCast(Eq.Brush, ImageBrush).ImageSource, BitmapImage).UriSource.OriginalString
        End If

        'Domain
        Dim DSText As String
        Dim DEText As String
        Dim MSC As New MathSymbolConverter
        DSText = MSC.ConvertForExport(Eq.DomainStart)
        DEText = MSC.ConvertForExport(Eq.DomainEnd)
        Dim DC2S As New DoubleCollectionToString
        Dim DExcept As String = DC2S.Convert(Eq.DomainExcept, GetType(String), Nothing, Nothing)

        m_text += Eq.Expression & ";" & Eq.Type & ";[" & DSText & "," & DEText & "]" & ";{" & DExcept & "};" & BrushText & ";" & Eq.Thickness & ";" & Eq.Opacity & ";" & Eq.IsVisible.To01 & vbCrLf
    End Sub

    'ذخیره متغییر
    Public Sub WriteVariable(Var As Variable)
        'Min Max
        Dim MinText As String
        Dim MaxText As String
        Dim MSC As New MathSymbolConverter
        MinText = MSC.ConvertForExport(Var.Min)
        MaxText = MSC.ConvertForExport(Var.Max)

        'Tick Mode
        Dim TM As Integer = Var.TickMode
        m_text += Var.Name & "=" & Var.Value & ";[" & MinText & "," & MaxText & "];" & Var.By & ";" & Var.Duration & ";" & TM.ToString & ";" & Var.Autoplay.To01 & vbCrLf
    End Sub

    'ذخیره نقطه
    Public Sub WritePoint(Pt As APointViewer)
        m_text += Pt.Name & "(" & Pt.Point.X & "," & Pt.Point.Y & "," & Pt.Point.Z & ");" & "sc:" & Pt.Color.ToString & ";" & Pt.IsVisible.To01 & ";" & Pt.IsLight.To01 & vbCrLf
    End Sub

    'ذخیره بردار
    Public Sub WriteVector(Vect As AVectorViewer)
        m_text += Vect.Name & "(" & Vect.Vector.X & "," & Vect.Vector.Y & "," & Vect.Vector.Z & ");" & Vect.PointName & ";" & "sc:" & Vect.Color.ToString & ";" & Vect.IsVisible.To01 & vbCrLf
    End Sub

    'ذخیره شکل
    Public Sub WriteShape(Sh As AShape)
        'Brush
        Dim BrushText As String = ""
        If Sh.Brush.GetType() = GetType(SolidColorBrush) Then
            BrushText = "sc:" & DirectCast(Sh.Brush, SolidColorBrush).Color.ToString
        End If
        If Sh.Brush.GetType() = GetType(ImageBrush) Then
            BrushText = "uri:" & TryCast(DirectCast(Sh.Brush, ImageBrush).ImageSource, BitmapImage).UriSource.OriginalString
        End If
        Dim ShType As Integer = Sh.Type

        m_text += Sh.Name & "(" & ShType & ");(" & Sh.PointName & ");(" & Sh.VectorName & ");" & Sh.Length & ";" & Sh.Width & ";" & Sh.Height & ";" _
            & Sh.Radius & ";" & Sh.Radius2 & ";" & Sh.Radius3 & ";" & Sh.FaceCount & ";" & Sh.CubeFace & ";" & Sh.BaseCap.To01 & ";" & Sh.TopCap.To01 & ";" _
            & Sh.ThetaDiv & ";" & Sh.PhiDiv & ";" & BrushText & ";" & Sh.IsVisible.To01 & vbCrLf
    End Sub

    'ذخیره تنظیمات
    Public Sub WriteSetting()
        Dim DM As String = TryCast(My.Application.MainWindow, MainWindow).DrawMode
        m_text += "drawmode=" & DM & vbCrLf
        If DM = "2D" Then
            m_text += "acc=" & TryCast(My.Application.MainWindow, MainWindow).Acc & vbCrLf
        ElseIf DM = "3D" Then
            m_text += "acc3d=" & TryCast(My.Application.MainWindow, MainWindow).Acc3D & vbCrLf
        End If
        If TryCast(My.Application.MainWindow, MainWindow).ColoringMethod = MainWindow.ColorMethod.ByCor Then m_text += "coloringmode=bycor" & vbCrLf
        If TryCast(My.Application.MainWindow, MainWindow).CorDetail <> 3 Then m_text += "cordetail=" & TryCast(My.Application.MainWindow, MainWindow).CorDetail.ToString & vbCrLf
    End Sub

End Class
