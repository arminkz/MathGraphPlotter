'---------------------------------------
' Sphere.cs (c) 2007 by Charles Petzold
'---------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class Sphere
	Inherits ModelVisualBase
	' Objects reused by calls to Triangulate.
	Private rotate As AxisAngleRotation3D
	Private xform As RotateTransform3D

	' Public constructor to initialize those fields, etc
	Public Sub New()
		rotate = New AxisAngleRotation3D()
		xform = New RotateTransform3D(rotate)

		PropertyChanged(Me, New DependencyPropertyChangedEventArgs())
	End Sub

	' Dependency properties
	Public Shared ReadOnly SlicesProperty As DependencyProperty = DependencyProperty.Register("Slices", GetType(Integer), GetType(Sphere), New PropertyMetadata(36, AddressOf PropertyChanged), AddressOf ValidatePositive)

	Public Shared ReadOnly StacksProperty As DependencyProperty = DependencyProperty.Register("Stacks", GetType(Integer), GetType(Sphere), New PropertyMetadata(18, AddressOf PropertyChanged), AddressOf ValidatePositive)

	Public Shared ReadOnly CenterProperty As DependencyProperty = DependencyProperty.Register("Center", GetType(Point3D), GetType(Sphere), New PropertyMetadata(New Point3D(0, 0, 0), AddressOf PropertyChanged))

	Public Shared ReadOnly LongitudeFromProperty As DependencyProperty = DependencyProperty.Register("LongitudeFrom", GetType([Double]), GetType(Sphere), New PropertyMetadata(-180.0, AddressOf PropertyChanged), AddressOf ValidateLongitude)

	Public Shared ReadOnly LongitudeToProperty As DependencyProperty = DependencyProperty.Register("LongitudeTo", GetType([Double]), GetType(Sphere), New PropertyMetadata(180.0, AddressOf PropertyChanged), AddressOf ValidateLongitude)

	Public Shared ReadOnly LatitudeFromProperty As DependencyProperty = DependencyProperty.Register("LatitudeFrom", GetType([Double]), GetType(Sphere), New PropertyMetadata(90.0, AddressOf PropertyChanged), AddressOf ValidateLatitude)

	Public Shared ReadOnly LatitudeToProperty As DependencyProperty = DependencyProperty.Register("LatitudeTo", GetType([Double]), GetType(Sphere), New PropertyMetadata(-90.0, AddressOf PropertyChanged), AddressOf ValidateLatitude)

	Public Shared ReadOnly RadiusProperty As DependencyProperty = DependencyProperty.Register("Radius", GetType(Double), GetType(Sphere), New PropertyMetadata(1.0, AddressOf PropertyChanged), AddressOf ValidateNonNegative)

	' CLR properties
	Public Property Slices() As Integer
		Get
			Return CInt(GetValue(SlicesProperty))
		End Get
		Set
			SetValue(SlicesProperty, value)
		End Set
	End Property

	Public Property Stacks() As Integer
		Get
			Return CInt(GetValue(StacksProperty))
		End Get
		Set
			SetValue(StacksProperty, value)
		End Set
	End Property

	Public Property Center() As Point3D
		Get
			Return CType(GetValue(CenterProperty), Point3D)
		End Get
		Set
			SetValue(CenterProperty, value)
		End Set
	End Property

	Public Property LongitudeFrom() As [Double]
		Get
			Return CType(GetValue(LongitudeFromProperty), [Double])
		End Get
		Set
			SetValue(LongitudeFromProperty, value)
		End Set
	End Property

	Public Property LongitudeTo() As [Double]
		Get
			Return CType(GetValue(LongitudeToProperty), [Double])
		End Get
		Set
			SetValue(LongitudeFromProperty, value)
		End Set
	End Property

	Public Property LatitudeFrom() As [Double]
		Get
			Return CType(GetValue(LatitudeFromProperty), [Double])
		End Get
		Set
			SetValue(LatitudeFromProperty, value)
		End Set
	End Property

	Public Property LatitudeTo() As [Double]
		Get
			Return CType(GetValue(LatitudeToProperty), [Double])
		End Get
		Set
			SetValue(LatitudeToProperty, value)
		End Set
	End Property

	Public Property Radius() As Double
		Get
			Return CDbl(GetValue(RadiusProperty))
		End Get
		Set
			SetValue(RadiusProperty, value)
		End Set
	End Property

	' Private validate methods
	Private Shared Function ValidatePositive(value As Object) As Boolean
		Return CInt(value) > 0
	End Function

	Private Shared Function ValidateNonNegative(value As Object) As Boolean
		Return CDbl(value) >= 0
	End Function

	Private Shared Function ValidateLongitude(value As Object) As Boolean
		Dim d As [Double] = CDbl(value)
		Return d >= -180 AndAlso d <= 180
	End Function

	Private Shared Function ValidateLatitude(value As Object) As Boolean
		Dim d As [Double] = CDbl(value)
		Return d >= -90 AndAlso d <= 90
	End Function

	Protected Overrides Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		' Copy properties to local variables to improve speed
		Dim slices__1 As Integer = Slices
		Dim stacks__2 As Integer = Stacks
		Dim radius__3 As Double = Radius
		Dim ctr As Point3D = Center

		Dim lat1 As Double = Math.Max(LatitudeFrom, LatitudeTo)
		' default is 90
		Dim lat2 As Double = Math.Min(LatitudeFrom, LatitudeTo)
		' default is -90
		Dim lng1 As Double = LongitudeFrom
		' default is -180
		Dim lng2 As Double = LongitudeTo
		' default is 180
		For lat As Integer = 0 To stacks__2
			Dim degrees As Double = lat1 - lat * (lat1 - lat2) / stacks__2

			Dim angle As Double = Math.PI * degrees / 180
			Dim y As Double = radius__3 * Math.Sin(angle)
			Dim scale As Double = Math.Cos(angle)

			For lng As Integer = 0 To slices__1
				Dim diff As Double = lng2 - lng1

				If diff < 0 Then
					diff += 360
				End If

				degrees = lng1 + lng * diff / slices__1
				angle = Math.PI * degrees / 180
				Dim x As Double = radius__3 * scale * Math.Sin(angle)
				Dim z As Double = radius__3 * scale * Math.Cos(angle)

				Dim vect As New Vector3D(x, y, z)
				vertices.Add(ctr + vect)
				normals.Add(vect)
				textures.Add(New Point(CDbl(lng) / slices__1, CDbl(lat) / stacks__2))
			Next
		Next

		For lat As Integer = 0 To stacks__2 - 1
			Dim start As Integer = lat * (slices__1 + 1)
			Dim [next] As Integer = start + slices__1 + 1

			For lng As Integer = 0 To slices__1 - 1
				indices.Add(start + lng)
				indices.Add([next] + lng)
				indices.Add([next] + lng + 1)

				indices.Add(start + lng)
				indices.Add([next] + lng + 1)
				indices.Add(start + lng + 1)
			Next
		Next
	End Sub
End Class
