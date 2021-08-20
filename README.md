# Discord Net Labs Interactions

Currently works with Labs only! Currently only slash commands are supported!
Support for message components will be added at some point.

## Parts of the library

There are 3 parts (assemblies) of the library, typical usage in a project will be to use
all 3.

There is `Discord.Net.Interactions.Abstractions` with interfaces and some extension
methods. This should typically be used in a library that will be implementing different
behavior than the one present in `Discord.Net.Interactions`

Second one is `Discord.Net.Interactions` with default implementations of the
abstractions and interactions service that can be used to manage and handle commands.

For better usage with dependency injection, `Discord.Net.Interactions.DI` may
be used. It provides extension methods for `IServiceCollection`

## Extensibility

Main goal of this library is extensibility and replaceability. Each part of the
library can be implemented differently to match your project needs. This is
achieved by making splitting the library to multiple parts and layers.

## How to use

Example can be found in `Discord.Net.Interactions.Example`.

The use basically lays in creating `CommandHolder` for storing the commands,
`CommandRegistrator` for registering the commands (one by one, bulk etc.),
`CommandExecutor` for executing the commands on different thread or auto deferring
them. And lastly implementing `ICommandGroup` to setup your commands.
`InteractionsService` is used to connect these together along
with handling the interactions.

There are default implementations of all of the classes except `ICommandGroup`
which must be implemented for specific commands. By using dependency injection
extensions from `Discord.Net.Interactions.DI` only a few calls of extension
methods are needed to get the library running then.
