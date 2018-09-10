Namespace AKP.Input.Joystick
    Public Class XboxControllerMovmentEventArgs
        Inherits EventArgs

        Private m_X As Integer
        Public Property X() As Integer
            Get
                Return m_X
            End Get
            Set(value As Integer)
                m_X = value
            End Set
        End Property

        Private m_Y As Integer
        Public Property Y() As Integer
            Get
                Return m_Y
            End Get
            Set(value As Integer)
                m_Y = value
            End Set
        End Property

    End Class
End Namespace