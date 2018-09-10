Imports System.Runtime.InteropServices

Namespace AKP.Notifications
    Module AKPDesktopNotification
        Public Const WM_SETTEXT As UInt32 = &HC

        <DllImport("user32.dll")> _
        Public Function SendMessage(ByVal hwnd As Integer, ByVal msg As UInteger, ByVal wparam As Integer, ByVal lparam As String) As Integer
        End Function

        <DllImport("user32.dll")> _
        Public Function FindWindow(ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
        End Function

        Public Sub AKPDesktop_ToastNotification(txt As String)
            Dim ReceiverHwnd As IntPtr = FindWindow(Nothing, "AKP Desktop")
            SendMessage(ReceiverHwnd, WM_SETTEXT, Nothing, txt)
        End Sub

    End Module
End Namespace