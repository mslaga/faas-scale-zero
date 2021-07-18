#!/bin/sh
#set -x

for ARGUMENT in "$@"
do
    KEY=$(echo $ARGUMENT | cut -f1 -d=)
    VALUE=$(echo $ARGUMENT | cut -f2 -d=)   

    case "$KEY" in
        VERSION)      VERSION="${VALUE}" ;;
        DOCKER_REPO)   DOCKER_REPO="${VALUE}" ;; 
        *)   
    esac    
done

if [ -z "${VERSION}" -a -n "${DOCKER_REPO}" ]; then
  echo "VERSION parameter is required"
  exit 1
fi

archs[0]=linux-musl-x64
archs[1]=linux-musl-arm
archs[2]=linux-musl-arm64
archs[3]=linux-x64
archs[4]=linux-arm64
archs[5]=linux-arm

dockerArch[0]=amd64
dockerArch[1]=arm
dockerArch[2]=arm64

DOCKER_CREATE_MANIFEST_CURRENT="docker manifest create ${DOCKER_REPO}:${VERSION}"
DOCKER_CREATE_MANIFEST_LATEST="docker manifest create ${DOCKER_REPO}:latest"
for i in "${!archs[@]}";
do
  echo "Build ${archs[$i]}\n"
  cp DockerfileTemplate Dockerfile
  sed -i '' -e "s/#arch#/${archs[$i]}/g" Dockerfile
  dotnet publish --runtime ${archs[$i]}  --configuration Release /p:TargetFramework=netcoreapp5.0 /p:PublishSingleFile=true /p:PublishTrimmed=true

  if [ -z "${dockerArch[${i}]}" ]; then
    cp "./bin/Release/netcoreapp5.0/${archs[$i]}/publish/faasSzero" "../faasSzero-${archs[$i]}"
  fi

  if [ -n "${dockerArch[${i}]}" -a -n "${DOCKER_REPO}" ]; then
    TAG_NAME="${DOCKER_REPO}:manifest-${archs[$i]}-${VERSION}"
    docker build -t "${TAG_NAME}" --platform "linux/${dockerArch[${i}]}" .
    docker push "${TAG_NAME}"

    DOCKER_CREATE_MANIFEST_CURRENT="${DOCKER_CREATE_MANIFEST_CURRENT} --amend ${TAG_NAME}"
    DOCKER_CREATE_MANIFEST_LATEST="${DOCKER_CREATE_MANIFEST_LATEST} --amend ${TAG_NAME}"
  fi
done

if [ -n "${DOCKER_REPO}" ]; then
  eval "${DOCKER_CREATE_MANIFEST_CURRENT}"
  eval "${DOCKER_CREATE_MANIFEST_LATEST}"

  docker manifest push "${DOCKER_REPO}:${VERSION}"
  docker manifest push "${DOCKER_REPO}:latest"
fi