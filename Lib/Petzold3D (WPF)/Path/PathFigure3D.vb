Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D


<System.Windows.Markup.ContentProperty("Segments")> _
Public Class PathFigure3D
	Inherits Animatable
	' ----------------------------------------------------
	' TODO: IsClosed, IsFilled, May have curves, etc, etc
	' ----------------------------------------------------

	Public Shared Approximation As Integer = 100

	' Filled when dependency properties change.
	Private figFlattened As PathFigure3D

	''' <summary>
	''' 
	''' </summary>
	Public Sub New()
		figFlattened = New PathFigure3D(0)
		Segments = New PathSegment3DCollection()
	End Sub

	Private Sub New(unused As Integer)
		Segments = New PathSegment3DCollection()
		Segments.Add(New PolyLineSegment3D())
	End Sub

	''' <summary>
	'''     Identifies the StartPoint dependency property.
	''' </summary>
	Public Shared ReadOnly StartPointProperty As DependencyProperty = DependencyProperty.Register("StartPoint", GetType(Point3D), GetType(PathFigure3D), New PropertyMetadata(New Point3D(), AddressOf StartPointPropertyChanged))

	''' <summary>
	''' 
	''' </summary>
	Public Property StartPoint() As Point3D
		Get
			Return CType(GetValue(StartPointProperty), Point3D)
		End Get
		Set
			SetValue(StartPointProperty, value)
		End Set
	End Property

	Private Shared Sub StartPointPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim fig As PathFigure3D = TryCast(obj, PathFigure3D)

		If fig.figFlattened IsNot Nothing Then
			fig.figFlattened.StartPoint = CType(args.NewValue, Point3D)
			SegmentsPropertyChanged(obj, args)
		End If
	End Sub

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly SegmentsProperty As DependencyProperty = DependencyProperty.Register("Segments", GetType(PathSegment3DCollection), GetType(PathFigure3D), New PropertyMetadata(Nothing, AddressOf SegmentsPropertyChanged))

	''' <summary>
	''' 
	''' </summary>
	Public Property Segments() As PathSegment3DCollection
		Get
			Return DirectCast(GetValue(SegmentsProperty), PathSegment3DCollection)
		End Get
		Set
			SetValue(SegmentsProperty, value)
		End Set
	End Property

	Private Shared Sub SegmentsPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim fig As PathFigure3D = TryCast(obj, PathFigure3D)

		If fig.figFlattened IsNot Nothing Then
			fig.SegmentsPropertyChanged()
		End If
	End Sub

	Private Sub SegmentsPropertyChanged()
		Dim polyseg As PolyLineSegment3D = TryCast(figFlattened.Segments(0), PolyLineSegment3D)
		Dim points As Point3DCollection = polyseg.Points
		polyseg.Points = Nothing

		points.Clear()
		Dim ptStart As Point3D = StartPoint

		For Each seg As PathSegment3D In Segments
			If TypeOf seg Is LineSegment3D Then
				Dim segLine As LineSegment3D = TryCast(seg, LineSegment3D)
				points.Add(segLine.Point)
				ptStart = segLine.Point
			ElseIf TypeOf seg Is PolyLineSegment3D Then
				Dim segPoly As PolyLineSegment3D = TryCast(seg, PolyLineSegment3D)

				For Each pt As Point3D In segPoly.Points
					points.Add(pt)
					ptStart = pt
				Next

			ElseIf TypeOf seg Is BezierSegment3D Then
				Dim segBez As BezierSegment3D = TryCast(seg, BezierSegment3D)
				ConvertBezier(points, ptStart, segBez.Point1, segBez.Point2, segBez.Point3)
				ptStart = segBez.Point3

			ElseIf TypeOf seg Is PolyBezierSegment3D Then
				Dim segPoly As PolyBezierSegment3D = TryCast(seg, PolyBezierSegment3D)

				For i As Integer = 0 To segPoly.Points.Count - 1 Step 3
					ConvertBezier(points, ptStart, segPoly.Points(i), segPoly.Points(i + 1), segPoly.Points(i + 2))
					ptStart = segPoly.Points(i + 2)
				Next
			End If
		Next
		polyseg.Points = points
	End Sub

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Public Function GetFlattenedPathFigure() As PathFigure3D
		Return figFlattened
	End Function

	Private Sub ConvertBezier(points As Point3DCollection, point0 As Point3D, point1 As Point3D, point2 As Point3D, point3 As Point3D)

		For i As Integer = 0 To Approximation - 1
			Dim t As Double = CDbl(i + 1) / Approximation

			Dim x As Double = (1 - t) * (1 - t) * (1 - t) * point0.X + 3 * t * (1 - t) * (1 - t) * point1.X + 3 * t * t * (1 - t) * point2.X + t * t * t * point3.X

			Dim y As Double = (1 - t) * (1 - t) * (1 - t) * point0.Y + 3 * t * (1 - t) * (1 - t) * point1.Y + 3 * t * t * (1 - t) * point2.Y + t * t * t * point3.Y

			Dim z As Double = (1 - t) * (1 - t) * (1 - t) * point0.Z + 3 * t * (1 - t) * (1 - t) * point1.Z + 3 * t * t * (1 - t) * point2.Z + t * t * t * point3.Z

			points.Add(New Point3D(x, y, z))
		Next
	End Sub

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Public Overrides Function ToString() As String
		Return "M" & Convert.ToString(StartPoint) & Convert.ToString(Segments)
	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New PathFigure3D()
	End Function
End Class
