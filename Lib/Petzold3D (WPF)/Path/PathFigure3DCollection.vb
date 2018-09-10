'-------------------------------------------------------
' PathFigure3DCollection.cs (c) 2007 by Charles Petzold
'-------------------------------------------------------
Imports System.Windows

'--------------------------------------------------------------------------------------
' TODO: [System.ComponentModel.TypeConverter(typeof(PathFigure3DCollectionConverter))] 
'--------------------------------------------------------------------------------------
Public Class PathFigure3DCollection
	Inherits FreezableCollection(Of PathFigure3D)
	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Public Overrides Function ToString() As String
		Dim str As String = ""

		For Each fig As PathFigure3D In Me
			str += fig.ToString()
		Next

		Return str
	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New PathFigure3DCollection()
	End Function
End Class
