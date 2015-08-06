#  AzureWebSitePublishModule.psm1 ist ein Windows PowerShell-Skriptmodul. Dieses Modul exportiert Windows PowerShell-Funktionen, die die Lebenszyklusverwaltung für Webanwendungen automatisieren. Sie können die Funktionen im vorliegenden Zustand verwenden oder an Ihre Anwendung und Veröffentlichungsumgebung anpassen.

Set-StrictMode -Version 3

# Eine Variable zum Speichern des ursprünglichen Abonnements.
$Script:originalCurrentSubscription = $null

# Eine Variable zum Speichern des ursprünglichen Speicherkontos.
$Script:originalCurrentStorageAccount = $null

# Eine Variable zum Speichern des Speicherkontos des vom Benutzer angegebenen Abonnements.
$Script:originalStorageAccountOfUserSpecifiedSubscription = $null

# Eine Variable zum Speichern des Abonnementnamens.
$Script:userSpecifiedSubscription = $null


<#
.SYNOPSIS
Stellt einer Meldung das Datum und die Uhrzeit voran.

.DESCRIPTION
Stellt einer Meldung das Datum und die Uhrzeit voran. Diese Funktion ist für Meldungen vorgesehen, die an die Streams Error und Verbose geschrieben werden.

.PARAMETER  Message
Dient zum Angeben der Meldungen ohne Datum.

.INPUTS
System.String

.OUTPUTS
System.String

.EXAMPLE
PS C:\> Format-DevTestMessageWithTime -Message "Hinzufügen der Datei $filename zum Verzeichnis"
2/5/2014 1:03:08 PM - Hinzufügen der Datei $filename zum Verzeichnis

.LINK
Write-VerboseWithTime

.LINK
Write-ErrorWithTime
#>
function Format-DevTestMessageWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    return ((Get-Date -Format G)  + ' - ' + $Message)
}


<#

.SYNOPSIS
Schreibt eine Fehlermeldung mit der aktuellen Zeit als Präfix.

.DESCRIPTION
Schreibt eine Fehlermeldung mit der aktuellen Zeit als Präfix. Diese Funktion ruft die Format-DevTestMessageWithTime-Funktion auf, um vor dem Schreiben einer Meldung an den Error-Stream die Zeit voranzustellen.

.PARAMETER  Message
Dient zum Angeben der Meldung im Fehlermeldungsaufruf. Die Meldungszeichenfolge kann an die Funktion weitergeleitet werden.

.INPUTS
System.String

.OUTPUTS
Keine. Die Funktion schreibt in den Error-Stream.

.EXAMPLE
PS C:> Write-ErrorWithTime -Message "Failed. Cannot find the file."

Write-Error: 2/6/2014 8:37:29 AM - Failed. Cannot find the file.
 + CategoryInfo     : NotSpecified: (:) [Write-Error], WriteErrorException
 + FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorException

.LINK
Write-Error

#>
function Write-ErrorWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    $Message | Format-DevTestMessageWithTime | Write-Error
}


<#
.SYNOPSIS
Schreibt eine ausführliche Meldung mit der aktuellen Zeit als Präfix.

.DESCRIPTION
Schreibt eine ausführliche meldung mit der aktuellen Zeit als Präfix. Durch den Aufruf von Write-Verbose wird die Meldung nur angezeigt, wenn das Skript mit dem Verbose-Parameter ausgeführt wird oder wenn die VerbosePreference-Einstellung auf Continue festgelegt ist.

.PARAMETER  Message
Dient zum Angeben der Meldung im ausführlichen Meldungsaufruf. Die Meldungszeichenfolge kann an die Funktion weitergeleitet werden.

.INPUTS
System.String

.OUTPUTS
Keine. Die Funktion schreibt in den Verbose-Stream.

.EXAMPLE
PS C:> Write-VerboseWithTime -Message "The operation succeeded."
PS C:>
PS C:\> Write-VerboseWithTime -Message "The operation succeeded." -Verbose
VERBOSE: 1/27/2014 11:02:37 AM - The operation succeeded.

.EXAMPLE
PS C:\ps-test> "The operation succeeded." | Write-VerboseWithTime -Verbose
VERBOSE: 1/27/2014 11:01:38 AM - The operation succeeded.

.LINK
Write-Verbose
#>
function Write-VerboseWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    $Message | Format-DevTestMessageWithTime | Write-Verbose
}


<#
.SYNOPSIS
Schreibt eine Host-Meldung mit der aktuellen Zeit als Präfix.

