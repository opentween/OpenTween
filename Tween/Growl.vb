Imports System.Reflection
Imports System.IO
Imports System.ComponentModel
Imports System.Drawing.Imaging

Public Class GrowlHelper

    Private _connector As Assembly = Nothing
    Private _core As Assembly = Nothing

    Private _growlNTreply As Object
    Private _growlNTdm As Object
    Private _growlNTnew As Object
    Private _growlNTusevent As Object
    Private _growlApp As Object

    Private _targetConnector As Object
    Private _targetCore As Object

    Private _appName As String = ""
    Dim _initialized As Boolean = False

    Public Class NotifyCallbackEventArgs
        Inherits EventArgs
        Public Property StatusId As Long
        Public Property NotifyType As NotifyType
        Public Sub New(ByVal notifyType As NotifyType, ByVal statusId As String)
            If statusId.Length > 1 Then
                Me.StatusId = CLng(statusId)
                Me.NotifyType = notifyType
            End If
        End Sub
    End Class

    Public Event NotifyClicked(ByVal sender As Object, ByVal e As NotifyCallbackEventArgs)

    Public ReadOnly Property AppName As String
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

    Private Overloads Function IconToByteArray(ByVal filename As String) As Byte()
        Return IconToByteArray(New Icon(filename))
    End Function

    Private Overloads Function IconToByteArray(ByVal icondata As Icon) As Byte()
        Using ms As New MemoryStream
            Dim ic As Icon = New Icon(icondata, 48, 48)
            ic.ToBitmap.Save(ms, ImageFormat.Png)
            Return ms.ToArray()
        End Using
    End Function

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
            _targetConnector = _connector.CreateInstance("Growl.Connector.GrowlConnector")
            _targetCore = _core.CreateInstance("Growl.CoreLibrary")
            Dim _t As Type = _connector.GetType("Growl.Connector.NotificationType")

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
            _targetConnector.GetType.InvokeMember("EncryptionAlgorithm", BindingFlags.SetProperty, Nothing, _targetConnector, New Object() {encryptType})

            _growlApp = _connector.CreateInstance(
                "Growl.Connector.Application", False, BindingFlags.Default, Nothing, New Object() {_appName}, Nothing, Nothing)


            If File.Exists(Path.Combine(Application.StartupPath, "Icons\Tween.png")) Then
                ' Icons\Tween.pngを使用
                Dim ci As ConstructorInfo = _core.GetType(
                    "Growl.CoreLibrary.Resource").GetConstructor(
                    BindingFlags.NonPublic Or BindingFlags.Instance,
                    Nothing, New Type() {GetType(System.String)}, Nothing)

                Dim data As Object = ci.Invoke(New Object() {Path.Combine(Application.StartupPath, "Icons\Tween.png")})
                Dim pi As PropertyInfo = _growlApp.GetType.GetProperty("Icon")
                pi.SetValue(_growlApp, data, Nothing)

            ElseIf File.Exists(Path.Combine(Application.StartupPath, "Icons\MIcon.ico")) Then
                ' アイコンセットにMIcon.icoが存在する場合それを使用
                Dim cibd As ConstructorInfo = _core.GetType(
                     "Growl.CoreLibrary.BinaryData").GetConstructor(
                     BindingFlags.Public Or BindingFlags.Instance,
                     Nothing, New Type() {GetType(Byte())}, Nothing)
                Dim tc As New TypeConverter
                Dim bdata As Object = cibd.Invoke(
                    New Object() {IconToByteArray(Path.Combine(Application.StartupPath, "Icons\MIcon.ico"))})

                Dim ciRes As ConstructorInfo = _core.GetType(
                    "Growl.CoreLibrary.Resource").GetConstructor(
                    BindingFlags.NonPublic Or BindingFlags.Instance,
                    Nothing, New Type() {bdata.GetType()}, Nothing)

                Dim data As Object = ciRes.Invoke(New Object() {bdata})
                Dim pi As PropertyInfo = _growlApp.GetType.GetProperty("Icon")
                pi.SetValue(_growlApp, data, Nothing)
            Else
                '内蔵アイコンリソースを使用
                Dim cibd As ConstructorInfo = _core.GetType(
                     "Growl.CoreLibrary.BinaryData").GetConstructor(
                     BindingFlags.Public Or BindingFlags.Instance,
                     Nothing, New Type() {GetType(Byte())}, Nothing)
                Dim tc As New TypeConverter
                Dim bdata As Object = cibd.Invoke(
                    New Object() {IconToByteArray(My.Resources.MIcon)})

                Dim ciRes As ConstructorInfo = _core.GetType(
                    "Growl.CoreLibrary.Resource").GetConstructor(
                    BindingFlags.NonPublic Or BindingFlags.Instance,
                    Nothing, New Type() {bdata.GetType()}, Nothing)

                Dim data As Object = ciRes.Invoke(New Object() {bdata})
                Dim pi As PropertyInfo = _growlApp.GetType.GetProperty("Icon")
                pi.SetValue(_growlApp, data, Nothing)
            End If

            Dim mi As MethodInfo = _targetConnector.GetType.GetMethod("Register", New Type() {_growlApp.GetType, _connector.GetType("Growl.Connector.NotificationType[]")})

            _t = _connector.GetType("Growl.Connector.NotificationType")

            Dim arglist As New ArrayList
            arglist.Add(_growlNTreply)
            arglist.Add(_growlNTdm)
            arglist.Add(_growlNTnew)
            arglist.Add(_growlNTusevent)

            mi.Invoke(_targetConnector, New Object() {_growlApp, arglist.ToArray(_t)})

            ' コールバックメソッドの登録
            Dim tGrowlConnector As Type = _connector.GetType("Growl.Connector.GrowlConnector")
            Dim evNotificationCallback As EventInfo = tGrowlConnector.GetEvent("NotificationCallback")
            Dim tDelegate As Type = evNotificationCallback.EventHandlerType
            Dim miHandler As MethodInfo = GetType(GrowlHelper).GetMethod("GrowlCallbackHandler", BindingFlags.NonPublic Or BindingFlags.Instance)
            Dim d As [Delegate] = [Delegate].CreateDelegate(tDelegate, Me, miHandler)
            Dim miAddHandler As MethodInfo = evNotificationCallback.GetAddMethod()
            Dim addHandlerArgs() As Object = {d}
            miAddHandler.Invoke(_targetConnector, addHandlerArgs)

            _initialized = True

        Catch ex As Exception
            _initialized = False
            Return False
        End Try

        Return True
    End Function

    Public Sub Notify(ByVal notificationType As NotifyType, ByVal id As String, ByVal title As String, ByVal text As String, Optional ByVal icon As Image = Nothing, Optional ByVal url As String = "")
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
        Dim n As Object = Nothing
        If icon IsNot Nothing OrElse Not String.IsNullOrEmpty(url) Then
            Dim gCore As Type = _core.GetType("Growl.CoreLibrary.Resource")
            Dim res As Object = Nothing
            If icon IsNot Nothing Then
                res = gCore.InvokeMember("op_Implicit",
                                                   BindingFlags.Public Or BindingFlags.Static Or BindingFlags.InvokeMethod,
                                                   Nothing,
                                                   Nothing,
                                                   New Object() {icon})
            Else
                res = gCore.InvokeMember("op_Implicit",
                                                   BindingFlags.Public Or BindingFlags.Static Or BindingFlags.InvokeMethod,
                                                   Nothing,
                                                   Nothing,
                                                   New Object() {url})
            End If
            Dim priority As Object =
                    _connector.GetType("Growl.Connector.Priority").InvokeMember(
                        "Normal", BindingFlags.GetField, Nothing, Nothing, Nothing)
            n = _connector.GetType("Growl.Connector.Notification").InvokeMember(
                    "Notification",
                    BindingFlags.CreateInstance,
                    Nothing,
                    _connector,
                    New Object() {_appName,
                                  notificationName,
                                  id,
                                  title,
                                  text,
                                  res,
                                  False,
                                  priority,
                                  "aaa"})
        Else
            n = _connector.GetType("Growl.Connector.Notification").InvokeMember(
                    "Notification",
                    BindingFlags.CreateInstance,
                    Nothing,
                    _connector,
                    New Object() {_appName,
                                    notificationName,
                                    id,
                                    title,
                                    text})
        End If
        '_targetConnector.GetType.InvokeMember("Notify", BindingFlags.InvokeMethod, Nothing, _targetConnector, New Object() {n})
        Dim cc As Object = _connector.GetType("Growl.Connector.CallbackContext").InvokeMember(
            Nothing, BindingFlags.CreateInstance, Nothing, _connector,
            New Object() {"some fake information", notificationName})
        _targetConnector.GetType.InvokeMember("Notify", BindingFlags.InvokeMethod, Nothing, _targetConnector, New Object() {n, cc})
    End Sub

    Private Sub GrowlCallbackHandler(ByVal response As Object, ByVal callbackData As Object, ByVal state As Object)
        Try
            ' 定数取得
            Dim vCLICK As Object =
            _core.GetType("Growl.CoreLibrary.CallbackResult").GetField(
                        "CLICK",
                       BindingFlags.Public Or BindingFlags.Static).GetRawConstantValue()
            ' 実際の値
            Dim vResult As Object = callbackData.GetType.GetProperty(
                        "Result",
                        BindingFlags.Public Or BindingFlags.Instance).GetGetMethod.Invoke(callbackData, Nothing)
            vResult = CType(vResult, Integer)
            Dim notifyId As String = CStr(callbackData.GetType.GetProperty("NotificationID").GetGetMethod.Invoke(callbackData, Nothing))
            Dim notifyName As String = CStr(callbackData.GetType.GetProperty("Type").GetGetMethod.Invoke(callbackData, Nothing))
            If vCLICK.Equals(vResult) Then
                Dim nt As NotifyType
                Select Case notifyName
                    Case "REPLY"
                        nt = NotifyType.Reply
                    Case "DIRECT_MESSAGE"
                        nt = NotifyType.DirectMessage
                    Case "NOTIFY"
                        nt = NotifyType.Notify
                    Case "USERSTREAM_EVENT"
                        nt = NotifyType.UserStreamEvent
                End Select
                RaiseEvent NotifyClicked(Me, New NotifyCallbackEventArgs(nt, notifyId))
            End If
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub
End Class
