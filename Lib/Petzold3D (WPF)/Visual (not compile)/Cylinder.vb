'-----------------------------------------
' Cylinder.cs (c) 2007 by Charles Petzold
'-----------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class Cylinder
	Inherits ModelVisualBase
	' Objects reused by calls to Triangulate.
	Private rotate As AxisAngleRotation3D
	Private xform As RotateTransform3D

	' Public constructor to initialize those fields, etc
	Public Sub New()
		rotate = New AxisAngleRotation3D()
		xform = New RotateTransform3D(rotate)

		PropertyChanged(New DependencyPropertyChangedEventArgs())
	End Sub

	''' <summary>
	'''     Identifies the <c>Point1</c> dependency property.
	''' </summary>
	Public Shared ReadOnly Point1Property As DependencyProperty = DependencyProperty.Register("Point1", GetType(Point3D), GetType(Cylinder), New PropertyMetadata(New Point3D(0, 1, 0), AddressOf PropertyChanged))

	''' <summary>
	''' 
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
	''' 
	''' </summary>
	Public Shared ReadOnly Point2Property As DependencyProperty = DependencyProperty.Register("Point2", GetType(Point3D), GetType(Cylinder), New PropertyMetadata(New Point3D(0, 0, 0), AddressOf PropertyChanged))

	''' <summary>
	''' 
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
	''' 
	''' </summary>
	Public Shared ReadOnly Radius1Property As DependencyProperty = DependencyProperty.Register("Radius1", GetType(Double), GetType(Cylinder), New PropertyMetadata(1.0, AddressOf PropertyChanged), Function(value As Object) CDbl(value) >= 0)

	''' <summary>
	''' 
	''' </summary>
	Public Property Radius1() As Double
		Get
			Return CDbl(GetValue(Radius1Property))
		End Get
		Set
			SetValue(Radius1Property, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly Radius2Property As DependencyProperty = DependencyProperty.Register("Radius2", GetType(Double), GetType(Cylinder), New PropertyMetadata(1.0, AddressOf PropertyChanged), Function(value As Object) CDbl(value) >= 0)

	''' <summary>
	''' 
	''' </summary>
	Public Property Radius2() As Double
		Get
			Return CDbl(GetValue(Radius2Property))
		End Get
		Set
			SetValue(Radius2Property, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly Fold1Property As DependencyProperty = DependencyProperty.Register("Fold1", GetType(Double), GetType(Cylinder), New PropertyMetadata(0.1, AddressOf PropertyChanged), Function(value As Object) CDbl(value) >= 0 AndAlso CDbl(value) <= 1)

	''' <summary>
	''' 
	''' </summary>
	Public Property Fold1() As Double
		Get
			Return CDbl(GetValue(Fold1Property))
		End Get
		Set
			SetValue(Fold1Property, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly Fold2Property As DependencyProperty = DependencyProperty.Register("Fold2", GetType(Double), GetType(Cylinder), New PropertyMetadata(0.9, AddressOf PropertyChanged), Function(value As Object) CDbl(value) >= 0 AndAlso CDbl(value) <= 1)

	''' <summary>
	''' 
	''' </summary>
	Public Property Fold2() As Double
		Get
			Return CDbl(GetValue(Fold2Property))
		End Get
		Set
			SetValue(Fold2Property, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly SlicesProperty As DependencyProperty = DependencyProperty.Register("Slices", GetType(Integer), GetType(Cylinder), New PropertyMetadata(36, AddressOf PropertyChanged), Function(value As Object) CInt(value) > 2)

	''' <summary>
	''' 
	''' </summary>
	Public Property Slices() As Integer
		Get
			Return CInt(GetValue(SlicesProperty))
		End Get
		Set
			SetValue(SlicesProperty, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly StacksProperty As DependencyProperty = DependencyProperty.Register("Stacks", GetType(Integer), GetType(Cylinder), New PropertyMetadata(1, AddressOf PropertyChanged), Function(value As Object) CInt(value) > 0)

	''' <summary>
	''' 
	''' </summary>
	Public Property Stacks() As Integer
		Get
			Return CInt(GetValue(StacksProperty))
		End Get
		Set
			SetValue(StacksProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the EndStacks dependency property.
	''' </summary>
	Public Shared ReadOnly EndStacksProperty As DependencyProperty = DependencyProperty.Register("EndStacks", GetType(Integer), GetType(Cylinder), New PropertyMetadata(1, AddressOf PropertyChanged), Function(value As Object) CInt(value) > 0)

	''' <summary>
	'''     Gets or sets the number of radial divisions on each end of 
	'''     the cylinder.
	''' </summary>
	''' <value>
	'''     The number of radial divisions on the end of the cylinder. 
	'''     This property must be at least 1, which is also the default value. 
	''' </value>
	''' <remarks>
	'''     The default value of 1 is appropriate in many cases. 
	'''     However, if PointLight or SpotLight objects are applied to the
	'''     cylinder, or if non-linear transforms are used to deform
	'''     the figure, you should set EndStacks to a higher value.
	''' </remarks>
	Public Property EndStacks() As Integer
		Get
			Return CInt(GetValue(EndStacksProperty))
		End Get
		Set
			SetValue(EndStacksProperty, value)
		End Set
	End Property

	Protected Overrides Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		' vectRearRadius points towards -Z (when possible).
		Dim vectCylinder As Vector3D = Point2 - Point1
		Dim vectRearRadius As Vector3D

		If vectCylinder.X = 0 AndAlso vectCylinder.Y = 0 Then
			' Special case: set rear-radius vector
			vectRearRadius = New Vector3D(0, -1, 0)
		Else
			' Find vector axis 90 degrees from cylinder where Z == 0
			rotate.Axis = Vector3D.CrossProduct(vectCylinder, New Vector3D(0, 0, 1))
			rotate.Angle = -90

			' Rotate cylinder 90 degrees to find radius vector
			vectRearRadius = vectCylinder * xform.Value
			vectRearRadius.Normalize()
		End If

		' Will rotate radius around cylinder axis
		rotate.Axis = -vectCylinder

		' Begin at the top end. Fill the collections.
		For stack As Integer = 0 To EndStacks
			Dim radius As Double = stack * Radius1 / EndStacks
			Dim vectRadius As Vector3D = radius * vectRearRadius
			Dim top As Integer = (stack + 0) * (Slices + 1)
			Dim bot As Integer = (stack + 1) * (Slices + 1)

			For slice As Integer = 0 To Slices
				rotate.Angle = slice * 360.0 / Slices
				vertices.Add(Point1 + vectRadius * xform.Value)
				normals.Add(-vectCylinder)
				textures.Add(New Point(CDbl(slice) / Slices, Fold1 * stack / EndStacks))

				If stack < EndStacks AndAlso slice < Slices Then
					If stack <> 0 Then
						indices.Add(top + slice)
						indices.Add(bot + slice)
						indices.Add(top + slice + 1)
					End If
					indices.Add(top + slice + 1)
					indices.Add(bot + slice)
					indices.Add(bot + slice + 1)
				End If
			Next
		Next

		Dim offset As Integer = vertices.Count

		' Go down length of cylinder and fill in the collections.
		For stack As Integer = 0 To Stacks
			Dim radius As Double = ((Stacks - stack) * Radius1 + stack * Radius2) / Stacks
			Dim vectRadius As Vector3D = radius * vectRearRadius
			Dim center As Point3D = CType(Point1 + stack * vectCylinder / Stacks, Point3D)
			Dim top As Integer = offset + (stack + 0) * (Slices + 1)
			Dim bot As Integer = offset + (stack + 1) * (Slices + 1)

			For slice As Integer = 0 To Slices
				rotate.Angle = slice * 360.0 / Slices
				Dim normal As Vector3D = vectRadius * xform.Value
				normals.Add(normal)
				vertices.Add(center + normal)
				textures.Add(New Point(CDbl(slice) / Slices, Fold1 + (Fold2 - Fold1) * stack / Stacks))

				If stack < Stacks AndAlso slice < Slices Then
					indices.Add(top + slice)
					indices.Add(bot + slice)
					indices.Add(top + slice + 1)

					indices.Add(top + slice + 1)
					indices.Add(bot + slice)
					indices.Add(bot + slice + 1)
				End If
			Next
		Next

		offset = vertices.Count

		' Finish with bottom.
		For stack As Integer = 0 To EndStacks
			Dim radius As Double = Radius2 * (1 - CDbl(stack) / EndStacks)
			Dim vectRadius As Vector3D = radius * vectRearRadius
			Dim top As Integer = offset + (stack + 0) * (Slices + 1)
			Dim bot As Integer = offset + (stack + 1) * (Slices + 1)

			For slice As Integer = 0 To Slices
				rotate.Angle = slice * 360.0 / Slices
				vertices.Add(Point2 + vectRadius * xform.Value)
				normals.Add(vectCylinder)
				textures.Add(New Point(CDbl(slice) / Slices, Fold2 + (1 - Fold2) * stack / EndStacks))

				If stack < EndStacks AndAlso slice < Slices Then
					indices.Add(top + slice)
					indices.Add(bot + slice)
					indices.Add(top + slice + 1)

					If stack <> EndStacks - 1 Then
						indices.Add(top + slice + 1)
						indices.Add(bot + slice)
						indices.Add(bot + slice + 1)
					End If
				End If
			Next
		Next
	End Sub
End Class
