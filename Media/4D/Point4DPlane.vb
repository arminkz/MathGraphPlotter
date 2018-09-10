Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D

Public Class Point4DPlane

    Public Sub New()
        m_pc = New List(Of Point4D)
    End Sub

    Private m_pc As List(Of Point4D)
    Public Property PointCollection As List(Of Point4D)
        Get
            Return m_pc
        End Get
        Set(value As List(Of Point4D))
            m_pc = value
        End Set
    End Property

    Private m_cols As Integer
    Public Property Columns As Integer
        Get
            Return m_cols
        End Get
        Set(value As Integer)
            m_cols = value
        End Set
    End Property

    Private m_rows As Integer
    Public Property Rows As Integer
        Get
            Return m_rows
        End Get
        Set(value As Integer)
            m_rows = value
        End Set
    End Property

End Class