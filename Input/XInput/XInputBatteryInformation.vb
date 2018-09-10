Imports System.Runtime.InteropServices

Namespace AKP.Input.Joystick
    <StructLayout(LayoutKind.Explicit)> _
    Public Structure XInputBatteryInformation
        <MarshalAs(UnmanagedType.I1)> _
        <FieldOffset(0)> _
        Public BatteryType As Byte

        <MarshalAs(UnmanagedType.I1)> _
        <FieldOffset(1)> _
        Public BatteryLevel As Byte
    End Structure
End Namespace