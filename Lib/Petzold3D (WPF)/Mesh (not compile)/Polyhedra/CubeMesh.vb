'-----------------------------------------
' CubeMesh.cs (c) 2007 by Charles Petzold
'-----------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class CubeMesh
	Inherits PolyhedronMeshBase
	' front
	' back
	' left
	' right
	' top
	' bottom
	Shared ReadOnly m_faces As Point3D(,) = New Point3D(5, 4) {{New Point3D(0, 0, 1), New Point3D(-1, 1, 1), New Point3D(-1, -1, 1), New Point3D(1, -1, 1), New Point3D(1, 1, 1)}, {New Point3D(0, 0, -1), New Point3D(1, 1, -1), New Point3D(1, -1, -1), New Point3D(-1, -1, -1), New Point3D(-1, 1, -1)}, {New Point3D(-1, 0, 0), New Point3D(-1, 1, -1), New Point3D(-1, -1, -1), New Point3D(-1, -1, 1), New Point3D(-1, 1, 1)}, {New Point3D(1, 0, 0), New Point3D(1, 1, 1), New Point3D(1, -1, 1), New Point3D(1, -1, -1), New Point3D(1, 1, -1)}, {New Point3D(0, 1, 0), New Point3D(-1, 1, -1), New Point3D(-1, 1, 1), New Point3D(1, 1, 1), New Point3D(1, 1, -1)}, {New Point3D(0, -1, 0), New Point3D(-1, -1, 1), New Point3D(-1, -1, -1), New Point3D(1, -1, -1), New Point3D(1, -1, 1)}}

	Public Sub New()
		' Set TextureCoordinates to default values.
		Dim textures As PointCollection = TextureCoordinates
		TextureCoordinates = Nothing

		textures.Add(New Point(0.5, 0.5))
		textures.Add(New Point(0, 0))
		textures.Add(New Point(0, 1))
		textures.Add(New Point(1, 1))
		textures.Add(New Point(1, 0))

		TextureCoordinates = textures
	End Sub

	Protected Overrides ReadOnly Property Faces() As Point3D(,)
		Get
			Return m_faces
		End Get
	End Property

	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New CubeMesh()
	End Function
End Class
