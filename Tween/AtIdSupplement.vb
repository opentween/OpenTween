Public Class AtIdSupplement

    Public inputText As String = ""
    Public isBack As Boolean = False
    Private startChar As String = ""

    Public Sub AddItem(ByVal id As String)
        If Not Me.TextId.AutoCompleteCustomSource.Contains(id) Then
            Me.TextId.AutoCompleteCustomSource.Add(id)
        End If
    End Sub

    Public Function GetItemList() As List(Of String)
        Dim ids As New List(Of String)
        For i As Integer = 0 To Me.TextId.AutoCompleteCustomSource.Count - 1
            ids.Add(Me.TextId.AutoCompleteCustomSource(i))
        Next
        Return ids
    End Function

    Public ReadOnly Property ItemCount() As Integer
        Get
            Return Me.TextId.AutoCompleteCustomSource.Count
        End Get
    End Property

    Private Sub ButtonOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonOK.Click
        inputText = Me.TextId.Text
        isBack = False
    End Sub

    Private Sub ButtonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        inputText = ""
        isBack = False
    End Sub

    Private Sub TextId_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextId.KeyDown
        'If e.KeyCode = Keys.Enter Then
        '    inputId = Me.TextId.Text
        '    Me.Close()
        'End If
        If e.KeyCode = Keys.Back AndAlso Me.TextId.Text = "" Then
            inputText = ""
            isBack = True
            Me.Close()
        End If
        If e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.Tab Then
            inputText = Me.TextId.Text + " "
            isBack = False
            Me.Close()
        End If
        If e.Control AndAlso e.KeyCode = Keys.Delete Then
            If Me.TextId.Text <> "" Then
                Dim idx As Integer = Me.TextId.AutoCompleteCustomSource.IndexOf(Me.TextId.Text)
                If idx > -1 Then
                    Me.TextId.Text = ""
                    Me.TextId.AutoCompleteCustomSource.RemoveAt(idx)
                End If
            End If
        End If
    End Sub

    Private Sub AtIdSupplement_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Me.Height = Me.TextId.Height + SystemInformation.ToolWindowCaptionHeight + Me.TextId.Margin.Top + Me.Label1.Height
    End Sub

    Private Sub AtIdSupplement_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        'TextId.Text = startChar
        'TextId.SelectionStart = 1
        TextId.Text = startChar
    End Sub

    Public Sub New()

        ' この呼び出しは、Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

    End Sub

    Public Sub New(ByVal ItemList As List(Of String), ByVal startCharacter As String)

        ' この呼び出しは、Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        For i As Integer = 0 To ItemList.Count - 1
            Me.TextId.AutoCompleteCustomSource.Add(ItemList(i))
        Next
        startChar = startCharacter

    End Sub

    Private Sub TextId_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles TextId.PreviewKeyDown
        If e.KeyCode = Keys.Tab Then
            inputText = Me.TextId.Text + " "
            isBack = False
            Me.Close()
        End If
    End Sub
End Class
