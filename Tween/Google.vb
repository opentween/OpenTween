' Tween - Client of Twitter
' Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
'           (c) 2008-2011 Moz (@syo68k)
'           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
'           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
'           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
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

Imports System.Collections.Specialized
Imports System.Net
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Web

Public Class Google

#Region "Translation"
    ' http://code.google.com/intl/ja/apis/ajaxlanguage/documentation/#fonje
    ' デベロッパー ガイド - Google AJAX Language API - Google Code

    Private Const TranslateEndPoint As String = "http://ajax.googleapis.com/ajax/services/language/translate"
    Private Const LanguageDetectEndPoint As String = "https://ajax.googleapis.com/ajax/services/language/detect"

#Region "言語テーブル定義"
    Private Shared LanguageTable As New List(Of String) From {
        "af",
        "sq",
        "am",
        "ar",
        "hy",
        "az",
        "eu",
        "be",
        "bn",
        "bh",
        "br",
        "bg",
        "my",
        "ca",
        "chr",
        "zh",
        "zh-CN",
        "zh-TW",
        "co",
        "hr",
        "cs",
        "da",
        "dv",
        "nl",
        "en",
        "eo",
        "et",
        "fo",
        "tl",
        "fi",
        "fr",
        "fy",
        "gl",
        "ka",
        "de",
        "el",
        "gu",
        "ht",
        "iw",
        "hi",
        "hu",
        "is",
        "id",
        "iu",
        "ga",
        "it",
        "ja",
        "jw",
        "kn",
        "kk",
        "km",
        "ko",
        "ku",
        "ky",
        "lo",
        "la",
        "lv",
        "lt",
        "lb",
        "mk",
        "ms",
        "ml",
        "mt",
        "mi",
        "mr",
        "mn",
        "ne",
        "no",
        "oc",
        "or",
        "ps",
        "fa",
        "pl",
        "pt",
        "pt-PT",
        "pa",
        "qu",
        "ro",
        "ru",
        "sa",
        "gd",
        "sr",
        "sd",
        "si",
        "sk",
        "sl",
        "es",
        "su",
        "sw",
        "sv",
        "syr",
        "tg",
        "ta",
        "tt",
        "te",
        "th",
        "bo",
        "to",
        "tr",
        "uk",
        "ur",
        "uz",
        "ug",
        "vi",
        "cy",
        "yi",
        "yo"
    }
#End Region

    <DataContract()> _
    Public Class TranslateResponseData
        <DataMember(Name:="translatedText")> Public TranslatedText As String
    End Class


    <DataContract()> _
    Private Class TranslateResponse
        <DataMember(Name:="responseData")> Public ResponseData As TranslateResponseData
        <DataMember(Name:="responseDetails")> Public ResponseDetails As String
        <DataMember(Name:="responseStatus")> Public ResponseStatus As HttpStatusCode
    End Class


    <DataContract()> _
    Public Class LanguageDetectResponseData
        <DataMember(Name:="language")> Public Language As String
        <DataMember(Name:="isReliable")> Public IsReliable As Boolean
        <DataMember(Name:="confidence")> Public Confidence As Double
    End Class

    <DataContract()> _
    Private Class LanguageDetectResponse
        <DataMember(Name:="responseData")> Public ResponseData As LanguageDetectResponseData
        <DataMember(Name:="responseDetails")> Public ResponseDetails As String
        <DataMember(Name:="responseStatus")> Public ResponseStatus As HttpStatusCode
    End Class

    Public Function Translate(ByVal srclng As String, ByVal dstlng As String, ByVal source As String, ByRef destination As String, ByRef ErrMsg As String) As Boolean
        Dim http As New HttpVarious()
        Dim apiurl As String = TranslateEndPoint
        Dim headers As New Dictionary(Of String, String)
        headers.Add("v", "1.0")

        ErrMsg = ""
        If String.IsNullOrEmpty(srclng) OrElse String.IsNullOrEmpty(dstlng) Then
            Return False
        End If
        headers.Add("User-Agent", GetUserAgentString())
        headers.Add("langpair", srclng + "|" + dstlng)

        headers.Add("q", source)

        Dim content As String = ""
        If http.GetData(apiurl, headers, content) Then
            Dim serializer As New DataContractJsonSerializer(GetType(TranslateResponse))
            Dim res As TranslateResponse

            Try
                res = CreateDataFromJson(Of TranslateResponse)(content)
            Catch ex As Exception
                ErrMsg = "Err:Invalid JSON"
                Return False
            End Try

            If res.ResponseData Is Nothing Then
                ErrMsg = "Err:" + res.ResponseDetails
                Return False
            End If
            Dim _body As String = res.ResponseData.TranslatedText
            Dim buf As String = HttpUtility.UrlDecode(_body)

            destination = String.Copy(buf)
            Return True
        End If
        Return False
    End Function

    Public Function LanguageDetect(ByVal source As String) As String
        Dim http As New HttpVarious()
        Dim apiurl As String = LanguageDetectEndPoint
        Dim headers As New Dictionary(Of String, String)
        headers.Add("User-Agent", GetUserAgentString())
        headers.Add("v", "1.0")
        headers.Add("q", source)
        Dim content As String = ""
        If http.GetData(apiurl, headers, content) Then
            Dim serializer As New DataContractJsonSerializer(GetType(LanguageDetectResponse))
            Try
                Dim res As LanguageDetectResponse = CreateDataFromJson(Of LanguageDetectResponse)(content)
                Return res.ResponseData.Language
            Catch ex As Exception
                Return ""
            End Try
        End If
        Return ""
    End Function

    Public Function GetLanguageEnumFromIndex(ByVal index As Integer) As String
        Return LanguageTable(index)
    End Function

    Public Function GetIndexFromLanguageEnum(ByVal lang As String) As Integer
        Return LanguageTable.IndexOf(lang)
    End Function
