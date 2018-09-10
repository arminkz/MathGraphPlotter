Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Input
Imports System.Windows.Controls
Imports System.Windows.Shapes

Public NotInheritable Class Arrow
    Inherits Shape

#Region "Dependency Properties"

    Public Shared ReadOnly X1Property As DependencyProperty = DependencyProperty.Register("X1", GetType(Double), GetType(Arrow), New FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender Or FrameworkPropertyMetadataOptions.AffectsMeasure))
    Public Shared ReadOnly Y1Property As DependencyProperty = DependencyProperty.Register("Y1", GetType(Double), GetType(Arrow), New FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender Or FrameworkPropertyMetadataOptions.AffectsMeasure))
    Public Shared ReadOnly X2Property As DependencyProperty = DependencyProperty.Register("X2", GetType(Double), GetType(Arrow), New FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender Or FrameworkPropertyMetadataOptions.AffectsMeasure))
    Public Shared ReadOnly Y2Property As DependencyProperty = DependencyProperty.Register("Y2", GetType(Double), GetType(Arrow), New FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender Or FrameworkPropertyMetadataOptions.AffectsMeasure))
    Public Shared ReadOnly HeadWidthProperty As DependencyProperty = DependencyProperty.Register("HeadWidth", GetType(Double), GetType(Arrow), New FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender Or FrameworkPropertyMetadataOptions.AffectsMeasure))
    Public Shared ReadOnly HeadHeightProperty As DependencyProperty = DependencyProperty.Register("HeadHeight", GetType(Double), GetType(Arrow), New FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender Or FrameworkPropertyMetadataOptions.AffectsMeasure))

#End Region

#Region "CLR Properties"

    <TypeConverter(GetType(LengthConverter))> _
    Public Property X1() As Double
        Get
            Return CDbl(MyBase.GetValue(X1Property))
        End Get
        Set(value As Double)
            MyBase.SetValue(X1Property, value)
        End Set
    End Property

    <TypeConverter(GetType(LengthConverter))> _
    Public Property Y1() As Double
        Get
            Return CDbl(MyBase.GetValue(Y1Property))
        End Get
        Set(value As Double)
            MyBase.SetValue(Y1Property, value)
        End Set
    End Property

    <TypeConverter(GetType(LengthConverter))> _
    Public Property X2() As Double
        Get
            Return CDbl(MyBase.GetValue(X2Property))
        End Get
        Set(value As Double)
            MyBase.SetValue(X2Property, value)
        End Set
    End Property

    <TypeConverter(GetType(LengthConverter))> _
    Public Property Y2() As Double
        Get
            Return CDbl(MyBase.GetValue(Y2Property))
        End Get
        Set(value As Double)
            MyBase.SetValue(Y2Property, value)
        End Set
    End Property

    <TypeConverter(GetType(LengthConverter))> _
    Public Property HeadWidth() As Double
        Get
            Return CDbl(MyBase.GetValue(HeadWidthProperty))
        End Get
        Set(value As Double)
            MyBase.SetValue(HeadWidthProperty, value)
        End Set
    End Property

    <TypeConverter(GetType(LengthConverter))> _
    Public Property HeadHeight() As Double
        Get
            Return CDbl(MyBase.GetValue(HeadHeightProperty))
        End Get
        Set(value As Double)
            MyBase.SetValue(HeadHeightProperty, value)
        End Set
    End Property

#End Region

#Region "Overrides"

    Protected Overrides ReadOnly Property DefiningGeometry() As Geometry
        Get
            Dim Geometry As New StreamGeometry()
            Geometry.FillRule = FillRule.EvenOdd

            Using Context As StreamGeometryContext = Geometry.Open()
                InternalDrawArrowGeometry(Context)
            End Using

            ' Freeze the geometry for performance benefits
            Geometry.Freeze()

            Return Geometry
        End Get
    End Property

#End Region

#Region "Privates"

    Private Sub InternalDrawArrowGeometry(Context As StreamGeometryContext)
        Dim Theta As Double = Math.Atan2(Y1 - Y2, X1 - X2)
        Dim Sint As Double = Math.Sin(Theta)
        Dim Cost As Double = Math.Cos(Theta)

        Dim Pt1 As New Point(X1, Y1)
        Dim Pt2 As New Point(X2, Y2)

        Dim Pt3 As New Point(X2 + (HeadWidth * Cost - HeadHeight * Sint), Y2 + (HeadWidth * Sint + HeadHeight * Cost))

        Dim Pt4 As New Point(X2 + (HeadWidth * Cost + HeadHeight * Sint), Y2 - (HeadHeight * Cost - HeadWidth * Sint))

        Context.BeginFigure(Pt1, True, False)
        Context.LineTo(Pt2, True, True)
        Context.LineTo(Pt3, True, True)
        Context.LineTo(Pt2, True, True)
        Context.LineTo(Pt4, True, True)
        Context.LineTo(Pt2, True, True)
    End Sub

#End Region
End Class