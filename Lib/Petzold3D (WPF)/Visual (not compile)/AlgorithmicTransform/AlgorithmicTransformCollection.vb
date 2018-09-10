'---------------------------------------------------------------
' AlgorithmicTransformCollection.cs (c) 2007 by Charles Petzold
'---------------------------------------------------------------
Imports System.Windows

''' <summary>
''' 
''' </summary>
Public Class AlgorithmicTransformCollection
	Inherits FreezableCollection(Of AlgorithmicTransform)
	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New AlgorithmicTransformCollection()
	End Function
End Class
