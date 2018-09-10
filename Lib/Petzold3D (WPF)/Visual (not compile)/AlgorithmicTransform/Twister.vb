'----------------------------------------
' Twister.cs (c) 2007 by Charles Petzold
'
' Part of Petzold.Media3D.dll
'
' Note: No PropertyChanged event handlers are required in this class
'  because instances are always children of an 
'  AlgorithmicTransformCollection, and that generates changes
'  notifications on its own.
'---------------------------------------- 
Imports System.Windows
Imports System.Windows.Media.Media3D

Public Class Twister
	Inherits AlgorithmicTransform
	' Private fields for Transform method.
	' ------------------------------------
	Private axisRotate As AxisAngleRotation3D
	Private xform As RotateTransform3D

	' Constructor just sets private fields.
	' -------------------------------------
	Public Sub New()
		axisRotate = New AxisAngleRotation3D()
		xform = New RotateTransform3D(axisRotate)
	End Sub

	' Axis property (probably shouldn't be (0, 0, 0) but we'll let it go).
	' --------------------------------------------------------------------
	Public Shared ReadOnly AxisProperty As DependencyProperty = DependencyProperty.Register("Axis", GetType(Vector3D), GetType(Twister), New PropertyMetadata(New Vector3D(0, 1, 0)))

	Public Property Axis() As Vector3D
		Get
			Return CType(GetValue(AxisProperty), Vector3D)
		End Get
		Set
			SetValue(AxisProperty, value)
		End Set
	End Property

	' Center property.
	' ----------------
	Public Shared ReadOnly CenterProperty As DependencyProperty = DependencyProperty.Register("Center", GetType(Point3D), GetType(Twister), New PropertyMetadata(New Point3D(0, 0, 0)))

	Public Property Center() As Point3D
		Get
			Return CType(GetValue(CenterProperty), Point3D)
		End Get
		Set
			SetValue(CenterProperty, value)
		End Set
	End Property

	' Angle property.
	' ---------------
	Public Shared ReadOnly AngleProperty As DependencyProperty = DependencyProperty.Register("Angle", GetType(Double), GetType(Twister), New PropertyMetadata(0.0))

	Public Property Angle() As Double
		Get
			Return CDbl(GetValue(AngleProperty))
		End Get
		Set
			SetValue(AngleProperty, value)
		End Set
	End Property

	' Attenuation property (must be greater than zero).
	' -------------------------------------------------
	Public Shared ReadOnly AttenuationProperty As DependencyProperty = DependencyProperty.Register("Attenuation", GetType(Double), GetType(Twister), New PropertyMetadata(1.0), AddressOf ValidateAttenuation)

	Public Property Attenuation() As Double
		Get
			Return CDbl(GetValue(AttenuationProperty))
		End Get
		Set
			SetValue(AttenuationProperty, value)
		End Set
	End Property

	Private Shared Function ValidateAttenuation(obj As Object) As Boolean
		Return CDbl(obj) > 0
	End Function

	' Required CreateInstanceCore method when inheriting from Freezable.
	' ------------------------------------------------------------------
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New Twister()
	End Function

	' Required Transform method when inheriting from AlgorithmicTransform.
	' --------------------------------------------------------------------
	Public Overrides Sub Transform(points As Point3DCollection)
		xform.CenterX = Center.X
		xform.CenterY = Center.Y
		xform.CenterZ = Center.Z

		axisRotate.Axis = Axis
		Dim axisNormalized As Vector3D = Axis
		axisNormalized.Normalize()
		Dim angleAttenuated As Double = Angle / Attenuation

		For i As Integer = 0 To points.Count - 1
			axisRotate.Angle = angleAttenuated * Vector3D.DotProduct(axisNormalized, points(i) - Center)

			points(i) = xform.Transform(points(i))
		Next
	End Sub
End Class
