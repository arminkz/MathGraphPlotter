Imports System.Text.RegularExpressions

Public Class NumberPutter     'عدد گذاری در تابع

    Public x_index As New List(Of Integer)
    Public y_index As New List(Of Integer)
    Public z_index As New List(Of Integer)
    Public t_index As New List(Of Integer)
    Public r_index As New List(Of Integer)

    Public cx As Boolean = False
    Public cy As Boolean = False
    Public cz As Boolean = False
    Public ct As Boolean = False
    Public cr As Boolean = False

    Public m_formula As String
    Public m_refformula As String

    Public Sub New(Eq As Equation)
        m_formula = Eq.Expression.ToLower
        m_refformula = Eq.Expression.ToLower
        FindIndex()
    End Sub

    Public Sub RefreshFormula()
        m_formula = m_refformula
    End Sub

    Public Sub FindIndex()
        Dim CleanedEq As String = EquationTools.CleanMathFunctions(m_formula)

        If CleanedEq.Contains("x") Then
            Dim TempStr As String = CleanedEq
            Do
                Dim TempIndex As Integer = TempStr.IndexOf("x")
                x_index.Add(TempIndex)
                Dim CFOV As String = TempStr.Substring(TempIndex + 1)
                If CFOV.Contains("x") Then
                    TempStr = TempStr.Remove(TempIndex, 1)
                    TempStr = TempStr.Insert(TempIndex, "?")
                Else
                    Exit Do
                End If
            Loop
            cx = True
        End If
        If CleanedEq.Contains("y") Then
            Dim TempStr As String = CleanedEq
            Do
                Dim TempIndex As Integer = TempStr.IndexOf("y")
                y_index.Add(TempIndex)
                Dim CFOV As String = TempStr.Substring(TempIndex + 1)
                If CFOV.Contains("y") Then
                    TempStr = TempStr.Remove(TempIndex, 1)
                    TempStr = TempStr.Insert(TempIndex, "?")
                Else
                    Exit Do
                End If
            Loop
            cy = True
        End If
        If CleanedEq.Contains("z") Then
            Dim TempStr As String = CleanedEq
            Do
                Dim TempIndex As Integer = TempStr.IndexOf("z")
                z_index.Add(TempIndex)
                Dim CFOV As String = TempStr.Substring(TempIndex + 1)
                If CFOV.Contains("z") Then
                    TempStr = TempStr.Remove(TempIndex, 1)
                    TempStr = TempStr.Insert(TempIndex, "?")
                Else
                    Exit Do
                End If
            Loop
            cz = True
        End If
        If CleanedEq.Contains("t") Then
            Dim TempStr As String = CleanedEq
            Do
                Dim TempIndex As Integer = TempStr.IndexOf("t")
                t_index.Add(TempIndex)
                Dim CFOV As String = TempStr.Substring(TempIndex + 1)
                If CFOV.Contains("t") Then
                    TempStr = TempStr.Remove(TempIndex, 1)
                    TempStr = TempStr.Insert(TempIndex, "?")
                Else
                    Exit Do
                End If
            Loop
            ct = True
        End If
        If CleanedEq.Contains("r") Then
            Dim TempStr As String = CleanedEq
            Do
                Dim TempIndex As Integer = TempStr.IndexOf("r")
                r_index.Add(TempIndex)
                Dim CFOV As String = TempStr.Substring(TempIndex + 1)
                If CFOV.Contains("r") Then
                    TempStr = TempStr.Remove(TempIndex, 1)
                    TempStr = TempStr.Insert(TempIndex, "?")
                Else
                    Exit Do
                End If
            Loop
            cz = True
        End If

        'If m_3d Then
        '    Dim PerXY As Boolean = CleanedEq.Contains("x") And CleanedEq.Contains("y") '1

        '    If PerXY Then
        '        m_index(0) = CleanedEq.IndexOf("x")
        '        m_index(1) = CleanedEq.IndexOf("y")
        '    End If

        '    If PerXY Then
        '        m_pernum = False
        '        m_formula.Remove(m_index(0), 1)
        '        m_formula.Remove(m_index(1), 1)
        '    Else
        '        m_pernum = True
        '    End If
        'Else
        '    Dim PerX As Boolean = CleanedEq.Contains("x") '1
        '    Dim PerY As Boolean = CleanedEq.Contains("y") '2
        '    Dim PerT As Boolean = CleanedEq.Contains("t") '3
        '    Dim PerR As Boolean = CleanedEq.Contains("r") '4

        '    If PerX Then m_index(0) = CleanedEq.IndexOf("x")
        '    If PerY Then m_index(0) = CleanedEq.IndexOf("y")
        '    If PerT Then m_index(0) = CleanedEq.IndexOf("t")
        '    If PerR Then m_index(0) = CleanedEq.IndexOf("r")

        '    If PerX Or PerY Or PerT Or PerR Then
        '        m_pernum = False
        '        m_formula.Remove(m_index(0), 1)
        '    Else
        '        m_pernum = True
        '    End If

        'End If
    End Sub

    Public Function PutX(Num As Double, Optional Preserve As Boolean = False)
        Dim R1 As String = m_formula
    End Function

    'Public Function PutX(Num As Double, Optional Preserve As Boolean = False) As String
    '    If cx Then
    '        Dim FormulaBackup As String = m_formula
    '        Dim Result As String = m_formula
    '        FindIndex()
    '        For Each Index As Integer In x_index
    '            Dim RemovedStr = Result.Remove(Index, 1)
    '            Result = RemovedStr.Insert(Index, Num.ToString)
    '        Next
    '        If Preserve Then
    '            m_formula = Result
    '            FindIndex()
    '        Else
    '            RefreshFormula()
    '        End If
    '        Return Result
    '    Else
    '        Return m_formula
    '    End If
    'End Function

    Public Function PutY(Num As Double, Optional Preserve As Boolean = False) As String
        If cy Then
            Dim FormulaBackup As String = m_formula
            Dim Result As String = m_formula
            For Each Index As Integer In y_index
                Dim RemovedStr = Result.Remove(Index, 1)
                Result = RemovedStr.Insert(Index, Num.ToString)
                FindIndex()
            Next
            If Preserve Then
                m_formula = Result
                FindIndex()
            Else
                m_formula = FormulaBackup
            End If
            Return Result
        Else
            Return m_formula
        End If
    End Function

    Public Function PutZ(Num As Double, Optional Preserve As Boolean = False) As String
        If cz Then
            Dim FormulaBackup As String = m_formula
            Dim Result As String = m_formula
            For Each Index As Integer In z_index
                Dim RemovedStr = Result.Remove(Index, 1)
                Result = RemovedStr.Insert(Index, Num.ToString)
                FindIndex()
            Next
            If Preserve Then
                m_formula = Result
                FindIndex()
            Else
                m_formula = FormulaBackup
            End If
            Return Result
        Else
            Return m_formula
        End If
    End Function

    Public Function PutT(Num As Double, Optional Preserve As Boolean = False) As String
        If ct Then
            Dim FormulaBackup As String = m_formula
            Dim Result As String = m_formula
            For Each Index As Integer In t_index
                Dim RemovedStr = Result.Remove(Index, 1)
                Result = RemovedStr.Insert(Index, Num.ToString)
                FindIndex()
            Next
            If Preserve Then
                m_formula = Result
                FindIndex()
            Else
                m_formula = FormulaBackup
            End If
            Return Result
        Else
            Return m_formula
        End If
    End Function

    Public Function PutR(Num As Double, Optional Preserve As Boolean = False) As String
        If cr Then
            Dim FormulaBackup As String = m_formula
            Dim Result As String = m_formula
            For Each Index As Integer In r_index
                Dim RemovedStr = Result.Remove(Index, 1)
                Result = RemovedStr.Insert(Index, Num.ToString)
                FindIndex()
            Next
            If Preserve Then
                m_formula = Result
                FindIndex()
            Else
                m_formula = FormulaBackup
            End If
            Return Result
        Else
            Return m_formula
        End If
    End Function

End Class
