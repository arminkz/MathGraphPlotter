Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D
Imports AKP_Math_Graph_Plotter.Petzold.Media2D

' for ArrowEnds enumeration
''' <summary>
''' 
''' </summary>
Public MustInherit Class WireBase
    Inherits ModelVisual3D
    ' Static fields for storing instances of WireBase.
    Shared listWireBases As New List(Of WireBaseAndUltimateParent)()
    Shared listRemove As New List(Of WireBaseAndUltimateParent)()

    ' Instance fields.
    Private needRecalculation As Boolean = True
    Private matxVisualToScreen As Matrix3D = Matrix3D.Identity
    Private matxScreenToVisual As Matrix3D
    Private rotate As New RotateTransform()

    ' Constructor
    ' -----------
    Public Sub New()
        LineCollection = New Point3DCollection()

        ' Create MeshGeometry3D.
        Dim mesh As New MeshGeometry3D()

        ' Create MaterialGroup.
        Dim matgrp As New MaterialGroup()
        matgrp.Children.Add(New DiffuseMaterial(Brushes.Black))
        matgrp.Children.Add(New EmissiveMaterial(New SolidColorBrush(Color)))

        ' Create GeometryModel3D.
        Dim model As New GeometryModel3D(mesh, matgrp)

        ' Remove this later
        model.BackMaterial = New DiffuseMaterial(Brushes.Red)

        ' Set the Content property to the GeometryModel3D.
        Content = model

        ' Add to collection.
        listWireBases.Add(New WireBaseAndUltimateParent(Me))
    End Sub

    ' Static constructor attaches Rendering handler for all instances.
    Shared Sub New()
        AddHandler CompositionTarget.Rendering, New EventHandler(AddressOf OnRendering)
    End Sub

    Private Shared ReadOnly LineCollectionKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly("LineCollection", GetType(Point3DCollection), GetType(WireBase), New PropertyMetadata(Nothing, AddressOf RecalcPropertyChanged))

    ''' <summary>
    '''     Identifies the BaseLines dependency property.
    ''' </summary>
    Public Shared ReadOnly LineCollectionProperty As DependencyProperty = LineCollectionKey.DependencyProperty

    ''' <summary>
    ''' 
    ''' </summary>
    Public Property LineCollection() As Point3DCollection
        Get
            Return DirectCast(GetValue(LineCollectionProperty), Point3DCollection)
        End Get
        Private Set(value As Point3DCollection)
            SetValue(LineCollectionKey, value)
        End Set
    End Property

    ''' <summary>
    '''     Identifies the Color depencency property.
    ''' </summary>
    Public Shared ReadOnly ColorProperty As DependencyProperty = DependencyProperty.Register("Color", GetType(Color), GetType(WireBase), New PropertyMetadata(Colors.Black, AddressOf ColorPropertyChanged))

    ''' <summary>
    ''' 
    ''' </summary>
    Public Property Color() As Color
        Get
            Return CType(GetValue(ColorProperty), Color)
        End Get
        Set(value As Color)
            SetValue(ColorProperty, value)
        End Set
    End Property

    '
    ' This is the only property that does not require a recalculation
    '  of the MeshGeometry3D.
    Private Shared Sub ColorPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim wirebase As WireBase = TryCast(obj, WireBase)
        Dim model As GeometryModel3D = TryCast(wirebase.Content, GeometryModel3D)
        Dim matgrp As MaterialGroup = TryCast(model.Material, MaterialGroup)
        Dim mat As EmissiveMaterial = TryCast(matgrp.Children(1), EmissiveMaterial)
        mat.Brush = New SolidColorBrush(CType(args.NewValue, Color))
    End Sub

    ''' <summary>
    '''     Identifies the Thickness dependency property.
    ''' </summary>
    Public Shared ReadOnly ThicknessProperty As DependencyProperty = DependencyProperty.Register("Thickness", GetType(Double), GetType(WireBase), New PropertyMetadata(1.0, AddressOf RecalcPropertyChanged))

    ''' <summary>
    ''' 
    ''' </summary>
    Public Property Thickness() As Double
        Get
            Return CDbl(GetValue(ThicknessProperty))
        End Get
        Set(value As Double)
            SetValue(ThicknessProperty, value)
        End Set
    End Property


    ' Rounding property        -- CHECK FOR VALUES < ZERO
    ' -----------------
    Public Shared ReadOnly RoundingProperty As DependencyProperty = DependencyProperty.Register("Rounding", GetType(Integer), GetType(WireBase), New PropertyMetadata(0, AddressOf RecalcPropertyChanged))

    Public Property Rounding() As Integer
        Get
            Return CInt(GetValue(RoundingProperty))
        End Get
        Set(value As Integer)
            SetValue(RoundingProperty, value)
        End Set
    End Property


    ''' <summary>
    '''     Identifies the ArrowAngle dependency property.
    ''' </summary>
    Public Shared ReadOnly ArrowAngleProperty As DependencyProperty = DependencyProperty.Register("ArrowAngle", GetType(Double), GetType(WireBase), New PropertyMetadata(45.0, AddressOf RecalcPropertyChanged))

    ''' <summary>
    '''     Gets or sets the angle between the two sides of the arrowhead.
    ''' </summary>
    Public Property ArrowAngle() As Double
        Get
            Return CDbl(GetValue(ArrowAngleProperty))
        End Get
        Set(value As Double)
            SetValue(ArrowAngleProperty, value)
        End Set
    End Property

    ''' <summary>
    '''     Identifies the ArrowLength dependency property.
    ''' </summary>
    Public Shared ReadOnly ArrowLengthProperty As DependencyProperty = DependencyProperty.Register("ArrowLength", GetType(Double), GetType(WireBase), New PropertyMetadata(12.0, AddressOf RecalcPropertyChanged))

    ''' <summary>
    '''     Gets or sets the length of the two sides of the arrowhead.
    ''' </summary>
    Public Property ArrowLength() As Double
        Get
            Return CDbl(GetValue(ArrowLengthProperty))
        End Get
        Set(value As Double)
            SetValue(ArrowLengthProperty, value)
        End Set
    End Property

    ''' <summary>
    '''     Identifies the ArrowEnds dependency property.
    ''' </summary>
    Public Shared ReadOnly ArrowEndsProperty As DependencyProperty = DependencyProperty.Register("ArrowEnds", GetType(ArrowEnds), GetType(WireBase), New PropertyMetadata(ArrowEnds.None, AddressOf RecalcPropertyChanged))

    ''' <summary>
    '''     Gets or sets the property that determines which ends of the
    '''     line have arrows.
    ''' </summary>
    Public Property ArrowEnds() As ArrowEnds
        Get
            Return CType(GetValue(ArrowEndsProperty), ArrowEnds)
        End Get
        Set(value As ArrowEnds)
            SetValue(ArrowEndsProperty, value)
        End Set
    End Property


    Private Shared Sub RecalcPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim linebase As WireBase = TryCast(obj, WireBase)
        linebase.needRecalculation = True
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="args"></param>
    Protected Shared Sub PropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        TryCast(obj, WireBase).PropertyChanged(args)
    End Sub

    Protected Sub PropertyChanged(args As DependencyPropertyChangedEventArgs)
        Dim lines As Point3DCollection = LineCollection
        LineCollection = Nothing

        Generate(args, lines)

        LineCollection = lines
    End Sub

    ''' <summary>
    '''     Sets the coordinates of all the individual lines in the visual.
    ''' </summary>
    ''' <param name="args">
    '''     The <c>DependencyPropertyChangedEventArgs</c> object associated 
    '''     with the property-changed event that resulted in this method 
    '''     being called.
    ''' </param>
    ''' <param name="lines">
    '''     The <c>Point3DCollection</c> to be filled.
    ''' </param>
    ''' <remarks>
    '''     <para>
    '''         Classes that derive from <c>WireBase</c> override this
    '''         method to fill the <c>lines</c> collection.
    '''         It is custmary for implementations of this method to clear
    '''         the <c>lines</c> collection first before filling it. 
    '''         Each pair of successive members of the <c>lines</c>
    '''         collection indicate one straight line.
    '''     </para>
    ''' </remarks>
    Protected MustOverride Sub Generate(args As DependencyPropertyChangedEventArgs, lines As Point3DCollection)


    Private Shared Sub OnRendering(sender As Object, args As EventArgs)
        For Each wirebaseAndParent As WireBaseAndUltimateParent In listWireBases
            Dim wirebase As WireBase = wirebaseAndParent.wirebase

            Dim obj As DependencyObject = wirebase
            While obj IsNot Nothing AndAlso Not (TypeOf obj Is Window)
                obj = VisualTreeHelper.GetParent(obj)
            End While

            If wirebaseAndParent.window Is Nothing Then
                If obj IsNot Nothing Then
                    wirebaseAndParent.window = TryCast(obj, Window)
                Else
                    ' Otherwise, the WireBase has no ultimate parent of type window,
                    '  so there's no reason to try rendering it.
                    Continue For
                End If
            Else

                ' A non-null 'window' field means the WireBase had an ultimate window
                '  parent at one time. 
                ' But now there's no ultimate window parent, so it's likely that
                '  this WireBase has been disconnected from a valid visual tree.
                If obj Is Nothing Then
                    listRemove.Add(wirebaseAndParent)
                    Continue For
                End If
            End If

            wirebase.OnRendering()
        Next

        ' Possibly remove objects from the rendering list.
        If listRemove.Count > 0 Then
            For Each wirebaseAndParent As WireBaseAndUltimateParent In listRemove
                listWireBases.Remove(wirebaseAndParent)
            Next
            listRemove.Clear()
        End If
    End Sub

    Private Sub OnRendering()
        If LineCollection.Count = 0 Then
            Return
        End If

        Dim matx As Matrix3D = VisualInfo.GetTotalTransform(Me)

        If matx = VisualInfo.ZeroMatrix Then
            Return
        End If

        ' How can this happen????
        If matx.IsIdentity Then
            Return
        End If

        If matx <> matxVisualToScreen Then
            matxVisualToScreen = matx
            matxScreenToVisual = matx

            If matxScreenToVisual.HasInverse Then
                matxScreenToVisual.Invert()
                ' might not be possible !!!!!!
                needRecalculation = True
            Else
                Throw New ApplicationException("Here's where the problem is")

            End If
        End If

        If needRecalculation Then
            Recalculate()
            needRecalculation = False
        End If
    End Sub

    Private Sub Recalculate()
        Dim model As GeometryModel3D = TryCast(Content, GeometryModel3D)
        Dim mesh As MeshGeometry3D = TryCast(model.Geometry, MeshGeometry3D)
        Dim points As Point3DCollection = mesh.Positions
        mesh.Positions = Nothing
        points.Clear()

        Dim indices As Int32Collection = mesh.TriangleIndices
        mesh.TriangleIndices = Nothing
        indices.Clear()

        Dim indicesBase As Integer = 0
        Dim lines As Point3DCollection = LineCollection

        For line As Integer = 0 To lines.Count - 2 Step 2
            Dim pt1 As Point3D = lines(line + 0)
            Dim pt2 As Point3D = lines(line + 1)

            DoLine(pt1, pt2, points, indices, indicesBase)

            If line = 0 AndAlso (ArrowEnds And ArrowEnds.Start) = ArrowEnds.Start Then
                DoArrow(pt2, pt1, points, indices, indicesBase)
            End If

            If line > lines.Count - 4 AndAlso (ArrowEnds And ArrowEnds.[End]) = ArrowEnds.[End] Then
                DoArrow(pt1, pt2, points, indices, indicesBase)
            End If
        Next

        mesh.TriangleIndices = indices
        mesh.Positions = points
    End Sub

    Private Sub DoArrow(pt1 As Point3D, pt2 As Point3D, points As Point3DCollection, indices As Int32Collection, ByRef indicesBase As Integer)
        Dim pt1Screen As Point3D = pt1 * matxVisualToScreen
        Dim pt2Screen As Point3D = pt2 * matxVisualToScreen

        Dim vectArrow As New Vector(pt1Screen.X - pt2Screen.X, pt1Screen.Y - pt2Screen.Y)
        vectArrow.Normalize()
        vectArrow *= ArrowLength

        Dim matx As New Matrix()
        matx.Rotate(ArrowAngle / 2)
        Dim ptArrow1 As Point3D = Widen(pt2, vectArrow * matx)
        matx.Rotate(-ArrowAngle)
        Dim ptArrow2 As Point3D = Widen(pt2, vectArrow * matx)

        DoLine(pt2, ptArrow1, points, indices, indicesBase)
        DoLine(pt2, ptArrow2, points, indices, indicesBase)
    End Sub

    Private Sub DoLine(pt1 As Point3D, pt2 As Point3D, points As Point3DCollection, indices As Int32Collection, ByRef indicesBase As Integer)
        Dim pt1Screen As Point3D = pt1 * matxVisualToScreen
        Dim pt2Screen As Point3D = pt2 * matxVisualToScreen

        Dim vectLine As Vector3D = pt2Screen - pt1Screen
        vectLine.Z = 0
        vectLine.Normalize()

        Dim delta As Vector = (Thickness / 2) * New Vector(-vectLine.Y, vectLine.X)

        points.Add(Widen(pt1, delta))
        points.Add(Widen(pt1, -delta))
        points.Add(Widen(pt2, delta))
        points.Add(Widen(pt2, -delta))

        indices.Add(indicesBase)
        indices.Add(indicesBase + 2)
        indices.Add(indicesBase + 1)
        indices.Add(indicesBase + 1)
        indices.Add(indicesBase + 2)
        indices.Add(indicesBase + 3)

        indicesBase += 4

        If Rounding > 0 Then
            AddRounding(pt1, delta, points, indices, indicesBase)
            AddRounding(pt2, -delta, points, indices, indicesBase)
        End If
    End Sub

    Private Function Widen(pointIn As Point3D, delta As Vector) As Point3D
        Dim pt4In As System.Windows.Media.Media3D.Point4D = CType(pointIn, System.Windows.Media.Media3D.Point4D)
        Dim pt4Out As System.Windows.Media.Media3D.Point4D = pt4In * matxVisualToScreen

        pt4Out.X += delta.X * pt4Out.W
        pt4Out.Y += delta.Y * pt4Out.W

        pt4Out *= matxScreenToVisual

        Dim ptOut As New Point3D(pt4Out.X / pt4Out.W, pt4Out.Y / pt4Out.W, pt4Out.Z / pt4Out.W)
        Return ptOut
    End Function

    Private Sub AddRounding(ptIn As Point3D, delta As Vector, points As Point3DCollection, indices As Int32Collection, ByRef indicesCount As Integer)
        points.Add(CalculatePoint(ptIn, New Vector(0, 0), 0))

        For i As Integer = 0 To Rounding
            points.Add(CalculatePoint(ptIn, delta, 180 * i \ Rounding))
        Next

        For i As Integer = 0 To Rounding - 1
            indices.Add(indicesCount)
            indices.Add(indicesCount + i + 2)
            indices.Add(indicesCount + i + 1)
        Next

        indicesCount += Rounding + 2
    End Sub

    Private Function CalculatePoint(ptIn As Point3D, delta As Vector, angle As Double) As Point3D
        Dim pt4In As System.Windows.Media.Media3D.Point4D = CType(ptIn, System.Windows.Media.Media3D.Point4D)
        Dim pt4Out As System.Windows.Media.Media3D.Point4D = pt4In * matxVisualToScreen

        rotate.Angle = angle
        delta = CType(rotate.Transform(CType(delta, Point)), Vector)

        pt4Out.X += delta.X * pt4Out.W
        pt4Out.Y += delta.Y * pt4Out.W

        pt4Out *= matxScreenToVisual

        Dim ptOut As New Point3D(pt4Out.X / pt4Out.W, pt4Out.Y / pt4Out.W, pt4Out.Z / pt4Out.W)
        Return ptOut
    End Function
End Class
