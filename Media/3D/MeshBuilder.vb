Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class MeshBuilder

#Region "Constructor"

    Private m_Normals As Vector3DCollection
    Private m_Positions As Point3DCollection
    Private m_TextureCoordinates As PointCollection
    Private m_TriangleIndices As Int32Collection

    ' مختصات 3 بعدی
    Public ReadOnly Property Positions() As Point3DCollection
        Get
            Return Me.m_Positions
        End Get
    End Property
    ' رئوس وجه
    Public ReadOnly Property TriangleIndices() As Int32Collection
        Get
            Return Me.m_TriangleIndices
        End Get
    End Property
    'بردار نرمال
    Public ReadOnly Property Normals() As Vector3DCollection
        Get
            Return Me.m_Normals
        End Get
    End Property
    ' مختصات تکسچر
    Public ReadOnly Property TextureCoordinates() As PointCollection
        Get
            Return Me.m_TextureCoordinates
        End Get
    End Property
    'ایا بردار نرمال تولید کند
    Public Property CreateNormals() As Boolean
        Get
            Return Me.m_Normals IsNot Nothing
        End Get

        Set(value As Boolean)
            If value AndAlso Me.m_Normals Is Nothing Then
                Me.m_Normals = New Vector3DCollection()
            End If

            If Not value Then
                Me.m_Normals = Nothing
            End If
        End Set
    End Property
    'ایا مختصات تکسچر تولید کند
    Public Property CreateTextureCoordinates() As Boolean
        Get
            Return Me.m_TextureCoordinates IsNot Nothing
        End Get

        Set(value As Boolean)
            If value AndAlso Me.m_TextureCoordinates Is Nothing Then
                Me.m_TextureCoordinates = New PointCollection()
            End If

            If Not value Then
                Me.m_TextureCoordinates = Nothing
            End If
        End Set
    End Property

#End Region

#Region "New Sub"

    Public Sub New()
        Me.New(True, True)
    End Sub

    Public Sub New(GenerateNormals As Boolean, GenerateTextureCoordinates As Boolean)
        Me.m_Positions = New Point3DCollection()
        Me.m_TriangleIndices = New Int32Collection()
        If GenerateNormals Then
            Me.m_Normals = New Vector3DCollection()
        End If
        If GenerateTextureCoordinates Then
            Me.m_TextureCoordinates = New PointCollection()
        End If
    End Sub

