'-------------------------------------------------
' DodecahedronMesh.cs (c) 2007 by Charles Petzold
'-------------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class DodecahedronMesh
	Inherits PolyhedronMeshBase
	Shared ReadOnly G As Double = (1 + Math.Sqrt(5)) / 2
	' approximately 1.618
	Shared ReadOnly H As Double = 1 / G
	' approximately 0.618
	Shared ReadOnly A As Double = (3 * H + 4) / 5
	' approximately 1.171
	Shared ReadOnly B As Double = (2 + G) / 5
	' approximately 0.724
	Shared ReadOnly m_faces As Point3D(,) = New Point3D(11, 5) {{New Point3D(A, -B, 0), New Point3D(1, -1, -1), New Point3D(G, 0, -H), New Point3D(G, 0, H), New Point3D(1, -1, 1), New Point3D(H, -G, 0)}, {New Point3D(-A, -B, 0), New Point3D(-1, -1, 1), New Point3D(-G, 0, H), New Point3D(-G, 0, -H), New Point3D(-1, -1, -1), New Point3D(-H, -G, 0)}, {New Point3D(-A, B, 0), New Point3D(-1, 1, -1), New Point3D(-G, 0, -H), New Point3D(-G, 0, H), New Point3D(-1, 1, 1), New Point3D(-H, G, 0)}, {New Point3D(A, B, 0), New Point3D(1, 1, 1), New Point3D(G, 0, H), New Point3D(G, 0, -H), New Point3D(1, 1, -1), New Point3D(H, G, 0)}, {New Point3D(-B, 0, -A), New Point3D(-1, 1, -1), New Point3D(0, H, -G), New Point3D(0, -H, -G), New Point3D(-1, -1, -1), New Point3D(-G, 0, -H)}, {New Point3D(-B, 0, A), New Point3D(-1, -1, 1), New Point3D(0, -H, G), New Point3D(0, H, G), New Point3D(-1, 1, 1), New Point3D(-G, 0, H)}, _
		{New Point3D(B, 0, -A), New Point3D(1, -1, -1), New Point3D(0, -H, -G), New Point3D(0, H, -G), New Point3D(1, 1, -1), New Point3D(G, 0, -H)}, {New Point3D(B, 0, A), New Point3D(1, 1, 1), New Point3D(0, H, G), New Point3D(0, -H, G), New Point3D(1, -1, 1), New Point3D(G, 0, H)}, {New Point3D(0, -A, -B), New Point3D(1, -1, -1), New Point3D(H, -G, 0), New Point3D(-H, -G, 0), New Point3D(-1, -1, -1), New Point3D(0, -H, -G)}, {New Point3D(0, A, -B), New Point3D(-1, 1, -1), New Point3D(-H, G, 0), New Point3D(H, G, 0), New Point3D(1, 1, -1), New Point3D(0, H, -G)}, {New Point3D(0, -A, B), New Point3D(-1, -1, 1), New Point3D(-H, -G, 0), New Point3D(H, -G, 0), New Point3D(1, -1, 1), New Point3D(0, -H, G)}, {New Point3D(0, A, B), New Point3D(1, 1, 1), New Point3D(H, G, 0), New Point3D(-H, G, 0), New Point3D(-1, 1, 1), New Point3D(0, H, G)}}

	Public Sub New()
		' Set TextureCoordinates to default values.
		Dim textures As PointCollection = TextureCoordinates
		TextureCoordinates = Nothing

		textures.Add(New Point(0.5, 0.5))
		textures.Add(New Point(0.5, 0))
		textures.Add(New Point(1, 0.4))
		textures.Add(New Point(0.85, 1))
		textures.Add(New Point(0.15, 1))
		textures.Add(New Point(0, 0.4))

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
