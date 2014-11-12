Imports System.Threading
Imports SMSHelper
Imports System.ServiceProcess
Imports System.ServiceModel

Public Class SMSHelperService
    Inherits ServiceBase
    Dim servicestarted As Boolean
    Dim workerthread As Thread
    Dim servicehost As ServiceHost = Nothing

    
    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.

        If Not EventLog.SourceExists("SMShelper") Then
            EventLog.CreateEventSource("SMShelper", "Application")
        End If
        EventLog.Source = "SMShelper"

        Try

            Dim st As New ThreadStart(AddressOf Workerfunction)
            workerthread = New Thread(st)

            servicestarted = True

            workerthread.Start()
            EventLog.WriteEntry("SMShelper started succesfully.", System.Diagnostics.EventLogEntryType.Information)




        Catch ex As Exception

            EventLog.WriteEntry("SMShelper did not start succesfully. Error message: " & ex.Message.ToString, System.Diagnostics.EventLogEntryType.Error)

        End Try

        'Starta SMShelperWebService

        Try
                 


            servicehost = New ServiceHost(GetType(SMShelperWebService))

            servicehost.Open()

            EventLog.WriteEntry("SMShelperWebService started succesfully.", System.Diagnostics.EventLogEntryType.Information)


        Catch ex As Exception
            EventLog.WriteEntry("SMShelperWebService did not start succesfully. Error message: " & ex.Message.ToString, System.Diagnostics.EventLogEntryType.Error)

        End Try


    End Sub

    Protected Overrides Sub OnStop()
        EventLog.Source = "SMShelper"
        Try

            servicestarted = False

            workerthread.Join(New TimeSpan(0, 2, 0))

            EventLog.WriteEntry("SMShelper was succesfully stopped.", System.Diagnostics.EventLogEntryType.Information)
        Catch ex As Exception
            EventLog.WriteEntry("SMShelper did not stop succesfully. Error message: " & ex.Message.ToString, System.Diagnostics.EventLogEntryType.Error)

        End Try



        Try
            If servicehost IsNot Nothing Then
                servicehost.Close()
                servicehost = Nothing
                EventLog.WriteEntry("SMShelperWebService was succesfully stopped.", System.Diagnostics.EventLogEntryType.Information)
            End If
            EventLog.WriteEntry("SMShelperWebService was already stopped or in an unknown state.", System.Diagnostics.EventLogEntryType.Information)
        Catch ex As Exception
            EventLog.WriteEntry("SMShelperWebService did not stop succesfully. Error message: " & ex.Message.ToString, System.Diagnostics.EventLogEntryType.Error)

        End Try
    End Sub

    Private Sub Workerfunction()

        EventLog.Source = "SMShelper"
        Try
            Dim s As New SMShelper
            While (servicestarted)

                s.LoopSMSes()
            

                If (servicestarted) Then

                    Thread.Sleep(New TimeSpan(0, 0, 30))

                End If

            End While

            Thread.CurrentThread.Abort()

        Catch ex As Exception

            EventLog.WriteEntry("SMShelper did not run succesfully. Error message: " & ex.Message.ToString & " Source:" & ex.Source.ToString, System.Diagnostics.EventLogEntryType.Error)

        End Try

    End Sub

   
End Class
