'----------------------------------------------------
' CylindricalMeshBase.cs (c) 2007 by Charles Petzold
'----------------------------------------------------
Imports System.Windows
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
'''     Defines properties for cylindrical mesh geometries.
''' </summary>
Public MustInherit Class CylindricalMeshBase
	Inherits MeshGeneratorBase
	''' <summary>
	'''     Identifies the Length dependency property.
	''' </summary>
	''' <value>
	'''     The identifier for the Length dependency property.
	''' </value>
    Public Shared ReadOnly LengthProperty As DependencyProperty = DependencyProperty.Register("Length", GetType(Double), GetType(CylindricalMeshBase), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the length of the cylinder.
	''' </summary>
	''' <value>
	'''     The length of the cylinder is an amount 
	'''     on the positive Y axis. The default is 1.
	''' </value>
	Public Property Length() As Double
		Get
			Return CDbl(GetValue(LengthProperty))
		End Get
		Set
			SetValue(LengthProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Radius dependency property.
	''' </summary>
	''' <value>
	'''     The radius of the cylinder in world units.
	'''     The default is 1. 
	'''     The Radius property can be set to a negative value,
	'''     but in effect the cylinder is turned inside out so that
	'''     the surface of the cylinder is colored with the BackMaterial
	'''     brush rather than the Material brush.
	''' </value>
	Public Shared ReadOnly RadiusProperty As DependencyProperty = DependencyProperty.Register("Radius", GetType(Double), GetType(CylindricalMeshBase), New PropertyMetadata(1.0, AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the radius of the hollow cylinder.
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
	Public Shared ReadOnly SlicesProperty As DependencyProperty = DependencyProperty.Register("Slices", GetType(Integer), GetType(CylindricalMeshBase), New PropertyMetadata(36, AddressOf PropertyChanged), AddressOf ValidateSlices)

	' Validation callback for Slices.
	Private Shared Function ValidateSlices(obj As Object) As Boolean
		Return CInt(obj) > 2
	End Function

	''' <summary>
	'''     Gets or sets the number of divisions around the Y axis
	'''     used to approximate the cylinder.
	''' </summary>
	''' <value>
	'''     The number of divisions around the Y axis. 
	'''     This property must be at least 3. 
	'''     The default value is 36.
	''' </value>
	''' <remarks>
	'''     The Slices property approximates the curvature of the cylinder.
	'''     The number of degrees of each slice is equivalent to 
	'''     360 divided by the Slices value.
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
	Public Shared ReadOnly StacksProperty As DependencyProperty = DependencyProperty.Register("Stacks", GetType(Integer), GetType(CylindricalMeshBase), New PropertyMetadata(1, AddressOf PropertyChanged), AddressOf ValidateStacks)

	' Validation callback for Stacks.
	Private Shared Function ValidateStacks(obj As Object) As Boolean
		Return CInt(obj) > 0
	End Function

	''' <summary>
	'''     Gets or sets the number of divisions parallel to the XZ
	'''     plane used to build the cylinder.
	''' </summary>
	''' <value>
	'''     The number of divisions parallel to the XZ plane. 
	'''     This property must be at least 1, which is also the 
	'''     default value. 
	''' </value>
	''' <remarks>
	'''     The default value of 1 is appropriate in many cases. 
	'''     However, if PointLight or SpotLight objects are applied to the
	'''     cylinder, or if non-linear transforms are used to deform
	'''     the figure, you should set Stacks to a higher value.
	''' </remarks>
	Public Property Stacks() As Integer
		Get
			Return CInt(GetValue(StacksProperty))
		End Get
		Set
			SetValue(StacksProperty, value)
		End Set
	End Property
End Class

