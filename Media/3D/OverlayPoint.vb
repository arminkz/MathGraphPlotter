Imports System.Windows.Media.Media3D
Imports System.ComponentModel

Public Class OverlayPoint
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private m_point As Point3D
    Public Property Point As Point3D
        Get
            Return m_point
        End Get
        Set(value As Point3D)
            m_point = value
        End Set
    End Property

    Private m_text As String
    Public Property Text As String
        Get
            Return m_text
        End Get
        Set(value As String)
            m_text = value
        End Set
    End Property

    Private m_color As Color
    Public Property Color As Color
        Get
            Return m_color
        End Get
        Set(value As Color)
            m_color = value
        End Set
    End Property

End Class
