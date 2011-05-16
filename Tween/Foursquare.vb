Imports System.Net
Imports System.Runtime.Serialization
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Text
Imports System.Runtime.Serialization.Json

Public Class Foursquare
    Inherits HttpConnection

    Private Shared _instance As New Foursquare
    Public Shared ReadOnly Property GetInstance As Foursquare
        Get
            Return _instance
        End Get
    End Property

    Private CheckInUrlsVenueCollection As New Dictionary(Of String, GlobalLocation)
    Public Function GetVenueId(ByVal url As String) As String
        Dim content As String = ""

        If content = "" Then Return "" 'ClientIdを設定するまで無効にします。設定後は外してください

        Dim res As HttpStatusCode = GetContent("GET", New Uri(url), Nothing, content)
        If res <> HttpStatusCode.OK Then MessageBox.Show(res.ToString)
        Dim mc As Match = Regex.Match(content, "/venue/(?<venueId>[0-9]+)", RegexOptions.IgnoreCase)
        If mc.Success Then
            Dim vId As String = mc.Result("${venueId}")
            Return vId
        Else
            Return ""
        End If
    End Function

    Public Function GetVenueInfo(ByVal venueId As String) As FourSquareDataModel.Venue
        Dim content As String = ""

        Dim res As HttpStatusCode = GetContent("GET", New Uri(CreateAuthUri(venueId)), Nothing, content)

        If res = HttpStatusCode.OK Then
            Dim curData As FourSquareDataModel.FourSquareData = Nothing
            Try
                curData = CreateDataFromJson(Of FourSquareDataModel.FourSquareData)(content)
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
            End Try

            Return curData.Response.Venue
        Else
            Dim curData As FourSquareDataModel.FourSquareData = Nothing
            Try
                curData = CreateDataFromJson(Of FourSquareDataModel.FourSquareData)(content)
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
            End Try
            MessageBox.Show(res.ToString + Environment.NewLine + curData.Meta.ErrorType + Environment.NewLine + curData.Meta.ErrorDetail)

        End If
        Return Nothing
    End Function

    Public Function GetMapsUri(ByVal url As String) As String
        If Not AppendSettingDialog.Instance.IsPreviewFoursquare Then Return Nothing
        Dim urlId As String = url.Replace("http://4sq.com/", "")
        If CheckInUrlsVenueCollection.ContainsKey(urlId) Then Return CreateGoogleMapsUri(CheckInUrlsVenueCollection(urlId))
        Dim curVenue As FourSquareDataModel.Venue = Nothing
        Dim venueId As String = GetVenueId(url)
        If Not venueId = "" Then curVenue = GetVenueInfo(venueId)
        If curVenue Is Nothing Then Return Nothing
        Dim curLocation As New GlobalLocation With {.Latitude = curVenue.Location.Latitude, .Longitude = curVenue.Location.Longitude}
        CheckInUrlsVenueCollection.Add(urlId, curLocation)
        Return CreateGoogleMapsUri(curLocation)
    End Function

    Public Class GlobalLocation
        Public Property Latitude As Double
        Public Property Longitude As Double
    End Class

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
            <DataMember(name:="meta", isRequired:=False)> Public Meta As Meta
            <DataMember(name:="response", isRequired:=False)> Public Response As Response
        End Class

        <DataContract()>
        Public Class Response
            <DataMember(name:="venue", isRequired:=False)> Public Venue As Venue
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
            <DataMember(name:="code")> Public Code As Integer
            <DataMember(Name:="errorType", IsRequired:=False)> Public ErrorType As String
            <DataMember(Name:="errorDetail", IsRequired:=False)> Public ErrorDetail As String
        End Class
    End Class
#End Region

    Private ReadOnly Property CreateAuthUri(ByVal vId As String) As String
        Get
            'コメントアウト部分を直してください
            Return "https://api.foursquare.com/v2/venues/" + vId '+ "?client_id=" + ClientId + "&client_secret=" + ClientSecret
        End Get
    End Property

    Private Overloads Function CreateGoogleMapsUri(ByVal locate As GlobalLocation) As String
        Return CreateGoogleMapsUri(locate.Latitude, locate.Longitude)
    End Function

    Private Overloads Function CreateGoogleMapsUri(ByVal lat As Double, ByVal lng As Double) As String
        Return "http://maps.google.com/maps/api/staticmap?center=" + lat.ToString + "," + lng.ToString + "&size=" + AppendSettingDialog.Instance.FoursquarePreviewWidth.ToString + "x" + AppendSettingDialog.Instance.FoursquarePreviewHeight.ToString + "&zoom=" + AppendSettingDialog.Instance.FoursquarePreviewZoom.ToString + "&markers=" + lat.ToString + "," + lng.ToString + "&sensor=false"
    End Function

    Public Function CreateDataFromJson(Of T)(ByVal content As String) As T
        Dim data As T
        Using stream As New MemoryStream()
            Dim buf As Byte() = Encoding.Unicode.GetBytes(content)
            stream.Write(Encoding.Unicode.GetBytes(content), offset:=0, count:=buf.Length)
            stream.Seek(offset:=0, loc:=SeekOrigin.Begin)
            data = DirectCast((New DataContractJsonSerializer(GetType(T))).ReadObject(stream), T)
        End Using
        Return data
    End Function

End Class