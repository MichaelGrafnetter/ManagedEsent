# Multi-Value Column Tutorial

Normally a column in a record has either zero (i.e. the column is null) or one value. For example, the value of column 'title' is "Match of the Day", the value of column 'Posted' is 5/29/2010 or the value of the column 'Moderated' is null. Sometimes you might want a column to have multiple values, an example of that is a 'tag' column for any sort of data which allows tagging (blog posts, e-mail etc.). For examples the values of the column 'tags' could be "news", "sports" and "local".

One way to provide this functionality is to put the tags in a separate table and using a join to retrieve all the data. This solution isn't very efficient because the record data is now spread across two tables and at least two different database pages. We could serialize the tags into one column but doing that doesn't allow the individual tags to be indexed.

ESENT multi-value columns are designed to deal with this type of data in a natural way. A column can have multiple values, with each value identified by an itag sequence (+the first itag sequence is one, not zero+). Multi-valued columns can be indexed and there is one index entry for each instance of the column. Multi-valued columns should be used in cases where the number of value instances is reasonably small (< 100). Examples of data that can be stored (and indexed) in multi-valued columns might include a post's categories, a person's phone numbers, a book's authors or the ingredients in a cocktail. 

## Creating a multi-value column
Any tagged column can be multi-valued, but by default only the first instance of the column will be indexed. In order to make sure that all instances of the column are indexed use **ColumndefGrbit.ColumnMultivalued**.

_I am using a "tags" column in this example. The "tags" column is completely different from the idea of tagged columns in ESENT. Tagged columns are columns that take no space whatsoever if they are not set in a record, but use slightly more space when set. Columns of type LongText or LongBinary are always tagged and other columns can be made tagged by specifying **ColumndefGrbit.ColumnTagged** when creating the column_:

```c#
columndef.coltyp = JET_coltyp.LongText;
columndef.cp = JET_CP.Unicode;
columndef.grbit = ColumndefGrbit.ColumnMultiValued;
Api.JetAddColumn(sesid, tableid, "tags", columndef, null, 0, out tagColumn);
```

## Indexing a multi-value column
One a multi-valued column has been created it can be indexed like any other column:
```c#
const string IndexKey = "+tags\0\0";
Api.JetCreateIndex(sesid, tableid, "tagsindex", CreateIndexGrbit.None, IndexKey, IndexKey.Length, 100);
```
_If there are multiple multi-valued columns in an index only the first multi-valued column will have all instances indexed, subsequent columns will all be treated as single-valued. Starting with Windows Vista there are two new options for multi-value indexing:_
1. _Creating an index with **VistaGrbits.IndexCrossProduct** causes all instances of all multi-valued columns to be indexed against each other. +Be very careful with this option as a cross-product indexes rapidly become very expensive and very slow+._
2. _Creating an index with **VistaGrbits.IndexNestedTable** causes all instances of a multi-valued column to be indexed against other column values with the same itag sequence._

## Setting a multi-valued column
The JetSetColumn API takes a `JET_SETINFO` object which can be used to specify the itag of the column to set. If no `JET_SETINFO` is passed in then the itag is assumed to be 1:
```c#
// Always sets itag 1
byte[]() data = Encoding.Unicode.GetBytes("foo");
Api.JetSetColumn(sesid, tableid, tagColumn, data, data.Length, SetColumnGrbit.None, null);
// Record has one instance of the 'tags' column: "foo"
```
When passing an itagSequence to JetSetColumn or JetSetColumns using a value of 0 will cause a new instance of the column to be created:
```c#
// Always creates a new instance of the column
JET_SETINFO setinfo = new JET_SETINFO();
data = Encoding.Unicode.GetBytes("bar");
Api.JetSetColumn(sesid, tableid, tagColumn, data, data.Length, SetColumnGrbit.None, setinfo);
// Record has two instances of the 'tags' column: "foo", "bar"
```
A common programming error when using ESENT is using an itagSequence of 0 when updating a column value. By default the `JET_SETINFO` and `JET_SETCOLUMN` structures are intialized with a itagSequence of 0. That works for inserts (which have to create a new instance of a record) but when updating a record the default behaviour will be to create a new instance of a column instead of updating the existing one. **To overwrite an existing column you must set the itagSequence, even if the column is single-valued**.

It is also possible to explicitly set the itagSequence to a value greater than the number of column instances. Doing so will create a new instance:
```c#
data = Encoding.Unicode.GetBytes("baz");
setinfo.itagSequence = 3;
Api.JetSetColumn(sesid, tableid, tagColumn, data, data.Length, SetColumnGrbit.None, setinfo);
// Record has three instances of the 'tags' column: "foo", "bar", "baz"
```

