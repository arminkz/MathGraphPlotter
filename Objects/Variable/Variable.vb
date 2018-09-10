Imports System.ComponentModel
Imports System.Windows.Media.Animation
Imports System.Windows.Threading

Public Class Variable 'ساختار متغییر
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Property Character As String

    Private Sub NotifyPropertyChanged(Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private m_dt As DispatcherTimer

    Public Sub New()
        m_dt = New DispatcherTimer
        AddHandler m_dt.Tick, AddressOf dt_Tick
    End Sub

    Public Sub Play()
        m_dt.IsEnabled = True
        Playing = True
    End Sub

    Public Sub Pause()
        m_dt.IsEnabled = False
        Playing = False
    End Sub

    Private m_ar As Boolean
    Private Sub dt_Tick()
        Select Case TickMode
            Case TickModes.AutoReverse
                If Not m_ar Then
                    Dim NewVal As Double = Math.Round(Value + By, 4, MidpointRounding.AwayFromZero)
                    If NewVal >= Max Then
                        Value = Max
                        m_ar = True
                    Else
                        Value = NewVal
                    End If
                Else
                    Dim NewVal As Double = Math.Round(Value - By, 4, MidpointRounding.AwayFromZero)
                    If NewVal <= Min Then
                        Value = Min
                        m_ar = False
                    Else
                        Value = NewVal
                    End If
                End If
            Case TickModes.Repeat
                Dim NewVal As Double = Math.Round(Value + By, 4, MidpointRounding.AwayFromZero)
                If NewVal <= Min Then NewVal = Min
                If NewVal >= Max Then NewVal = Min
                Value = NewVal
        End Select
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
                ErrorMsg = "نام متغییر غیر قابل قبول است"
            End If
            NotifyPropertyChanged("Name")
        End Set
    End Property

    Private m_value As Double
    Public Property Value As Double
        Get
            Return m_value
        End Get
        Set(value As Double)
            m_value = value
            NotifyPropertyChanged("Value")
        End Set
    End Property

    Private m_min As Double
    Public Property Min As Double
        Get
            Return m_min
        End Get
        Set(value As Double)
            m_min = value
            NotifyPropertyChanged("Min")
        End Set
    End Property

    Private m_max As Double
    Public Property Max As Double
        Get
            Return m_max
        End Get
        Set(value As Double)
            m_max = value
            NotifyPropertyChanged("Max")
        End Set
    End Property

    Public Property Duration As Double
        Get
            Return m_dt.Interval.TotalSeconds
        End Get
        Set(value As Double)
            m_dt.Interval = TimeSpan.FromSeconds(value)
            NotifyPropertyChanged("Duration")
        End Set
    End Property

    Public Enum TickModes
        AutoReverse
        Repeat
    End Enum
    Private m_tickmode As TickModes
    Public Property TickMode As TickModes
        Get
            Return m_tickmode
        End Get
        Set(value As TickModes)
            m_tickmode = value
            NotifyPropertyChanged("TickMode")
        End Set
    End Property

    Private m_by As Double
    Public Property By As Double
        Get
            Return m_by
        End Get
        Set(value As Double)
            m_by = value
            NotifyPropertyChanged("By")
        End Set
    End Property

    Private m_playing As Boolean
    Public Property Playing As Boolean
        Get
            Return m_playing
        End Get
        Set(value As Boolean)
            m_playing = value
            NotifyPropertyChanged("Playing")
        End Set
    End Property

    Private m_autoplay As Boolean
    Public Property Autoplay As Boolean
        Get
            Return m_autoplay
        End Get
        Set(value As Boolean)
            m_autoplay = value
            NotifyPropertyChanged("Autoplay")
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
