'''---------------------------------------------------------------------------
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

Imports System.Collections
Imports System.Collections.Generic
Imports System.Windows.Media.Media3D

''' <summary>
'''     Matrix3DStack is a stack of Matrix3Ds.
''' </summary>
Public Class Matrix3DStack
	Implements IEnumerable(Of Matrix3D)
	Implements ICollection
	Public Function Peek() As Matrix3D
		Return _storage(_storage.Count - 1)
	End Function

	Public Sub Push(item As Matrix3D)
		_storage.Add(item)
	End Sub

	Public Sub Append(item As Matrix3D)
		If Count > 0 Then
			Dim top As Matrix3D = Peek()
			top.Append(item)
			Push(top)
		Else
			Push(item)
		End If
	End Sub

	Public Sub Prepend(item As Matrix3D)
		If Count > 0 Then
			Dim top As Matrix3D = Peek()
			top.Prepend(item)
			Push(top)
		Else
			Push(item)
		End If
	End Sub

	Public Function Pop() As Matrix3D
		Dim result As Matrix3D = Peek()
		_storage.RemoveAt(_storage.Count - 1)

		Return result
	End Function

	Public ReadOnly Property Count() As Integer Implements ICollection.Count
		Get
			Return _storage.Count
		End Get
	End Property

	Private Sub Clear()
		_storage.Clear()
	End Sub

	Private Function Contains(item As Matrix3D) As Boolean
		Return _storage.Contains(item)
	End Function

	Private ReadOnly _storage As New List(Of Matrix3D)()

	#Region "ICollection Members"

	Private Sub ICollection_CopyTo(array As Array, index As Integer) Implements ICollection.CopyTo
		DirectCast(_storage, ICollection).CopyTo(array, index)
	End Sub

	Private ReadOnly Property ICollection_IsSynchronized() As Boolean Implements ICollection.IsSynchronized
		Get
			Return DirectCast(_storage, ICollection).IsSynchronized
		End Get
	End Property

	Private ReadOnly Property ICollection_SyncRoot() As Object Implements ICollection.SyncRoot
		Get
			Return DirectCast(_storage, ICollection).SyncRoot
		End Get
	End Property

	#End Region

	#Region "IEnumerable Members"

	Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
		Return DirectCast(Me, IEnumerable(Of Matrix3D)).GetEnumerator()
	End Function

	#End Region

	#Region "IEnumerable<Matrix3D> Members"

    Private Function GetEnumerator() As IEnumerator(Of Matrix3D) Implements IEnumerable(Of Matrix3D).GetEnumerator
        Return _storage
    End Function

	#End Region
End Class