.DESCRIPTION
Diese Funktion schreibt eine Meldung an das Hostprogramm (Write-Host) mit der aktuellen Zeit als Präfix. Die Auswirkungen des Schreibvorgangs an das Hostprogramm variieren. Die meisten Programme, die Windows PowerShell hosten, schreiben solche Meldungen an die Standardausgabe.

.PARAMETER  Message
Dient zum Angeben der Basismeldung ohne Datum. Die Meldungszeichenfolge kann an die Funktion weitergeleitet werden.

.INPUTS
System.String

.OUTPUTS
Keine. Die Funktion schreibt die Meldung an das Hostprogramm.

.EXAMPLE
PS C:> Write-HostWithTime -Message "Der Vorgang war erfolgreich."
1/27/2014 11:02:37 AM - Der Vorgang war erfolgreich.

.LINK
Write-Host
#>
function Write-HostWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )
    
    if ((Get-Variable SendHostMessagesToOutput -Scope Global -ErrorAction SilentlyContinue) -and $Global:SendHostMessagesToOutput)
    {
        if (!(Get-Variable -Scope Global AzureWebAppPublishOutput -ErrorAction SilentlyContinue) -or !$Global:AzureWebAppPublishOutput)
        {
            New-Variable -Name AzureWebAppPublishOutput -Value @() -Scope Global -Force
        }

        $Global:AzureWebAppPublishOutput += $Message | Format-DevTestMessageWithTime
    }
    else 
    {
        $Message | Format-DevTestMessageWithTime | Write-Host
    }
}


<#
.SYNOPSIS
Gibt $true zurück, wenn eine Eigenschaft oder Methode Mitglied des Objekts ist. Andernfalls wird $false zurückgegeben.

.DESCRIPTION
Gibt $true zurück, wenn die Eigenschaft oder Methode ein Mitglied des Objekts ist. Für statische Methoden der Klasse und für Ansichten wie PSBase und PSObject gibt die Funktion $false zurück.

.PARAMETER  Object
Dient zum Angeben des Objekts im Test. Geben Sie eine Variable ein, die ein Objekt oder einen Ausdruck enthält, der ein Objekt zurückgibt. Sie können keine Typen (wie [DateTime]) angeben oder Objekte an diese Funktion weiterleiten.

.PARAMETER  Member
Dient zum Angeben des Namens der Eigenschaft oder Methode im Test. Wenn Sie eine Methode angeben, lassen Sie die Klammern nach dem Methodenamen weg.

.INPUTS
Keine. Diese Funktion akzeptiert keine Eingaben aus der Pipeline.

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Test-Member -Object (Get-Date) -Member DayOfWeek
True

.EXAMPLE
PS C:\> $date = Get-Date
PS C:\> Test-Member -Object $date -Member AddDays
True

.EXAMPLE
PS C:\> [DateTime]::IsLeapYear((Get-Date).Year)
True
PS C:\> Test-Member -Object (Get-Date) -Member IsLeapYear
False

.LINK
Get-Member
#>
function Test-Member
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Object,

        [Parameter(Mandatory = $true)]
        [String]
        $Member
    )

    return $null -ne ($Object | Get-Member -Name $Member)
}


<#
.SYNOPSIS
Gibt $true zurück, wenn das Azure-Modul mindestens die Version 0.7.4 aufweist. Andernfalls wird $false zurückgegeben.

.DESCRIPTION
Test-AzureModuleVersion gibt $true zurück, wenn das Azure-Modul mindestens die Version 0.7.4 aufweist. Wenn das Modul nicht installiert ist oder eine niedrigere Versionsnummer aufweist, wird $false zurückgegeben. Diese Funktion hat keine Parameter.

.INPUTS
Keine

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Get-Module Azure -ListAvailable
PS C:\> #No module
PS C:\> Test-AzureModuleVersion
False

.EXAMPLE
PS C:\> (Get-Module Azure -ListAvailable).Version

Major  Minor  Build  Revision
-----  -----  -----  --------
0      7      4      -1

PS C:\> Test-AzureModuleVersion
True

.LINK
Get-Module

.LINK
PSModuleInfo object (http://msdn.microsoft.com/en-us/library/system.management.automation.psmoduleinfo(v=vs.85).aspx)
#>
function Test-AzureModuleVersion
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateNotNull()]
        [System.Version]
        $Version
    )

    return ($Version.Major -gt 0) -or ($Version.Minor -gt 7) -or ($Version.Minor -eq 7 -and $Version.Build -ge 4)
}


<#
.SYNOPSIS
Gibt $true zurück, wenn mindestens Version 0.7.4 des Azure-Moduls installiert ist.

