'-------------------------------------------
' SphereMesh.cs (c) 2007 by Charles Petzold
'-------------------------------------------
Imports System.Windows
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D

''' <summary>
'''     Generates a MeshGeometry3D object for a sphere.
''' </summary>
''' <remarks>
'''     The MeshGeometry3D object this class creates is available as the
'''     Geometry property. You can share the same instance of a SphereMesh
'''     object with multiple 3D visuals. In XAML files, the SphereMesh
'''     tag will probably appear in a resource section.
''' </remarks>
Public Class SphereMesh
	Inherits MeshGeneratorBase
	''' <summary>
	'''     Initializes a new instance of the SphereMesh class.
	''' </summary>
	Public Sub New()
		PropertyChanged(New DependencyPropertyChangedEventArgs())
	End Sub

	''' <summary>
	'''     Identifies the Center dependency property.
	''' </summary>
	''' <value>
	'''     The identifier for the Center dependency property.
	''' </value>
	Public Shared ReadOnly CenterProperty As DependencyProperty = DependencyProperty.Register("Center", GetType(Point3D), GetType(SphereMesh), New PropertyMetadata(New Point3D(), AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the center of the sphere.
	''' </summary>
	''' <value>
	'''     The center of the sphere in the 3D coordinate system.
	'''     The default is the origin, the point (0, 0, 0).
	''' </value>
	Public Property Center() As Point3D
		Get
			Return CType(GetValue(CenterProperty), Point3D)
		End Get
		Set
			SetValue(CenterProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Radius dependency property.
	''' </summary>
	''' <value>
	'''     The radius of the sphere in world units.
	'''     The default is 1. 
	'''     The Radius property can be set to a negative value,
	'''     but in effect the sphere is turned inside out so that
	'''     the surface of the sphere is colored with the BackMaterial
	'''     brush rather than the Material brush.
	''' </value>
	Public Shared ReadOnly RadiusProperty As DependencyProperty = DependencyProperty.Register("Radius", GetType(Double), GetType(SphereMesh), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the radius of the sphere.
	''' </summary>
	Public Property Radius() As Double
		Get
			Return CDbl(GetValue(RadiusProperty))
		End Get
		Set
			SetValue(RadiusProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Slices dependency property.
	''' </summary>
	Public Shared ReadOnly SlicesProperty As DependencyProperty = DependencyProperty.Register("Slices", GetType(Integer), GetType(SphereMesh), New PropertyMetadata(36, AddressOf PropertyChanged), AddressOf ValidateSlices)

	' Validation callback for Slices.
	Private Shared Function ValidateSlices(obj As Object) As Boolean
		Return CInt(obj) > 2
	End Function

	''' <summary>
	'''     Gets or sets the number of divisions around the Y axis
	'''     used to approximate the sphere.
	''' </summary>
	''' <value>
	'''     The number of divisions around the Y axis. 
	'''     This property must be at least 3. 
	'''     The default value is 36.
	''' </value>
	''' <remarks>
	'''     If the sphere is pictured as a globe with the north pole 
	'''     to the top, the Slices property divides the sphere along 
	'''     lines of longitude. Each slice is equivalent to a number
	'''     of degrees of longitude equal to 360 divided by the 
	'''     Slices value.
	''' </remarks>
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
	Public Shared ReadOnly StacksProperty As DependencyProperty = DependencyProperty.Register("Stacks", GetType(Integer), GetType(SphereMesh), New PropertyMetadata(18, AddressOf PropertyChanged), AddressOf ValidateStacks)

	' Validation callback for Stacks.
	Private Shared Function ValidateStacks(obj As Object) As Boolean
		Return CInt(obj) > 1
	End Function

	''' <summary>
	'''     Gets or sets the number of divisions parallel to the XZ
	'''     plane used to approximate the sphere.
	''' </summary>
	''' <value>
	'''     The number of divisions parallel to the XZ plane. 
	'''     This property must be at least 2. 
	'''     The default value is 18.
	''' </value>
	''' <remarks>
	'''     If the sphere is pictured as a globe with the north pole 
	'''     to the top, the Stacks property divides the sphere along 
	'''     lines of latitude. Each stack is equivalent to a number
	'''     of degrees of latitude equal to 180 divided by the 
	'''     Stacks value.
	''' </remarks>
	Public Property Stacks() As Integer
		Get
			Return CInt(GetValue(StacksProperty))
		End Get
		Set
			SetValue(StacksProperty, value)
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
		' Clear all four collections.
		vertices.Clear()
		normals.Clear()
		indices.Clear()
		textures.Clear()

		' Fill the vertices, normals, and textures collections.
		For stack As Integer = 0 To Stacks
			Dim phi As Double = Math.PI / 2 - stack * Math.PI / Stacks
			Dim y As Double = Radius * Math.Sin(phi)
			Dim scale As Double = -Radius * Math.Cos(phi)

			For slice As Integer = 0 To Slices
				Dim theta As Double = slice * 2 * Math.PI / Slices
				Dim x As Double = scale * Math.Sin(theta)
				Dim z As Double = scale * Math.Cos(theta)

				Dim normal As New Vector3D(x, y, z)
				normals.Add(normal)
				vertices.Add(normal + Center)
				textures.Add(New Point(CDbl(slice) / Slices, CDbl(stack) / Stacks))
			Next
		Next

		' Fill the indices collection.
		For stack As Integer = 0 To Stacks - 1
			For slice As Integer = 0 To Slices - 1
				If stack <> 0 Then
					indices.Add((stack + 0) * (Slices + 1) + slice)
					indices.Add((stack + 1) * (Slices + 1) + slice)
					indices.Add((stack + 0) * (Slices + 1) + slice + 1)
				End If

				If stack <> Stacks - 1 Then
					indices.Add((stack + 0) * (Slices + 1) + slice + 1)
					indices.Add((stack + 1) * (Slices + 1) + slice)
					indices.Add((stack + 1) * (Slices + 1) + slice + 1)
				End If
			Next
		Next
	End Sub

	''' <summary>
	'''     Creates a new instance of the SphereMesh class.
	''' </summary>
	''' <returns>
	'''     A new instance of SphereMesh.
	''' </returns>
	''' <remarks>
	'''     Overriding this method is required when deriving 
	'''     from the Freezable class.
	''' </remarks>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New SphereMesh()
	End Function
End Class

