Imports System.Runtime.InteropServices

Namespace AKP.Input.Joystick
    <StructLayout(LayoutKind.Explicit)> _
    Public Structure XInputKeystroke
        <MarshalAs(UnmanagedType.I2)> _
        <FieldOffset(0)> _
        Public VirtualKey As Short

        <MarshalAs(UnmanagedType.I2)> _
        <FieldOffset(2)> _
        Public Unicode As Char

        <MarshalAs(UnmanagedType.I2)> _
        <FieldOffset(4)> _
        Public Flags As Short

        <MarshalAs(UnmanagedType.I2)> _
        <FieldOffset(5)> _
        Public UserIndex As Byte

        <MarshalAs(UnmanagedType.I1)> _
        <FieldOffset(6)> _
        Public HidCode As Byte
    End Structure
End Namespace