.DESCRIPTION
Test-AzureModule gibt $true zurück, wenn die installierte Version des Azure-Moduls 0.7.4 oder höher ist. Es wird $false zurückgegeben, wenn das Modul nicht installiert ist oder eine frühere Version aufweist. Diese Funktion besitzt keine Parameter.

.INPUTS
Keine

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Get-Module Azure -ListAvailable
PS C:\> #No module
PS C:\> Test-AzureModule
False

.EXAMPLE
PS C:\> (Get-Module Azure -ListAvailable).Version

Major  Minor  Build  Revision
-----  -----  -----  --------
    0      7      4      -1

PS C:\> Test-AzureModule
True

.LINK
Get-Module

.LINK
PSModuleInfo object (http://msdn.microsoft.com/en-us/library/system.management.automation.psmoduleinfo(v=vs.85).aspx)
#>
function Test-AzureModule
{
    [CmdletBinding()]

    $module = Get-Module -Name Azure

    if (!$module)
    {
        $module = Get-Module -Name Azure -ListAvailable

        if (!$module -or !(Test-AzureModuleVersion $module.Version))
        {
            return $false;
        }
        else
        {
            $ErrorActionPreference = 'Continue'
            Import-Module -Name Azure -Global -Verbose:$false
            $ErrorActionPreference = 'Stop'

            return $true
        }
    }
    else
    {
        return (Test-AzureModuleVersion $module.Version)
    }
}


<#
.SYNOPSIS
Speichert das aktuelle Microsoft Azure-Abonnement in der $Script:originalSubscription-Variablen im Skriptbereich.

.DESCRIPTION
Die Backup-Subscription-Funktion speichert das aktuelle Microsoft Azure-Abonnement (Get-AzureSubscription -Current) und das zugehörige Speicherkonto sowie das von diesem Skript ($UserSpecifiedSubscription) geänderte Abonnement und das zugehörige Speicherkonto im Skriptbereich. Das Speichern der Werte bietet Ihnen die Möglichkeit, das ursprüngliche aktuelle Abonnement und Speicherkonto mithilfe einer Funktion wie Restore-Subscription auf den aktuellen Status wiederherzustellen, falls sich der aktuelle Status geändert hat.

.PARAMETER UserSpecifiedSubscription
Dient zum Angeben des Namens des Abonnements, in dem die neuen Ressourcen erstellt und veröffentlicht werden. Die Funktion speichert die Namen des Abonnements und der zugehörigen Speicherkonten in einem Skriptbereich. Dieser Parameter muss angegeben werden.

.INPUTS
Keine

.OUTPUTS
Keine

.EXAMPLE
PS C:\> Backup-Subscription -UserSpecifiedSubscription Contoso
PS C:\>

.EXAMPLE
PS C:\> Backup-Subscription -UserSpecifiedSubscription Contoso -Verbose
VERBOSE: Backup-Subscription: Start
VERBOSE: Backup-Subscription: Original subscription is Microsoft Azure MSDN - Visual Studio Ultimate
VERBOSE: Backup-Subscription: End
#>
function Backup-Subscription
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]
        $UserSpecifiedSubscription
    )

    Write-VerboseWithTime 'Backup-Subscription: Start'

    $Script:originalCurrentSubscription = Get-AzureSubscription -Current -ErrorAction SilentlyContinue
    if ($Script:originalCurrentSubscription)
    {
        Write-VerboseWithTime ('Backup-Subscription: Ursprüngliches Abonnement: ' + $Script:originalCurrentSubscription.SubscriptionName)
        $Script:originalCurrentStorageAccount = $Script:originalCurrentSubscription.CurrentStorageAccountName
    }
    
    $Script:userSpecifiedSubscription = $UserSpecifiedSubscription
    if ($Script:userSpecifiedSubscription)
    {        
        $userSubscription = Get-AzureSubscription -SubscriptionName $Script:userSpecifiedSubscription -ErrorAction SilentlyContinue
        if ($userSubscription)
        {
            $Script:originalStorageAccountOfUserSpecifiedSubscription = $userSubscription.CurrentStorageAccountName
        }        
    }

    Write-VerboseWithTime 'Backup-Subscription: Ende'
}


<#
.SYNOPSIS
Stellt für das Microsoft Azure-Abonnement, das in der $Script:originalSubscription-Variablen im Skriptbereich gespeichert ist, den aktuellen Status wieder her.

