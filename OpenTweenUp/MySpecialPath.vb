Imports System
Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.Win32

Public Class MySpecialPath

    Public Shared ReadOnly Property UserAppDataPath() As String
        Get
            Return GetFileSystemPath(Environment.SpecialFolder.ApplicationData)
        End Get
    End Property

    Public Shared ReadOnly Property UserAppDataPath(ByVal productName As String) As String
        Get
            Return GetFileSystemPath(Environment.SpecialFolder.ApplicationData, productName)
        End Get
    End Property

    Public Shared ReadOnly Property CommonAppDataPath() As String
        Get
            Return GetFileSystemPath(Environment.SpecialFolder.CommonApplicationData)
        End Get
    End Property

    Public Shared ReadOnly Property LocalUserAppDataPath() As String
        Get
            Return GetFileSystemPath(Environment.SpecialFolder.LocalApplicationData)
        End Get
    End Property

    Public Shared ReadOnly Property CommonAppDataRegistry() As RegistryKey
        Get
            Return GetRegistryPath(Registry.LocalMachine)
        End Get
    End Property

    Public Shared ReadOnly Property UserAppDataRegistry() As RegistryKey
        Get
            Return GetRegistryPath(Registry.CurrentUser)
        End Get
    End Property


    Private Shared Function GetFileSystemPath(ByVal folder As Environment.SpecialFolder) As String
        ' パスを取得
        Dim path As String = String.Format("{0}{3]{1}{3}{2}", _
            Environment.GetFolderPath(folder), _
            Application.CompanyName, _
            Application.ProductName,
            System.IO.Path.DirectorySeparatorChar.ToString)

        ' パスのフォルダを作成
        SyncLock GetType(Application)
            If Not Directory.Exists(path) Then
                Directory.CreateDirectory(path)
            End If
        End SyncLock
        Return path
    End Function 'GetFileSystemPath

    Private Shared Function GetFileSystemPath(ByVal folder As Environment.SpecialFolder, ByVal productName As String) As String
        ' パスを取得
        Dim path As String = String.Format("{0}{3]{1}{3}{2}", _
            Environment.GetFolderPath(folder), _
            Application.CompanyName, _
            productName,
            System.IO.Path.DirectorySeparatorChar)

        ' パスのフォルダを作成
        SyncLock GetType(Application)
            If Not Directory.Exists(path) Then
                Directory.CreateDirectory(path)
            End If
        End SyncLock
        Return path
    End Function 'GetFileSystemPath

    Private Shared Function GetRegistryPath(ByVal key As RegistryKey) As RegistryKey
        ' パスを取得
        Dim basePath As String
        If key Is Registry.LocalMachine Then
            basePath = "SOFTWARE"
        Else
            basePath = "Software"
        End If
        Dim path As String = String.Format("{0}\{1}\{2}", _
            basePath, _
            Application.CompanyName, _
            Application.ProductName)
        ' パスのレジストリ・キーの取得（および作成）
        Return key.CreateSubKey(path)
    End Function 'GetRegistryPath
End Class