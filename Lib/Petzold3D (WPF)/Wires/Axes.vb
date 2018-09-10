'--------------------------------------
' Axes.cs )(c) 2007 by Charles Petzold
'--------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class Axes
	Inherits WireBase
	Private txtgen As New TextGenerator()

	Public Sub New()
		PropertyChanged(New DependencyPropertyChangedEventArgs())
	End Sub

	' Extent property
	' ---------------
	' This property is not animatable
	Public Shared ReadOnly ExtentProperty As DependencyProperty = DependencyProperty.Register("Extent", GetType(Double), GetType(Axes), New UIPropertyMetadata(3.0, AddressOf PropertyChanged, Nothing, True))

	Public Property Extent() As Double
		Get
			Return CDbl(GetValue(ExtentProperty))
		End Get
		Set
			SetValue(ExtentProperty, value)
		End Set
	End Property

	' ShowNumbers property
	' --------------------
	' This property is not animatable
	Public Shared ReadOnly ShowNumbersProperty As DependencyProperty = DependencyProperty.Register("ShowNumbers", GetType(Boolean), GetType(Axes), New UIPropertyMetadata(True, AddressOf PropertyChanged, Nothing, True))

	Public Property ShowNumbers() As Boolean
		Get
			Return CBool(GetValue(ShowNumbersProperty))
		End Get
		Set
			SetValue(ShowNumbersProperty, value)
		End Set
	End Property

	' LargeTick property (not animatable)
	' ------------------
	Public Shared ReadOnly LargeTickProperty As DependencyProperty = DependencyProperty.Register("LargeTick", GetType(Double), GetType(Axes), New UIPropertyMetadata(0.05, AddressOf PropertyChanged, Nothing, True))

	Public Property LargeTick() As Double
		Get
			Return CDbl(GetValue(LargeTickProperty))
		End Get
		Set
			SetValue(LargeTickProperty, value)
		End Set
	End Property

	' SmallTick property (not animatable)
	' ------------------
	Public Shared ReadOnly SmallTickProperty As DependencyProperty = DependencyProperty.Register("SmallTick", GetType(Double), GetType(Axes), New UIPropertyMetadata(0.025, AddressOf PropertyChanged, Nothing, True))

	Public Property SmallTick() As Double
		Get
			Return CDbl(GetValue(SmallTickProperty))
		End Get
		Set
			SetValue(SmallTickProperty, value)
		End Set
	End Property

	' Can these be Add Owner on WireText???

	' Font property
	' ---------------
	Public Shared ReadOnly FontProperty As DependencyProperty = DependencyProperty.Register("Font", GetType(Font), GetType(Axes), New UIPropertyMetadata(Font.Modern, AddressOf PropertyChanged, Nothing, True))

	Public Property Font() As Font
		Get
			Return CType(GetValue(FontProperty), Font)
		End Get
		Set
			SetValue(FontProperty, value)
		End Set
	End Property

	' FontSize property
	' ---------------
	Public Shared ReadOnly FontSizeProperty As DependencyProperty = DependencyProperty.Register("FontSize", GetType(Double), GetType(Axes), New UIPropertyMetadata(0.1, AddressOf PropertyChanged, Nothing, True))

	Public Property FontSize() As Double
		Get
			Return CDbl(GetValue(FontSizeProperty))
		End Get
		Set
			SetValue(FontSizeProperty, value)
		End Set
	End Property


	Public Shared ReadOnly LabelsProperty As DependencyProperty = DependencyProperty.Register("Labels", GetType(String), GetType(Axes), New PropertyMetadata("XYZ", AddressOf PropertyChanged), AddressOf ValidateLabels)

	Public Property Labels() As String
		Get
			Return DirectCast(GetValue(LabelsProperty), String)
		End Get
		Set
			SetValue(LabelsProperty, value)
		End Set
	End Property

	' Ensure Labels string is either null or a multiple of 3 characters
	Private Shared Function ValidateLabels(obj As Object) As Boolean
		Dim str As String = TryCast(obj, String)

		If str Is Nothing Then
			Return True
		End If

		Return str.Length Mod 3 = 0
	End Function


	Protected Overrides Sub Generate(args As DependencyPropertyChangedEventArgs, lines As Point3DCollection)
		lines.Clear()

		' X axis.
		lines.Add(New Point3D(-Extent, 0, 0))
		lines.Add(New Point3D(Extent, 0, 0))

		' Y axis.
		lines.Add(New Point3D(0, -Extent, 0))
		lines.Add(New Point3D(0, Extent, 0))

		' Z axis.
		lines.Add(New Point3D(0, 0, -Extent))
		lines.Add(New Point3D(0, 0, Extent))

		For i As Integer = CInt(Math.Truncate(-10 * Extent)) To CInt(Math.Truncate(10 * Extent))
			Dim tick As Double = If((i Mod 10 = 0), LargeTick, SmallTick)
			Dim d As Double = i / 10.0

			' X axis tick marks.
			lines.Add(New Point3D(d, -tick, 0))
			lines.Add(New Point3D(d, tick, 0))

			' Y axis tick marks.
			lines.Add(New Point3D(-tick, d, 0))
			lines.Add(New Point3D(tick, d, 0))

			' Z axis tick marks.
			lines.Add(New Point3D(0, -tick, d))
			lines.Add(New Point3D(0, tick, d))

			txtgen.Font = Font
			txtgen.FontSize = FontSize
			txtgen.Thickness = Thickness
			txtgen.Rounding = Rounding
			txtgen.BaselineDirection = New Vector3D(1, 0, 0)
			txtgen.UpDirection = New Vector3D(0, 1, 0)

			If i <> 0 AndAlso i Mod 10 = 0 Then
				Dim str As String = CInt(Math.Truncate(d)).ToString()
				Dim isEnd As Boolean = (i = CInt(Math.Truncate(-10 * Extent))) OrElse (i = CInt(Math.Truncate(10 * Extent)))
				Dim strPrefix As String = If((i = CInt(Math.Truncate(-10 * Extent))), "-", "+")

				' X axis numbers and labels.
				If isEnd AndAlso Labels IsNot Nothing Then
					str = strPrefix & Labels.Substring(0, Labels.Length \ 3)
				End If

				If isEnd OrElse ShowNumbers Then
					txtgen.Origin = New Point3D(d, -tick * 1.25, 0)
					txtgen.HorizontalAlignment = HorizontalAlignment.Center
					txtgen.VerticalAlignment = VerticalAlignment.Top
					txtgen.Generate(lines, str)
				End If

				' Y axis numbers and labels.
				If isEnd Then
					str = strPrefix & Labels.Substring(Labels.Length \ 3, Labels.Length \ 3)
				End If

				If isEnd OrElse ShowNumbers Then
					txtgen.Origin = New Point3D(tick * 1.25, d, 0)
					txtgen.HorizontalAlignment = HorizontalAlignment.Left
					txtgen.VerticalAlignment = VerticalAlignment.Center
					txtgen.Generate(lines, str)
				End If

				' Want to make Z either viewed from left or right !!!!!!!!!!!!!!!!!!

				' Z axis numbers and labels.
				If isEnd Then
					str = strPrefix & Labels.Substring(2 * Labels.Length \ 3)
				End If

				If isEnd OrElse ShowNumbers Then
					txtgen.Origin = New Point3D(0, -tick * 1.25, d)
					txtgen.BaselineDirection = New Vector3D(0, 0, 1)
					txtgen.HorizontalAlignment = HorizontalAlignment.Center
					txtgen.VerticalAlignment = VerticalAlignment.Top
					txtgen.Generate(lines, str)
				End If
			End If
		Next
	End Sub
End Class