.DESCRIPTION
Die Restore-Subscription-Funktion macht das in der $Script:originalSubscription-Variablen gespeicherte Abonnement (erneut) zum aktuellen Abonnement. Falls das ursprüngliche Abonnement über ein Speicherkonto verfügt, macht diese Funktion dieses Speicherkonto für das aktuelle Abonnement zum aktuellen Speicherkonto. Die Funktion stellt das Abonnement nur wieder her, wenn in der Umgebung eine $SubscriptionName-Variable vorhanden ist, die nicht NULL ist. Andernfalls wird sie beendet. Wenn die $SubscriptionName-Variable einen Wert aufweist, $Script:originalSubscription jedoch $null ist, verwendet Restore-Subscription das Select-AzureSubscription-Cmdlet, um die Einstellungen "Aktuell" und "Standard" für Abonnements in Microsoft Azure PowerShell zu löschen. Diese Funktion besitzt keine Parameter, akzeptiert keine Eingaben und gibt nichts zurück (leer). Sie können -Verbose verwenden, um Meldungen an den Verbose-Stream zu schreiben.

.INPUTS
Keine

.OUTPUTS
Keine

.EXAMPLE
PS C:\> Restore-Subscription
PS C:\>

.EXAMPLE
PS C:\> Restore-Subscription -Verbose
VERBOSE: Restore-Subscription: Start
VERBOSE: Restore-Subscription: End
#>
function Restore-Subscription
{
    [CmdletBinding()]
    param()

    Write-VerboseWithTime 'Restore-Subscription: Start'

    if ($Script:originalCurrentSubscription)
    {
        if ($Script:originalCurrentStorageAccount)
        {
            Set-AzureSubscription `
                -SubscriptionName $Script:originalCurrentSubscription.SubscriptionName `
                -CurrentStorageAccountName $Script:originalCurrentStorageAccount
        }

        Select-AzureSubscription -SubscriptionName $Script:originalCurrentSubscription.SubscriptionName
    }
    else 
    {
        Select-AzureSubscription -NoCurrent
        Select-AzureSubscription -NoDefault
    }
    
    if ($Script:userSpecifiedSubscription -and $Script:originalStorageAccountOfUserSpecifiedSubscription)
    {
        Set-AzureSubscription `
            -SubscriptionName $Script:userSpecifiedSubscription `
            -CurrentStorageAccountName $Script:originalStorageAccountOfUserSpecifiedSubscription
    }

    Write-VerboseWithTime 'Restore-Subscription: Ende'
}


<#
.SYNOPSIS
Überprüft die Konfigurationsdatei und gibt eine Hash-Tabelle mit Konfigurationsdateiwerten zurück.

.DESCRIPTION
Die Read-ConfigFile-Funktion prüft die JSON-Konfigurationsdatei und gibt eine Hash-Tabelle mit ausgewählten Werten zurück.
-- Es beginnt mit dem Konvertieren der JSON-Datei in ein PSCustomObject. Die Hashtabelle der Website weist die folgenden Schlüssel auf:
-- Location: Ort der Website
-- Databases: SQL-Datenbanken der Website

.PARAMETER  ConfigurationFile
Dient zum Angeben von Pfad und Name der JSON-Konfigurationsdatei für Ihr Webprojekt. Visual Studio generiert die JSON-Datei automatisch, wenn Sie ein Webprojekt erstellen, und speichert sie im PublishScripts-Ordner Ihrer Lösung.

.PARAMETER HasWebDeployPackage
Gibt an, dass eine Web Deploy-Paket (ZIP-Datei) für die Webanwendung vorhanden ist. Sie geben den Wert '$true' an, indem Sie -HasWebDeployPackage oder HasWebDeployPackage:$true verwenden. Sie geben den Wert 'false' an, indem Sie HasWebDeployPackage:$false verwenden. Dieser Parameter muss angegeben werden.

.INPUTS
Keine. An diese Funktion können keine Eingaben weitergeleitet werden.

.OUTPUTS
System.Collections.Hashtable

.EXAMPLE
PS C:\> Read-ConfigFile -ConfigurationFile <path> -HasWebDeployPackage