#End Region

    Private Shared CircleCache As New Dictionary(Of Integer, PointCollection)
    Public Shared Function GetCircle(ThetaDiv As Integer) As PointCollection
        Dim Circle As New PointCollection
        If CircleCache.TryGetValue(ThetaDiv, Circle) = False Then
            Circle = New PointCollection
            For i As Integer = 0 To ThetaDiv - 1
                Dim Theta As Double = Math.PI * 2 * (CDbl(i) / (ThetaDiv - 1))
                Circle.Add(New Point(Math.Cos(Theta), Math.Sin(Theta)))
            Next
            Circle.Freeze()
            CircleCache.Add(ThetaDiv, Circle)
        Else
            Circle = CircleCache.Item(ThetaDiv)
        End If
        Return Circle
    End Function

    Public Sub RevolveGeometry(PlanePoints As PointCollection, Origin As Point3D, Dir As Vector3D, ThetaDiv As Integer)
        Dir.Normalize()

        Dim U As Vector3D = Dir.FindAnyPerpendicular ' بردار عمود بر بردار بردار جهت
        Dim V As Vector3D = Vector3D.CrossProduct(Dir, U) ' بردار عمود بر بردار جهت و بردار قبلی (عمود بر هر دو) ضرب خارجی
        U.Normalize()
        V.Normalize()

        Dim Circle As PointCollection = GetCircle(ThetaDiv)
        Dim PCount As Integer = PlanePoints.Count
        Dim Index0 As Integer = m_Positions.Count
        Dim RowNode As Integer = (PCount - 1) * 2 ' تعداد گره های بین دو نقطه در یکبار اجرا شدن حلقه
        Dim TotalNode As Integer = RowNode * ThetaDiv ' تعداد گره های بین دو نقطه در کل پروسه

        For i = 0 To ThetaDiv - 1
            Dim W As Vector3D = (Circle(i).X * U) + (Circle(i).Y * V) ' بردار مرکز تا محیط

            Dim j As Integer = 0
            While j + 1 < PCount
                ' دو نقطه ی متوالی
                Dim P1 As Point3D = Origin + (PlanePoints(j).X * W) + (PlanePoints(j).Y * Dir)
                Dim P2 As Point3D = Origin + (PlanePoints(j + 1).X * W) + (PlanePoints(j + 1).Y * Dir)
                m_Positions.Add(P1)
                m_Positions.Add(P2)
                'اختلاف طول ها
                Dim Tx As Double = PlanePoints(j + 1).X - PlanePoints(j).X
                'اختلاف عرض ها
                Dim Ty As Double = PlanePoints(j + 1).Y - PlanePoints(j).Y
                'بردار نرمال
                If m_Normals IsNot Nothing Then
                    Dim Norm As Vector3D
                    Norm = (Ty * W) + (Tx * -Dir)
                    Norm.Normalize()
                    m_Normals.Add(Norm)
                    m_Normals.Add(Norm)
                End If

                'مختصات تکسچر
                If m_TextureCoordinates IsNot Nothing Then
                    Dim TexPoint1 As New Point(CDbl(i) / ThetaDiv - 1, CDbl(j) / PCount - 1)
                    Dim TexPoint2 As New Point(CDbl(i) / ThetaDiv - 1, (CDbl(j) + 1) / PCount - 1)
                    m_TextureCoordinates.Add(TexPoint1)
                    m_TextureCoordinates.Add(TexPoint2)
                End If

                ' شاخص های مثلث
                Dim I0 As Integer = Index0 + (i * RowNode) + (j * 2)
                Dim I1 As Integer = I0 + 1
                Dim I2 As Integer = Index0 + ((((i + 1) * RowNode) + (j * 2)) Mod TotalNode)
                Dim I3 As Integer = I2 + 1
                ' مثلث اول
                m_TriangleIndices.Add(I1)
                m_TriangleIndices.Add(I0)
                m_TriangleIndices.Add(I2)
                ' مثلث دوم
                m_TriangleIndices.Add(I1)
                m_TriangleIndices.Add(I2)
                m_TriangleIndices.Add(I3)

                j += 1
            End While
        Next
    End Sub

    Public Sub ExtrudeGeometry(Points As PointCollection, XAxis As Vector3D, p0 As Point3D, p1 As Point3D)
        Dim YDirection = Vector3D.CrossProduct(XAxis, p1 - p0)
        YDirection.Normalize()
        XAxis.Normalize()

        Dim Index0 As Integer = Me.Positions.Count
        Dim NP As Integer = 2 * Points.Count
        For Each P As Point In Points
            Dim V = (XAxis * P.X) + (YDirection * P.Y)
            Me.Positions.Add(p0 + V)
            Me.Positions.Add(p1 + V)
            V.Normalize()

            If Me.Normals IsNot Nothing Then
                Me.Normals.Add(V)
                Me.Normals.Add(V)
            End If

            If Me.TextureCoordinates IsNot Nothing Then
                Me.TextureCoordinates.Add(New Point(0, 0))
                Me.TextureCoordinates.Add(New Point(1, 0))
            End If

            Dim i1 As Integer = Index0 + 1
            Dim i2 As Integer = (Index0 + 2) Mod NP
            Dim i3 As Integer = ((Index0 + 2) Mod NP) + 1

            Me.TriangleIndices.Add(i1)
            Me.TriangleIndices.Add(i2)
            Me.TriangleIndices.Add(Index0)

            Me.TriangleIndices.Add(i1)
            Me.TriangleIndices.Add(i3)
            Me.TriangleIndices.Add(i2)
        Next
    End Sub

    Private Sub AddRectangularMeshTriangleIndices(Index0 As Integer, Rows As Integer, Columns As Integer, Optional IsSpherical As Boolean = False)
        For i As Integer = 0 To Rows - 2
            For j As Integer = 0 To Columns - 2
                Dim ij As Integer = (i * Columns) + j
                If Not IsSpherical OrElse i > 0 Then
                    Me.m_TriangleIndices.Add(Index0 + ij)
                    Me.m_TriangleIndices.Add(Index0 + ij + 1 + Columns)
                    Me.m_TriangleIndices.Add(Index0 + ij + 1)
                End If

                If Not IsSpherical OrElse i < Rows - 2 Then
                    Me.m_TriangleIndices.Add(Index0 + ij + 1 + Columns)
                    Me.m_TriangleIndices.Add(Index0 + ij)
                    Me.m_TriangleIndices.Add(Index0 + ij + Columns)
                End If
            Next
        Next
    End Sub

    Public Sub AddCubeFace(Center As Point3D, Normal As Vector3D, Up As Vector3D, Dist As Double, Width As Double, Height As Double)
        Dim Right = Vector3D.CrossProduct(Normal, Up)
        Dim N = Normal * Dist / 2
        Up *= Height / 2
        Right *= Width / 2
        Dim P1 = Center + N - Up - Right
        Dim P2 = Center + N - Up + Right
        Dim P3 = Center + N + Up + Right
        Dim P4 = Center + N + Up - Right

        Dim i0 As Integer = Me.m_Positions.Count
        Me.m_Positions.Add(p1)
        Me.m_Positions.Add(p2)
        Me.m_Positions.Add(p3)
        Me.m_Positions.Add(p4)
        If Me.m_Normals IsNot Nothing Then
            Me.m_Normals.Add(Normal)
            Me.m_Normals.Add(Normal)
            Me.m_Normals.Add(Normal)
            Me.m_Normals.Add(Normal)
        End If

        If Me.m_TextureCoordinates IsNot Nothing Then
            Me.m_TextureCoordinates.Add(New Point(1, 1))
            Me.m_TextureCoordinates.Add(New Point(0, 1))
            Me.m_TextureCoordinates.Add(New Point(0, 0))
            Me.m_TextureCoordinates.Add(New Point(1, 0))
        End If

        Me.m_TriangleIndices.Add(i0 + 2)
        Me.m_TriangleIndices.Add(i0 + 1)
        Me.m_TriangleIndices.Add(i0 + 0)
        Me.m_TriangleIndices.Add(i0 + 0)
        Me.m_TriangleIndices.Add(i0 + 3)
        Me.m_TriangleIndices.Add(i0 + 2)
    End Sub

    'صفحه
    Public Sub AddPlane(Pc As Point3DCollection)
        If Pc.Count > 2 Then
            Dim Index0 As Integer = Me.Positions.Count
            For Each P As Point3D In Pc
                m_Positions.Add(P)
            Next
            For i = 1 To Pc.Count - 1
                m_TriangleIndices.Add(Index0)
                m_TriangleIndices.Add(Index0 + i)
                m_TriangleIndices.Add(Index0 + i + 1)

                m_TriangleIndices.Add(Index0 + i + 1)
                m_TriangleIndices.Add(Index0 + i)
                m_TriangleIndices.Add(Index0)
            Next
        End If
    End Sub
    'مخروط
    Public Sub AddCone(Origin As Point3D, Direction As Vector3D, Radius As Double, Height As Double, _
                       Optional Cap As Boolean = True, Optional ThetaDiv As Integer = 20)
        AddTCone(Origin, Direction, Radius, 0, Height, Cap, False, ThetaDiv)
    End Sub
    'مخروط ناقص
    Public Sub AddTCone(Origin As Point3D, Direction As Vector3D, BaseRadius As Double, TopRadius As Double, Height As Double, _
                        Optional BaseCap As Boolean = True, Optional TopCap As Boolean = True, Optional ThetaDiv As Integer = 40)
        Dim Plane As New PointCollection
        If BaseCap Then Plane.Add(New Point(0, 0))
        Plane.Add(New Point(BaseRadius, 0))
        Plane.Add(New Point(TopRadius, Height))
        If TopCap Then Plane.Add(New Point(0, Height))
        RevolveGeometry(Plane, Origin, Direction, ThetaDiv)
    End Sub
    'استوانه
    Public Sub AddCylinder(Origin As Point3D, Direction As Vector3D, Radius As Double, Height As Double, _
                           Optional BaseCap As Boolean = True, Optional TopCap As Boolean = True, Optional ThetaDiv As Integer = 40)
        AddTCone(Origin, Direction, Radius, Radius, Height, BaseCap, TopCap, ThetaDiv)
    End Sub
    'فلش
    Public Sub AddArrow(Origin As Point3D, Dir As Vector3D, Radius As Double, HeadLen As Double, Optional ThetaDiv As Integer = 40)
        Dim Plane As New PointCollection
        Dim Height As Double = Dir.Length
        Plane.Add(New Point(0, 0))
        Plane.Add(New Point(Radius, 0))
        Plane.Add(New Point(Radius, Height - HeadLen))
        Plane.Add(New Point(2 * Radius, Height - HeadLen))
        Plane.Add(New Point(0, Height))
        RevolveGeometry(Plane, Origin, Dir, ThetaDiv)
    End Sub
    'مکعب
    Public Sub AddCube(Center As Point3D, Length As Double, Faces As BoxFaces)
        AddBox(Center, Length, Length, Length, Faces)
    End Sub
    'مکعب مستطیل
    Public Sub AddBox(Center As Point3D, Xlength As Double, Ylength As Double, Zlength As Double, Faces As BoxFaces)
        If (Faces And BoxFaces.Front) = BoxFaces.Front Then
            Me.AddCubeFace(Center, New Vector3D(1, 0, 0), New Vector3D(0, 0, 1), Xlength, Ylength, Zlength)
        End If

        If (Faces And BoxFaces.Back) = BoxFaces.Back Then
            Me.AddCubeFace(Center, New Vector3D(-1, 0, 0), New Vector3D(0, 0, 1), Xlength, Ylength, Zlength)
        End If

        If (Faces And BoxFaces.Left) = BoxFaces.Left Then
            Me.AddCubeFace(Center, New Vector3D(0, -1, 0), New Vector3D(0, 0, 1), Ylength, Xlength, Zlength)
        End If

        If (Faces And BoxFaces.Right) = BoxFaces.Right Then
            Me.AddCubeFace(Center, New Vector3D(0, 1, 0), New Vector3D(0, 0, 1), Ylength, Xlength, Zlength)
        End If

        If (Faces And BoxFaces.Top) = BoxFaces.Top Then
            Me.AddCubeFace(Center, New Vector3D(0, 0, 1), New Vector3D(0, 1, 0), Zlength, Xlength, Ylength)
        End If

        If (Faces And BoxFaces.Bottom) = BoxFaces.Bottom Then
            Me.AddCubeFace(Center, New Vector3D(0, 0, -1), New Vector3D(0, 1, 0), Zlength, Xlength, Ylength)
        End If
    End Sub

    'کره
    Public Sub AddSphere(Center As Point3D, Radius As Double, _
                         Optional ThetaDiv As Integer = 40, Optional PhiDiv As Integer = 20)
        AddEllipsoid(Center, Radius, Radius, Radius, ThetaDiv, PhiDiv)
    End Sub
    'کره بیضوی
    Public Sub AddEllipsoid(Center As Point3D, RadiusX As Double, RadiusY As Double, RadiusZ As Double, _
                            Optional ThetaDiv As Integer = 40, Optional PhiDiv As Integer = 20)
        Dim Index0 As Integer = Me.Positions.Count

        Dim DT As Double = 2 * Math.PI / ThetaDiv  ' زاویه تتا بین صفر و 2 پی می باشد
        Dim DP As Double = Math.PI / PhiDiv ' زاویه فی بین صفر و پی می باشد

        For Pi As Integer = 0 To PhiDiv
            Dim Phi As Double = Pi * DP ' زاویه فی

            For Ti As Integer = 0 To ThetaDiv
                Dim Theta As Double = Ti * DT ' زاویه تتا

                ' مختصات کروی
                'x= Cos(Theta) * Sin(Phi) * r
                'y= Sin(Theta) * Sin(Phi) * r
                'z= Cos(Phi) * r

                Dim X As Double = Math.Cos(Theta) * Math.Sin(Phi)
                Dim Y As Double = Math.Sin(Theta) * Math.Sin(Phi)
                Dim Z As Double = Math.Cos(Phi)

                Dim p = New Point3D(Center.X + (RadiusX * X), Center.Y + (RadiusY * Y), Center.Z + (RadiusZ * Z))
                Me.m_Positions.Add(p)

                If Me.m_Normals IsNot Nothing Then
                    Dim Norm = New Vector3D(X, Y, Z)
                    Me.m_Normals.Add(Norm)
                End If

                If Me.m_TextureCoordinates IsNot Nothing Then
                    Dim Tex = New Point(Theta / (2 * Math.PI), Phi / Math.PI)
                    Me.m_TextureCoordinates.Add(Tex)
                End If
            Next
        Next
        Me.AddRectangularMeshTriangleIndices(Index0, PhiDiv + 1, ThetaDiv + 1, True)
    End Sub
    
    Public Function ToMesh() As MeshGeometry3D
        If Me.m_Normals IsNot Nothing AndAlso Me.m_Positions.Count <> Me.m_Normals.Count Then
            Throw New InvalidOperationException("WrongNumberOfNormals #AKP")
        End If
        If Me.m_TextureCoordinates IsNot Nothing AndAlso Me.m_Positions.Count <> Me.m_TextureCoordinates.Count Then
            Throw New InvalidOperationException("WrongNumberOfTextureCoordinates #AKP")
        End If
        Dim Mesh = New MeshGeometry3D() With { _
            .Positions = Me.m_Positions, _
            .TriangleIndices = Me.m_TriangleIndices, _
            .Normals = Me.m_Normals, _
            .TextureCoordinates = Me.m_TextureCoordinates _
        }
        Return Mesh
    End Function

End Class
