Imports System.Diagnostics
Imports System.Threading
Imports System.IO
Imports System
Imports System.Threading.Thread

Public Class Form1

    Private TWEENEXEPATH As String = Application.StartupPath
    Private SOURCEPATH As String = Application.StartupPath

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' 文字列リソースから設定
        Me.Text = My.Resources.FormTitle
        Label1.Text = My.Resources.TweenUpdating
        Label2.Text = My.Resources.PleaseWait

        If My.Application.CommandLineArgs.Count > 0 Then TWEENEXEPATH = My.Application.CommandLineArgs(0)

        If My.Application.CommandLineArgs.Count = 1 AndAlso IsRequiredUAC() Then
            Me.Visible = False
            Dim p As New Process()
            p.StartInfo.FileName = Path.Combine(Application.StartupPath, My.Application.Info.AssemblyName + ".exe")
            p.StartInfo.UseShellExecute = True
            p.StartInfo.WorkingDirectory = Application.StartupPath
            p.StartInfo.Arguments = """" + TWEENEXEPATH + """ up"
            p.StartInfo.Verb = "RunAs"
            Try
                p.Start()
                p.WaitForExit()
            Catch ex As System.ComponentModel.Win32Exception
                Application.Exit()
            Catch ex As Exception
            Finally
                p.Close()
            End Try

            Process.Start(Path.Combine(TWEENEXEPATH, My.Resources.FilenameTweenExe))
            Application.Exit()
            Exit Sub
        End If

        ' exe自身からフォームのアイコンを取得
        Me.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath)

#If 0 Then
        If Environment.GetCommandLineArgs().Length <> 1 AndAlso Directory.Exists(Environment.GetCommandLineArgs()(1)) Then
            TWEENEXEPATH = Environment.GetCommandLineArgs()(1)
            SOURCEPATH = Path.GetTempPath
        End If
