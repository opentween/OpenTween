' Tween - Client of Twitter
' Copyright (c) 2007-2009 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2009 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2009 takeshik (@takeshik) <http://www.takeshik.org/>
' All rights reserved.
' 
' This file is part of Tween.
' 
' This program is free software; you can redistribute it and/or modify it
' under the terms of the GNU General Public License as published by the Free
' Software Foundation; either version 3 of the License, or (at your option)
' any later version.
' 
' This program is distributed in the hope that it will be useful, but
' WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
' or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
' for more details. 
' 
' You should have received a copy of the GNU General Public License along
' with this program. If not, see <http://www.gnu.org/licenses/>, or write to
' the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
' Boston, MA 02110-1301, USA.

Imports System.Xml
Imports System.Xml.Serialization
Imports System.Xml.Schema
Imports System.IO

<XmlRoot(ElementName:="configuration", Namespace:="urn:XSpect.Configuration.XmlConfiguration")> _
Public Class XmlConfiguration
    Implements IDictionary(Of String, Object), _
               IXmlSerializable

    Private ReadOnly _dictionary As Dictionary(Of String, KeyValuePair(Of Type, Object))
    Protected Shared ReadOnly _lockObj As New Object

    Protected _configurationFile As FileInfo

    Private ReadOnly Property _dictionaryCollection() As ICollection(Of KeyValuePair(Of String, KeyValuePair(Of Type, Object)))
        Get
            Return Me._dictionary
        End Get
    End Property

    Public Property ConfigurationFile As FileInfo
        Get
            Return Me._configurationFile
        End Get
        Set(ByVal value As FileInfo)
            Me._configurationFile = value
        End Set
    End Property

#Region "IDictionary<string,object> メンバ"

    Public Sub Add(ByVal key As String, ByVal value As Object) _
        Implements IDictionary(Of String, Object).Add

        Me._dictionary.Add(key, Me.GetInternalValue(value))
    End Sub

    Public Function ContainsKey(ByVal key As String) As Boolean _
        Implements IDictionary(Of String, Object).ContainsKey

        Return Me._dictionary.ContainsKey(key)
    End Function

    Public ReadOnly Property Keys() As ICollection(Of String) _
        Implements IDictionary(Of String, Object).Keys

        Get
            Return Me._dictionary.Keys
        End Get
    End Property

    Public Function Remove(ByVal key As String) As Boolean _
        Implements IDictionary(Of String, Object).Remove

        Return Me._dictionary.Remove(key)
    End Function

    Public Function TryGetValue(ByVal key As String, ByRef value As Object) As Boolean _
        Implements IDictionary(Of String, Object).TryGetValue

        Return Me.TryGetValue(Of Object)(key, value)
    End Function

    Public ReadOnly Property Values() As ICollection(Of Object) _
        Implements IDictionary(Of String, Object).Values

        Get
            Dim list As List(Of Object) = New List(Of Object)(Me._dictionary.Values.Count)

            For Each p As KeyValuePair(Of Type, Object) In Me._dictionary.Values
                list.Add(p.Value)
            Next
            Return list
        End Get
    End Property

    Default Public Property Item(ByVal key As String) As Object _
        Implements IDictionary(Of String, Object).Item

        Get
            Return Me.GetValue(key)
        End Get
        Set(ByVal value As Object)
            Me.SetValue(key, value)
        End Set
    End Property

#End Region

#Region "ICollection<KeyValuePair<string,object>> メンバ"

    Public Sub Add(ByVal item As KeyValuePair(Of String, Object)) _
        Implements ICollection(Of KeyValuePair(Of String, Object)).Add

        Me._dictionaryCollection.Add(Me.GetInternalValue(item))
    End Sub

    Public Sub Clear() _
        Implements ICollection(Of KeyValuePair(Of String, Object)).Clear

        Me._dictionary.Clear()
    End Sub

    Public Function Contains(ByVal item As KeyValuePair(Of String, Object)) As Boolean _
        Implements ICollection(Of KeyValuePair(Of String, Object)).Contains

        Return Me._dictionaryCollection.Contains(Me.GetInternalValue(item))
    End Function

    Public Sub CopyTo(ByVal array As KeyValuePair(Of String, Object)(), ByVal arrayIndex As Integer) _
        Implements ICollection(Of KeyValuePair(Of String, Object)).CopyTo

        Dim list As List(Of KeyValuePair(Of String, KeyValuePair(Of Type, Object))) _
            = New List(Of KeyValuePair(Of String, KeyValuePair(Of Type, Object)))(array.Length)

        For Each p As KeyValuePair(Of String, Object) In array
            list.Add(Me.GetInternalValue(p))
        Next
        Me._dictionaryCollection.CopyTo(list.ToArray(), arrayIndex)
    End Sub

    Public ReadOnly Property Count() As Integer _
        Implements ICollection(Of KeyValuePair(Of String, Object)).Count
        Get
            Return Me._dictionary.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly() As Boolean _
        Implements ICollection(Of KeyValuePair(Of String, Object)).IsReadOnly

        Get
            Return Me._dictionaryCollection.IsReadOnly
        End Get
    End Property

    Public Function Remove(ByVal item As KeyValuePair(Of String, Object)) As Boolean _
        Implements ICollection(Of KeyValuePair(Of String, Object)).Remove

        Return Me._dictionaryCollection.Remove(Me.GetInternalValue(item))
    End Function

