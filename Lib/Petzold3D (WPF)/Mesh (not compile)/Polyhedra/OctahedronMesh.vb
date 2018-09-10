'-----------------------------------------------
' OctahedronMesh.cs (c) 2007 by Charles Petzold
'-----------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class OctahedronMesh
	Inherits PolyhedronMeshBase
	' front upper right

	' front upper left

	' front lower left

	' front lower right

	' back lower right

	' back lower left

	' back upper left

	' back upper right
	Shared ReadOnly m_faces As Point3D(,) = New Point3D(7, 2) {{New Point3D(0, 1, 0), New Point3D(0, 0, 1), New Point3D(1, 0, 0)}, {New Point3D(0, 1, 0), New Point3D(-1, 0, 0), New Point3D(0, 0, 1)}, {New Point3D(0, -1, 0), New Point3D(0, 0, 1), New Point3D(-1, 0, 0)}, {New Point3D(0, -1, 0), New Point3D(1, 0, 0), New Point3D(0, 0, 1)}, {New Point3D(0, -1, 0), New Point3D(0, 0, -1), New Point3D(1, 0, 0)}, {New Point3D(0, -1, 0), New Point3D(-1, 0, 0), New Point3D(0, 0, -1)}, _
		{New Point3D(0, 1, 0), New Point3D(0, 0, -1), New Point3D(-1, 0, 0)}, {New Point3D(0, 1, 0), New Point3D(1, 0, 0), New Point3D(0, 0, -1)}}

	Public Sub New()
		' Set TextureCoordinates to default values.
		Dim textures As PointCollection = TextureCoordinates
		TextureCoordinates = Nothing

		textures.Add(New Point(0, 0))
		textures.Add(New Point(1, 1))
		textures.Add(New Point(1, 0))

		textures.Add(New Point(0, 0))
		textures.Add(New Point(0, 1))
		textures.Add(New Point(1, 1))

		textures.Add(New Point(0, 0))
		textures.Add(New Point(1, 1))
		textures.Add(New Point(0, 1))

		textures.Add(New Point(0, 0))
		textures.Add(New Point(1, 0))
		textures.Add(New Point(1, 1))

		TextureCoordinates = textures
	End Sub

	Protected Overrides ReadOnly Property Faces() As Point3D(,)
		Get
			Return m_faces
		End Get
	End Property

	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New OctahedronMesh()
	End Function
End Class
