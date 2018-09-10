'----------------------------------------------------
' PolyBezierSegment3D.cs (c) 2007 by Charles Petzold
'----------------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class PolyBezierSegment3D
	Inherits PathSegment3D
	''' <summary>
	'''     Initializes a new instance of the <c>PolyBezierSegment3D</c> 
	'''     class.
	''' </summary>
	Public Sub New()
		Points = New Point3DCollection()
	End Sub

	' Points Property.
	' ---------------
	''' <summary>
	'''     Identifies the <c>Points</c> dependency property.
	''' </summary>
	Public Shared ReadOnly PointsProperty As DependencyProperty = DependencyProperty.Register("Points", GetType(Point3DCollection), GetType(PolyBezierSegment3D), New PropertyMetadata(Nothing))

	''' <summary>
	''' 
	''' </summary>
	Public Property Points() As Point3DCollection
		Get
			Return DirectCast(GetValue(PointsProperty), Point3DCollection)
		End Get
		Set
			SetValue(PointsProperty, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Public Overrides Function ToString() As String
		Return "C" & Points.ToString()
	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New PolyBezierSegment3D()
	End Function
End Class
