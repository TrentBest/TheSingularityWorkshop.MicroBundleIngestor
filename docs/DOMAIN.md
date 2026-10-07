# MicroBundle Ingestor Domain

The Ingestor is the boundary between an external capability and Forge.

The important output is not an HTTP document. It is a capability definition that can become a MicroBundle and can therefore be supplied to an Experience through ports.

The domain separates:

1. Source description — how a capability was discovered.
2. Capability definition — what the capability can do and what data it requires or produces.
3. Experience composition — how Forge presents and wires that capability.

## A capability is a callable contract

A capability exposes operations. Each operation exposes ports.

~~~
Experience
   |
   +-- user/location data -----> latitude port
   +-- user/category data -----> category port
   |
   v
+-----------------------------+
|       API MicroBundle       |
|                             |
|  findPlaces operation       |
|     ^ inputs                |
|     |                       |
|     v outputs               |
+-----------------------------+
   |
   v
returned capability data
~~~

The Experience does not need to know how the remote service is implemented.

## Multiple API paths

### 1. Capability MicroBundle

The API itself becomes a reusable MicroBundle.

~~~
Add API MicroBundle
      |
      v
provide required data
      |
      v
connect data to ports
      |
      v
invoke operation
      |
      v
API call occurs on behalf of the Experience
~~~

### 2. Content-producing API

An API may return expensive or durable content.

~~~
API capability
      |
      v
request
      |
      v
terrain / mesh / places / world data
      |
      v
content MicroBundle
      |
      v
reusable Experience content
~~~

The API contract and returned content are different artifacts.

### 3. Repository capability

The MicroBundle Repository is deliberately a proving case.

Its conceptual API is:

~~~
Get(bundleId, version, contentHash)
Put(artifact)
~~~

Forge can consume this capability to publish MicroBundles.

The intended authorization distinction is:

~~~
                 MicroBundle Repository
                         |
              +----------+----------+
              |                     |
             Get                    Put
              |                     |
       allowed consumers      Forge only
~~~

The capability definition can declare this policy, but policy metadata is not enforcement. The repository host must enforce that only the Forge identity can perform Put.

## Why ports matter

OpenAPI tells us about transport and schema. It does not fully determine the Experience.

For example:

- latitude + longitude may become one map/location control;
- an enum may become a selection control;
- a required numeric range may become a slider;
- an authentication requirement may become a credential binding.

Therefore:

~~~
OpenAPI / external contract
        |
        v
     Ingestor
        |
        v
capability + semantic hints
        |
        v
      Forge
        |
        +-- GUI choice
        +-- FSM behavior
        +-- port wiring
        |
        v
     Experience
~~~

The Ingestor supplies facts and hints. Forge remains responsible for composition.

## Determinism

A capability definition should be deterministic for the same source contract.

A future compiler can therefore produce:

~~~
Capability definition
      |
      v
deterministic MicroBundle artifact
      |
      v
BundleId + Version + ContentHash
~~~

The content hash identifies artifact bytes. Semantic identity identifies what the capability represents. These are deliberately different concerns.

## Security boundary

A MicroBundle describing a capability does not grant authority by itself.

The execution host must establish caller identity, allowed operations, credential ownership, resource limits, network policy, and write restrictions.

For the Repository, the intended rule is explicit:

> microbundle.put is a Forge-only capability.

Other Experiences may consume repository Get when appropriate, but they must not inherit Forge's publication authority.

## Non-goals

This repository does not:

- become Forge;
- become a GUI framework;
- become a general REST client;
- hard-code an Experience for one API;
- require a particular storage provider;
- equate an OpenAPI document with a finished MicroBundle.

Those responsibilities remain at their appropriate architectural boundaries.
