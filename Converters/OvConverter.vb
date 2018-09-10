Public Class OvConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing Then Return ""
        Dim MO As MathObject = DirectCast(value, MathObject)
        If MO.Type = 0 Then
            Dim MOInner As Equation = CType(MO.Inner, Equation)
            If MOInner.Type <> EquationType.CartesianN Then
                Return "/AKP Math Graph Plotter;component/Images/CurveOv.png"
            Else
                Return "/AKP Math Graph Plotter;component/Images/SequenceOv.png"
            End If
        End If
        If MO.Type = 2 Then
            Return "/AKP Math Graph Plotter;component/Images/PointOv.png"
        End If
        If MO.Type = 3 Then
            Return "/AKP Math Graph Plotter;component/Images/VectorOv.png"
        End If
        If MO.Type = 4 Then
            Return "/AKP Math Graph Plotter;component/Images/ShapeOv.png"
        End If
        Return ""
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException("Not Supported. #AKP")
    End Function

End Class
