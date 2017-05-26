param (
    [Parameter(Mandatory=$True)]
    [string]$Text,

    [Parameter(Mandatory=$True)]
    [string]$Component,

	[Parameter(Mandatory=$True)]
    [string]$OutFile
)

$Enviroment = "DEV";
$MWLogPath="C:\inetpub\TSC.iBanking.GPB.MW.Host`$${Enviroment}\logs";


switch($Component)
{
	"MW" {
		$LogFile = Get-ChildItem -Path $MWLogPath | Sort-Object -Property Name -Descending | Select-Object -First 1
	}
	"IMW" {
	}
	"UI" {
	}
}


Select-String -Path $LogFile -Pattern $Text -SimpleMatch -Context 0,5 `
			| %{ "###{0,5}: {1} {2}" -f $_.LineNumber, $_.Line,  [system.String]::Join("`n" ,$_.Context.DisplayPostContext) } `
			| Out-File -FilePath $OutFile -Encoding utf8 -Force
