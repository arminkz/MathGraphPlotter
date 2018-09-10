Namespace AKP.Input.Joystick

    <Flags> _
    Public Enum ButtonFlags As Integer
        XINPUT_GAMEPAD_DPAD_UP = &H1
        XINPUT_GAMEPAD_DPAD_DOWN = &H2
        XINPUT_GAMEPAD_DPAD_LEFT = &H4
        XINPUT_GAMEPAD_DPAD_RIGHT = &H8
        XINPUT_GAMEPAD_START = &H10
        XINPUT_GAMEPAD_BACK = &H20
        XINPUT_GAMEPAD_LEFT_THUMB = &H40
        XINPUT_GAMEPAD_RIGHT_THUMB = &H80
        XINPUT_GAMEPAD_LEFT_SHOULDER = &H100
        XINPUT_GAMEPAD_RIGHT_SHOULDER = &H200
        XINPUT_GAMEPAD_A = &H1000
        XINPUT_GAMEPAD_B = &H2000
        XINPUT_GAMEPAD_X = &H4000
        XINPUT_GAMEPAD_Y = &H8000
    End Enum

    <Flags> _
    Public Enum ControllerSubtypes
        XINPUT_DEVSUBTYPE_UNKNOWN = &H0
        XINPUT_DEVSUBTYPE_WHEEL = &H2
        XINPUT_DEVSUBTYPE_ARCADE_STICK = &H3
        XINPUT_DEVSUBTYPE_FLIGHT_STICK = &H4
        XINPUT_DEVSUBTYPE_DANCE_PAD = &H5
        XINPUT_DEVSUBTYPE_GUITAR = &H6
        XINPUT_DEVSUBTYPE_GUITAR_ALTERNATE = &H7
        XINPUT_DEVSUBTYPE_DRUM_KIT = &H8
        XINPUT_DEVSUBTYPE_GUITAR_BASS = &HB
        XINPUT_DEVSUBTYPE_ARCADE_PAD = &H13
    End Enum

    Public Enum BatteryTypes As Byte
        BATTERY_TYPE_DISCONNECTED = &H0
        ' This device is not connected
        BATTERY_TYPE_WIRED = &H1
        ' Wired device, no battery
        BATTERY_TYPE_ALKALINE = &H2
        ' Alkaline battery source
        BATTERY_TYPE_NIMH = &H3
        ' Nickel Metal Hydride battery source
        BATTERY_TYPE_UNKNOWN = &HFF
        ' Cannot determine the battery type
    End Enum

    ' These are only valid for wireless, connected devices, with known battery types
    ' The amount of use time remaining depends on the type of device.
    Public Enum BatteryLevel As Byte
        BATTERY_LEVEL_EMPTY = &H0
        BATTERY_LEVEL_LOW = &H1
        BATTERY_LEVEL_MEDIUM = &H2
        BATTERY_LEVEL_FULL = &H3
    End Enum

    Public Enum BatteryDeviceType As Byte
        BATTERY_DEVTYPE_GAMEPAD = &H0
        BATTERY_DEVTYPE_HEADSET = &H1
    End Enum

    Public Class XInputConstants
        Public Const XINPUT_DEVTYPE_GAMEPAD As Integer = &H1

        '
        ' Device subtypes available in XINPUT_CAPABILITIES
        '
        Public Const XINPUT_DEVSUBTYPE_GAMEPAD As Integer = &H1

        '
        ' Flags for XINPUT_CAPABILITIES
        '
        Public Enum CapabilityFlags
            XINPUT_CAPS_VOICE_SUPPORTED = &H4
            'For Windows 8 only
            XINPUT_CAPS_FFB_SUPPORTED = &H1
            XINPUT_CAPS_WIRELESS = &H2
            XINPUT_CAPS_PMD_SUPPORTED = &H8
            XINPUT_CAPS_NO_NAVIGATION = &H10
        End Enum
        '
        ' Constants for gamepad buttons
        '

        '
        ' Gamepad thresholds
        '
        Public Const XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE As Integer = 7849
        Public Const XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE As Integer = 8689
        Public Const XINPUT_GAMEPAD_TRIGGER_THRESHOLD As Integer = 30

        '
        ' Flags to pass to XInputGetCapabilities
        '
        Public Const XINPUT_FLAG_GAMEPAD As Integer = &H1


    End Class
End Namespace