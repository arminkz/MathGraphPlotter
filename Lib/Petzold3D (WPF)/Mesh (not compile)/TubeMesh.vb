'-----------------------------------------
' TubeMesh.cs (c) 2007 by Charles Petzold
'-----------------------------------------
Imports System.Windows
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
'''     Generates a MeshGeometry3D object for a tube.
''' </summary>
''' <remarks>
'''     The MeshGeometry3D object this class creates is available as the
'''     Geometry property. You can share the same instance of a 
'''     CylinderMesh object with multiple 3D visuals. 
'''     In XAML files, the TubeMesh
'''     tag will probably appear in a resource section.
'''     The cylinder is centered on the positive Y axis.
''' </remarks>
Public Class TubeMesh
	Inherits CylindricalMeshBase
	''' <summary>
	'''     Initializes a new instance of the TubeMesh class.
	''' </summary>
	Public Sub New()
		' Initialize Geometry property.
		PropertyChanged(New DependencyPropertyChangedEventArgs())
	End Sub

	''' <summary>
	'''     Identifies the Thickness dependency property.
	''' </summary>
	Public Shared ReadOnly ThicknessProperty As DependencyProperty = DependencyProperty.Register("Thickness", GetType(Double), GetType(TubeMesh), New PropertyMetadata(0.25, AddressOf PropertyChanged), AddressOf ValidateThickness)

	' Validation callback for Thickness.
	Private Shared Function ValidateThickness(obj As Object) As Boolean
		Return CDbl(obj) >= 0
	End Function

	''' <summary>
	'''     Gets or sets the thickness of the wall of the tube.
	''' </summary>
	Public Property Thickness() As Double
		Get
			Return CDbl(GetValue(ThicknessProperty))
		End Get
		Set
			SetValue(ThicknessProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the EndStacks dependency property.
	''' </summary>
	Public Shared ReadOnly EndStacksProperty As DependencyProperty = DependencyProperty.Register("EndStacks", GetType(Integer), GetType(TubeMesh), New PropertyMetadata(1, AddressOf PropertyChanged), AddressOf ValidateEndStacks)

	' Validation callback for EndStacks.
	Private Shared Function ValidateEndStacks(obj As Object) As Boolean
		Return CInt(obj) > 0
	End Function

	''' <summary>
	'''     Gets or sets the number of radial divisions of the wall of 
	'''     the tube.
	''' </summary>
	''' <value>
	'''     The number of radial divisions on the wall of the tube. 
	'''     This property must be at least 1, which is also the default value. 
	''' </value>
	''' <remarks>
	'''     The default value of 1 is appropriate in many cases. 
	'''     However, if PointLight or SpotLight objects are applied to the
	'''     tube, or if non-linear transforms are used to deform
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
	Public Shared ReadOnly FoldProperty As DependencyProperty = DependencyProperty.Register("Fold", GetType(Double), GetType(TubeMesh), New PropertyMetadata(0.125, AddressOf PropertyChanged), AddressOf ValidateFold)

	' Validation callback for Fold.
	Private Shared Function ValidateFold(obj As Object) As Boolean
		Return CDbl(obj) < 0.5
	End Function

	''' <summary>
	'''     Gets or sets the fraction of the brush that appears on
	'''     the top and bottom ends of the tube.
	''' </summary>
	''' <value>
	'''     The fraction of the brush that folds over the top and
	'''     bottom ends of the tube. The default is 0.1. The
	'''     property cannot be greater than 0.5.
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
	''' <param name="args">
	'''     The DependencyPropertyChangedEventArgs object originally 
	'''     passed to the PropertyChanged handler that initiated this
	'''     recalculation.
	''' </param>
	''' <param name="vertices">
	'''     The Point3DCollection corresponding to the Positions property
	'''     of the MeshGeometry3D.
	''' </param>
	''' <param name="normals">
	'''     The Vector3DCollection corresponding to the Normals property
	'''     of the MeshGeometry3D.
	''' </param>
	''' <param name="indices">
	'''     The Int32Collection corresponding to the TriangleIndices
	'''     property of the MeshGeometry3D.
	''' </param>
	''' <param name="textures">
	'''     The PointCollection corresponding to the TextureCoordinates
	'''     property of the MeshGeometry3D.
	''' </param>
	Protected Overrides Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		' Clear all four collections.
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		' Loop for outside (side = 1) and inside (side = -1).
		For side As Integer = 1 To -1 Step -2
			Dim offset As Integer = vertices.Count

			' Begin at the top end. Fill the collections.
			For stack As Integer = 0 To EndStacks
				Dim y As Double = Length
				Dim radius__1 As Double = Radius + side * stack * Thickness / 2 / EndStacks
				Dim top As Integer = offset + (stack + 0) * (Slices + 1)
				Dim bot As Integer = offset + (stack + 1) * (Slices + 1)

				For slice As Integer = 0 To Slices
					Dim theta As Double = slice * 2 * Math.PI / Slices
					Dim x As Double = -radius__1 * Math.Sin(theta)
					Dim z As Double = -radius__1 * Math.Cos(theta)

					vertices.Add(New Point3D(x, y, z))
					normals.Add(New Vector3D(0, side, 0))
					textures.Add(New Point(CDbl(slice) / Slices, Fold * stack / EndStacks))

					If stack < EndStacks AndAlso slice < Slices Then
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

			' Length of the tube: Fill in the collections.
			For stack As Integer = 0 To Stacks
				Dim y As Double = Length - stack * Length / Stacks
				Dim top As Integer = offset + (stack + 0) * (Slices + 1)
				Dim bot As Integer = offset + (stack + 1) * (Slices + 1)

				For slice As Integer = 0 To Slices
					Dim theta As Double = slice * 2 * Math.PI / Slices
					Dim x As Double = -(Radius + side * Thickness / 2) * Math.Sin(theta)
					Dim z As Double = -(Radius + side * Thickness / 2) * Math.Cos(theta)

					vertices.Add(New Point3D(x, y, z))
					normals.Add(New Vector3D(side * x, 0, side * z))
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
				Dim radius__1 As Double = Radius + side * Thickness / 2 * (1 - CDbl(stack) / EndStacks)
				Dim top As Integer = offset + (stack + 0) * (Slices + 1)
				Dim bot As Integer = offset + (stack + 1) * (Slices + 1)

				For slice As Integer = 0 To Slices
					Dim theta As Double = slice * 2 * Math.PI / Slices
					Dim x As Double = -radius__1 * Math.Sin(theta)
					Dim z As Double = -radius__1 * Math.Cos(theta)

					vertices.Add(New Point3D(x, y, z))
					normals.Add(New Vector3D(0, -side, 0))
					textures.Add(New Point(CDbl(slice) / Slices, (1 - Fold) + Fold * stack / EndStacks))

					If stack < EndStacks AndAlso slice < Slices Then
						indices.Add(top + slice)
						indices.Add(bot + slice)
						indices.Add(top + slice + 1)

						indices.Add(top + slice + 1)
						indices.Add(bot + slice)
						indices.Add(bot + slice + 1)
					End If
				Next
			Next
		Next
	End Sub

	''' <summary>
	'''     Creates a new instance of the TubeMesh class.
	''' </summary>
	''' <returns>
	'''     A new instance of TubeMesh.
	''' </returns>
	''' <remarks>
	'''     Overriding this method is required when deriving 
	'''     from the Freezable class.
	''' </remarks>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New TubeMesh()
	End Function
End Class

