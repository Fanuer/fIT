#Requires -Version 3.0

<#
.SYNOPSIS
Erstellt eine Microsoft Azure-Website für ein Visual Studio-Webprojekt und stellt diese bereit.
Eine detailliertere Dokumentation finden Sie unter: http://go.microsoft.com/fwlink/?LinkID=394471 

.EXAMPLE
PS C:\> .\Publish-WebApplicationWebSite.ps1 `
-Configuration .\Configurations\WebApplication1-WAWS-dev.json `
-WebDeployPackage ..\WebApplication1\WebApplication1.zip `
-Verbose

#>
[CmdletBinding(HelpUri = 'http://go.microsoft.com/fwlink/?LinkID=391696')]
param
(
    [Parameter(Mandatory = $true)]
    [ValidateScript({Test-Path $_ -PathType Leaf})]
    [String]
    $Configuration,

    [Parameter(Mandatory = $false)]
    [String]
    $SubscriptionName,

    [Parameter(Mandatory = $false)]
    [ValidateScript({Test-Path $_ -PathType Leaf})]
    [String]
    $WebDeployPackage,

    [Parameter(Mandatory = $false)]
    [ValidateScript({ !($_ | Where-Object { !$_.Contains('Name') -or !$_.Contains('Password')}) })]
    [Hashtable[]]
    $DatabaseServerPassword,

    [Parameter(Mandatory = $false)]
    [Switch]
    $SendHostMessagesToOutput = $false
)


function New-WebDeployPackage
{
    #Funktion zum Entwickeln und Verpacken Ihrer Webanwendung erstellen

    #Verwenden Sie 'MsBuild.exe', um Ihre Webanwendung zu erstellen. Hilfe dazu finden Sie in der MSBuild-Befehlszeilenreferenz unter http://go.microsoft.com/fwlink/?LinkId=391339.
}

function Test-WebApplication
{
    #Diese Funktion bearbeiten, um Komponententests an Ihrer Webanwendung durchzuführen

    #Schreiben Sie eine Funktion zum Ausführen von Komponententests für Ihre Webanwendung (unter Verwendung von 'VSTest.Console.exe'). Hilfe dazu finden Sie in der VSTest.Console-Befehlszeilenreferenz unter http://go.microsoft.com/fwlink/?LinkId=391340.
}

function New-AzureWebApplicationWebsiteEnvironment
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Configuration,

        [Parameter (Mandatory = $false)]
        [AllowNull()]
        [Hashtable[]]
        $DatabaseServerPassword
    )
       
    Add-AzureWebsite -Name $Config.name -Location $Config.location | Out-String | Write-HostWithTime
    # Erstellen Sie die SQL-Datenbanken. Die Verbindungszeichenfolge wird für die Bereitstellung verwendet.
    $connectionString = New-Object -TypeName Hashtable
    
    if ($Config.Contains('databases'))
    {
        @($Config.databases) |
            Where-Object {$_.connectionStringName -ne ''} |
            Add-AzureSQLDatabases -DatabaseServerPassword $DatabaseServerPassword -CreateDatabase |
            ForEach-Object { $connectionString.Add($_.Name, $_.ConnectionString) }           
    }
    
    return @{ConnectionString = $connectionString}   
}

function Publish-AzureWebApplicationToWebsite
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Configuration,

        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Hashtable]
        $ConnectionString,

        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $WebDeployPackage
    )

    if ($ConnectionString -and $ConnectionString.Count -gt 0)
    {
        Publish-AzureWebsiteProject `
            -Name $Config.name `
            -Package $WebDeployPackage `
            -ConnectionString $ConnectionString
    }
    else
    {
        Publish-AzureWebsiteProject `
            -Name $Config.name `
            -Package $WebDeployPackage
    }
}


# Hauptroutine des Skripts
Set-StrictMode -Version 3
Import-Module Azure

