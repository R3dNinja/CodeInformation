using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace ManageCodeInformation
{
    class MyProjectSettingStorage
    {
        readonly Guid settingDsId = new Guid("{BE5D4775-FFB3-4243-A69E-00B0F8D743DA}");

        public MyProjectSettings ReadSettings(Document doc)
        {
            var settingsEntity = GetSettingsEntity(doc);
            if (settingsEntity == null || !settingsEntity.IsValid())
            {
                return null;
            }

            MyProjectSettings settings = new MyProjectSettings();
            settings.Parameter1 = settingsEntity.Get<int>("Parameter1");
            settings.Parameter2 = settingsEntity.Get<string>("Parameter2");

            return settings;
        }

        public void WriteSettings(Document doc, MyProjectSettings settings)
        {
            var settingDs = GetSettingsDataStorage(doc);
            if (settingDs == null)
            {
                settingDs = DataStorage.Create(doc);
            }

            Entity settingsEntity = new Entity(MyProjectSettingsSchema.GetSchema());
            settingsEntity.Set("Parameter1", settings.Parameter1); 
            settingsEntity.Set("Parameter2", settings.Parameter2);

            Entity idEntity = new Entity(DataStorageUniqueIdSchema.GetSchema());
            idEntity.Set("Id", settingDsId);
            settingDs.SetEntity(idEntity);
            settingDs.SetEntity(settingsEntity);
        }

        private DataStorage GetSettingsDataStorage(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var dataStorages = collector.OfClass(typeof(DataStorage));
            foreach (DataStorage dataStorage in dataStorages)
            {
                Entity settingIdEntity = dataStorage.GetEntity(DataStorageUniqueIdSchema.GetSchema());

                if (!settingIdEntity.IsValid()) continue;
                var id = settingIdEntity.Get<Guid>("Id");
                if (!id.Equals(settingDsId)) continue;
                return dataStorage;
            }
            return null;
        }

        private Entity GetSettingsEntity(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var dataStorages = collector.OfClass(typeof(DataStorage));

            foreach (DataStorage dataStorage in dataStorages)
            {
                Entity settingEntity = dataStorage.GetEntity(MyProjectSettingsSchema.GetSchema());
                if (!settingEntity.IsValid()) continue;
                return settingEntity;
            }
            return null;
        }
    }
}
