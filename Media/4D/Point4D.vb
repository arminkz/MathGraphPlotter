Public Class Point4D

    Public Sub New(X As Double, Y As Double, Z As Double, W As Double)
        m_x = X
        m_y = Y
        m_z = Z
        m_w = W
    End Sub

    Public Property X As Double
        Get
            Return m_x
        End Get
        Set(value As Double)
            m_x = value
        End Set
    End Property
    Private m_x As Double

    Public Property Y As Double
        Get
            Return m_y
        End Get
        Set(value As Double)
            m_y = value
        End Set
    End Property
    Private m_y As Double

    Public Property Z As Double
        Get
            Return m_z
        End Get
        Set(value As Double)
            m_z = value
        End Set
    End Property
    Private m_z As Double

    Public Property W As Double
        Get
            Return m_w
        End Get
        Set(value As Double)
            m_w = value
        End Set
    End Property
    Private m_w As Double

    Shared Operator -(P1 As Point4D, P2 As Point4D) As Vector4D
        Return New Vector4D(P1.X - P2.X, P1.Y - P2.Y, P1.Z - P2.Z, P1.W - P2.W)
    End Operator

    Public Overrides Function ToString() As String
        Return Me.X.ToString & "," & Me.Y.ToString & "," & Me.Z.ToString & "," & Me.W.ToString
    End Function

End Class
