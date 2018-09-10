Imports System.Collections.ObjectModel

Public Class BorderClipConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        If values.Length = 3 And values(0).GetType Is GetType(Double) And values(1).GetType Is GetType(Double) _
            And values(2).GetType Is GetType(CornerRadius) Then

            Dim Width As Double = CType(values(0), Double)
            Dim Height As Double = CType(values(1), Double)

            If Width < Double.Epsilon Or Height < Double.Epsilon Then
                Return Geometry.Empty
            End If

            Dim Radius As CornerRadius = CType(values(2), CornerRadius)

            'Beta Version Note :
            'This Clip Converter Does Not Support Different Corner Radiuses For Each Corner, All Four Corners Must Have Same Value.

            If Not (Radius.TopLeft = Radius.TopRight And Radius.TopRight = Radius.BottomRight And Radius.BottomRight = Radius.BottomLeft) Then
                Throw New NotSupportedException("Different Corner Radiuses For Each Corner Are NOT Supported. #AKP")
            End If

            Dim Clip = New RectangleGeometry(New Rect(0, 0, Width, Height), Radius.TopLeft, Radius.TopLeft)
            Clip.Freeze()

            Return Clip
        End If

        Return DependencyProperty.UnsetValue
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotSupportedException("Not Supported #AKP")
    End Function
End Class