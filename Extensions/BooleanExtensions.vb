Imports System.Runtime.CompilerServices

Module BooleanExtensions

    <Extension> Public Function To01(B As Boolean) As String
        If B Then
            Return "1"
        Else
            Return "0"
        End If
    End Function

End Module