Name                           Value                                                                                                                                                                     
----                           -----                                                                                                                                                                     
databases                      {@{connectionStringName=; databaseName=; serverName=; user=; password=}}                                                                                                  
website                        @{name="mysite"; location="West US";}                                                      
#>
function Read-ConfigFile
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $ConfigurationFile
    )

    Write-VerboseWithTime 'Read-ConfigFile: Start'

    # Inhalte der JSON-Datei (-raw ignoriert Zeilenumbrüche) abrufen und in ein PSCustomObject konvertieren
    $config = Get-Content $ConfigurationFile -Raw | ConvertFrom-Json

    if (!$config)
    {
        throw ('Read-ConfigFile: Fehler bei ConvertFrom-Json: ' + $error[0])
    }

    # Festlegen, ob das environmentSettings-Objekt 'webSite'-Eigenschaften aufweist (ungeachtet des Eigenschaftenwerts)
    $hasWebsiteProperty =  Test-Member -Object $config.environmentSettings -Member 'webSite'

    if (!$hasWebsiteProperty)
    {
        throw 'Read-ConfigFile: Die Konfigurationsdatei weist keine webSite-Eigenschaft auf.'
    }

    # Hash-Tabelle aus den Werten im PSCustomObject erstellen
    $returnObject = New-Object -TypeName Hashtable

    $returnObject.Add('name', $config.environmentSettings.webSite.name)
    $returnObject.Add('location', $config.environmentSettings.webSite.location)

    if (Test-Member -Object $config.environmentSettings -Member 'databases')
    {
        $returnObject.Add('databases', $config.environmentSettings.databases)
    }

    Write-VerboseWithTime 'Read-ConfigFile: Ende'

    return $returnObject
}


<#
.SYNOPSIS
Erstellt eine Microsoft Azure-Website.

.DESCRIPTION
Erstellt eine Microsoft Azure-Website mit dem spezifischen Namen und Ort. Diese Funktion ruft das New-AzureWebsite-Cmdlet im Azure-Modul auf. Falls das Abonnement noch keine Website mit dem angegebenen Namen aufweist, erstellt diese Funktion die Website und gibt ein Websiteobjekt zurück. Andernfalls gibt es die vorhandene Website zurück.

.PARAMETER  Name
Dient zum Angeben eines Namens für die neue Website. Der Name muss in Microsoft Azure eindeutig sein. Dieser Parameter muss angegeben werden.

.PARAMETER  Location
Dient zum Angeben des Orts der Website. Gültige Werte sind Microsoft Azure-Orte wie "USA West". Dieser Parameter muss angegeben werden.

.INPUTS
KEINE.

.OUTPUTS
Microsoft.WindowsAzure.Commands.Utilities.Websites.Services.WebEntities.Site

.EXAMPLE
Add-AzureWebsite -Name TestSite -Location "West US"

Name       : contoso
State      : Running
Host Names : contoso.azurewebsites.net

.LINK
New-AzureWebsite
#>
function Add-AzureWebsite
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $Name,

        [Parameter(Mandatory = $true)]
        [String]
        $Location
    )

    Write-VerboseWithTime 'Add-AzureWebsite: Start'
    $website = Get-AzureWebsite -Name $Name -ErrorAction SilentlyContinue

    if ($website)
    {
        Write-HostWithTime ('Add-AzureWebsite: Eine vorhandene Website ' +
        $website.Name + ' wurde gefunden')
    }
    else
    {
        if (Test-AzureName -Website -Name $Name)
        {
            Write-ErrorWithTime ('Die Website {0} ist bereits vorhanden.' -f $Name)
        }
        else
        {
            $website = New-AzureWebsite -Name $Name -Location $Location
        }
    }

    $website | Out-String | Write-VerboseWithTime
    Write-VerboseWithTime 'Add-AzureWebsite: Ende'

    return $website
}

<#
.SYNOPSIS
Gibt $True zurück, wenn die URL absolut ist und das Schema 'https' lautet.

.DESCRIPTION
Die Test-HttpsUrl-Funktion konvertiert die eingegebene URL in ein System.Uri-Objekt. Gibt $True zurück, wenn die URL absolut (nicht relativ) ist und das Schema 'https' lautet. Wenn eine der Bedingungen nicht erfüllt ist oder die eingegebene Zeichenfolge nicht in eine URL konvertiert werden kann, wird $false zurückgegeben.

.PARAMETER Url
Dient zum Angeben der zu testenden URL. Geben Sie eine URL-Zeichenfolge ein.

.INPUTS
KEINE.

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\>$profile.publishUrl
waws-prod-bay-001.publish.azurewebsites.windows.net:443

PS C:\>Test-HttpsUrl -Url 'waws-prod-bay-001.publish.azurewebsites.windows.net:443'
False
#>
function Test-HttpsUrl
{

    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $Url
    )

    # Falls '$uri' nicht in ein System.Uri-Objekt konvertiert werden kann, wird von Test-HttpsUrl '$false' zurückgegeben.
    $uri = $Url -as [System.Uri]

    return $uri.IsAbsoluteUri -and $uri.Scheme -eq 'https'
}


