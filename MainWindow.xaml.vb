'کتابخانه ریاضی
Imports System.Math

' کتابخانه گرافیک
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Shapes
Imports System.Windows.Media.Animation

'کتابخانه متن
Imports System.Text
Imports System.Text.RegularExpressions

'کتابخانه فایل
Imports System.IO
Imports Microsoft.Win32

Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Windows.Threading
Imports System.Xml.Serialization
Imports AKP_Math_Graph_Plotter.AKP.Input.Joystick
Imports System.Media

Class MainWindow
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged
    Private Sub NotifyPropertyChanged(Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    'بارگذاری پنجره
    Private Sub Window_Load(sender As Object, e As RoutedEventArgs)
        UseLayoutRounding = True
        CenterPoint = New Point(Cor2D_Behind.ActualWidth / 2, Cor2D_Behind.ActualHeight / 2)

        DrawCoordinateSystem2D()
        DrawCoordinateSystem3D()

        MathObjList.Add(New MathObject(99))
        AddFunction()

        DeveloperMarquee()
        'If IsTouchScreen() Then
        '    MarqueeMsg("صفحه لمسی شناسایی شد . هماهنگی کامل با صفحه لمسی و تبلت های ویندوزی بزودی ...", 4)
        'End If

        AKPTerminal.Owner = Window.GetWindow(Me)
        AboutWindow.Owner = Window.GetWindow(Me)
        ExamplesWindow.Owner = Window.GetWindow(Me)

        'Joystick = XboxController.RetrieveController(0)
        Joystick.StartPolling()
    End Sub
    'بستن پنجره
    Private Sub Window_Closed(sender As Object, e As EventArgs)
        Joystick.StopPolling()
        Application.Current.Shutdown()
    End Sub

    'پنجره ها جانبی
    Public AKPTerminal As New TerminalWindow
    Private Sub OpenTerminal(sender As Object, e As RoutedEventArgs)
        AKPTerminal.Show()
    End Sub

    Dim ExamplesWindow As New Examples
    Private Sub OpenExamples(sender As Object, e As RoutedEventArgs)
        ExamplesWindow.Show()
    End Sub

    Dim AboutWindow As New About
    Private Sub OpenAbout(sender As Object, e As RoutedEventArgs)
        AboutWindow.Show()
    End Sub

    Private Sub OpenHelp(sender As Object, e As RoutedEventArgs)
        Dim D As New Dialog("برای راهنما به فایل PDF موجود در سی دی نرم افزار مراجعه کنید .", "Help")
        D.ShowDialog()
    End Sub

    'تنظیمات رسم
    Private Sub ShowDrawSetting(sender As Object, e As RoutedEventArgs)
        DrawSettingPopup.IsOpen = Not DrawSettingPopup.IsOpen
    End Sub

    Shared Property UnlockBetaFeatures As Boolean = False


    'اطلاعات برنامه نویس
    Private Sub DeveloperMarquee()
        Dev0.Inlines.Add(New Run("نرم افزار مدلساز و رسم نمودار ریاضی    ") With {.Foreground = Brushes.Blue})
        Dev0.Inlines.Add(New Run("برنامه نویس : ") With {.Foreground = Brushes.Red})
        Dev0.Inlines.Add("آرمین کاظمی      ")
        Dev0.Inlines.Add(New Run("دبیرستان شهید مدنی 1 تبریز      ") With {.Foreground = Brushes.DarkGreen})
        Dev0.Inlines.Add(New Run("ناحیه 4      ") With {.Foreground = Brushes.Olive})
        Dev0.UpdateLayout()
        Dim DevAnim As New DoubleAnimation
        DevAnim.From = DevCan.ActualWidth
        DevAnim.To = -1 * Dev0.ActualWidth
        DevAnim.Duration = TimeSpan.FromSeconds(ScrollTime)
        DevAnim.RepeatBehavior = RepeatBehavior.Forever
        Dev0.BeginAnimation(Canvas.LeftProperty, DevAnim)
    End Sub

    'پیام به کاربر
    Private ScrollTime As Integer = 15
    Private Sub MarqueeMsg(Msg As String, Optional ReadTime As Integer = 2)
        Dev0.Inlines.Add(New Run("       " + Msg) With {.Foreground = Brushes.LimeGreen})
        Dev0.UpdateLayout()
        Dim DevAnim As New DoubleAnimation
        DevAnim.From = DevCan.ActualWidth
        DevAnim.To = -1 * Dev0.ActualWidth
        ScrollTime += ReadTime
        DevAnim.Duration = TimeSpan.FromSeconds(ScrollTime)
        DevAnim.RepeatBehavior = RepeatBehavior.Forever
        Dev0.BeginAnimation(Canvas.LeftProperty, DevAnim)
    End Sub

    'بررسی صفحه لمسی بودن دستگاه
    Private Function IsTouchScreen() As Boolean
        For Each TabletDevice As TabletDevice In Tablet.TabletDevices
            If TabletDevice.Type = TabletDeviceType.Touch Then
                Return True
            End If
        Next
        Return False
    End Function


#Region "ابزار های ریاضی"

    'محاسبه گر ریاضی
    Dim AKPEval As New MathEvaluator

    'تولید کننده عدد تصادفی
    Dim Rnd As New Random

#End Region

#Region "لیست اشیا"

    Public Property MathObjList As New ObservableCollection(Of MathObject)

    'اضافه کردن تابع
    Public Sub AddFunction(Optional Exp As String = "", Optional Ty As Integer = 1, _
                           Optional DS As Double = Double.NegativeInfinity, Optional DE As Double = Double.PositiveInfinity, _
                           Optional DS2 As Double = Double.NegativeInfinity, Optional DE2 As Double = Double.PositiveInfinity, _
                           Optional DExcept As DoubleCollection = Nothing, _
                           Optional EqBrush As Brush = Nothing, Optional ThickN As Double = 2, Optional Opacity As Double = 1, _
                           Optional IsVisible As Boolean = True)
        If DExcept Is Nothing Then DExcept = New DoubleCollection
        If EqBrush Is Nothing Then EqBrush = New SolidColorBrush(PickColor())
        MathObjList.Insert(Eq_ListBox.Items.Count - 1, New MathObject(0) With {.Inner = New Equation With {.Expression = Exp, .Type = Ty,
                                      .DomainStart = DS, .DomainEnd = DE, .SecondDomainStart = DS2, .SecondDomainEnd = DE2, .DomainExcept = DExcept,
                                      .Brush = EqBrush, .Thickness = ThickN, .Opacity = Opacity, .IsVisible = IsVisible,
                                      .HasError = False}})
        Eq_ListBox.Items.Refresh()
        Eq_ListBox.SelectedIndex = Eq_ListBox.Items.Count - 2
    End Sub
    Public Sub AddFunction(F As Equation)
        MathObjList.Insert(Eq_ListBox.Items.Count - 1, New MathObject(0) With {.Inner = F})
        Eq_ListBox.Items.Refresh()
    End Sub
    Public Sub AddEmptyFunction(sender As Object, e As RoutedEventArgs)
        AddFunction()
        AutoRedraw()
    End Sub

    'اضافه کردن متغییر
    Public Sub AddVariable(Optional Name As String = Nothing, Optional Value As Double = 0, _
                           Optional Min As Double = -10, Optional Max As Double = 10, _
                           Optional By As Double = 1, Optional Duration As Double = 0.5, _
                           Optional TickMode As Variable.TickModes = Variable.TickModes.AutoReverse, Optional Autoplay As Boolean = False)

        If Name = Nothing Then Name = PickChar()
        MathObjList.Insert(Eq_ListBox.Items.Count - 1, New MathObject(1) With {.Inner = New Variable With {.Name = Name, .Value = Value, .By = By _
                                                                           , .Duration = Duration, .Min = Min, .Max = Max, .TickMode = TickMode, .Autoplay = Autoplay}})
        Eq_ListBox.Items.Refresh()
        Eq_ListBox.SelectedIndex = Eq_ListBox.Items.Count - 2
    End Sub
    Public Sub AddVariable(V As Variable)
        MathObjList.Insert(Eq_ListBox.Items.Count - 1, New MathObject(1) With {.Inner = V})
        Eq_ListBox.Items.Refresh()
    End Sub
    Public Sub AddEmptyVariable(sender As Object, e As RoutedEventArgs)
        AddVariable()
        AutoRedraw()
    End Sub

    'اضافه کردن نقطه
    Public Sub AddPoint(Optional Name As String = Nothing, _
                        Optional X As String = "0", Optional Y As String = "0", Optional Z As String = "0", _
                        Optional PColor As Color = Nothing, Optional IsVisible As Boolean = True, Optional IsLight As Boolean = False)
        If PColor = Nothing Then PColor = PickColor()
        If Name = Nothing Then Name = PickChar("P")
        MathObjList.Insert(Eq_ListBox.Items.Count - 1, New MathObject(2) With {.Inner = New APointViewer With {.Name = Name, .Point = New APoint(X, Y, Z), .Color = PColor, .IsVisible = IsVisible, .IsLight = IsLight}})
        Eq_ListBox.Items.Refresh()
        Eq_ListBox.SelectedIndex = Eq_ListBox.Items.Count - 2
    End Sub
    Public Sub AddPoint(P As APointViewer)
        MathObjList.Insert(Eq_ListBox.Items.Count - 1, New MathObject(2) With {.Inner = P})
        Eq_ListBox.Items.Refresh()
    End Sub
    Public Sub AddEmptyPoint(sender As Object, e As RoutedEventArgs)
        AddPoint()
        AutoRedraw()
    End Sub

    'اضافه کردن بردار
    Public Sub AddVector(Optional Name As String = Nothing, _
                         Optional I As String = "0", Optional J As String = "0", Optional K As String = "0", _
                         Optional PointName As String = "", _
                         Optional PColor As Color = Nothing, Optional IsVisible As Boolean = True)
        If PColor = Nothing Then PColor = PickColor()
        If Name = Nothing Then Name = PickChar("V")
        MathObjList.Insert(Eq_ListBox.Items.Count - 1, New MathObject(3) With {.Inner = New AVectorViewer With {.Name = Name, .Vector = New AVector(I, J, K), .PointName = PointName, .Color = PColor, .IsVisible = IsVisible}})
        Eq_ListBox.Items.Refresh()
        Eq_ListBox.SelectedIndex = Eq_ListBox.Items.Count - 2
    End Sub
    Public Sub AddVector(V As AVectorViewer)
        MathObjList.Insert(Eq_ListBox.Items.Count - 1, New MathObject(3) With {.Inner = V})
        Eq_ListBox.Items.Refresh()
    End Sub
    Public Sub AddEmptyVector(sender As Object, e As RoutedEventArgs)
        AddVector()
        AutoRedraw()
    End Sub

    'اضافه کردن شکل
    Public Sub AddShape(Optional Name As String = "", Optional ShapeType As ShapeType = ShapeType.None, _
                        Optional PointName As String = "", Optional VectorName As String = "", _
                        Optional Length As String = "", Optional Width As String = "", Optional Height As String = "", _
                        Optional Radius As String = "", Optional Radius2 As String = "", Optional Radius3 As String = "",
                        Optional FaceCount As Integer = 3, Optional CubeFace As Integer = BoxFaces.All, _
                        Optional BaseCap As Boolean = True, Optional TopCap As Boolean = True, _
                        Optional ThetaDiv As Integer = 40, Optional PhiDiv As Integer = 20, _
                        Optional ShBrush As Brush = Nothing, Optional Visibility As Boolean = True)
        If ShBrush Is Nothing Then ShBrush = New SolidColorBrush(PickColor())
        MathObjList.Insert(Eq_ListBox.Items.Count - 1, New MathObject(4) With {.Inner = New AShape With {.Name = Name, .Type = ShapeType, .PointName = PointName, .VectorName = VectorName, .Length = Length, .Width = Width, .Height = Height, _
        .Radius = Radius, .Radius2 = Radius2, .Radius3 = Radius3, .FaceCount = FaceCount, .CubeFace = CubeFace, .BaseCap = BaseCap, .TopCap = TopCap, .ThetaDiv = ThetaDiv, .PhiDiv = PhiDiv, .Brush = ShBrush, .IsVisible = Visibility}})
        Eq_ListBox.Items.Refresh()
        Eq_ListBox.SelectedIndex = Eq_ListBox.Items.Count - 2
    End Sub
    Public Sub AddShape(S As AShape)
        MathObjList.Insert(Eq_ListBox.Items.Count - 1, New MathObject(4) With {.Inner = S})
        Eq_ListBox.Items.Refresh()
    End Sub
    Public Sub AddEmptyCone()
        AddShape(, ShapeType.Cone)
    End Sub
    Public Sub AddEmptyTCone()
        AddShape(, ShapeType.TCone)
    End Sub
    Public Sub AddEmptyCylinder()
        AddShape(, ShapeType.Cylinder)
    End Sub
    Public Sub AddEmptyPyramid()
        AddShape(, ShapeType.Pyramid)
    End Sub
    Public Sub AddEmptyCube()
        AddShape(, ShapeType.Cube)
    End Sub
    Public Sub AddEmptyBox()
        AddShape(, ShapeType.Box)
    End Sub
    Public Sub AddEmptySphere()
        AddShape(, ShapeType.Sphere)
    End Sub
    Public Sub AddEmptyEllipsoid()
        AddShape(, ShapeType.Ellipsoid)
    End Sub

    'حذف
    Public Sub Delete(MO As MathObject)
        If MO.Type <> 1 And MO.Type <> 99 Then
            For Each GO In MO.Inner.GraphObjectt
                If Cor2D_Front.Children.Contains(TryCast(GO, UIElement)) Then
                    Cor2D_Front.Children.Remove(GO)
                End If
                If VisualModel.Children.Contains(TryCast(GO, Model3D)) Then
                    VisualModel.Children.Remove(GO)
                End If
            Next
        End If
    End Sub

    'حذف همه
    Public Sub DeleteAll()
        Cor2D_Front.Children.Clear()
        VisualModel.Children.Clear()
        MathObjList.Clear()
        MathObjList.Add(New MathObject(99))
    End Sub

#End Region

#Region "ابزار های تابع"

    'ساده سازی ضابطه
    Public Function SimplifyFunction(ByRef Eq As Equation, ByRef Success As Boolean) As String
        ' Input : y=f(x) 
        ' Undirect Tasks : Detect Function Type , Check For Error
        ' Direct Output : f(x)

        Dim EqExp As String = Eq.Expression

        Dim IsCompletlyTyped As Boolean = EqExp.Contains("=")
        Dim IsParametric As Boolean = EqExp.Contains(",") 'معادله پارامتریک
        Dim IsImplicit As Boolean = False 'تابع ضمنی

        Dim Output As String = ""

        Success = True

        'ورودی کارتزین
        Dim InX As Boolean = False
        Dim InY As Boolean = False
        Dim InZ As Boolean = False

        'ورودی قطبی
        Dim InT As Boolean = False
        Dim InR As Boolean = False
        Dim InP As Boolean = False

        'خروجی کارتزین
        Dim OutX As Boolean = False
        Dim OutY As Boolean = False
        Dim OutZ As Boolean = False

        'خروجی قطبی
        Dim OutT As Boolean = False
        Dim OutR As Boolean = False
        Dim OutP As Boolean = False

        If Not (EqExp Is Nothing Or EqExp = "") Then

            If IsParametric Then 'معادله پارامتریک است
                Dim Pram() As String = EqExp.Split(",")
                If Pram.Length = 2 Then
                    'Eq.Type = 7
                    Return EqExp
                ElseIf Pram.Length = 3 Then

                End If
            End If

            If IsCompletlyTyped Then ' ضابطه تابع بصورت کامل تایپ شده
                Dim Func() As String = EqExp.Split("=")
                Dim FuncFrist As String = Func(0).Trim(" ")
                Dim FuncSecond As String = Func(1).Trim(" ")

                If FuncFrist.Length = 1 And (Not IsNumeric(FuncFrist)) Then ' تابع واقعی

                    Select Case FuncFrist.ToLower
                        Case "x"
                            OutX = True
                        Case "y"
                            OutY = True
                        Case "z"
                            OutZ = True
                        Case "t"
                            OutT = True
                        Case "r"
                            OutR = True
                        Case "p"
                            OutP = True
                    End Select

                    Output = FuncSecond

                ElseIf FuncSecond.Length = 1 And (Not IsNumeric(FuncSecond)) Then ' تابع واقعی با جابجایی

                    Select Case FuncSecond.ToLower
                        Case "x"
                            OutX = True
                        Case "y"
                            OutY = True
                        Case "z"
                            OutZ = True
                        Case "t"
                            OutT = True
                        Case "r"
                            OutR = True
                        Case "p"
                            OutP = True
                    End Select

                    Output = FuncFrist

                Else   ' تابع ضمنی

                    IsImplicit = True

                    Success = False
                    'Eq.HasError = True
                    'Eq.ErrorMsg = "خروجی تابع باید بر حسب y یا x (کارتزین) و یا r یا t (قطبی) باشد    لطفا به بخش راهنما مراجعه کنید"
                End If
            Else 'ضابطه تابع بصورت ناقص تایپ شده
                ''TODO    AUTO DETECT

                Success = False
                'Eq.HasError = True
                'Eq.ErrorMsg = "خروجی تابع مشخص نشده است ! لطفا به بخش راهنما مراجعه کنید"
            End If
        Else 'ضابطه تابع تایپ نشده
            Success = False
        End If

        EquationTools.FixNotWrittenMul(Output)

        Return Output.ToLower

    End Function

    'تشخیص وابسته بودن به متغییر
    Public Function HaveVarsInFormula(Eq As Equation) As Boolean
        Dim EqMain As String = Eq.Expression.ToLower
        For Each MOV As MathObject In MathObjList
            If MOV.Type = 1 Then
                Dim Var As Variable = MOV.Inner
                If EqMain.Contains(Var.Name) Then
                    EqMain = EquationTools.Replace(EqMain, Var.Name, Var.Value)
                    Return True
                End If
            End If
        Next
        Return False
    End Function

#End Region

#Region "رسم کلی"

    'حالت رسم - 2 یا 3 یا 4 بعدی
    Private m_DrawMode As String = "2D"
    Public Property DrawMode As String
        Get
            Return m_DrawMode
        End Get
        Set(value As String)
            If Not (value = m_DrawMode) Then
                m_DrawMode = value
                NotifyPropertyChanged("DrawMode")
                ClearGraphObjects()
            End If
        End Set
    End Property
    Private Sub SwitchDM()
        If DrawMode = "2D" Then
            IsCor3DModified = True
            DrawMode = "3D"
        ElseIf DrawMode = "3D" Then
            IsCor3DModified = True
            DrawMode = "4D"
        ElseIf DrawMode = "4D" Then
            IsCor2DModified = True
            DrawMode = "2D"
        End If
        AutoRedraw()
    End Sub
    Private Sub ReverseSwitchDM()
        If DrawMode = "2D" Then
            IsCor3DModified = True
            DrawMode = "4D"
        ElseIf DrawMode = "3D" Then
            IsCor2DModified = True
            DrawMode = "2D"
        ElseIf DrawMode = "4D" Then
            IsCor3DModified = True
            DrawMode = "3D"
        End If
        AutoRedraw()
    End Sub

    'رسم دوباره نمودار ها
    Public Sub AutoRedraw()
        If DrawMode = "2D" Then
            Unload3D()
            Draw2D()
        ElseIf DrawMode = "3D" Then
            Draw3D()
        ElseIf DrawMode = "4D" Then
            Draw4D()
        End If
    End Sub
    Public Sub AutoRedrawCM()
        If DrawMode = "2D" Then IsCor2DModified = True
        If DrawMode = "3D" Then IsCor3DModified = True
        AutoRedraw()
    End Sub

    'تازی سازی نمودار بعد از تغییر سایز
    Public Sub SizeChangedRedraw() Handles Cor2D_Behind.SizeChanged
        If DrawMode = "2D" Then
            AutoRedrawCM()
        End If
    End Sub

    'پاک سازی نمودار های از پیش رسم شده
    Public Sub ClearGraphObjects()
        For Each MO As MathObject In MathObjList
            If (MO.Type <> 1) And (MO.Type <> 99) Then
                For Each GO In MO.Inner.GraphObject
                    If Cor2D_Front.Children.Contains(TryCast(GO, UIElement)) Then
                        Cor2D_Front.Children.Remove(GO)
                    End If
                    If VisualModel.Children.Contains(TryCast(GO, Model3D)) Then
                        VisualModel.Children.Remove(GO)
                    End If
                Next
                MO.Inner.GraphObject.Clear()
            End If
        Next
    End Sub

    'بازگرداندن تنظیمات به حالت اولیه
    Public Sub RestoreDefaults()
        DrawMode = "2D"
        CorDetail = 3
        CorBackground = Brushes.White
        Cor3DType = Cor3DTypes.Central
        ActivePlaneMode = PlaneMode.Normal
        Labels3DMode = Labels3D.Space
        ColoringMethod = 0
        ColorX = True
        ColorY = True
        ColorZ = True
        ColorW = True
        Len = 1
        LenScale = 100
        Len3DX = 1
        Len3DY = 1
        Len3DZ = 1
        LenVPX = 5
        LenVPY = 5
        LenVPZ = 5
        LenVPScale = 1
        ExtraX = 0
        ExtraY = 0
        MovedCamera2D = New Vector(0, 0)
        IsCor2DModified = True
        IsCor3DModified = True
        Acc = 50
        Acc3D = 5
        CameraLight = True
        SemiZoom = 5
        Zoom = 0
        ZoomMode3D = ZoomModes3D.Viewport
    End Sub

    'جزئیات دستگاه مختصات
    Private m_CorDetail As Integer = 3
    ' 0 --> No Cor    1 --> Just Main Division    2 --> Main And Sub Division   3 --> With Labels
    Public Property CorDetail As Integer
        Get
            Return m_CorDetail
        End Get
        Set(value As Integer)
            m_CorDetail = value
            NotifyPropertyChanged("CorDetail")
            DrawCoordinateSystem2D()
            DrawCoordinateSystem3D()
            DrawCoordinateSystem4D()
        End Set
    End Property

    'پس زمینه دستگاه مختصات
    Private m_CorBackground As Brush = Brushes.White
    Public Property CorBackground As Brush
        Get
            If m_CorBackground IsNot Nothing Then
                Return m_CorBackground
            Else
                Return Brushes.White
            End If
        End Get
        Set(value As Brush)
            m_CorBackground = value
            NotifyPropertyChanged("CorBackground")
        End Set
    End Property

#End Region

#Region "سیستم رنگ بندی"

    Private m_ColoringMethod As ColorMethod = ColorMethod.Brush
    Public Property ColoringMethod As ColorMethod
        Get
            Return m_ColoringMethod
        End Get
        Set(value As ColorMethod)
            m_ColoringMethod = value
            NotifyPropertyChanged("ColoringMethod")
        End Set
    End Property
    Public Enum ColorMethod
        Brush
        ByCor
    End Enum
#Region "ColorX,Y,Z,W Bools"
    Private m_ColorX As Boolean = True
    Public Property ColorX As Boolean
        Get
            Return m_ColorX
        End Get
        Set(value As Boolean)
            m_ColorX = value
            NotifyPropertyChanged("ColorX")
        End Set
    End Property
    Private m_ColorY As Boolean = True
    Public Property ColorY As Boolean
        Get
            Return m_ColorY
        End Get
        Set(value As Boolean)
            m_ColorY = value
            NotifyPropertyChanged("ColorY")
        End Set
    End Property
    Private m_ColorZ As Boolean = True
    Public Property ColorZ As Boolean
        Get
            Return m_ColorZ
        End Get
        Set(value As Boolean)
            m_ColorZ = value
            NotifyPropertyChanged("ColorZ")
        End Set
    End Property
    Private m_ColorW As Boolean = True
    Public Property ColorW As Boolean
        Get
            Return m_ColorW
        End Get
        Set(value As Boolean)
            m_ColorW = value
            NotifyPropertyChanged("ColorW")
        End Set
    End Property
#End Region

#End Region

#Region "رسم 2 بعدی"

    'تنظیمات رسم
    Private m_acc As Integer = 50
    Public Property Acc As Integer
        Get
            Return m_acc
        End Get
        Set(value As Integer)
            m_acc = value
            NotifyPropertyChanged("Acc")
        End Set
    End Property

    'تنظیمات دستگاه مختصات
    Public Property Len As Double = 1
    Public Property LenScale As Double = 100
    Public Property ExtraX As Single = 0
    Public Property ExtraY As Single = 0
    Public Property IsCor2DModified As Boolean = True

    'تنظیمات جابجایی
    Public Property CenterPoint As Point
    Public Property MovedCamera2D As New Vector(0, 0)

    Private Sub Draw2D()
        If DrawMode = "2D" Then
            If IsCor2DModified Then
                ExtraX = (-MovedCamera2D.X \ LenScale)
                ExtraY = (-MovedCamera2D.Y \ LenScale)
                DrawCoordinateSystem2D()
                DebugMsg("REDRAW --> Cor2D", 1)
            End If
            For Each MO As MathObject In MathObjList
                MO.Inner.HasError = False
                MO.Inner.ErrorMsg = ""
                Select Case MO.Type
                    Case 0 'Equation
                        If MO.Inner.NeedsRecalculation Or HaveVarsInFormula(MO.Inner) Or IsCor2DModified Then
                            For Each GO In MO.Inner.GraphObject
                                Cor2D_Front.Children.Remove(GO)
                            Next
                            MO.Inner.GraphObject.Clear()
                            If MO.Inner.IsVisible Then DrawGraph2D(MO.Inner)
                        End If
                    Case 2 'Point
                        For Each GO In MO.Inner.GraphObject
                            Cor2D_Front.Children.Remove(GO)
                        Next
                        MO.Inner.GraphObject.Clear()
                        If MO.Inner.IsVisible Then DrawPoint2D(MO.Inner)
                    Case 3 'Vector
                        For Each GO In MO.Inner.GraphObject
                            Cor2D_Front.Children.Remove(GO)
                        Next
                        MO.Inner.GraphObject.Clear()
                        If MO.Inner.IsVisible Then DrawVector2D(MO.Inner)
                    Case 4 'Shape
                        MO.Inner.HasError = True
                        MO.Inner.ErrorMsg = "لطفا حالت رسم را به 3 بعدی تغییر دهید"
                End Select
            Next
            IsCor2DModified = False
        End If
    End Sub
    'رسم دستگاه مختصات 2 بعدی
    Public Sub DrawCoordinateSystem2D()
        Cor2D_Behind.Children.Clear()

        Dim AW As Double = Cor2D_Behind.ActualWidth
        Dim AH As Double = Cor2D_Behind.ActualHeight

        Dim DivY As Integer = (AH \ (LenScale * 2)) + 1
        Dim DivX As Integer = (AW \ (LenScale * 2)) + 1

        CenterPoint = New Point(AW / 2, AH / 2) + MovedCamera2D

        'Pens
        Dim PenMain As New Pen(New SolidColorBrush(Colors.Black), 2)

        Dim PenS1 As New Pen(Brushes.DarkGray, 0.5)
        Dim PenS2 As New Pen(Brushes.Gray, 0.3)
        PenS2.DashStyle = New DashStyle({7, 3}, 5)

        If CorDetail >= 1 Then
            'محور های اصلی
            DrawLine2D(PenMain, CenterPoint.X, 0, CenterPoint.X, AH)
            DrawLine2D(PenMain, 0, CenterPoint.Y, AW, CenterPoint.Y)
        End If

        If CorDetail >= 2 Then

            If CorDetail = 3 Then DrawText2D(CenterPoint.X - 10, CenterPoint.Y, "0", Colors.Black)

            For i = -DivX + ExtraX To DivX + ExtraX
                Dim Value As Double
                Value = 0 + Len * i
                If Value <> 0 Then
                    DrawLine2D(PenS1, (CenterPoint.X + LenScale * i), 0, (CenterPoint.X + LenScale * i), AH)
                    If CorDetail = 3 Then
                        Dim TS As Size = MeasureTextSize(Value.ToString)
                        DrawText2D(CenterPoint.X + LenScale * i - (TS.Width / 2), CenterPoint.Y, Value.ToString, Colors.Black)
                    End If
                End If
            Next

            For i = -DivY + ExtraY To DivY + ExtraY
                Dim Value As Double
                Value = 0 - (Len * i)
                If Value <> 0 Then
                    DrawLine2D(PenS1, 0, (CenterPoint.Y + LenScale * i), AW, (CenterPoint.Y + LenScale * i))
                    If CorDetail = 3 Then
                        Dim TS As Size = MeasureTextSize(Value.ToString)
                        DrawText2D(CenterPoint.X - (TS.Width + 4), CenterPoint.Y + LenScale * i - (TS.Width / 2), Value.ToString, Colors.Black)
                    End If
                End If
            Next

        End If
    End Sub
    'رسم نمودار 
    Private Sub DrawGraph2D(Equation As Equation)
        Try
            EquationTools.DetectEqType(Equation, Me.DrawMode)
            For Each PL As PointCollection In GetGraphPoints2D(Equation)
                If Equation.Type <> EquationType.CartesianN Then
                    Dim Pol As New Polyline
                    Pol.Stroke = Equation.Brush
                    Pol.StrokeThickness = Equation.Thickness
                    Pol.Points = PL
                    Equation.GraphObject.Add(Pol)
                    Cor2D_Front.Children.Add(Pol)
                    Equation.NeedsRecalculation = False
                Else
                    For Each Pt As Point In PL
                        Dim El As New Ellipse
                        Dim ElW As Integer = 4
                        El.Fill = Equation.Brush
                        El.Width = 2 * ElW
                        El.Height = 2 * ElW
                        Canvas.SetLeft(El, Pt.X - ElW)
                        Canvas.SetTop(El, Pt.Y - ElW)
                        Equation.GraphObject.Add(El)
                        Cor2D_Front.Children.Add(El)
                        Equation.NeedsRecalculation = False
                    Next
                End If
            Next
        Catch Ex As Exception
            Equation.HasError = True
            Equation.ErrorMsg = "خطا در رسم" & vbCrLf & Ex.Message
        End Try
    End Sub

    Private Function GetGraphPoints2D(ByRef Equation As Equation) As List(Of PointCollection)

        'Cartesian
        'Type = 1  y=f(x)
        'Type = 2  x=f(y)

        'Polar
        'Type = 4  r=f(t)
        'Type = 5  t=f(r)

        'Parametric 2D
        'Type = 7  x=f(u) & y=f(u)

        Dim ValidTypes() As EquationType = {EquationType.CartesianY, EquationType.CartesianX, EquationType.CartesianN, EquationType.PolarR, EquationType.PolarT, EquationType.ParametricU}
        If Not ValidTypes.Contains(Equation.Type) Then
            Return New List(Of PointCollection)
        End If

        Dim AW As Double = Cor2D_Front.ActualWidth
        Dim AH As Double = Cor2D_Front.ActualHeight

        Dim DivY As Integer = (AH \ (LenScale * 2)) + 1
        Dim DivX As Integer = (AW \ (LenScale * 2)) + 1

        Dim LenY As Double = (AH / LenScale) * Len / 2
        Dim LenX As Double = (AW / LenScale) * Len / 2

        Dim SimplifySuccess As Boolean
        Dim EqMain As String = SimplifyFunction(Equation, SimplifySuccess)
        If SimplifySuccess = False Then Return New List(Of PointCollection)

        Dim PointsList As New List(Of PointCollection)
        Dim AccValue As Double = (1 / Acc) * (Len / 2)

        Dim EqDC As Boolean = EqMain.Contains("int") _
                              Or EqMain.Contains("floor") _
                              Or EqMain.Contains("ceil") _
                              Or EqMain.Contains("tan")

        Dim AllowedSpace As Double = 5

        Dim LastPointWasOutside As Boolean = False

        'جایگذاری متغییر ها
        For Each MOV As MathObject In MathObjList
            If MOV.Type = 1 Then
                Dim Var As Variable = MOV.Inner
                EqMain = EquationTools.Replace(EqMain, Var.Name, Var.Value)
            End If
        Next

        DebugMsg("EqM --> " & EqMain, 3)

        Select Case Equation.Type
            Case EquationType.CartesianY
                Dim PC As New PointCollection
                ' Draw Based On x  - تابع دکارتی
                Dim DS As Single
                Dim DE As Single

                If Equation.DomainStart = Double.NegativeInfinity Then
                    DS = -LenX + (-MovedCamera2D.X / LenScale) * Len
                Else
                    DS = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DE = +LenX + (-MovedCamera2D.X / LenScale) * Len
                Else
                    DE = Equation.DomainEnd
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim x, y As Single

                EqStr = EquationTools.Replace(EqMain, "x", DS)
                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)

                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = -EvalResult
                Equation.RangeMin = -EvalResult

                Dim OldPoint As New Point(CenterPoint.X + DS * (AW / (LenX * 2)), CenterPoint.Y - EvalResult * (AH / (LenY * 2)))

                For i = DS To DE Step AccValue

                    EqStr = EquationTools.Replace(EqMain, "x", MathTools.Round(i))
                    Dim EvalSuccess As Boolean
                    EvalResult = AKPEval.Eval(EqStr, EvalSuccess)

                    If EvalSuccess Then
                        EvalResult = Val(EvalResult)
                        x = CenterPoint.X + i * (AW / (LenX * 2))
                        y = CenterPoint.Y - EvalResult * (AH / (LenY * 2))

                        If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                        If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                        'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                        Dim IsInTheScreen As Boolean = InTheScreen(x, y)
                        'بررسی پیوستگی نمودار
                        Dim IsConnected As Boolean = True

                        Dim NewPoint As New Point(x, y)
                        If EqDC And PointsSpace(OldPoint, NewPoint) > AllowedSpace Then IsConnected = False
                        OldPoint = NewPoint

                        If IsInTheScreen Then
                            If LastPointWasOutside Then
                                IsConnected = False
                                LastPointWasOutside = False
                            End If
                            If IsConnected Then
                                PC.Add(NewPoint)
                            Else
                                PointsList.Add(PC)
                                PC = New PointCollection
                            End If
                        Else
                            LastPointWasOutside = True
                        End If
                    End If
                Next
                PointsList.Add(PC)

            Case EquationType.CartesianX
                Dim PC As New PointCollection
                ' Draw Based On y 
                Dim DS As Single
                Dim DE As Single

                If Equation.DomainStart = Double.NegativeInfinity Then
                    DS = -LenY + +(MovedCamera2D.Y / LenScale) * Len
                Else
                    DS = Val(Equation.DomainStart)
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DE = +LenY + +(MovedCamera2D.Y / LenScale) * Len
                Else
                    DE = Val(Equation.DomainEnd)
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim x, y As Single

                EqStr = EquationTools.Replace(EqMain, "y", DS)
                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)

                EvalResult = Val(EvalResult)
                'اولین برد
                Equation.RangeMax = EvalResult
                Equation.RangeMin = EvalResult

                Dim OldPoint As New Point(CenterPoint.X + DS * (AW / (LenX * 2)), CenterPoint.Y - EvalResult * (AH / (LenY * 2)))

                For i = DS To DE Step AccValue

                    EqStr = EquationTools.Replace(EqMain, "y", MathTools.Round(i))
                    Dim EvalSuccess As Boolean
                    EvalResult = AKPEval.Eval(EqStr, EvalSuccess)

                    If EvalSuccess Then
                        EvalResult = Val(EvalResult)
                        y = CenterPoint.Y - i * (Cor2D_Front.Height / (LenY * 2))
                        x = CenterPoint.X + EvalResult * (Cor2D_Front.Width / (LenX * 2))

                        If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                        If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                        Dim IsInTheScreen As Boolean = InTheScreen(x, y)
                        Dim IsConnected As Boolean = True

                        Dim NewPoint As New Point(x, y)
                        If EqDC And PointsSpace(OldPoint, NewPoint) > AllowedSpace Then IsConnected = False
                        OldPoint = NewPoint

                        If IsInTheScreen Then
                            If LastPointWasOutside Then
                                IsConnected = False
                                LastPointWasOutside = False
                            End If
                            If IsConnected Then
                                PC.Add(NewPoint)
                            Else
                                PointsList.Add(PC)
                                PC = New PointCollection
                            End If
                        Else
                            LastPointWasOutside = True
                        End If

                    End If
                Next
                PointsList.Add(PC)

            Case EquationType.CartesianN
                Dim PC As New PointCollection
                ' Draw Based On x  - تابع دکارتی
                Dim DS As Integer
                Dim DE As Integer

                If Equation.DomainStart = Double.NegativeInfinity Then
                    DS = 1
                Else
                    DS = CInt(Equation.DomainStart)
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DE = CInt(+LenX + (-MovedCamera2D.X / LenScale) * Len)
                Else
                    DE = CInt(Equation.DomainEnd)
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim n, y As Single

                EqStr = EquationTools.Replace(EqMain, "n", DS)
                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)

                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = -EvalResult
                Equation.RangeMin = -EvalResult

                Dim OldPoint As New Point(CenterPoint.X + DS * (AW / (LenX * 2)), CenterPoint.Y - EvalResult * (AH / (LenY * 2)))

                For i = DS To DE

                    EqStr = EquationTools.Replace(EqMain, "n", i)
                    Dim EvalSuccess As Boolean
                    EvalResult = AKPEval.Eval(EqStr, EvalSuccess)

                    If EvalSuccess Then
                        EvalResult = Val(EvalResult)
                        n = CenterPoint.X + i * (AW / (LenX * 2))
                        y = CenterPoint.Y - EvalResult * (AH / (LenY * 2))

                        If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                        If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                        'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                        Dim IsInTheScreen As Boolean = InTheScreen(n, y)
                        'بررسی پیوستگی نمودار
                        Dim IsConnected As Boolean = True

                        Dim NewPoint As New Point(n, y)
                        If EqDC And PointsSpace(OldPoint, NewPoint) > AllowedSpace Then IsConnected = False
                        OldPoint = NewPoint

                        If IsInTheScreen Then
                            If LastPointWasOutside Then
                                IsConnected = False
                                LastPointWasOutside = False
                            End If
                            If IsConnected Then
                                PC.Add(NewPoint)
                            Else
                                PointsList.Add(PC)
                                PC = New PointCollection
                            End If
                        Else
                            LastPointWasOutside = True
                        End If
                    End If
                Next
                PointsList.Add(PC)

            Case EquationType.PolarR
                Dim PC As New PointCollection

                ' Draw Based On T 
                Dim DS As Single
                Dim DE As Single

                If Equation.DomainStart = Double.NegativeInfinity Then
                    DS = 0
                Else
                    DS = Val(Equation.DomainStart)
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DE = Math.PI * 4 * Len
                Else
                    DE = Val(Equation.DomainEnd)
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim r, t, x, y As Single

                EqStr = EquationTools.Replace(EqMain, "t", DS)
                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)
                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = EvalResult
                Equation.RangeMin = EvalResult

                Dim OldPoint As New Point(CenterPoint.X + (EvalResult * Cos(DS)) * (Cor2D_Front.Width / (LenX * 2)) _
                                          , CenterPoint.Y - (EvalResult * Sin(DS)) * (Cor2D_Front.Height / (LenY * 2)))

                For i = DS To DE Step AccValue

                    EqStr = EquationTools.Replace(EqMain, "t", MathTools.Round(i))
                    Dim EvalSuccess As Boolean
                    EvalResult = AKPEval.Eval(EqStr, EvalSuccess)

                    If EvalSuccess Then
                        EvalResult = Val(EvalResult)

                        t = i
                        r = EvalResult
                        x = CenterPoint.X + (r * Cos(t)) * (Cor2D_Front.Width / (LenX * 2))
                        y = CenterPoint.Y - (r * Sin(t)) * (Cor2D_Front.Height / (LenY * 2))

                        If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                        If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                        Dim IsInTheScreen As Boolean = InTheScreen(x, y)
                        Dim IsConnected As Boolean = True

                        Dim NewPoint As New Point(x, y)
                        If EqDC And PointsSpace(OldPoint, NewPoint) > AllowedSpace Then IsConnected = False
                        OldPoint = NewPoint

                        If IsInTheScreen Then
                            If LastPointWasOutside Then
                                IsConnected = False
                                LastPointWasOutside = False
                            End If
                            If IsConnected Then
                                PC.Add(NewPoint)
                            Else
                                PointsList.Add(PC)
                                PC = New PointCollection
                            End If
                        Else
                            LastPointWasOutside = True
                        End If

                    End If
                Next
                PointsList.Add(PC)

            Case EquationType.PolarT
                Dim PC As New PointCollection

                ' Draw Based On R 
                Dim DS As Single
                Dim DE As Single

                If Equation.DomainStart = Double.NegativeInfinity Then
                    DS = 0
                Else
                    DS = Val(Equation.DomainStart)
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DE = Math.PI * 4 * Len
                Else
                    DE = Val(Equation.DomainEnd)
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim r, t, x, y As Single

                EqStr = EquationTools.Replace(EqMain, "r", DS)
                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)
                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = EvalResult
                Equation.RangeMin = EvalResult

                'ProssesBar
                Dim ATP As Double = Int(((DE - DS) * Acc) / 100)

                Dim OldPoint As New Point(CenterPoint.X + (DS * Sin(EvalResult)) * (Cor2D_Front.Height / (LenY * 2)) _
                                          , CenterPoint.Y - (DS * Cos(EvalResult)) * (Cor2D_Front.Width / (LenX * 2)))

                For i = DS To DE Step AccValue


                    EqStr = EquationTools.Replace(EqMain, "r", MathTools.Round(i))
                    Dim EvalSuccess As Boolean
                    EvalResult = AKPEval.Eval(EqStr, EvalSuccess)

                    If EvalSuccess Then
                        EvalResult = Val(EvalResult)

                        r = i
                        t = EvalResult
                        y = CenterPoint.Y - (r * Sin(t)) * (Cor2D_Front.Height / (LenY * 2))
                        x = CenterPoint.X + (r * Cos(t)) * (Cor2D_Front.Width / (LenX * 2))

                        If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                        If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                        Dim IsInTheScreen As Boolean = InTheScreen(x, y)
                        Dim IsConnected As Boolean = True

                        Dim NewPoint As New Point(x, y)
                        If EqDC And PointsSpace(OldPoint, NewPoint) > AllowedSpace Then IsConnected = False
                        OldPoint = NewPoint

                        If IsInTheScreen Then
                            If LastPointWasOutside Then
                                IsConnected = False
                                LastPointWasOutside = False
                            End If
                            If IsConnected Then
                                PC.Add(NewPoint)
                            Else
                                PointsList.Add(PC)
                                PC = New PointCollection
                            End If
                        Else
                            LastPointWasOutside = True
                        End If

                    End If
                Next
                PointsList.Add(PC)

            Case EquationType.ParametricU
                Dim PC As New PointCollection

                Dim XParameter As String = EquationTools.GetParameter(Equation, "x")
                Dim YParameter As String = EquationTools.GetParameter(Equation, "y")

                'MsgBox("XPARAM = " & XParameter)
                'MsgBox("YPARAM = " & YParameter)

                'جایگذاری متغییر ها
                For Each MOV As MathObject In MathObjList
                    If MOV.Type = 1 Then
                        Dim Var As Variable = MOV.Inner
                        XParameter = EquationTools.Replace(XParameter, Var.Name, Var.Value)
                        YParameter = EquationTools.Replace(YParameter, Var.Name, Var.Value)
                    End If
                Next

                Dim DS As Single
                Dim DE As Single

                If Equation.DomainStart = Double.NegativeInfinity Then
                    DS = -LenX + (-MovedCamera2D.X / LenScale) * Len
                Else
                    DS = Val(Equation.DomainStart)
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DE = +LenX + (-MovedCamera2D.X / LenScale) * Len
                Else
                    DE = Val(Equation.DomainEnd)
                End If

                Dim EqStrPX As String
                Dim EqStrPY As String
                Dim EvalResultPX As Double
                Dim EvalResultPY As Double
                Dim x, y As Single

                EqStrPX = EquationTools.Replace(XParameter, "u", DS)
                EqStrPY = EquationTools.Replace(YParameter, "u", DS)

                'MsgBox("EqStrPX = " & EqStrPX)
                'MsgBox("EqStrPY = " & EqStrPY)

                EvalResultPX = AKPEval.Eval(EqStrPX, New Boolean, True)
                EvalResultPX = Val(EvalResultPX)

                EvalResultPY = AKPEval.Eval(EqStrPY, New Boolean, True)
                EvalResultPY = Val(EvalResultPY)

                'اولین برد
                Equation.RangeMax = -EvalResultPY
                Equation.RangeMin = -EvalResultPY

                Dim OldPoint As New Point(CenterPoint.X + EvalResultPX * (AW / (LenX * 2)), CenterPoint.Y - EvalResultPY * (AH / (LenY * 2)))

                For u = DS To DE Step AccValue

                    EqStrPX = EquationTools.Replace(XParameter, "u", MathTools.Round(u))
                    Dim EvalSuccessPX As Boolean
                    EvalResultPX = AKPEval.Eval(EqStrPX, EvalSuccessPX)

                    EqStrPY = EquationTools.Replace(YParameter, "u", MathTools.Round(u))
                    Dim EvalSuccessPY As Boolean
                    EvalResultPY = AKPEval.Eval(EqStrPY, EvalSuccessPY)

                    Dim EvalSuccess As Boolean = EvalSuccessPX And EvalSuccessPY

                    If EvalSuccess Then
                        EvalResultPX = Val(EvalResultPX)
                        EvalResultPY = Val(EvalResultPY)

                        x = CenterPoint.X + EvalResultPX * (AW / (LenX * 2))
                        y = CenterPoint.Y - EvalResultPY * (AH / (LenY * 2))

                        If EvalResultPY > Equation.RangeMax Then Equation.RangeMax = EvalResultPY
                        If EvalResultPY < Equation.RangeMin Then Equation.RangeMin = EvalResultPY

                        'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                        Dim IsInTheScreen As Boolean = InTheScreen(x, y)
                        'بررسی پیوستگی نمودار
                        Dim IsConnected As Boolean = True

                        Dim NewPoint As New Point(x, y)
                        If EqDC And PointsSpace(OldPoint, NewPoint) > AllowedSpace Then IsConnected = False
                        OldPoint = NewPoint

                        If IsInTheScreen Then
                            If LastPointWasOutside Then
                                IsConnected = False
                                LastPointWasOutside = False
                            End If
                            If IsConnected Then
                                PC.Add(NewPoint)
                            Else
                                PointsList.Add(PC)
                                PC = New PointCollection
                            End If
                        Else
                            LastPointWasOutside = True
                        End If
                    End If
                Next
                PointsList.Add(PC)

        End Select

        Return PointsList

    End Function


