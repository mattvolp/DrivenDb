#DrivenDb

###Version 2.0 - Release Notes

In order to continue developing DrivenDb, it seemed necessary to go through the project thoroughly and determine what was valuable.  In the end, some ideas never took hold.  As well, my coding practices and standards have changed.  The core of the library will remain intact.  Some peripheral concepts and classes will be dropped.  Namespaces have been reorganized.  The library in general has been re-branded along with the move to GitHub.  The goal was for the project to be more focused and current. Here is a list of changes:


### Gained

* A Windows Phone 8.1 target has been added to the Core project.

### Improve

* TODO: update console, keep current one or make command line
  * TODO: sqlite need to just do longs
* TODO: make studio custom tool for schema generation?
* TODO: solve inheritance problem?  is it a problem?

### Fixed

* rename protected DbEntity member variables m_LastModified? or make sure they're excluded?  
	double __ prefix?

### Depricated

* lose aggregate stuff?
* collections good or no? no.
* MissingFieldException gone?
* Link2Sql Contracts not supported?
* DbAggregate concept
* void Update(T other, bool checkIdentity = true); remove "check"
* read identity bad idea? review whole accessor interface

