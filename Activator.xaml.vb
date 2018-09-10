Imports System.Threading
Imports System.Net
Imports AKP_Math_Graph_Plotter.AKP.Security.Encryption

Public Class Activator

    Private User As String
    Private Pass As String
    Dim PC_MachineName As String = Environment.MachineName
    Dim PC_UserName As String = Environment.UserName
    Private ServerProblem As Boolean


    Private Sub Window_Closed(sender As Object, e As EventArgs)
        My.Application.Shutdown()
    End Sub
    Private Sub CloseWindow()
        My.Application.Shutdown()
    End Sub

    Public Sub StartConnection()
        Dim ThreadC As New Thread(New ThreadStart(AddressOf SendToServer))
        ThreadC.Start()
    End Sub

    Public Sub SendToServer()
        Dim TimeDate As String = DateTime.Now.ToString

        Dim SubText1 As String = TimeDate + " : " _
                                 + vbCrLf + "MachineUserName : " + PC_UserName _
                                 + vbCrLf + "MachineName : " + PC_MachineName _
                                 + vbCrLf + "Username : " + User _
                                 + vbCrLf + "Password : " + Pass

        Dim AKP_Check As New System.Net.WebClient
        Try
            Try
                Dim AKP_Submit As WebRequest = WebRequest.Create(My.Settings.SubmitURI & ".php?w=" & SubText1)
                'AKP_Submit.Headers.Add("Referer", My.Settings.SubmitURI)
                'AKP_Submit.Headers.Add("Accept: text/html, application/xhtml+xml, */*")
                'AKP_Submit.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)")
                AKP_Submit.GetResponse()
            Catch ex As Exception
                'MsgBox(ex.Message)
            End Try
        Catch
            ServerProblem = True
        End Try
    End Sub

    Private Sub ActivateProgram()
        User = UserText.Text
        Pass = PassText.Password

        StatusText.Visibility = Windows.Visibility.Visible
        StartConnection()

        Dim Key As String = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\AKP", "MGP", Nothing)

        If Key = Nothing Then
            If User.ToLower = "madani" And Pass.ToLower = "madani" Then

                Dim DeviceID As String = PC_MachineName + "%%" + PC_UserName + "%%" + User
                Dim WKey As String = AES_Encrypt(DeviceID, "akz7896321")

                My.Computer.Registry.CurrentUser.CreateSubKey("AKP")
                My.Computer.Registry.SetValue("HKEY_CURRENT_USER\AKP", "MGP", WKey)

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location)
                Application.Current.Shutdown()

            ElseIf User.ToLower = "user" And Pass.ToLower = "user" Then

                Dim DeviceID As String = PC_MachineName + "%%" + PC_UserName + "%%" + User
                Dim WKey As String = AES_Encrypt(DeviceID, "akz7896321")

                My.Computer.Registry.CurrentUser.CreateSubKey("AKP")
                My.Computer.Registry.SetValue("HKEY_CURRENT_USER\AKP", "MGP", WKey)

                'Restart App
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location)
                Application.Current.Shutdown()
            End If
        Else
            StatusText.Text = "برنامه هم اکنون فعال است "
        End If
    End Sub

End Class
