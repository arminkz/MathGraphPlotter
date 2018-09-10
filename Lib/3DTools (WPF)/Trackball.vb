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

Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Media.Media3D
Imports System.Windows.Markup

''' <summary>
'''     Trackball is a utility class which observes the mouse events
'''     on a specified FrameworkElement and produces a Transform3D
'''     with the resultant rotation and scale.
''' 
'''     Example Usage:
''' 
'''         Trackball trackball = new Trackball();
'''         trackball.EventSource = myElement;
'''         myViewport3D.Camera.Transform = trackball.Transform;
''' 
'''     Because Viewport3Ds only raise events when the mouse is over the
'''     rendered 3D geometry (as opposed to not when the mouse is within
'''     the layout bounds) you usually want to use another element as 
'''     your EventSource.  For example, a transparent border placed on
'''     top of your Viewport3D works well:
'''     
'''         <Grid>
'''           <ColumnDefinition />
'''           <RowDefinition />
'''           <Viewport3D Name="myViewport" ClipToBounds="True" Grid.Row="0" Grid.Column="0" />
'''           <Border Name="myElement" Background="Transparent" Grid.Row="0" Grid.Column="0" />
'''         </Grid>
'''     
'''     NOTE: The Transform property may be shared by multiple Cameras
'''           if you want to have auxilary views following the trackball.
''' 
'''           It can also be useful to share the Transform property with
'''           models in the scene that you want to move with the camera.
'''           (For example, the Trackport3D's headlight is implemented
'''           this way.)
''' 
'''           You may also use a Transform3DGroup to combine the
'''           Transform property with additional Transforms.
''' </summary> 
Public Class Trackball
	Private _eventSource As FrameworkElement
	Private _previousPosition2D As Point
	Private _previousPosition3D As New Vector3D(0, 0, 1)

	Private _transform As Transform3DGroup
	Private _scale As New ScaleTransform3D()
	Private _rotation As New AxisAngleRotation3D()

	Public Sub New()
		_transform = New Transform3DGroup()
		_transform.Children.Add(_scale)
		_transform.Children.Add(New RotateTransform3D(_rotation))
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

	''' <summary>
	'''     The FrameworkElement we listen to for mouse events.
	''' </summary>
	Public Property EventSource() As FrameworkElement
		Get
			Return _eventSource
		End Get

		Set
			If _eventSource IsNot Nothing Then
				RemoveHandler _eventSource.MouseDown, AddressOf Me.OnMouseDown
				RemoveHandler _eventSource.MouseUp, AddressOf Me.OnMouseUp
				RemoveHandler _eventSource.MouseMove, AddressOf Me.OnMouseMove
			End If

			_eventSource = value

			AddHandler _eventSource.MouseDown, AddressOf Me.OnMouseDown
			AddHandler _eventSource.MouseUp, AddressOf Me.OnMouseUp
			AddHandler _eventSource.MouseMove, AddressOf Me.OnMouseMove
		End Set
	End Property

	Private Sub OnMouseDown(sender As Object, e As MouseEventArgs)
		Mouse.Capture(EventSource, CaptureMode.Element)
		_previousPosition2D = e.GetPosition(EventSource)
		_previousPosition3D = ProjectToTrackball(EventSource.ActualWidth, EventSource.ActualHeight, _previousPosition2D)
	End Sub

	Private Sub OnMouseUp(sender As Object, e As MouseEventArgs)
		Mouse.Capture(EventSource, CaptureMode.None)
	End Sub

	Private Sub OnMouseMove(sender As Object, e As MouseEventArgs)
		Dim currentPosition As Point = e.GetPosition(EventSource)

		' Prefer tracking to zooming if both buttons are pressed.
		If e.LeftButton = MouseButtonState.Pressed Then
			Track(currentPosition)
		ElseIf e.RightButton = MouseButtonState.Pressed Then
			Zoom(currentPosition)
		End If

		_previousPosition2D = currentPosition
	End Sub

	#End Region

	Private Sub Track(currentPosition As Point)
		Dim currentPosition3D As Vector3D = ProjectToTrackball(EventSource.ActualWidth, EventSource.ActualHeight, currentPosition)

		Dim axis As Vector3D = Vector3D.CrossProduct(_previousPosition3D, currentPosition3D)
		Dim angle As Double = Vector3D.AngleBetween(_previousPosition3D, currentPosition3D)
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
End Class
