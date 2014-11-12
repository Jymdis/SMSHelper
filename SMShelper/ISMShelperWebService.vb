Imports System.ServiceModel
Imports System.ComponentModel
Imports System.ServiceProcess
Imports System.Configuration
Imports System.Configuration.Install
' NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISMShelperWebService" in both code and config file together.
<ServiceContract()>
Public Interface ISMShelperWebService

    <OperationContract()>
    Sub SendSMS(ByVal strSMS As String, ByVal strPhonenumber As String)
    'Sub DoWork()





End Interface