By default a multi-value column can have multiple instances with the same value. To prevent this use **SetColumnGrbit.UniqueMultiValues** or **SetColumnGrbit.UniqueNormalizedMultiValues** when setting the column:
```c#
data = Encoding.Unicode.GetBytes("foo");
setinfo.itagSequence = 0;
// This will throw an EsentErrorException with a JET_err.MultiValuedDuplicate error
Api.JetSetColumn(sesid, tableid, tagColumn, data, data.Length, SetColumnGrbit.UniqueMultiValues, setinfo);
```

## Seeking on a multi-value column
Seeking on an indexed multi-value column works like a normal index. All of the multi-value instances are in the index and we can seek for any of them:
```c#
// Find the (first) record tagged with "bar"
Api.JetSetCurrentIndex(sesid, tableid, "tagsindex");
Api.MakeKey(sesid, tableid, "bar", Encoding.Unicode, MakeKeyGrbit.NewKey);
Api.JetSeek(sesid, tableid, SeekGrbit.SeekEQ);
```
_JetSeek will throw an exception if no record is found. Use Api.TrySeek if you aren't sure the record is there._

This index in this example isn't unique. If there are multiple records with the same tag use an index range to iterate over all of them:
```c#
// Iterate over all records tagged with "foo"
Api.MakeKey(sesid, tableid, "foo", Encoding.Unicode, MakeKeyGrbit.NewKey);
if (Api.TrySeek(sesid, tableid, SeekGrbit.SeekEQ))
{
    Api.MakeKey(sesid, tableid, "foo", Encoding.Unicode, MakeKeyGrbit.NewKey);
    Api.JetSetIndexRange(sesid, tableid, SetIndexRangeGrbit.RangeInclusive | SetIndexRangeGrbit.RangeUpperLimit);
    do
    {
        // Something
    } while (Api.TryMoveNext(sesid, tableid));
}
```

## Counting multi-value instances
To retrieve the number of instances of a multi-valued column use the JetRetrieveColumns API with the itagSequence member of the `JET_RETRIEVECOLUMN` object set to 0. After the call the itagSequence member will contain the number of instances of the column:
```c#
JET_RETRIEVECOLUMN retrievecolumn = new JET_RETRIEVECOLUMN();
retrievecolumn.columnid = tagColumn;
retrievecolumn.itagSequence = 0;
Api.JetRetrieveColumns(sesid, tableid, new[]() { retrievecolumn }, 1); 
Console.WriteLine("Column has {0} instances", retrievecolumn.itagSequence);
```
Using the record from this example the output will be {{'Column has 3 instances'}}.

The optional `JET_RETCOL` parameter to JetRetrieveColumn also has an itagSequence member but you cannot use an itagSequence of 0 (you will get a JET_err.BadItagSequence exception). **Only JetRetrieveColumns supports this functionality**.

## Retrieving multi-values
Retrieving an instance of a multi-value column can be done by specifying an itagSequence in a `JET_RETINFO` or `JET_RETRIEVECOLUMN` object. If no `JET_RETINFO` object is passed to JetRetrieveColumn then itag 1 will be retrieved by default. This code will retrieve all instances of the column in the sample record:
```c#
// IMPORTANT: itags start at 1, not 0
for (int itag = 1; itag <= 3; ++itag)
{
    JET_RETINFO retinfo = new JET_RETINFO { itagSequence = itag };
    string s = Encoding.Unicode.GetString(Api.RetrieveColumn(sesid, tableid, tagColumn, RetrieveColumnGrbit.None, retinfo));
    Console.WriteLine("{0}: {1}", itag, s);
}
```
For the sample record this code will print:
1: foo
2: bar
3: baz

## Updating a multi-value column
Existing instances of a multi-value column can be updated by setting the correct itagSequence in the `JET_SETINFO` or `JET_SETCOLUMN` objects. If no `JET_SETINFO` object is passed to JetSetColumn itag 1 will be updated. **Setting an instance to null will delete it and decrease the itagSequence of any subsequent instances**:
```c#
// Set an instance to null to delete it.
setinfo.itagSequence = 2;
Api.JetSetColumn(sesid, tableid, tagColumn, null, 0, SetColumnGrbit.None, setinfo);

// Removing itag 2 moved the other itags down (i.e. itag 3 became itag 2).
// Overwrite itag 2.
data = Encoding.Unicode.GetBytes("xyzzy");
setinfo.itagSequence = 2;
Api.JetSetColumn(sesid, tableid, tagColumn, data, data.Length, SetColumnGrbit.None, setinfo);

// Now add a new instance by setting itag 0. This instance will go at the end.
data = Encoding.Unicode.GetBytes("flob");
setinfo.itagSequence = 0;
Api.JetSetColumn(sesid, tableid, tagColumn, data, data.Length, SetColumnGrbit.None, setinfo);
```
Dumping the record with the code from the previous section will now print:
1: qux
2: xyzzy
3: flob
