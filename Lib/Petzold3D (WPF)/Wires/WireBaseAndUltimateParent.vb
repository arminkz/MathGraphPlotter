'----------------------------------------------------------
' WireBaseAndUltimateParent.cs (c) 2007 by Charles Petzold
'----------------------------------------------------------
Imports System.Windows

Class WireBaseAndUltimateParent
	Public wirebase As WireBase
	Public window As Window

	Public Sub New(wirebase As WireBase)
		Me.wirebase = wirebase
	End Sub
End Class