<#
.SYNOPSIS
Erstellt eine Zeichenfolge, mit der Sie eine Verbindung mit einer Microsoft Azure-SQL-Datenbank herstellen können.

.DESCRIPTION
Die Get-AzureSQLDatabaseConnectionString-Funktion erstellt eine Verbindungszeichenfolge für die Herstellung einer Verbindung mit einer Microsoft Azure-SQL-Datenbank.

.PARAMETER  DatabaseServerName
Dient zum Angeben des Namens eines vorhandenen Datenbankservers im Microsoft Azure-Abonnement. Alle Microsoft Azure-SQL-Datenbanken müssen einem SQL-Datenbankserver zugeordnet werden. Verwenden Sie zum Abrufen des Servernamens das Get-AzureSqlDatabaseServer-Cmdlet (Azure-Modul). Dieser Parameter muss angegeben werden.

.PARAMETER  DatabaseName
Dient zum Angeben des Namens für die SQL-Datenbank. Hierbei kann es sich um eine vorhandene SQL-Datenbank oder um den Namen für eine neue SQL-Datenbank handeln. Dieser Parameter muss angegeben werden.

.PARAMETER  Username
Dient zum Angeben des Namens des SQL-Datenbankadministrators. Der Benutzername lautet $Username@DatabaseServerName. Dieser Parameter muss angegeben werden.

.PARAMETER  Password
Dient zum Angeben eines Kennworts für den SQL-Datenbankadministrator. Geben Sie ein Kennwort im Nur-Text-Format ein. Sichere Zeichenfolgen sind nicht zulässig. Dieser Parameter muss angegeben werden.

.INPUTS
Keine.

.OUTPUTS
System.String

.EXAMPLE
PS C:\> $ServerName = (Get-AzureSqlDatabaseServer).ServerName[0]
PS C:\> Get-AzureSQLDatabaseConnectionString -DatabaseServerName $ServerName `
        -DatabaseName 'testdb' -UserName 'admin'  -Password 'password'

Server=tcp:testserver.database.windows.net,1433;Database=testdb;User ID=admin@testserver;Password=password;Trusted_Connection=False;Encrypt=True;Connection Timeout=20;
#>
function Get-AzureSQLDatabaseConnectionString
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $DatabaseServerName,

        [Parameter(Mandatory = $true)]
        [String]
        $DatabaseName,

        [Parameter(Mandatory = $true)]
        [String]
        $UserName,

        [Parameter(Mandatory = $true)]
        [String]
        $Password
    )

    return ('Server=tcp:{0}.database.windows.net,1433;Database={1};' +
           'User ID={2}@{0};' +
           'Password={3};' +
           'Trusted_Connection=False;' +
           'Encrypt=True;' +
           'Connection Timeout=20;') `
           -f $DatabaseServerName, $DatabaseName, $UserName, $Password
}


<#
.SYNOPSIS
Erstellt Microsoft Azure-SQL-Datenbanken auf der Grundlage der Werte in der von Visual Studio generierten JSON-Konfigurationsdatei.

.DESCRIPTION
Die Add-AzureSQLDatabases-Funktion verwendet Informationen aus dem Datenbankbereich der JSON-Datei. Diese Funktion, Add-AzureSQLDatabases (Plural), ruft die Funktion Add-AzureSQLDatabase (Singular) für jede SQL-Datenbank in der JSON-Datei auf. Add-AzureSQLDatabase (Singular) ruft das New-AzureSqlDatabase-Cmdlet (Azure-Modul) auf, das die SQL-Datenbanken erstellt. Diese Funktion gibt kein Datenbankobjekt, sondern eine Hash-Tabelle mit Werten zurück, die zum Erstellen der Datenbanken verwendet wurden.

.PARAMETER DatabaseConfig
 Akzeptiert ein Array aus PSCustomObjects aus der JSON-Datei, die die Read-ConfigFile-Funktion zurückgibt, wenn die JSON-Datei eine Websiteeigenschaft besitzt. Enthält die environmentSettings.databases-Eigenschaften. Die Liste kann an diese Funktion weitergeleitet werden.
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases| where {$_.connectionStringName}
PS C:\> $DatabaseConfig
connectionStringName: Default Connection
databasename : TestDB1
edition   :
size     : 1
collation  : SQL_Latin1_General_CP1_CI_AS
servertype  : New SQL Database Server
servername  : r040tvt2gx
user     : dbuser
password   : Test.123
location   : West US

