'-----------------------------------------------
' PathGeometry3D.cs (c) 2007 by Charles Petzold
'-----------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D

''' <summary>
''' 
''' </summary>
'------------------------------------------------------------------------------
' TODO: [System.ComponentModel.TypeConverter(typeof(PathGeometry3DConverter))]
'------------------------------------------------------------------------------
<System.Windows.Markup.ContentProperty("Figures")> _
Public Class PathGeometry3D
	Inherits Animatable
	' TODO: Bounds, FillRule, MayHaveCurves

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly FiguresProperty As DependencyProperty = DependencyProperty.Register("Figures", GetType(PathFigure3DCollection), GetType(PathGeometry3D))

	''' <summary>
	''' 
	''' </summary>
	Public Property Figures() As PathFigure3DCollection
		Get
			Return DirectCast(GetValue(FiguresProperty), PathFigure3DCollection)
		End Get
		Set
			SetValue(FiguresProperty, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	Public Sub New()
		Figures = New PathFigure3DCollection()
	End Sub

	Public Shared ReadOnly TransformProperty As DependencyProperty = DependencyProperty.Register("Transform", GetType(Transform3D), GetType(PathGeometry3D), New PropertyMetadata(Transform3D.Identity))

	Public Property Transform() As Transform3D
		Get
			Return DirectCast(GetValue(TransformProperty), Transform3D)
		End Get
		Set
			SetValue(TransformProperty, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	''' <param name="progress"></param>
	''' <param name="point"></param>
	''' <param name="tangent"></param>
	Public Sub GetPointAtFractionLength(progress As Double, ByRef point As Point3D, ByRef tangent As Vector3D)
		progress = Math.Max(0, Math.Min(1, progress))
		point = New Point3D()
		tangent = New Vector3D()
		Dim lengthTotal As Double = 0, length As Double = 0

		For Each fig As PathFigure3D In Figures
			Dim figFlattened As PathFigure3D = fig.GetFlattenedPathFigure()
			Dim ptStart As Point3D = figFlattened.StartPoint

			For Each seg As PathSegment3D In figFlattened.Segments
				Dim segPoly As PolyLineSegment3D = TryCast(seg, PolyLineSegment3D)

				For Each pt As Point3D In segPoly.Points
					lengthTotal += (pt - ptStart).Length
					ptStart = pt
				Next
			Next
		Next

		For Each fig As PathFigure3D In Figures
			Dim figFlattened As PathFigure3D = fig.GetFlattenedPathFigure()
			Dim ptStart As Point3D = figFlattened.StartPoint

			For Each seg As PathSegment3D In figFlattened.Segments
				Dim segPoly As PolyLineSegment3D = TryCast(seg, PolyLineSegment3D)

				For Each pt As Point3D In segPoly.Points
					tangent = pt - ptStart
					Dim lengthSeg As Double = tangent.Length
					length += lengthSeg

					If length / lengthTotal >= progress Then
						Dim factor1 As Double = ((length / lengthTotal) - progress) / (lengthSeg / lengthTotal)
						Dim factor2 As Double = 1 - factor1

						point = CType(factor1 * CType(ptStart, Vector3D) + factor2 * CType(pt, Vector3D), Point3D)
						Return
					End If

					ptStart = pt
				Next
			Next
		Next
	End Sub

	Public Overrides Function ToString() As String
		Return Figures.ToString()
	End Function

	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New PathGeometry3D()
	End Function
End Class