#End Region

#Region "IEnumerable<KeyValuePair<string,object>> メンバ"

    Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, Object)) _
        Implements IEnumerable(Of KeyValuePair(Of String, Object)).GetEnumerator

        Dim list As List(Of KeyValuePair(Of String, Object)) = New List(Of KeyValuePair(Of String, Object))(Me.Count)
        For Each p As KeyValuePair(Of String, KeyValuePair(Of Type, Object)) In Me._dictionary
            list.Add(New KeyValuePair(Of String, Object)(p.Key, p.Value.Value))
        Next
        Return list.GetEnumerator()
    End Function

#End Region

#Region "IEnumerable メンバ"

    Private Function GetUntypedEnumerator() As IEnumerator _
        Implements IEnumerable.GetEnumerator

        Return Me.GetEnumerator()
    End Function

#End Region

#Region "IXmlSerializable メンバ"

    Public Function GetSchema() As XmlSchema _
        Implements IXmlSerializable.GetSchema

        Return Nothing
    End Function

    Public Sub ReadXml(ByVal reader As XmlReader) _
        Implements IXmlSerializable.ReadXml

        Dim xdoc As XmlDocument = New XmlDocument()
        xdoc.LoadXml(reader.ReadOuterXml())
        For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("./entry")
            Dim xentry As XmlElement = CType(xentryNode, XmlElement)
            Me.Add( _
                xentry.Attributes.ItemOf("key").Value, _
                New XmlSerializer(Type.GetType(xentry.Attributes.ItemOf("type").Value, True)) _
                    .Deserialize(New XmlNodeReader(xentry.GetElementsByTagName("*").Item(0))) _
            )
        Next
    End Sub

    Public Sub WriteXml(ByVal writer As XmlWriter) _
        Implements IXmlSerializable.WriteXml

        Dim xdoc As XmlDocument = New XmlDocument()
        For Each entry As KeyValuePair(Of String, Object) In Me
            Dim xentry As XmlElement = xdoc.CreateElement("entry", String.Empty)
            xentry.SetAttributeNode("key", Nothing)
            xentry.SetAttribute("key", entry.Key)
            xentry.SetAttributeNode("type", Nothing)
            xentry.SetAttribute("type", entry.Value.GetType().AssemblyQualifiedName)
            Using stream As MemoryStream = New MemoryStream()
                Dim serializer As XmlSerializer = New XmlSerializer(entry.Value.GetType())
                serializer.Serialize(stream, entry.Value)
                stream.Seek(0, SeekOrigin.Begin)
                Dim reader As XmlReader = XmlReader.Create(stream)
                ' HACK: ルート要素に移動する
                reader.MoveToContent()
                xentry.AppendChild(xdoc.ReadNode(reader.ReadSubtree()))
                xentry.WriteTo(writer)
            End Using
        Next
    End Sub

