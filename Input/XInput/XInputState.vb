Imports System.Runtime.InteropServices

Namespace AKP.Input.Joystick
    <StructLayout(LayoutKind.Explicit)> _
    Public Structure XInputState
        <FieldOffset(0)> _
        Public PacketNumber As Integer

        <FieldOffset(4)> _
        Public Gamepad As XInputGamepad

        Public Sub Copy(Source As XInputState)
            PacketNumber = Source.PacketNumber
            Gamepad.Copy(Source.Gamepad)
        End Sub

        Public Overrides Function Equals(Obj As Object) As Boolean
            If (Obj Is Nothing) OrElse (Not (TypeOf Obj Is XInputState)) Then
                Return False
            End If
            Dim Source As XInputState = CType(Obj, XInputState)

            Return ((PacketNumber = Source.PacketNumber) AndAlso (Gamepad.Equals(Source.Gamepad)))
        End Function
    End Structure
End Namespace