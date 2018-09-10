Imports System.ComponentModel
Imports System.Windows.Media.Media3D

Public Class APointTrial
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub


    Public Sub Reset()
        Points = New List(Of Point3D)
    End Sub

    Private m_trailpoints As New List(Of Point3D)
    Public Property Points As List(Of Point3D)
        Get
            Return m_trailpoints
        End Get
        Set(value As List(Of Point3D))
            m_trailpoints = value
            NotifyPropertyChanged("Points")
        End Set
    End Property

    Private m_triallength As Integer = 10
    Public Property Length As Integer
        Get
            Return m_triallength
        End Get
        Set(value As Integer)
            m_triallength = value
            NotifyPropertyChanged("Length")
        End Set
    End Property

    Private m_trialcolor As Color = Colors.Blue
    Public Property Color As Color
        Get
            Return m_trialcolor
        End Get
        Set(value As Color)
            m_trialcolor = value
        End Set
    End Property

End Class
