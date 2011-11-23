Namespace TweenCustomControl
    Public Class PictureBoxEx
        Inherits PictureBox
        Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
            Try
                MyBase.OnPaint(pe)
            Catch ex As OutOfMemoryException

            End Try
        End Sub
    End Class
End Namespace