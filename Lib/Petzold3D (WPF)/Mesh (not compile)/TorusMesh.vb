'------------------------------------------
' TorusMesh.cs (c) 2007 by Charles Petzold
'------------------------------------------
Imports System.Windows
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D

''' <summary>
'''     Generates a MeshGeometry3D object for a torus.
''' </summary>
''' <remarks>
'''     The MeshGeometry3D object this class creates is available as the
'''     Geometry property. You can share the same instance of a TorusMesh
'''     object with multiple 3D visuals. In XAML files, the TorusMesh
'''     tag will probably appear in a resource section.
''' </remarks>
Public Class TorusMesh
	Inherits MeshGeneratorBase
	''' <summary>
	'''     Initializes a new instance of the TorusMesh class.
	''' </summary>
	Public Sub New()
		PropertyChanged(New DependencyPropertyChangedEventArgs())
	End Sub

	''' <summary>
	'''     Identifies the Radius dependency property.
	''' </summary>
	''' <value>
	'''     The radius of the torus in world units.
	'''     The default is 1. 
	'''     The Radius property can be set to a negative value,
	'''     but in effect the torus is turned inside out so that
	'''     the surface of the torus is colored with the BackMaterial
	'''     brush rather than the Material brush.
	''' </value>
	Public Shared ReadOnly RadiusProperty As DependencyProperty = DependencyProperty.Register("Radius", GetType(Double), GetType(TorusMesh), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the radius of the torus.
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
	'''     Identifies the TubeRadius dependency property.
	''' </summary>
	''' <value>
	'''     The identifier for the TubeRadius dependency property.
	''' </value>
	Public Shared ReadOnly TubeRadiusProperty As DependencyProperty = DependencyProperty.Register("TubeRadius", GetType(Double), GetType(TorusMesh), New PropertyMetadata(0.25, AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the tube radius of the torus.
	''' </summary>
	''' <value>
	'''     The radius of the tube that makes up the torus.
	'''     The default is 0.25.
	''' </value>
	Public Property TubeRadius() As Double
		Get
			Return CDbl(GetValue(TubeRadiusProperty))
		End Get
		Set
			SetValue(TubeRadiusProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Slices dependency property.
	''' </summary>
	Public Shared ReadOnly SlicesProperty As DependencyProperty = DependencyProperty.Register("Slices", GetType(Integer), GetType(TorusMesh), New PropertyMetadata(18, AddressOf PropertyChanged), AddressOf ValidateSlices)

	' Validation callback for Slices.
	Private Shared Function ValidateSlices(obj As Object) As Boolean
		Return CInt(obj) > 2
	End Function

	''' <summary>
	'''     Gets or sets the number of divisions around the torus tube.
	''' </summary>
	''' <value>
	'''     The number of divisions around the torus tube. 
	'''     This property must be at least 3. 
	'''     The default value is 18.
	''' </value>
	''' <remarks>
	'''     Each slice is equivalent to a number
	'''     of degrees around the torus tube equal to 360 divided by the 
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
	Public Shared ReadOnly StacksProperty As DependencyProperty = DependencyProperty.Register("Stacks", GetType(Integer), GetType(TorusMesh), New PropertyMetadata(36, AddressOf PropertyChanged), AddressOf ValidateStacks)

	' Validation callback for Stacks.
	Private Shared Function ValidateStacks(obj As Object) As Boolean
		Return CInt(obj) > 2
	End Function

	''' <summary>
	'''     Gets or sets the number of divisions around the
	'''     entire torus.
	''' </summary>
	''' <value>
	'''     This property must be at least 3. 
	'''     The default value is 36.
	''' </value>
	''' <remarks>
	''' </remarks>
	Public Property Stacks() As Integer
		Get
			Return CInt(GetValue(StacksProperty))
		End Get
		Set
			SetValue(StacksProperty, value)
		End Set
	End Property
	'
'        // Static method called for any property change.
'        static void PropertyChanged(DependencyObject obj,
'                                    DependencyPropertyChangedEventArgs args)
'        {
'            (obj as TorusMesh).PropertyChanged(args);
'        }
' 

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
			Dim phi As Double = stack * 2 * Math.PI / Stacks

			Dim xCenter As Double = Radius * Math.Sin(phi)
			Dim yCenter As Double = Radius * Math.Cos(phi)
			Dim pointCenter As New Point3D(xCenter, yCenter, 0)

			For slice As Integer = 0 To Slices
				Dim theta As Double = slice * 2 * Math.PI / Slices + Math.PI
				Dim x As Double = (Radius + TubeRadius * Math.Cos(theta)) * Math.Sin(phi)
				Dim y As Double = (Radius + TubeRadius * Math.Cos(theta)) * Math.Cos(phi)
				Dim z As Double = -TubeRadius * Math.Sin(theta)
				Dim point As New Point3D(x, y, z)

				vertices.Add(point)
				normals.Add(point - pointCenter)
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
	End Sub

	''' <summary>
	'''     Creates a new instance of the TorusMesh class.
	''' </summary>
	''' <returns>
	'''     A new instance of TorusMesh.
	''' </returns>
	''' <remarks>
	'''     Overriding this method is required when deriving 
	'''     from the Freezable class.
	''' </remarks>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New TorusMesh()
	End Function
End Class

