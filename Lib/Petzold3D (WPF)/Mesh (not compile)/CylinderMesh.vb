'---------------------------------------------
' CylinderMesh.cs (c) 2007 by Charles Petzold
'---------------------------------------------
Imports System.Windows
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
'''     Generates a MeshGeometry3D object for a cylinder.
''' </summary>
''' <remarks>
'''     The MeshGeometry3D object this class creates is available as the
'''     Geometry property. You can share the same instance of a 
'''     CylinderMesh object with multiple 3D visuals. 
'''     In XAML files, the CylinderMesh
'''     tag will probably appear in a resource section.
'''     The cylinder is centered on the positive Y axis.
''' </remarks>
Public Class CylinderMesh
	Inherits CylindricalMeshBase
	''' <summary>
	'''     Initializes a new instance of the CylinderMesh class.
	''' </summary>
	Public Sub New()
		PropertyChanged(New DependencyPropertyChangedEventArgs())
	End Sub

	''' <summary>
	'''     Identifies the EndStacks dependency property.
	''' </summary>
	Public Shared ReadOnly EndStacksProperty As DependencyProperty = DependencyProperty.Register("EndStacks", GetType(Integer), GetType(CylinderMesh), New PropertyMetadata(1, AddressOf PropertyChanged), AddressOf ValidateEndStacks)

	' Validation callback for EndStacks.
	Private Shared Function ValidateEndStacks(obj As Object) As Boolean
		Return CInt(obj) > 0
	End Function

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

	''' <summary>
	'''     Identifies the Fold dependency property.
	''' </summary>
	Public Shared ReadOnly FoldProperty As DependencyProperty = DependencyProperty.Register("Fold", GetType(Double), GetType(CylinderMesh), New PropertyMetadata(1.0 / 3, AddressOf PropertyChanged), AddressOf ValidateFold)

	' Validation callback for Fold.
	Private Shared Function ValidateFold(obj As Object) As Boolean
		Return CDbl(obj) < 0.5
	End Function

	''' <summary>
	'''     Gets or sets the fraction of the brush that appears on
	'''     the top and bottom ends of the cylinder.
	''' </summary>
	''' <value>
	'''     The fraction of the brush that folds over the top and
	'''     bottom ends of the cylinder. The default is 1/3. The
	'''     property cannot be greater than 1/2.
	''' </value>
	Public Property Fold() As Double
		Get
			Return CDbl(GetValue(FoldProperty))
		End Get
		Set
			SetValue(FoldProperty, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	''' <param name="args"></param>
	''' <param name="vertices"></param>
	''' <param name="normals"></param>
	''' <param name="indices"></param>
	''' <param name="textures"></param>
	Protected Overrides Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		' Clear all four collections.
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		' Begin at the top end. Fill the collections.
		For stack As Integer = 0 To EndStacks
			Dim y As Double = Length
			Dim radius__1 As Double = stack * Radius / EndStacks
			Dim top As Integer = (stack + 0) * (Slices + 1)
			Dim bot As Integer = (stack + 1) * (Slices + 1)

			For slice As Integer = 0 To Slices
				Dim theta As Double = slice * 2 * Math.PI / Slices
				Dim x As Double = -radius__1 * Math.Sin(theta)
				Dim z As Double = -radius__1 * Math.Cos(theta)

				vertices.Add(New Point3D(x, y, z))
				normals.Add(New Vector3D(0, 1, 0))
				textures.Add(New Point(CDbl(slice) / Slices, Fold * stack / EndStacks))

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

		' Length of the cylinder: Fill in the collections.
		For stack As Integer = 0 To Stacks
			Dim y As Double = Length - stack * Length / Stacks
			Dim top As Integer = offset + (stack + 0) * (Slices + 1)
			Dim bot As Integer = offset + (stack + 1) * (Slices + 1)

			For slice As Integer = 0 To Slices
				Dim theta As Double = slice * 2 * Math.PI / Slices
				Dim x As Double = -Radius * Math.Sin(theta)
				Dim z As Double = -Radius * Math.Cos(theta)

				vertices.Add(New Point3D(x, y, z))
				normals.Add(New Vector3D(x, 0, z))
				textures.Add(New Point(CDbl(slice) / Slices, Fold + (1 - 2 * Fold) * stack / Stacks))

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

		' Finish with the bottom end. Fill the collections.
		For stack As Integer = 0 To EndStacks
			Dim y As Double = 0
			Dim radius__1 As Double = (EndStacks - stack) * Radius / EndStacks
			Dim top As Integer = offset + (stack + 0) * (Slices + 1)
			Dim bot As Integer = offset + (stack + 1) * (Slices + 1)

			For slice As Integer = 0 To Slices
				Dim theta As Double = slice * 2 * Math.PI / Slices
				Dim x As Double = -radius__1 * Math.Sin(theta)
				Dim z As Double = -radius__1 * Math.Cos(theta)

				vertices.Add(New Point3D(x, y, z))
				normals.Add(New Vector3D(0, -1, 0))
				textures.Add(New Point(CDbl(slice) / Slices, (1 - Fold) + Fold * stack / EndStacks))

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

	''' <summary>
	'''     Creates a new instance of the CylinderMesh class.
	''' </summary>
	''' <returns>
	'''     A new instance of CylinderMesh.
	''' </returns>
	''' <remarks>
	'''     Overriding this method is required when deriving 
	'''     from the Freezable class.
	''' </remarks>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New CylinderMesh()
	End Function
End Class

