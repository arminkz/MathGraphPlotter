Imports System.Runtime.InteropServices

Namespace AKP.Input.Joystick
    <StructLayout(LayoutKind.Sequential)> _
    Public Structure XInputVibration
        <MarshalAs(UnmanagedType.I2)> _
        Public LeftMotorSpeed As UShort

        <MarshalAs(UnmanagedType.I2)> _
        Public RightMotorSpeed As UShort
    End Structure
End Namespace