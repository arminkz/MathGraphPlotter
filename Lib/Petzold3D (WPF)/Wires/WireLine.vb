'-----------------------------------------
' WireLine.cs (c) 2007 by Charles Petzold
'-----------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
'''     Draws a straight line of constant perceived width in 3D space
'''     between two points. 
''' </summary>
Public Class WireLine
	Inherits WireBase
	''' <summary>
	'''     Identifies the Point1 dependency property.
	''' </summary>
    Public Shared ReadOnly Point1Property As DependencyProperty = DependencyProperty.Register("Point1", GetType(Point3D), GetType(WireLine), New PropertyMetadata(New Point3D(), AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the Line start point.
	''' </summary>
	Public Property Point1() As Point3D
		Get
			Return CType(GetValue(Point1Property), Point3D)
		End Get
		Set
			SetValue(Point1Property, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the Point2 dependency property.
	''' </summary>
	Public Shared ReadOnly Point2Property As DependencyProperty = DependencyProperty.Register("Point2", GetType(Point3D), GetType(WireLine), New PropertyMetadata(New Point3D(), AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets the Line end point.
	''' </summary>
	Public Property Point2() As Point3D
		Get
			Return CType(GetValue(Point2Property), Point3D)
		End Get
		Set
			SetValue(Point2Property, value)
		End Set
	End Property

	''' <summary>
	'''     Sets the coordinates of all the individual lines in the visual.
	''' </summary>
	''' <param name="args">
	'''     The <c>DependencyPropertyChangedEventArgs</c> object associated 
	'''     with the property-changed event that resulted in this method 
	'''     being called.
	''' </param>
	''' <param name="lines">
	'''     The <c>Point3DCollection</c> to be filled.
	''' </param>
	''' <remarks>
	'''     <para>
	'''         Classes that derive from <c>WireBase</c> override this
	'''         method to fill the <c>lines</c> collection.
	'''         It is custmary for implementations of this method to clear
	'''         the <c>lines</c> collection first before filling it. 
	'''         Each pair of successive members of the <c>lines</c>
	'''         collection indicate one straight line.
	'''     </para>
	'''     <para>
	'''         The <c>WireLine</c> class implements this method by 
	'''         clearing the <c>lines</c> collection and then adding 
	'''         <c>Point1</c> and <c>Point2</c> to the collection.
	'''     </para>
	''' </remarks>
	Protected Overrides Sub Generate(args As DependencyPropertyChangedEventArgs, lines As Point3DCollection)
		lines.Clear()
		lines.Add(Point1)
		lines.Add(Point2)
	End Sub
End Class