#End Region

#Region "ابزار های رسم 2 بعدی"

    'خط 2 بعدی
    Private Sub DrawLine2D(Pen As Pen, X1 As Double, Y1 As Double, X2 As Double, Y2 As Double)
        Dim Line As New Line

        Line.Stroke = Pen.Brush
        Line.StrokeThickness = Pen.Thickness
        Line.StrokeDashArray = Pen.DashStyle.Dashes
        Line.StrokeDashOffset = Pen.DashStyle.Offset

        Line.X1 = X1
        Line.Y1 = Y1
        Line.X2 = X2
        Line.Y2 = Y2

        Cor2D_Behind.Children.Add(Line)
    End Sub
    'متن 2 بعدی
    Private Sub DrawText2D(X As Double, Y As Double, Text As String, Color As Color)
        Dim TB As New TextBlock

        TB.Text = Text
        TB.Foreground = New SolidColorBrush(Color)

        Canvas.SetLeft(TB, X)
        Canvas.SetTop(TB, Y)

        Cor2D_Behind.Children.Add(TB)
    End Sub
    'نقطه 2 بعدی
    Private Sub DrawPoint2D(PtV As APointViewer)

        Dim Tx As String = PtV.Point.X
        Dim Ty As String = PtV.Point.Y
        Dim Tz As String = PtV.Point.Z

        Dim Rx As Double
        Dim Ry As Double
        Dim Rz As Double

        Dim IsMovingPoint As Boolean

        'جایگذاری متغییر ها
        For Each MOV As MathObject In MathObjList
            If MOV.Type = 1 Then
                Dim Var As Variable = MOV.Inner
                IsMovingPoint = IsMovingPoint Or Tx.Contains(Var.Name) Or Ty.Contains(Var.Name) Or Tz.Contains(Var.Name)
                Tx = EquationTools.Replace(Tx.ToLower, Var.Name, Var.Value)
                Ty = EquationTools.Replace(Ty.ToLower, Var.Name, Var.Value)
                Tz = EquationTools.Replace(Tz.ToLower, Var.Name, Var.Value)
            End If
        Next
        Dim TxEvalSuccess As Boolean
        Dim TyEvalSuccess As Boolean
        Dim TzEvalSuccess As Boolean

        Tx = AKPEval.Eval(Tx, TxEvalSuccess)
        Ty = AKPEval.Eval(Ty, TyEvalSuccess)
        Tz = AKPEval.Eval(Tz, TzEvalSuccess)

        If IsNumeric(Tx) And TxEvalSuccess Then
            Rx = CDbl(Tx)
        Else
            PtV.HasError = True
            PtV.ErrorMsg = Tx + " تعریف نشده است."
        End If
        If IsNumeric(Ty) And TyEvalSuccess Then
            Ry = CDbl(Ty)
        Else
            PtV.HasError = True
            PtV.ErrorMsg = Ty + " تعریف نشده است."
        End If
        If IsNumeric(Tz) And TzEvalSuccess Then
            Rz = CDbl(Tz)
        Else
            PtV.HasError = True
            PtV.ErrorMsg = Tz + " تعریف نشده است."
        End If

        Dim Ci As New Ellipse
        Dim CiD As Integer = 4
        Ci.Width = 2 * CiD
        Ci.Height = 2 * CiD
        Dim AW As Double = Cor2D_Front.ActualWidth
        Dim AH As Double = Cor2D_Front.ActualHeight
        Dim LenY As Double = (AH / LenScale) * Len / 2
        Dim LenX As Double = (AW / LenScale) * Len / 2
        Dim CiX As Double = CenterPoint.X + Rx * (AW / (LenX * 2)) - CiD
        Dim CiY As Double = CenterPoint.Y - Ry * (AH / (LenY * 2)) - CiD
        Canvas.SetLeft(Ci, CiX)
        Canvas.SetTop(Ci, CiY)
        Ci.StrokeThickness = 3
        Ci.Stretch = Stretch.Fill
        Ci.Stroke = New SolidColorBrush(PtV.Color)
        Ci.Fill = New SolidColorBrush(PtV.Color)
        Ci.IsHitTestVisible = True
        Ci.ToolTip = New ToolTip With {.Content = Rx.ToString + " , " + Ry.ToString}
        PtV.GraphObject.Add(Ci)
        Cor2D_Front.Children.Add(Ci)

        'Trial Effect
        CorInfo.Text = "HaveTrial : " & PtV.HaveTrial.ToString
        If PtV.HaveTrial And IsMovingPoint Then
            CorInfo.Text = "TESTING TRIAL EFFECT"
            If PtV.Trial.Points.Count < PtV.Trial.Length Then
                PtV.Trial.Points.Add(New Point3D(Rx, Ry, 0))
            Else
                PtV.Trial.Points.RemoveAt(0)
                PtV.Trial.Points.Add(New Point3D(Rx, Ry, 0))
            End If
            If PtV.Trial.Points.Count > 1 Then
                Dim GP As New GradientPath
                Dim PathGeo As New PathGeometry
                Dim PathFig As New PathFigure()
                Dim SubPts As New List(Of Point)
                For Di = 0 To PtV.Trial.Points.Count - 1
                    Dim TPPX As Double = CenterPoint.X + PtV.Trial.Points(Di).X * (AW / (LenX * 2))
                    Dim TPPY As Double = CenterPoint.Y - PtV.Trial.Points(Di).Y * (AH / (LenY * 2))
                    SubPts.Add(New Point(TPPX, TPPY))
                Next
                'PathFig.StartPoint = PtV.Trial.Points(0).ToPoint2D
                PathFig.StartPoint = SubPts(0)
                PathFig.Segments.Add(New PolyLineSegment(SubPts, True))
                PathGeo.Figures.Add(PathFig)
                GP.Data = PathGeo
                GP.GradientMode = GradientMode.Parallel
                GP.GradientStops.Add(New GradientStop(Colors.Transparent, 0))
                GP.GradientStops.Add(New GradientStop(PtV.Color, 1))
                'Dim GPX As Double = CenterPoint.X + Rx * (AW / (LenX * 2))
                'Dim GPY As Double = CenterPoint.Y - Ry * (AH / (LenY * 2))
                'Canvas.SetLeft(GP, GPX)
                'Canvas.SetTop(GP, GPY)
                GP.StrokeThickness = 3
                PtV.GraphObject.Add(GP)
                Cor2D_Front.Children.Add(GP)
            End If
        End If

    End Sub
    'بردار 2 بعدی
    Private Sub DrawVector2D(VectV As AVectorViewer)
        Dim Tx As String = "0"
        Dim Ty As String = "0"

        'دریافت اطلاعات نقطه
        If VectV.PointName <> "" Then
            If VectV.PointName.Contains(",") Then 'مختصات داخلی
                Dim PNWP As String = VectV.PointName.Trim("(", ")")
                Dim SCor() As String = PNWP.Split(",")
                Try
                    Tx = If(SCor(0) <> "", SCor(0), "0")
                    Ty = If(SCor(1) <> "", SCor(1), "0")
                Catch
                    VectV.HasError = True
                    VectV.ErrorMsg = "لطفا مختصات نقطه مبدا را بصورت کامل وارد کنید"
                End Try
            Else 'جستجو برای مختصات خارجی
                For Each MOV As MathObject In MathObjList
                    If MOV.Type = 2 Then
                        Dim Pt As APointViewer = TryCast(MOV.Inner, APointViewer)
                        If Pt.Name = VectV.PointName Then
                            Tx = Pt.Point.X
                            Ty = Pt.Point.Y
                        End If
                    End If
                Next
            End If
        End If

        Dim Ti As String = VectV.Vector.X
        Dim Tj As String = VectV.Vector.Y

        Dim Rx As Double
        Dim Ry As Double

        Dim Ri As Double
        Dim Rj As Double

        'جایگذاری متغییر ها
        For Each MOV As MathObject In MathObjList
            If MOV.Type = 1 Then
                Dim Var As Variable = MOV.Inner
                Tx = EquationTools.Replace(Tx.ToLower, Var.Name, Var.Value)
                Ty = EquationTools.Replace(Ty.ToLower, Var.Name, Var.Value)

                Ti = EquationTools.Replace(Ti.ToLower, Var.Name, Var.Value)
                Tj = EquationTools.Replace(Tj.ToLower, Var.Name, Var.Value)
            End If
        Next
        Dim TxEvalSuccess As Boolean
        Dim TyEvalSuccess As Boolean
        Tx = AKPEval.Eval(Tx, TxEvalSuccess)
        Ty = AKPEval.Eval(Ty, TyEvalSuccess)

        Dim TiEvalSuccess As Boolean
        Dim TjEvalSuccess As Boolean
        Ti = AKPEval.Eval(Ti, TiEvalSuccess)
        Tj = AKPEval.Eval(Tj, TjEvalSuccess)

        If IsNumeric(Tx) And TxEvalSuccess Then
            Rx = CDbl(Tx)
        Else
            VectV.HasError = True
            VectV.ErrorMsg = Tx + " تعریف نشده است."
        End If
        If IsNumeric(Ty) And TyEvalSuccess Then
            Ry = CDbl(Ty)
        Else
            VectV.HasError = True
            VectV.ErrorMsg = Ty + " تعریف نشده است."
        End If


        If IsNumeric(Ti) And TiEvalSuccess Then
            Ri = CDbl(Ti)
        Else
            VectV.HasError = True
            VectV.ErrorMsg = Ti + " تعریف نشده است."
        End If
        If IsNumeric(Tj) And TjEvalSuccess Then
            Rj = CDbl(Tj)
        Else
            VectV.HasError = True
            VectV.ErrorMsg = Tj + " تعریف نشده است."
        End If

        Dim X1 As Double = Rx
        Dim X2 As Double = Rx + Ri
        Dim Y1 As Double = Ry
        Dim Y2 As Double = Ry + Rj

        Dim Ar As New Arrow
        Dim AW As Double = Cor2D_Front.ActualWidth
        Dim AH As Double = Cor2D_Front.ActualHeight
        Dim LenY As Double = (AH / LenScale) * Len / 2
        Dim LenX As Double = (AW / LenScale) * Len / 2
        Dim ArX1 As Double = CenterPoint.X + X1 * (AW / (LenX * 2))
        Dim ArY1 As Double = CenterPoint.Y - Y1 * (AH / (LenY * 2))
        Dim ArX2 As Double = CenterPoint.X + X2 * (AW / (LenX * 2))
        Dim ArY2 As Double = CenterPoint.Y - Y2 * (AH / (LenY * 2))
        Ar.X1 = ArX1
        Ar.Y1 = ArY1
        Ar.X2 = ArX2
        Ar.Y2 = ArY2
        Ar.HeadHeight = 7
        Ar.HeadWidth = 7
        Ar.StrokeThickness = 2
        Ar.Stroke = New SolidColorBrush(VectV.Color)
        Ar.IsHitTestVisible = True
        Ar.ToolTip = New ToolTip With {.Content = (X2 - X1).ToString + " , " + (Y2 - Y1).ToString}
        VectV.GraphObject.Add(Ar)
        Cor2D_Front.Children.Add(Ar)
    End Sub

#End Region

