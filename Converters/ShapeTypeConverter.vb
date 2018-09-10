Public Class ShapeTypeConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim ASH As AShape = CType(value, AShape)
        If ASH.Name = "" Then
            Select Case ASH.Type
                Case ShapeType.Cone
                    Return "مخروط"
                Case ShapeType.TCone
                    Return "مخروط ناقص"
                Case ShapeType.Cylinder
                    Return "استوانه"
                Case ShapeType.Pyramid
                    Return "هرم"
                Case ShapeType.Cube
                    Return "مکعب"
                Case ShapeType.Box
                    Return "مکعب مستطیل"
                Case ShapeType.Sphere
                    Return "کره"
                Case ShapeType.Ellipsoid
                    Return "کره ی بیضوی"
                Case ShapeType.None
                    Return ""
                Case Else
                    Return ""
            End Select
        Else
            Return ASH.Name
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException
    End Function
End Class
