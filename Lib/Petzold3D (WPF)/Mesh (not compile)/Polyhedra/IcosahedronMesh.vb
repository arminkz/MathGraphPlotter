'------------------------------------------------
' IcosahedronMesh.cs (c) 2007 by Charles Petzold
'------------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class IcosahedronMesh
	Inherits PolyhedronMeshBase
	Shared ReadOnly G As Double = (1 + Math.Sqrt(5)) / 2











	Shared ReadOnly m_faces As Point3D(,) = New Point3D(19, 2) {{New Point3D(0, G, 1), New Point3D(-1, 0, G), New Point3D(1, 0, G)}, {New Point3D(-1, 0, G), New Point3D(0, -G, 1), New Point3D(1, 0, G)}, {New Point3D(0, G, 1), New Point3D(1, 0, G), New Point3D(G, 1, 0)}, {New Point3D(1, 0, G), New Point3D(G, -1, 0), New Point3D(G, 1, 0)}, {New Point3D(0, G, 1), New Point3D(G, 1, 0), New Point3D(0, G, -1)}, {New Point3D(G, 1, 0), New Point3D(1, 0, -G), New Point3D(0, G, -1)}, _
		{New Point3D(0, G, 1), New Point3D(0, G, -1), New Point3D(-G, 1, 0)}, {New Point3D(0, G, -1), New Point3D(-1, 0, -G), New Point3D(-G, 1, 0)}, {New Point3D(0, G, 1), New Point3D(-G, 1, 0), New Point3D(-1, 0, G)}, {New Point3D(-G, 1, 0), New Point3D(-G, -1, 0), New Point3D(-1, 0, G)}, {New Point3D(1, 0, G), New Point3D(0, -G, 1), New Point3D(G, -1, 0)}, {New Point3D(0, -G, 1), New Point3D(0, -G, -1), New Point3D(G, -1, 0)}, _
		{New Point3D(G, 1, 0), New Point3D(G, -1, 0), New Point3D(1, 0, -G)}, {New Point3D(G, -1, 0), New Point3D(0, -G, -1), New Point3D(1, 0, -G)}, {New Point3D(0, G, -1), New Point3D(1, 0, -G), New Point3D(-1, 0, -G)}, {New Point3D(1, 0, -G), New Point3D(0, -G, -1), New Point3D(-1, 0, -G)}, {New Point3D(-G, 1, 0), New Point3D(-1, 0, -G), New Point3D(-G, -1, 0)}, {New Point3D(-1, 0, -G), New Point3D(0, -G, -1), New Point3D(-G, -1, 0)}, _
		{New Point3D(-1, 0, G), New Point3D(-G, -1, 0), New Point3D(0, -G, 1)}, {New Point3D(-G, -1, 0), New Point3D(0, -G, -1), New Point3D(0, -G, 1)}}

	Public Sub New()
		' Set TextureCoordinates to default values.
		Dim textures As PointCollection = TextureCoordinates
		TextureCoordinates = Nothing

		textures.Add(New Point(1, 0))
		textures.Add(New Point(0, 0))
		textures.Add(New Point(1, 1))

		textures.Add(New Point(0, 0))
		textures.Add(New Point(0, 1))
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
