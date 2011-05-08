param($rootPath, $toolsPath, $package, $project)

### Copied from MvcScaffolding
function CountSolutionFilesByExtension($extension) {
	$files = (Get-Project).DTE.Solution `
		| ?{ $_.FileName } `
		| %{ [System.IO.Path]::GetDirectoryName($_.FileName) } `
		| %{ [System.IO.Directory]::EnumerateFiles($_, "*." + $extension, [System.IO.SearchOption]::AllDirectories) }
	($files | Measure-Object).Count
}

function InferPreferredViewEngine() {
	# Assume you want Razor except if you already have some ASPX views and no Razor ones
	if ((CountSolutionFilesByExtension aspx) -eq 0) { return "razor" }
	if (((CountSolutionFilesByExtension cshtml) -gt 0) -or ((CountSolutionFilesByExtension vbhtml) -gt 0)) { return "razor" }
	return "aspx"
}

# Infer which view engine you're using based on the files in your project
### End copied

$mailerScaffolder = if ([string](InferPreferredViewEngine) -eq 'aspx') { "Mailer.Aspx" } else { "Mailer.Razor" }
Set-DefaultScaffolder -Name Mailer -Scaffolder $mailerScaffolder -SolutionWide -DoNotOverwriteExistingSetting

Write-Host 
Write-Host 

Write-Host ---------------------------READ ME---------------------------------------------------
Write-Host 
Write-Host Your default Mailer Scaffolder is set to $mailerScaffolder
Write-Host
Write-Host You can generate your Mailers and Views using the following Scaffolder Command
Write-Host
Write-Host "PM> Scaffold Mailer UserMailer Welcome,GoodBye"
Write-Host
Write-Host Edit the smtp configuration at web.config file before you send an email
Write-Host
Write-Host You can find more at: https://github.com/smsohan/MvcMailer/wiki/MvcMailer-Step-by-Step-Guide
Write-Host 
Write-Host -------------------------------------------------------------------------------------