#Region "رسم 3 بعدی"

    'تنظیمات رسم
    Private m_acc3d As Integer = 5
    Public Property Acc3D As Integer
        Get
            Return m_acc3d
        End Get
        Set(value As Integer)
            m_acc3d = value
            NotifyPropertyChanged("Acc3D")
        End Set
    End Property

    'تنظیمات دستگاه مختصات
    Public Property Len3DX As Double = 1
    Public Property Len3DY As Double = 1
    Public Property Len3DZ As Double = 1
    Public Property Len3DW As Double = 1
    Public Property LenVPX As Double = 5
    Public Property LenVPY As Double = 5
    Public Property LenVPZ As Double = 5
    Public Property LenVPW As Double = 5
    Public Property LenVPScale As Double = 1
    Public Property IsCor3DModified As Boolean = True

    'زاویه دید 3 بعدی
    Public Property Transform3D As New Transform3DGroup

    Private Sub Draw3D()
        VisualWire.Children.Clear()
        InitializeLight()

        If IsCor3DModified = True Then
            DrawCoordinateSystem3D()
        End If

        Select Case ActivePlaneMode
            Case PlaneMode.Normal
                For Each MO As MathObject In MathObjList
                    MO.Inner.HasError = False
                    MO.Inner.ErrorMsg = ""
                    Select Case MO.Type
                        Case 0 'Equation
                            DebugMsg("REDRAW 3D [Normal] --> " & MO.Inner.Expression, 1)
                            DebugMsg("Needs Recalculation :" & MO.Inner.NeedsRecalculation, 2)
                            If MO.Inner.NeedsRecalculation Or HaveVarsInFormula(MO.Inner) Or IsCor3DModified Then
                                For Each GO In MO.Inner.GraphObject
                                    VisualModel.Children.Remove(GO)
                                Next
                                MO.Inner.GraphObject.Clear()
                                If MO.Inner.IsVisible Then DrawGraph3D(MO.Inner)
                            End If
                        Case 2 'Point
                            For Each GO In MO.Inner.GraphObject
                                VisualModel.Children.Remove(GO)
                            Next
                            MO.Inner.GraphObject.Clear()
                            If MO.Inner.IsVisible Then DrawPoint3D(MO.Inner)
                        Case 3 'Vector
                            If MO.Inner.IsVisible Then DrawVector3D(MO.Inner)
                        Case 4 'Shape
                            For Each GO In MO.Inner.GraphObject
                                VisualModel.Children.Remove(GO)
                            Next
                            MO.Inner.GraphObject.Clear()
                            If MO.Inner.IsVisible Then DrawShape3D(MO.Inner)
                    End Select
                Next

            Case PlaneMode.Wireframe
                For Each MO As MathObject In MathObjList
                    MO.Inner.HasError = False
                    MO.Inner.ErrorMsg = ""
                    Select Case MO.Type
                        Case 0 'Equation
                            DebugMsg("REDRAW 3D [Wire] --> Equation (" & MO.Inner.Expression & ")", 1)
                            If MO.Inner.NeedsRecalculation Or HaveVarsInFormula(MO.Inner) Or IsCor3DModified Then
                                For Each GO In MO.Inner.GraphObject
                                    VisualModel.Children.Remove(GO)
                                Next
                                MO.Inner.GraphObject.Clear()
                                If MO.Inner.IsVisible Then DrawGraphWireframe3D(MO.Inner)
                            End If
                        Case 2 'Point                            
                            For Each GO In MO.Inner.GraphObject
                                VisualModel.Children.Remove(GO)
                            Next
                            MO.Inner.GraphObject.Clear()
                            If MO.Inner.IsVisible Then DrawPoint3D(MO.Inner)
                        Case 3 'Vector
                            If MO.Inner.IsVisible Then DrawVector3D(MO.Inner)
                    End Select
                Next

            Case PlaneMode.PointModel
                For Each MO As MathObject In MathObjList
                    MO.Inner.HasError = False
                    MO.Inner.ErrorMsg = ""
                    Select Case MO.Type
                        Case 0 'Equation
                            Dim Eq As Equation = MO.Inner
                            DebugMsg("REDRAW 3D [Point] --> Equation (" & MO.Inner.Expression & ")", 1)
                            If Eq.NeedsRecalculation Or IsCor3DModified Then
                                For Each GO In Eq.GraphObject
                                    VisualModel.Children.Remove(GO)
                                Next
                                Eq.GraphObject.Clear()
                                If MO.Inner.IsVisible Then DrawGraphPointModel3D(MO.Inner)
                            End If
                        Case 2 'Point
                            For Each GO In MO.Inner.GraphObject
                                VisualModel.Children.Remove(GO)
                            Next
                            MO.Inner.GraphObject.Clear()
                            If MO.Inner.IsVisible Then DrawPoint3D(MO.Inner)
                        Case 3 'Vector
                            If MO.Inner.IsVisible Then DrawVector3D(MO.Inner)
                    End Select
                Next

            Case PlaneMode.DebugMode
                For Each MO As MathObject In MathObjList
                    MO.Inner.HasError = False
                    MO.Inner.ErrorMsg = ""
                    Select Case MO.Type
                        Case 0 'Equation
                            DebugMsg("REDRAW 3D [DEBUG] --> " & MO.Inner.Expression, 1)
                            DebugMsg("Needs Recalculation :" & MO.Inner.NeedsRecalculation, 2)
                            If MO.Inner.NeedsRecalculation Or HaveVarsInFormula(MO.Inner) Or IsCor3DModified Then
                                For Each GO In MO.Inner.GraphObject
                                    VisualModel.Children.Remove(GO)
                                Next
                                MO.Inner.GraphObject.Clear()
                                If MO.Inner.IsVisible Then DrawGraph3DDebugMode(MO.Inner)
                            End If
                        Case 2 'Point
                            For Each GO In MO.Inner.GraphObject
                                VisualModel.Children.Remove(GO)
                            Next
                            MO.Inner.GraphObject.Clear()
                            If MO.Inner.IsVisible Then DrawPoint3D(MO.Inner)
                        Case 3 'Vector
                            If MO.Inner.IsVisible Then DrawVector3D(MO.Inner)
                        Case 4 'Shape
                            For Each GO In MO.Inner.GraphObject
                                VisualModel.Children.Remove(GO)
                            Next
                            MO.Inner.GraphObject.Clear()
                            If MO.Inner.IsVisible Then DrawShape3D(MO.Inner)
                    End Select
                Next
        End Select
        IsCor3DModified = False
    End Sub
    'آزاد کردن منابع مصرفی
    Private Sub Unload3D()
        VisualModel.Children.Clear()
        VisualWire.Children.Clear()
    End Sub
    'نور پردازی
    Public Property CameraLight As Boolean = True
    Private Sub InitializeLight()
        GroupLight.Children.Clear()
        If ColoringMethod = ColorMethod.Brush Then
            If CameraLight Then
                Dim DirLight As New DirectionalLight(Colors.White, New Vector3D(0, 0, -1))
                GroupLight.Children.Add(DirLight)
            End If
            For Each MO As MathObject In MathObjList
                If MO.Type = 2 Then
                    Dim InnerPt As APointViewer = TryCast(MO.Inner, APointViewer)
                    If InnerPt.IsLight Then
                        Dim Tx As String = InnerPt.Point.X
                        Dim Ty As String = InnerPt.Point.Y
                        Dim Tz As String = InnerPt.Point.Z

                        Dim Rx As Double
                        Dim Ry As Double
                        Dim Rz As Double

                        'جایگذاری متغییر ها
                        For Each MOV As MathObject In MathObjList
                            If MOV.Type = 1 Then
                                Dim Var As Variable = MOV.Inner
                                Tx = EquationTools.Replace(Tx.ToLower, Var.Name, Var.Value)
                                Ty = EquationTools.Replace(Ty.ToLower, Var.Name, Var.Value)
                                Tz = EquationTools.Replace(Tz.ToLower, Var.Name, Var.Value)
                            End If
                        Next
                        Dim TxEvalSuccess As Boolean
                        Dim TyEvalSuccess As Boolean
                        Dim TzEvalSuccess As Boolean
                        Tx = AKPEval.Eval(Tx, TxEvalSuccess)
                        Ty = AKPEval.Eval(Ty, TyEvalSuccess)
                        Tz = AKPEval.Eval(Tz, TzEvalSuccess)

                        If IsNumeric(Tx) And TxEvalSuccess Then
                            Rx = CDbl(Tx)
                        End If
                        If IsNumeric(Ty) And TyEvalSuccess Then
                            Ry = CDbl(Ty)
                        End If
                        If IsNumeric(Tz) And TzEvalSuccess Then
                            Rz = CDbl(Tz)
                        End If
                        Dim PtLight As New PointLight()
                        PtLight.Position = New Point3D(Rx, Ry, Rz)
                        PtLight.Color = Colors.White
                        PtLight.ConstantAttenuation = 1
                        GroupLight.Children.Add(PtLight)
                    End If
                End If
            Next
        Else
            Dim AmbLight As New AmbientLight(Colors.White)
            GroupLight.Children.Add(AmbLight)
        End If
    End Sub
    'نوع دستگاه مختصات 3 بعدی
    Private m_Cor3DType As Cor3DTypes = Cor3DTypes.Central
    Public Property Cor3DType As Cor3DTypes
        Get
            Return m_Cor3DType
        End Get
        Set(value As Cor3DTypes)
            If Not (value = Cor3DType) Then
                m_Cor3DType = value
                NotifyPropertyChanged("Cor3DType")
            End If
        End Set
    End Property
    Enum Cor3DTypes
        Central
        Corneral
    End Enum
    'رسم دستگاه مختصات 3 بعدی
    Dim Cor3DThickness As Integer = 3
    Public Sub DrawCoordinateSystem3D()
        VisualCorLines.Children.Clear()
        OverlayPoints.Clear()

        If Cor3DType = Cor3DTypes.Corneral Then

            'دستگاه مختصات گوشه ای
            If CorDetail >= 1 Then
                Dim XAxis As New WireLine
                XAxis.Color = Colors.Blue
                XAxis.Thickness = Cor3DThickness
                XAxis.Point1 = New Point3D(-LenVPX, -LenVPY, -LenVPZ)
                XAxis.Point2 = New Point3D(LenVPX, -LenVPY, -LenVPZ)
                XAxis.ArrowEnds = 2
                XAxis.ArrowLength = 10
                XAxis.ArrowAngle = 45

                Dim XLbl As New WireText
                XLbl.Text = "X"
                XLbl.Color = Colors.Blue
                XLbl.UpDirection = New Vector3D(0, 1, 0)
                XLbl.BaselineDirection = New Vector3D(1, 0, 0)
                XLbl.Thickness = Cor3DThickness
                XLbl.Origin = New Point3D(LenVPX + 0.2, -LenVPY, -LenVPZ)
                XLbl.Font = Font.Roman
                XLbl.FontSize = 0.5

                Dim YAxis As New WireLine
                YAxis.Color = Colors.Red
                YAxis.Thickness = Cor3DThickness
                YAxis.Point1 = New Point3D(-LenVPX, -LenVPY, -LenVPZ)
                YAxis.Point2 = New Point3D(-LenVPX, LenVPY, -LenVPZ)
                YAxis.ArrowEnds = 2
                YAxis.ArrowLength = 10
                YAxis.ArrowAngle = 45

                Dim YLbl As New WireText
                YLbl.Text = "Y"
                YLbl.Color = Colors.Red
                YLbl.UpDirection = New Vector3D(0, 1, 0)
                YLbl.Thickness = Cor3DThickness
                YLbl.Origin = New Point3D(-LenVPX, LenVPY + 0.5, -LenVPZ)
                YLbl.Font = Font.Roman
                YLbl.FontSize = 0.5

                Dim ZAxis As New WireLine
                ZAxis.Color = Colors.Green
                ZAxis.Thickness = Cor3DThickness
                ZAxis.Point1 = New Point3D(-LenVPX, -LenVPY, -LenVPZ)
                ZAxis.Point2 = New Point3D(-LenVPX, -LenVPY, LenVPZ)
                ZAxis.ArrowEnds = 2
                ZAxis.ArrowLength = 10
                ZAxis.ArrowAngle = 45

                Dim ZLbl As New WireText
                ZLbl.Text = "Z"
                ZLbl.Color = Colors.Green
                ZLbl.UpDirection = New Vector3D(0, 1, 0)
                ZLbl.BaselineDirection = New Vector3D(0, 0, -1)
                ZLbl.Thickness = Cor3DThickness
                ZLbl.Origin = New Point3D(-LenVPX, -LenVPY, LenVPZ + 0.5)
                ZLbl.Font = Font.Roman
                ZLbl.FontSize = 0.5

                VisualCorLines.Children.Add(XAxis)
                VisualCorLines.Children.Add(XLbl)

                VisualCorLines.Children.Add(YAxis)
                VisualCorLines.Children.Add(YLbl)

                VisualCorLines.Children.Add(ZAxis)
                VisualCorLines.Children.Add(ZLbl)
            End If


            Dim DivX As Integer = (LenVPX \ LenVPScale)
            Dim DivChangeVPX As Double = LenVPX / DivX

            For i = -DivX + 1 To DivX
                Dim Value As Single
                Value = (Len3DX * i)

                If CorDetail >= 2 Then
                    Dim PSX As New Point3D((DivChangeVPX * i), -LenVPY, -LenVPZ)
                    Dim PSY As New Point3D((DivChangeVPX * i), LenVPY, -LenVPZ)
                    Dim PSZ As New Point3D((DivChangeVPX * i), -LenVPY, LenVPZ)
                    DrawLine3D(Colors.DarkGray, PSX, PSY)
                    DrawLine3D(Colors.DarkGray, PSX, PSZ)
                End If

                If CorDetail = 3 Then
                    Dim PText As New Point3D((DivChangeVPX * i), -LenVPY + 0.3, -LenVPZ)
                    Dim VectO As New Vector3D(1, 0, 0) 'VectorOver
                    If Labels3DMode = Labels3D.Space Then
                        DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Blue)
                    ElseIf Labels3DMode = Labels3D.Overlay Then
                        DrawText3D_Overlay(PText, Value.ToString, Colors.Blue)
                    End If
                End If
            Next

            Dim DivY As Integer = (LenVPY \ LenVPScale)
            Dim DivChangeVPY As Double = LenVPY / DivY

            For i = -DivY + 1 To DivY
                Dim Value As Single
                Value = (Len3DY * i)

                If CorDetail >= 2 Then
                    Dim PSY As New Point3D(-LenVPX, (DivChangeVPY * i), -LenVPZ)
                    Dim PSX As New Point3D(LenVPX, (DivChangeVPY * i), -LenVPZ)
                    Dim PSZ As New Point3D(-LenVPX, (DivChangeVPY * i), LenVPZ)
                    DrawLine3D(Colors.DarkGray, PSY, PSX)
                    DrawLine3D(Colors.DarkGray, PSY, PSZ)
                End If

                If CorDetail = 3 Then
                    Dim PText As New Point3D(-LenVPX + 0.3, (DivChangeVPY * i), -LenVPZ)
                    Dim VectO As New Vector3D(1, 0, 0) 'VectorOver
                    If Labels3DMode = Labels3D.Space Then
                        DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Red)
                    ElseIf Labels3DMode = Labels3D.Overlay Then
                        DrawText3D_Overlay(PText, Value.ToString, Colors.Red)
                    End If
                End If
            Next

            Dim DivZ As Integer = (LenVPZ \ LenVPScale)
            Dim DivChangeVPZ As Double = LenVPZ / DivZ

            For i = -DivZ + 1 To DivZ
                Dim Value As Single
                Value = (Len3DZ * i)

                If CorDetail >= 2 Then
                    Dim PSZ As New Point3D(-LenVPX, -LenVPY, (DivChangeVPZ * i))
                    Dim PSX As New Point3D(LenVPX, -LenVPY, (DivChangeVPZ * i))
                    Dim PSY As New Point3D(-LenVPX, LenVPY, (DivChangeVPZ * i))
                    DrawLine3D(Colors.DarkGray, PSZ, PSX)
                    DrawLine3D(Colors.DarkGray, PSZ, PSY)
                End If

                If CorDetail = 3 Then
                    Dim PText As New Point3D(-LenVPX, -LenVPY + 0.3, (DivChangeVPZ * i))
                    Dim VectO As New Vector3D(0, 0, 1) 'VectorOver
                    If Labels3DMode = Labels3D.Space Then
                        DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Green)
                    ElseIf Labels3DMode = Labels3D.Overlay Then
                        DrawText3D_Overlay(PText, Value.ToString, Colors.Green)
                    End If
                End If
            Next

            UpdateOverlay()

        ElseIf Cor3DType = Cor3DTypes.Central Then

            'دستگاه مختصات مرکزی

            If CorDetail >= 1 Then
                Dim XAxis As New WireLine
                XAxis.Color = Colors.Blue
                XAxis.Thickness = Cor3DThickness
                XAxis.Point1 = New Point3D(-LenVPX, 0, 0)
                XAxis.Point2 = New Point3D(LenVPX, 0, 0)
                XAxis.ArrowEnds = 2
                XAxis.ArrowLength = 10
                XAxis.ArrowAngle = 45
                VisualCorLines.Children.Add(XAxis)

                Dim XLbl As New WireText
                XLbl.Text = "X"
                XLbl.Color = Colors.Blue
                XLbl.UpDirection = New Vector3D(0, 1, 0)
                XLbl.BaselineDirection = New Vector3D(1, 0, 0)
                XLbl.Thickness = Cor3DThickness
                XLbl.Origin = New Point3D(LenVPX + 0.2, 0, 0)
                XLbl.Font = Font.Roman
                XLbl.FontSize = 0.5
                VisualCorLines.Children.Add(XLbl)

                Dim YAxis As New WireLine
                YAxis.Color = Colors.Red
                YAxis.Thickness = Cor3DThickness
                YAxis.Point1 = New Point3D(0, -LenVPY, 0)
                YAxis.Point2 = New Point3D(0, LenVPY, 0)
                YAxis.ArrowEnds = 2
                YAxis.ArrowLength = 10
                YAxis.ArrowAngle = 45
                VisualCorLines.Children.Add(YAxis)

                Dim YLbl As New WireText
                YLbl.Text = "Y"
                YLbl.Color = Colors.Red
                YLbl.UpDirection = New Vector3D(0, 1, 0)
                YLbl.Thickness = Cor3DThickness
                YLbl.Origin = New Point3D(0, LenVPY + 0.5, 0)
                YLbl.Font = Font.Roman
                YLbl.FontSize = 0.5
                VisualCorLines.Children.Add(YLbl)

                Dim ZAxis As New WireLine
                ZAxis.Color = Colors.Green
                ZAxis.Thickness = Cor3DThickness
                ZAxis.Point1 = New Point3D(0, 0, -LenVPZ)
                ZAxis.Point2 = New Point3D(0, 0, LenVPZ)
                ZAxis.ArrowEnds = 2
                ZAxis.ArrowLength = 10
                ZAxis.ArrowAngle = 45
                VisualCorLines.Children.Add(ZAxis)

                Dim ZLbl As New WireText
                ZLbl.Text = "Z"
                ZLbl.Color = Colors.Green
                ZLbl.UpDirection = New Vector3D(0, 1, 0)
                ZLbl.BaselineDirection = New Vector3D(0, 0, -1)
                ZLbl.Thickness = Cor3DThickness
                ZLbl.Origin = New Point3D(0, 0, LenVPZ + 0.5)
                ZLbl.Font = Font.Roman
                ZLbl.FontSize = 0.5
                VisualCorLines.Children.Add(ZLbl)
            End If

            Dim DivX As Integer = (LenVPX \ LenVPScale)
            Dim DivChangeVPX As Double = LenVPX / DivX

            For i = -DivX To DivX
                Dim Value As Single
                Value = (Len3DX * i)
                If i <> 0 Then
                    If CorDetail >= 2 Then
                        Dim P1 As New Point3D((DivChangeVPX * i), -0.1, 0)
                        Dim P2 As New Point3D((DivChangeVPX * i), 0.1, 0)
                        Dim PText As New Point3D((DivChangeVPX * i), 0.3, 0)
                        Dim VectO As New Vector3D(1, 0, 0) 'VectorOver
                        DrawLine3D(Colors.Blue, P1, P2)

                        If CorDetail = 3 Then
                            If Labels3DMode = Labels3D.Space Then
                                DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Blue)
                            ElseIf Labels3DMode = Labels3D.Overlay Then
                                DrawText3D_Overlay(PText, Value.ToString, Colors.Blue)
                            End If
                        End If
                    End If
                End If
            Next

            Dim DivY As Integer = (LenVPY \ LenVPScale)
            Dim DivChangeVPY As Double = LenVPY / DivY

            For i = -DivY To DivY
                Dim Value As Single
                Value = (Len3DY * i)
                If Value <> 0 Then
                    If CorDetail >= 2 Then
                        Dim P1 As New Point3D(-0.1, (DivChangeVPY * i), 0)
                        Dim P2 As New Point3D(0.1, (DivChangeVPY * i), 0)
                        Dim PText As New Point3D(0.3, (DivChangeVPY * i), 0)
                        Dim VectO As New Vector3D(1, 0, 0) 'VectorOver
                        DrawLine3D(Colors.Red, P1, P2)

                        If CorDetail = 3 Then
                            If Labels3DMode = Labels3D.Space Then
                                DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Red)
                            ElseIf Labels3DMode = Labels3D.Overlay Then
                                DrawText3D_Overlay(PText, Value.ToString, Colors.Red)
                            End If
                        End If
                    End If
                End If
            Next

            Dim DivZ As Integer = (LenVPZ \ LenVPScale)
            Dim DivChangeVPZ As Double = LenVPZ / DivZ

            For i = -DivZ To DivZ
                Dim Value As Single
                Value = (Len3DZ * i)
                If Value <> 0 Then
                    If CorDetail >= 2 Then
                        Dim P1 As New Point3D(0, -0.1, (DivChangeVPZ * i))
                        Dim P2 As New Point3D(0, 0.1, (DivChangeVPZ * i))
                        Dim PText As New Point3D(0, 0.3, (DivChangeVPZ * i))
                        Dim VectO As New Vector3D(0, 0, 1) 'VectorOver
                        DrawLine3D(Colors.Green, P1, P2)

                        If CorDetail = 3 Then
                            If Labels3DMode = Labels3D.Space Then
                                DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Green)
                            ElseIf Labels3DMode = Labels3D.Overlay Then
                                DrawText3D_Overlay(PText, Value.ToString, Colors.Green)
                            End If
                        End If
                    End If
                End If
            Next

        End If

        UpdateOverlay()

    End Sub
    Public Property Moving3DTimer As New DispatcherTimer
    Public Sub DrawCoordinateCube()
        Dim P1 As New Point3D(-LenVPX, -LenVPY, LenVPZ)
        Dim P2 As New Point3D(LenVPX, -LenVPY, LenVPZ)
        Dim P3 As New Point3D(LenVPX, LenVPY, LenVPZ)
        Dim P4 As New Point3D(-LenVPX, LenVPY, LenVPZ)
        Dim P5 As New Point3D(-LenVPX, -LenVPY, -LenVPZ)
        Dim P6 As New Point3D(LenVPX, -LenVPY, -LenVPZ)
        Dim P7 As New Point3D(LenVPX, LenVPY, -LenVPZ)
        Dim P8 As New Point3D(-LenVPX, LenVPY, -LenVPZ)

        Dim W21 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P2, .Point2 = P1}
        Dim W23 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P2, .Point2 = P3}
        Dim W26 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P2, .Point2 = P6}
        Dim W41 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P4, .Point2 = P1}
        Dim W43 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P4, .Point2 = P3}
        Dim W48 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P4, .Point2 = P8}
        Dim W51 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P5, .Point2 = P1}
        Dim W56 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P5, .Point2 = P6}
        Dim W58 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P5, .Point2 = P8}
        Dim W73 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P7, .Point2 = P3}
        Dim W76 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P7, .Point2 = P6}
        Dim W78 As New WireLine With {.Color = Colors.Black, .Thickness = 1.5, .Point1 = P7, .Point2 = P8}

        VisualCorCube.Children.Add(W21)
        VisualCorCube.Children.Add(W23)
        VisualCorCube.Children.Add(W26)
        VisualCorCube.Children.Add(W41)
        VisualCorCube.Children.Add(W43)
        VisualCorCube.Children.Add(W48)
        VisualCorCube.Children.Add(W51)
        VisualCorCube.Children.Add(W56)
        VisualCorCube.Children.Add(W58)
        VisualCorCube.Children.Add(W73)
        VisualCorCube.Children.Add(W76)
        VisualCorCube.Children.Add(W78)

    End Sub
    'رسم نمودار 
    Private Sub DrawGraph3D(Equation As Equation)
        'Try
        EquationTools.DetectEqType(Equation, Me.DrawMode)
        If Equation.Type <> EquationType.ParametricU Then
            For Each PP As Point3DPlane In GetGraphSurface3D(Equation, True)
                Dim MatGroup As New MaterialGroup
                'TODO : Must Fix
                Select ColoringMethod
                    Case 0
                        Select Case Equation.Material

                            Case PlaneMaterial.Diffuse
                                Dim MatD As New DiffuseMaterial(Equation.Brush)
                                MatGroup.Children.Add(MatD)

                            Case PlaneMaterial.Specular
                                Dim MatD As New DiffuseMaterial(Equation.Brush)
                                Dim MatS As New SpecularMaterial(Equation.Brush, 30)
                                MatGroup.Children.Add(MatD)
                                MatGroup.Children.Add(MatS)

                            Case PlaneMaterial.Emissive
                                Dim MatD As New DiffuseMaterial(Equation.Brush)
                                Dim MatE As New EmissiveMaterial(Equation.Brush)
                                MatGroup.Children.Add(MatD)
                                MatGroup.Children.Add(MatE)

                            Case PlaneMaterial.ByCor
                                Dim MatB As New DiffuseMaterial(New ImageBrush(CreateMeshBrush(PP)))
                                MatGroup.Children.Add(MatB)

                        End Select

                        '        Dim MatD As New DiffuseMaterial(Equation.Brush)
                        '        'Dim MatE As New EmissiveMaterial(Equation.Brush)
                        '        MatGroup.Children.Add(MatD)
                        '        If Equation.Material = PlaneMaterial.Specular Then
                        '            Dim MatS As New SpecularMaterial(Equation.Brush, 30)
                        '            MatGroup.Children.Add(MatS)
                        '        End If
                    Case 1
                        Dim MatB As New DiffuseMaterial(New ImageBrush(CreateMeshBrush(PP)))
                        MatGroup.Children.Add(MatB)
                End Select
                

                Dim MeshF As MeshGeometry3D = CreateMeshFront(PP)
                Dim MeshB As MeshGeometry3D = CreateMeshBack(PP)
                Dim GeoF As New GeometryModel3D(MeshF, MatGroup)
                Dim GeoB As New GeometryModel3D(MeshB, MatGroup)
                Equation.GraphObject.Add(GeoF)
                Equation.GraphObject.Add(GeoB)
                VisualModel.Children.Add(GeoF)
                VisualModel.Children.Add(GeoB)
                Equation.NeedsRecalculation = False
            Next
        Else
            'TODO : Fix Coloring Method
            For Each PC As Point3DCollection In GetGraphLine3D(Equation)
                Dim MatGroup As New MaterialGroup
                'Select Case ColoringMethod
                '    Case 0
                Dim SCBrush As New SolidColorBrush
                If Equation.Brush.GetType Is GetType(SolidColorBrush) Then
                    SCBrush = Equation.Brush
                Else
                    Equation.HasError = True
                    Equation.ErrorMsg = "لطفا از قلمو ی رنگ جامد استفاده کنید."
                End If

                Dim WPolyLine As New WirePolyline
                WPolyLine.Color = SCBrush.Color
                WPolyLine.Thickness = Equation.Thickness
                WPolyLine.Points = PC

                'Case 1
                '    Dim MatB As New DiffuseMaterial(New ImageBrush(CreateMeshBrush(PP)))
                '    MatGroup.Children.Add(MatB)

                VisualWire.Children.Add(WPolyLine)
                'Equation.NeedsRecalculation = False
            Next
        End If
        'Catch Ex As Exception
        '    Equation.HasError = True
        '    Equation.ErrorMsg = "خطا در رسم" & vbCrLf & Ex.Message
        'End Try
    End Sub
    Private Sub DrawGraphWireframe3D(Equation As Equation)
        Try
            EquationTools.DetectEqType(Equation, Me.DrawMode)
            For Each PP As Point3DPlane In GetGraphSurface3D(Equation)
                Dim SCBrush As New SolidColorBrush
                If Equation.Brush.GetType Is GetType(SolidColorBrush) Then
                    SCBrush = Equation.Brush
                    CreateWireFrame(PP, SCBrush.Color)
                Else
                    Equation.HasError = True
                    Equation.ErrorMsg = "در حالت ورفرم فقط قلمو های رنگ جامد قابل قبول هستند."
                End If
            Next
        Catch Ex As Exception
            Equation.HasError = True
            Equation.ErrorMsg = "خطا در رسم" & vbCrLf & Ex.Message
        End Try
    End Sub
    Private Sub DrawGraphPointModel3D(Equation As Equation)
        Try
            EquationTools.DetectEqType(Equation, Me.DrawMode)
            For Each PP As Point3DPlane In GetGraphSurface3D(Equation)
                For Each P3D As Point3D In PP.PointCollection
                    Dim MB As New MeshBuilder
                    MB.AddSphere(New Point3D(P3D.X, P3D.Y, P3D.Z), 0.05, 4, 2)

                    Dim PMBrush As SolidColorBrush
                    If Equation.Brush.GetType Is GetType(SolidColorBrush) Then
                        PMBrush = Equation.Brush
                    Else
                        Equation.HasError = True
                        Equation.ErrorMsg = "در حالت مدل نقطه ای فقط قلمو های رنگ جامد قابل قبول هستند."
                        Exit For
                    End If

                    Dim MatGroup As New MaterialGroup
                    Dim MatD As New DiffuseMaterial(PMBrush)
                    Dim MatE As New EmissiveMaterial(PMBrush)
                    Dim MatS As New SpecularMaterial(PMBrush, 30)
                    MatGroup.Children.Add(MatD)
                    MatGroup.Children.Add(MatS)
                    Dim Geo As New GeometryModel3D(MB.ToMesh, MatGroup)
                    Equation.GraphObject.Add(Geo)
                    VisualModel.Children.Add(Geo)
                    Equation.NeedsRecalculation = False
                Next
            Next
        Catch Ex As Exception
            Equation.HasError = True
            Equation.ErrorMsg = "خطا در رسم" & vbCrLf & Ex.Message
        End Try
    End Sub

    Private Function GetGraphSurface3D(ByVal Equation As Equation, Optional HighACC As Boolean = False) As List(Of Point3DPlane)

        'Cartesian
        'Type = 1  y=f(x,z)
        'Type = 2  x=f(y,z)
        'Type = 3  z=f(x,y)

        'Polar
        'Type = 4  r=f(t,p)
        'Type = 5  t=f(r,p)
        'Type = 6  p=f(r,t)

        'Parametric Surface 3D
        'Type = 9  x=f(u,v)  y=f(u,v)  z=f(u,v)

        Dim ValidTypes() As EquationType = {EquationType.CartesianY, EquationType.CartesianX, EquationType.CartesianZ, EquationType.PolarR, EquationType.PolarT, EquationType.PolarP, EquationType.ParametricUV}
        If Not ValidTypes.Contains(Equation.Type) Then
            Return New List(Of Point3DPlane)
        End If

        Dim PlanesList As New List(Of Point3DPlane)
        Dim AccValue As Double = (1 / Acc3D) * Len3DX
        If HighACC Then AccValue /= 2

        Dim DivX As Integer = (LenVPX \ LenVPScale)
        Dim DivY As Integer = (LenVPY \ LenVPScale)
        Dim DivZ As Integer = (LenVPZ \ LenVPScale)
        Dim DivChangeVPX As Double = LenVPX / DivX
        Dim DivChangeVPY As Double = LenVPY / DivY
        Dim DivChangeVPZ As Double = LenVPZ / DivZ

        Dim SimplifySuccess As Boolean
        Dim EqMain As String = SimplifyFunction(Equation, SimplifySuccess)
        If SimplifySuccess = False Then Return New List(Of Point3DPlane)

        'جایگذاری متغییر ها
        For Each MOV As MathObject In MathObjList
            If MOV.Type = 1 Then
                Dim Var As Variable = MOV.Inner
                EqMain = EquationTools.Replace(EqMain, Var.Name, Var.Value)
            End If
        Next

        Select Case Equation.Type
            Case EquationType.CartesianY ' Y=f(x,z)
                Dim PP As New Point3DPlane
                ' Draw Based On x,z  - تابع دکارتی
                Dim DSX As Single
                Dim DEX As Single

                Dim DSZ As Single
                Dim DEZ As Single

                'Domain Of X
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSX = -(Len3DX * DivX)
                Else
                    DSX = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEX = +(Len3DX * DivX)
                Else
                    DEX = Equation.DomainEnd
                End If

                'Domain Of Z
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSZ = -(Len3DZ * DivZ)
                Else
                    DSZ = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEZ = +(Len3DZ * DivZ)
                Else
                    DEZ = Equation.DomainEnd
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim x, y, z As Single

                EqStr = EquationTools.Replace(EqMain, "x", DSX)
                EqStr = EquationTools.Replace(EqStr, "z", DSZ)

                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)
                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = EvalResult
                Equation.RangeMin = EvalResult

                Dim OldPoint As New Point3D(DSX * (DivChangeVPX / Len3DX), EvalResult * (DivChangeVPY / Len3DY), DSZ * (DivChangeVPZ / Len3DZ))

                Dim RowsCount As Integer = 0
                Dim ColumnsCount As Integer = 0

                For i = DSX To DEX Step AccValue

                    EqStr = EquationTools.Replace(EqMain, "x", MathTools.Round(i))

                    ColumnsCount += 1
                    RowsCount = 0

                    For j = DSZ To DEZ Step AccValue

                        Dim Eq2Str As String
                        Eq2Str = EquationTools.Replace(EqStr, "z", MathTools.Round(j))
                        Dim EvalSuccess As Boolean
                        EvalResult = AKPEval.Eval(Eq2Str, EvalSuccess)

                        If EvalSuccess Then
                            EvalResult = Val(EvalResult)
                            RowsCount += 1

                            x = i * (DivChangeVPX / Len3DX)
                            y = EvalResult * (DivChangeVPY / Len3DY)
                            z = j * (DivChangeVPZ / Len3DZ)

                            If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                            If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                            'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                            Dim IsInTheScreen As Boolean = True 'InTheScreen(x, y, z)

                            'بررسی پیوستگی نمودار
                            Dim IsConnected As Boolean = True

                            Dim NewPoint As New Point3D(x, y, z)
                            If PointsSpace(OldPoint, NewPoint) > 30 Then IsConnected = False
                            OldPoint = NewPoint

                            If IsInTheScreen Then
                                If IsConnected Then
                                    PP.PointCollection.Add(NewPoint)
                                Else
                                    PP.Columns = ColumnsCount
                                    PP.Rows = RowsCount
                                    If PP.PointCollection.Count > 0 Then PlanesList.Add(PP)
                                    ColumnsCount = 0
                                    RowsCount = 0
                                    PP = New Point3DPlane
                                End If
                            Else
                                RowsCount -= 1
                            End If
                        End If
                    Next
                Next
                PP.Columns = ColumnsCount
                PP.Rows = RowsCount
                PlanesList.Add(PP)

            Case EquationType.CartesianX 'X =f(y,z)
                Dim PP As New Point3DPlane
                ' Draw Based On y,z  - تابع دکارتی
                Dim DSY As Single
                Dim DEY As Single

                Dim DSZ As Single
                Dim DEZ As Single

                'Domain Of Y
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSY = -(Len3DY * DivY)
                Else
                    DSY = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEY = +(Len3DY * DivY)
                Else
                    DEY = Equation.DomainEnd
                End If

                'Domain Of Z
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSZ = -(Len3DZ * DivZ)
                Else
                    DSZ = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEZ = +(Len3DZ * DivZ)
                Else
                    DEZ = Equation.DomainEnd
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim x, y, z As Single

                EqStr = EquationTools.Replace(EqMain, "y", DSY)
                EqStr = EquationTools.Replace(EqStr, "z", DSZ)

                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)
                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = EvalResult
                Equation.RangeMin = EvalResult

                Dim OldPoint As New Point3D(EvalResult * (DivChangeVPX / Len3DX), DSY * (DivChangeVPY / Len3DY), DSZ * (DivChangeVPZ / Len3DZ))

                Dim RowsCount As Integer = 0
                Dim ColumnsCount As Integer = 0

                For i = DSY To DEY Step AccValue

                    EqStr = EquationTools.Replace(EqMain, "y", MathTools.Round(i))

                    ColumnsCount += 1
                    RowsCount = 0

                    For j = DSZ To DEZ Step AccValue

                        Dim Eq2Str As String
                        Eq2Str = EquationTools.Replace(EqStr, "z", MathTools.Round(j))
                        Dim EvalSuccess As Boolean
                        EvalResult = AKPEval.Eval(Eq2Str, EvalSuccess)

                        If EvalSuccess Then
                            EvalResult = Val(EvalResult)
                            RowsCount += 1

                            x = EvalResult * (DivChangeVPX / Len3DX)
                            y = i * (DivChangeVPY / Len3DY)
                            z = j * (DivChangeVPZ / Len3DZ)

                            If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                            If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                            'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                            Dim IsInTheScreen As Boolean = True 'InTheScreen(x, y, z)

                            'بررسی پیوستگی نمودار
                            Dim IsConnected As Boolean = True

                            Dim NewPoint As New Point3D(x, y, z)
                            If PointsSpace(OldPoint, NewPoint) > 30 Then IsConnected = False
                            OldPoint = NewPoint

                            If IsInTheScreen Then
                                If IsConnected Then
                                    PP.PointCollection.Add(NewPoint)
                                Else
                                    PP.Columns = ColumnsCount
                                    PP.Rows = RowsCount
                                    If PP.PointCollection.Count > 0 Then PlanesList.Add(PP)
                                    ColumnsCount = 0
                                    RowsCount = 0
                                    PP = New Point3DPlane
                                End If
                            Else
                                RowsCount -= 1
                            End If
                        End If
                    Next
                Next
                PP.Columns = ColumnsCount
                PP.Rows = RowsCount
                PlanesList.Add(PP)

            Case EquationType.CartesianZ ' Z=f(x,y)
                Dim PP As New Point3DPlane
                ' Draw Based On x,y  - تابع دکارتی
                Dim DSX As Single
                Dim DEX As Single

                Dim DSY As Single
                Dim DEY As Single

                'Domain Of X
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSX = -(Len3DX * DivX)
                Else
                    DSX = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEX = +(Len3DX * DivX)
                Else
                    DEX = Equation.DomainEnd
                End If

                'Domain Of Y
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSY = -(Len3DY * DivY)
                Else
                    DSY = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEY = +(Len3DY * DivY)
                Else
                    DEY = Equation.DomainEnd
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim x, y, z As Single

                EqStr = EquationTools.Replace(EqMain, "x", DSX)
                EqStr = EquationTools.Replace(EqStr, "y", DSY)

                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)
                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = EvalResult
                Equation.RangeMin = EvalResult

                Dim OldPoint As New Point3D(DSX * (DivChangeVPX / Len3DX), DSY * (DivChangeVPY / Len3DY), EvalResult * (DivChangeVPZ / Len3DZ))

                Dim RowsCount As Integer = 0
                Dim ColumnsCount As Integer = 0

                For i = DSX To DEX Step AccValue

                    EqStr = EquationTools.Replace(EqMain, "x", MathTools.Round(i))

                    ColumnsCount += 1
                    RowsCount = 0

                    For j = DSY To DEY Step AccValue

                        Dim Eq2Str As String
                        Eq2Str = EquationTools.Replace(EqStr, "y", MathTools.Round(j))
                        Dim EvalSuccess As Boolean
                        EvalResult = AKPEval.Eval(Eq2Str, EvalSuccess)

                        If EvalSuccess Then
                            EvalResult = Val(EvalResult)
                            RowsCount += 1

                            x = i * (DivChangeVPX / Len3DX)
                            y = j * (DivChangeVPY / Len3DY)
                            z = EvalResult * (DivChangeVPZ / Len3DZ)

                            If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                            If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                            'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                            Dim IsInTheScreen As Boolean = True 'InTheScreen(x, y, z)

                            'بررسی پیوستگی نمودار
                            Dim IsConnected As Boolean = True

                            Dim NewPoint As New Point3D(x, y, z)
                            If PointsSpace(OldPoint, NewPoint) > 30 Then IsConnected = False
                            OldPoint = NewPoint

                            If IsInTheScreen Then
                                If IsConnected Then
                                    PP.PointCollection.Add(NewPoint)
                                Else
                                    PP.Columns = ColumnsCount
                                    PP.Rows = RowsCount
                                    If PP.PointCollection.Count > 0 Then PlanesList.Add(PP)
                                    ColumnsCount = 0
                                    RowsCount = 0
                                    PP = New Point3DPlane
                                End If
                            Else
                                RowsCount -= 1
                            End If
                        End If
                    Next
                Next
                PP.Columns = ColumnsCount
                PP.Rows = RowsCount
                PlanesList.Add(PP)

            Case EquationType.PolarR  ' R=f(t,p)
                Dim PP As New Point3DPlane
                ' Draw Based On t,p  - قطبی
                Dim DST As Single
                Dim DET As Single

                Dim DSP As Single
                Dim DEP As Single

                'Domain Of T
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DST = 0
                Else
                    DST = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DET = 2 * PI
                Else
                    DET = Equation.DomainEnd
                End If

                'Domain Of P
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSP = 0
                Else
                    DSP = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEP = 2 * PI
                Else
                    DEP = Equation.DomainEnd
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim x, y, z, r, t, p As Single

                EqStr = EquationTools.Replace(EqMain, "t", DST)
                EqStr = EquationTools.Replace(EqStr, "p", DSP)

                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)
                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = EvalResult
                Equation.RangeMin = EvalResult

                Dim OldPoint As New Point3D((EvalResult * Sin(DST) * Cos(DSP)) * (DivChangeVPX / Len3DX), _
                                            (EvalResult * Sin(DST) * Sin(DSP)) * (DivChangeVPY / Len3DY), _
                                            (EvalResult * Cos(DST)) * (DivChangeVPZ / Len3DZ))

                Dim RowsCount As Integer = 0
                Dim ColumnsCount As Integer = 0

                For i = DST To DET Step AccValue

                    EqStr = EquationTools.Replace(EqMain, "t", i)

                    ColumnsCount += 1
                    RowsCount = 0

                    For j = DSP To DEP Step AccValue

                        Dim Eq2Str As String
                        Eq2Str = EquationTools.Replace(EqStr, "p", j)
                        Dim EvalSuccess As Boolean
                        EvalResult = AKPEval.Eval(Eq2Str, EvalSuccess)

                        If EvalSuccess Then
                            EvalResult = Val(EvalResult)
                            RowsCount += 1

                            r = EvalResult
                            t = i
                            p = j

                            x = r * Sin(t) * Cos(p) * (DivChangeVPX / Len3DX)
                            y = r * Sin(t) * Sin(p) * (DivChangeVPY / Len3DY)
                            z = r * Cos(t) * (DivChangeVPZ / Len3DZ)

                            If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                            If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                            'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                            Dim IsInTheScreen As Boolean = True 'InTheScreen(x, y, z)

                            'بررسی پیوستگی نمودار
                            Dim IsConnected As Boolean = True

                            Dim NewPoint As New Point3D(x, y, z)
                            If PointsSpace(OldPoint, NewPoint) > 30 Then IsConnected = False
                            OldPoint = NewPoint

                            If IsInTheScreen Then
                                If IsConnected Then
                                    PP.PointCollection.Add(NewPoint)
                                Else
                                    PP.Columns = ColumnsCount
                                    PP.Rows = RowsCount
                                    If PP.PointCollection.Count > 0 Then PlanesList.Add(PP)
                                    ColumnsCount = 0
                                    RowsCount = 0
                                    PP = New Point3DPlane
                                End If
                            Else
                                RowsCount -= 1
                            End If
                        End If
                    Next
                Next
                PP.Columns = ColumnsCount
                PP.Rows = RowsCount
                PlanesList.Add(PP)

            Case EquationType.PolarT   ' T = f(r,p)
                Dim PP As New Point3DPlane
                ' Draw Based On r,p  - قطبی
                Dim DSR As Single
                Dim DER As Single

                Dim DSP As Single
                Dim DEP As Single

                'Domain Of R
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSR = 0
                Else
                    DSR = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DER = 2 * PI
                Else
                    DER = Equation.DomainEnd
                End If

                'Domain Of P
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSP = 0
                Else
                    DSP = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEP = 2 * PI
                Else
                    DEP = Equation.DomainEnd
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim x, y, z, r, t, p As Single

                EqStr = EquationTools.Replace(EqMain, "r", DSR)
                EqStr = EquationTools.Replace(EqStr, "p", DSP)

                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)
                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = EvalResult
                Equation.RangeMin = EvalResult

                Dim OldPoint As New Point3D((DSR * Sin(EvalResult) * Cos(DSP)) * (DivChangeVPX / Len3DX), _
                                            (DSR * Sin(EvalResult) * Sin(DSP)) * (DivChangeVPY / Len3DY), _
                                            (DSR * Cos(EvalResult)) * (DivChangeVPZ / Len3DZ))

                Dim RowsCount As Integer = 0
                Dim ColumnsCount As Integer = 0

                For i = DSR To DER Step AccValue

                    EqStr = EquationTools.Replace(EqMain, "r", i)

                    ColumnsCount += 1
                    RowsCount = 0

                    For j = DSP To DEP Step AccValue

                        Dim Eq2Str As String
                        Eq2Str = EquationTools.Replace(EqStr, "p", j)
                        Dim EvalSuccess As Boolean
                        EvalResult = AKPEval.Eval(Eq2Str, EvalSuccess)

                        If EvalSuccess Then
                            EvalResult = Val(EvalResult)
                            RowsCount += 1

                            r = i
                            t = EvalResult
                            p = j

                            x = r * Sin(t) * Cos(p) * (DivChangeVPX / Len3DX)
                            y = r * Sin(t) * Sin(p) * (DivChangeVPY / Len3DY)
                            z = r * Cos(t) * (DivChangeVPZ / Len3DZ)

                            If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                            If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                            'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                            Dim IsInTheScreen As Boolean = True 'InTheScreen(x, y, z)

                            'بررسی پیوستگی نمودار
                            Dim IsConnected As Boolean = True

                            Dim NewPoint As New Point3D(x, y, z)
                            If PointsSpace(OldPoint, NewPoint) > 30 Then IsConnected = False
                            OldPoint = NewPoint

                            If IsInTheScreen Then
                                If IsConnected Then
                                    PP.PointCollection.Add(NewPoint)
                                Else
                                    PP.Columns = ColumnsCount
                                    PP.Rows = RowsCount
                                    If PP.PointCollection.Count > 0 Then PlanesList.Add(PP)
                                    ColumnsCount = 0
                                    RowsCount = 0
                                    PP = New Point3DPlane
                                End If
                            Else
                                RowsCount -= 1
                            End If
                        End If
                    Next
                Next
                PP.Columns = ColumnsCount
                PP.Rows = RowsCount
                PlanesList.Add(PP)

            Case EquationType.PolarP   ' P = f(r,t)
                Dim PP As New Point3DPlane
                ' Draw Based On r,t  - قطبی
                Dim DSR As Single
                Dim DER As Single

                Dim DST As Single
                Dim DET As Single

                'Domain Of R
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSR = 0
                Else
                    DSR = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DER = 2 * PI
                Else
                    DER = Equation.DomainEnd
                End If

                'Domain Of T
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DST = 0
                Else
                    DST = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DET = 2 * PI
                Else
                    DET = Equation.DomainEnd
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim x, y, z, r, t, p As Single

                EqStr = EquationTools.Replace(EqMain, "r", DSR)
                EqStr = EquationTools.Replace(EqStr, "t", DST)

                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)
                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = EvalResult
                Equation.RangeMin = EvalResult

                Dim OldPoint As New Point3D((DSR * Sin(DST) * Cos(EvalResult)) * (DivChangeVPX / Len3DX), _
                                            (DSR * Sin(DST) * Sin(EvalResult)) * (DivChangeVPY / Len3DY), _
                                            (DSR * Cos(DST)) * (DivChangeVPZ / Len3DZ))

                Dim RowsCount As Integer = 0
                Dim ColumnsCount As Integer = 0

                For i = DSR To DER Step AccValue

                    EqStr = EquationTools.Replace(EqMain, "r", i)

                    ColumnsCount += 1
                    RowsCount = 0

                    For j = DST To DET Step AccValue

                        Dim Eq2Str As String
                        Eq2Str = EquationTools.Replace(EqStr, "t", j)
                        Dim EvalSuccess As Boolean
                        EvalResult = AKPEval.Eval(Eq2Str, EvalSuccess)

                        If EvalSuccess Then
                            EvalResult = Val(EvalResult)
                            RowsCount += 1

                            r = i
                            t = j
                            p = EvalResult

                            x = r * Sin(t) * Cos(p) * (DivChangeVPX / Len3DX)
                            y = r * Sin(t) * Sin(p) * (DivChangeVPY / Len3DY)
                            z = r * Cos(t) * (DivChangeVPZ / Len3DZ)

                            If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                            If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                            'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                            Dim IsInTheScreen As Boolean = True 'InTheScreen(x, y, z)

                            'بررسی پیوستگی نمودار
                            Dim IsConnected As Boolean = True

                            Dim NewPoint As New Point3D(x, y, z)
                            If PointsSpace(OldPoint, NewPoint) > 30 Then IsConnected = False
                            OldPoint = NewPoint

                            If IsInTheScreen Then
                                If IsConnected Then
                                    PP.PointCollection.Add(NewPoint)
                                Else
                                    PP.Columns = ColumnsCount
                                    PP.Rows = RowsCount
                                    If PP.PointCollection.Count > 0 Then PlanesList.Add(PP)
                                    ColumnsCount = 0
                                    RowsCount = 0
                                    PP = New Point3DPlane
                                End If
                            Else
                                RowsCount -= 1
                            End If
                        End If
                    Next
                Next
                PP.Columns = ColumnsCount
                PP.Rows = RowsCount
                PlanesList.Add(PP)

            Case EquationType.ParametricUV   '3D Parametric :  x=f(u,v)   y=f(u,v)  z=f(u,v)
                Dim PP As New Point3DPlane

                Dim XParameter As String = EquationTools.GetParameter(Equation, "x")
                Dim YParameter As String = EquationTools.GetParameter(Equation, "y")
                Dim ZParameter As String = EquationTools.GetParameter(Equation, "z")

                'جایگذاری متغییر ها
                For Each MOV As MathObject In MathObjList
                    If MOV.Type = 1 Then
                        Dim Var As Variable = MOV.Inner
                        XParameter = EquationTools.Replace(XParameter, Var.Name, Var.Value)
                        YParameter = EquationTools.Replace(YParameter, Var.Name, Var.Value)
                        ZParameter = EquationTools.Replace(ZParameter, Var.Name, Var.Value)
                    End If
                Next

                Dim DSU As Single
                Dim DEU As Single

                Dim DSV As Single
                Dim DEV As Single

                'Domain Of U
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSU = -(Len3DX * DivX)
                Else
                    DSU = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEU = +(Len3DX * DivX)
                Else
                    DEU = Equation.DomainEnd
                End If

                'Domain Of V
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSV = -(Len3DX * DivX)
                Else
                    DSV = Equation.SecondDomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEV = +(Len3DX * DivX)
                Else
                    DEV = Equation.SecondDomainEnd
                End If

                Dim EqStrPX As String
                Dim EqStrPY As String
                Dim EqStrPZ As String
                Dim EvalResultPX As Double
                Dim EvalResultPY As Double
                Dim EvalResultPZ As Double
                Dim x, y, z As Single

                EqStrPX = EquationTools.Replace(XParameter, "u", DSU)
                EqStrPY = EquationTools.Replace(YParameter, "u", DSU)
                EqStrPZ = EquationTools.Replace(ZParameter, "u", DSU)

                EqStrPX = EquationTools.Replace(EqStrPX, "v", DSV)
                EqStrPY = EquationTools.Replace(EqStrPY, "v", DSV)
                EqStrPZ = EquationTools.Replace(EqStrPZ, "v", DSV)

                EvalResultPX = AKPEval.Eval(XParameter, New Boolean, True)
                EvalResultPX = Val(EvalResultPX)

                EvalResultPY = AKPEval.Eval(YParameter, New Boolean, True)
                EvalResultPY = Val(EvalResultPY)

                EvalResultPZ = AKPEval.Eval(ZParameter, New Boolean, True)
                EvalResultPZ = Val(EvalResultPZ)

                'اولین برد
                Equation.RangeMax = EvalResultPY
                Equation.RangeMin = EvalResultPY

                Dim OldPoint As New Point3D(EvalResultPX * (DivChangeVPX / Len3DX), EvalResultPY * (DivChangeVPY / Len3DY), EvalResultPZ * (DivChangeVPZ / Len3DZ))

                Dim RowsCount As Integer = 0
                Dim ColumnsCount As Integer = 0

                For u = DSU To DEU Step AccValue

                    EqStrPX = EquationTools.Replace(XParameter, "u", MathTools.Round(u))
                    EqStrPY = EquationTools.Replace(YParameter, "u", MathTools.Round(u))
                    EqStrPZ = EquationTools.Replace(ZParameter, "u", MathTools.Round(u))

                    ColumnsCount += 1
                    RowsCount = 0



                    For v = DSV To DEV Step AccValue

                        Dim Eq2StrPX As String
                        Dim Eq2StrPY As String
                        Dim Eq2StrPZ As String

                        Eq2StrPX = EquationTools.Replace(EqStrPX, "v", MathTools.Round(v))
                        Dim EvalSuccessPX As Boolean
                        EvalResultPX = AKPEval.Eval(Eq2StrPX, EvalSuccessPX)

                        Eq2StrPY = EquationTools.Replace(EqStrPY, "v", MathTools.Round(v))
                        Dim EvalSuccessPY As Boolean
                        EvalResultPY = AKPEval.Eval(Eq2StrPY, EvalSuccessPY)

                        Eq2StrPZ = EquationTools.Replace(EqStrPZ, "v", MathTools.Round(v))
                        Dim EvalSuccessPZ As Boolean
                        EvalResultPZ = AKPEval.Eval(Eq2StrPZ, EvalSuccessPZ)

                        Dim EvalSuccess As Boolean = EvalSuccessPX And EvalSuccessPY And EvalSuccessPZ

                        If EvalSuccess Then

                            'EvalResultPX = Val(EvalResult)
                            RowsCount += 1

                            x = EvalResultPX * (DivChangeVPX / Len3DX)
                            y = EvalResultPY * (DivChangeVPY / Len3DY)
                            z = EvalResultPZ * (DivChangeVPZ / Len3DZ)

                            If EvalResultPY > Equation.RangeMax Then Equation.RangeMax = EvalResultPY
                            If EvalResultPY < Equation.RangeMin Then Equation.RangeMin = EvalResultPY

                            'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                            Dim IsInTheScreen As Boolean = True 'InTheScreen(x, y, z)

                            'بررسی پیوستگی نمودار
                            Dim IsConnected As Boolean = True

                            Dim NewPoint As New Point3D(x, y, z)
                            If PointsSpace(OldPoint, NewPoint) > 30 Then IsConnected = False
                            OldPoint = NewPoint

                            If IsInTheScreen Then
                                If IsConnected Then
                                    PP.PointCollection.Add(NewPoint)
                                Else
                                    PP.Columns = ColumnsCount
                                    PP.Rows = RowsCount
                                    If PP.PointCollection.Count > 0 Then PlanesList.Add(PP)
                                    ColumnsCount = 0
                                    RowsCount = 0
                                    PP = New Point3DPlane
                                End If
                            Else
                                RowsCount -= 1
                            End If

                            ' MsgBox("Col : " & ColumnsCount & "  Row : " & RowsCount)

                        End If
                    Next
                Next
                PP.Columns = ColumnsCount
                PP.Rows = RowsCount
                PlanesList.Add(PP)

        End Select

        Return PlanesList
    End Function
    Private Function GetGraphLine3D(ByVal Equation As Equation) As List(Of Point3DCollection)

        'Parametric Line 3D
        'Type = 8  x=f(u)  y=f(u)  z=f(u)

        Dim ValidTypes() As Integer = {EquationType.ParametricU}
        If Not ValidTypes.Contains(Equation.Type) Then
            Return New List(Of Point3DCollection)
        End If

        Dim PointsList As New List(Of Point3DCollection)
        Dim AccValue As Double = (1 / Acc) * Len3DX / 2

        Dim DivX As Integer = (LenVPX \ LenVPScale)
        Dim DivY As Integer = (LenVPY \ LenVPScale)
        Dim DivZ As Integer = (LenVPZ \ LenVPScale)
        Dim DivChangeVPX As Double = LenVPX / DivX
        Dim DivChangeVPY As Double = LenVPY / DivY
        Dim DivChangeVPZ As Double = LenVPZ / DivZ

        Dim SimplifySuccess As Boolean
        Dim EqMain As String = SimplifyFunction(Equation, SimplifySuccess)
        If SimplifySuccess = False Then Return New List(Of Point3DCollection)

        Dim EqDC As Boolean = EqMain.Contains("int") _
              Or EqMain.Contains("floor") _
              Or EqMain.Contains("ceil") _
              Or EqMain.Contains("tan")

        Dim AllowedSpace As Double = 1

        Select Case Equation.Type
            Case EquationType.ParametricU
                Dim PC As New Point3DCollection

                Dim XParameter As String = EquationTools.GetParameter(Equation, "x")
                Dim YParameter As String = EquationTools.GetParameter(Equation, "y")
                Dim ZParameter As String = EquationTools.GetParameter(Equation, "z")

                'جایگذاری متغییر ها
                For Each MOV As MathObject In MathObjList
                    If MOV.Type = 1 Then
                        Dim Var As Variable = MOV.Inner
                        XParameter = EquationTools.Replace(XParameter, Var.Name, Var.Value)
                        YParameter = EquationTools.Replace(YParameter, Var.Name, Var.Value)
                        ZParameter = EquationTools.Replace(ZParameter, Var.Name, Var.Value)
                    End If
                Next

                Dim DS As Single
                Dim DE As Single

                If Equation.DomainStart = Double.NegativeInfinity Then
                    DS = -(Len3DX * DivX)
                Else
                    DS = Val(Equation.DomainStart)
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DE = +(Len3DX * DivX)
                Else
                    DE = Val(Equation.DomainEnd)
                End If

                Dim EqStrPX As String
                Dim EqStrPY As String
                Dim EqStrPZ As String
                Dim EvalResultPX As Double
                Dim EvalResultPY As Double
                Dim EvalResultPZ As Double
                Dim x, y, z As Single

                EqStrPX = EquationTools.Replace(XParameter, "u", DS)
                EqStrPY = EquationTools.Replace(YParameter, "u", DS)
                EqStrPZ = EquationTools.Replace(ZParameter, "u", DS)

                EvalResultPX = AKPEval.Eval(EqStrPX, New Boolean, True)
                EvalResultPX = Val(EvalResultPX)

                EvalResultPY = AKPEval.Eval(EqStrPY, New Boolean, True)
                EvalResultPY = Val(EvalResultPY)

                EvalResultPZ = AKPEval.Eval(EqStrPZ, New Boolean, True)
                EvalResultPZ = Val(EvalResultPZ)

                'اولین برد
                Equation.RangeMax = EvalResultPY
                Equation.RangeMin = EvalResultPY

                Dim OldPoint As New Point3D(EvalResultPX * (DivChangeVPX / Len3DX), EvalResultPY * (DivChangeVPY / Len3DY), EvalResultPZ * (DivChangeVPZ / Len3DZ))
                'Dim OldPoint As New Point(CenterPoint.X + EvalResultPX * (AW / (LenX * 2)), CenterPoint.Y - EvalResultPY * (AH / (LenY * 2)))

                For u = DS To DE Step AccValue

                    EqStrPX = EquationTools.Replace(XParameter, "u", MathTools.Round(u))
                    Dim EvalSuccessPX As Boolean
                    EvalResultPX = AKPEval.Eval(EqStrPX, EvalSuccessPX)

                    EqStrPY = EquationTools.Replace(YParameter, "u", MathTools.Round(u))
                    Dim EvalSuccessPY As Boolean
                    EvalResultPY = AKPEval.Eval(EqStrPY, EvalSuccessPY)

                    EqStrPZ = EquationTools.Replace(ZParameter, "u", MathTools.Round(u))
                    Dim EvalSuccessPZ As Boolean
                    EvalResultPZ = AKPEval.Eval(EqStrPZ, EvalSuccessPZ)

                    Dim EvalSuccess As Boolean = EvalSuccessPX And EvalSuccessPY And EvalSuccessPZ

                    If EvalSuccess Then
                        EvalResultPX = Val(EvalResultPX)
                        EvalResultPY = Val(EvalResultPY)
                        EvalResultPZ = Val(EvalResultPZ)

                        x = EvalResultPX * (DivChangeVPX / Len3DX)
                        y = EvalResultPY * (DivChangeVPY / Len3DY)
                        z = EvalResultPZ * (DivChangeVPZ / Len3DZ)

                        If EvalResultPY > Equation.RangeMax Then Equation.RangeMax = EvalResultPY
                        If EvalResultPY < Equation.RangeMin Then Equation.RangeMin = EvalResultPY

                        'بررسی پیوستگی نمودار
                        Dim IsConnected As Boolean = True

                        Dim NewPoint As New Point3D(x, y, z)
                        If EqDC And PointsSpace(OldPoint, NewPoint) > AllowedSpace Then IsConnected = False
                        OldPoint = NewPoint

                        If IsConnected Then
                            PC.Add(NewPoint)
                        Else
                            PointsList.Add(PC)
                            PC = New Point3DCollection
                            PC.Add(NewPoint)
                        End If
                    End If
                Next
                PointsList.Add(PC)

        End Select

        Return PointsList
    End Function

    'نوع سطح 3 بعدی
    Private m_ActivePlaneMode As PlaneMode = PlaneMode.Normal
    Public Property ActivePlaneMode As PlaneMode
        Get
            Return m_ActivePlaneMode
        End Get
        Set(value As PlaneMode)
            m_ActivePlaneMode = value
            NotifyPropertyChanged("ActivePlaneMode")
        End Set
    End Property
    Public Enum PlaneMode
        Normal
        Wireframe
        PointModel
        DebugMode
    End Enum
    Public Sub SwitchAPM()
        If ActivePlaneMode = PlaneMode.Normal Then
            ActivePlaneMode = PlaneMode.Wireframe
        ElseIf ActivePlaneMode = PlaneMode.Wireframe Then
            ActivePlaneMode = PlaneMode.PointModel
        ElseIf ActivePlaneMode = PlaneMode.PointModel Then
            ActivePlaneMode = PlaneMode.Normal
        End If
        IsCor3DModified = True
        AutoRedraw()
    End Sub

