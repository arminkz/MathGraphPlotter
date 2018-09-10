'------------------------------------------
' Billboard.cs (c) 2007 by Charles Petzold
'------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class Billboard
	Inherits ModelVisualBase
	Public Sub New()
		PropertyChanged(New DependencyPropertyChangedEventArgs())
	End Sub

	' Dependency properties

	Public Shared ReadOnly UpperLeftProperty As DependencyProperty = DependencyProperty.Register("UpperLeft", GetType(Point3D), GetType(Billboard), New PropertyMetadata(New Point3D(-1, 1, 0), AddressOf PropertyChanged))

	Public Property UpperLeft() As Point3D
		Get
			Return CType(GetValue(UpperLeftProperty), Point3D)
		End Get
		Set
			SetValue(UpperLeftProperty, value)
		End Set
	End Property

	Public Shared ReadOnly UpperRightProperty As DependencyProperty = DependencyProperty.Register("UpperRight", GetType(Point3D), GetType(Billboard), New PropertyMetadata(New Point3D(1, 1, 0), AddressOf PropertyChanged))

	Public Property UpperRight() As Point3D
		Get
			Return CType(GetValue(UpperRightProperty), Point3D)
		End Get
		Set
			SetValue(UpperRightProperty, value)
		End Set
	End Property

	Public Shared ReadOnly LowerLeftProperty As DependencyProperty = DependencyProperty.Register("LowerLeft", GetType(Point3D), GetType(Billboard), New PropertyMetadata(New Point3D(-1, -1, 0), AddressOf PropertyChanged))

	Public Property LowerLeft() As Point3D
		Get
			Return CType(GetValue(LowerLeftProperty), Point3D)
		End Get
		Set
			SetValue(LowerLeftProperty, value)
		End Set
	End Property

	Public Shared ReadOnly LowerRightProperty As DependencyProperty = DependencyProperty.Register("LowerRight", GetType(Point3D), GetType(Billboard), New PropertyMetadata(New Point3D(1, -1, 0), AddressOf PropertyChanged))

	Public Property LowerRight() As Point3D
		Get
			Return CType(GetValue(LowerRightProperty), Point3D)
		End Get
		Set
			SetValue(LowerRightProperty, value)
		End Set
	End Property

	Public Shared ReadOnly SlicesProperty As DependencyProperty = DependencyProperty.Register("Slices", GetType(Integer), GetType(Billboard), New PropertyMetadata(1, AddressOf PropertyChanged), AddressOf ValidateSlicesAndStacks)

	Public Property Slices() As Integer
		Get
			Return CInt(GetValue(SlicesProperty))
		End Get
		Set
			SetValue(SlicesProperty, value)
		End Set
	End Property

	Public Shared ReadOnly StacksProperty As DependencyProperty = DependencyProperty.Register("Stacks", GetType(Integer), GetType(Billboard), New PropertyMetadata(1, AddressOf PropertyChanged), AddressOf ValidateSlicesAndStacks)

	Public Property Stacks() As Integer
		Get
			Return CInt(GetValue(StacksProperty))
		End Get
		Set
			SetValue(StacksProperty, value)
		End Set
	End Property

	' Private validate method
	Private Shared Function ValidateSlicesAndStacks(value As Object) As Boolean
		Return CInt(value) > 0
	End Function

	Protected Overrides Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		' Variables for vertices collection.
		Dim UL As Vector3D = CType(UpperLeft, Vector3D)
		Dim UR As Vector3D = CType(UpperRight, Vector3D)
		Dim LL As Vector3D = CType(LowerLeft, Vector3D)
		Dim LR As Vector3D = CType(LowerRight, Vector3D)
		Dim product As Integer = Slices * Stacks

		' Variables for textures collection
		Dim ptOrigin As New Point(0, 0)
		Dim vectSlice As Vector = (New Point(1, 0) - ptOrigin) / Slices
		Dim vectStack As Vector = (New Point(0, 1) - ptOrigin) / Stacks

		For stack As Integer = 0 To Stacks
			For slice As Integer = 0 To Slices
				vertices.Add(CType(((Stacks - stack) * (Slices - slice) * UL + stack * (Slices - slice) * LL + (Stacks - stack) * slice * UR + stack * slice * LR) / product, Point3D))

				textures.Add(ptOrigin + stack * vectStack + slice * vectSlice)

				If slice < Slices AndAlso stack < Stacks Then
					indices.Add((Slices + 1) * stack + slice)
					indices.Add((Slices + 1) * (stack + 1) + slice)
					indices.Add((Slices + 1) * stack + slice + 1)

					indices.Add((Slices + 1) * stack + slice + 1)
					indices.Add((Slices + 1) * (stack + 1) + slice)
					indices.Add((Slices + 1) * (stack + 1) + slice + 1)
				End If
			Next
		Next
	End Sub
End Class
