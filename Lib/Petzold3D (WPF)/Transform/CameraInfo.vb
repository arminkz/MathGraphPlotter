'-------------------------------------------
' CameraInfo.cs (c) 2007 by Charles Petzold
'-------------------------------------------
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D

Public Class CameraInfo
	Inherits Animatable
	Public Shared ReadOnly ZeroMatrix As New Matrix3D(0, 0, 0, 0, 0, 0, _
		0, 0, 0, 0, 0, 0, _
		0, 0, 0, 0)

	Public Sub New()
	End Sub

	''' <summary>
	'''     Identifies the Name dependency property.
	''' </summary>

	Public Shared ReadOnly NameProperty As DependencyProperty = DependencyProperty.Register("Name", GetType(String), GetType(CameraInfo))

	''' <summary>
	'''     Gets or sets the identifying name of the object. ETC.
	'''     This is a dependency property.
	''' </summary>

	Public Property Name() As String
		Get
			Return DirectCast(GetValue(NameProperty), String)
		End Get
		Set
			SetValue(NameProperty, value)
		End Set
	End Property

	Public Shared ReadOnly CameraProperty As DependencyProperty = DependencyProperty.Register("Camera", GetType(Camera), GetType(CameraInfo), New PropertyMetadata(Nothing, AddressOf CameraPropertyChanged))

	Public Property Camera() As Camera
		Get
			Return DirectCast(GetValue(CameraProperty), Camera)
		End Get
		Set
			SetValue(CameraProperty, value)
		End Set
	End Property

	Public Shared ReadOnly ViewportWidthProperty As DependencyProperty = DependencyProperty.Register("ViewportWidth", GetType(Double), GetType(CameraInfo), New PropertyMetadata(1.0, AddressOf ViewportPropertyChanged))

	Public Property ViewportWidth() As Double
		Get
			Return CDbl(GetValue(ViewportWidthProperty))
		End Get
		Set
			SetValue(ViewportWidthProperty, value)
		End Set
	End Property

	Public Shared ReadOnly ViewportHeightProperty As DependencyProperty = DependencyProperty.Register("ViewportHeight", GetType(Double), GetType(CameraInfo), New PropertyMetadata(1.0, AddressOf ViewportPropertyChanged))

	Public Property ViewportHeight() As Double
		Get
			Return CDbl(GetValue(ViewportHeightProperty))
		End Get
		Set
			SetValue(ViewportHeightProperty, value)
		End Set
	End Property

	Shared ReadOnly ViewMatrixKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly("ViewMatrix", GetType(Matrix3D), GetType(CameraInfo), New PropertyMetadata(New Matrix3D()))

	Public Shared ReadOnly ViewMatrixProperty As DependencyProperty = ViewMatrixKey.DependencyProperty

	Public Property ViewMatrix() As Matrix3D
		Get
			Return CType(GetValue(ViewMatrixProperty), Matrix3D)
		End Get
		Private Set
			SetValue(ViewMatrixKey, value)
		End Set
	End Property

	Shared ReadOnly ProjectionMatrixKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly("ProjectionMatrix", GetType(Matrix3D), GetType(CameraInfo), New PropertyMetadata(New Matrix3D()))

	Public Shared ReadOnly ProjectionMatrixProperty As DependencyProperty = ProjectionMatrixKey.DependencyProperty

	Public Property ProjectionMatrix() As Matrix3D
		Get
			Return CType(GetValue(ProjectionMatrixProperty), Matrix3D)
		End Get
		Private Set
			SetValue(ProjectionMatrixKey, value)
		End Set
	End Property

	Public Shared ReadOnly TotalTransformProperty As DependencyProperty = DependencyProperty.Register("TotalTransform", GetType(Matrix3D), GetType(CameraInfo))

	Public Property TotalTransform() As Matrix3D
		Get
			Return CType(GetValue(TotalTransformProperty), Matrix3D)
		End Get
		Protected Set
			SetValue(TotalTransformProperty, value)
		End Set
	End Property

	Public Shared ReadOnly InverseTransformProperty As DependencyProperty = DependencyProperty.Register("InverseTransform", GetType(Matrix3D), GetType(CameraInfo))

	Public Property InverseTransform() As Matrix3D
		Get
			Return CType(GetValue(InverseTransformProperty), Matrix3D)
		End Get
		Protected Set
			SetValue(InverseTransformProperty, value)
		End Set
	End Property

	Private Shared Sub CameraPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim caminfo As CameraInfo = TryCast(obj, CameraInfo)
		caminfo.ViewMatrix = GetViewMatrix(caminfo.Camera)
		ViewportPropertyChanged(obj, args)
	End Sub

	Private Shared Sub ViewportPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim caminfo As CameraInfo = TryCast(obj, CameraInfo)
		caminfo.ViewMatrix = GetViewMatrix(caminfo.Camera)

		caminfo.ProjectionMatrix = GetProjectionMatrix(caminfo.Camera, caminfo.ViewportWidth / caminfo.ViewportHeight)

		' Can these two be made more efficient -- not getting view and projection again ?????

		caminfo.TotalTransform = GetTotalTransform(caminfo.Camera, caminfo.ViewportWidth / caminfo.ViewportHeight)

		caminfo.InverseTransform = GetInverseTransform(caminfo.Camera, caminfo.ViewportWidth / caminfo.ViewportHeight)
	End Sub

	''' <summary>
	'''     Obtains the view transform matrix for a camera.
	''' </summary>
	''' <param name="camera">
	'''     Camera to obtain the 
	''' </param>
	''' <returns>
	'''     A Matrix3D objecvt with the camera view transform matrix,
	'''     or a Matrix3D with all zeros if the "camera" is null.
	''' </returns>
	''' <exception cref="TK">
	'''     if the 'camera' is neither of type MatrixCamera nor
	'''     ProjectionCamera.
	''' </exception>

	Public Shared Function GetViewMatrix(camera As Camera) As Matrix3D
		Dim matx As Matrix3D = Matrix3D.Identity

		If camera Is Nothing Then
			matx = ZeroMatrix

		ElseIf TypeOf camera Is MatrixCamera Then
			matx = TryCast(camera, MatrixCamera).ViewMatrix
		ElseIf TypeOf camera Is ProjectionCamera Then
			Dim projcam As ProjectionCamera = TryCast(camera, ProjectionCamera)

			Dim zAxis As Vector3D = -projcam.LookDirection
			zAxis.Normalize()

			Dim xAxis As Vector3D = Vector3D.CrossProduct(projcam.UpDirection, zAxis)
			xAxis.Normalize()

			Dim yAxis As Vector3D = Vector3D.CrossProduct(zAxis, xAxis)
			Dim pos As Vector3D = CType(projcam.Position, Vector3D)


			matx = New Matrix3D(xAxis.X, yAxis.X, zAxis.X, 0, xAxis.Y, yAxis.Y, _
				zAxis.Y, 0, xAxis.Z, yAxis.Z, zAxis.Z, 0, _
				-Vector3D.DotProduct(xAxis, pos), -Vector3D.DotProduct(yAxis, pos), -Vector3D.DotProduct(zAxis, pos), 1)

		ElseIf camera IsNot Nothing Then
			Throw New ApplicationException("ViewMatrix")
		End If
		Return matx
	End Function

	Public Shared Function GetProjectionMatrix(cam As Camera, aspectRatio As Double) As Matrix3D
		Dim matx As Matrix3D = Matrix3D.Identity

		If cam Is Nothing Then
			matx = ZeroMatrix

		ElseIf TypeOf cam Is MatrixCamera Then
			matx = TryCast(cam, MatrixCamera).ProjectionMatrix

		ElseIf TypeOf cam Is OrthographicCamera Then
			Dim orthocam As OrthographicCamera = TryCast(cam, OrthographicCamera)

			Dim xScale As Double = 2 / orthocam.Width
			Dim yScale As Double = xScale * aspectRatio
			Dim zNear As Double = orthocam.NearPlaneDistance
			Dim zFar As Double = orthocam.FarPlaneDistance

			' Hey, check this out!
			If [Double].IsPositiveInfinity(zFar) Then
				zFar = 10000000000.0
			End If


			matx = New Matrix3D(xScale, 0, 0, 0, 0, yScale, _
				0, 0, 0, 0, 1 / (zNear - zFar), 0, _
				0, 0, zNear / (zNear - zFar), 1)

		ElseIf TypeOf cam Is PerspectiveCamera Then
			Dim perscam As PerspectiveCamera = TryCast(cam, PerspectiveCamera)

			' The angle-to-radian formula is a little off because only
			'  half the angle enters the calculation.
			Dim xScale As Double = 1 / Math.Tan(Math.PI * perscam.FieldOfView / 360)
			Dim yScale As Double = xScale * aspectRatio
			Dim zNear As Double = perscam.NearPlaneDistance
			Dim zFar As Double = perscam.FarPlaneDistance
			Dim zScale As Double = (If(zFar = Double.PositiveInfinity, -1, (zFar / (zNear - zFar))))
			Dim zOffset As Double = zNear * zScale

			matx = New Matrix3D(xScale, 0, 0, 0, 0, yScale, _
				0, 0, 0, 0, zScale, -1, _
				0, 0, zOffset, 0)

		ElseIf cam IsNot Nothing Then
			Throw New ApplicationException("ProjectionMatrix")
		End If

		Return matx
	End Function


	Public Shared Function GetTotalTransform(cam As Camera, aspectRatio As Double) As Matrix3D
		Dim matx As Matrix3D = Matrix3D.Identity

		If cam Is Nothing Then
			matx = ZeroMatrix
		Else

			If cam.Transform IsNot Nothing Then
				Dim matxCameraTransform As Matrix3D = cam.Transform.Value

				If Not matxCameraTransform.HasInverse Then
					matx = ZeroMatrix
				Else
					matxCameraTransform.Invert()
					matx.Append(matxCameraTransform)
				End If
			End If

			matx.Append(CameraInfo.GetViewMatrix(cam))
			matx.Append(CameraInfo.GetProjectionMatrix(cam, aspectRatio))
		End If
		Return matx
	End Function


	Public Shared Function GetInverseTransform(cam As Camera, aspectRatio As Double) As Matrix3D
		Dim matx As Matrix3D = GetTotalTransform(cam, aspectRatio)

		If matx = ZeroMatrix Then
			

		ElseIf Not matx.HasInverse Then
			matx = ZeroMatrix
		Else
			matx.Invert()
		End If
		Return matx
	End Function


	Public Overrides Function ToString() As String
		Return [String].Format("View Matrix: {0}" & vbLf & "Projection Matrix: {1}" & vbLf & "Total Transform: {2}", ViewMatrix, ProjectionMatrix, TotalTransform)
	End Function

	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New CameraInfo()
	End Function
End Class