#End Region

#Region "ابزار های رسم 3 بعدی"

    'خط 3 بعدی
    Private Sub DrawLine3D(Color As Color, P3D1 As Point3D, P3D2 As Point3D, Optional Thk As Integer = 1)
        Dim WLine As New WireLine

        WLine.Color = Color
        WLine.Thickness = Thk
        WLine.Point1 = P3D1
        WLine.Point2 = P3D2

        VisualCorLines.Children.Add(WLine)
    End Sub

    'متن 3 بعدی فضایی
    Private Sub DrawText3D_Space(P3D As Point3D, VectorOver As Vector3D, VectorUp As Vector3D, Text As String, _
                          Color As Color, Optional IsCenterPoint As Boolean = True, Optional IsDoubleSided As Boolean = True)

        Dim Textblock As New TextBlock(New Run(Text))
        Textblock.Background = Brushes.Transparent
        Textblock.Foreground = New SolidColorBrush(Color)
        Textblock.FontFamily = New FontFamily("Arial")

        'Dim Width As Double = Text.Length * Height
        Dim TS As Size = MeasureTextSize(Text)
        Dim TH As Double = TS.Height / 45
        Dim TW As Double = TS.Width / 45
        Textblock.Height = TS.Height
        Textblock.Width = TS.Width

        Dim MaterialWithLabel As New DiffuseMaterial()
        MaterialWithLabel.Brush = New VisualBrush(Textblock)

        ' p0: گوشه چپ پایین     p1: گوشه چپ بالا
        ' p2: گوشه راست پایین   p3: گوشه راست بالا
        Dim p0 As Point3D = P3D
        If IsCenterPoint Then
            p0 = P3D - TW / 2 * VectorOver - TH / 2 * VectorUp
        End If
        Dim p1 As Point3D = p0 + VectorUp * TH
        Dim p2 As Point3D = p0 + VectorOver * TW
        Dim p3 As Point3D = p0 + VectorUp * TH + VectorOver * TW

        Dim m_3DRect As New MeshGeometry3D()
        m_3DRect.Positions = New Point3DCollection()
        m_3DRect.Positions.Add(p0) '0
        m_3DRect.Positions.Add(p1) '
        m_3DRect.Positions.Add(p2) '2
        m_3DRect.Positions.Add(p3) '3
        If IsDoubleSided Then
            m_3DRect.Positions.Add(p0) '4
            m_3DRect.Positions.Add(p1) '5
            m_3DRect.Positions.Add(p2) '6
            m_3DRect.Positions.Add(p3) '7
        End If

        m_3DRect.TriangleIndices.Add(0)
        m_3DRect.TriangleIndices.Add(3)
        m_3DRect.TriangleIndices.Add(1)
        m_3DRect.TriangleIndices.Add(0)
        m_3DRect.TriangleIndices.Add(2)
        m_3DRect.TriangleIndices.Add(3)
        If IsDoubleSided Then
            m_3DRect.TriangleIndices.Add(4)
            m_3DRect.TriangleIndices.Add(5)
            m_3DRect.TriangleIndices.Add(7)
            m_3DRect.TriangleIndices.Add(4)
            m_3DRect.TriangleIndices.Add(7)
            m_3DRect.TriangleIndices.Add(6)
        End If

        m_3DRect.TextureCoordinates.Add(New Point(0, 1))
        m_3DRect.TextureCoordinates.Add(New Point(0, 0))
        m_3DRect.TextureCoordinates.Add(New Point(1, 1))
        m_3DRect.TextureCoordinates.Add(New Point(1, 0))
        If IsDoubleSided Then
            m_3DRect.TextureCoordinates.Add(New Point(1, 1))
            m_3DRect.TextureCoordinates.Add(New Point(1, 0))
            m_3DRect.TextureCoordinates.Add(New Point(0, 1))
            m_3DRect.TextureCoordinates.Add(New Point(0, 0))
        End If

        Dim Geometry As New GeometryModel3D(m_3DRect, MaterialWithLabel)
        Geometry.BackMaterial = New DiffuseMaterial(Brushes.Transparent)

        Dim Result As New ModelVisual3D()
        Result.Content = Geometry

        VisualCorLines.Children.Add(Result)
    End Sub
    'متن 3 بعدی پوششی
    Public Property OverlayPoints As New List(Of OverlayPoint)
    Private Sub DrawText3D_Overlay(P3D As Point3D, Text As String, Color As Color)
        Dim NOP As OverlayPoint = New OverlayPoint With {.Point = P3D, .Text = Text, .Color = Color}
        OverlayPoints.Add(NOP)
        UpdateOverlay()
    End Sub
    Private Sub UpdateOverlay()
        Cor3D_Overlay.Children.Clear()
        For Each OP As OverlayPoint In OverlayPoints
            Dim TB As New TextBlock(New Run(OP.Text))
            TB.FontWeight = FontWeights.SemiBold
            TB.Foreground = New SolidColorBrush(OP.Color)
            Dim P2D As Point = Point3DTools.Get2DPointTransformed(OP.Point, VisualCorLines, Cor3D)
            Canvas.SetTop(TB, P2D.Y)
            Canvas.SetLeft(TB, P2D.X)

            Cor3D_Overlay.Children.Add(TB)
        Next
    End Sub
    Private m_Labels3DMode As Labels3D = Labels3D.Space
    Public Property Labels3DMode As Labels3D
        Get
            Return m_Labels3DMode
        End Get
        Set(value As Labels3D)
            m_Labels3DMode = value
            NotifyPropertyChanged("Labels3DMode")
        End Set
    End Property
    Public Enum Labels3D
        Space
        Overlay
    End Enum
    Public Sub SwitchLT()
        If Labels3DMode = Labels3D.Space Then
            Labels3DMode = Labels3D.Overlay
        ElseIf Labels3DMode = Labels3D.Overlay Then
            Labels3DMode = Labels3D.Space
        End If
        DrawCoordinateSystem3D()
    End Sub

    'نقطه 3 بعدی
    Private Sub DrawPoint3D(PtV As APointViewer)

        Dim DivX As Integer = (LenVPX \ LenVPScale)
        Dim DivY As Integer = (LenVPY \ LenVPScale)
        Dim DivZ As Integer = (LenVPZ \ LenVPScale)
        Dim DivChangeVPX As Double = LenVPX / DivX
        Dim DivChangeVPY As Double = LenVPY / DivY
        Dim DivChangeVPZ As Double = LenVPZ / DivZ

        Dim Tx As String = PtV.Point.X
        Dim Ty As String = PtV.Point.Y
        Dim Tz As String = PtV.Point.Z

        Dim Rx As Double
        Dim Ry As Double
        Dim Rz As Double

        'جایگذاری متغییر ها
        For Each MOV As MathObject In MathObjList
            If MOV.Type = 1 Then
                Dim Var As Variable = MOV.Inner
                Tx = EquationTools.Replace(Tx.ToLower, Var.Name, Var.Value)
                Ty = EquationTools.Replace(Ty.ToLower, Var.Name, Var.Value)
                Tz = EquationTools.Replace(Tz.ToLower, Var.Name, Var.Value)
            End If
        Next
        Dim TxEvalSuccess As Boolean
        Dim TyEvalSuccess As Boolean
        Dim TzEvalSuccess As Boolean
        Tx = AKPEval.Eval(Tx, TxEvalSuccess)
        Ty = AKPEval.Eval(Ty, TyEvalSuccess)
        Tz = AKPEval.Eval(Tz, TzEvalSuccess)

        If IsNumeric(Tx) And TxEvalSuccess Then
            Rx = CDbl(Tx) * (DivChangeVPX / Len3DX)
        Else
            PtV.HasError = True
            PtV.ErrorMsg = Tx + " تعریف نشده است."
        End If
        If IsNumeric(Ty) And TyEvalSuccess Then
            Ry = CDbl(Ty) * (DivChangeVPY / Len3DY)
        Else
            PtV.HasError = True
            PtV.ErrorMsg = Ty + " تعریف نشده است."
        End If
        If IsNumeric(Tz) And TzEvalSuccess Then
            Rz = CDbl(Tz) * (DivChangeVPZ / Len3DZ)
        Else
            PtV.HasError = True
            PtV.ErrorMsg = Tz + " تعریف نشده است."
        End If

        Dim MB As New MeshBuilder(True, True)
        MB.AddSphere(New Point3D(Rx, Ry, Rz), 0.07)

        Dim MatGroup As New MaterialGroup
        Select Case ColoringMethod
            Case 0
                Dim MatD As New DiffuseMaterial(New SolidColorBrush(PtV.Color))
                Dim MatE As New EmissiveMaterial(New SolidColorBrush(PtV.Color))
                Dim MatS As New SpecularMaterial(New SolidColorBrush(PtV.Color), 30)
                MatGroup.Children.Add(MatD)
                MatGroup.Children.Add(MatS)
            Case 1
                Dim MatB As New DiffuseMaterial(New SolidColorBrush(PointToColor(New Point3D(Rx, Ry, Rz), 5, -5)))
                MatGroup.Children.Add(MatB)
        End Select

        Dim Geo As New GeometryModel3D(MB.ToMesh, MatGroup)
        PtV.GraphObject.Add(Geo)
        VisualModel.Children.Add(Geo)
    End Sub
    'بردار 3 بعدی
    Private Sub DrawVector3D(VectV As AVectorViewer)

        Dim DivX As Integer = (LenVPX \ LenVPScale)
        Dim DivY As Integer = (LenVPY \ LenVPScale)
        Dim DivZ As Integer = (LenVPZ \ LenVPScale)
        Dim DivChangeVPX As Double = LenVPX / DivX
        Dim DivChangeVPY As Double = LenVPY / DivY
        Dim DivChangeVPZ As Double = LenVPZ / DivZ

        Dim Tx As String = "0"
        Dim Ty As String = "0"
        Dim Tz As String = "0"

        'دریافت اطلاعات نقطه
        If VectV.PointName <> "" Then
            If VectV.PointName.Contains(",") Then 'مختصات داخلی
                Dim PNWP As String = VectV.PointName.Trim("(", ")")
                Dim SCor() As String = PNWP.Split(",")
                Try
                    Tx = If(SCor(0) <> "", SCor(0), "0")
                    Ty = If(SCor(1) <> "", SCor(1), "0")
                    Tz = If(SCor(2) <> "", SCor(2), "0")
                Catch
                    VectV.HasError = True
                    VectV.ErrorMsg = "لطفا مختصات نقطه مبدا را بصورت کامل وارد کنید"
                End Try
            Else 'جستجو برای مختصات خارجی
                For Each MOV As MathObject In MathObjList
                    If MOV.Type = 2 Then
                        Dim Pt As APointViewer = TryCast(MOV.Inner, APointViewer)
                        If Pt.Name = VectV.PointName Then
                            Tx = Pt.Point.X
                            Ty = Pt.Point.Y
                            Tz = Pt.Point.Z
                        End If
                    End If
                Next
            End If
        End If

        Dim Ti As String = VectV.Vector.X
        Dim Tj As String = VectV.Vector.Y
        Dim Tk As String = VectV.Vector.Z

        Dim Rx As Double
        Dim Ry As Double
        Dim Rz As Double

        Dim Ri As Double
        Dim Rj As Double
        Dim Rk As Double

        'جایگذاری متغییر ها
        For Each MOV As MathObject In MathObjList
            If MOV.Type = 1 Then
                Dim Var As Variable = MOV.Inner
                Tx = EquationTools.Replace(Tx.ToLower, Var.Name, Var.Value)
                Ty = EquationTools.Replace(Ty.ToLower, Var.Name, Var.Value)
                Tz = EquationTools.Replace(Tz.ToLower, Var.Name, Var.Value)

                Ti = EquationTools.Replace(Ti.ToLower, Var.Name, Var.Value)
                Tj = EquationTools.Replace(Tj.ToLower, Var.Name, Var.Value)
                Tk = EquationTools.Replace(Tk.ToLower, Var.Name, Var.Value)
            End If
        Next
        Dim TxEvalSuccess As Boolean
        Dim TyEvalSuccess As Boolean
        Dim TzEvalSuccess As Boolean
        Tx = AKPEval.Eval(Tx, TxEvalSuccess)
        Ty = AKPEval.Eval(Ty, TyEvalSuccess)
        Tz = AKPEval.Eval(Tz, TzEvalSuccess)

        Dim TiEvalSuccess As Boolean
        Dim TjEvalSuccess As Boolean
        Dim TkEvalSuccess As Boolean
        Ti = AKPEval.Eval(Ti, TiEvalSuccess)
        Tj = AKPEval.Eval(Tj, TjEvalSuccess)
        Tk = AKPEval.Eval(Tk, TkEvalSuccess)

        If IsNumeric(Tx) And TxEvalSuccess Then
            Rx = CDbl(Tx) * (DivChangeVPX / Len3DX)
        Else
            VectV.HasError = True
            VectV.ErrorMsg = Tx + " تعریف نشده است."
        End If
        If IsNumeric(Ty) And TyEvalSuccess Then
            Ry = CDbl(Ty) * (DivChangeVPY / Len3DY)
        Else
            VectV.HasError = True
            VectV.ErrorMsg = Ty + " تعریف نشده است."
        End If
        If IsNumeric(Tz) And TzEvalSuccess Then
            Rz = CDbl(Tz) * (DivChangeVPZ / Len3DZ)
        Else
            VectV.HasError = True
            VectV.ErrorMsg = Tz + " تعریف نشده است."
        End If


        If IsNumeric(Ti) And TiEvalSuccess Then
            Ri = CDbl(Ti) * (DivChangeVPX / Len3DX)
        Else
            VectV.HasError = True
            VectV.ErrorMsg = Ti + " تعریف نشده است."
        End If
        If IsNumeric(Tj) And TjEvalSuccess Then
            Rj = CDbl(Tj) * (DivChangeVPY / Len3DY)
        Else
            VectV.HasError = True
            VectV.ErrorMsg = Tj + " تعریف نشده است."
        End If
        If IsNumeric(Tk) And TkEvalSuccess Then
            Rk = CDbl(Tk) * (DivChangeVPZ / Len3DZ)
        Else
            VectV.HasError = True
            VectV.ErrorMsg = Tk + " تعریف نشده است."
        End If

        Dim Arrow As New WireLine
        Arrow.Color = VectV.Color
        Arrow.Thickness = 1
        Arrow.Point1 = New Point3D(Rx, Ry, Rz)
        Arrow.Point2 = New Point3D(Rx + Ri, Ry + Rj, Rz + Rk)
        Arrow.ArrowEnds = 2
        Arrow.ArrowLength = 10
        Arrow.ArrowAngle = 45
        VisualWire.Children.Add(Arrow)
    End Sub
    'شکل 3 بعدی
    Private Sub DrawShape3D(ByRef S As AShape)

        Dim DivX As Integer = (LenVPX \ LenVPScale)
        Dim DivY As Integer = (LenVPY \ LenVPScale)
        Dim DivZ As Integer = (LenVPZ \ LenVPScale)
        Dim DivChangeVPX As Double = LenVPX / DivX
        Dim DivChangeVPY As Double = LenVPY / DivY
        Dim DivChangeVPZ As Double = LenVPZ / DivZ

        Dim Ox As String = "0"
        Dim Oy As String = "0"
        Dim Oz As String = "0"

        'دریافت اطلاعات نقطه
        If S.PointName <> "" Then
            If S.PointName.Contains(",") Then 'مختصات داخلی
                Dim PNWP As String = S.PointName.Trim("(", ")")
                Dim SCor() As String = PNWP.Split(",")
                Try
                    Ox = If(SCor(0) <> "", SCor(0), "0")
                    Oy = If(SCor(1) <> "", SCor(1), "0")
                    Oz = If(SCor(2) <> "", SCor(2), "0")
                Catch
                    S.HasError = True
                    S.ErrorMsg = "لطفا مختصات نقطه مبدا را بصورت کامل وارد کنید"
                End Try
            Else 'جستجو برای مختصات خارجی
                For Each MOV As MathObject In MathObjList
                    If MOV.Type = 2 Then
                        Dim Pt As APointViewer = TryCast(MOV.Inner, APointViewer)
                        If Pt.Name = S.PointName Then
                            Ox = Pt.Point.X
                            Oy = Pt.Point.Y
                            Oz = Pt.Point.Z
                        End If
                    End If
                Next
            End If
        End If

        Dim Di As String = "0"
        Dim Dj As String = "1"
        Dim Dk As String = "0"

        'دریافت اطلاعات بردار
        If S.VectorName <> "" Then
            If S.VectorName.Contains(",") Then 'مختصات داخلی
                Dim VNWP As String = S.VectorName.Trim("(", ")")
                Dim SCor() As String = VNWP.Split(",")
                Try
                    Di = If(SCor(0) <> "", SCor(0), "0")
                    Dj = If(SCor(1) <> "", SCor(1), "0")
                    Dk = If(SCor(2) <> "", SCor(2), "0")
                Catch
                    S.HasError = True
                    S.ErrorMsg = "لطفا مختصات بردار جهت را بصورت کامل وارد کنید"
                End Try
            Else 'جستجو برای مختصات خارجی
                For Each MOV As MathObject In MathObjList
                    If MOV.Type = 3 Then
                        Dim Vt As AVectorViewer = TryCast(MOV.Inner, AVectorViewer)
                        If Vt.Name = S.VectorName Then
                            Di = Vt.Vector.X
                            Dj = Vt.Vector.Y
                            Dk = Vt.Vector.Z
                        End If
                    End If
                Next
            End If
        End If

        Dim ROx As Double
        Dim ROy As Double
        Dim ROz As Double

        Dim RDi As Double
        Dim RDj As Double
        Dim RDk As Double

        Dim Sl As String = S.Length
        Dim Sw As String = S.Width
        Dim Sh As String = S.Height
        Dim RSl As Double
        Dim RSw As Double
        Dim RSh As Double

        Dim Srx As String = S.Radius
        Dim Sry As String = S.Radius2
        Dim Srz As String = S.Radius3
        Dim RSrx As Double
        Dim RSry As Double
        Dim RSrz As Double

        'جایگذاری متغییر ها
        For Each MOV As MathObject In MathObjList
            If MOV.Type = 1 Then
                Dim Var As Variable = MOV.Inner
                Ox = EquationTools.Replace(Ox.ToLower, Var.Name, Var.Value)
                Oy = EquationTools.Replace(Oy.ToLower, Var.Name, Var.Value)
                Oz = EquationTools.Replace(Oz.ToLower, Var.Name, Var.Value)

                Di = EquationTools.Replace(Di.ToLower, Var.Name, Var.Value)
                Dj = EquationTools.Replace(Dj.ToLower, Var.Name, Var.Value)
                Dk = EquationTools.Replace(Dk.ToLower, Var.Name, Var.Value)

                Sl = EquationTools.Replace(Sl.ToLower, Var.Name, Var.Value)
                Sw = EquationTools.Replace(Sw.ToLower, Var.Name, Var.Value)
                Sh = EquationTools.Replace(Sh.ToLower, Var.Name, Var.Value)

                Srx = EquationTools.Replace(Srx.ToLower, Var.Name, Var.Value)
                Sry = EquationTools.Replace(Sry.ToLower, Var.Name, Var.Value)
                Srz = EquationTools.Replace(Srz.ToLower, Var.Name, Var.Value)
            End If
        Next

        Dim OxEvalSuccess As Boolean
        Dim OyEvalSuccess As Boolean
        Dim OzEvalSuccess As Boolean
        Ox = AKPEval.Eval(Ox, OxEvalSuccess)
        Oy = AKPEval.Eval(Oy, OyEvalSuccess)
        Oz = AKPEval.Eval(Oz, OzEvalSuccess)

        Dim DiEvalSuccess As Boolean
        Dim DjEvalSuccess As Boolean
        Dim DkEvalSuccess As Boolean
        Di = AKPEval.Eval(Di, DiEvalSuccess)
        Dj = AKPEval.Eval(Dj, DjEvalSuccess)
        Dk = AKPEval.Eval(Dk, DkEvalSuccess)

        If IsNumeric(Ox) And OxEvalSuccess Then
            ROx = CDbl(Ox) * (DivChangeVPX / Len3DX)
        Else
            S.HasError = True
            S.ErrorMsg += vbCrLf & "خطا در مختصات مبدا : " & vbCrLf & Ox & " تعریف نشده است."
        End If
        If IsNumeric(Oy) And OyEvalSuccess Then
            ROy = CDbl(Oy) * (DivChangeVPY / Len3DY)
        Else
            S.HasError = True
            S.ErrorMsg += vbCrLf & "خطا در مختصات مبدا : " & vbCrLf & Oy & " تعریف نشده است."
        End If
        If IsNumeric(Oz) And OzEvalSuccess Then
            ROz = CDbl(Oz) * (DivChangeVPZ / Len3DZ)
        Else
            S.HasError = True
            S.ErrorMsg += vbCrLf & "خطا در مختصات مبدا : " & vbCrLf & Oz & " تعریف نشده است."
        End If
        If IsNumeric(Di) And DiEvalSuccess Then
            RDi = CDbl(Di) * (DivChangeVPX / Len3DX)
        Else
            S.HasError = True
            S.ErrorMsg += vbCrLf & "خطا در مختصات بردار جهت : " & vbCrLf & Di & " تعریف نشده است."
        End If
        If IsNumeric(Dj) And DjEvalSuccess Then
            RDj = CDbl(Dj) * (DivChangeVPY / Len3DY)
        Else
            S.HasError = True
            S.ErrorMsg += vbCrLf & "خطا در مختصات بردار جهت : " & vbCrLf & Dj & " تعریف نشده است."
        End If
        If IsNumeric(Dk) And DkEvalSuccess Then
            RDk = CDbl(Dk) * (DivChangeVPZ / Len3DZ)
        Else
            S.HasError = True
            S.ErrorMsg += vbCrLf & "خطا در مختصات بردار جهت : " & vbCrLf & Dk & " تعریف نشده است."
        End If


        Dim SlEvalSuccess As Boolean
        Dim SwEvalSuccess As Boolean
        Dim ShEvalSuccess As Boolean
        If Sl <> "" Then
            Sl = AKPEval.Eval(Sl, SlEvalSuccess)
            If IsNumeric(Sl) And SlEvalSuccess Then
                RSl = CDbl(Sl) * (DivChangeVPX / Len3DX)
            Else
                S.HasError = True
                S.ErrorMsg += vbCrLf & "خطا در طول : " & vbCrLf & S.Length & " تعریف نشده است."
            End If
        End If
        If Sw <> "" Then
            Sw = AKPEval.Eval(Sw, SwEvalSuccess)
            If IsNumeric(Sw) And SwEvalSuccess Then
                RSw = CDbl(Sw) * (DivChangeVPY / Len3DY)
            Else
                S.HasError = True
                S.ErrorMsg += vbCrLf & "خطا در عرض : " & vbCrLf & S.Width & " تعریف نشده است."
            End If
        End If
        If Sh <> "" Then
            Sh = AKPEval.Eval(Sh, ShEvalSuccess)
            If IsNumeric(Sh) And ShEvalSuccess Then
                RSh = CDbl(Sh) * (DivChangeVPZ / Len3DZ)
            Else
                S.HasError = True
                S.ErrorMsg += vbCrLf & "خطا در ارتفاع : " & vbCrLf & S.Height & " تعریف نشده است."
            End If
        End If

        Dim SrxEvalSuccess As Boolean
        Dim SryEvalSuccess As Boolean
        Dim SrzEvalSuccess As Boolean
        If Srx <> "" Then
            Srx = AKPEval.Eval(Srx, SrxEvalSuccess)
            If IsNumeric(Srx) And SrxEvalSuccess Then
                RSrx = CDbl(Srx) * (DivChangeVPX / Len3DX)
            Else
                S.HasError = True
                S.ErrorMsg += vbCrLf & "خطا در شعاع : " & vbCrLf & S.Radius & " تعریف نشده است."
            End If
        End If
        If Sry <> "" Then
            Sry = AKPEval.Eval(Sry, SryEvalSuccess)
            If IsNumeric(Sry) And SryEvalSuccess Then
                RSry = CDbl(Sry) * (DivChangeVPY / Len3DY)
            Else
                S.HasError = True
                S.ErrorMsg += vbCrLf & "خطا در شعاع دوم : " & vbCrLf & S.Radius2 & " تعریف نشده است."
            End If
        End If
        If Srz <> "" Then
            Srz = AKPEval.Eval(Srz, SrzEvalSuccess)
            If IsNumeric(Srz) And SrzEvalSuccess Then
                RSrz = CDbl(Srz) * (DivChangeVPZ / Len3DZ)
            Else
                S.HasError = True
                S.ErrorMsg = vbCrLf & "خطا در شعاع سوم : " & vbCrLf & S.Radius3 & " تعریف نشده است."
            End If
        End If

        If RSl < 0 Then
            S.HasError = True
            S.ErrorMsg = "خطا در طول : " & vbCrLf & "طول نمی تواند عددی منفی باشد."
        End If
        If RSw < 0 Then
            S.HasError = True
            S.ErrorMsg = "خطا در عرض : " & vbCrLf & "عرض نمی تواند عددی منفی باشد."
        End If
        If RSh < 0 Then
            S.HasError = True
            S.ErrorMsg = "خطا در ارتفاع : " & vbCrLf & "ارتفاع نمی تواند عددی منفی باشد."
        End If
        If RSrx < 0 Then
            S.HasError = True
            S.ErrorMsg = "خطا در شعاع : " & vbCrLf & "شعاع نمی تواند عددی منفی باشد."
        End If
        If RSry < 0 Then
            S.HasError = True
            S.ErrorMsg = "خطا در شعاع دوم : " & vbCrLf & "شعاع نمی تواند عددی منفی باشد."
        End If
        If RSrz < 0 Then
            S.HasError = True
            S.ErrorMsg = "خطا در شعاع سوم : " & vbCrLf & "شعاع نمی تواند عددی منفی باشد."
        End If

        Dim MB As New MeshBuilder
        Select Case S.Type
            Case ShapeType.Cone
                MB.AddCone(New Point3D(ROx, ROy, ROz), New Vector3D(RDi, RDj, RDk), RSrx, RSh, S.BaseCap, S.ThetaDiv)
            Case ShapeType.TCone
                MB.AddTCone(New Point3D(ROx, ROy, ROz), New Vector3D(RDi, RDj, RDk), RSrx, RSry, RSh, S.BaseCap, S.TopCap, S.ThetaDiv)
            Case ShapeType.Pyramid
                MB.AddCone(New Point3D(ROx, ROy, ROz), New Vector3D(RDi, RDj, RDk), RSrx, RSh, S.BaseCap, S.FaceCount + 1)
            Case ShapeType.Cylinder
                MB.AddCylinder(New Point3D(ROx, ROy, ROz), New Vector3D(RDi, RDj, RDk), RSrx, RSh, S.BaseCap, S.TopCap, S.ThetaDiv)
            Case ShapeType.Cube
                MB.AddCube(New Point3D(ROx, ROy, ROz), RSl, S.CubeFace)
            Case ShapeType.Box
                MB.AddBox(New Point3D(ROx, ROy, ROz), RSl, RSw, RSh, S.CubeFace)
            Case ShapeType.Sphere
                MB.AddSphere(New Point3D(ROx, ROy, ROz), RSrx, S.ThetaDiv, S.PhiDiv)
            Case ShapeType.Ellipsoid
                MB.AddEllipsoid(New Point3D(ROx, ROy, ROz), RSrx, RSry, RSrz, S.ThetaDiv, S.PhiDiv)
        End Select

        Dim MatGroup As New MaterialGroup
        Select Case ColoringMethod
            Case 0
                Dim MatD As New DiffuseMaterial(S.Brush)
                Dim MatE As New EmissiveMaterial(S.Brush)
                Dim MatS As New SpecularMaterial(S.Brush, 30)
                MatGroup.Children.Add(MatD)
                MatGroup.Children.Add(MatS)
            Case 1
                Dim MatB As New DiffuseMaterial(New SolidColorBrush(PointToColor(New Point3D(ROx, ROy, ROz), 5, -5)))
                MatGroup.Children.Add(MatB)
        End Select

        Dim Geo As New GeometryModel3D(MB.ToMesh, MatGroup)
        S.GraphObject.Add(Geo)
        VisualModel.Children.Add(Geo)
    End Sub

    'ایجاد صفحه جلو 3 بعدی
    Private Function CreateMeshFront(Plane As Point3DPlane) As MeshGeometry3D

        Dim Mesh As New MeshGeometry3D
        Dim Points As Point3DCollection = Plane.PointCollection
        Dim Cols As Integer = Plane.Columns
        Dim Rows As Integer = Plane.Rows
        Dim Index0 As Integer

        For i = 0 To Cols - 2
            For j = 0 To Rows - 2
                Dim ij As Integer = (i * Rows) + j

                Dim P0 As Integer = ij + Rows
                Dim P1 As Integer = ij
                Dim P2 As Integer = ij + Rows + 1
                Dim P3 As Integer = ij + 1

                Dim Pt0 As Point3D = Plane.PointCollection(P0)
                Dim Pt1 As Point3D = Plane.PointCollection(P1)
                Dim Pt2 As Point3D = Plane.PointCollection(P2)
                Dim Pt3 As Point3D = Plane.PointCollection(P3)

                'Positions
                Mesh.Positions.Add(Pt0)
                Mesh.Positions.Add(Pt1)
                Mesh.Positions.Add(Pt2)
                Mesh.Positions.Add(Pt3)

                'Normals
                Dim Norm1 As Vector3D = CalculateNormal(Pt0, Pt3, Pt1)
                Norm1.Normalize()
                Mesh.Normals.Add(Norm1)
                Mesh.Normals.Add(Norm1)
                Mesh.Normals.Add(Norm1)
                Mesh.Normals.Add(Norm1)

                'Trig Indices
                Mesh.TriangleIndices.Add(Index0 + 3)
                Mesh.TriangleIndices.Add(Index0 + 1)
                Mesh.TriangleIndices.Add(Index0 + 0)

                Mesh.TriangleIndices.Add(Index0 + 2)
                Mesh.TriangleIndices.Add(Index0 + 3)
                Mesh.TriangleIndices.Add(Index0 + 0)

                Index0 += 4

                'Texture Cordinates
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Cols - 1)), (j / (Rows - 1))))       'چپ پایین 0
                Mesh.TextureCoordinates.Add(New Point((i / (Cols - 1)), (j / (Rows - 1))))             'چپ بالا 1
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Cols - 1)), ((j + 1) / (Rows - 1)))) '2 راست پایین
                Mesh.TextureCoordinates.Add(New Point((i / (Cols - 1)), ((j + 1) / (Rows - 1))))       '3 راست بالا

            Next
        Next
        Return Mesh
    End Function
    'ایجاد صفحه عقب 3 بعدی
    Private Function CreateMeshBack(Plane As Point3DPlane) As MeshGeometry3D

        Dim Mesh As New MeshGeometry3D
        Dim Points As Point3DCollection = Plane.PointCollection
        Dim Cols As Integer = Plane.Columns
        Dim Rows As Integer = Plane.Rows
        Dim Index0 As Integer

        For i = 0 To Cols - 2
            For j = 0 To Rows - 2
                Dim ij As Integer = (i * Rows) + j

                Dim P0 As Integer = ij + Rows
                Dim P1 As Integer = ij
                Dim P2 As Integer = ij + Rows + 1
                Dim P3 As Integer = ij + 1

                Dim Pt4 As Point3D = Plane.PointCollection(P0)
                Dim Pt5 As Point3D = Plane.PointCollection(P1)
                Dim Pt6 As Point3D = Plane.PointCollection(P2)
                Dim Pt7 As Point3D = Plane.PointCollection(P3)

                'Positions
                Mesh.Positions.Add(Pt4)
                Mesh.Positions.Add(Pt5)
                Mesh.Positions.Add(Pt6)
                Mesh.Positions.Add(Pt7)

                'Normals
                Dim Norm2 As Vector3D = CalculateNormal(Pt4, Pt5, Pt7)
                Norm2.Normalize()
                Mesh.Normals.Add(Norm2)
                Mesh.Normals.Add(Norm2)
                Mesh.Normals.Add(Norm2)
                Mesh.Normals.Add(Norm2)

                'Trig Indices
                Mesh.TriangleIndices.Add(Index0 + 3)
                Mesh.TriangleIndices.Add(Index0 + 0)
                Mesh.TriangleIndices.Add(Index0 + 1)

                Mesh.TriangleIndices.Add(Index0 + 2)
                Mesh.TriangleIndices.Add(Index0 + 0)
                Mesh.TriangleIndices.Add(Index0 + 3)

                Index0 += 4

                'Texture Cordinates
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Cols - 1)), (j / (Rows - 1)))) '6 راست پایین
                Mesh.TextureCoordinates.Add(New Point((i / (Cols - 1)), (j / (Rows - 1))))       '7 راست بالا
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Cols - 1)), ((j + 1) / (Rows - 1))))       'چپ پایین 4
                Mesh.TextureCoordinates.Add(New Point((i / (Cols - 1)), ((j + 1) / (Rows - 1))))             'چپ بالا 5

            Next
        Next
        Return Mesh
    End Function

    'ایجاد مدل ورفرم
    Private Sub CreateWireFrame(Plane As Point3DPlane, Color As Color)
        Dim Rows As Integer = Plane.Rows
        Dim Cols As Integer = Plane.Columns

        Dim Wireframe As New ModelVisual3D
        Dim WPC As New Point3DCollection

        For i = 0 To Cols - 1
            For j = 0 To Rows - 1
                If i Mod 2 = 0 Then
                    Dim ij As Integer = (i * Rows) + j
                    Dim Pt0 As Point3D = Plane.PointCollection(ij)
                    WPC.Add(Pt0)
                Else
                    Dim ij As Integer = ((i + 1) * Rows) - j - 1
                    Dim Pt0 As Point3D = Plane.PointCollection(ij)
                    WPC.Add(Pt0)
                End If
            Next
        Next

        For i = 0 To Rows - 1
            For j = Cols - 1 To 0 Step -1
                If i Mod 2 = 0 Then
                    Dim ij As Integer = (j * Rows) + i
                    Dim Pt0 As Point3D = Plane.PointCollection(ij)
                    WPC.Add(Pt0)
                Else
                    Dim ij As Integer = ((Cols - j - 1) * Rows) + i
                    Dim Pt0 As Point3D = Plane.PointCollection(ij)
                    WPC.Add(Pt0)
                End If
            Next
        Next

        Dim WPolyLine As New WirePolyline
        WPolyLine.Color = Color
        WPolyLine.Thickness = 1
        WPolyLine.Points = WPC

        VisualWire.Children.Add(WPolyLine)
    End Sub

    'ایجاد مدل نقطه ای
    Private Function CreatePointModel(Plane As Point3DPlane, Color As Color) As Model3DGroup

        Dim Points As Point3DCollection = Plane.PointCollection
        Dim Model As New Model3DGroup

        For Each P3D As Point3D In Points
            Dim MB As New MeshBuilder
            MB.AddSphere(New Point3D(P3D.X, P3D.Y, P3D.Z), 0.07)

            Dim MatGroup As New MaterialGroup
            Dim MatD As New DiffuseMaterial(New SolidColorBrush(Color))
            Dim MatE As New EmissiveMaterial(New SolidColorBrush(Color))
            Dim MatS As New SpecularMaterial(New SolidColorBrush(Color), 30)
            MatGroup.Children.Add(MatD)
            MatGroup.Children.Add(MatS)

            Dim Geo As New GeometryModel3D(MB.ToMesh, MatGroup)
            Model.Children.Add(Geo)
        Next
        Return Model
    End Function

    'ایجاد قلمو
    Private Function CreateMeshBrush(Plane As Point3DPlane) As WriteableBitmap

        Dim WB As New WriteableBitmap(Plane.Columns, Plane.Rows, 96, 96, PixelFormats.Bgr32, Nothing)

        Dim Cols As Integer = Plane.Columns
        Dim Rows As Integer = Plane.Rows

        For i = 0 To Rows - 2
            For j = 0 To Cols - 2
                Dim ij As Integer = (i * Cols) + j

                Dim P0 As Integer = ij + Cols
                Dim P1 As Integer = ij
                Dim P2 As Integer = ij + Cols + 1
                Dim P3 As Integer = ij + 1

                Dim Pt0 As Point3D = Plane.PointCollection(P0)
                Dim Pt1 As Point3D = Plane.PointCollection(P1)
                Dim Pt2 As Point3D = Plane.PointCollection(P2)
                Dim Pt3 As Point3D = Plane.PointCollection(P3)

                Dim Col0 As Color = PointToColor(Pt0, 5, -5)
                Dim Col1 As Color = PointToColor(Pt1, 5, -5)
                Dim Col2 As Color = PointToColor(Pt2, 5, -5)
                Dim Col3 As Color = PointToColor(Pt3, 5, -5)

                Dim Pixels(1, 1) As BitmapTools.PixelColor
                Dim Pixel0 As New BitmapTools.PixelColor With {.Red = Col0.R, .Green = Col0.G, .Blue = Col0.B, .Alpha = 255}
                Dim Pixel1 As New BitmapTools.PixelColor With {.Red = Col1.R, .Green = Col1.G, .Blue = Col1.B, .Alpha = 255}
                Dim Pixel2 As New BitmapTools.PixelColor With {.Red = Col2.R, .Green = Col2.G, .Blue = Col2.B, .Alpha = 255}
                Dim Pixel3 As New BitmapTools.PixelColor With {.Red = Col3.R, .Green = Col3.G, .Blue = Col3.B, .Alpha = 255}

                Pixels(0, 0) = Pixel1
                Pixels(0, 1) = Pixel3
                Pixels(1, 0) = Pixel0
                Pixels(1, 1) = Pixel2

                BitmapTools.PutPixels(WB, Pixels, i, j)
            Next
        Next

        Return WB
    End Function
    Private Function CreateMeshBrush(Plane As Point4DPlane) As WriteableBitmap

        Dim WB As New WriteableBitmap(Plane.Columns, Plane.Rows, 96, 96, PixelFormats.Bgr32, Nothing)

        Dim Cols As Integer = Plane.Columns
        Dim Rows As Integer = Plane.Rows

        For i = 0 To Rows - 2
            For j = 0 To Cols - 2
                Dim ij As Integer = (i * Cols) + j

                Dim P0 As Integer = ij + Cols
                Dim P1 As Integer = ij
                Dim P2 As Integer = ij + Cols + 1
                Dim P3 As Integer = ij + 1

                Dim Pt0 As Point4D = Plane.PointCollection(P0)
                Dim Pt1 As Point4D = Plane.PointCollection(P1)
                Dim Pt2 As Point4D = Plane.PointCollection(P2)
                Dim Pt3 As Point4D = Plane.PointCollection(P3)

                Dim Col0 As Color = PointToColor(Pt0, 5, -5)
                Dim Col1 As Color = PointToColor(Pt1, 5, -5)
                Dim Col2 As Color = PointToColor(Pt2, 5, -5)
                Dim Col3 As Color = PointToColor(Pt3, 5, -5)

                Dim Pixels(1, 1) As BitmapTools.PixelColor
                Dim Pixel0 As New BitmapTools.PixelColor With {.Red = Col0.R, .Green = Col0.G, .Blue = Col0.B, .Alpha = Col0.A}
                Dim Pixel1 As New BitmapTools.PixelColor With {.Red = Col1.R, .Green = Col1.G, .Blue = Col1.B, .Alpha = Col1.A}
                Dim Pixel2 As New BitmapTools.PixelColor With {.Red = Col2.R, .Green = Col2.G, .Blue = Col2.B, .Alpha = Col2.A}
                Dim Pixel3 As New BitmapTools.PixelColor With {.Red = Col3.R, .Green = Col3.G, .Blue = Col3.B, .Alpha = Col3.A}

                Pixels(0, 0) = Pixel1
                Pixels(0, 1) = Pixel3
                Pixels(1, 0) = Pixel0
                Pixels(1, 1) = Pixel2

                BitmapTools.PutPixels(WB, Pixels, i, j)
            Next
        Next

        Return WB
    End Function