.PARAMETER  DatabaseServerPassword
Gibt das Kennwort für den SQL-Datenbankserveradministrator an. Geben Sie eine Hash-Tabelle mit Namens- und Kennwortschlüsseln an. Der Wert des Namens ist der Name des SQL-Datenbankservers. Der Wert des Kennworts ist das Administratorkennwort. Beispiel: @Name = "TestDB1"; Password = "password" Dieser Parameter ist optional. Falls Sie ihn auslassen oder der SQL-Datenbankservername nicht mit dem Wert der serverName-Eigenschaft des $DatabaseConfig-Objekts übereinstimmt, verwendet die Funktion die Password-Eigenschaft des $DatabaseConfig-Objekts für die SQL-Datenbank in der Verbindungszeichenfolge.

.PARAMETER CreateDatabase
Stellt sicher, dass Sie eine Datenbank erstellen möchten. Dieser Parameter ist optional.

.INPUTS
System.Collections.Hashtable[]

.OUTPUTS
System.Collections.Hashtable

.EXAMPLE
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases| where {$_.connectionStringName}
PS C:\> $DatabaseConfig | Add-AzureSQLDatabases

Name                           Value
----                           -----
ConnectionString               Server=tcp:testdb1.database.windows.net,1433;Database=testdb;User ID=admin@testdb1;Password=password;Trusted_Connection=False;Encrypt=True;Connection Timeout=20;
Name                           Default Connection
Type                           SQLAzure

.LINK
Get-AzureSQLDatabaseConnectionString

.LINK
Create-AzureSQLDatabase
#>
function Add-AzureSQLDatabases
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [PSCustomObject]
        $DatabaseConfig,

        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Hashtable[]]
        $DatabaseServerPassword,

        [Parameter(Mandatory = $false)]
        [Switch]
        $CreateDatabase = $false
    )

    begin
    {
        Write-VerboseWithTime 'Add-AzureSQLDatabases: Start'
    }
    process
    {
        Write-VerboseWithTime ('Add-AzureSQLDatabases: Erstellen ' + $DatabaseConfig.databaseName)

        if ($CreateDatabase)
        {
            # Erstellt eine neue SQL-Datenbank mit den DatabaseConfig-Werten (sofern noch keine vorhanden ist)
            # Die Befehlsausgabe wird unterdrückt.
            Add-AzureSQLDatabase -DatabaseConfig $DatabaseConfig | Out-Null
        }

        $serverPassword = $null
        if ($DatabaseServerPassword)
        {
            foreach ($credential in $DatabaseServerPassword)
            {
               if ($credential.Name -eq $DatabaseConfig.serverName)
               {
                   $serverPassword = $credential.password             
                   break
               }
            }               
        }

        if (!$serverPassword)
        {
            $serverPassword = $DatabaseConfig.password
        }

        return @{
            Name = $DatabaseConfig.connectionStringName;
            Type = 'SQLAzure';
            ConnectionString = Get-AzureSQLDatabaseConnectionString `
                -DatabaseServerName $DatabaseConfig.serverName `
                -DatabaseName $DatabaseConfig.databaseName `
                -UserName $DatabaseConfig.user `
                -Password $serverPassword }
    }
    end
    {
        Write-VerboseWithTime 'Add-AzureSQLDatabases: Ende'
    }
}


<#
.SYNOPSIS
Erstellt eine neue Microsoft Azure-SQL-Datenbank.

.DESCRIPTION
Die Add-AzureSQLDatabase-Funktion erstellt eine Microsoft Azure-SQL-Datenbank auf der Grundlage der Daten in der von Visual Studio generierten JSON-Konfigurationsdatei und gibt die neue Datenbank zurück. Falls das Abonnement bereits über eine SQL-Datenbank mit dem angegebenen Datenbanknamen auf dem angegebenen SQL-Datenbankserver verfügt, gibt die Funktion die vorhandene Datenbank zurück. Diese Funktion ruft das New-AzureSqlDatabase-Cmdlet (Azure-Modul) auf, das die SQL-Datenbank erstellt.

.PARAMETER DatabaseConfig
Akzeptiert ein PSCustomObject aus der JSON-Konfigurationsdatei, die die Read-ConfigFile-Funktion zurückgibt, wenn die JSON-Datei eine Websiteeigenschaft besitzt. Enthält die environmentSettings.databases-Eigenschaften. Das Objekt kann nicht an diese Funktion weitergeleitet werden. Visual Studio generiert eine JSON-Konfigurationsdatei für alle Webprojekte und speichert sie im PublishScripts-Ordner Ihrer Lösung.

