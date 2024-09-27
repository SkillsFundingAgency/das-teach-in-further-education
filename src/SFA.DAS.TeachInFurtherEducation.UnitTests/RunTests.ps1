Remove-Item -Path "./TestResults/*" -Recurse -Force

dotnet test /p:CollectCoverage=true /p:ExcludeByFile="**/Views/**/*.cshtml" /p:Exclude="[*]SFA.DAS.TeachInFurtherEducation.Web.Models.*%2c[*]SFA.DAS.TeachInFurtherEducation.Web.Data.Models.*"  /p:CoverletOutput='./TestResults/' /p:CoverletOutputFormat=cobertura

reportgenerator "-reports:./TestResults/**/coverage.cobertura.xml" "-targetdir:TestReport" -reporttypes:Html

reportgenerator "-reports:./TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults" -reporttypes:TextSummary

Get-Content .\TestResults\Summary.txt

$reportPath = "file:///$($pwd.Path.Replace('\', '/'))/TestReport/index.htm"
Write-Host "Test report is available at: $reportPath"
