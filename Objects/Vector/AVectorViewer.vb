Imports System.ComponentModel

Public Class AVectorViewer
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#Region "Constructor"
    Private m_name As String
    Public Property Name As String
        Get
            Return m_name
        End Get
        Set(value As String)
            m_name = value
            If value = "" Then
                HasError = True
                ErrorMsg = "نام بردار غیر قابل قبول است"
            Else
                HasError = False
                ErrorMsg = ""
            End If
            NotifyPropertyChanged("Name")
        End Set
    End Property

    Private m_vect As AVector
    Public Property Vector As AVector
        Get
            Return m_vect
        End Get
        Set(value As AVector)
            m_vect = value
            NotifyPropertyChanged("Vector")
        End Set
    End Property

    Private m_origin As String
    Public Property PointName As String
        Get
            Return m_origin
        End Get
        Set(value As String)
            m_origin = value
            NotifyPropertyChanged("PointName")
        End Set
    End Property

    'رنگ
    Private m_color As Color
    Public Property Color As Color
        Get
            Return m_color
        End Get
        Set(value As Color)
            m_color = value
            NotifyPropertyChanged("Color")
        End Set
    End Property

    'مرئی بودن
    Private m_isvisible As Boolean
    Public Property IsVisible As Boolean
        Get
            Return m_isvisible
        End Get
        Set(value As Boolean)
            m_isvisible = value
            NotifyPropertyChanged("IsVisible")
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

    'کلاس نمودار
    Private m_graphobject As New List(Of Object)
    Public Property GraphObject As List(Of Object)
        Get
            Return m_graphobject
        End Get
        Set(value As List(Of Object))
            m_graphobject = value
            NotifyPropertyChanged("GraphObject")
        End Set
    End Property

    'کلک انیمیشن
    Public ReadOnly Property IC As Boolean
        Get
            'Animation Trick (Always Return True!)
            Return True
        End Get
    End Property
#End Region

End Class
