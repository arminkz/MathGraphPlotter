'---------------------------------------------------------------------------
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
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Shapes
Imports System.Windows.Input
Imports System.Windows.Markup
' IAddChild, ContentPropertyAttribute
''' <summary>
''' This class enables a Viewport3D to be enhanced by allowing UIElements to be placed 
''' behind and in front of the Viewport3D.  These can then be used for various enhancements.  
''' For examples see the Trackball, or InteractiveViewport3D.
''' </summary>
<ContentProperty("Content")> _
Public MustInherit Class Viewport3DDecorator
	Inherits FrameworkElement
	Implements IAddChild
	''' <summary>
	''' Creates the Viewport3DDecorator
	''' </summary>
	Public Sub New()
		' create the two lists of children
		_preViewportChildren = New UIElementCollection(Me, Me)
		_postViewportChildren = New UIElementCollection(Me, Me)

		' no content yet
		_content = Nothing
	End Sub

	''' <summary>
	''' The content/child of the Viewport3DDecorator.  A Viewport3DDecorator only has one
	''' child and this child must be either another Viewport3DDecorator or a Viewport3D.
	''' </summary>
	Public Property Content() As UIElement
		Get
			Return _content
		End Get

		Set
			' check to make sure it is a Viewport3D or a Viewport3DDecorator                
			If Not (TypeOf value Is Viewport3D OrElse TypeOf value Is Viewport3DDecorator) Then
				Throw New ArgumentException("Not a valid child type", "value")
			End If

			' check to make sure we're attempting to set something new
			If _content IsNot value Then
				Dim oldContent As UIElement = _content
				Dim newContent As UIElement = value

				' remove the previous child
				RemoveVisualChild(oldContent)
				RemoveLogicalChild(oldContent)

				' set the private variable
				_content = value

				' link in the new child
				AddLogicalChild(newContent)
				AddVisualChild(newContent)

				' let anyone know that derives from us that there was a change
				OnViewport3DDecoratorContentChange(oldContent, newContent)

				' data bind to what is below us so that we have the same width/height
				' as the Viewport3D being enhanced
				' create the bindings now for use later
				BindToContentsWidthHeight(newContent)

				' Invalidate measure to indicate a layout update may be necessary
				InvalidateMeasure()
			End If
		End Set
	End Property

	''' <summary>
	''' Data binds the (Max/Min)Width and (Max/Min)Height properties to the same
	''' ones as the content.  This will make it so we end up being sized to be
	''' exactly the same ActualWidth and ActualHeight as waht is below us.
	''' </summary>
	''' <param name="newContent">What to bind to</param>
	Private Sub BindToContentsWidthHeight(newContent As UIElement)
		' bind to width height
		Dim _widthBinding As New Binding("Width")
		_widthBinding.Mode = BindingMode.OneWay
		Dim _heightBinding As New Binding("Height")
		_heightBinding.Mode = BindingMode.OneWay

		_widthBinding.Source = newContent
		_heightBinding.Source = newContent

		BindingOperations.SetBinding(Me, WidthProperty, _widthBinding)
		BindingOperations.SetBinding(Me, HeightProperty, _heightBinding)


		' bind to max width and max height
		Dim _maxWidthBinding As New Binding("MaxWidth")
		_maxWidthBinding.Mode = BindingMode.OneWay
		Dim _maxHeightBinding As New Binding("MaxHeight")
		_maxHeightBinding.Mode = BindingMode.OneWay

		_maxWidthBinding.Source = newContent
		_maxHeightBinding.Source = newContent

		BindingOperations.SetBinding(Me, MaxWidthProperty, _maxWidthBinding)
		BindingOperations.SetBinding(Me, MaxHeightProperty, _maxHeightBinding)


		' bind to min width and min height
		Dim _minWidthBinding As New Binding("MinWidth")
		_minWidthBinding.Mode = BindingMode.OneWay
		Dim _minHeightBinding As New Binding("MinHeight")
		_minHeightBinding.Mode = BindingMode.OneWay

		_minWidthBinding.Source = newContent
		_minHeightBinding.Source = newContent

		BindingOperations.SetBinding(Me, MinWidthProperty, _minWidthBinding)
		BindingOperations.SetBinding(Me, MinHeightProperty, _minHeightBinding)
	End Sub

	''' <summary>
	''' Extenders of Viewport3DDecorator can override this function to be notified
	''' when the Content property changes
	''' </summary>
	''' <param name="oldContent">The old value of the Content property</param>
	''' <param name="newContent">The new value of the Content property</param>
	Protected Overridable Sub OnViewport3DDecoratorContentChange(oldContent As UIElement, newContent As UIElement)
	End Sub

	''' <summary>
	''' Property to get the Viewport3D that is being enhanced.
	''' </summary>
	Public ReadOnly Property Viewport3D() As Viewport3D
		Get
			Dim viewport3D__1 As Viewport3D = Nothing
			Dim currEnhancer As Viewport3DDecorator = Me

			' we follow the enhancers down until we get the
			' Viewport3D they are enhancing
			While True
				Dim currContent As UIElement = currEnhancer.Content

				If currContent Is Nothing Then
					Exit While
				ElseIf TypeOf currContent Is Viewport3D Then
					viewport3D__1 = DirectCast(currContent, Viewport3D)
					Exit While
				Else
					currEnhancer = DirectCast(currContent, Viewport3DDecorator)
				End If
			End While

			Return viewport3D__1
		End Get
	End Property

	''' <summary>
	''' The UIElements that occur before the Viewport3D
	''' </summary>
	Protected ReadOnly Property PreViewportChildren() As UIElementCollection
		Get
			Return _preViewportChildren
		End Get
	End Property

	''' <summary>
	''' The UIElements that occur after the Viewport3D
	''' </summary>
	Protected ReadOnly Property PostViewportChildren() As UIElementCollection
		Get
			Return _postViewportChildren
		End Get
	End Property

	''' <summary>
	''' Returns the number of Visual children this element has.
	''' </summary>
	Protected Overrides ReadOnly Property VisualChildrenCount() As Integer
		Get
			Dim contentCount As Integer = (If(Content Is Nothing, 0, 1))

			Return PreViewportChildren.Count + PostViewportChildren.Count + contentCount
		End Get
	End Property

	''' <summary>
	''' Returns the child at the specified index.
	''' </summary>
	Protected Overrides Function GetVisualChild(index As Integer) As Visual
		Dim orginalIndex As Integer = index

		' see if index is in the pre viewport children
		If index < PreViewportChildren.Count Then
			Return PreViewportChildren(index)
		End If
		index -= PreViewportChildren.Count

		' see if it's the content
		If Content IsNot Nothing AndAlso index = 0 Then
			Return Content
		End If
		index -= (If(Content Is Nothing, 0, 1))

		' see if it's the post viewport children
		If index < PostViewportChildren.Count Then
			Return PostViewportChildren(index)
		End If

		' if we didn't return then the index is out of range - throw an error
		Throw New ArgumentOutOfRangeException("index", orginalIndex, "Out of range visual requested")
	End Function

	''' <summary> 
	''' Returns an enumertor to this element's logical children
	''' </summary>
	Protected Overrides ReadOnly Property LogicalChildren() As IEnumerator
		Get
			Dim logicalChildren__1 As Visual() = New Visual(VisualChildrenCount - 1) {}

			For i As Integer = 0 To VisualChildrenCount - 1
				logicalChildren__1(i) = GetVisualChild(i)
			Next

			' return an enumerator to the ArrayList
			Return logicalChildren__1.GetEnumerator()
		End Get
	End Property

	''' <summary>
	''' Updates the DesiredSize of the Viewport3DDecorator
	''' </summary>
	''' <param name="constraint">The "upper limit" that the return value should not exceed</param>
	''' <returns>The desired size of the Viewport3DDecorator</returns>
	Protected Overrides Function MeasureOverride(constraint As Size) As Size
		Dim finalSize As New Size()

		MeasurePreViewportChildren(constraint)

		' measure our Viewport3D(Enhancer)
		If Content IsNot Nothing Then
			Content.Measure(constraint)
			finalSize = Content.DesiredSize
		End If

		MeasurePostViewportChildren(constraint)

		Return finalSize
	End Function

	''' <summary>
	''' Measures the size of all the PreViewportChildren.  If special measuring behavior is needed, this
	''' method should be overridden.
	''' </summary>
	''' <param name="constraint">The "upper limit" on the size of an element</param>
	Protected Overridable Sub MeasurePreViewportChildren(constraint As Size)
		' measure the pre viewport children
		MeasureUIElementCollection(PreViewportChildren, constraint)
	End Sub

	''' <summary>
	''' Measures the size of all the PostViewportChildren.  If special measuring behavior is needed, this
	''' method should be overridden.
	''' </summary>
	''' <param name="constraint">The "upper limit" on the size of an element</param>
	Protected Overridable Sub MeasurePostViewportChildren(constraint As Size)
		' measure the post viewport children
		MeasureUIElementCollection(PostViewportChildren, constraint)
	End Sub

	''' <summary>
	''' Measures all of the UIElements in a UIElementCollection
	''' </summary>
	''' <param name="collection">The collection to measure</param>
	''' <param name="constraint">The "upper limit" on the size of an element</param>
	Private Sub MeasureUIElementCollection(collection As UIElementCollection, constraint As Size)
		' measure the pre viewport visual visuals
		For Each uiElem As UIElement In collection
			uiElem.Measure(constraint)
		Next
	End Sub

	''' <summary>
	''' Arranges the Pre and Post Viewport children, and arranges itself
	''' </summary>
	''' <param name="arrangeSize">The final size to use to arrange itself and its children</param>
	Protected Overrides Function ArrangeOverride(arrangeSize As Size) As Size
		ArrangePreViewportChildren(arrangeSize)

		' arrange our Viewport3D(Enhancer)
		If Content IsNot Nothing Then
			Content.Arrange(New Rect(arrangeSize))
		End If

		ArrangePostViewportChildren(arrangeSize)

		Return arrangeSize
	End Function

	''' <summary>
	''' Arranges all the PreViewportChildren.  If special measuring behavior is needed, this
	''' method should be overridden.
	''' </summary>
	''' <param name="arrangeSize">The final size to use to arrange each child</param>
	Protected Overridable Sub ArrangePreViewportChildren(arrangeSize As Size)
		ArrangeUIElementCollection(PreViewportChildren, arrangeSize)
	End Sub

	''' <summary>
	''' Arranges all the PostViewportChildren.  If special measuring behavior is needed, this
	''' method should be overridden.
	''' </summary>
	''' <param name="arrangeSize">The final size to use to arrange each child</param>
	Protected Overridable Sub ArrangePostViewportChildren(arrangeSize As Size)
		ArrangeUIElementCollection(PostViewportChildren, arrangeSize)
	End Sub

	''' <summary>
	''' Arranges all the UIElements in the passed in UIElementCollection
	''' </summary>
	''' <param name="collection">The collection that should be arranged</param>
	''' <param name="constraint">The final size that element should use to arrange itself and its children</param>
	Private Sub ArrangeUIElementCollection(collection As UIElementCollection, constraint As Size)
		' measure the pre viewport visual visuals
		For Each uiElem As UIElement In collection
			uiElem.Arrange(New Rect(constraint))
		Next
	End Sub

	'------------------------------------------------------
	'
	'  IAddChild implementation
	'
	'------------------------------------------------------

	Private Sub IAddChild_AddChild(value As [Object]) Implements IAddChild.AddChild
		' check against null
		If value Is Nothing Then
			Throw New ArgumentNullException("value")
		End If

		' we only can have one child
		If Me.Content IsNot Nothing Then
			Throw New ArgumentException("Viewport3DDecorator can only have one child")
		End If

		' now we can actually set the content
		Content = DirectCast(value, UIElement)
	End Sub

	Private Sub IAddChild_AddText(text As String) Implements IAddChild.AddText
		' The only text we accept is whitespace, which we ignore.
		For i As Integer = 0 To text.Length - 1
			If Not [Char].IsWhiteSpace(text(i)) Then
				Throw New ArgumentException("Non whitespace in add text", text)
			End If
		Next
	End Sub

	'---------------------------------------------------------
	' 
	'  Private data
	'
	'---------------------------------------------------------        
	Private _preViewportChildren As UIElementCollection
	Private _postViewportChildren As UIElementCollection
	Private _content As UIElement
End Class

