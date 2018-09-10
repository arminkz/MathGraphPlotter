Imports System.Runtime.CompilerServices
Imports System.Windows.Media.Media3D

Module Point4DTools

    <Extension>
    Public Function ToPoint3D(Input As Point4D) As Point3D
        Return New Point3D(Input.X, Input.Y, Input.Z)
    End Function

    <Extension>
    Public Function ToPoint3DPlane(Input As Point4DPlane) As Point3DPlane
        Dim Output As New Point3DPlane
        For Each P As Point4D In Input.PointCollection
            Output.PointCollection.Add(P.ToPoint3D)
        Next
        Output.Columns = Input.Columns
        Output.Rows = Input.Rows
        Return Output
    End Function

End Module
