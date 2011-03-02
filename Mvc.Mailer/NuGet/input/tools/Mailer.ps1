[T4Scaffolding.Scaffolder(Description = "Scaffold your mailers")][CmdletBinding()]
param(
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$MailerName,        
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string[]]$MailerMethods,
    [switch]$Text = $false,
	[string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false
)

$script_dir = Split-Path -Parent $MyInvocation.MyCommand.Path
. "$script_dir\MailerFunctions.ps1"

CreateCSFiles
CreateLayoutAndViews($Text)