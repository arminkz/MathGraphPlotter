'--------------------------------------------------
' PolyLineSegment3D.cs (c) 2007 by Charles Petzold
'--------------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class PolyLineSegment3D
	Inherits PathSegment3D
	' Points Property.
	' ---------------
	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly PointsProperty As DependencyProperty = DependencyProperty.Register("Points", GetType(Point3DCollection), GetType(PolyLineSegment3D))
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
	'''     Initializes a new instance of PolyLineSegment3D.
	''' </summary>
	Public Sub New()
		Points = New Point3DCollection()
	End Sub

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Public Overrides Function ToString() As String
		Return "L" & Convert.ToString(Points)
	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New PolyLineSegment3D()
	End Function
End Class
