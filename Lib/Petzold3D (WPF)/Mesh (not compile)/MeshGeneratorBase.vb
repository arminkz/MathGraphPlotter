'--------------------------------------------------
' MeshGeneratorBase.cs (c) 2007 by Charles Petzold
'--------------------------------------------------
Imports System.Windows
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D

''' <summary>
'''     Abstract base class for classes that generate 
'''     MeshGeometry3D objects.
''' </summary>
<RuntimeNameProperty("Name")> _
Public MustInherit Class MeshGeneratorBase
	Inherits Animatable
	''' <summary>
	'''     Identifies the Name dependency property.
	''' </summary>
	Public Shared ReadOnly NameProperty As DependencyProperty = DependencyProperty.Register("Name", GetType(String), GetType(MeshGeneratorBase))

	''' <summary>
	'''     Gets or sets the identifying name of this mesh
	'''     generator object.
	''' </summary>
	Public Property Name() As String
		Get
			Return DirectCast(GetValue(NameProperty), String)
		End Get
		Set
			SetValue(NameProperty, value)
		End Set
	End Property

	Shared GeometryKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly("Geometry", GetType(MeshGeometry3D), GetType(MeshGeneratorBase), New PropertyMetadata(New MeshGeometry3D()))

	''' <summary>
	'''     Identifies the Geometry dependency property.
	''' </summary>
	Public Shared ReadOnly GeometryProperty As DependencyProperty = GeometryKey.DependencyProperty

	''' <summary>
	'''     Gets or sets the Geometry property.
	''' </summary>
	Public Property Geometry() As MeshGeometry3D
		Get
			Return DirectCast(GetValue(GeometryProperty), MeshGeometry3D)
		End Get
		Protected Set
			SetValue(GeometryKey, value)
		End Set
	End Property

	''' <summary>
	'''     Initializes a new instance of MeshGeneratorBase.
	''' </summary>
	Public Sub New()
		Geometry = New MeshGeometry3D()
	End Sub

	''' <summary>
	''' 
	''' </summary>
	''' <param name="obj"></param>
	''' <param name="args"></param>
	Protected Shared Sub PropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		TryCast(obj, MeshGeneratorBase).PropertyChanged(args)
	End Sub

	''' <summary>
	''' 
	''' </summary>
	''' <param name="args"></param>
	Protected Overridable Sub PropertyChanged(args As DependencyPropertyChangedEventArgs)
		' Get the MeshGeometry3D for local convenience.
		Dim mesh As MeshGeometry3D = Geometry

		' Obtain the four collection of the MeshGeometry3D.
		Dim vertices As Point3DCollection = mesh.Positions
		Dim normals As Vector3DCollection = mesh.Normals
		Dim indices As Int32Collection = mesh.TriangleIndices
		Dim textures As PointCollection = mesh.TextureCoordinates

		' Set the MeshGeometry3D collections to null while updating.
		mesh.Positions = Nothing
		mesh.Normals = Nothing
		mesh.TriangleIndices = Nothing
		mesh.TextureCoordinates = Nothing

		' Call the abstract method to fill the collections.
		Triangulate(args, vertices, normals, indices, textures)

		' Set the updated collections to the MeshGeometry3D.
		mesh.TextureCoordinates = textures
		mesh.TriangleIndices = indices
		mesh.Normals = normals
		mesh.Positions = vertices
	End Sub

	''' <summary>
	''' 
	''' </summary>
	''' <param name="args"></param>
	''' <param name="vertices"></param>
	''' <param name="normals"></param>
	''' <param name="indices"></param>
	''' <param name="textures"></param>
	Protected MustOverride Sub Triangulate(args As DependencyPropertyChangedEventArgs, vertices As Point3DCollection, normals As Vector3DCollection, indices As Int32Collection, textures As PointCollection)
End Class