#End Region

#Region "رسم 4 بعدی"
    Private m_Rotate4D As Boolean = False
    Public Property Rotate4D As Boolean
        Get
            Return m_Rotate4D
        End Get
        Set(value As Boolean)
            m_Rotate4D = value
            NotifyPropertyChanged("Rotate4D")
        End Set
    End Property
    Public Sub ResetCamera4D()
        Proj4D = New Projection4D(New Point4D(0, 0, 0, 2))
        AutoRedraw()
    End Sub

    Public Proj4D As Projection4D = New Projection4D(New Point4D(0, 0, 0, 2))

    Private m_tess As Boolean = False
    Public Property DrawTesseract As Boolean
        Get
            Return m_tess
        End Get
        Set(value As Boolean)
            m_tess = value
            NotifyPropertyChanged("DrawTesseract")
            AutoRedraw()
        End Set
    End Property

    Private Sub Draw4D()

        If DrawTesseract Then

            VisualCorLines4D.Children.Clear()
            VisualModel4D.Children.Clear()
            VisualWire4D.Children.Clear()

            Dim T0 As New Point4D(-1, -1, -1, -1)
            Dim T1 As New Point4D(-1, -1, -1, 1)
            Dim T2 As New Point4D(-1, -1, 1, -1)
            Dim T3 As New Point4D(-1, 1, -1, -1)
            Dim T4 As New Point4D(1, -1, -1, -1)
            Dim T5 As New Point4D(-1, -1, 1, 1)
            Dim T6 As New Point4D(-1, 1, -1, 1)
            Dim T7 As New Point4D(1, -1, -1, 1)
            Dim T8 As New Point4D(-1, 1, 1, -1)
            Dim T9 As New Point4D(1, -1, 1, -1)
            Dim T10 As New Point4D(1, 1, -1, -1)
            Dim T11 As New Point4D(-1, 1, 1, 1)
            Dim T12 As New Point4D(1, -1, 1, 1)
            Dim T13 As New Point4D(1, 1, -1, 1)
            Dim T14 As New Point4D(1, 1, 1, -1)
            Dim T15 As New Point4D(1, 1, 1, 1)

            Dim P4L As New List(Of Point4D) _
                From {T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}

            'Dim Proj As New Projection4D(CamPos4D)
            Dim P3L As List(Of Point3D) = Proj4D.ProjectTo3D(P4L)

            Dim L0 As New WireLine With {.Point1 = P3L(0), .Point2 = P3L(1)}
            Dim L1 As New WireLine With {.Point1 = P3L(1), .Point2 = P3L(5)}
            Dim L2 As New WireLine With {.Point1 = P3L(5), .Point2 = P3L(2)}
            Dim L3 As New WireLine With {.Point1 = P3L(2), .Point2 = P3L(0)}
            Dim L4 As New WireLine With {.Point1 = P3L(0), .Point2 = P3L(3)}
            Dim L5 As New WireLine With {.Point1 = P3L(3), .Point2 = P3L(10)}
            Dim L6 As New WireLine With {.Point1 = P3L(10), .Point2 = P3L(4)}
            Dim L7 As New WireLine With {.Point1 = P3L(4), .Point2 = P3L(0)}
            Dim L8 As New WireLine With {.Point1 = P3L(1), .Point2 = P3L(6)}
            Dim L9 As New WireLine With {.Point1 = P3L(6), .Point2 = P3L(3)}
            Dim L10 As New WireLine With {.Point1 = P3L(1), .Point2 = P3L(7)}
            Dim L11 As New WireLine With {.Point1 = P3L(2), .Point2 = P3L(8)}
            Dim L12 As New WireLine With {.Point1 = P3L(2), .Point2 = P3L(9)}
            Dim L13 As New WireLine With {.Point1 = P3L(3), .Point2 = P3L(8)}
            Dim L14 As New WireLine With {.Point1 = P3L(4), .Point2 = P3L(7)}
            Dim L15 As New WireLine With {.Point1 = P3L(4), .Point2 = P3L(9)}
            Dim L16 As New WireLine With {.Point1 = P3L(5), .Point2 = P3L(11)}
            Dim L17 As New WireLine With {.Point1 = P3L(5), .Point2 = P3L(12)}
            Dim L18 As New WireLine With {.Point1 = P3L(6), .Point2 = P3L(11)}
            Dim L19 As New WireLine With {.Point1 = P3L(6), .Point2 = P3L(13)}
            Dim L20 As New WireLine With {.Point1 = P3L(7), .Point2 = P3L(12)}
            Dim L21 As New WireLine With {.Point1 = P3L(7), .Point2 = P3L(13)}
            Dim L22 As New WireLine With {.Point1 = P3L(8), .Point2 = P3L(11)}
            Dim L23 As New WireLine With {.Point1 = P3L(8), .Point2 = P3L(14)}
            Dim L24 As New WireLine With {.Point1 = P3L(9), .Point2 = P3L(12)}
            Dim L25 As New WireLine With {.Point1 = P3L(9), .Point2 = P3L(14)}
            Dim L26 As New WireLine With {.Point1 = P3L(10), .Point2 = P3L(13)}
            Dim L27 As New WireLine With {.Point1 = P3L(10), .Point2 = P3L(14)}
            Dim L28 As New WireLine With {.Point1 = P3L(11), .Point2 = P3L(15)}
            Dim L29 As New WireLine With {.Point1 = P3L(12), .Point2 = P3L(15)}
            Dim L30 As New WireLine With {.Point1 = P3L(13), .Point2 = P3L(15)}
            Dim L31 As New WireLine With {.Point1 = P3L(14), .Point2 = P3L(15)}

            Dim LC As New List(Of Visual3D) _
                From {L0, L1, L2, L3, L4, L5, L6, L7, L8, L9, L10, L11, L12, L13, L14, L15, L16, L17, L18, L19, L20, L21, L22, L23, L24, L25, L26, L27, L28, L29, L30, L31}

            For Each L In LC
                VisualWire4D.Children.Add(L)
            Next

        Else

            VisualWire4D.Children.Clear()
            VisualModel4D.Children.Clear()
            DrawCoordinateSystem4D()

            'InitializeLight()

            'If IsCor3DModified = True Then
            '    DrawCoordinateSystem3D()
            'End If

            Select Case ActivePlaneMode
                Case PlaneMode.Normal
                    For Each MO As MathObject In MathObjList
                        MO.Inner.HasError = False
                        MO.Inner.ErrorMsg = ""
                        Select Case MO.Type
                            Case 0 'Equation
                                DebugMsg("REDRAW 4D [Normal] --> " & MO.Inner.Expression, 1)
                                'DebugMsg("Needs Recalculation :" & MO.Inner.NeedsRecalculation, 2)
                                'If MO.Inner.NeedsRecalculation Or HaveVarsInFormula(MO.Inner) Or IsCor3DModified Then
                                'For Each GO In MO.Inner.GraphObject
                                '    VisualModel.Children.Remove(GO)
                                'Next
                                'MO.Inner.GraphObject.Clear()
                                If MO.Inner.IsVisible Then DrawGraph4D(MO.Inner)
                                'End If
                                'Case 2 'Point
                                '    For Each GO In MO.Inner.GraphObject
                                '        VisualModel.Children.Remove(GO)
                                '    Next
                                '    MO.Inner.GraphObject.Clear()
                                '    If MO.Inner.IsVisible Then DrawPoint3D(MO.Inner)
                                'Case 3 'Vector
                                '    If MO.Inner.IsVisible Then DrawVector3D(MO.Inner)
                                'Case 4 'Shape
                                '    For Each GO In MO.Inner.GraphObject
                                '        VisualModel.Children.Remove(GO)
                                '    Next
                                '    MO.Inner.GraphObject.Clear()
                                '    If MO.Inner.IsVisible Then DrawShape3D(MO.Inner)
                        End Select
                    Next

                Case PlaneMode.Wireframe
                    For Each MO As MathObject In MathObjList
                        MO.Inner.HasError = False
                        MO.Inner.ErrorMsg = ""
                        Select Case MO.Type
                            Case 0 'Equation
                                DebugMsg("REDRAW 4D [Wire] --> Equation (" & MO.Inner.Expression & ")", 1)
                                '            If MO.Inner.NeedsRecalculation Or HaveVarsInFormula(MO.Inner) Or IsCor3DModified Then
                                '                For Each GO In MO.Inner.GraphObject
                                '                    VisualModel.Children.Remove(GO)
                                '                Next
                                '                MO.Inner.GraphObject.Clear()
                                If MO.Inner.IsVisible Then DrawGraphWireframe4D(MO.Inner)
                                '            End If
                                '        Case 2 'Point                            
                                '            For Each GO In MO.Inner.GraphObject
                                '                VisualModel.Children.Remove(GO)
                                '            Next
                                '            MO.Inner.GraphObject.Clear()
                                '            If MO.Inner.IsVisible Then DrawPoint3D(MO.Inner)
                                '        Case 3 'Vector
                                '            If MO.Inner.IsVisible Then DrawVector3D(MO.Inner)
                        End Select
                    Next

                    '    Case PlaneMode.PointModel
                    'For Each MO As MathObject In MathObjList
                    '    MO.Inner.HasError = False
                    '    MO.Inner.ErrorMsg = ""
                    '    Select Case MO.Type
                    '        Case 0 'Equation
                    '            Dim Eq As Equation = MO.Inner
                    '            DebugMsg("REDRAW 3D [Point] --> Equation (" & MO.Inner.Expression & ")", 1)
                    '            If Eq.NeedsRecalculation Or IsCor3DModified Then
                    '                For Each GO In Eq.GraphObject
                    '                    VisualModel.Children.Remove(GO)
                    '                Next
                    '                Eq.GraphObject.Clear()
                    '                If MO.Inner.IsVisible Then DrawGraphPointModel3D(MO.Inner)
                    '            End If
                    '        Case 2 'Point
                    '            For Each GO In MO.Inner.GraphObject
                    '                VisualModel.Children.Remove(GO)
                    '            Next
                    '            MO.Inner.GraphObject.Clear()
                    '            If MO.Inner.IsVisible Then DrawPoint3D(MO.Inner)
                    '        Case 3 'Vector
                    '            If MO.Inner.IsVisible Then DrawVector3D(MO.Inner)
                    '    End Select
                    'Next
                    'End Select
                    'IsCor3DModified = False

                    ' VisualModel = New Model3DGroup
                    'VisualWire.Children.Clear()
                    'InitializeLight()

                    'Dim Model As New Model3DGroup
                    'Model.Transform = New Transform3DGroup
                    'For Each MO As MathObject In MathObjList
                    '    If MO.Type = 0 Then
                    '        Dim Eq As Equation = MO.Inner
                    '        AKPTerminal.LogWriteLine("4D Plot : " + Eq.Expression)
                    '        For Each PP As Point4DPlane In DrawGraph4D(Eq, True)

                    '            Dim MatGroup As New MaterialGroup

                    '            Dim MatB As New DiffuseMaterial(New ImageBrush(CreateMeshBrush(PP)))
                    '            MatGroup.Children.Add(MatB)

                    '            Dim MeshF As MeshGeometry3D = CreateMeshFront(PP.ToPoint3DPlane)
                    '            Dim MeshB As MeshGeometry3D = CreateMeshBack(PP.ToPoint3DPlane)

                    '            Dim GeoF As New GeometryModel3D(MeshF, MatGroup)
                    '            Dim GeoB As New GeometryModel3D(MeshB, MatGroup)
                    '            Model.Children.Add(GeoF)
                    '            Model.Children.Add(GeoB)
                    '        Next
                    '    End If
                    'Next
                    'VisualModel.Children.Add(Model)
            End Select

        End If

    End Sub

    Public Sub DrawCoordinateSystem4D()
        VisualCorLines4D.Children.Clear()
        'OverlayPoints.Clear()

        'If Cor3DType = Cor3DTypes.Corneral Then

        '    'دستگاه مختصات گوشه ای
        '    If CorDetail >= 1 Then
        '        Dim XAxis As New WireLine
        '        XAxis.Color = Colors.Blue
        '        XAxis.Thickness = Cor3DThickness
        '        XAxis.Point1 = New Point3D(-LenVPX, -LenVPY, -LenVPZ)
        '        XAxis.Point2 = New Point3D(LenVPX, -LenVPY, -LenVPZ)
        '        XAxis.ArrowEnds = 2
        '        XAxis.ArrowLength = 10
        '        XAxis.ArrowAngle = 45

        '        Dim XLbl As New WireText
        '        XLbl.Text = "X"
        '        XLbl.Color = Colors.Blue
        '        XLbl.UpDirection = New Vector3D(0, 1, 0)
        '        XLbl.BaselineDirection = New Vector3D(1, 0, 0)
        '        XLbl.Thickness = Cor3DThickness
        '        XLbl.Origin = New Point3D(LenVPX + 0.2, -LenVPY, -LenVPZ)
        '        XLbl.Font = Font.Roman
        '        XLbl.FontSize = 0.5

        '        Dim YAxis As New WireLine
        '        YAxis.Color = Colors.Red
        '        YAxis.Thickness = Cor3DThickness
        '        YAxis.Point1 = New Point3D(-LenVPX, -LenVPY, -LenVPZ)
        '        YAxis.Point2 = New Point3D(-LenVPX, LenVPY, -LenVPZ)
        '        YAxis.ArrowEnds = 2
        '        YAxis.ArrowLength = 10
        '        YAxis.ArrowAngle = 45

        '        Dim YLbl As New WireText
        '        YLbl.Text = "Y"
        '        YLbl.Color = Colors.Red
        '        YLbl.UpDirection = New Vector3D(0, 1, 0)
        '        YLbl.Thickness = Cor3DThickness
        '        YLbl.Origin = New Point3D(-LenVPX, LenVPY + 0.5, -LenVPZ)
        '        YLbl.Font = Font.Roman
        '        YLbl.FontSize = 0.5

        '        Dim ZAxis As New WireLine
        '        ZAxis.Color = Colors.Green
        '        ZAxis.Thickness = Cor3DThickness
        '        ZAxis.Point1 = New Point3D(-LenVPX, -LenVPY, -LenVPZ)
        '        ZAxis.Point2 = New Point3D(-LenVPX, -LenVPY, LenVPZ)
        '        ZAxis.ArrowEnds = 2
        '        ZAxis.ArrowLength = 10
        '        ZAxis.ArrowAngle = 45

        '        Dim ZLbl As New WireText
        '        ZLbl.Text = "Z"
        '        ZLbl.Color = Colors.Green
        '        ZLbl.UpDirection = New Vector3D(0, 1, 0)
        '        ZLbl.BaselineDirection = New Vector3D(0, 0, -1)
        '        ZLbl.Thickness = Cor3DThickness
        '        ZLbl.Origin = New Point3D(-LenVPX, -LenVPY, LenVPZ + 0.5)
        '        ZLbl.Font = Font.Roman
        '        ZLbl.FontSize = 0.5

        '        VisualCorLines.Children.Add(XAxis)
        '        VisualCorLines.Children.Add(XLbl)

        '        VisualCorLines.Children.Add(YAxis)
        '        VisualCorLines.Children.Add(YLbl)

        '        VisualCorLines.Children.Add(ZAxis)
        '        VisualCorLines.Children.Add(ZLbl)
        '    End If


        '    Dim DivX As Integer = (LenVPX \ LenVPScale)
        '    Dim DivChangeVPX As Double = LenVPX / DivX

        '    For i = -DivX + 1 To DivX
        '        Dim Value As Single
        '        Value = (Len3DX * i)

        '        If CorDetail >= 2 Then
        '            Dim PSX As New Point3D((DivChangeVPX * i), -LenVPY, -LenVPZ)
        '            Dim PSY As New Point3D((DivChangeVPX * i), LenVPY, -LenVPZ)
        '            Dim PSZ As New Point3D((DivChangeVPX * i), -LenVPY, LenVPZ)
        '            DrawLine3D(Colors.DarkGray, PSX, PSY)
        '            DrawLine3D(Colors.DarkGray, PSX, PSZ)
        '        End If

        '        If CorDetail = 3 Then
        '            Dim PText As New Point3D((DivChangeVPX * i), -LenVPY + 0.3, -LenVPZ)
        '            Dim VectO As New Vector3D(1, 0, 0) 'VectorOver
        '            If Labels3DMode = Labels3D.Space Then
        '                DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Blue)
        '            ElseIf Labels3DMode = Labels3D.Overlay Then
        '                DrawText3D_Overlay(PText, Value.ToString, Colors.Blue)
        '            End If
        '        End If
        '    Next

        '    Dim DivY As Integer = (LenVPY \ LenVPScale)
        '    Dim DivChangeVPY As Double = LenVPY / DivY

        '    For i = -DivY + 1 To DivY
        '        Dim Value As Single
        '        Value = (Len3DY * i)

        '        If CorDetail >= 2 Then
        '            Dim PSY As New Point3D(-LenVPX, (DivChangeVPY * i), -LenVPZ)
        '            Dim PSX As New Point3D(LenVPX, (DivChangeVPY * i), -LenVPZ)
        '            Dim PSZ As New Point3D(-LenVPX, (DivChangeVPY * i), LenVPZ)
        '            DrawLine3D(Colors.DarkGray, PSY, PSX)
        '            DrawLine3D(Colors.DarkGray, PSY, PSZ)
        '        End If

        '        If CorDetail = 3 Then
        '            Dim PText As New Point3D(-LenVPX + 0.3, (DivChangeVPY * i), -LenVPZ)
        '            Dim VectO As New Vector3D(1, 0, 0) 'VectorOver
        '            If Labels3DMode = Labels3D.Space Then
        '                DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Red)
        '            ElseIf Labels3DMode = Labels3D.Overlay Then
        '                DrawText3D_Overlay(PText, Value.ToString, Colors.Red)
        '            End If
        '        End If
        '    Next

        '    Dim DivZ As Integer = (LenVPZ \ LenVPScale)
        '    Dim DivChangeVPZ As Double = LenVPZ / DivZ

        '    For i = -DivZ + 1 To DivZ
        '        Dim Value As Single
        '        Value = (Len3DZ * i)

        '        If CorDetail >= 2 Then
        '            Dim PSZ As New Point3D(-LenVPX, -LenVPY, (DivChangeVPZ * i))
        '            Dim PSX As New Point3D(LenVPX, -LenVPY, (DivChangeVPZ * i))
        '            Dim PSY As New Point3D(-LenVPX, LenVPY, (DivChangeVPZ * i))
        '            DrawLine3D(Colors.DarkGray, PSZ, PSX)
        '            DrawLine3D(Colors.DarkGray, PSZ, PSY)
        '        End If

        '        If CorDetail = 3 Then
        '            Dim PText As New Point3D(-LenVPX, -LenVPY + 0.3, (DivChangeVPZ * i))
        '            Dim VectO As New Vector3D(0, 0, 1) 'VectorOver
        '            If Labels3DMode = Labels3D.Space Then
        '                DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Green)
        '            ElseIf Labels3DMode = Labels3D.Overlay Then
        '                DrawText3D_Overlay(PText, Value.ToString, Colors.Green)
        '            End If
        '        End If
        '    Next

        '    UpdateOverlay()

        'ElseIf Cor3DType = Cor3DTypes.Central Then

        'دستگاه مختصات مرکزی

        If CorDetail >= 1 Then
            Dim XAxis As New WireLine
            XAxis.Color = Colors.Blue
            XAxis.Thickness = Cor3DThickness
            Dim PPx1 As Point3D = Proj4D.ProjectTo3D(New Point4D(-1, 0, 0, 0))
            Dim PPx2 As Point3D = Proj4D.ProjectTo3D(New Point4D(1, 0, 0, 0))
            'XAxis.Point1 = New Point3D(-LenVPX, 0, 0)
            'XAxis.Point2 = New Point3D(LenVPX, 0, 0)
            XAxis.Point1 = PPx1
            XAxis.Point2 = PPx2
            XAxis.ArrowEnds = 2
            XAxis.ArrowLength = 10
            XAxis.ArrowAngle = 45
            VisualCorLines4D.Children.Add(XAxis)

            Dim XLbl As New WireText
            XLbl.Text = "X"
            XLbl.Color = Colors.Blue
            XLbl.UpDirection = New Vector3D(0, 1, 0)
            XLbl.BaselineDirection = PPx2.ToVector3D
            XLbl.Thickness = Cor3DThickness
            'XLbl.Origin = New Point3D(LenVPX + 0.2, 0, 0)
            XLbl.Origin = PPx2
            XLbl.Font = Font.Roman
            XLbl.FontSize = 0.1
            VisualCorLines4D.Children.Add(XLbl)

            Dim YAxis As New WireLine
            YAxis.Color = Colors.Red
            YAxis.Thickness = Cor3DThickness
            Dim PPy1 As Point3D = Proj4D.ProjectTo3D(New Point4D(0, -1, 0, 0))
            Dim PPy2 As Point3D = Proj4D.ProjectTo3D(New Point4D(0, 1, 0, 0))
            'YAxis.Point1 = New Point3D(0, -LenVPY, 0)
            'YAxis.Point2 = New Point3D(0, LenVPY, 0)
            YAxis.Point1 = PPy1
            YAxis.Point2 = PPy2
            YAxis.ArrowEnds = 2
            YAxis.ArrowLength = 10
            YAxis.ArrowAngle = 45
            VisualCorLines4D.Children.Add(YAxis)

            Dim YLbl As New WireText
            YLbl.Text = "Y"
            YLbl.Color = Colors.Red
            YLbl.UpDirection = New Vector3D(0, 1, 0)
            YLbl.Thickness = Cor3DThickness
            YLbl.Origin = PPy2
            YLbl.Font = Font.Roman
            YLbl.FontSize = 0.1
            VisualCorLines4D.Children.Add(YLbl)

            Dim ZAxis As New WireLine
            ZAxis.Color = Colors.Green
            ZAxis.Thickness = Cor3DThickness
            Dim PPz1 As Point3D = Proj4D.ProjectTo3D(New Point4D(0, 0, -1, 0))
            Dim PPz2 As Point3D = Proj4D.ProjectTo3D(New Point4D(0, 0, 1, 0))
            'ZAxis.Point1 = New Point3D(0, 0, -LenVPZ)
            'ZAxis.Point2 = New Point3D(0, 0, LenVPZ)
            ZAxis.Point1 = PPz1
            ZAxis.Point2 = PPz2
            ZAxis.ArrowEnds = 2
            ZAxis.ArrowLength = 10
            ZAxis.ArrowAngle = 45
            VisualCorLines4D.Children.Add(ZAxis)

            Dim ZLbl As New WireText
            ZLbl.Text = "Z"
            ZLbl.Color = Colors.Green
            ZLbl.UpDirection = New Vector3D(0, 1, 0)
            ZLbl.BaselineDirection = PPz2.ToVector3D
            ZLbl.Thickness = Cor3DThickness
            ZLbl.Origin = PPz2
            ZLbl.Font = Font.Roman
            ZLbl.FontSize = 0.1
            VisualCorLines4D.Children.Add(ZLbl)

            Dim WAxis As New WireLine
            WAxis.Color = Colors.Purple
            WAxis.Thickness = Cor3DThickness
            Dim PPw1 As Point3D = Proj4D.ProjectTo3D(New Point4D(0, 0, 0, -1))
            Dim PPw2 As Point3D = Proj4D.ProjectTo3D(New Point4D(0, 0, 0, 1))
            'ZAxis.Point1 = New Point3D(0, 0, -LenVPZ)
            'ZAxis.Point2 = New Point3D(0, 0, LenVPZ)
            WAxis.Point1 = PPw1
            WAxis.Point2 = PPw2
            WAxis.ArrowEnds = 2
            WAxis.ArrowLength = 10
            WAxis.ArrowAngle = 45
            VisualCorLines4D.Children.Add(WAxis)

            Dim WLbl As New WireText
            WLbl.Text = "W"
            WLbl.Color = Colors.Purple
            WLbl.UpDirection = New Vector3D(0, 1, 0)
            WLbl.BaselineDirection = PPw2.ToVector3D
            WLbl.Thickness = Cor3DThickness
            WLbl.Origin = PPw2
            WLbl.Font = Font.Roman
            WLbl.FontSize = 0.1
            VisualCorLines4D.Children.Add(WLbl)
        End If

        'Dim DivX As Integer = (LenVPX \ LenVPScale)
        'Dim DivChangeVPX As Double = LenVPX / DivX

        'For i = -DivX To DivX
        '    Dim Value As Single
        '    Value = (Len3DX * i)
        '    If i <> 0 Then
        '        If CorDetail >= 2 Then
        '            Dim P1 As New Point3D((DivChangeVPX * i), -0.1, 0)
        '            Dim P2 As New Point3D((DivChangeVPX * i), 0.1, 0)
        '            Dim PText As New Point3D((DivChangeVPX * i), 0.3, 0)
        '            Dim VectO As New Vector3D(1, 0, 0) 'VectorOver
        '            DrawLine3D(Colors.Blue, P1, P2)

        '            If CorDetail = 3 Then
        '                If Labels3DMode = Labels3D.Space Then
        '                    DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Blue)
        '                ElseIf Labels3DMode = Labels3D.Overlay Then
        '                    DrawText3D_Overlay(PText, Value.ToString, Colors.Blue)
        '                End If
        '            End If
        '        End If
        '    End If
        'Next

        'Dim DivY As Integer = (LenVPY \ LenVPScale)
        'Dim DivChangeVPY As Double = LenVPY / DivY

        'For i = -DivY To DivY
        '    Dim Value As Single
        '    Value = (Len3DY * i)
        '    If Value <> 0 Then
        '        If CorDetail >= 2 Then
        '            Dim P1 As New Point3D(-0.1, (DivChangeVPY * i), 0)
        '            Dim P2 As New Point3D(0.1, (DivChangeVPY * i), 0)
        '            Dim PText As New Point3D(0.3, (DivChangeVPY * i), 0)
        '            Dim VectO As New Vector3D(1, 0, 0) 'VectorOver
        '            DrawLine3D(Colors.Red, P1, P2)

        '            If CorDetail = 3 Then
        '                If Labels3DMode = Labels3D.Space Then
        '                    DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Red)
        '                ElseIf Labels3DMode = Labels3D.Overlay Then
        '                    DrawText3D_Overlay(PText, Value.ToString, Colors.Red)
        '                End If
        '            End If
        '        End If
        '    End If
        'Next

        'Dim DivZ As Integer = (LenVPZ \ LenVPScale)
        'Dim DivChangeVPZ As Double = LenVPZ / DivZ

        'For i = -DivZ To DivZ
        '    Dim Value As Single
        '    Value = (Len3DZ * i)
        '    If Value <> 0 Then
        '        If CorDetail >= 2 Then
        '            Dim P1 As New Point3D(0, -0.1, (DivChangeVPZ * i))
        '            Dim P2 As New Point3D(0, 0.1, (DivChangeVPZ * i))
        '            Dim PText As New Point3D(0, 0.3, (DivChangeVPZ * i))
        '            Dim VectO As New Vector3D(0, 0, 1) 'VectorOver
        '            DrawLine3D(Colors.Green, P1, P2)

        '            If CorDetail = 3 Then
        '                If Labels3DMode = Labels3D.Space Then
        '                    DrawText3D_Space(PText, VectO, New Vector3D(0, 1, 0), Value.ToString, Colors.Green)
        '                ElseIf Labels3DMode = Labels3D.Overlay Then
        '                    DrawText3D_Overlay(PText, Value.ToString, Colors.Green)
        '                End If
        '            End If
        '        End If
        '    End If
        'Next

        'End If

        ' UpdateOverlay()

    End Sub

    Private Sub DrawGraph4D(Equation As Equation)
        'Try
        EquationTools.DetectEqType(Equation, Me.DrawMode)
        'MsgBox("DETECTED " & Equation.Type)
        For Each PP As Point4DPlane In GetGraphSurface4D(Equation, True)
            Dim MatGroup As New MaterialGroup
            Select Case ColoringMethod
                Case 0
                    Dim MatD As New DiffuseMaterial(Equation.Brush)
                    Dim MatE As New EmissiveMaterial(Equation.Brush)
                    Dim MatS As New SpecularMaterial(Equation.Brush, 30)
                    MatGroup.Children.Add(MatD)
                    MatGroup.Children.Add(MatS)
                Case 1
                    Dim MatB As New DiffuseMaterial(New ImageBrush(CreateMeshBrush(PP)))
                    MatGroup.Children.Add(MatB)
            End Select
            Dim MeshF As MeshGeometry3D = CreateMeshFront(PP)
            Dim MeshB As MeshGeometry3D = CreateMeshBack(PP)
            Dim GeoF As New GeometryModel3D(MeshF, MatGroup)
            Dim GeoB As New GeometryModel3D(MeshB, MatGroup)
            'Equation.GraphObject.Add(GeoF)
            'Equation.GraphObject.Add(GeoB)
            VisualModel4D.Children.Add(GeoF)
            VisualModel4D.Children.Add(GeoB)
            'Equation.NeedsRecalculation = False
        Next

        'Catch Ex As Exception
        '    Equation.HasError = True
        '    Equation.ErrorMsg = "خطا در رسم" & vbCrLf & Ex.Message
        'End Try
    End Sub
    Private Sub DrawGraphWireframe4D(Equation As Equation)
        Try
            EquationTools.DetectEqType(Equation, Me.DrawMode)
            For Each PP As Point4DPlane In GetGraphSurface4D(Equation)
                Dim SCBrush As New SolidColorBrush
                If Equation.Brush.GetType Is GetType(SolidColorBrush) Then
                    SCBrush = Equation.Brush
                    CreateWireFrame(PP, SCBrush.Color)
                Else
                    Equation.HasError = True
                    Equation.ErrorMsg = "در حالت ورفرم فقط قلمو های رنگ جامد قابل قبول هستند."
                End If
            Next
        Catch Ex As Exception
            Equation.HasError = True
            Equation.ErrorMsg = "خطا در رسم" & vbCrLf & Ex.Message
        End Try
    End Sub

    'رسم نمودار
    Private Function GetGraphSurface4D(ByVal Equation As Equation, Optional HighACC As Boolean = False) As List(Of Point4DPlane)
        'Type 10  w=f(x,y,z)

        Dim ValidTypes() As EquationType = {EquationType.CartesianW, EquationType.Parametric4D}
        If Not ValidTypes.Contains(Equation.Type) Then
            Return New List(Of Point4DPlane)
        End If

        Dim PlanesList As New List(Of Point4DPlane)
        Dim AccValue As Double = (1 / Acc3D) * Len3DX
        'If HighACC Then AccValue /= 2

        Dim DivX As Integer = (LenVPX \ LenVPScale)
        Dim DivY As Integer = (LenVPY \ LenVPScale)
        Dim DivZ As Integer = (LenVPZ \ LenVPScale)
        Dim DivW As Integer = (LenVPW \ LenVPScale)
        Dim DivChangeVPX As Double = LenVPX / DivX
        Dim DivChangeVPY As Double = LenVPY / DivY
        Dim DivChangeVPZ As Double = LenVPZ / DivZ
        Dim DivChangeVPW As Double = LenVPW / DivW

        Dim SimplifySuccess As Boolean
        Dim EqMain As String = SimplifyFunction(Equation, SimplifySuccess)
        If SimplifySuccess = False Then Return New List(Of Point4DPlane)

        'جایگذاری متغییر ها
        For Each MOV As MathObject In MathObjList
            If MOV.Type = 1 Then
                Dim Var As Variable = MOV.Inner
                EqMain = EquationTools.Replace(EqMain, Var.Name, Var.Value)
            End If
        Next

        Select Case Equation.Type
            Case EquationType.Parametric4D  '4D Parametric :  x=f(u,v)   y=f(u,v)  z=f(u,v)  w=f(u,v)

                ' MsgBox("4D Parametric Drawing ... ")

                Dim PP As New Point4DPlane

                Dim XParameter As String = EquationTools.GetParameter(Equation, "x")
                Dim YParameter As String = EquationTools.GetParameter(Equation, "y")
                Dim ZParameter As String = EquationTools.GetParameter(Equation, "z")
                Dim WParameter As String = EquationTools.GetParameter(Equation, "w")

                'جایگذاری متغییر ها
                For Each MOV As MathObject In MathObjList
                    If MOV.Type = 1 Then
                        Dim Var As Variable = MOV.Inner
                        XParameter = EquationTools.Replace(XParameter, Var.Name, Var.Value)
                        YParameter = EquationTools.Replace(YParameter, Var.Name, Var.Value)
                        ZParameter = EquationTools.Replace(ZParameter, Var.Name, Var.Value)
                        WParameter = EquationTools.Replace(WParameter, Var.Name, Var.Value)
                    End If
                Next

                Dim DSU As Single
                Dim DEU As Single

                Dim DSV As Single
                Dim DEV As Single

                'Domain Of U
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSU = -(Len3DX * DivX)
                Else
                    DSU = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEU = +(Len3DX * DivX)
                Else
                    DEU = Equation.DomainEnd
                End If

                'Domain Of V
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSV = -(Len3DX * DivX)
                Else
                    DSV = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEV = +(Len3DX * DivX)
                Else
                    DEV = Equation.DomainEnd
                End If

                Dim EqStrPX As String
                Dim EqStrPY As String
                Dim EqStrPZ As String
                Dim EqStrPW As String
                Dim EvalResultPX As Double
                Dim EvalResultPY As Double
                Dim EvalResultPZ As Double
                Dim EvalResultPW As Double
                Dim x, y, z, w As Single

                EqStrPX = EquationTools.Replace(XParameter, "u", DSU)
                EqStrPY = EquationTools.Replace(YParameter, "u", DSU)
                EqStrPZ = EquationTools.Replace(ZParameter, "u", DSU)
                EqStrPW = EquationTools.Replace(WParameter, "u", DSU)

                EqStrPX = EquationTools.Replace(EqStrPX, "v", DSV)
                EqStrPY = EquationTools.Replace(EqStrPY, "v", DSV)
                EqStrPZ = EquationTools.Replace(EqStrPZ, "v", DSV)
                EqStrPW = EquationTools.Replace(EqStrPW, "v", DSV)

                EvalResultPX = AKPEval.Eval(XParameter, New Boolean, True)
                EvalResultPX = Val(EvalResultPX)

                EvalResultPY = AKPEval.Eval(YParameter, New Boolean, True)
                EvalResultPY = Val(EvalResultPY)

                EvalResultPZ = AKPEval.Eval(ZParameter, New Boolean, True)
                EvalResultPZ = Val(EvalResultPZ)

                EvalResultPW = AKPEval.Eval(WParameter, New Boolean, True)
                EvalResultPZ = Val(EvalResultPZ)

                'اولین برد
                Equation.RangeMax = EvalResultPY
                Equation.RangeMin = EvalResultPY

                Dim OldPoint As New Point4D(EvalResultPX * (DivChangeVPX / Len3DX), EvalResultPY * (DivChangeVPY / Len3DY), EvalResultPZ * (DivChangeVPZ / Len3DZ), EvalResultPW * (DivChangeVPW / Len3DW))

                Dim RowsCount As Integer = 0
                Dim ColumnsCount As Integer = 0

                For u = DSU To DEU Step AccValue

                    EqStrPX = EquationTools.Replace(XParameter, "u", MathTools.Round(u))
                    EqStrPY = EquationTools.Replace(YParameter, "u", MathTools.Round(u))
                    EqStrPZ = EquationTools.Replace(ZParameter, "u", MathTools.Round(u))
                    EqStrPW = EquationTools.Replace(WParameter, "u", MathTools.Round(u))

                    ColumnsCount += 1
                    RowsCount = 0

                    For v = DSV To DEV Step AccValue

                        Dim Eq2StrPX As String
                        Dim Eq2StrPY As String
                        Dim Eq2StrPZ As String
                        Dim Eq2StrPW As String

                        Eq2StrPX = EquationTools.Replace(EqStrPX, "v", MathTools.Round(v))
                        Dim EvalSuccessPX As Boolean
                        EvalResultPX = AKPEval.Eval(Eq2StrPX, EvalSuccessPX)

                        Eq2StrPY = EquationTools.Replace(EqStrPY, "v", MathTools.Round(v))
                        Dim EvalSuccessPY As Boolean
                        EvalResultPY = AKPEval.Eval(Eq2StrPY, EvalSuccessPY)

                        Eq2StrPZ = EquationTools.Replace(EqStrPZ, "v", MathTools.Round(v))
                        Dim EvalSuccessPZ As Boolean
                        EvalResultPZ = AKPEval.Eval(Eq2StrPZ, EvalSuccessPZ)

                        Eq2StrPW = EquationTools.Replace(EqStrPW, "v", MathTools.Round(v))
                        Dim EvalSuccessPW As Boolean
                        EvalResultPW = AKPEval.Eval(Eq2StrPW, EvalSuccessPW)

                        Dim EvalSuccess As Boolean = EvalSuccessPX And EvalSuccessPY And EvalSuccessPZ And EvalSuccessPW

                        If EvalSuccess Then

                            'EvalResultPX = Val(EvalResult)
                            RowsCount += 1

                            x = EvalResultPX * (DivChangeVPX / Len3DX)
                            y = EvalResultPY * (DivChangeVPY / Len3DY)
                            z = EvalResultPZ * (DivChangeVPZ / Len3DZ)
                            w = EvalResultPW * (DivChangeVPW / Len3DW)

                            If EvalResultPY > Equation.RangeMax Then Equation.RangeMax = EvalResultPY
                            If EvalResultPY < Equation.RangeMin Then Equation.RangeMin = EvalResultPY

                            'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                            Dim IsInTheScreen As Boolean = True 'InTheScreen(x, y, z)

                            'بررسی پیوستگی نمودار
                            Dim IsConnected As Boolean = True

                            Dim NewPoint As New Point4D(x, y, z, w)
                            'If PointsSpace(OldPoint, NewPoint) > 30 Then IsConnected = False
                            OldPoint = NewPoint

                            'If IsInTheScreen Then
                            'If IsConnected Then
                            PP.PointCollection.Add(NewPoint)
                            'Else
                            '    PP.Columns = ColumnsCount
                            '    PP.Rows = RowsCount
                            '    If PP.PointCollection.Count > 0 Then PlanesList.Add(PP)
                            '    ColumnsCount = 0
                            '    RowsCount = 0
                            '    PP = New Point4DPlane
                            'End If
                            'Else
                            'RowsCount -= 1
                            'End If

                            ' MsgBox("Col : " & ColumnsCount & "  Row : " & RowsCount)

                        End If
                    Next
                Next
                PP.Columns = ColumnsCount
                PP.Rows = RowsCount
                PlanesList.Add(PP)

            Case EquationType.CartesianW ' w=f(x,y,z)
                Dim PP As New Point4DPlane
                ' Draw Based On x,y,z  - تابع دکارتی
                Dim DSX As Single
                Dim DEX As Single

                Dim DSY As Single
                Dim DEY As Single

                Dim DSZ As Single
                Dim DEZ As Single

                Dim DSW As Single
                Dim DEW As Single

                'Domain Of X
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSX = -Len3DX
                Else
                    DSX = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEX = +Len3DX
                Else
                    DEX = Equation.DomainEnd
                End If

                'Domain Of Y
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSY = -Len3DY
                Else
                    DSY = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEY = +Len3DY
                Else
                    DEY = Equation.DomainEnd
                End If

                'Domain Of Z
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSZ = -Len3DZ
                Else
                    DSZ = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEZ = +Len3DZ
                Else
                    DEZ = Equation.DomainEnd
                End If

                '_______________________________ Temp Bug : No W LEN ! Use Z Len Instead !
                'Domain Of W
                If Equation.DomainStart = Double.NegativeInfinity Then
                    DSW = -Len3DZ
                Else
                    DSW = Equation.DomainStart
                End If
                If Equation.DomainEnd = Double.PositiveInfinity Then
                    DEW = +Len3DZ
                Else
                    DEW = Equation.DomainEnd
                End If

                Dim EqStr As String
                Dim EvalResult As Double
                Dim x, y, z, w As Single

                EqStr = EquationTools.Replace(EqMain, "x", DSX)
                EqStr = EquationTools.Replace(EqStr, "y", DSY)
                EqStr = EquationTools.Replace(EqStr, "z", DSZ)

                EvalResult = AKPEval.Eval(EqStr, New Boolean, True)
                EvalResult = Val(EvalResult)

                'اولین برد
                Equation.RangeMax = EvalResult
                Equation.RangeMin = EvalResult

                Dim OldPoint As New Point4D(DSX, DSY, DSZ, EvalResult)

                Dim RowsCount As Integer = 0
                Dim ColumnsCount As Integer = 0

                For i = DSX To DEX Step AccValue

                    EqStr = EquationTools.Replace(EqMain, "x", MathTools.Round(i))

                    ColumnsCount += 1
                    RowsCount = 0

                    For j = DSY To DEY Step AccValue

                        Dim Eq2Str As String
                        Eq2Str = EquationTools.Replace(EqStr, "y", MathTools.Round(j))

                        RowsCount += 1

                        For k = DSZ To DEZ Step AccValue

                            Dim Eq3Str As String
                            Eq3Str = EquationTools.Replace(Eq2Str, "z", MathTools.Round(k))
                            Dim EvalSuccess As Boolean
                            EvalResult = AKPEval.Eval(Eq3Str, EvalSuccess)

                            If Double.IsNaN(EvalResult) Then
                                'Throw New Exception("Math Error :  " + EqStr)
                            Else
                                EvalResult = Val(EvalResult)

                                x = i
                                y = j
                                z = k
                                w = EvalResult

                                'MsgBox("X : " & x & vbCrLf & "Y : " & y & vbCrLf & "Z : " & z & vbCrLf & "W : " & w)
                                If EvalResult > Equation.RangeMax Then Equation.RangeMax = EvalResult
                                If EvalResult < Equation.RangeMin Then Equation.RangeMin = EvalResult

                                'صرف نظر از رسم نقاطی که بیرون صفحه قرار دارند
                                'Dim IsInTheScreen As Boolean = True 'InTheScreen(x, y, z)

                                'بررسی پیوستگی نمودار
                                'Dim IsConnected As Boolean = True

                                Dim NewPoint As New Point4D(x, y, z, w)
                                'If PointsSpace(OldPoint.ToPoint3D, NewPoint.ToPoint3D) > 1 Then IsConnected = False
                                OldPoint = NewPoint

                                'If IsInTheScreen Then
                                'If IsConnected Then
                                PP.PointCollection.Add(NewPoint)
                                '    Else
                                '    PP.Columns = ColumnsCount
                                '    PP.Rows = RowsCount
                                '    If PP.PointCollection.Count > 0 Then PlanesList.Add(PP)
                                '    ColumnsCount = 0
                                '    RowsCount = 0
                                '    PP = New Point4DPlane
                                'End If
                                '    Else
                                ''RowsCount -= 1
                                'End If
                            End If
                        Next
                    Next
                Next
                PP.Columns = ColumnsCount
                PP.Rows = RowsCount
                PlanesList.Add(PP)
        End Select

        'MsgBox("Returning : " & PlanesList(0).PointCollection.Count & " Points. " & PlanesList(0).Rows & " x " & PlanesList(0).Columns)
        Return PlanesList
    End Function

    'ایجاد مدل ورفرم
    Private Sub CreateWireFrame(Plane As Point4DPlane, Color As Color)
        Dim Rows As Integer = Plane.Rows
        Dim Cols As Integer = Plane.Columns
        Dim Points As List(Of Point3D) = Proj4D.ProjectTo3D(Plane.PointCollection)
        Dim Wireframe As New ModelVisual3D
        Dim WPC As New Point3DCollection

        For i = 0 To Cols - 1
            For j = 0 To Rows - 1
                If i Mod 2 = 0 Then
                    Dim ij As Integer = (i * Rows) + j
                    Dim Pt0 As Point3D = Points(ij)
                    WPC.Add(Pt0)
                Else
                    Dim ij As Integer = ((i + 1) * Rows) - j - 1
                    Dim Pt0 As Point3D = Points(ij)
                    WPC.Add(Pt0)
                End If
            Next
        Next

        For i = 0 To Rows - 1
            For j = Cols - 1 To 0 Step -1
                If i Mod 2 = 0 Then
                    Dim ij As Integer = (j * Rows) + i
                    Dim Pt0 As Point3D = Points(ij)
                    WPC.Add(Pt0)
                Else
                    Dim ij As Integer = ((Cols - j - 1) * Rows) + i
                    Dim Pt0 As Point3D = Points(ij)
                    WPC.Add(Pt0)
                End If
            Next
        Next

        Dim WPolyLine As New WirePolyline
        WPolyLine.Color = Color
        WPolyLine.Thickness = 1
        WPolyLine.Points = WPC

        VisualWire4D.Children.Add(WPolyLine)
    End Sub

    'ایجاد صفحه جلو 4 بعدی
    Private Function CreateMeshFront(Plane As Point4DPlane) As MeshGeometry3D

        Dim Mesh As New MeshGeometry3D
        Dim Points As List(Of Point3D) = Proj4D.ProjectTo3D(Plane.PointCollection)
        Dim Cols As Integer = Plane.Columns
        Dim Rows As Integer = Plane.Rows
        Dim Index0 As Integer

        For i = 0 To Cols - 2
            For j = 0 To Rows - 2
                Dim ij As Integer = (i * Rows) + j

                Dim P0 As Integer = ij + Rows
                Dim P1 As Integer = ij
                Dim P2 As Integer = ij + Rows + 1
                Dim P3 As Integer = ij + 1

                Dim Pt0 As Point3D = Points(P0)
                Dim Pt1 As Point3D = Points(P1)
                Dim Pt2 As Point3D = Points(P2)
                Dim Pt3 As Point3D = Points(P3)

                'Positions
                Mesh.Positions.Add(Pt0)
                Mesh.Positions.Add(Pt1)
                Mesh.Positions.Add(Pt2)
                Mesh.Positions.Add(Pt3)

                'Normals
                Dim Norm1 As Vector3D = CalculateNormal(Pt0, Pt3, Pt1)
                Norm1.Normalize()
                Mesh.Normals.Add(Norm1)
                Mesh.Normals.Add(Norm1)
                Mesh.Normals.Add(Norm1)
                Mesh.Normals.Add(Norm1)

                'Trig Indices
                Mesh.TriangleIndices.Add(Index0 + 3)
                Mesh.TriangleIndices.Add(Index0 + 1)
                Mesh.TriangleIndices.Add(Index0 + 0)

                Mesh.TriangleIndices.Add(Index0 + 2)
                Mesh.TriangleIndices.Add(Index0 + 3)
                Mesh.TriangleIndices.Add(Index0 + 0)

                Index0 += 4

                'Texture Cordinates
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Cols - 1)), (j / (Rows - 1))))       'چپ پایین 0
                Mesh.TextureCoordinates.Add(New Point((i / (Cols - 1)), (j / (Rows - 1))))             'چپ بالا 1
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Cols - 1)), ((j + 1) / (Rows - 1)))) '2 راست پایین
                Mesh.TextureCoordinates.Add(New Point((i / (Cols - 1)), ((j + 1) / (Rows - 1))))       '3 راست بالا

            Next
        Next
        Return Mesh
    End Function
    'ایجاد صفحه عقب 4 بعدی
    Private Function CreateMeshBack(Plane As Point4DPlane) As MeshGeometry3D

        Dim Mesh As New MeshGeometry3D
        Dim Points As List(Of Point3D) = Proj4D.ProjectTo3D(Plane.PointCollection)
        Dim Cols As Integer = Plane.Columns
        Dim Rows As Integer = Plane.Rows
        Dim Index0 As Integer

        For i = 0 To Cols - 2
            For j = 0 To Rows - 2
                Dim ij As Integer = (i * Rows) + j

                Dim P0 As Integer = ij + Rows
                Dim P1 As Integer = ij
                Dim P2 As Integer = ij + Rows + 1
                Dim P3 As Integer = ij + 1

                Dim Pt4 As Point3D = Points(P0)
                Dim Pt5 As Point3D = Points(P1)
                Dim Pt6 As Point3D = Points(P2)
                Dim Pt7 As Point3D = Points(P3)

                'Positions
                Mesh.Positions.Add(Pt4)
                Mesh.Positions.Add(Pt5)
                Mesh.Positions.Add(Pt6)
                Mesh.Positions.Add(Pt7)

                'Normals
                Dim Norm2 As Vector3D = CalculateNormal(Pt4, Pt5, Pt7)
                Norm2.Normalize()
                Mesh.Normals.Add(Norm2)
                Mesh.Normals.Add(Norm2)
                Mesh.Normals.Add(Norm2)
                Mesh.Normals.Add(Norm2)

                'Trig Indices
                Mesh.TriangleIndices.Add(Index0 + 3)
                Mesh.TriangleIndices.Add(Index0 + 0)
                Mesh.TriangleIndices.Add(Index0 + 1)

                Mesh.TriangleIndices.Add(Index0 + 2)
                Mesh.TriangleIndices.Add(Index0 + 0)
                Mesh.TriangleIndices.Add(Index0 + 3)

                Index0 += 4

                'Texture Cordinates
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Cols - 1)), (j / (Rows - 1)))) '6 راست پایین
                Mesh.TextureCoordinates.Add(New Point((i / (Cols - 1)), (j / (Rows - 1))))       '7 راست بالا
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Cols - 1)), ((j + 1) / (Rows - 1))))       'چپ پایین 4
                Mesh.TextureCoordinates.Add(New Point((i / (Cols - 1)), ((j + 1) / (Rows - 1))))             'چپ بالا 5

            Next
        Next
        Return Mesh
    End Function

