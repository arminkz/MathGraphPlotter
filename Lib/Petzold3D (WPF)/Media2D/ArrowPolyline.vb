'----------------------------------------------
' ArrowPolyline.cs (c) 2007 by Charles Petzold
'----------------------------------------------
Imports System.Windows
Imports System.Windows.Media

Namespace Petzold.Media2D
	''' <summary>
	'''     Draws a series of connected straight lines with
	'''     optional arrows on the ends.
	''' </summary>
	Public Class ArrowPolyline
		Inherits ArrowLineBase
		''' <summary>
		'''     Identifies the Points dependency property.
		''' </summary>
		Public Shared ReadOnly PointsProperty As DependencyProperty = DependencyProperty.Register("Points", GetType(PointCollection), GetType(ArrowPolyline), New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.AffectsMeasure))

		''' <summary>
		'''     Gets or sets a collection that contains the 
		'''     vertex points of the ArrowPolyline.
		''' </summary>
		Public Property Points() As PointCollection
			Get
				Return DirectCast(GetValue(PointsProperty), PointCollection)
			End Get
			Set
				SetValue(PointsProperty, value)
			End Set
		End Property

		''' <summary>
		'''     Initializes a new instance of the ArrowPolyline class. 
		''' </summary>
		Public Sub New()
			Points = New PointCollection()
		End Sub

		''' <summary>
		'''     Gets a value that represents the Geometry of the ArrowPolyline.
		''' </summary>
		Protected Overrides ReadOnly Property DefiningGeometry() As Geometry
			Get
				' Clear out the PathGeometry.
				pathgeo.Figures.Clear()

				' Try to avoid unnecessary indexing exceptions.
				If Points.Count > 0 Then
					' Define a PathFigure containing the points.
					pathfigLine.StartPoint = Points(0)
					polysegLine.Points.Clear()

					For i As Integer = 1 To Points.Count - 1
						polysegLine.Points.Add(Points(i))
					Next

					pathgeo.Figures.Add(pathfigLine)
				End If

				' Call the base property to add arrows on the ends.
				Return MyBase.DefiningGeometry
			End Get
		End Property
	End Class
End Namespace
