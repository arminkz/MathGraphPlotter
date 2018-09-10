Imports System.ComponentModel

Public Class APointViewer
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
                ErrorMsg = "نام نقطه غیر قابل قبول است"
            Else
                HasError = False
                ErrorMsg = ""
            End If
            NotifyPropertyChanged("Name")
        End Set
    End Property

    Private m_point As APoint
    Public Property Point As APoint
        Get
            Return m_point
        End Get
        Set(value As APoint)
            m_point = value
            NotifyPropertyChanged("Point")
        End Set
    End Property

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

    'منیر بودن
    Private m_islight As Boolean
    Public Property IsLight As Boolean
        Get
            Return m_islight
        End Get
        Set(value As Boolean)
            m_islight = value
            NotifyPropertyChanged("IsLight")
        End Set
    End Property

    'نمایش دنباله 
    Private m_havetrial As Boolean = True
    Public Property HaveTrial As Boolean
        Get
            Return m_havetrial
        End Get
        Set(value As Boolean)
            m_havetrial = value
            'Reset Trial
            m_trial = New APointTrial
            NotifyPropertyChanged("HaveTrial")
        End Set
    End Property

    'دنباله
    Private m_trial As New APointTrial
    Public Property Trial As APointTrial
        Get
            Return m_trial
        End Get
        Set(value As APointTrial)
            m_trial = value
            NotifyPropertyChanged("Trial")
        End Set
    End Property

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
