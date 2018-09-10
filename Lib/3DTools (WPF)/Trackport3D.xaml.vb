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
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Media3D
Imports System.Windows.Input
Imports System.Windows.Markup

''' <summary>
'''     Trackport3D loads a Model3D from a xaml file and displays it.  The user
'''     may rotate the view by dragging the mouse with the left mouse button.
'''     Dragging with the right mouse button will zoom in and out.
''' 
'''     Trackport3D is primarily an example of how to use the Trackball utility
'''     class, but it may be used as a custom control in your own applications.
''' </summary>
Public Partial Class Trackport3D
	Inherits UserControl
	Private _trackball As New Trackball()
	Private ReadOnly Wireframe As New ScreenSpaceLines3D()

	Public Sub New()
		InitializeComponent()

		Me.Viewport.Children.Add(Wireframe)
		Me.Camera.Transform = _trackball.Transform
		Me.Headlight.Transform = _trackball.Transform
	End Sub

	''' <summary>
	'''     Loads and displays the given Xaml file.  Expects the root of
	'''     the Xaml file to be a Model3D.
	''' </summary>
	Public Sub LoadModel(fileStream As System.IO.Stream)
		_model = DirectCast(XamlReader.Load(fileStream), Model3D)

		SetupScene()
	End Sub

	Public Property HeadlightColor() As Color
		Get
			Return Me.Headlight.Color
		End Get
		Set
			Me.Headlight.Color = value
		End Set
	End Property

	Public Property AmbientLightColor() As Color
		Get
			Return Me.AmbientLight.Color
		End Get
		Set
			Me.AmbientLight.Color = value
		End Set
	End Property

	Public Property ViewMode() As ViewMode
		Get
			Return _viewMode
		End Get
		Set
			_viewMode = value
			SetupScene()
		End Set
	End Property

	Private Sub SetupScene()
		Select Case ViewMode
			Case ViewMode.Solid
				Me.Root.Content = _model
				Me.Wireframe.Points.Clear()
				Exit Select

			Case ViewMode.Wireframe
				Me.Root.Content = Nothing
				Me.Wireframe.MakeWireframe(_model)
				Exit Select
		End Select
	End Sub

	Private Sub OnLoaded(sender As Object, e As RoutedEventArgs)
		' Viewport3Ds only raise events when the mouse is over the rendered 3D geometry.
		' In order to capture events whenever the mouse is over the client are we use a
		' same sized transparent Border positioned on top of the Viewport3D.
		_trackball.EventSource = CaptureBorder
	End Sub

	Private _viewMode As ViewMode
	Private _model As Model3D
End Class
