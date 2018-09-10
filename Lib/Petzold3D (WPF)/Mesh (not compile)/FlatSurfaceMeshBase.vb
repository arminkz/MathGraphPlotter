'----------------------------------------------------
' FlatSurfaceMeshBase.cs (c) 2007 by Charles Petzold
'----------------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public MustInherit Class FlatSurfaceMeshBase
	Inherits MeshGeneratorBase
	' Precreated arrayes used in the TriangleSubdivide method.
	Private verticesBase As Point3D() = New Point3D(2) {}
	Private normalsBase As Vector3D() = New Vector3D(2) {}
	Private indicesBase As Integer() = New Integer(2) {}
	Private texturesBase As Point() = New Point(2) {}

	''' <summary>
	''' 
	''' </summary>
    Public Shared ReadOnly SlicesProperty As DependencyProperty = DependencyProperty.Register("Slices", GetType(Integer), GetType(FlatSurfaceMeshBase), New PropertyMetadata(1, AddressOf PropertyChanged, AddressOf ValidateSlices))

	''' <summary>
	''' 
	''' </summary>
	Public Property Slices() As Integer
		Get
			Return CInt(GetValue(SlicesProperty))
		End Get
		Set
			SetValue(SlicesProperty, value)
		End Set
	End Property

	Private Shared Function ValidateSlices(obj As Object) As Boolean
		Return CInt(obj) > 0
	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <param name="vertices"></param>
	''' <param name="normals"></param>
	''' <param name="indices"></param>
	''' <param name="textures"></param>
	Protected Sub TriangleSubdivide(vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		For i As Integer = 0 To 2
			verticesBase(2 - i) = vertices(vertices.Count - 1)
			normalsBase(2 - i) = normals(vertices.Count - 1)
			texturesBase(2 - i) = textures(vertices.Count - 1)

			vertices.RemoveAt(vertices.Count - 1)
			normals.RemoveAt(normals.Count - 1)
			indices.RemoveAt(indices.Count - 1)
			textures.RemoveAt(textures.Count - 1)
		Next

		Dim indexStart As Integer = vertices.Count

		For slice As Integer = 0 To Slices
			Dim weight As Double = CDbl(slice) / Slices

			Dim vertex1 As Point3D = Point3DWeight(verticesBase(0), verticesBase(1), weight)
			Dim vertex2 As Point3D = Point3DWeight(verticesBase(0), verticesBase(2), weight)

			Dim normal1 As Vector3D = Vector3DWeight(normalsBase(0), normalsBase(1), weight)
			Dim normal2 As Vector3D = Vector3DWeight(normalsBase(0), normalsBase(2), weight)

			Dim texture1 As Point = PointWeight(texturesBase(0), texturesBase(1), weight)
			Dim texture2 As Point = PointWeight(texturesBase(0), texturesBase(2), weight)

			For i As Integer = 0 To slice
				weight = CDbl(i) / slice

				If [Double].IsNaN(weight) Then
					weight = 0
				End If

				vertices.Add(Point3DWeight(vertex1, vertex2, weight))
				normals.Add(Vector3DWeight(normal1, normal2, weight))
				textures.Add(PointWeight(texture1, texture2, weight))
			Next
		Next

		For slice As Integer = 0 To Slices - 1
			Dim base1 As Integer = (slice + 1) * slice \ 2
			Dim base2 As Integer = base1 + slice + 1

			For i As Integer = 0 To 2 * slice
				Dim half As Integer = i \ 2

				If (i And 1) = 0 Then
					' even
					indices.Add(indexStart + base1 + half)
					indices.Add(indexStart + base2 + half)
					indices.Add(indexStart + base2 + half + 1)
				Else
					' odd
					indices.Add(indexStart + base1 + half)
					indices.Add(indexStart + base2 + half + 1)
					indices.Add(indexStart + base1 + half + 1)
				End If
			Next
		Next
	End Sub

	Private Function Point3DWeight(one As Point3D, two As Point3D, wt2 As Double) As Point3D
		Dim wt1 As Double = 1 - wt2
		Return New Point3D(wt1 * one.X + wt2 * two.X, wt1 * one.Y + wt2 * two.Y, wt1 * one.Z + wt2 * two.Z)
	End Function

	Private Function Vector3DWeight(one As Vector3D, two As Vector3D, wt2 As Double) As Vector3D
		Dim wt1 As Double = 1 - wt2
		Return New Vector3D(wt1 * one.X + wt2 * two.X, wt1 * one.Y + wt2 * two.Y, wt1 * one.Z + wt2 * two.Z)
	End Function

	Private Function PointWeight(one As Point, two As Point, wt2 As Double) As Point
		Dim wt1 As Double = 1 - wt2
		Return New Point(wt1 * one.X + wt2 * two.X, wt1 * one.Y + wt2 * two.Y)
	End Function
End Class
