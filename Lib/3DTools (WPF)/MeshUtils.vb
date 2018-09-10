'---------------------------------------------------------------------------
'
' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Limited Permissive License.
' See http://www.microsoft.com/resources/sharedsource/licensingbasics/limitedpermissivelicense.mspx
' All other rights reserved.
'
' This file is part of the 3D Tools for Windows Presentation Foundation
' project.  For more information, see:
' 
' http://CodePlex.com/Wiki/View.aspx?ProjectName=3DTools
'
'---------------------------------------------------------------------------

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Data
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

#Region "Mesh IValueConverters"

''' <summary>
'''     Abstract base class for all type converters that have a MeshGeometry3D source
''' </summary>
Public MustInherit Class MeshConverter(Of TargetType)
	Implements IValueConverter
	Public Sub New()
	End Sub

	''' <summary>
	'''     IValueConverter.Convert
	''' </summary>
	''' <param name="value">The binding source (should be a MeshGeometry3D)</param>
	''' <param name="targetType">The binding target</param>
	''' <param name="parameter">Optionaly parameter to the converter</param>
	''' <param name="culture">(ignored)</param>
	''' <returns>The converted value</returns>
	Private Function IValueConverter_Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
		If value Is Nothing Then
			Return DependencyProperty.UnsetValue
		End If

		If targetType IsNot GetType(TargetType) Then
			Throw New ArgumentException([String].Format("MeshConverter must target a {0}", GetType(TargetType).Name))
		End If

		Dim mesh As MeshGeometry3D = TryCast(value, MeshGeometry3D)
		If mesh Is Nothing Then
			Throw New ArgumentException("MeshConverter can only convert from a MeshGeometry3D")
		End If

		Return Convert(mesh, parameter)
	End Function

	''' <summary>
	'''     IValueConverter.ConvertBack
	''' 
	'''     Not implemented
	''' </summary>
	Private Function IValueConverter_ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

	''' <summary>
	'''     Subclasses should override this to do conversion
	''' </summary>
	''' <param name="mesh">The mesh source</param>
	''' <param name="parameter">Optional converter argument</param>
	''' <returns>The converted value</returns>
	Public MustOverride Function Convert(mesh As MeshGeometry3D, parameter As Object) As Object
End Class

''' <summary>
''' MeshTextureCoordinateConverter
''' 
''' A MeshConverter that returns a PointCollection and takes an optional direction argument
''' </summary>
Public MustInherit Class MeshTextureCoordinateConverter
	Inherits MeshConverter(Of PointCollection)
	Public Sub New()
	End Sub

	''' <summary>
	'''     
	''' </summary>
	''' <param name="mesh">The source mesh</param>
	''' <param name="parameter">The optional parameter</param>
	''' <returns>The converted value</returns>
	Public Overrides Function Convert(mesh As MeshGeometry3D, parameter As Object) As Object
		Dim paramAsString As String = TryCast(parameter, String)
		If parameter IsNot Nothing AndAlso paramAsString Is Nothing Then
			Throw New ArgumentException("Parameter must be a string.")
		End If

		' Default to the positive Y axis
		Dim dir As Vector3D = MathUtils.YAxis

		If paramAsString IsNot Nothing Then
			dir = Vector3D.Parse(paramAsString)
			MathUtils.TryNormalize(dir)
		End If

		Return Convert(mesh, dir)
	End Function

	''' <summary>
	'''     Subclasses should override this to do conversion
	''' </summary>
	''' <param name="mesh">The source mesh</param>
	''' <param name="dir">The normalized direction parameter</param>
	''' <returns></returns>
	Public MustOverride Overloads Function Convert(mesh As MeshGeometry3D, dir As Vector3D) As Object
End Class

''' <summary>
'''     IValueConverter that generates texture coordinates for a plane.
''' </summary>
Public Class PlanarTextureCoordinateGenerator
	Inherits MeshTextureCoordinateConverter
	Public Sub New()
	End Sub

	Public Overrides Function Convert(mesh As MeshGeometry3D, dir As Vector3D) As Object
		Return MeshUtils.GeneratePlanarTextureCoordinates(mesh, dir)
	End Function
