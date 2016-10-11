#region Namespaces
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;

using Autodesk;
using Autodesk.Revit;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
#endregion

namespace ManageCodeInformation
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        internal static Command thisCommand = null;
        public CodeInfoForm dialog;
        public string filePath = null;
        public Image[] images;
        public System.Drawing.Point dimensions;
        public int pages;
        string sheight;
        double height;
        string swidth;
        double width;
        public string sheetSize;
        public double centerLine;
        public int imagePerSheet;
        public ElementId titleBlockTypeId;
        public string templateCategory;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            ReadWriteSettingsCommand readWrite = new ReadWriteSettingsCommand();

            FindTemplateType(uiapp);
            string path;
            string searchString = "A0.20";
            thisCommand = this;
            RetrieveTitleBlockHeight(uiapp, searchString);
            path = ShowForm(uiapp);

            if (path != "")
            {
                string imagePath = Path.GetDirectoryName(path);
                readWrite.WriteFilePath(doc, path);
            }
            uidoc.RefreshActiveView();
            return Result.Succeeded;
        }

        private void FindTemplateType(UIApplication uiapp)
        {
            UIApplication app = uiapp;
            Document doc = app.ActiveUIDocument.Document;
            Parameter p;

            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfCategory(BuiltInCategory.OST_Sheets);
            col.OfClass(typeof(ViewSheet));

            foreach (Element v in col)
            {
                ViewSheet vs = v as ViewSheet;
                var sheetName = vs.Name;
                if (sheetName == "STARTING VIEW")
                {
                    p = vs.LookupParameter("TemplateCategory");
                    setTemplateCategory(p.AsString());
                }
            }
        }

        private void RetrieveTitleBlockHeight(UIApplication uiapp, string searchString)
        {
            UIApplication app = uiapp;
            Document doc = app.ActiveUIDocument.Document;
            FilteredElementCollector a;
            Parameter p;
            a = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks).OfClass(typeof(FamilyInstance));
            foreach (FamilyInstance e in a)
            {
                p = e.get_Parameter(BuiltInParameter.SHEET_NUMBER);
                string sheet_number = p.AsString();
                if (sheet_number.Contains(searchString))
                {
                    p = e.get_Parameter(BuiltInParameter.SHEET_WIDTH);
                    string swidth = p.AsValueString();
                    double width = p.AsDouble();

                    p = e.get_Parameter(BuiltInParameter.SHEET_HEIGHT);
                    string sheight = p.AsValueString();
                    double height = p.AsDouble();

                    ElementId typeId = e.GetTypeId();
                    Element type = doc.GetElement(typeId);

                    setTitleBlockID(typeId);
                    setSheetParameters(sheight, height, swidth, width);
                    break;
                }
            }
        }

        public string ShowForm(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            ReadWriteSettingsCommand readWrite = new ReadWriteSettingsCommand();

            string filePath = readWrite.ReadFilePath(doc);

            if (dialog == null || dialog.IsDisposed)
            {
                dialog = new CodeInfoForm(doc, filePath);
                var result = dialog.ShowDialog();


                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string returnVal = dialog.returnPath;
                    return returnVal;
                }
                else
                {
                    string returnVal = dialog.returnPath;
                    return returnVal;
                }
            }
            else
            {
                return filePath;
            }
            }

        public void SetFilePath(string path)
        {
            filePath = path;
        }

        public void holdImages(Image[] imagesFromWord)
        {
            images = imagesFromWord;
        }

        public Image[] getImages()
        {
            return images;
        }

        public void setDimension(System.Drawing.Point dimension)
        {
            dimensions = dimension;
        }

        public System.Drawing.Point getDimension()
        {
            return dimensions;
        }

        public void setPageCount(int pageCount)
        {
            pages = pageCount;
        }

        public int getPageCount()
        {
            return pages;
        }

        string GetParameterValueString(Element e, BuiltInParameter bip)
        {
            Parameter p = e.get_Parameter(bip);

            string s = string.Empty;

            if (null != p)
            {
                switch (p.StorageType)
                {
                    case StorageType.Integer:
                        s = p.AsInteger().ToString();
                        break;

                    case StorageType.ElementId:
                        s = p.AsElementId().IntegerValue.ToString();
                        break;

                    case StorageType.Double:
                        s = Util.RealString(p.AsDouble());
                        break;

                    case StorageType.String:
                        s = string.Format("{0} ({1})",
                          p.AsValueString(),
                          Util.RealString(p.AsDouble()));
                        break;

                    default: s = "";
                        break;
                }
                s = ", " + bip.ToString() + "=" + s;
            }
            return s;
        }

        public class Util
        {
            static public string RealString(double a)
            {
                return a.ToString("0.##");
            }
        }

        private void setSheetParameters(string sheightV, double heightV, string swidthV, double widthV)
        {
            sheight = sheightV;
            height = Math.Round(heightV, 1);
            swidth = swidthV;
            width = Math.Round(widthV, 1);
        }

        public double getSheetWidth()
        {
            return width;
        }

        public void setSheetSize(string sheetS)
        {
            sheetSize = sheetS;
        }

        public string getSheetSize()
        {
            return sheetSize;
        }

        public void setTitleBlockID(ElementId typeId)
        {
            titleBlockTypeId = typeId;
        }

        public ElementId getTitleBlockID()
        {
            return titleBlockTypeId;
        }

        public void setTemplateCategory(string TemplateCategory)
        {
            templateCategory = TemplateCategory;
        }

        public string getTemplateCategory()
        {
            return templateCategory;
        }
        }

}
