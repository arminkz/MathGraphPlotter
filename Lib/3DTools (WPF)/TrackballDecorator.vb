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
' The following article discusses the mechanics behind this
' trackball implementation: http://viewport3d.com/trackball.htm
'
' Reading the article is not required to use this sample code,
' but skimming it might be useful.
'
'---------------------------------------------------------------------------

Imports System.Collections
Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Shapes
Imports System.Windows.Input
Imports System.Windows.Markup
' IAddChild, ContentPropertyAttribute
Public Class TrackballDecorator
	Inherits Viewport3DDecorator
	Public Sub New()
		' the transform that will be applied to the viewport 3d's camera
		_transform = New Transform3DGroup()
		_transform.Children.Add(_scale)
		_transform.Children.Add(New RotateTransform3D(_rotation))

		' used so that we always get events while activity occurs within
		' the viewport3D
		_eventSource = New Border()
		_eventSource.Background = Brushes.Transparent

		PreViewportChildren.Add(_eventSource)
	End Sub

	''' <summary>
	'''     A transform to move the camera or scene to the trackball's
	'''     current orientation and scale.
	''' </summary>
	Public ReadOnly Property Transform() As Transform3D
		Get
			Return _transform
		End Get
	End Property

	#Region "Event Handling"

	Protected Overrides Sub OnMouseDown(e As MouseButtonEventArgs)
		MyBase.OnMouseDown(e)

		_previousPosition2D = e.GetPosition(Me)
		_previousPosition3D = ProjectToTrackball(ActualWidth, ActualHeight, _previousPosition2D)
		If Mouse.Captured Is Nothing Then
			Mouse.Capture(Me, CaptureMode.Element)
		End If
	End Sub

	Protected Overrides Sub OnMouseUp(e As MouseButtonEventArgs)
		MyBase.OnMouseUp(e)

		If IsMouseCaptured Then
			Mouse.Capture(Me, CaptureMode.None)
		End If
	End Sub

	Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
		MyBase.OnMouseMove(e)

		If IsMouseCaptured Then
			Dim currentPosition As Point = e.GetPosition(Me)

			' avoid any zero axis conditions
			If currentPosition = _previousPosition2D Then
				Return
			End If

			' Prefer tracking to zooming if both buttons are pressed.
			If e.LeftButton = MouseButtonState.Pressed Then
				Track(currentPosition)
			ElseIf e.RightButton = MouseButtonState.Pressed Then
				Zoom(currentPosition)
			End If

			_previousPosition2D = currentPosition

			Dim viewport3D As Viewport3D = Me.Viewport3D
			If viewport3D IsNot Nothing Then
				If viewport3D.Camera IsNot Nothing Then
					If viewport3D.Camera.IsFrozen Then
						viewport3D.Camera = viewport3D.Camera.Clone()
					End If

					If viewport3D.Camera.Transform IsNot _transform Then
						viewport3D.Camera.Transform = _transform
					End If
				End If
			End If
		End If
	End Sub

	#End Region

	Private Sub Track(currentPosition As Point)
		Dim currentPosition3D As Vector3D = ProjectToTrackball(ActualWidth, ActualHeight, currentPosition)

		Dim axis As Vector3D = Vector3D.CrossProduct(_previousPosition3D, currentPosition3D)
		Dim angle As Double = Vector3D.AngleBetween(_previousPosition3D, currentPosition3D)

		' quaterion will throw if this happens - sometimes we can get 3D positions that
		' are very similar, so we avoid the throw by doing this check and just ignoring
		' the event 
		If axis.Length = 0 Then
			Return
		End If

		Dim delta As New Quaternion(axis, -angle)

		' Get the current orientantion from the RotateTransform3D
		Dim r As AxisAngleRotation3D = _rotation
		Dim q As New Quaternion(_rotation.Axis, _rotation.Angle)

		' Compose the delta with the previous orientation
		q *= delta

		' Write the new orientation back to the Rotation3D
		_rotation.Axis = q.Axis
		_rotation.Angle = q.Angle

		_previousPosition3D = currentPosition3D
	End Sub

	Private Function ProjectToTrackball(width As Double, height As Double, point As Point) As Vector3D
		Dim x As Double = point.X / (width / 2)
		' Scale so bounds map to [0,0] - [2,2]
		Dim y As Double = point.Y / (height / 2)

		x = x - 1
		' Translate 0,0 to the center
		y = 1 - y
		' Flip so +Y is up instead of down
		Dim z2 As Double = 1 - x * x - y * y
		' z^2 = 1 - x^2 - y^2
		Dim z As Double = If(z2 > 0, Math.Sqrt(z2), 0)

		Return New Vector3D(x, y, z)
	End Function

	Private Sub Zoom(currentPosition As Point)
		Dim yDelta As Double = currentPosition.Y - _previousPosition2D.Y

		Dim scale As Double = Math.Exp(yDelta / 100)
		' e^(yDelta/100) is fairly arbitrary.
		_scale.ScaleX *= scale
		_scale.ScaleY *= scale
		_scale.ScaleZ *= scale
	End Sub

	'--------------------------------------------------------------------
	'
	' Private data
	'
	'--------------------------------------------------------------------

	Private _previousPosition2D As Point
	Private _previousPosition3D As New Vector3D(0, 0, 1)

	Private _transform As Transform3DGroup
	Private _scale As New ScaleTransform3D()
	Private _rotation As New AxisAngleRotation3D()

	Private _eventSource As Border
End Class
