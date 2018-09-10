Imports System.Runtime.InteropServices

Namespace AKP.Input.Joystick
    Public NotInheritable Class XInput
        Private Sub New()
        End Sub
#If WINDOWS7 Then
	' [in] Index of the gamer associated with the device
	<DllImport("xinput9_1_0.dll")> _
	Public Shared Function XInputGetState(dwUserIndex As Integer, ByRef pState As XInputState) As Integer
		' [out] Receives the current state
	End Function

	' [in] Index of the gamer associated with the device
	<DllImport("xinput9_1_0.dll")> _
	Public Shared Function XInputSetState(dwUserIndex As Integer, ByRef pVibration As XInputVibration) As Integer
		' [in, out] The vibration information to send to the controller
	End Function

	' [in] Index of the gamer associated with the device
	' [in] Input flags that identify the device type
	<DllImport("xinput9_1_0.dll")> _
	Public Shared Function XInputGetCapabilities(dwUserIndex As Integer, dwFlags As Integer, ByRef pCapabilities As XInputCapabilities) As Integer
		' [out] Receives the capabilities
	End Function


	'this function is not available prior to Windows 8
	' Index of the gamer associated with the device
	' Which device on this user index
	Public Shared Function XInputGetBatteryInformation(dwUserIndex As Integer, devType As Byte, ByRef pBatteryInformation As XInputBatteryInformation) As Integer
	' Contains the level and types of batteries
		Return 0
	End Function

	'this function is not available prior to Windows 8
	' Index of the gamer associated with the device
	' Reserved for future use
	Public Shared Function XInputGetKeystroke(dwUserIndex As Integer, dwReserved As Integer, ByRef pKeystroke As XInputKeystroke) As Integer
	' Pointer to an XINPUT_KEYSTROKE structure that receives an input event.
		Return 0
	End Function
#Else
        ' [in] Index of the gamer associated with the device
        <DllImport("xinput1_4.dll")> _
        Public Shared Function XInputGetState(dwUserIndex As Integer, ByRef pState As XInputState) As Integer
            ' [out] Receives the current state
        End Function

        ' [in] Index of the gamer associated with the device
        <DllImport("xinput1_4.dll")> _
        Public Shared Function XInputSetState(dwUserIndex As Integer, ByRef pVibration As XInputVibration) As Integer
            ' [in, out] The vibration information to send to the controller
        End Function

        ' [in] Index of the gamer associated with the device
        ' [in] Input flags that identify the device type
        <DllImport("xinput1_4.dll")> _
        Public Shared Function XInputGetCapabilities(dwUserIndex As Integer, dwFlags As Integer, ByRef pCapabilities As XInputCapabilities) As Integer
            ' [out] Receives the capabilities
        End Function

        ' Index of the gamer associated with the device
        ' Which device on this user index
        <DllImport("xinput1_4.dll")> _
        Public Shared Function XInputGetBatteryInformation(dwUserIndex As Integer, devType As Byte, ByRef pBatteryInformation As XInputBatteryInformation) As Integer
            ' Contains the level and types of batteries
        End Function

        ' Index of the gamer associated with the device
        ' Reserved for future use
        <DllImport("xinput1_4.dll")> _
        Public Shared Function XInputGetKeystroke(dwUserIndex As Integer, dwReserved As Integer, ByRef pKeystroke As XInputKeystroke) As Integer
            ' Pointer to an XINPUT_KEYSTROKE structure that receives an input event.
        End Function
#End If
    End Class
End Namespace