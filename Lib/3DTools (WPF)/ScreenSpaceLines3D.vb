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

Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
'''     ScreenSpaceLines3D are a 3D line primitive whose thickness
'''     is constant in 2D space post projection.
''' 
'''     This means that the lines do not become foreshortened as
'''     they receed from the camera as other 3D primitives do under
'''     a typical perspective projection.
''' 
'''     Example Usage:
''' 
'''     &lt;tools:ScreenSpaceLines3D
'''         Points="0,0,0 0,1,0 0,1,0 1,1,0 1,1,0 0,0,1"
'''         Thickness="5" Color="Red"&gt;
''' 
'''     "Screen space" is a bit of a misnomer as the line thickness
'''     is specified in the 2D coordinate system of the container
'''     Viewport3D, not the screen.
''' </summary>
Public Class ScreenSpaceLines3D
	Inherits ModelVisual3D
	Public Sub New()
		_mesh = New MeshGeometry3D()
		_model = New GeometryModel3D()
		_model.Geometry = _mesh
		SetColor(Me.Color)

		Me.Content = _model
		Me.Points = New Point3DCollection()

		AddHandler CompositionTarget.Rendering, AddressOf OnRender
	End Sub

	Public Shared ReadOnly ColorProperty As DependencyProperty = DependencyProperty.Register("Color", GetType(Color), GetType(ScreenSpaceLines3D), New PropertyMetadata(Colors.White, AddressOf OnColorChanged))

	Private Shared Sub OnColorChanged(sender As DependencyObject, args As DependencyPropertyChangedEventArgs)
		DirectCast(sender, ScreenSpaceLines3D).SetColor(CType(args.NewValue, Color))
	End Sub

	Private Sub SetColor(color As Color)
		Dim unlitMaterial As New MaterialGroup()
		unlitMaterial.Children.Add(New DiffuseMaterial(New SolidColorBrush(Colors.Black)))
		unlitMaterial.Children.Add(New EmissiveMaterial(New SolidColorBrush(color)))
		unlitMaterial.Freeze()

		_model.Material = unlitMaterial
		_model.BackMaterial = unlitMaterial
	End Sub

	Public Property Color() As Color
		Get
			Return CType(GetValue(ColorProperty), Color)
		End Get
		Set
			SetValue(ColorProperty, value)
		End Set
	End Property

	Public Shared ReadOnly ThicknessProperty As DependencyProperty = DependencyProperty.Register("Thickness", GetType(Double), GetType(ScreenSpaceLines3D), New PropertyMetadata(1.0, AddressOf OnThicknessChanged))

	Private Shared Sub OnThicknessChanged(sender As DependencyObject, args As DependencyPropertyChangedEventArgs)
		DirectCast(sender, ScreenSpaceLines3D).GeometryDirty()
	End Sub

	Public Property Thickness() As Double
		Get
			Return CDbl(GetValue(ThicknessProperty))
		End Get
		Set
			SetValue(ThicknessProperty, value)
		End Set
	End Property

	Public Shared ReadOnly PointsProperty As DependencyProperty = DependencyProperty.Register("Points", GetType(Point3DCollection), GetType(ScreenSpaceLines3D), New PropertyMetadata(Nothing, AddressOf OnPointsChanged))

	Private Shared Sub OnPointsChanged(sender As DependencyObject, args As DependencyPropertyChangedEventArgs)
		DirectCast(sender, ScreenSpaceLines3D).GeometryDirty()
	End Sub

	Public Property Points() As Point3DCollection
		Get
			Return DirectCast(GetValue(PointsProperty), Point3DCollection)
		End Get
		Set
			SetValue(PointsProperty, value)
		End Set
	End Property

	Private Sub OnRender(sender As Object, e As EventArgs)
		If Points.Count = 0 AndAlso _mesh.Positions.Count = 0 Then
			Return
		End If

		If UpdateTransforms() Then
			RebuildGeometry()
		End If
	End Sub

	Private Sub GeometryDirty()
		' Force next call to UpdateTransforms() to return true.
		_visualToScreen = MathUtils.ZeroMatrix
	End Sub

	Private Sub RebuildGeometry()
		Dim halfThickness As Double = Thickness / 2.0
		Dim numLines As Integer = Points.Count \ 2

		Dim positions As New Point3DCollection(numLines * 4)

		For i As Integer = 0 To numLines - 1
			Dim startIndex As Integer = i * 2

			Dim startPoint As Point3D = Points(startIndex)
			Dim endPoint As Point3D = Points(startIndex + 1)

			AddSegment(positions, startPoint, endPoint, halfThickness)
		Next

		positions.Freeze()
		_mesh.Positions = positions

		Dim indices As New Int32Collection(Points.Count * 3)

		For i As Integer = 0 To Points.Count \ 2 - 1
			indices.Add(i * 4 + 2)
			indices.Add(i * 4 + 1)
			indices.Add(i * 4 + 0)

			indices.Add(i * 4 + 2)
			indices.Add(i * 4 + 3)
			indices.Add(i * 4 + 1)
		Next

		indices.Freeze()
		_mesh.TriangleIndices = indices
	End Sub

	Private Sub AddSegment(positions As Point3DCollection, startPoint As Point3D, endPoint As Point3D, halfThickness As Double)
		' NOTE: We want the vector below to be perpendicular post projection so
		'       we need to compute the line direction in post-projective space.
		Dim lineDirection As Vector3D = endPoint * _visualToScreen - startPoint * _visualToScreen
		lineDirection.Z = 0
		lineDirection.Normalize()

		' NOTE: Implicit Rot(90) during construction to get a perpendicular vector.
		Dim delta As New Vector(-lineDirection.Y, lineDirection.X)
		delta *= halfThickness

		Dim pOut1 As Point3D, pOut2 As Point3D

		Widen(startPoint, delta, pOut1, pOut2)

		positions.Add(pOut1)
		positions.Add(pOut2)

		Widen(endPoint, delta, pOut1, pOut2)

		positions.Add(pOut1)
		positions.Add(pOut2)
	End Sub

	Private Sub Widen(pIn As Point3D, delta As Vector, ByRef pOut1 As Point3D, ByRef pOut2 As Point3D)
		Dim pIn4 As Point4D = CType(pIn, Point4D)
		Dim pOut41 As Point4D = pIn4 * _visualToScreen
		Dim pOut42 As Point4D = pOut41

		pOut41.X += delta.X * pOut41.W
		pOut41.Y += delta.Y * pOut41.W

		pOut42.X -= delta.X * pOut42.W
		pOut42.Y -= delta.Y * pOut42.W

		pOut41 *= _screenToVisual
		pOut42 *= _screenToVisual

		' NOTE: Z is not modified above, so we use the original Z below.

		pOut1 = New Point3D(pOut41.X / pOut41.W, pOut41.Y / pOut41.W, pOut41.Z / pOut41.W)

		pOut2 = New Point3D(pOut42.X / pOut42.W, pOut42.Y / pOut42.W, pOut42.Z / pOut42.W)
	End Sub

	Private Function UpdateTransforms() As Boolean
		Dim viewport As Viewport3DVisual
		Dim success As Boolean

		Dim visualToScreen As Matrix3D = MathUtils.TryTransformTo2DAncestor(Me, viewport, success)

		If Not success OrElse Not visualToScreen.HasInverse Then
			_mesh.Positions = Nothing
			Return False
		End If

		If visualToScreen = _visualToScreen Then
			Return False
		End If

		_visualToScreen = InlineAssignHelper(_screenToVisual, visualToScreen)
		_screenToVisual.Invert()

		Return True
	End Function

	#Region "MakeWireframe"

	Public Sub MakeWireframe(model As Model3D)
		Me.Points.Clear()

		If model Is Nothing Then
			Return
		End If

		Dim transform As New Matrix3DStack()
		transform.Push(Matrix3D.Identity)

		WireframeHelper(model, transform)
	End Sub

	Private Sub WireframeHelper(model As Model3D, matrixStack As Matrix3DStack)
		Dim transform As Transform3D = model.Transform

		If transform IsNot Nothing AndAlso transform IsNot Transform3D.Identity Then
			matrixStack.Prepend(model.Transform.Value)
		End If

		Try
			Dim group As Model3DGroup = TryCast(model, Model3DGroup)

			If group IsNot Nothing Then
				WireframeHelper(group, matrixStack)
				Return
			End If

			Dim geometry As GeometryModel3D = TryCast(model, GeometryModel3D)

			If geometry IsNot Nothing Then
				WireframeHelper(geometry, matrixStack)
				Return
			End If
		Finally
			If transform IsNot Nothing AndAlso transform IsNot Transform3D.Identity Then
				matrixStack.Pop()
			End If
		End Try
	End Sub

	Private Sub WireframeHelper(group As Model3DGroup, matrixStack As Matrix3DStack)
		For Each child As Model3D In group.Children
			WireframeHelper(child, matrixStack)
		Next
	End Sub

	Private Sub WireframeHelper(model As GeometryModel3D, matrixStack As Matrix3DStack)
		Dim geometry As Geometry3D = model.Geometry
		Dim mesh As MeshGeometry3D = TryCast(geometry, MeshGeometry3D)

		If mesh IsNot Nothing Then
			Dim positions As Point3D() = New Point3D(mesh.Positions.Count - 1) {}
			mesh.Positions.CopyTo(positions, 0)
			matrixStack.Peek().Transform(positions)

			Dim indices As Int32Collection = mesh.TriangleIndices

			If indices.Count > 0 Then
				Dim limit As Integer = positions.Length - 1

				Dim i As Integer = 2, count As Integer = indices.Count
				While i < count
					Dim i0 As Integer = indices(i - 2)
					Dim i1 As Integer = indices(i - 1)
					Dim i2 As Integer = indices(i)

					' WPF halts rendering on the first deformed triangle.  We should
					' do the same.
					If (0 > i0 OrElse i0 > limit) OrElse (0 > i1 OrElse i1 > limit) OrElse (0 > i2 OrElse i2 > limit) Then
						Exit While
					End If

					AddTriangle(positions, i0, i1, i2)
					i += 3
				End While
			Else
				Dim i As Integer = 2, count As Integer = positions.Length
				While i < count
					Dim i0 As Integer = i - 2
					Dim i1 As Integer = i - 1
					Dim i2 As Integer = i

					AddTriangle(positions, i0, i1, i2)
					i += 3
				End While
			End If
		End If
	End Sub

	Private Sub AddTriangle(positions As Point3D(), i0 As Integer, i1 As Integer, i2 As Integer)
		Points.Add(positions(i0))
		Points.Add(positions(i1))
		Points.Add(positions(i1))
		Points.Add(positions(i2))
		Points.Add(positions(i2))
		Points.Add(positions(i0))
	End Sub

	#End Region

	Private _visualToScreen As Matrix3D
	Private _screenToVisual As Matrix3D
	Private ReadOnly _model As GeometryModel3D
	Private ReadOnly _mesh As MeshGeometry3D
	Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
		target = value
		Return value
	End Function
End Class
