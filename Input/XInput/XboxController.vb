Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

Namespace AKP.Input.Joystick
    Public Class XboxController
        Private _playerIndex As Integer
        Shared keepRunning As Boolean
        Shared m_updateFrequency As Integer
        Shared waitTime As Integer
        Shared isRunning As Boolean
        Shared [SyncLock] As Object
        Shared pollingThread As Thread

        Private _stopMotorTimerActive As Boolean
        Private _stopMotorTime As DateTime
        Private _batteryInformationGamepad As XInputBatteryInformation
        Private _batterInformationHeadset As XInputBatteryInformation
        'XInputCapabilities _capabilities;

        Private GamepadStatePrev As New XInputState()
        Private GamepadStateCurrent As New XInputState()

        Public Shared Property UpdateFrequency() As Integer
            Get
                Return m_updateFrequency
            End Get
            Set(value As Integer)
                m_updateFrequency = value
                waitTime = 1000 \ m_updateFrequency
            End Set
        End Property

        Public Property BatteryInformationGamepad() As XInputBatteryInformation
            Get
                Return _batteryInformationGamepad
            End Get
            Friend Set(value As XInputBatteryInformation)
                _batteryInformationGamepad = value
            End Set
        End Property

        Public Property BatteryInformationHeadset() As XInputBatteryInformation
            Get
                Return _batterInformationHeadset
            End Get
            Friend Set(value As XInputBatteryInformation)
                _batterInformationHeadset = value
            End Set
        End Property

        Public Const MAX_CONTROLLER_COUNT As Integer = 4
        Public Const FIRST_CONTROLLER_INDEX As Integer = 0
        Public Const LAST_CONTROLLER_INDEX As Integer = MAX_CONTROLLER_COUNT - 1

        Shared Controllers As XboxController()


        Shared Sub New()
            Controllers = New XboxController(MAX_CONTROLLER_COUNT - 1) {}
            [SyncLock] = New Object()
            For i As Integer = FIRST_CONTROLLER_INDEX To LAST_CONTROLLER_INDEX
                Controllers(i) = New XboxController(i)
            Next
            UpdateFrequency = 25
        End Sub

        Public Event StateChanged As EventHandler(Of XboxControllerStateChangedEventArgs)
        Public Event LeftThumbStickMoving As EventHandler(Of XboxControllerMovmentEventArgs)
        Public Event RightThumbStickMoving As EventHandler(Of XboxControllerMovmentEventArgs)
        Public Event LeftTriggerMoving As EventHandler(Of XboxControllerMovmentEventArgs)
        Public Event RightTriggerMoving As EventHandler(Of XboxControllerMovmentEventArgs)

        Public Shared Function RetrieveController(index As Integer) As XboxController
            Return Controllers(index)
        End Function

        Private Sub New(playerIndex As Integer)
            _playerIndex = playerIndex
            gamepadStatePrev.Copy(gamepadStateCurrent)
        End Sub

        Public Sub UpdateBatteryState()
            Dim headset As New XInputBatteryInformation(), gamepad As New XInputBatteryInformation()

            XInput.XInputGetBatteryInformation(_playerIndex, CByte(BatteryDeviceType.BATTERY_DEVTYPE_GAMEPAD), gamepad)
            XInput.XInputGetBatteryInformation(_playerIndex, CByte(BatteryDeviceType.BATTERY_DEVTYPE_HEADSET), headset)

            BatteryInformationHeadset = headset
            BatteryInformationGamepad = gamepad
        End Sub

        Protected Sub OnStateChanged()
            RaiseEvent StateChanged(Me, New XboxControllerStateChangedEventArgs() With { _
                .CurrentInputState = gamepadStateCurrent, _
                .PreviousInputState = gamepadStatePrev _
            })
        End Sub

        Protected Sub OnLeftThumbStickMove(Cx As Integer, Cy As Integer)
            RaiseEvent LeftThumbStickMoving(Me, New XboxControllerMovmentEventArgs() With { _
                .X = Cx, _
                .Y = Cy _
            })
        End Sub

        Protected Sub OnRightThumbStickMove(Cx As Integer, Cy As Integer)
            RaiseEvent RightThumbStickMoving(Me, New XboxControllerMovmentEventArgs() With { _
                .X = Cx, _
                .Y = Cy _
            })
        End Sub

        'Protected Sub OnLeftTriggerMove()
        '    RaiseEvent StateChanged(Me, New XboxControllerStateChangedEventArgs() With { _
        '        .CurrentInputState = GamepadStateCurrent, _
        '        .PreviousInputState = GamepadStatePrev _
        '    })
        'End Sub

        'Protected Sub OnRightTriggerMove()
        '    RaiseEvent StateChanged(Me, New XboxControllerStateChangedEventArgs() With { _
        '        .CurrentInputState = GamepadStateCurrent, _
        '        .PreviousInputState = GamepadStatePrev _
        '    })
        'End Sub

        Public Function GetCapabilities() As XInputCapabilities
            Dim capabilities As New XInputCapabilities()
            XInput.XInputGetCapabilities(_playerIndex, XInputConstants.XINPUT_FLAG_GAMEPAD, capabilities)
            Return capabilities
        End Function


