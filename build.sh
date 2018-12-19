#!/usr/bin/env bash

echo "${APOLLO_DOCKER_PASSWORD}" | docker login -u "${APOLLO_DOCKER_USERNAME}" --password-stdin "${APOLLO_DOCKER_REGISTRY}"

if dotnet test ./tests/Apollo.Tests.Unit/
  then
    if docker build -t lykkecorp.azurecr.io/apollo:latest --rm . 
      then
        if [[ "$TRAVIS_BRANCH" = "master" ]]; then
          echo "Publishing apollo"
          docker push lykkecorp.azurecr.io/apollo:latest
        fi
    else
      echo "Failed to build docker image"
      exit 1
    fi
else
  echo "Unit tests failed"
  exit 1
fi