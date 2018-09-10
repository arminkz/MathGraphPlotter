Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D

Public Class Point3DPlane

    Public Sub New()
        m_pc = New Point3DCollection
    End Sub

    Private m_pc As Point3DCollection
    Public Property PointCollection As Point3DCollection
        Get
            Return m_pc
        End Get
        Set(value As Point3DCollection)
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