#Region "Digital Button States"
        Public ReadOnly Property IsDPadUpPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_DPAD_UP))
            End Get
        End Property

        Public ReadOnly Property IsDPadDownPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_DPAD_DOWN))
            End Get
        End Property

        Public ReadOnly Property IsDPadLeftPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_DPAD_LEFT))
            End Get
        End Property

        Public ReadOnly Property IsDPadRightPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_DPAD_RIGHT))
            End Get
        End Property

        Public ReadOnly Property IsAPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_A))
            End Get
        End Property

        Public ReadOnly Property IsBPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_B))
            End Get
        End Property

        Public ReadOnly Property IsXPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_X))
            End Get
        End Property

        Public ReadOnly Property IsYPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_Y))
            End Get
        End Property


        Public ReadOnly Property IsBackPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_BACK))
            End Get
        End Property


        Public ReadOnly Property IsStartPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_START))
            End Get
        End Property


        Public ReadOnly Property IsLeftShoulderPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER))
            End Get
        End Property


        Public ReadOnly Property IsRightShoulderPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER))
            End Get
        End Property

        Public ReadOnly Property IsLeftStickPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_LEFT_THUMB))
            End Get
        End Property

        Public ReadOnly Property IsRightStickPressed() As Boolean
            Get
                Return GamepadStateCurrent.Gamepad.IsButtonPressed(CInt(ButtonFlags.XINPUT_GAMEPAD_RIGHT_THUMB))
            End Get
        End Property
#End Region

#Region "Analogue Input States"
        Public ReadOnly Property LeftTrigger() As Integer
            Get
                Return CInt(GamepadStateCurrent.Gamepad.bLeftTrigger)
            End Get
        End Property

        Public ReadOnly Property RightTrigger() As Integer
            Get
                Return CInt(GamepadStateCurrent.Gamepad.bRightTrigger)
            End Get
        End Property

        Public ReadOnly Property LeftThumbStick() As Point
            Get
                Dim P As New Point() With { _
                    .X = GamepadStateCurrent.Gamepad.sThumbLX, _
                    .Y = GamepadStateCurrent.Gamepad.sThumbLY _
                }
                Return P
            End Get
        End Property

        Public ReadOnly Property RightThumbStick() As Point
            Get
                Dim P As New Point() With { _
                    .X = GamepadStateCurrent.Gamepad.sThumbRX, _
                    .Y = GamepadStateCurrent.Gamepad.sThumbRY _
                }
                Return P
            End Get
        End Property

#End Region

        Private m_IsConnected As Boolean
        Public Property IsConnected() As Boolean
            Get
                Return m_IsConnected
            End Get
            Friend Set(value As Boolean)
                m_IsConnected = value
            End Set
        End Property

