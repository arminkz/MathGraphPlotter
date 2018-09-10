'------------------------------------------
' LineRange.cs (c) 2007 by Charles Petzold
'------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Structure LineRange
	Private m_point1 As Point3D
	Private m_point2 As Point3D

	Public Sub New(point1 As Point3D, point2 As Point3D)
		Me.m_point1 = point1
		Me.m_point2 = point2
	End Sub

	Public Property Point1() As Point3D
		Get
			Return m_point1
		End Get
		Set
			m_point1 = value
		End Set
	End Property

	Public Property Point2() As Point3D
		Get
			Return m_point2
		End Get
		Set
			m_point2 = value
		End Set
	End Property

	Public Function PointFromX(x As Double) As Point3D
		Dim factor As Double = (x - Point1.X) / (Point2.X - Point1.X)
		Dim y As Double = Point1.Y + factor * (Point2.Y - Point1.Y)
		Dim z As Double = Point1.Z + factor * (Point2.Z - Point1.Z)
		Return New Point3D(x, y, z)
	End Function

	Public Function PointFromY(y As Double) As Point3D
		Dim factor As Double = (y - Point1.Y) / (Point2.Y - Point1.Y)
		Dim x As Double = Point1.X + factor * (Point2.X - Point1.X)
		Dim z As Double = Point1.Z + factor * (Point2.Z - Point1.Z)
		Return New Point3D(x, y, z)
	End Function

	Public Function PointFromZ(z As Double) As Point3D
		Dim factor As Double = (z - Point1.Z) / (Point2.Z - Point1.Z)
		Dim x As Double = Point1.X + factor * (Point2.X - Point1.X)
		Dim y As Double = Point1.Y + factor * (Point2.Y - Point1.Y)
		Return New Point3D(x, y, z)
	End Function
End Structure
