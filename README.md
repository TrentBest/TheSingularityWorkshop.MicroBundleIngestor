# TheSingularityWorkshop.MicroBundleIngestor

The Forge needs a way to turn an external capability into something an Experience can understand.

This repository is the ingestion boundary.

## First target

Give the ingestor a URL containing an OpenAPI/Swagger JSON document.

URL
  -> MicroBundleIngestor
  -> canonical API Experience definition
  -> Forge
  -> Experience GUI and request composition
  -> external REST API
  -> dynamic result

The ingestor does not hard-code a GUI for a particular API. It captures the information Forge needs to construct one.

For example, a places API can become an Experience that asks the player for a location and category, composes the request, calls the API, and presents the returned places.

The same API description can later support a second path:

API definition
  -> request
  -> mesh / terrain / place data
  -> MicroBundle cache
  -> Experience

The API contract is reusable capability metadata. Returned world data is a potentially cacheable MicroBundle payload.

## REST repository boundary

The project references TheSingularityWorkshop.MicroBundleRepository.Rest because the eventual publication path is:

Forge / Ingestor
  -> IMicroBundleRepository
  -> REST repository adapter
  -> MicroBundle Repository

The ingestor should never know whether the repository is backed by Azure, another service, or something else.

## Current MVP

The current command accepts one OpenAPI/Swagger JSON URL and emits the canonical API Experience definition as JSON.

No live repository publication is performed by this MVP.

## Architecture direction

Forge remains the authoring environment. This repository is an importer/tooling boundary, not a replacement for Forge.

The eventual pipeline is:

external API description
  -> ingestion
  -> API capability MicroBundle definition
  -> Forge
  -> Experience that presents the appropriate GUI
  -> API invocation at runtime

For APIs that return expensive or stable geometry, terrain, meshes, or other world data, Forge can also define a MicroBundle-producing path so the result can become reusable content rather than being fetched repeatedly.
