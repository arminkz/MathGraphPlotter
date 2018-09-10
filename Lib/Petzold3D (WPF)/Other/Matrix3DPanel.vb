'----------------------------------------------
' Matrix3DPanel.cs (c) 2007 by Charles Petzold
'----------------------------------------------
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

''' <summary>
''' 
''' </summary>
Public Class Matrix3DPanel
	Inherits FrameworkElement
	Private unigrid As UniformGrid

	''' <summary>
	''' 
	''' </summary>
	Public Sub New()
		Dim strIds As String() = {"M11", "M12", "M13", "M14", "M21", "M22", _
			"M23", "M24", "M31", "M32", "M33", "M34", _
			"OffsetX", "OffsetY", "OffsetZ", "M44"}

		NameScope.SetNameScope(Me, New NameScope())

		' Create UniformGrid and add it as a child.
		unigrid = New UniformGrid()
		unigrid.Rows = 4
		unigrid.Columns = 4
		AddVisualChild(unigrid)
		AddLogicalChild(unigrid)

		For i As Integer = 0 To strIds.Length - 1
			' StackPanel for each cell.
			Dim stack1 As New StackPanel()
			stack1.Orientation = Orientation.Vertical
			stack1.Margin = New Thickness(12)
			unigrid.Children.Add(stack1)

			' StackPanel for TextBlock elements.
			Dim stack2 As New StackPanel()
			stack2.Orientation = Orientation.Horizontal
			stack2.HorizontalAlignment = HorizontalAlignment.Center
			stack1.Children.Add(stack2)

			' ScrollBar for each cell.
			Dim scroll As New ScrollBar()
			scroll.Orientation = Orientation.Horizontal
			scroll.Value = If((i Mod 5 = 0), 1, 0)
			scroll.SmallChange = 0.01
			scroll.LargeChange = 0.1
			scroll.Focusable = True
			stack1.Children.Add(scroll)

			RegisterName(strIds(i), scroll)

			' Set bindings for scrollbars.
			Dim binding As New Binding("Minimum")
			binding.Source = Me
			binding.Mode = BindingMode.OneWay
			scroll.SetBinding(ScrollBar.MinimumProperty, binding)

			binding = New Binding("Maximum")
			binding.Source = Me
			binding.Mode = BindingMode.OneWay
			scroll.SetBinding(ScrollBar.MaximumProperty, binding)

			' TextBlock elements to show values.
			Dim txtblk As New TextBlock()
			txtblk.Text = strIds(i) & " = "
			stack2.Children.Add(txtblk)

			binding = New Binding("Value")
			binding.Source = scroll
			binding.Mode = BindingMode.OneWay

			txtblk = New TextBlock()
			txtblk.SetBinding(TextBlock.TextProperty, binding)
			stack2.Children.Add(txtblk)
		Next

		[AddHandler](ScrollBar.ValueChangedEvent, New RoutedEventHandler(AddressOf ScrollBarOnValueChanged))
	End Sub

	' Dependency property key for Matrix.
	Shared ReadOnly MatrixKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly("Matrix", GetType(Matrix3D), GetType(Matrix3DPanel), New PropertyMetadata(New Matrix3D()))

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly MatrixProperty As DependencyProperty = MatrixKey.DependencyProperty

	''' <summary>
	''' 
	''' </summary>
	Public Property Matrix() As Matrix3D
		Get
			Return CType(GetValue(MatrixProperty), Matrix3D)
		End Get
		Private Set
			SetValue(MatrixKey, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly MinimumProperty As DependencyProperty = ScrollBar.MinimumProperty.AddOwner(GetType(Matrix3DPanel), New PropertyMetadata(-3.0))

	''' <summary>
	''' 
	''' </summary>
	Public Property Minimum() As Double
		Get
			Return CDbl(GetValue(MinimumProperty))
		End Get
		Set
			SetValue(MinimumProperty, value)
		End Set
	End Property

	''' <summary>
	''' 
	''' </summary>
	Public Shared ReadOnly MaximumProperty As DependencyProperty = ScrollBar.MaximumProperty.AddOwner(GetType(Matrix3DPanel), New PropertyMetadata(3.0))

	''' <summary>
	''' 
	''' </summary>
	Public Property Maximum() As Double
		Get
			Return CDbl(GetValue(MaximumProperty))
		End Get
		Set
			SetValue(MaximumProperty, value)
		End Set
	End Property

	' Set Matrix from ScrollBar values.
	Private Sub ScrollBarOnValueChanged(sender As Object, args As RoutedEventArgs)
		Dim matx As New Matrix3D()

		matx.M11 = TryCast(FindName("M11"), ScrollBar).Value
		matx.M12 = TryCast(FindName("M12"), ScrollBar).Value
		matx.M13 = TryCast(FindName("M13"), ScrollBar).Value
		matx.M14 = TryCast(FindName("M14"), ScrollBar).Value

		matx.M21 = TryCast(FindName("M21"), ScrollBar).Value
		matx.M22 = TryCast(FindName("M22"), ScrollBar).Value
		matx.M23 = TryCast(FindName("M23"), ScrollBar).Value
		matx.M24 = TryCast(FindName("M24"), ScrollBar).Value

		matx.M31 = TryCast(FindName("M31"), ScrollBar).Value
		matx.M32 = TryCast(FindName("M32"), ScrollBar).Value
		matx.M33 = TryCast(FindName("M33"), ScrollBar).Value
		matx.M34 = TryCast(FindName("M34"), ScrollBar).Value

		matx.OffsetX = TryCast(FindName("OffsetX"), ScrollBar).Value
		matx.OffsetY = TryCast(FindName("OffsetY"), ScrollBar).Value
		matx.OffsetZ = TryCast(FindName("OffsetZ"), ScrollBar).Value
		matx.M44 = TryCast(FindName("M44"), ScrollBar).Value

		Matrix = matx
	End Sub

	''' <summary>
	''' 
	''' </summary>
	Protected Overrides ReadOnly Property VisualChildrenCount() As Integer
		Get
			Return 1
		End Get
	End Property

	''' <summary>
	''' 
	''' </summary>
	''' <param name="index"></param>
	''' <returns></returns>
	Protected Overrides Function GetVisualChild(index As Integer) As Visual
		If index > 0 Then
			Throw New ArgumentException("index")
		End If

		Return unigrid
	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <param name="availableSize"></param>
	''' <returns></returns>
	Protected Overrides Function MeasureOverride(availableSize As Size) As Size
		unigrid.Measure(availableSize)
		Return unigrid.DesiredSize
	End Function


	''' <summary>
	''' 
	''' </summary>
	''' <param name="finalSize"></param>
	''' <returns></returns>
	Protected Overrides Function ArrangeOverride(finalSize As Size) As Size
		unigrid.Arrange(New Rect(New Point(0, 0), finalSize))
		Return finalSize
	End Function
End Class