try {
    $AzureToolsUserAgentString = New-Object -TypeName System.Net.Http.Headers.ProductInfoHeaderValue -ArgumentList 'VSAzureTools', '1.4'
    [Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.UserAgents.Add($AzureToolsUserAgentString)
} catch {}

Remove-Module AzureWebSitePublishModule -ErrorAction SilentlyContinue
$scriptDirectory = Split-Path -Parent $PSCmdlet.MyInvocation.MyCommand.Definition
Import-Module ($scriptDirectory + '\AzureWebSitePublishModule.psm1') -Scope Local -Verbose:$false

New-Variable -Name VMWebDeployWaitTime -Value 30 -Option Constant -Scope Script 
New-Variable -Name AzureWebAppPublishOutput -Value @() -Scope Global -Force
New-Variable -Name SendHostMessagesToOutput -Value $SendHostMessagesToOutput -Scope Global -Force

try
{
    $originalErrorActionPreference = $Global:ErrorActionPreference
    $originalVerbosePreference = $Global:VerbosePreference
    
    if ($PSBoundParameters['Verbose'])
    {
        $Global:VerbosePreference = 'Continue'
    }
    
    $scriptName = $MyInvocation.MyCommand.Name + ':'
    
    Write-VerboseWithTime ($scriptName + ' Starten')
    
    $Global:ErrorActionPreference = 'Stop'
    Write-VerboseWithTime ('{0} $ErrorActionPreference ist auf {1} festgelegt.' -f $scriptName, $ErrorActionPreference)
    
    Write-Debug ('{0}: $PSCmdlet.ParameterSetName = {1}' -f $scriptName, $PSCmdlet.ParameterSetName)

    # Speichern Sie das aktuelle Abonnement. Es wird später im Skript auf den aktuellen Status zurückgesetzt.
    Backup-Subscription -UserSpecifiedSubscription $SubscriptionName
    
    # Prüfen Sie, ob Sie Azure-Modul Version 0.7.4 oder höher verwenden.
    if (-not (Test-AzureModule))
    {
         throw 'Ihre Version von Microsoft Azure PowerShell ist veraltet. Die neueste Version finden Sie unter http://go.microsoft.com/fwlink/?LinkID=320552.'
    }
    
    if ($SubscriptionName)
    {

        # Falls Sie einen Abonnementnamen angegeben haben, prüfen Sie, ob das Abonnement in Ihrem Konto vorhanden ist.
        if (!(Get-AzureSubscription -SubscriptionName $SubscriptionName))
        {
            throw ("{0}: Der Abonnementname $SubscriptionName wurde nicht gefunden." -f $scriptName)

        }

        # Legen Sie für das angegebene Abonnement aktuell fest.
        Select-AzureSubscription -SubscriptionName $SubscriptionName | Out-Null

        Write-VerboseWithTime ('{0}: Das Abonnement ist auf {1} festgelegt.' -f $scriptName, $SubscriptionName)
    }

    $Config = Read-ConfigFile $Configuration 

    #Webanwendung entwickeln und verpacken
    New-WebDeployPackage

    #Komponententests an Ihrer Webanwendung ausführen
    Test-WebApplication

    #Azure-Umgebung gemäß Beschreibung in der JSON-Konfigurationsdatei erstellen
    $newEnvironmentResult = New-AzureWebApplicationWebsiteEnvironment -Configuration $Config -DatabaseServerPassword $DatabaseServerPassword

    #Webanwendungspaket bereitstellen, wenn $WebDeployPackage vom Benutzer angegeben wird 
    if($WebDeployPackage)
    {
        Publish-AzureWebApplicationToWebsite `
            -Configuration $Config `
            -ConnectionString $newEnvironmentResult.ConnectionString `
            -WebDeployPackage $WebDeployPackage
    }
}
finally
{
    $Global:ErrorActionPreference = $originalErrorActionPreference
    $Global:VerbosePreference = $originalVerbosePreference

    # Das ursprüngliche aktuelle Abonnement auf den aktuellen Status zurücksetzen
    Restore-Subscription

    Write-Output $Global:AzureWebAppPublishOutput    
    $Global:AzureWebAppPublishOutput = @()
}
