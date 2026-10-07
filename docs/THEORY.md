# Theory: Address Space, Capability Space, and Physical Cost

A large logical address space is not automatically an efficient implementation.

The central distinction is:

~~~
logical possibility
        !=
physical allocation
        !=
stored per-object metadata
        !=
runtime working set
~~~

An address-space design can expose an enormous number of possible locations while allocating physical structures only where data exists. Sparse data structures are a standard example of this separation.

For the Workshop ontology, the useful question is not:

> How many theoretical addresses can we advertise?

It is:

> How much information must be stored, traversed, hashed, cached, and executed to identify the things that actually exist?

## N-dimensional Workshop addressing

With N dimensions and a full 64-bit range in each dimension:

2^(64N)

For nine dimensions:

2^576 ≈ 10^173.4

For fifteen dimensions:

2^960 ≈ 10^289.0

Thus a system whose advertised capacity is approximately 10^272 is roughly comparable in scale to a fifteen-dimensional full-UInt64 address construction.

The Workshop does not need to win a maximum-number contest.

Adding dimensions solely to exceed another system's headline number would be the wrong optimization target. The better target is:

- compact representation;
- predictable lookup;
- cache locality;
- bounded working sets;
- sparse allocation;
- deterministic identity;
- cheap traversal of populated addresses.

## Address width versus address storage

Even when a logical address is mathematically large, it does not follow that every object must physically carry the entire logical coordinate.

Possible representations include:

- hierarchical coordinates;
- sparse nodes;
- compact local indices;
- hashes;
- tables of populated ranges;
- compressed prefixes;
- application-specific projections.

The implementation should pay for information that exists, not for the entire universe of information that could exist.

## Capability space

An external API may expose hundreds of operations, but an Experience may need only three.

The capability MicroBundle should therefore describe the reusable contract without forcing every consumer to materialize every possible operation or GUI element.

A useful runtime path is:

~~~
Capability
   |
   v
selected operation
   |
   v
connected ports
   |
   v
FSM execution
   |
   v
result
~~~

The system pays for the capability actually exercised by the Experience.

## Repository as the proving case

The MicroBundle Repository is deliberately small enough to make the architecture obvious.

~~~
Repository capability
        |
        +-- Get
        |
        +-- Put
             |
             +-- Forge-only
~~~

Forge can use the capability to publish an artifact without knowing whether the underlying repository is Azure Blob, REST, another service, local storage, or a future repository implementation.

## Efficiency principle

The Workshop's design goal is not the biggest address number.

It is:

> the smallest representation and execution cost that preserves the required semantic space.

That principle should be reflected in benchmarks, documentation, and future ontology implementations.
