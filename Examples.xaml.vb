Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class Examples

    Private ExmList As New ObservableCollection(Of Example)

    Public Sub New()
        InitializeComponent()

        For Each E As Example In ExampleList.Items
            ExmList.Add(E)
        Next

        Dim View As ICollectionView = CollectionViewSource.GetDefaultView(ExmList)
        View.GroupDescriptions.Add(New PropertyGroupDescription("Group"))
        'View.SortDescriptions.Add(New SortDescription("Country", ListSortDirection.Ascending))
        'View.SortDescriptions.Add(New SortDescription("Name", ListSortDirection.Ascending))
        ExampleList.Items.Clear()
        ExampleList.ItemsSource = View

    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)


    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub LoadExample(sender As Object, e As RoutedEventArgs)
        If ExampleList.SelectedIndex <> -1 Then
            Dim KeyToLoad As String = TryCast(ExampleList.SelectedItem, Example).Key & ".mgp"
            TryCast(Owner, MainWindow).ReadFromResource(KeyToLoad)
            Me.Hide()
        End If
    End Sub


End Class

Public Class Example

    Public Property Key As String
    Public Property Description As String
    Public Property Group As String

    Public ReadOnly Property ImageSource As String
        Get
            Return "/AKP Math Graph Plotter;component/Examples/Example Images/" + Key + ".png"
        End Get
    End Property

    Private m_bc As Color
    Public Property BackColor As Color
        Get
            If m_bc <> Nothing Then
                Return m_bc
            Else
                Return Colors.Transparent
            End If
        End Get
        Set(value As Color)
            m_bc = value
        End Set
    End Property

    Private m_fc
    Public Property ForeColor As Color
        Get
            If m_fc <> Nothing Then
                Return m_fc
            Else
                Return Colors.Black
            End If
        End Get
        Set(value As Color)
            m_fc = value
        End Set
    End Property
End Class