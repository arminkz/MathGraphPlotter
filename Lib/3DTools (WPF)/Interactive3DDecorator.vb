'---------------------------------------------------------------------------
'
' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Limited Permissive License.
' See http://www.microsoft.com/resources/sharedsource/licensingbasics/limitedpermissivelicense.mspx
' All other rights reserved.
'
' This file is part of the 3D Tools for Windows Presentation Foundation
' project.  For more information, see:
' 
' http://CodePlex.com/Wiki/View.aspx?ProjectName=3DTools
'
'---------------------------------------------------------------------------

Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Shapes
Imports System.Windows.Input
Imports System.Windows.Markup
' IAddChild, ContentPropertyAttribute
''' <summary>
''' Class causes a Viewport3D to become interactive.  To cause the interactivity,
''' a hidden visual, corresponding to the Visual being interacted with, is placed
''' on the PostViewportChildren layer, and is then interacted with, giving the illusion
''' of interacting with the 3D object.
''' </summary>
Public Class Interactive3DDecorator
	Inherits Viewport3DDecorator
	''' <summary>
	''' Constructs the InteractiveViewport3D
	''' </summary>
	Public Sub New()
		MyBase.New()
		' keep everything within our bounds so that the hidden visuals are only
		' accessable over the Viewport3D
		ClipToBounds = True

		' the offset of the hidden visual and the transform associated with it
		_offsetX = InlineAssignHelper(_offsetY, 0.0)
		_scale = 1

		' set up the hidden visual transforms
		_hiddenVisTranslate = New TranslateTransform(_offsetX, _offsetY)
		_hiddenVisScale = New ScaleTransform(_scale, _scale)
		_hiddenVisTransform = New TransformGroup()
		_hiddenVisTransform.Children.Add(_hiddenVisScale)
		_hiddenVisTransform.Children.Add(_hiddenVisTranslate)

		' the layer that contains our moving visual
		_hiddenVisual = New Decorator()
		_hiddenVisual.Opacity = 0.0
		_hiddenVisual.RenderTransform = _hiddenVisTransform

		' where we store the previous hidden visual so that it can be in the tree
		' after it is removed so any state (i.e. mouse over) can be updated.
		_oldHiddenVisual = New Decorator()
		_oldHiddenVisual.Opacity = 0.0

		' the keyboard focus visual
		_oldKeyboardFocusVisual = New Decorator()
		_oldKeyboardFocusVisual.Opacity = 0.0

		' add all of the hidden visuals
		PostViewportChildren.Add(_oldHiddenVisual)
		PostViewportChildren.Add(_oldKeyboardFocusVisual)
		PostViewportChildren.Add(_hiddenVisual)

		' initialize other member variables to null
		_closestIntersectInfo = Nothing
		_lastValidClosestIntersectInfo = Nothing

		AllowDrop = True
	End Sub

	''' <summary>
	''' We want our visuals to size themselves to their desired size, so we impose no
	''' constraint on them when measuring.
	''' </summary>
	''' <param name="Constraint"></param>
	Protected Overrides Sub MeasurePostViewportChildren(constraint As Size)
		Dim noConstraintSize As New Size([Double].PositiveInfinity, [Double].PositiveInfinity)

		' measure the post viewport visuals
		For Each uiElem As UIElement In PostViewportChildren
			uiElem.Measure(noConstraintSize)
		Next
	End Sub

	''' <summary>
	''' The hidden visuals are all set at their desired size.  The passed in 
	''' arrangeSize is ignored.
	''' </summary>
	''' <param name="arrangeSize"></param>
	Protected Overrides Sub ArrangePostViewportChildren(arrangeSize As Size)
		' measure the post viewport visual visuals
		For Each uiElem As UIElement In PostViewportChildren
			uiElem.Arrange(New Rect(uiElem.DesiredSize))
		Next
	End Sub

	''' <summary>
	''' When a drag is in progress we do the same hit testing as in a 
	''' regular mouse move, except we need to scale up the hidden visual
	''' to "correct" for how mouse positions are calculated during a drag
	''' and drop operation.
	''' </summary>
	''' <param name="e"></param>
	Protected Overrides Sub OnPreviewDragOver(e As DragEventArgs)
		MyBase.OnPreviewDragOver(e)

		If Viewport3D IsNot Nothing Then
			Dim mousePosition As Point = e.GetPosition(Viewport3D)
			ArrangeHiddenVisual(mousePosition, True)
		End If
	End Sub

	''' <summary>
	''' Although the InteractiveViewport3D sets the AllowDrop flag to true
	''' so that it can intercept preview drag moves, it doesn't actually
	''' do anything with drag+drop, so if a DragOver event ever reaches us
	''' set the effects to be none.
	''' </summary>
	''' <param name="e"></param>
	Protected Overrides Sub OnDragOver(e As DragEventArgs)
		MyBase.OnDragOver(e)

		e.Effects = DragDropEffects.None
		e.Handled = True
	End Sub

	''' <summary>
	''' Although the InteractiveViewport3D sets the AllowDrop flag to true
	''' so that it can intercept preview drag moves, it doesn't actually
	''' do anything with drag+drop, so if a OnDragEnter event ever reaches us
	''' set the effects to be none.
	''' </summary>
	''' <param name="e"></param>
	Protected Overrides Sub OnDragEnter(e As DragEventArgs)
		MyBase.OnDragEnter(e)

		e.Effects = DragDropEffects.None
		e.Handled = True
	End Sub

	''' <summary>
	''' On a mouse move, we hit test the Viewport3D, and arrange the hidden visuals
	''' to be in the correct locations.  This function is the core event that needs
	''' to be handled so that the InteractiveViewport3D works.
	''' </summary>
	''' <param name="e">The mouse event arguments</param>
	Protected Overrides Sub OnPreviewMouseMove(e As MouseEventArgs)
		MyBase.OnPreviewMouseMove(e)

		' quick speedup to avoid hit testing again right after we moved in to the 
		' correct position
		If _isInPosition Then
			_isInPosition = False
		Else
			If Viewport3D IsNot Nothing Then
				Dim needsMouseResync As Boolean = ArrangeHiddenVisual(e.GetPosition(Viewport3D), False)

				If needsMouseResync Then
					e.Handled = True

					_isInPosition = True

					' we need to make this InvalidateArrange call so that inking works.
					' This is potentially a time consuming call, so by default it does not
					' happen unless ContainsInk is set to be true.
					If ContainsInk Then
						InvalidateArrange()
					End If

					' resynch the mouse since we just moved things around
					Mouse.Synchronize()
				End If
			End If
		End If
	End Sub

	''' <summary>
	''' Arranges the hidden visuals so that interactivity is achieved.
	''' </summary>
	''' <param name="mouseposition">The location of the mouse</param>
	''' <returns>Whether a mouse resynch is necessary</returns>
	Private Function ArrangeHiddenVisual(mouseposition As Point, scaleHiddenVisual As Boolean) As Boolean
		Dim needMouseResync As Boolean = False

		' get the underlying viewport3D we're enhancing
		Dim viewport3D__1 As Viewport3D = Viewport3D

		' if the viewport3D exists - perform a hit test operation on the underlying visuals
		If viewport3D__1 IsNot Nothing Then
			' set up the hit test parameters
			Dim pointparams As New PointHitTestParameters(mouseposition)
			_closestIntersectInfo = Nothing
			_mouseCaptureInHiddenVisual = _hiddenVisual.IsMouseCaptureWithin

			' first hit test - this one attempts to hit a visible mesh if possible
			VisualTreeHelper.HitTest(viewport3D__1, AddressOf InteractiveMV3DFilter, AddressOf HTResult, pointparams)

			' perform capture positioning if we didn't hit anything and something has capture
			If _closestIntersectInfo Is Nothing AndAlso _mouseCaptureInHiddenVisual AndAlso _lastValidClosestIntersectInfo IsNot Nothing Then
				HandleMouseCaptureButOffMesh(_lastValidClosestIntersectInfo.InteractiveModelVisual3DHit, mouseposition)
			ElseIf _closestIntersectInfo IsNot Nothing Then
				' save it for if we walk off the mesh and something has capture                  
				_lastValidClosestIntersectInfo = _closestIntersectInfo
			End If

			' update the location if we have positioning information
			needMouseResync = UpdateHiddenVisual(_closestIntersectInfo, mouseposition, scaleHiddenVisual)
		End If

		Return needMouseResync
	End Function

	''' <summary>
	''' Function to deal with mouse capture when off the mesh.
	''' </summary>
	''' <param name="imv3DHit">The model hit</param>
	''' <param name="mousePos">The location of the mouse</param>
	Private Sub HandleMouseCaptureButOffMesh(imv3DHit As InteractiveVisual3D, mousePos As Point)
		' process the mouse capture if it exists
		Dim uie As UIElement = DirectCast(Mouse.Captured, UIElement)

		' get the size of the element
		Dim contBounds As Rect = VisualTreeHelper.GetDescendantBounds(uie)

		' translate to the parent's coordinate system
		Dim gt As GeneralTransform = uie.TransformToAncestor(_hiddenVisual)

		Dim visCorners As Point() = New Point(3) {}

		' get the points relative to the parent
		visCorners(0) = gt.Transform(New Point(contBounds.Left, contBounds.Top))
		visCorners(1) = gt.Transform(New Point(contBounds.Right, contBounds.Top))
		visCorners(2) = gt.Transform(New Point(contBounds.Right, contBounds.Bottom))
		visCorners(3) = gt.Transform(New Point(contBounds.Left, contBounds.Bottom))

		' get the u,v texture coordinate values of the above points
		Dim texCoordsOfInterest As Point() = New Point(3) {}
		For i As Integer = 0 To visCorners.Length - 1
			texCoordsOfInterest(i) = VisualCoordsToTextureCoords(visCorners(i), _hiddenVisual)
		Next

		' get the edges that map to the given visual
		Dim edges As List(Of HitTestEdge) = imv3DHit.GetVisualEdges(texCoordsOfInterest)

		If Debug Then
			Dim myAdornerLayer As AdornerLayer = AdornerLayer.GetAdornerLayer(Me)
			If _DEBUGadorner Is Nothing Then
				_DEBUGadorner = New DebugEdgesAdorner(Me, edges)
				myAdornerLayer.Add(_DEBUGadorner)
			Else
				myAdornerLayer.Remove(_DEBUGadorner)
				_DEBUGadorner = New DebugEdgesAdorner(Me, edges)
				myAdornerLayer.Add(_DEBUGadorner)
			End If
		End If

		' find the closest intersection of the mouse position and the edge list
		FindClosestIntersection(mousePos, edges, imv3DHit)
	End Sub

	''' <summary>
	''' Finds the point in edges that is closest ot the mouse position.  Updates closestIntersectionInfo
	''' with the results of this calculation
	''' </summary>
	''' <param name="mousePos">The mouse position</param>
	''' <param name="edges">The edges to test against</param>
	''' <param name="imv3DHit">The model that has the visual on it with capture</param>
	Private Sub FindClosestIntersection(mousePos As Point, edges As List(Of HitTestEdge), imv3DHit As InteractiveVisual3D)
		Dim closestDistance As Double = [Double].MaxValue
		Dim closestIntersection As New Point()
		' the uv of the closest intersection
		' Find the closest point to the mouse position            
		For i As Integer = 0 To edges.Count - 1
			Dim v1 As Vector = mousePos - edges(i)._p1Transformed
			Dim v2 As Vector = edges(i)._p2Transformed - edges(i)._p1Transformed

			Dim currClosest As Point
			Dim distance As Double

			' calculate the distance from the mouse position to this edge
			' The closest distance can be computed by projecting v1 on to v2.  If the
			' projectiong occurs between _p1Transformed and _p2Transformed, then this is the
			' closest point.  Otherwise, depending on which side it lies, it is either _p1Transformed
			' or _p2Transformed.  
			'
			' The projection equation is given as: (v1 DOT v2) / (v2 DOT v2) * v2.
			' v2 DOT v2 will always be positive.  Thus, if v1 DOT v2 is negative, we know the projection
			' will occur before _p1Transformed (and so it is the closest point).  If (v1 DOT v2) is greater
			' than (v2 DOT v2), then we have gone passed _p2Transformed and so it is the closest point.
			' Otherwise the projection gives us this value.
			'
			Dim denom As Double = v2 * v2
			If denom = 0 Then
				currClosest = edges(i)._p1Transformed
				distance = v1.Length
			Else
				Dim numer As Double = v2 * v1
				If numer < 0 Then
					currClosest = edges(i)._p1Transformed
				Else
					If numer > denom Then
						currClosest = edges(i)._p2Transformed
					Else
						currClosest = edges(i)._p1Transformed + (numer / denom) * v2
					End If
				End If

				distance = (mousePos - currClosest).Length
			End If

			' see if we found a new closest distance
			If distance < closestDistance Then
				closestDistance = distance

				If denom <> 0 Then
					closestIntersection = ((currClosest - edges(i)._p1Transformed).Length / Math.Sqrt(denom) * (edges(i)._uv2 - edges(i)._uv1)) + edges(i)._uv1
				Else
					closestIntersection = edges(i)._uv1
				End If
			End If
		Next

		If closestDistance <> [Double].MaxValue Then
			Dim uiElemWCapture As UIElement = DirectCast(Mouse.Captured, UIElement)
			Dim uiElemOnMesh As UIElement = imv3DHit.InternalVisual

			Dim contBounds As Rect = VisualTreeHelper.GetDescendantBounds(uiElemWCapture)
			Dim ptOnVisual As Point = TextureCoordsToVisualCoords(closestIntersection, uiElemOnMesh)
			Dim ptRelToCapture As Point = uiElemOnMesh.TransformToDescendant(uiElemWCapture).Transform(ptOnVisual)

			' we want to "ring" around the outside so things like buttons are not pressed
			' this code here does that - the +BUFFER_SIZE and -BUFFER_SIZE are to give a bit of a 
			' buffer for any numerical issues
			If ptRelToCapture.X <= contBounds.Left + 1 Then
				ptRelToCapture.X -= BUFFER_SIZE
			End If
			If ptRelToCapture.Y <= contBounds.Top + 1 Then
				ptRelToCapture.Y -= BUFFER_SIZE
			End If
			If ptRelToCapture.X >= contBounds.Right - 1 Then
				ptRelToCapture.X += BUFFER_SIZE
			End If
			If ptRelToCapture.Y >= contBounds.Bottom - 1 Then
				ptRelToCapture.Y += BUFFER_SIZE
			End If

			Dim finalVisualPoint As Point = uiElemWCapture.TransformToAncestor(uiElemOnMesh).Transform(ptRelToCapture)

			_closestIntersectInfo = New ClosestIntersectionInfo(VisualCoordsToTextureCoords(finalVisualPoint, uiElemOnMesh), imv3DHit.InternalVisual, imv3DHit)
		End If
	End Sub

	''' <summary>
	''' Filter for hit testing.  In the case that the hidden visual has capture
	''' then all Visual3Ds are skipped except for the one it is on.  This gives the 
	''' same behavior as capture in the 2D case.
	''' </summary>
	''' <param name="o"></param>
	''' <returns></returns>
	Public Function InteractiveMV3DFilter(o As DependencyObject) As HitTestFilterBehavior
		' by default everything is ok
		Dim behavior As HitTestFilterBehavior = HitTestFilterBehavior.[Continue]

		' if the hidden visual has mouse capture - then we only want to test against
		' the IMV3D that has capture
		If TypeOf o Is Visual3D AndAlso _mouseCaptureInHiddenVisual Then
			If TypeOf o Is InteractiveVisual3D Then
				Dim imv3D As InteractiveVisual3D = DirectCast(o, InteractiveVisual3D)

				If imv3D.InternalVisual IsNot _hiddenVisual.Child Then
					behavior = HitTestFilterBehavior.ContinueSkipSelf
				End If
			Else
				behavior = HitTestFilterBehavior.ContinueSkipSelf
			End If
		End If

		Return behavior
	End Function

	''' <summary>
	''' This function sets the passed in uiElem as the hidden visual, and aligns
	''' it so that the point the uv coordinates map to on the visual are located
	''' at the same location as mousePos.
	''' </summary>
	''' <param name="uiElem">The UIElement that should be the hidden visual</param>
	''' <param name="uv">The uv coordinates on that UIElement that should be aligned with mousePos</param>
	''' <param name="mousePos">The mouse location</param>
	''' <param name="scaleHiddenVisual">Whether to scale the visual in addition to moving it</param>
	''' <returns></returns>
	Private Function UpdateHiddenVisual(isectInfo As ClosestIntersectionInfo, mousePos As Point, scaleHiddenVisual As Boolean) As Boolean
		Dim needsMouseReSync As Boolean = False
		Dim newOffsetX As Double, newOffsetY As Double

		' compute positioning information
		If isectInfo IsNot Nothing Then
			Dim uiElem As UIElement = isectInfo.UIElementHit

			' set our UIElement to be the one passed in
			If _hiddenVisual.Child IsNot uiElem Then
				' we need to replace the old one with this new one
				Dim prevVisual As UIElement = _hiddenVisual.Child

				' clear out uiElem from any of our hidden visuals
				If _oldHiddenVisual.Child Is uiElem Then
					_oldHiddenVisual.Child = Nothing
				End If
				If _oldKeyboardFocusVisual.Child Is uiElem Then
					_oldKeyboardFocusVisual.Child = Nothing
				End If

				' also clear out prevVisual
				If _oldHiddenVisual.Child Is prevVisual Then
					_oldHiddenVisual.Child = Nothing
				End If
				If _oldKeyboardFocusVisual.Child Is prevVisual Then
					_oldKeyboardFocusVisual.Child = Nothing
				End If

				' depending on whether or not it has focus, do two different things, either
				' use the _oldKeyboardFocusVisual or the _oldHiddenVisual
				Dim _oldVisToUse As Decorator = Nothing
				If prevVisual IsNot Nothing AndAlso prevVisual.IsKeyboardFocusWithin Then
					_oldVisToUse = _oldKeyboardFocusVisual
				Else
					_oldVisToUse = _oldHiddenVisual
				End If

				' now safely link everything up
				_hiddenVisual.Child = uiElem
				_oldVisToUse.Child = prevVisual

				needsMouseReSync = True
			End If

			Dim ptOnVisual As Point = TextureCoordsToVisualCoords(isectInfo.PointHit, _hiddenVisual)
			newOffsetX = mousePos.X - ptOnVisual.X
			newOffsetY = mousePos.Y - ptOnVisual.Y
		Else
			' because we didn't interesect with anything, we need to move the hidden visual off
			' screen so that it can no longer be interacted with
			newOffsetX = ActualWidth + 1
			newOffsetY = ActualHeight + 1
		End If

		' compute the scale needed
		Dim newScale As Double
		If scaleHiddenVisual Then
			newScale = Math.Max(Viewport3D.RenderSize.Width, Viewport3D.RenderSize.Height)
		Else
			newScale = 1.0
		End If

		' do the actual positioning
		needsMouseReSync = needsMouseReSync Or PositionHiddenVisual(newOffsetX, newOffsetY, newScale, mousePos)

		Return needsMouseReSync
	End Function

	''' <summary>
	''' Positions the hidden visual based upon the offset and scale specified.
	''' </summary>
	''' <param name="newOffsetX">The new hidden visual x offset</param>
	''' <param name="newOffsetY">The new hidden visual y offset</param>
	''' <param name="newScale">The new scale to perform on the visual</param>
	''' <param name="mousePosition">The position of the mouse</param>
	''' <returns>Whether the new offset/scale was different than the previous</returns>
	Private Function PositionHiddenVisual(newOffsetX As Double, newOffsetY As Double, newScale As Double, mousePosition As Point) As Boolean
		Dim positionChanged As Boolean = False

		If newOffsetX <> _offsetX OrElse newOffsetY <> _offsetY OrElse _scale <> newScale Then
			_offsetX = newOffsetX
			_offsetY = newOffsetY
			_scale = newScale

			' change where we're putting the object                    
			_hiddenVisTranslate.X = _scale * (_offsetX - mousePosition.X) + mousePosition.X
			_hiddenVisTranslate.Y = _scale * (_offsetY - mousePosition.Y) + mousePosition.Y
			_hiddenVisScale.ScaleX = _scale
			_hiddenVisScale.ScaleY = _scale

			positionChanged = True
		End If

		Return positionChanged
	End Function

	''' <summary>
	''' Converts a point given in texture coordinates to the corresponding
	''' 2D point on the UIElement passed in.
	''' </summary>
	''' <param name="uv">The texture coordinate to convert</param>
	''' <param name="uiElem">The UIElement whose coordinate system is to be used</param>
	''' <returns>
	''' The 2D point on the passed in UIElement cooresponding to the
	''' passed in texture coordinate. 
	''' </returns>
	Private Shared Function TextureCoordsToVisualCoords(uv As Point, uiElem As UIElement) As Point
		Dim descBounds As Rect = VisualTreeHelper.GetDescendantBounds(uiElem)

		Return New Point(uv.X * descBounds.Width + descBounds.Left, uv.Y * descBounds.Height + descBounds.Top)
	End Function

	''' <summary>
	''' Converts a point on the passed in UIElement to the corresponding
	''' texture coordinate for that point.  The function assumes (0, 0)
	''' is the upper-left texture coordinate and (1,1) is the lower-right.
	''' </summary>
	''' <param name="pt">The 2D point on the passed in UIElement to convert</param>
	''' <param name="uiElem">The UIElement whose coordinate system is being used</param>
	''' <returns>
	''' The texture coordinate corresponding to the 2D point on the passed in UIElement
	''' </returns>
	Private Shared Function VisualCoordsToTextureCoords(pt As Point, uiElem As UIElement) As Point
		Dim descBounds As Rect = VisualTreeHelper.GetDescendantBounds(uiElem)

		Return New Point((pt.X - descBounds.Left) / (descBounds.Right - descBounds.Left), (pt.Y - descBounds.Top) / (descBounds.Bottom - descBounds.Top))
	End Function


	''' <summary>
	''' we want to keep the _oldKeyboardFocusVisual and _oldHiddenVisual off screen at all 
	''' times so that they can't be interacted with.  This method is overridden to know
	''' when the size of the Viewport3D changes so that we can always keep the above two
	''' hidden visuals off screen
	''' </summary>
	''' <param name="info"></param>
	Protected Overrides Sub OnRenderSizeChanged(info As SizeChangedInfo)
		MyBase.OnRenderSizeChanged(info)

		Dim newSize As Size = info.NewSize
		Dim tt As New TranslateTransform(newSize.Width + 1, 0)
		_oldKeyboardFocusVisual.RenderTransform = tt
		_oldHiddenVisual.RenderTransform = tt
	End Sub

	''' <summary>
	''' The HTResult function simply takes the intersection closest to the origin and
	''' and stores the intersection info for that closest intersection point.
	''' </summary>
	''' <param name="rawresult"></param>
	''' <returns></returns>
	Private Function HTResult(rawresult As System.Windows.Media.HitTestResult) As HitTestResultBehavior
		Dim rayResult As RayHitTestResult = TryCast(rawresult, RayHitTestResult)
		Dim hitTestResultBehavior__1 As HitTestResultBehavior = HitTestResultBehavior.[Continue]

		' since we're hit testing a viewport3D we should be getting the ray hit test result back
		If rayResult IsNot Nothing Then
			_closestIntersectInfo = GetIntersectionInfo(rayResult)
			hitTestResultBehavior__1 = HitTestResultBehavior.[Stop]
		End If

		Return hitTestResultBehavior__1
	End Function

	''' <summary>
	''' Returns the intersection info for the given rayHitResult.  Intersection info
	''' only exists for an InteractiveModelVisual3D, so if an InteractiveModelVisual3D
	''' is not hit, then the return value is null.
	''' </summary>
	''' <param name="rayHitResult"></param>
	''' <returns>
	''' Returns ClosestIntersectionInfo if an InteractiveModelVisual3D is hit, otherwise
	''' returns null.
	''' </returns>
	Private Function GetIntersectionInfo(rayHitResult As RayHitTestResult) As ClosestIntersectionInfo
		Dim isectInfo As ClosestIntersectionInfo = Nothing

		' try to cast to a RaymeshGeometry3DHitTestResult
		Dim rayMeshResult As RayMeshGeometry3DHitTestResult = TryCast(rayHitResult, RayMeshGeometry3DHitTestResult)
		If rayMeshResult IsNot Nothing Then
			' see if we hit an InteractiveVisual3D
			Dim imv3D As InteractiveVisual3D = TryCast(rayMeshResult.VisualHit, InteractiveVisual3D)
			If imv3D IsNot Nothing Then
				' we can now extract the mesh and visual for the object we hit
				Dim geom As MeshGeometry3D = rayMeshResult.MeshHit
				Dim uiElem As UIElement = imv3D.InternalVisual

				If uiElem IsNot Nothing Then
					' pull the barycentric coordinates of the intersection point
					Dim vertexWeight1 As Double = rayMeshResult.VertexWeight1
					Dim vertexWeight2 As Double = rayMeshResult.VertexWeight2
					Dim vertexWeight3 As Double = rayMeshResult.VertexWeight3

					' the indices in to where the actual intersection occurred
					Dim index1 As Integer = rayMeshResult.VertexIndex1
					Dim index2 As Integer = rayMeshResult.VertexIndex2
					Dim index3 As Integer = rayMeshResult.VertexIndex3

					' texture coordinates of the three vertices hit
					' in the case that no texture coordinates are supplied we will simply
					' treat it as if no intersection occurred
					If geom.TextureCoordinates IsNot Nothing AndAlso index1 < geom.TextureCoordinates.Count AndAlso index2 < geom.TextureCoordinates.Count AndAlso index3 < geom.TextureCoordinates.Count Then
						Dim texCoord1 As Point = geom.TextureCoordinates(index1)
						Dim texCoord2 As Point = geom.TextureCoordinates(index2)
						Dim texCoord3 As Point = geom.TextureCoordinates(index3)

						' get the final uv values based on the barycentric coordinates
						Dim finalPoint As New Point(texCoord1.X * vertexWeight1 + texCoord2.X * vertexWeight2 + texCoord3.X * vertexWeight3, texCoord1.Y * vertexWeight1 + texCoord2.Y * vertexWeight2 + texCoord3.Y * vertexWeight3)

						' create and return a valid intersection info
						isectInfo = New ClosestIntersectionInfo(finalPoint, uiElem, imv3D)
					End If
				End If
			End If
		End If

		Return isectInfo
	End Function

	''' <summary>
	''' The following DP allows for the debugging of InteractiveViewport3D by making the
	''' hidden visual no longer transparent, and also draws all of the edges created during 
	''' capture.
	''' </summary>
	Public Shared ReadOnly DebugProperty As DependencyProperty = DependencyProperty.Register("Debug", GetType(Boolean), GetType(Interactive3DDecorator), New PropertyMetadata(False, New PropertyChangedCallback(AddressOf OnDebugPropertyChanged)))

	Public Property Debug() As Boolean
		Get
			Return CBool(GetValue(DebugProperty))
		End Get
		Set
			SetValue(DebugProperty, value)
		End Set
	End Property

	Friend Shared Sub OnDebugPropertyChanged(sender As [Object], e As DependencyPropertyChangedEventArgs)
		Dim iv3D As Interactive3DDecorator = DirectCast(sender, Interactive3DDecorator)

		If CBool(e.NewValue) = True Then
			iv3D._hiddenVisual.Opacity = 0.2
		Else
			iv3D._hiddenVisual.Opacity = 0.0

			If iv3D._DEBUGadorner IsNot Nothing Then
				Dim myAdornerLayer As AdornerLayer = AdornerLayer.GetAdornerLayer(iv3D)
				myAdornerLayer.Remove(iv3D._DEBUGadorner)
				iv3D._DEBUGadorner = Nothing
			End If
		End If
	End Sub

	''' <summary>
	''' The following DP indicates whether any of the 3D objects within the
	''' Viewport3D will have 2D visuals with ink on them - special processing
	''' is required in this case.
	''' </summary>
	Public Shared ReadOnly ContainsInkProperty As DependencyProperty = DependencyProperty.Register("ContainsInk", GetType(Boolean), GetType(Interactive3DDecorator), New PropertyMetadata(False))

	Public Property ContainsInk() As Boolean
		Get
			Return CBool(GetValue(ContainsInkProperty))
		End Get
		Set
			SetValue(ContainsInkProperty, value)
		End Set
	End Property

	''' <summary>
	''' The DebugEdgesAdorner enables the edges returned when the mouse is captured
	''' to be visualized on screen in order to debug where they are, and verify
	''' it is working correctly.
	''' </summary>
	Public Class DebugEdgesAdorner
		Inherits Adorner
		''' <summary>
		''' Constructs the DebugEdgesAdorner class
		''' </summary>
		''' <param name="adornedElement">The element being adorned</param>
		''' <param name="edges">The edges that are to be displayed</param>
		Public Sub New(adornedElement As UIElement, edges As List(Of HitTestEdge))
			MyBase.New(adornedElement)
			_edges = edges
		End Sub

		''' <summary>
		''' Draws all of the edges.
		''' </summary>
		''' <param name="drawingContext"></param>
		Protected Overrides Sub OnRender(drawingContext As DrawingContext)
			Dim renderPen As New Pen(New SolidColorBrush(Colors.Navy), 1.5)

			For i As Integer = 0 To _edges.Count - 1
				drawingContext.DrawLine(renderPen, _edges(i)._p1Transformed, _edges(i)._p2Transformed)
			Next

		End Sub

		Private _edges As List(Of HitTestEdge)
	End Class

	'------------------------------------------------------
	'
	'  Private data
	'
	'------------------------------------------------------ 

	''' <summary>
	''' The ClosestIntersectionInfo class is a wrapper class that contains all the 
	''' information necessary to process an intersection with an InteractiveModelVisual3D
	''' </summary>
	Private Class ClosestIntersectionInfo
		Public Sub New(p As Point, v As UIElement, iv3D As InteractiveVisual3D)
			_pointHit = p
			_uiElemHit = v
			_imv3DHit = iv3D
		End Sub

		' the point on the visual that we hit
		Private _pointHit As Point
		Public Property PointHit() As Point
			Get
				Return _pointHit
			End Get
			Set
				_pointHit = value
			End Set
		End Property

		' the visual hit by the intersection
		Private _uiElemHit As UIElement
		Public Property UIElementHit() As UIElement
			Get
				Return _uiElemHit
			End Get
			Set
				_uiElemHit = value
			End Set
		End Property

		' the InteractiveModelVisual3D hit by the intersection
		Private _imv3DHit As InteractiveVisual3D
		Public Property InteractiveModelVisual3DHit() As InteractiveVisual3D
			Get
				Return _imv3DHit
			End Get
			Set
				_imv3DHit = value
			End Set
		End Property
	End Class

	Private _hiddenVisual As Decorator
	' the hidden visual that is interacted with
	Private _oldHiddenVisual As Decorator
	' the previous visual - used so that things like losing
	' focus can occur if two visuals are being interacted with
	Private _oldKeyboardFocusVisual As Decorator
	' if the old visual has keyboard focus, we place it in here
	' rather than oldHiddenVisual so that it can retain focus
	' as long as is necessary        
	Private _hiddenVisTranslate As TranslateTransform
	' hidden visual's transform    
	Private _hiddenVisScale As ScaleTransform
	' hidden visual's scale
	Private _hiddenVisTransform As TransformGroup
	' the combined transform
	Private _mouseCaptureInHiddenVisual As Boolean

	Private _offsetX As Double
	' the offset needed to move the visual so
	Private _offsetY As Double
	' that hit testing will work on the hidden visual    
	Private _scale As Double

	Private _closestIntersectInfo As ClosestIntersectionInfo
	Private _lastValidClosestIntersectInfo As ClosestIntersectionInfo = Nothing

	Private _DEBUGadorner As DebugEdgesAdorner = Nothing

	Private _isInPosition As Boolean = False
	' optimization so that things aren't rechecked after they are moved 
	Private Const BUFFER_SIZE As Double = 2.0
	Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
		target = value
		Return value
	End Function
	' the "ring" around the element with capture to use in the capture case        
End Class
