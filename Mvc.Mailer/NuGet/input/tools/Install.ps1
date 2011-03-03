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