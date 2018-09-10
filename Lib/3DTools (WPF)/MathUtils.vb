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

Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public NotInheritable Class MathUtils
	Private Sub New()
	End Sub
	Public Shared Function GetAspectRatio(size As Size) As Double
		Return size.Width / size.Height
	End Function

	Public Shared Function DegreesToRadians(degrees As Double) As Double
		Return degrees * (Math.PI / 180.0)
	End Function

	Private Shared Function GetViewMatrix(camera As ProjectionCamera) As Matrix3D
		Debug.Assert(camera IsNot Nothing, "Caller needs to ensure camera is non-null.")

		' This math is identical to what you find documented for
		' D3DXMatrixLookAtRH with the exception that WPF uses a
		' LookDirection vector rather than a LookAt point.

		Dim zAxis As Vector3D = -camera.LookDirection
		zAxis.Normalize()

		Dim xAxis As Vector3D = Vector3D.CrossProduct(camera.UpDirection, zAxis)
		xAxis.Normalize()

		Dim yAxis As Vector3D = Vector3D.CrossProduct(zAxis, xAxis)

		Dim position As Vector3D = CType(camera.Position, Vector3D)
		Dim offsetX As Double = -Vector3D.DotProduct(xAxis, position)
		Dim offsetY As Double = -Vector3D.DotProduct(yAxis, position)
		Dim offsetZ As Double = -Vector3D.DotProduct(zAxis, position)

		Return New Matrix3D(xAxis.X, yAxis.X, zAxis.X, 0, xAxis.Y, yAxis.Y, _
			zAxis.Y, 0, xAxis.Z, yAxis.Z, zAxis.Z, 0, _
			offsetX, offsetY, offsetZ, 1)
	End Function

	''' <summary>
	'''     Computes the effective view matrix for the given
	'''     camera.
	''' </summary>
	Public Shared Function GetViewMatrix(camera As Camera) As Matrix3D
		If camera Is Nothing Then
			Throw New ArgumentNullException("camera")
		End If

		Dim projectionCamera As ProjectionCamera = TryCast(camera, ProjectionCamera)

		If projectionCamera IsNot Nothing Then
			Return GetViewMatrix(projectionCamera)
		End If

		Dim matrixCamera As MatrixCamera = TryCast(camera, MatrixCamera)

		If matrixCamera IsNot Nothing Then
			Return matrixCamera.ViewMatrix
		End If

		Throw New ArgumentException([String].Format("Unsupported camera type '{0}'.", camera.[GetType]().FullName), "camera")
	End Function

	Private Shared Function GetProjectionMatrix(camera As OrthographicCamera, aspectRatio As Double) As Matrix3D
		Debug.Assert(camera IsNot Nothing, "Caller needs to ensure camera is non-null.")

		' This math is identical to what you find documented for
		' D3DXMatrixOrthoRH with the exception that in WPF only
		' the camera's width is specified.  Height is calculated
		' from width and the aspect ratio.

		Dim w As Double = camera.Width
		Dim h As Double = w / aspectRatio
		Dim zn As Double = camera.NearPlaneDistance
		Dim zf As Double = camera.FarPlaneDistance

		Dim m33 As Double = 1 / (zn - zf)
		Dim m43 As Double = zn * m33

		Return New Matrix3D(2 / w, 0, 0, 0, 0, 2 / h, _
			0, 0, 0, 0, m33, 0, _
			0, 0, m43, 1)
	End Function

	Private Shared Function GetProjectionMatrix(camera As PerspectiveCamera, aspectRatio As Double) As Matrix3D
		Debug.Assert(camera IsNot Nothing, "Caller needs to ensure camera is non-null.")

		' This math is identical to what you find documented for
		' D3DXMatrixPerspectiveFovRH with the exception that in
		' WPF the camera's horizontal rather the vertical
		' field-of-view is specified.

		Dim hFoV As Double = MathUtils.DegreesToRadians(camera.FieldOfView)
		Dim zn As Double = camera.NearPlaneDistance
		Dim zf As Double = camera.FarPlaneDistance

		Dim xScale As Double = 1 / Math.Tan(hFoV / 2)
		Dim yScale As Double = aspectRatio * xScale
		Dim m33 As Double = If((zf = Double.PositiveInfinity), -1, (zf / (zn - zf)))
		Dim m43 As Double = zn * m33

		Return New Matrix3D(xScale, 0, 0, 0, 0, yScale, _
			0, 0, 0, 0, m33, -1, _
			0, 0, m43, 0)
	End Function

	''' <summary>
	'''     Computes the effective projection matrix for the given
	'''     camera.
	''' </summary>
	Public Shared Function GetProjectionMatrix(camera As Camera, aspectRatio As Double) As Matrix3D
		If camera Is Nothing Then
			Throw New ArgumentNullException("camera")
		End If

		Dim perspectiveCamera As PerspectiveCamera = TryCast(camera, PerspectiveCamera)

		If perspectiveCamera IsNot Nothing Then
			Return GetProjectionMatrix(perspectiveCamera, aspectRatio)
		End If

		Dim orthographicCamera As OrthographicCamera = TryCast(camera, OrthographicCamera)

		If orthographicCamera IsNot Nothing Then
			Return GetProjectionMatrix(orthographicCamera, aspectRatio)
		End If

		Dim matrixCamera As MatrixCamera = TryCast(camera, MatrixCamera)

		If matrixCamera IsNot Nothing Then
			Return matrixCamera.ProjectionMatrix
		End If

		Throw New ArgumentException([String].Format("Unsupported camera type '{0}'.", camera.[GetType]().FullName), "camera")
	End Function

	Private Shared Function GetHomogeneousToViewportTransform(viewport As Rect) As Matrix3D
		Dim scaleX As Double = viewport.Width / 2
		Dim scaleY As Double = viewport.Height / 2
		Dim offsetX As Double = viewport.X + scaleX
		Dim offsetY As Double = viewport.Y + scaleY

		Return New Matrix3D(scaleX, 0, 0, 0, 0, -scaleY, _
			0, 0, 0, 0, 1, 0, _
			offsetX, offsetY, 0, 1)
	End Function

	''' <summary>
	'''     Computes the transform from world space to the Viewport3DVisual's
	'''     inner 2D space.
	''' 
	'''     This method can fail if Camera.Transform is non-invertable
	'''     in which case the camera clip planes will be coincident and
	'''     nothing will render.  In this case success will be false.
	''' </summary>
	Public Shared Function TryWorldToViewportTransform(visual As Viewport3DVisual, ByRef success As Boolean) As Matrix3D
		success = False
		Dim result As Matrix3D = TryWorldToCameraTransform(visual, success)

		If success Then
			result.Append(GetProjectionMatrix(visual.Camera, MathUtils.GetAspectRatio(visual.Viewport.Size)))
			result.Append(GetHomogeneousToViewportTransform(visual.Viewport))
			success = True
		End If

		Return result
	End Function


	''' <summary>
	'''     Computes the transform from world space to camera space
	''' 
	'''     This method can fail if Camera.Transform is non-invertable
	'''     in which case the camera clip planes will be coincident and
	'''     nothing will render.  In this case success will be false.
	''' </summary>
	Public Shared Function TryWorldToCameraTransform(visual As Viewport3DVisual, ByRef success As Boolean) As Matrix3D
		success = False
		Dim result As Matrix3D = Matrix3D.Identity

		Dim camera As Camera = visual.Camera

		If camera Is Nothing Then
			Return ZeroMatrix
		End If

		Dim viewport As Rect = visual.Viewport

		If viewport = Rect.Empty Then
			Return ZeroMatrix
		End If

		Dim cameraTransform As Transform3D = camera.Transform

		If cameraTransform IsNot Nothing Then
			Dim m As Matrix3D = cameraTransform.Value

			If Not m.HasInverse Then
				Return ZeroMatrix
			End If

			m.Invert()
			result.Append(m)
		End If

		result.Append(GetViewMatrix(camera))

		success = True
		Return result
	End Function

	''' <summary>
	''' Gets the object space to world space transformation for the given DependencyObject
	''' </summary>
	''' <param name="visual">The visual whose world space transform should be found</param>
	''' <param name="viewport">The Viewport3DVisual the Visual is contained within</param>
	''' <returns>The world space transformation</returns>
	Private Shared Function GetWorldTransformationMatrix(visual As DependencyObject, ByRef viewport As Viewport3DVisual) As Matrix3D
		Dim worldTransform As Matrix3D = Matrix3D.Identity
		viewport = Nothing

		If Not (TypeOf visual Is Visual3D) Then
			Throw New ArgumentException("Must be of type Visual3D.", "visual")
		End If

		While visual IsNot Nothing
			If Not (TypeOf visual Is ModelVisual3D) Then
				Exit While
			End If

			Dim transform As Transform3D = DirectCast(visual.GetValue(ModelVisual3D.TransformProperty), Transform3D)

			If transform IsNot Nothing Then
				worldTransform.Append(transform.Value)
			End If

			visual = VisualTreeHelper.GetParent(visual)
		End While

		viewport = TryCast(visual, Viewport3DVisual)

		If viewport Is Nothing Then
			If visual IsNot Nothing Then
				' In WPF 3D v1 the only possible configuration is a chain of
				' ModelVisual3Ds leading up to a Viewport3DVisual.

				Throw New ApplicationException([String].Format("Unsupported type: '{0}'.  Expected tree of ModelVisual3Ds leading up to a Viewport3DVisual.", visual.[GetType]().FullName))
			End If

			Return ZeroMatrix
		End If

		Return worldTransform
	End Function

	''' <summary>
	'''     Computes the transform from the inner space of the given
	'''     Visual3D to the 2D space of the Viewport3DVisual which
	'''     contains it.
	''' 
	'''     The result will contain the transform of the given visual.
	''' 
	'''     This method can fail if Camera.Transform is non-invertable
	'''     in which case the camera clip planes will be coincident and
	'''     nothing will render.  In this case success will be false.
	''' </summary>
	''' <param name="visual"></param>
	''' <param name="success"></param>
	''' <returns></returns>
	Public Shared Function TryTransformTo2DAncestor(visual As DependencyObject, ByRef viewport As Viewport3DVisual, ByRef success As Boolean) As Matrix3D
		Dim to2D As Matrix3D = GetWorldTransformationMatrix(visual, viewport)
		to2D.Append(MathUtils.TryWorldToViewportTransform(viewport, success))

		If Not success Then
			Return ZeroMatrix
		End If

		Return to2D
	End Function


	''' <summary>
	'''     Computes the transform from the inner space of the given
	'''     Visual3D to the camera coordinate space
	''' 
	'''     The result will contain the transform of the given visual.
	''' 
	'''     This method can fail if Camera.Transform is non-invertable
	'''     in which case the camera clip planes will be coincident and
	'''     nothing will render.  In this case success will be false.
	''' </summary>
	''' <param name="visual"></param>
	''' <param name="success"></param>
	''' <returns></returns>
	Public Shared Function TryTransformToCameraSpace(visual As DependencyObject, ByRef viewport As Viewport3DVisual, ByRef success As Boolean) As Matrix3D
		Dim toViewSpace As Matrix3D = GetWorldTransformationMatrix(visual, viewport)
		toViewSpace.Append(MathUtils.TryWorldToCameraTransform(viewport, success))

		If Not success Then
			Return ZeroMatrix
		End If

		Return toViewSpace
	End Function

	''' <summary>
	'''     Transforms the axis-aligned bounding box 'bounds' by
	'''     'transform'
	''' </summary>
	''' <param name="bounds">The AABB to transform</param>
	''' <returns>Transformed AABB</returns>
	Public Shared Function TransformBounds(bounds As Rect3D, transform As Matrix3D) As Rect3D
		Dim x1 As Double = bounds.X
		Dim y1 As Double = bounds.Y
		Dim z1 As Double = bounds.Z
		Dim x2 As Double = bounds.X + bounds.SizeX
		Dim y2 As Double = bounds.Y + bounds.SizeY
		Dim z2 As Double = bounds.Z + bounds.SizeZ

		Dim points As Point3D() = New Point3D() {New Point3D(x1, y1, z1), New Point3D(x1, y1, z2), New Point3D(x1, y2, z1), New Point3D(x1, y2, z2), New Point3D(x2, y1, z1), New Point3D(x2, y1, z2), _
			New Point3D(x2, y2, z1), New Point3D(x2, y2, z2)}

		transform.Transform(points)

		' reuse the 1 and 2 variables to stand for smallest and largest
		Dim p As Point3D = points(0)
		x1 = InlineAssignHelper(x2, p.X)
		y1 = InlineAssignHelper(y2, p.Y)
		z1 = InlineAssignHelper(z2, p.Z)

		For i As Integer = 1 To points.Length - 1
			p = points(i)

			x1 = Math.Min(x1, p.X)
			y1 = Math.Min(y1, p.Y)
			z1 = Math.Min(z1, p.Z)
			x2 = Math.Max(x2, p.X)
			y2 = Math.Max(y2, p.Y)
			z2 = Math.Max(z2, p.Z)
		Next

		Return New Rect3D(x1, y1, z1, x2 - x1, y2 - y1, z2 - z1)
	End Function

	''' <summary>
	'''     Normalizes v if |v| > 0.
	''' 
	'''     This normalization is slightly different from Vector3D.Normalize. Here
	'''     we just divide by the length but Vector3D.Normalize tries to avoid
	'''     overflow when finding the length.
	''' </summary>
	''' <param name="v">The vector to normalize</param>
	''' <returns>'true' if v was normalized</returns>
	Public Shared Function TryNormalize(ByRef v As Vector3D) As Boolean
		Dim length As Double = v.Length

		If length <> 0 Then
			v /= length
			Return True
		End If

		Return False
	End Function

	''' <summary>
	'''     Computes the center of 'box'
	''' </summary>
	''' <param name="box">The Rect3D we want the center of</param>
	''' <returns>The center point</returns>
	Public Shared Function GetCenter(box As Rect3D) As Point3D
		Return New Point3D(box.X + box.SizeX / 2, box.Y + box.SizeY / 2, box.Z + box.SizeZ / 2)
	End Function

	Public Shared ReadOnly ZeroMatrix As New Matrix3D(0, 0, 0, 0, 0, 0, _
		0, 0, 0, 0, 0, 0, _
		0, 0, 0, 0)

	Public Shared ReadOnly XAxis As New Vector3D(1, 0, 0)
	Public Shared ReadOnly YAxis As New Vector3D(0, 1, 0)
	Public Shared ReadOnly ZAxis As New Vector3D(0, 0, 1)
	Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
		target = value
		Return value
	End Function
End Class
