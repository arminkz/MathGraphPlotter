'---------------------------------------------------
' HollowCylinderMesh.cs (c) 2007 by Charles Petzold
'---------------------------------------------------
Imports System.Windows
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
'''     Generates a MeshGeometry3D object for a hollow cylinder.
''' </summary>
''' <remarks>
'''     The MeshGeometry3D object this class creates is available as the
'''     Geometry property. You can share the same instance of a 
'''     HollowCylinderMesh object with multiple 3D visuals. 
'''     In XAML files, the HollowCylinderMesh
'''     tag will probably appear in a resource section.
'''     The cylinder is centered on the positive Y axis.
''' </remarks>
Public Class HollowCylinderMesh
	Inherits CylindricalMeshBase
	''' <summary>
	'''     Initializes a new instance of the HollowCylinderMesh class.
	''' </summary>
	Public Sub New()
		PropertyChanged(New DependencyPropertyChangedEventArgs())
	End Sub

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

		' Fill the vertices, normals, and textures collections.
		For stack As Integer = 0 To Stacks
			Dim y As Double = Length - stack * Length / Stacks

			For slice As Integer = 0 To Slices
				Dim theta As Double = slice * 2 * Math.PI / Slices
				Dim x As Double = -Radius * Math.Sin(theta)
				Dim z As Double = -Radius * Math.Cos(theta)

				normals.Add(New Vector3D(x, 0, z))
				vertices.Add(New Point3D(x, y, z))
				textures.Add(New Point(CDbl(slice) / Slices, CDbl(stack) / Stacks))
			Next
		Next

		' Fill the indices collection.
		For stack As Integer = 0 To Stacks - 1
			For slice As Integer = 0 To Slices - 1
				indices.Add((stack + 0) * (Slices + 1) + slice)
				indices.Add((stack + 1) * (Slices + 1) + slice)
				indices.Add((stack + 0) * (Slices + 1) + slice + 1)

				indices.Add((stack + 0) * (Slices + 1) + slice + 1)
				indices.Add((stack + 1) * (Slices + 1) + slice)
				indices.Add((stack + 1) * (Slices + 1) + slice + 1)
			Next
		Next
	End Sub

	''' <summary>
	'''     Creates a new instance of the HollowCylinderMesh class.
	''' </summary>
	''' <returns>
	'''     A new instance of HollowCylinderMesh.
	''' </returns>
	''' <remarks>
	'''     Overriding this method is required when deriving 
	'''     from the Freezable class.
	''' </remarks>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New HollowCylinderMesh()
	End Function
End Class

