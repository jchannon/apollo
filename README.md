# Apollo

Apollo is an Lykke KYC API that can be called to verify customer details for example, email and telephone.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

There are user secrets that has to be defined in order configuration to be complete:

```json
{
  "IdentityServer": {
    "ApiSecret": "<api secret goes here>"
  },
  "Db": {
    "DataConnString": "<azure storage account connection string>"
  }
}
```

## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/LykkeCorp/apollo/tags). 

