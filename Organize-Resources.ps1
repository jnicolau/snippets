[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True,Position=1)]
	[string]$path,
	[Parameter(Mandatory=$True,Position=2)]
	[string]$outputPath
)

$yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes"
$no = New-Object System.Management.Automation.Host.ChoiceDescription "&No"
$yesno = [System.Management.Automation.Host.ChoiceDescription[]]($yes,$no)

# List languages
$languageDirs = Get-ChildItem $path | ?{ $_.PSIsContainer }
$languageDirs

$result = $Host.UI.PromptForChoice("" + $languageDirs.Count + " languages found in the specified path", "Continue?", $yesno, 0)
if($result -eq 1) { Exit }

# List resources
$resources = Get-ChildItem $path | ?{ $_.PSIsContainer } | Select-Object -First 1 | Get-ChildItem
$resources

$result = $Host.UI.PromptForChoice("" + $resources.Count + " resources found", "Continue?", $yesno, 0)
if($result -eq 1) { Exit }

# Check if outputPath exists and create it if it doesn't
if(!(Test-Path -Path $outputPath)){
	$result = $Host.UI.PromptForChoice("The outputPath '" + $outputPath + "' doesn't exist yet. I'm going to create it.", "Continue?", $yesno, 0)
	if($result -eq 1) { Exit }
    New-Item -ItemType directory -Path $outputPath
	"Directory created."
}

# List each folder per resource
" "
Get-ChildItem $path | ?{ $_.PSIsContainer } | Select-Object -First 1 `
	| Get-ChildItem | % {$_.BaseName}

$result = $Host.UI.PromptForChoice("I'm going to create these folders inside the outputPath.", "Continue?", $yesno, 0)
if($result -eq 1) { Exit }

# Create each resource folder inside outputPath
$newFolders = Get-ChildItem $path | ?{ $_.PSIsContainer } | Select-Object -First 1 `
	| Get-ChildItem | % {$outputPath + "\" + $_.BaseName}
ForEach ($newFolder In $newFolders) {
	if(!(Test-Path -Path $newFolder)){
    	New-Item -ItemType directory -Path $newFolder
	}
}
"All folders created."
" "

# Copy each resource for each language
ForEach ($languageDir In $languageDirs) {
	$result = $Host.UI.PromptForChoice("I'm going to process language " + $languageDir, "Continue?", $yesno, 0)
	if($result -eq 1) { Exit }
	ForEach ($file In ($languageDir | Get-ChildItem)) {
		$newFileDestination = $outputPath + "\" + ($file | % {$_.BaseName}) + "\" + ($file | % {$_.BaseName}) + "." + $languageDir + ".resx"
		"Copying '" + $file.FullName + "' to '" + $newFileDestination + "'"
		Copy-Item $file.FullName -Destination $newFileDestination
	}
	" "
}

