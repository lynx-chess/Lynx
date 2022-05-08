# $tenant_id = "<tenant_id>"
# $subscription_id = "<subscription_id>"
# az login --tenant $tenant_id
# az account set subscription-id $subscription_id

$bot_name = "lynx-teams-bot"
$rg_name = "lynx-rg"
$zip_name = "Lynx.TeamsApi.zip"

if (Test-Path .deployment) {
    remove-item .deployment
}

dotnet tool update -g dotnet-combine
# dotnet-combine zip .. -o ./$zip_name
az bot prepare-deploy --lang Csharp --code-dir "." --proj-file-path "../Lynx.TeamsApi.csproj"
az webapp deployment source config-zip --resource-group $rg_name --name "$bot_name-app" --src "./$zip_name"