'---------------------------------------------------
' PolyhedronMeshBase.cs (c) 2007 by Charles Petzold
'---------------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public MustInherit Class PolyhedronMeshBase
	Inherits FlatSurfaceMeshBase
	''' <summary>
	''' 
	''' </summary>
	Public Sub New()
		TextureCoordinates = TextureCoordinates.Clone()
	End Sub

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly TextureCoordinatesProperty As DependencyProperty = MeshGeometry3D.TextureCoordinatesProperty.AddOwner(GetType(PolyhedronMeshBase), New PropertyMetadata(AddressOf PropertyChanged))

	''' <summary>
	''' 
	''' </summary>
	Public Property TextureCoordinates() As PointCollection
		Get
			Return DirectCast(GetValue(TextureCoordinatesProperty), PointCollection)
		End Get
		Set
			SetValue(TextureCoordinatesProperty, value)
		End Set
	End Property

	Protected MustOverride ReadOnly Property Faces() As Point3D(,)

	Protected Overrides Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		Dim faces__1 As Point3D(,) = Faces
		Dim texturesBase As PointCollection = TextureCoordinates
		Dim indexTextures As Integer = 0

		For face As Integer = 0 To faces__1.GetLength(0) - 1
			Dim normal As Vector3D = Vector3D.CrossProduct(faces__1(face, 1) - faces__1(face, 0), faces__1(face, 2) - faces__1(face, 0))

			' For faces that are triangles.
			If faces__1.GetLength(1) = 3 Then
				Dim indexBase As Integer = vertices.Count

				For i As Integer = 0 To 2
					vertices.Add(faces__1(face, i))
					normals.Add(normal)
					indices.Add(indexBase + i)

					If texturesBase IsNot Nothing AndAlso texturesBase.Count > 0 Then
						textures.Add(texturesBase(indexTextures))
						indexTextures = (indexTextures + 1) Mod texturesBase.Count
					End If
				Next

				If Slices > 1 Then
					TriangleSubdivide(vertices, normals, indices, textures)
				End If
			Else

				' For faces that are not triangles.
				For i As Integer = 0 To faces__1.GetLength(1) - 2
					Dim indexBase As Integer = vertices.Count
					Dim num As Integer = faces__1.GetLength(1) - 1

					vertices.Add(faces__1(face, 0))
					vertices.Add(faces__1(face, i + 1))
					vertices.Add(faces__1(face, (i + 1) Mod num + 1))

					If texturesBase IsNot Nothing AndAlso texturesBase.Count >= faces__1.GetLength(1) Then
						textures.Add(texturesBase(indexTextures + 0))
						textures.Add(texturesBase(indexTextures + i + 1))
						textures.Add(texturesBase(indexTextures + (i + 1) Mod num + 1))
					End If

					normals.Add(normal)
					normals.Add(normal)
					normals.Add(normal)

					indices.Add(indexBase + 0)
					indices.Add(indexBase + 1)
					indices.Add(indexBase + 2)

					If Slices > 1 Then
						TriangleSubdivide(vertices, normals, indices, textures)
					End If
				Next
				If texturesBase IsNot Nothing AndAlso texturesBase.Count > 0 Then
					indexTextures = (indexTextures + faces__1.GetLength(1)) Mod texturesBase.Count
				End If
			End If
		Next
	End Sub
End Class

