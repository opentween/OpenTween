Imports System.Reflection
Imports System.IO

Public Class GrowlHelper

    Private _connector As Assembly = Nothing
    Private _core As Assembly = Nothing

    Private _growlNTreply As Object
    Private _growlNTdm As Object
    Private _growlNTnew As Object
    Private _growlNTusevent As Object
    Private _growlApp As Object

    Private _t As Type
    Private _target As Object

    Private _appName As String = ""
    Dim _initialized As Boolean = False

    Public ReadOnly Property appName As String
        Get
            Return _appName
        End Get
    End Property

    Public Enum NotifyType
        Reply = 0
        DirectMessage = 1
        Notify = 2
        UserStreamEvent = 3
    End Enum

    Public Sub New(ByVal appName As String)
        _appName = appName
    End Sub

    Public ReadOnly Property IsAvailable As Boolean
        Get
            If _connector Is Nothing OrElse _core Is Nothing OrElse Not _initialized Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    Public Shared ReadOnly Property IsDllExists As Boolean
        Get
            Dim dir As String = Application.StartupPath
            Dim connectorPath As String = Path.Combine(dir, "Growl.Connector.dll")
            Dim corePath As String = Path.Combine(dir, "Growl.CoreLibrary.dll")
            If File.Exists(connectorPath) AndAlso File.Exists(corePath) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Function RegisterGrowl() As Boolean

        _initialized = False

        Dim dir As String = Application.StartupPath
        Dim connectorPath As String = Path.Combine(dir, "Growl.Connector.dll")
        Dim corePath As String = Path.Combine(dir, "Growl.CoreLibrary.dll")

        Try
            If Not IsDllExists Then Return False
            _connector = Assembly.LoadFile(connectorPath)
            _core = Assembly.LoadFile(corePath)
        Catch ex As Exception
            Return False
        End Try

        Try
            _target = _connector.CreateInstance("Growl.Connector.GrowlConnector")
            _t = _connector.GetType("Growl.Connector.NotificationType")

            _growlNTreply = _t.InvokeMember(Nothing,
                BindingFlags.CreateInstance, Nothing, Nothing, New Object() {"REPLY", "Reply"})

            _growlNTdm = _t.InvokeMember(Nothing,
                BindingFlags.CreateInstance, Nothing, Nothing, New Object() {"DIRECT_MESSAGE", "DirectMessage"})

            _growlNTnew = _t.InvokeMember(Nothing,
                BindingFlags.CreateInstance, Nothing, Nothing, New Object() {"NOTIFY", "新着通知"})

            _growlNTusevent = _t.InvokeMember(Nothing,
                BindingFlags.CreateInstance, Nothing, Nothing, New Object() {"USERSTREAM_EVENT", "UserStream Event"})

            Dim encryptType As Object =
                    _connector.GetType("Growl.Connector.Cryptography+SymmetricAlgorithmType").InvokeMember(
                        "PlainText", BindingFlags.GetField, Nothing, Nothing, Nothing)
            _target.GetType.InvokeMember("EncryptionAlgorithm", BindingFlags.SetProperty, Nothing, _target, New Object() {encryptType})

            _growlApp = _connector.CreateInstance(
                "Growl.Connector.Application", False, BindingFlags.Default, Nothing, New Object() {"Tween"}, Nothing, Nothing)

            'If File.Exists(Path.Combine(Application.StartupPath, "Icons\Tween.png")) Then
            '    _growlApp.GetType().InvokeMember("Icon", BindingFlags.SetProperty, Nothing, Nothing, New Object() {New Icon(Path.Combine(Application.StartupPath, "Icons\Tween.ico"))})
            'End If
            Dim mi As MethodInfo = _target.GetType.GetMethod("Register", New Type() {_growlApp.GetType, _connector.GetType("Growl.Connector.NotificationType[]")})

            _t = _connector.GetType("Growl.Connector.NotificationType")

            Dim arglist As New ArrayList
            arglist.Add(_growlNTreply)
            arglist.Add(_growlNTdm)
            arglist.Add(_growlNTnew)
            arglist.Add(_growlNTusevent)

            mi.Invoke(_target, New Object() {_growlApp, arglist.ToArray(_t)})

            _initialized = True

        Catch ex As Exception
            _initialized = False
            Return False
        End Try

    End Function

    Public Sub Notify(ByVal notificationType As NotifyType, ByVal id As String, ByVal title As String, ByVal text As String)
        If Not _initialized Then Return
        Dim notificationName As String = ""
        Select Case notificationType
            Case NotifyType.Reply
                notificationName = "REPLY"
            Case NotifyType.DirectMessage
                notificationName = "DIRECT_MESSAGE"
            Case NotifyType.Notify
                notificationName = "NOTIFY"
            Case NotifyType.UserStreamEvent
                notificationName = "USERSTREAM_EVENT"
        End Select

        Dim n As Object =
                _connector.GetType("Growl.Connector.Notification").InvokeMember(
                    "Notification",
                    BindingFlags.CreateInstance,
                    Nothing,
                    _connector,
                    New Object() {_appName,
                                  notificationName,
                                  id,
                                  title,
                                  text})
        _target.GetType.InvokeMember("Notify", BindingFlags.InvokeMethod, Nothing, _target, New Object() {n})
    End Sub
End Class
