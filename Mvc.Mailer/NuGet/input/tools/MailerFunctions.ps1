###########################################################
###														###
### FUNCTIONS											###
###														###
###########################################################
function CreateCSFileFromTemplate
{
	param ($MailerTemplate, $outputPath, $NoInterface)

	$outputFolder = "Mailers"
	
	$isInterface = $true
	if($NoInterface)
	{
		$isInterface = $false
	}


	$templateFile = Find-ScaffolderTemplate cs\$MailerTemplate -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -ErrorIfNotFound
	if ($templateFile) {
		# Render it, adding the output to the Visual Studio project
		$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value
		$wroteFile = Invoke-ScaffoldTemplate -Template $templateFile -Model @{ 
						Namespace = $namespace; 
						MailerName = $MailerName;
						MailerMethods = $MailerMethods;
						Interface = $isInterface;
						} -Project $Project -OutputPath $outputFolder\$outputPath -Force:$Force 
		if($wroteFile) {
			Write-Host "Added MvcMailer output '$wroteFile'"
		}
	}
}

function CreateCSFiles
{
	param($NoInterface)

	if(! $NoInterface)
	{
		CreateCSFileFromTemplate IMailerTemplate  I$MailerName $Interface
	}
	CreateCSFileFromTemplate MailerTemplate  $MailerName $NoInterface
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
	param($Text, $Aspx)

	
	$viewExtension = ''
	$masterExtension = ''

	if($Aspx){
		$viewExtension = "aspx"
		$masterExtension = "Master"
	}
	else
	{
		$viewExtension = "cshtml"
		$masterExtension = "cshtml"
	}

	# HTML Views
	CreateViewFileFromTemplate Layout.$masterExtension Views\$MailerName\_Layout
	foreach ($viewName in $MailerMethods)
	{
		CreateViewFileFromTemplate Mail.$viewExtension Views\$MailerName\$viewName $viewName	
	}

	# Text Views
	if($Text)
	{
		CreateViewFileFromTemplate Layout.text.$masterExtension Views\$MailerName\_Layout
		foreach ($viewName in $MailerMethods)
		{
			CreateViewFileFromTemplate Mail.text.$viewExtension Views\$MailerName\$viewName $viewName	
		}
	}
}


#function DeleteMailer
#{
#	param($Project, $MailerName)
#
#	Remove-Item $Project\Mailers\$MailerName.cs
#	Remove-Item $Project\Mailers\I$MailerName.cs
#	Remove-Item $Project\Views\$MailerName\*
#}
#
#function DeleteMailers
#{
#	param($Project, $MailerNames)
#
#	foreach($mailerName in $MailerNames)
#	{
#		DeleteMailer($Project, $mailerName)
#	}
#}
######## END FUNCTIONS ####################################