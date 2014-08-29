#DrivenDb

###Version 2.0 - Unreleased Notes

In order to continue developing DrivenDb, it seemed necessary to go through the project thoroughly and determine what was valuable.  In the end, some ideas never took hold.  As well, my coding practices and standards have changed.  The core of the library will remain intact.  Some peripheral concepts and classes will be dropped.  Namespaces have been reorganized.  The library in general has been re-branded along with the move to GitHub.  The goal was for the project to be more focused and current. Here is a list of changes that are *possibly* coming:


### Gained

* A Windows Phone 8.1 target has been added to the Core project.

### Changed

* The default extension mode is now All instead or None.
* AccessorExtensions is now AccessorOptions
* Parallel mapping is the default behavior now

### Improve

* TODO: update console, keep current one or make command line
  * TODO: sqlite need to just do longs
* TODO: make studio custom tool for schema generation?
* TODO: solve inheritance problem?  is it a problem?
* TODO: review ToUpdate, ToNew, ToMapped, MapTo etc... methods 
* Fallback accessor just need to be options

### Fixed

* Inherited member variables from DbRecord and DbEntity have been renamed as to avoid conflicts with member variables that inherit from them.

### Removed

* Aggregate loading and saving is gone.  A fresh approach is required?
* The collection classes were not that useful, and were ok at best.
* MissingFieldException is gone, due to the PCL targeting ?
* Link2Sql contract support is gone, due to it's age, lack of MS support, and the new PCL target.
* The identity check on the "void Update(T other, bool checkIdentity = true)" method was too much responsibility for this method.
* The read identity methods have been deprecated for some time due to their volatile nature.  They are now gone.
* Removed DbRecord.TableOverride property.  The ReadType methods now satisfy the original motive for this.
* The "ReadRelated" methods on the DbAccessor have been removed.
* Removed IEquitable & IComparable & SameAs implementations on DbRecord
* Removed DbRecord.PrimaryKey property
