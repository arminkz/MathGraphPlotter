'------------------------------------------
' ArrowEnds.cs (c) 2007 by Charles Petzold
'------------------------------------------

Namespace Petzold.Media2D
	''' <summary>
	'''     Indicates which end of the line has an arrow.
	''' </summary>
	<Flags> _
	Public Enum ArrowEnds
		None = 0
		Start = 1
		Begin = 1
		[End] = 2
		Both = 3
	End Enum
End Namespace
