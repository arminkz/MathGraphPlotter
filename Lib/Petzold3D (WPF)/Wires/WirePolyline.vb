'---------------------------------------------
' WirePolyline.cs (c) 2007 by Charles Petzold
'---------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
'''     Draws a polyline of constant perceived width in 3D space
'''     between two points. 
''' </summary>
Public Class WirePolyline
	Inherits WireBase
	''' <summary>
	'''     Identifies the Points dependency property.
	''' </summary>
    Public Shared ReadOnly PointsProperty As DependencyProperty = DependencyProperty.Register("Points", GetType(Point3DCollection), GetType(WirePolyline), New PropertyMetadata(Nothing, AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets a collection that contains the points of 
	'''     the polyline.
	''' </summary>
	Public Property Points() As Point3DCollection
		Get
			Return DirectCast(GetValue(PointsProperty), Point3DCollection)
		End Get
		Set
			SetValue(PointsProperty, value)
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
	'''         The <c>WirePolyline</c> class implements this method by 
	'''         clearing the <c>lines</c> collection and then breaking
	'''         down its <c>Points</c> collection into individual lines
	'''         and then adding the start and end points to the collection.
	'''     </para>
	''' </remarks>
	Protected Overrides Sub Generate(args As DependencyPropertyChangedEventArgs, lines As Point3DCollection)
		Dim points__1 As Point3DCollection = Points
		lines.Clear()

		For i As Integer = 0 To points__1.Count - 2
			lines.Add(points__1(i))
			lines.Add(points__1(i + 1))
		Next
	End Sub
End Class
