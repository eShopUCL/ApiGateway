#!/bin/zsh

# Define environment variables
ACR_NAME="eshopregistry"
RESOURCE_GROUP="eShopResourceGroup"
AKS_NAME="eShopCluster"
IMAGE_NAME="api-gateway"
YAML_FILE="api-gateway.yml"

# Step 1: Authenticate with Azure Container Registry
echo "Logging in to Azure Container Registry..."
az acr login --name ${ACR_NAME}

if [ $? -ne 0 ]; then
  echo "ERROR: Failed to log in to Azure Container Registry."
  exit 1
fi

# Step 2: Use Docker Buildx to Build the API Gateway Image
echo "Building Docker image for API Gateway (linux/amd64)..."
export DOCKER_BUILDKIT=1

# Remove the existing builder and recreate it to avoid append mode issues
docker buildx rm multiarch-builder || true
docker buildx create --use --name multiarch-builder --driver docker-container

# Build and push the API Gateway image to Azure Container Registry
docker buildx build --platform linux/amd64 \
  --tag ${ACR_NAME}.azurecr.io/${IMAGE_NAME}:latest \
  --file Dockerfile . --push

if [ $? -ne 0 ]; then
  echo "ERROR: Failed to build and push Docker image for API Gateway."
  exit 1
fi

# Step 3: Retrieve AKS Credentials
echo "Retrieving AKS credentials..."
az aks get-credentials --resource-group ${RESOURCE_GROUP} --name ${AKS_NAME} --overwrite-existing

if [ $? -ne 0 ]; then
  echo "ERROR: Failed to retrieve AKS credentials."
  exit 1
fi

# Step 4: Deploy and Restart API Gateway in AKS
if [ -f ${YAML_FILE} ]; then
  echo "Deploying API Gateway using ${YAML_FILE}..."
  kubectl apply -f ${YAML_FILE}
  if [ $? -ne 0 ]; then
    echo "ERROR: Failed to deploy API Gateway using ${YAML_FILE}."
    exit 1
  fi
else
  echo "ERROR: ${YAML_FILE} does not exist. Please ensure the file is available."
  exit 1
fi

# Step 5: Restart the Deployment to Apply Changes Immediately
echo "Restarting the API Gateway deployment to apply changes..."
kubectl rollout restart deployment api-gateway

if [ $? -ne 0 ]; then
  echo "ERROR: Failed to restart API Gateway deployment."
  exit 1
fi

echo "API Gateway deployment and restart completed successfully!"
