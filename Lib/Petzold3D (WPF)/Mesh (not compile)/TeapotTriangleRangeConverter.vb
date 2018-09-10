'-------------------------------------------------------------
' TeapotTriangleRangeConverter.cs (c) 2007 by Charles Petzold
'-------------------------------------------------------------
Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection

''' <summary>
'''     Converts instances of other types to and from a 
'''     TeapotTriangleRange. 
''' </summary>
Public Class TeapotTriangleRangeConverter
	Inherits TypeConverter
	''' <summary>
	'''     Indicates whether an object can be converted from a given 
	'''     type to an instance of a TeapotTriangleRange. 
	''' </summary>
	''' <param name="context">
	'''     Describes the context information of a type.
	''' </param>
	''' <param name="sourceType">
	'''     The source Type that is being queried for conversion support.
	''' </param>
	''' <returns>
	'''     true if object of the specified type can be converted to a 
	'''     TeapotTriangleRange; otherwise, false.  
	''' </returns>
	Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean
		If sourceType Is GetType(String) Then
			Return True
		End If

		Return MyBase.CanConvertFrom(context, sourceType)
	End Function

	''' <summary>
	'''     Determines whether instances of TeapotTriangleRange can be 
	'''     converted to the specified type. 
	''' </summary>
	''' <param name="context">
	'''     Describes the context information of a type.
	''' </param>
	''' <param name="destinationType">
	'''     The desired type this TeapotTriangleRange is being evaluated 
	'''     to be converted to.
	''' </param>
	''' <returns>
	'''     true if instances of TeapotTriangleRange can be converted to 
	'''     destinationType; otherwise, false.
	''' </returns>
	Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
		If destinationType Is GetType(String) Then
			Return True
		End If

		Return MyBase.CanConvertTo(context, destinationType)
	End Function

	''' <summary>
	'''     Converts the specified object to a TeapotTriangleRange. 
	''' </summary>
	''' <param name="context">
	'''     Describes the context information of a type.
	''' </param>
	''' <param name="culture">
	'''     Describes the CultureInfo of the type being converted. 
	''' </param>
	''' <param name="value">
	'''     The object being converted.
	''' </param>
	''' <returns>
	'''     The TeapotTriangleRange created from converting value. 
	''' </returns>
	Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As CultureInfo, value As Object) As Object
		If value.[GetType]() IsNot GetType(String) Then
			Return MyBase.ConvertFrom(context, culture, value)
		End If

		Dim str As String = TryCast(value, String).Trim()
		Dim props As PropertyInfo() = GetType(TeapotTriangleRange).GetProperties(BindingFlags.[Public] Or BindingFlags.[Static])

		For Each prop As PropertyInfo In props
			If prop.PropertyType Is GetType(TeapotTriangleRange) AndAlso 0 = [String].Compare(str, prop.Name, True) Then
				Return prop.GetValue(Nothing, Nothing)
			End If
		Next

		Return TeapotTriangleRange.Parse(TryCast(value, String))
	End Function

	''' <summary>
	'''     Converts the specified TeapotTriangleRange to the specified type.
	''' </summary>
	''' <param name="context">
	'''     Describes the context information of a type.
	''' </param>
	''' <param name="culture">
	'''     Describes the CultureInfo of the type being converted.
	''' </param>
	''' <param name="value">
	'''     The TeapotTriangleRange to convert.
	''' </param>
	''' <param name="destinationType">
	'''     The type to convert the TeapotTriangleRange to.
	''' </param>
	''' <returns>
	'''     A new instance of the destinationType. 
	''' </returns>
	Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As CultureInfo, value As Object, destinationType As Type) As Object
		If destinationType Is GetType(String) Then
			Return MyBase.ConvertTo(context, culture, value, destinationType)
		End If

		Return value.ToString()
	End Function
End Class
