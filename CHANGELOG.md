# Changelog

## [0.1.0](https://github.com/st0o0/Signal.Bot/compare/v0.1.1...v0.1.0) (2026-09-13)


### Features

* add global.json with SDK roll-forward and MTP runner ([831fa64](https://github.com/st0o0/Signal.Bot/commit/831fa6419f34314fccfeb25e21bfbf4165c222f2))
* add missing API endpoints for health, devices, trust mode, pins and avatars ([2caf467](https://github.com/st0o0/Signal.Bot/commit/2caf467876d28df1c49422adecb35f8407dc780b))
* add missing fields to Device, Group and Acknowledged types ([6bfd94b](https://github.com/st0o0/Signal.Bot/commit/6bfd94b81df7d39ccd1e095313d93f27a958e8d2))
* decouple release-please from build workflow ([4563b3b](https://github.com/st0o0/Signal.Bot/commit/4563b3b0b8c986bdf39e62cc548831d2c171bd8e))


### Bug Fixes

* correct HTTP method in RemoteDelete test and fix disposal order in SignalBotReceiver ([cee0869](https://github.com/st0o0/Signal.Bot/commit/cee08695319f20a12a00a489a1692944171967fe))
* correct HTTP methods for RemoteDelete, UpdateProfile and Unregister ([6cd52ab](https://github.com/st0o0/Signal.Bot/commit/6cd52ab4dfb72dde84516fa15ea075cb72c6d097))
* deploy docs only on release, not on every push to main ([47fb912](https://github.com/st0o0/Signal.Bot/commit/47fb912026be871583e7f560dac82afd4d262cd4))
* remove pull_request trigger from CodeQL ([ad7be2f](https://github.com/st0o0/Signal.Bot/commit/ad7be2f7d23ab7580776d1cf9e1b03394350ac68))
* resolve whitespace formatting violations ([c5a09d4](https://github.com/st0o0/Signal.Bot/commit/c5a09d4c0d93a9c2109dbe7fecba79def1c19c9e))
* update release-please manifest to current version 0.1.0 ([dbae2b2](https://github.com/st0o0/Signal.Bot/commit/dbae2b205c122bc2aa42e439014bbdb5c70cd286))


### Refactoring

* migrate to shared workflows, renovate, and release-please ([75bcd79](https://github.com/st0o0/Signal.Bot/commit/75bcd7994d220185cecce832d52d6292889508cf))
* rename CI jobs for cleaner GitHub check names ([5fbe34f](https://github.com/st0o0/Signal.Bot/commit/5fbe34fb7e946197887449b98cf932dc043c12e0))
* replace CodeQL with Trivy filesystem scan ([adb0ed6](https://github.com/st0o0/Signal.Bot/commit/adb0ed658982d295c40a94a17d6f5a50dfa2ac35))
