using System;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace ManageCodeInformation
{
    public static class MyProjectSettingsSchema
    {
        readonly static Guid schemaGuid = new Guid("{E44A8717-2F01-4B28-8EFE-03F02DD29563}");

        public static Schema GetSchema()
        {
            Schema schema = Schema.Lookup(schemaGuid);

            if (schema != null) return schema;

            SchemaBuilder schemaBuilder = new SchemaBuilder(schemaGuid);
            schemaBuilder.SetSchemaName("MyProjectSettings");
            schemaBuilder.AddSimpleField("Parameter1", typeof(int));
            schemaBuilder.AddSimpleField("Parameter2", typeof(string));
            return schemaBuilder.Finish();
        }
    }
}