#End Region

    Public Sub New()
        Me._dictionary = New Dictionary(Of String, KeyValuePair(Of Type, Object))()
    End Sub

    Public Shared Function Load(ByVal file As FileInfo) As XmlConfiguration
        SyncLock _lockObj
            Using reader As XmlReader = XmlReader.Create(file.FullName)
                Dim config As XmlConfiguration = DirectCast(New XmlSerializer(GetType(XmlConfiguration)).Deserialize(reader), XmlConfiguration)
                config.ConfigurationFile = file
                Return config
            End Using
        End SyncLock
    End Function

    Public Shared Function Load(ByVal path As String) As XmlConfiguration
        Return Load(New FileInfo(path))
    End Function

    Public Sub Save(ByVal file As FileInfo)
        SyncLock _lockObj
            Using stream As MemoryStream = New MemoryStream()
                Using writer As XmlWriter = XmlWriter.Create(stream)
                    Dim serializer As XmlSerializer = New XmlSerializer(GetType(XmlConfiguration))
                    serializer.Serialize(writer, Me)
                End Using
                stream.Seek(0, SeekOrigin.Begin)
                Dim xdoc As XmlDocument = New XmlDocument()
                xdoc.Load(stream)
                xdoc.Save(file.FullName)
            End Using
            Me.ConfigurationFile = file
        End SyncLock
    End Sub

    Public Sub Save(ByVal path As String)
        Me.Save(New FileInfo(path))
    End Sub

    Public Sub Save()
        Me.Save(Me.ConfigurationFile)
    End Sub

    Private Function GetInternalValue(ByVal value As Object) As KeyValuePair(Of Type, Object)
        Return New KeyValuePair(Of Type, Object)(value.GetType, value)
    End Function

    Private Function GetInternalValue(ByVal item As KeyValuePair(Of String, Object)) As KeyValuePair(Of String, KeyValuePair(Of Type, Object))
        Return New KeyValuePair(Of String, KeyValuePair(Of Type, Object))(item.Key, Me.GetInternalValue(item.Value))
    End Function

    Public Function GetValue(Of T)(ByVal key As String) As T
        Return DirectCast(Me._dictionary.Item(key).Value, T)
    End Function

    Public Function GetValue(ByVal key As String) As Object
        Return Me.GetValue(Of Object)(key)
    End Function

    Public Sub SetValue(Of T)(ByVal key As String, ByVal value As T)
        Me._dictionary.Item(key) = Me.GetInternalValue(value)
    End Sub

    Public Sub SetValue(ByVal key As String, ByVal value As Object)
        Me.SetValue(Of Object)(key, value)
    End Sub

    Public Function TryGetValue(Of T)(ByVal key As String, ByRef value As T) As Boolean
        Dim outValue As KeyValuePair(Of Type, Object) = Nothing
        Dim result As Boolean = Me._dictionary.TryGetValue(key, outValue)
        If result Then value = DirectCast(outValue.Value, T)
        Return result
    End Function

    Public Function GetValueOrDefault(Of T)(ByVal key As String, ByVal defaultValue As T) As T
        Dim value As T
        If Me.TryGetValue(Of T)(key, value) Then
            Return value
        End If
        Me.Add(key, defaultValue)
        Return defaultValue
    End Function

    Public Function GetValueOrDefault(Of T)(ByVal key As String) As T
        Return Me.GetValueOrDefault(Of T)(key, CType(Nothing, T))
    End Function

    Public Function GetValueOrDefault(ByVal key As String, ByVal defaultValue As Object) As Object
        Return Me.GetValueOrDefault(Of Object)(key, defaultValue)
    End Function

    Public Function GetValueOrDefault(ByVal key As String) As Object
        Return Me.GetValueOrDefault(Of Object)(key)
    End Function

    Public Function GetChild(ByVal key As String) As XmlConfiguration
        Return Me.GetValueOrDefault(key, New XmlConfiguration())
    End Function
End Class

