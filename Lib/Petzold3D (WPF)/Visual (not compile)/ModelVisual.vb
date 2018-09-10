'--------------------------------------------
' ModelVisual.cs (c) 2007 by Charles Petzold
'--------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class ModelVisual
	Inherits ModelVisualBase
    Public Shared ReadOnly MeshGeneratorProperty As DependencyProperty = DependencyProperty.Register("MeshGeometry", GetType(MeshGeneratorBase), GetType(ModelVisual), New PropertyMetadata(Nothing, AddressOf PropertyChanged))

	Public Property MeshGenerator() As MeshGeneratorBase
		Get
			Return DirectCast(GetValue(MeshGeneratorProperty), MeshGeneratorBase)
		End Get
		Set
			SetValue(MeshGeneratorProperty, value)
		End Set
	End Property

	Protected Overrides Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		Dim mesh As MeshGeometry3D = MeshGenerator.Geometry

		For Each vertex As Point3D In mesh.Positions
			vertices.Add(vertex)
		Next

		For Each normal As Vector3D In mesh.Normals
			normals.Add(normal)
		Next

		For Each index As Integer In mesh.TriangleIndices
			indices.Add(index)
		Next

		For Each texture As Point In mesh.TextureCoordinates
			textures.Add(texture)
		Next
	End Sub
End Class
