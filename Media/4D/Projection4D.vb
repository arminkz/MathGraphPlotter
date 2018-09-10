Imports System.Windows.Media.Media3D

Public Class Projection4D

    Private m_LookAt As Point4D
    Public Property LookAt As Point4D
        Get
            Return m_LookAt
        End Get
        Set(value As Point4D)
            m_LookAt = value
            InitializeEyeCoorSystem()
        End Set
    End Property

    Private m_From4D As Point4D
    Public Property From4D As Point4D
        Get
            Return m_From4D
        End Get
        Set(value As Point4D)
            m_From4D = value
            InitializeEyeCoorSystem()
        End Set
    End Property

    Public Property UpDir As Vector4D = New Vector4D(0, 1, 0, 0)
    Public Property OverDir As Vector4D = New Vector4D(0, 0, 1, 0)

    Public Property Model4DRadius As Double = 1

    Public Property VAngle4D As Double = Math.PI / 4

    Public Property ProjType As ProjectionType = ProjectionType.Perspective

    Dim A As Vector4D
    Dim B As Vector4D
    Dim C As Vector4D
    Dim D As Vector4D

    Public Enum ProjectionType
        Parallel    'Not Recommended : Ignores The forth dimension.
        Perspective
    End Enum

    Public Sub New(FromP As Point4D, Optional LookAtP As Point4D = Nothing)
        m_From4D = FromP
        m_LookAt = If(LookAtP Is Nothing, New Point4D(0, 0, 0, 0), LookAtP)
        InitializeEyeCoorSystem()
    End Sub

    Public Sub InitializeEyeCoorSystem()
        Dim wD = LookAt - From4D
        wD.Normalize()

        Dim wA = Vector4D.CrossProduct(UpDir, OverDir, wD)
        wA.Normalize()

        Dim wB = Vector4D.CrossProduct(OverDir, wD, wA)
        wB.Normalize()

        Dim wC = Vector4D.CrossProduct(wD, wA, wB)
        wC.Normalize()

        A = wA
        B = wB
        C = wC
        D = wD
    End Sub

    Public Function ProjectTo3D(PL As List(Of Point4D)) As List(Of Point3D)
        Dim RL As New List(Of Point3D)

        Dim S As Double 'Main Div
        Dim T As Double 'Prespective Pre Div

        If ProjType = ProjectionType.Parallel Then
            S = 1 / Model4DRadius
        Else
            T = 1 / Math.Tan(VAngle4D / 2)
        End If

        For Each P As Point4D In PL
            'MsgBox("Input : " & P.ToString)
            Dim V As Vector4D = P - From4D
            If ProjType = ProjectionType.Perspective Then S = T / Vector4D.DotProduct(V, D)

            Dim x As Double = S * Vector4D.DotProduct(V, A)
            Dim y As Double = S * Vector4D.DotProduct(V, B)
            Dim z As Double = S * Vector4D.DotProduct(V, C)
            RL.Add(New Point3D(x, y, z))
            'MsgBox("Output : " & New Point3D(x, y, z).ToString)
        Next

        Return RL
    End Function

    Public Function ProjectTo3D(P As Point4D) As Point3D
        Dim R As New Point3D

        Dim S As Double 'Main Div
        Dim T As Double 'Prespective Pre Div

        If ProjType = ProjectionType.Parallel Then
            S = 1 / Model4DRadius
        Else
            T = 1 / Math.Tan(VAngle4D / 2)
        End If

        Dim V As Vector4D = P - From4D
        If ProjType = ProjectionType.Perspective Then S = T / Vector4D.DotProduct(V, D)

        Dim x As Double = S * Vector4D.DotProduct(V, A)
        Dim y As Double = S * Vector4D.DotProduct(V, B)
        Dim z As Double = S * Vector4D.DotProduct(V, C)
        R = New Point3D(x, y, z)

        Return R
    End Function

End Class
