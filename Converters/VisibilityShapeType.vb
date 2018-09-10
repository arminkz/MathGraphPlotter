Public Class VisibilityShapeType
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim Stype As ShapeType = CType(value, ShapeType)
        Dim Sprop As String = CStr(parameter)
        Select Case Stype
            Case ShapeType.Cube
                If Sprop = "L" Or Sprop = "CF" Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case ShapeType.Box
                If Sprop = "L" Or Sprop = "W" Or Sprop = "H" Or Sprop = "CF" Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case ShapeType.Cone
                If Sprop = "BR" Or Sprop = "H" Or Sprop = "VE" Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case ShapeType.TCone
                If Sprop = "BR" Or Sprop = "TR" Or Sprop = "H" Or Sprop = "VE" Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case ShapeType.Cylinder
                If Sprop = "R" Or Sprop = "H" Or Sprop = "VE" Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case ShapeType.Pyramid
                If Sprop = "BR" Or Sprop = "H" Or Sprop = "N" Or Sprop = "VE" Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case ShapeType.Sphere
                If Sprop = "R" Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case ShapeType.Ellipsoid
                If Sprop = "RX" Or Sprop = "RY" Or Sprop = "RZ" Then
                    Return Visibility.Visible
                Else
                    Return Visibility.Collapsed
                End If
            Case Else
                Return Visibility.Collapsed
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class