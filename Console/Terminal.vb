Imports System.Collections.Generic
Imports System.Media
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.IO
Imports System.Text.RegularExpressions

Public Class Terminal
    Inherits TextBox

    Protected Enum CommandHistoryDirection
        Backward
        Forward
    End Enum

    Private m_IsPromptInsertedAtLaunch As Boolean
    Public Property IsPromptInsertedAtLaunch() As Boolean
        Get
            Return m_IsPromptInsertedAtLaunch
        End Get
        Set(value As Boolean)
            m_IsPromptInsertedAtLaunch = value
        End Set
    End Property

    Private m_IsSystemBeepEnabled As Boolean
    Public Property IsSystemBeepEnabled() As Boolean
        Get
            Return m_IsSystemBeepEnabled
        End Get
        Set(value As Boolean)
            m_IsSystemBeepEnabled = value
        End Set
    End Property

    Private m_Prompt As String
    Public Property Prompt() As String
        Get
            Return m_Prompt
        End Get
        Set(value As String)
            m_Prompt = value
        End Set
    End Property

    Private m_RegisteredCommands As List(Of String)
    Public Property RegisteredCommands() As List(Of String)
        Get
            Return m_RegisteredCommands
        End Get
        Private Set(value As List(Of String))
            m_RegisteredCommands = value
        End Set
    End Property

    Private m_CommandLog As List(Of TerminalCommand)
    Public Property CommandLog() As List(Of TerminalCommand)
        Get
            Return m_CommandLog
        End Get
        Private Set(value As List(Of TerminalCommand))
            m_CommandLog = value
        End Set
    End Property

    Private m_LastPomptIndex As Integer
    Public Property LastPomptIndex() As Integer
        Get
            Return m_LastPomptIndex
        End Get
        Private Set(value As Integer)
            m_LastPomptIndex = value
        End Set
    End Property

    Private m_IsInputEnabled As Boolean
    Public Property IsInputEnabled() As Boolean
        Get
            Return m_IsInputEnabled
        End Get
        Private Set(value As Boolean)
            m_IsInputEnabled = value
        End Set
    End Property

    Private CommandLogIndex As Integer = 0

    Public Sub New()
        'غیر فعال سازی برخی خواص TextBox
        IsUndoEnabled = False
        AcceptsReturn = False
        AcceptsTab = False

        RegisteredCommands = New List(Of String)()
        CommandLog = New List(Of TerminalCommand)()
        IsPromptInsertedAtLaunch = True
        IsSystemBeepEnabled = True
        LastPomptIndex = -1
        Prompt = "> "
        IsInputEnabled = False

        AddHandler Loaded, Function(s, e)
                               If IsPromptInsertedAtLaunch Then
                                   InsertNewPrompt()
                               End If
                               Return 0
                           End Function

        AddHandler TextChanged, Function(s, e)
                                    ScrollToEnd()
                                    Return 0
                                End Function

    End Sub

#Region "رابط کاربری"

    Public Sub InsertNewPrompt()
        If Text.Length > 0 Then
            Text += If(Text.EndsWith(vbCrLf), "", vbCrLf)
        End If
        Text += Prompt
        CaretIndex = Text.Length
        LastPomptIndex = Text.Length
        IsInputEnabled = True
    End Sub

    Public Sub InsertLineBeforePrompt(Text1 As String)
        Dim OldPromptIndex As Integer = LastPomptIndex
        Dim InsertedText As String = Text1 & (If(Text1.EndsWith(vbCrLf), "", vbCrLf))
        Text = Text.Insert(LastPomptIndex - Prompt.Length, InsertedText)
        CaretIndex = Text.Length
        LastPomptIndex = OldPromptIndex + InsertedText.Length
    End Sub

#End Region

