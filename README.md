**FileMakerConnect**

FileMakerConnect is an abstraction layer for dealing with FileMaker databases via ODBC, written in C#. A select query is this simple:

    FileMakerConnect connect = new FileMakerConnect(connectionString);
    FileMakerSelect cmd = new FileMakerSelect(connect);

    cmd.Select("Foo", "Bar", "Do", "Boo");
    cmd.From("MyTable");
    cmd.Where("Boo", Condition.Equals, "hoo");
    cmd.Where("Bar", Condition.GreaterThanOrEqualTo, 37);
    cmd.OrderBy("Foo", SortOrder.Descending);
    cmd.OrderBy("Bar", SortOrder.Ascending);

    Foo foo = cmd.Execute().ExtractObject<Foo>();

Or if you prefer DataTables:

    FileMakerResult result = cmd.Execute();
    DataTable table = result.ResultSet;

And an update as simple as this:

    FileMakerUpdate upd = new FileMakerUpdate(connect);
    upd.Set("Foo", "bar");
    upd.Set("Do", DateTime.Now);
    upd.Where("FooID", Condition.Equals, 1);
    upd.Execute();
