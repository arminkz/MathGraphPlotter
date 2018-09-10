Imports System.ComponentModel
Imports System.Windows.Media.Media3D

Public Class APoint
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#Region "Constructor"
    Private m_x As String
    Public Property X As String
        Get
            Return m_x
        End Get
        Set(value As String)
            m_x = value
            NotifyPropertyChanged("X")
        End Set
    End Property

    Private m_y As String
    Public Property Y As String
        Get
            Return m_y
        End Get
        Set(value As String)
            m_y = value
            NotifyPropertyChanged("Y")
        End Set
    End Property

    Private m_z As String
    Public Property Z As String
        Get
            Return m_z
        End Get
        Set(value As String)
            m_z = value
            NotifyPropertyChanged("Z")
        End Set
    End Property

#End Region


    Public Sub New(X, Y, Z)
        m_x = X.ToString
        m_y = Y.ToString
        m_z = Z.ToString
    End Sub

    Public Function ToPoint3D() As Point3D
        Return New Point3D(CDbl(m_x), CDbl(m_y), CDbl(m_z))
    End Function

    Public Function ToPoint2D() As Point
        Return New Point(CDbl(m_x), CDbl(m_y))
    End Function

End Class
