Imports System.Net
Imports System.Runtime.Serialization
Imports System.Text.RegularExpressions

Public Class Foursquare
    Inherits HttpConnection

    Private Shared _instance As New Foursquare
    Public Shared ReadOnly Property GetInstance As Foursquare
        Get
            Return _instance
        End Get
    End Property

    Private _authKey As New Dictionary(Of String, String) From {
        {"client_id", "VWVC5NMXB1T5HKOYAKARCXKZDOHDNYSRLEMDDQYJNSJL2SUU"},
        {"client_secret", DecryptString("eXXMGYXZyuDxz/lJ9nLApihoUeEGXNLEO0ZDCAczvwdKgGRExZl1Xyac/ezNTwHFOLUZqaA8tbA=")}
    }

    Private CheckInUrlsVenueCollection As New Dictionary(Of String, Google.GlobalLocation)

    Private Function GetVenueId(ByVal url As String) As String
        Dim content As String = ""
        Try
            Dim res As HttpStatusCode = GetContent("GET", New Uri(url), Nothing, content)
            If res <> HttpStatusCode.OK Then Return ""
        Catch ex As Exception
            Return ""
        End Try
        Dim mc As Match = Regex.Match(content, "/venue/(?<venueId>[0-9]+)", RegexOptions.IgnoreCase)
        If mc.Success Then
            Dim vId As String = mc.Result("${venueId}")
            Return vId
        Else
            Return ""
        End If
    End Function

    Private Function GetVenueInfo(ByVal venueId As String) As FourSquareDataModel.Venue
        Dim content As String = ""
        Try
            Dim res As HttpStatusCode = GetContent("GET",
                                                   New Uri("https://api.foursquare.com/v2/venues/" + venueId),
                                                   _authKey,
                                                   content)

            If res = HttpStatusCode.OK Then
                Dim curData As FourSquareDataModel.FourSquareData = Nothing
                Try
                    curData = CreateDataFromJson(Of FourSquareDataModel.FourSquareData)(content)
                Catch ex As Exception
                    Return Nothing
                End Try

                Return curData.Response.Venue
            Else
                'Dim curData As FourSquareDataModel.FourSquareData = Nothing
                'Try
                '    curData = CreateDataFromJson(Of FourSquareDataModel.FourSquareData)(content)
                'Catch ex As Exception
                '    Return Nothing
                'End Try
                'MessageBox.Show(res.ToString + Environment.NewLine + curData.Meta.ErrorType + Environment.NewLine + curData.Meta.ErrorDetail)
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetMapsUri(ByVal url As String, ByRef refText As String) As String
        If Not AppendSettingDialog.Instance.IsPreviewFoursquare Then Return Nothing

        Dim urlId As String = Regex.Replace(url, "https?://(4sq|foursquare)\.com/", "")

        If CheckInUrlsVenueCollection.ContainsKey(urlId) Then
            refText = CheckInUrlsVenueCollection(urlId).LocateInfo
            Return (New Google).CreateGoogleStaticMapsUri(CheckInUrlsVenueCollection(urlId))
        End If

        Dim curVenue As FourSquareDataModel.Venue = Nothing
        Dim venueId As String = GetVenueId(url)
        If String.IsNullOrEmpty(venueId) Then Return Nothing

        curVenue = GetVenueInfo(venueId)
        If curVenue Is Nothing Then Return Nothing

        Dim curLocation As New Google.GlobalLocation With {.Latitude = curVenue.Location.Latitude, .Longitude = curVenue.Location.Longitude, .LocateInfo = CreateVenueInfoText(curVenue)}
        '例外発生の場合があるため
        If Not CheckInUrlsVenueCollection.ContainsKey(urlId) Then CheckInUrlsVenueCollection.Add(urlId, curLocation)
        refText = curLocation.LocateInfo
        Return (New Google).CreateGoogleStaticMapsUri(curLocation)
    End Function

    Private ReadOnly Property CreateVenueInfoText(ByVal info As FourSquareDataModel.Venue) As String
        Get
            Return info.Name + Environment.NewLine + info.Stats.UsersCount.ToString + "/" + info.Stats.CheckinsCount.ToString + Environment.NewLine + info.Location.Address + Environment.NewLine + info.Location.City + info.Location.State + Environment.NewLine + info.Location.Latitude.ToString + Environment.NewLine + info.Location.Longitude.ToString
        End Get
    End Property
    Public Function GetContent(ByVal method As String, _
                ByVal requestUri As Uri, _
                ByVal param As Dictionary(Of String, String), _
                ByRef content As String) As HttpStatusCode

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    False)
        Dim code As HttpStatusCode
        code = GetResponse(webReq, content, Nothing, False)
        Return code
    End Function

#Region "FourSquare DataModel"

    Public Class FourSquareDataModel

        <DataContract()>
        Public Class FourSquareData
            <DataMember(Name:="meta", isRequired:=False)> Public Meta As Meta
            <DataMember(Name:="response", isRequired:=False)> Public Response As Response
        End Class

        <DataContract()>
        Public Class Response
            <DataMember(Name:="venue", isRequired:=False)> Public Venue As Venue
        End Class

        <DataContract()>
        Public Class Venue
            <DataMember(Name:="id")> Public Id As String
            <DataMember(Name:="name")> Public Name As String
            <DataMember(Name:="location")> Public Location As Location
            <DataMember(Name:="verified")> Public Verified As Boolean
            <DataMember(Name:="stats")> Public Stats As Stats
            <DataMember(Name:="mayor")> Public Mayor As Mayor
            <DataMember(Name:="shortUrl")> Public ShortUrl As String
            <DataMember(Name:="timeZone")> Public TimeZone As String
        End Class

        <DataContract()>
        Public Class Location
            <DataMember(Name:="address")> Public Address As String
            <DataMember(Name:="city")> Public City As String
            <DataMember(Name:="state")> Public State As String
            <DataMember(Name:="lat")> Public Latitude As Double
            <DataMember(Name:="lng")> Public Longitude As Double
        End Class

        <DataContract()>
        Public Class Stats
            <DataMember(Name:="checkinsCount")> Public CheckinsCount As Integer
            <DataMember(Name:="usersCount")> Public UsersCount As Integer
        End Class


        <DataContract()>
        Public Class Mayor
            <DataMember(Name:="count")> Public Count As Integer
            <DataMember(Name:="user", isrequired:=False)> Public User As FoursquareUser
        End Class

        <DataContract()>
        Public Class FoursquareUser
            <DataMember(Name:="id")> Public Id As Integer
            <DataMember(Name:="firstName")> Public FirstName As String
            <DataMember(Name:="photo")> Public Photo As String
            <DataMember(Name:="gender")> Public Gender As String
            <DataMember(Name:="homeCity")> Public HomeCity As String
        End Class

        <DataContract()>
        Public Class Meta
            <DataMember(Name:="code")> Public Code As Integer
            <DataMember(Name:="errorType", IsRequired:=False)> Public ErrorType As String
            <DataMember(Name:="errorDetail", IsRequired:=False)> Public ErrorDetail As String
        End Class
    End Class
#End Region


End Class