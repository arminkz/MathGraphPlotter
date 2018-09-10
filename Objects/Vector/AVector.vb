Imports System.ComponentModel

Public Class AVector
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

    Public Sub New(X As String, Y As String, Z As String)
        m_x = X
        m_y = Y
        m_z = Z
    End Sub


    'اندازه بردار
    Public ReadOnly Property Magnitude() As Double
        Get
            Return Math.Sqrt(CDbl(X) ^ 2 + CDbl(Y) ^ 2 + CDbl(Z) ^ 2)
        End Get
    End Property

    'منفی بردار
    Public Shared Operator -(v As AVector) As AVector
        Return New AVector(-CDbl(v.X), -CDbl(v.Y), -CDbl(v.Z))
    End Operator

    'جمع دو بردار
    Shared Operator +(v1 As AVector, v2 As AVector)
        Dim nx As Double = CDbl(v1.X) + CDbl(v2.X)
        Dim ny As Double = CDbl(v1.Y) + CDbl(v2.Y)
        Dim nz As Double = CDbl(v1.Z) + CDbl(v2.Z)
        Return New AVector(nx, ny, nz)
    End Operator
    'تفریق دو بردار
    Shared Operator -(v1 As AVector, v2 As AVector)
        Dim nx As Double = CDbl(v1.X) - CDbl(v2.X)
        Dim ny As Double = CDbl(v1.Y) - CDbl(v2.Y)
        Dim nz As Double = CDbl(v1.Z) - CDbl(v2.Z)
        Return New AVector(nx, ny, nz)
    End Operator
    'ضرب خارجی دو بردار
    Shared Function CrossProduct(v1 As AVector, v2 As AVector) As AVector
        'Cross Product = [(y1*z2)-(z1*y2)]i - [(z1*x2)-(x1*z2)]j + [(x1*y2)-(y1*x2)]k
        Dim nx As Double = (CDbl(v1.Y) * CDbl(v2.Z)) - (CDbl(v1.Z) * CDbl(v2.Y))
        Dim ny As Double = (CDbl(v1.Z) * CDbl(v2.X)) - (CDbl(v1.X) * CDbl(v2.Z))
        Dim nz As Double = (CDbl(v1.X) * CDbl(v2.Y)) - (CDbl(v1.Y) * CDbl(v2.X))
        Return New AVector(nx, -ny, nz)
    End Function
    'ضرب داخلی دو بردار
    Shared Function DotProduct(v1 As AVector, v2 As AVector) As Double
        'DotProduct = (x1*x2 + y1*y2 + z1*z2)
        Return CDbl(v1.X) * CDbl(v2.X) + CDbl(v1.Y) * CDbl(v2.Y) + CDbl(v1.Z) * CDbl(v2.Z)
    End Function


    'جمع نقطه با بردار
    Shared Operator +(p As APoint, v As AVector)
        Dim nx As Double = CDbl(p.X) + CDbl(v.X)
        Dim ny As Double = CDbl(p.Y) + CDbl(v.Y)
        Dim nz As Double = CDbl(p.Z) + CDbl(v.Z)
        Return New APoint(nx, ny, nz)
    End Operator
    'تفریق نقطه با بردار
    Shared Operator -(p As APoint, v As AVector)
        Dim nx As Double = CDbl(p.X) - CDbl(v.X)
        Dim ny As Double = CDbl(p.Y) - CDbl(v.Y)
        Dim nz As Double = CDbl(p.Z) - CDbl(v.Z)
        Return New APoint(nx, ny, nz)
    End Operator


    'ضرب عدد در بردار
    Shared Operator *(d As Double, v As AVector)
        Dim nx As Double = v.X * d
        Dim ny As Double = v.Y * d
        Dim nz As Double = v.Z * d
        Return New AVector(nx, ny, nz)
    End Operator
    Shared Operator *(v As AVector, d As Double)
        Dim nx As Double = v.X * d
        Dim ny As Double = v.Y * d
        Dim nz As Double = v.Z * d
        Return New AVector(nx, ny, nz)
    End Operator
    'تقسیم بردار بر عدد
    Shared Operator /(v As AVector, d As Double)
        Dim nx As Double = v.X / d
        Dim ny As Double = v.Y / d
        Dim nz As Double = v.Z / d
        Return New AVector(nx, ny, nz)
    End Operator

    Public Function ToPoint() As APoint
        Return New APoint(m_x, m_y, m_z)
    End Function

End Class
