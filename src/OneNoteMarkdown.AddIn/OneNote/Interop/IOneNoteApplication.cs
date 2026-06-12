using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.OneNote;

namespace OneNoteMarkdown.OneNote.Interop
{
    [ComImport]
    [Guid("452AC71A-B655-4967-A208-A4CC39DD7949")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IOneNoteApplication
    {
        void GetHierarchy([MarshalAs(UnmanagedType.BStr)] string bstrStartNodeID, HierarchyScope hsScope, [MarshalAs(UnmanagedType.BStr)] out string pbstrHierarchyXmlOut, XMLSchema xsSchema);
        void UpdateHierarchy([MarshalAs(UnmanagedType.BStr)] string bstrChangesXmlIn, XMLSchema xsSchema);
        void OpenHierarchy([MarshalAs(UnmanagedType.BStr)] string bstrPath, [MarshalAs(UnmanagedType.BStr)] string bstrRelativeToObjectID, [MarshalAs(UnmanagedType.BStr)] out string pbstrObjectID, CreateFileType cftIfNotExist);
        void DeleteHierarchy([MarshalAs(UnmanagedType.BStr)] string bstrObjectID, DateTime dateExpectedLastModified, bool deletePermanently);
        void CreateNewPage([MarshalAs(UnmanagedType.BStr)] string bstrSectionID, [MarshalAs(UnmanagedType.BStr)] out string pbstrPageID, NewPageStyle npsNewPageStyle);
        void CloseNotebook([MarshalAs(UnmanagedType.BStr)] string bstrNotebookID, bool force);
        void GetHierarchyParent([MarshalAs(UnmanagedType.BStr)] string bstrObjectID, [MarshalAs(UnmanagedType.BStr)] out string pbstrParentID);
        void GetPageContent([MarshalAs(UnmanagedType.BStr)] string bstrPageID, [MarshalAs(UnmanagedType.BStr)] out string pbstrPageXmlOut, PageInfo pageInfoToExport, XMLSchema xsSchema);
        void UpdatePageContent([MarshalAs(UnmanagedType.BStr)] string bstrPageChangesXmlIn, DateTime dateExpectedLastModified, XMLSchema xsSchema, bool force);
        void GetBinaryPageContent([MarshalAs(UnmanagedType.BStr)] string bstrPageID, [MarshalAs(UnmanagedType.BStr)] string bstrCallbackID, [MarshalAs(UnmanagedType.BStr)] out string pbstrBinaryObjectB64Out);
        void DeletePageContent([MarshalAs(UnmanagedType.BStr)] string bstrPageID, [MarshalAs(UnmanagedType.BStr)] string bstrObjectID, DateTime dateExpectedLastModified, bool force);
        void NavigateTo([MarshalAs(UnmanagedType.BStr)] string bstrHierarchyObjectID, [MarshalAs(UnmanagedType.BStr)] string bstrObjectID, bool fNewWindow);
        void NavigateToUrl([MarshalAs(UnmanagedType.BStr)] string bstrUrl, bool fNewWindow);
        void Publish([MarshalAs(UnmanagedType.BStr)] string bstrHierarchyID, [MarshalAs(UnmanagedType.BStr)] string bstrTargetFilePath, PublishFormat pfPublishFormat, [MarshalAs(UnmanagedType.BStr)] string bstrCLSIDofExporter);
        void OpenPackage([MarshalAs(UnmanagedType.BStr)] string bstrPathPackage, [MarshalAs(UnmanagedType.BStr)] string bstrPathDest, [MarshalAs(UnmanagedType.BStr)] out string pbstrPathOut);
        void GetHyperlinkToObject([MarshalAs(UnmanagedType.BStr)] string bstrHierarchyID, [MarshalAs(UnmanagedType.BStr)] string bstrPageContentObjectID, [MarshalAs(UnmanagedType.BStr)] out string pbstrHyperlinkOut);
        void FindPages([MarshalAs(UnmanagedType.BStr)] string bstrStartNodeID, [MarshalAs(UnmanagedType.BStr)] string bstrSearchString, [MarshalAs(UnmanagedType.BStr)] out string pbstrHierarchyXmlOut, bool fIncludeUnindexedPages, bool fDisplay, XMLSchema xsSchema);
        void FindMeta([MarshalAs(UnmanagedType.BStr)] string bstrStartNodeID, [MarshalAs(UnmanagedType.BStr)] string bstrSearchStringName, [MarshalAs(UnmanagedType.BStr)] out string pbstrHierarchyXmlOut, bool fIncludeUnindexedPages, XMLSchema xsSchema);
        void GetSpecialLocation(SpecialLocation slToGet, [MarshalAs(UnmanagedType.BStr)] out string pbstrSpecialLocationPath);
        void MergeFiles([MarshalAs(UnmanagedType.BStr)] string bstrBaseFile, [MarshalAs(UnmanagedType.BStr)] string bstrClientFile, [MarshalAs(UnmanagedType.BStr)] string bstrServerFile, [MarshalAs(UnmanagedType.BStr)] string bstrTargetFile);
        [return: MarshalAs(UnmanagedType.Interface)] IQuickFilingDialog QuickFiling();
        void SyncHierarchy([MarshalAs(UnmanagedType.BStr)] string bstrHierarchyID);
        void SetFilingLocation(FilingLocation flToSet, FilingLocationType fltToSet, [MarshalAs(UnmanagedType.BStr)] string bstrFilingSectionID);
        [return: MarshalAs(UnmanagedType.Interface)] Windows GetWindows();
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetDummy1();
        void MergeSections([MarshalAs(UnmanagedType.BStr)] string bstrSectionSourceId, [MarshalAs(UnmanagedType.BStr)] string bstrSectionDestinationId);
        [return: MarshalAs(UnmanagedType.IDispatch)] object GetCOMAddIns();
        [return: MarshalAs(UnmanagedType.IDispatch)] object GetLanguageSettings();
        void GetWebHyperlinkToObject([MarshalAs(UnmanagedType.BStr)] string bstrHierarchyID, [MarshalAs(UnmanagedType.BStr)] string bstrPageContentObjectID, [MarshalAs(UnmanagedType.BStr)] out string pbstrHyperlinkOut);
    }
}
