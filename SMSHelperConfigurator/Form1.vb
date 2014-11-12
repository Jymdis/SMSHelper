Imports System.Configuration
Public Class frmSMSHelperConfigurator
    Private Sub frmSMSHelperConfigurator_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        SMSHelperController.MachineName = System.Windows.Forms.SystemInformation.ComputerName
        CheckServiceStatus()
        PopulateSettings()
    End Sub
    Private Sub CheckServiceStatus()

        btnStart.Enabled = False
        btnStop.Enabled = False

        SMSHelperController.Refresh()

        Select Case SMSHelperController.Status
            Case ServiceProcess.ServiceControllerStatus.Stopped
                lblStatus.Text = "Stopped"
                btnStart.Enabled = True
            Case ServiceProcess.ServiceControllerStatus.Running
                lblStatus.Text = "Running"
                btnStop.Enabled = True
            Case Else
                lblStatus.Text = "Unknown"
        End Select

    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStart.Click
        StartService()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        StopService()
    End Sub

    Private Sub StopService()
        btnStop.Enabled = False
        BackgroundWorker1.RunWorkerAsync("Stop")
    End Sub
    Private Sub StartService()
        btnStop.Enabled = False
        BackgroundWorker1.RunWorkerAsync("Start")
    End Sub
    Private Sub tmrStatus_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrStatus.Tick
        CheckServiceStatus()
    End Sub
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork

        Select Case e.Argument.ToString
            Case "Start"
                SMSHelperController.Start()
                SMSHelperController.WaitForStatus(ServiceProcess.ServiceControllerStatus.Running)
            Case "Stop"
                SMSHelperController.Stop()
                SMSHelperController.WaitForStatus(ServiceProcess.ServiceControllerStatus.Stopped)
        End Select

    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        CheckServiceStatus()
    End Sub



    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        If FolderBrowserDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtPath.Text = FolderBrowserDialog1.SelectedPath.ToString
        End If
    End Sub

    Private Sub PopulateSettings()

        Dim strExepath As String

        'Måste åtgärdas, går inte att ha hårdkodat
        strExepath = "C:\Users\ala\Documents\Visual Studio 2010\Projects\SMShelper\SMShelper\SMShelper.exe"

        Dim filemap As New ExeConfigurationFileMap()
        filemap.ExeConfigFilename = "SMShelper.exe.config"
        Dim config As Configuration
        config = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None)

        txtEmailTo.Text = config.AppSettings.Settings.Item("Mailto").ToString

    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        PopulateSettings()
    End Sub


    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

    End Sub
End Class
