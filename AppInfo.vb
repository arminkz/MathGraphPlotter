Public Class AppInfo

    Shared ReadOnly Property Version As String
        Get
            Return My.Application.Info.Version.Major.ToString + "." + My.Application.Info.Version.Minor.ToString
        End Get
    End Property

    Shared ReadOnly Property TerminalVersion As String
        Get
            Return "0.9"
        End Get
    End Property

    Shared ReadOnly Property SetupVersion As String
        Get
            Return "0.3"
        End Get
    End Property

End Class