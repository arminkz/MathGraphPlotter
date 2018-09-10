'----------------------------------------
' BoxMesh.cs (c) 2007 by Charles Petzold
'----------------------------------------
Imports System.Windows
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D

''' <summary>
'''     Generates a MeshGeometry3D object for a box centered on the origin.
''' </summary>
''' <remarks>
'''     The MeshGeometry3D object this class creates is available as the
'''     Geometry property. You can share the same instance of a BoxMesh
'''     object with multiple 3D visuals. In XAML files, the BoxMesh
'''     tag will probably appear in a resource section.
''' </remarks>
Public Class BoxMesh
	Inherits MeshGeneratorBase
	''' <summary>
	'''     Initializes a new instance of the BoxMesh class.
	''' </summary>
	Public Sub New()
		PropertyChanged(New DependencyPropertyChangedEventArgs())
	End Sub

	''' <summary>
	'''     Identifies the Width dependency property.
	''' </summary>
	''' <value>
	'''     The width of the box in world units.
	'''     The default is 1. 
	''' </value>
	Public Shared ReadOnly WidthProperty As DependencyProperty = DependencyProperty.Register("Width", GetType(Double), GetType(BoxMesh), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the width of the box.
	''' </summary>
	Public Property Width() As Double
		Get
			Return CDbl(GetValue(WidthProperty))
		End Get
		Set
			SetValue(WidthProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Height dependency property.
	''' </summary>
	''' <value>
	'''     The height of the box in world units.
	'''     The default is 1. 
	''' </value>
	Public Shared ReadOnly HeightProperty As DependencyProperty = DependencyProperty.Register("Height", GetType(Double), GetType(BoxMesh), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the height of the box.
	''' </summary>
	Public Property Height() As Double
		Get
			Return CDbl(GetValue(HeightProperty))
		End Get
		Set
			SetValue(HeightProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Depth dependency property.
	''' </summary>
	''' <value>
	'''     The depth of the box in world units.
	'''     The default is 1. 
	''' </value>
	Public Shared ReadOnly DepthProperty As DependencyProperty = DependencyProperty.Register("Depth", GetType(Double), GetType(BoxMesh), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the depth of the box.
	''' </summary>
	Public Property Depth() As Double
		Get
			Return CDbl(GetValue(DepthProperty))
		End Get
		Set
			SetValue(DepthProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Slices dependency property.
	''' </summary>
	Public Shared ReadOnly SlicesProperty As DependencyProperty = DependencyProperty.Register("Slices", GetType(Integer), GetType(BoxMesh), New PropertyMetadata(1, AddressOf PropertyChanged), AddressOf ValidateDivisions)

	''' <summary>
	'''     Gets or sets the number of divisions across the box width.
	''' </summary>
	''' <value>
	'''     The number of divisions across the box width. 
	'''     This property must be at least 1. 
	'''     The default value is 1.
	''' </value>
	Public Property Slices() As Integer
		Get
			Return CInt(GetValue(SlicesProperty))
		End Get
		Set
			SetValue(SlicesProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Stacks dependency property.
	''' </summary>
	Public Shared ReadOnly StacksProperty As DependencyProperty = DependencyProperty.Register("Stacks", GetType(Integer), GetType(BoxMesh), New PropertyMetadata(1, AddressOf PropertyChanged), AddressOf ValidateDivisions)

	''' <summary>
	'''     Gets or sets the number of divisions in the box height.
	''' </summary>
	''' <value>
	'''     This property must be at least 1. 
	'''     The default value is 1.
	''' </value>
	Public Property Stacks() As Integer
		Get
			Return CInt(GetValue(StacksProperty))
		End Get
		Set
			SetValue(StacksProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Layers dependency property.
	''' </summary>
	Public Shared ReadOnly LayersProperty As DependencyProperty = DependencyProperty.Register("Layers", GetType(Integer), GetType(BoxMesh), New PropertyMetadata(1, AddressOf PropertyChanged), AddressOf ValidateDivisions)

	''' <summary>
	'''     Gets or sets the number of divisions in the box depth.
	''' </summary>
	''' <value>
	'''     This property must be at least 1. 
	'''     The default value is 1.
	''' </value>
	Public Property Layers() As Integer
		Get
			Return CInt(GetValue(LayersProperty))
		End Get
		Set
			SetValue(LayersProperty, value)
		End Set
	End Property

	' Validation callback for Slices, Stacks, Layers.
	Private Shared Function ValidateDivisions(obj As Object) As Boolean
		Return CInt(obj) > 0
	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <param name="args"></param>
	''' <param name="vertices"></param>
	''' <param name="normals"></param>
	''' <param name="indices"></param>
	''' <param name="textures"></param>
	Protected Overrides Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
		' Clear all four collections.
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		Dim x As Double, y As Double, z As Double
		Dim indexBase As Integer = 0

		' Front side.
		' -----------
		z = Depth / 2

		' Fill the vertices, normals, textures collections.
		For stack As Integer = 0 To Stacks
			y = Height / 2 - stack * Height / Stacks

			For slice As Integer = 0 To Slices
				x = -Width / 2 + slice * Width / Slices
				Dim point As New Point3D(x, y, z)
				vertices.Add(point)

				normals.Add(point - New Point3D(x, y, 0))
				textures.Add(New Point(CDbl(slice) / Slices, CDbl(stack) / Stacks))
			Next
		Next

		' Fill the indices collection.
		For stack As Integer = 0 To Stacks - 1
			For slice As Integer = 0 To Slices - 1
				indices.Add((stack + 0) * (Slices + 1) + slice)
				indices.Add((stack + 1) * (Slices + 1) + slice)
				indices.Add((stack + 0) * (Slices + 1) + slice + 1)

				indices.Add((stack + 0) * (Slices + 1) + slice + 1)
				indices.Add((stack + 1) * (Slices + 1) + slice)
				indices.Add((stack + 1) * (Slices + 1) + slice + 1)
			Next
		Next

		' Rear side.
		' -----------
		indexBase = vertices.Count
		z = -Depth / 2

		' Fill the vertices, normals, textures collections.
		For stack As Integer = 0 To Stacks
			y = Height / 2 - stack * Height / Stacks

			For slice As Integer = 0 To Slices
				x = Width / 2 - slice * Width / Slices
				Dim point As New Point3D(x, y, z)
				vertices.Add(point)

				normals.Add(point - New Point3D(x, y, 0))
				textures.Add(New Point(CDbl(slice) / Slices, CDbl(stack) / Stacks))
			Next
		Next

		' Fill the indices collection.
		For stack As Integer = 0 To Stacks - 1
			For slice As Integer = 0 To Slices - 1
				indices.Add(indexBase + (stack + 0) * (Slices + 1) + slice)
				indices.Add(indexBase + (stack + 1) * (Slices + 1) + slice)
				indices.Add(indexBase + (stack + 0) * (Slices + 1) + slice + 1)

				indices.Add(indexBase + (stack + 0) * (Slices + 1) + slice + 1)
				indices.Add(indexBase + (stack + 1) * (Slices + 1) + slice)
				indices.Add(indexBase + (stack + 1) * (Slices + 1) + slice + 1)
			Next
		Next

		' Left side.
		' -----------
		indexBase = vertices.Count
		x = -Width / 2

		' Fill the vertices, normals, textures collections.
		For stack As Integer = 0 To Stacks
			y = Height / 2 - stack * Height / Stacks

			For layer As Integer = 0 To Layers
				z = -Depth / 2 + layer * Depth / Layers
				Dim point As New Point3D(x, y, z)
				vertices.Add(point)

				normals.Add(point - New Point3D(0, y, z))
				textures.Add(New Point(CDbl(layer) / Layers, CDbl(stack) / Stacks))
			Next
		Next

		' Fill the indices collection.
		For stack As Integer = 0 To Stacks - 1
			For layer As Integer = 0 To Layers - 1
				indices.Add(indexBase + (stack + 0) * (Layers + 1) + layer)
				indices.Add(indexBase + (stack + 1) * (Layers + 1) + layer)
				indices.Add(indexBase + (stack + 0) * (Layers + 1) + layer + 1)

				indices.Add(indexBase + (stack + 0) * (Layers + 1) + layer + 1)
				indices.Add(indexBase + (stack + 1) * (Layers + 1) + layer)
				indices.Add(indexBase + (stack + 1) * (Layers + 1) + layer + 1)
			Next
		Next

		' Right side.
		' -----------
		indexBase = vertices.Count
		x = Width / 2

		' Fill the vertices, normals, textures collections.
		For stack As Integer = 0 To Stacks
			y = Height / 2 - stack * Height / Stacks

			For layer As Integer = 0 To Layers
				z = Depth / 2 - layer * Depth / Layers
				Dim point As New Point3D(x, y, z)
				vertices.Add(point)

				normals.Add(point - New Point3D(0, y, z))
				textures.Add(New Point(CDbl(layer) / Layers, CDbl(stack) / Stacks))
			Next
		Next

		' Fill the indices collection.
		For stack As Integer = 0 To Stacks - 1
			For layer As Integer = 0 To Layers - 1
				indices.Add(indexBase + (stack + 0) * (Layers + 1) + layer)
				indices.Add(indexBase + (stack + 1) * (Layers + 1) + layer)
				indices.Add(indexBase + (stack + 0) * (Layers + 1) + layer + 1)

				indices.Add(indexBase + (stack + 0) * (Layers + 1) + layer + 1)
				indices.Add(indexBase + (stack + 1) * (Layers + 1) + layer)
				indices.Add(indexBase + (stack + 1) * (Layers + 1) + layer + 1)
			Next
		Next

		' Top side.
		' -----------
		indexBase = vertices.Count
		y = Height / 2

		' Fill the vertices, normals, textures collections.
		For layer As Integer = 0 To Layers
			z = -Depth / 2 + layer * Depth / Layers

			For slice As Integer = 0 To Slices
				x = -Width / 2 + slice * Width / Slices
				Dim point As New Point3D(x, y, z)
				vertices.Add(point)

				normals.Add(point - New Point3D(x, 0, z))
				textures.Add(New Point(CDbl(slice) / Slices, CDbl(layer) / Layers))
			Next
		Next

		' Fill the indices collection.
		For layer As Integer = 0 To Layers - 1
			For slice As Integer = 0 To Slices - 1
				indices.Add(indexBase + (layer + 0) * (Slices + 1) + slice)
				indices.Add(indexBase + (layer + 1) * (Slices + 1) + slice)
				indices.Add(indexBase + (layer + 0) * (Slices + 1) + slice + 1)

				indices.Add(indexBase + (layer + 0) * (Slices + 1) + slice + 1)
				indices.Add(indexBase + (layer + 1) * (Slices + 1) + slice)
				indices.Add(indexBase + (layer + 1) * (Slices + 1) + slice + 1)
			Next
		Next

		' Bottom side.
		' -----------
		indexBase = vertices.Count
		y = -Height / 2

		' Fill the vertices, normals, textures collections.
		For layer As Integer = 0 To Layers
			z = Depth / 2 - layer * Depth / Layers

			For slice As Integer = 0 To Slices
				x = -Width / 2 + slice * Width / Slices
				Dim point As New Point3D(x, y, z)
				vertices.Add(point)

				normals.Add(point - New Point3D(x, 0, z))
				textures.Add(New Point(CDbl(slice) / Slices, CDbl(layer) / Layers))
			Next
		Next

		' Fill the indices collection.
		For layer As Integer = 0 To Layers - 1
			For slice As Integer = 0 To Slices - 1
				indices.Add(indexBase + (layer + 0) * (Slices + 1) + slice)
				indices.Add(indexBase + (layer + 1) * (Slices + 1) + slice)
				indices.Add(indexBase + (layer + 0) * (Slices + 1) + slice + 1)

				indices.Add(indexBase + (layer + 0) * (Slices + 1) + slice + 1)
				indices.Add(indexBase + (layer + 1) * (Slices + 1) + slice)
				indices.Add(indexBase + (layer + 1) * (Slices + 1) + slice + 1)
			Next
		Next
	End Sub

	''' <summary>
	'''     Creates a new instance of the BoxMesh class.
	''' </summary>
	''' <returns>
	'''     A new instance of BoxMesh.
	''' </returns>
	''' <remarks>
	'''     Overriding this method is required when deriving 
	'''     from the Freezable class.
	''' </remarks>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New BoxMesh()
	End Function
End Class

