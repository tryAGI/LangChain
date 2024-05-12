namespace LangChain.Extensions;

public enum AddDocumentsToDatabaseBehavior
{
    /// <summary>
    /// If the collection already exists, it will be returned without any changes.
    /// </summary>
    JustReturnCollectionIfCollectionIsAlreadyExists,
    
    /// <summary>
    /// If the collection already exists, it will be deleted and recreated.
    /// </summary>
    OverwriteExistingCollection,
    
    /// <summary>
    /// If the collection already exists, all documents will be added to the existing collection.
    /// </summary>
    AlwaysAddDocuments,
}