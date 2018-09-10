Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Public Class TerminalCommand

    Private m_Raw As String
    Public Property Raw() As String
        Get
            Return m_Raw
        End Get
        Private Set(value As String)
            m_Raw = value
        End Set
    End Property

    Private m_Name As String
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Private Set(value As String)
            m_Name = value
        End Set
    End Property

    Private m_Args As String()
    Public Property Args() As String()
        Get
            Return m_Args
        End Get
        Private Set(value As String())
            m_Args = value
        End Set
    End Property

    Private m_IsRemote As Boolean
    Public Property IsRemote
        Get
            Return m_IsRemote
        End Get
        Set(value)
            m_IsRemote = value
        End Set
    End Property


    Public Sub New(Raw1 As String, Name2 As String, Args3 As String())
        Raw = Raw1
        Name = Name2
        Args = Args3
    End Sub

    Public Function GetDescription(CommandFormat As String, FirstArgFormat As String, OtherArgsFormat As String, [End] As String) As String
        Dim Ret As String = String.Format(CommandFormat, Name)
        If Args.Length > 0 Then
            Ret += String.Format(FirstArgFormat, Args(0))
        End If
        For i As Integer = 1 To Args.Length - 1
            Ret += String.Format(OtherArgsFormat, Args(i))
        Next
        Return Ret & [End]
    End Function

End Class
