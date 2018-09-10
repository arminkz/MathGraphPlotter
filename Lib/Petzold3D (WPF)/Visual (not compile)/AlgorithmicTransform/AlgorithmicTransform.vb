'-----------------------------------------------------
' AlgorithmicTransform.cs (c) 2007 by Charles Petzold
'-----------------------------------------------------
Imports System.Windows
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Media3D

''' <summary>
''' 
''' </summary>
Public MustInherit Class AlgorithmicTransform
	Inherits Animatable
	''' <summary>
	''' 
	''' </summary>
	''' <param name="points"></param>
	Public MustOverride Sub Transform(points As Point3DCollection)
End Class
