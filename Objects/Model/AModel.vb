Imports System.ComponentModel
Imports System.Windows.Media.Media3D

Public Class AModel
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler _
        Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#Region "Constructor"

    'نام
    Private m_name As String
    Public Property Name As String
        Get
            Return m_name
        End Get
        Set(value As String)
            m_name = value
            NotifyPropertyChanged("Name")
        End Set
    End Property

    'نوع شکل
    Private m_type As ShapeType
    Public Property Type As ShapeType
        Get
            Return m_type
        End Get
        Set(value As ShapeType)
            m_type = value
        End Set
    End Property

    'مبدا
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

    'جهت
    Private m_dir As String
    Public Property VectorName As String
        Get
            Return m_dir
        End Get
        Set(value As String)
            m_dir = value
            NotifyPropertyChanged("VectorName")
        End Set
    End Property

    'طول
    Private m_length As String
    Public Property Length As String
        Get
            Return m_length
        End Get
        Set(value As String)
            m_length = value
            NotifyPropertyChanged("Length")
        End Set
    End Property

    'عرض
    Private m_width As String
    Public Property Width As String
        Get
            Return m_width
        End Get
        Set(value As String)
            m_width = value
            NotifyPropertyChanged("Width")
        End Set
    End Property

    'ارتفاع
    Private m_height As String
    Public Property Height As String
        Get
            Return m_height
        End Get
        Set(value As String)
            m_height = value
            NotifyPropertyChanged("Height")
        End Set
    End Property

    'شعاع
    Private m_radius As String
    Public Property Radius As String
        Get
            Return m_radius
        End Get
        Set(value As String)
            m_radius = value
            NotifyPropertyChanged("Radius")
        End Set
    End Property

    'شعاع دوم
    Private m_radius2 As String
    Public Property Radius2 As String
        Get
            Return m_radius2
        End Get
        Set(value As String)
            m_radius2 = value
            NotifyPropertyChanged("Radius2")
        End Set
    End Property

    'شعاع سوم
    Private m_radius3 As String
    Public Property Radius3 As String
        Get
            Return m_radius3
        End Get
        Set(value As String)
            m_radius3 = value
            NotifyPropertyChanged("Radius3")
        End Set
    End Property

    'تعداد وجه
    Private m_facecount As Integer
    Public Property FaceCount As Integer
        Get
            Return m_facecount
        End Get
        Set(value As Integer)
            m_facecount = value
            NotifyPropertyChanged("FaceCount")
        End Set
    End Property

    'وجه های مکعب
    Private m_cubeface As Integer
    Public Property CubeFace As Integer
        Get
            Return m_cubeface
        End Get
        Set(value As Integer)
            m_cubeface = value
            NotifyPropertyChanged("CubeFace")
        End Set
    End Property

    'تقسیمات زاویه تتا
    Private m_thetadiv As Double
    Public Property ThetaDiv As Double
        Get
            Return m_thetadiv
        End Get
        Set(value As Double)
            m_thetadiv = value
            NotifyPropertyChanged("ThetaDiv")
        End Set
    End Property

    'تقسیمات زاویه فی
    Private m_phidiv As Double
    Public Property PhiDiv As Double
        Get
            Return m_phidiv
        End Get
        Set(value As Double)
            m_phidiv = value
            NotifyPropertyChanged("PhiDiv")
        End Set
    End Property

    'کف
    Private m_basecap As Boolean
    Public Property BaseCap As Boolean
        Get
            Return m_basecap
        End Get
        Set(value As Boolean)
            m_basecap = value
            NotifyPropertyChanged("BaseCap")
        End Set
    End Property

    'سقف
    Private m_topcap As Boolean
    Public Property TopCap As Boolean
        Get
            Return m_topcap
        End Get
        Set(value As Boolean)
            m_topcap = value
            NotifyPropertyChanged("TopCap")
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
            NotifyPropertyChanged("Brush")
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