#End Region

#Region "توابع اندازه گیری"

    'اندازه گیری سایز متن
    Private Shared Function MeasureTextSize(Text As String, Optional FSize As Single = 12) As Size
        Dim FFamily As New FontFamily("Segoe UI")
        Dim FStyle As FontStyle = FontStyles.Normal
        Dim FWeight As FontWeight = FontWeights.Normal
        Dim FStretch As FontStretch = FontStretches.Normal
        Dim FT As New FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _
                                    New Typeface(FFamily, FStyle, FWeight, FStretch), FSize, Brushes.Black)
        Return New Size(FT.Width, FT.Height)
    End Function

    'بررسی در صفحه بودن
    Private Function InTheScreen(x As Double, y As Double) As Boolean
        If x >= -15 And x <= Cor2D_Front.Width + 15 And y >= -15 And y <= Cor2D_Front.Height + 15 Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Function InTheScreen(x As Double, y As Double, z As Double) As Boolean
        If x <= Len3DX And x >= -Len3DX And y <= Len3DY And y >= -Len3DY And z <= Len3DZ And z >= -Len3DZ Then
            Return True
        Else
            Return False
        End If
    End Function

    'محاسبه فاصله ی دو نقطه
    Private Function PointsSpace(pt1 As Point, pt2 As Point)
        Dim Len As Double = Abs(Math.Sqrt(Math.Pow((pt2.X - pt1.X), 2) + Math.Pow((pt2.Y - pt1.Y), 2)))
        Return Len
    End Function
    Private Function PointsSpace(pt1 As Point3D, pt2 As Point3D)
        Dim Len As Double = Abs(Math.Sqrt(Math.Pow((pt2.X - pt1.X), 2) + Math.Pow((pt2.Y - pt1.Y), 2) + Math.Pow((pt2.Z - pt1.Z), 2)))
        Return Len
    End Function
    Private Function PointsSpace(pt1 As Point4D, pt2 As Point4D)
        Dim Len As Double = Abs(Math.Sqrt(Math.Pow((pt2.X - pt1.X), 2) + Math.Pow((pt2.Y - pt1.Y), 2) + Math.Pow((pt2.Z - pt1.Z), 2) + Math.Pow((pt2.W - pt1.W), 2)))
        Return Len
    End Function

    'محاسبه بردار نرمال
    Private Function CalculateNormal(p0 As Point3D, p1 As Point3D, p2 As Point3D) As Vector3D
        Dim v0 As New Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z)
        Dim v1 As New Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z)
        Return Vector3D.CrossProduct(v0, v1) ' ضرب خارجی
    End Function

