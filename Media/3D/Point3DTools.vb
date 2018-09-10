Imports System.Windows.Media.Media3D

Public Module Point3DTools

    Public Function Get2DPoint(P3D As Point3D, VP As Viewport3D) As Point
        Dim TransformationResultOK As Boolean
        Dim vp3Dv As Viewport3DVisual = TryCast(VisualTreeHelper.GetParent(VP.Children(0)), Viewport3DVisual)
        Dim m As Matrix3D = MathUtils.TryWorldToViewportTransform(vp3Dv, TransformationResultOK)
        If Not TransformationResultOK Then
            Return New Point(0, 0)
        End If
        Dim PTemp As Point3D = m.Transform(P3D)
        Dim P2D As New Point(PTemp.X, PTemp.Y)
        Return P2D
    End Function

    Public Function Get2DPointTransformed(P3D As Point3D, Visual As Visual3D, VP As Viewport3D) As Point
        Dim TransformationResultOK As Boolean
        Dim vp3Dv As Viewport3DVisual = TryCast(VisualTreeHelper.GetParent(VP.Children(0)), Viewport3DVisual)
        Dim m As Matrix3D = MathUtils.TryTransformTo2DAncestor(Visual, vp3Dv, TransformationResultOK)
        If Not TransformationResultOK Then
            Return New Point(0, 0)
        End If
        Dim PTemp As Point3D = m.Transform(P3D)
        Dim P2D As New Point(PTemp.X, PTemp.Y)
        Return P2D
    End Function

End Module
