###########################################################
###														###
### FUNCTIONS											###
###														###
###########################################################
function CreateCSFileFromTemplate
{
	param ($MailerTemplate, $outputPath)

	$outputFolder = "Mailers"
	
	$templateFile = Find-ScaffolderTemplate cs\$MailerTemplate -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -ErrorIfNotFound
	if ($templateFile) {
		# Render it, adding the output to the Visual Studio project
		$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value
		$wroteFile = Invoke-ScaffoldTemplate -Template $templateFile -Model @{ 
						Namespace = $namespace; 
						MailerName = $MailerName;
						MailerMethods = $MailerMethods;
						} -Project $Project -OutputPath $outputFolder\$outputPath -Force:$Force 
		if($wroteFile) {
			Write-Host "Added MvcMailer output '$wroteFile'"
		}
	}
}

function CreateCSFiles
{
	CreateCSFileFromTemplate IMailerTemplate  I$MailerName
	CreateCSFileFromTemplate MailerTemplate  $MailerName
}

function CreateViewFileFromTemplate
{
	param ($ViewTemplate, $viewPath, $viewName='')
	
	$templateFile = Find-ScaffolderTemplate view\$ViewTemplate -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -ErrorIfNotFound
	$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

	if ($templateFile) {
		$wroteFile = Invoke-ScaffoldTemplate -Template $templateFile -Model @{
											Namespace = $namespace;
											MailerName=$MailerName;
											ViewName=$viewName;
											} -Project $Project -OutputPath $viewPath -Force:$force
		if($wroteFile) {
			Write-Host "Added MyScaffolder output '$wroteFile'"
		}
	}
}

function CreateLayoutAndViews
{
	CreateViewFileFromTemplate Layout.cshtml Views\$MailerName\_Layout
	foreach ($viewName in $MailerMethods)
	{
		CreateViewFileFromTemplate Mail.cshtml Views\$MailerName\$viewName $viewName	
	}
}

######## END FUNCTIONS ####################################