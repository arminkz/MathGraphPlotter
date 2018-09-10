Namespace AKP.Input.Joystick
    Public Class XboxControllerStateChangedEventArgs
        Inherits EventArgs

        Private m_CurrentInputState As XInputState
        Public Property CurrentInputState() As XInputState
            Get
                Return m_CurrentInputState
            End Get
            Set(value As XInputState)
                m_CurrentInputState = value
            End Set
        End Property

        Private m_PreviousInputState As XInputState
        Public Property PreviousInputState() As XInputState
            Get
                Return m_PreviousInputState
            End Get
            Set(value As XInputState)
                m_PreviousInputState = value
            End Set
        End Property
    End Class
End Namespace