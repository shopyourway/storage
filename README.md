# Storage
Storage is a CRUD data access abstraction for .Net applications, allowing usage of different data sources with the same interface.

## Highlights
* Single interface - easy to use
* Support MySql and [Moranbernate](https://github.com/shopyourway/moranbernate)
* .Net Core support - targeting .Net Standard 1.6

# WIP
Storage is still a work in progress.

Open issues as of now:
* Bootstrap & Configuration
* Support for MongoDB
* Interface issues to comply with MongoDB driver
* UpdateByQuery support
* Add bulk iterators support

## Development

### How to contribute
We encorage contribution via pull requests on any feature you see fit.

When submitting a pull request make sure to do the following:
* Check that new and updated code follows OhioBox existing code formatting and naming standard
* Run all unit and integration tests to ensure no existing functionality has been affected
* Write unit or integration tests to test your changes. All features and fixed bugs must have tests to verify they work
Read [GitHub Help](https://help.github.com/articles/about-pull-requests/) for more details about creating pull requests