Imports System.Configuration
Imports System.Diagnostics


Public Class SMShelper



    Protected Function CallCommand(ByVal strCommand As String)
        Dim strResponse As String
        Try

            Dim p As New Process
            p.StartInfo.UseShellExecute = False
            p.StartInfo.RedirectStandardOutput = True
            p.StartInfo.CreateNoWindow = True

            p.StartInfo.FileName = ConfigurationManager.AppSettings("PathToTele2Connect") & "\eSupAt.exe"
            p.StartInfo.Arguments = strCommand
            p.Start()

            strResponse = p.StandardOutput.ReadToEnd()
            p.WaitForExit()

            Return strResponse



        Catch ex As Exception

            Dim myLog As New EventLog()
            myLog.Source = "SMShelper"
            myLog.WriteEntry("SMShelper could not parse SMS (function CallCommand): " & ex.Message.ToString & " Stacktrace:" & ex.StackTrace.ToString, System.Diagnostics.EventLogEntryType.Error)


        End Try

        strResponse = "0,25"

        Return strResponse
    End Function


    Public Function SMSes()
        Dim intSMSes As Integer
        Dim strTemp As String
        Dim strSMSes(2) As String

        Try
        

            strTemp = CallCommand("-smses")
            strSMSes = Split(strTemp, ",")

            intSMSes = Convert.ToInt32(strSMSes(0))



            Return intSMSes

        Catch ex As Exception
            Dim myLog As New EventLog()
            myLog.Source = "SMShelper"
            myLog.WriteEntry("SMShelper could not parse SMS (function CallCommand): " & ex.Message.ToString & " Stacktrace:" & ex.StackTrace.ToString, System.Diagnostics.EventLogEntryType.Error)



        End Try



        intSMSes = 0

        Return intSMSes

    End Function
    Public Function ReadSMS(ByVal intSMS As Integer) As Array



        Dim strSMS(3) As String

        Try
            Dim strTemp As String
            Dim strSMSText As String

            strTemp = CallCommand("-smsread " & intSMS.ToString)

            strSMS = Split(strTemp, ",")
            strSMSText = strSMS(3)

            If ConfigurationManager.AppSettings("DeleteSMSesAfterMail") = "True" Then
                CallCommand("-smsdel " & intSMS.ToString)
            End If

        Catch ex As Exception
            Dim myLog As New EventLog()
            myLog.Source = "SMShelper"
            myLog.WriteEntry("SMShelper could not parse SMS (function ReadSMS): " & ex.Message.ToString & " Stacktrace:" & ex.StackTrace.ToString, System.Diagnostics.EventLogEntryType.Error)


        End Try


        'Gammal kod
        'Return strSMSText
        'Ny kod
        Return strSMS
    End Function

    Public Function LoopSMSes()

        Dim intSMSes As Integer
        Dim strSMSInfo(3) As String

        Try
            intSMSes = SMSes()

            For i = 0 To intSMSes - 1
                strSMSInfo = ReadSMS(i)
                SendMail(strSMSInfo(3), strSMSInfo(1), strSMSInfo(2))
            Next
        Catch ex As Exception
            Dim myLog As New EventLog()
            myLog.Source = "SMShelper"
            myLog.WriteEntry("SMShelper could not parse SMS (function LoopSMSes): " & ex.Message.ToString & " Stacktrace:" & ex.StackTrace.ToString, System.Diagnostics.EventLogEntryType.Error)


        End Try
        Return True
    End Function

    Public Sub SendMail(ByVal strSMSText As String, ByVal strSMSPhoneno As String, ByVal strSMSDate As String)
        Try

     
        Dim strBody As String
        strBody = Replace(ConfigurationManager.AppSettings("MailBody"), "$strSMSText", strSMSText)
        strBody = Replace(strBody, "$strSMSPhoneno", strSMSPhoneno)
        strBody = Replace(strBody, "$strSMSDate", strSMSDate)

        Dim MailObj As New System.Net.Mail.SmtpClient
        Dim mm As New System.Net.Mail.MailMessage(ConfigurationManager.AppSettings("MailFrom"), ConfigurationManager.AppSettings("MailTo"))
        mm.Subject = ConfigurationManager.AppSettings("MailSubject")
        mm.Body = strBody
        mm.IsBodyHtml = True
        MailObj.Host = ConfigurationManager.AppSettings("SMTPserver")
        MailObj.Send(mm)
        MailObj = Nothing
        Catch ex As Exception
            Dim myLog As New EventLog()
            myLog.Source = "SMShelper"
            myLog.WriteEntry("SMShelper could not send mail: " & ex.Message.ToString & " Stacktrace:" & ex.StackTrace.ToString, System.Diagnostics.EventLogEntryType.Error)

        End Try
    End Sub
    Public Function SendMessage(ByVal strPhoneno As String, ByVal strMessage As String)
        'Exprimental, not finished yet. 

        Try

     
            CallCommand("-smssend " & strPhoneno & " '" & strMessage & "'")
            Return True

        Catch ex As Exception
            Dim myLog As New EventLog()
            myLog.Source = "SMShelper"
            myLog.WriteEntry("SMShelper could not send SMS: " & ex.Message.ToString & " Stacktrace:" & ex.StackTrace.ToString, System.Diagnostics.EventLogEntryType.Error)

        End Try
        Return False

    End Function
End Class
