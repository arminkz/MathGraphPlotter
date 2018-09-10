Imports System.Runtime.CompilerServices
Imports System.Windows.Media.Media3D

Module Point3DExtensions

    <Extension> Public Function ToPoint2D(P3D As Point3D) As Point
        Return New Point(P3D.X, P3D.Y)
    End Function

    <Extension> Public Function ToVector3D(P3D As Point3D) As Vector3D
        Return New Vector3D(P3D.X, P3D.Y, P3D.Z)
    End Function

End Module
