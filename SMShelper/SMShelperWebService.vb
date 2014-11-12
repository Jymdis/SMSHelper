' NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SMShelperWebService" in both code and config file together.
Imports SMSHelper.SMShelper

Public Class SMShelperWebService
    Implements ISMShelperWebService

    'Public Sub DoWork() Implements ISMShelperWebService.DoWork
    'End Sub

    Public Sub SendSMS(ByVal strSMS As String, ByVal strPhonenumber As String) Implements ISMShelperWebService.SendSMS

        Dim s As New SMShelper
        s.SendMessage(strPhonenumber, strSMS)


    End Sub

End Class
