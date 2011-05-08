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

function AddMethodToMailerInterface
{
	param($MailerName, $MethodName)
}


function AddMethodToMailer
{
	param($MailerName, $MethodName, $MethodTemplate)

	$defaultNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value
	$mailerFullName = "$defaultNamespace.Mailers.$MailerName"

	$codeClass = Get-ProjectType $mailerFullName -Project $Project

	if($codeClass)
	{
		Add-ClassMemberViaTemplate -Name $MethodName -CodeClass $codeClass -Template $MethodTemplate `
			-Model @{ MethodName = $MethodName } `
			-SuccessMessage "Added method $MethodName to $mailerFullName" `
			-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
	}
}

function AddMailerMethodsWithViews
{
	param($MailerName, $MethodNames, $Aspx, $WithText)

	foreach($methodName in $MethodNames)
	{
		#AddMethodToMailer	I$MailerName $methodName IMailerMethodTemplate
		AddMethodToMailer	$MailerName $methodName MailerMethodTemplate
	}

	CreateLayoutAndViews $WithText $Aspx
}

function RunMailerScaffolder
{
	param($Aspx)

	if($Add)
	{
		AddMailerMethodsWithViews $MailerName $MailerMethods $Aspx $WithText
	}
	else
	{
		CreateCSFiles $NoInterface
		CreateLayoutAndViews $WithText $Aspx
	}
}

######## END FUNCTIONS ####################################