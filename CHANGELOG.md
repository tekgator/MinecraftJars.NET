# Changelog
All notable changes to this project will be documented in this file.

## [Unreleased]


## [1.4.6] - 2023-06-07

### Changed
- Do not throw for returning single record methods in MinecraftJar, instead return NULL


## [1.4.5] - 2023-06-04

### Fixed
- Mojang Bedrock versions are now sorted newest to oldest

### Added
- Add property OS to VersionOptions in case a project is for a certain OS only (currently Mojang Bedrock only)


## [1.4.4] - 2023-06-03

### Added
- Implement GetProject on MinecraftJar to get a project by it's name

### Changed
- Renamed Group to ProjectGroup to prevent LINQ group keyword issues 
- Renamed Runtime to ProjectRuntime to be consistent


## [1.4.3] - 2023-06-03

### Added
- IMinecraftProject indicates what type of runtime the project requires (e.g. Java, Php or no special runtime)


## [1.4.2] - 2023-06-03

### Removed
- Removed ProviderOptions as it had no use and created an unnecessary dependency

### Added
- Added MinecraftJarOptions so the optional IHttpClientFactory can be provided

### Fixed
- Removed unnecessary dependency of MinecraftJars.Core causing Nuget installation to fail


## [1.4.1] - 2023-06-03

### Changed
- Renamed MinecraftJarManager to MinecraftJar

### Added
- IMinecraftJar interface
- Extension for dependency injection
- Test case for dependency injection
- Automatic deployment of dependency injection injection to nuget.org


## [1.3.2] - 2023-06-01

### Added
- Fabric version snapshot test missing

### Fixed
- Fabric returned only snapshots with the new IsSnapshot flag


## [1.3.1] - 2023-06-01

### Fixed
- All plugin share the same (static) HttpClient now if HttpClientFactory is not provided, preventing Socket exhaustion


## [1.3.0] - 2023-06-01
### Added
- Paper plugin flag pre and snapshot builds as IsSnapshot
- Pocketmine flag alpha builds as IsSnapshot
- Spigot plugin flag pre and snapshot builds as IsSnapshot

### Changed
- Renamed IProvider to IMinecraftProvider
- Renamed IProject to IMinecraftProject
- Renamed IVersion to IMinecraftVersion
- Renamed IDownload to IMinecraftDownload
- Rename ProviderManager to MinecraftProviderManager
- Change LINQ method calls to query format


## [1.2.3] - 2023-05-31
### Changed
- GetVersion on IProvider now returns all versions for all projects

### Added
- GetVersion on IProject returns all versions for that project


## [1.2.2] - 2023-05-30
### Added
- Add version option to include snapshots during load, by default this is false
- Add indicator to show that the current version is a snapshot / preview / experimental / beta / alpha build
- Add IsSnapshot test cases

### Changed
- Restructure and simplify API directories

### Removed
- Separation of Snapshot ans Standard versions for Mojang, Fabric and Pocketmine plugin, this is handled now via the IsSnapshot indicator on IVersion


## [1.2.1] - 2023-05-29
### Fixed
- Nuget release process


## [1.2.0] - 2023-05-29
### Added
- Add logos in different sizes for fixing Readme on Nuget.org
- Add Fabric Provider Plugin

### Fixed
- Fix logos and links in Readme

### Changed
- Renamed Mohist project from MohistMC to Mohist


## [1.1.0] - 2023-05-29
### Added
- Add Pocketmine Provider Plugin
- Add logos to each Readme


## [1.0.0] - 2023-05-28
### Fixed
- In case of Task cancellation for a Spigot build make sure the whole process tree is killed
- Website (Mojang, Spigot) did not respond because AcceptEncoding has to be provided. Always add AcceptEncoding identity

### Changed
- Normalize names of Plugins e.g. SpigotMC is now just Spigot
- Created temp folders during Spigot build include the Plugin name for easier identification 

### Added
- First working version
- Unit testing project added
- Add comment for old versions not proving Url due to missing server in Mojang Plugin Readme
- Implement version filter in Spigot version factory
- For Website crawling (Mojang, Spigot) add AcceptLanguage en-US, en to make sure content is not changed due to change of language
- For Website crawling (Mojang, Spigot) add CacheControl with NoCache to get latest version of the website


## [0.1.0] - 2023-05-28
### Added
- First working version


This project is MIT Licensed // Created & maintained by Patrick Weiss
