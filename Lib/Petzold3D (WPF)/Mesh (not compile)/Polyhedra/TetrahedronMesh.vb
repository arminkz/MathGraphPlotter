'------------------------------------------------
' TetrahedronMesh.cs (c) 2007 by Charles Petzold
'------------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class TetrahedronMesh
	Inherits PolyhedronMeshBase
	' upper-left front

	' lower-right front 

	' upper-right back

	' lower-left back
	Shared ReadOnly m_faces As Point3D(,) = New Point3D(3, 2) {{New Point3D(-1, 1, -1), New Point3D(-1, -1, 1), New Point3D(1, 1, 1)}, {New Point3D(1, -1, -1), New Point3D(1, 1, 1), New Point3D(-1, -1, 1)}, {New Point3D(1, 1, 1), New Point3D(1, -1, -1), New Point3D(-1, 1, -1)}, {New Point3D(-1, -1, 1), New Point3D(-1, 1, -1), New Point3D(1, -1, -1)}}

	Public Sub New()
		' Set TextureCoordinates to default values.
		Dim textures As PointCollection = TextureCoordinates
		TextureCoordinates = Nothing

		textures.Add(New Point(0, 0))
		textures.Add(New Point(0, 1))
		textures.Add(New Point(1, 0))

		textures.Add(New Point(1, 1))
		textures.Add(New Point(1, 0))
		textures.Add(New Point(0, 1))

		TextureCoordinates = textures
	End Sub

	Protected Overrides ReadOnly Property Faces() As Point3D(,)
		Get
			Return m_faces
		End Get
	End Property

	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New TetrahedronMesh()
	End Function
End Class
