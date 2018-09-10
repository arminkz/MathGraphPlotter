Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D

' Derive from TransformInfo so can define ZeroMatrix and Name properties ????

Public Class VisualInfo
	Inherits Animatable
	Public Shared ReadOnly ZeroMatrix As New Matrix3D(0, 0, 0, 0, 0, 0, _
		0, 0, 0, 0, 0, 0, _
		0, 0, 0, 0)


    Public Shared ReadOnly ModelVisual3DProperty As DependencyProperty = DependencyProperty.Register("ModelVisual3D", GetType(ModelVisual3D), GetType(VisualInfo), New PropertyMetadata(Nothing, AddressOf ModelVisual3DChanged))

	Public Property ModelVisual3D() As ModelVisual3D
		Get
			Return DirectCast(GetValue(ModelVisual3DProperty), ModelVisual3D)
		End Get
		Set
			SetValue(ModelVisual3DProperty, value)
		End Set
	End Property

	Shared ReadOnly TotalTransformKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly("TotalTransform", GetType(Matrix3D), GetType(VisualInfo), New PropertyMetadata(New Matrix3D()))

	Public Shared ReadOnly TotalTransformProperty As DependencyProperty = TotalTransformKey.DependencyProperty

	Public Property TotalTransform() As Matrix3D
		Get
			Return CType(GetValue(TotalTransformProperty), Matrix3D)
		End Get
		Protected Set
			SetValue(TotalTransformKey, value)
		End Set
	End Property


	Private Shared Sub ModelVisual3DChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		TryCast(obj, VisualInfo).ModelVisual3DChanged(args)
	End Sub

	Private Sub ModelVisual3DChanged(args As DependencyPropertyChangedEventArgs)
		TotalTransform = GetTotalTransform(TryCast(args.NewValue, ModelVisual3D))
	End Sub



	' TotalTransform

	' ViewportTransoform

	' SpaceTransform




	' could have Model3D here as well????
	' But can't find group that it's a part of.
	' Plus, it's not unique since it can be shared !!!!


	Public Shared Function GetTotalTransform(obj As DependencyObject) As Matrix3D
	' argument is really a ModelVisual3D
		Dim matx As Matrix3D = Matrix3D.Identity

		While Not (TypeOf obj Is Viewport3DVisual)
			' This occurs when the visual is parent-less.
			If obj Is Nothing Then
				Return ZeroMatrix

			ElseIf TypeOf obj Is ModelVisual3D Then
				If TryCast(obj, ModelVisual3D).Transform IsNot Nothing Then
					matx.Append(TryCast(obj, ModelVisual3D).Transform.Value)
				End If
			Else

				Throw New ApplicationException("didn't end in Viewport3DVisual")
			End If

			obj = VisualTreeHelper.GetParent(obj)
		End While

		' At this point, we know obj is Viewport3DVisual
		Dim vis As Viewport3DVisual = TryCast(obj, Viewport3DVisual)
		Dim matxViewport As Matrix3D = ViewportInfo.GetTotalTransform(vis)
		matx.Append(matxViewport)

		Return matx
	End Function

	Public Overrides Function ToString() As String
		Return TotalTransform.ToString()
	End Function

	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New VisualInfo()
	End Function
End Class