#Region "رسیدگی کننده روادید"

    Protected Overrides Sub OnPreviewKeyDown(e As KeyEventArgs)

        'F3   واگذاری رسیدگی به پنجره
        If e.Key = Key.F3 Then
            e.Handled = False
            Return
        End If

        'Ctrl+C لغو فرمان درخواست شده
        If e.Key = Key.C Then
            If Keyboard.IsKeyDown(Key.LeftCtrl) OrElse Keyboard.IsKeyDown(Key.RightCtrl) Then
                RaiseAbortRequested()
                e.Handled = True
                Return
            End If
        End If

        'طول اولیه متن
        Dim InitialLength As Integer = Text.Length

        'هشدار به کاربر در صورت غیرفعال بودن ورودی
        If Not IsInputEnabled Then
            If IsSystemBeepEnabled Then
                SystemSounds.Beep.Play()
            End If
            e.Handled = True
            Return
        End If

        'تست موقیعت نشان فرمان

        ' 1. اگر قبل از خط فرمان باشد
        '    --> نشان فرمان را به اخر منتقل کن و اگر کاربر قصد پاک کردن دارد فرمان را نادیده بگیر
        '        اگر قصد پاک کردن ندارد فرمان را اجرا کن
        If CaretIndex < LastPomptIndex Then
            If IsSystemBeepEnabled Then
                SystemSounds.Beep.Play()
            End If
            CaretIndex = Text.Length
            e.Handled = False
            If e.Key = Key.Back OrElse e.Key = Key.Delete Then
                e.Handled = True
            End If

            ' 2. اگر دقیقا بعد از خط فرمان باشد
            '    --> اگر کاربر قصد پاک کردن دارد نادیده بگیر
        ElseIf CaretIndex = LastPomptIndex AndAlso e.Key = Key.Back Then
            If IsSystemBeepEnabled Then
                SystemSounds.Beep.Play()
            End If
            e.Handled = True

            ' 3. اگر دقیقا بعد از خط فرمان باشد
            '    --> اگر کاربر می خواهد به عقب برگردد فرمان رانادیده بگیر
        ElseIf CaretIndex = LastPomptIndex AndAlso e.Key = Key.Left Then
            If IsSystemBeepEnabled Then
                SystemSounds.Beep.Play()
            End If
            e.Handled = True

            ' 4. اگر دقیقا بعد از خط فرمان باشد 
            '    --> اگر کاربر کلید بالا را فشار دهد از تاریخچه فرمان قبلی را بازخوانی کن
        ElseIf CaretIndex >= LastPomptIndex AndAlso e.Key = Key.Up Then
            HandleCommandHistoryRequest(CommandHistoryDirection.BACKWARD)
            e.Handled = True

            ' 5. اگر دقیقا بعد از خط فرمان باشد 
            '    --> اگر کاربر کلید پایین را فشار دهد از تاریخچه فرمان بعدی را بازخوانی کن
        ElseIf CaretIndex >= LastPomptIndex AndAlso e.Key = Key.Down Then
            HandleCommandHistoryRequest(CommandHistoryDirection.FORWARD)
            e.Handled = True
        End If



        ' اگر فرمان نادیده گرفته نشد بدنبال کلید های ویژه بگرد

        ' ENTER   => تصویب فرمان
        ' TAB     => کامل کردن فرمان با مقادیر از پیش ثبت شده

        If Not e.Handled Then
            Select Case e.Key
                Case Key.Enter
                    HandleEnterKey()
                    e.Handled = True
                    Exit Select
                Case Key.Tab
                    HandleTabKey()
                    e.Handled = True
                    Exit Select
            End Select
        End If

        MyBase.OnPreviewKeyDown(e)
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseButtonEventArgs)
        Me.Focus()
        CaretIndex = Text.Length
        e.Handled = True
        MyBase.OnMouseDown(e)
    End Sub

#End Region

#Region "تاریخچه دستورات"

    Protected Overridable Sub HandleCommandHistoryRequest(Direction As CommandHistoryDirection)
        Select Case Direction
            Case CommandHistoryDirection.Backward
                If CommandLogIndex > 0 Then
                    CommandLogIndex -= 1
                End If
                If CommandLog.Count > 0 Then
                    Text = GetTextWithPromptSuffix(CommandLog(CommandLogIndex).Raw)
                    CaretIndex = Text.Length
                End If
                Exit Select

            Case CommandHistoryDirection.Forward
                If CommandLogIndex < CommandLog.Count - 1 Then
                    CommandLogIndex += 1
                End If
                If CommandLog.Count > 0 Then
                    Text = GetTextWithPromptSuffix(CommandLog(CommandLogIndex).Raw)
                    CaretIndex = Text.Length
                End If
                Exit Select
        End Select
    End Sub
