Imports System.Runtime.InteropServices

Namespace AKP.Input.Joystick
    <StructLayout(LayoutKind.Explicit)> _
    Public Structure XInputGamepad
        <MarshalAs(UnmanagedType.I2)> _
        <FieldOffset(0)> _
        Public wButtons As Short

        <MarshalAs(UnmanagedType.I1)> _
        <FieldOffset(2)> _
        Public bLeftTrigger As Byte

        <MarshalAs(UnmanagedType.I1)> _
        <FieldOffset(3)> _
        Public bRightTrigger As Byte

        <MarshalAs(UnmanagedType.I2)> _
        <FieldOffset(4)> _
        Public sThumbLX As Short

        <MarshalAs(UnmanagedType.I2)> _
        <FieldOffset(6)> _
        Public sThumbLY As Short

        <MarshalAs(UnmanagedType.I2)> _
        <FieldOffset(8)> _
        Public sThumbRX As Short

        <MarshalAs(UnmanagedType.I2)> _
        <FieldOffset(10)> _
        Public sThumbRY As Short


        Public Function IsButtonPressed(ButtonFlags As Integer) As Boolean
            Return (wButtons And ButtonFlags) = ButtonFlags
        End Function

        Public Function IsButtonPresent(ButtonFlags As Integer) As Boolean
            Return (wButtons And ButtonFlags) = ButtonFlags
        End Function

        Public Sub Copy(Source As XInputGamepad)
            sThumbLX = Source.sThumbLX
            sThumbLY = Source.sThumbLY
            sThumbRX = Source.sThumbRX
            sThumbRY = Source.sThumbRY
            bLeftTrigger = Source.bLeftTrigger
            bRightTrigger = Source.bRightTrigger
            wButtons = Source.wButtons
        End Sub

        Public Overrides Function Equals(Obj As Object) As Boolean
            If Not (TypeOf Obj Is XInputGamepad) Then
                Return False
            End If
            Dim Source As XInputGamepad = CType(Obj, XInputGamepad)
            Return ((sThumbLX = Source.sThumbLX) AndAlso (sThumbLY = Source.sThumbLY) AndAlso (sThumbRX = Source.sThumbRX) AndAlso (sThumbRY = Source.sThumbRY) AndAlso (bLeftTrigger = Source.bLeftTrigger) AndAlso (bRightTrigger = Source.bRightTrigger) AndAlso (wButtons = Source.wButtons))
        End Function
    End Structure
End Namespace