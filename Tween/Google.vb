Imports System.Web
Imports System.Text.RegularExpressions
Imports System.Runtime.Serialization.Json
Imports System.Net
Imports System.Runtime.Serialization

' http://code.google.com/intl/ja/apis/ajaxlanguage/documentation/#fonje
' デベロッパー ガイド - Google AJAX Language API - Google Code

Public Class Google

    Private Const TranslateEndPoint As String = "http://ajax.googleapis.com/ajax/services/language/translate"
    Private Const LanguageDetectEndPoint As String = "https://ajax.googleapis.com/ajax/services/language/detect"

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

    Public Function Translate(ByVal srclng As String, ByVal dstlng As String, ByVal source As String, ByRef destination As String) As Boolean
        Dim http As New HttpVarious()
        Dim apiurl As String = TranslateEndPoint
        Dim headers As New Dictionary(Of String, String)
        headers.Add("v", "1.0")

        If String.IsNullOrEmpty(srclng) OrElse String.IsNullOrEmpty(dstlng) Then
            Return False
        End If
        headers.Add("User-Agent", "Tween/" + fileVersion)
        headers.Add("langpair", srclng + "|" + dstlng)

        headers.Add("q", source)

        Dim content As String = ""
        If http.GetData(apiurl, headers, content) Then
            Dim serializer As New DataContractJsonSerializer(GetType(TranslateResponse))
            Dim res As TranslateResponse = CreateDataFromJson(Of TranslateResponse)(content)

            If res.ResponseData Is Nothing Then
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
        headers.Add("User-Agent", "Tween/" + fileVersion)
        headers.Add("v", "1.0")
        headers.Add("q", source)
        Dim content As String = ""
        If http.GetData(apiurl, headers, content) Then
            Dim serializer As New DataContractJsonSerializer(GetType(LanguageDetectResponse))
            Dim res As LanguageDetectResponse = CreateDataFromJson(Of LanguageDetectResponse)(content)
            Return res.ResponseData.Language
        End If
        Return ""
    End Function

End Class
