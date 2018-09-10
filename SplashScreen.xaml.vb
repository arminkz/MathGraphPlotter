Imports System.Windows.Threading
Imports System.Windows.Media.Animation
Imports System.ComponentModel
Imports System.Net
Imports AKP_Math_Graph_Plotter.AKP.Security.Encryption
Imports AKP_Math_Graph_Plotter.AKP.Notifications

Public Class SplashScreen

    Public Property TBC As Byte
    Public WithEvents ErrorTimer As New DispatcherTimer

    Public WithEvents DelayTimer As New DispatcherTimer
    Dim DelayToDo As DelayJobs

    Public WithEvents DotBlinkTimer As New DispatcherTimer

    Dim IsConnected As Boolean
    Dim OfflineVersion As System.Version = My.Application.Info.Version
    Dim OnlineVersion As System.Version

    Dim ActError As Boolean = False

    Dim WithEvents AKPupdate As New System.Net.WebClient

    Dim OpString As String = "چک کردن اتصال به اینترنت"

    Private Enum DelayJobs
        CheckInternet
        CheckUpdate
        LaunchApp
    End Enum

    Public Sub StartApp(sender As Object, e As RoutedEventArgs)

#If DEBUG Then
        BottomText.FontFamily = New FontFamily("Segou UI")
        BottomText.FontSize = 12
        BottomText.Text = "Debug Build " & My.Settings.TimesLaunched
