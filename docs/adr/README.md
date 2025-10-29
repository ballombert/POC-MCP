# Architecture Decision Records (ADR)

This directory contains Architecture Decision Records for the POC-MCP project. ADRs document the important architectural decisions made during the development of this project, including the context, options considered, and rationale behind each decision.

## ADR Format

Each ADR follows this structure:
- **Title**: Short noun phrase describing the decision
- **Status**: Proposed, Accepted, Deprecated, or Superseded
- **Context**: The situation that requires a decision
- **Decision**: The change we're proposing or have agreed to implement
- **Consequences**: The positive and negative outcomes of this decision

## Current ADRs

- [ADR-001: Use .NET 9.0 for MCP Server Implementation](001-dotnet-9-mcp-server.md)
- [ADR-002: Choose stdio Transport for MCP Communication](002-stdio-transport.md)
- [ADR-003: Use Attribute-Based Tool Discovery](003-attribute-based-tools.md)
- [ADR-004: Self-Contained Single-File Distribution](004-self-contained-distribution.md)

## Creating New ADRs

When making significant architectural decisions:

1. **Create a new ADR file** following the naming convention: `XXX-short-title.md`
2. **Use the next sequential number** (e.g., if last ADR is 004, create 005)
3. **Follow the standard template** (see existing ADRs for examples)
4. **Update this README** to include the new ADR in the list above

## ADR Lifecycle

- **Proposed**: Decision is under consideration
- **Accepted**: Decision has been agreed upon and implemented
- **Deprecated**: Decision is no longer recommended but not yet replaced
- **Superseded**: Decision has been replaced by a newer ADR (link to the replacement)

---

For more information about ADRs, see:
- [ADR GitHub Organization](https://adr.github.io/)
- [Documenting Architecture Decisions](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)