[T4Scaffolding.Scaffolder(Description = "Scaffold your mailers using Razor Views")][CmdletBinding()]
param(
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$MailerName,        
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string[]]$MailerMethods,
    [switch]$WithText = $false,
	[switch]$NoInterface = $false,
	[switch]$Add = $false,
	[string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false
)

$script_dir = Split-Path -Parent $MyInvocation.MyCommand.Path
. "$script_dir\MailerFunctions.ps1"

RunMailerScaffolder $false