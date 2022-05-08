# $tenant_id = "<tenant_id>"
# $subscription_id = "<subscription_id>"
# az login --tenant $tenant_id
# az account set subscription-id $subscription_id

$bot_name="lynx-bot"
$id_name = "$bot_name-id"
$rg_name = "lynx-rg"
$location = "westeurope"

az group create --name $rg_name --location $location
$identity_id = az identity create --resource-group $rg_name --name $id_name --query clientId

az deployment group create `
    --resource-group $rg_name `
    --template-file "DeploymentTemplates/template-with-preexisting-rg.json" `
    --parameters `
        appType="UserAssignedMSI" `
        appId=$identity_id `
        existingUserAssignedMSIName=$id_name `
        existingUserAssignedMSIResourceGroupName=$rg_name `
        botId="$bot_name-bot" `
        botSku="F0" `
        newWebAppName="$bot_name-app" `
        newAppServicePlanName="$bot_name-sp" `
        appServicePlanLocation=$location `
    --name "$bot_name-deployment"

# az deployment sub create `
#     --resource-group $rg_name `
#     --template-file "DeploymentTemplates/template-with-preexisting-rg.json" `
#     --location $location `
#     --parameters `
#         appType="MultiTenant" `
#         appId="" `
#         appSecret="" `
#         botId="$bot_name-bot" `
#         botSku=F1 `
#         newWebAppName="$bot_name-app" `
#         newAppServicePlanName="$bot_name-sp" `
#         groupName=$rg_name `
#         groupLocation=$location   `
#         newAppServicePlanLocation=$location `
#     --name "$bot_name-deployment-2"

