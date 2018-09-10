Imports System.ComponentModel
Imports System.Xml.Serialization

Public Class MathObject
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Sub New()

    End Sub

    Public Sub New(Type As Integer)
        Select Case Type
            Case 0 ' Equation = 0
                m_inner = New Equation
                m_type = 0
            Case 1 ' Variable = 1
                m_inner = New Variable
                m_type = 1
            Case 2 ' Point = 2
                m_inner = New APointViewer
                m_type = 2
            Case 3 ' Vector = 3
                m_inner = New AVectorViewer
                m_type = 3
            Case 4 ' Shape = 4
                m_inner = New AShape
                m_type = 4

            Case 99 ' AddNewObject = 99
                m_inner = New Equation
                m_type = 99
        End Select
    End Sub

    Private m_type As Integer
    Public Property Type As Integer
        Get
            Return m_type
        End Get
        Set(value As Integer)
            m_type = value
            NotifyPropertyChanged("Type")
        End Set
    End Property

    Private m_inner As Object
    Public Property Inner As Object
        Get
            Return m_inner
        End Get
        Set(value As Object)
            m_inner = value
            NotifyPropertyChanged("Inner")
        End Set
    End Property

    'پنجره تنظیمات
    Private m_optionopen As Boolean = False
    Public Property IsOptionOpen As Boolean
        Get
            Return m_optionopen
        End Get
        Set(value As Boolean)
            m_optionopen = value
            NotifyPropertyChanged("IsOptionOpen")
        End Set
    End Property

End Class
