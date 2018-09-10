'
'
'
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
'''     Draws a series of successive straight line of constant perceived 
'''     width in 3D space between two points. 
''' </summary>
Public Class WireLines
	Inherits WireBase
	''' <summary>
	'''     Identifies the Lines dependency property.
	''' </summary>
    Public Shared ReadOnly LinesProperty As DependencyProperty = DependencyProperty.Register("Lines", GetType(Point3DCollection), GetType(WireLines), New PropertyMetadata(Nothing, AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets a collection that contains the start and end
	'''     points of each individual line.
	''' </summary>
	''' <remarks>
	'''     This collection normally contains an even number of points.
	'''     Each pair of points specifies one line.
	''' </remarks>
	Public Property Lines() As Point3DCollection
		Get
			Return DirectCast(GetValue(LinesProperty), Point3DCollection)
		End Get
		Set
			SetValue(LinesProperty, value)
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
	'''         The <c>WireLines</c> class implements this method by 
	'''         clearing the <c>lines</c> collection and then copying
	'''         its own <c>Lines</c> collection to it.
	'''     </para>
	''' </remarks>
	Protected Overrides Sub Generate(args As DependencyPropertyChangedEventArgs, lines__1 As Point3DCollection)
		lines__1.Clear()

		For Each point As Point3D In Lines
			lines__1.Add(point)
		Next
	End Sub
End Class
