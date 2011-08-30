Public Class Bing

    Private Const AppId As String = "8DFACAC0C4891D0F75F5728391C9D30664B797A1"

#Region "言語テーブル定義"
    Private Shared ReadOnly LanguageTable As New List(Of String) From {
        "af",
        "sq",
        "ar-sa",
        "ar-iq",
        "ar-eg",
        "ar-ly",
        "ar-dz",
        "ar-ma",
        "ar-tn",
        "ar-om",
        "ar-ye",
        "ar-sy",
        "ar-jo",
        "ar-lb",
        "ar-kw",
        "ar-ae",
        "ar-bh",
        "ar-qa",
        "eu",
        "bg",
        "be",
        "ca",
        "zh-tw",
        "zh-cn",
        "zh-hk",
        "zh-sg",
        "hr",
        "cs",
        "da",
        "nl",
        "nl-be",
        "en",
        "en-us",
        "en-gb",
        "en-au",
        "en-ca",
        "en-nz",
        "en-ie",
        "en-za",
        "en-jm",
        "en",
        "en-bz",
        "en-tt",
        "et",
        "fo",
        "fa",
        "fi",
        "fr",
        "fr-be",
        "fr-ca",
        "fr-ch",
        "fr-lu",
        "gd",
        "ga",
        "de",
        "de-ch",
        "de-at",
        "de-lu",
        "de-li",
        "el",
        "he",
        "hi",
        "hu",
        "is",
        "id",
        "it",
        "it-ch",
        "ja",
        "ko",
        "ko",
        "lv",
        "lt",
        "mk",
        "ms",
        "mt",
        "no",
        "no",
        "pl",
        "pt-br",
        "pt",
        "rm",
        "ro",
        "ro-mo",
        "ru",
        "ru-mo",
        "sz",
        "sr",
        "sr",
        "sk",
        "sl",
        "sb",
        "es",
        "es-mx",
        "es-gt",
        "es-cr",
        "es-pa",
        "es-do",
        "es-ve",
        "es-co",
        "es-pe",
        "es-ar",
        "es-ec",
        "es-cl",
        "es-uy",
        "es-py",
        "es-bo",
        "es-sv",
        "es-hn",
        "es-ni",
        "es-pr",
        "sx",
        "sv",
        "sv-fi",
        "th",
        "ts",
        "tn",
        "tr",
        "uk",
        "ur",
        "ve",
        "vi",
        "xh",
        "ji",
        "zu"
    }
#End Region

#Region "Translation"

    Private Const TranslateUri As String = "http://api.microsofttranslator.com/v2/Http.svc/Translate?appId=" + AppId

    Public Function Translate(ByVal _from As String,
                                ByVal _to As String,
                                ByVal _text As String,
                                ByRef buf As String) As Boolean

        Dim http As New HttpVarious()
        Dim apiurl As String = TranslateUri + "&text=" + _text + "&to=" + _to
        Dim content As String = ""
        If http.GetData(apiurl, Nothing, content) Then
            buf = String.Copy(content)
            Return True
        End If
        Return False
    End Function

    Public Function GetLanguageEnumFromIndex(ByVal index As Integer) As String
        Return LanguageTable(index)
    End Function

    Public Function GetIndexFromLanguageEnum(ByVal lang As String) As Integer
        Return LanguageTable.IndexOf(lang)
    End Function
#End Region
End Class
