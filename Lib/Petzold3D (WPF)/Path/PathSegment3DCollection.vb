'--------------------------------------------------------
' PathSegment3DCollection.cs (c) 2007 by Charles Petzold
'--------------------------------------------------------
Imports System.Windows

''' <summary>
'''     Represents a collection of PathSegment3D objects that can be 
'''     individually accessed by index. 
''' </summary>
Public Class PathSegment3DCollection
	Inherits FreezableCollection(Of PathSegment3D)
	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Public Overrides Function ToString() As String
		Dim str As String = ""

		For Each seg As PathSegment3D In Me
			str += seg.ToString()
		Next

		Return str
	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New PathSegment3DCollection()
	End Function
End Class
