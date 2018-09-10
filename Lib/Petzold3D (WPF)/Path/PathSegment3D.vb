'----------------------------------------------
' PathSegment3D.cs (c) 2007 by Charles Petzold
'----------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D

Public MustInherit Class PathSegment3D
	Inherits Animatable
	' IsStroked property.
	' -------------------
	Public Shared ReadOnly IsStrokedProperty As DependencyProperty = DependencyProperty.Register("IsStroked", GetType(Boolean), GetType(PathSegment3D), New PropertyMetadata(True))

	Public Property IsStroked() As Boolean
		Get
			Return CBool(GetValue(IsStrokedProperty))
		End Get
		Set
			SetValue(IsStrokedProperty, value)
		End Set
	End Property

	' IsSmoothJoin property.
	' ----------------------
	Public Shared ReadOnly IsSmoothJoinProperty As DependencyProperty = DependencyProperty.Register("IsSmoothJoin", GetType(Boolean), GetType(PathSegment3D), New PropertyMetadata(False))

	Public Property IsSmoothJoin() As Boolean
		Get
			Return CBool(GetValue(IsSmoothJoinProperty))
		End Get
		Set
			SetValue(IsSmoothJoinProperty, value)
		End Set
	End Property
End Class
