Public Class Dialog

    Public Sub New(Text As String, Optional Title As String = "", Optional HasCancel As Boolean = False)
        InitializeComponent()
        Msg.Text = Text
        Me.Title = Title
        If HasCancel Then CancelButton.Visibility = Windows.Visibility.Visible
    End Sub

    Public Property OKButtonString As String
        Get
            Return OKButtonText.Text
        End Get
        Set(value As String)
            OKButtonText.Text = value
        End Set
    End Property

    Public Property CancelButtonString As String
        Get
            Return CancelButtonText.Text
        End Get
        Set(value As String)
            CancelButtonText.Text = value
        End Set
    End Property

    Private Sub OKClick(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = True
    End Sub

    Private Sub CancelClick(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = False
    End Sub

End Class
