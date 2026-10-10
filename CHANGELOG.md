# Changelog

## [0.2.0](https://github.com/st0o0/Signal.Bot/compare/v0.1.1...v0.2.0) (2026-10-10)


### ⚠ BREAKING CHANGES

* add new Signal message types for stories, polls, contacts, edits, and payments
* rename TargetAuthorUuid to TargetAuthorId and use GroupInfoType enum

### Features

* add global.json with SDK roll-forward and MTP runner ([831fa64](https://github.com/st0o0/Signal.Bot/commit/831fa6419f34314fccfeb25e21bfbf4165c222f2))
* add integration tests for account, contact, device, group, health, and poll extensions ([9569e22](https://github.com/st0o0/Signal.Bot/commit/9569e222e5e158ff716b46f6b6c541966a6d1a42))
* add missing API endpoints for health, devices, trust mode, pins and avatars ([2caf467](https://github.com/st0o0/Signal.Bot/commit/2caf467876d28df1c49422adecb35f8407dc780b))
* add missing fields to Device, Group and Acknowledged types ([6bfd94b](https://github.com/st0o0/Signal.Bot/commit/6bfd94b81df7d39ccd1e095313d93f27a958e8d2))
* add new Signal message types for stories, polls, contacts, edits, and payments ([534494c](https://github.com/st0o0/Signal.Bot/commit/534494c52a5128f3e207f7ed815a73579ef4020e))
* add serialization tests for new message types ([04b8742](https://github.com/st0o0/Signal.Bot/commit/04b8742aea92b28d2c84c9514500e18cef56b3c4))
* Configure custom domain for documentation ([401c4ea](https://github.com/st0o0/Signal.Bot/commit/401c4eae832c07904d6f8897374de0727fc90466))
* decouple release-please from build workflow ([4563b3b](https://github.com/st0o0/Signal.Bot/commit/4563b3b0b8c986bdf39e62cc548831d2c171bd8e))
* extend docker preset for base image digest pinning + automerge ([e9415cc](https://github.com/st0o0/Signal.Bot/commit/e9415cc24784b5e985e0c7384e26628eee8ff6a3))
* Refactor project setup and dependencies ([e6a1620](https://github.com/st0o0/Signal.Bot/commit/e6a162088babf68816299f74531f037254196b7c))
* Remove outdated test timeouts ([00585a5](https://github.com/st0o0/Signal.Bot/commit/00585a51a62054c58beed68dc4a0e9aa54f00a49))


### Bug Fixes

* correct HTTP method in RemoteDelete test and fix disposal order in SignalBotReceiver ([cee0869](https://github.com/st0o0/Signal.Bot/commit/cee08695319f20a12a00a489a1692944171967fe))
* correct HTTP methods for RemoteDelete, UpdateProfile and Unregister ([6cd52ab](https://github.com/st0o0/Signal.Bot/commit/6cd52ab4dfb72dde84516fa15ea075cb72c6d097))
* deploy docs only on release, not on every push to main ([47fb912](https://github.com/st0o0/Signal.Bot/commit/47fb912026be871583e7f560dac82afd4d262cd4))
* grant pull-requests/issues write permission to labeler and label-sync callers ([101bfc9](https://github.com/st0o0/Signal.Bot/commit/101bfc9997c5569868a05119393bee4cec2c703b))
* remove pull_request trigger from CodeQL ([ad7be2f](https://github.com/st0o0/Signal.Bot/commit/ad7be2f7d23ab7580776d1cf9e1b03394350ac68))
* resolve whitespace formatting violations ([c5a09d4](https://github.com/st0o0/Signal.Bot/commit/c5a09d4c0d93a9c2109dbe7fecba79def1c19c9e))
* update release-please manifest to current version 0.1.0 ([dbae2b2](https://github.com/st0o0/Signal.Bot/commit/dbae2b205c122bc2aa42e439014bbdb5c70cd286))
* use pnpm renovate preset instead of npm ([63b7e46](https://github.com/st0o0/Signal.Bot/commit/63b7e4669f23f7bfe905176d7acefeed9ed1ee2e))
* use softprops/action-gh-release for nupkg upload ([1a20c96](https://github.com/st0o0/Signal.Bot/commit/1a20c96c99af1fa0b2c33a9da053168e8ba0fd60))


### Documentation

* Add AGENTS.md and CLAUDE.md ([97cf0e3](https://github.com/st0o0/Signal.Bot/commit/97cf0e3345e40913a1199afbf4336595b9d1628b))
* align README badges ([afb5957](https://github.com/st0o0/Signal.Bot/commit/afb5957ab70d5cd116f79df770c4740758cb9499))
* extend test conventions with xUnit best practices ([c74d9f3](https://github.com/st0o0/Signal.Bot/commit/c74d9f3556b26b36d821457af0949b54796434cd))


### Refactoring

* Improve test assertion clarity ([98074ee](https://github.com/st0o0/Signal.Bot/commit/98074eea8605bfa1eefefa603765e2cf8bacfc40))
* improve test assertions with Assert.Multiple and cleanup ([e0e3ba7](https://github.com/st0o0/Signal.Bot/commit/e0e3ba710e20d3fd12fe8bdd335def3e07a62070))
* improve XML documentation across type models and requests ([1d2d4e6](https://github.com/st0o0/Signal.Bot/commit/1d2d4e6de6117a25a88d5557967ceb93b1d0bb90))
* migrate to shared workflows, renovate, and release-please ([75bcd79](https://github.com/st0o0/Signal.Bot/commit/75bcd7994d220185cecce832d52d6292889508cf))
* rename CI jobs for cleaner GitHub check names ([5fbe34f](https://github.com/st0o0/Signal.Bot/commit/5fbe34fb7e946197887449b98cf932dc043c12e0))
* rename TargetAuthorUuid to TargetAuthorId and use GroupInfoType enum ([75e77b7](https://github.com/st0o0/Signal.Bot/commit/75e77b709e65f9aa35190f9309ee6c49da7fdd6e))
* replace CodeQL with Trivy filesystem scan ([adb0ed6](https://github.com/st0o0/Signal.Bot/commit/adb0ed658982d295c40a94a17d6f5a50dfa2ac35))
* standardize test timeouts and replace DateTime.Now with TimeProvider ([6cc7a80](https://github.com/st0o0/Signal.Bot/commit/6cc7a802064066b10f44d98847f83be68ce70cee))
* Use collection expressions ([bc721e2](https://github.com/st0o0/Signal.Bot/commit/bc721e2e288db0da69fc6bcf079236e4e88f45dc))