#End Region

#Region "سایر ابزار ها"

    'انتخاب رنگ
    Private PDColors As New List(Of Color) From {Colors.Red, Colors.Blue, Colors.Green, Colors.Purple, Colors.Lime, Colors.Magenta}
    Private Function PickColor() As Color
        If PDColors.Count > 0 Then
            Dim RndIndex As Integer = Rnd.Next(PDColors.Count)
            Dim PColor As Color = PDColors(RndIndex)
            PDColors.Remove(PColor)
            Return PColor
        Else
            'عدد تصادفی
1:          Dim R As Integer = Rnd.Next(255) 'Red
            Dim G As Integer = Rnd.Next(255) 'Green
            Dim B As Integer = Rnd.Next(255) 'Blue
            Dim A As Integer = 200           'Alpha
            ' جلوگیری از رنگ های خیلی روشن
            If R > 170 AndAlso G > 170 AndAlso B > 170 Then GoTo 1
            Dim Color As Color = Color.FromRgb(R, G, B)
            Return Color
        End If
    End Function

    'انتخاب حرف
    Private Function PickChar(Optional Base As String = "") As String
        If Base <> "" Then
            Dim Count As Integer = 1
            Dim Sure As Boolean = False
            While Not Sure
                Sure = True
                For Each MO As MathObject In MathObjList
                    Dim ProdChar As String = Base & Count.ToString
                    If MO.Type = 1 Or MO.Type = 2 Or MO.Type = 3 Then
                        If MO.Inner.Name = ProdChar Then
                            Count += 1
                            Sure = False
                            Exit For
                        End If
                    End If
                Next
            End While
            Return Base & Count.ToString
        Else
            Dim PDStr As New List(Of String) From {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "q", "s", "u"} 'Removed x,y,z,r,t,p
            For Each MO As MathObject In MathObjList
                If MO.Type = 1 Or MO.Type = 2 Or MO.Type = 3 Then
                    If PDStr.Contains(MO.Inner.Name) Then
                        PDStr.Remove(MO.Inner.Name)
                    End If
                End If
            Next
            Dim RandomIndex As Integer = Rnd.Next(PDStr.Count - 1)
            Return PDStr(RandomIndex)
        End If
    End Function

    'ایجاد رنگ گرم و سرد بر اساس مختصات
    Private Function PointToColor(P3D As Point3D, Max As Double, Min As Double) As Color

        Dim I As Double = Max - Min

        Dim Xi As Double = P3D.X - Min
        Dim Yi As Double = P3D.Y - Min
        Dim Zi As Double = P3D.Z - Min

        Dim Xc As Double = (Xi / I) * 255
        Dim Yc As Double = (Yi / I) * 255
        Dim Zc As Double = (Zi / I) * 255

        If Xc < 0 Then Xc = 0
        If Yc < 0 Then Yc = 0
        If Zc < 0 Then Zc = 0

        If Xc > 255 Then Xc = 255
        If Yc > 255 Then Yc = 255
        If Zc > 255 Then Zc = 255

        'XYZ
        Dim R As Integer = 0
        Dim G As Integer = 0
        Dim B As Integer = 0

        If ColorX Then B = Int(Xc)
        If ColorY Then R = Int(Yc)
        If ColorZ Then G = Int(Zc)
        Return Color.FromRgb(R, G, B)
    End Function
    Private Function PointToColor(P4D As Point4D, Max As Double, Min As Double) As Color

        Dim I As Double = Max - Min

        Dim Xi As Double = P4D.X - Min
        Dim Yi As Double = P4D.Y - Min
        Dim Zi As Double = P4D.Z - Min
        Dim Wi As Double = P4D.W - Min

        Dim Xc As Double = (Xi / I) * 255
        Dim Yc As Double = (Yi / I) * 255
        Dim Zc As Double = (Zi / I) * 255
        Dim Wc As Double = (Wi / I) * 255

        If Xc < 0 Then Xc = 0
        If Yc < 0 Then Yc = 0
        If Zc < 0 Then Zc = 0
        If Wc < 0 Then Wc = 0

        If Xc > 255 Then Xc = 255
        If Yc > 255 Then Yc = 255
        If Zc > 255 Then Zc = 255
        If Wc > 255 Then Wc = 255

        'XYZ
        Dim R As Integer = 0
        Dim G As Integer = 0
        Dim B As Integer = 0
        Dim A As Integer = 0

        If ColorX Then B = Int(Xc)
        If ColorY Then R = Int(Yc)
        If ColorZ Then G = Int(Zc)
        If ColorW Then
            A = Int(Wc)
        Else
            A = 255
        End If
        Return Color.FromArgb(R, G, B, A)
    End Function

    'عکس گرفتن از نمودار
    Public Sub TakeSnapshot(Optional ToClipboard As Boolean = False)
        If DrawMode = "4D" Then
            Dim RTB As New RenderTargetBitmap(Cor4D.ActualWidth, Cor4D.ActualHeight, 96, 96, PixelFormats.Pbgra32)
            RTB.Render(Cor4D)
            If ToClipboard Then
                Clipboard.SetImage(RTB)
                Dim D As New Dialog("فایل در Clipboard کپی شد.", "Copied To Clipboard !")
                D.ShowDialog()
            Else
                Dim PNG As New PngBitmapEncoder()
                PNG.Frames.Add(BitmapFrame.Create(RTB))

                Using STRM As Stream = File.Create(My.Computer.FileSystem.SpecialDirectories.Desktop + "/akptest.png")
                    PNG.Save(STRM)
                End Using
                Dim D As New Dialog("فایل در دسکتاپ ذخیره شد.", "File Saved In Desktop")
                D.ShowDialog()
            End If
        ElseIf DrawMode = "3D" Then
            Dim RTB As New RenderTargetBitmap(Cor3D.ActualWidth, Cor3D.ActualHeight, 96, 96, PixelFormats.Pbgra32)
            RTB.Render(Cor3D)
            If ToClipboard Then
                Clipboard.SetImage(RTB)
                Dim D As New Dialog("فایل در Clipboard کپی شد.", "Copied To Clipboard !")
                D.ShowDialog()
            Else
                Dim PNG As New PngBitmapEncoder()
                PNG.Frames.Add(BitmapFrame.Create(RTB))

                Using STRM As Stream = File.Create(My.Computer.FileSystem.SpecialDirectories.Desktop + "/akptest.png")
                    PNG.Save(STRM)
                End Using
                Dim D As New Dialog("فایل در دسکتاپ ذخیره شد.", "File Saved In Desktop")
                D.ShowDialog()
            End If
        ElseIf DrawMode = "2D" Then
            Dim RTB As New RenderTargetBitmap(Cor2D_Front.ActualWidth, Cor2D_Front.ActualHeight, 96, 96, PixelFormats.Pbgra32)
            RTB.Render(Cor2D_Behind)
            RTB.Render(Cor2D_Front)
            If ToClipboard Then
                Clipboard.SetImage(RTB)
                Dim D As New Dialog("فایل در Clipboard کپی شد.", "Copied To Clipboard !")
                D.ShowDialog()
            Else
                Dim PNG As New PngBitmapEncoder()
                PNG.Frames.Add(BitmapFrame.Create(RTB))

                Using STRM As Stream = File.Create(My.Computer.FileSystem.SpecialDirectories.Desktop + "/akptest.png")
                    PNG.Save(STRM)
                End Using
                Dim D As New Dialog("فایل در دسکتاپ ذخیره شد.", "File Saved In Desktop")
                D.ShowDialog()
            End If
        End If
    End Sub

#End Region

