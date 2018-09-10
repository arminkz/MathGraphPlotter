'-----------------------------------------
' WireText.cs (c) 2007 by Charles Petzold
'-----------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
''' 
''' </summary>
Public Class WireText
	Inherits WireBase
	Private txtgen As New TextGenerator()

	''' <summary>
	'''     Identifies the Text dependency property.
	''' </summary>
    Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(WireText), New UIPropertyMetadata("", AddressOf PropertyChanged, Nothing, True))

	''' <summary>
	''' 
	''' </summary>
	Public Property Text() As String
		Get
			Return DirectCast(GetValue(TextProperty), String)
		End Get
		Set
			SetValue(TextProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Font dependency property.
	''' </summary>
	Public Shared ReadOnly FontProperty As DependencyProperty = DependencyProperty.Register("Font", GetType(Font), GetType(WireText), New UIPropertyMetadata(Font.Modern, AddressOf PropertyChanged, Nothing, True))

	''' <summary>
	''' 
	''' </summary>
	Public Property Font() As Font
		Get
			Return CType(GetValue(FontProperty), Font)
		End Get
		Set
			SetValue(FontProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the FontSize dependency property. 
	''' </summary>
	Public Shared ReadOnly FontSizeProperty As DependencyProperty = DependencyProperty.Register("FontSize", GetType(Double), GetType(WireText), New UIPropertyMetadata(0.1, AddressOf PropertyChanged, Nothing, True))

	''' <summary>
	''' 
	''' </summary>
	Public Property FontSize() As Double
		Get
			Return CDbl(GetValue(FontSizeProperty))
		End Get
		Set
			SetValue(FontSizeProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Origin dependency property.
	''' </summary>
	Public Shared ReadOnly OriginProperty As DependencyProperty = DependencyProperty.Register("Origin", GetType(Point3D), GetType(WireText), New UIPropertyMetadata(New Point3D(), AddressOf PropertyChanged, Nothing, True))

	''' <summary>
	''' 
	''' </summary>
	Public Property Origin() As Point3D
		Get
			Return CType(GetValue(OriginProperty), Point3D)
		End Get
		Set
			SetValue(OriginProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the BaselineDirection dependency property.
	''' </summary>
	Public Shared ReadOnly BaselineDirectionProperty As DependencyProperty = DependencyProperty.Register("BaselineDirection", GetType(Vector3D), GetType(WireText), New UIPropertyMetadata(New Vector3D(1, 0, 0), AddressOf PropertyChanged, Nothing, True))

	''' <summary>
	''' 
	''' </summary>
	Public Property BaselineDirection() As Vector3D
		Get
			Return CType(GetValue(BaselineDirectionProperty), Vector3D)
		End Get
		Set
			SetValue(BaselineDirectionProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the UpDirection dependency property.
	''' </summary>
	Public Shared ReadOnly UpDirectionProperty As DependencyProperty = DependencyProperty.Register("UpDirection", GetType(Vector3D), GetType(WireText), New UIPropertyMetadata(New Vector3D(0, 1, 0), AddressOf PropertyChanged, Nothing, True))

	''' <summary>
	''' 
	''' </summary>
	Public Property UpDirection() As Vector3D
		Get
			Return CType(GetValue(UpDirectionProperty), Vector3D)
		End Get
		Set
			SetValue(UpDirectionProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the HorizontalAlignment dependency property.
	''' </summary>
	Public Shared ReadOnly HorizontalAlignmentProperty As DependencyProperty = DependencyProperty.Register("HorizontalAlignment", GetType(HorizontalAlignment), GetType(WireText), New UIPropertyMetadata(HorizontalAlignment.Left, AddressOf PropertyChanged, Nothing, True))

	''' <summary>
	''' 
	''' </summary>
	Public Property HorizontalAlignment() As HorizontalAlignment
		Get
			Return CType(GetValue(HorizontalAlignmentProperty), HorizontalAlignment)
		End Get
		Set
			SetValue(HorizontalAlignmentProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the <c>VerticalAlignment</c> dependency property.
	''' </summary>
	Public Shared ReadOnly VerticalAlignmentProperty As DependencyProperty = DependencyProperty.Register("VerticalAlignment", GetType(VerticalAlignment), GetType(WireText), New UIPropertyMetadata(VerticalAlignment.Top, AddressOf PropertyChanged, Nothing, True))

	''' <summary>
	''' 
	''' </summary>
	Public Property VerticalAlignment() As VerticalAlignment
		Get
			Return CType(GetValue(VerticalAlignmentProperty), VerticalAlignment)
		End Get
		Set
			SetValue(VerticalAlignmentProperty, value)
		End Set
	End Property

	Protected Overrides Sub Generate(args As DependencyPropertyChangedEventArgs, lines As Point3DCollection)
		lines.Clear()

		txtgen.Font = Font
		txtgen.FontSize = FontSize
		txtgen.Rounding = Rounding
		txtgen.Thickness = Thickness
		txtgen.BaselineDirection = BaselineDirection
		txtgen.Origin = Origin
		txtgen.UpDirection = UpDirection
		txtgen.VerticalAlignment = VerticalAlignment
		txtgen.HorizontalAlignment = HorizontalAlignment

		txtgen.Generate(lines, Text)
	End Sub
End Class



