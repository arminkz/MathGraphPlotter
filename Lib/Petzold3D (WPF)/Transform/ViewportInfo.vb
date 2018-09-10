'---------------------------------------------
' ViewportInfo.cs (c) 2007 by Charles Petzold
'---------------------------------------------
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D

Public Class ViewportInfo
	Inherits Animatable
	Public Shared ReadOnly ZeroMatrix As New Matrix3D(0, 0, 0, 0, 0, 0, _
		0, 0, 0, 0, 0, 0, _
		0, 0, 0, 0)

    Public Shared ReadOnly Viewport3DProperty As DependencyProperty = DependencyProperty.Register("Viewport3D", GetType(Viewport3D), GetType(ViewportInfo), New PropertyMetadata(Nothing, AddressOf Viewport3DChanged))

	Public Property Viewport3D() As Viewport3D
		Get
			Return DirectCast(GetValue(Viewport3DProperty), Viewport3D)
		End Get
		Set
			SetValue(Viewport3DProperty, value)
		End Set
	End Property

	Shared ReadOnly TransformKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly("Transform", GetType(Matrix3D), GetType(ViewportInfo), New PropertyMetadata(New Matrix3D()))

	Public Shared ReadOnly TransformProperty As DependencyProperty = TransformKey.DependencyProperty

	Public Property Transform() As Matrix3D
		Get
			Return CType(GetValue(TransformProperty), Matrix3D)
		End Get
		Protected Set
			SetValue(TransformKey, value)
		End Set
	End Property


	' Properties: Total Transform, Camera Transform, Viewport Transform.


	Private Shared Sub Viewport3DChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		TryCast(obj, ViewportInfo).Viewport3DChanged(args)
	End Sub

	Private Sub Viewport3DChanged(args As DependencyPropertyChangedEventArgs)
		If Viewport3D Is Nothing Then
			Transform = ZeroMatrix
		Else

			Transform = CameraInfo.GetTotalTransform(Viewport3D.Camera, Viewport3D.ActualWidth / Viewport3D.ActualHeight)
		End If
	End Sub




	Public Shared Function GetTotalTransform(vis As Viewport3DVisual) As Matrix3D
		Dim matx As Matrix3D = GetCameraTransform(vis)
		matx.Append(GetViewportTransform(vis))
		Return matx
	End Function

	Public Shared Function GetTotalTransform(viewport As Viewport3D) As Matrix3D
		Dim matx As Matrix3D = GetCameraTransform(viewport)
		matx.Append(GetViewportTransform(viewport))
		Return matx
	End Function

	Public Shared Function GetCameraTransform(vis As Viewport3DVisual) As Matrix3D
		Return CameraInfo.GetTotalTransform(vis.Camera, vis.Viewport.Size.Width / vis.Viewport.Size.Height)
	End Function

	Public Shared Function GetCameraTransform(viewport As Viewport3D) As Matrix3D
		Return CameraInfo.GetTotalTransform(viewport.Camera, viewport.ActualWidth / viewport.ActualHeight)
	End Function

	Public Shared Function GetViewportTransform(vis As Viewport3DVisual) As Matrix3D
		Return New Matrix3D(vis.Viewport.Width / 2, 0, 0, 0, 0, -vis.Viewport.Height / 2, _
			0, 0, 0, 0, 1, 0, _
			vis.Viewport.X + vis.Viewport.Width / 2, vis.Viewport.Y + vis.Viewport.Height / 2, 0, 1)

	End Function

	Public Shared Function GetViewportTransform(viewport As Viewport3D) As Matrix3D
		Return New Matrix3D(viewport.ActualWidth / 2, 0, 0, 0, 0, -viewport.ActualHeight / 2, _
			0, 0, 0, 0, 1, 0, _
			viewport.ActualWidth / 2, viewport.ActualHeight / 2, 0, 1)
	End Function




	Public Shared Function Point3DtoPoint2D(viewport As Viewport3D, point As Point3D) As Point
		Dim matx As Matrix3D = GetTotalTransform(viewport)
		Dim pointTransformed As Point3D = matx.Transform(point)
		Dim pt As New Point(pointTransformed.X, pointTransformed.Y)
		Return pt
	End Function

	Public Shared Function Point2DtoPoint3D(viewport As Viewport3D, ptIn As Point, ByRef range As LineRange) As Boolean
		range = New LineRange()

		Dim pointIn As New Point3D(ptIn.X, ptIn.Y, 0)
		Dim matxViewport As Matrix3D = GetViewportTransform(viewport)
		Dim matxCamera As Matrix3D = GetCameraTransform(viewport)

		If Not matxViewport.HasInverse Then
			Return False
		End If

		If Not matxCamera.HasInverse Then
			Return False
		End If

		matxViewport.Invert()
		matxCamera.Invert()

		Dim pointNormalized As Point3D = matxViewport.Transform(pointIn)
		pointNormalized.Z = 0.01
		Dim pointNear As Point3D = matxCamera.Transform(pointNormalized)
		pointNormalized.Z = 0.99
		Dim pointFar As Point3D = matxCamera.Transform(pointNormalized)

		range = New LineRange(pointNear, pointFar)

		Return True
	End Function



	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New ViewportInfo()
	End Function
End Class
