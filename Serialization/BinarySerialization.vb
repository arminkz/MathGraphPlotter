Imports System.IO
Imports System.Text

Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

Namespace AKP.Serialization
    Public Class BinarySerialization

        Public Shared Sub WriteToFile(ByVal Obj As Object, ByVal Location As String, Optional ByVal Overwrite As Boolean = True)
            If IO.File.Exists(Location) Then
                If Overwrite = True Then
                    IO.File.Delete(Location)
                End If
            End If
            Dim Fs As New FileStream(Location, FileMode.CreateNew)
            Dim Bw As New BinaryWriter(Fs)
            Bw.Write(Serialize(Obj))
            Bw.Close()
            Fs.Close()
        End Sub

        Shared Function Serialize(ByVal Data As Object) As Byte()
            If TypeOf Data Is Byte() Then Return Data
            Using M As New IO.MemoryStream : Dim F As New BinaryFormatter : F.Serialize(M, Data) : Return M.ToArray() : End Using
        End Function

        Shared Function Deserialize(Of T)(ByVal Data As Byte()) As T
            Using M As New IO.MemoryStream(Data, False) : Return CType((New BinaryFormatter).Deserialize(M), T) : End Using
        End Function

    End Class
End Namespace