End Class

''' <summary>
'''     IValueConverter that generates texture coordinates for a sphere.
''' </summary>
Public Class SphericalTextureCoordinateGenerator
	Inherits MeshTextureCoordinateConverter
	Public Sub New()
	End Sub

	Public Overrides Function Convert(mesh As MeshGeometry3D, dir As Vector3D) As Object
		Return MeshUtils.GenerateSphericalTextureCoordinates(mesh, dir)
	End Function
End Class

''' <summary>
'''     IValueConverter that generates texture coordinates for a cylinder
''' </summary>
Public Class CylindricalTextureCoordinateGenerator
	Inherits MeshTextureCoordinateConverter
	Public Sub New()
	End Sub

	Public Overrides Function Convert(mesh As MeshGeometry3D, dir As Vector3D) As Object
		Return MeshUtils.GenerateCylindricalTextureCoordinates(mesh, dir)
	End Function
End Class

#End Region

Public NotInheritable Class MeshUtils
	Private Sub New()
	End Sub
	#Region "Texture Coordinate Generation Methods"

	''' <summary>
	'''     Generates texture coordinates as if mesh were a cylinder.
	''' 
	'''     Notes: 
	'''         1) v is flipped for you automatically
	'''         2) 'mesh' is not modified. If you want the generated coordinates
	'''            to be assigned to mesh, do:
	''' 
	'''            mesh.TextureCoordinates = GenerateCylindricalTextureCoordinates(mesh, foo)
	''' 
	''' </summary>
	''' <param name="mesh">The mesh</param>
	''' <param name="dir">The axis of rotation for the cylinder</param>
	''' <returns>The generated texture coordinates</returns>
	Public Shared Function GenerateCylindricalTextureCoordinates(mesh As MeshGeometry3D, dir As Vector3D) As PointCollection
		If mesh Is Nothing Then
			Return Nothing
		End If

		Dim bounds As Rect3D = mesh.Bounds
		Dim count As Integer = mesh.Positions.Count
		Dim texcoords As New PointCollection(count)
		Dim positions As IEnumerable(Of Point3D) = TransformPoints(bounds, mesh.Positions, dir)

		For Each vertex As Point3D In positions
			texcoords.Add(New Point(GetUnitCircleCoordinate(-vertex.Z, vertex.X), 1.0 - GetPlanarCoordinate(vertex.Y, bounds.Y, bounds.SizeY)))
		Next

		Return texcoords
	End Function

	''' <summary>
	'''     Generates texture coordinates as if mesh were a sphere.
	''' 
	'''     Notes: 
	'''         1) v is flipped for you automatically
	'''         2) 'mesh' is not modified. If you want the generated coordinates
	'''            to be assigned to mesh, do:
	''' 
	'''            mesh.TextureCoordinates = GenerateSphericalTextureCoordinates(mesh, foo)
	''' 
	''' </summary>
	''' <param name="mesh">The mesh</param>
	''' <param name="dir">The axis of rotation for the sphere</param>
	''' <returns>The generated texture coordinates</returns>
	Public Shared Function GenerateSphericalTextureCoordinates(mesh As MeshGeometry3D, dir As Vector3D) As PointCollection
		If mesh Is Nothing Then
			Return Nothing
		End If

		Dim bounds As Rect3D = mesh.Bounds
		Dim count As Integer = mesh.Positions.Count
		Dim texcoords As New PointCollection(count)
		Dim positions As IEnumerable(Of Point3D) = TransformPoints(bounds, mesh.Positions, dir)

		For Each vertex As Point3D In positions
			' Don't need to do 'vertex - center' since TransformPoints put us
			' at the origin
			Dim radius As New Vector3D(vertex.X, vertex.Y, vertex.Z)
			MathUtils.TryNormalize(radius)

			texcoords.Add(New Point(GetUnitCircleCoordinate(-radius.Z, radius.X), 1.0 - (Math.Asin(radius.Y) / Math.PI + 0.5)))
		Next

		Return texcoords
	End Function

	''' <summary>
	'''     Generates texture coordinates as if mesh were a plane.
	''' 
	'''     Notes: 
	'''         1) v is flipped for you automatically
	'''         2) 'mesh' is not modified. If you want the generated coordinates
	'''            to be assigned to mesh, do:
	''' 
	'''            mesh.TextureCoordinates = GeneratePlanarTextureCoordinates(mesh, foo)
	''' 
	''' </summary>
	''' <param name="mesh">The mesh</param>
	''' <param name="dir">The normal of the plane</param>
	''' <returns>The generated texture coordinates</returns>
	Public Shared Function GeneratePlanarTextureCoordinates(mesh As MeshGeometry3D, dir As Vector3D) As PointCollection
		If mesh Is Nothing Then
			Return Nothing
		End If

		Dim bounds As Rect3D = mesh.Bounds
		Dim count As Integer = mesh.Positions.Count
		Dim texcoords As New PointCollection(count)
		Dim positions As IEnumerable(Of Point3D) = TransformPoints(bounds, mesh.Positions, dir)

		For Each vertex As Point3D In positions
			' The plane is looking along positive Y, so Z is really Y

			texcoords.Add(New Point(GetPlanarCoordinate(vertex.X, bounds.X, bounds.SizeX), GetPlanarCoordinate(vertex.Z, bounds.Z, bounds.SizeZ)))
		Next

		Return texcoords
	End Function

	#End Region

	Friend Shared Function GetPlanarCoordinate([end] As Double, start As Double, width As Double) As Double
		Return ([end] - start) / width
	End Function

	Friend Shared Function GetUnitCircleCoordinate(y As Double, x As Double) As Double
		Return Math.Atan2(y, x) / (2.0 * Math.PI) + 0.5
	End Function

	''' <summary>
	'''     Finds the transform from 'dir' to '<0, 1, 0>' and transforms 'bounds'
	'''     and 'points' by it.
	''' </summary>
	''' <param name="bounds">The bounds to transform</param>
	''' <param name="points">The vertices to transform</param>
	''' <param name="dir">The orientation of the mesh</param>
	''' <returns>
	'''     The transformed points. If 'dir' is already '<0, 1, 0>' then this
	'''     will equal 'points.'
	''' </returns>
	Friend Shared Function TransformPoints(ByRef bounds As Rect3D, points As Point3DCollection, ByRef dir As Vector3D) As IEnumerable(Of Point3D)
		If dir = MathUtils.YAxis Then
			Return points
		End If

		Dim rotAxis As Vector3D = Vector3D.CrossProduct(dir, MathUtils.YAxis)
		Dim rotAngle As Double = Vector3D.AngleBetween(dir, MathUtils.YAxis)
		Dim q As Quaternion

		If rotAxis.X <> 0 OrElse rotAxis.Y <> 0 OrElse rotAxis.Z <> 0 Then
			Debug.Assert(rotAngle <> 0)

			q = New Quaternion(rotAxis, rotAngle)
		Else
			Debug.Assert(dir = -MathUtils.YAxis)

			q = New Quaternion(MathUtils.XAxis, rotAngle)
		End If

		Dim center As New Vector3D(bounds.X + bounds.SizeX / 2, bounds.Y + bounds.SizeY / 2, bounds.Z + bounds.SizeZ / 2)

		Dim t As Matrix3D = Matrix3D.Identity
		t.Translate(-center)
		t.Rotate(q)

		Dim count As Integer = points.Count
		Dim transformedPoints As Point3D() = New Point3D(count - 1) {}

		For i As Integer = 0 To count - 1
			transformedPoints(i) = t.Transform(points(i))
		Next

		' Finally, transform the bounds too
		bounds = MathUtils.TransformBounds(bounds, t)

		Return transformedPoints
	End Function
End Class
