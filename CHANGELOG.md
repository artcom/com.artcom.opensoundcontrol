# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.2] - 2020-08-13

This update brings cleanups and slight refactoring to existing code. Examples have been added.

### Changes

- Renaming ``OscAdapter`` to ``UnityOscAdapter``, clearing up their intent and usage
- Cleaning up on multiple occasions since introducing the interfaces
- More hardening against serialization-reloads (recompile, reimport)
- OSC Monitor scrolls now properly, message history is longer, too.
- Adding an example, showing more usages, will include optionally an OpenStageControl config

## [1.0.1] - 2020-08-10

### Changes

- Under the hood changes to ensure proper type safety.
- Interfaces for main Components are introduced.
- ``Scripts`` Namespace moved to ``Components``
- Adding icons for components

## [1.0.0] - 2020-08-06

### initial public release 

Open Sound Control has matured enough to be released publicly.

