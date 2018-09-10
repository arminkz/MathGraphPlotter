Imports System.Runtime.Serialization
Imports System.Collections
Imports System.Reflection

Namespace AKP.Serialization
    <Serializable()> _
    Public MustInherit Class SerializableObject
        Implements ISerializable, IDeserializationCallback

        Public Sub New()
            MyBase.New()
        End Sub
        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)

            ' Instantiate base class
            MyBase.New()

            ' Record the Info object between the constructor and the deserialization callback
            ' because if we set the values of fields that are declared with
            ' an initializer, the initializer will come along and overwrite the value
            ' after this constructor has been called
            mInfo = info

        End Sub

#Region "Serialization Implementation"


        ' Variable to store the serialization info between New() and OnDeserialization()
        <NonSerialized()> Private mInfo As SerializationInfo


        '*******************************************************************************
        ' GetObjectData: Serializes the object
        '                This takes care of all objects in the inheritance 
        '                hierarchy. Derived classes should only override this
        '                method to add extra data.
        '*******************************************************************************
        Protected Overridable Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext) Implements ISerializable.GetObjectData

            ' Use the shared method to populate the SerializationInfo object
            SerializableObject.SerializeObject(Me, info)

        End Sub


        '*******************************************************************************
        ' OnDeserialization: Called when serialization is finished to complete the process
        '                    This takes care of all objects in the inheritance 
        '                    hierarchy. Derived classes should only override this
        '                    method to preform any extra initialization
        '*******************************************************************************
        Protected Overridable Sub OnDeserialization(ByVal sender As Object) Implements System.Runtime.Serialization.IDeserializationCallback.OnDeserialization

            ' Call the shared method to deserialize the object
            SerializableObject.DeserializeObject(Me, mInfo)

            ' Kill the recorded info object
            mInfo = Nothing

        End Sub




#End Region

#Region "Shared Methods"

        '*******************************************************************************
        ' SerializeObejct: Serializes all the private fields of an object and filters
        '                  out event delegates
        '*******************************************************************************
        Public Shared Sub SerializeObject(ByVal obj As Object, ByVal info As SerializationInfo)


            ' Local Variables
            Dim aMembersToSerialize As MemberInfo()

            ' Error handler
            Try

                ' Get a list of all fields in this object and derived objects
                ' that are to be serialized
                aMembersToSerialize = SerializableObject.GetSerializableMembers(obj.GetType)

                '  Loop around all fields and save their values
                For Each objMember As MemberInfo In aMembersToSerialize

                    ' It is valid to serialize if it is in this array
                    ' Derived Fields with the same name as base fields
                    ' will automaticall be handled, so we don't need to
                    ' worry about duplicates

                    ' Determine if it is a filed or property
                    If objMember.MemberType = MemberTypes.Field Then

                        Dim objField As FieldInfo = DirectCast(objMember, FieldInfo)
                        info.AddValue(objField.Name, objField.GetValue(obj))

                    ElseIf objMember.MemberType = MemberTypes.Property Then

                        Dim objProperty As PropertyInfo = DirectCast(objMember, PropertyInfo)
                        info.AddValue(objProperty.Name, objProperty.GetValue(obj, Nothing))

                    End If

                Next

            Catch ex As Exception

                ' Trace any errors
                Trace.WriteLine("Error during serialization: " & ex.Message)
                Throw New SerializationException("Error during Serialization. ", ex)

            End Try

        End Sub


        '*******************************************************************************
        ' DeserializeObject: Populates an object instance from a serialized instance
        '                    The object must be fully created for this method to be called
        '                    i.e. Don't call this from a constructor - instead call it
        '                    from IDeserializationCallback.OnDeserialization
        '*******************************************************************************
        <System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, Assertion:=True, SerializationFormatter:=True)> _
        Public Shared Sub DeserializeObject(ByVal obj As Object, ByVal info As SerializationInfo)

            ' Local Variables
            Dim aFieldsToDeserialize As MemberInfo()
            Dim aValues As Object()

            ' Error handler
            Try


                ' Get a list of all fields in this object and derived objects
                aFieldsToDeserialize = SerializableObject.GetSerializableMembers(obj.GetType)

                ' Loop around all fields and get their values from the info object
                aValues = DirectCast(Array.CreateInstance(GetType(Object), aFieldsToDeserialize.Length), Object())

                For nCount As Integer = 0 To aFieldsToDeserialize.Length - 1

                    ' Determine if it is a field or property
                    If aFieldsToDeserialize(nCount).MemberType = MemberTypes.Field Then

                        ' get the field
                        Dim objField As FieldInfo = DirectCast(aFieldsToDeserialize(nCount), FieldInfo)

                        ' Get the value of the field from the info object
                        aValues(nCount) = info.GetValue(objField.Name, objField.FieldType)

                    ElseIf aFieldsToDeserialize(nCount).MemberType = MemberTypes.Property Then

                        ' get the property
                        Dim objProperty As PropertyInfo = DirectCast(aFieldsToDeserialize(nCount), PropertyInfo)

                        ' Get the value of the field from the info object
                        aValues(nCount) = info.GetValue(objProperty.Name, objProperty.PropertyType)

                    End If

                Next

                ' Now use formatter services to populate this object's fields
                FormatterServices.PopulateObjectMembers(obj, aFieldsToDeserialize, aValues)

            Catch ex As Exception

                ' Trace errors
                Trace.WriteLine("Error during deserialization: " & ex.Message)
                Throw New SerializationException("Error during deserialization", ex)

            End Try

        End Sub


        '*******************************************************************************
        ' GetSerializableMembers: Returns an array of all fields in the type and base types
        '                         That are to be serialized.
        '                         It excludes delegates and event delegates
        '*******************************************************************************
        <System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, Assertion:=True, SerializationFormatter:=True)> _
        Private Shared Function GetSerializableMembers(ByVal PersistableType As System.Type) As MemberInfo()

            ' Local Variables
            Dim aAllMembers() As System.Reflection.MemberInfo
            Dim aSerilizableMembers As System.Collections.ArrayList

            '  Get all the serializable members
            aAllMembers = FormatterServices.GetSerializableMembers(PersistableType)

            aSerilizableMembers = New System.Collections.ArrayList

            ' Filter non-serializable fields and event delegates
            For Each objMember As MemberInfo In aAllMembers

                ' We're only interested in fields and properties
                If objMember.MemberType = MemberTypes.Field Then

                    ' get the field
                    Dim objField As FieldInfo = DirectCast(objMember, FieldInfo)

                    ' If it has a NonSerialized Attribute or if it is an event delegate, skip it
                    If (Not GetType(System.Delegate).IsAssignableFrom(objField.FieldType)) Then

                        ' Add the field as it is valid for serialization
                        aSerilizableMembers.Add(objField)

                    End If

                ElseIf objMember.MemberType = MemberTypes.Property Then

                    ' get the property
                    Dim objProperty As PropertyInfo = DirectCast(objMember, PropertyInfo)

                    ' if it is an event delegate, skip it
                    If (Not GetType(System.Delegate).IsAssignableFrom(objProperty.PropertyType)) Then

                        ' Add the field as it is valid for serialization
                        aSerilizableMembers.Add(objProperty)

                    End If

                End If

            Next objMember


            ' Return the array of persistable fields - need to convert to an array first
            Return DirectCast(aSerilizableMembers.ToArray(GetType(MemberInfo)), MemberInfo())

        End Function


#End Region

    End Class
End Namespace