.INPUTS
Keine. Diese Funktion akzeptiert keine Eingaben aus der Pipeline.

.OUTPUTS
Microsoft.WindowsAzure.Commands.SqlDatabase.Services.Server.Database

.EXAMPLE
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases | where connectionStringName
PS C:\> $DatabaseConfig

connectionStringName    : Default Connection
databasename : TestDB1
edition      :
size         : 1
collation    : SQL_Latin1_General_CP1_CI_AS
servertype   : New SQL Database Server
servername   : r040tvt2gx
user         : dbuser
password     : Test.123
location     : West US

PS C:\> Add-AzureSQLDatabase -DatabaseConfig $DatabaseConfig

.LINK
Add-AzureSQLDatabases

.LINK
New-AzureSQLDatabase
#>
function Add-AzureSQLDatabase
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateNotNull()]
        [Object]
        $DatabaseConfig
    )

    Write-VerboseWithTime 'Add-AzureSQLDatabase: Start'

    # Fehler, wenn der Parameterwert keine serverName-Eigenschaft aufweist oder die serverName-Eigenschaft keinen Wert enthält.
    if (-not (Test-Member $DatabaseConfig 'serverName') -or -not $DatabaseConfig.serverName)
    {
        throw 'Add-AzureSQLDatabase: Im DatabaseConfig-Wert fehlt der (erforderliche) Datenbankservername.'
    }

    # Fehler, wenn der Parameterwert keine databasename-Eigenschaft aufweist oder die databasename-Eigenschaft keinen Wert enthält.
    if (-not (Test-Member $DatabaseConfig 'databaseName') -or -not $DatabaseConfig.databaseName)
    {
        throw 'Add-AzureSQLDatabase: Im DatabaseConfig-Wert fehlt der (erforderliche) Datenbankname.'
    }

    $DbServer = $null

    if (Test-HttpsUrl $DatabaseConfig.serverName)
    {
        $absoluteDbServer = $DatabaseConfig.serverName -as [System.Uri]
        $subscription = Get-AzureSubscription -Current -ErrorAction SilentlyContinue

        if ($subscription -and $subscription.ServiceEndpoint -and $subscription.SubscriptionId)
        {
            $absoluteDbServerRegex = 'https:\/\/{0}\/{1}\/services\/sqlservers\/servers\/(.+)\.database\.windows\.net\/databases' -f `
                                     $subscription.serviceEndpoint.Host, $subscription.SubscriptionId

            if ($absoluteDbServer -match $absoluteDbServerRegex -and $Matches.Count -eq 2)
            {
                 $DbServer = $Matches[1]
            }
        }
    }

    if (!$DbServer)
    {
        $DbServer = $DatabaseConfig.serverName
    }

    $db = Get-AzureSqlDatabase -ServerName $DbServer -DatabaseName $DatabaseConfig.databaseName -ErrorAction SilentlyContinue

    if ($db)
    {
        Write-HostWithTime ('Create-AzureSQLDatabase: Vorhandene Datenbank wird verwendet' + $db.Name)
        $db | Out-String | Write-VerboseWithTime
    }
    else
    {
        $param = New-Object -TypeName Hashtable
        $param.Add('serverName', $DbServer)
        $param.Add('databaseName', $DatabaseConfig.databaseName)

        if ((Test-Member $DatabaseConfig 'size') -and $DatabaseConfig.size)
        {
            $param.Add('MaxSizeGB', $DatabaseConfig.size)
        }
        else
        {
            $param.Add('MaxSizeGB', 1)
        }

        # Falls das $DatabaseConfig-Objekt eine Sortiereigenschaft aufweist und nicht NULL oder leer ist
        if ((Test-Member $DatabaseConfig 'collation') -and $DatabaseConfig.collation)
        {
            $param.Add('Collation', $DatabaseConfig.collation)
        }

        # Falls das $DatabaseConfig-Objekt eine Bearbeitungseigenschaft aufweist und nicht NULL oder leer ist
        if ((Test-Member $DatabaseConfig 'edition') -and $DatabaseConfig.edition)
        {
            $param.Add('Edition', $DatabaseConfig.edition)
        }

        # Hash-Tabelle in Verbose-Stream schreiben
        $param | Out-String | Write-VerboseWithTime
        # 'New-AzureSqlDatabase' mit Verteilung aufrufen (Ausgabe unterdrücken)
        $db = New-AzureSqlDatabase @param
    }

    Write-VerboseWithTime 'Add-AzureSQLDatabase: Ende'
    return $db
}
