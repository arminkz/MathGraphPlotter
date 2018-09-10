Imports System.ComponentModel
Imports System.Windows.Media.Animation
Imports System.Windows.Threading

Public Class Plane 'ساختار صفحه
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#Region "Constructor"
    Private m_pc As List(Of APoint)
    Public Property Points As List(Of APoint)
        Get
            Return m_pc
        End Get
        Set(value As List(Of APoint))
            m_pc = value
            NotifyPropertyChanged("Points")
        End Set
    End Property

    'خطا
    Private m_error As Boolean = False
    Public Property HasError As Boolean
        Get
            Return m_error
        End Get
        Set(value As Boolean)
            m_error = value
            NotifyPropertyChanged("HasError")
        End Set
    End Property

    'توضیحات خطا
    Private m_errormsg As String
    Public Property ErrorMsg As String
        Get
            Return m_errormsg
        End Get
        Set(value As String)
            m_errormsg = value
            NotifyPropertyChanged("ErrorMsg")
        End Set
    End Property

    'کلک انیمیشن
    Private m_IC As Boolean
    Public ReadOnly Property IC As Boolean
        Get
            'Animation Trick (Always Return True!)
            Return True
        End Get
    End Property

#End Region

End Class