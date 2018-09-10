Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Module MathTools

    'تبدیل درجه به رادیان
    Public Function DegreeToRadian(ByVal Degree As Double) As Double
        Dim Radian As Double
        Radian = (Math.PI * Degree) / 180
        Return Radian
    End Function
    'تبدیل رادیان به درجه
    Public Function RadianToDegree(ByVal Radian As Double) As Double
        Dim Degree As Double
        Degree = (180 * Radian) / Math.PI
        Return Degree
    End Function
    'رند کردن عدد
    Public Function Round(Value As Double, Optional Dec As Integer = 5) As Double
        Return Math.Round(Value, Dec, MidpointRounding.AwayFromZero)
    End Function
    'نسبت طول به عرض
    Public Function GetAspectRatio(Size As Size) As Double
        Return Size.Width / Size.Height
    End Function
    'ماتریس صفر
    Public ReadOnly ZeroMatrix As New Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
    'ماتریس واحد
    Public ReadOnly OneMatrix As Matrix3D = Matrix3D.Identity
    'محور های مختصات
    Public ReadOnly XAxis As New Vector3D(1, 0, 0)
    Public ReadOnly YAxis As New Vector3D(0, 1, 0)
    Public ReadOnly ZAxis As New Vector3D(0, 0, 1)

End Module
