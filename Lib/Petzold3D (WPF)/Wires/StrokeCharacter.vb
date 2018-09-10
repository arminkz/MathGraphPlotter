'------------------------------------------------
' StrokeCharacter.cs (c) 2007 by Charles Petzold
'------------------------------------------------

'-------------------------------------------------------
' This class describes the elements of the 
' *FontResourceDictionary.xaml files.
'
' The class is internal to the Petzold.Medi3D assembly.
'-------------------------------------------------------
Imports System.Windows
Imports System.Windows.Media

Class StrokeCharacter
	Private m_width As Integer
	Private geo As Geometry

	Public Sub New(width__1 As Integer, strGeometry As String)
		Width = width__1
		Geometry = Geometry.Parse(strGeometry)
	End Sub

	' The Width of the font character.
	Public Property Width() As Integer
		Get
			Return m_width
		End Get
		Set
			m_width = value
		End Set
	End Property

	' The Geometry that defines the font character.
	Public Property Geometry() As Geometry
		Get
			Return geo
		End Get
		Set
			geo = value
		End Set
	End Property
End Class