#End Region

    Protected Overridable Sub HandleEnterKey()
        Dim Line As String = Text.Substring(LastPomptIndex)
        If Line = "" Then
            If IsSystemBeepEnabled Then
                SystemSounds.Beep.Play()
            End If
            Return
        End If
        Text += vbLf
        IsInputEnabled = False
        LastPomptIndex = Integer.MaxValue

        Dim CMD As TerminalCommand = ParseCommandLine(Line)
        CommandLog.Add(CMD)
        CommandLogIndex = CommandLog.Count
        RaiseCommandEntered(CMD)
    End Sub

    Protected Overridable Sub HandleTabKey()
        'تکمیل خودکار Auto Complete
        If CaretIndex <> Text.Length OrElse CaretIndex = LastPomptIndex Then
            Return
        End If

        Dim Line As String = Text.Substring(LastPomptIndex)
        Dim Commands As String() = GetAssociatedCommands(Line)

        If Commands.Length > 0 Then
            Dim CommonPrefix As String = GetCommonPrefix(Commands)
            If CommonPrefix = Line Then
                ' If there are more than one command to print
                If Commands.Length > 1 Then
                    ' Print every associated command and insert a new prompt
                    For Each cmd As String In Commands
                        Text += vbLf & cmd
                    Next
                    InsertNewPrompt()
                    Text += Line
                    CaretIndex = Text.Length
                End If
            Else
                Text = Text.Remove(LastPomptIndex)
                Text += CommonPrefix
                CaretIndex = Text.Length
            End If
            Return
        End If

