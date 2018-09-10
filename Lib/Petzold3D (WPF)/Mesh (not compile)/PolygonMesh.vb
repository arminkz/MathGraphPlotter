'--------------------------------------------
' PolygonMesh.cs (c) 2007 by Charles Petzold 
'--------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
''' 
''' </summary>
Public Class PolygonMesh
	Inherits FlatSurfaceMeshBase
	''' <summary>
	''' 
	''' </summary>
	Public Sub New()
		PropertyChanged(New DependencyPropertyChangedEventArgs())
	End Sub

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly LengthProperty As DependencyProperty = DependencyProperty.Register("Length", GetType(Double), GetType(PolygonMesh), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	''' <summary>
	''' 
	''' </summary>
	Public Property Length() As Double
		Get
			Return CDbl(GetValue(LengthProperty))
		End Get
		Set
			SetValue(LengthProperty, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	' check that greater than 2 !!!!!
	Public Shared ReadOnly SidesProperty As DependencyProperty = DependencyProperty.Register("Sides", GetType(Integer), GetType(PolygonMesh), New PropertyMetadata(5, AddressOf PropertyChanged))

	''' <summary>
	''' 
	''' </summary>
	Public Property Sides() As Integer
		Get
			Return CInt(GetValue(SidesProperty))
		End Get
		Set
			SetValue(SidesProperty, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	''' <param name="args"></param>
	''' <param name="vertices"></param>
	''' <param name="normals"></param>
	''' <param name="indices"></param>
	''' <param name="textures"></param>
	Protected Overrides Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		Dim normal As New Vector3D(0, 0, 1)
		Dim angleInner As Double = 2 * Math.PI / Sides
		Dim radius As Double = Length / 2 / Math.Sin(angleInner / 2)
		Dim angle As Double = 3 * Math.PI / 2 + angleInner / 2
		Dim xMin As Double = 0, xMax As Double = 0, yMin As Double = 0, yMax As Double = 0

		For side As Integer = 0 To Sides - 1
			Dim x As Double = Math.Cos(angle)
			Dim y As Double = Math.Sin(angle)

			xMin = Math.Min(xMin, x)
			xMax = Math.Max(xMax, x)
			yMin = Math.Min(yMin, y)
			yMax = Math.Max(yMax, y)

			angle += angleInner
		Next

		angle = 3 * Math.PI / 2 + angleInner / 2

		For side As Integer = 0 To Sides - 1
			vertices.Add(New Point3D(0, 0, 0))
			textures.Add(New Point(-xMin / (xMax - xMin), yMax / (yMax - yMin)))
			normals.Add(normal)

			Dim x As Double = Math.Cos(angle)
			Dim y As Double = Math.Sin(angle)
			vertices.Add(New Point3D(x, y, 0))
			textures.Add(New Point((x - xMin) / (xMax - xMin), (yMax - y) / (yMax - yMin)))
			normals.Add(normal)

			angle += angleInner
			x = Math.Cos(angle)
			y = Math.Sin(angle)
			vertices.Add(New Point3D(x, y, 0))
			textures.Add(New Point((x - xMin) / (xMax - xMin), (yMax - y) / (yMax - yMin)))
			normals.Add(normal)

			Dim index As Integer = vertices.Count - 3
			indices.Add(index)
			indices.Add(index + 1)
			indices.Add(index + 2)

			If Slices > 1 Then
				TriangleSubdivide(vertices, normals, indices, textures)
			End If
		Next
	End Sub

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New PolygonMesh()
	End Function
End Class
