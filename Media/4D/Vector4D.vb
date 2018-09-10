Public Class Vector4D

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

    Public Function Length() As Double
        Return Math.Sqrt(m_x ^ 2 + m_y ^ 2 + m_z ^ 2 + m_w ^ 2)
    End Function

    Public Function LengthSqured() As Double
        Return m_x ^ 2 + m_y ^ 2 + m_z ^ 2 + m_w ^ 2
    End Function

    Public Sub Normalize()
        m_x /= Me.Length
        m_y /= Me.Length
        m_z /= Me.Length
        m_w /= Me.Length
    End Sub

    Shared Operator +(V1 As Vector4D, V2 As Vector4D) As Vector4D
        Return New Vector4D(V1.X + V2.X, V1.Y + V2.Y, V1.Z + V2.Z, V1.W + V2.W)
    End Operator

    Shared Operator -(V1 As Vector4D, V2 As Vector4D) As Vector4D
        Return New Vector4D(V1.X - V2.X, V1.Y - V2.Y, V1.Z - V2.Z, V1.W - V2.W)
    End Operator

    Shared Operator *(K As Double, V As Vector4D) As Vector4D
        Return New Vector4D(K * V.X, K * V.Y, K * V.Z, K * V.W)
    End Operator

    Shared Operator /(V As Vector4D, K As Double) As Vector4D
        Return New Vector4D(V.X / K, V.Y / K, V.Z / K, V.W / K)
    End Operator

    Shared Function CrossProduct(V1 As Vector4D, V2 As Vector4D, V3 As Vector4D) As Vector4D
        'Determinant Values
        Dim A, B, C, D, E, F As Double
        A = (V2.X * V3.Y) - (V2.Y * V3.X)
        B = (V2.X * V3.Z) - (V2.Z * V3.X)
        C = (V2.X * V3.W) - (V2.W * V3.X)
        D = (V2.Y * V3.Z) - (V2.Z * V3.Y)
        E = (V2.Y * V3.W) - (V2.W * V3.Y)
        F = (V2.Z * V3.W) - (V2.W * V3.Z)

        Dim Temp As New Vector4D(0, 0, 0, 0)
        Temp.X = (V1.Y * F) - (V1.Z * E) + (V1.W * D)
        Temp.Y = -(V1.X * F) + (V1.Z * C) - (V1.W * B)
        Temp.Z = (V1.X * E) - (V1.Y * C) + (V1.W * A)
        Temp.W = -(V1.X * D) + (V1.Y * B) - (V1.Z * A)

        Return Temp
    End Function

    Shared Function DotProduct(V As Vector4D, U As Vector4D) As Double
        Return V.X * U.X + V.Y * U.Y + V.Z * U.Z + V.W * U.W
    End Function

End Class