#End If

        Me.TaskbarItemInfo.ProgressValue = 1
        Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.Paused

        Dim UTF8 As New System.Text.UTF8Encoding
        AKPupdate.Encoding = UTF8
        AKPupdate.Headers.Add("Referer", My.Settings.UpdateURI)
        AKPupdate.Headers.Add("Accept: text/html, application/xhtml+xml, */*")
        AKPupdate.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)")

        ErrorTimer.Interval = TimeSpan.FromSeconds(0.7)

        DotBlinkTimer.Interval = TimeSpan.FromSeconds(0.3)

        DelayTimer.Interval = TimeSpan.FromSeconds(5)

        'DLL Check
        'Dim SystemInteractivityDll As String = AppDomain.CurrentDomain.BaseDirectory & "System.Windows.Interactivity.dll"
        'If IO.File.Exists(SystemInteractivityDll) = False Then
        '    IncompleteFileError()
        '    ErrorTimer.Start()
        '    Return
        'End If

        'Product Activation
        Dim Key As String = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\AKP", "MGP", Nothing)
        If Key <> Nothing Then
            Dim DecKey As String = AES_Decrypt(Key, "akz7896321")
            Dim IDs() As String
            IDs = DecKey.Split("%%")

            If IDs(0) = Environment.MachineName And IDs(2) = Environment.UserName Then
                Dim LCFU As Date = My.Settings.CheckForUpdateLast
                Dim Today As Date = Date.Today
                Dim SSDate As System.TimeSpan = Today - LCFU

                If My.Settings.UpdateEnabled AndAlso SSDate > TimeSpan.FromDays(My.Settings.CheckForUpdateDays) Then
                    DotBlinkTimer.Start()
                    DelayToDo = DelayJobs.CheckInternet
                    DelayTimer.Start()
                    My.Settings.CheckForUpdateLast = Today
                    My.Settings.Save()
                Else
                    DelayToDo = DelayJobs.LaunchApp
                    DelayTimer.Start()
                End If
            Else
                ActivationError()
                ErrorTimer.Start()
            End If
        Else
            ActivationError()
            ErrorTimer.Start()
        End If
    End Sub

    Private Sub LaunchMain()
        Me.Hide()
        Dim MainForm As New MainWindow
        My.Application.MainWindow = MainForm
        MainForm.Show()
        My.Settings.TimesLaunched += 1
        My.Settings.Save()
        Me.Close()
    End Sub

    Public Sub DoWithDelay() Handles DelayTimer.Tick
        DelayTimer.Stop()
        Select Case DelayToDo
            Case DelayJobs.CheckInternet
                CheckInternet()
            Case DelayJobs.CheckUpdate
                OpString = "اتصال به سرور"
                MsgText.Text = "اتصال به سرور"
                TBC = 0
                CheckUpdate()
            Case DelayJobs.LaunchApp
                LaunchMain()
        End Select
    End Sub

    Public Sub CheckInternet()
        'برقراری ارتباط با اینترنت
        Dim NetworkCheck As Boolean = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()
        Dim InternetCheck As Boolean = NetworkCheck

        If NetworkCheck Then
            Dim Req As WebRequest = WebRequest.Create("http://www.google.com/")
            Dim Resp As WebResponse
            Try
                Resp = Req.GetResponse()
                Resp.Close()
                Req = Nothing
                InternetCheck = True
            Catch ex As Exception
                Req = Nothing
                InternetCheck = False
            End Try
        End If

        If InternetCheck Then
            DelayToDo = DelayJobs.CheckUpdate
            DelayTimer.Start()
        Else
            MsgText.Text = "ارتباط اینترنتی وجود ندارد"
            'Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.Error
            DotBlinkTimer.Stop()
            DelayToDo = DelayJobs.LaunchApp
            DelayTimer.Start()
        End If

    End Sub

    Private Sub CheckUpdate()
        Dim Dir As New Uri(My.Settings.UpdateURI)
        AKPupdate.DownloadStringAsync(Dir)
    End Sub

    Private Sub CheckUpdateCompleted(sender As Object, e As DownloadStringCompletedEventArgs) Handles AKPupdate.DownloadStringCompleted
        Dim ServerString() As String
        Try
            ServerString = e.Result.Split(vbCrLf)
        Catch
            ServerConnectionError()
        End Try
        If e.Error IsNot Nothing Or e.Cancelled = True Then
            ServerConnectionError()
        ElseIf e.Result IsNot Nothing Then
            OnlineVersion = New System.Version(ServerString(0))
            TechNote.Text = "نسخه آنلاین :" + OnlineVersion.ToString
            If OnlineVersion > OfflineVersion Then
                ''
                ''
                'Some Stuff To Update The App (Comming Soon)
            Else
                If ServerString.Length > 1 AndAlso ServerString(1).Substring(1) <> "" Then
                    MsgText.Text = "پیام از سرور : " + ServerString(1).Substring(1)
                Else
                    MsgText.Text = ""
                End If
                Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.Normal
                DotBlinkTimer.Stop()
                DelayToDo = DelayJobs.LaunchApp
                DelayTimer.Start()
            End If
        Else
            ServerDownError()
        End If
    End Sub

    Private Sub ActivationError()
        MsgText.Visibility = Windows.Visibility.Hidden
        MsgText.Foreground = Brushes.Red
        MsgText.FontWeight = FontWeights.Bold
        MsgText.Text = "مشکل در فعال سازی نرم افزار"
        ActError = True
    End Sub

    Private Sub IncompleteFileError()
        MsgText.Visibility = Windows.Visibility.Hidden
        MsgText.Foreground = Brushes.Red
        MsgText.FontWeight = FontWeights.Bold
        MsgText.Text = "فایل های نرم افزار ناقص است."
    End Sub

    Private Sub ServerConnectionError()
        DotBlinkTimer.Stop()
        MsgText.Text = "خطا در اتصال به سرور"
        Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.Error
        DelayToDo = DelayJobs.LaunchApp
        DelayTimer.Start()
    End Sub

    Private Sub ServerDownError()
        MsgText.Text = "سرور برنامه فعلا غیرفعال می باشد"
        Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.Error
        DotBlinkTimer.Stop()
        DelayToDo = DelayJobs.LaunchApp
        DelayTimer.Start()
    End Sub

    Private Sub ErrorBlink(sender As Object, e As EventArgs) Handles ErrorTimer.Tick
        Select Case TBC
            Case Is = 0
                MsgText.Visibility = Windows.Visibility.Visible
                Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.Error
            Case Is = 1
                MsgText.Visibility = Windows.Visibility.Hidden
                Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.None
            Case Is = 2
                MsgText.Visibility = Windows.Visibility.Visible
                Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.Error
            Case Is = 3
                MsgText.Visibility = Windows.Visibility.Hidden
                Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.None
            Case Is = 4
                MsgText.Visibility = Windows.Visibility.Visible
                Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.Error
            Case Is = 5
                MsgText.Visibility = Windows.Visibility.Hidden
                Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.None
            Case Is = 6
                MsgText.Visibility = Windows.Visibility.Visible
                Me.TaskbarItemInfo.ProgressState = Shell.TaskbarItemProgressState.Error
                Me.Hide()
                If ActError Then
                    Dim Act As New Activator
                    Act.Show()
                Else
                    Application.Current.Shutdown()
                End If
                ErrorTimer.Stop()
        End Select
        TBC += 1
    End Sub

    Private Sub DotBlink(sender As Object, e As EventArgs) Handles DotBlinkTimer.Tick
        Select Case TBC
            Case Is = 0
                MsgText.Text = OpString
            Case Is = 1
                MsgText.Text = OpString + "."
            Case Is = 2
                MsgText.Text = OpString + ".."
            Case Is = 3
                MsgText.Text = OpString + "..."
                TBC = 0
                Return
        End Select
        TBC += 1
    End Sub



End Class