#End Region

#Region "UrlShortener"
    ' http://code.google.com/intl/ja/apis/urlshortener/v1/getting_started.html
    ' Google URL Shortener API

    <DataContract()>
    Private Class UrlShortenerParameter
        <DataMember(Name:="longUrl")> Dim LongUrl As String
    End Class

    <DataContract()>
    Private Class UrlShortenerResponse

    End Class

    Public Function Shorten(ByVal source As String) As String
        Dim http As New HttpVarious
        Dim apiurl As String = "https://www.googleapis.com/urlshortener/v1/url"
        Dim headers As New Dictionary(Of String, String)
        headers.Add("User-Agent", GetUserAgentString())
        headers.Add("Content-Type", "application/json")

        http.PostData(apiurl, headers)
        Return ""
    End Function
#End Region

#Region "GoogleMaps"
    Public Overloads Function CreateGoogleStaticMapsUri(ByVal locate As GlobalLocation) As String
        Return CreateGoogleStaticMapsUri(locate.Latitude, locate.Longitude)
    End Function

    Public Overloads Function CreateGoogleStaticMapsUri(ByVal lat As Double, ByVal lng As Double) As String
        Return "http://maps.google.com/maps/api/staticmap?center=" + lat.ToString + "," + lng.ToString + "&size=" + AppendSettingDialog.Instance.FoursquarePreviewWidth.ToString + "x" + AppendSettingDialog.Instance.FoursquarePreviewHeight.ToString + "&zoom=" + AppendSettingDialog.Instance.FoursquarePreviewZoom.ToString + "&markers=" + lat.ToString + "," + lng.ToString + "&sensor=false"
    End Function

    Public Overloads Function CreateGoogleMapsUri(ByVal locate As GlobalLocation) As String
        Return CreateGoogleMapsUri(locate.Latitude, locate.Longitude)
    End Function

    Public Overloads Function CreateGoogleMapsUri(ByVal lat As Double, ByVal lng As Double) As String
        Return "http://maps.google.com/maps?ll=" + lat.ToString + "," + lng.ToString + "&z=" + AppendSettingDialog.Instance.FoursquarePreviewZoom.ToString + "&q=" + lat.ToString + "," + lng.ToString
    End Function

    Public Class GlobalLocation
        Public Property Latitude As Double
        Public Property Longitude As Double
        Public Property LocateInfo As String
    End Class

#End Region

