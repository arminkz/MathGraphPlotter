Imports System.ComponentModel

Public Class Equation 'ساختار تابع
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#Region "Constructor"
    'ضابطه تابع
    Private m_exp As String
    Public Property Expression As String
        Get
            Return m_exp
        End Get
        Set(value As String)
            m_exp = value
            NeedsRecalculation = True
            NotifyPropertyChanged("Expression")
        End Set
    End Property

    'نوع تابع
    Private m_type As EquationType
    Public Property Type As EquationType
        Get
            Return m_type
        End Get
        Set(value As EquationType)
            m_type = value
            NotifyPropertyChanged("Type")
        End Set
    End Property

    'قلمو
    Private m_brush As Brush
    Public Property Brush As Brush
        Get
            Return m_brush
        End Get
        Set(value As Brush)
            m_brush = value
            NeedsRecalculation = True
            NotifyPropertyChanged("Brush")
        End Set
    End Property

    Private m_material As PlaneMaterial
    Public Property Material As PlaneMaterial
        Get
            Return m_material
        End Get
        Set(value As PlaneMaterial)
            m_material = value
            NeedsRecalculation = True
            NotifyPropertyChanged("Material")
        End Set
    End Property

    'ضخامت قلم
    Private m_thick As Double
    Public Property Thickness As Double
        Get
            Return m_thick
        End Get
        Set(value As Double)
            m_thick = value
            NeedsRecalculation = True
            NotifyPropertyChanged("Thickness")
        End Set
    End Property

    'شفافیت
    Private m_opacity As Double
    Public Property Opacity As Double
        Get
            Return m_opacity
        End Get
        Set(value As Double)
            If value >= 0 And value <= 1 Then
                m_opacity = value
                NeedsRecalculation = True
                NotifyPropertyChanged("Opacity")
            Else
                Throw New Exception("Set Opacity Between 0 & 1")
            End If
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
            NeedsRecalculation = True
            NotifyPropertyChanged("IsVisible")
        End Set
    End Property

    'شروع دامنه
    Private m_domain_start As Double
    Public Property DomainStart As Double
        Get
            Return m_domain_start
        End Get
        Set(value As Double)
            m_domain_start = value
            NeedsRecalculation = True
            NotifyPropertyChanged("DomainStart")
        End Set
    End Property

    'پایان دامنه
    Private m_domain_end As Double
    Public Property DomainEnd As Double
        Get
            Return m_domain_end
        End Get
        Set(value As Double)
            m_domain_end = value
            NeedsRecalculation = True
            NotifyPropertyChanged("DomainEnd")
        End Set
    End Property

    'نقاط استثنا دامنه
    Private m_domain_except As DoubleCollection
    Public Property DomainExcept As DoubleCollection
        Get
            Return m_domain_except
        End Get
        Set(value As DoubleCollection)
            m_domain_except = value
            NeedsRecalculation = True
            NotifyPropertyChanged("DomainExcept")
        End Set
    End Property

    'شروع دامنه دوم
    Private m_domain_start_2 As Double
    Public Property SecondDomainStart As Double
        Get
            Return m_domain_start_2
        End Get
        Set(value As Double)
            m_domain_start_2 = value
            NeedsRecalculation = True
            NotifyPropertyChanged("SecondDomainStart")
        End Set
    End Property

    'پایان دامنه دوم
    Private m_domain_end_2 As Double
    Public Property SecondDomainEnd As Double
        Get
            Return m_domain_end_2
        End Get
        Set(value As Double)
            m_domain_end_2 = value
            NeedsRecalculation = True
            NotifyPropertyChanged("SecondDomainEnd")
        End Set
    End Property

    'نقاط استثنا دامنه دوم
    Private m_domain_except_2 As DoubleCollection
    Public Property SecondDomainExcept As DoubleCollection
        Get
            Return m_domain_except_2
        End Get
        Set(value As DoubleCollection)
            m_domain_except_2 = value
            NeedsRecalculation = True
            NotifyPropertyChanged("SecondDomainExcept")
        End Set
    End Property

    'شروع برد
    Private m_range_min As Double
    Public Property RangeMin As Double
        Get
            Return m_range_min
        End Get
        Set(value As Double)
            m_range_min = value
            NotifyPropertyChanged("RangeMin")
        End Set
    End Property

    'پایان برد
    Private m_range_max As Double
    Public Property RangeMax As Double
        Get
            Return m_range_max
        End Get
        Set(value As Double)
            m_range_max = value
            NotifyPropertyChanged("RangeMax")
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

    'نیاز به محاسبه دوباره 
    Private m_needsrecalc As Boolean = True
    Public Property NeedsRecalculation As Boolean
        Get
            Return m_needsrecalc
        End Get
        Set(value As Boolean)
            m_needsrecalc = value
            NotifyPropertyChanged("NeedsRecalculation")
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

    Private m_description As String
    Public Property Description As String
        Get
            Return m_description
        End Get
        Set(value As String)
            m_description = value
            NotifyPropertyChanged("Description")
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