#Region "تنظیمات دوربین و حرکت دستگاه مختصات"

    Public WithEvents RotateTransformDT As New DispatcherTimer With {.Interval = TimeSpan.FromMilliseconds(10)}
    Public RotateTransformAxis As Vector3D = New Vector3D(0, 1, 0)
    Public RotateTransformSpeed As Double = 1

    'مقادیر زوم
    Public Property SemiZoom As Integer = 5
    Public Property Zoom As Integer = 0

    'مقادیر موس
    Public Property IsMousePressed As Boolean = False
    Public Property MouseLoc As New Point

    'حالت زوم 3 بعدی
    Private m_ZoomMode As ZoomModes3D = ZoomModes3D.Viewport
    Public Property ZoomMode3D As ZoomModes3D
        Get
            Return m_ZoomMode
        End Get
        Set(value As ZoomModes3D)
            m_ZoomMode = value
            NotifyPropertyChanged("ZoomMode3D")
        End Set
    End Property
    Public Enum ZoomModes3D
        Viewport = 1
        Value = 2
    End Enum
    Private Sub SwitchZM(sender As Object, e As RoutedEventArgs)
        If ZoomMode3D = ZoomModes3D.Viewport Then
            ZoomMode3D = ZoomModes3D.Value
        ElseIf ZoomMode3D = ZoomModes3D.Value Then
            ZoomMode3D = ZoomModes3D.Viewport
        End If
    End Sub

    'حرکت با موس
    Private Sub Cor_MouseMove(sender As Object, e As MouseEventArgs)
        If IsMousePressed Then
            If DrawMode = "3D" Or DrawMode = "4D" Then

                If Rotate4D Then
                    Dim Pos As Point = Mouse.GetPosition(CorMain)
                    Dim ActualPos As New Point(Pos.X - CorMain.ActualWidth / 2, CorMain.ActualHeight / 2 - Pos.Y)
                    Dim Dx As Double = ActualPos.X - MouseLoc.X

                    Dim T As Double = 2 * Math.PI * Dx / CorMain.ActualWidth / 20

                    Dim O As Point4D = Proj4D.From4D
                    Dim Ox As Double = O.X
                    Dim Oy As Double = O.Y
                    Dim Oz As Double = O.Z
                    Dim Ow As Double = O.W

                    Dim S As Double = Math.Sin(T)
                    Dim C As Double = Math.Cos(T)

                    Dim Xn As Double
                    Dim Yn As Double
                    Dim Zn As Double
                    Dim Wn As Double

                    If R4D_XY.IsChecked Then
                        Xn = Ox * C - Oy * S
                        Yn = Ox * S + Oy * C
                        Zn = Oz
                        Wn = Ow

                    ElseIf R4D_YZ.IsChecked Then
                        Xn = Ox
                        Yn = Oy * C - Oz * S
                        Zn = Oy * S + Oz * C
                        Wn = Ow

                    ElseIf R4D_XZ.IsChecked Then
                        Xn = Ox * C + Oz * S
                        Yn = Oy
                        Zn = -Ox * S + Oz * C
                        Wn = Ow

                    ElseIf R4D_XW.IsChecked Then
                        Xn = Ox * C - Ow * S
                        Yn = Oy
                        Zn = Oz
                        Wn = Ox * S + Ow * C

                    ElseIf R4D_YW.IsChecked Then
                        Xn = Ox
                        Yn = Oy * C + Ow * S
                        Zn = Oz
                        Wn = -Oy * S + Ow * C

                    ElseIf R4D_ZW.IsChecked Then
                        Xn = Ox
                        Yn = Oy
                        Zn = Oz * C + Ow * S
                        Wn = -Oz * S + Ow * C
                    End If

                    Proj4D.From4D = New Point4D(Xn, Yn, Zn, Wn)
                    Draw4D()
                Else
                    Dim Pos As Point = Mouse.GetPosition(CorMain)
                    Dim ActualPos As New Point(Pos.X - CorMain.ActualWidth / 2, CorMain.ActualHeight / 2 - Pos.Y)
                    Dim Dx As Double = ActualPos.X - MouseLoc.X
                    Dim Dy As Double = ActualPos.Y - MouseLoc.Y

                    Dim MouseAngle As Double = 0 ' زاویه موس
                    If Dx <> 0 AndAlso Dy <> 0 Then
                        MouseAngle = Math.Asin(Math.Abs(Dy) / Math.Sqrt(Math.Pow(Dx, 2) + Math.Pow(Dy, 2)))
                        If Dx < 0 AndAlso Dy > 0 Then     ' ناحیه 2
                            MouseAngle += Math.PI / 2
                        ElseIf Dx < 0 AndAlso Dy < 0 Then ' ناحیه 3
                            MouseAngle += Math.PI
                        ElseIf Dx > 0 AndAlso Dy < 0 Then ' ناحیه 4
                            MouseAngle += Math.PI * 1.5
                        End If
                    ElseIf Dx = 0 AndAlso Dy <> 0 Then    ' زاویه 90 یا 270
                        MouseAngle = If(Math.Sign(Dy) > 0, Math.PI / 2, Math.PI * 1.5)
                    ElseIf Dx <> 0 AndAlso Dy = 0 Then    ' زاویه 0 یا 180
                        MouseAngle = If(Math.Sign(Dx) > 0, 0, Math.PI)
                    End If

                    Dim AxisAngle As Double = MouseAngle + Math.PI / 2 ' زاویه محور
                    Dim Axis As New Vector3D(Math.Cos(AxisAngle) * 4, Math.Sin(AxisAngle) * 4, 0) ' تبدیل زاویه به بردار
                    Dim Rotation As Double = 0.005 * Math.Sqrt(Math.Pow(Dx, 2) + Math.Pow(Dy, 2)) ' برایند طول و عرض
                    Dim R As New QuaternionRotation3D(New Quaternion(Axis, MathTools.RadianToDegree(Rotation)))
                    Transform3D.Children.Add(New RotateTransform3D(R))

                    MouseLoc = ActualPos

                    UpdateOverlay()
                End If
            ElseIf DrawMode = "2D" Then

                Dim Pos As Point = Mouse.GetPosition(CorMain)
                Dim ActualPos As New Point(Pos.X - CorMain.ActualWidth / 2, CorMain.ActualHeight / 2 - Pos.Y)
                Dim Dx As Double = ActualPos.X - MouseLoc.X
                Dim Dy As Double = -(ActualPos.Y - MouseLoc.Y)

                MovedCamera2D += New Vector(Dx, Dy)
                IsCor2DModified = True
                AutoRedraw()
                MouseLoc = ActualPos
            End If
        End If
    End Sub
    Private Sub Cor_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            IsMousePressed = True
            Dim Pos As Point = Mouse.GetPosition(CorMain)
            MouseLoc = New Point(Pos.X - CorMain.ActualWidth / 2, CorMain.ActualHeight / 2 - Pos.Y)
        End If
        If DrawMode = "3D" Or DrawMode = "4D" Then
            DrawCoordinateCube()
        End If
    End Sub
    Private Sub Cor_MouseUp(sender As Object, e As MouseButtonEventArgs)
        IsMousePressed = False
        If DrawMode = "3D" Or DrawMode = "4D" Then
            VisualCorCube.Children.Clear()
        End If
    End Sub

    'حرکت با دسته بازی
    Public WithEvents Joystick As New XboxControllerBeta(0)
    Private JoystickFristTime As Boolean = True
    Public Sub JoystickChanged() Handles Joystick.StateChanged
        If Joystick.IsConnected Then
            If JoystickFristTime Then
                SystemSounds.Hand.Play()
                'Joystick.IntroVibrate()
                CorInfo.Text = "Joystick Control Detected."
                JoystickFristTime = False
            Else
                'Draw Mode Changing
                If Joystick.IsStartPressed Then
                    SwitchDM()
                End If
                If Joystick.IsBackPressed Then
                    ReverseSwitchDM()
                End If
            End If
        Else
            JoystickFristTime = True
        End If
    End Sub

    Public Sub JoystickLeftTS(sender As Object, e As XboxControllerMovmentEventArgs) Handles Joystick.LeftThumbStickMoving
        If DrawMode = "3D" Or DrawMode = "4D" Then
            Dim Dx As Double = (e.X - 128) / 32767 * 10
            Dim Dy As Double = (e.Y - 128) / 32767 * 10
            Dim JoyTSAngle As Double = 0 'Math.Atan2(Dy, Dx)
            If Dx <> 0 AndAlso Dy <> 0 Then
                JoyTSAngle = Math.Asin(Math.Abs(Dy) / Math.Sqrt(Math.Pow(Dx, 2) + Math.Pow(Dy, 2)))
                If Dx < 0 AndAlso Dy > 0 Then     ' ناحیه 2
                    JoyTSAngle += Math.PI / 2
                ElseIf Dx < 0 AndAlso Dy < 0 Then ' ناحیه 3
                    JoyTSAngle += Math.PI
                ElseIf Dx > 0 AndAlso Dy < 0 Then ' ناحیه 4
                    JoyTSAngle += Math.PI * 1.5
                End If
            ElseIf Dx = 0 AndAlso Dy <> 0 Then    ' زاویه 90 یا 270
                JoyTSAngle = If(Math.Sign(Dy) > 0, Math.PI / 2, Math.PI * 1.5)
            ElseIf Dx <> 0 AndAlso Dy = 0 Then    ' زاویه 0 یا 180
                JoyTSAngle = If(Math.Sign(Dx) > 0, 0, Math.PI)
            End If

            Dim AxisAngle As Double = JoyTSAngle + Math.PI / 2 ' زاویه محور
            Dim Axis As New Vector3D(Math.Cos(AxisAngle) * 4, Math.Sin(AxisAngle) * 4, 0) ' تبدیل زاویه به بردار
            Dim Rotation As Double = 0.005 * Math.Sqrt(Math.Pow(Dx, 2) + Math.Pow(Dy, 2)) ' برایند طول و عرض
            Dim R As New QuaternionRotation3D(New Quaternion(Axis, MathTools.RadianToDegree(Rotation)))
            Transform3D.Children.Add(New RotateTransform3D(R))
            UpdateOverlay()
        ElseIf DrawMode = "2D" Then
            Dim Dx As Double = -(e.X - 128) / 32767 * 10
            Dim Dy As Double = (e.Y - 128) / 32767 * 10

            MovedCamera2D += New Vector(Dx, Dy)
            IsCor2DModified = True
            AutoRedraw()
        End If
    End Sub

    Public Sub JoystickRightTS(sender As Object, e As XboxControllerMovmentEventArgs) Handles Joystick.RightThumbStickMoving
        If DrawMode = "4D" Then
            Dim Dx As Double = (e.X - 128) / 32767
            Dim Dy As Double = (e.Y - 128) / 32767

            Dim T As Double = Math.PI * Dx / 100

            Dim O As Point4D = Proj4D.From4D
            Dim Ox As Double = O.X
            Dim Oy As Double = O.Y
            Dim Oz As Double = O.Z
            Dim Ow As Double = O.W

            Dim S As Double = Math.Sin(T)
            Dim C As Double = Math.Cos(T)

            Dim Xn As Double
            Dim Yn As Double
            Dim Zn As Double
            Dim Wn As Double
            If R4D_XY.IsChecked Then
                Xn = Ox * C - Oy * S
                Yn = Ox * S + Oy * C
                Zn = Oz
                Wn = Ow
            ElseIf R4D_YZ.IsChecked Then
                Xn = Ox
                Yn = Oy * C - Oz * S
                Zn = Oy * S + Oz * C
                Wn = Ow
            ElseIf R4D_XZ.IsChecked Then
                Xn = Ox * C + Oz * S
                Yn = Oy
                Zn = -Ox * S + Oz * C
                Wn = Ow
            ElseIf R4D_XW.IsChecked Then
                Xn = Ox * C - Ow * S
                Yn = Oy
                Zn = Oz
                Wn = Ox * S + Ow * C
            ElseIf R4D_YW.IsChecked Then
                Xn = Ox
                Yn = Oy * C + Ow * S
                Zn = Oz
                Wn = -Oy * S + Ow * C
            ElseIf R4D_ZW.IsChecked Then
                Xn = Ox
                Yn = Oy
                Zn = Oz * C + Ow * S
                Wn = -Oz * S + Ow * C
            End If
            Proj4D.From4D = New Point4D(Xn, Yn, Zn, Wn)
            Draw4D()
        End If
    End Sub

    'حرکت با کیبورد - فقط 2 بعدی
    Private Sub App_KeyDown(sender As Object, e As KeyEventArgs)
        If DrawMode = "3D" Then

        ElseIf DrawMode = "2D" Then
            Dim MoveSpeed = 10
            If e.Key = Key.Up Then
                e.Handled = True
                MovedCamera2D += New Vector(0, MoveSpeed)
                IsCor2DModified = True
                AutoRedraw()
            End If
            If e.Key = Key.Down Then
                e.Handled = True
                MovedCamera2D += New Vector(0, -MoveSpeed)
                IsCor2DModified = True
                AutoRedraw()
            End If
            If e.Key = Key.Right Then
                e.Handled = True
                MovedCamera2D += New Vector(-MoveSpeed, 0)
                IsCor2DModified = True
                AutoRedraw()
            End If
            If e.Key = Key.Left Then
                e.Handled = True
                MovedCamera2D += New Vector(MoveSpeed, 0)
                IsCor2DModified = True
                AutoRedraw()
            End If
        End If
    End Sub

    'زوم با اسکرول موس
    Private Sub Cor_MouseWheel(sender As Object, e As MouseWheelEventArgs)
        If DrawMode = "4D" Then
            If ZoomMode3D = ZoomModes3D.Viewport Then
                Camera4D.Position = New Point3D(Camera4D.Position.X, Camera4D.Position.Y, Camera4D.Position.Z - (e.Delta / 48))
                UpdateOverlay()
            ElseIf ZoomMode3D = ZoomModes3D.Value Then
                Dim S As Double = e.Delta / 120
                If S < 0 Then
                    Zoom += 1
                    Len3DX = GetZoomValue(Zoom)
                    Len3DY = GetZoomValue(Zoom)
                    Len3DZ = GetZoomValue(Zoom)
                    IsCor3DModified = True
                    AutoRedraw()
                Else
                    Zoom -= 1
                    Len3DX = GetZoomValue(Zoom)
                    Len3DY = GetZoomValue(Zoom)
                    Len3DZ = GetZoomValue(Zoom)
                    IsCor3DModified = True
                    AutoRedraw()
                End If
            End If
        ElseIf DrawMode = "3D" Then
            If ZoomMode3D = ZoomModes3D.Viewport Then
                Camera3D.Position = New Point3D(Camera3D.Position.X, Camera3D.Position.Y, Camera3D.Position.Z - (e.Delta / 48))
                UpdateOverlay()
            ElseIf ZoomMode3D = ZoomModes3D.Value Then
                Dim S As Double = e.Delta / 120
                If S < 0 Then
                    Zoom += 1
                    Len3DX = GetZoomValue(Zoom)
                    Len3DY = GetZoomValue(Zoom)
                    Len3DZ = GetZoomValue(Zoom)
                    IsCor3DModified = True
                    AutoRedraw()
                Else
                    Zoom -= 1
                    Len3DX = GetZoomValue(Zoom)
                    Len3DY = GetZoomValue(Zoom)
                    Len3DZ = GetZoomValue(Zoom)
                    IsCor3DModified = True
                    AutoRedraw()
                End If
            End If
        ElseIf DrawMode = "2D" Then
            Dim S As Double = e.Delta / 120
            If S < 0 Then
                SemiZoom += 1
                If SemiZoom > 10 Then
                    SemiZoom = 5
                    Zoom += 1
                    Len = GetZoomValue(Zoom)
                    LenScale = 100
                Else
                    LenScale -= 10
                End If
                IsCor2DModified = True
                AutoRedraw()
            Else
                SemiZoom -= 1
                If SemiZoom < 0 Then
                    SemiZoom = 5
                    Zoom -= 1
                    Len = GetZoomValue(Zoom)
                    LenScale = 100
                Else
                    LenScale += 10
                End If
                IsCor2DModified = True
                AutoRedraw()
            End If
        End If
    End Sub

    ' زوم با دکمه
    Private Sub ZoomIn(sender As Object, e As RoutedEventArgs)
        If DrawMode = "2D" Then
            Zoom -= 1
            Len = GetZoomValue(Zoom)
            IsCor2DModified = True
            AutoRedraw()
        ElseIf DrawMode = "3D" Then
            Zoom -= 1
            Len3DX = GetZoomValue(Zoom)
            Len3DY = GetZoomValue(Zoom)
            Len3DZ = GetZoomValue(Zoom)
            IsCor3DModified = True
            AutoRedraw()
        End If
    End Sub
    Private Sub ZoomHome(sender As Object, e As RoutedEventArgs)
        If DrawMode = "2D" Then
            Zoom = 0
            Len = GetZoomValue(0)
            MovedCamera2D = New Vector(0, 0)
            IsCor2DModified = True
            AutoRedraw()
        ElseIf DrawMode = "3D" Then
            Zoom = 0
            Len = GetZoomValue(0)
            MovedCamera2D = New Vector(0, 0)
            IsCor3DModified = True
            AutoRedraw()
        End If
    End Sub
    Private Sub ZoomOut(sender As Object, e As RoutedEventArgs)
        If DrawMode = "2D" Then
            Zoom += 1
            Len = GetZoomValue(Zoom)
            IsCor2DModified = True
            AutoRedraw()
        ElseIf DrawMode = "3D" Then
            Zoom += 1
            Len3DX = GetZoomValue(Zoom)
            Len3DY = GetZoomValue(Zoom)
            Len3DZ = GetZoomValue(Zoom)
            IsCor3DModified = True
            AutoRedraw()
        End If
    End Sub

    'محاسبه زوم
    Private Function GetZoomValue(ZoomLevel As Integer) As Double
        Dim MB As Integer = ZoomLevel Mod 3
        Dim IP As Integer = ZoomLevel \ 3
        If MB >= 0 Then
            Select Case MB
                Case 0
                    Return 1 * (10 ^ IP)
                Case 1
                    Return 2 * (10 ^ IP)
                Case 2
                    Return 5 * (10 ^ IP)
            End Select
        Else
            Select Case MB
                Case -1
                    Return 5 * (10 ^ (IP - 1))
                Case -2
                    Return 2 * (10 ^ (IP - 1))
                Case 0
                    Return 1 * (10 ^ (IP - 1))
            End Select
        End If
        Return 0
    End Function

    Private Sub RotateAnim() Handles RotateTransformDT.Tick
        If Rotate4D Then
            Dim T As Double = MathTools.DegreeToRadian(RotateTransformSpeed)

            Dim O As Point4D = Proj4D.From4D
            Dim Ox As Double = O.X
            Dim Oy As Double = O.Y
            Dim Oz As Double = O.Z
            Dim Ow As Double = O.W

            Dim S As Double = Math.Sin(T)
            Dim C As Double = Math.Cos(T)

            Dim Xn As Double
            Dim Yn As Double
            Dim Zn As Double
            Dim Wn As Double

            If R4D_XY.IsChecked Then
                Xn = Ox * C - Oy * S
                Yn = Ox * S + Oy * C
                Zn = Oz
                Wn = Ow

            ElseIf R4D_YZ.IsChecked Then
                Xn = Ox
                Yn = Oy * C - Oz * S
                Zn = Oy * S + Oz * C
                Wn = Ow

            ElseIf R4D_XZ.IsChecked Then
                Xn = Ox * C + Oz * S
                Yn = Oy
                Zn = -Ox * S + Oz * C
                Wn = Ow

            ElseIf R4D_XW.IsChecked Then
                Xn = Ox * C - Ow * S
                Yn = Oy
                Zn = Oz
                Wn = Ox * S + Ow * C

            ElseIf R4D_YW.IsChecked Then
                Xn = Ox
                Yn = Oy * C + Ow * S
                Zn = Oz
                Wn = -Oy * S + Ow * C

            ElseIf R4D_ZW.IsChecked Then
                Xn = Ox
                Yn = Oy
                Zn = Oz * C + Ow * S
                Wn = -Oz * S + Ow * C
            End If

            Proj4D.From4D = New Point4D(Xn, Yn, Zn, Wn)
            Draw4D()
        Else
            Dim R As New QuaternionRotation3D(New Quaternion(RotateTransformAxis, RotateTransformSpeed))
            Transform3D.Children.Add(New RotateTransform3D(R))
        End If
    End Sub

#End Region

#Region "دستورات"

    'اضافه کردن
    Public Shared AddCommand As New RoutedCommand
    Public Sub AddCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub
    Public Sub AddEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        AddFunction()
    End Sub

    'رسم
    Public Shared DrawCommand As New RoutedCommand
    Public Sub DrawCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub
    Public Sub DrawEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim Trg As Object = e.Parameter
        Eq_ListBox.Items.Refresh()
        Eq_ListBox.UnselectAll()
        AutoRedraw()
    End Sub

    'تازه سازی
    Public Shared RefreshCommand As New RoutedCommand
    Public Sub RefreshCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub
    Public Sub RefreshEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        AutoRedraw()
    End Sub

    'تغییر قلمو - رنگ
    Public Shared SetSolidColorBrushCommand As New RoutedCommand
    Public Sub SetSolidColorBrushCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub
    Public Sub SetSolidColorBrushEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim Trg As MathObject = TryCast(e.Parameter, MathObject)
        Trg.IsOptionOpen = Not Trg.IsOptionOpen

        Dim CD As New System.Windows.Forms.ColorDialog
        CD.AnyColor = True
        Dim DR As System.Windows.Forms.DialogResult = CD.ShowDialog()
        If DR = Forms.DialogResult.OK Then
            Dim Col As New Color
            Col.R = CD.Color.R
            Col.G = CD.Color.G
            Col.B = CD.Color.B
            Col.A = CD.Color.A
            If Trg.Type = 0 Or Trg.Type = 4 Then 'Set Brush
                Dim B As New SolidColorBrush(Col)
                Trg.Inner.Brush = B
            Else                                 'Set Color
                Trg.Inner.Color = Col
            End If
        End If
        AutoRedraw()
    End Sub

    'تغییر قلمو - تصویر
    Public Shared SetImageBrushCommand As New RoutedCommand
    Public Sub SetImageBrushCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub
    Public Sub SetImageBrushEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim Trg As MathObject = TryCast(e.Parameter, MathObject)
        Trg.IsOptionOpen = Not Trg.IsOptionOpen

        Dim OFD As New OpenFileDialog
        OFD.AddExtension = True
        OFD.Filter = "All Image Files|*.jpg;*.png|JPEG Image|*.jpg|PNG Image|*.png|All files|*.*"
        Dim DR As Boolean? = OFD.ShowDialog()
        If DR = True Then
            Dim ImgB As New ImageBrush(New BitmapImage(New Uri(OFD.FileName, UriKind.Absolute)))
            If Trg.Type = 0 Or Trg.Type = 4 Then
                Trg.Inner.Brush = ImgB
            Else
                Throw New Exception("Cant Use Image Brush On MathObject Type:" & Trg.Type & " #AKP")
            End If
        End If
        AutoRedraw()
    End Sub


    'باز کردن پنجره تنظیمات
    Public Shared OpenOptionCommand As New RoutedCommand
    Public Sub OpenOptionCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub
    Public Sub OpenOptionEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim Trg As Object = e.Parameter
        Trg.IsOptionOpen = Not Trg.IsOptionOpen
    End Sub

    'کپی
    Public Shared CopyCommand As New RoutedCommand
    Public Sub CopyCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = False
    End Sub
    Public Sub CopyEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim D As New Dialog("در دست ساخت ...")
        D.ShowDialog()
    End Sub

    'تغییر قابل رویت بودن
    Public Shared ChangeVisibilityCommand As New RoutedCommand
    Public Sub ChangeVisibilityCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub
    Public Sub ChangeVisibilityEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim Trg As MathObject = TryCast(e.Parameter, MathObject)
        If Trg.Type <> 1 And Trg.Type <> 99 Then
            Trg.Inner.IsVisible = Not Trg.Inner.IsVisible
        End If
        AutoRedraw()
    End Sub

    'حذف
    Public Shared DeleteCommand As New RoutedCommand
    Public Sub DeleteCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        If Eq_ListBox.Items.Count > 2 Then
            e.CanExecute = True
        Else
            e.CanExecute = False
        End If
    End Sub
    Public Sub DeleteEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim Trg As Object = e.Parameter
        For Each MO As MathObject In MathObjList
            If MO.Equals(Trg) Then
                If MO.Type <> 1 Then
                    If DrawMode = "2D" Then
                        For Each GO In MO.Inner.GraphObject
                            Cor2D_Front.Children.Remove(GO)
                        Next
                    ElseIf DrawMode = "3D" Then
                        For Each GO In MO.Inner.GraphObject
                            VisualModel.Children.Remove(GO)
                        Next
                    End If
                End If
                MathObjList.Remove(MO)
                Exit For
            End If
        Next
        Eq_ListBox.Items.Refresh()
        AutoRedraw()
    End Sub

    ' تغییر اتوماتیک متغییر
    Public Shared PlayVarCommand As New RoutedCommand
    Public Sub PlayVarCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        Dim Trg As Object = e.Parameter
        Dim Bool As Boolean = TryCast(Trg, Variable).Playing
        e.CanExecute = Not Bool
    End Sub
    Public Sub PlayVarEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim Trg As Object = e.Parameter
        Dim Var As Variable = TryCast(Trg, Variable)
        Var.Play()
    End Sub

    'توقف تغییر اتوماتیک متغییر
    Public Shared PauseVarCommand As New RoutedCommand
    Public Sub PauseVarCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        Dim Trg As Object = e.Parameter
        Dim Bool As Boolean = TryCast(Trg, Variable).Playing
        e.CanExecute = Bool
    End Sub
    Public Sub PauseVarEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim Trg As Object = e.Parameter
        Dim Var As Variable = TryCast(Trg, Variable)
        Var.Pause()
    End Sub

    'دستورات هنگام تغییر متن معادله
    Public Shared EqExpChangedCommand As New RoutedCommand
    Public Sub EqExpChangedCE(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub
    Public Sub EqExpChangedEX(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        For Each MO As MathObject In MathObjList
            If MO.Type = 1 Then TryCast(MO.Inner, Variable).Pause()
        Next
    End Sub

#End Region

#Region "ورود داده از فایل"

    'بارگذاری از فایل
    Private Sub OpenFile(sender As Object, e As RoutedEventArgs)
        ReadFromFile()
    End Sub
    Private Sub ReadFromFile(Optional FileDir As String = "")
        Dim OFD As New OpenFileDialog
        OFD.Filter = "Math Graph File|*.MGP"

        Dim DR As Boolean
        If FileDir = "" Then DR = OFD.ShowDialog
        If DR = True Or FileDir <> "" Then
            If FileDir = "" Then FileDir = OFD.FileName
            Dim Ext As String = IO.Path.GetExtension(FileDir)
            If Ext = ".mgp" Then
                Dim FileText As String = File.ReadAllText(FileDir)
                Dim MGPI As New MGPImporter(FileText)
                MGPI.BeginImport()
            ElseIf Ext = ".mgpb" Then
                Dim D As New Dialog("این فرمت فعلا پشتیبانی نمی شود.", "Binary File Not Supported.")
                D.ShowDialog()
            ElseIf Ext = ".mgpx" Then
                Dim D As New Dialog("این فرمت فعلا پشتیبانی نمی شود.", "XML File Not Supported.")
                D.ShowDialog()
            Else
                Dim D As New Dialog("فایل غیر قابل قبول", "Error")
                D.ShowDialog()
            End If
        End If
    End Sub
    'بار گذاری از ریسورس
    Private Sub OpenFromResource(sender As Object, e As RoutedEventArgs)
        ReadFromResource(InputBox("Enter Key !") & ".mgp")
    End Sub
    Public Sub ReadFromResource(ResourceKey As String)
        Dim Assembly_1 As Assembly = Assembly.GetExecutingAssembly()
        Dim ResourceName = "AKP_Math_Graph_Plotter." & ResourceKey

        Dim ResourceText As String
        Try
            Using Stream As Stream = Assembly_1.GetManifestResourceStream(ResourceName)
                Using Reader As New StreamReader(Stream)
                    ResourceText = Reader.ReadToEnd()
                End Using
            End Using
        Catch
            MsgBox("Resource Must Be Embedded !")
            Return
        End Try

        Dim MGPI As New MGPImporter(ResourceText)
        MGPI.BeginImport()
    End Sub
    'آغاز تایمر های مورد نیاز
    Public Sub AutoplayVariables()
        For Each MO As MathObject In MathObjList
            If MO.Type = 1 Then
                Dim Inner As Variable = TryCast(MO.Inner, Variable)
                If Inner.Autoplay Then
                    TryCast(MO.Inner, Variable).Play()
                End If
            End If
        Next
    End Sub

#End Region

#Region "ذخیره داده در فایل"

    'ذخیره در فایل
    Private Sub SaveFile(sender As Object, e As RoutedEventArgs)
        WriteToFile()
    End Sub
    Private Sub SaveFileSerial(sender As Object, e As RoutedEventArgs)
        Dim SFD As New SaveFileDialog
        SFD.AddExtension = True
        SFD.CheckPathExists = True
        SFD.Filter = "Math Graph File (XML) |*.mgpx"     '"|MGPB File - Binary|*.mgpb|MGPX File - Xml Based|*.mgpx"
        SFD.OverwritePrompt = True
        If SFD.ShowDialog = True Then
            Dim FileDir = SFD.FileName
            Dim Ext As String = IO.Path.GetExtension(FileDir)
            If Ext = ".mgpx" Then
                Dim Xs As New XmlSerializer(GetType(ObservableCollection(Of MathObject)))
                Using Wr As New StreamWriter(FileDir)
                    Xs.Serialize(Wr, MathObjList)
                End Using
            Else
                Dim D As New Dialog("فرمت فایل غیر قابل قبول است", "Error")
                D.ShowDialog()
            End If
        End If

    End Sub
    Private Sub WriteToFile(Optional FileDir As String = "")
        Dim SFD As New SaveFileDialog
        SFD.AddExtension = True
        SFD.CheckPathExists = True
        SFD.Filter = "Math Graph File|*.mgp"     '"|MGPB File - Binary|*.mgpb|MGPX File - Xml Based|*.mgpx"
        SFD.OverwritePrompt = True

        Dim DR As Boolean
        If FileDir = "" Then DR = SFD.ShowDialog
        If DR = True Or FileDir <> "" Then
            If FileDir = "" Then FileDir = SFD.FileName
            Dim Ext As String = IO.Path.GetExtension(FileDir)
            If Ext = ".mgp" Then
                Dim D As New Dialog("آیا مایل به ذخیره تنظیمات رسم هستید؟", "", True)
                D.OKButtonString = "بله"
                D.CancelButtonString = "خیر"
                Dim DRes As Boolean? = D.ShowDialog()
                Dim MGPE As MGPExporter
                If DRes = True Then
                    MGPE = New MGPExporter(MathObjList, True)
                ElseIf DRes = False Then
                    MGPE = New MGPExporter(MathObjList, False)
                Else
                    Return
                End If
                MGPE.WriteToFile(FileDir)
            Else
                Dim D As New Dialog("فرمت فایل غیر قابل قبول است", "Error")
                D.ShowDialog()
            End If
        End If
    End Sub

#End Region


#Region "ابزار های عیب یابی"

    Public Property DebugLevel As Integer = 0
    Public Sub DebugMsg(Msg As String, Level As Integer)
        If DebugLevel >= Level Then AKPTerminal.LogWriteLine(Msg)
    End Sub

    Public Sub DoEvents()
        Dim Frame As New DispatcherFrame()
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, New DispatcherOperationCallback(AddressOf ExitFrame), Frame)
        Dispatcher.PushFrame(Frame)
    End Sub
    Public Function ExitFrame(F As Object) As Object
        DirectCast(F, DispatcherFrame).[Continue] = False
        Return Nothing
    End Function

    Private Sub DrawGraph3DDebugMode(Equation As Equation)
        'Try
        EquationTools.DetectEqType(Equation, Me.DrawMode)
        If Equation.Type <> EquationType.ParametricU Then
            For Each PP As Point3DPlane In GetGraphSurface3D(Equation, False)
                Dim MatGroup As New MaterialGroup
                'Select Case ColoringMethod
                '    Case 0

                '        Dim MatD As New DiffuseMaterial(Equation.Brush)
                '        'Dim MatE As New EmissiveMaterial(Equation.Brush)
                '        MatGroup.Children.Add(MatD)
                '        If Equation.Material = PlaneMaterial.Specular Then
                '            Dim MatS As New SpecularMaterial(Equation.Brush, 30)
                '            MatGroup.Children.Add(MatS)
                '        End If
                '    Case 1
                '        Dim MatB As New DiffuseMaterial(New ImageBrush(CreateMeshBrush(PP)))
                '        MatGroup.Children.Add(MatB)
                'End Select
                Select Case Equation.Material

                    Case PlaneMaterial.Diffuse
                        Dim MatD As New DiffuseMaterial(Equation.Brush)
                        MatGroup.Children.Add(MatD)

                    Case PlaneMaterial.Specular
                        Dim MatD As New DiffuseMaterial(Equation.Brush)
                        Dim MatS As New SpecularMaterial(Equation.Brush, 30)
                        MatGroup.Children.Add(MatD)
                        MatGroup.Children.Add(MatS)

                    Case PlaneMaterial.Emissive
                        Dim MatD As New DiffuseMaterial(Equation.Brush)
                        Dim MatE As New EmissiveMaterial(Equation.Brush)
                        MatGroup.Children.Add(MatD)
                        MatGroup.Children.Add(MatE)

                    Case PlaneMaterial.ByCor
                        Dim MatB As New DiffuseMaterial(New ImageBrush(CreateMeshBrush(PP)))
                        MatGroup.Children.Add(MatB)

                End Select

                Dim MeshF As MeshGeometry3D = CreateDebugFront(PP)
                Dim MeshB As MeshGeometry3D = CreateDebugRear(PP)
                Dim GeoF As New GeometryModel3D(MeshF, MatGroup)
                Dim GeoB As New GeometryModel3D(MeshB, MatGroup)
                Equation.GraphObject.Add(GeoF)
                Equation.GraphObject.Add(GeoB)
                VisualModel.Children.Add(GeoF)
                VisualModel.Children.Add(GeoB)
                Equation.NeedsRecalculation = False
            Next
        Else
            'TODO : Fix Coloring Method
            For Each PC As Point3DCollection In GetGraphLine3D(Equation)
                Dim MatGroup As New MaterialGroup
                'Select Case ColoringMethod
                '    Case 0
                Dim SCBrush As New SolidColorBrush
                If Equation.Brush.GetType Is GetType(SolidColorBrush) Then
                    SCBrush = Equation.Brush
                Else
                    Equation.HasError = True
                    Equation.ErrorMsg = "لطفا از قلمو ی رنگ جامد استفاده کنید."
                End If

                Dim WPolyLine As New WirePolyline
                WPolyLine.Color = SCBrush.Color
                WPolyLine.Thickness = Equation.Thickness
                WPolyLine.Points = PC

                'Case 1
                '    Dim MatB As New DiffuseMaterial(New ImageBrush(CreateMeshBrush(PP)))
                '    MatGroup.Children.Add(MatB)

                VisualWire.Children.Add(WPolyLine)
                'Equation.NeedsRecalculation = False
            Next
        End If
        'Catch Ex As Exception
        '    Equation.HasError = True
        '    Equation.ErrorMsg = "خطا در رسم" & vbCrLf & Ex.Message
        'End Try
    End Sub
    'عیب یابی 3 بعدی
    Public Function CreateDebugFront(Plane As Point3DPlane) As MeshGeometry3D

        Dim Mesh As New MeshGeometry3D
        Dim Points As Point3DCollection = Plane.PointCollection
        Dim Cols As Integer = Plane.Columns
        Dim Rows As Integer = Plane.Rows

        Mesh.Positions = Points

        For i = 0 To Rows - 2
            For j = 0 To Cols - 2
                Dim ij As Integer = (i * Cols) + j

                Dim p0 As Integer = ij
                Dim p1 As Integer = ij + 1 + Cols
                Dim p2 As Integer = ij + 1
                Mesh.TriangleIndices.Add(p0)
                Mesh.TriangleIndices.Add(p1)
                Mesh.TriangleIndices.Add(p2)
                Dim N As Vector3D = CalculateNormal(Points(p0), Points(p1), Points(p2))
                N.Normalize()
                Mesh.Normals.Add(N)
                Mesh.Normals.Add(N)
                Mesh.Normals.Add(N)
                Dim WIRE As New WireLine
                WIRE.Color = Colors.Green
                WIRE.Thickness = 1
                WIRE.Point1 = Points(p0)
                WIRE.Point2 = Points(p0) + N
                VisualDebug.Children.Add(WIRE)
                Mesh.TextureCoordinates.Add(New Point((i / (Rows - 1)), (j / (Cols - 1))))
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Rows - 1)), (j / (Cols - 1))))
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Cols - 1)), ((j + 1) / (Rows - 1))))

                Dim p3 As Integer = ij + 1 + Cols
                Dim p4 As Integer = ij
                Dim p5 As Integer = ij + Cols
                Mesh.TriangleIndices.Add(p3)
                Mesh.TriangleIndices.Add(p4)
                Mesh.TriangleIndices.Add(p5)
                Dim N2 As Vector3D = CalculateNormal(Points(p3), Points(p4), Points(p5))
                N2.Normalize()
                Mesh.Normals.Add(N2)
                Mesh.Normals.Add(N2)
                Mesh.Normals.Add(N2)
                Dim WIRE2 As New WireLine
                WIRE2.Color = Colors.Blue
                WIRE2.Thickness = 1
                WIRE2.Point1 = Points(p3)
                WIRE2.Point2 = Points(p3) + N
                VisualDebug.Children.Add(WIRE2)
                Mesh.TextureCoordinates.Add(New Point((i / (Rows - 1)), (j / (Cols - 1))))
                Mesh.TextureCoordinates.Add(New Point((i / (Rows - 1)), ((j + 1) / (Cols - 1))))
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Rows - 1)), ((j + 1) / (Cols - 1))))
            Next
        Next
        Return Mesh
    End Function
    Public Function CreateDebugRear(Plane As Point3DPlane) As MeshGeometry3D

        Dim Mesh As New MeshGeometry3D
        Dim Points As Point3DCollection = Plane.PointCollection
        Dim Cols As Integer = Plane.Columns
        Dim Rows As Integer = Plane.Rows

        Mesh.Positions = Points

        For i = 0 To Rows - 2
            For j = 0 To Cols - 2

                Dim ij As Integer = (i * Cols) + j

                Dim p0 As Integer = ij
                Dim p1 As Integer = ij + 1 + Cols
                Dim p2 As Integer = ij + 1
                Mesh.TriangleIndices.Add(p2)
                Mesh.TriangleIndices.Add(p1)
                Mesh.TriangleIndices.Add(p0)
                Dim N As Vector3D = -1 * CalculateNormal(Points(p2), Points(p1), Points(p0))
                N.Normalize()
                Mesh.Normals.Add(N)
                Mesh.Normals.Add(N)
                Mesh.Normals.Add(N)
                Dim WIRE3 As New WireLine
                WIRE3.Color = Colors.Pink
                WIRE3.Thickness = 1
                WIRE3.Point1 = Points(p0)
                WIRE3.Point2 = Points(p0) + N
                VisualDebug.Children.Add(WIRE3)
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Rows - 1)), ((j + 1) / (Cols - 1))))
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Rows - 1)), (j / (Cols - 1))))
                Mesh.TextureCoordinates.Add(New Point((i / (Rows - 1)), (j / (Cols - 1))))

                Dim p3 As Integer = ij + 1 + Cols
                Dim p4 As Integer = ij
                Dim p5 As Integer = ij + Cols
                Mesh.TriangleIndices.Add(p5)
                Mesh.TriangleIndices.Add(p4)
                Mesh.TriangleIndices.Add(p3)
                Dim WIRE4 As New WireLine
                WIRE4.Color = Colors.Red
                WIRE4.Thickness = 1
                WIRE4.Point1 = Points(p3)
                WIRE4.Point2 = Points(p3) + N
                VisualDebug.Children.Add(WIRE4)
                Dim N2 As Vector3D = -1 * CalculateNormal(Points(p5), Points(p4), Points(p3))
                N2.Normalize()
                Mesh.Normals.Add(N2)
                Mesh.Normals.Add(N2)
                Mesh.Normals.Add(N2)
                Mesh.TextureCoordinates.Add(New Point(((i + 1) / (Rows - 1)), ((j + 1) / (Cols - 1))))
                Mesh.TextureCoordinates.Add(New Point((i / (Rows - 1)), ((j + 1) / (Cols - 1))))
                Mesh.TextureCoordinates.Add(New Point((i / (Rows - 1)), (j / (Cols - 1))))

            Next
        Next
        Return Mesh
    End Function

#End Region

    Private Sub ChangeCor3DType()
        If Cor3DType = 0 Then
            Cor3DType = 1
        Else
            Cor3DType = 0
        End If
    End Sub





    Private Sub OpenDropDown(sender As Object, e As RoutedEventArgs)
        TryCast(sender, Button).ContextMenu.IsEnabled = True
        TryCast(sender, Button).ContextMenu.PlacementTarget = TryCast(sender, Button)
        TryCast(sender, Button).ContextMenu.Placement = Primitives.PlacementMode.Bottom
        TryCast(sender, Button).ContextMenu.IsOpen = True
    End Sub





    Private Sub SetCorSolidColorBrush(sender As Object, e As RoutedEventArgs)
        DrawSettingPopup.IsOpen = False
        Dim CD As New System.Windows.Forms.ColorDialog
        CD.AnyColor = True
        Dim DR As System.Windows.Forms.DialogResult = CD.ShowDialog()
        If DR = Forms.DialogResult.OK Then
            Dim Col As New Color
            Col.R = CD.Color.R
            Col.G = CD.Color.G
            Col.B = CD.Color.B
            Col.A = CD.Color.A
            CorBackground = New SolidColorBrush(Col)
        End If
    End Sub

    Private Sub SetCorImageBrush(sender As Object, e As RoutedEventArgs)
        DrawSettingPopup.IsOpen = False
        Dim OFD As New OpenFileDialog
        OFD.AddExtension = True
        OFD.Filter = "All Image Files|*.jpg;*.png|JPEG Image|*.jpg|PNG Image|*.png|All files|*.*"
        Dim DR As Boolean? = OFD.ShowDialog()
        If DR = True Then
            Dim ImgB As New ImageBrush(New BitmapImage(New Uri(OFD.FileName, UriKind.Absolute)))
            CorBackground = ImgB
        End If
    End Sub




End Class