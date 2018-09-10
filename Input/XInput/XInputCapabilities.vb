Imports System.Runtime.InteropServices

Namespace AKP.Input.Joystick
    <StructLayout(LayoutKind.Explicit)> _
    Public Structure XInputCapabilities
        <MarshalAs(UnmanagedType.I1)> _
        <FieldOffset(0)> _
        Private Type As Byte

        <MarshalAs(UnmanagedType.I1)> _
        <FieldOffset(1)> _
        Public SubType As Byte

        <MarshalAs(UnmanagedType.I2)> _
        <FieldOffset(2)> _
        Public Flags As Short

        <FieldOffset(4)> _
        Public Gamepad As XInputGamepad

        <FieldOffset(16)> _
        Public Vibration As XInputVibration
    End Structure
End Namespace