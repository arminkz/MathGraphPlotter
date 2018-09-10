

Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
'''     Draws a line between two points in a <c>PathFigure3D</c>.
''' </summary>
''' <remarks>
'''     A <c>LineSegment3D</c> is always part of a <c>PathFigure3D</c>. 
'''     The starting point of the line is given by the <c>StartPoint</c> property of
'''     <c>PathFigure3D</c> or the end point of the previous <c>PathSegment3D</c> object in 
'''     the <c>PathFigure3D</c>.
''' </remarks>
Public Class LineSegment3D
	Inherits PathSegment3D
	' ---------------------------------------
	' Point dependency property and property.
	' ---------------------------------------

	''' <summary>
	'''     Identifies the <c>Point</c> dependency property.
	''' </summary>
	Public Shared ReadOnly PointProperty As DependencyProperty = DependencyProperty.Register("Point", GetType(Point3D), GetType(LineSegment3D), New PropertyMetadata(New Point3D(0, 0, 0)))

	''' <summary>
	'''     Gets or sets the end point of the line segment. This is a dependency property.
	''' </summary>
	Public Property Point() As Point3D
		Get
			Return CType(GetValue(PointProperty), Point3D)
		End Get
		Set
			SetValue(PointProperty, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Public Overrides Function ToString() As String
		Return "L" & Convert.ToString(Point)
	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New LineSegment3D()
	End Function
End Class
