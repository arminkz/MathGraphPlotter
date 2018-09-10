Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Threading

Namespace AKP.Input.Joystick
    Public Class XboxControllerBeta
        Private PlayerIndex As Integer
        Shared KeepRunning As Boolean
        Shared IsRunning As Boolean
        Shared SLock As Object
        Shared PollingThread As Thread

        Private _StopMotorTimerActive As Boolean
        Private _StopMotorTime As DateTime

        'XInputCapabilities _capabilities;

        Private GamepadStatePrev As New XInputState()
        Private GamepadStateCurrent As New XInputState()

        Shared m_UpdateFrequency As Integer
        Shared WaitTime As Integer
        Public Shared Property UpdateFrequency() As Integer
            Get
                Return m_UpdateFrequency
            End Get
            Set(value As Integer)
                m_UpdateFrequency = value
                WaitTime = 1000 \ m_UpdateFrequency
            End Set
        End Property

        Private m_BatteryInformationGamepad As XInputBatteryInformation
        Public Property BatteryInformationGamepad() As XInputBatteryInformation
            Get
                Return m_BatteryInformationGamepad
            End Get
            Friend Set(value As XInputBatteryInformation)
                m_BatteryInformationGamepad = value
            End Set
        End Property

        Private m_BatterInformationHeadset As XInputBatteryInformation
        Public Property BatteryInformationHeadset() As XInputBatteryInformation
            Get
                Return m_BatterInformationHeadset
            End Get
            Friend Set(value As XInputBatteryInformation)
                m_BatterInformationHeadset = value
            End Set
        End Property

        Public Const MAX_CONTROLLER_COUNT As Integer = 4
        Public Const FIRST_CONTROLLER_INDEX As Integer = 0
        Public Const LAST_CONTROLLER_INDEX As Integer = MAX_CONTROLLER_COUNT - 1

        'Shared Controllers() As XboxController

        Public Sub New(JoystickIndex As Integer)
            PlayerIndex = JoystickIndex
            'Controllers = New XboxController(MAX_CONTROLLER_COUNT - 1) {}
            SLock = New Object()
            'For i As Integer = FIRST_CONTROLLER_INDEX To LAST_CONTROLLER_INDEX
            '    Controllers(i) = New XboxController(i)
            'Next
            UpdateFrequency = 25
        End Sub

        Public Event StateChanged As EventHandler(Of XboxControllerStateChangedEventArgs)
        Public Event LeftThumbStickMoving As EventHandler(Of XboxControllerMovmentEventArgs)
        Public Event RightThumbStickMoving As EventHandler(Of XboxControllerMovmentEventArgs)
        Public Event LeftTriggerMoving As EventHandler(Of XboxControllerMovmentEventArgs)
        Public Event RightTriggerMoving As EventHandler(Of XboxControllerMovmentEventArgs)

        'Public Shared Function RetrieveController(Index As Integer) As XboxController
        '    'Return Controllers(Index)
        'End Function

        'Public Sub New(PlayerIndex As Integer)
        '    _PlayerIndex = PlayerIndex
        '    GamepadStatePrev.Copy(GamepadStateCurrent)
        'End Sub

        Public Sub UpdateBatteryState()
            Dim Headset As New XInputBatteryInformation(), Gamepad As New XInputBatteryInformation()

            XInput.XInputGetBatteryInformation(PlayerIndex, CByte(BatteryDeviceType.BATTERY_DEVTYPE_GAMEPAD), Gamepad)
            XInput.XInputGetBatteryInformation(PlayerIndex, CByte(BatteryDeviceType.BATTERY_DEVTYPE_HEADSET), Headset)

            BatteryInformationHeadset = Headset
            BatteryInformationGamepad = Gamepad
        End Sub

        Protected Sub OnStateChanged()
            RaiseEvent StateChanged(Me, New XboxControllerStateChangedEventArgs() With { _
                .CurrentInputState = GamepadStateCurrent, _
                .PreviousInputState = GamepadStatePrev _
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

        Public Function GetCapabilities() As XInputCapabilities
            Dim capabilities As New XInputCapabilities()
            XInput.XInputGetCapabilities(PlayerIndex, XInputConstants.XINPUT_FLAG_GAMEPAD, capabilities)
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
                Dim p As New Point() With { _
                    .X = GamepadStateCurrent.Gamepad.sThumbLX, _
                    .Y = GamepadStateCurrent.Gamepad.sThumbLY _
                }
                Return p
            End Get
        End Property

        Public ReadOnly Property RightThumbStick() As Point
            Get
                Dim p As New Point() With { _
                    .X = GamepadStateCurrent.Gamepad.sThumbRX, _
                    .Y = GamepadStateCurrent.Gamepad.sThumbRY _
                }
                Return p
            End Get
        End Property

#End Region

        Private _IsConnected As Boolean
        Public Property IsConnected() As Boolean
            Get
                Return _IsConnected
            End Get
            Friend Set(value As Boolean)
                _IsConnected = value
            End Set
        End Property

#Region "Polling"
        Private WithEvents PDT As New DispatcherTimer With {.Interval = TimeSpan.FromMilliseconds(25)}
        Public Sub StartPolling()
            If Not IsRunning Then
                PDT.Start()
                IsRunning = True
            End If
            'If Not IsRunning Then
            '    SyncLock SLock
            '        If Not IsRunning Then
            '            PollingThread = New Thread(AddressOf PollerLoop)
            '            PollingThread.Start()
            '        End If
            '    End SyncLock
            'End If
        End Sub

        Public Sub StopPolling()
            If IsRunning Then
                PDT.Stop()
                IsRunning = False
            End If
        End Sub

        'Public Sub PollerLoop()
        '    SyncLock SLock
        '        If IsRunning = True Then
        '            Return
        '        End If
        '        IsRunning = True
        '    End SyncLock
        '    KeepRunning = True
        '    While KeepRunning
        '        'For i As Integer = FIRST_CONTROLLER_INDEX To LAST_CONTROLLER_INDEX
        '        UpdateState()
        '        'Next
        '        Thread.Sleep(m_UpdateFrequency)
        '    End While
        '    SyncLock SLock
        '        IsRunning = False
        '    End SyncLock
        'End Sub

        Public Sub UpdateState() Handles PDT.Tick
            Dim X As New XInputCapabilities()
            Dim Result As Integer = XInput.XInputGetState(PlayerIndex, GamepadStateCurrent)
            IsConnected = (Result = 0)

            If Math.Abs(LeftThumbStick.X - 128) > 1000 Or Math.Abs(LeftThumbStick.Y - 128) > 1000 Then
                OnLeftThumbStickMove(LeftThumbStick.X, LeftThumbStick.Y)
            End If

            If Math.Abs(RightThumbStick.X - 128) > 1000 Or Math.Abs(RightThumbStick.Y - 128) > 1000 Then
                OnRightThumbStickMove(RightThumbStick.X, RightThumbStick.Y)
            End If

            UpdateBatteryState()
            If GamepadStateCurrent.PacketNumber <> GamepadStatePrev.PacketNumber Then
                OnStateChanged()
            End If
            GamepadStatePrev.Copy(GamepadStateCurrent)

            If _StopMotorTimerActive AndAlso (DateTime.Now >= _StopMotorTime) Then
                Dim StopStrength As New XInputVibration() With { _
                    .LeftMotorSpeed = 0, _
                    .RightMotorSpeed = 0 _
                }
                XInput.XInputSetState(PlayerIndex, StopStrength)
            End If
        End Sub
#End Region

#Region "Motor Functions"
        Public Sub Vibrate(LeftMotor As Double, RightMotor As Double)
            Vibrate(LeftMotor, RightMotor, TimeSpan.MinValue)
        End Sub

        Public Sub Vibrate(LeftMotor As Double, RightMotor As Double, Length As TimeSpan)
            LeftMotor = Math.Max(0.0, Math.Min(1.0, LeftMotor))
            RightMotor = Math.Max(0.0, Math.Min(1.0, RightMotor))

            Dim Vibration As New XInputVibration() With { _
                .LeftMotorSpeed = CUShort(Math.Truncate(65535.0 * LeftMotor)), _
                .RightMotorSpeed = CUShort(Math.Truncate(65535.0 * RightMotor)) _
            }
            Vibrate(Vibration, Length)
        End Sub


        Public Sub Vibrate(Strength As XInputVibration)
            _StopMotorTimerActive = False
            XInput.XInputSetState(PlayerIndex, Strength)
        End Sub

        Public Sub Vibrate(strength As XInputVibration, length As TimeSpan)
            XInput.XInputSetState(PlayerIndex, strength)
            If length <> TimeSpan.MinValue Then
                _StopMotorTime = DateTime.Now.Add(length)
                _StopMotorTimerActive = True
            End If
        End Sub

        Public Sub IntroVibrate()
            IVDT.Start()
        End Sub
        Dim IV As Integer = 0
        Private WithEvents IVDT As New DispatcherTimer With {.Interval = TimeSpan.FromMilliseconds(700)}
        Private Sub IVibe() Handles IVDT.Tick
            If IV Mod 2 = 0 Then
                Vibrate(1, 1, TimeSpan.FromMilliseconds(450))
            Else
                Vibrate(0, 1, TimeSpan.FromMilliseconds(450))
            End If
            IV += 1
            If IV = 4 Then
                IV = 0
                IVDT.Stop()
            End If
        End Sub


#End Region

        Public Overrides Function ToString() As String
            Return PlayerIndex.ToString()
        End Function

    End Class
End Namespace