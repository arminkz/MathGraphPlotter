Imports System.Runtime.CompilerServices
Imports System.Windows.Media.Media3D

Module Vector3DExtensions

    <Extension>
    Public Function FindAnyPerpendicular(V As Vector3D) As Vector3D
        V.Normalize()
        Dim U As Vector3D = Vector3D.CrossProduct(New Vector3D(0, 1, 0), V)
        If U.LengthSquared < 0.001 Then
            U = Vector3D.CrossProduct(New Vector3D(1, 0, 0), V)
        End If

        Return U
    End Function

    <Extension>
    Public Function IsUndefined(V As Vector3D) As Boolean
        Return Double.IsNaN(v.X) AndAlso Double.IsNaN(v.Y) AndAlso Double.IsNaN(v.Z)
    End Function

    <Extension>
    Public Function ToPoint3D(V As Vector3D) As Point3D
        Return New Point3D(V.X, V.Y, V.Z)
    End Function

End Module
