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
Imports System.Diagnostics
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Security
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media.Composition
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Media3D
Imports System.Windows.Documents
Imports System.Collections

''' <summary>
''' Helper class that encapsulates return data needed for the
''' hit test capture methods.
''' </summary>
Public Class HitTestEdge
	''' <summary>
	''' Constructs a new hit test edge
	''' </summary>
	''' <param name="p1">First edge point</param>
	''' <param name="p2">Second edge point</param>
	''' <param name="uv1">Texture coordinate of first edge point</param>
	''' <param name="uv2">Texture coordinate of second edge point</param>
	Public Sub New(p1 As Point3D, p2 As Point3D, uv1 As Point, uv2 As Point)
		_p1 = p1
		_p2 = p2

		_uv1 = uv1
		_uv2 = uv2
	End Sub

	''' <summary>
	''' Projects the stored 3D points in to 2D.
	''' </summary>
	''' <param name="objectToViewportTransform">The transformation matrix to use</param>
	Public Sub Project(objectToViewportTransform As Matrix3D)
		Dim projPoint1 As Point3D = objectToViewportTransform.Transform(_p1)
		Dim projPoint2 As Point3D = objectToViewportTransform.Transform(_p2)

		_p1Transformed = New Point(projPoint1.X, projPoint1.Y)
		_p2Transformed = New Point(projPoint2.X, projPoint2.Y)
	End Sub

	Public _p1 As Point3D, _p2 As Point3D
	Public _uv1 As Point, _uv2 As Point

	' the transformed Point3D value
	Public _p1Transformed As Point, _p2Transformed As Point
End Class

''' <summary>
''' The InteractiveModelVisual3D class represents a model visual 3D that can 
''' be interacted with.  The class adds some properties that make it easy
''' to construct an interactive 3D object (geometry and visual), and also makes
''' it so those Visual3Ds that want to be interactive can explicitly state this
''' via their type.
''' </summary>
Public Class InteractiveVisual3D
	Inherits ModelVisual3D
	''' <summary>
	''' Constructs a new InteractiveModelVisual3D
	''' </summary>
	Public Sub New()
		InternalVisualBrush = CreateVisualBrush()

		' create holders for the intersection plane and content
		_content = New GeometryModel3D()
		Content = _content

		GenerateMaterial()
	End Sub

	Shared Sub New()
		_defaultMaterialPropertyValue = New DiffuseMaterial()
		_defaultMaterialPropertyValue.SetValue(InteractiveVisual3D.IsInteractiveMaterialProperty, True)
		_defaultMaterialPropertyValue.Freeze()

		MaterialProperty = DependencyProperty.Register("Material", GetType(Material), GetType(InteractiveVisual3D), New PropertyMetadata(_defaultMaterialPropertyValue, New PropertyChangedCallback(AddressOf OnMaterialPropertyChanged)))
	End Sub

	''' <summary>
	''' When a property of the IMV3D changes we play it safe and invalidate the saved
	''' corner cache.
	''' </summary>
	''' <param name="e"></param>
	Protected Overrides Sub OnPropertyChanged(e As DependencyPropertyChangedEventArgs)
		MyBase.OnPropertyChanged(e)

		' invalidate the cache
		_lastVisCorners = Nothing
	End Sub

	''' <summary>
	''' Gets the visual edges that correspond to the passed in texture coordinates of interest.
	''' </summary>
	''' <param name="texCoordsOfInterest">The texture coordinates whose edges should be found</param>
	''' <returns>The visual edges corresponding to the given texture coordinates</returns>
	Friend Function GetVisualEdges(texCoordsOfInterest As Point()) As List(Of HitTestEdge)
		' get and then cache the edges
		_lastEdges = GrabValidEdges(texCoordsOfInterest)
		_lastVisCorners = texCoordsOfInterest

		Return _lastEdges
	End Function

	''' <summary>
	''' Function takes the passed in list of texture coordinate points, and then finds the 
	''' visible outline of the rectangle specified by those points and returns it.
	''' </summary>
	''' <param name="tc">The points specifying the rectangle to search for</param>
	''' <returns>The edges of that rectangle</returns>
	Private Function GrabValidEdges(tc As Point()) As List(Of HitTestEdge)
		' our final edge list
		Dim hitTestEdgeList As New List(Of HitTestEdge)()
		Dim adjInformation As New Dictionary(Of Edge, EdgeInfo)()

		' store some important info in local variables for easier access
		Dim contentGeom As MeshGeometry3D = DirectCast(_content.Geometry, MeshGeometry3D)
		Dim positions As Point3DCollection = contentGeom.Positions
		Dim textureCoords As PointCollection = contentGeom.TextureCoordinates
		Dim triIndices As Int32Collection = contentGeom.TriangleIndices
		Dim contentTransform As Transform3D = _content.Transform

		' we want to map from the camera space to this visual3D's space
		Dim containingVisual As Viewport3DVisual
		Dim success As Boolean

		' this call actually gets the object to camera transform, but we will invert it later, and because of that
		' the local variable is named cameraToObjecTransform.
		Dim cameraToObjectTransform As Matrix3D = MathUtils.TryTransformToCameraSpace(Me, containingVisual, success)
		If Not success Then
			Return New List(Of HitTestEdge)()
		End If

		' take in to account any transform on model
		If contentTransform IsNot Nothing Then
			cameraToObjectTransform.Prepend(contentTransform.Value)
		End If

		' also get the object to screen space transform for use later
		Dim objectToViewportTransform As Matrix3D = MathUtils.TryTransformTo2DAncestor(Me, containingVisual, success)
		If Not success Then
			Return New List(Of HitTestEdge)()
		End If
		If contentTransform IsNot Nothing Then
			objectToViewportTransform.Prepend(contentTransform.Value)
		End If

		' check the cached copy to avoid extra work
		Dim sameAsBefore As Boolean = _lastVisCorners IsNot Nothing
		If _lastVisCorners IsNot Nothing Then
			For i As Integer = 0 To tc.Length - 1
				If tc(i) <> _lastVisCorners(i) Then
					sameAsBefore = False
					Exit For
				End If
			Next

			If _lastMatrix3D <> objectToViewportTransform Then
				sameAsBefore = False
			End If
		End If
		If sameAsBefore Then
			Return _lastEdges
		End If

		' save the matrix that was just used
		_lastMatrix3D = objectToViewportTransform

		' try to invert so we actually have the camera->object transform
		Try
			cameraToObjectTransform.Invert()
		Catch generatedExceptionName As InvalidOperationException
			Return New List(Of HitTestEdge)()
		End Try

		Dim camPosObjSpace As Point3D = cameraToObjectTransform.Transform(New Point3D(0, 0, 0))

		' get the bounding box around the passed in texture coordinates to help
		' with early rejection tests
		Dim bbox As Rect = Rect.Empty
		For i As Integer = 0 To tc.Length - 1
			bbox.Union(tc(i))
		Next

		' walk through the triangles - and look for the triangles we care about
		Dim indices As Integer() = New Integer(2) {}
		Dim p As Point3D() = New Point3D(2) {}
		Dim uv As Point() = New Point(2) {}

		For i As Integer = 0 To triIndices.Count - 1 Step 3
			' get the triangle indices
			Dim triBBox As Rect = Rect.Empty

			For j As Integer = 0 To 2
				indices(j) = triIndices(i + j)

				p(j) = positions(indices(j))
				uv(j) = textureCoords(indices(j))

				triBBox.Union(uv(j))
			Next

			If bbox.IntersectsWith(triBBox) Then
				ProcessTriangle(p, uv, tc, hitTestEdgeList, adjInformation, camPosObjSpace)
			End If
		Next

		' also handle the case of an edge that doesn't also have a backface - i.e a single plane            
		For Each edge As Edge In adjInformation.Keys
			Dim ei As EdgeInfo = adjInformation(edge)

			If ei.hasFrontFace AndAlso ei.numSharing = 1 Then
				HandleSilhouetteEdge(ei.uv1, ei.uv2, edge._start, edge._end, tc, hitTestEdgeList)
			End If
		Next

		' project all the edges to get at the 2D point of interest
		For i As Integer = 0 To hitTestEdgeList.Count - 1
			hitTestEdgeList(i).Project(objectToViewportTransform)
		Next

		Return hitTestEdgeList
	End Function

	''' <summary>
	''' Processes the passed in triangle by checking to see if it is facing the camera and if
	''' so searches to see if the texture coordinate edges intersect it.  It also looks
	''' to see if there are any silhouette edges and processes these as well.
	''' </summary>
	''' <param name="p">The triangle's vertices</param>
	''' <param name="uv">The texture coordinates for those vertices</param>   
	''' <param name="tc">The texture coordinate edges to intersect with</param>
	''' <param name="edgeList">The edge list that results should be placed on</param>
	''' <param name="adjInformation">The adjacency information for the mesh</param>
	Private Sub ProcessTriangle(p As Point3D(), uv As Point(), tc As Point(), edgeList As List(Of HitTestEdge), adjInformation As Dictionary(Of Edge, EdgeInfo), camPosObjSpace As Point3D)
		' calculate the normal of the mesh and the vector from a point on the mesh to the camera 
		' for back face removal calculations.
		Dim normal As Vector3D = Vector3D.CrossProduct(p(1) - p(0), p(2) - p(0))
		Dim dirToCamera As Vector3D = camPosObjSpace - p(0)

		' ignore any triangles that have a normal of (0,0,0)
		If Not (normal.X = 0 AndAlso normal.Y = 0 AndAlso normal.Z = 0) Then
			Dim dotProd As Double = Vector3D.DotProduct(normal, dirToCamera)

			' if the dot product is > 0 then the triangle is visible, otherwise invisible
			If dotProd > 0.0 Then
				' loop over the triangle and update any edge information
				ProcessTriangleEdges(p, uv, tc, PolygonSide.FRONT, edgeList, adjInformation)

				' intersect the bounds of the visual with the triangle
				ProcessVisualBoundsIntersections(p, uv, tc, edgeList)
			Else
				ProcessTriangleEdges(p, uv, tc, PolygonSide.BACK, edgeList, adjInformation)
			End If
		End If
	End Sub

	''' <summary>
	''' Function intersects the edges specified by tc with the texture coordinates
	''' on the passed in triangle.  If there are any intersections, the edges
	''' of these intersections are added to the edgelist
	''' </summary>
	''' <param name="p">The vertices of the triangle</param>
	''' <param name="uv">The texture coordinates for that triangle</param>
	''' <param name="tc">The texture coordinate edges to be intersected against</param>
	''' <param name="edgeList">The list of edges any intersecte edges should be added to</param>
	Private Sub ProcessVisualBoundsIntersections(p As Point3D(), uv As Point(), tc As Point(), edgeList As List(Of HitTestEdge))
		Dim pointList As New List(Of Point3D)()
		Dim uvList As New List(Of Point)()

		' loop over the visual's texture coordinate bounds
		For i As Integer = 0 To tc.Length - 1
			Dim visEdgeStart As Point = tc(i)
			Dim visEdgeEnd As Point = tc((i + 1) Mod tc.Length)

			' clear out anything that used to be there
			pointList.Clear()
			uvList.Clear()

			' loop over triangle edges
			Dim skipListProcessing As Boolean = False
			For j As Integer = 0 To uv.Length - 1
				Dim uv1 As Point = uv(j)
				Dim uv2 As Point = uv((j + 1) Mod uv.Length)
				Dim p3D1 As Point3D = p(j)
				Dim p3D2 As Point3D = p((j + 1) Mod p.Length)

				' initial rejection processing
				If Not ((Math.Max(visEdgeStart.X, visEdgeEnd.X) < Math.Min(uv1.X, uv2.X)) OrElse (Math.Min(visEdgeStart.X, visEdgeEnd.X) > Math.Max(uv1.X, uv2.X)) OrElse (Math.Max(visEdgeStart.Y, visEdgeEnd.Y) < Math.Min(uv1.Y, uv2.Y)) OrElse (Math.Min(visEdgeStart.Y, visEdgeEnd.Y) > Math.Max(uv1.Y, uv2.Y))) Then
					' intersect the two lines
					Dim areCoincident As Boolean = False
					Dim dir As Vector = uv2 - uv1
					Dim t As Double = IntersectRayLine(uv1, dir, visEdgeStart, visEdgeEnd, areCoincident)

					' if they are coincident then we have two intersections and don't need to
					' do anymore processing
					If areCoincident Then
						HandleCoincidentLines(visEdgeStart, visEdgeEnd, p3D1, p3D2, uv1, uv2, _
							edgeList)
						skipListProcessing = True
						Exit For
					ElseIf t >= 0 AndAlso t <= 1 Then
						Dim intersUV As Point = uv1 + dir * t
						Dim intersPoint3D As Point3D = p3D1 + (p3D2 - p3D1) * t

						Dim visEdgeDiff As Double = (visEdgeStart - visEdgeEnd).Length

						If (intersUV - visEdgeStart).Length < visEdgeDiff AndAlso (intersUV - visEdgeEnd).Length < visEdgeDiff Then
							pointList.Add(intersPoint3D)
							uvList.Add(intersUV)
						End If
					End If
				End If
			Next

			If Not skipListProcessing Then
				If pointList.Count >= 2 Then
					edgeList.Add(New HitTestEdge(pointList(0), pointList(1), uvList(0), uvList(1)))
				ElseIf pointList.Count = 1 Then
					Dim outputPoint As Point3D

					' To avoid an edge cases caused by generating a point extremely
					' close to one of the bound points, we test if both points are inside
					' the bounds to be on the safe side - in the worst case we do 
					' extra work or generate a small edge
					If IsPointInTriangle(visEdgeStart, uv, p, outputPoint) Then
						edgeList.Add(New HitTestEdge(pointList(0), outputPoint, uvList(0), visEdgeStart))
					End If

					If IsPointInTriangle(visEdgeEnd, uv, p, outputPoint) Then
						edgeList.Add(New HitTestEdge(pointList(0), outputPoint, uvList(0), visEdgeEnd))
					End If
				Else
					Dim outputPoint1 As Point3D, outputPoint2 As Point3D

					If IsPointInTriangle(visEdgeStart, uv, p, outputPoint1) AndAlso IsPointInTriangle(visEdgeEnd, uv, p, outputPoint2) Then
						edgeList.Add(New HitTestEdge(outputPoint1, outputPoint2, visEdgeStart, visEdgeEnd))
					End If
				End If
			End If
		Next
	End Sub

	''' <summary>
	''' Function tests to see if the given texture coordinate point p is contained within the 
	''' given triangle.  If it is it returns the 3D point corresponding to that intersection.
	''' </summary>
	''' <param name="p">The point to test</param>
	''' <param name="triUVVertices">The texture coordinates of the triangle</param>
	''' <param name="tri3DVertices">The 3D coordinates of the triangle</param>
	''' <param name="inters3DPoint">The 3D point of intersection</param>
	''' <returns>True if the point is in the triangle, false otherwise</returns>
	Private Function IsPointInTriangle(p As Point, triUVVertices As Point(), tri3DVertices As Point3D(), ByRef inters3DPoint As Point3D) As Boolean
		Dim denom As Double = 0.0
		inters3DPoint = New Point3D()

		Dim A As Double = triUVVertices(0).X - triUVVertices(2).X
		Dim B As Double = triUVVertices(1).X - triUVVertices(2).X
		Dim C As Double = triUVVertices(2).X - p.X
		Dim D As Double = triUVVertices(0).Y - triUVVertices(2).Y
		Dim E As Double = triUVVertices(1).Y - triUVVertices(2).Y
		Dim F As Double = triUVVertices(2).Y - p.Y

		denom = (A * E - B * D)
		If denom = 0 Then
			Return False
		End If
		Dim lambda1 As Double = (B * F - C * E) / denom

		denom = (B * D - A * E)
		If denom = 0 Then
			Return False
		End If
		Dim lambda2 As Double = (A * F - C * D) / denom

		If lambda1 < 0 OrElse lambda1 > 1 OrElse lambda2 < 0 OrElse lambda2 > 1 OrElse (lambda1 + lambda2) > 1 Then
			Return False
		End If

		inters3DPoint = CType(lambda1 * CType(tri3DVertices(0), Vector3D) + lambda2 * CType(tri3DVertices(1), Vector3D) + (1F - lambda1 - lambda2) * CType(tri3DVertices(2), Vector3D), Point3D)

		Return True
	End Function

	''' <summary>
	''' Handles adding an edge when the two line segments are coincident.
	''' </summary>
	''' <param name="visUV1">The texture coordinates of the boundary edge</param>
	''' <param name="visUV2">The texture coordinates of the boundary edge</param>
	''' <param name="tri3D1">The 3D coordinate of the triangle edge</param>
	''' <param name="tri3D2">The 3D coordinates of the triangle edge</param>
	''' <param name="triUV1">The texture coordinates of the triangle edge</param>
	''' <param name="triUV2">The texture coordinates of the triangle edge</param>
	''' <param name="edgeList">The edge list to add to</param>
	Private Sub HandleCoincidentLines(visUV1 As Point, visUV2 As Point, tri3D1 As Point3D, tri3D2 As Point3D, triUV1 As Point, triUV2 As Point, _
		edgeList As List(Of HitTestEdge))
		Dim minVisUV As Point, maxVisUV As Point

		Dim minTriUV As Point, maxTriUV As Point
		Dim minTri3D As Point3D, maxTri3D As Point3D

		' to be used in final edge creation
		Dim uv1 As Point, uv2 As Point
		Dim p1 As Point3D, p2 As Point3D

		' order the points and give refs to them for ease of use
		If Math.Abs(visUV1.X - visUV2.X) > Math.Abs(visUV1.Y - visUV2.Y) Then
			If visUV1.X <= visUV2.X Then
				minVisUV = visUV1
				maxVisUV = visUV2
			Else
				minVisUV = visUV2
				maxVisUV = visUV1
			End If

			If triUV1.X <= triUV2.X Then
				minTriUV = triUV1
				minTri3D = tri3D1

				maxTriUV = triUV2
				maxTri3D = tri3D2
			Else
				minTriUV = triUV2
				minTri3D = tri3D2

				maxTriUV = triUV1
				maxTri3D = tri3D1
			End If

			' now actually create the edge           
			' compute the minimum value
			If minVisUV.X < minTriUV.X Then
				uv1 = minTriUV
				p1 = minTri3D
			Else
				uv1 = minVisUV
				p1 = minTri3D + (minVisUV.X - minTriUV.X) / (maxTriUV.X - minTriUV.X) * (maxTri3D - minTri3D)
			End If

			' compute the maximum value
			If maxVisUV.X > maxTriUV.X Then
				uv2 = maxTriUV
				p2 = maxTri3D
			Else
				uv2 = maxVisUV
				p2 = minTri3D + (maxVisUV.X - minTriUV.X) / (maxTriUV.X - minTriUV.X) * (maxTri3D - minTri3D)
			End If
		Else
			If visUV1.Y <= visUV2.Y Then
				minVisUV = visUV1
				maxVisUV = visUV2
			Else
				minVisUV = visUV2
				maxVisUV = visUV1
			End If

			If triUV1.Y <= triUV2.Y Then
				minTriUV = triUV1
				minTri3D = tri3D1

				maxTriUV = triUV2
				maxTri3D = tri3D2
			Else
				minTriUV = triUV2
				minTri3D = tri3D2

				maxTriUV = triUV1
				maxTri3D = tri3D1
			End If

			' now actually create the edge           
			' compute the minimum value
			If minVisUV.Y < minTriUV.Y Then
				uv1 = minTriUV
				p1 = minTri3D
			Else
				uv1 = minVisUV
				p1 = minTri3D + (minVisUV.Y - minTriUV.Y) / (maxTriUV.Y - minTriUV.Y) * (maxTri3D - minTri3D)
			End If

			' compute the maximum value
			If maxVisUV.Y > maxTriUV.Y Then
				uv2 = maxTriUV
				p2 = maxTri3D
			Else
				uv2 = maxVisUV
				p2 = minTri3D + (maxVisUV.Y - minTriUV.Y) / (maxTriUV.Y - minTriUV.Y) * (maxTri3D - minTri3D)
			End If
		End If

		' add the edge
		edgeList.Add(New HitTestEdge(p1, p2, uv1, uv2))
	End Sub

	''' <summary>
	''' Intersects a ray with the line specified by the passed in end points.  The parameterized coordinate along the ray of
	''' intersection is returned.  
	''' </summary>
	''' <param name="o">The ray origin</param>
	''' <param name="d">The ray direction</param>
	''' <param name="p1">First point of the line to intersect against</param>
	''' <param name="p2">Second point of the line to intersect against</param>
	''' <param name="coinc">Whether the ray and line are coincident</param>        
	''' <returns>
	''' The parameter along the ray of the point of intersection.
	''' If the ray and line are parallel and not coincident, this will be -1.
	''' </returns>
	Private Function IntersectRayLine(o As Point, d As Vector, p1 As Point, p2 As Point, ByRef coinc As Boolean) As Double
		coinc = False

		' deltas
		Dim dy As Double = p2.Y - p1.Y
		Dim dx As Double = p2.X - p1.X

		' handle case of a vertical line
		If dx = 0 Then
			If d.X = 0 Then
				coinc = (o.X = p1.X)
				Return -1
			Else
				Return (p2.X - o.X) / d.X
			End If
		End If

		' now need to do more general intersection
		Dim numer As Double = (o.X - p1.X) * dy / dx - o.Y + p1.Y
		Dim denom As Double = (d.Y - d.X * dy / dx)

		' if denominator is zero, then the lines are parallel
		If denom = 0 Then
			Dim b0 As Double = -o.X * dy / dx + o.Y
			Dim b1 As Double = -p1.X * dy / dx + p1.Y

			coinc = (b0 = b1)
			Return -1
		Else
			Return (numer / denom)
		End If
	End Function

	''' <summary>
	''' Helper structure to represent an edge
	''' </summary>
	Private Structure Edge
		Public Sub New(s As Point3D, e As Point3D)
			_start = s
			_end = e
		End Sub

		Public _start As Point3D
		Public _end As Point3D
	End Structure

	''' <summary>
	''' Information about an edge such as whether it belongs to a front/back facing
	''' triangle, the texture coordinates for the edge, and how many polygons refer
	''' to that edge.
	''' </summary>
	Private Class EdgeInfo
		Public Sub New()
			hasFrontFace = InlineAssignHelper(hasBackFace, False)
			numSharing = 0
		End Sub

		Public hasFrontFace As Boolean
		Public hasBackFace As Boolean
		Public uv1 As Point
		Public uv2 As Point
		Public numSharing As Integer
		Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
			target = value
			Return value
		End Function
	End Class

	''' <summary>
	''' Processes the edges of the given triangle.  It does so by updating
	''' the adjacency information based on the direction the polygon is facing.
	''' If there is a silhouette edge found, then this edge is added to the list
	''' of edges if it is within the texture coordinate bounds passed to the function.
	''' </summary>
	''' <param name="p">The triangle's vertices</param>
	''' <param name="uv">The texture coordinates for those vertices</param>
	''' <param name="tc">The texture coordinate edges being searched for</param>
	''' <param name="polygonSide">Which side the polygon is facing (greateer than 0 front, less than 0 back)</param>
	''' <param name="edgeList">The list of edges comprosing the visual outline</param>
	''' <param name="adjInformation">The adjacency information structure</param>
	Private Sub ProcessTriangleEdges(p As Point3D(), uv As Point(), tc As Point(), polygonSide__1 As PolygonSide, edgeList As List(Of HitTestEdge), adjInformation As Dictionary(Of Edge, EdgeInfo))
		' loop over all the edges and add them to the adjacency list
		For i As Integer = 0 To p.Length - 1
			Dim uv1 As Point, uv2 As Point
			Dim p3D1 As Point3D = p(i)
			Dim p3D2 As Point3D = p((i + 1) Mod p.Length)

			Dim edge As Edge

			' order the edge points so insertion in to adjInformation is consistent
			If p3D1.X < p3D2.X OrElse (p3D1.X = p3D2.X AndAlso p3D1.Y < p3D2.Y) OrElse (p3D1.X = p3D2.X AndAlso p3D1.Y = p3D2.Y AndAlso p3D1.Z < p3D1.Z) Then
				edge = New Edge(p3D1, p3D2)
				uv1 = uv(i)
				uv2 = uv((i + 1) Mod p.Length)
			Else
				edge = New Edge(p3D2, p3D1)
				uv2 = uv(i)
				uv1 = uv((i + 1) Mod p.Length)
			End If

			' look up the edge information
			Dim edgeInfo As EdgeInfo
			If adjInformation.ContainsKey(edge) Then
				edgeInfo = adjInformation(edge)
			Else
				edgeInfo = New EdgeInfo()
				adjInformation(edge) = edgeInfo
			End If
			edgeInfo.numSharing += 1

			' whether or not the edge has already been added to the edge list
			Dim alreadyAdded As Boolean = edgeInfo.hasBackFace AndAlso edgeInfo.hasFrontFace

			' add the edge to the info list
			If polygonSide__1 = PolygonSide.FRONT Then
				edgeInfo.hasFrontFace = True
				edgeInfo.uv1 = uv1
				edgeInfo.uv2 = uv2
			Else
				edgeInfo.hasBackFace = True
			End If

			' if the sides are different we may need to add an edge
			If Not alreadyAdded AndAlso edgeInfo.hasBackFace AndAlso edgeInfo.hasFrontFace Then
				HandleSilhouetteEdge(edgeInfo.uv1, edgeInfo.uv2, edge._start, edge._end, tc, edgeList)
			End If
		Next
	End Sub

	''' <summary>
	''' Handles intersecting a silhouette edge against the passed in texture coordinate 
	''' bounds.  It behaves similarly to the case of intersection the bounds with a triangle 
	''' except the testing order is switched.
	''' </summary>
	''' <param name="uv1">The texture coordinates of the edge</param>
	''' <param name="uv2">The texture coordinates of the edge</param>
	''' <param name="p3D1">The 3D point of the edge</param>
	''' <param name="p3D2">The 3D point of the edge</param>
	''' <param name="bounds">The texture coordinate bounds</param>
	''' <param name="edgeList">The list of edges</param>
	Private Sub HandleSilhouetteEdge(uv1 As Point, uv2 As Point, p3D1 As Point3D, p3D2 As Point3D, bounds As Point(), edgeList As List(Of HitTestEdge))
		Dim pointList As New List(Of Point3D)()
		Dim uvList As New List(Of Point)()
		Dim dir As Vector = uv2 - uv1

		' loop over object bounds
		For i As Integer = 0 To bounds.Length - 1
			Dim visEdgeStart As Point = bounds(i)
			Dim visEdgeEnd As Point = bounds((i + 1) Mod bounds.Length)

			' initial rejection processing
			If Not ((Math.Max(visEdgeStart.X, visEdgeEnd.X) < Math.Min(uv1.X, uv2.X)) OrElse (Math.Min(visEdgeStart.X, visEdgeEnd.X) > Math.Max(uv1.X, uv2.X)) OrElse (Math.Max(visEdgeStart.Y, visEdgeEnd.Y) < Math.Min(uv1.Y, uv2.Y)) OrElse (Math.Min(visEdgeStart.Y, visEdgeEnd.Y) > Math.Max(uv1.Y, uv2.Y))) Then
				' intersect the two lines
				Dim areCoincident As Boolean = False
				Dim t As Double = IntersectRayLine(uv1, dir, visEdgeStart, visEdgeEnd, areCoincident)

				' silhouette edge processing will only include non-coincident lines
				If areCoincident Then
					' if it's coincident, we'll let the normal processing handle this edge
					Return
				ElseIf t >= 0 AndAlso t <= 1 Then
					Dim intersUV As Point = uv1 + dir * t
					Dim intersPoint3D As Point3D = p3D1 + (p3D2 - p3D1) * t

					Dim visEdgeDiff As Double = (visEdgeStart - visEdgeEnd).Length

					If (intersUV - visEdgeStart).Length < visEdgeDiff AndAlso (intersUV - visEdgeEnd).Length < visEdgeDiff Then
						pointList.Add(intersPoint3D)
						uvList.Add(intersUV)
					End If
				End If
			End If
		Next

		If pointList.Count >= 2 Then
			edgeList.Add(New HitTestEdge(pointList(0), pointList(1), uvList(0), uvList(1)))
		ElseIf pointList.Count = 1 Then
			' for the case that uv1/2 is actually a point on or extremely close to the bounds
			' of the polygon, we do the pointinpolygon test on both to avoid any numerical
			' precision issues - in the worst case we end up with a very small edge and
			' the right edge
			If IsPointInPolygon(bounds, uv1) Then
				edgeList.Add(New HitTestEdge(pointList(0), p3D1, uvList(0), uv1))
			End If
			If IsPointInPolygon(bounds, uv2) Then
				edgeList.Add(New HitTestEdge(pointList(0), p3D2, uvList(0), uv2))
			End If
		Else
			If IsPointInPolygon(bounds, uv1) AndAlso IsPointInPolygon(bounds, uv2) Then
				edgeList.Add(New HitTestEdge(p3D1, p3D2, uv1, uv2))
			End If
		End If
	End Sub

	''' <summary>
	''' Function tests to see whether the point p is contained within the polygon
	''' specified by the list of points passed to the function.  p is considered within
	''' this polygon if it is on the same side of all the edges.  A point on any of
	''' the edges of the polygon is not considered within the polygon.
	''' </summary>
	''' <param name="polygon">The polygon to test against</param>
	''' <param name="p">The point to be tested against</param>
	''' <returns>Whether the point is in the polygon</returns>
	Private Function IsPointInPolygon(polygon As Point(), p As Point) As Boolean
		Dim sign As Boolean = False

		For i As Integer = 0 To polygon.Length - 1
			Dim crossProduct As Double = Vector.CrossProduct(polygon((i + 1) Mod polygon.Length) - polygon(i), polygon(i) - p)

			Dim currSign As Boolean = crossProduct > 0

			If i = 0 Then
				sign = currSign
			Else
				If sign <> currSign Then
					Return False
				End If
			End If
		Next

		Return True
	End Function

	''' <summary>
	''' GenerateMaterial creates the material for the InteractiveModelVisual3D.  The
	''' material is composed of the Visual, which is displayed on a VisualBrush on a 
	''' DiffuseMaterial, as well as any post materials which are also applied.
	''' </summary>
	Private Sub GenerateMaterial()
		Dim material__1 As Material

		' begin order dependent operations            
		InternalVisualBrush.Visual = Nothing
		InternalVisualBrush = CreateVisualBrush()

		material__1 = Material.Clone()
		_content.Material = material__1

		InternalVisualBrush.Visual = InternalVisual

		SwapInVisualBrush(material__1)
		' end order dependent operations

		If IsBackVisible Then
			_content.BackMaterial = material__1
		End If
	End Sub

	''' <summary>
	''' Creates the VisualBrush that will be used to hold the interactive
	''' 2D content.
	''' </summary>
	''' <returns>The VisualBrush to hold the interactive 2D content</returns>
	Private Function CreateVisualBrush() As VisualBrush
		Dim vb As New VisualBrush()
		RenderOptions.SetCachingHint(vb, CachingHint.Cache)
		vb.ViewportUnits = BrushMappingMode.Absolute
		vb.TileMode = TileMode.None

		Return vb
	End Function

	''' <summary>
	''' Replaces any instances of the sentinal brush with the internal visual brush
	''' </summary>
	''' <param name="material">The material to look through</param>
	Private Sub SwapInVisualBrush(material As Material)
		Dim foundMaterialToSwap As Boolean = False
		Dim materialStack As New Stack(Of Material)()
		materialStack.Push(material)

		While materialStack.Count > 0
			Dim currMaterial As Material = materialStack.Pop()

			If TypeOf currMaterial Is DiffuseMaterial Then
				Dim diffMaterial As DiffuseMaterial = DirectCast(currMaterial, DiffuseMaterial)
				If CType(diffMaterial.GetValue(InteractiveVisual3D.IsInteractiveMaterialProperty), [Boolean]) Then
					diffMaterial.Brush = InternalVisualBrush
					foundMaterialToSwap = True
				End If
			ElseIf TypeOf currMaterial Is EmissiveMaterial Then
				Dim emmMaterial As EmissiveMaterial = DirectCast(currMaterial, EmissiveMaterial)
				If CType(emmMaterial.GetValue(InteractiveVisual3D.IsInteractiveMaterialProperty), [Boolean]) Then
					emmMaterial.Brush = InternalVisualBrush
					foundMaterialToSwap = True
				End If
			ElseIf TypeOf currMaterial Is SpecularMaterial Then
				Dim specMaterial As SpecularMaterial = DirectCast(currMaterial, SpecularMaterial)
				If CType(specMaterial.GetValue(InteractiveVisual3D.IsInteractiveMaterialProperty), [Boolean]) Then
					specMaterial.Brush = InternalVisualBrush
					foundMaterialToSwap = True
				End If
			ElseIf TypeOf currMaterial Is MaterialGroup Then
				Dim matGroup As MaterialGroup = DirectCast(currMaterial, MaterialGroup)
				For Each m As Material In matGroup.Children
					materialStack.Push(m)
				Next
			Else
				Throw New ArgumentException("material needs to be either a DiffuseMaterial, EmissiveMaterial, SpecularMaterial or a MaterialGroup", "material")
			End If
		End While

		' make sure there is at least one interactive material
		If Not foundMaterialToSwap Then
			Throw New ArgumentException("material needs to contain at least one material that has the IsInteractiveMaterial attached property", "material")
		End If
	End Sub

	''' <summary>
	''' The visual applied to the VisualBrush, which is then used on the 3D object
	''' </summary>
	Private Shared VisualProperty As DependencyProperty = DependencyProperty.Register("Visual", GetType(Visual), GetType(InteractiveVisual3D), New PropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnVisualChanged)))

	Public Property Visual() As Visual
		Get
			Return DirectCast(GetValue(VisualProperty), Visual)
		End Get
		Set
			SetValue(VisualProperty, value)
		End Set
	End Property

	''' <summary>
	''' The actual visual being placed on the brush.
	''' so that the patterns on visuals caused by tabbing, etc... work, 
	''' we wrap the Visual DependencyProperty in a AdornerDecorator.
	''' </summary>
	Friend ReadOnly Property InternalVisual() As UIElement
		Get
			Return _internalVisual
		End Get
	End Property

	''' <summary>
	''' The visual brush that the internal visual is contained on.
	''' </summary>
	Private Property InternalVisualBrush() As VisualBrush
		Get
			Return _visualBrush
		End Get

		Set
			_visualBrush = value
		End Set
	End Property

	Friend Shared Sub OnVisualChanged(sender As [Object], e As DependencyPropertyChangedEventArgs)
		Dim imv3D As InteractiveVisual3D = DirectCast(sender, InteractiveVisual3D)
		Dim ad As AdornerDecorator = Nothing
		If imv3D.InternalVisual IsNot Nothing Then
			ad = DirectCast(imv3D.InternalVisual, AdornerDecorator)
			If TypeOf ad.Child Is VisualDecorator Then
				Dim oldVisualDecorator As VisualDecorator = DirectCast(ad.Child, VisualDecorator)
				oldVisualDecorator.Content = Nothing
			End If
		End If
		' so that the patterns on visuals caused by tabbing, etc... work, 
		' we put an adorner layer here so that anything adorned gets adorned 
		' within the visual and not at the adorner layer on the window
		If ad Is Nothing Then
			ad = New AdornerDecorator()
		End If
		Dim adornerDecoratorChild As UIElement
		If TypeOf imv3D.Visual Is UIElement Then
			adornerDecoratorChild = DirectCast(imv3D.Visual, UIElement)
		Else
			Dim visDecorator As New VisualDecorator()
			visDecorator.Content = imv3D.Visual
			adornerDecoratorChild = visDecorator
		End If
		ad.Child = Nothing
		ad.Child = adornerDecoratorChild
		imv3D._internalVisual = ad
		imv3D.InternalVisualBrush.Visual = imv3D.InternalVisual
	End Sub


	''' <summary>
	''' The BackFaceVisibleProperty specifies whether or not the back face of the 3D object
	''' should be considered visible.  If it is then when generating the material, the back material
	''' is also set.
	''' </summary>
	Private Shared ReadOnly IsBackVisibleProperty As DependencyProperty = DependencyProperty.Register("IsBackVisible", GetType(Boolean), GetType(InteractiveVisual3D), New PropertyMetadata(False, New PropertyChangedCallback(AddressOf OnIsBackVisiblePropertyChanged)))

	Public Property IsBackVisible() As Boolean
		Get
			Return CBool(GetValue(IsBackVisibleProperty))
		End Get
		Set
			SetValue(IsBackVisibleProperty, value)
		End Set
	End Property

	Friend Shared Sub OnIsBackVisiblePropertyChanged(sender As [Object], e As DependencyPropertyChangedEventArgs)
		Dim imv3D As InteractiveVisual3D = DirectCast(sender, InteractiveVisual3D)

		If imv3D.IsBackVisible Then
			imv3D._content.BackMaterial = imv3D._content.Material
		Else
			imv3D._content.BackMaterial = Nothing
		End If
	End Sub



	''' <summary>
	''' The emissive color of the material
	''' </summary>
	Private Shared ReadOnly _defaultMaterialPropertyValue As DiffuseMaterial
	Public Shared ReadOnly MaterialProperty As DependencyProperty

	Public Property Material() As Material
		Get
			Return DirectCast(GetValue(MaterialProperty), Material)
		End Get
		Set
			SetValue(MaterialProperty, value)
		End Set
	End Property

	Friend Shared Sub OnMaterialPropertyChanged(sender As [Object], e As DependencyPropertyChangedEventArgs)
		Dim imv3D As InteractiveVisual3D = DirectCast(sender, InteractiveVisual3D)

		imv3D.GenerateMaterial()
	End Sub

	''' <summary>
	''' The 3D geometry that the InteractiveModelVisual3D represents
	''' </summary>
	Public Shared ReadOnly GeometryProperty As DependencyProperty = DependencyProperty.Register("Geometry", GetType(Geometry3D), GetType(InteractiveVisual3D), New PropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnGeometryChanged)))

	Public Property Geometry() As Geometry3D
		Get
			Return DirectCast(GetValue(GeometryProperty), Geometry3D)
		End Get
		Set
			SetValue(GeometryProperty, value)
		End Set
	End Property

	Friend Shared Sub OnGeometryChanged(sender As [Object], e As DependencyPropertyChangedEventArgs)
		Dim imv3D As InteractiveVisual3D = DirectCast(sender, InteractiveVisual3D)

		imv3D._content.Geometry = imv3D.Geometry
	End Sub

	''' <summary>
	''' The attached dependency property used to indicate whether a material should be made
	''' interactive.
	''' </summary>
	Public Shared ReadOnly IsInteractiveMaterialProperty As DependencyProperty = DependencyProperty.RegisterAttached("IsInteractiveMaterial", GetType([Boolean]), GetType(InteractiveVisual3D), New PropertyMetadata(False))

	Public Shared Sub SetIsInteractiveMaterial(element As UIElement, value As [Boolean])
		element.SetValue(IsInteractiveMaterialProperty, value)
	End Sub
	Public Shared Function GetIsInteractiveMaterial(element As UIElement) As [Boolean]
		Return CType(element.GetValue(IsInteractiveMaterialProperty), [Boolean])
	End Function

	''' <summary>
	''' Done so that the Content property is not serialized and not visible by a visual designer
	''' </summary>
	<EditorBrowsableAttribute(EditorBrowsableState.Never)> _
	<DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)> _
	Public Shadows Property Content() As Model3D
		Get
			Return MyBase.Content
		End Get
		Set
			MyBase.Content = value
		End Set
	End Property

	'------------------------------------------------------------------------
	'
	' PRIVATE DATA
	'
	'------------------------------------------------------------------------

	Private Enum PolygonSide
		FRONT
		BACK
	End Enum

	' the geometry model that represents this visual3D
	Friend ReadOnly _content As GeometryModel3D

	' helper functions to cache the last created visual edges and also to tell
	' if we need to recompute these values, or can use the cache
	Private _lastVisCorners As Point() = Nothing
	Private _lastEdges As List(Of HitTestEdge) = Nothing
	Private _lastMatrix3D As Matrix3D

	' the actual visual that is created
	Private _internalVisual As UIElement

	Private _visualBrush As VisualBrush
End Class

''' <summary>
''' The VisualDecorator class simply holds one Visual as a child.  It is used
''' to provide a bridge between the AdornerDecorator and the Visual that 
''' is intended to be placed on the 3D mesh.  The reason being that AdornerDecorator
''' only takes a UIElement as a child - so in the case that a Visual (non UI/FE) 
''' is to be placed on the 3D mesh, a VisualDecorator is needed to provide that
''' bridge.
''' </summary>
Friend Class VisualDecorator
	Inherits FrameworkElement
	Public Sub New()
		_visual = Nothing
	End Sub

	''' <summary>
	''' The content/child of the VisualDecorator.
	''' </summary>
	Public Property Content() As Visual
		Get
			Return _visual
		End Get

		Set
			' check to make sure we're attempting to set something new
			If _visual IsNot value Then
				Dim oldVisual As Visual = _visual
				Dim newVisual As Visual = value

				' remove the previous child
				RemoveVisualChild(oldVisual)
				RemoveLogicalChild(oldVisual)

				' set the private variable
				_visual = value

				' link in the new child
				AddLogicalChild(newVisual)
				AddVisualChild(newVisual)
			End If
		End Set
	End Property

	''' <summary>
	''' Returns the number of Visual children this element has.
	''' </summary>
	Protected Overrides ReadOnly Property VisualChildrenCount() As Integer
		Get
			Return (If(Content IsNot Nothing, 1, 0))
		End Get
	End Property

	''' <summary>
	''' Returns the child at the specified index.
	''' </summary>
	Protected Overrides Function GetVisualChild(index As Integer) As Visual
		If index = 0 AndAlso Content IsNot Nothing Then
			Return _visual
		End If

		' if we didn't return then the index is out of range - throw an error
		Throw New ArgumentOutOfRangeException("index", index, "Out of range visual requested")
	End Function

	''' <summary> 
	''' Returns an enumertor to this element's logical children
	''' </summary>
	Protected Overrides ReadOnly Property LogicalChildren() As IEnumerator
		Get
			Dim logicalChildren__1 As Visual() = New Visual(VisualChildrenCount - 1) {}
			For i As Integer = 0 To VisualChildrenCount - 1
				logicalChildren__1(i) = GetVisualChild(i)
			Next

			Return logicalChildren__1.GetEnumerator()
		End Get
	End Property

	' the visual being referenced
	Private _visual As Visual
End Class
