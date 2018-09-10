Imports System.Windows.Controls.Primitives

Public Class MouseDragPopupBehavior
    Inherits Behavior(Of Popup)

    Private IsMouseDown As Boolean
    Private OldMousePosition As Point

    Protected Overrides Sub OnAttached()
        'AssociatedObject Is The Attached Item 
        AddHandler AssociatedObject.MouseLeftButtonDown, Function(s, e)
                                                             IsMouseDown = True
                                                             OldMousePosition = AssociatedObject.PointToScreen(e.GetPosition(AssociatedObject))
                                                             AssociatedObject.Child.CaptureMouse()
                                                             Return 0
                                                         End Function

        AddHandler AssociatedObject.MouseMove, Function(s, e)
                                                   If Not IsMouseDown Then
                                                       Return 0
                                                   End If
                                                   Dim NewMousePosition = AssociatedObject.PointToScreen(e.GetPosition(AssociatedObject))
                                                   Dim Offset = NewMousePosition - OldMousePosition
                                                   OldMousePosition = NewMousePosition
                                                   AssociatedObject.HorizontalOffset += Offset.X
                                                   AssociatedObject.VerticalOffset += Offset.Y
                                                   Return 0
                                               End Function

        AddHandler AssociatedObject.MouseLeftButtonUp, Function(s, e)
                                                           IsMouseDown = False
                                                           AssociatedObject.Child.ReleaseMouseCapture()
                                                           Return 0
                                                       End Function

    End Sub
End Class