#If DEBUG Then
        ' If no command exists, try path completion
        If Line.Contains("""") AndAlso Line.Split(""""c).Length Mod 2 = 0 Then
            Dim idx As Integer = Line.LastIndexOf(""""c)
            Dim prefix As String = Line.Substring(0, idx + 1)
            Dim suffix As String = Line.Substring(idx + 1, Line.Length - prefix.Length)
            CompletePath(prefix, suffix)
        Else
            Dim idx As Integer = Math.Max(Line.LastIndexOf(" "c), Line.LastIndexOf(ControlChars.Tab))
            Dim prefix As String = Line.Substring(0, idx + 1)
            Dim suffix As String = Line.Substring(idx + 1, Line.Length - prefix.Length)
            CompletePath(prefix, suffix)
        End If
#End If

    End Sub

#Region "تبدیل متن به دستور"

    Public Sub RemoteCommand(Line As String)
        Dim CMD As TerminalCommand = ParseCommandLine(Line)
        CMD.IsRemote = True
        RaiseCommandEntered(CMD)
    End Sub

    Public Shared Function ParseCommandLine(Line As String) As TerminalCommand
        Dim Command As String = ""
        Dim Args As New List(Of String)()

        Dim M As Match = Regex.Match(Line.Trim() & " ", "^(.+?)(?:\s+|$)(.*)")
        If M.Success Then
            Command = M.Groups(1).Value.Trim()
            Dim ArgsLine As String = M.Groups(2).Value.Trim()
            Dim M2 As Match = Regex.Match(ArgsLine & " ", "(?<!\\)"".*?(?<!\\)""|[\S]+")
            While M2.Success
                Dim Arg As String = Regex.Replace(M2.Value.Trim(), "^""(.*?)""$", "$1")
                Args.Add(Arg)
                M2 = M2.NextMatch()
            End While
        End If

        Return New TerminalCommand(Line, Command, Args.ToArray())
    End Function

#End Region

#Region "ابزار های اختصاصی"

    Protected Sub CompletePath(LinePrefix As String, LineSuffix As String)
        If LineSuffix.Contains("\") OrElse LineSuffix.Contains("/") Then
            Dim idx As Integer = Math.Max(LineSuffix.LastIndexOf("\"), LineSuffix.LastIndexOf("/"))
            Dim dir As String = LineSuffix.Substring(0, idx + 1)
            Dim prefix As String = LineSuffix.Substring(idx + 1, LineSuffix.Length - dir.Length)
            Dim files As String() = GetFileList(dir, LineSuffix(idx) = "\"c)

            Dim commonPrefixFiles As New List(Of String)()
            For Each file As String In files
                If file.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase) Then
                    commonPrefixFiles.Add(file)
                End If
            Next
            If commonPrefixFiles.Count > 0 Then
                Dim commonPrefix As String = GetCommonPrefix(commonPrefixFiles.ToArray())
                If commonPrefix = prefix Then
                    For Each file As String In commonPrefixFiles
                        Text += vbLf & file
                    Next
                    InsertNewPrompt()
                    Text += LinePrefix & LineSuffix
                    CaretIndex = Text.Length
                Else
                    Text = Text.Remove(LastPomptIndex)
                    Text += LinePrefix & dir & commonPrefix
                    CaretIndex = Text.Length
                End If
            End If
        End If
    End Sub

    Protected Function GetTextWithPromptSuffix(Suffix As String) As String
        Dim Ret As String = Text.Substring(0, LastPomptIndex)
        Return Ret & Suffix
    End Function

    Protected Function GetAssociatedCommands(Prefix As String) As String()
        Dim Ret As New List(Of String)()
        For Each CMD As Object In RegisteredCommands
            If CMD.StartsWith(Prefix, StringComparison.InvariantCultureIgnoreCase) Then
                Ret.Add(CMD)
            End If
        Next
        Return Ret.ToArray()
    End Function
#End Region

#Region "ابزار های عمومی"

    Protected Function GetShortestString(Strs As String()) As String
        Dim Ret As String = Strs(0)
        For Each Str As String In Strs
            Ret = If(Str.Length < Ret.Length, Str, Ret)
        Next
        Return Ret
    End Function

    Protected Function GetCommonPrefix(Strs As String()) As String
        Dim ShortestStr As String = GetShortestString(Strs)
        For i As Integer = 0 To ShortestStr.Length - 1
            For Each Str As String In Strs
                If Char.ToLower(Str(i)) <> Char.ToLower(ShortestStr(i)) Then
                    Return ShortestStr.Substring(0, i)
                End If
            Next
        Next
        Return ShortestStr
    End Function

    Protected Function GetFileList(dir As String, useAntislash As Boolean) As String()
        If Not Directory.Exists(dir) Then
            Return New String(-1) {}
        End If
        Dim dirs As String() = Directory.GetDirectories(dir)
        Dim files As String() = Directory.GetFiles(dir)

        For i As Integer = 0 To dirs.Length - 1
            dirs(i) = Path.GetFileName(dirs(i)) & (If(useAntislash, "\", "/"))
        Next
        For i As Integer = 0 To files.Length - 1
            files(i) = Path.GetFileName(files(i))
        Next

        Dim ret As New List(Of String)()
        ret.AddRange(dirs)
        ret.AddRange(files)
        Return ret.ToArray()
    End Function

#End Region

#Region "رخداد ها"

    Public Event AbortRequested As EventHandler(Of EventArgs)
    Public Event CommandEntered As EventHandler(Of CommandEventArgs)

    Public Class CommandEventArgs
        Inherits EventArgs

        Private m_Command As TerminalCommand
        Public Property Command() As TerminalCommand
            Get
                Return m_Command
            End Get
            Private Set(value As TerminalCommand)
                m_Command = value
            End Set
        End Property

        Public Sub New(Command1 As TerminalCommand)
            Command = Command1
        End Sub
    End Class

    Private Sub RaiseAbortRequested()
        RaiseEvent AbortRequested(Me, New EventArgs())
    End Sub

    Public Sub RaiseCommandEntered(Command As TerminalCommand)
        RaiseEvent CommandEntered(Me, New CommandEventArgs(Command))
    End Sub

#End Region

End Class