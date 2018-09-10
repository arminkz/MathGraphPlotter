'------------------------------------------
' WireFrame.cs (c) 2007 by Charles Petzold
'------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class WireFrame
	Inherits WireBase
	''' <summary>
	'''     Identifies the Positions dependency property.
	''' </summary>
    Public Shared ReadOnly PositionsProperty As DependencyProperty = MeshGeometry3D.PositionsProperty.AddOwner(GetType(WireFrame), New PropertyMetadata(AddressOf PropertyChanged))

	''' <summary>
	''' 
	''' </summary>
	Public Property Positions() As Point3DCollection
		Get
			Return DirectCast(GetValue(PositionsProperty), Point3DCollection)
		End Get
		Set
			SetValue(PositionsProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identfies the Normals dependency property.
	''' </summary>
	Public Shared ReadOnly NormalsProperty As DependencyProperty = MeshGeometry3D.NormalsProperty.AddOwner(GetType(WireFrame), New PropertyMetadata(AddressOf PropertyChanged))

	''' <summary>
	''' 
	''' </summary>
	Public Property Normals() As Vector3DCollection
		Get
			Return DirectCast(GetValue(NormalsProperty), Vector3DCollection)
		End Get
		Set
			SetValue(NormalsProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the TriangleIndices dependency property.
	''' </summary>
	Public Shared ReadOnly TriangleIndicesProperty As DependencyProperty = MeshGeometry3D.TriangleIndicesProperty.AddOwner(GetType(WireFrame), New PropertyMetadata(AddressOf PropertyChanged))

	''' <summary>
	''' 
	''' </summary>
	Public Property TriangleIndices() As Int32Collection
		Get
			Return DirectCast(GetValue(TriangleIndicesProperty), Int32Collection)
		End Get
		Set
			SetValue(TriangleIndicesProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the TextureCoordinates dependency property.
	''' </summary>
	Public Shared ReadOnly TextureCoordinatesProperty As DependencyProperty = MeshGeometry3D.TextureCoordinatesProperty.AddOwner(GetType(WireFrame), New PropertyMetadata(AddressOf PropertyChanged))

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

	''' <summary>
	''' 
	''' </summary>
	''' <param name="args"></param>
	''' <param name="lines"></param>
	Protected Overrides Sub Generate(args As DependencyPropertyChangedEventArgs, lines As Point3DCollection)
		Dim vertices As Point3DCollection = Positions
		Dim indices As Int32Collection = TriangleIndices
		lines.Clear()

		If vertices IsNot Nothing AndAlso vertices.Count > 0 AndAlso indices IsNot Nothing AndAlso indices.Count > 0 Then

			' Check that this doesn't overflow !!!!!!
			' -----------------------------------------

			' Special logic if there are no indices !!!!
			' -------------------------------------------

			For i As Integer = 0 To indices.Count - 1 Step 3
				lines.Add(vertices(indices(i + 0)))
				lines.Add(vertices(indices(i + 1)))

				lines.Add(vertices(indices(i + 1)))
				lines.Add(vertices(indices(i + 2)))

				lines.Add(vertices(indices(i + 2)))
				lines.Add(vertices(indices(i + 0)))
			Next
		End If
	End Sub
End Class






