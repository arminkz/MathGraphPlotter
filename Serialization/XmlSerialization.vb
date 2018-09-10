Imports System.IO
Imports System.Text

Imports System.Runtime.Serialization
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization.Formatters.Binary


Namespace AKP.Serialization
    Public Class XmlSerialization

#Region "Write To File"
        Public Shared Sub WriteToFile(ByVal Obj As Object, ByVal Location As String, Optional ByVal Overwrite As Boolean = True)
            If IO.File.Exists(Location) Then
                If Overwrite = True Then
                    IO.File.Delete(Location)
                End If
            End If

            Dim ObjStreamWriter As New StreamWriter(Location)
            Dim XmlSer As New XmlSerializer(Obj.GetType)
            XmlSer.Serialize(ObjStreamWriter, Obj)
            ObjStreamWriter.Close()
        End Sub
#End Region


#Region "Read From File"
        Public Shared Function ReadFromFile(ByVal Location As String) As Object
            Dim ObjStreamReader As New StreamReader(Location)
            '<yourtype>  may need change
            Dim Res As New Object
            Dim XmlSer As Xml.Serialization.XmlSerializer
            Res = XmlSer.Deserialize(ObjStreamReader)
            ObjStreamReader.Close()
            Return Res
        End Function
#End Region

        Public Shared Function GetNamespaces() As XmlSerializerNamespaces
            Dim Ns As New XmlSerializerNamespaces
            Ns.Add("xs", "http://www.w3.org/2001/XMLSchema")
            Ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance")
            Return Ns
        End Function

        Public Shared ReadOnly Property TargetNamespace() As String
            Get
                Return "http://www.w3.org/2001/XMLSchema"
            End Get
        End Property

        Public Shared Function ToXml(ByVal Obj As Object, ByVal ObjType As System.Type) As String
            Dim XmlSer As New XmlSerializer(ObjType, TargetNamespace)

            Dim MemStream As New MemoryStream
            Dim XmlWriter As New XmlTextWriter(MemStream, Encoding.UTF8)

            XmlWriter.Namespaces = True
            XmlSer.Serialize(XmlWriter, Obj, GetNamespaces())
            XmlWriter.Close()
            MemStream.Close()


            Dim Xml As String
            Xml = Encoding.UTF8.GetString(MemStream.GetBuffer())
            Xml = Xml.Substring(Xml.IndexOf(Convert.ToChar(60)))
            Xml = Xml.Substring(0, (Xml.LastIndexOf(Convert.ToChar(62)) + 1))
            Return Xml
        End Function


        Public Shared Function FromXml(ByVal Xml As String, ByVal ObjType As System.Type) As Object
            Dim XmlSer As New XmlSerializer(ObjType)

            Dim StringReader As New StringReader(Xml)
            Dim XmlReader As New XmlTextReader(StringReader)

            Dim Obj As Object
            Obj = XmlSer.Deserialize(XmlReader)
            XmlReader.Close()
            StringReader.Close()

            Return Obj
        End Function

    End Class
End Namespace