#Region "Polling"
        Public Shared Sub StartPolling()
            If Not isRunning Then
                SyncLock [SyncLock]
                    If Not isRunning Then
                        pollingThread = New Thread(AddressOf PollerLoop)
                        pollingThread.Start()
                    End If
                End SyncLock
            End If
        End Sub

        Public Shared Sub StopPolling()
            If isRunning Then
                keepRunning = False
            End If
        End Sub

        Private Shared Sub PollerLoop()
            SyncLock [SyncLock]
                If isRunning = True Then
                    Return
                End If
                isRunning = True
            End SyncLock
            keepRunning = True
            While keepRunning
                For i As Integer = FIRST_CONTROLLER_INDEX To LAST_CONTROLLER_INDEX
                    Controllers(i).UpdateState()
                Next
                Thread.Sleep(m_updateFrequency)
            End While
            SyncLock [SyncLock]
                isRunning = False
            End SyncLock
        End Sub

        Public Sub UpdateState()
            Dim X As New XInputCapabilities()
            Dim Result As Integer = XInput.XInputGetState(_playerIndex, GamepadStateCurrent)
            IsConnected = (Result = 0)

            If Math.Abs(LeftThumbStick.X - 128) > 1000 Or Math.Abs(LeftThumbStick.Y - 128) > 1000 Then
                OnLeftThumbStickMove(LeftThumbStick.X, LeftThumbStick.Y)
            End If

            If Math.Abs(RightThumbStick.X - 128) > 1000 Or Math.Abs(RightThumbStick.Y - 128) > 1000 Then
                OnRightThumbStickMove(LeftThumbStick.X, LeftThumbStick.Y)
            End If

            'If Math.Abs(LeftTrigger - 128) > 1000 Or Math.Abs(LeftThumbStick.Y - 128) > 1000 Then
            '    OnLeftThumbStickMove()
            'End If

            UpdateBatteryState()
            If GamepadStateCurrent.PacketNumber <> GamepadStatePrev.PacketNumber Then
                OnStateChanged()
            End If
            GamepadStatePrev.Copy(GamepadStateCurrent)

            If _stopMotorTimerActive AndAlso (DateTime.Now >= _stopMotorTime) Then
                Dim StopStrength As New XInputVibration() With { _
                    .LeftMotorSpeed = 0, _
                    .RightMotorSpeed = 0 _
                }
                XInput.XInputSetState(_playerIndex, StopStrength)
            End If
        End Sub
#End Region

#Region "Motor Functions"
        Public Sub Vibrate(leftMotor As Double, rightMotor As Double)
            Vibrate(leftMotor, rightMotor, TimeSpan.MinValue)
        End Sub

        Public Sub Vibrate(leftMotor As Double, rightMotor As Double, length As TimeSpan)
            leftMotor = Math.Max(0.0, Math.Min(1.0, leftMotor))
            rightMotor = Math.Max(0.0, Math.Min(1.0, rightMotor))

            Dim vibration As New XInputVibration() With { _
                .LeftMotorSpeed = CUShort(Math.Truncate(65535.0 * leftMotor)), _
                .RightMotorSpeed = CUShort(Math.Truncate(65535.0 * rightMotor)) _
            }
            Vibrate(vibration, length)
        End Sub


        Public Sub Vibrate(strength As XInputVibration)
            _stopMotorTimerActive = False
            XInput.XInputSetState(_playerIndex, strength)
        End Sub

        Public Sub Vibrate(strength As XInputVibration, length As TimeSpan)
            XInput.XInputSetState(_playerIndex, strength)
            If length <> TimeSpan.MinValue Then
                _stopMotorTime = DateTime.Now.Add(length)
                _stopMotorTimerActive = True
            End If
        End Sub

        Public Sub IntroVibrate()
            Dim IV As Integer = 0
            While IV < 4
                If IV Mod 2 = 0 Then
                    Vibrate(1, 1, TimeSpan.FromMilliseconds(450))
                Else
                    Vibrate(0, 1, TimeSpan.FromMilliseconds(450))
                End If
                IV += 1
                Thread.Sleep(500)
            End While
            'Dim IVT As New Thread(AddressOf IVibe)
            'IVT.Start()
        End Sub
        Private Sub IVibe()

        End Sub

#End Region

        Public Overrides Function ToString() As String
            Return _playerIndex.ToString()
        End Function

    End Class
End Namespace