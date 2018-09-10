'----------------------------------------------------
' TeapotTriangleRange.cs (c) 2007 by Charles Petzold
'----------------------------------------------------
Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Animation

<TypeConverter(GetType(TeapotTriangleRangeConverter))> _
Public Class TeapotTriangleRange
	Inherits Animatable
	''' <summary>
	'''     Identifies the Begin dependency property.
	''' </summary>
	Public Shared ReadOnly BeginProperty As DependencyProperty = DependencyProperty.Register("Begin", GetType(Integer), GetType(TeapotTriangleRange), New PropertyMetadata(0))

	Public Property Begin() As Integer
		Get
			Return CInt(GetValue(BeginProperty))
		End Get
		Set
			SetValue(BeginProperty, value)
		End Set
	End Property

	''' <summary>
	'''     Identifies the End dependency property.
	''' </summary>
	Public Shared ReadOnly EndProperty As DependencyProperty = DependencyProperty.Register("End", GetType(Integer), GetType(TeapotTriangleRange), New PropertyMetadata(2255))

	Public Property [End]() As Integer
		Get
			Return CInt(GetValue(EndProperty))
		End Get
		Set
			SetValue(EndProperty, value)
		End Set
	End Property

	Public Sub New()
	End Sub

	Public Sub New(begin__1 As Integer, end__2 As Integer)
		Begin = begin__1
		[End] = end__2
	End Sub

	Public Shared ReadOnly Property All() As TeapotTriangleRange
		Get
			Return New TeapotTriangleRange()
		End Get
	End Property

	Public Shared ReadOnly Property Pot() As TeapotTriangleRange
		Get
			Return New TeapotTriangleRange(0, 1703)
		End Get
	End Property

	Public Shared ReadOnly Property Body() As TeapotTriangleRange
		Get
			Return New TeapotTriangleRange(0, 1127)
		End Get
	End Property

	Public Shared ReadOnly Property Handle() As TeapotTriangleRange
		Get
			Return New TeapotTriangleRange(1128, 1415)
		End Get
	End Property

	Public Shared ReadOnly Property Spout() As TeapotTriangleRange
		Get
			Return New TeapotTriangleRange(1416, 1703)
		End Get
	End Property

	Public Shared ReadOnly Property Lid() As TeapotTriangleRange
		Get
			Return New TeapotTriangleRange(1704, 2255)
		End Get
	End Property

	Public Shared Function Parse(str As String) As TeapotTriangleRange
		Dim strTokens As String() = str.Split(" "C, ","C)
		Dim num As Integer = 0
		Dim values As Integer() = New Integer(1) {}

		For Each strToken As String In strTokens
			If strToken.Length > 0 Then
				If num = 2 Then
					Throw New FormatException("Too many tokens in string.")
				End If

				values(System.Math.Max(System.Threading.Interlocked.Increment(num),num - 1)) = Int32.Parse(strToken)
			End If
		Next

		If num <> 2 Then
			Throw New FormatException("Not enough tokens in string.")
		End If

		Return New TeapotTriangleRange(values(0), values(1))
	End Function

	Public Overrides Function ToString() As String
		Return [String].Format("{0},{1}", Begin, [End])
	End Function

	''' <summary>
	'''     Creates a new instance of the TeapotTriangleRange class.
	''' </summary>
	''' <returns>
	'''     A new instance of TeapotTriangleRange.
	''' </returns>
	''' <remarks>
	'''     Overriding this method is required when deriving 
	'''     from the Freezable class.
	''' </remarks>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New SphereMesh()
	End Function

End Class
