'
'
'
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class Cuboid
	Inherits ModelVisualBase
	Public Sub New()
		PropertyChanged(Me, New DependencyPropertyChangedEventArgs())
	End Sub
	' Width property.
	' ---------------
	Public Shared ReadOnly WidthProperty As DependencyProperty = DependencyProperty.Register("Width", GetType(Double), GetType(Cuboid), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	Public Property Width() As Double
		Get
			Return CDbl(GetValue(WidthProperty))
		End Get
		Set
			SetValue(WidthProperty, value)
		End Set
	End Property

	' Height property.
	' ----------------
	Public Shared ReadOnly HeightProperty As DependencyProperty = DependencyProperty.Register("Height", GetType(Double), GetType(Cuboid), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	Public Property Height() As Double
		Get
			Return CDbl(GetValue(HeightProperty))
		End Get
		Set
			SetValue(HeightProperty, value)
		End Set
	End Property

	' Depth property.
	' ---------------
	Public Shared ReadOnly DepthProperty As DependencyProperty = DependencyProperty.Register("Depth", GetType(Double), GetType(Cuboid), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	Public Property Depth() As Double
		Get
			Return CDbl(GetValue(DepthProperty))
		End Get
		Set
			SetValue(DepthProperty, value)
		End Set
	End Property

	' Origin property.
	' ----------------

	Public Shared ReadOnly OriginProperty As DependencyProperty = DependencyProperty.Register("Origin", GetType(Point3D), GetType(Cuboid), New PropertyMetadata(New Point3D(-0.5, -0.5, -0.5), AddressOf PropertyChanged))

	Public Property Origin() As Point3D
		Get
			Return CType(GetValue(OriginProperty), Point3D)
		End Get
		Set
			SetValue(OriginProperty, value)
		End Set
	End Property


	' Slices property.
	' ----------------
	Public Shared ReadOnly SlicesProperty As DependencyProperty = DependencyProperty.Register("Slices", GetType(Integer), GetType(Cuboid), New PropertyMetadata(10, AddressOf PropertyChanged), AddressOf ValidateSlices)

	Public Property Slices() As Integer
		Get
			Return CInt(GetValue(SlicesProperty))
		End Get
		Set
			SetValue(SlicesProperty, value)
		End Set
	End Property

	' Stacks property.
	' ----------------
	Public Shared ReadOnly StacksProperty As DependencyProperty = DependencyProperty.Register("Stacks", GetType(Integer), GetType(Cuboid), New PropertyMetadata(10, AddressOf PropertyChanged), AddressOf ValidateSlices)

	Public Property Stacks() As Integer
		Get
			Return CInt(GetValue(StacksProperty))
		End Get
		Set
			SetValue(StacksProperty, value)
		End Set
	End Property

	' Slivers property.
	' ----------------
	Public Shared ReadOnly SliversProperty As DependencyProperty = DependencyProperty.Register("Slivers", GetType(Integer), GetType(Cuboid), New PropertyMetadata(10, AddressOf PropertyChanged), AddressOf ValidateSlices)

	Public Property Slivers() As Integer
		Get
			Return CInt(GetValue(SliversProperty))
		End Get
		Set
			SetValue(SliversProperty, value)
		End Set
	End Property

	Private Shared Function ValidateSlices(obj As Object) As Boolean
		Return CInt(obj) > 0
	End Function


	Protected Overrides Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		' Front.
		For iy As Integer = 0 To Stacks
			Dim y As Double = Origin.Y + Height - iy * Height / Stacks

			For ix As Integer = 0 To Slices
				Dim x As Double = Origin.X + ix * Width / Slices
				vertices.Add(New Point3D(x, y, Origin.Z + Depth))
			Next
		Next

		' Back
		For iy As Integer = 0 To Stacks
			Dim y As Double = Origin.Y + Height - iy * Height / Stacks

			For ix As Integer = 0 To Slices
				Dim x As Double = Origin.X + Width - ix * Width / Slices
				vertices.Add(New Point3D(x, y, Origin.Z))
			Next
		Next

		' Left
		For iy As Integer = 0 To Stacks
			Dim y As Double = Origin.Y + Height - iy * Height / Stacks

			For iz As Integer = 0 To Slivers
				Dim z As Double = Origin.Z + iz * Depth / Slivers
				vertices.Add(New Point3D(Origin.X, y, z))
			Next
		Next

		' Right
		For iy As Integer = 0 To Stacks
			Dim y As Double = Origin.Y + Height - iy * Height / Stacks

			For iz As Integer = 0 To Slivers
				Dim z As Double = Origin.Z + Depth - iz * Depth / Slivers
				vertices.Add(New Point3D(Origin.X + Width, y, z))
			Next
		Next

		' Top
		For iz As Integer = 0 To Slivers
			Dim z As Double = Origin.Z + iz * Depth / Slivers

			For ix As Integer = 0 To Slices
				Dim x As Double = Origin.X + ix * Width / Slices
				vertices.Add(New Point3D(x, Origin.Y + Height, z))
			Next
		Next

		' Top
		For iz As Integer = 0 To Slivers
			Dim z As Double = Origin.Z + Depth - iz * Depth / Slivers

			For ix As Integer = 0 To Slices
				Dim x As Double = Origin.X + ix * Width / Slices
				vertices.Add(New Point3D(x, Origin.Y, z))
			Next
		Next

		For side As Integer = 0 To 5
			For iy As Integer = 0 To Stacks
				Dim y As Double = CDbl(iy) / Stacks

				For ix As Integer = 0 To Slices
					Dim x As Double = CDbl(ix) / Slices
					textures.Add(New Point(x, y))
				Next
			Next
		Next

		' Front, back, left, right
		For side As Integer = 0 To 5
			For iy As Integer = 0 To Stacks - 1
				For ix As Integer = 0 To Slices - 1
					indices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix)
					indices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix)
					indices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix + 1)

					indices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix + 1)
					indices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix)
					indices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix + 1)
				Next
			Next
		Next
	End Sub
End Class
