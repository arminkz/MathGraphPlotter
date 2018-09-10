'-----------------------------------------
' WirePath.cs (c) 2007 by Charles Petzold
'-----------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
'''     Draws a series of straight lines and curves of perceived uniform
'''     width in 3D space.
''' </summary>
Public Class WirePath
	Inherits WireBase
	''' <summary>
	'''     Initializes a new instance of the <c>WirePath</c> class.
	''' </summary>
	Public Sub New()
		Data = DirectCast(Data.Clone(), PathGeometry3D)
	End Sub

	''' <summary>
	'''     Identifies the <c>Data</c> dependency property.
	''' </summary>
	Public Shared ReadOnly DataProperty As DependencyProperty = DependencyProperty.Register("Data", GetType(PathGeometry3D), GetType(WirePath), New PropertyMetadata(New PathGeometry3D(), AddressOf PropertyChanged))

	''' <summary>
	'''     Gets or sets a <c>PathGeometry3D</c> that specifies the 
	'''     shape to be drawn. 
	''' </summary>
	Public Property Data() As PathGeometry3D
		Get
			Return DirectCast(GetValue(DataProperty), PathGeometry3D)
		End Get
		Set
			SetValue(DataProperty, value)
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
	''' </remarks>
	Protected Overrides Sub Generate(args As DependencyPropertyChangedEventArgs, lines As Point3DCollection)
		lines.Clear()

		If Data Is Nothing Then
			Return
		End If

		Dim xform As Transform3D = Data.Transform

		For Each fig As PathFigure3D In Data.Figures
			Dim figFlattened As PathFigure3D = fig.GetFlattenedPathFigure()
			Dim pointStart As Point3D = xform.Transform(figFlattened.StartPoint)

			For Each seg As PathSegment3D In figFlattened.Segments
				Dim segPoly As PolyLineSegment3D = TryCast(seg, PolyLineSegment3D)

				For i As Integer = 0 To segPoly.Points.Count - 1
					lines.Add(pointStart)
					Dim point As Point3D = xform.Transform(segPoly.Points(i))
					lines.Add(point)
					pointStart = point
				Next
			Next
		Next
	End Sub
End Class