#End If
    End Sub

    Private Function IsRequiredUAC() As Boolean
        Dim os As OperatingSystem = System.Environment.OSVersion
        If os.Platform = PlatformID.Win32NT AndAlso os.Version.Major >= 6 Then Return True
        Return False
    End Function

    Private Sub Form1_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Me.BackgroundWorker1.WorkerReportsProgress = True
        Me.BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub BackupConfigurationFile()
        'Dim SrcFile As String
        'Dim DstFile As String

        'Try
        '    SrcFile = Path.Combine(TWEENEXEPATH, "Tween.exe.Config")
        '    DstFile = Path.Combine(TWEENEXEPATH, "Tween.exe.Config.Backup" + DateTime.Now.ToString("yyyyMMddHHmmss"))

        '    File.Copy(SrcFile, DstFile, True)
        'Catch ex As Exception

        'End Try

        'Try
        '    SrcFile = Path.Combine(TWEENEXEPATH, "TweenConf.xml")
        '    DstFile = Path.Combine(TWEENEXEPATH, "TweenConf.xml.Backup" + DateTime.Now.ToString("yyyyMMddHHmmss"))

        '    File.Copy(SrcFile, DstFile, True)
        'Catch ex As Exception

        'End Try

        Try
            Dim bkDir2 As String = Path.Combine(SOURCEPATH, "TweenBackup2nd")
            If Directory.Exists(bkDir2) Then
                Directory.Delete(bkDir2, True)
            End If
            Dim bkDir As String = Path.Combine(SOURCEPATH, "TweenBackup1st")
            If Directory.Exists(bkDir) Then
                Directory.Move(bkDir, bkDir2)
            End If
        Catch ex As Exception

        End Try
        Try
            Dim bkDir As String = Path.Combine(SOURCEPATH, "TweenBackup1st")
            If Not Directory.Exists(bkDir) Then
                Directory.CreateDirectory(bkDir)
            End If
            For Each file As FileInfo In (New DirectoryInfo(SOURCEPATH + Path.DirectorySeparatorChar)).GetFiles("*.xml")
                file.CopyTo(Path.Combine(bkDir, file.Name), True)
            Next
        Catch ex As Exception

        End Try

    End Sub

    Private Sub DeleteOldFiles()
        Try
            Dim bkDir As String = Path.Combine(SOURCEPATH, "TweenOldFiles")
            If Not Directory.Exists(bkDir) Then
                Directory.CreateDirectory(bkDir)
            End If
            'ログファイルの削除
            Dim cDir As New DirectoryInfo(SOURCEPATH + Path.DirectorySeparatorChar)
            For Each file As FileInfo In cDir.GetFiles("Tween*.log")
                file.MoveTo(Path.Combine(bkDir, file.Name))
            Next
            '旧設定ファイルの削除
            For Each file As FileInfo In cDir.GetFiles("Tween.exe.config.Backup*")
                file.MoveTo(Path.Combine(bkDir, file.Name))
            Next
            '旧設定XMLファイルの削除
            For Each file As FileInfo In cDir.GetFiles("TweenConf.xml.Backup*")
                file.MoveTo(Path.Combine(bkDir, file.Name))
            Next
            '旧設定XMLファイルの削除
            For Each file As FileInfo In cDir.GetFiles("Setting*.xml.Backup*")
                file.MoveTo(Path.Combine(bkDir, file.Name))
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Const WaitTime As Integer = 5000 ' スリープ時間
        Dim cultures As New List(Of String)     'リソースを配置するフォルダ名（カルチャ名）

        cultures.AddRange(New String() { _
                            "en" _
                          })
        Dim curCul As String = ""
        If Not CurrentThread.CurrentUICulture.IsNeutralCulture Then
            Dim idx As Integer = CurrentThread.CurrentUICulture.Name.LastIndexOf("-"c)
            If idx > -1 Then
                curCul = CurrentThread.CurrentUICulture.Name.Substring(0, idx)
            Else
                curCul = CurrentThread.CurrentUICulture.Name
            End If
        Else
            curCul = CurrentThread.CurrentUICulture.Name
        End If
        If String.IsNullOrEmpty(curCul) AndAlso curCul <> "en" Then cultures.Add(curCul)

        BackgroundWorker1.ReportProgress(0, userState:=My.Resources.ProgressWaitForTweenExit)
        System.Threading.Thread.Sleep(WaitTime) ' スリープ

        Dim ImagePath As New ArrayList()

        'TweenUp.exeと同じフォルダのTween.exeは無条件に対象
        ImagePath.Add(Path.Combine(TWEENEXEPATH, My.Resources.FilenameTweenExe))

        ' Tween という名前のプロセスを取得
        Dim ps As Process() = Process.GetProcessesByName(My.Resources.WaitProcessName)
        Dim p As Process

        '       Console.WriteLine("取得開始")

        ' コレクションをスキャン
        For Each p In ps
            If ImagePath.Contains(p.MainModule.FileName) = True Then
                '' 終了指示
                'If Not p.CloseMainWindow() Then
                '    'アイコン化、ダイアログ表示など、終了を受け付けられる状態ではない。
                '    Throw New ApplicationException(My.Resources.TimeOutException)
                'End If
                If Not p.WaitForExit(60000) Then
                    ' 強制終了
                    'p.Kill()
                    'If Not p.WaitForExit(10000) Then
                    ' だいたい30秒ぐらい（適当）たってもだめなら例外を発生させる
                    Throw New ApplicationException(My.Resources.TimeOutException)
                    'End If
                End If
                Exit For
            End If
        Next

        'BackgroundWorker1.ReportProgress(0, userState:=My.Resources.ProgressProcessKill)
        'Thread.Sleep(WaitTime) ' スリープ

        ' 「Tweenの終了を検出しました」
        BackgroundWorker1.ReportProgress(0, userState:=My.Resources.ProgressDetectTweenExit)

        Thread.Sleep(WaitTime) ' スリープ

        ' 設定ファイルのバックアップ
        BackgroundWorker1.ReportProgress(0, userState:=My.Resources.ProgressBackup)
        BackupConfigurationFile()
        DeleteOldFiles()
        Thread.Sleep(WaitTime)

        BackgroundWorker1.ReportProgress(0, userState:=My.Resources.ProgressCopying)
        For Each DstFile In ImagePath
            '本体
            Dim SrcFile As String = Path.Combine(SOURCEPATH, My.Resources.FilenameNew)
            If System.IO.File.Exists(SrcFile) Then
                ' ImagePathに格納されているファイルにTweenNew.exeを上書き
                File.Copy(SrcFile, DstFile.ToString, True)
                ' TweenNew.exeを削除
                File.Delete(Path.Combine(SOURCEPATH, My.Resources.FilenameNew))
            End If
            'リソース
            'Dim resDirs As String() = Directory.GetDirectories(SOURCEPATH, "*", SearchOption.TopDirectoryOnly)
            'ディレクトリ探索
            For Each spath As String In Directory.GetDirectories(SOURCEPATH, "*", SearchOption.TopDirectoryOnly)
                Dim cul As String = spath.Substring(spath.LastIndexOf(Path.DirectorySeparatorChar) + 1)
                Dim SrcFileRes As String = Path.Combine(spath, My.Resources.FilenameResourceNew)
                Dim DstFileRes As String = Path.Combine(Path.Combine(TWEENEXEPATH, cul), My.Resources.FilenameResourceDll)

                If System.IO.File.Exists(SrcFileRes) Then
                    ' リソースフォルダが更新先に存在しない場合は作成する
                    If Not Directory.Exists(Path.Combine(TWEENEXEPATH, cul)) Then
                        Directory.CreateDirectory(Path.Combine(TWEENEXEPATH, cul))
                    End If
                    ' リソースファイルの上書き
                    File.Copy(SrcFileRes, DstFileRes, True)
                    ' リソースファイル削除
                    File.Delete(SrcFileRes)
                End If
            Next
            'シリアライザDLL
            Dim SrcFileDll As String = Path.Combine(SOURCEPATH, My.Resources.FilenameDllNew)
            Dim DstFileDll As String = Path.Combine(TWEENEXEPATH, My.Resources.FilenameDll)
            If System.IO.File.Exists(SrcFileDll) Then
                File.Copy(SrcFileDll, DstFileDll, True)
                File.Delete(SrcFileDll)
            End If
        Next

        Thread.Sleep(WaitTime) ' スリープ


        ' ネイティブイメージにコンパイル
        'Call GenerateNativeImage()

        ' 「新しいTweenを起動しています」
        BackgroundWorker1.ReportProgress(0, userState:=My.Resources.ProgressExecuteTween)

        If My.Application.CommandLineArgs.Count = 1 Then
            Process.Start(Path.Combine(TWEENEXEPATH, My.Resources.FilenameTweenExe))
        End If

        'Process.Start(Path.Combine(TWEENEXEPATH, My.Resources.FilenameTweenExe))

    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If (e.Error IsNot Nothing) Then
            ' 例外が発生していた場合はthrowする
            Throw e.Error
        End If

        Me.Close()
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        ' 進行状況を書き直す　同時にVisibleにする
        LabelProgress.Text = e.UserState.ToString


        ' ラベルコントロールをセンタリング
        LabelProgress.Left = (Me.ClientSize.Width - LabelProgress.Size.Width) \ 2

        LabelProgress.Refresh()
        LabelProgress.Visible = True

    End Sub
#If 0 Then
    Private Sub GenerateNativeImage()
        ' Tween.exeをプリコンパイル
        Try
            Dim psi = New ProcessStartInfo()

            psi.Arguments = "/nologo /silent " + Chr(34) + Path.Combine(TWEENEXEPATH, My.Resources.FilenameTweenExe) + Chr(34)
            psi.FileName = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "ngen.exe")
            psi.WindowStyle = ProcessWindowStyle.Hidden
            Process.Start(psi).WaitForExit()
        Catch

        End Try

    End Sub
#End If
End Class
