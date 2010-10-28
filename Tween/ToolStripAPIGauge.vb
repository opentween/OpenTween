Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public Class ToolStripAPIGauge
    Inherits ToolStripControlHost

    Public Sub New()
        MyBase.New(New Control())
        Me.AutoToolTip = True
        AddHandler Me.Control.Paint, AddressOf Draw
    End Sub

    Private _gaugeHeight As Integer
    Public Property GaugeHeight As Integer
        Set(ByVal value As Integer)
            Me._gaugeHeight = value
            Me.Control.Refresh()
        End Set
        Get
            Return _gaugeHeight
        End Get
    End Property

    Private _maxCount As Integer = 350
    Public Property MaxCount As Integer
        Set(ByVal value As Integer)
            Me._maxCount = value
            Me.Control.Refresh()
        End Set
        Get
            Return Me._maxCount
        End Get
    End Property

    Private _remainCount As Integer
    Public Property RemainCount As Integer
        Set(ByVal value As Integer)
            Me._remainCount = value
            Me.Control.Refresh()
        End Set
        Get
            Return Me._remainCount
        End Get
    End Property

    Private _resetTime As DateTime
    Public Property ResetTime As DateTime
        Set(ByVal value As DateTime)
            Me._resetTime = value
            If Me._resetTime >= DateTime.Now Then
                Me.ToolTipText = "ResetTime " + Me._resetTime.ToString()
            Else
                Me.ToolTipText = "ResetTime ???"
            End If
            Me.Control.Refresh()
        End Set
        Get
            Return Me._resetTime
        End Get
    End Property

    Private Sub Draw(ByVal sender As Object, ByVal e As PaintEventArgs)
        Dim textFormat As String = "API {0}/{1}"
        Dim text As String
        If Me._remainCount > -1 AndAlso Me._maxCount > -1 Then
            ' 正常
            text = String.Format(textFormat, Me._remainCount, Me._maxCount)
        ElseIf Me.RemainCount > -1 Then
            ' uppercount不正
            text = String.Format(textFormat, Me._remainCount, "???")
        ElseIf Me._maxCount < -1 Then
            ' remaincount不正
            text = String.Format(textFormat, "???", Me._maxCount)
        Else
            ' 両方とも不正
            text = String.Format(textFormat, "???", "???")
        End If

        Dim minute As Double = (Me.ResetTime - DateTime.Now).TotalMinutes
        Dim apiGaugeBounds As New Rectangle(0, _
                                            CType((Me.Control.Height - (Me._gaugeHeight * 2)) / 2, Integer), _
                                            CType(e.ClipRectangle.Width * (Me.RemainCount / Me._maxCount), Integer), _
                                            Me._gaugeHeight)
        Dim timeGaugeBounds As New Rectangle(0, _
                                             apiGaugeBounds.Top + Me._gaugeHeight, _
                                             CType(e.ClipRectangle.Width * (minute / 60), Integer), _
                                             Me._gaugeHeight)

        e.Graphics.FillRectangle(Brushes.LightBlue, apiGaugeBounds)
        e.Graphics.FillRectangle(Brushes.LightPink, timeGaugeBounds)
        e.Graphics.DrawString(text, Me.Control.Font, SystemBrushes.ControlText, 0, CType(timeGaugeBounds.Top - (Me.Control.Font.Height / 2), Single))
    End Sub
End Class