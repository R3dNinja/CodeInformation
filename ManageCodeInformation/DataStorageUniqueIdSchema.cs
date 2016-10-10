using System;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace ManageCodeInformation
{
    static class DataStorageUniqueIdSchema
    {
        static readonly Guid schemaGuid = new Guid("{44E320C7-73E5-4DCB-8802-21AF8E9B6FAE}");

        public static Schema GetSchema()
        {
            Schema schema = Schema.Lookup(schemaGuid);
            if (schema != null) return schema;

            SchemaBuilder schemaBuilder = new SchemaBuilder(schemaGuid);
            schemaBuilder.SetSchemaName("DataStorageUniqueId");
            schemaBuilder.AddSimpleField("Id", typeof(Guid));

            return schemaBuilder.Finish();
        }
    }
}
