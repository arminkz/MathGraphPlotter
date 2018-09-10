


Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public MustInherit Class ModelVisualBase
	Inherits ModelVisual3D
	Private verticesPreTransform As New Point3DCollection()
	Private normalsPreTransform As New Vector3DCollection()
	Private normalsAsPoints As New Point3DCollection()
	Private wireframe As WireFrame

	' TODO: Let's have a WireFrame property so color and thickness can be defined
	' Keep the IsWireFrame property as well

	Public Sub New()
		Geometry = Geometry.Clone()
		Dim model As New GeometryModel3D(Geometry, Nothing)
		Content = model

		AlgorithmicTransforms = New AlgorithmicTransformCollection()
	End Sub

	''' <summary>
	'''     Identifies the Name dependency property.
	''' </summary>
	Public Shared ReadOnly NameProperty As DependencyProperty = DependencyProperty.Register("Name", GetType(String), GetType(ModelVisualBase))

	''' <summary>
	'''     Gets or sets the identifying name of the object. ETC.
	'''     This is a dependency property.
	''' </summary>
	Public Property Name() As String
		Get
			Return DirectCast(GetValue(NameProperty), String)
		End Get
		Set
			SetValue(NameProperty, value)
		End Set
	End Property

	Public Shared ReadOnly MaterialProperty As DependencyProperty = GeometryModel3D.MaterialProperty.AddOwner(GetType(ModelVisualBase), New PropertyMetadata(AddressOf MaterialPropertyChanged))

	Public Property Material() As Material
		Get
			Return DirectCast(GetValue(MaterialProperty), Material)
		End Get
		Set
			SetValue(MaterialProperty, value)
		End Set
	End Property

	Public Shared ReadOnly BackMaterialProperty As DependencyProperty = GeometryModel3D.BackMaterialProperty.AddOwner(GetType(ModelVisualBase), New PropertyMetadata(AddressOf MaterialPropertyChanged))

	Public Property BackMaterial() As Material
		Get
			Return DirectCast(GetValue(BackMaterialProperty), Material)
		End Get
		Set
			SetValue(BackMaterialProperty, value)
		End Set
	End Property

	Private Shared Sub MaterialPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		TryCast(obj, ModelVisualBase).MaterialPropertyChanged(args)
	End Sub

	Private Sub MaterialPropertyChanged(args As DependencyPropertyChangedEventArgs)
		Dim model As GeometryModel3D = TryCast(Content, GeometryModel3D)

		If args.[Property] Is MaterialProperty Then
			model.Material = TryCast(args.NewValue, Material)

		ElseIf args.[Property] Is BackMaterialProperty Then
			model.BackMaterial = TryCast(args.NewValue, Material)
		End If
	End Sub


	Shared ReadOnly GeometryKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly("Geometry", GetType(MeshGeometry3D), GetType(ModelVisualBase), New PropertyMetadata(New MeshGeometry3D()))

	Public Shared ReadOnly GeometryProperty As DependencyProperty = GeometryKey.DependencyProperty

	Public Property Geometry() As MeshGeometry3D
		Get
			Return DirectCast(GetValue(GeometryProperty), MeshGeometry3D)
		End Get
		Protected Set
			SetValue(GeometryKey, value)
		End Set
	End Property

	Public Shared ReadOnly AlgorithmicTransformsProperty As DependencyProperty = DependencyProperty.Register("AlgorithmicTransform", GetType(AlgorithmicTransformCollection), GetType(ModelVisualBase), New PropertyMetadata(Nothing, AddressOf PropertyChanged))

	Public Property AlgorithmicTransforms() As AlgorithmicTransformCollection
		Get
			Return DirectCast(GetValue(AlgorithmicTransformsProperty), AlgorithmicTransformCollection)
		End Get
		Set
			SetValue(AlgorithmicTransformsProperty, value)
		End Set
	End Property

	Public Shared ReadOnly IsWireFrameProperty As DependencyProperty = DependencyProperty.Register("IsWireFrame", GetType(Boolean), GetType(ModelVisualBase), New PropertyMetadata(False, AddressOf PropertyChanged))

	Public Property IsWireFrame() As Boolean
		Get
			Return CBool(GetValue(IsWireFrameProperty))
		End Get
		Set
			SetValue(IsWireFrameProperty, value)
		End Set
	End Property

	Protected Shared Sub PropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		TryCast(obj, ModelVisualBase).PropertyChanged(args)
	End Sub

	Protected Overridable Sub PropertyChanged(args As DependencyPropertyChangedEventArgs)
		Dim vertices As Point3DCollection
		Dim normals As Vector3DCollection
		Dim indices As Int32Collection
		Dim textures As PointCollection

		If Not IsWireFrame OrElse args.[Property] Is IsWireFrameProperty AndAlso Not CBool(args.OldValue) Then
			' Obtain the MeshGeometry3D.
			Dim mesh As MeshGeometry3D = Geometry

			' Get all four collectiona.
			vertices = mesh.Positions
			normals = mesh.Normals
			indices = mesh.TriangleIndices
			textures = mesh.TextureCoordinates

			' Set the MeshGeometry3D collections to null while updating.
			mesh.Positions = Nothing
			mesh.Normals = Nothing
			mesh.TriangleIndices = Nothing
			mesh.TextureCoordinates = Nothing
		Else

			' Get properties from WireFrame object
			vertices = wireframe.Positions
			normals = wireframe.Normals
			indices = wireframe.TriangleIndices
			textures = wireframe.TextureCoordinates

			wireframe.Positions = Nothing
			wireframe.Normals = Nothing
			wireframe.TriangleIndices = Nothing
			wireframe.TextureCoordinates = Nothing
		End If

		' If args.Property not IsWireFrame OR Algorithmic transforms
		If args.[Property] IsNot AlgorithmicTransformsProperty AndAlso args.[Property] IsNot IsWireFrameProperty Then
			' Call the abstract method to fill the collections.
			Triangulate(args, vertices, normals, indices, textures)

			' Transfer vertices and normals to internal collections.
			verticesPreTransform.Clear()
			normalsPreTransform.Clear()

			For Each vertex As Point3D In vertices
				verticesPreTransform.Add(vertex)
			Next

			For Each normal As Vector3D In normals
				normalsPreTransform.Add(normal)
			Next
		End If

		If args.[Property] Is AlgorithmicTransformsProperty Then
			vertices.Clear()
			normals.Clear()
			normalsAsPoints.Clear()

			' Transfer saved vertices and normals.
			For Each vertex As Point3D In verticesPreTransform
				vertices.Add(vertex)
			Next

			For Each normal As Vector3D In normalsPreTransform
				normalsAsPoints.Add(CType(normal, Point3D))
			Next
		End If

		If args.[Property] IsNot IsWireFrameProperty Then
			For Each xform As AlgorithmicTransform In AlgorithmicTransforms
				xform.Transform(vertices)
				xform.Transform(normalsAsPoints)
			Next

			For Each point As Point3D In normalsAsPoints
				normals.Add(CType(point, Vector3D))
			Next
		End If

		If IsWireFrame Then
			' Set stuff to WireFrame object, and create it if necessary
			If wireframe Is Nothing Then
				wireframe = New WireFrame()
					' do we want to remove it when it's no longer used?
				Children.Add(wireframe)
			End If

			wireframe.TextureCoordinates = textures
			wireframe.TriangleIndices = indices
			wireframe.Normals = normals
			wireframe.Positions = vertices
		Else
			' Obtain the MeshGeometry3D.
			Dim mesh As MeshGeometry3D = Geometry

			' Set the updated collections to the MeshGeometry3D.
			mesh.TextureCoordinates = textures
			mesh.TriangleIndices = indices
			mesh.Normals = normals
			mesh.Positions = vertices
		End If
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
