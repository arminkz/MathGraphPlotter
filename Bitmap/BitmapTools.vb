Imports System.Runtime.InteropServices

Public Class BitmapTools

    ' LayoutKind.Sequential --> Unmanaged Memory
    <StructLayout(LayoutKind.Sequential)> _
    Public Structure PixelColor
        Public Blue As Byte
        Public Green As Byte
        Public Red As Byte
        Public Alpha As Byte
    End Structure

    Public Shared Function GetPixels(Source As BitmapSource) As PixelColor(,)
        If Source.Format <> PixelFormats.Bgra32 Then
            Source = New FormatConvertedBitmap(Source, PixelFormats.Bgra32, Nothing, 0)
        End If

        Dim Width As Integer = Source.PixelWidth
        Dim Height As Integer = Source.PixelHeight
        Dim Result As PixelColor(,) = New PixelColor(Width - 1, Height - 1) {}

        Source.CopyPixels(Result, Width * 4, 0)
        Return Result
    End Function

    Public Shared Sub PutPixels(Bitmap As WriteableBitmap, Pixels As PixelColor(,), X As Integer, Y As Integer)
        Dim Width As Integer = Pixels.GetLength(0)
        Dim Height As Integer = Pixels.GetLength(1)
        Bitmap.WritePixels(New Int32Rect(0, 0, Width, Height), Pixels, Width * 4, X, Y)
    End Sub

End Class
