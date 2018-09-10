'------------------------------------------------
' BezierSegment3D.cs (c) 2007 by Charles Petzold
'------------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class BezierSegment3D
	Inherits PathSegment3D
	''' <summary>
	'''     Identifies the Point1 dependency property.
	''' </summary>
	Public Shared ReadOnly Point1Property As DependencyProperty = DependencyProperty.Register("Point1", GetType(Point3D), GetType(BezierSegment3D), New PropertyMetadata(New Point3D()))

	''' <summary>
	'''     Gets or sets the first control point of the curve. 
	''' </summary>
	Public Property Point1() As Point3D
		Get
			Return CType(GetValue(Point1Property), Point3D)
		End Get
		Set
			SetValue(Point1Property, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Point2 dependency property. 
	''' </summary>
	Public Shared ReadOnly Point2Property As DependencyProperty = DependencyProperty.Register("Point2", GetType(Point3D), GetType(BezierSegment3D), New PropertyMetadata(New Point3D()))

	''' <summary>
	'''     Gets or sets the second control point of the curve. 
	''' </summary>
	Public Property Point2() As Point3D
		Get
			Return CType(GetValue(Point2Property), Point3D)
		End Get
		Set
			SetValue(Point2Property, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Point3 dependency property. 
	''' </summary>
	Public Shared ReadOnly Point3Property As DependencyProperty = DependencyProperty.Register("Point3", GetType(Point3D), GetType(BezierSegment3D), New PropertyMetadata(New Point3D()))

	''' <summary>
	'''     Gets or sets the final point of the curve. 
	''' </summary>
	Public Property Point3() As Point3D
		Get
			Return CType(GetValue(Point3Property), Point3D)
		End Get
		Set
			SetValue(Point3Property, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Public Overrides Function ToString() As String
		Return [String].Format("C{0} {1} {2}", Point1, Point2, Point3)
	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New BezierSegment3D()
	End Function
End Class