#Region "Google Analytics"
    Public Class GASender
        Inherits HttpConnection

        Private Const GA_ACCOUNT As String = "UA-4618605-5"
        Private Const GA_DOMAIN_HASH As String = "211246021" ' この hash を あとで みつける
        Private Const GA_HOSTNAME As String = "apps.tweenapp.org"
        Private Const GA_VERSION As String = "5.1.5"
        Private Const GA_CHARACTER_SET As String = "shift_jis"

        '#define GA_COLOR_DEPTH                  @"24-bit" // とれるなら かんきょう から
        Private Const GA_JAVA_ENABLED As String = "1"
        Private Const GA_FLASH_VERSION As String = "10.0 r32" '"10.1 r102"をURLエンコード
        Private Const GA_PAGE_TITLE As String = "Tween"
        Private Const GA_GIF_URL As String = "http://www.google-analytics.com/__utm.gif"

        Private UnixEpoch As DateTime = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
        Private Shared rnd As New Random

        Private _language As String
        Private _screenResolution As String
        Private _screenColorDepth As String
        Private _sessionCount As Integer

        Public Property SessionFirst As Long
        Public Property SessionLast As Long

        'Singleton
        Private Shared _me As New GASender
        Public Shared Function GetInstance() As GASender
            Return _me
        End Function

        Private Sub New()
            Me._language = System.Globalization.CultureInfo.CurrentCulture.Name.Replace("_"c, "-"c)
            'Me._language = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName
            Me._screenResolution = String.Format("{0}x{1}",
                                                 System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
                                                 System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height)
            Me._screenColorDepth = String.Format("{0}-bit",
                                                 System.Windows.Forms.Screen.PrimaryScreen.BitsPerPixel)
        End Sub

        Private Sub Init()
            Me._sessionFirst =
                Convert.ToInt64(
                    (DateTime.Now - UnixEpoch).TotalSeconds)
            Me._sessionLast = Me._sessionFirst
        End Sub

        Private Sub SendRequest(ByVal info As Dictionary(Of String, String), ByVal userId As Long)
            If userId = 0 Then Exit Sub
            If Me._SessionFirst = 0 Then Me.Init()

            Me._sessionCount += 1
            Dim sessionCurrent As Long =
                Convert.ToInt64(
                    (DateTime.UtcNow - UnixEpoch).TotalSeconds)
            Dim utma As String =
                String.Format("{0}.{1}.{2}.{3}.{4}.{5}",
                              GA_DOMAIN_HASH,
                              userId,
                              Me._SessionFirst,
                              Me._SessionLast,
                              sessionCurrent,
                              Me._sessionCount)
            Dim utmz As String =
                String.Format("{0}.{1}.{2}.{3}.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)",
                              GA_DOMAIN_HASH,
                              Me._SessionFirst,
                              1,
                              1)
            'Dim utmcc = String.Format("__utma={0};+__utmz={1};",
            '                        utma,
            '                        utmz)
            Dim utmcc = String.Format("__utma={0};",
                                    utma)
            Me._SessionLast = sessionCurrent

            Dim params As New Dictionary(Of String, String) From {
                {"utmwv", GA_VERSION},
                {"utms", "1"},
                {"utmn", rnd.Next().ToString()},
                {"utmhn", GA_HOSTNAME},
                {"utmcs", GA_CHARACTER_SET},
                {"utmsr", Me._screenResolution},
                {"utmsc", Me._screenColorDepth},
                {"utmul", Me._language},
                {"utmje", GA_JAVA_ENABLED},
                {"utmfl", GA_FLASH_VERSION},
                {"utmhid", rnd.Next().ToString()},
                {"utmr", "-"},
                {"utmp", "/"},
                {"utmac", GA_ACCOUNT},
                {"utmcc", utmcc},
                {"utmu", "q~"}
            }
            '                {"utmdt", GA_PAGE_TITLE},

            If info.ContainsKey("page") Then
                params("utmp") = info("page")
                If info.ContainsKey("referer") Then
                    params("utmr") = info("referer")
                End If
            End If
            If info.ContainsKey("event") Then
                params.Add("utmt", "event")
                params.Add("utme", info("event"))
                params("utmr") = "0"
            End If

            Me.GetAsync(params, New Uri(GA_GIF_URL))
        End Sub

        Private Sub GetAsync(ByVal params As Dictionary(Of String, String), ByVal url As Uri)
            Try
                Dim req As HttpWebRequest = CreateRequest(GetMethod, url, params, False)
                req.AllowAutoRedirect = True
                req.Accept = "*/*"
                req.Referer = "http://apps.tweenapp.org/foo.html"
                req.Headers.Add("Accept-Language", "ja-JP")
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; MALC)"
                req.Headers.Add("Accept-Encoding", "gzip, deflate")

                Dim r As IAsyncResult = CType(req.BeginGetResponse(New AsyncCallback(AddressOf GetAsyncResponse), req), IAsyncResult)
            Catch ex As Exception
                'nothing to do
            End Try
        End Sub

        Private Sub GetAsyncResponse(ByVal ar As IAsyncResult)
            Dim res As HttpWebResponse = Nothing
            Try
                res = CType(CType(ar.AsyncState, HttpWebRequest).EndGetResponse(ar), HttpWebResponse)
            Catch ex As Exception
                'nothing to do
            Finally
                If res IsNot Nothing Then res.Close()
            End Try
        End Sub

        Public Sub TrackPage(ByVal page As String, ByVal userId As Long)
            Me.SendRequest(New Dictionary(Of String, String) From {{"page", page}}, userId)
        End Sub

        Public Sub TrackEventWithCategory(ByVal category As String,
                                          ByVal action As String,
                                          ByVal userId As Long)
            Me.TrackEventWithCategory(category, action, Nothing, Nothing, userId)
        End Sub

        Public Sub TrackEventWithCategory(ByVal category As String,
                                          ByVal action As String,
                                          ByVal label As String,
                                          ByVal userId As Long)
            Me.TrackEventWithCategory(category, action, label, Nothing, userId)
        End Sub

        Public Sub TrackEventWithCategory(ByVal category As String,
                                          ByVal action As String,
                                          ByVal label As String,
                                          ByVal value As String,
                                          ByVal userId As Long)
            Dim builder As New System.Text.StringBuilder
            builder.AppendFormat("5({0}*{1}", category, action)
            If Not String.IsNullOrEmpty(label) Then
                builder.AppendFormat("*{0}", label)
            End If
            If Not String.IsNullOrEmpty(value) Then
                builder.AppendFormat(")({0}", value)
            End If
            builder.Append(")")
            Me.SendRequest(New Dictionary(Of String, String) From {{"event", builder.ToString()}}, userId)
        End Sub

    End Class
#End Region
End Class
