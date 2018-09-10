Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Shapes

Public Class GradientPath
    Inherits FrameworkElement
    Const OutlinePenWidth As Double = 1

    Public Shared ReadOnly DataProperty As DependencyProperty = Path.DataProperty.AddOwner(GetType(GradientPath), New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.AffectsMeasure Or FrameworkPropertyMetadataOptions.AffectsRender))

    Public Shared ReadOnly GradientStopsProperty As DependencyProperty = GradientBrush.GradientStopsProperty.AddOwner(GetType(GradientPath), New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.AffectsRender))

    Public Shared ReadOnly GradientModeProperty As DependencyProperty = DependencyProperty.Register("GradientMode", GetType(GradientMode), GetType(GradientPath), New FrameworkPropertyMetadata(GradientMode.Perpendicular, FrameworkPropertyMetadataOptions.AffectsRender))

    Public Shared ReadOnly ColorInterpolationModeProperty As DependencyProperty = GradientBrush.ColorInterpolationModeProperty.AddOwner(GetType(GradientPath), New FrameworkPropertyMetadata(ColorInterpolationMode.SRgbLinearInterpolation, FrameworkPropertyMetadataOptions.AffectsRender))

    Public Shared ReadOnly StrokeThicknessProperty As DependencyProperty = Shape.StrokeThicknessProperty.AddOwner(GetType(GradientPath), New FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsMeasure Or FrameworkPropertyMetadataOptions.AffectsRender))

    Public Shared ReadOnly StrokeStartLineCapProperty As DependencyProperty = Shape.StrokeStartLineCapProperty.AddOwner(GetType(GradientPath), New FrameworkPropertyMetadata(PenLineCap.Flat, FrameworkPropertyMetadataOptions.AffectsMeasure Or FrameworkPropertyMetadataOptions.AffectsRender))

    Public Shared ReadOnly StrokeEndLineCapProperty As DependencyProperty = Shape.StrokeEndLineCapProperty.AddOwner(GetType(GradientPath), New FrameworkPropertyMetadata(PenLineCap.Flat, FrameworkPropertyMetadataOptions.AffectsMeasure Or FrameworkPropertyMetadataOptions.AffectsRender))

    Public Shared ReadOnly ToleranceProperty As DependencyProperty = DependencyProperty.Register("Tolerance", GetType(Double), GetType(GradientPath), New FrameworkPropertyMetadata(Geometry.StandardFlatteningTolerance, FrameworkPropertyMetadataOptions.AffectsRender))

    Public Sub New()
        GradientStops = New GradientStopCollection()
    End Sub

    Public Property Data() As Geometry
        Get
            Return DirectCast(GetValue(DataProperty), Geometry)
        End Get
        Set(value As Geometry)
            SetValue(DataProperty, value)
        End Set
    End Property

    Public Property GradientStops() As GradientStopCollection
        Get
            Return DirectCast(GetValue(GradientStopsProperty), GradientStopCollection)
        End Get
        Set(value As GradientStopCollection)
            SetValue(GradientStopsProperty, value)
        End Set
    End Property

    Public Property GradientMode() As GradientMode
        Get
            Return DirectCast(GetValue(GradientModeProperty), GradientMode)
        End Get
        Set(value As GradientMode)
            SetValue(GradientModeProperty, value)
        End Set
    End Property

    Public Property ColorInterpolationMode() As ColorInterpolationMode
        Get
            Return DirectCast(GetValue(ColorInterpolationModeProperty), ColorInterpolationMode)
        End Get
        Set(value As ColorInterpolationMode)
            SetValue(ColorInterpolationModeProperty, value)
        End Set
    End Property

    Public Property StrokeThickness() As Double
        Get
            Return CDbl(GetValue(StrokeThicknessProperty))
        End Get
        Set(value As Double)
            SetValue(StrokeThicknessProperty, value)
        End Set
    End Property

    Public Property StrokeStartLineCap() As PenLineCap
        Get
            Return DirectCast(GetValue(StrokeStartLineCapProperty), PenLineCap)
        End Get
        Set(value As PenLineCap)
            SetValue(StrokeStartLineCapProperty, value)
        End Set
    End Property

    Public Property StrokeEndLineCap() As PenLineCap
        Get
            Return DirectCast(GetValue(StrokeEndLineCapProperty), PenLineCap)
        End Get
        Set(value As PenLineCap)
            SetValue(StrokeEndLineCapProperty, value)
        End Set
    End Property

    Public Property Tolerance() As Double
        Get
            Return CDbl(GetValue(ToleranceProperty))
        End Get
        Set(value As Double)
            SetValue(ToleranceProperty, value)
        End Set
    End Property

    Protected Overrides Function MeasureOverride(AvailableSize As Size) As Size
        If Data Is Nothing Then
            Return MyBase.MeasureOverride(availableSize)
        End If

        Dim Pen As New Pen() With { _
            .Brush = Brushes.Black, _
            .Thickness = StrokeThickness, _
            .StartLineCap = StrokeStartLineCap, _
            .EndLineCap = StrokeEndLineCap _
        }

        Dim Rect As Rect = Data.GetRenderBounds(Pen)
        Try
            Return New Size(Rect.Right, Rect.Bottom)
        Catch
            Return New Size(0, 0)
        End Try
    End Function

    Protected Overrides Sub OnRender(DC As DrawingContext)
        If Data Is Nothing Then
            Return
        End If

        ' Flatten the PathGeometry
        Dim pathGeometry As PathGeometry = Data.GetFlattenedPathGeometry(Tolerance, ToleranceType.Absolute)

        For Each pathFigure As PathFigure In pathGeometry.Figures
            If pathFigure.Segments.Count <> 1 Then
                Throw New NotSupportedException("More than one PathSegment in a flattened PathFigure")
            End If

            If Not (TypeOf pathFigure.Segments(0) Is PolyLineSegment) Then
                Throw New NotSupportedException("Segment is not PolylineSegment in flattened PathFigure")
            End If

            Dim polylineSegment As PolyLineSegment = TryCast(pathFigure.Segments(0), PolyLineSegment)
            Dim points As PointCollection = polylineSegment.Points

            If points.Count < 1 Then
                Throw New NotSupportedException("Empty PointCollection in PolylineSegment in flattened PathFigure")
            End If

            ' Calculate total length for ParallelMode
            Dim totalLength As Double = 0
            Dim accumLength As Double = 0
            Dim ptPrev As Point = pathFigure.StartPoint
            For Each pt As Point In points
                totalLength += (pt - ptPrev).Length
                ptPrev = pt
            Next

            ' Get the first vector
            Dim vector As Vector = points(0) - pathFigure.StartPoint

            ' Use that to draw the start line cap
            DrawLineCap(dc, pathFigure.StartPoint, vector, StrokeStartLineCap, PenLineCap.Flat)

            ' Rotate the vector counter-clockwise 90 degrees
            Dim vector90 As New Vector(vector.Y, -vector.X)
            vector90.Normalize()

            ' Calculate perpendiculars
            Dim ptUpPrev As Point = pathFigure.StartPoint + StrokeThickness / 2 * vector90
            Dim ptDnPrev As Point = pathFigure.StartPoint - StrokeThickness / 2 * vector90

            ' Begin with the StartPoint
            ptPrev = pathFigure.StartPoint

            ' Loop through the PointCollection
            For index As Integer = 0 To points.Count - 1
                Dim ptUp As Point, ptDn As Point
                Dim point As Point = points(index)
                Dim vect1 As Vector = ptPrev - point
                Dim angle1 As Double = Math.Atan2(vect1.Y, vect1.X)

                ' Treat the last point much like the first
                If index = points.Count - 1 Then
                    ' Rotate clockwise 90 degrees
                    vector90 = New Vector(-vect1.Y, vect1.X)
                    vector90.Normalize()
                    ptUp = point + (StrokeThickness / 2) * vector90
                    ptDn = point - (StrokeThickness / 2) * vector90
                Else

                    ' Get the next vector and average the two
                    Dim vect2 As Vector = points(index + 1) - point
                    Dim angle2 As Double = Math.Atan2(vect2.Y, vect2.X)
                    Dim diff As Double = angle2 - angle1

                    If diff < 0 Then
                        diff += 2 * Math.PI
                    End If

                    Dim angle As Double = angle1 + diff / 2
                    Dim vect As New Vector(Math.Cos(angle), Math.Sin(angle))
                    vect.Normalize()
                    ptUp = point + StrokeThickness / 2 * vect
                    ptDn = point - StrokeThickness / 2 * vect
                End If
                ' Transform to horizontalize tetragon constructed of perpendiculars
                Dim rotate As New RotateTransform(-180 * angle1 / Math.PI, ptPrev.X, ptPrev.Y)

                ' Construct the tetragon
                Dim tetragonGeo As New PathGeometry()
                Dim tetragonFig As New PathFigure()
                tetragonFig.StartPoint = rotate.Transform(ptUpPrev)
                tetragonFig.IsClosed = True
                tetragonFig.IsFilled = True
                Dim tetragonSeg As New PolyLineSegment()
                tetragonSeg.Points.Add(rotate.Transform(ptUp))
                tetragonSeg.Points.Add(rotate.Transform(ptDn))
                tetragonSeg.Points.Add(rotate.Transform(ptDnPrev))
                tetragonFig.Segments.Add(tetragonSeg)
                tetragonGeo.Figures.Add(tetragonFig)

                Dim brush As LinearGradientBrush

                If GradientMode = GradientMode.Perpendicular Then
                    brush = New LinearGradientBrush(GradientStops, New Point(0, 1), New Point(0, 0))
                Else
                    ' Find where we are in the total path
                    Dim offset1 As Double = accumLength / totalLength
                    accumLength += vect1.Length
                    Dim offset2 As Double = accumLength / totalLength

                    ' Calculate ax + b factors for gradientStop.Offset conversion
                    Dim a As Double = 1 / (offset2 - offset1)
                    Dim b As Double = -offset1 * a

                    ' Calculate a new GradientStopCollection based on restricted lenth
                    Dim gradientStops__1 As New GradientStopCollection()

                    If GradientStops IsNot Nothing Then
                        For Each gradientStop As GradientStop In GradientStops
                            gradientStops__1.Add(New GradientStop(gradientStop.Color, a * gradientStop.Offset + b))
                        Next
                    End If

                    brush = New LinearGradientBrush(gradientStops__1, New Point(1, 0), New Point(0, 0))
                End If

                ' Draw the tetragon rotated back into place
                brush.ColorInterpolationMode = ColorInterpolationMode
                Dim pen As New Pen(brush, OutlinePenWidth)
                rotate.Angle = 180 * angle1 / Math.PI
                dc.PushTransform(rotate)
                dc.DrawGeometry(brush, pen, tetragonGeo)
                dc.Pop()

                ' Something special for the last point
                If index = points.Count - 1 Then
                    DrawLineCap(dc, point, -vect1, PenLineCap.Flat, StrokeEndLineCap)
                End If

                ' Prepare for next iteration
                ptPrev = point
                ptUpPrev = ptUp
                ptDnPrev = ptDn
            Next
        Next
    End Sub

    Private Sub DrawLineCap(dc As DrawingContext, point As Point, vector As Vector, startLineCap As PenLineCap, endLineCap As PenLineCap)
        If startLineCap = PenLineCap.Flat AndAlso endLineCap = PenLineCap.Flat Then
            Return
        End If

        ' Construct really tiny horizontal line
        vector.Normalize()
        Dim angle As Double = Math.Atan2(vector.Y, vector.X)
        Dim rotate As New RotateTransform(-180 * angle / Math.PI, point.X, point.Y)
        Dim point1 As Point = rotate.Transform(point)
        Dim point2 As Point = rotate.Transform(point + 0.25 * vector)

        ' Construct pen for that line
        Dim pen As New Pen() With { _
            .Thickness = StrokeThickness, _
            .StartLineCap = startLineCap, _
            .EndLineCap = endLineCap _
        }

        ' Why don't I just call dc.DrawLine at this point? Well, to avoid gaps between 
        '  the tetragons, I had to draw them with an 'outlinePenWidth' pen based on the 
        '  same brush as the fill. If I just called dc.DrawLine here, the caps would 
        '  look a little smaller than the line, so....

        Dim lineGeo As New LineGeometry(point1, point2)
        Dim pathGeo As PathGeometry = lineGeo.GetWidenedPathGeometry(Pen)
        Dim brush As Brush = Nothing

        If GradientMode = GradientMode.Perpendicular Then
            brush = New LinearGradientBrush(GradientStops, New Point(0, 0), New Point(0, 1))
            TryCast(brush, LinearGradientBrush).ColorInterpolationMode = ColorInterpolationMode
        Else
            Dim offset As Double = If(endLineCap = PenLineCap.Flat, 0, 1)
            brush = New SolidColorBrush(GetColorFromGradientStops(offset))
        End If

        Pen = New Pen(brush, outlinePenWidth)
        rotate.Angle = 180 * angle / Math.PI
        dc.PushTransform(rotate)
        dc.DrawGeometry(brush, Pen, pathGeo)
        dc.Pop()
    End Sub

    Private Function GetColorFromGradientStops(offset As Double) As Color
        If GradientStops Is Nothing OrElse GradientStops.Count = 0 Then
            Return Color.FromArgb(0, 0, 0, 0)
        End If

        If GradientStops.Count = 1 Then
            Return GradientStops(0).Color
        End If

        Dim lowerOffset As Double = [Double].MinValue
        Dim upperOffset As Double = [Double].MaxValue
        Dim lowerIndex As Integer = -1
        Dim upperIndex As Integer = -1

        For i As Integer = 0 To GradientStops.Count - 1
            Dim gradientStop As GradientStop = GradientStops(i)

            If lowerOffset < gradientStop.Offset AndAlso gradientStop.Offset <= offset Then
                lowerOffset = gradientStop.Offset
                lowerIndex = i
            End If

            If upperOffset > gradientStop.Offset AndAlso gradientStop.Offset >= offset Then
                upperOffset = gradientStop.Offset
                upperIndex = i
            End If
        Next

        If lowerIndex = -1 Then
            Return GradientStops(upperIndex).Color

        ElseIf upperIndex = -1 Then
            Return GradientStops(lowerIndex).Color
        End If

        If lowerIndex = upperIndex Then
            Return GradientStops(lowerIndex).Color
        End If

        Dim clr1 As Color = GradientStops(lowerIndex).Color
        Dim clr2 As Color = GradientStops(upperIndex).Color
        Dim den As Double = upperOffset - lowerOffset
        Dim wt1 As Single = CSng((upperOffset - offset) / den)
        Dim wt2 As Single = CSng((offset - lowerOffset) / den)
        Dim clr As New Color()

        Select Case ColorInterpolationMode
            Case ColorInterpolationMode.SRgbLinearInterpolation
                clr = Color.FromArgb(CByte(Math.Truncate(wt1 * clr1.A + wt2 * clr2.A)), CByte(Math.Truncate(wt1 * clr1.R + wt2 * clr2.R)), CByte(Math.Truncate(wt1 * clr1.G + wt2 * clr2.G)), CByte(Math.Truncate(wt1 * clr1.B + wt2 * clr2.B)))
                Exit Select

            Case ColorInterpolationMode.ScRgbLinearInterpolation
                clr = clr1 * wt1 + clr2 * wt2
                Exit Select
        End Select
        Return clr
    End Function
End Class