<XmlRoot(ElementName:="configuration", Namespace:="urn:XSpect.Configuration.XmlConfiguration")> _
Public NotInheritable Class SettingToConfig
    Inherits XmlConfiguration

    Public Sub New()
        If ConfigurationFile Is Nothing Then
            ConfigurationFile = New FileInfo(Path.Combine(My.Application.Info.DirectoryPath, "TweenConf.xml"))
        End If
    End Sub

    Public Shared Shadows Function Load() As SettingToConfig
        SyncLock _lockObj
            Dim fileConf As FileInfo = New FileInfo(Path.Combine(My.Application.Info.DirectoryPath, "TweenConf.xml"))
            If Not fileConf.Exists Then Return Nothing
            Using reader As XmlReader = XmlReader.Create(fileConf.FullName)
                Dim config As SettingToConfig = DirectCast(New XmlSerializer(GetType(SettingToConfig)).Deserialize(reader), SettingToConfig)
                config.ConfigurationFile = fileConf
                Return config
            End Using
        End SyncLock
    End Function

    Public Shadows Sub Save()
        Dim cnt As Integer = 0
        Do
            Try
                MyBase.Save()
                Exit Do
            Catch ex As IOException
                If cnt = 1 Then Throw ex
                Threading.Thread.Sleep(500)
                cnt += 1
            End Try
        Loop While cnt < 2
    End Sub

    Public Property Tabs() As Dictionary(Of String, TabClass)
        Get
            Dim tconf As List(Of XmlConfiguration) = Nothing
            tconf = GetValueOrDefault("tabs", New List(Of XmlConfiguration))
            If tconf.Count = 0 Then
                Dim tdic As New Dictionary(Of String, TabClass)
                tdic.Add(DEFAULTTAB.RECENT, New TabClass(DEFAULTTAB.RECENT))
                tdic.Add(DEFAULTTAB.REPLY, New TabClass(DEFAULTTAB.REPLY))
                tdic.Add(DEFAULTTAB.DM, New TabClass(DEFAULTTAB.DM))
                tdic.Add(DEFAULTTAB.FAV, New TabClass(DEFAULTTAB.FAV))
                Return tdic
            End If
            Dim tbd As New Dictionary(Of String, TabClass)
            For Each tc As XmlConfiguration In tconf
                Dim name As String = tc.GetValueOrDefault("tabName", "")
                If name = "" Then Exit For
                Dim tb As New TabClass(name)
                tb.Notify = tc.GetValueOrDefault("notify", True)
                tb.SoundFile = tc.GetValueOrDefault("soundFile", "")
                tb.UnreadManage = tc.GetValueOrDefault("unreadManage", True)
                tb.Filters = Filters(tc.GetValueOrDefault("filters", New List(Of XmlConfiguration)))

                tbd.Add(name, tb)
            Next
            Return tbd
        End Get
        Set(ByVal value As Dictionary(Of String, TabClass))
            Dim tl As New List(Of XmlConfiguration)
            For Each tn As String In value.Keys
                Dim tcfg As New XmlConfiguration
                tcfg.Item("tabName") = tn
                tcfg.Item("notify") = value(tn).Notify
                tcfg.Item("soundFile") = value(tn).SoundFile
                tcfg.Item("unreadManage") = value(tn).UnreadManage
                Dim fltrs As List(Of XmlConfiguration) = tcfg.GetValueOrDefault("filters", New List(Of XmlConfiguration))
                Filters(fltrs) = value(tn).Filters
                tcfg.Item("filters") = fltrs
                tl.Add(tcfg)
            Next
            Item("tabs") = tl
        End Set
    End Property

    Private Property Filters(ByVal fltConf As List(Of XmlConfiguration)) As List(Of FiltersClass)
        Get
            If fltConf.Count = 0 Then Return New List(Of FiltersClass)
            Dim flt As New List(Of FiltersClass)
            For Each fc As XmlConfiguration In fltConf
                Dim ft As New FiltersClass
                ft.BodyFilter = fc.GetValueOrDefault("bodyFilter", New List(Of String))
                ft.MoveFrom = fc.GetValueOrDefault("moveFrom", False)
                ft.NameFilter = fc.GetValueOrDefault("nameFilter", "")
                ft.SearchBoth = fc.GetValueOrDefault("searchBoth", True)
                ft.SearchUrl = fc.GetValueOrDefault("searchUrl", False)
                ft.SetMark = fc.GetValueOrDefault("setMark", False)
                ft.UseRegex = fc.GetValueOrDefault("useRegex", False)
                flt.Add(ft)
            Next
            Return flt
        End Get
        Set(ByVal value As List(Of FiltersClass))
            For Each ft As FiltersClass In value
                Dim fc As New XmlConfiguration
                fc.Item("bodyFilter") = ft.BodyFilter
                fc.Item("moveFrom") = ft.MoveFrom
                fc.Item("nameFilter") = ft.NameFilter
                fc.Item("searchBoth") = ft.SearchBoth
                fc.Item("searchUrl") = ft.SearchUrl
                fc.Item("setMark") = ft.SetMark
                fc.Item("useRegex") = ft.UseRegex
                fltConf.Add(fc)
            Next
        End Set
    End Property

    Public Property UserName() As String
        Get
            Return GetValueOrDefault("userName", "")
        End Get
        Set(ByVal value As String)
            Item("userName") = value
        End Set
    End Property

    Public Property Password() As String
        Get
            Dim pwd As String = GetValueOrDefault("password", "")
            If pwd.Length > 0 Then
                Try
                    pwd = DecryptString(pwd)
                Catch ex As Exception
                    pwd = ""
                End Try
            End If
            Return pwd
        End Get
        Set(ByVal value As String)
            Dim pwd As String = value.Trim()
            If pwd.Length > 0 Then
                Try
                    Item("password") = EncryptString(value)
                Catch ex As Exception
                    Item("password") = ""
                End Try
            Else
                Item("password") = ""
            End If
        End Set
    End Property

    Public Property FormLocation() As Point
        Get
            Return GetValueOrDefault("formPosition", New Point(0, 0))
        End Get
        Set(ByVal value As Point)
            Item("formPosition") = value
        End Set
    End Property

    Public Property SplitterDistance() As Integer
        Get
            Return GetValueOrDefault("splitterDistance", 320)
        End Get
        Set(ByVal value As Integer)
            Item("splitterDistance") = value
        End Set
    End Property

    Public Property FormSize() As Size
        Get
            Return GetValueOrDefault("formSize", New Size(436, 476))
        End Get
        Set(ByVal value As Size)
            Item("formSize") = value
        End Set
    End Property

    Public Property NextPageThreshold() As Integer
        Get
            Return GetValueOrDefault("nextPageThreshold", 20)
        End Get
        Set(ByVal value As Integer)
            Item("nextPageThreshold") = value
        End Set
    End Property

    Public Property NextPages() As Integer
        Get
            Return GetValueOrDefault("nextPages", 1)
        End Get
        Set(ByVal value As Integer)
            Item("nextPages") = value
        End Set
    End Property

    Public Property TimelinePeriod() As Integer
        Get
            Return GetValueOrDefault("timelinePeriod", 90)
        End Get
        Set(ByVal value As Integer)
            Item("timelinePeriod") = value
        End Set
    End Property

    Public Property ReplyPeriod() As Integer
        Get
            Return GetValueOrDefault("replyPeriod", 600)
        End Get
        Set(ByVal value As Integer)
            Item("replyPeriod") = value
        End Set
    End Property

    Public Property DMPeriod() As Integer
        Get
            Return GetValueOrDefault("dmPeriod", 600)
        End Get
        Set(ByVal value As Integer)
            Item("dmPeriod") = value
        End Set
    End Property

    Public Property ReadPages() As Integer
        Get
            Return GetValueOrDefault("readPages", 1)
        End Get
        Set(ByVal value As Integer)
            Item("readPages") = value
        End Set
    End Property

    Public Property ReadPagesReply() As Integer
        Get
            Return GetValueOrDefault("readPagesReply", 1)
        End Get
        Set(ByVal value As Integer)
            Item("readPagesReply") = value
        End Set
    End Property

    Public Property ReadPagesDM() As Integer
        Get
            Return GetValueOrDefault("readPagesDm", 1)
        End Get
        Set(ByVal value As Integer)
            Item("readPagesDm") = value
        End Set
    End Property

    Public Property MaxPostNum() As Integer
        Get
            Return GetValueOrDefault("maxPostNum", 125)
        End Get
        Set(ByVal value As Integer)
            Item("maxPostNum") = value
        End Set
    End Property

    Public Property Read() As Boolean
        Get
            Return GetValueOrDefault("startupRead", True)
        End Get
        Set(ByVal value As Boolean)
            Item("startupRead") = value
        End Set
    End Property

    Public Property ListLock() As Boolean
        Get
            Return GetValueOrDefault("listLock", False)
        End Get
        Set(ByVal value As Boolean)
            Item("listLock") = value
        End Set
    End Property

    Public Property IconSize() As IconSizes
        Get
            Return GetValueOrDefault("listIconSize", IconSizes.Icon16)
        End Get
        Set(ByVal value As IconSizes)
            Item("listIconSize") = value
        End Set
    End Property

    Public Property NewAllPop() As Boolean
        Get
            Return GetValueOrDefault("newAllPop", True)
        End Get
        Set(ByVal value As Boolean)
            Item("newAllPop") = value
        End Set
    End Property

    Public Property StatusText() As String
        Get
            Return GetValueOrDefault("statusText", "")
        End Get
        Set(ByVal value As String)
            Item("statusText") = value
        End Set
    End Property

    Public Property PlaySound() As Boolean
        Get
            Return GetValueOrDefault("playSound", False)
        End Get
        Set(ByVal value As Boolean)
            Item("playSound") = value
        End Set
    End Property

    Public Property UnreadManage() As Boolean
        Get
            Return GetValueOrDefault("unreadManage", True)
        End Get
        Set(ByVal value As Boolean)
            Item("unreadManage") = value
        End Set
    End Property

    Public Property OneWayLove() As Boolean
        Get
            Return GetValueOrDefault("oneWayLove", True)
        End Get
        Set(ByVal value As Boolean)
            Item("oneWayLove") = value
        End Set
    End Property

    Public Property FontUnread() As System.Drawing.Font
        Get
            Dim fc As New FontConverter
            Dim f2str As String = fc.ConvertToString(New Font(System.Drawing.SystemFonts.DefaultFont, FontStyle.Bold Or FontStyle.Underline))
            Return DirectCast(fc.ConvertFromString(GetValueOrDefault("fontUnread", f2str)), Font)
        End Get
        Set(ByVal value As Font)
            Dim fc As New FontConverter
            Item("fontUnread") = fc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorUnread() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(System.Drawing.SystemColors.ControlText)
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorUnread", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorUnread") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property FontRead() As Font
        Get
            Dim fc As New FontConverter
            Dim f2str As String = fc.ConvertToString(System.Drawing.SystemFonts.DefaultFont)
            Return DirectCast(fc.ConvertFromString(GetValueOrDefault("fontRead", f2str)), Font)
        End Get
        Set(ByVal value As Font)
            Dim fc As New FontConverter
            Item("fontRead") = fc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorRead() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(Color.FromKnownColor(System.Drawing.KnownColor.Gray))
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorRead", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorRead") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorFav() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(Color.FromKnownColor(KnownColor.Red))
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorFav", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorFav") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorOWL() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(Color.FromKnownColor(KnownColor.Blue))
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorOwl", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorOwl") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property FontDetail() As Font
        Get
            Dim fc As New FontConverter
            Dim f2str As String = fc.ConvertToString(System.Drawing.SystemFonts.DefaultFont)
            Return DirectCast(fc.ConvertFromString(GetValueOrDefault("fontDetail", f2str)), Font)
        End Get
        Set(ByVal value As Font)
            Dim fc As New FontConverter
            Item("fontDetail") = fc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorSelf() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(Color.FromKnownColor(KnownColor.AliceBlue))
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorSelf", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorSelf") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorAtSelf() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(Color.FromKnownColor(KnownColor.AntiqueWhite))
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorAtSelf", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorAtSelf") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorTarget() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(Color.FromKnownColor(KnownColor.LemonChiffon))
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorTarget", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorTarget") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorAtTarget() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(Color.FromKnownColor(KnownColor.LavenderBlush))
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorAtTarget", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorAtTarget") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorAtFromTarget() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(Color.FromKnownColor(KnownColor.Honeydew))
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorAtFromTarget", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorAtFromTarget") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorInputBackcolor() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(Color.LemonChiffon)
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorInputBackcolor", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorInputBackcolor") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property ColorInputFont() As Color
        Get
            Dim cc As New ColorConverter
            Dim c2str As String = cc.ConvertToString(Color.FromKnownColor(KnownColor.ControlText))
            Return DirectCast(cc.ConvertFromString(GetValueOrDefault("colorInputFont", c2str)), Color)
        End Get
        Set(ByVal value As Color)
            Dim cc As New ColorConverter
            Item("colorInputFont") = cc.ConvertToString(value)
        End Set
    End Property

    Public Property FontInputFont() As Font
        Get
            Dim fc As New FontConverter
            Dim f2str As String = fc.ConvertToString(System.Drawing.SystemFonts.DefaultFont)
            Return DirectCast(fc.ConvertFromString(GetValueOrDefault("fontInputFont", f2str)), Font)
        End Get
        Set(ByVal value As Font)
            Dim fc As New FontConverter
            Item("fontInputFont") = fc.ConvertToString(value)
        End Set
    End Property

    Public Property NameBalloon() As NameBalloonEnum
        Get
            Return GetValueOrDefault("nameBalloon", NameBalloonEnum.NickName)
        End Get
        Set(ByVal value As NameBalloonEnum)
            Item("nameBalloon") = value
        End Set
    End Property

    Public Property Width1() As Integer
        Get
            Return GetValueOrDefault("width1", 48)
        End Get
        Set(ByVal value As Integer)
            Item("width1") = value
        End Set
    End Property

    Public Property Width2() As Integer
        Get
            Return GetValueOrDefault("width2", 80)
        End Get
        Set(ByVal value As Integer)
            Item("width2") = value
        End Set
    End Property

    Public Property Width3() As Integer
        Get
            Return GetValueOrDefault("width3", 290)
        End Get
        Set(ByVal value As Integer)
            Item("width3") = value
        End Set
    End Property

    Public Property Width4() As Integer
        Get
            Return GetValueOrDefault("width4", 120)
        End Get
        Set(ByVal value As Integer)
            Item("width4") = value
        End Set
    End Property

    Public Property Width5() As Integer
        Get
            Return GetValueOrDefault("width5", 50)
        End Get
        Set(ByVal value As Integer)
            Item("width5") = value
        End Set
    End Property

    Public Property Width6() As Integer
        Get
            Return GetValueOrDefault("width6", 16)
        End Get
        Set(ByVal value As Integer)
            Item("width6") = value
        End Set
    End Property

    Public Property Width7() As Integer
        Get
            Return GetValueOrDefault("width7", 32)
        End Get
        Set(ByVal value As Integer)
            Item("width7") = value
        End Set
    End Property

    Public Property Width8() As Integer
        Get
            Return GetValueOrDefault("width8", 50)
        End Get
        Set(ByVal value As Integer)
            Item("width8") = value
        End Set
    End Property

    Public Property SortColumn() As Integer
        Get
            Return GetValueOrDefault("sortColumn", 3)
        End Get
        Set(ByVal value As Integer)
            Item("sortColumn") = value
        End Set
    End Property

    Public Property SortOrder() As Integer
        Get
            Return GetValueOrDefault("sortOrder", 1)
        End Get
        Set(ByVal value As Integer)
            Item("sortOrder") = value
        End Set
    End Property

    Public Property DisplayIndex1() As Integer
        Get
            Return GetValueOrDefault("displayIndex1", 0)
        End Get
        Set(ByVal value As Integer)
            Item("displayIndex1") = value
        End Set
    End Property

    Public Property DisplayIndex2() As Integer
        Get
            Return GetValueOrDefault("displayIndex2", 1)
        End Get
        Set(ByVal value As Integer)
            Item("displayIndex2") = value
        End Set
    End Property

    Public Property DisplayIndex3() As Integer
        Get
            Return GetValueOrDefault("displayIndex3", 2)
        End Get
        Set(ByVal value As Integer)
            Item("displayIndex3") = value
        End Set
    End Property

    Public Property DisplayIndex4() As Integer
        Get
            Return GetValueOrDefault("displayIndex4", 3)
        End Get
        Set(ByVal value As Integer)
            Item("displayIndex4") = value
        End Set
    End Property

    Public Property DisplayIndex5() As Integer
        Get
            Return GetValueOrDefault("displayIndex5", 4)
        End Get
        Set(ByVal value As Integer)
            Item("displayIndex5") = value
        End Set
    End Property

    Public Property DisplayIndex6() As Integer
        Get
            Return GetValueOrDefault("displayIndex6", 5)
        End Get
        Set(ByVal value As Integer)
            Item("displayIndex6") = value
        End Set
    End Property

    Public Property DisplayIndex7() As Integer
        Get
            Return GetValueOrDefault("displayIndex7", 6)
        End Get
        Set(ByVal value As Integer)
            Item("displayIndex7") = value
        End Set
    End Property

    Public Property DisplayIndex8() As Integer
        Get
            Return GetValueOrDefault("displayIndex8", 7)
        End Get
        Set(ByVal value As Integer)
            Item("displayIndex8") = value
        End Set
    End Property

    Public Property PostCtrlEnter() As Boolean
        Get
            Return GetValueOrDefault("postCtrlEnter", False)
        End Get
        Set(ByVal value As Boolean)
            Item("postCtrlEnter") = value
        End Set
    End Property

    Public Property UseAPI() As Boolean
        Get
            Return GetValueOrDefault("useApi", False)
        End Get
        Set(ByVal value As Boolean)
            Item("useApi") = value
        End Set
    End Property

    Public Property UsePostMethod() As Boolean
        Get
            'Return GetValueOrDefault("usePostMethod", False)
            Return False
        End Get
        Set(ByVal value As Boolean)
            Item("usePostMethod") = False
        End Set
    End Property

    Public Property CountApi() As Integer
        Get
            Return GetValueOrDefault("countApi", 60)
        End Get
        Set(ByVal value As Integer)
            Item("countApi") = value
        End Set
    End Property

    Public Property CheckReply() As Boolean
        Get
            Return GetValueOrDefault("checkReply", True)
        End Get
        Set(ByVal value As Boolean)
            Item("checkReply") = value
        End Set
    End Property

    Public Property PostAndGet() As Boolean
        Get
            Return GetValueOrDefault("postAndGet", True)
        End Get
        Set(ByVal value As Boolean)
            Item("postAndGet") = value
        End Set
    End Property

    Public Property UseRecommendStatus() As Boolean
        Get
            Return GetValueOrDefault("useRecommendStatus", False)
        End Get
        Set(ByVal value As Boolean)
            Item("useRecommendStatus") = value
        End Set
    End Property

    Public Property DispUsername() As Boolean
        Get
            Return GetValueOrDefault("dispUsername", False)
        End Get
        Set(ByVal value As Boolean)
            Item("dispUsername") = value
        End Set
    End Property

    Public Property MinimizeToTray() As Boolean
        Get
            Return GetValueOrDefault("minimizeToTray", False)
        End Get
        Set(ByVal value As Boolean)
            Item("minimizeToTray") = value
        End Set
    End Property

    Public Property CloseToExit() As Boolean
        Get
            Return GetValueOrDefault("closeToExit", False)
        End Get
        Set(ByVal value As Boolean)
            Item("closeToExit") = value
        End Set
    End Property

    Public Property DispLatestPost() As DispTitleEnum
        Get
            Return GetValueOrDefault("dispLatestPost", DispTitleEnum.Post)
        End Get
        Set(ByVal value As DispTitleEnum)
            Item("dispLatestPost") = value
        End Set
    End Property

    Public Property HubServer() As String
        Get
            Return GetValueOrDefault("hubServer", "twitter.com")
        End Get
        Set(ByVal value As String)
            Item("hubServer") = value
        End Set
    End Property

    Public Property BrowserPath() As String
        Get
            Return GetValueOrDefault("browserPath", "")
        End Get
        Set(ByVal value As String)
            Item("browserPath") = value
        End Set
    End Property

    Public Property SortOrderLock() As Boolean
        Get
            Return GetValueOrDefault("sortOrderLock", False)
        End Get
        Set(ByVal value As Boolean)
            Item("sortOrderLock") = value
        End Set
    End Property

    Public Property TinyURLResolve() As Boolean
        Get
            Return GetValueOrDefault("tinyurlResolve", True)
        End Get
        Set(ByVal value As Boolean)
            Item("tinyurlResolve") = value
        End Set
    End Property

    Public Property ProxyType() As ProxyTypeEnum
        Get
            Return GetValueOrDefault("proxyType", ProxyTypeEnum.IE)
        End Get
        Set(ByVal value As ProxyTypeEnum)
            Item("proxyType") = value
        End Set
    End Property

    Public Property ProxyAddress() As String
        Get
            Return GetValueOrDefault("proxyAddress", "127.0.0.1")
        End Get
        Set(ByVal value As String)
            Item("proxyAddress") = value
        End Set
    End Property

    Public Property ProxyPort() As Integer
        Get
            Return GetValueOrDefault("proxyPort", 80)
        End Get
        Set(ByVal value As Integer)
            Item("proxyPort") = value
        End Set
    End Property

    Public Property ProxyUser() As String
        Get
            Return GetValueOrDefault("proxyUser", "")
        End Get
        Set(ByVal value As String)
            Item("proxyUser") = value
        End Set
    End Property

    Public Property ProxyPassword() As String
        Get
            Dim pwd As String = GetValueOrDefault("proxyPassword", "")
            If pwd.Length > 0 Then
                Try
                    pwd = DecryptString(pwd)
                Catch ex As Exception
                    pwd = ""
                End Try
            End If
            Return pwd
        End Get
        Set(ByVal value As String)
            Dim pwd As String = value.Trim()
            If pwd.Length > 0 Then
                Try
                    Item("proxyPassword") = EncryptString(pwd)
                Catch ex As Exception
                    Item("proxyPassword") = ""
                End Try
            Else
                Item("proxyPassword") = ""
            End If
        End Set
    End Property

    Public Property PeriodAdjust() As Boolean
        Get
            Return GetValueOrDefault("periodAdjust", True)
        End Get
        Set(ByVal value As Boolean)
            Item("periodAdjust") = value
        End Set
    End Property

    Public Property StartupVersion() As Boolean
        Get
            Return GetValueOrDefault("startupVersion", True)
        End Get
        Set(ByVal value As Boolean)
            Item("startupVersion") = value
        End Set
    End Property

    Public Property StartupKey() As Boolean
        Get
            Return GetValueOrDefault("startupKey", True)
        End Get
        Set(ByVal value As Boolean)
            Item("startupKey") = value
        End Set
    End Property

    Public Property StartupFollowers() As Boolean
        Get
            Return GetValueOrDefault("startupFollowers", True)
        End Get
        Set(ByVal value As Boolean)
            Item("startupFollowers") = value
        End Set
    End Property

    Public Property StartupAPImodeNoWarning() As Boolean
        Get
            Return GetValueOrDefault("startupAPImodeNoWarning", False)
        End Get
        Set(ByVal value As Boolean)
            Item("startupAPImodeNoWarning") = value
        End Set
    End Property

    Public Property RestrictFavCheck() As Boolean
        Get
            Return GetValueOrDefault("restrictFavCheck", False)
        End Get
        Set(ByVal value As Boolean)
            Item("restrictFavCheck") = value
        End Set
    End Property

    Public Property AlwaysTop() As Boolean
        Get
            Return GetValueOrDefault("alwaysTop", False)
        End Get
        Set(ByVal value As Boolean)
            Item("alwaysTop") = value
        End Set
    End Property

    Public Property StatusMultiline() As Boolean
        Get
            Return GetValueOrDefault("statusMultiline", False)
        End Get
        Set(ByVal value As Boolean)
            Item("statusMultiline") = value
        End Set
    End Property

    Public Property StatusTextHeight() As Integer
        Get
            Return GetValueOrDefault("statusTextHeight", 38)
        End Get
        Set(ByVal value As Integer)
            Item("statusTextHeight") = value
        End Set
    End Property

    Public Property cultureCode() As String
        Get
            Return GetValueOrDefault("cultureCode", "")
        End Get
        Set(ByVal value As String)
            Item("cultureCode") = value
        End Set
    End Property

    Public Property UrlConvertAuto() As Boolean
        Get
            Return GetValueOrDefault("urlConvertAuto", False)
        End Get
        Set(ByVal value As Boolean)
            Item("urlConvertAuto") = value
        End Set
    End Property

    Public Property Outputz() As Boolean
        Get
            Return GetValueOrDefault("outputz", False)
        End Get
        Set(ByVal value As Boolean)
            Item("outputz") = value
        End Set
    End Property

    Public Property OutputzKey() As String
        Get
            Dim key As String = GetValueOrDefault("outputzKey", "")
            If key.Length > 0 Then
                Try
                    key = DecryptString(key)
                Catch ex As Exception
                    key = ""
                End Try
            End If
            Return key
        End Get
        Set(ByVal value As String)
            Dim key As String = value.Trim()
            If key.Length > 0 Then
                Try
                    Item("outputzKey") = EncryptString(key)
                Catch ex As Exception
                    Item("outputzKey") = ""
                End Try
            Else
                Item("outputzKey") = ""
            End If
        End Set
    End Property

    Public Property AutoShortUrlFirst() As UrlConverter
        Get
            Return GetValueOrDefault("AutoShortUrlFirst", UrlConverter.Bitly)
        End Get
        Set(ByVal value As UrlConverter)
            Item("AutoShortUrlFirst") = value
        End Set
    End Property

    Public Property OutputzUrlmode() As OutputzUrlmode
        Get
            Return GetValueOrDefault("outputzUrlMode", OutputzUrlmode.twittercom)
        End Get
        Set(ByVal value As OutputzUrlmode)
            Item("outputzUrlMode") = value
        End Set
    End Property
    Public Property UseUnreadStyle() As Boolean
        Get
            Return GetValueOrDefault("useUnreadStyle", True)
        End Get
        Set(ByVal value As Boolean)
            Item("useUnreadStyle") = value
        End Set
    End Property

    Public Property DateTimeFormat() As String
        Get
            Return GetValueOrDefault("datetimeFormat", "yyyy/MM/dd H:mm:ss")
        End Get
        Set(ByVal value As String)
            Item("datetimeFormat") = value
        End Set
    End Property

    Public Property DefaultTimeOut() As Integer
        Get
            Return GetValueOrDefault("defaultTimeout", 20)
        End Get
        Set(ByVal value As Integer)
            Item("defaultTimeout") = value
        End Set
    End Property

    Public Property ProtectNotInclude() As Boolean
        Get
            Return GetValueOrDefault("protectNotInclude", True)
        End Get
        Set(ByVal value As Boolean)
            Item("protectNotInclude") = value
        End Set
    End Property

    Public Property LimitBalloon() As Boolean
        Get
            Return GetValueOrDefault("limitBalloon", False)
        End Get
        Set(ByVal value As Boolean)
            Item("limitBalloon") = value
        End Set
    End Property
End Class
